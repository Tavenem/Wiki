window.csharpmode = {
    Context: function (indented, column, type, info, align, prev) {
        this.indented = indented;
        this.column = column;
        this.type = type;
        this.info = info;
        this.align = align;
        this.prev = prev;
    },
    pushContext: function (state, col, type, info) {
        var indent = state.indented;
        if (state.context && state.context.type == "statement" && type != "statement")
            indent = state.context.indented;
        return state.context = new window.csharpmode.Context(indent, col, type, info, null, state.context);
    },
    popContext: function (state) {
        var t = state.context.type;
        if (t == ")" || t == "]" || t == "}")
            state.indented = state.context.indented;
        return state.context = state.context.prev;
    },

    typeBefore: function (stream, state, pos) {
        if (state.prevToken == "variable" || state.prevToken == "type") return true;
        if (/\S(?:[^- ]>|[*\]])\s*$|\*$/.test(stream.string.slice(0, pos))) return true;
        if (state.typeAtEndOfLine && stream.column() == stream.indentation()) return true;
    },

    isTopScope: function (context) {
        for (; ;) {
            if (!context || context.type == "top") return true;
            if (context.type == "}" && context.prev.info != "namespace") return false;
            context = context.prev;
        }
    },

    words: function (str) {
        var obj = {}, words = str.split(" ");
        for (var i = 0; i < words.length; ++i) obj[words[i]] = true;
        return obj;
    },
    contains: function (words, word) {
        if (typeof words === "function") {
            return words(word);
        } else {
            return words.propertyIsEnumerable(word);
        }
    },

    indentUnit: 4,
    statementIndentUnit: 4,
    hooks: {
        "@": function (stream, state) {
            if (stream.eat('"')) {
                state.tokenize = window.csharpmode.tokenAtString;
                return window.csharpmode.tokenAtString(stream, state);
            }
            stream.eatWhile(/[\w\$_]/);
            return "meta";
        }
    },

    curPunc: null,
    isDefKeyword: null,

    tokenAtString: function (stream, state) {
        var next;
        while ((next = stream.next()) != null) {
            if (next == '"' && !stream.eat('"')) {
                state.tokenize = null;
                break;
            }
        }
        return "string";
    },

    tokenString: function (quote) {
        return function (stream, state) {
            var escaped = false, next, end = false;
            while ((next = stream.next()) != null) {
                if (next == quote && !escaped) { end = true; break; }
                escaped = !escaped && next == "\\";
            }
            if (end || !escaped)
                state.tokenize = null;
            return "string";
        };
    },

    tokenComment: function (stream, state) {
        var maybeEnd = false, ch;
        while (ch = stream.next()) {
            if (ch == "/" && maybeEnd) {
                state.tokenize = null;
                break;
            }
            maybeEnd = (ch == "*");
        }
        return "comment";
    },

    tokenBase: function (stream, state) {
        var ch = stream.next();
        if (window.csharpmode.hooks[ch]) {
            var result = window.csharpmode.hooks[ch](stream, state);
            if (result !== false) return result;
        }
        if (ch == '"' || ch == "'") {
            state.tokenize = window.csharpmode.tokenString(ch);
            return state.tokenize(stream, state);
        }
        if (/[\[\]{}\(\),;\:\.]/.test(ch)) {
            window.csharpmode.curPunc = ch;
            return null;
        }
        if (/[\d\.]/.test(ch)) {
            stream.backUp(1)
            if (stream.match(/^(?:0x[a-f\d]+|0b[01]+|(?:\d+\.?\d*|\.\d+)(?:e[-+]?\d+)?)(u|ll?|l|f)?/i)) return "number"
            stream.next()
        }
        if (ch == "/") {
            if (stream.eat("*")) {
                state.tokenize = window.csharpmode.tokenComment;
                return window.csharpmode.tokenComment(stream, state);
            }
            if (stream.eat("/")) {
                stream.skipToEnd();
                return "comment";
            }
        }
        const isOperatorChar = /[+\-*&%=<>!?|\/]/;
        if (isOperatorChar.test(ch)) {
            while (!stream.match(/^\/[\/*]/, false) && stream.eat(isOperatorChar)) { }
            return "operator";
        }
        stream.eatWhile(/[\w\$_\xa1-\uffff]/);

        var cur = stream.current();
        if (window.csharpmode.contains(window.csharpmode.keywords, cur)) {
            if (window.csharpmode.contains(window.csharpmode.blockKeywords, cur)) window.csharpmode.curPunc = "newstatement";
            if (window.csharpmode.contains(window.csharpmode.defKeywords, cur)) window.csharpmode.isDefKeyword = true;
            return "keyword";
        }
        if (window.csharpmode.contains(window.csharpmode.types, cur)) return "type";
        if (window.csharpmode.contains(window.csharpmode.atoms, cur)) return "atom";
        return "variable";
    },

    maybeEOL: function (stream, state) {
        if (stream.eol() && window.csharpmode.isTopScope(state.context))
            state.typeAtEndOfLine = window.csharpmode.typeBefore(stream, state, stream.pos)
    },

    // Interface:

    startState: function (basecolumn) {
        return {
            tokenize: null,
            context: new window.csharpmode.Context((basecolumn || 0) - window.csharpmode.indentUnit, 0, "top", null, false),
            indented: 0,
            startOfLine: true,
            prevToken: null
        };
    },

    token: function (stream, state) {
        var ctx = state.context;
        if (stream.sol()) {
            if (ctx.align == null) ctx.align = false;
            state.indented = stream.indentation();
            state.startOfLine = true;
        }
        if (stream.eatSpace()) { window.csharpmode.maybeEOL(stream, state); return null; }
        window.csharpmode.curPunc = window.csharpmode.isDefKeyword = null;
        var style = (state.tokenize || window.csharpmode.tokenBase)(stream, state);
        if (style == "comment" || style == "meta") return style;
        if (ctx.align == null) ctx.align = true;

        if (window.csharpmode.curPunc == ";" || window.csharpmode.curPunc == ":" || (window.csharpmode.curPunc == "," && stream.match(/^\s*(?:\/\/.*)?$/, false)))
            while (state.context.type == "statement") window.csharpmode.popContext(state);
        else if (window.csharpmode.curPunc == "{") window.csharpmode.pushContext(state, stream.column(), "}");
        else if (window.csharpmode.curPunc == "[") window.csharpmode.pushContext(state, stream.column(), "]");
        else if (window.csharpmode.curPunc == "(") window.csharpmode.pushContext(state, stream.column(), ")");
        else if (window.csharpmode.curPunc == "}") {
            while (ctx.type == "statement") ctx = window.csharpmode.popContext(state);
            if (ctx.type == "}") ctx = window.csharpmode.popContext(state);
            while (ctx.type == "statement") ctx = window.csharpmode.popContext(state);
        }
        else if (window.csharpmode.curPunc == ctx.type) window.csharpmode.popContext(state);
        else if (((ctx.type == "}" || ctx.type == "top") && window.csharpmode.curPunc != ";") ||
            (ctx.type == "statement" && window.csharpmode.curPunc == "newstatement")) {
            window.csharpmode.pushContext(state, stream.column(), "statement", stream.current());
        }

        if (style == "variable" &&
            ((state.prevToken == "def" ||
                (window.csharpmode.typeBefore(stream, state, stream.start) &&
                    window.csharpmode.isTopScope(state.context) && stream.match(/^\s*\(/, false)))))
            style = "def";

        state.startOfLine = false;
        state.prevToken = window.csharpmode.isDefKeyword ? "def" : style || window.csharpmode.curPunc;
        window.csharpmode.maybeEOL(stream, state);
        return style;
    },

    indent: function (state, textAfter) {
        if (state.tokenize != window.csharpmode.tokenBase && state.tokenize != null || state.typeAtEndOfLine) return CodeMirror.Pass;
        var ctx = state.context, firstChar = textAfter && textAfter.charAt(0);
        var closing = firstChar == ctx.type;
        if (ctx.type == "statement" && firstChar == "}") ctx = ctx.prev;
        var switchBlock = ctx.prev && ctx.prev.info == "switch";
        if (/[{(]/.test(firstChar)) {
            while (ctx.type != "top" && ctx.type != "}") ctx = ctx.prev
            return ctx.indented
        }
        if (ctx.type == "statement")
            return ctx.indented + (firstChar == "{" ? 0 : window.csharpmode.statementIndentUnit);
        if (ctx.align)
            return ctx.column + (closing ? 0 : 1);
        if (ctx.type == ")" && !closing)
            return ctx.indented + window.csharpmode.statementIndentUnit;

        return ctx.indented + (closing ? 0 : window.csharpmode.indentUnit) +
            (!closing && switchBlock && !/^(?:case|default)\b/.test(textAfter) ? window.csharpmode.indentUnit : 0);
    },

    electricInput: /^\s*(?:case .*?:|default:|\{\}?|\})$/,
    blockCommentStart: "/*",
    blockCommentEnd: "*/",
    blockCommentContinue: " * ",
    lineComment: "//",
    fold: "brace"
};
window.csharpmode.keywords = window.csharpmode.words(
    "abstract as async await base break case catch checked class const continue" +
    " default delegate do else enum event explicit extern finally fixed for" +
    " foreach goto if implicit in interface internal is lock namespace new" +
    " operator out override params private protected public readonly ref return sealed" +
    " sizeof stackalloc static struct switch this throw try typeof unchecked" +
    " unsafe using virtual void volatile while add alias ascending descending dynamic from get" +
    " global group into join let orderby partial remove select set value var yield");
window.csharpmode.types = window.csharpmode.words(
    "Action Boolean Byte Char DateTime DateTimeOffset Decimal Double Func" +
    " Guid Int16 Int32 Int64 Object SByte Single String Task TimeSpan UInt16 UInt32" +
    " UInt64 bool byte char decimal double short int long object" +
    " sbyte float string ushort uint ulong" +
    " ASCIIEncoding Decoder Encoder Encoding StringBuilder UnicodeEncoding UTF32Encoding UTF7Encoding UTF8Encoding" +
    " Comparer Dictionary EqualityComparer HashSet KeyValuePair LinkedList LinkedListNode List" +
    " Queue ReferenceEqualityComparer SortedDictionary SortedList SortedSet Stack" +
    " ICollection IComparer IDictionary IEnumerable IEnumerator IEqualityComparer IList" +
    " IReadOnlyCollection IReadOnlyDictionary IReadOnlyList IReadOnlySet ISet" +
    " Enumerable ");
window.csharpmode.blockKeywords = window.csharpmode.words("catch class do else finally for foreach if struct switch try while");
window.csharpmode.defKeywords = window.csharpmode.words("class interface namespace struct var");
window.csharpmode.atoms = window.csharpmode.words("true false null");

window.wikimvceditor = {
    currentMode: 0,
    getMDE: function(overlay) {
        var options = {
            autosave: {
                enabled: true,
                uniqueId: `wiki-editor-${wikiItemId}`,
            },
            element: document.getElementById('Markdown'),
            indentWithTabs: false,
            initialValue: initialWikiEditValue || '',
            placeholder: "Enter your content here, in markdown format.",
            tabSize: 4,
            toolbar: ['bold', 'italic', '|', 'heading-1', 'heading-2', 'heading-3', '|', 'unordered-list', 'ordered-list', '|', 'link', 'image', '|',
                {
                    name: "mode",
                    action: function modeSwitch() {
                        if (window.wikimvceditor.currentMode === 0) {
                            window.wikimvceditor.easyMDE.toTextArea();
                            window.wikimvceditor.easyMDE = window.wikimvceditor.getMDE(true);
                            window.wikimvceditor.currentMode = 1;
                        } else {
                            window.wikimvceditor.easyMDE.toTextArea();
                            window.wikimvceditor.easyMDE = window.wikimvceditor.getMDE();
                            window.wikimvceditor.currentMode = 0;
                        }
                    },
                    className: "fa fa-code",
                    title: "Toggle script mode",
                }
            ]
        };
        if (overlay) {
            options.overlayMode = {
                mode: window.csharpmode,
                combine: false,
            };
            options.theme = "vscode-dark";
        }
        let mde = new EasyMDE(options);
        mde.codemirror.addKeyMap({
            'Home': 'goLineLeft',
            'End': 'goLineRight',
        });
        return mde;
    },

    toggleOwnerSelf: function(e) {
        e = e || window.event;
        if (e.isComposing || e.keyCode === 229 || (e.key && e.key !== "Enter")) {
            return;
        }
        const i = document.getElementById('Owner');
        if (!i) {
            return;
        }
        const c = document.getElementById('OwnerSelf');
        if (!c) {
            return;
        }
        if (c.checked) {
            i.value = "";
        }
        i.readOnly = c.checked;
    },

    toggleEditorSelf: function(e) {
        e = e || window.event;
        if (e.isComposing || e.keyCode === 229 || (e.key && e.key !== "Enter")) {
            return;
        }
        const i = document.getElementById('AllowedEditors');
        if (!i) {
            return;
        }
        const c = document.getElementById('EditorSelf');
        if (!c) {
            return;
        }
        if (c.checked) {
            i.value = "";
        }
        i.readOnly = c.checked;
    },

    toggleViewerSelf: function(e) {
        e = e || window.event;
        if (e.isComposing || e.keyCode === 229 || (e.key && e.key !== "Enter")) {
            return;
        }
        const i = document.getElementById('AllowedViewers');
        if (!i) {
            return;
        }
        const c = document.getElementById('ViewerSelf');
        if (!c) {
            return;
        }
        if (c.checked) {
            i.value = "";
        }
        i.readOnly = c.checked;
    },

    redirectCheck: function() {
        const r = document.getElementById('wiki-edit-redirect');
        if (!r) {
            return;
        }
        const t = document.getElementById('Title');
        if (!t) {
            return;
        }
        if (t.dataset.original
            && t.value != t.dataset.original) {
            r.style.display = 'block';
        } else {
            r.style.display = 'none';
        }
    },

    toggleTransclusions: function(e) {
        e = e || window.event;
        if (e.isComposing || e.keyCode === 229 || (e.key && e.key !== "Enter")) {
            return;
        }
        const l = document.getElementById('wiki-edit-transclusionlist');
        if (!l) {
            return;
        }
        const t = document.getElementById('wiki-edit-transclusions-toggler');
        if (!t) {
            return;
        }
        window.wikimvc.editTransclusionsShown = !window.wikimvc.editTransclusionsShown;
        t.classList.toggle("is-active");
        if (window.wikimvc.editTransclusionsShown) {
            l.style.display = 'block';
        } else {
            l.style.display = 'none';
        }
    },

    delete: function(e) {
        e = e || window.event;
        if (e.isComposing || e.keyCode === 229 || (e.key && e.key !== "Enter")) {
            return;
        }
        const d = document.getElementById('wiki-edit-confirmed-delete-button');
        if (!d) {
            return;
        }
        const b = document.getElementById('wiki-edit-delete-button');
        if (!b) {
            return;
        }
        b.style.display = 'none';
        d.style.display = 'inline-block';
    },
}
window.wikimvceditor.easyMDE = window.wikimvceditor.getMDE();
