(function(){function r(e,n,t){function o(i,f){if(!n[i]){if(!e[i]){var c="function"==typeof require&&require;if(!f&&c)return c(i,!0);if(u)return u(i,!0);var a=new Error("Cannot find module '"+i+"'");throw a.code="MODULE_NOT_FOUND",a}var p=n[i]={exports:{}};e[i][0].call(p.exports,function(r){var n=e[i][1][r];return o(n||r)},p,p.exports,r,e,n,t)}return n[i].exports}for(var u="function"==typeof require&&require,i=0;i<t.length;i++)o(t[i]);return o}return r})()({1:[function(require,module,exports){
"use strict";window.csharpmode={Context:function(e,n,t,o,i,r){this.indented=e,this.column=n,this.type=t,this.info=o,this.align=i,this.prev=r},pushContext:function(e,n,t,o){var i=e.indented;return e.context&&"statement"==e.context.type&&"statement"!=t&&(i=e.context.indented),e.context=new window.csharpmode.Context(i,n,t,o,null,e.context)},popContext:function(e){var n=e.context.type;return")"!=n&&"]"!=n&&"}"!=n||(e.indented=e.context.indented),e.context=e.context.prev},typeBefore:function(e,n,t){return"variable"==n.prevToken||"type"==n.prevToken||(!!/\S(?:[^- ]>|[*\]])\s*$|\*$/.test(e.string.slice(0,t))||(!(!n.typeAtEndOfLine||e.column()!=e.indentation())||void 0))},isTopScope:function(e){for(;;){if(!e||"top"==e.type)return!0;if("}"==e.type&&"namespace"!=e.prev.info)return!1;e=e.prev}},words:function(e){for(var n={},t=e.split(" "),o=0;o<t.length;++o)n[t[o]]=!0;return n},contains:function(e,n){return"function"==typeof e?e(n):e.propertyIsEnumerable(n)},indentUnit:4,statementIndentUnit:4,hooks:{"@":function(e,n){return e.eat('"')?(n.tokenize=window.csharpmode.tokenAtString,window.csharpmode.tokenAtString(e,n)):(e.eatWhile(/[\w\$_]/),"meta")}},curPunc:null,isDefKeyword:null,tokenAtString:function(e,n){for(var t;null!=(t=e.next());)if('"'==t&&!e.eat('"')){n.tokenize=null;break}return"string"},tokenString:function(e){return function(n,t){for(var o,i=!1,r=!1;null!=(o=n.next());){if(o==e&&!i){r=!0;break}i=!i&&"\\"==o}return!r&&i||(t.tokenize=null),"string"}},tokenComment:function(e,n){for(var t,o=!1;t=e.next();){if("/"==t&&o){n.tokenize=null;break}o="*"==t}return"comment"},tokenBase:function(e,n){var t=e.next();if(window.csharpmode.hooks[t]){var o=window.csharpmode.hooks[t](e,n);if(!1!==o)return o}if('"'==t||"'"==t)return n.tokenize=window.csharpmode.tokenString(t),n.tokenize(e,n);if(/[\[\]{}\(\),;\:\.]/.test(t))return window.csharpmode.curPunc=t,null;if(/[\d\.]/.test(t)){if(e.backUp(1),e.match(/^(?:0x[a-f\d]+|0b[01]+|(?:\d+\.?\d*|\.\d+)(?:e[-+]?\d+)?)(u|ll?|l|f)?/i))return"number";e.next()}if("/"==t){if(e.eat("*"))return n.tokenize=window.csharpmode.tokenComment,window.csharpmode.tokenComment(e,n);if(e.eat("/"))return e.skipToEnd(),"comment"}var i=/[+\-*&%=<>!?|\/]/;if(i.test(t)){for(;!e.match(/^\/[\/*]/,!1)&&e.eat(i););return"operator"}e.eatWhile(/[\w\$_\xa1-\uffff]/);var r=e.current();return window.csharpmode.contains(window.csharpmode.keywords,r)?(window.csharpmode.contains(window.csharpmode.blockKeywords,r)&&(window.csharpmode.curPunc="newstatement"),window.csharpmode.contains(window.csharpmode.defKeywords,r)&&(window.csharpmode.isDefKeyword=!0),"keyword"):window.csharpmode.contains(window.csharpmode.types,r)?"type":window.csharpmode.contains(window.csharpmode.atoms,r)?"atom":"variable"},maybeEOL:function(e,n){e.eol()&&window.csharpmode.isTopScope(n.context)&&(n.typeAtEndOfLine=window.csharpmode.typeBefore(e,n,e.pos))},startState:function(e){return{tokenize:null,context:new window.csharpmode.Context((e||0)-window.csharpmode.indentUnit,0,"top",null,!1),indented:0,startOfLine:!0,prevToken:null}},token:function(e,n){var t=n.context;if(e.sol()&&(null==t.align&&(t.align=!1),n.indented=e.indentation(),n.startOfLine=!0),e.eatSpace())return window.csharpmode.maybeEOL(e,n),null;window.csharpmode.curPunc=window.csharpmode.isDefKeyword=null;var o=(n.tokenize||window.csharpmode.tokenBase)(e,n);if("comment"==o||"meta"==o)return o;if(null==t.align&&(t.align=!0),";"==window.csharpmode.curPunc||":"==window.csharpmode.curPunc||","==window.csharpmode.curPunc&&e.match(/^\s*(?:\/\/.*)?$/,!1))for(;"statement"==n.context.type;)window.csharpmode.popContext(n);else if("{"==window.csharpmode.curPunc)window.csharpmode.pushContext(n,e.column(),"}");else if("["==window.csharpmode.curPunc)window.csharpmode.pushContext(n,e.column(),"]");else if("("==window.csharpmode.curPunc)window.csharpmode.pushContext(n,e.column(),")");else if("}"==window.csharpmode.curPunc){for(;"statement"==t.type;)t=window.csharpmode.popContext(n);for("}"==t.type&&(t=window.csharpmode.popContext(n));"statement"==t.type;)t=window.csharpmode.popContext(n)}else window.csharpmode.curPunc==t.type?window.csharpmode.popContext(n):(("}"==t.type||"top"==t.type)&&";"!=window.csharpmode.curPunc||"statement"==t.type&&"newstatement"==window.csharpmode.curPunc)&&window.csharpmode.pushContext(n,e.column(),"statement",e.current());return"variable"==o&&("def"==n.prevToken||window.csharpmode.typeBefore(e,n,e.start)&&window.csharpmode.isTopScope(n.context)&&e.match(/^\s*\(/,!1))&&(o="def"),n.startOfLine=!1,n.prevToken=window.csharpmode.isDefKeyword?"def":o||window.csharpmode.curPunc,window.csharpmode.maybeEOL(e,n),o},indent:function(e,n){if(e.tokenize!=window.csharpmode.tokenBase&&null!=e.tokenize||e.typeAtEndOfLine)return CodeMirror.Pass;var t=e.context,o=n&&n.charAt(0),i=o==t.type;"statement"==t.type&&"}"==o&&(t=t.prev);var r=t.prev&&"switch"==t.prev.info;if(/[{(]/.test(o)){for(;"top"!=t.type&&"}"!=t.type;)t=t.prev;return t.indented}return"statement"==t.type?t.indented+("{"==o?0:window.csharpmode.statementIndentUnit):t.align?t.column+(i?0:1):")"!=t.type||i?t.indented+(i?0:window.csharpmode.indentUnit)+(i||!r||/^(?:case|default)\b/.test(n)?0:window.csharpmode.indentUnit):t.indented+window.csharpmode.statementIndentUnit},electricInput:/^\s*(?:case .*?:|default:|\{\}?|\})$/,blockCommentStart:"/*",blockCommentEnd:"*/",blockCommentContinue:" * ",lineComment:"//",fold:"brace"},window.csharpmode.keywords=window.csharpmode.words("abstract as async await base break case catch checked class const continue default delegate do else enum event explicit extern finally fixed for foreach goto if implicit in interface internal is lock namespace new operator out override params private protected public readonly ref return sealed sizeof stackalloc static struct switch this throw try typeof unchecked unsafe using virtual void volatile while add alias ascending descending dynamic from get global group into join let orderby partial remove select set value var yield"),window.csharpmode.types=window.csharpmode.words("Action Boolean Byte Char DateTime DateTimeOffset Decimal Double Func Guid Int16 Int32 Int64 Object SByte Single String Task TimeSpan UInt16 UInt32 UInt64 bool byte char decimal double short int long object sbyte float string ushort uint ulong ASCIIEncoding Decoder Encoder Encoding StringBuilder UnicodeEncoding UTF32Encoding UTF7Encoding UTF8Encoding Comparer Dictionary EqualityComparer HashSet KeyValuePair LinkedList LinkedListNode List Queue ReferenceEqualityComparer SortedDictionary SortedList SortedSet Stack ICollection IComparer IDictionary IEnumerable IEnumerator IEqualityComparer IList IReadOnlyCollection IReadOnlyDictionary IReadOnlyList IReadOnlySet ISet Enumerable "),window.csharpmode.blockKeywords=window.csharpmode.words("catch class do else finally for foreach if struct switch try while"),window.csharpmode.defKeywords=window.csharpmode.words("class interface namespace struct var"),window.csharpmode.atoms=window.csharpmode.words("true false null"),window.wikimvceditor={currentMode:0,getMDE:function(e){var n={autosave:{enabled:!0,uniqueId:"wiki-editor-".concat(wikiItemId)},element:document.getElementById("Markdown"),indentWithTabs:!1,initialValue:initialWikiEditValue||"",placeholder:"Enter your content here, in markdown format.",tabSize:4,toolbar:["bold","italic","|","heading-1","heading-2","heading-3","|","unordered-list","ordered-list","|","link","image","|",{name:"mode",action:function(){0===window.wikimvceditor.currentMode?(window.wikimvceditor.easyMDE.toTextArea(),window.wikimvceditor.easyMDE=window.wikimvceditor.getMDE(!0),window.wikimvceditor.currentMode=1):(window.wikimvceditor.easyMDE.toTextArea(),window.wikimvceditor.easyMDE=window.wikimvceditor.getMDE(),window.wikimvceditor.currentMode=0)},className:"fa fa-code",title:"Toggle script mode"}]};e&&(n.overlayMode={mode:window.csharpmode,combine:!1},n.theme="vscode-dark");var t=new EasyMDE(n);return t.codemirror.addKeyMap({Home:"goLineLeft",End:"goLineRight"}),t},toggleOwnerSelf:function(e){if(!((e=e||window.event).isComposing||229===e.keyCode||e.key&&"Enter"!==e.key)){var n=document.getElementById("Owner");if(n){var t=document.getElementById("OwnerSelf");t&&(t.checked&&(n.value=""),n.readOnly=t.checked)}}},toggleEditorSelf:function(e){if(!((e=e||window.event).isComposing||229===e.keyCode||e.key&&"Enter"!==e.key)){var n=document.getElementById("AllowedEditors");if(n){var t=document.getElementById("EditorSelf");t&&(t.checked&&(n.value=""),n.readOnly=t.checked)}}},toggleViewerSelf:function(e){if(!((e=e||window.event).isComposing||229===e.keyCode||e.key&&"Enter"!==e.key)){var n=document.getElementById("AllowedViewers");if(n){var t=document.getElementById("ViewerSelf");t&&(t.checked&&(n.value=""),n.readOnly=t.checked)}}},redirectCheck:function(){var e=document.getElementById("wiki-edit-redirect");if(e){var n=document.getElementById("Title");n&&(n.dataset.original&&n.value!=n.dataset.original?e.style.display="block":e.style.display="none")}},toggleTransclusions:function(e){if(!((e=e||window.event).isComposing||229===e.keyCode||e.key&&"Enter"!==e.key)){var n=document.getElementById("wiki-edit-transclusionlist");if(n){var t=document.getElementById("wiki-edit-transclusions-toggler");t&&(window.wikimvc.editTransclusionsShown=!window.wikimvc.editTransclusionsShown,t.classList.toggle("is-active"),window.wikimvc.editTransclusionsShown?n.style.display="block":n.style.display="none")}}},delete:function(e){if(!((e=e||window.event).isComposing||229===e.keyCode||e.key&&"Enter"!==e.key)){var n=document.getElementById("wiki-edit-confirmed-delete-button");if(n){var t=document.getElementById("wiki-edit-delete-button");t&&(t.style.display="none",n.style.display="inline-block")}}}},window.wikimvceditor.easyMDE=window.wikimvceditor.getMDE();

},{}]},{},[1]);
