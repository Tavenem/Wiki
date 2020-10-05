window.jsmode = {
    indentUnit: 4,
    statementIndent: 4,
    wordRE: /[\w$\xa1-\uffff]/,

    // Tokenizer

    keywords: function () {
        function kw(type) { return { type: type, style: "keyword" }; }
        var A = kw("keyword a"), B = kw("keyword b"), C = kw("keyword c"), D = kw("keyword d");
        var operator = kw("operator"), atom = { type: "atom", style: "atom" };

        return {
            "if": kw("if"), "while": A, "with": A, "else": B, "do": B, "try": B, "finally": B,
            "return": D, "break": D, "continue": D, "new": kw("new"), "delete": C, "void": C, "throw": C,
            "debugger": kw("debugger"), "var": kw("var"), "const": kw("var"), "let": kw("var"),
            "function": kw("function"), "catch": kw("catch"),
            "for": kw("for"), "switch": kw("switch"), "case": kw("case"), "default": kw("default"),
            "in": operator, "typeof": operator, "instanceof": operator,
            "true": atom, "false": atom, "null": atom, "undefined": atom, "NaN": atom, "Infinity": atom,
            "this": kw("this"), "class": kw("class"), "super": kw("atom"),
            "yield": C, "export": kw("export"), "import": kw("import"), "extends": C,
            "await": C
        };
    }(),

    isOperatorChar: /[+\-*&%=<>!?|~^@]/,

    readRegexp: function(stream) {
        var escaped = false, next, inSet = false;
        while ((next = stream.next()) != null) {
            if (!escaped) {
                if (next == "/" && !inSet) return;
                if (next == "[") inSet = true;
                else if (inSet && next == "]") inSet = false;
            }
            escaped = !escaped && next == "\\";
        }
    },

    // Used as scratch variables to communicate multiple values without
    // consing up tons of objects.
    type: null,
    content: null,
    ret: function(tp, style, cont) {
        window.jsmode.type = tp; window.jsmode.content = cont;
        return style;
    },
    tokenBase: function(stream, state) {
        var ch = stream.next();
        if (ch == '"' || ch == "'") {
            state.tokenize = window.jsmode.tokenString(ch);
            return state.tokenize(stream, state);
        } else if (ch == "." && stream.match(/^\d[\d_]*(?:[eE][+\-]?[\d_]+)?/)) {
            return window.jsmode.ret("number", "number");
        } else if (ch == "." && stream.match("..")) {
            return window.jsmode.ret("spread", "meta");
        } else if (/[\[\]{}\(\),;\:\.]/.test(ch)) {
            return window.jsmode.ret(ch);
        } else if (ch == "=" && stream.eat(">")) {
            return window.jsmode.ret("=>", "operator");
        } else if (ch == "0" && stream.match(/^(?:x[\dA-Fa-f_]+|o[0-7_]+|b[01_]+)n?/)) {
            return window.jsmode.ret("number", "number");
        } else if (/\d/.test(ch)) {
            stream.match(/^[\d_]*(?:n|(?:\.[\d_]*)?(?:[eE][+\-]?[\d_]+)?)?/);
            return window.jsmode.ret("number", "number");
        } else if (ch == "/") {
            if (stream.eat("*")) {
                state.tokenize = window.jsmode.tokenComment;
                return window.jsmode.tokenComment(stream, state);
            } else if (stream.eat("/")) {
                stream.skipToEnd();
                return window.jsmode.ret("comment", "comment");
            } else if (window.jsmode.expressionAllowed(stream, state, 1)) {
                window.jsmode.readRegexp(stream);
                stream.match(/^\b(([gimyus])(?![gimyus]*\2))+\b/);
                return window.jsmode.ret("regexp", "string-2");
            } else {
                stream.eat("=");
                return window.jsmode.ret("operator", "operator", stream.current());
            }
        } else if (ch == "`") {
            state.tokenize = window.jsmode.tokenQuasi;
            return window.jsmode.tokenQuasi(stream, state);
        } else if (ch == "#" && stream.peek() == "!") {
            stream.skipToEnd();
            return window.jsmode.ret("meta", "meta");
        } else if (ch == "#" && stream.eatWhile(window.jsmode.wordRE)) {
            return window.jsmode.ret("variable", "property")
        } else if (ch == "<" && stream.match("!--") ||
            (ch == "-" && stream.match("->") && !/\S/.test(stream.string.slice(0, stream.start)))) {
            stream.skipToEnd()
            return window.jsmode.ret("comment", "comment")
        } else if (window.jsmode.isOperatorChar.test(ch)) {
            if (ch != ">" || !state.lexical || state.lexical.type != ">") {
                if (stream.eat("=")) {
                    if (ch == "!" || ch == "=") stream.eat("=")
                } else if (/[<>*+\-|&?]/.test(ch)) {
                    stream.eat(ch)
                    if (ch == ">") stream.eat(ch)
                }
            }
            if (ch == "?" && stream.eat(".")) return window.jsmode.ret(".")
            return window.jsmode.ret("operator", "operator", stream.current());
        } else if (window.jsmode.wordRE.test(ch)) {
            stream.eatWhile(window.jsmode.wordRE);
            var word = stream.current()
            if (state.lastType != ".") {
                if (window.jsmode.keywords.propertyIsEnumerable(word)) {
                    var kw = window.jsmode.keywords[word]
                    return window.jsmode.ret(kw.type, kw.style, word)
                }
                if (word == "async" && stream.match(/^(\s|\/\*.*?\*\/)*[\[\(\w]/, false))
                    return window.jsmode.ret("async", "keyword", word)
            }
            return window.jsmode.ret("variable", "variable", word)
        }
    },

    tokenString: function(quote) {
        return function (stream, state) {
            var escaped = false, next;
            while ((next = stream.next()) != null) {
                if (next == quote && !escaped) break;
                escaped = !escaped && next == "\\";
            }
            if (!escaped) state.tokenize = window.jsmode.tokenBase;
            return window.jsmode.ret("string", "string");
        };
    },

    tokenComment: function(stream, state) {
        var maybeEnd = false, ch;
        while (ch = stream.next()) {
            if (ch == "/" && maybeEnd) {
                state.tokenize = window.jsmode.tokenBase;
                break;
            }
            maybeEnd = (ch == "*");
        }
        return window.jsmode.ret("comment", "comment");
    },

    tokenQuasi: function(stream, state) {
        var escaped = false, next;
        while ((next = stream.next()) != null) {
            if (!escaped && (next == "`" || next == "$" && stream.eat("{"))) {
                state.tokenize = window.jsmode.tokenBase;
                break;
            }
            escaped = !escaped && next == "\\";
        }
        return window.jsmode.ret("window.jsmode.quasi", "string-2", stream.current());
    },

    brackets: "([{}])",
    // This is a crude lookahead trick to try and notice that we're
    // parsing the argument patterns for a fat-arrow function before we
    // actually hit the arrow token. It only works if the arrow is on
    // the same line as the arguments and there's no strange noise
    // (comments) in between. Fallback is to only notice when we hit the
    // arrow, and not declare the arguments as locals for the arrow
    // body.
    findFatArrow: function(stream, state) {
        if (state.fatArrowAt) state.fatArrowAt = null;
        var arrow = stream.string.indexOf("=>", stream.start);
        if (arrow < 0) return;

        var depth = 0, sawSomething = false;
        for (var pos = arrow - 1; pos >= 0; --pos) {
            var ch = stream.string.charAt(pos);
            var bracket = window.jsmode.brackets.indexOf(ch);
            if (bracket >= 0 && bracket < 3) {
                if (!depth) { ++pos; break; }
                if (--depth == 0) { if (ch == "(") sawSomething = true; break; }
            } else if (bracket >= 3 && bracket < 6) {
                ++depth;
            } else if (window.jsmode.wordRE.test(ch)) {
                sawSomething = true;
            } else if (/["'\/`]/.test(ch)) {
                for (; ; --pos) {
                    if (pos == 0) return
                    var next = stream.string.charAt(pos - 1)
                    if (next == ch && stream.string.charAt(pos - 2) != "\\") { pos--; break }
                }
            } else if (sawSomething && !depth) {
                ++pos;
                break;
            }
        }
        if (sawSomething && !depth) state.fatArrowAt = pos;
    },

    // Parser

    atomicTypes: { "atom": true, "number": true, "variable": true, "string": true, "regexp": true, "this": true, "jsonld-keyword": true },

    JSLexical: function(indented, column, type, align, prev, info) {
        this.indented = indented;
        this.column = column;
        this.type = type;
        this.prev = prev;
        this.info = info;
        if (align != null) this.align = align;
    },

    inScope: function(state, varname) {
        for (var v = state.localVars; v; v = v.next)
            if (v.name == varname) return true;
        for (var cx = state.context; cx; cx = cx.prev) {
            for (var v = cx.vars; v; v = v.next)
                if (v.name == varname) return true;
        }
    },

    parseJS: function(state, style, type, content, stream) {
        var cc = state.cc;
        // Communicate our context to the combinators.
        // (Less wasteful than consing up a hundred closures on every call.)
        window.jsmode.cx.state = state; window.jsmode.cx.stream = stream; window.jsmode.cx.marked = null, window.jsmode.cx.cc = cc; window.jsmode.cx.style = style;

        if (!state.lexical.hasOwnProperty("align"))
            state.lexical.align = true;

        while (true) {
            var combinator = cc.length ? cc.pop() : window.jsmode.statement;
            if (combinator(type, content)) {
                while (cc.length && cc[cc.length - 1].lex)
                    cc.pop()();
                if (window.jsmode.cx.marked) return window.jsmode.cx.marked;
                if (type == "variable" && window.jsmode.inScope(state, content)) return "variable-2";
                return style;
            }
        }
    },

    // Combinator utils

    cx: { state: null, column: null, marked: null, cc: null },
    pass: function() {
        for (var i = arguments.length - 1; i >= 0; i--) window.jsmode.cx.cc.push(arguments[i]);
    },
    cont: function() {
        window.jsmode.pass.apply(null, arguments);
        return true;
    },
    inList: function(name, list) {
        for (var v = list; v; v = v.next) if (v.name == name) return true
        return false;
    },
    register: function(varname) {
        var state = window.jsmode.cx.state;
        window.jsmode.cx.marked = "def";
        if (state.context) {
            if (state.lexical.info == "var" && state.context && state.context.block) {
                // FIXME function decls are also not block scoped
                var newContext = window.jsmode.registerVarScoped(varname, state.context)
                if (newContext != null) {
                    state.context = newContext
                    return
                }
            } else if (!window.jsmode.inList(varname, state.localVars)) {
                state.localVars = new window.jsmode.Var(varname, state.localVars)
                return
            }
        }
        // Fall through means this is global
        if (!window.jsmode.inList(varname, state.globalVars))
            state.globalVars = new window.jsmode.Var(varname, state.globalVars)
    },
    registerVarScoped: function(varname, context) {
        if (!context) {
            return null
        } else if (context.block) {
            var inner = window.jsmode.registerVarScoped(varname, context.prev)
            if (!inner) return null
            if (inner == context.prev) return context
            return new window.jsmode.Context(inner, context.vars, true)
        } else if (window.jsmode.inList(varname, context.vars)) {
            return context
        } else {
            return new window.jsmode.Context(context.prev, new window.jsmode.Var(varname, context.vars), false)
        }
    },

    isModifier: function(name) {
        return name == "public" || name == "private" || name == "protected" || name == "abstract" || name == "readonly"
    },

    // Combinators

    Context: function(prev, vars, block) { this.prev = prev; this.vars = vars; this.block = block },
    Var: function(name, next) { this.name = name; this.next = next },

    pushcontext: function() {
        window.jsmode.cx.state.context = new window.jsmode.Context(window.jsmode.cx.state.context, window.jsmode.cx.state.localVars, false)
        window.jsmode.cx.state.localVars = defaultVars
    },
    pushblockcontext: function() {
        window.jsmode.cx.state.context = new window.jsmode.Context(window.jsmode.cx.state.context, window.jsmode.cx.state.localVars, true)
        window.jsmode.cx.state.localVars = null
    },
    popcontext: function() {
        window.jsmode.cx.state.localVars = window.jsmode.cx.state.context.vars
        window.jsmode.cx.state.context = window.jsmode.cx.state.context.prev
    },
    pushlex: function(type, info) {
        var result = function () {
            var state = window.jsmode.cx.state, indent = state.indented;
            if (state.lexical.type == "stat") indent = state.lexical.indented;
            else for (var outer = state.lexical; outer && outer.type == ")" && outer.align; outer = outer.prev)
                indent = outer.indented;
            state.lexical = new window.jsmode.JSLexical(indent, window.jsmode.cx.stream.column(), type, null, state.lexical, info);
        };
        result.lex = true;
        return result;
    },
    poplex: function() {
        var state = window.jsmode.cx.state;
        if (state.lexical.prev) {
            if (state.lexical.type == ")")
                state.indented = state.lexical.indented;
            state.lexical = state.lexical.prev;
        }
    },

    expect: function(wanted) {
        function exp(type) {
            if (type == wanted) return window.jsmode.cont();
            else if (wanted == ";" || type == "}" || type == ")" || type == "]") return window.jsmode.pass();
            else return window.jsmode.cont(exp);
        };
        return exp;
    },

    statement: function(type, value) {
        if (type == "var") return window.jsmode.cont(window.jsmode.pushlex("window.jsmode.vardef", value), window.jsmode.vardef, window.jsmode.expect(";"), window.jsmode.poplex);
        if (type == "keyword a") return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.parenExpr, window.jsmode.statement, window.jsmode.poplex);
        if (type == "keyword b") return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.statement, window.jsmode.poplex);
        if (type == "keyword d") return window.jsmode.cx.stream.match(/^\s*$/, false) ? window.jsmode.cont() : window.jsmode.cont(window.jsmode.pushlex("stat"), window.jsmode.maybeexpression, window.jsmode.expect(";"), window.jsmode.poplex);
        if (type == "debugger") return window.jsmode.cont(window.jsmode.expect(";"));
        if (type == "{") return window.jsmode.cont(window.jsmode.pushlex("}"), window.jsmode.pushblockcontext, window.jsmode.block, window.jsmode.poplex, window.jsmode.popcontext);
        if (type == ";") return window.jsmode.cont();
        if (type == "if") {
            if (window.jsmode.cx.state.lexical.info == "else" && window.jsmode.cx.state.cc[window.jsmode.cx.state.cc.length - 1] == window.jsmode.poplex)
                window.jsmode.cx.state.cc.pop()();
            return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.parenExpr, window.jsmode.statement, window.jsmode.poplex, window.jsmode.maybeelse);
        }
        if (type == "function") return window.jsmode.cont(window.jsmode.functiondef);
        if (type == "for") return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.forspec, window.jsmode.statement, window.jsmode.poplex);
        if (type == "class") {
            window.jsmode.cx.marked = "keyword"
            return window.jsmode.cont(window.jsmode.pushlex("form", type == "class" ? type : value), window.jsmode.className, window.jsmode.poplex)
        }
        if (type == "variable") {
            return window.jsmode.cont(window.jsmode.pushlex("stat"), window.jsmode.maybelabel);
        }
        if (type == "switch") return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.parenExpr, window.jsmode.expect("{"), window.jsmode.pushlex("}", "switch"), window.jsmode.pushblockcontext,
            window.jsmode.block, window.jsmode.poplex, window.jsmode.poplex, window.jsmode.popcontext);
        if (type == "case") return window.jsmode.cont(window.jsmode.expression, window.jsmode.expect(":"));
        if (type == "default") return window.jsmode.cont(window.jsmode.expect(":"));
        if (type == "catch") return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.pushcontext, window.jsmode.maybeCatchBinding, window.jsmode.statement, window.jsmode.poplex, window.jsmode.popcontext);
        if (type == "export") return window.jsmode.cont(window.jsmode.pushlex("stat"), window.jsmode.afterExport, window.jsmode.poplex);
        if (type == "import") return window.jsmode.cont(window.jsmode.pushlex("stat"), window.jsmode.afterImport, window.jsmode.poplex);
        if (type == "async") return window.jsmode.cont(window.jsmode.statement)
        if (value == "@") return window.jsmode.cont(window.jsmode.expression, window.jsmode.statement)
        return window.jsmode.pass(window.jsmode.pushlex("stat"), window.jsmode.expression, window.jsmode.expect(";"), window.jsmode.poplex);
    },
    maybeCatchBinding: function(type) {
        if (type == "(") return window.jsmode.cont(window.jsmode.funarg, window.jsmode.expect(")"))
    },
    expression: function(type, value) {
        return window.jsmode.expressionInner(type, value, false);
    },
    expressionNoComma: function(type, value) {
        return window.jsmode.expressionInner(type, value, true);
    },
    parenExpr: function(type) {
        if (type != "(") return window.jsmode.pass()
        return window.jsmode.cont(window.jsmode.pushlex(")"), window.jsmode.maybeexpression, window.jsmode.expect(")"), window.jsmode.poplex)
    },
    expressionInner: function(type, value, noComma) {
        if (window.jsmode.cx.state.fatArrowAt == window.jsmode.cx.stream.start) {
            var body = noComma ? window.jsmode.arrowBodyNoComma : window.jsmode.arrowBody;
            if (type == "(") return window.jsmode.cont(window.jsmode.pushcontext, window.jsmode.pushlex(")"), window.jsmode.commasep(window.jsmode.funarg, ")"), window.jsmode.poplex, window.jsmode.expect("=>"), body, window.jsmode.popcontext);
            else if (type == "variable") return window.jsmode.pass(window.jsmode.pushcontext, window.jsmode.pattern, window.jsmode.expect("=>"), body, window.jsmode.popcontext);
        }

        var maybeop = noComma ? window.jsmode.maybeoperatorNoComma : window.jsmode.maybeoperatorComma;
        if (window.jsmode.atomicTypes.hasOwnProperty(type)) return window.jsmode.cont(maybeop);
        if (type == "function") return window.jsmode.cont(window.jsmode.functiondef, maybeop);
        if (type == "class") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.pushlex("form"), window.jsmode.classExpression, window.jsmode.poplex); }
        if (type == "keyword c" || type == "async") return window.jsmode.cont(noComma ? window.jsmode.expressionNoComma : window.jsmode.expression);
        if (type == "(") return window.jsmode.cont(window.jsmode.pushlex(")"), window.jsmode.maybeexpression, window.jsmode.expect(")"), window.jsmode.poplex, maybeop);
        if (type == "operator" || type == "spread") return window.jsmode.cont(noComma ? window.jsmode.expressionNoComma : window.jsmode.expression);
        if (type == "[") return window.jsmode.cont(window.jsmode.pushlex("]"), window.jsmode.arrayLiteral, window.jsmode.poplex, maybeop);
        if (type == "{") return window.jsmode.contCommasep(window.jsmode.objprop, "}", null, maybeop);
        if (type == "window.jsmode.quasi") return window.jsmode.pass(window.jsmode.quasi, maybeop);
        if (type == "new") return window.jsmode.cont(window.jsmode.maybeTarget(noComma));
        if (type == "import") return window.jsmode.cont(window.jsmode.expression);
        return window.jsmode.cont();
    },
    maybeexpression: function(type) {
        if (type.match(/[;\}\)\],]/)) return window.jsmode.pass();
        return window.jsmode.pass(window.jsmode.expression);
    },

    maybeoperatorComma: function(type, value) {
        if (type == ",") return window.jsmode.cont(window.jsmode.maybeexpression);
        return window.jsmode.maybeoperatorNoComma(type, value, false);
    },
    maybeoperatorNoComma: function(type, value, noComma) {
        var me = noComma == false ? window.jsmode.maybeoperatorComma : window.jsmode.maybeoperatorNoComma;
        var expr = noComma == false ? window.jsmode.expression : window.jsmode.expressionNoComma;
        if (type == "=>") return window.jsmode.cont(window.jsmode.pushcontext, noComma ? window.jsmode.arrowBodyNoComma : window.jsmode.arrowBody, window.jsmode.popcontext);
        if (type == "operator") {
            if (/\+\+|--/.test(value)) return window.jsmode.cont(me);
            if (value == "?") return window.jsmode.cont(window.jsmode.expression, window.jsmode.expect(":"), expr);
            return window.jsmode.cont(expr);
        }
        if (type == "window.jsmode.quasi") { return window.jsmode.pass(window.jsmode.quasi, me); }
        if (type == ";") return;
        if (type == "(") return window.jsmode.contCommasep(window.jsmode.expressionNoComma, ")", "call", me);
        if (type == ".") return window.jsmode.cont(window.jsmode.property, me);
        if (type == "[") return window.jsmode.cont(window.jsmode.pushlex("]"), window.jsmode.maybeexpression, window.jsmode.expect("]"), window.jsmode.poplex, me);
        if (type == "regexp") {
            window.jsmode.cx.state.lastType = window.jsmode.cx.marked = "operator"
            window.jsmode.cx.stream.backUp(window.jsmode.cx.stream.pos - window.jsmode.cx.stream.start - 1)
            return window.jsmode.cont(expr)
        }
    },
    quasi: function(type, value) {
        if (type != "quasi") return window.jsmode.pass();
        if (value.slice(value.length - 2) != "${") return window.jsmode.cont(window.jsmode.quasi);
        return window.jsmode.cont(window.jsmode.expression, window.jsmode.continueQuasi);
    },
    continueQuasi: function(type) {
        if (type == "}") {
            window.jsmode.cx.marked = "string-2";
            window.jsmode.cx.state.tokenize = window.jsmode.tokenQuasi;
            return window.jsmode.cont(window.jsmode.quasi);
        }
    },
    arrowBody: function(type) {
        window.jsmode.findFatArrow(window.jsmode.cx.stream, window.jsmode.cx.state);
        return window.jsmode.pass(type == "{" ? window.jsmode.statement : window.jsmode.expression);
    },
    arrowBodyNoComma: function(type) {
        window.jsmode.findFatArrow(window.jsmode.cx.stream, window.jsmode.cx.state);
        return window.jsmode.pass(type == "{" ? window.jsmode.statement : window.jsmode.expressionNoComma);
    },
    maybeTarget: function(noComma) {
        return function (type) {
            if (type == ".") return window.jsmode.cont(noComma ? window.jsmode.targetNoComma : window.jsmode.target);
            else return window.jsmode.pass(noComma ? window.jsmode.expressionNoComma : window.jsmode.expression);
        };
    },
    target: function(_, value) {
        if (value == "target") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.maybeoperatorComma); }
    },
    targetNoComma: function(_, value) {
        if (value == "target") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.maybeoperatorNoComma); }
    },
    maybelabel: function(type) {
        if (type == ":") return window.jsmode.cont(window.jsmode.poplex, window.jsmode.statement);
        return window.jsmode.pass(window.jsmode.maybeoperatorComma, window.jsmode.expect(";"), window.jsmode.poplex);
    },
    property: function(type) {
        if (type == "variable") { window.jsmode.cx.marked = "property"; return window.jsmode.cont(); }
    },
    objprop: function(type, value) {
        if (type == "async") {
            window.jsmode.cx.marked = "property";
            return window.jsmode.cont(window.jsmode.objprop);
        } else if (type == "variable" || window.jsmode.cx.style == "keyword") {
            window.jsmode.cx.marked = "property";
            if (value == "get" || value == "set") return window.jsmode.cont(window.jsmode.getterSetter);
            var m // Work around fat-arrow-detection complication for detecting typescript typed arrow params
            return window.jsmode.cont(window.jsmode.afterprop);
        } else if (type == "number" || type == "string") {
            window.jsmode.cx.marked = window.jsmode.cx.style + " property";
            return window.jsmode.cont(window.jsmode.afterprop);
        } else if (type == "jsonld-keyword") {
            return window.jsmode.cont(window.jsmode.afterprop);
        } else if (type == "[") {
            return window.jsmode.cont(window.jsmode.expression, window.jsmode.maybetype, window.jsmode.expect("]"), window.jsmode.afterprop);
        } else if (type == "spread") {
            return window.jsmode.cont(window.jsmode.expressionNoComma, window.jsmode.afterprop);
        } else if (value == "*") {
            window.jsmode.cx.marked = "keyword";
            return window.jsmode.cont(window.jsmode.objprop);
        } else if (type == ":") {
            return window.jsmode.pass(window.jsmode.afterprop)
        }
    },
    getterSetter: function(type) {
        if (type != "variable") return window.jsmode.pass(window.jsmode.afterprop);
        window.jsmode.cx.marked = "property";
        return window.jsmode.cont(window.jsmode.functiondef);
    },
    afterprop: function(type) {
        if (type == ":") return window.jsmode.cont(window.jsmode.expressionNoComma);
        if (type == "(") return window.jsmode.pass(window.jsmode.functiondef);
    },
    commasep: function(what, end, sep) {
        function proceed(type, value) {
            if (sep ? sep.indexOf(type) > -1 : type == ",") {
                var lex = window.jsmode.cx.state.lexical;
                if (lex.info == "call") lex.pos = (lex.pos || 0) + 1;
                return window.jsmode.cont(function (type, value) {
                    if (type == end || value == end) return window.jsmode.pass()
                    return window.jsmode.pass(what)
                }, proceed);
            }
            if (type == end || value == end) return window.jsmode.cont();
            if (sep && sep.indexOf(";") > -1) return window.jsmode.pass(what)
            return window.jsmode.cont(window.jsmode.expect(end));
        }
        return function (type, value) {
            if (type == end || value == end) return window.jsmode.cont();
            return window.jsmode.pass(what, proceed);
        };
    },
    contCommasep: function(what, end, info) {
        for (var i = 3; i < arguments.length; i++)
            window.jsmode.cx.cc.push(arguments[i]);
        return window.jsmode.cont(window.jsmode.pushlex(end, info), window.jsmode.commasep(what, end), window.jsmode.poplex);
    },
    block: function(type) {
        if (type == "}") return window.jsmode.cont();
        return window.jsmode.pass(window.jsmode.statement, window.jsmode.block);
    },
    maybetype: function() {},
    maybetypeOrIn: function() {},
    mayberettype: function() {},
    typeexpr: function(type, value) {
        if (value == "keyof" || value == "typeof" || value == "infer") {
            window.jsmode.cx.marked = "keyword"
            return window.jsmode.cont(value == "typeof" ? window.jsmode.expressionNoComma : window.jsmode.typeexpr)
        }
        if (type == "variable" || value == "void") {
            window.jsmode.cx.marked = "type"
            return window.jsmode.cont(window.jsmode.afterType)
        }
        if (value == "|" || value == "&") return window.jsmode.cont(window.jsmode.typeexpr)
        if (type == "string" || type == "number" || type == "atom") return window.jsmode.cont(window.jsmode.afterType);
        if (type == "[") return window.jsmode.cont(window.jsmode.pushlex("]"), window.jsmode.commasep(window.jsmode.typeexpr, "]", ","), window.jsmode.poplex, window.jsmode.afterType)
        if (type == "{") return window.jsmode.cont(window.jsmode.pushlex("}"), window.jsmode.commasep(window.jsmode.typeprop, "}", ",;"), window.jsmode.poplex, window.jsmode.afterType)
        if (type == "(") return window.jsmode.cont(window.jsmode.commasep(window.jsmode.typearg, ")"), window.jsmode.maybeReturnType, window.jsmode.afterType)
        if (type == "<") return window.jsmode.cont(window.jsmode.commasep(window.jsmode.typeexpr, ">"), window.jsmode.typeexpr)
    },
    maybeReturnType: function(type) {
        if (type == "=>") return window.jsmode.cont(window.jsmode.typeexpr)
    },
    typeprop: function(type, value) {
        if (type == "variable" || window.jsmode.cx.style == "keyword") {
            window.jsmode.cx.marked = "property"
            return window.jsmode.cont(window.jsmode.typeprop)
        } else if (value == "?" || type == "number" || type == "string") {
            return window.jsmode.cont(window.jsmode.typeprop)
        } else if (type == ":") {
            return window.jsmode.cont(window.jsmode.typeexpr)
        } else if (type == "[") {
            return window.jsmode.cont(window.jsmode.expect("variable"), window.jsmode.maybetypeOrIn, window.jsmode.expect("]"), window.jsmode.typeprop)
        } else if (type == "(") {
            return window.jsmode.pass(window.jsmode.functiondecl, window.jsmode.typeprop)
        }
    },
    typearg: function(type, value) {
        if (type == "variable" && window.jsmode.cx.stream.match(/^\s*[?:]/, false) || value == "?") return window.jsmode.cont(window.jsmode.typearg)
        if (type == ":") return window.jsmode.cont(window.jsmode.typeexpr)
        if (type == "spread") return window.jsmode.cont(window.jsmode.typearg)
        return window.jsmode.pass(window.jsmode.typeexpr)
    },
    afterType: function(type, value) {
        if (value == "<") return window.jsmode.cont(window.jsmode.pushlex(">"), window.jsmode.commasep(window.jsmode.typeexpr, ">"), window.jsmode.poplex, window.jsmode.afterType)
        if (value == "|" || type == "." || value == "&") return window.jsmode.cont(window.jsmode.typeexpr)
        if (type == "[") return window.jsmode.cont(window.jsmode.typeexpr, window.jsmode.expect("]"), window.jsmode.afterType)
        if (value == "extends" || value == "implements") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.typeexpr) }
        if (value == "?") return window.jsmode.cont(window.jsmode.typeexpr, window.jsmode.expect(":"), window.jsmode.typeexpr)
    },
    typeparam: function() {
        return window.jsmode.pass(window.jsmode.typeexpr, window.jsmode.maybeTypeDefault)
    },
    maybeTypeDefault: function(_, value) {
        if (value == "=") return window.jsmode.cont(window.jsmode.typeexpr)
    },
    vardef: function(_, value) {
        if (value == "enum") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.enumdef) }
        return window.jsmode.pass(window.jsmode.pattern, window.jsmode.maybetype, window.jsmode.maybeAssign, window.jsmode.vardefCont);
    },
    pattern: function(type, value) {
        if (type == "variable") { window.jsmode.register(value); return window.jsmode.cont(); }
        if (type == "spread") return window.jsmode.cont(window.jsmode.pattern);
        if (type == "[") return window.jsmode.contCommasep(window.jsmode.eltpattern, "]");
        if (type == "{") return window.jsmode.contCommasep(window.jsmode.proppattern, "}");
    },
    proppattern: function(type, value) {
        if (type == "variable" && !window.jsmode.cx.stream.match(/^\s*:/, false)) {
            window.jsmode.register(value);
            return window.jsmode.cont(window.jsmode.maybeAssign);
        }
        if (type == "variable") window.jsmode.cx.marked = "property";
        if (type == "spread") return window.jsmode.cont(window.jsmode.pattern);
        if (type == "}") return window.jsmode.pass();
        if (type == "[") return window.jsmode.cont(window.jsmode.expression, window.jsmode.expect(']'), window.jsmode.expect(':'), window.jsmode.proppattern);
        return window.jsmode.cont(window.jsmode.expect(":"), window.jsmode.pattern, window.jsmode.maybeAssign);
    },
    eltpattern: function() {
        return window.jsmode.pass(window.jsmode.pattern, window.jsmode.maybeAssign)
    },
    maybeAssign: function(_type, value) {
        if (value == "=") return window.jsmode.cont(window.jsmode.expressionNoComma);
    },
    vardefCont: function(type) {
        if (type == ",") return window.jsmode.cont(window.jsmode.vardef);
    },
    maybeelse: function(type, value) {
        if (type == "keyword b" && value == "else") return window.jsmode.cont(window.jsmode.pushlex("form", "else"), window.jsmode.statement, window.jsmode.poplex);
    },
    forspec: function(type, value) {
        if (value == "await") return window.jsmode.cont(window.jsmode.forspec);
        if (type == "(") return window.jsmode.cont(window.jsmode.pushlex(")"), window.jsmode.forspec1, window.jsmode.poplex);
    },
    forspec1: function(type) {
        if (type == "var") return window.jsmode.cont(window.jsmode.vardef, window.jsmode.forspec2);
        if (type == "variable") return window.jsmode.cont(window.jsmode.forspec2);
        return window.jsmode.pass(window.jsmode.forspec2)
    },
    forspec2: function(type, value) {
        if (type == ")") return window.jsmode.cont()
        if (type == ";") return window.jsmode.cont(window.jsmode.forspec2)
        if (value == "in" || value == "of") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.expression, window.jsmode.forspec2) }
        return window.jsmode.pass(window.jsmode.expression, window.jsmode.forspec2)
    },
    functiondef: function(type, value) {
        if (value == "*") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.functiondef); }
        if (type == "variable") { window.jsmode.register(value); return window.jsmode.cont(window.jsmode.functiondef); }
        if (type == "(") return window.jsmode.cont(window.jsmode.pushcontext, window.jsmode.pushlex(")"), window.jsmode.commasep(window.jsmode.funarg, ")"), window.jsmode.poplex, window.jsmode.mayberettype, window.jsmode.statement, window.jsmode.popcontext);
    },
    functiondecl: function(type, value) {
        if (value == "*") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.functiondecl); }
        if (type == "variable") { window.jsmode.register(value); return window.jsmode.cont(window.jsmode.functiondecl); }
        if (type == "(") return window.jsmode.cont(window.jsmode.pushcontext, window.jsmode.pushlex(")"), window.jsmode.commasep(window.jsmode.funarg, ")"), window.jsmode.poplex, window.jsmode.mayberettype, window.jsmode.popcontext);
    },
    funarg: function(type, value) {
        if (value == "@") window.jsmode.cont(window.jsmode.expression, window.jsmode.funarg)
        if (type == "spread") return window.jsmode.cont(window.jsmode.funarg);
        return window.jsmode.pass(window.jsmode.pattern, window.jsmode.maybetype, window.jsmode.maybeAssign);
    },
    classExpression: function(type, value) {
        // Class expressions may have an optional name.
        if (type == "variable") return window.jsmode.className(type, value);
        return window.jsmode.classNameAfter(type, value);
    },
    className: function(type, value) {
        if (type == "variable") { window.jsmode.register(value); return window.jsmode.cont(window.jsmode.classNameAfter); }
    },
    classNameAfter: function(type, value) {
        if (value == "<") return window.jsmode.cont(window.jsmode.pushlex(">"), window.jsmode.commasep(window.jsmode.typeparam, ">"), window.jsmode.poplex, window.jsmode.classNameAfter)
        if (value == "extends" || value == "implements") {
            if (value == "implements") window.jsmode.cx.marked = "keyword";
            return window.jsmode.cont(window.jsmode.expression, window.jsmode.classNameAfter);
        }
        if (type == "{") return window.jsmode.cont(window.jsmode.pushlex("}"), window.jsmode.classBody, window.jsmode.poplex);
    },
    classBody: function(type, value) {
        if (type == "async" ||
            (type == "variable" &&
                (value == "static" || value == "get" || value == "set") &&
                window.jsmode.cx.stream.match(/^\s+[\w$\xa1-\uffff]/, false))) {
            window.jsmode.cx.marked = "keyword";
            return window.jsmode.cont(window.jsmode.classBody);
        }
        if (type == "variable" || window.jsmode.cx.style == "keyword") {
            window.jsmode.cx.marked = "property";
            return window.jsmode.cont(window.jsmode.classfield, window.jsmode.classBody);
        }
        if (type == "number" || type == "string") return window.jsmode.cont(window.jsmode.classfield, window.jsmode.classBody);
        if (type == "[")
            return window.jsmode.cont(window.jsmode.expression, window.jsmode.maybetype, window.jsmode.expect("]"), window.jsmode.classfield, window.jsmode.classBody)
        if (value == "*") {
            window.jsmode.cx.marked = "keyword";
            return window.jsmode.cont(window.jsmode.classBody);
        }
        if (type == ";" || type == ",") return window.jsmode.cont(window.jsmode.classBody);
        if (type == "}") return window.jsmode.cont();
        if (value == "@") return window.jsmode.cont(window.jsmode.expression, window.jsmode.classBody)
    },
    classfield: function(type, value) {
        if (value == "?") return window.jsmode.cont(window.jsmode.classfield)
        if (type == ":") return window.jsmode.cont(window.jsmode.typeexpr, window.jsmode.maybeAssign)
        if (value == "=") return window.jsmode.cont(window.jsmode.expressionNoComma)
        var context = window.jsmode.cx.state.lexical.prev, isInterface = context && context.info == "interface"
        return window.jsmode.pass(isInterface ? window.jsmode.functiondecl : window.jsmode.functiondef)
    },
    afterExport: function(type, value) {
        if (value == "*") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.maybeFrom, window.jsmode.expect(";")); }
        if (value == "default") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.expression, window.jsmode.expect(";")); }
        if (type == "{") return window.jsmode.cont(window.jsmode.commasep(window.jsmode.exportField, "}"), window.jsmode.maybeFrom, window.jsmode.expect(";"));
        return window.jsmode.pass(window.jsmode.statement);
    },
    exportField: function(type, value) {
        if (value == "as") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.expect("variable")); }
        if (type == "variable") return window.jsmode.pass(window.jsmode.expressionNoComma, window.jsmode.exportField);
    },
    afterImport: function(type) {
        if (type == "string") return window.jsmode.cont();
        if (type == "(") return window.jsmode.pass(window.jsmode.expression);
        return window.jsmode.pass(window.jsmode.importSpec, window.jsmode.maybeMoreImports, window.jsmode.maybeFrom);
    },
    importSpec: function(type, value) {
        if (type == "{") return window.jsmode.contCommasep(window.jsmode.importSpec, "}");
        if (type == "variable") window.jsmode.register(value);
        if (value == "*") window.jsmode.cx.marked = "keyword";
        return window.jsmode.cont(window.jsmode.maybeAs);
    },
    maybeMoreImports: function(type) {
        if (type == ",") return window.jsmode.cont(window.jsmode.importSpec, window.jsmode.maybeMoreImports)
    },
    maybeAs: function(_type, value) {
        if (value == "as") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.importSpec); }
    },
    maybeFrom: function(_type, value) {
        if (value == "from") { window.jsmode.cx.marked = "keyword"; return window.jsmode.cont(window.jsmode.expression); }
    },
    arrayLiteral: function(type) {
        if (type == "]") return window.jsmode.cont();
        return window.jsmode.pass(window.jsmode.commasep(window.jsmode.expressionNoComma, "]"));
    },
    enumdef: function() {
        return window.jsmode.pass(window.jsmode.pushlex("form"), window.jsmode.pattern, window.jsmode.expect("{"), window.jsmode.pushlex("}"), window.jsmode.commasep(window.jsmode.enummember, "}"), window.jsmode.poplex, window.jsmode.poplex)
    },
    enummember: function() {
        return window.jsmode.pass(window.jsmode.pattern, window.jsmode.maybeAssign);
    },

    isContinuedStatement: function(state, textAfter) {
        return state.lastType == "operator" || state.lastType == "," ||
            window.jsmode.isOperatorChar.test(textAfter.charAt(0)) ||
            /[,.]/.test(textAfter.charAt(0));
    },

    expressionAllowed: function(stream, state, backUp) {
        return state.tokenize == window.jsmode.tokenBase &&
            /^(?:operator|sof|keyword [bcd]|case|new|export|default|spread|[\[{}\(,;:]|=>)$/.test(state.lastType) ||
            (state.lastType == "window.jsmode.quasi" && /\{\s*$/.test(stream.string.slice(0, stream.pos - (backUp || 0))))
    },

    // Interface

    startState: function (basecolumn) {
        return {
            tokenize: window.jsmode.tokenBase,
            lastType: "sof",
            cc: [],
            lexical: new window.jsmode.JSLexical((basecolumn || 0) - window.jsmode.indentUnit, 0, "block", false),
            localVars: {},
            context: new window.jsmode.Context(null, null, false),
            indented: basecolumn || 0
        };
    },

    token: function (stream, state) {
        if (stream.sol()) {
            if (!state.lexical.hasOwnProperty("align"))
                state.lexical.align = false;
            state.indented = stream.indentation();
            window.jsmode.findFatArrow(stream, state);
        }
        if (state.tokenize != window.jsmode.tokenComment && stream.eatSpace()) return null;
        var style = state.tokenize(stream, state);
        if (window.jsmode.type == "comment") return style;
        state.lastType = window.jsmode.type == "operator" && (window.jsmode.content == "++" || window.jsmode.content == "--") ? "incdec" : window.jsmode.type;
        return window.jsmode.parseJS(state, style, window.jsmode.type, window.jsmode.content, stream);
    },

    indent: function (state, textAfter) {
        if (state.tokenize == window.jsmode.tokenComment) return CodeMirror.Pass;
        if (state.tokenize != window.jsmode.tokenBase) return 0;
        var firstChar = textAfter && textAfter.charAt(0), lexical = state.lexical, top
        // Kludge to prevent 'maybelse' from blocking lexical scope pops
        if (!/^\s*else\b/.test(textAfter)) for (var i = state.cc.length - 1; i >= 0; --i) {
            var c = state.cc[i];
            if (c == window.jsmode.poplex) lexical = lexical.prev;
            else if (c != window.jsmode.maybeelse) break;
        }
        while ((lexical.type == "stat" || lexical.type == "form") &&
            (firstChar == "}" || ((top = state.cc[state.cc.length - 1]) &&
                (top == window.jsmode.maybeoperatorComma || top == window.jsmode.maybeoperatorNoComma) &&
                !/^[,\.=+\-*:?[\(]/.test(textAfter))))
            lexical = lexical.prev;
        if (window.jsmode.statementIndent && lexical.type == ")" && lexical.prev.type == "stat")
            lexical = lexical.prev;
        var type = lexical.type, closing = firstChar == type;

        if (type == "window.jsmode.vardef") return lexical.indented + (state.lastType == "operator" || state.lastType == "," ? lexical.info.length + 1 : 0);
        else if (type == "form" && firstChar == "{") return lexical.indented;
        else if (type == "form") return lexical.indented + window.jsmode.indentUnit;
        else if (type == "stat")
            return lexical.indented + (window.jsmode.isContinuedStatement(state, textAfter) ? window.jsmode.statementIndent || window.jsmode.indentUnit : 0);
        else if (lexical.info == "switch" && !closing)
            return lexical.indented + (/^(?:case|default)\b/.test(textAfter) ? window.jsmode.indentUnit : 2 * window.jsmode.indentUnit);
        else if (lexical.align) return lexical.column + (closing ? 0 : 1);
        else return lexical.indented + (closing ? 0 : window.jsmode.indentUnit);
    },

    electricInput: /^\s*(?:case .*?:|default:|\{|\})$/,
    blockCommentStart: "/*",
    blockCommentEnd: "*/",
    blockCommentContinue: " * ",
    lineComment: "//",
    fold: "brace",
    closeBrackets: "()[]{}''\"\"``",

    helperType: "javascript",
    jsonldMode: false,
    jsonMode: false,

    skipExpression: function (state) {
        var top = state.cc[state.cc.length - 1]
        if (top == window.jsmode.expression || top == window.jsmode.expressionNoComma) state.cc.pop()
    }
};
window.jsmode.defaultVars = new window.jsmode.Var("this", new window.jsmode.Var("arguments", null));
window.jsmode.popcontext.lex = true;
window.jsmode.poplex.lex = true;

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
                mode: window.jsmode,
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
window.wikimvceditor.easyMDE = window.wikimvceditor.getMDE(initialWikiEditMode === "script");
