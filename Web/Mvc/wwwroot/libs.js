/**
 * easymde v2.12.0
 * Copyright Jeroen Akkerman
 * @link https://github.com/ionaru/easy-markdown-editor
 * @license MIT
 */
!function(e){if("object"==typeof exports&&"undefined"!=typeof module)module.exports=e();else if("function"==typeof define&&define.amd)define([],e);else{("undefined"!=typeof window?window:"undefined"!=typeof global?global:"undefined"!=typeof self?self:this).EasyMDE=e()}}((function(){return function e(t,n,r){function i(a,l){if(!n[a]){if(!t[a]){var s="function"==typeof require&&require;if(!l&&s)return s(a,!0);if(o)return o(a,!0);var c=new Error("Cannot find module '"+a+"'");throw c.code="MODULE_NOT_FOUND",c}var u=n[a]={exports:{}};t[a][0].call(u.exports,(function(e){return i(t[a][1][e]||e)}),u,u.exports,e,t,n,r)}return n[a].exports}for(var o="function"==typeof require&&require,a=0;a<r.length;a++)i(r[a]);return i}({1:[function(e,t,n){},{}],2:[function(e,t,n){"use strict";var r=e("typo-js");function i(e){"function"==typeof(e=e||{}).codeMirrorInstance&&"function"==typeof e.codeMirrorInstance.defineMode?(String.prototype.includes||(String.prototype.includes=function(){return-1!==String.prototype.indexOf.apply(this,arguments)}),e.codeMirrorInstance.defineMode("spell-checker",(function(t){if(!i.aff_loading){i.aff_loading=!0;var n=new XMLHttpRequest;n.open("GET","https://cdn.jsdelivr.net/codemirror.spell-checker/latest/en_US.aff",!0),n.onload=function(){4===n.readyState&&200===n.status&&(i.aff_data=n.responseText,i.num_loaded++,2==i.num_loaded&&(i.typo=new r("en_US",i.aff_data,i.dic_data,{platform:"any"})))},n.send(null)}if(!i.dic_loading){i.dic_loading=!0;var o=new XMLHttpRequest;o.open("GET","https://cdn.jsdelivr.net/codemirror.spell-checker/latest/en_US.dic",!0),o.onload=function(){4===o.readyState&&200===o.status&&(i.dic_data=o.responseText,i.num_loaded++,2==i.num_loaded&&(i.typo=new r("en_US",i.aff_data,i.dic_data,{platform:"any"})))},o.send(null)}var a='!"#$%&()*+,-./:;<=>?@[\\]^_`{|}~ ',l={token:function(e){var t=e.peek(),n="";if(a.includes(t))return e.next(),null;for(;null!=(t=e.peek())&&!a.includes(t);)n+=t,e.next();return i.typo&&!i.typo.check(n)?"spell-error":null}},s=e.codeMirrorInstance.getMode(t,t.backdrop||"text/plain");return e.codeMirrorInstance.overlayMode(s,l,!0)}))):console.log("CodeMirror Spell Checker: You must provide an instance of CodeMirror via the option `codeMirrorInstance`")}i.num_loaded=0,i.aff_loading=!1,i.dic_loading=!1,i.aff_data="",i.dic_data="",i.typo,t.exports=i},{"typo-js":15}],3:[function(e,t,n){(function(e){"use strict";e.defineOption("fullScreen",!1,(function(t,n,r){r==e.Init&&(r=!1),!r!=!n&&(n?function(e){var t=e.getWrapperElement();e.state.fullScreenRestore={scrollTop:window.pageYOffset,scrollLeft:window.pageXOffset,width:t.style.width,height:t.style.height},t.style.width="",t.style.height="auto",t.className+=" CodeMirror-fullscreen",document.documentElement.style.overflow="hidden",e.refresh()}(t):function(e){var t=e.getWrapperElement();t.className=t.className.replace(/\s*CodeMirror-fullscreen\b/,""),document.documentElement.style.overflow="";var n=e.state.fullScreenRestore;t.style.width=n.width,t.style.height=n.height,window.scrollTo(n.scrollLeft,n.scrollTop),e.refresh()}(t))}))})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],4:[function(e,t,n){(function(e){function t(e){e.state.placeholder&&(e.state.placeholder.parentNode.removeChild(e.state.placeholder),e.state.placeholder=null)}function n(e){t(e);var n=e.state.placeholder=document.createElement("pre");n.style.cssText="height: 0; overflow: visible",n.style.direction=e.getOption("direction"),n.className="CodeMirror-placeholder CodeMirror-line-like";var r=e.getOption("placeholder");"string"==typeof r&&(r=document.createTextNode(r)),n.appendChild(r),e.display.lineSpace.insertBefore(n,e.display.lineSpace.firstChild)}function r(e){o(e)&&n(e)}function i(e){var r=e.getWrapperElement(),i=o(e);r.className=r.className.replace(" CodeMirror-empty","")+(i?" CodeMirror-empty":""),i?n(e):t(e)}function o(e){return 1===e.lineCount()&&""===e.getLine(0)}e.defineOption("placeholder","",(function(o,a,l){var s=l&&l!=e.Init;if(a&&!s)o.on("blur",r),o.on("change",i),o.on("swapDoc",i),e.on(o.getInputField(),"compositionupdate",o.state.placeholderCompose=function(){!function(e){setTimeout((function(){var r=!1,i=e.getInputField();"TEXTAREA"==i.nodeName?r=!i.value:1==e.lineCount()&&(r=!/[^\u200b]/.test(i.querySelector(".CodeMirror-line").textContent)),r?n(e):t(e)}),20)}(o)}),i(o);else if(!a&&s){o.off("blur",r),o.off("change",i),o.off("swapDoc",i),e.off(o.getInputField(),"compositionupdate",o.state.placeholderCompose),t(o);var c=o.getWrapperElement();c.className=c.className.replace(" CodeMirror-empty","")}a&&!o.hasFocus()&&r(o)}))})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],5:[function(e,t,n){(function(e){"use strict";var t=/^(\s*)(>[> ]*|[*+-] \[[x ]\]\s|[*+-]\s|(\d+)([.)]))(\s*)/,n=/^(\s*)(>[> ]*|[*+-] \[[x ]\]|[*+-]|(\d+)[.)])(\s*)$/,r=/[*+-]\s/;function i(e,n){var r=n.line,i=0,o=0,a=t.exec(e.getLine(r)),l=a[1];do{var s=r+(i+=1),c=e.getLine(s),u=t.exec(c);if(u){var d=u[1],h=parseInt(a[3],10)+i-o,f=parseInt(u[3],10),p=f;if(l!==d||isNaN(f)){if(l.length>d.length)return;if(l.length<d.length&&1===i)return;o+=1}else h===f&&(p=f+1),h>f&&(p=h+1),e.replaceRange(c.replace(t,d+p+u[4]+u[5]),{line:s,ch:0},{line:s,ch:c.length})}}while(u)}e.commands.newlineAndIndentContinueMarkdownList=function(o){if(o.getOption("disableInput"))return e.Pass;for(var a=o.listSelections(),l=[],s=0;s<a.length;s++){var c=a[s].head,u=o.getStateAfter(c.line),d=e.innerMode(o.getMode(),u);if("markdown"!==d.mode.name)return void o.execCommand("newlineAndIndent");var h=!1!==(u=d.state).list,f=0!==u.quote,p=o.getLine(c.line),m=t.exec(p),g=/^\s*$/.test(p.slice(0,c.ch));if(!a[s].empty()||!h&&!f||!m||g)return void o.execCommand("newlineAndIndent");if(n.test(p)){var v=f&&/>\s*$/.test(p),x=!/>\s*$/.test(p);(v||x)&&o.replaceRange("",{line:c.line,ch:0},{line:c.line,ch:c.ch+1}),l[s]="\n"}else{var y=m[1],b=m[5],w=!(r.test(m[2])||m[2].indexOf(">")>=0),k=w?parseInt(m[3],10)+1+m[4]:m[2].replace("x"," ");l[s]="\n"+y+k+b,w&&i(o,c)}}o.replaceSelections(l)}})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],6:[function(e,t,n){(function(e){"use strict";e.overlayMode=function(t,n,r){return{startState:function(){return{base:e.startState(t),overlay:e.startState(n),basePos:0,baseCur:null,overlayPos:0,overlayCur:null,streamSeen:null}},copyState:function(r){return{base:e.copyState(t,r.base),overlay:e.copyState(n,r.overlay),basePos:r.basePos,baseCur:null,overlayPos:r.overlayPos,overlayCur:null}},token:function(e,i){return(e!=i.streamSeen||Math.min(i.basePos,i.overlayPos)<e.start)&&(i.streamSeen=e,i.basePos=i.overlayPos=e.start),e.start==i.basePos&&(i.baseCur=t.token(e,i.base),i.basePos=e.pos),e.start==i.overlayPos&&(e.pos=e.start,i.overlayCur=n.token(e,i.overlay),i.overlayPos=e.pos),e.pos=Math.min(i.basePos,i.overlayPos),null==i.overlayCur?i.baseCur:null!=i.baseCur&&i.overlay.combineTokens||r&&null==i.overlay.combineTokens?i.baseCur+" "+i.overlayCur:i.overlayCur},indent:t.indent&&function(e,n,r){return t.indent(e.base,n,r)},electricChars:t.electricChars,innerMode:function(e){return{state:e.base,mode:t}},blankLine:function(e){var i,o;return t.blankLine&&(i=t.blankLine(e.base)),n.blankLine&&(o=n.blankLine(e.overlay)),null==o?i:r&&null!=i?i+" "+o:o}}}})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],7:[function(e,t,n){(function(e){"use strict";var t,n,r=e.Pos;function i(e,t){for(var n=function(e){var t=e.flags;return null!=t?t:(e.ignoreCase?"i":"")+(e.global?"g":"")+(e.multiline?"m":"")}(e),r=n,i=0;i<t.length;i++)-1==r.indexOf(t.charAt(i))&&(r+=t.charAt(i));return n==r?e:new RegExp(e.source,r)}function o(e){return/\\s|\\n|\n|\\W|\\D|\[\^/.test(e.source)}function a(e,t,n){t=i(t,"g");for(var o=n.line,a=n.ch,l=e.lastLine();o<=l;o++,a=0){t.lastIndex=a;var s=e.getLine(o),c=t.exec(s);if(c)return{from:r(o,c.index),to:r(o,c.index+c[0].length),match:c}}}function l(e,t,n){if(!o(t))return a(e,t,n);t=i(t,"gm");for(var l,s=1,c=n.line,u=e.lastLine();c<=u;){for(var d=0;d<s&&!(c>u);d++){var h=e.getLine(c++);l=null==l?h:l+"\n"+h}s*=2,t.lastIndex=n.ch;var f=t.exec(l);if(f){var p=l.slice(0,f.index).split("\n"),m=f[0].split("\n"),g=n.line+p.length-1,v=p[p.length-1].length;return{from:r(g,v),to:r(g+m.length-1,1==m.length?v+m[0].length:m[m.length-1].length),match:f}}}}function s(e,t,n){for(var r,i=0;i<=e.length;){t.lastIndex=i;var o=t.exec(e);if(!o)break;var a=o.index+o[0].length;if(a>e.length-n)break;(!r||a>r.index+r[0].length)&&(r=o),i=o.index+1}return r}function c(e,t,n){t=i(t,"g");for(var o=n.line,a=n.ch,l=e.firstLine();o>=l;o--,a=-1){var c=e.getLine(o),u=s(c,t,a<0?0:c.length-a);if(u)return{from:r(o,u.index),to:r(o,u.index+u[0].length),match:u}}}function u(e,t,n){if(!o(t))return c(e,t,n);t=i(t,"gm");for(var a,l=1,u=e.getLine(n.line).length-n.ch,d=n.line,h=e.firstLine();d>=h;){for(var f=0;f<l&&d>=h;f++){var p=e.getLine(d--);a=null==a?p:p+"\n"+a}l*=2;var m=s(a,t,u);if(m){var g=a.slice(0,m.index).split("\n"),v=m[0].split("\n"),x=d+g.length,y=g[g.length-1].length;return{from:r(x,y),to:r(x+v.length-1,1==v.length?y+v[0].length:v[v.length-1].length),match:m}}}}function d(e,t,n,r){if(e.length==t.length)return n;for(var i=0,o=n+Math.max(0,e.length-t.length);;){if(i==o)return i;var a=i+o>>1,l=r(e.slice(0,a)).length;if(l==n)return a;l>n?o=a:i=a+1}}function h(e,i,o,a){if(!i.length)return null;var l=a?t:n,s=l(i).split(/\r|\n\r?/);e:for(var c=o.line,u=o.ch,h=e.lastLine()+1-s.length;c<=h;c++,u=0){var f=e.getLine(c).slice(u),p=l(f);if(1==s.length){var m=p.indexOf(s[0]);if(-1==m)continue e;return o=d(f,p,m,l)+u,{from:r(c,d(f,p,m,l)+u),to:r(c,d(f,p,m+s[0].length,l)+u)}}var g=p.length-s[0].length;if(p.slice(g)==s[0]){for(var v=1;v<s.length-1;v++)if(l(e.getLine(c+v))!=s[v])continue e;var x=e.getLine(c+s.length-1),y=l(x),b=s[s.length-1];if(y.slice(0,b.length)==b)return{from:r(c,d(f,p,g,l)+u),to:r(c+s.length-1,d(x,y,b.length,l))}}}}function f(e,i,o,a){if(!i.length)return null;var l=a?t:n,s=l(i).split(/\r|\n\r?/);e:for(var c=o.line,u=o.ch,h=e.firstLine()-1+s.length;c>=h;c--,u=-1){var f=e.getLine(c);u>-1&&(f=f.slice(0,u));var p=l(f);if(1==s.length){var m=p.lastIndexOf(s[0]);if(-1==m)continue e;return{from:r(c,d(f,p,m,l)),to:r(c,d(f,p,m+s[0].length,l))}}var g=s[s.length-1];if(p.slice(0,g.length)==g){var v=1;for(o=c-s.length+1;v<s.length-1;v++)if(l(e.getLine(o+v))!=s[v])continue e;var x=e.getLine(c+1-s.length),y=l(x);if(y.slice(y.length-s[0].length)==s[0])return{from:r(c+1-s.length,d(x,y,x.length-s[0].length,l)),to:r(c,d(f,p,g.length,l))}}}}function p(e,t,n,o){var s;this.atOccurrence=!1,this.doc=e,n=n?e.clipPos(n):r(0,0),this.pos={from:n,to:n},"object"==typeof o?s=o.caseFold:(s=o,o=null),"string"==typeof t?(null==s&&(s=!1),this.matches=function(n,r){return(n?f:h)(e,t,r,s)}):(t=i(t,"gm"),o&&!1===o.multiline?this.matches=function(n,r){return(n?c:a)(e,t,r)}:this.matches=function(n,r){return(n?u:l)(e,t,r)})}String.prototype.normalize?(t=function(e){return e.normalize("NFD").toLowerCase()},n=function(e){return e.normalize("NFD")}):(t=function(e){return e.toLowerCase()},n=function(e){return e}),p.prototype={findNext:function(){return this.find(!1)},findPrevious:function(){return this.find(!0)},find:function(t){for(var n=this.matches(t,this.doc.clipPos(t?this.pos.from:this.pos.to));n&&0==e.cmpPos(n.from,n.to);)t?n.from.ch?n.from=r(n.from.line,n.from.ch-1):n=n.from.line==this.doc.firstLine()?null:this.matches(t,this.doc.clipPos(r(n.from.line-1))):n.to.ch<this.doc.getLine(n.to.line).length?n.to=r(n.to.line,n.to.ch+1):n=n.to.line==this.doc.lastLine()?null:this.matches(t,r(n.to.line+1,0));if(n)return this.pos=n,this.atOccurrence=!0,this.pos.match||!0;var i=r(t?this.doc.firstLine():this.doc.lastLine()+1,0);return this.pos={from:i,to:i},this.atOccurrence=!1},from:function(){if(this.atOccurrence)return this.pos.from},to:function(){if(this.atOccurrence)return this.pos.to},replace:function(t,n){if(this.atOccurrence){var i=e.splitLines(t);this.doc.replaceRange(i,this.pos.from,this.pos.to,n),this.pos.to=r(this.pos.from.line+i.length-1,i[i.length-1].length+(1==i.length?this.pos.from.ch:0))}}},e.defineExtension("getSearchCursor",(function(e,t,n){return new p(this.doc,e,t,n)})),e.defineDocExtension("getSearchCursor",(function(e,t,n){return new p(this,e,t,n)})),e.defineExtension("selectMatches",(function(t,n){for(var r=[],i=this.getSearchCursor(t,this.getCursor("from"),n);i.findNext()&&!(e.cmpPos(i.to(),this.getCursor("to"))>0);)r.push({anchor:i.from(),head:i.to()});r.length&&this.setSelections(r,0)}))})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],8:[function(e,t,n){(function(e){"use strict";function t(e){e.state.markedSelection&&e.operation((function(){!function(e){if(!e.somethingSelected())return a(e);if(e.listSelections().length>1)return l(e);var t=e.getCursor("start"),n=e.getCursor("end"),r=e.state.markedSelection;if(!r.length)return o(e,t,n);var s=r[0].find(),c=r[r.length-1].find();if(!s||!c||n.line-t.line<=8||i(t,c.to)>=0||i(n,s.from)<=0)return l(e);for(;i(t,s.from)>0;)r.shift().clear(),s=r[0].find();for(i(t,s.from)<0&&(s.to.line-t.line<8?(r.shift().clear(),o(e,t,s.to,0)):o(e,t,s.from,0));i(n,c.to)<0;)r.pop().clear(),c=r[r.length-1].find();i(n,c.to)>0&&(n.line-c.from.line<8?(r.pop().clear(),o(e,c.from,n)):o(e,c.to,n))}(e)}))}function n(e){e.state.markedSelection&&e.state.markedSelection.length&&e.operation((function(){a(e)}))}e.defineOption("styleSelectedText",!1,(function(r,i,o){var s=o&&o!=e.Init;i&&!s?(r.state.markedSelection=[],r.state.markedSelectionStyle="string"==typeof i?i:"CodeMirror-selectedtext",l(r),r.on("cursorActivity",t),r.on("change",n)):!i&&s&&(r.off("cursorActivity",t),r.off("change",n),a(r),r.state.markedSelection=r.state.markedSelectionStyle=null)}));var r=e.Pos,i=e.cmpPos;function o(e,t,n,o){if(0!=i(t,n))for(var a=e.state.markedSelection,l=e.state.markedSelectionStyle,s=t.line;;){var c=s==t.line?t:r(s,0),u=s+8,d=u>=n.line,h=d?n:r(u,0),f=e.markText(c,h,{className:l});if(null==o?a.push(f):a.splice(o++,0,f),d)break;s=u}}function a(e){for(var t=e.state.markedSelection,n=0;n<t.length;++n)t[n].clear();t.length=0}function l(e){a(e);for(var t=e.listSelections(),n=0;n<t.length;n++)o(e,t[n].from(),t[n].to())}})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],9:[function(e,t,n){!function(e,r){"object"==typeof n&&void 0!==t?t.exports=r():(e=e||self).CodeMirror=r()}(this,(function(){"use strict";var e=navigator.userAgent,t=navigator.platform,n=/gecko\/\d/i.test(e),r=/MSIE \d/.test(e),i=/Trident\/(?:[7-9]|\d{2,})\..*rv:(\d+)/.exec(e),o=/Edge\/(\d+)/.exec(e),a=r||i||o,l=a&&(r?document.documentMode||6:+(o||i)[1]),s=!o&&/WebKit\//.test(e),c=s&&/Qt\/\d+\.\d+/.test(e),u=!o&&/Chrome\//.test(e),d=/Opera\//.test(e),h=/Apple Computer/.test(navigator.vendor),f=/Mac OS X 1\d\D([8-9]|\d\d)\D/.test(e),p=/PhantomJS/.test(e),m=!o&&/AppleWebKit/.test(e)&&/Mobile\/\w+/.test(e),g=/Android/.test(e),v=m||g||/webOS|BlackBerry|Opera Mini|Opera Mobi|IEMobile/i.test(e),x=m||/Mac/.test(t),y=/\bCrOS\b/.test(e),b=/win/i.test(t),w=d&&e.match(/Version\/(\d*\.\d*)/);w&&(w=Number(w[1])),w&&w>=15&&(d=!1,s=!0);var k=x&&(c||d&&(null==w||w<12.11)),C=n||a&&l>=9;function S(e){return new RegExp("(^|\\s)"+e+"(?:$|\\s)\\s*")}var L,T=function(e,t){var n=e.className,r=S(t).exec(n);if(r){var i=n.slice(r.index+r[0].length);e.className=n.slice(0,r.index)+(i?r[1]+i:"")}};function M(e){for(var t=e.childNodes.length;t>0;--t)e.removeChild(e.firstChild);return e}function A(e,t){return M(e).appendChild(t)}function N(e,t,n,r){var i=document.createElement(e);if(n&&(i.className=n),r&&(i.style.cssText=r),"string"==typeof t)i.appendChild(document.createTextNode(t));else if(t)for(var o=0;o<t.length;++o)i.appendChild(t[o]);return i}function D(e,t,n,r){var i=N(e,t,n,r);return i.setAttribute("role","presentation"),i}function F(e,t){if(3==t.nodeType&&(t=t.parentNode),e.contains)return e.contains(t);do{if(11==t.nodeType&&(t=t.host),t==e)return!0}while(t=t.parentNode)}function E(){var e;try{e=document.activeElement}catch(t){e=document.body||null}for(;e&&e.shadowRoot&&e.shadowRoot.activeElement;)e=e.shadowRoot.activeElement;return e}function O(e,t){var n=e.className;S(t).test(n)||(e.className+=(n?" ":"")+t)}function I(e,t){for(var n=e.split(" "),r=0;r<n.length;r++)n[r]&&!S(n[r]).test(t)&&(t+=" "+n[r]);return t}L=document.createRange?function(e,t,n,r){var i=document.createRange();return i.setEnd(r||e,n),i.setStart(e,t),i}:function(e,t,n){var r=document.body.createTextRange();try{r.moveToElementText(e.parentNode)}catch(e){return r}return r.collapse(!0),r.moveEnd("character",n),r.moveStart("character",t),r};var z=function(e){e.select()};function H(e){var t=Array.prototype.slice.call(arguments,1);return function(){return e.apply(null,t)}}function R(e,t,n){for(var r in t||(t={}),e)!e.hasOwnProperty(r)||!1===n&&t.hasOwnProperty(r)||(t[r]=e[r]);return t}function _(e,t,n,r,i){null==t&&-1==(t=e.search(/[^\s\u00a0]/))&&(t=e.length);for(var o=r||0,a=i||0;;){var l=e.indexOf("\t",o);if(l<0||l>=t)return a+(t-o);a+=l-o,a+=n-a%n,o=l+1}}m?z=function(e){e.selectionStart=0,e.selectionEnd=e.value.length}:a&&(z=function(e){try{e.select()}catch(e){}});var P=function(){this.id=null,this.f=null,this.time=0,this.handler=H(this.onTimeout,this)};function W(e,t){for(var n=0;n<e.length;++n)if(e[n]==t)return n;return-1}P.prototype.onTimeout=function(e){e.id=0,e.time<=+new Date?e.f():setTimeout(e.handler,e.time-+new Date)},P.prototype.set=function(e,t){this.f=t;var n=+new Date+e;(!this.id||n<this.time)&&(clearTimeout(this.id),this.id=setTimeout(this.handler,e),this.time=n)};var B={toString:function(){return"CodeMirror.Pass"}},j={scroll:!1},q={origin:"*mouse"},U={origin:"+move"};function $(e,t,n){for(var r=0,i=0;;){var o=e.indexOf("\t",r);-1==o&&(o=e.length);var a=o-r;if(o==e.length||i+a>=t)return r+Math.min(a,t-i);if(i+=o-r,r=o+1,(i+=n-i%n)>=t)return r}}var G=[""];function V(e){for(;G.length<=e;)G.push(K(G)+" ");return G[e]}function K(e){return e[e.length-1]}function X(e,t){for(var n=[],r=0;r<e.length;r++)n[r]=t(e[r],r);return n}function Z(){}function Y(e,t){var n;return Object.create?n=Object.create(e):(Z.prototype=e,n=new Z),t&&R(t,n),n}var Q=/[\u00df\u0587\u0590-\u05f4\u0600-\u06ff\u3040-\u309f\u30a0-\u30ff\u3400-\u4db5\u4e00-\u9fcc\uac00-\ud7af]/;function J(e){return/\w/.test(e)||e>""&&(e.toUpperCase()!=e.toLowerCase()||Q.test(e))}function ee(e,t){return t?!!(t.source.indexOf("\\w")>-1&&J(e))||t.test(e):J(e)}function te(e){for(var t in e)if(e.hasOwnProperty(t)&&e[t])return!1;return!0}var ne=/[\u0300-\u036f\u0483-\u0489\u0591-\u05bd\u05bf\u05c1\u05c2\u05c4\u05c5\u05c7\u0610-\u061a\u064b-\u065e\u0670\u06d6-\u06dc\u06de-\u06e4\u06e7\u06e8\u06ea-\u06ed\u0711\u0730-\u074a\u07a6-\u07b0\u07eb-\u07f3\u0816-\u0819\u081b-\u0823\u0825-\u0827\u0829-\u082d\u0900-\u0902\u093c\u0941-\u0948\u094d\u0951-\u0955\u0962\u0963\u0981\u09bc\u09be\u09c1-\u09c4\u09cd\u09d7\u09e2\u09e3\u0a01\u0a02\u0a3c\u0a41\u0a42\u0a47\u0a48\u0a4b-\u0a4d\u0a51\u0a70\u0a71\u0a75\u0a81\u0a82\u0abc\u0ac1-\u0ac5\u0ac7\u0ac8\u0acd\u0ae2\u0ae3\u0b01\u0b3c\u0b3e\u0b3f\u0b41-\u0b44\u0b4d\u0b56\u0b57\u0b62\u0b63\u0b82\u0bbe\u0bc0\u0bcd\u0bd7\u0c3e-\u0c40\u0c46-\u0c48\u0c4a-\u0c4d\u0c55\u0c56\u0c62\u0c63\u0cbc\u0cbf\u0cc2\u0cc6\u0ccc\u0ccd\u0cd5\u0cd6\u0ce2\u0ce3\u0d3e\u0d41-\u0d44\u0d4d\u0d57\u0d62\u0d63\u0dca\u0dcf\u0dd2-\u0dd4\u0dd6\u0ddf\u0e31\u0e34-\u0e3a\u0e47-\u0e4e\u0eb1\u0eb4-\u0eb9\u0ebb\u0ebc\u0ec8-\u0ecd\u0f18\u0f19\u0f35\u0f37\u0f39\u0f71-\u0f7e\u0f80-\u0f84\u0f86\u0f87\u0f90-\u0f97\u0f99-\u0fbc\u0fc6\u102d-\u1030\u1032-\u1037\u1039\u103a\u103d\u103e\u1058\u1059\u105e-\u1060\u1071-\u1074\u1082\u1085\u1086\u108d\u109d\u135f\u1712-\u1714\u1732-\u1734\u1752\u1753\u1772\u1773\u17b7-\u17bd\u17c6\u17c9-\u17d3\u17dd\u180b-\u180d\u18a9\u1920-\u1922\u1927\u1928\u1932\u1939-\u193b\u1a17\u1a18\u1a56\u1a58-\u1a5e\u1a60\u1a62\u1a65-\u1a6c\u1a73-\u1a7c\u1a7f\u1b00-\u1b03\u1b34\u1b36-\u1b3a\u1b3c\u1b42\u1b6b-\u1b73\u1b80\u1b81\u1ba2-\u1ba5\u1ba8\u1ba9\u1c2c-\u1c33\u1c36\u1c37\u1cd0-\u1cd2\u1cd4-\u1ce0\u1ce2-\u1ce8\u1ced\u1dc0-\u1de6\u1dfd-\u1dff\u200c\u200d\u20d0-\u20f0\u2cef-\u2cf1\u2de0-\u2dff\u302a-\u302f\u3099\u309a\ua66f-\ua672\ua67c\ua67d\ua6f0\ua6f1\ua802\ua806\ua80b\ua825\ua826\ua8c4\ua8e0-\ua8f1\ua926-\ua92d\ua947-\ua951\ua980-\ua982\ua9b3\ua9b6-\ua9b9\ua9bc\uaa29-\uaa2e\uaa31\uaa32\uaa35\uaa36\uaa43\uaa4c\uaab0\uaab2-\uaab4\uaab7\uaab8\uaabe\uaabf\uaac1\uabe5\uabe8\uabed\udc00-\udfff\ufb1e\ufe00-\ufe0f\ufe20-\ufe26\uff9e\uff9f]/;function re(e){return e.charCodeAt(0)>=768&&ne.test(e)}function ie(e,t,n){for(;(n<0?t>0:t<e.length)&&re(e.charAt(t));)t+=n;return t}function oe(e,t,n){for(var r=t>n?-1:1;;){if(t==n)return t;var i=(t+n)/2,o=r<0?Math.ceil(i):Math.floor(i);if(o==t)return e(o)?t:n;e(o)?n=o:t=o+r}}var ae=null;function le(e,t,n){var r;ae=null;for(var i=0;i<e.length;++i){var o=e[i];if(o.from<t&&o.to>t)return i;o.to==t&&(o.from!=o.to&&"before"==n?r=i:ae=i),o.from==t&&(o.from!=o.to&&"before"!=n?r=i:ae=i)}return null!=r?r:ae}var se=function(){var e=/[\u0590-\u05f4\u0600-\u06ff\u0700-\u08ac]/,t=/[stwN]/,n=/[LRr]/,r=/[Lb1n]/,i=/[1n]/;function o(e,t,n){this.level=e,this.from=t,this.to=n}return function(a,l){var s="ltr"==l?"L":"R";if(0==a.length||"ltr"==l&&!e.test(a))return!1;for(var c,u=a.length,d=[],h=0;h<u;++h)d.push((c=a.charCodeAt(h))<=247?"bbbbbbbbbtstwsbbbbbbbbbbbbbbssstwNN%%%NNNNNN,N,N1111111111NNNNNNNLLLLLLLLLLLLLLLLLLLLLLLLLLNNNNNNLLLLLLLLLLLLLLLLLLLLLLLLLLNNNNbbbbbbsbbbbbbbbbbbbbbbbbbbbbbbbbb,N%%%%NNNNLNNNNN%%11NLNNN1LNNNNNLLLLLLLLLLLLLLLLLLLLLLLNLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLN".charAt(c):1424<=c&&c<=1524?"R":1536<=c&&c<=1785?"nnnnnnNNr%%r,rNNmmmmmmmmmmmrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrmmmmmmmmmmmmmmmmmmmmmnnnnnnnnnn%nnrrrmrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrmmmmmmmnNmmmmmmrrmmNmmmmrr1111111111".charAt(c-1536):1774<=c&&c<=2220?"r":8192<=c&&c<=8203?"w":8204==c?"b":"L");for(var f=0,p=s;f<u;++f){var m=d[f];"m"==m?d[f]=p:p=m}for(var g=0,v=s;g<u;++g){var x=d[g];"1"==x&&"r"==v?d[g]="n":n.test(x)&&(v=x,"r"==x&&(d[g]="R"))}for(var y=1,b=d[0];y<u-1;++y){var w=d[y];"+"==w&&"1"==b&&"1"==d[y+1]?d[y]="1":","!=w||b!=d[y+1]||"1"!=b&&"n"!=b||(d[y]=b),b=w}for(var k=0;k<u;++k){var C=d[k];if(","==C)d[k]="N";else if("%"==C){var S=void 0;for(S=k+1;S<u&&"%"==d[S];++S);for(var L=k&&"!"==d[k-1]||S<u&&"1"==d[S]?"1":"N",T=k;T<S;++T)d[T]=L;k=S-1}}for(var M=0,A=s;M<u;++M){var N=d[M];"L"==A&&"1"==N?d[M]="L":n.test(N)&&(A=N)}for(var D=0;D<u;++D)if(t.test(d[D])){var F=void 0;for(F=D+1;F<u&&t.test(d[F]);++F);for(var E="L"==(D?d[D-1]:s),O=E==("L"==(F<u?d[F]:s))?E?"L":"R":s,I=D;I<F;++I)d[I]=O;D=F-1}for(var z,H=[],R=0;R<u;)if(r.test(d[R])){var _=R;for(++R;R<u&&r.test(d[R]);++R);H.push(new o(0,_,R))}else{var P=R,W=H.length,B="rtl"==l?1:0;for(++R;R<u&&"L"!=d[R];++R);for(var j=P;j<R;)if(i.test(d[j])){P<j&&(H.splice(W,0,new o(1,P,j)),W+=B);var q=j;for(++j;j<R&&i.test(d[j]);++j);H.splice(W,0,new o(2,q,j)),W+=B,P=j}else++j;P<R&&H.splice(W,0,new o(1,P,R))}return"ltr"==l&&(1==H[0].level&&(z=a.match(/^\s+/))&&(H[0].from=z[0].length,H.unshift(new o(0,0,z[0].length))),1==K(H).level&&(z=a.match(/\s+$/))&&(K(H).to-=z[0].length,H.push(new o(0,u-z[0].length,u)))),"rtl"==l?H.reverse():H}}();function ce(e,t){var n=e.order;return null==n&&(n=e.order=se(e.text,t)),n}var ue=[],de=function(e,t,n){if(e.addEventListener)e.addEventListener(t,n,!1);else if(e.attachEvent)e.attachEvent("on"+t,n);else{var r=e._handlers||(e._handlers={});r[t]=(r[t]||ue).concat(n)}};function he(e,t){return e._handlers&&e._handlers[t]||ue}function fe(e,t,n){if(e.removeEventListener)e.removeEventListener(t,n,!1);else if(e.detachEvent)e.detachEvent("on"+t,n);else{var r=e._handlers,i=r&&r[t];if(i){var o=W(i,n);o>-1&&(r[t]=i.slice(0,o).concat(i.slice(o+1)))}}}function pe(e,t){var n=he(e,t);if(n.length)for(var r=Array.prototype.slice.call(arguments,2),i=0;i<n.length;++i)n[i].apply(null,r)}function me(e,t,n){return"string"==typeof t&&(t={type:t,preventDefault:function(){this.defaultPrevented=!0}}),pe(e,n||t.type,e,t),we(t)||t.codemirrorIgnore}function ge(e){var t=e._handlers&&e._handlers.cursorActivity;if(t)for(var n=e.curOp.cursorActivityHandlers||(e.curOp.cursorActivityHandlers=[]),r=0;r<t.length;++r)-1==W(n,t[r])&&n.push(t[r])}function ve(e,t){return he(e,t).length>0}function xe(e){e.prototype.on=function(e,t){de(this,e,t)},e.prototype.off=function(e,t){fe(this,e,t)}}function ye(e){e.preventDefault?e.preventDefault():e.returnValue=!1}function be(e){e.stopPropagation?e.stopPropagation():e.cancelBubble=!0}function we(e){return null!=e.defaultPrevented?e.defaultPrevented:0==e.returnValue}function ke(e){ye(e),be(e)}function Ce(e){return e.target||e.srcElement}function Se(e){var t=e.which;return null==t&&(1&e.button?t=1:2&e.button?t=3:4&e.button&&(t=2)),x&&e.ctrlKey&&1==t&&(t=3),t}var Le,Te,Me=function(){if(a&&l<9)return!1;var e=N("div");return"draggable"in e||"dragDrop"in e}();function Ae(e){if(null==Le){var t=N("span","​");A(e,N("span",[t,document.createTextNode("x")])),0!=e.firstChild.offsetHeight&&(Le=t.offsetWidth<=1&&t.offsetHeight>2&&!(a&&l<8))}var n=Le?N("span","​"):N("span"," ",null,"display: inline-block; width: 1px; margin-right: -1px");return n.setAttribute("cm-text",""),n}function Ne(e){if(null!=Te)return Te;var t=A(e,document.createTextNode("AخA")),n=L(t,0,1).getBoundingClientRect(),r=L(t,1,2).getBoundingClientRect();return M(e),!(!n||n.left==n.right)&&(Te=r.right-n.right<3)}var De,Fe=3!="\n\nb".split(/\n/).length?function(e){for(var t=0,n=[],r=e.length;t<=r;){var i=e.indexOf("\n",t);-1==i&&(i=e.length);var o=e.slice(t,"\r"==e.charAt(i-1)?i-1:i),a=o.indexOf("\r");-1!=a?(n.push(o.slice(0,a)),t+=a+1):(n.push(o),t=i+1)}return n}:function(e){return e.split(/\r\n?|\n/)},Ee=window.getSelection?function(e){try{return e.selectionStart!=e.selectionEnd}catch(e){return!1}}:function(e){var t;try{t=e.ownerDocument.selection.createRange()}catch(e){}return!(!t||t.parentElement()!=e)&&0!=t.compareEndPoints("StartToEnd",t)},Oe="oncopy"in(De=N("div"))||(De.setAttribute("oncopy","return;"),"function"==typeof De.oncopy),Ie=null;var ze={},He={};function Re(e,t){arguments.length>2&&(t.dependencies=Array.prototype.slice.call(arguments,2)),ze[e]=t}function _e(e){if("string"==typeof e&&He.hasOwnProperty(e))e=He[e];else if(e&&"string"==typeof e.name&&He.hasOwnProperty(e.name)){var t=He[e.name];"string"==typeof t&&(t={name:t}),(e=Y(t,e)).name=t.name}else{if("string"==typeof e&&/^[\w\-]+\/[\w\-]+\+xml$/.test(e))return _e("application/xml");if("string"==typeof e&&/^[\w\-]+\/[\w\-]+\+json$/.test(e))return _e("application/json")}return"string"==typeof e?{name:e}:e||{name:"null"}}function Pe(e,t){t=_e(t);var n=ze[t.name];if(!n)return Pe(e,"text/plain");var r=n(e,t);if(We.hasOwnProperty(t.name)){var i=We[t.name];for(var o in i)i.hasOwnProperty(o)&&(r.hasOwnProperty(o)&&(r["_"+o]=r[o]),r[o]=i[o])}if(r.name=t.name,t.helperType&&(r.helperType=t.helperType),t.modeProps)for(var a in t.modeProps)r[a]=t.modeProps[a];return r}var We={};function Be(e,t){R(t,We.hasOwnProperty(e)?We[e]:We[e]={})}function je(e,t){if(!0===t)return t;if(e.copyState)return e.copyState(t);var n={};for(var r in t){var i=t[r];i instanceof Array&&(i=i.concat([])),n[r]=i}return n}function qe(e,t){for(var n;e.innerMode&&(n=e.innerMode(t))&&n.mode!=e;)t=n.state,e=n.mode;return n||{mode:e,state:t}}function Ue(e,t,n){return!e.startState||e.startState(t,n)}var $e=function(e,t,n){this.pos=this.start=0,this.string=e,this.tabSize=t||8,this.lastColumnPos=this.lastColumnValue=0,this.lineStart=0,this.lineOracle=n};function Ge(e,t){if((t-=e.first)<0||t>=e.size)throw new Error("There is no line "+(t+e.first)+" in the document.");for(var n=e;!n.lines;)for(var r=0;;++r){var i=n.children[r],o=i.chunkSize();if(t<o){n=i;break}t-=o}return n.lines[t]}function Ve(e,t,n){var r=[],i=t.line;return e.iter(t.line,n.line+1,(function(e){var o=e.text;i==n.line&&(o=o.slice(0,n.ch)),i==t.line&&(o=o.slice(t.ch)),r.push(o),++i})),r}function Ke(e,t,n){var r=[];return e.iter(t,n,(function(e){r.push(e.text)})),r}function Xe(e,t){var n=t-e.height;if(n)for(var r=e;r;r=r.parent)r.height+=n}function Ze(e){if(null==e.parent)return null;for(var t=e.parent,n=W(t.lines,e),r=t.parent;r;t=r,r=r.parent)for(var i=0;r.children[i]!=t;++i)n+=r.children[i].chunkSize();return n+t.first}function Ye(e,t){var n=e.first;e:do{for(var r=0;r<e.children.length;++r){var i=e.children[r],o=i.height;if(t<o){e=i;continue e}t-=o,n+=i.chunkSize()}return n}while(!e.lines);for(var a=0;a<e.lines.length;++a){var l=e.lines[a].height;if(t<l)break;t-=l}return n+a}function Qe(e,t){return t>=e.first&&t<e.first+e.size}function Je(e,t){return String(e.lineNumberFormatter(t+e.firstLineNumber))}function et(e,t,n){if(void 0===n&&(n=null),!(this instanceof et))return new et(e,t,n);this.line=e,this.ch=t,this.sticky=n}function tt(e,t){return e.line-t.line||e.ch-t.ch}function nt(e,t){return e.sticky==t.sticky&&0==tt(e,t)}function rt(e){return et(e.line,e.ch)}function it(e,t){return tt(e,t)<0?t:e}function ot(e,t){return tt(e,t)<0?e:t}function at(e,t){return Math.max(e.first,Math.min(t,e.first+e.size-1))}function lt(e,t){if(t.line<e.first)return et(e.first,0);var n=e.first+e.size-1;return t.line>n?et(n,Ge(e,n).text.length):function(e,t){var n=e.ch;return null==n||n>t?et(e.line,t):n<0?et(e.line,0):e}(t,Ge(e,t.line).text.length)}function st(e,t){for(var n=[],r=0;r<t.length;r++)n[r]=lt(e,t[r]);return n}$e.prototype.eol=function(){return this.pos>=this.string.length},$e.prototype.sol=function(){return this.pos==this.lineStart},$e.prototype.peek=function(){return this.string.charAt(this.pos)||void 0},$e.prototype.next=function(){if(this.pos<this.string.length)return this.string.charAt(this.pos++)},$e.prototype.eat=function(e){var t=this.string.charAt(this.pos);if("string"==typeof e?t==e:t&&(e.test?e.test(t):e(t)))return++this.pos,t},$e.prototype.eatWhile=function(e){for(var t=this.pos;this.eat(e););return this.pos>t},$e.prototype.eatSpace=function(){for(var e=this.pos;/[\s\u00a0]/.test(this.string.charAt(this.pos));)++this.pos;return this.pos>e},$e.prototype.skipToEnd=function(){this.pos=this.string.length},$e.prototype.skipTo=function(e){var t=this.string.indexOf(e,this.pos);if(t>-1)return this.pos=t,!0},$e.prototype.backUp=function(e){this.pos-=e},$e.prototype.column=function(){return this.lastColumnPos<this.start&&(this.lastColumnValue=_(this.string,this.start,this.tabSize,this.lastColumnPos,this.lastColumnValue),this.lastColumnPos=this.start),this.lastColumnValue-(this.lineStart?_(this.string,this.lineStart,this.tabSize):0)},$e.prototype.indentation=function(){return _(this.string,null,this.tabSize)-(this.lineStart?_(this.string,this.lineStart,this.tabSize):0)},$e.prototype.match=function(e,t,n){if("string"!=typeof e){var r=this.string.slice(this.pos).match(e);return r&&r.index>0?null:(r&&!1!==t&&(this.pos+=r[0].length),r)}var i=function(e){return n?e.toLowerCase():e};if(i(this.string.substr(this.pos,e.length))==i(e))return!1!==t&&(this.pos+=e.length),!0},$e.prototype.current=function(){return this.string.slice(this.start,this.pos)},$e.prototype.hideFirstChars=function(e,t){this.lineStart+=e;try{return t()}finally{this.lineStart-=e}},$e.prototype.lookAhead=function(e){var t=this.lineOracle;return t&&t.lookAhead(e)},$e.prototype.baseToken=function(){var e=this.lineOracle;return e&&e.baseToken(this.pos)};var ct=function(e,t){this.state=e,this.lookAhead=t},ut=function(e,t,n,r){this.state=t,this.doc=e,this.line=n,this.maxLookAhead=r||0,this.baseTokens=null,this.baseTokenPos=1};function dt(e,t,n,r){var i=[e.state.modeGen],o={};bt(e,t.text,e.doc.mode,n,(function(e,t){return i.push(e,t)}),o,r);for(var a=n.state,l=function(r){n.baseTokens=i;var l=e.state.overlays[r],s=1,c=0;n.state=!0,bt(e,t.text,l.mode,n,(function(e,t){for(var n=s;c<e;){var r=i[s];r>e&&i.splice(s,1,e,i[s+1],r),s+=2,c=Math.min(e,r)}if(t)if(l.opaque)i.splice(n,s-n,e,"overlay "+t),s=n+2;else for(;n<s;n+=2){var o=i[n+1];i[n+1]=(o?o+" ":"")+"overlay "+t}}),o),n.state=a,n.baseTokens=null,n.baseTokenPos=1},s=0;s<e.state.overlays.length;++s)l(s);return{styles:i,classes:o.bgClass||o.textClass?o:null}}function ht(e,t,n){if(!t.styles||t.styles[0]!=e.state.modeGen){var r=ft(e,Ze(t)),i=t.text.length>e.options.maxHighlightLength&&je(e.doc.mode,r.state),o=dt(e,t,r);i&&(r.state=i),t.stateAfter=r.save(!i),t.styles=o.styles,o.classes?t.styleClasses=o.classes:t.styleClasses&&(t.styleClasses=null),n===e.doc.highlightFrontier&&(e.doc.modeFrontier=Math.max(e.doc.modeFrontier,++e.doc.highlightFrontier))}return t.styles}function ft(e,t,n){var r=e.doc,i=e.display;if(!r.mode.startState)return new ut(r,!0,t);var o=function(e,t,n){for(var r,i,o=e.doc,a=n?-1:t-(e.doc.mode.innerMode?1e3:100),l=t;l>a;--l){if(l<=o.first)return o.first;var s=Ge(o,l-1),c=s.stateAfter;if(c&&(!n||l+(c instanceof ct?c.lookAhead:0)<=o.modeFrontier))return l;var u=_(s.text,null,e.options.tabSize);(null==i||r>u)&&(i=l-1,r=u)}return i}(e,t,n),a=o>r.first&&Ge(r,o-1).stateAfter,l=a?ut.fromSaved(r,a,o):new ut(r,Ue(r.mode),o);return r.iter(o,t,(function(n){pt(e,n.text,l);var r=l.line;n.stateAfter=r==t-1||r%5==0||r>=i.viewFrom&&r<i.viewTo?l.save():null,l.nextLine()})),n&&(r.modeFrontier=l.line),l}function pt(e,t,n,r){var i=e.doc.mode,o=new $e(t,e.options.tabSize,n);for(o.start=o.pos=r||0,""==t&&mt(i,n.state);!o.eol();)gt(i,o,n.state),o.start=o.pos}function mt(e,t){if(e.blankLine)return e.blankLine(t);if(e.innerMode){var n=qe(e,t);return n.mode.blankLine?n.mode.blankLine(n.state):void 0}}function gt(e,t,n,r){for(var i=0;i<10;i++){r&&(r[0]=qe(e,n).mode);var o=e.token(t,n);if(t.pos>t.start)return o}throw new Error("Mode "+e.name+" failed to advance stream.")}ut.prototype.lookAhead=function(e){var t=this.doc.getLine(this.line+e);return null!=t&&e>this.maxLookAhead&&(this.maxLookAhead=e),t},ut.prototype.baseToken=function(e){if(!this.baseTokens)return null;for(;this.baseTokens[this.baseTokenPos]<=e;)this.baseTokenPos+=2;var t=this.baseTokens[this.baseTokenPos+1];return{type:t&&t.replace(/( |^)overlay .*/,""),size:this.baseTokens[this.baseTokenPos]-e}},ut.prototype.nextLine=function(){this.line++,this.maxLookAhead>0&&this.maxLookAhead--},ut.fromSaved=function(e,t,n){return t instanceof ct?new ut(e,je(e.mode,t.state),n,t.lookAhead):new ut(e,je(e.mode,t),n)},ut.prototype.save=function(e){var t=!1!==e?je(this.doc.mode,this.state):this.state;return this.maxLookAhead>0?new ct(t,this.maxLookAhead):t};var vt=function(e,t,n){this.start=e.start,this.end=e.pos,this.string=e.current(),this.type=t||null,this.state=n};function xt(e,t,n,r){var i,o,a=e.doc,l=a.mode,s=Ge(a,(t=lt(a,t)).line),c=ft(e,t.line,n),u=new $e(s.text,e.options.tabSize,c);for(r&&(o=[]);(r||u.pos<t.ch)&&!u.eol();)u.start=u.pos,i=gt(l,u,c.state),r&&o.push(new vt(u,i,je(a.mode,c.state)));return r?o:new vt(u,i,c.state)}function yt(e,t){if(e)for(;;){var n=e.match(/(?:^|\s+)line-(background-)?(\S+)/);if(!n)break;e=e.slice(0,n.index)+e.slice(n.index+n[0].length);var r=n[1]?"bgClass":"textClass";null==t[r]?t[r]=n[2]:new RegExp("(?:^|\\s)"+n[2]+"(?:$|\\s)").test(t[r])||(t[r]+=" "+n[2])}return e}function bt(e,t,n,r,i,o,a){var l=n.flattenSpans;null==l&&(l=e.options.flattenSpans);var s,c=0,u=null,d=new $e(t,e.options.tabSize,r),h=e.options.addModeClass&&[null];for(""==t&&yt(mt(n,r.state),o);!d.eol();){if(d.pos>e.options.maxHighlightLength?(l=!1,a&&pt(e,t,r,d.pos),d.pos=t.length,s=null):s=yt(gt(n,d,r.state,h),o),h){var f=h[0].name;f&&(s="m-"+(s?f+" "+s:f))}if(!l||u!=s){for(;c<d.start;)i(c=Math.min(d.start,c+5e3),u);u=s}d.start=d.pos}for(;c<d.pos;){var p=Math.min(d.pos,c+5e3);i(p,u),c=p}}var wt=!1,kt=!1;function Ct(e,t,n){this.marker=e,this.from=t,this.to=n}function St(e,t){if(e)for(var n=0;n<e.length;++n){var r=e[n];if(r.marker==t)return r}}function Lt(e,t){for(var n,r=0;r<e.length;++r)e[r]!=t&&(n||(n=[])).push(e[r]);return n}function Tt(e,t){if(t.full)return null;var n=Qe(e,t.from.line)&&Ge(e,t.from.line).markedSpans,r=Qe(e,t.to.line)&&Ge(e,t.to.line).markedSpans;if(!n&&!r)return null;var i=t.from.ch,o=t.to.ch,a=0==tt(t.from,t.to),l=function(e,t,n){var r;if(e)for(var i=0;i<e.length;++i){var o=e[i],a=o.marker;if(null==o.from||(a.inclusiveLeft?o.from<=t:o.from<t)||o.from==t&&"bookmark"==a.type&&(!n||!o.marker.insertLeft)){var l=null==o.to||(a.inclusiveRight?o.to>=t:o.to>t);(r||(r=[])).push(new Ct(a,o.from,l?null:o.to))}}return r}(n,i,a),s=function(e,t,n){var r;if(e)for(var i=0;i<e.length;++i){var o=e[i],a=o.marker;if(null==o.to||(a.inclusiveRight?o.to>=t:o.to>t)||o.from==t&&"bookmark"==a.type&&(!n||o.marker.insertLeft)){var l=null==o.from||(a.inclusiveLeft?o.from<=t:o.from<t);(r||(r=[])).push(new Ct(a,l?null:o.from-t,null==o.to?null:o.to-t))}}return r}(r,o,a),c=1==t.text.length,u=K(t.text).length+(c?i:0);if(l)for(var d=0;d<l.length;++d){var h=l[d];if(null==h.to){var f=St(s,h.marker);f?c&&(h.to=null==f.to?null:f.to+u):h.to=i}}if(s)for(var p=0;p<s.length;++p){var m=s[p];if(null!=m.to&&(m.to+=u),null==m.from)St(l,m.marker)||(m.from=u,c&&(l||(l=[])).push(m));else m.from+=u,c&&(l||(l=[])).push(m)}l&&(l=Mt(l)),s&&s!=l&&(s=Mt(s));var g=[l];if(!c){var v,x=t.text.length-2;if(x>0&&l)for(var y=0;y<l.length;++y)null==l[y].to&&(v||(v=[])).push(new Ct(l[y].marker,null,null));for(var b=0;b<x;++b)g.push(v);g.push(s)}return g}function Mt(e){for(var t=0;t<e.length;++t){var n=e[t];null!=n.from&&n.from==n.to&&!1!==n.marker.clearWhenEmpty&&e.splice(t--,1)}return e.length?e:null}function At(e){var t=e.markedSpans;if(t){for(var n=0;n<t.length;++n)t[n].marker.detachLine(e);e.markedSpans=null}}function Nt(e,t){if(t){for(var n=0;n<t.length;++n)t[n].marker.attachLine(e);e.markedSpans=t}}function Dt(e){return e.inclusiveLeft?-1:0}function Ft(e){return e.inclusiveRight?1:0}function Et(e,t){var n=e.lines.length-t.lines.length;if(0!=n)return n;var r=e.find(),i=t.find(),o=tt(r.from,i.from)||Dt(e)-Dt(t);if(o)return-o;var a=tt(r.to,i.to)||Ft(e)-Ft(t);return a||t.id-e.id}function Ot(e,t){var n,r=kt&&e.markedSpans;if(r)for(var i=void 0,o=0;o<r.length;++o)(i=r[o]).marker.collapsed&&null==(t?i.from:i.to)&&(!n||Et(n,i.marker)<0)&&(n=i.marker);return n}function It(e){return Ot(e,!0)}function zt(e){return Ot(e,!1)}function Ht(e,t){var n,r=kt&&e.markedSpans;if(r)for(var i=0;i<r.length;++i){var o=r[i];o.marker.collapsed&&(null==o.from||o.from<t)&&(null==o.to||o.to>t)&&(!n||Et(n,o.marker)<0)&&(n=o.marker)}return n}function Rt(e,t,n,r,i){var o=Ge(e,t),a=kt&&o.markedSpans;if(a)for(var l=0;l<a.length;++l){var s=a[l];if(s.marker.collapsed){var c=s.marker.find(0),u=tt(c.from,n)||Dt(s.marker)-Dt(i),d=tt(c.to,r)||Ft(s.marker)-Ft(i);if(!(u>=0&&d<=0||u<=0&&d>=0)&&(u<=0&&(s.marker.inclusiveRight&&i.inclusiveLeft?tt(c.to,n)>=0:tt(c.to,n)>0)||u>=0&&(s.marker.inclusiveRight&&i.inclusiveLeft?tt(c.from,r)<=0:tt(c.from,r)<0)))return!0}}}function _t(e){for(var t;t=It(e);)e=t.find(-1,!0).line;return e}function Pt(e,t){var n=Ge(e,t),r=_t(n);return n==r?t:Ze(r)}function Wt(e,t){if(t>e.lastLine())return t;var n,r=Ge(e,t);if(!Bt(e,r))return t;for(;n=zt(r);)r=n.find(1,!0).line;return Ze(r)+1}function Bt(e,t){var n=kt&&t.markedSpans;if(n)for(var r=void 0,i=0;i<n.length;++i)if((r=n[i]).marker.collapsed){if(null==r.from)return!0;if(!r.marker.widgetNode&&0==r.from&&r.marker.inclusiveLeft&&jt(e,t,r))return!0}}function jt(e,t,n){if(null==n.to){var r=n.marker.find(1,!0);return jt(e,r.line,St(r.line.markedSpans,n.marker))}if(n.marker.inclusiveRight&&n.to==t.text.length)return!0;for(var i=void 0,o=0;o<t.markedSpans.length;++o)if((i=t.markedSpans[o]).marker.collapsed&&!i.marker.widgetNode&&i.from==n.to&&(null==i.to||i.to!=n.from)&&(i.marker.inclusiveLeft||n.marker.inclusiveRight)&&jt(e,t,i))return!0}function qt(e){for(var t=0,n=(e=_t(e)).parent,r=0;r<n.lines.length;++r){var i=n.lines[r];if(i==e)break;t+=i.height}for(var o=n.parent;o;o=(n=o).parent)for(var a=0;a<o.children.length;++a){var l=o.children[a];if(l==n)break;t+=l.height}return t}function Ut(e){if(0==e.height)return 0;for(var t,n=e.text.length,r=e;t=It(r);){var i=t.find(0,!0);r=i.from.line,n+=i.from.ch-i.to.ch}for(r=e;t=zt(r);){var o=t.find(0,!0);n-=r.text.length-o.from.ch,n+=(r=o.to.line).text.length-o.to.ch}return n}function $t(e){var t=e.display,n=e.doc;t.maxLine=Ge(n,n.first),t.maxLineLength=Ut(t.maxLine),t.maxLineChanged=!0,n.iter((function(e){var n=Ut(e);n>t.maxLineLength&&(t.maxLineLength=n,t.maxLine=e)}))}var Gt=function(e,t,n){this.text=e,Nt(this,t),this.height=n?n(this):1};function Vt(e){e.parent=null,At(e)}Gt.prototype.lineNo=function(){return Ze(this)},xe(Gt);var Kt={},Xt={};function Zt(e,t){if(!e||/^\s*$/.test(e))return null;var n=t.addModeClass?Xt:Kt;return n[e]||(n[e]=e.replace(/\S+/g,"cm-$&"))}function Yt(e,t){var n=D("span",null,null,s?"padding-right: .1px":null),r={pre:D("pre",[n],"CodeMirror-line"),content:n,col:0,pos:0,cm:e,trailingSpace:!1,splitSpaces:e.getOption("lineWrapping")};t.measure={};for(var i=0;i<=(t.rest?t.rest.length:0);i++){var o=i?t.rest[i-1]:t.line,a=void 0;r.pos=0,r.addToken=Jt,Ne(e.display.measure)&&(a=ce(o,e.doc.direction))&&(r.addToken=en(r.addToken,a)),r.map=[],nn(o,r,ht(e,o,t!=e.display.externalMeasured&&Ze(o))),o.styleClasses&&(o.styleClasses.bgClass&&(r.bgClass=I(o.styleClasses.bgClass,r.bgClass||"")),o.styleClasses.textClass&&(r.textClass=I(o.styleClasses.textClass,r.textClass||""))),0==r.map.length&&r.map.push(0,0,r.content.appendChild(Ae(e.display.measure))),0==i?(t.measure.map=r.map,t.measure.cache={}):((t.measure.maps||(t.measure.maps=[])).push(r.map),(t.measure.caches||(t.measure.caches=[])).push({}))}if(s){var l=r.content.lastChild;(/\bcm-tab\b/.test(l.className)||l.querySelector&&l.querySelector(".cm-tab"))&&(r.content.className="cm-tab-wrap-hack")}return pe(e,"renderLine",e,t.line,r.pre),r.pre.className&&(r.textClass=I(r.pre.className,r.textClass||"")),r}function Qt(e){var t=N("span","•","cm-invalidchar");return t.title="\\u"+e.charCodeAt(0).toString(16),t.setAttribute("aria-label",t.title),t}function Jt(e,t,n,r,i,o,s){if(t){var c,u=e.splitSpaces?function(e,t){if(e.length>1&&!/  /.test(e))return e;for(var n=t,r="",i=0;i<e.length;i++){var o=e.charAt(i);" "!=o||!n||i!=e.length-1&&32!=e.charCodeAt(i+1)||(o=" "),r+=o,n=" "==o}return r}(t,e.trailingSpace):t,d=e.cm.state.specialChars,h=!1;if(d.test(t)){c=document.createDocumentFragment();for(var f=0;;){d.lastIndex=f;var p=d.exec(t),m=p?p.index-f:t.length-f;if(m){var g=document.createTextNode(u.slice(f,f+m));a&&l<9?c.appendChild(N("span",[g])):c.appendChild(g),e.map.push(e.pos,e.pos+m,g),e.col+=m,e.pos+=m}if(!p)break;f+=m+1;var v=void 0;if("\t"==p[0]){var x=e.cm.options.tabSize,y=x-e.col%x;(v=c.appendChild(N("span",V(y),"cm-tab"))).setAttribute("role","presentation"),v.setAttribute("cm-text","\t"),e.col+=y}else"\r"==p[0]||"\n"==p[0]?((v=c.appendChild(N("span","\r"==p[0]?"␍":"␤","cm-invalidchar"))).setAttribute("cm-text",p[0]),e.col+=1):((v=e.cm.options.specialCharPlaceholder(p[0])).setAttribute("cm-text",p[0]),a&&l<9?c.appendChild(N("span",[v])):c.appendChild(v),e.col+=1);e.map.push(e.pos,e.pos+1,v),e.pos++}}else e.col+=t.length,c=document.createTextNode(u),e.map.push(e.pos,e.pos+t.length,c),a&&l<9&&(h=!0),e.pos+=t.length;if(e.trailingSpace=32==u.charCodeAt(t.length-1),n||r||i||h||o||s){var b=n||"";r&&(b+=r),i&&(b+=i);var w=N("span",[c],b,o);if(s)for(var k in s)s.hasOwnProperty(k)&&"style"!=k&&"class"!=k&&w.setAttribute(k,s[k]);return e.content.appendChild(w)}e.content.appendChild(c)}}function en(e,t){return function(n,r,i,o,a,l,s){i=i?i+" cm-force-border":"cm-force-border";for(var c=n.pos,u=c+r.length;;){for(var d=void 0,h=0;h<t.length&&!((d=t[h]).to>c&&d.from<=c);h++);if(d.to>=u)return e(n,r,i,o,a,l,s);e(n,r.slice(0,d.to-c),i,o,null,l,s),o=null,r=r.slice(d.to-c),c=d.to}}}function tn(e,t,n,r){var i=!r&&n.widgetNode;i&&e.map.push(e.pos,e.pos+t,i),!r&&e.cm.display.input.needsContentAttribute&&(i||(i=e.content.appendChild(document.createElement("span"))),i.setAttribute("cm-marker",n.id)),i&&(e.cm.display.input.setUneditable(i),e.content.appendChild(i)),e.pos+=t,e.trailingSpace=!1}function nn(e,t,n){var r=e.markedSpans,i=e.text,o=0;if(r)for(var a,l,s,c,u,d,h,f=i.length,p=0,m=1,g="",v=0;;){if(v==p){s=c=u=l="",h=null,d=null,v=1/0;for(var x=[],y=void 0,b=0;b<r.length;++b){var w=r[b],k=w.marker;if("bookmark"==k.type&&w.from==p&&k.widgetNode)x.push(k);else if(w.from<=p&&(null==w.to||w.to>p||k.collapsed&&w.to==p&&w.from==p)){if(null!=w.to&&w.to!=p&&v>w.to&&(v=w.to,c=""),k.className&&(s+=" "+k.className),k.css&&(l=(l?l+";":"")+k.css),k.startStyle&&w.from==p&&(u+=" "+k.startStyle),k.endStyle&&w.to==v&&(y||(y=[])).push(k.endStyle,w.to),k.title&&((h||(h={})).title=k.title),k.attributes)for(var C in k.attributes)(h||(h={}))[C]=k.attributes[C];k.collapsed&&(!d||Et(d.marker,k)<0)&&(d=w)}else w.from>p&&v>w.from&&(v=w.from)}if(y)for(var S=0;S<y.length;S+=2)y[S+1]==v&&(c+=" "+y[S]);if(!d||d.from==p)for(var L=0;L<x.length;++L)tn(t,0,x[L]);if(d&&(d.from||0)==p){if(tn(t,(null==d.to?f+1:d.to)-p,d.marker,null==d.from),null==d.to)return;d.to==p&&(d=!1)}}if(p>=f)break;for(var T=Math.min(f,v);;){if(g){var M=p+g.length;if(!d){var A=M>T?g.slice(0,T-p):g;t.addToken(t,A,a?a+s:s,u,p+A.length==v?c:"",l,h)}if(M>=T){g=g.slice(T-p),p=T;break}p=M,u=""}g=i.slice(o,o=n[m++]),a=Zt(n[m++],t.cm.options)}}else for(var N=1;N<n.length;N+=2)t.addToken(t,i.slice(o,o=n[N]),Zt(n[N+1],t.cm.options))}function rn(e,t,n){this.line=t,this.rest=function(e){for(var t,n;t=zt(e);)e=t.find(1,!0).line,(n||(n=[])).push(e);return n}(t),this.size=this.rest?Ze(K(this.rest))-n+1:1,this.node=this.text=null,this.hidden=Bt(e,t)}function on(e,t,n){for(var r,i=[],o=t;o<n;o=r){var a=new rn(e.doc,Ge(e.doc,o),o);r=o+a.size,i.push(a)}return i}var an=null;var ln=null;function sn(e,t){var n=he(e,t);if(n.length){var r,i=Array.prototype.slice.call(arguments,2);an?r=an.delayedCallbacks:ln?r=ln:(r=ln=[],setTimeout(cn,0));for(var o=function(e){r.push((function(){return n[e].apply(null,i)}))},a=0;a<n.length;++a)o(a)}}function cn(){var e=ln;ln=null;for(var t=0;t<e.length;++t)e[t]()}function un(e,t,n,r){for(var i=0;i<t.changes.length;i++){var o=t.changes[i];"text"==o?fn(e,t):"gutter"==o?mn(e,t,n,r):"class"==o?pn(e,t):"widget"==o&&gn(e,t,r)}t.changes=null}function dn(e){return e.node==e.text&&(e.node=N("div",null,null,"position: relative"),e.text.parentNode&&e.text.parentNode.replaceChild(e.node,e.text),e.node.appendChild(e.text),a&&l<8&&(e.node.style.zIndex=2)),e.node}function hn(e,t){var n=e.display.externalMeasured;return n&&n.line==t.line?(e.display.externalMeasured=null,t.measure=n.measure,n.built):Yt(e,t)}function fn(e,t){var n=t.text.className,r=hn(e,t);t.text==t.node&&(t.node=r.pre),t.text.parentNode.replaceChild(r.pre,t.text),t.text=r.pre,r.bgClass!=t.bgClass||r.textClass!=t.textClass?(t.bgClass=r.bgClass,t.textClass=r.textClass,pn(e,t)):n&&(t.text.className=n)}function pn(e,t){!function(e,t){var n=t.bgClass?t.bgClass+" "+(t.line.bgClass||""):t.line.bgClass;if(n&&(n+=" CodeMirror-linebackground"),t.background)n?t.background.className=n:(t.background.parentNode.removeChild(t.background),t.background=null);else if(n){var r=dn(t);t.background=r.insertBefore(N("div",null,n),r.firstChild),e.display.input.setUneditable(t.background)}}(e,t),t.line.wrapClass?dn(t).className=t.line.wrapClass:t.node!=t.text&&(t.node.className="");var n=t.textClass?t.textClass+" "+(t.line.textClass||""):t.line.textClass;t.text.className=n||""}function mn(e,t,n,r){if(t.gutter&&(t.node.removeChild(t.gutter),t.gutter=null),t.gutterBackground&&(t.node.removeChild(t.gutterBackground),t.gutterBackground=null),t.line.gutterClass){var i=dn(t);t.gutterBackground=N("div",null,"CodeMirror-gutter-background "+t.line.gutterClass,"left: "+(e.options.fixedGutter?r.fixedPos:-r.gutterTotalWidth)+"px; width: "+r.gutterTotalWidth+"px"),e.display.input.setUneditable(t.gutterBackground),i.insertBefore(t.gutterBackground,t.text)}var o=t.line.gutterMarkers;if(e.options.lineNumbers||o){var a=dn(t),l=t.gutter=N("div",null,"CodeMirror-gutter-wrapper","left: "+(e.options.fixedGutter?r.fixedPos:-r.gutterTotalWidth)+"px");if(e.display.input.setUneditable(l),a.insertBefore(l,t.text),t.line.gutterClass&&(l.className+=" "+t.line.gutterClass),!e.options.lineNumbers||o&&o["CodeMirror-linenumbers"]||(t.lineNumber=l.appendChild(N("div",Je(e.options,n),"CodeMirror-linenumber CodeMirror-gutter-elt","left: "+r.gutterLeft["CodeMirror-linenumbers"]+"px; width: "+e.display.lineNumInnerWidth+"px"))),o)for(var s=0;s<e.display.gutterSpecs.length;++s){var c=e.display.gutterSpecs[s].className,u=o.hasOwnProperty(c)&&o[c];u&&l.appendChild(N("div",[u],"CodeMirror-gutter-elt","left: "+r.gutterLeft[c]+"px; width: "+r.gutterWidth[c]+"px"))}}}function gn(e,t,n){t.alignable&&(t.alignable=null);for(var r=S("CodeMirror-linewidget"),i=t.node.firstChild,o=void 0;i;i=o)o=i.nextSibling,r.test(i.className)&&t.node.removeChild(i);xn(e,t,n)}function vn(e,t,n,r){var i=hn(e,t);return t.text=t.node=i.pre,i.bgClass&&(t.bgClass=i.bgClass),i.textClass&&(t.textClass=i.textClass),pn(e,t),mn(e,t,n,r),xn(e,t,r),t.node}function xn(e,t,n){if(yn(e,t.line,t,n,!0),t.rest)for(var r=0;r<t.rest.length;r++)yn(e,t.rest[r],t,n,!1)}function yn(e,t,n,r,i){if(t.widgets)for(var o=dn(n),a=0,l=t.widgets;a<l.length;++a){var s=l[a],c=N("div",[s.node],"CodeMirror-linewidget"+(s.className?" "+s.className:""));s.handleMouseEvents||c.setAttribute("cm-ignore-events","true"),bn(s,c,n,r),e.display.input.setUneditable(c),i&&s.above?o.insertBefore(c,n.gutter||n.text):o.appendChild(c),sn(s,"redraw")}}function bn(e,t,n,r){if(e.noHScroll){(n.alignable||(n.alignable=[])).push(t);var i=r.wrapperWidth;t.style.left=r.fixedPos+"px",e.coverGutter||(i-=r.gutterTotalWidth,t.style.paddingLeft=r.gutterTotalWidth+"px"),t.style.width=i+"px"}e.coverGutter&&(t.style.zIndex=5,t.style.position="relative",e.noHScroll||(t.style.marginLeft=-r.gutterTotalWidth+"px"))}function wn(e){if(null!=e.height)return e.height;var t=e.doc.cm;if(!t)return 0;if(!F(document.body,e.node)){var n="position: relative;";e.coverGutter&&(n+="margin-left: -"+t.display.gutters.offsetWidth+"px;"),e.noHScroll&&(n+="width: "+t.display.wrapper.clientWidth+"px;"),A(t.display.measure,N("div",[e.node],null,n))}return e.height=e.node.parentNode.offsetHeight}function kn(e,t){for(var n=Ce(t);n!=e.wrapper;n=n.parentNode)if(!n||1==n.nodeType&&"true"==n.getAttribute("cm-ignore-events")||n.parentNode==e.sizer&&n!=e.mover)return!0}function Cn(e){return e.lineSpace.offsetTop}function Sn(e){return e.mover.offsetHeight-e.lineSpace.offsetHeight}function Ln(e){if(e.cachedPaddingH)return e.cachedPaddingH;var t=A(e.measure,N("pre","x","CodeMirror-line-like")),n=window.getComputedStyle?window.getComputedStyle(t):t.currentStyle,r={left:parseInt(n.paddingLeft),right:parseInt(n.paddingRight)};return isNaN(r.left)||isNaN(r.right)||(e.cachedPaddingH=r),r}function Tn(e){return 50-e.display.nativeBarWidth}function Mn(e){return e.display.scroller.clientWidth-Tn(e)-e.display.barWidth}function An(e){return e.display.scroller.clientHeight-Tn(e)-e.display.barHeight}function Nn(e,t,n){if(e.line==t)return{map:e.measure.map,cache:e.measure.cache};for(var r=0;r<e.rest.length;r++)if(e.rest[r]==t)return{map:e.measure.maps[r],cache:e.measure.caches[r]};for(var i=0;i<e.rest.length;i++)if(Ze(e.rest[i])>n)return{map:e.measure.maps[i],cache:e.measure.caches[i],before:!0}}function Dn(e,t,n,r){return On(e,En(e,t),n,r)}function Fn(e,t){if(t>=e.display.viewFrom&&t<e.display.viewTo)return e.display.view[ur(e,t)];var n=e.display.externalMeasured;return n&&t>=n.lineN&&t<n.lineN+n.size?n:void 0}function En(e,t){var n=Ze(t),r=Fn(e,n);r&&!r.text?r=null:r&&r.changes&&(un(e,r,n,or(e)),e.curOp.forceUpdate=!0),r||(r=function(e,t){var n=Ze(t=_t(t)),r=e.display.externalMeasured=new rn(e.doc,t,n);r.lineN=n;var i=r.built=Yt(e,r);return r.text=i.pre,A(e.display.lineMeasure,i.pre),r}(e,t));var i=Nn(r,t,n);return{line:t,view:r,rect:null,map:i.map,cache:i.cache,before:i.before,hasHeights:!1}}function On(e,t,n,r,i){t.before&&(n=-1);var o,s=n+(r||"");return t.cache.hasOwnProperty(s)?o=t.cache[s]:(t.rect||(t.rect=t.view.text.getBoundingClientRect()),t.hasHeights||(!function(e,t,n){var r=e.options.lineWrapping,i=r&&Mn(e);if(!t.measure.heights||r&&t.measure.width!=i){var o=t.measure.heights=[];if(r){t.measure.width=i;for(var a=t.text.firstChild.getClientRects(),l=0;l<a.length-1;l++){var s=a[l],c=a[l+1];Math.abs(s.bottom-c.bottom)>2&&o.push((s.bottom+c.top)/2-n.top)}}o.push(n.bottom-n.top)}}(e,t.view,t.rect),t.hasHeights=!0),(o=function(e,t,n,r){var i,o=Hn(t.map,n,r),s=o.node,c=o.start,u=o.end,d=o.collapse;if(3==s.nodeType){for(var h=0;h<4;h++){for(;c&&re(t.line.text.charAt(o.coverStart+c));)--c;for(;o.coverStart+u<o.coverEnd&&re(t.line.text.charAt(o.coverStart+u));)++u;if((i=a&&l<9&&0==c&&u==o.coverEnd-o.coverStart?s.parentNode.getBoundingClientRect():Rn(L(s,c,u).getClientRects(),r)).left||i.right||0==c)break;u=c,c-=1,d="right"}a&&l<11&&(i=function(e,t){if(!window.screen||null==screen.logicalXDPI||screen.logicalXDPI==screen.deviceXDPI||!function(e){if(null!=Ie)return Ie;var t=A(e,N("span","x")),n=t.getBoundingClientRect(),r=L(t,0,1).getBoundingClientRect();return Ie=Math.abs(n.left-r.left)>1}(e))return t;var n=screen.logicalXDPI/screen.deviceXDPI,r=screen.logicalYDPI/screen.deviceYDPI;return{left:t.left*n,right:t.right*n,top:t.top*r,bottom:t.bottom*r}}(e.display.measure,i))}else{var f;c>0&&(d=r="right"),i=e.options.lineWrapping&&(f=s.getClientRects()).length>1?f["right"==r?f.length-1:0]:s.getBoundingClientRect()}if(a&&l<9&&!c&&(!i||!i.left&&!i.right)){var p=s.parentNode.getClientRects()[0];i=p?{left:p.left,right:p.left+ir(e.display),top:p.top,bottom:p.bottom}:zn}for(var m=i.top-t.rect.top,g=i.bottom-t.rect.top,v=(m+g)/2,x=t.view.measure.heights,y=0;y<x.length-1&&!(v<x[y]);y++);var b=y?x[y-1]:0,w=x[y],k={left:("right"==d?i.right:i.left)-t.rect.left,right:("left"==d?i.left:i.right)-t.rect.left,top:b,bottom:w};i.left||i.right||(k.bogus=!0);e.options.singleCursorHeightPerLine||(k.rtop=m,k.rbottom=g);return k}(e,t,n,r)).bogus||(t.cache[s]=o)),{left:o.left,right:o.right,top:i?o.rtop:o.top,bottom:i?o.rbottom:o.bottom}}var In,zn={left:0,right:0,top:0,bottom:0};function Hn(e,t,n){for(var r,i,o,a,l,s,c=0;c<e.length;c+=3)if(l=e[c],s=e[c+1],t<l?(i=0,o=1,a="left"):t<s?o=(i=t-l)+1:(c==e.length-3||t==s&&e[c+3]>t)&&(i=(o=s-l)-1,t>=s&&(a="right")),null!=i){if(r=e[c+2],l==s&&n==(r.insertLeft?"left":"right")&&(a=n),"left"==n&&0==i)for(;c&&e[c-2]==e[c-3]&&e[c-1].insertLeft;)r=e[2+(c-=3)],a="left";if("right"==n&&i==s-l)for(;c<e.length-3&&e[c+3]==e[c+4]&&!e[c+5].insertLeft;)r=e[(c+=3)+2],a="right";break}return{node:r,start:i,end:o,collapse:a,coverStart:l,coverEnd:s}}function Rn(e,t){var n=zn;if("left"==t)for(var r=0;r<e.length&&(n=e[r]).left==n.right;r++);else for(var i=e.length-1;i>=0&&(n=e[i]).left==n.right;i--);return n}function _n(e){if(e.measure&&(e.measure.cache={},e.measure.heights=null,e.rest))for(var t=0;t<e.rest.length;t++)e.measure.caches[t]={}}function Pn(e){e.display.externalMeasure=null,M(e.display.lineMeasure);for(var t=0;t<e.display.view.length;t++)_n(e.display.view[t])}function Wn(e){Pn(e),e.display.cachedCharWidth=e.display.cachedTextHeight=e.display.cachedPaddingH=null,e.options.lineWrapping||(e.display.maxLineChanged=!0),e.display.lineNumChars=null}function Bn(){return u&&g?-(document.body.getBoundingClientRect().left-parseInt(getComputedStyle(document.body).marginLeft)):window.pageXOffset||(document.documentElement||document.body).scrollLeft}function jn(){return u&&g?-(document.body.getBoundingClientRect().top-parseInt(getComputedStyle(document.body).marginTop)):window.pageYOffset||(document.documentElement||document.body).scrollTop}function qn(e){var t=0;if(e.widgets)for(var n=0;n<e.widgets.length;++n)e.widgets[n].above&&(t+=wn(e.widgets[n]));return t}function Un(e,t,n,r,i){if(!i){var o=qn(t);n.top+=o,n.bottom+=o}if("line"==r)return n;r||(r="local");var a=qt(t);if("local"==r?a+=Cn(e.display):a-=e.display.viewOffset,"page"==r||"window"==r){var l=e.display.lineSpace.getBoundingClientRect();a+=l.top+("window"==r?0:jn());var s=l.left+("window"==r?0:Bn());n.left+=s,n.right+=s}return n.top+=a,n.bottom+=a,n}function $n(e,t,n){if("div"==n)return t;var r=t.left,i=t.top;if("page"==n)r-=Bn(),i-=jn();else if("local"==n||!n){var o=e.display.sizer.getBoundingClientRect();r+=o.left,i+=o.top}var a=e.display.lineSpace.getBoundingClientRect();return{left:r-a.left,top:i-a.top}}function Gn(e,t,n,r,i){return r||(r=Ge(e.doc,t.line)),Un(e,r,Dn(e,r,t.ch,i),n)}function Vn(e,t,n,r,i,o){function a(t,a){var l=On(e,i,t,a?"right":"left",o);return a?l.left=l.right:l.right=l.left,Un(e,r,l,n)}r=r||Ge(e.doc,t.line),i||(i=En(e,r));var l=ce(r,e.doc.direction),s=t.ch,c=t.sticky;if(s>=r.text.length?(s=r.text.length,c="before"):s<=0&&(s=0,c="after"),!l)return a("before"==c?s-1:s,"before"==c);function u(e,t,n){return a(n?e-1:e,1==l[t].level!=n)}var d=le(l,s,c),h=ae,f=u(s,d,"before"==c);return null!=h&&(f.other=u(s,h,"before"!=c)),f}function Kn(e,t){var n=0;t=lt(e.doc,t),e.options.lineWrapping||(n=ir(e.display)*t.ch);var r=Ge(e.doc,t.line),i=qt(r)+Cn(e.display);return{left:n,right:n,top:i,bottom:i+r.height}}function Xn(e,t,n,r,i){var o=et(e,t,n);return o.xRel=i,r&&(o.outside=r),o}function Zn(e,t,n){var r=e.doc;if((n+=e.display.viewOffset)<0)return Xn(r.first,0,null,-1,-1);var i=Ye(r,n),o=r.first+r.size-1;if(i>o)return Xn(r.first+r.size-1,Ge(r,o).text.length,null,1,1);t<0&&(t=0);for(var a=Ge(r,i);;){var l=er(e,a,i,t,n),s=Ht(a,l.ch+(l.xRel>0||l.outside>0?1:0));if(!s)return l;var c=s.find(1);if(c.line==i)return c;a=Ge(r,i=c.line)}}function Yn(e,t,n,r){r-=qn(t);var i=t.text.length,o=oe((function(t){return On(e,n,t-1).bottom<=r}),i,0);return{begin:o,end:i=oe((function(t){return On(e,n,t).top>r}),o,i)}}function Qn(e,t,n,r){return n||(n=En(e,t)),Yn(e,t,n,Un(e,t,On(e,n,r),"line").top)}function Jn(e,t,n,r){return!(e.bottom<=n)&&(e.top>n||(r?e.left:e.right)>t)}function er(e,t,n,r,i){i-=qt(t);var o=En(e,t),a=qn(t),l=0,s=t.text.length,c=!0,u=ce(t,e.doc.direction);if(u){var d=(e.options.lineWrapping?nr:tr)(e,t,n,o,u,r,i);l=(c=1!=d.level)?d.from:d.to-1,s=c?d.to:d.from-1}var h,f,p=null,m=null,g=oe((function(t){var n=On(e,o,t);return n.top+=a,n.bottom+=a,!!Jn(n,r,i,!1)&&(n.top<=i&&n.left<=r&&(p=t,m=n),!0)}),l,s),v=!1;if(m){var x=r-m.left<m.right-r,y=x==c;g=p+(y?0:1),f=y?"after":"before",h=x?m.left:m.right}else{c||g!=s&&g!=l||g++,f=0==g?"after":g==t.text.length?"before":On(e,o,g-(c?1:0)).bottom+a<=i==c?"after":"before";var b=Vn(e,et(n,g,f),"line",t,o);h=b.left,v=i<b.top?-1:i>=b.bottom?1:0}return Xn(n,g=ie(t.text,g,1),f,v,r-h)}function tr(e,t,n,r,i,o,a){var l=oe((function(l){var s=i[l],c=1!=s.level;return Jn(Vn(e,et(n,c?s.to:s.from,c?"before":"after"),"line",t,r),o,a,!0)}),0,i.length-1),s=i[l];if(l>0){var c=1!=s.level,u=Vn(e,et(n,c?s.from:s.to,c?"after":"before"),"line",t,r);Jn(u,o,a,!0)&&u.top>a&&(s=i[l-1])}return s}function nr(e,t,n,r,i,o,a){var l=Yn(e,t,r,a),s=l.begin,c=l.end;/\s/.test(t.text.charAt(c-1))&&c--;for(var u=null,d=null,h=0;h<i.length;h++){var f=i[h];if(!(f.from>=c||f.to<=s)){var p=On(e,r,1!=f.level?Math.min(c,f.to)-1:Math.max(s,f.from)).right,m=p<o?o-p+1e9:p-o;(!u||d>m)&&(u=f,d=m)}}return u||(u=i[i.length-1]),u.from<s&&(u={from:s,to:u.to,level:u.level}),u.to>c&&(u={from:u.from,to:c,level:u.level}),u}function rr(e){if(null!=e.cachedTextHeight)return e.cachedTextHeight;if(null==In){In=N("pre",null,"CodeMirror-line-like");for(var t=0;t<49;++t)In.appendChild(document.createTextNode("x")),In.appendChild(N("br"));In.appendChild(document.createTextNode("x"))}A(e.measure,In);var n=In.offsetHeight/50;return n>3&&(e.cachedTextHeight=n),M(e.measure),n||1}function ir(e){if(null!=e.cachedCharWidth)return e.cachedCharWidth;var t=N("span","xxxxxxxxxx"),n=N("pre",[t],"CodeMirror-line-like");A(e.measure,n);var r=t.getBoundingClientRect(),i=(r.right-r.left)/10;return i>2&&(e.cachedCharWidth=i),i||10}function or(e){for(var t=e.display,n={},r={},i=t.gutters.clientLeft,o=t.gutters.firstChild,a=0;o;o=o.nextSibling,++a){var l=e.display.gutterSpecs[a].className;n[l]=o.offsetLeft+o.clientLeft+i,r[l]=o.clientWidth}return{fixedPos:ar(t),gutterTotalWidth:t.gutters.offsetWidth,gutterLeft:n,gutterWidth:r,wrapperWidth:t.wrapper.clientWidth}}function ar(e){return e.scroller.getBoundingClientRect().left-e.sizer.getBoundingClientRect().left}function lr(e){var t=rr(e.display),n=e.options.lineWrapping,r=n&&Math.max(5,e.display.scroller.clientWidth/ir(e.display)-3);return function(i){if(Bt(e.doc,i))return 0;var o=0;if(i.widgets)for(var a=0;a<i.widgets.length;a++)i.widgets[a].height&&(o+=i.widgets[a].height);return n?o+(Math.ceil(i.text.length/r)||1)*t:o+t}}function sr(e){var t=e.doc,n=lr(e);t.iter((function(e){var t=n(e);t!=e.height&&Xe(e,t)}))}function cr(e,t,n,r){var i=e.display;if(!n&&"true"==Ce(t).getAttribute("cm-not-content"))return null;var o,a,l=i.lineSpace.getBoundingClientRect();try{o=t.clientX-l.left,a=t.clientY-l.top}catch(e){return null}var s,c=Zn(e,o,a);if(r&&c.xRel>0&&(s=Ge(e.doc,c.line).text).length==c.ch){var u=_(s,s.length,e.options.tabSize)-s.length;c=et(c.line,Math.max(0,Math.round((o-Ln(e.display).left)/ir(e.display))-u))}return c}function ur(e,t){if(t>=e.display.viewTo)return null;if((t-=e.display.viewFrom)<0)return null;for(var n=e.display.view,r=0;r<n.length;r++)if((t-=n[r].size)<0)return r}function dr(e,t,n,r){null==t&&(t=e.doc.first),null==n&&(n=e.doc.first+e.doc.size),r||(r=0);var i=e.display;if(r&&n<i.viewTo&&(null==i.updateLineNumbers||i.updateLineNumbers>t)&&(i.updateLineNumbers=t),e.curOp.viewChanged=!0,t>=i.viewTo)kt&&Pt(e.doc,t)<i.viewTo&&fr(e);else if(n<=i.viewFrom)kt&&Wt(e.doc,n+r)>i.viewFrom?fr(e):(i.viewFrom+=r,i.viewTo+=r);else if(t<=i.viewFrom&&n>=i.viewTo)fr(e);else if(t<=i.viewFrom){var o=pr(e,n,n+r,1);o?(i.view=i.view.slice(o.index),i.viewFrom=o.lineN,i.viewTo+=r):fr(e)}else if(n>=i.viewTo){var a=pr(e,t,t,-1);a?(i.view=i.view.slice(0,a.index),i.viewTo=a.lineN):fr(e)}else{var l=pr(e,t,t,-1),s=pr(e,n,n+r,1);l&&s?(i.view=i.view.slice(0,l.index).concat(on(e,l.lineN,s.lineN)).concat(i.view.slice(s.index)),i.viewTo+=r):fr(e)}var c=i.externalMeasured;c&&(n<c.lineN?c.lineN+=r:t<c.lineN+c.size&&(i.externalMeasured=null))}function hr(e,t,n){e.curOp.viewChanged=!0;var r=e.display,i=e.display.externalMeasured;if(i&&t>=i.lineN&&t<i.lineN+i.size&&(r.externalMeasured=null),!(t<r.viewFrom||t>=r.viewTo)){var o=r.view[ur(e,t)];if(null!=o.node){var a=o.changes||(o.changes=[]);-1==W(a,n)&&a.push(n)}}}function fr(e){e.display.viewFrom=e.display.viewTo=e.doc.first,e.display.view=[],e.display.viewOffset=0}function pr(e,t,n,r){var i,o=ur(e,t),a=e.display.view;if(!kt||n==e.doc.first+e.doc.size)return{index:o,lineN:n};for(var l=e.display.viewFrom,s=0;s<o;s++)l+=a[s].size;if(l!=t){if(r>0){if(o==a.length-1)return null;i=l+a[o].size-t,o++}else i=l-t;t+=i,n+=i}for(;Pt(e.doc,n)!=n;){if(o==(r<0?0:a.length-1))return null;n+=r*a[o-(r<0?1:0)].size,o+=r}return{index:o,lineN:n}}function mr(e){for(var t=e.display.view,n=0,r=0;r<t.length;r++){var i=t[r];i.hidden||i.node&&!i.changes||++n}return n}function gr(e){e.display.input.showSelection(e.display.input.prepareSelection())}function vr(e,t){void 0===t&&(t=!0);for(var n=e.doc,r={},i=r.cursors=document.createDocumentFragment(),o=r.selection=document.createDocumentFragment(),a=0;a<n.sel.ranges.length;a++)if(t||a!=n.sel.primIndex){var l=n.sel.ranges[a];if(!(l.from().line>=e.display.viewTo||l.to().line<e.display.viewFrom)){var s=l.empty();(s||e.options.showCursorWhenSelecting)&&xr(e,l.head,i),s||br(e,l,o)}}return r}function xr(e,t,n){var r=Vn(e,t,"div",null,null,!e.options.singleCursorHeightPerLine),i=n.appendChild(N("div"," ","CodeMirror-cursor"));if(i.style.left=r.left+"px",i.style.top=r.top+"px",i.style.height=Math.max(0,r.bottom-r.top)*e.options.cursorHeight+"px",r.other){var o=n.appendChild(N("div"," ","CodeMirror-cursor CodeMirror-secondarycursor"));o.style.display="",o.style.left=r.other.left+"px",o.style.top=r.other.top+"px",o.style.height=.85*(r.other.bottom-r.other.top)+"px"}}function yr(e,t){return e.top-t.top||e.left-t.left}function br(e,t,n){var r=e.display,i=e.doc,o=document.createDocumentFragment(),a=Ln(e.display),l=a.left,s=Math.max(r.sizerWidth,Mn(e)-r.sizer.offsetLeft)-a.right,c="ltr"==i.direction;function u(e,t,n,r){t<0&&(t=0),t=Math.round(t),r=Math.round(r),o.appendChild(N("div",null,"CodeMirror-selected","position: absolute; left: "+e+"px;\n                             top: "+t+"px; width: "+(null==n?s-e:n)+"px;\n                             height: "+(r-t)+"px"))}function d(t,n,r){var o,a,d=Ge(i,t),h=d.text.length;function f(n,r){return Gn(e,et(t,n),"div",d,r)}function p(t,n,r){var i=Qn(e,d,null,t),o="ltr"==n==("after"==r)?"left":"right";return f("after"==r?i.begin:i.end-(/\s/.test(d.text.charAt(i.end-1))?2:1),o)[o]}var m=ce(d,i.direction);return function(e,t,n,r){if(!e)return r(t,n,"ltr",0);for(var i=!1,o=0;o<e.length;++o){var a=e[o];(a.from<n&&a.to>t||t==n&&a.to==t)&&(r(Math.max(a.from,t),Math.min(a.to,n),1==a.level?"rtl":"ltr",o),i=!0)}i||r(t,n,"ltr")}(m,n||0,null==r?h:r,(function(e,t,i,d){var g="ltr"==i,v=f(e,g?"left":"right"),x=f(t-1,g?"right":"left"),y=null==n&&0==e,b=null==r&&t==h,w=0==d,k=!m||d==m.length-1;if(x.top-v.top<=3){var C=(c?b:y)&&k,S=(c?y:b)&&w?l:(g?v:x).left,L=C?s:(g?x:v).right;u(S,v.top,L-S,v.bottom)}else{var T,M,A,N;g?(T=c&&y&&w?l:v.left,M=c?s:p(e,i,"before"),A=c?l:p(t,i,"after"),N=c&&b&&k?s:x.right):(T=c?p(e,i,"before"):l,M=!c&&y&&w?s:v.right,A=!c&&b&&k?l:x.left,N=c?p(t,i,"after"):s),u(T,v.top,M-T,v.bottom),v.bottom<x.top&&u(l,v.bottom,null,x.top),u(A,x.top,N-A,x.bottom)}(!o||yr(v,o)<0)&&(o=v),yr(x,o)<0&&(o=x),(!a||yr(v,a)<0)&&(a=v),yr(x,a)<0&&(a=x)})),{start:o,end:a}}var h=t.from(),f=t.to();if(h.line==f.line)d(h.line,h.ch,f.ch);else{var p=Ge(i,h.line),m=Ge(i,f.line),g=_t(p)==_t(m),v=d(h.line,h.ch,g?p.text.length+1:null).end,x=d(f.line,g?0:null,f.ch).start;g&&(v.top<x.top-2?(u(v.right,v.top,null,v.bottom),u(l,x.top,x.left,x.bottom)):u(v.right,v.top,x.left-v.right,v.bottom)),v.bottom<x.top&&u(l,v.bottom,null,x.top)}n.appendChild(o)}function wr(e){if(e.state.focused){var t=e.display;clearInterval(t.blinker);var n=!0;t.cursorDiv.style.visibility="",e.options.cursorBlinkRate>0?t.blinker=setInterval((function(){e.hasFocus()||Lr(e),t.cursorDiv.style.visibility=(n=!n)?"":"hidden"}),e.options.cursorBlinkRate):e.options.cursorBlinkRate<0&&(t.cursorDiv.style.visibility="hidden")}}function kr(e){e.state.focused||(e.display.input.focus(),Sr(e))}function Cr(e){e.state.delayingBlurEvent=!0,setTimeout((function(){e.state.delayingBlurEvent&&(e.state.delayingBlurEvent=!1,Lr(e))}),100)}function Sr(e,t){e.state.delayingBlurEvent&&(e.state.delayingBlurEvent=!1),"nocursor"!=e.options.readOnly&&(e.state.focused||(pe(e,"focus",e,t),e.state.focused=!0,O(e.display.wrapper,"CodeMirror-focused"),e.curOp||e.display.selForContextMenu==e.doc.sel||(e.display.input.reset(),s&&setTimeout((function(){return e.display.input.reset(!0)}),20)),e.display.input.receivedFocus()),wr(e))}function Lr(e,t){e.state.delayingBlurEvent||(e.state.focused&&(pe(e,"blur",e,t),e.state.focused=!1,T(e.display.wrapper,"CodeMirror-focused")),clearInterval(e.display.blinker),setTimeout((function(){e.state.focused||(e.display.shift=!1)}),150))}function Tr(e){for(var t=e.display,n=t.lineDiv.offsetTop,r=0;r<t.view.length;r++){var i=t.view[r],o=e.options.lineWrapping,s=void 0,c=0;if(!i.hidden){if(a&&l<8){var u=i.node.offsetTop+i.node.offsetHeight;s=u-n,n=u}else{var d=i.node.getBoundingClientRect();s=d.bottom-d.top,!o&&i.text.firstChild&&(c=i.text.firstChild.getBoundingClientRect().right-d.left-1)}var h=i.line.height-s;if((h>.005||h<-.005)&&(Xe(i.line,s),Mr(i.line),i.rest))for(var f=0;f<i.rest.length;f++)Mr(i.rest[f]);if(c>e.display.sizerWidth){var p=Math.ceil(c/ir(e.display));p>e.display.maxLineLength&&(e.display.maxLineLength=p,e.display.maxLine=i.line,e.display.maxLineChanged=!0)}}}}function Mr(e){if(e.widgets)for(var t=0;t<e.widgets.length;++t){var n=e.widgets[t],r=n.node.parentNode;r&&(n.height=r.offsetHeight)}}function Ar(e,t,n){var r=n&&null!=n.top?Math.max(0,n.top):e.scroller.scrollTop;r=Math.floor(r-Cn(e));var i=n&&null!=n.bottom?n.bottom:r+e.wrapper.clientHeight,o=Ye(t,r),a=Ye(t,i);if(n&&n.ensure){var l=n.ensure.from.line,s=n.ensure.to.line;l<o?(o=l,a=Ye(t,qt(Ge(t,l))+e.wrapper.clientHeight)):Math.min(s,t.lastLine())>=a&&(o=Ye(t,qt(Ge(t,s))-e.wrapper.clientHeight),a=s)}return{from:o,to:Math.max(a,o+1)}}function Nr(e,t){var n=e.display,r=rr(e.display);t.top<0&&(t.top=0);var i=e.curOp&&null!=e.curOp.scrollTop?e.curOp.scrollTop:n.scroller.scrollTop,o=An(e),a={};t.bottom-t.top>o&&(t.bottom=t.top+o);var l=e.doc.height+Sn(n),s=t.top<r,c=t.bottom>l-r;if(t.top<i)a.scrollTop=s?0:t.top;else if(t.bottom>i+o){var u=Math.min(t.top,(c?l:t.bottom)-o);u!=i&&(a.scrollTop=u)}var d=e.curOp&&null!=e.curOp.scrollLeft?e.curOp.scrollLeft:n.scroller.scrollLeft,h=Mn(e)-(e.options.fixedGutter?n.gutters.offsetWidth:0),f=t.right-t.left>h;return f&&(t.right=t.left+h),t.left<10?a.scrollLeft=0:t.left<d?a.scrollLeft=Math.max(0,t.left-(f?0:10)):t.right>h+d-3&&(a.scrollLeft=t.right+(f?0:10)-h),a}function Dr(e,t){null!=t&&(Or(e),e.curOp.scrollTop=(null==e.curOp.scrollTop?e.doc.scrollTop:e.curOp.scrollTop)+t)}function Fr(e){Or(e);var t=e.getCursor();e.curOp.scrollToPos={from:t,to:t,margin:e.options.cursorScrollMargin}}function Er(e,t,n){null==t&&null==n||Or(e),null!=t&&(e.curOp.scrollLeft=t),null!=n&&(e.curOp.scrollTop=n)}function Or(e){var t=e.curOp.scrollToPos;t&&(e.curOp.scrollToPos=null,Ir(e,Kn(e,t.from),Kn(e,t.to),t.margin))}function Ir(e,t,n,r){var i=Nr(e,{left:Math.min(t.left,n.left),top:Math.min(t.top,n.top)-r,right:Math.max(t.right,n.right),bottom:Math.max(t.bottom,n.bottom)+r});Er(e,i.scrollLeft,i.scrollTop)}function zr(e,t){Math.abs(e.doc.scrollTop-t)<2||(n||si(e,{top:t}),Hr(e,t,!0),n&&si(e),ri(e,100))}function Hr(e,t,n){t=Math.max(0,Math.min(e.display.scroller.scrollHeight-e.display.scroller.clientHeight,t)),(e.display.scroller.scrollTop!=t||n)&&(e.doc.scrollTop=t,e.display.scrollbars.setScrollTop(t),e.display.scroller.scrollTop!=t&&(e.display.scroller.scrollTop=t))}function Rr(e,t,n,r){t=Math.max(0,Math.min(t,e.display.scroller.scrollWidth-e.display.scroller.clientWidth)),(n?t==e.doc.scrollLeft:Math.abs(e.doc.scrollLeft-t)<2)&&!r||(e.doc.scrollLeft=t,di(e),e.display.scroller.scrollLeft!=t&&(e.display.scroller.scrollLeft=t),e.display.scrollbars.setScrollLeft(t))}function _r(e){var t=e.display,n=t.gutters.offsetWidth,r=Math.round(e.doc.height+Sn(e.display));return{clientHeight:t.scroller.clientHeight,viewHeight:t.wrapper.clientHeight,scrollWidth:t.scroller.scrollWidth,clientWidth:t.scroller.clientWidth,viewWidth:t.wrapper.clientWidth,barLeft:e.options.fixedGutter?n:0,docHeight:r,scrollHeight:r+Tn(e)+t.barHeight,nativeBarWidth:t.nativeBarWidth,gutterWidth:n}}var Pr=function(e,t,n){this.cm=n;var r=this.vert=N("div",[N("div",null,null,"min-width: 1px")],"CodeMirror-vscrollbar"),i=this.horiz=N("div",[N("div",null,null,"height: 100%; min-height: 1px")],"CodeMirror-hscrollbar");r.tabIndex=i.tabIndex=-1,e(r),e(i),de(r,"scroll",(function(){r.clientHeight&&t(r.scrollTop,"vertical")})),de(i,"scroll",(function(){i.clientWidth&&t(i.scrollLeft,"horizontal")})),this.checkedZeroWidth=!1,a&&l<8&&(this.horiz.style.minHeight=this.vert.style.minWidth="18px")};Pr.prototype.update=function(e){var t=e.scrollWidth>e.clientWidth+1,n=e.scrollHeight>e.clientHeight+1,r=e.nativeBarWidth;if(n){this.vert.style.display="block",this.vert.style.bottom=t?r+"px":"0";var i=e.viewHeight-(t?r:0);this.vert.firstChild.style.height=Math.max(0,e.scrollHeight-e.clientHeight+i)+"px"}else this.vert.style.display="",this.vert.firstChild.style.height="0";if(t){this.horiz.style.display="block",this.horiz.style.right=n?r+"px":"0",this.horiz.style.left=e.barLeft+"px";var o=e.viewWidth-e.barLeft-(n?r:0);this.horiz.firstChild.style.width=Math.max(0,e.scrollWidth-e.clientWidth+o)+"px"}else this.horiz.style.display="",this.horiz.firstChild.style.width="0";return!this.checkedZeroWidth&&e.clientHeight>0&&(0==r&&this.zeroWidthHack(),this.checkedZeroWidth=!0),{right:n?r:0,bottom:t?r:0}},Pr.prototype.setScrollLeft=function(e){this.horiz.scrollLeft!=e&&(this.horiz.scrollLeft=e),this.disableHoriz&&this.enableZeroWidthBar(this.horiz,this.disableHoriz,"horiz")},Pr.prototype.setScrollTop=function(e){this.vert.scrollTop!=e&&(this.vert.scrollTop=e),this.disableVert&&this.enableZeroWidthBar(this.vert,this.disableVert,"vert")},Pr.prototype.zeroWidthHack=function(){var e=x&&!f?"12px":"18px";this.horiz.style.height=this.vert.style.width=e,this.horiz.style.pointerEvents=this.vert.style.pointerEvents="none",this.disableHoriz=new P,this.disableVert=new P},Pr.prototype.enableZeroWidthBar=function(e,t,n){e.style.pointerEvents="auto",t.set(1e3,(function r(){var i=e.getBoundingClientRect();("vert"==n?document.elementFromPoint(i.right-1,(i.top+i.bottom)/2):document.elementFromPoint((i.right+i.left)/2,i.bottom-1))!=e?e.style.pointerEvents="none":t.set(1e3,r)}))},Pr.prototype.clear=function(){var e=this.horiz.parentNode;e.removeChild(this.horiz),e.removeChild(this.vert)};var Wr=function(){};function Br(e,t){t||(t=_r(e));var n=e.display.barWidth,r=e.display.barHeight;jr(e,t);for(var i=0;i<4&&n!=e.display.barWidth||r!=e.display.barHeight;i++)n!=e.display.barWidth&&e.options.lineWrapping&&Tr(e),jr(e,_r(e)),n=e.display.barWidth,r=e.display.barHeight}function jr(e,t){var n=e.display,r=n.scrollbars.update(t);n.sizer.style.paddingRight=(n.barWidth=r.right)+"px",n.sizer.style.paddingBottom=(n.barHeight=r.bottom)+"px",n.heightForcer.style.borderBottom=r.bottom+"px solid transparent",r.right&&r.bottom?(n.scrollbarFiller.style.display="block",n.scrollbarFiller.style.height=r.bottom+"px",n.scrollbarFiller.style.width=r.right+"px"):n.scrollbarFiller.style.display="",r.bottom&&e.options.coverGutterNextToScrollbar&&e.options.fixedGutter?(n.gutterFiller.style.display="block",n.gutterFiller.style.height=r.bottom+"px",n.gutterFiller.style.width=t.gutterWidth+"px"):n.gutterFiller.style.display=""}Wr.prototype.update=function(){return{bottom:0,right:0}},Wr.prototype.setScrollLeft=function(){},Wr.prototype.setScrollTop=function(){},Wr.prototype.clear=function(){};var qr={native:Pr,null:Wr};function Ur(e){e.display.scrollbars&&(e.display.scrollbars.clear(),e.display.scrollbars.addClass&&T(e.display.wrapper,e.display.scrollbars.addClass)),e.display.scrollbars=new qr[e.options.scrollbarStyle]((function(t){e.display.wrapper.insertBefore(t,e.display.scrollbarFiller),de(t,"mousedown",(function(){e.state.focused&&setTimeout((function(){return e.display.input.focus()}),0)})),t.setAttribute("cm-not-content","true")}),(function(t,n){"horizontal"==n?Rr(e,t):zr(e,t)}),e),e.display.scrollbars.addClass&&O(e.display.wrapper,e.display.scrollbars.addClass)}var $r=0;function Gr(e){var t;e.curOp={cm:e,viewChanged:!1,startHeight:e.doc.height,forceUpdate:!1,updateInput:0,typing:!1,changeObjs:null,cursorActivityHandlers:null,cursorActivityCalled:0,selectionChanged:!1,updateMaxLine:!1,scrollLeft:null,scrollTop:null,scrollToPos:null,focus:!1,id:++$r},t=e.curOp,an?an.ops.push(t):t.ownsGroup=an={ops:[t],delayedCallbacks:[]}}function Vr(e){var t=e.curOp;t&&function(e,t){var n=e.ownsGroup;if(n)try{!function(e){var t=e.delayedCallbacks,n=0;do{for(;n<t.length;n++)t[n].call(null);for(var r=0;r<e.ops.length;r++){var i=e.ops[r];if(i.cursorActivityHandlers)for(;i.cursorActivityCalled<i.cursorActivityHandlers.length;)i.cursorActivityHandlers[i.cursorActivityCalled++].call(null,i.cm)}}while(n<t.length)}(n)}finally{an=null,t(n)}}(t,(function(e){for(var t=0;t<e.ops.length;t++)e.ops[t].cm.curOp=null;!function(e){for(var t=e.ops,n=0;n<t.length;n++)Kr(t[n]);for(var r=0;r<t.length;r++)Xr(t[r]);for(var i=0;i<t.length;i++)Zr(t[i]);for(var o=0;o<t.length;o++)Yr(t[o]);for(var a=0;a<t.length;a++)Qr(t[a])}(e)}))}function Kr(e){var t=e.cm,n=t.display;!function(e){var t=e.display;!t.scrollbarsClipped&&t.scroller.offsetWidth&&(t.nativeBarWidth=t.scroller.offsetWidth-t.scroller.clientWidth,t.heightForcer.style.height=Tn(e)+"px",t.sizer.style.marginBottom=-t.nativeBarWidth+"px",t.sizer.style.borderRightWidth=Tn(e)+"px",t.scrollbarsClipped=!0)}(t),e.updateMaxLine&&$t(t),e.mustUpdate=e.viewChanged||e.forceUpdate||null!=e.scrollTop||e.scrollToPos&&(e.scrollToPos.from.line<n.viewFrom||e.scrollToPos.to.line>=n.viewTo)||n.maxLineChanged&&t.options.lineWrapping,e.update=e.mustUpdate&&new oi(t,e.mustUpdate&&{top:e.scrollTop,ensure:e.scrollToPos},e.forceUpdate)}function Xr(e){e.updatedDisplay=e.mustUpdate&&ai(e.cm,e.update)}function Zr(e){var t=e.cm,n=t.display;e.updatedDisplay&&Tr(t),e.barMeasure=_r(t),n.maxLineChanged&&!t.options.lineWrapping&&(e.adjustWidthTo=Dn(t,n.maxLine,n.maxLine.text.length).left+3,t.display.sizerWidth=e.adjustWidthTo,e.barMeasure.scrollWidth=Math.max(n.scroller.clientWidth,n.sizer.offsetLeft+e.adjustWidthTo+Tn(t)+t.display.barWidth),e.maxScrollLeft=Math.max(0,n.sizer.offsetLeft+e.adjustWidthTo-Mn(t))),(e.updatedDisplay||e.selectionChanged)&&(e.preparedSelection=n.input.prepareSelection())}function Yr(e){var t=e.cm;null!=e.adjustWidthTo&&(t.display.sizer.style.minWidth=e.adjustWidthTo+"px",e.maxScrollLeft<t.doc.scrollLeft&&Rr(t,Math.min(t.display.scroller.scrollLeft,e.maxScrollLeft),!0),t.display.maxLineChanged=!1);var n=e.focus&&e.focus==E();e.preparedSelection&&t.display.input.showSelection(e.preparedSelection,n),(e.updatedDisplay||e.startHeight!=t.doc.height)&&Br(t,e.barMeasure),e.updatedDisplay&&ui(t,e.barMeasure),e.selectionChanged&&wr(t),t.state.focused&&e.updateInput&&t.display.input.reset(e.typing),n&&kr(e.cm)}function Qr(e){var t=e.cm,n=t.display,r=t.doc;(e.updatedDisplay&&li(t,e.update),null==n.wheelStartX||null==e.scrollTop&&null==e.scrollLeft&&!e.scrollToPos||(n.wheelStartX=n.wheelStartY=null),null!=e.scrollTop&&Hr(t,e.scrollTop,e.forceScroll),null!=e.scrollLeft&&Rr(t,e.scrollLeft,!0,!0),e.scrollToPos)&&function(e,t){if(!me(e,"scrollCursorIntoView")){var n=e.display,r=n.sizer.getBoundingClientRect(),i=null;if(t.top+r.top<0?i=!0:t.bottom+r.top>(window.innerHeight||document.documentElement.clientHeight)&&(i=!1),null!=i&&!p){var o=N("div","​",null,"position: absolute;\n                         top: "+(t.top-n.viewOffset-Cn(e.display))+"px;\n                         height: "+(t.bottom-t.top+Tn(e)+n.barHeight)+"px;\n                         left: "+t.left+"px; width: "+Math.max(2,t.right-t.left)+"px;");e.display.lineSpace.appendChild(o),o.scrollIntoView(i),e.display.lineSpace.removeChild(o)}}}(t,function(e,t,n,r){var i;null==r&&(r=0),e.options.lineWrapping||t!=n||(n="before"==(t=t.ch?et(t.line,"before"==t.sticky?t.ch-1:t.ch,"after"):t).sticky?et(t.line,t.ch+1,"before"):t);for(var o=0;o<5;o++){var a=!1,l=Vn(e,t),s=n&&n!=t?Vn(e,n):l,c=Nr(e,i={left:Math.min(l.left,s.left),top:Math.min(l.top,s.top)-r,right:Math.max(l.left,s.left),bottom:Math.max(l.bottom,s.bottom)+r}),u=e.doc.scrollTop,d=e.doc.scrollLeft;if(null!=c.scrollTop&&(zr(e,c.scrollTop),Math.abs(e.doc.scrollTop-u)>1&&(a=!0)),null!=c.scrollLeft&&(Rr(e,c.scrollLeft),Math.abs(e.doc.scrollLeft-d)>1&&(a=!0)),!a)break}return i}(t,lt(r,e.scrollToPos.from),lt(r,e.scrollToPos.to),e.scrollToPos.margin));var i=e.maybeHiddenMarkers,o=e.maybeUnhiddenMarkers;if(i)for(var a=0;a<i.length;++a)i[a].lines.length||pe(i[a],"hide");if(o)for(var l=0;l<o.length;++l)o[l].lines.length&&pe(o[l],"unhide");n.wrapper.offsetHeight&&(r.scrollTop=t.display.scroller.scrollTop),e.changeObjs&&pe(t,"changes",t,e.changeObjs),e.update&&e.update.finish()}function Jr(e,t){if(e.curOp)return t();Gr(e);try{return t()}finally{Vr(e)}}function ei(e,t){return function(){if(e.curOp)return t.apply(e,arguments);Gr(e);try{return t.apply(e,arguments)}finally{Vr(e)}}}function ti(e){return function(){if(this.curOp)return e.apply(this,arguments);Gr(this);try{return e.apply(this,arguments)}finally{Vr(this)}}}function ni(e){return function(){var t=this.cm;if(!t||t.curOp)return e.apply(this,arguments);Gr(t);try{return e.apply(this,arguments)}finally{Vr(t)}}}function ri(e,t){e.doc.highlightFrontier<e.display.viewTo&&e.state.highlight.set(t,H(ii,e))}function ii(e){var t=e.doc;if(!(t.highlightFrontier>=e.display.viewTo)){var n=+new Date+e.options.workTime,r=ft(e,t.highlightFrontier),i=[];t.iter(r.line,Math.min(t.first+t.size,e.display.viewTo+500),(function(o){if(r.line>=e.display.viewFrom){var a=o.styles,l=o.text.length>e.options.maxHighlightLength?je(t.mode,r.state):null,s=dt(e,o,r,!0);l&&(r.state=l),o.styles=s.styles;var c=o.styleClasses,u=s.classes;u?o.styleClasses=u:c&&(o.styleClasses=null);for(var d=!a||a.length!=o.styles.length||c!=u&&(!c||!u||c.bgClass!=u.bgClass||c.textClass!=u.textClass),h=0;!d&&h<a.length;++h)d=a[h]!=o.styles[h];d&&i.push(r.line),o.stateAfter=r.save(),r.nextLine()}else o.text.length<=e.options.maxHighlightLength&&pt(e,o.text,r),o.stateAfter=r.line%5==0?r.save():null,r.nextLine();if(+new Date>n)return ri(e,e.options.workDelay),!0})),t.highlightFrontier=r.line,t.modeFrontier=Math.max(t.modeFrontier,r.line),i.length&&Jr(e,(function(){for(var t=0;t<i.length;t++)hr(e,i[t],"text")}))}}var oi=function(e,t,n){var r=e.display;this.viewport=t,this.visible=Ar(r,e.doc,t),this.editorIsHidden=!r.wrapper.offsetWidth,this.wrapperHeight=r.wrapper.clientHeight,this.wrapperWidth=r.wrapper.clientWidth,this.oldDisplayWidth=Mn(e),this.force=n,this.dims=or(e),this.events=[]};function ai(e,t){var n=e.display,r=e.doc;if(t.editorIsHidden)return fr(e),!1;if(!t.force&&t.visible.from>=n.viewFrom&&t.visible.to<=n.viewTo&&(null==n.updateLineNumbers||n.updateLineNumbers>=n.viewTo)&&n.renderedView==n.view&&0==mr(e))return!1;hi(e)&&(fr(e),t.dims=or(e));var i=r.first+r.size,o=Math.max(t.visible.from-e.options.viewportMargin,r.first),a=Math.min(i,t.visible.to+e.options.viewportMargin);n.viewFrom<o&&o-n.viewFrom<20&&(o=Math.max(r.first,n.viewFrom)),n.viewTo>a&&n.viewTo-a<20&&(a=Math.min(i,n.viewTo)),kt&&(o=Pt(e.doc,o),a=Wt(e.doc,a));var l=o!=n.viewFrom||a!=n.viewTo||n.lastWrapHeight!=t.wrapperHeight||n.lastWrapWidth!=t.wrapperWidth;!function(e,t,n){var r=e.display;0==r.view.length||t>=r.viewTo||n<=r.viewFrom?(r.view=on(e,t,n),r.viewFrom=t):(r.viewFrom>t?r.view=on(e,t,r.viewFrom).concat(r.view):r.viewFrom<t&&(r.view=r.view.slice(ur(e,t))),r.viewFrom=t,r.viewTo<n?r.view=r.view.concat(on(e,r.viewTo,n)):r.viewTo>n&&(r.view=r.view.slice(0,ur(e,n)))),r.viewTo=n}(e,o,a),n.viewOffset=qt(Ge(e.doc,n.viewFrom)),e.display.mover.style.top=n.viewOffset+"px";var c=mr(e);if(!l&&0==c&&!t.force&&n.renderedView==n.view&&(null==n.updateLineNumbers||n.updateLineNumbers>=n.viewTo))return!1;var u=function(e){if(e.hasFocus())return null;var t=E();if(!t||!F(e.display.lineDiv,t))return null;var n={activeElt:t};if(window.getSelection){var r=window.getSelection();r.anchorNode&&r.extend&&F(e.display.lineDiv,r.anchorNode)&&(n.anchorNode=r.anchorNode,n.anchorOffset=r.anchorOffset,n.focusNode=r.focusNode,n.focusOffset=r.focusOffset)}return n}(e);return c>4&&(n.lineDiv.style.display="none"),function(e,t,n){var r=e.display,i=e.options.lineNumbers,o=r.lineDiv,a=o.firstChild;function l(t){var n=t.nextSibling;return s&&x&&e.display.currentWheelTarget==t?t.style.display="none":t.parentNode.removeChild(t),n}for(var c=r.view,u=r.viewFrom,d=0;d<c.length;d++){var h=c[d];if(h.hidden);else if(h.node&&h.node.parentNode==o){for(;a!=h.node;)a=l(a);var f=i&&null!=t&&t<=u&&h.lineNumber;h.changes&&(W(h.changes,"gutter")>-1&&(f=!1),un(e,h,u,n)),f&&(M(h.lineNumber),h.lineNumber.appendChild(document.createTextNode(Je(e.options,u)))),a=h.node.nextSibling}else{var p=vn(e,h,u,n);o.insertBefore(p,a)}u+=h.size}for(;a;)a=l(a)}(e,n.updateLineNumbers,t.dims),c>4&&(n.lineDiv.style.display=""),n.renderedView=n.view,function(e){if(e&&e.activeElt&&e.activeElt!=E()&&(e.activeElt.focus(),!/^(INPUT|TEXTAREA)$/.test(e.activeElt.nodeName)&&e.anchorNode&&F(document.body,e.anchorNode)&&F(document.body,e.focusNode))){var t=window.getSelection(),n=document.createRange();n.setEnd(e.anchorNode,e.anchorOffset),n.collapse(!1),t.removeAllRanges(),t.addRange(n),t.extend(e.focusNode,e.focusOffset)}}(u),M(n.cursorDiv),M(n.selectionDiv),n.gutters.style.height=n.sizer.style.minHeight=0,l&&(n.lastWrapHeight=t.wrapperHeight,n.lastWrapWidth=t.wrapperWidth,ri(e,400)),n.updateLineNumbers=null,!0}function li(e,t){for(var n=t.viewport,r=!0;;r=!1){if(r&&e.options.lineWrapping&&t.oldDisplayWidth!=Mn(e))r&&(t.visible=Ar(e.display,e.doc,n));else if(n&&null!=n.top&&(n={top:Math.min(e.doc.height+Sn(e.display)-An(e),n.top)}),t.visible=Ar(e.display,e.doc,n),t.visible.from>=e.display.viewFrom&&t.visible.to<=e.display.viewTo)break;if(!ai(e,t))break;Tr(e);var i=_r(e);gr(e),Br(e,i),ui(e,i),t.force=!1}t.signal(e,"update",e),e.display.viewFrom==e.display.reportedViewFrom&&e.display.viewTo==e.display.reportedViewTo||(t.signal(e,"viewportChange",e,e.display.viewFrom,e.display.viewTo),e.display.reportedViewFrom=e.display.viewFrom,e.display.reportedViewTo=e.display.viewTo)}function si(e,t){var n=new oi(e,t);if(ai(e,n)){Tr(e),li(e,n);var r=_r(e);gr(e),Br(e,r),ui(e,r),n.finish()}}function ci(e){var t=e.gutters.offsetWidth;e.sizer.style.marginLeft=t+"px"}function ui(e,t){e.display.sizer.style.minHeight=t.docHeight+"px",e.display.heightForcer.style.top=t.docHeight+"px",e.display.gutters.style.height=t.docHeight+e.display.barHeight+Tn(e)+"px"}function di(e){var t=e.display,n=t.view;if(t.alignWidgets||t.gutters.firstChild&&e.options.fixedGutter){for(var r=ar(t)-t.scroller.scrollLeft+e.doc.scrollLeft,i=t.gutters.offsetWidth,o=r+"px",a=0;a<n.length;a++)if(!n[a].hidden){e.options.fixedGutter&&(n[a].gutter&&(n[a].gutter.style.left=o),n[a].gutterBackground&&(n[a].gutterBackground.style.left=o));var l=n[a].alignable;if(l)for(var s=0;s<l.length;s++)l[s].style.left=o}e.options.fixedGutter&&(t.gutters.style.left=r+i+"px")}}function hi(e){if(!e.options.lineNumbers)return!1;var t=e.doc,n=Je(e.options,t.first+t.size-1),r=e.display;if(n.length!=r.lineNumChars){var i=r.measure.appendChild(N("div",[N("div",n)],"CodeMirror-linenumber CodeMirror-gutter-elt")),o=i.firstChild.offsetWidth,a=i.offsetWidth-o;return r.lineGutter.style.width="",r.lineNumInnerWidth=Math.max(o,r.lineGutter.offsetWidth-a)+1,r.lineNumWidth=r.lineNumInnerWidth+a,r.lineNumChars=r.lineNumInnerWidth?n.length:-1,r.lineGutter.style.width=r.lineNumWidth+"px",ci(e.display),!0}return!1}function fi(e,t){for(var n=[],r=!1,i=0;i<e.length;i++){var o=e[i],a=null;if("string"!=typeof o&&(a=o.style,o=o.className),"CodeMirror-linenumbers"==o){if(!t)continue;r=!0}n.push({className:o,style:a})}return t&&!r&&n.push({className:"CodeMirror-linenumbers",style:null}),n}function pi(e){var t=e.gutters,n=e.gutterSpecs;M(t),e.lineGutter=null;for(var r=0;r<n.length;++r){var i=n[r],o=i.className,a=i.style,l=t.appendChild(N("div",null,"CodeMirror-gutter "+o));a&&(l.style.cssText=a),"CodeMirror-linenumbers"==o&&(e.lineGutter=l,l.style.width=(e.lineNumWidth||1)+"px")}t.style.display=n.length?"":"none",ci(e)}function mi(e){pi(e.display),dr(e),di(e)}function gi(e,t,r,i){var o=this;this.input=r,o.scrollbarFiller=N("div",null,"CodeMirror-scrollbar-filler"),o.scrollbarFiller.setAttribute("cm-not-content","true"),o.gutterFiller=N("div",null,"CodeMirror-gutter-filler"),o.gutterFiller.setAttribute("cm-not-content","true"),o.lineDiv=D("div",null,"CodeMirror-code"),o.selectionDiv=N("div",null,null,"position: relative; z-index: 1"),o.cursorDiv=N("div",null,"CodeMirror-cursors"),o.measure=N("div",null,"CodeMirror-measure"),o.lineMeasure=N("div",null,"CodeMirror-measure"),o.lineSpace=D("div",[o.measure,o.lineMeasure,o.selectionDiv,o.cursorDiv,o.lineDiv],null,"position: relative; outline: none");var c=D("div",[o.lineSpace],"CodeMirror-lines");o.mover=N("div",[c],null,"position: relative"),o.sizer=N("div",[o.mover],"CodeMirror-sizer"),o.sizerWidth=null,o.heightForcer=N("div",null,null,"position: absolute; height: 50px; width: 1px;"),o.gutters=N("div",null,"CodeMirror-gutters"),o.lineGutter=null,o.scroller=N("div",[o.sizer,o.heightForcer,o.gutters],"CodeMirror-scroll"),o.scroller.setAttribute("tabIndex","-1"),o.wrapper=N("div",[o.scrollbarFiller,o.gutterFiller,o.scroller],"CodeMirror"),a&&l<8&&(o.gutters.style.zIndex=-1,o.scroller.style.paddingRight=0),s||n&&v||(o.scroller.draggable=!0),e&&(e.appendChild?e.appendChild(o.wrapper):e(o.wrapper)),o.viewFrom=o.viewTo=t.first,o.reportedViewFrom=o.reportedViewTo=t.first,o.view=[],o.renderedView=null,o.externalMeasured=null,o.viewOffset=0,o.lastWrapHeight=o.lastWrapWidth=0,o.updateLineNumbers=null,o.nativeBarWidth=o.barHeight=o.barWidth=0,o.scrollbarsClipped=!1,o.lineNumWidth=o.lineNumInnerWidth=o.lineNumChars=null,o.alignWidgets=!1,o.cachedCharWidth=o.cachedTextHeight=o.cachedPaddingH=null,o.maxLine=null,o.maxLineLength=0,o.maxLineChanged=!1,o.wheelDX=o.wheelDY=o.wheelStartX=o.wheelStartY=null,o.shift=!1,o.selForContextMenu=null,o.activeTouch=null,o.gutterSpecs=fi(i.gutters,i.lineNumbers),pi(o),r.init(o)}oi.prototype.signal=function(e,t){ve(e,t)&&this.events.push(arguments)},oi.prototype.finish=function(){for(var e=0;e<this.events.length;e++)pe.apply(null,this.events[e])};var vi=0,xi=null;function yi(e){var t=e.wheelDeltaX,n=e.wheelDeltaY;return null==t&&e.detail&&e.axis==e.HORIZONTAL_AXIS&&(t=e.detail),null==n&&e.detail&&e.axis==e.VERTICAL_AXIS?n=e.detail:null==n&&(n=e.wheelDelta),{x:t,y:n}}function bi(e){var t=yi(e);return t.x*=xi,t.y*=xi,t}function wi(e,t){var r=yi(t),i=r.x,o=r.y,a=e.display,l=a.scroller,c=l.scrollWidth>l.clientWidth,u=l.scrollHeight>l.clientHeight;if(i&&c||o&&u){if(o&&x&&s)e:for(var h=t.target,f=a.view;h!=l;h=h.parentNode)for(var p=0;p<f.length;p++)if(f[p].node==h){e.display.currentWheelTarget=h;break e}if(i&&!n&&!d&&null!=xi)return o&&u&&zr(e,Math.max(0,l.scrollTop+o*xi)),Rr(e,Math.max(0,l.scrollLeft+i*xi)),(!o||o&&u)&&ye(t),void(a.wheelStartX=null);if(o&&null!=xi){var m=o*xi,g=e.doc.scrollTop,v=g+a.wrapper.clientHeight;m<0?g=Math.max(0,g+m-50):v=Math.min(e.doc.height,v+m+50),si(e,{top:g,bottom:v})}vi<20&&(null==a.wheelStartX?(a.wheelStartX=l.scrollLeft,a.wheelStartY=l.scrollTop,a.wheelDX=i,a.wheelDY=o,setTimeout((function(){if(null!=a.wheelStartX){var e=l.scrollLeft-a.wheelStartX,t=l.scrollTop-a.wheelStartY,n=t&&a.wheelDY&&t/a.wheelDY||e&&a.wheelDX&&e/a.wheelDX;a.wheelStartX=a.wheelStartY=null,n&&(xi=(xi*vi+n)/(vi+1),++vi)}}),200)):(a.wheelDX+=i,a.wheelDY+=o))}}a?xi=-.53:n?xi=15:u?xi=-.7:h&&(xi=-1/3);var ki=function(e,t){this.ranges=e,this.primIndex=t};ki.prototype.primary=function(){return this.ranges[this.primIndex]},ki.prototype.equals=function(e){if(e==this)return!0;if(e.primIndex!=this.primIndex||e.ranges.length!=this.ranges.length)return!1;for(var t=0;t<this.ranges.length;t++){var n=this.ranges[t],r=e.ranges[t];if(!nt(n.anchor,r.anchor)||!nt(n.head,r.head))return!1}return!0},ki.prototype.deepCopy=function(){for(var e=[],t=0;t<this.ranges.length;t++)e[t]=new Ci(rt(this.ranges[t].anchor),rt(this.ranges[t].head));return new ki(e,this.primIndex)},ki.prototype.somethingSelected=function(){for(var e=0;e<this.ranges.length;e++)if(!this.ranges[e].empty())return!0;return!1},ki.prototype.contains=function(e,t){t||(t=e);for(var n=0;n<this.ranges.length;n++){var r=this.ranges[n];if(tt(t,r.from())>=0&&tt(e,r.to())<=0)return n}return-1};var Ci=function(e,t){this.anchor=e,this.head=t};function Si(e,t,n){var r=e&&e.options.selectionsMayTouch,i=t[n];t.sort((function(e,t){return tt(e.from(),t.from())})),n=W(t,i);for(var o=1;o<t.length;o++){var a=t[o],l=t[o-1],s=tt(l.to(),a.from());if(r&&!a.empty()?s>0:s>=0){var c=ot(l.from(),a.from()),u=it(l.to(),a.to()),d=l.empty()?a.from()==a.head:l.from()==l.head;o<=n&&--n,t.splice(--o,2,new Ci(d?u:c,d?c:u))}}return new ki(t,n)}function Li(e,t){return new ki([new Ci(e,t||e)],0)}function Ti(e){return e.text?et(e.from.line+e.text.length-1,K(e.text).length+(1==e.text.length?e.from.ch:0)):e.to}function Mi(e,t){if(tt(e,t.from)<0)return e;if(tt(e,t.to)<=0)return Ti(t);var n=e.line+t.text.length-(t.to.line-t.from.line)-1,r=e.ch;return e.line==t.to.line&&(r+=Ti(t).ch-t.to.ch),et(n,r)}function Ai(e,t){for(var n=[],r=0;r<e.sel.ranges.length;r++){var i=e.sel.ranges[r];n.push(new Ci(Mi(i.anchor,t),Mi(i.head,t)))}return Si(e.cm,n,e.sel.primIndex)}function Ni(e,t,n){return e.line==t.line?et(n.line,e.ch-t.ch+n.ch):et(n.line+(e.line-t.line),e.ch)}function Di(e){e.doc.mode=Pe(e.options,e.doc.modeOption),Fi(e)}function Fi(e){e.doc.iter((function(e){e.stateAfter&&(e.stateAfter=null),e.styles&&(e.styles=null)})),e.doc.modeFrontier=e.doc.highlightFrontier=e.doc.first,ri(e,100),e.state.modeGen++,e.curOp&&dr(e)}function Ei(e,t){return 0==t.from.ch&&0==t.to.ch&&""==K(t.text)&&(!e.cm||e.cm.options.wholeLineUpdateBefore)}function Oi(e,t,n,r){function i(e){return n?n[e]:null}function o(e,n,i){!function(e,t,n,r){e.text=t,e.stateAfter&&(e.stateAfter=null),e.styles&&(e.styles=null),null!=e.order&&(e.order=null),At(e),Nt(e,n);var i=r?r(e):1;i!=e.height&&Xe(e,i)}(e,n,i,r),sn(e,"change",e,t)}function a(e,t){for(var n=[],o=e;o<t;++o)n.push(new Gt(c[o],i(o),r));return n}var l=t.from,s=t.to,c=t.text,u=Ge(e,l.line),d=Ge(e,s.line),h=K(c),f=i(c.length-1),p=s.line-l.line;if(t.full)e.insert(0,a(0,c.length)),e.remove(c.length,e.size-c.length);else if(Ei(e,t)){var m=a(0,c.length-1);o(d,d.text,f),p&&e.remove(l.line,p),m.length&&e.insert(l.line,m)}else if(u==d)if(1==c.length)o(u,u.text.slice(0,l.ch)+h+u.text.slice(s.ch),f);else{var g=a(1,c.length-1);g.push(new Gt(h+u.text.slice(s.ch),f,r)),o(u,u.text.slice(0,l.ch)+c[0],i(0)),e.insert(l.line+1,g)}else if(1==c.length)o(u,u.text.slice(0,l.ch)+c[0]+d.text.slice(s.ch),i(0)),e.remove(l.line+1,p);else{o(u,u.text.slice(0,l.ch)+c[0],i(0)),o(d,h+d.text.slice(s.ch),f);var v=a(1,c.length-1);p>1&&e.remove(l.line+1,p-1),e.insert(l.line+1,v)}sn(e,"change",e,t)}function Ii(e,t,n){!function e(r,i,o){if(r.linked)for(var a=0;a<r.linked.length;++a){var l=r.linked[a];if(l.doc!=i){var s=o&&l.sharedHist;n&&!s||(t(l.doc,s),e(l.doc,r,s))}}}(e,null,!0)}function zi(e,t){if(t.cm)throw new Error("This document is already in use.");e.doc=t,t.cm=e,sr(e),Di(e),Hi(e),e.options.lineWrapping||$t(e),e.options.mode=t.modeOption,dr(e)}function Hi(e){("rtl"==e.doc.direction?O:T)(e.display.lineDiv,"CodeMirror-rtl")}function Ri(e){this.done=[],this.undone=[],this.undoDepth=1/0,this.lastModTime=this.lastSelTime=0,this.lastOp=this.lastSelOp=null,this.lastOrigin=this.lastSelOrigin=null,this.generation=this.maxGeneration=e||1}function _i(e,t){var n={from:rt(t.from),to:Ti(t),text:Ve(e,t.from,t.to)};return qi(e,n,t.from.line,t.to.line+1),Ii(e,(function(e){return qi(e,n,t.from.line,t.to.line+1)}),!0),n}function Pi(e){for(;e.length;){if(!K(e).ranges)break;e.pop()}}function Wi(e,t,n,r){var i=e.history;i.undone.length=0;var o,a,l=+new Date;if((i.lastOp==r||i.lastOrigin==t.origin&&t.origin&&("+"==t.origin.charAt(0)&&i.lastModTime>l-(e.cm?e.cm.options.historyEventDelay:500)||"*"==t.origin.charAt(0)))&&(o=function(e,t){return t?(Pi(e.done),K(e.done)):e.done.length&&!K(e.done).ranges?K(e.done):e.done.length>1&&!e.done[e.done.length-2].ranges?(e.done.pop(),K(e.done)):void 0}(i,i.lastOp==r)))a=K(o.changes),0==tt(t.from,t.to)&&0==tt(t.from,a.to)?a.to=Ti(t):o.changes.push(_i(e,t));else{var s=K(i.done);for(s&&s.ranges||ji(e.sel,i.done),o={changes:[_i(e,t)],generation:i.generation},i.done.push(o);i.done.length>i.undoDepth;)i.done.shift(),i.done[0].ranges||i.done.shift()}i.done.push(n),i.generation=++i.maxGeneration,i.lastModTime=i.lastSelTime=l,i.lastOp=i.lastSelOp=r,i.lastOrigin=i.lastSelOrigin=t.origin,a||pe(e,"historyAdded")}function Bi(e,t,n,r){var i=e.history,o=r&&r.origin;n==i.lastSelOp||o&&i.lastSelOrigin==o&&(i.lastModTime==i.lastSelTime&&i.lastOrigin==o||function(e,t,n,r){var i=t.charAt(0);return"*"==i||"+"==i&&n.ranges.length==r.ranges.length&&n.somethingSelected()==r.somethingSelected()&&new Date-e.history.lastSelTime<=(e.cm?e.cm.options.historyEventDelay:500)}(e,o,K(i.done),t))?i.done[i.done.length-1]=t:ji(t,i.done),i.lastSelTime=+new Date,i.lastSelOrigin=o,i.lastSelOp=n,r&&!1!==r.clearRedo&&Pi(i.undone)}function ji(e,t){var n=K(t);n&&n.ranges&&n.equals(e)||t.push(e)}function qi(e,t,n,r){var i=t["spans_"+e.id],o=0;e.iter(Math.max(e.first,n),Math.min(e.first+e.size,r),(function(n){n.markedSpans&&((i||(i=t["spans_"+e.id]={}))[o]=n.markedSpans),++o}))}function Ui(e){if(!e)return null;for(var t,n=0;n<e.length;++n)e[n].marker.explicitlyCleared?t||(t=e.slice(0,n)):t&&t.push(e[n]);return t?t.length?t:null:e}function $i(e,t){var n=function(e,t){var n=t["spans_"+e.id];if(!n)return null;for(var r=[],i=0;i<t.text.length;++i)r.push(Ui(n[i]));return r}(e,t),r=Tt(e,t);if(!n)return r;if(!r)return n;for(var i=0;i<n.length;++i){var o=n[i],a=r[i];if(o&&a)e:for(var l=0;l<a.length;++l){for(var s=a[l],c=0;c<o.length;++c)if(o[c].marker==s.marker)continue e;o.push(s)}else a&&(n[i]=a)}return n}function Gi(e,t,n){for(var r=[],i=0;i<e.length;++i){var o=e[i];if(o.ranges)r.push(n?ki.prototype.deepCopy.call(o):o);else{var a=o.changes,l=[];r.push({changes:l});for(var s=0;s<a.length;++s){var c=a[s],u=void 0;if(l.push({from:c.from,to:c.to,text:c.text}),t)for(var d in c)(u=d.match(/^spans_(\d+)$/))&&W(t,Number(u[1]))>-1&&(K(l)[d]=c[d],delete c[d])}}}return r}function Vi(e,t,n,r){if(r){var i=e.anchor;if(n){var o=tt(t,i)<0;o!=tt(n,i)<0?(i=t,t=n):o!=tt(t,n)<0&&(t=n)}return new Ci(i,t)}return new Ci(n||t,t)}function Ki(e,t,n,r,i){null==i&&(i=e.cm&&(e.cm.display.shift||e.extend)),Ji(e,new ki([Vi(e.sel.primary(),t,n,i)],0),r)}function Xi(e,t,n){for(var r=[],i=e.cm&&(e.cm.display.shift||e.extend),o=0;o<e.sel.ranges.length;o++)r[o]=Vi(e.sel.ranges[o],t[o],null,i);Ji(e,Si(e.cm,r,e.sel.primIndex),n)}function Zi(e,t,n,r){var i=e.sel.ranges.slice(0);i[t]=n,Ji(e,Si(e.cm,i,e.sel.primIndex),r)}function Yi(e,t,n,r){Ji(e,Li(t,n),r)}function Qi(e,t,n){var r=e.history.done,i=K(r);i&&i.ranges?(r[r.length-1]=t,eo(e,t,n)):Ji(e,t,n)}function Ji(e,t,n){eo(e,t,n),Bi(e,e.sel,e.cm?e.cm.curOp.id:NaN,n)}function eo(e,t,n){(ve(e,"beforeSelectionChange")||e.cm&&ve(e.cm,"beforeSelectionChange"))&&(t=function(e,t,n){var r={ranges:t.ranges,update:function(t){this.ranges=[];for(var n=0;n<t.length;n++)this.ranges[n]=new Ci(lt(e,t[n].anchor),lt(e,t[n].head))},origin:n&&n.origin};return pe(e,"beforeSelectionChange",e,r),e.cm&&pe(e.cm,"beforeSelectionChange",e.cm,r),r.ranges!=t.ranges?Si(e.cm,r.ranges,r.ranges.length-1):t}(e,t,n));var r=n&&n.bias||(tt(t.primary().head,e.sel.primary().head)<0?-1:1);to(e,ro(e,t,r,!0)),n&&!1===n.scroll||!e.cm||Fr(e.cm)}function to(e,t){t.equals(e.sel)||(e.sel=t,e.cm&&(e.cm.curOp.updateInput=1,e.cm.curOp.selectionChanged=!0,ge(e.cm)),sn(e,"cursorActivity",e))}function no(e){to(e,ro(e,e.sel,null,!1))}function ro(e,t,n,r){for(var i,o=0;o<t.ranges.length;o++){var a=t.ranges[o],l=t.ranges.length==e.sel.ranges.length&&e.sel.ranges[o],s=oo(e,a.anchor,l&&l.anchor,n,r),c=oo(e,a.head,l&&l.head,n,r);(i||s!=a.anchor||c!=a.head)&&(i||(i=t.ranges.slice(0,o)),i[o]=new Ci(s,c))}return i?Si(e.cm,i,t.primIndex):t}function io(e,t,n,r,i){var o=Ge(e,t.line);if(o.markedSpans)for(var a=0;a<o.markedSpans.length;++a){var l=o.markedSpans[a],s=l.marker,c="selectLeft"in s?!s.selectLeft:s.inclusiveLeft,u="selectRight"in s?!s.selectRight:s.inclusiveRight;if((null==l.from||(c?l.from<=t.ch:l.from<t.ch))&&(null==l.to||(u?l.to>=t.ch:l.to>t.ch))){if(i&&(pe(s,"beforeCursorEnter"),s.explicitlyCleared)){if(o.markedSpans){--a;continue}break}if(!s.atomic)continue;if(n){var d=s.find(r<0?1:-1),h=void 0;if((r<0?u:c)&&(d=ao(e,d,-r,d&&d.line==t.line?o:null)),d&&d.line==t.line&&(h=tt(d,n))&&(r<0?h<0:h>0))return io(e,d,t,r,i)}var f=s.find(r<0?-1:1);return(r<0?c:u)&&(f=ao(e,f,r,f.line==t.line?o:null)),f?io(e,f,t,r,i):null}}return t}function oo(e,t,n,r,i){var o=r||1,a=io(e,t,n,o,i)||!i&&io(e,t,n,o,!0)||io(e,t,n,-o,i)||!i&&io(e,t,n,-o,!0);return a||(e.cantEdit=!0,et(e.first,0))}function ao(e,t,n,r){return n<0&&0==t.ch?t.line>e.first?lt(e,et(t.line-1)):null:n>0&&t.ch==(r||Ge(e,t.line)).text.length?t.line<e.first+e.size-1?et(t.line+1,0):null:new et(t.line,t.ch+n)}function lo(e){e.setSelection(et(e.firstLine(),0),et(e.lastLine()),j)}function so(e,t,n){var r={canceled:!1,from:t.from,to:t.to,text:t.text,origin:t.origin,cancel:function(){return r.canceled=!0}};return n&&(r.update=function(t,n,i,o){t&&(r.from=lt(e,t)),n&&(r.to=lt(e,n)),i&&(r.text=i),void 0!==o&&(r.origin=o)}),pe(e,"beforeChange",e,r),e.cm&&pe(e.cm,"beforeChange",e.cm,r),r.canceled?(e.cm&&(e.cm.curOp.updateInput=2),null):{from:r.from,to:r.to,text:r.text,origin:r.origin}}function co(e,t,n){if(e.cm){if(!e.cm.curOp)return ei(e.cm,co)(e,t,n);if(e.cm.state.suppressEdits)return}if(!(ve(e,"beforeChange")||e.cm&&ve(e.cm,"beforeChange"))||(t=so(e,t,!0))){var r=wt&&!n&&function(e,t,n){var r=null;if(e.iter(t.line,n.line+1,(function(e){if(e.markedSpans)for(var t=0;t<e.markedSpans.length;++t){var n=e.markedSpans[t].marker;!n.readOnly||r&&-1!=W(r,n)||(r||(r=[])).push(n)}})),!r)return null;for(var i=[{from:t,to:n}],o=0;o<r.length;++o)for(var a=r[o],l=a.find(0),s=0;s<i.length;++s){var c=i[s];if(!(tt(c.to,l.from)<0||tt(c.from,l.to)>0)){var u=[s,1],d=tt(c.from,l.from),h=tt(c.to,l.to);(d<0||!a.inclusiveLeft&&!d)&&u.push({from:c.from,to:l.from}),(h>0||!a.inclusiveRight&&!h)&&u.push({from:l.to,to:c.to}),i.splice.apply(i,u),s+=u.length-3}}return i}(e,t.from,t.to);if(r)for(var i=r.length-1;i>=0;--i)uo(e,{from:r[i].from,to:r[i].to,text:i?[""]:t.text,origin:t.origin});else uo(e,t)}}function uo(e,t){if(1!=t.text.length||""!=t.text[0]||0!=tt(t.from,t.to)){var n=Ai(e,t);Wi(e,t,n,e.cm?e.cm.curOp.id:NaN),po(e,t,n,Tt(e,t));var r=[];Ii(e,(function(e,n){n||-1!=W(r,e.history)||(xo(e.history,t),r.push(e.history)),po(e,t,null,Tt(e,t))}))}}function ho(e,t,n){var r=e.cm&&e.cm.state.suppressEdits;if(!r||n){for(var i,o=e.history,a=e.sel,l="undo"==t?o.done:o.undone,s="undo"==t?o.undone:o.done,c=0;c<l.length&&(i=l[c],n?!i.ranges||i.equals(e.sel):i.ranges);c++);if(c!=l.length){for(o.lastOrigin=o.lastSelOrigin=null;;){if(!(i=l.pop()).ranges){if(r)return void l.push(i);break}if(ji(i,s),n&&!i.equals(e.sel))return void Ji(e,i,{clearRedo:!1});a=i}var u=[];ji(a,s),s.push({changes:u,generation:o.generation}),o.generation=i.generation||++o.maxGeneration;for(var d=ve(e,"beforeChange")||e.cm&&ve(e.cm,"beforeChange"),h=function(n){var r=i.changes[n];if(r.origin=t,d&&!so(e,r,!1))return l.length=0,{};u.push(_i(e,r));var o=n?Ai(e,r):K(l);po(e,r,o,$i(e,r)),!n&&e.cm&&e.cm.scrollIntoView({from:r.from,to:Ti(r)});var a=[];Ii(e,(function(e,t){t||-1!=W(a,e.history)||(xo(e.history,r),a.push(e.history)),po(e,r,null,$i(e,r))}))},f=i.changes.length-1;f>=0;--f){var p=h(f);if(p)return p.v}}}}function fo(e,t){if(0!=t&&(e.first+=t,e.sel=new ki(X(e.sel.ranges,(function(e){return new Ci(et(e.anchor.line+t,e.anchor.ch),et(e.head.line+t,e.head.ch))})),e.sel.primIndex),e.cm)){dr(e.cm,e.first,e.first-t,t);for(var n=e.cm.display,r=n.viewFrom;r<n.viewTo;r++)hr(e.cm,r,"gutter")}}function po(e,t,n,r){if(e.cm&&!e.cm.curOp)return ei(e.cm,po)(e,t,n,r);if(t.to.line<e.first)fo(e,t.text.length-1-(t.to.line-t.from.line));else if(!(t.from.line>e.lastLine())){if(t.from.line<e.first){var i=t.text.length-1-(e.first-t.from.line);fo(e,i),t={from:et(e.first,0),to:et(t.to.line+i,t.to.ch),text:[K(t.text)],origin:t.origin}}var o=e.lastLine();t.to.line>o&&(t={from:t.from,to:et(o,Ge(e,o).text.length),text:[t.text[0]],origin:t.origin}),t.removed=Ve(e,t.from,t.to),n||(n=Ai(e,t)),e.cm?function(e,t,n){var r=e.doc,i=e.display,o=t.from,a=t.to,l=!1,s=o.line;e.options.lineWrapping||(s=Ze(_t(Ge(r,o.line))),r.iter(s,a.line+1,(function(e){if(e==i.maxLine)return l=!0,!0})));r.sel.contains(t.from,t.to)>-1&&ge(e);Oi(r,t,n,lr(e)),e.options.lineWrapping||(r.iter(s,o.line+t.text.length,(function(e){var t=Ut(e);t>i.maxLineLength&&(i.maxLine=e,i.maxLineLength=t,i.maxLineChanged=!0,l=!1)})),l&&(e.curOp.updateMaxLine=!0));(function(e,t){if(e.modeFrontier=Math.min(e.modeFrontier,t),!(e.highlightFrontier<t-10)){for(var n=e.first,r=t-1;r>n;r--){var i=Ge(e,r).stateAfter;if(i&&(!(i instanceof ct)||r+i.lookAhead<t)){n=r+1;break}}e.highlightFrontier=Math.min(e.highlightFrontier,n)}})(r,o.line),ri(e,400);var c=t.text.length-(a.line-o.line)-1;t.full?dr(e):o.line!=a.line||1!=t.text.length||Ei(e.doc,t)?dr(e,o.line,a.line+1,c):hr(e,o.line,"text");var u=ve(e,"changes"),d=ve(e,"change");if(d||u){var h={from:o,to:a,text:t.text,removed:t.removed,origin:t.origin};d&&sn(e,"change",e,h),u&&(e.curOp.changeObjs||(e.curOp.changeObjs=[])).push(h)}e.display.selForContextMenu=null}(e.cm,t,r):Oi(e,t,r),eo(e,n,j),e.cantEdit&&oo(e,et(e.firstLine(),0))&&(e.cantEdit=!1)}}function mo(e,t,n,r,i){var o;r||(r=n),tt(r,n)<0&&(n=(o=[r,n])[0],r=o[1]),"string"==typeof t&&(t=e.splitLines(t)),co(e,{from:n,to:r,text:t,origin:i})}function go(e,t,n,r){n<e.line?e.line+=r:t<e.line&&(e.line=t,e.ch=0)}function vo(e,t,n,r){for(var i=0;i<e.length;++i){var o=e[i],a=!0;if(o.ranges){o.copied||((o=e[i]=o.deepCopy()).copied=!0);for(var l=0;l<o.ranges.length;l++)go(o.ranges[l].anchor,t,n,r),go(o.ranges[l].head,t,n,r)}else{for(var s=0;s<o.changes.length;++s){var c=o.changes[s];if(n<c.from.line)c.from=et(c.from.line+r,c.from.ch),c.to=et(c.to.line+r,c.to.ch);else if(t<=c.to.line){a=!1;break}}a||(e.splice(0,i+1),i=0)}}}function xo(e,t){var n=t.from.line,r=t.to.line,i=t.text.length-(r-n)-1;vo(e.done,n,r,i),vo(e.undone,n,r,i)}function yo(e,t,n,r){var i=t,o=t;return"number"==typeof t?o=Ge(e,at(e,t)):i=Ze(t),null==i?null:(r(o,i)&&e.cm&&hr(e.cm,i,n),o)}function bo(e){this.lines=e,this.parent=null;for(var t=0,n=0;n<e.length;++n)e[n].parent=this,t+=e[n].height;this.height=t}function wo(e){this.children=e;for(var t=0,n=0,r=0;r<e.length;++r){var i=e[r];t+=i.chunkSize(),n+=i.height,i.parent=this}this.size=t,this.height=n,this.parent=null}Ci.prototype.from=function(){return ot(this.anchor,this.head)},Ci.prototype.to=function(){return it(this.anchor,this.head)},Ci.prototype.empty=function(){return this.head.line==this.anchor.line&&this.head.ch==this.anchor.ch},bo.prototype={chunkSize:function(){return this.lines.length},removeInner:function(e,t){for(var n=e,r=e+t;n<r;++n){var i=this.lines[n];this.height-=i.height,Vt(i),sn(i,"delete")}this.lines.splice(e,t)},collapse:function(e){e.push.apply(e,this.lines)},insertInner:function(e,t,n){this.height+=n,this.lines=this.lines.slice(0,e).concat(t).concat(this.lines.slice(e));for(var r=0;r<t.length;++r)t[r].parent=this},iterN:function(e,t,n){for(var r=e+t;e<r;++e)if(n(this.lines[e]))return!0}},wo.prototype={chunkSize:function(){return this.size},removeInner:function(e,t){this.size-=t;for(var n=0;n<this.children.length;++n){var r=this.children[n],i=r.chunkSize();if(e<i){var o=Math.min(t,i-e),a=r.height;if(r.removeInner(e,o),this.height-=a-r.height,i==o&&(this.children.splice(n--,1),r.parent=null),0==(t-=o))break;e=0}else e-=i}if(this.size-t<25&&(this.children.length>1||!(this.children[0]instanceof bo))){var l=[];this.collapse(l),this.children=[new bo(l)],this.children[0].parent=this}},collapse:function(e){for(var t=0;t<this.children.length;++t)this.children[t].collapse(e)},insertInner:function(e,t,n){this.size+=t.length,this.height+=n;for(var r=0;r<this.children.length;++r){var i=this.children[r],o=i.chunkSize();if(e<=o){if(i.insertInner(e,t,n),i.lines&&i.lines.length>50){for(var a=i.lines.length%25+25,l=a;l<i.lines.length;){var s=new bo(i.lines.slice(l,l+=25));i.height-=s.height,this.children.splice(++r,0,s),s.parent=this}i.lines=i.lines.slice(0,a),this.maybeSpill()}break}e-=o}},maybeSpill:function(){if(!(this.children.length<=10)){var e=this;do{var t=new wo(e.children.splice(e.children.length-5,5));if(e.parent){e.size-=t.size,e.height-=t.height;var n=W(e.parent.children,e);e.parent.children.splice(n+1,0,t)}else{var r=new wo(e.children);r.parent=e,e.children=[r,t],e=r}t.parent=e.parent}while(e.children.length>10);e.parent.maybeSpill()}},iterN:function(e,t,n){for(var r=0;r<this.children.length;++r){var i=this.children[r],o=i.chunkSize();if(e<o){var a=Math.min(t,o-e);if(i.iterN(e,a,n))return!0;if(0==(t-=a))break;e=0}else e-=o}}};var ko=function(e,t,n){if(n)for(var r in n)n.hasOwnProperty(r)&&(this[r]=n[r]);this.doc=e,this.node=t};function Co(e,t,n){qt(t)<(e.curOp&&e.curOp.scrollTop||e.doc.scrollTop)&&Dr(e,n)}ko.prototype.clear=function(){var e=this.doc.cm,t=this.line.widgets,n=this.line,r=Ze(n);if(null!=r&&t){for(var i=0;i<t.length;++i)t[i]==this&&t.splice(i--,1);t.length||(n.widgets=null);var o=wn(this);Xe(n,Math.max(0,n.height-o)),e&&(Jr(e,(function(){Co(e,n,-o),hr(e,r,"widget")})),sn(e,"lineWidgetCleared",e,this,r))}},ko.prototype.changed=function(){var e=this,t=this.height,n=this.doc.cm,r=this.line;this.height=null;var i=wn(this)-t;i&&(Bt(this.doc,r)||Xe(r,r.height+i),n&&Jr(n,(function(){n.curOp.forceUpdate=!0,Co(n,r,i),sn(n,"lineWidgetChanged",n,e,Ze(r))})))},xe(ko);var So=0,Lo=function(e,t){this.lines=[],this.type=t,this.doc=e,this.id=++So};function To(e,t,n,r,i){if(r&&r.shared)return function(e,t,n,r,i){(r=R(r)).shared=!1;var o=[To(e,t,n,r,i)],a=o[0],l=r.widgetNode;return Ii(e,(function(e){l&&(r.widgetNode=l.cloneNode(!0)),o.push(To(e,lt(e,t),lt(e,n),r,i));for(var s=0;s<e.linked.length;++s)if(e.linked[s].isParent)return;a=K(o)})),new Mo(o,a)}(e,t,n,r,i);if(e.cm&&!e.cm.curOp)return ei(e.cm,To)(e,t,n,r,i);var o=new Lo(e,i),a=tt(t,n);if(r&&R(r,o,!1),a>0||0==a&&!1!==o.clearWhenEmpty)return o;if(o.replacedWith&&(o.collapsed=!0,o.widgetNode=D("span",[o.replacedWith],"CodeMirror-widget"),r.handleMouseEvents||o.widgetNode.setAttribute("cm-ignore-events","true"),r.insertLeft&&(o.widgetNode.insertLeft=!0)),o.collapsed){if(Rt(e,t.line,t,n,o)||t.line!=n.line&&Rt(e,n.line,t,n,o))throw new Error("Inserting collapsed marker partially overlapping an existing one");kt=!0}o.addToHistory&&Wi(e,{from:t,to:n,origin:"markText"},e.sel,NaN);var l,s=t.line,c=e.cm;if(e.iter(s,n.line+1,(function(e){c&&o.collapsed&&!c.options.lineWrapping&&_t(e)==c.display.maxLine&&(l=!0),o.collapsed&&s!=t.line&&Xe(e,0),function(e,t){e.markedSpans=e.markedSpans?e.markedSpans.concat([t]):[t],t.marker.attachLine(e)}(e,new Ct(o,s==t.line?t.ch:null,s==n.line?n.ch:null)),++s})),o.collapsed&&e.iter(t.line,n.line+1,(function(t){Bt(e,t)&&Xe(t,0)})),o.clearOnEnter&&de(o,"beforeCursorEnter",(function(){return o.clear()})),o.readOnly&&(wt=!0,(e.history.done.length||e.history.undone.length)&&e.clearHistory()),o.collapsed&&(o.id=++So,o.atomic=!0),c){if(l&&(c.curOp.updateMaxLine=!0),o.collapsed)dr(c,t.line,n.line+1);else if(o.className||o.startStyle||o.endStyle||o.css||o.attributes||o.title)for(var u=t.line;u<=n.line;u++)hr(c,u,"text");o.atomic&&no(c.doc),sn(c,"markerAdded",c,o)}return o}Lo.prototype.clear=function(){if(!this.explicitlyCleared){var e=this.doc.cm,t=e&&!e.curOp;if(t&&Gr(e),ve(this,"clear")){var n=this.find();n&&sn(this,"clear",n.from,n.to)}for(var r=null,i=null,o=0;o<this.lines.length;++o){var a=this.lines[o],l=St(a.markedSpans,this);e&&!this.collapsed?hr(e,Ze(a),"text"):e&&(null!=l.to&&(i=Ze(a)),null!=l.from&&(r=Ze(a))),a.markedSpans=Lt(a.markedSpans,l),null==l.from&&this.collapsed&&!Bt(this.doc,a)&&e&&Xe(a,rr(e.display))}if(e&&this.collapsed&&!e.options.lineWrapping)for(var s=0;s<this.lines.length;++s){var c=_t(this.lines[s]),u=Ut(c);u>e.display.maxLineLength&&(e.display.maxLine=c,e.display.maxLineLength=u,e.display.maxLineChanged=!0)}null!=r&&e&&this.collapsed&&dr(e,r,i+1),this.lines.length=0,this.explicitlyCleared=!0,this.atomic&&this.doc.cantEdit&&(this.doc.cantEdit=!1,e&&no(e.doc)),e&&sn(e,"markerCleared",e,this,r,i),t&&Vr(e),this.parent&&this.parent.clear()}},Lo.prototype.find=function(e,t){var n,r;null==e&&"bookmark"==this.type&&(e=1);for(var i=0;i<this.lines.length;++i){var o=this.lines[i],a=St(o.markedSpans,this);if(null!=a.from&&(n=et(t?o:Ze(o),a.from),-1==e))return n;if(null!=a.to&&(r=et(t?o:Ze(o),a.to),1==e))return r}return n&&{from:n,to:r}},Lo.prototype.changed=function(){var e=this,t=this.find(-1,!0),n=this,r=this.doc.cm;t&&r&&Jr(r,(function(){var i=t.line,o=Ze(t.line),a=Fn(r,o);if(a&&(_n(a),r.curOp.selectionChanged=r.curOp.forceUpdate=!0),r.curOp.updateMaxLine=!0,!Bt(n.doc,i)&&null!=n.height){var l=n.height;n.height=null;var s=wn(n)-l;s&&Xe(i,i.height+s)}sn(r,"markerChanged",r,e)}))},Lo.prototype.attachLine=function(e){if(!this.lines.length&&this.doc.cm){var t=this.doc.cm.curOp;t.maybeHiddenMarkers&&-1!=W(t.maybeHiddenMarkers,this)||(t.maybeUnhiddenMarkers||(t.maybeUnhiddenMarkers=[])).push(this)}this.lines.push(e)},Lo.prototype.detachLine=function(e){if(this.lines.splice(W(this.lines,e),1),!this.lines.length&&this.doc.cm){var t=this.doc.cm.curOp;(t.maybeHiddenMarkers||(t.maybeHiddenMarkers=[])).push(this)}},xe(Lo);var Mo=function(e,t){this.markers=e,this.primary=t;for(var n=0;n<e.length;++n)e[n].parent=this};function Ao(e){return e.findMarks(et(e.first,0),e.clipPos(et(e.lastLine())),(function(e){return e.parent}))}function No(e){for(var t=function(t){var n=e[t],r=[n.primary.doc];Ii(n.primary.doc,(function(e){return r.push(e)}));for(var i=0;i<n.markers.length;i++){var o=n.markers[i];-1==W(r,o.doc)&&(o.parent=null,n.markers.splice(i--,1))}},n=0;n<e.length;n++)t(n)}Mo.prototype.clear=function(){if(!this.explicitlyCleared){this.explicitlyCleared=!0;for(var e=0;e<this.markers.length;++e)this.markers[e].clear();sn(this,"clear")}},Mo.prototype.find=function(e,t){return this.primary.find(e,t)},xe(Mo);var Do=0,Fo=function(e,t,n,r,i){if(!(this instanceof Fo))return new Fo(e,t,n,r,i);null==n&&(n=0),wo.call(this,[new bo([new Gt("",null)])]),this.first=n,this.scrollTop=this.scrollLeft=0,this.cantEdit=!1,this.cleanGeneration=1,this.modeFrontier=this.highlightFrontier=n;var o=et(n,0);this.sel=Li(o),this.history=new Ri(null),this.id=++Do,this.modeOption=t,this.lineSep=r,this.direction="rtl"==i?"rtl":"ltr",this.extend=!1,"string"==typeof e&&(e=this.splitLines(e)),Oi(this,{from:o,to:o,text:e}),Ji(this,Li(o),j)};Fo.prototype=Y(wo.prototype,{constructor:Fo,iter:function(e,t,n){n?this.iterN(e-this.first,t-e,n):this.iterN(this.first,this.first+this.size,e)},insert:function(e,t){for(var n=0,r=0;r<t.length;++r)n+=t[r].height;this.insertInner(e-this.first,t,n)},remove:function(e,t){this.removeInner(e-this.first,t)},getValue:function(e){var t=Ke(this,this.first,this.first+this.size);return!1===e?t:t.join(e||this.lineSeparator())},setValue:ni((function(e){var t=et(this.first,0),n=this.first+this.size-1;co(this,{from:t,to:et(n,Ge(this,n).text.length),text:this.splitLines(e),origin:"setValue",full:!0},!0),this.cm&&Er(this.cm,0,0),Ji(this,Li(t),j)})),replaceRange:function(e,t,n,r){mo(this,e,t=lt(this,t),n=n?lt(this,n):t,r)},getRange:function(e,t,n){var r=Ve(this,lt(this,e),lt(this,t));return!1===n?r:r.join(n||this.lineSeparator())},getLine:function(e){var t=this.getLineHandle(e);return t&&t.text},getLineHandle:function(e){if(Qe(this,e))return Ge(this,e)},getLineNumber:function(e){return Ze(e)},getLineHandleVisualStart:function(e){return"number"==typeof e&&(e=Ge(this,e)),_t(e)},lineCount:function(){return this.size},firstLine:function(){return this.first},lastLine:function(){return this.first+this.size-1},clipPos:function(e){return lt(this,e)},getCursor:function(e){var t=this.sel.primary();return null==e||"head"==e?t.head:"anchor"==e?t.anchor:"end"==e||"to"==e||!1===e?t.to():t.from()},listSelections:function(){return this.sel.ranges},somethingSelected:function(){return this.sel.somethingSelected()},setCursor:ni((function(e,t,n){Yi(this,lt(this,"number"==typeof e?et(e,t||0):e),null,n)})),setSelection:ni((function(e,t,n){Yi(this,lt(this,e),lt(this,t||e),n)})),extendSelection:ni((function(e,t,n){Ki(this,lt(this,e),t&&lt(this,t),n)})),extendSelections:ni((function(e,t){Xi(this,st(this,e),t)})),extendSelectionsBy:ni((function(e,t){Xi(this,st(this,X(this.sel.ranges,e)),t)})),setSelections:ni((function(e,t,n){if(e.length){for(var r=[],i=0;i<e.length;i++)r[i]=new Ci(lt(this,e[i].anchor),lt(this,e[i].head));null==t&&(t=Math.min(e.length-1,this.sel.primIndex)),Ji(this,Si(this.cm,r,t),n)}})),addSelection:ni((function(e,t,n){var r=this.sel.ranges.slice(0);r.push(new Ci(lt(this,e),lt(this,t||e))),Ji(this,Si(this.cm,r,r.length-1),n)})),getSelection:function(e){for(var t,n=this.sel.ranges,r=0;r<n.length;r++){var i=Ve(this,n[r].from(),n[r].to());t=t?t.concat(i):i}return!1===e?t:t.join(e||this.lineSeparator())},getSelections:function(e){for(var t=[],n=this.sel.ranges,r=0;r<n.length;r++){var i=Ve(this,n[r].from(),n[r].to());!1!==e&&(i=i.join(e||this.lineSeparator())),t[r]=i}return t},replaceSelection:function(e,t,n){for(var r=[],i=0;i<this.sel.ranges.length;i++)r[i]=e;this.replaceSelections(r,t,n||"+input")},replaceSelections:ni((function(e,t,n){for(var r=[],i=this.sel,o=0;o<i.ranges.length;o++){var a=i.ranges[o];r[o]={from:a.from(),to:a.to(),text:this.splitLines(e[o]),origin:n}}for(var l=t&&"end"!=t&&function(e,t,n){for(var r=[],i=et(e.first,0),o=i,a=0;a<t.length;a++){var l=t[a],s=Ni(l.from,i,o),c=Ni(Ti(l),i,o);if(i=l.to,o=c,"around"==n){var u=e.sel.ranges[a],d=tt(u.head,u.anchor)<0;r[a]=new Ci(d?c:s,d?s:c)}else r[a]=new Ci(s,s)}return new ki(r,e.sel.primIndex)}(this,r,t),s=r.length-1;s>=0;s--)co(this,r[s]);l?Qi(this,l):this.cm&&Fr(this.cm)})),undo:ni((function(){ho(this,"undo")})),redo:ni((function(){ho(this,"redo")})),undoSelection:ni((function(){ho(this,"undo",!0)})),redoSelection:ni((function(){ho(this,"redo",!0)})),setExtending:function(e){this.extend=e},getExtending:function(){return this.extend},historySize:function(){for(var e=this.history,t=0,n=0,r=0;r<e.done.length;r++)e.done[r].ranges||++t;for(var i=0;i<e.undone.length;i++)e.undone[i].ranges||++n;return{undo:t,redo:n}},clearHistory:function(){var e=this;this.history=new Ri(this.history.maxGeneration),Ii(this,(function(t){return t.history=e.history}),!0)},markClean:function(){this.cleanGeneration=this.changeGeneration(!0)},changeGeneration:function(e){return e&&(this.history.lastOp=this.history.lastSelOp=this.history.lastOrigin=null),this.history.generation},isClean:function(e){return this.history.generation==(e||this.cleanGeneration)},getHistory:function(){return{done:Gi(this.history.done),undone:Gi(this.history.undone)}},setHistory:function(e){var t=this.history=new Ri(this.history.maxGeneration);t.done=Gi(e.done.slice(0),null,!0),t.undone=Gi(e.undone.slice(0),null,!0)},setGutterMarker:ni((function(e,t,n){return yo(this,e,"gutter",(function(e){var r=e.gutterMarkers||(e.gutterMarkers={});return r[t]=n,!n&&te(r)&&(e.gutterMarkers=null),!0}))})),clearGutter:ni((function(e){var t=this;this.iter((function(n){n.gutterMarkers&&n.gutterMarkers[e]&&yo(t,n,"gutter",(function(){return n.gutterMarkers[e]=null,te(n.gutterMarkers)&&(n.gutterMarkers=null),!0}))}))})),lineInfo:function(e){var t;if("number"==typeof e){if(!Qe(this,e))return null;if(t=e,!(e=Ge(this,e)))return null}else if(null==(t=Ze(e)))return null;return{line:t,handle:e,text:e.text,gutterMarkers:e.gutterMarkers,textClass:e.textClass,bgClass:e.bgClass,wrapClass:e.wrapClass,widgets:e.widgets}},addLineClass:ni((function(e,t,n){return yo(this,e,"gutter"==t?"gutter":"class",(function(e){var r="text"==t?"textClass":"background"==t?"bgClass":"gutter"==t?"gutterClass":"wrapClass";if(e[r]){if(S(n).test(e[r]))return!1;e[r]+=" "+n}else e[r]=n;return!0}))})),removeLineClass:ni((function(e,t,n){return yo(this,e,"gutter"==t?"gutter":"class",(function(e){var r="text"==t?"textClass":"background"==t?"bgClass":"gutter"==t?"gutterClass":"wrapClass",i=e[r];if(!i)return!1;if(null==n)e[r]=null;else{var o=i.match(S(n));if(!o)return!1;var a=o.index+o[0].length;e[r]=i.slice(0,o.index)+(o.index&&a!=i.length?" ":"")+i.slice(a)||null}return!0}))})),addLineWidget:ni((function(e,t,n){return function(e,t,n,r){var i=new ko(e,n,r),o=e.cm;return o&&i.noHScroll&&(o.display.alignWidgets=!0),yo(e,t,"widget",(function(t){var n=t.widgets||(t.widgets=[]);if(null==i.insertAt?n.push(i):n.splice(Math.min(n.length-1,Math.max(0,i.insertAt)),0,i),i.line=t,o&&!Bt(e,t)){var r=qt(t)<e.scrollTop;Xe(t,t.height+wn(i)),r&&Dr(o,i.height),o.curOp.forceUpdate=!0}return!0})),o&&sn(o,"lineWidgetAdded",o,i,"number"==typeof t?t:Ze(t)),i}(this,e,t,n)})),removeLineWidget:function(e){e.clear()},markText:function(e,t,n){return To(this,lt(this,e),lt(this,t),n,n&&n.type||"range")},setBookmark:function(e,t){var n={replacedWith:t&&(null==t.nodeType?t.widget:t),insertLeft:t&&t.insertLeft,clearWhenEmpty:!1,shared:t&&t.shared,handleMouseEvents:t&&t.handleMouseEvents};return To(this,e=lt(this,e),e,n,"bookmark")},findMarksAt:function(e){var t=[],n=Ge(this,(e=lt(this,e)).line).markedSpans;if(n)for(var r=0;r<n.length;++r){var i=n[r];(null==i.from||i.from<=e.ch)&&(null==i.to||i.to>=e.ch)&&t.push(i.marker.parent||i.marker)}return t},findMarks:function(e,t,n){e=lt(this,e),t=lt(this,t);var r=[],i=e.line;return this.iter(e.line,t.line+1,(function(o){var a=o.markedSpans;if(a)for(var l=0;l<a.length;l++){var s=a[l];null!=s.to&&i==e.line&&e.ch>=s.to||null==s.from&&i!=e.line||null!=s.from&&i==t.line&&s.from>=t.ch||n&&!n(s.marker)||r.push(s.marker.parent||s.marker)}++i})),r},getAllMarks:function(){var e=[];return this.iter((function(t){var n=t.markedSpans;if(n)for(var r=0;r<n.length;++r)null!=n[r].from&&e.push(n[r].marker)})),e},posFromIndex:function(e){var t,n=this.first,r=this.lineSeparator().length;return this.iter((function(i){var o=i.text.length+r;if(o>e)return t=e,!0;e-=o,++n})),lt(this,et(n,t))},indexFromPos:function(e){var t=(e=lt(this,e)).ch;if(e.line<this.first||e.ch<0)return 0;var n=this.lineSeparator().length;return this.iter(this.first,e.line,(function(e){t+=e.text.length+n})),t},copy:function(e){var t=new Fo(Ke(this,this.first,this.first+this.size),this.modeOption,this.first,this.lineSep,this.direction);return t.scrollTop=this.scrollTop,t.scrollLeft=this.scrollLeft,t.sel=this.sel,t.extend=!1,e&&(t.history.undoDepth=this.history.undoDepth,t.setHistory(this.getHistory())),t},linkedDoc:function(e){e||(e={});var t=this.first,n=this.first+this.size;null!=e.from&&e.from>t&&(t=e.from),null!=e.to&&e.to<n&&(n=e.to);var r=new Fo(Ke(this,t,n),e.mode||this.modeOption,t,this.lineSep,this.direction);return e.sharedHist&&(r.history=this.history),(this.linked||(this.linked=[])).push({doc:r,sharedHist:e.sharedHist}),r.linked=[{doc:this,isParent:!0,sharedHist:e.sharedHist}],function(e,t){for(var n=0;n<t.length;n++){var r=t[n],i=r.find(),o=e.clipPos(i.from),a=e.clipPos(i.to);if(tt(o,a)){var l=To(e,o,a,r.primary,r.primary.type);r.markers.push(l),l.parent=r}}}(r,Ao(this)),r},unlinkDoc:function(e){if(e instanceof Ma&&(e=e.doc),this.linked)for(var t=0;t<this.linked.length;++t){if(this.linked[t].doc==e){this.linked.splice(t,1),e.unlinkDoc(this),No(Ao(this));break}}if(e.history==this.history){var n=[e.id];Ii(e,(function(e){return n.push(e.id)}),!0),e.history=new Ri(null),e.history.done=Gi(this.history.done,n),e.history.undone=Gi(this.history.undone,n)}},iterLinkedDocs:function(e){Ii(this,e)},getMode:function(){return this.mode},getEditor:function(){return this.cm},splitLines:function(e){return this.lineSep?e.split(this.lineSep):Fe(e)},lineSeparator:function(){return this.lineSep||"\n"},setDirection:ni((function(e){var t;("rtl"!=e&&(e="ltr"),e!=this.direction)&&(this.direction=e,this.iter((function(e){return e.order=null})),this.cm&&Jr(t=this.cm,(function(){Hi(t),dr(t)})))}))}),Fo.prototype.eachLine=Fo.prototype.iter;var Eo=0;function Oo(e){var t=this;if(Io(t),!me(t,e)&&!kn(t.display,e)){ye(e),a&&(Eo=+new Date);var n=cr(t,e,!0),r=e.dataTransfer.files;if(n&&!t.isReadOnly())if(r&&r.length&&window.FileReader&&window.File)for(var i=r.length,o=Array(i),l=0,s=function(){++l==i&&ei(t,(function(){var e={from:n=lt(t.doc,n),to:n,text:t.doc.splitLines(o.filter((function(e){return null!=e})).join(t.doc.lineSeparator())),origin:"paste"};co(t.doc,e),Qi(t.doc,Li(lt(t.doc,n),lt(t.doc,Ti(e))))}))()},c=function(e,n){if(t.options.allowDropFileTypes&&-1==W(t.options.allowDropFileTypes,e.type))s();else{var r=new FileReader;r.onerror=function(){return s()},r.onload=function(){var e=r.result;/[\x00-\x08\x0e-\x1f]{2}/.test(e)||(o[n]=e),s()},r.readAsText(e)}},u=0;u<r.length;u++)c(r[u],u);else{if(t.state.draggingText&&t.doc.sel.contains(n)>-1)return t.state.draggingText(e),void setTimeout((function(){return t.display.input.focus()}),20);try{var d=e.dataTransfer.getData("Text");if(d){var h;if(t.state.draggingText&&!t.state.draggingText.copy&&(h=t.listSelections()),eo(t.doc,Li(n,n)),h)for(var f=0;f<h.length;++f)mo(t.doc,"",h[f].anchor,h[f].head,"drag");t.replaceSelection(d,"around","paste"),t.display.input.focus()}}catch(e){}}}}function Io(e){e.display.dragCursor&&(e.display.lineSpace.removeChild(e.display.dragCursor),e.display.dragCursor=null)}function zo(e){if(document.getElementsByClassName){for(var t=document.getElementsByClassName("CodeMirror"),n=[],r=0;r<t.length;r++){var i=t[r].CodeMirror;i&&n.push(i)}n.length&&n[0].operation((function(){for(var t=0;t<n.length;t++)e(n[t])}))}}var Ho=!1;function Ro(){var e;Ho||(de(window,"resize",(function(){null==e&&(e=setTimeout((function(){e=null,zo(_o)}),100))})),de(window,"blur",(function(){return zo(Lr)})),Ho=!0)}function _o(e){var t=e.display;t.cachedCharWidth=t.cachedTextHeight=t.cachedPaddingH=null,t.scrollbarsClipped=!1,e.setSize()}for(var Po={3:"Pause",8:"Backspace",9:"Tab",13:"Enter",16:"Shift",17:"Ctrl",18:"Alt",19:"Pause",20:"CapsLock",27:"Esc",32:"Space",33:"PageUp",34:"PageDown",35:"End",36:"Home",37:"Left",38:"Up",39:"Right",40:"Down",44:"PrintScrn",45:"Insert",46:"Delete",59:";",61:"=",91:"Mod",92:"Mod",93:"Mod",106:"*",107:"=",109:"-",110:".",111:"/",145:"ScrollLock",173:"-",186:";",187:"=",188:",",189:"-",190:".",191:"/",192:"`",219:"[",220:"\\",221:"]",222:"'",224:"Mod",63232:"Up",63233:"Down",63234:"Left",63235:"Right",63272:"Delete",63273:"Home",63275:"End",63276:"PageUp",63277:"PageDown",63302:"Insert"},Wo=0;Wo<10;Wo++)Po[Wo+48]=Po[Wo+96]=String(Wo);for(var Bo=65;Bo<=90;Bo++)Po[Bo]=String.fromCharCode(Bo);for(var jo=1;jo<=12;jo++)Po[jo+111]=Po[jo+63235]="F"+jo;var qo={};function Uo(e){var t,n,r,i,o=e.split(/-(?!$)/);e=o[o.length-1];for(var a=0;a<o.length-1;a++){var l=o[a];if(/^(cmd|meta|m)$/i.test(l))i=!0;else if(/^a(lt)?$/i.test(l))t=!0;else if(/^(c|ctrl|control)$/i.test(l))n=!0;else{if(!/^s(hift)?$/i.test(l))throw new Error("Unrecognized modifier name: "+l);r=!0}}return t&&(e="Alt-"+e),n&&(e="Ctrl-"+e),i&&(e="Cmd-"+e),r&&(e="Shift-"+e),e}function $o(e){var t={};for(var n in e)if(e.hasOwnProperty(n)){var r=e[n];if(/^(name|fallthrough|(de|at)tach)$/.test(n))continue;if("..."==r){delete e[n];continue}for(var i=X(n.split(" "),Uo),o=0;o<i.length;o++){var a=void 0,l=void 0;o==i.length-1?(l=i.join(" "),a=r):(l=i.slice(0,o+1).join(" "),a="...");var s=t[l];if(s){if(s!=a)throw new Error("Inconsistent bindings for "+l)}else t[l]=a}delete e[n]}for(var c in t)e[c]=t[c];return e}function Go(e,t,n,r){var i=(t=Zo(t)).call?t.call(e,r):t[e];if(!1===i)return"nothing";if("..."===i)return"multi";if(null!=i&&n(i))return"handled";if(t.fallthrough){if("[object Array]"!=Object.prototype.toString.call(t.fallthrough))return Go(e,t.fallthrough,n,r);for(var o=0;o<t.fallthrough.length;o++){var a=Go(e,t.fallthrough[o],n,r);if(a)return a}}}function Vo(e){var t="string"==typeof e?e:Po[e.keyCode];return"Ctrl"==t||"Alt"==t||"Shift"==t||"Mod"==t}function Ko(e,t,n){var r=e;return t.altKey&&"Alt"!=r&&(e="Alt-"+e),(k?t.metaKey:t.ctrlKey)&&"Ctrl"!=r&&(e="Ctrl-"+e),(k?t.ctrlKey:t.metaKey)&&"Mod"!=r&&(e="Cmd-"+e),!n&&t.shiftKey&&"Shift"!=r&&(e="Shift-"+e),e}function Xo(e,t){if(d&&34==e.keyCode&&e.char)return!1;var n=Po[e.keyCode];return null!=n&&!e.altGraphKey&&(3==e.keyCode&&e.code&&(n=e.code),Ko(n,e,t))}function Zo(e){return"string"==typeof e?qo[e]:e}function Yo(e,t){for(var n=e.doc.sel.ranges,r=[],i=0;i<n.length;i++){for(var o=t(n[i]);r.length&&tt(o.from,K(r).to)<=0;){var a=r.pop();if(tt(a.from,o.from)<0){o.from=a.from;break}}r.push(o)}Jr(e,(function(){for(var t=r.length-1;t>=0;t--)mo(e.doc,"",r[t].from,r[t].to,"+delete");Fr(e)}))}function Qo(e,t,n){var r=ie(e.text,t+n,n);return r<0||r>e.text.length?null:r}function Jo(e,t,n){var r=Qo(e,t.ch,n);return null==r?null:new et(t.line,r,n<0?"after":"before")}function ea(e,t,n,r,i){if(e){"rtl"==t.doc.direction&&(i=-i);var o=ce(n,t.doc.direction);if(o){var a,l=i<0?K(o):o[0],s=i<0==(1==l.level)?"after":"before";if(l.level>0||"rtl"==t.doc.direction){var c=En(t,n);a=i<0?n.text.length-1:0;var u=On(t,c,a).top;a=oe((function(e){return On(t,c,e).top==u}),i<0==(1==l.level)?l.from:l.to-1,a),"before"==s&&(a=Qo(n,a,1))}else a=i<0?l.to:l.from;return new et(r,a,s)}}return new et(r,i<0?n.text.length:0,i<0?"before":"after")}qo.basic={Left:"goCharLeft",Right:"goCharRight",Up:"goLineUp",Down:"goLineDown",End:"goLineEnd",Home:"goLineStartSmart",PageUp:"goPageUp",PageDown:"goPageDown",Delete:"delCharAfter",Backspace:"delCharBefore","Shift-Backspace":"delCharBefore",Tab:"defaultTab","Shift-Tab":"indentAuto",Enter:"newlineAndIndent",Insert:"toggleOverwrite",Esc:"singleSelection"},qo.pcDefault={"Ctrl-A":"selectAll","Ctrl-D":"deleteLine","Ctrl-Z":"undo","Shift-Ctrl-Z":"redo","Ctrl-Y":"redo","Ctrl-Home":"goDocStart","Ctrl-End":"goDocEnd","Ctrl-Up":"goLineUp","Ctrl-Down":"goLineDown","Ctrl-Left":"goGroupLeft","Ctrl-Right":"goGroupRight","Alt-Left":"goLineStart","Alt-Right":"goLineEnd","Ctrl-Backspace":"delGroupBefore","Ctrl-Delete":"delGroupAfter","Ctrl-S":"save","Ctrl-F":"find","Ctrl-G":"findNext","Shift-Ctrl-G":"findPrev","Shift-Ctrl-F":"replace","Shift-Ctrl-R":"replaceAll","Ctrl-[":"indentLess","Ctrl-]":"indentMore","Ctrl-U":"undoSelection","Shift-Ctrl-U":"redoSelection","Alt-U":"redoSelection",fallthrough:"basic"},qo.emacsy={"Ctrl-F":"goCharRight","Ctrl-B":"goCharLeft","Ctrl-P":"goLineUp","Ctrl-N":"goLineDown","Alt-F":"goWordRight","Alt-B":"goWordLeft","Ctrl-A":"goLineStart","Ctrl-E":"goLineEnd","Ctrl-V":"goPageDown","Shift-Ctrl-V":"goPageUp","Ctrl-D":"delCharAfter","Ctrl-H":"delCharBefore","Alt-D":"delWordAfter","Alt-Backspace":"delWordBefore","Ctrl-K":"killLine","Ctrl-T":"transposeChars","Ctrl-O":"openLine"},qo.macDefault={"Cmd-A":"selectAll","Cmd-D":"deleteLine","Cmd-Z":"undo","Shift-Cmd-Z":"redo","Cmd-Y":"redo","Cmd-Home":"goDocStart","Cmd-Up":"goDocStart","Cmd-End":"goDocEnd","Cmd-Down":"goDocEnd","Alt-Left":"goGroupLeft","Alt-Right":"goGroupRight","Cmd-Left":"goLineLeft","Cmd-Right":"goLineRight","Alt-Backspace":"delGroupBefore","Ctrl-Alt-Backspace":"delGroupAfter","Alt-Delete":"delGroupAfter","Cmd-S":"save","Cmd-F":"find","Cmd-G":"findNext","Shift-Cmd-G":"findPrev","Cmd-Alt-F":"replace","Shift-Cmd-Alt-F":"replaceAll","Cmd-[":"indentLess","Cmd-]":"indentMore","Cmd-Backspace":"delWrappedLineLeft","Cmd-Delete":"delWrappedLineRight","Cmd-U":"undoSelection","Shift-Cmd-U":"redoSelection","Ctrl-Up":"goDocStart","Ctrl-Down":"goDocEnd",fallthrough:["basic","emacsy"]},qo.default=x?qo.macDefault:qo.pcDefault;var ta={selectAll:lo,singleSelection:function(e){return e.setSelection(e.getCursor("anchor"),e.getCursor("head"),j)},killLine:function(e){return Yo(e,(function(t){if(t.empty()){var n=Ge(e.doc,t.head.line).text.length;return t.head.ch==n&&t.head.line<e.lastLine()?{from:t.head,to:et(t.head.line+1,0)}:{from:t.head,to:et(t.head.line,n)}}return{from:t.from(),to:t.to()}}))},deleteLine:function(e){return Yo(e,(function(t){return{from:et(t.from().line,0),to:lt(e.doc,et(t.to().line+1,0))}}))},delLineLeft:function(e){return Yo(e,(function(e){return{from:et(e.from().line,0),to:e.from()}}))},delWrappedLineLeft:function(e){return Yo(e,(function(t){var n=e.charCoords(t.head,"div").top+5;return{from:e.coordsChar({left:0,top:n},"div"),to:t.from()}}))},delWrappedLineRight:function(e){return Yo(e,(function(t){var n=e.charCoords(t.head,"div").top+5,r=e.coordsChar({left:e.display.lineDiv.offsetWidth+100,top:n},"div");return{from:t.from(),to:r}}))},undo:function(e){return e.undo()},redo:function(e){return e.redo()},undoSelection:function(e){return e.undoSelection()},redoSelection:function(e){return e.redoSelection()},goDocStart:function(e){return e.extendSelection(et(e.firstLine(),0))},goDocEnd:function(e){return e.extendSelection(et(e.lastLine()))},goLineStart:function(e){return e.extendSelectionsBy((function(t){return na(e,t.head.line)}),{origin:"+move",bias:1})},goLineStartSmart:function(e){return e.extendSelectionsBy((function(t){return ra(e,t.head)}),{origin:"+move",bias:1})},goLineEnd:function(e){return e.extendSelectionsBy((function(t){return function(e,t){var n=Ge(e.doc,t),r=function(e){for(var t;t=zt(e);)e=t.find(1,!0).line;return e}(n);r!=n&&(t=Ze(r));return ea(!0,e,n,t,-1)}(e,t.head.line)}),{origin:"+move",bias:-1})},goLineRight:function(e){return e.extendSelectionsBy((function(t){var n=e.cursorCoords(t.head,"div").top+5;return e.coordsChar({left:e.display.lineDiv.offsetWidth+100,top:n},"div")}),U)},goLineLeft:function(e){return e.extendSelectionsBy((function(t){var n=e.cursorCoords(t.head,"div").top+5;return e.coordsChar({left:0,top:n},"div")}),U)},goLineLeftSmart:function(e){return e.extendSelectionsBy((function(t){var n=e.cursorCoords(t.head,"div").top+5,r=e.coordsChar({left:0,top:n},"div");return r.ch<e.getLine(r.line).search(/\S/)?ra(e,t.head):r}),U)},goLineUp:function(e){return e.moveV(-1,"line")},goLineDown:function(e){return e.moveV(1,"line")},goPageUp:function(e){return e.moveV(-1,"page")},goPageDown:function(e){return e.moveV(1,"page")},goCharLeft:function(e){return e.moveH(-1,"char")},goCharRight:function(e){return e.moveH(1,"char")},goColumnLeft:function(e){return e.moveH(-1,"column")},goColumnRight:function(e){return e.moveH(1,"column")},goWordLeft:function(e){return e.moveH(-1,"word")},goGroupRight:function(e){return e.moveH(1,"group")},goGroupLeft:function(e){return e.moveH(-1,"group")},goWordRight:function(e){return e.moveH(1,"word")},delCharBefore:function(e){return e.deleteH(-1,"codepoint")},delCharAfter:function(e){return e.deleteH(1,"char")},delWordBefore:function(e){return e.deleteH(-1,"word")},delWordAfter:function(e){return e.deleteH(1,"word")},delGroupBefore:function(e){return e.deleteH(-1,"group")},delGroupAfter:function(e){return e.deleteH(1,"group")},indentAuto:function(e){return e.indentSelection("smart")},indentMore:function(e){return e.indentSelection("add")},indentLess:function(e){return e.indentSelection("subtract")},insertTab:function(e){return e.replaceSelection("\t")},insertSoftTab:function(e){for(var t=[],n=e.listSelections(),r=e.options.tabSize,i=0;i<n.length;i++){var o=n[i].from(),a=_(e.getLine(o.line),o.ch,r);t.push(V(r-a%r))}e.replaceSelections(t)},defaultTab:function(e){e.somethingSelected()?e.indentSelection("add"):e.execCommand("insertTab")},transposeChars:function(e){return Jr(e,(function(){for(var t=e.listSelections(),n=[],r=0;r<t.length;r++)if(t[r].empty()){var i=t[r].head,o=Ge(e.doc,i.line).text;if(o)if(i.ch==o.length&&(i=new et(i.line,i.ch-1)),i.ch>0)i=new et(i.line,i.ch+1),e.replaceRange(o.charAt(i.ch-1)+o.charAt(i.ch-2),et(i.line,i.ch-2),i,"+transpose");else if(i.line>e.doc.first){var a=Ge(e.doc,i.line-1).text;a&&(i=new et(i.line,1),e.replaceRange(o.charAt(0)+e.doc.lineSeparator()+a.charAt(a.length-1),et(i.line-1,a.length-1),i,"+transpose"))}n.push(new Ci(i,i))}e.setSelections(n)}))},newlineAndIndent:function(e){return Jr(e,(function(){for(var t=e.listSelections(),n=t.length-1;n>=0;n--)e.replaceRange(e.doc.lineSeparator(),t[n].anchor,t[n].head,"+input");t=e.listSelections();for(var r=0;r<t.length;r++)e.indentLine(t[r].from().line,null,!0);Fr(e)}))},openLine:function(e){return e.replaceSelection("\n","start")},toggleOverwrite:function(e){return e.toggleOverwrite()}};function na(e,t){var n=Ge(e.doc,t),r=_t(n);return r!=n&&(t=Ze(r)),ea(!0,e,r,t,1)}function ra(e,t){var n=na(e,t.line),r=Ge(e.doc,n.line),i=ce(r,e.doc.direction);if(!i||0==i[0].level){var o=Math.max(n.ch,r.text.search(/\S/)),a=t.line==n.line&&t.ch<=o&&t.ch;return et(n.line,a?0:o,n.sticky)}return n}function ia(e,t,n){if("string"==typeof t&&!(t=ta[t]))return!1;e.display.input.ensurePolled();var r=e.display.shift,i=!1;try{e.isReadOnly()&&(e.state.suppressEdits=!0),n&&(e.display.shift=!1),i=t(e)!=B}finally{e.display.shift=r,e.state.suppressEdits=!1}return i}var oa=new P;function aa(e,t,n,r){var i=e.state.keySeq;if(i){if(Vo(t))return"handled";if(/\'$/.test(t)?e.state.keySeq=null:oa.set(50,(function(){e.state.keySeq==i&&(e.state.keySeq=null,e.display.input.reset())})),la(e,i+" "+t,n,r))return!0}return la(e,t,n,r)}function la(e,t,n,r){var i=function(e,t,n){for(var r=0;r<e.state.keyMaps.length;r++){var i=Go(t,e.state.keyMaps[r],n,e);if(i)return i}return e.options.extraKeys&&Go(t,e.options.extraKeys,n,e)||Go(t,e.options.keyMap,n,e)}(e,t,r);return"multi"==i&&(e.state.keySeq=t),"handled"==i&&sn(e,"keyHandled",e,t,n),"handled"!=i&&"multi"!=i||(ye(n),wr(e)),!!i}function sa(e,t){var n=Xo(t,!0);return!!n&&(t.shiftKey&&!e.state.keySeq?aa(e,"Shift-"+n,t,(function(t){return ia(e,t,!0)}))||aa(e,n,t,(function(t){if("string"==typeof t?/^go[A-Z]/.test(t):t.motion)return ia(e,t)})):aa(e,n,t,(function(t){return ia(e,t)})))}var ca=null;function ua(e){var t=this;if(!(e.target&&e.target!=t.display.input.getField()||(t.curOp.focus=E(),me(t,e)))){a&&l<11&&27==e.keyCode&&(e.returnValue=!1);var r=e.keyCode;t.display.shift=16==r||e.shiftKey;var i=sa(t,e);d&&(ca=i?r:null,i||88!=r||Oe||!(x?e.metaKey:e.ctrlKey)||t.replaceSelection("",null,"cut")),n&&!x&&!i&&46==r&&e.shiftKey&&!e.ctrlKey&&document.execCommand&&document.execCommand("cut"),18!=r||/\bCodeMirror-crosshair\b/.test(t.display.lineDiv.className)||function(e){var t=e.display.lineDiv;function n(e){18!=e.keyCode&&e.altKey||(T(t,"CodeMirror-crosshair"),fe(document,"keyup",n),fe(document,"mouseover",n))}O(t,"CodeMirror-crosshair"),de(document,"keyup",n),de(document,"mouseover",n)}(t)}}function da(e){16==e.keyCode&&(this.doc.sel.shift=!1),me(this,e)}function ha(e){var t=this;if(!(e.target&&e.target!=t.display.input.getField()||kn(t.display,e)||me(t,e)||e.ctrlKey&&!e.altKey||x&&e.metaKey)){var n=e.keyCode,r=e.charCode;if(d&&n==ca)return ca=null,void ye(e);if(!d||e.which&&!(e.which<10)||!sa(t,e)){var i=String.fromCharCode(null==r?n:r);"\b"!=i&&(function(e,t,n){return aa(e,"'"+n+"'",t,(function(t){return ia(e,t,!0)}))}(t,e,i)||t.display.input.onKeyPress(e))}}}var fa,pa,ma=function(e,t,n){this.time=e,this.pos=t,this.button=n};function ga(e){var t=this,n=t.display;if(!(me(t,e)||n.activeTouch&&n.input.supportsTouch()))if(n.input.ensurePolled(),n.shift=e.shiftKey,kn(n,e))s||(n.scroller.draggable=!1,setTimeout((function(){return n.scroller.draggable=!0}),100));else if(!ya(t,e)){var r=cr(t,e),i=Se(e),o=r?function(e,t){var n=+new Date;return pa&&pa.compare(n,e,t)?(fa=pa=null,"triple"):fa&&fa.compare(n,e,t)?(pa=new ma(n,e,t),fa=null,"double"):(fa=new ma(n,e,t),pa=null,"single")}(r,i):"single";window.focus(),1==i&&t.state.selectingText&&t.state.selectingText(e),r&&function(e,t,n,r,i){var o="Click";"double"==r?o="Double"+o:"triple"==r&&(o="Triple"+o);return aa(e,Ko(o=(1==t?"Left":2==t?"Middle":"Right")+o,i),i,(function(t){if("string"==typeof t&&(t=ta[t]),!t)return!1;var r=!1;try{e.isReadOnly()&&(e.state.suppressEdits=!0),r=t(e,n)!=B}finally{e.state.suppressEdits=!1}return r}))}(t,i,r,o,e)||(1==i?r?function(e,t,n,r){a?setTimeout(H(kr,e),0):e.curOp.focus=E();var i,o=function(e,t,n){var r=e.getOption("configureMouse"),i=r?r(e,t,n):{};if(null==i.unit){var o=y?n.shiftKey&&n.metaKey:n.altKey;i.unit=o?"rectangle":"single"==t?"char":"double"==t?"word":"line"}(null==i.extend||e.doc.extend)&&(i.extend=e.doc.extend||n.shiftKey);null==i.addNew&&(i.addNew=x?n.metaKey:n.ctrlKey);null==i.moveOnDrag&&(i.moveOnDrag=!(x?n.altKey:n.ctrlKey));return i}(e,n,r),c=e.doc.sel;e.options.dragDrop&&Me&&!e.isReadOnly()&&"single"==n&&(i=c.contains(t))>-1&&(tt((i=c.ranges[i]).from(),t)<0||t.xRel>0)&&(tt(i.to(),t)>0||t.xRel<0)?function(e,t,n,r){var i=e.display,o=!1,c=ei(e,(function(t){s&&(i.scroller.draggable=!1),e.state.draggingText=!1,fe(i.wrapper.ownerDocument,"mouseup",c),fe(i.wrapper.ownerDocument,"mousemove",u),fe(i.scroller,"dragstart",d),fe(i.scroller,"drop",c),o||(ye(t),r.addNew||Ki(e.doc,n,null,null,r.extend),s&&!h||a&&9==l?setTimeout((function(){i.wrapper.ownerDocument.body.focus({preventScroll:!0}),i.input.focus()}),20):i.input.focus())})),u=function(e){o=o||Math.abs(t.clientX-e.clientX)+Math.abs(t.clientY-e.clientY)>=10},d=function(){return o=!0};s&&(i.scroller.draggable=!0);e.state.draggingText=c,c.copy=!r.moveOnDrag,i.scroller.dragDrop&&i.scroller.dragDrop();de(i.wrapper.ownerDocument,"mouseup",c),de(i.wrapper.ownerDocument,"mousemove",u),de(i.scroller,"dragstart",d),de(i.scroller,"drop",c),Cr(e),setTimeout((function(){return i.input.focus()}),20)}(e,r,t,o):function(e,t,n,r){var i=e.display,o=e.doc;ye(t);var a,l,s=o.sel,c=s.ranges;r.addNew&&!r.extend?(l=o.sel.contains(n),a=l>-1?c[l]:new Ci(n,n)):(a=o.sel.primary(),l=o.sel.primIndex);if("rectangle"==r.unit)r.addNew||(a=new Ci(n,n)),n=cr(e,t,!0,!0),l=-1;else{var u=va(e,n,r.unit);a=r.extend?Vi(a,u.anchor,u.head,r.extend):u}r.addNew?-1==l?(l=c.length,Ji(o,Si(e,c.concat([a]),l),{scroll:!1,origin:"*mouse"})):c.length>1&&c[l].empty()&&"char"==r.unit&&!r.extend?(Ji(o,Si(e,c.slice(0,l).concat(c.slice(l+1)),0),{scroll:!1,origin:"*mouse"}),s=o.sel):Zi(o,l,a,q):(l=0,Ji(o,new ki([a],0),q),s=o.sel);var d=n;function h(t){if(0!=tt(d,t))if(d=t,"rectangle"==r.unit){for(var i=[],c=e.options.tabSize,u=_(Ge(o,n.line).text,n.ch,c),h=_(Ge(o,t.line).text,t.ch,c),f=Math.min(u,h),p=Math.max(u,h),m=Math.min(n.line,t.line),g=Math.min(e.lastLine(),Math.max(n.line,t.line));m<=g;m++){var v=Ge(o,m).text,x=$(v,f,c);f==p?i.push(new Ci(et(m,x),et(m,x))):v.length>x&&i.push(new Ci(et(m,x),et(m,$(v,p,c))))}i.length||i.push(new Ci(n,n)),Ji(o,Si(e,s.ranges.slice(0,l).concat(i),l),{origin:"*mouse",scroll:!1}),e.scrollIntoView(t)}else{var y,b=a,w=va(e,t,r.unit),k=b.anchor;tt(w.anchor,k)>0?(y=w.head,k=ot(b.from(),w.anchor)):(y=w.anchor,k=it(b.to(),w.head));var C=s.ranges.slice(0);C[l]=function(e,t){var n=t.anchor,r=t.head,i=Ge(e.doc,n.line);if(0==tt(n,r)&&n.sticky==r.sticky)return t;var o=ce(i);if(!o)return t;var a=le(o,n.ch,n.sticky),l=o[a];if(l.from!=n.ch&&l.to!=n.ch)return t;var s,c=a+(l.from==n.ch==(1!=l.level)?0:1);if(0==c||c==o.length)return t;if(r.line!=n.line)s=(r.line-n.line)*("ltr"==e.doc.direction?1:-1)>0;else{var u=le(o,r.ch,r.sticky),d=u-a||(r.ch-n.ch)*(1==l.level?-1:1);s=u==c-1||u==c?d<0:d>0}var h=o[c+(s?-1:0)],f=s==(1==h.level),p=f?h.from:h.to,m=f?"after":"before";return n.ch==p&&n.sticky==m?t:new Ci(new et(n.line,p,m),r)}(e,new Ci(lt(o,k),y)),Ji(o,Si(e,C,l),q)}}var f=i.wrapper.getBoundingClientRect(),p=0;function m(t){var n=++p,a=cr(e,t,!0,"rectangle"==r.unit);if(a)if(0!=tt(a,d)){e.curOp.focus=E(),h(a);var l=Ar(i,o);(a.line>=l.to||a.line<l.from)&&setTimeout(ei(e,(function(){p==n&&m(t)})),150)}else{var s=t.clientY<f.top?-20:t.clientY>f.bottom?20:0;s&&setTimeout(ei(e,(function(){p==n&&(i.scroller.scrollTop+=s,m(t))})),50)}}function g(t){e.state.selectingText=!1,p=1/0,t&&(ye(t),i.input.focus()),fe(i.wrapper.ownerDocument,"mousemove",v),fe(i.wrapper.ownerDocument,"mouseup",x),o.history.lastSelOrigin=null}var v=ei(e,(function(e){0!==e.buttons&&Se(e)?m(e):g(e)})),x=ei(e,g);e.state.selectingText=x,de(i.wrapper.ownerDocument,"mousemove",v),de(i.wrapper.ownerDocument,"mouseup",x)}(e,r,t,o)}(t,r,o,e):Ce(e)==n.scroller&&ye(e):2==i?(r&&Ki(t.doc,r),setTimeout((function(){return n.input.focus()}),20)):3==i&&(C?t.display.input.onContextMenu(e):Cr(t)))}}function va(e,t,n){if("char"==n)return new Ci(t,t);if("word"==n)return e.findWordAt(t);if("line"==n)return new Ci(et(t.line,0),lt(e.doc,et(t.line+1,0)));var r=n(e,t);return new Ci(r.from,r.to)}function xa(e,t,n,r){var i,o;if(t.touches)i=t.touches[0].clientX,o=t.touches[0].clientY;else try{i=t.clientX,o=t.clientY}catch(e){return!1}if(i>=Math.floor(e.display.gutters.getBoundingClientRect().right))return!1;r&&ye(t);var a=e.display,l=a.lineDiv.getBoundingClientRect();if(o>l.bottom||!ve(e,n))return we(t);o-=l.top-a.viewOffset;for(var s=0;s<e.display.gutterSpecs.length;++s){var c=a.gutters.childNodes[s];if(c&&c.getBoundingClientRect().right>=i)return pe(e,n,e,Ye(e.doc,o),e.display.gutterSpecs[s].className,t),we(t)}}function ya(e,t){return xa(e,t,"gutterClick",!0)}function ba(e,t){kn(e.display,t)||function(e,t){if(!ve(e,"gutterContextMenu"))return!1;return xa(e,t,"gutterContextMenu",!1)}(e,t)||me(e,t,"contextmenu")||C||e.display.input.onContextMenu(t)}function wa(e){e.display.wrapper.className=e.display.wrapper.className.replace(/\s*cm-s-\S+/g,"")+e.options.theme.replace(/(^|\s)\s*/g," cm-s-"),Wn(e)}ma.prototype.compare=function(e,t,n){return this.time+400>e&&0==tt(t,this.pos)&&n==this.button};var ka={toString:function(){return"CodeMirror.Init"}},Ca={},Sa={};function La(e,t,n){if(!t!=!(n&&n!=ka)){var r=e.display.dragFunctions,i=t?de:fe;i(e.display.scroller,"dragstart",r.start),i(e.display.scroller,"dragenter",r.enter),i(e.display.scroller,"dragover",r.over),i(e.display.scroller,"dragleave",r.leave),i(e.display.scroller,"drop",r.drop)}}function Ta(e){e.options.lineWrapping?(O(e.display.wrapper,"CodeMirror-wrap"),e.display.sizer.style.minWidth="",e.display.sizerWidth=null):(T(e.display.wrapper,"CodeMirror-wrap"),$t(e)),sr(e),dr(e),Wn(e),setTimeout((function(){return Br(e)}),100)}function Ma(e,t){var n=this;if(!(this instanceof Ma))return new Ma(e,t);this.options=t=t?R(t):{},R(Ca,t,!1);var r=t.value;"string"==typeof r?r=new Fo(r,t.mode,null,t.lineSeparator,t.direction):t.mode&&(r.modeOption=t.mode),this.doc=r;var i=new Ma.inputStyles[t.inputStyle](this),o=this.display=new gi(e,r,i,t);for(var c in o.wrapper.CodeMirror=this,wa(this),t.lineWrapping&&(this.display.wrapper.className+=" CodeMirror-wrap"),Ur(this),this.state={keyMaps:[],overlays:[],modeGen:0,overwrite:!1,delayingBlurEvent:!1,focused:!1,suppressEdits:!1,pasteIncoming:-1,cutIncoming:-1,selectingText:!1,draggingText:!1,highlight:new P,keySeq:null,specialChars:null},t.autofocus&&!v&&o.input.focus(),a&&l<11&&setTimeout((function(){return n.display.input.reset(!0)}),20),function(e){var t=e.display;de(t.scroller,"mousedown",ei(e,ga)),de(t.scroller,"dblclick",a&&l<11?ei(e,(function(t){if(!me(e,t)){var n=cr(e,t);if(n&&!ya(e,t)&&!kn(e.display,t)){ye(t);var r=e.findWordAt(n);Ki(e.doc,r.anchor,r.head)}}})):function(t){return me(e,t)||ye(t)});de(t.scroller,"contextmenu",(function(t){return ba(e,t)})),de(t.input.getField(),"contextmenu",(function(n){t.scroller.contains(n.target)||ba(e,n)}));var n,r={end:0};function i(){t.activeTouch&&(n=setTimeout((function(){return t.activeTouch=null}),1e3),(r=t.activeTouch).end=+new Date)}function o(e){if(1!=e.touches.length)return!1;var t=e.touches[0];return t.radiusX<=1&&t.radiusY<=1}function s(e,t){if(null==t.left)return!0;var n=t.left-e.left,r=t.top-e.top;return n*n+r*r>400}de(t.scroller,"touchstart",(function(i){if(!me(e,i)&&!o(i)&&!ya(e,i)){t.input.ensurePolled(),clearTimeout(n);var a=+new Date;t.activeTouch={start:a,moved:!1,prev:a-r.end<=300?r:null},1==i.touches.length&&(t.activeTouch.left=i.touches[0].pageX,t.activeTouch.top=i.touches[0].pageY)}})),de(t.scroller,"touchmove",(function(){t.activeTouch&&(t.activeTouch.moved=!0)})),de(t.scroller,"touchend",(function(n){var r=t.activeTouch;if(r&&!kn(t,n)&&null!=r.left&&!r.moved&&new Date-r.start<300){var o,a=e.coordsChar(t.activeTouch,"page");o=!r.prev||s(r,r.prev)?new Ci(a,a):!r.prev.prev||s(r,r.prev.prev)?e.findWordAt(a):new Ci(et(a.line,0),lt(e.doc,et(a.line+1,0))),e.setSelection(o.anchor,o.head),e.focus(),ye(n)}i()})),de(t.scroller,"touchcancel",i),de(t.scroller,"scroll",(function(){t.scroller.clientHeight&&(zr(e,t.scroller.scrollTop),Rr(e,t.scroller.scrollLeft,!0),pe(e,"scroll",e))})),de(t.scroller,"mousewheel",(function(t){return wi(e,t)})),de(t.scroller,"DOMMouseScroll",(function(t){return wi(e,t)})),de(t.wrapper,"scroll",(function(){return t.wrapper.scrollTop=t.wrapper.scrollLeft=0})),t.dragFunctions={enter:function(t){me(e,t)||ke(t)},over:function(t){me(e,t)||(!function(e,t){var n=cr(e,t);if(n){var r=document.createDocumentFragment();xr(e,n,r),e.display.dragCursor||(e.display.dragCursor=N("div",null,"CodeMirror-cursors CodeMirror-dragcursors"),e.display.lineSpace.insertBefore(e.display.dragCursor,e.display.cursorDiv)),A(e.display.dragCursor,r)}}(e,t),ke(t))},start:function(t){return function(e,t){if(a&&(!e.state.draggingText||+new Date-Eo<100))ke(t);else if(!me(e,t)&&!kn(e.display,t)&&(t.dataTransfer.setData("Text",e.getSelection()),t.dataTransfer.effectAllowed="copyMove",t.dataTransfer.setDragImage&&!h)){var n=N("img",null,null,"position: fixed; left: 0; top: 0;");n.src="data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==",d&&(n.width=n.height=1,e.display.wrapper.appendChild(n),n._top=n.offsetTop),t.dataTransfer.setDragImage(n,0,0),d&&n.parentNode.removeChild(n)}}(e,t)},drop:ei(e,Oo),leave:function(t){me(e,t)||Io(e)}};var c=t.input.getField();de(c,"keyup",(function(t){return da.call(e,t)})),de(c,"keydown",ei(e,ua)),de(c,"keypress",ei(e,ha)),de(c,"focus",(function(t){return Sr(e,t)})),de(c,"blur",(function(t){return Lr(e,t)}))}(this),Ro(),Gr(this),this.curOp.forceUpdate=!0,zi(this,r),t.autofocus&&!v||this.hasFocus()?setTimeout((function(){n.hasFocus()&&!n.state.focused&&Sr(n)}),20):Lr(this),Sa)Sa.hasOwnProperty(c)&&Sa[c](this,t[c],ka);hi(this),t.finishInit&&t.finishInit(this);for(var u=0;u<Aa.length;++u)Aa[u](this);Vr(this),s&&t.lineWrapping&&"optimizelegibility"==getComputedStyle(o.lineDiv).textRendering&&(o.lineDiv.style.textRendering="auto")}Ma.defaults=Ca,Ma.optionHandlers=Sa;var Aa=[];function Na(e,t,n,r){var i,o=e.doc;null==n&&(n="add"),"smart"==n&&(o.mode.indent?i=ft(e,t).state:n="prev");var a=e.options.tabSize,l=Ge(o,t),s=_(l.text,null,a);l.stateAfter&&(l.stateAfter=null);var c,u=l.text.match(/^\s*/)[0];if(r||/\S/.test(l.text)){if("smart"==n&&((c=o.mode.indent(i,l.text.slice(u.length),l.text))==B||c>150)){if(!r)return;n="prev"}}else c=0,n="not";"prev"==n?c=t>o.first?_(Ge(o,t-1).text,null,a):0:"add"==n?c=s+e.options.indentUnit:"subtract"==n?c=s-e.options.indentUnit:"number"==typeof n&&(c=s+n),c=Math.max(0,c);var d="",h=0;if(e.options.indentWithTabs)for(var f=Math.floor(c/a);f;--f)h+=a,d+="\t";if(h<c&&(d+=V(c-h)),d!=u)return mo(o,d,et(t,0),et(t,u.length),"+input"),l.stateAfter=null,!0;for(var p=0;p<o.sel.ranges.length;p++){var m=o.sel.ranges[p];if(m.head.line==t&&m.head.ch<u.length){var g=et(t,u.length);Zi(o,p,new Ci(g,g));break}}}Ma.defineInitHook=function(e){return Aa.push(e)};var Da=null;function Fa(e){Da=e}function Ea(e,t,n,r,i){var o=e.doc;e.display.shift=!1,r||(r=o.sel);var a=+new Date-200,l="paste"==i||e.state.pasteIncoming>a,s=Fe(t),c=null;if(l&&r.ranges.length>1)if(Da&&Da.text.join("\n")==t){if(r.ranges.length%Da.text.length==0){c=[];for(var u=0;u<Da.text.length;u++)c.push(o.splitLines(Da.text[u]))}}else s.length==r.ranges.length&&e.options.pasteLinesPerSelection&&(c=X(s,(function(e){return[e]})));for(var d=e.curOp.updateInput,h=r.ranges.length-1;h>=0;h--){var f=r.ranges[h],p=f.from(),m=f.to();f.empty()&&(n&&n>0?p=et(p.line,p.ch-n):e.state.overwrite&&!l?m=et(m.line,Math.min(Ge(o,m.line).text.length,m.ch+K(s).length)):l&&Da&&Da.lineWise&&Da.text.join("\n")==s.join("\n")&&(p=m=et(p.line,0)));var g={from:p,to:m,text:c?c[h%c.length]:s,origin:i||(l?"paste":e.state.cutIncoming>a?"cut":"+input")};co(e.doc,g),sn(e,"inputRead",e,g)}t&&!l&&Ia(e,t),Fr(e),e.curOp.updateInput<2&&(e.curOp.updateInput=d),e.curOp.typing=!0,e.state.pasteIncoming=e.state.cutIncoming=-1}function Oa(e,t){var n=e.clipboardData&&e.clipboardData.getData("Text");if(n)return e.preventDefault(),t.isReadOnly()||t.options.disableInput||Jr(t,(function(){return Ea(t,n,0,null,"paste")})),!0}function Ia(e,t){if(e.options.electricChars&&e.options.smartIndent)for(var n=e.doc.sel,r=n.ranges.length-1;r>=0;r--){var i=n.ranges[r];if(!(i.head.ch>100||r&&n.ranges[r-1].head.line==i.head.line)){var o=e.getModeAt(i.head),a=!1;if(o.electricChars){for(var l=0;l<o.electricChars.length;l++)if(t.indexOf(o.electricChars.charAt(l))>-1){a=Na(e,i.head.line,"smart");break}}else o.electricInput&&o.electricInput.test(Ge(e.doc,i.head.line).text.slice(0,i.head.ch))&&(a=Na(e,i.head.line,"smart"));a&&sn(e,"electricInput",e,i.head.line)}}}function za(e){for(var t=[],n=[],r=0;r<e.doc.sel.ranges.length;r++){var i=e.doc.sel.ranges[r].head.line,o={anchor:et(i,0),head:et(i+1,0)};n.push(o),t.push(e.getRange(o.anchor,o.head))}return{text:t,ranges:n}}function Ha(e,t,n,r){e.setAttribute("autocorrect",n?"":"off"),e.setAttribute("autocapitalize",r?"":"off"),e.setAttribute("spellcheck",!!t)}function Ra(){var e=N("textarea",null,null,"position: absolute; bottom: -1em; padding: 0; width: 1px; height: 1em; outline: none"),t=N("div",[e],null,"overflow: hidden; position: relative; width: 3px; height: 0px;");return s?e.style.width="1000px":e.setAttribute("wrap","off"),m&&(e.style.border="1px solid black"),Ha(e),t}function _a(e,t,n,r,i){var o=t,a=n,l=Ge(e,t.line),s=i&&"rtl"==e.direction?-n:n;function c(o){var a,c;if("codepoint"==r){var u=l.text.charCodeAt(t.ch+(r>0?0:-1));a=isNaN(u)?null:new et(t.line,Math.max(0,Math.min(l.text.length,t.ch+n*(u>=55296&&u<56320?2:1))),-n)}else a=i?function(e,t,n,r){var i=ce(t,e.doc.direction);if(!i)return Jo(t,n,r);n.ch>=t.text.length?(n.ch=t.text.length,n.sticky="before"):n.ch<=0&&(n.ch=0,n.sticky="after");var o=le(i,n.ch,n.sticky),a=i[o];if("ltr"==e.doc.direction&&a.level%2==0&&(r>0?a.to>n.ch:a.from<n.ch))return Jo(t,n,r);var l,s=function(e,n){return Qo(t,e instanceof et?e.ch:e,n)},c=function(n){return e.options.lineWrapping?(l=l||En(e,t),Qn(e,t,l,n)):{begin:0,end:t.text.length}},u=c("before"==n.sticky?s(n,-1):n.ch);if("rtl"==e.doc.direction||1==a.level){var d=1==a.level==r<0,h=s(n,d?1:-1);if(null!=h&&(d?h<=a.to&&h<=u.end:h>=a.from&&h>=u.begin)){var f=d?"before":"after";return new et(n.line,h,f)}}var p=function(e,t,r){for(var o=function(e,t){return t?new et(n.line,s(e,1),"before"):new et(n.line,e,"after")};e>=0&&e<i.length;e+=t){var a=i[e],l=t>0==(1!=a.level),c=l?r.begin:s(r.end,-1);if(a.from<=c&&c<a.to)return o(c,l);if(c=l?a.from:s(a.to,-1),r.begin<=c&&c<r.end)return o(c,l)}},m=p(o+r,r,u);if(m)return m;var g=r>0?u.end:s(u.begin,-1);return null==g||r>0&&g==t.text.length||!(m=p(r>0?0:i.length-1,r,c(g)))?null:m}(e.cm,l,t,n):Jo(l,t,n);if(null==a){if(o||(c=t.line+s)<e.first||c>=e.first+e.size||(t=new et(c,t.ch,t.sticky),!(l=Ge(e,c))))return!1;t=ea(i,e.cm,l,t.line,s)}else t=a;return!0}if("char"==r||"codepoint"==r)c();else if("column"==r)c(!0);else if("word"==r||"group"==r)for(var u=null,d="group"==r,h=e.cm&&e.cm.getHelper(t,"wordChars"),f=!0;!(n<0)||c(!f);f=!1){var p=l.text.charAt(t.ch)||"\n",m=ee(p,h)?"w":d&&"\n"==p?"n":!d||/\s/.test(p)?null:"p";if(!d||f||m||(m="s"),u&&u!=m){n<0&&(n=1,c(),t.sticky="after");break}if(m&&(u=m),n>0&&!c(!f))break}var g=oo(e,t,o,a,!0);return nt(o,g)&&(g.hitSide=!0),g}function Pa(e,t,n,r){var i,o,a=e.doc,l=t.left;if("page"==r){var s=Math.min(e.display.wrapper.clientHeight,window.innerHeight||document.documentElement.clientHeight),c=Math.max(s-.5*rr(e.display),3);i=(n>0?t.bottom:t.top)+n*c}else"line"==r&&(i=n>0?t.bottom+3:t.top-3);for(;(o=Zn(e,l,i)).outside;){if(n<0?i<=0:i>=a.height){o.hitSide=!0;break}i+=5*n}return o}var Wa=function(e){this.cm=e,this.lastAnchorNode=this.lastAnchorOffset=this.lastFocusNode=this.lastFocusOffset=null,this.polling=new P,this.composing=null,this.gracePeriod=!1,this.readDOMTimeout=null};function Ba(e,t){var n=Fn(e,t.line);if(!n||n.hidden)return null;var r=Ge(e.doc,t.line),i=Nn(n,r,t.line),o=ce(r,e.doc.direction),a="left";o&&(a=le(o,t.ch)%2?"right":"left");var l=Hn(i.map,t.ch,a);return l.offset="right"==l.collapse?l.end:l.start,l}function ja(e,t){return t&&(e.bad=!0),e}function qa(e,t,n){var r;if(t==e.display.lineDiv){if(!(r=e.display.lineDiv.childNodes[n]))return ja(e.clipPos(et(e.display.viewTo-1)),!0);t=null,n=0}else for(r=t;;r=r.parentNode){if(!r||r==e.display.lineDiv)return null;if(r.parentNode&&r.parentNode==e.display.lineDiv)break}for(var i=0;i<e.display.view.length;i++){var o=e.display.view[i];if(o.node==r)return Ua(o,t,n)}}function Ua(e,t,n){var r=e.text.firstChild,i=!1;if(!t||!F(r,t))return ja(et(Ze(e.line),0),!0);if(t==r&&(i=!0,t=r.childNodes[n],n=0,!t)){var o=e.rest?K(e.rest):e.line;return ja(et(Ze(o),o.text.length),i)}var a=3==t.nodeType?t:null,l=t;for(a||1!=t.childNodes.length||3!=t.firstChild.nodeType||(a=t.firstChild,n&&(n=a.nodeValue.length));l.parentNode!=r;)l=l.parentNode;var s=e.measure,c=s.maps;function u(t,n,r){for(var i=-1;i<(c?c.length:0);i++)for(var o=i<0?s.map:c[i],a=0;a<o.length;a+=3){var l=o[a+2];if(l==t||l==n){var u=Ze(i<0?e.line:e.rest[i]),d=o[a]+r;return(r<0||l!=t)&&(d=o[a+(r?1:0)]),et(u,d)}}}var d=u(a,l,n);if(d)return ja(d,i);for(var h=l.nextSibling,f=a?a.nodeValue.length-n:0;h;h=h.nextSibling){if(d=u(h,h.firstChild,0))return ja(et(d.line,d.ch-f),i);f+=h.textContent.length}for(var p=l.previousSibling,m=n;p;p=p.previousSibling){if(d=u(p,p.firstChild,-1))return ja(et(d.line,d.ch+m),i);m+=p.textContent.length}}Wa.prototype.init=function(e){var t=this,n=this,r=n.cm,i=n.div=e.lineDiv;function o(e){for(var t=e.target;t;t=t.parentNode){if(t==i)return!0;if(/\bCodeMirror-(?:line)?widget\b/.test(t.className))break}return!1}function a(e){if(o(e)&&!me(r,e)){if(r.somethingSelected())Fa({lineWise:!1,text:r.getSelections()}),"cut"==e.type&&r.replaceSelection("",null,"cut");else{if(!r.options.lineWiseCopyCut)return;var t=za(r);Fa({lineWise:!0,text:t.text}),"cut"==e.type&&r.operation((function(){r.setSelections(t.ranges,0,j),r.replaceSelection("",null,"cut")}))}if(e.clipboardData){e.clipboardData.clearData();var a=Da.text.join("\n");if(e.clipboardData.setData("Text",a),e.clipboardData.getData("Text")==a)return void e.preventDefault()}var l=Ra(),s=l.firstChild;r.display.lineSpace.insertBefore(l,r.display.lineSpace.firstChild),s.value=Da.text.join("\n");var c=document.activeElement;z(s),setTimeout((function(){r.display.lineSpace.removeChild(l),c.focus(),c==i&&n.showPrimarySelection()}),50)}}Ha(i,r.options.spellcheck,r.options.autocorrect,r.options.autocapitalize),de(i,"paste",(function(e){!o(e)||me(r,e)||Oa(e,r)||l<=11&&setTimeout(ei(r,(function(){return t.updateFromDOM()})),20)})),de(i,"compositionstart",(function(e){t.composing={data:e.data,done:!1}})),de(i,"compositionupdate",(function(e){t.composing||(t.composing={data:e.data,done:!1})})),de(i,"compositionend",(function(e){t.composing&&(e.data!=t.composing.data&&t.readFromDOMSoon(),t.composing.done=!0)})),de(i,"touchstart",(function(){return n.forceCompositionEnd()})),de(i,"input",(function(){t.composing||t.readFromDOMSoon()})),de(i,"copy",a),de(i,"cut",a)},Wa.prototype.screenReaderLabelChanged=function(e){e?this.div.setAttribute("aria-label",e):this.div.removeAttribute("aria-label")},Wa.prototype.prepareSelection=function(){var e=vr(this.cm,!1);return e.focus=document.activeElement==this.div,e},Wa.prototype.showSelection=function(e,t){e&&this.cm.display.view.length&&((e.focus||t)&&this.showPrimarySelection(),this.showMultipleSelections(e))},Wa.prototype.getSelection=function(){return this.cm.display.wrapper.ownerDocument.getSelection()},Wa.prototype.showPrimarySelection=function(){var e=this.getSelection(),t=this.cm,r=t.doc.sel.primary(),i=r.from(),o=r.to();if(t.display.viewTo==t.display.viewFrom||i.line>=t.display.viewTo||o.line<t.display.viewFrom)e.removeAllRanges();else{var a=qa(t,e.anchorNode,e.anchorOffset),l=qa(t,e.focusNode,e.focusOffset);if(!a||a.bad||!l||l.bad||0!=tt(ot(a,l),i)||0!=tt(it(a,l),o)){var s=t.display.view,c=i.line>=t.display.viewFrom&&Ba(t,i)||{node:s[0].measure.map[2],offset:0},u=o.line<t.display.viewTo&&Ba(t,o);if(!u){var d=s[s.length-1].measure,h=d.maps?d.maps[d.maps.length-1]:d.map;u={node:h[h.length-1],offset:h[h.length-2]-h[h.length-3]}}if(c&&u){var f,p=e.rangeCount&&e.getRangeAt(0);try{f=L(c.node,c.offset,u.offset,u.node)}catch(e){}f&&(!n&&t.state.focused?(e.collapse(c.node,c.offset),f.collapsed||(e.removeAllRanges(),e.addRange(f))):(e.removeAllRanges(),e.addRange(f)),p&&null==e.anchorNode?e.addRange(p):n&&this.startGracePeriod()),this.rememberSelection()}else e.removeAllRanges()}}},Wa.prototype.startGracePeriod=function(){var e=this;clearTimeout(this.gracePeriod),this.gracePeriod=setTimeout((function(){e.gracePeriod=!1,e.selectionChanged()&&e.cm.operation((function(){return e.cm.curOp.selectionChanged=!0}))}),20)},Wa.prototype.showMultipleSelections=function(e){A(this.cm.display.cursorDiv,e.cursors),A(this.cm.display.selectionDiv,e.selection)},Wa.prototype.rememberSelection=function(){var e=this.getSelection();this.lastAnchorNode=e.anchorNode,this.lastAnchorOffset=e.anchorOffset,this.lastFocusNode=e.focusNode,this.lastFocusOffset=e.focusOffset},Wa.prototype.selectionInEditor=function(){var e=this.getSelection();if(!e.rangeCount)return!1;var t=e.getRangeAt(0).commonAncestorContainer;return F(this.div,t)},Wa.prototype.focus=function(){"nocursor"!=this.cm.options.readOnly&&(this.selectionInEditor()&&document.activeElement==this.div||this.showSelection(this.prepareSelection(),!0),this.div.focus())},Wa.prototype.blur=function(){this.div.blur()},Wa.prototype.getField=function(){return this.div},Wa.prototype.supportsTouch=function(){return!0},Wa.prototype.receivedFocus=function(){var e=this;this.selectionInEditor()?this.pollSelection():Jr(this.cm,(function(){return e.cm.curOp.selectionChanged=!0})),this.polling.set(this.cm.options.pollInterval,(function t(){e.cm.state.focused&&(e.pollSelection(),e.polling.set(e.cm.options.pollInterval,t))}))},Wa.prototype.selectionChanged=function(){var e=this.getSelection();return e.anchorNode!=this.lastAnchorNode||e.anchorOffset!=this.lastAnchorOffset||e.focusNode!=this.lastFocusNode||e.focusOffset!=this.lastFocusOffset},Wa.prototype.pollSelection=function(){if(null==this.readDOMTimeout&&!this.gracePeriod&&this.selectionChanged()){var e=this.getSelection(),t=this.cm;if(g&&u&&this.cm.display.gutterSpecs.length&&function(e){for(var t=e;t;t=t.parentNode)if(/CodeMirror-gutter-wrapper/.test(t.className))return!0;return!1}(e.anchorNode))return this.cm.triggerOnKeyDown({type:"keydown",keyCode:8,preventDefault:Math.abs}),this.blur(),void this.focus();if(!this.composing){this.rememberSelection();var n=qa(t,e.anchorNode,e.anchorOffset),r=qa(t,e.focusNode,e.focusOffset);n&&r&&Jr(t,(function(){Ji(t.doc,Li(n,r),j),(n.bad||r.bad)&&(t.curOp.selectionChanged=!0)}))}}},Wa.prototype.pollContent=function(){null!=this.readDOMTimeout&&(clearTimeout(this.readDOMTimeout),this.readDOMTimeout=null);var e,t,n,r=this.cm,i=r.display,o=r.doc.sel.primary(),a=o.from(),l=o.to();if(0==a.ch&&a.line>r.firstLine()&&(a=et(a.line-1,Ge(r.doc,a.line-1).length)),l.ch==Ge(r.doc,l.line).text.length&&l.line<r.lastLine()&&(l=et(l.line+1,0)),a.line<i.viewFrom||l.line>i.viewTo-1)return!1;a.line==i.viewFrom||0==(e=ur(r,a.line))?(t=Ze(i.view[0].line),n=i.view[0].node):(t=Ze(i.view[e].line),n=i.view[e-1].node.nextSibling);var s,c,u=ur(r,l.line);if(u==i.view.length-1?(s=i.viewTo-1,c=i.lineDiv.lastChild):(s=Ze(i.view[u+1].line)-1,c=i.view[u+1].node.previousSibling),!n)return!1;for(var d=r.doc.splitLines(function(e,t,n,r,i){var o="",a=!1,l=e.doc.lineSeparator(),s=!1;function c(e){return function(t){return t.id==e}}function u(){a&&(o+=l,s&&(o+=l),a=s=!1)}function d(e){e&&(u(),o+=e)}function h(t){if(1==t.nodeType){var n=t.getAttribute("cm-text");if(n)return void d(n);var o,f=t.getAttribute("cm-marker");if(f){var p=e.findMarks(et(r,0),et(i+1,0),c(+f));return void(p.length&&(o=p[0].find(0))&&d(Ve(e.doc,o.from,o.to).join(l)))}if("false"==t.getAttribute("contenteditable"))return;var m=/^(pre|div|p|li|table|br)$/i.test(t.nodeName);if(!/^br$/i.test(t.nodeName)&&0==t.textContent.length)return;m&&u();for(var g=0;g<t.childNodes.length;g++)h(t.childNodes[g]);/^(pre|p)$/i.test(t.nodeName)&&(s=!0),m&&(a=!0)}else 3==t.nodeType&&d(t.nodeValue.replace(/\u200b/g,"").replace(/\u00a0/g," "))}for(;h(t),t!=n;)t=t.nextSibling,s=!1;return o}(r,n,c,t,s)),h=Ve(r.doc,et(t,0),et(s,Ge(r.doc,s).text.length));d.length>1&&h.length>1;)if(K(d)==K(h))d.pop(),h.pop(),s--;else{if(d[0]!=h[0])break;d.shift(),h.shift(),t++}for(var f=0,p=0,m=d[0],g=h[0],v=Math.min(m.length,g.length);f<v&&m.charCodeAt(f)==g.charCodeAt(f);)++f;for(var x=K(d),y=K(h),b=Math.min(x.length-(1==d.length?f:0),y.length-(1==h.length?f:0));p<b&&x.charCodeAt(x.length-p-1)==y.charCodeAt(y.length-p-1);)++p;if(1==d.length&&1==h.length&&t==a.line)for(;f&&f>a.ch&&x.charCodeAt(x.length-p-1)==y.charCodeAt(y.length-p-1);)f--,p++;d[d.length-1]=x.slice(0,x.length-p).replace(/^\u200b+/,""),d[0]=d[0].slice(f).replace(/\u200b+$/,"");var w=et(t,f),k=et(s,h.length?K(h).length-p:0);return d.length>1||d[0]||tt(w,k)?(mo(r.doc,d,w,k,"+input"),!0):void 0},Wa.prototype.ensurePolled=function(){this.forceCompositionEnd()},Wa.prototype.reset=function(){this.forceCompositionEnd()},Wa.prototype.forceCompositionEnd=function(){this.composing&&(clearTimeout(this.readDOMTimeout),this.composing=null,this.updateFromDOM(),this.div.blur(),this.div.focus())},Wa.prototype.readFromDOMSoon=function(){var e=this;null==this.readDOMTimeout&&(this.readDOMTimeout=setTimeout((function(){if(e.readDOMTimeout=null,e.composing){if(!e.composing.done)return;e.composing=null}e.updateFromDOM()}),80))},Wa.prototype.updateFromDOM=function(){var e=this;!this.cm.isReadOnly()&&this.pollContent()||Jr(this.cm,(function(){return dr(e.cm)}))},Wa.prototype.setUneditable=function(e){e.contentEditable="false"},Wa.prototype.onKeyPress=function(e){0==e.charCode||this.composing||(e.preventDefault(),this.cm.isReadOnly()||ei(this.cm,Ea)(this.cm,String.fromCharCode(null==e.charCode?e.keyCode:e.charCode),0))},Wa.prototype.readOnlyChanged=function(e){this.div.contentEditable=String("nocursor"!=e)},Wa.prototype.onContextMenu=function(){},Wa.prototype.resetPosition=function(){},Wa.prototype.needsContentAttribute=!0;var $a=function(e){this.cm=e,this.prevInput="",this.pollingFast=!1,this.polling=new P,this.hasSelection=!1,this.composing=null};$a.prototype.init=function(e){var t=this,n=this,r=this.cm;this.createField(e);var i=this.textarea;function o(e){if(!me(r,e)){if(r.somethingSelected())Fa({lineWise:!1,text:r.getSelections()});else{if(!r.options.lineWiseCopyCut)return;var t=za(r);Fa({lineWise:!0,text:t.text}),"cut"==e.type?r.setSelections(t.ranges,null,j):(n.prevInput="",i.value=t.text.join("\n"),z(i))}"cut"==e.type&&(r.state.cutIncoming=+new Date)}}e.wrapper.insertBefore(this.wrapper,e.wrapper.firstChild),m&&(i.style.width="0px"),de(i,"input",(function(){a&&l>=9&&t.hasSelection&&(t.hasSelection=null),n.poll()})),de(i,"paste",(function(e){me(r,e)||Oa(e,r)||(r.state.pasteIncoming=+new Date,n.fastPoll())})),de(i,"cut",o),de(i,"copy",o),de(e.scroller,"paste",(function(t){if(!kn(e,t)&&!me(r,t)){if(!i.dispatchEvent)return r.state.pasteIncoming=+new Date,void n.focus();var o=new Event("paste");o.clipboardData=t.clipboardData,i.dispatchEvent(o)}})),de(e.lineSpace,"selectstart",(function(t){kn(e,t)||ye(t)})),de(i,"compositionstart",(function(){var e=r.getCursor("from");n.composing&&n.composing.range.clear(),n.composing={start:e,range:r.markText(e,r.getCursor("to"),{className:"CodeMirror-composing"})}})),de(i,"compositionend",(function(){n.composing&&(n.poll(),n.composing.range.clear(),n.composing=null)}))},$a.prototype.createField=function(e){this.wrapper=Ra(),this.textarea=this.wrapper.firstChild},$a.prototype.screenReaderLabelChanged=function(e){e?this.textarea.setAttribute("aria-label",e):this.textarea.removeAttribute("aria-label")},$a.prototype.prepareSelection=function(){var e=this.cm,t=e.display,n=e.doc,r=vr(e);if(e.options.moveInputWithCursor){var i=Vn(e,n.sel.primary().head,"div"),o=t.wrapper.getBoundingClientRect(),a=t.lineDiv.getBoundingClientRect();r.teTop=Math.max(0,Math.min(t.wrapper.clientHeight-10,i.top+a.top-o.top)),r.teLeft=Math.max(0,Math.min(t.wrapper.clientWidth-10,i.left+a.left-o.left))}return r},$a.prototype.showSelection=function(e){var t=this.cm.display;A(t.cursorDiv,e.cursors),A(t.selectionDiv,e.selection),null!=e.teTop&&(this.wrapper.style.top=e.teTop+"px",this.wrapper.style.left=e.teLeft+"px")},$a.prototype.reset=function(e){if(!this.contextMenuPending&&!this.composing){var t=this.cm;if(t.somethingSelected()){this.prevInput="";var n=t.getSelection();this.textarea.value=n,t.state.focused&&z(this.textarea),a&&l>=9&&(this.hasSelection=n)}else e||(this.prevInput=this.textarea.value="",a&&l>=9&&(this.hasSelection=null))}},$a.prototype.getField=function(){return this.textarea},$a.prototype.supportsTouch=function(){return!1},$a.prototype.focus=function(){if("nocursor"!=this.cm.options.readOnly&&(!v||E()!=this.textarea))try{this.textarea.focus()}catch(e){}},$a.prototype.blur=function(){this.textarea.blur()},$a.prototype.resetPosition=function(){this.wrapper.style.top=this.wrapper.style.left=0},$a.prototype.receivedFocus=function(){this.slowPoll()},$a.prototype.slowPoll=function(){var e=this;this.pollingFast||this.polling.set(this.cm.options.pollInterval,(function(){e.poll(),e.cm.state.focused&&e.slowPoll()}))},$a.prototype.fastPoll=function(){var e=!1,t=this;t.pollingFast=!0,t.polling.set(20,(function n(){t.poll()||e?(t.pollingFast=!1,t.slowPoll()):(e=!0,t.polling.set(60,n))}))},$a.prototype.poll=function(){var e=this,t=this.cm,n=this.textarea,r=this.prevInput;if(this.contextMenuPending||!t.state.focused||Ee(n)&&!r&&!this.composing||t.isReadOnly()||t.options.disableInput||t.state.keySeq)return!1;var i=n.value;if(i==r&&!t.somethingSelected())return!1;if(a&&l>=9&&this.hasSelection===i||x&&/[\uf700-\uf7ff]/.test(i))return t.display.input.reset(),!1;if(t.doc.sel==t.display.selForContextMenu){var o=i.charCodeAt(0);if(8203!=o||r||(r="​"),8666==o)return this.reset(),this.cm.execCommand("undo")}for(var s=0,c=Math.min(r.length,i.length);s<c&&r.charCodeAt(s)==i.charCodeAt(s);)++s;return Jr(t,(function(){Ea(t,i.slice(s),r.length-s,null,e.composing?"*compose":null),i.length>1e3||i.indexOf("\n")>-1?n.value=e.prevInput="":e.prevInput=i,e.composing&&(e.composing.range.clear(),e.composing.range=t.markText(e.composing.start,t.getCursor("to"),{className:"CodeMirror-composing"}))})),!0},$a.prototype.ensurePolled=function(){this.pollingFast&&this.poll()&&(this.pollingFast=!1)},$a.prototype.onKeyPress=function(){a&&l>=9&&(this.hasSelection=null),this.fastPoll()},$a.prototype.onContextMenu=function(e){var t=this,n=t.cm,r=n.display,i=t.textarea;t.contextMenuPending&&t.contextMenuPending();var o=cr(n,e),c=r.scroller.scrollTop;if(o&&!d){n.options.resetSelectionOnContextMenu&&-1==n.doc.sel.contains(o)&&ei(n,Ji)(n.doc,Li(o),j);var u,h=i.style.cssText,f=t.wrapper.style.cssText,p=t.wrapper.offsetParent.getBoundingClientRect();if(t.wrapper.style.cssText="position: static",i.style.cssText="position: absolute; width: 30px; height: 30px;\n      top: "+(e.clientY-p.top-5)+"px; left: "+(e.clientX-p.left-5)+"px;\n      z-index: 1000; background: "+(a?"rgba(255, 255, 255, .05)":"transparent")+";\n      outline: none; border-width: 0; outline: none; overflow: hidden; opacity: .05; filter: alpha(opacity=5);",s&&(u=window.scrollY),r.input.focus(),s&&window.scrollTo(null,u),r.input.reset(),n.somethingSelected()||(i.value=t.prevInput=" "),t.contextMenuPending=v,r.selForContextMenu=n.doc.sel,clearTimeout(r.detectingSelectAll),a&&l>=9&&g(),C){ke(e);var m=function(){fe(window,"mouseup",m),setTimeout(v,20)};de(window,"mouseup",m)}else setTimeout(v,50)}function g(){if(null!=i.selectionStart){var e=n.somethingSelected(),o="​"+(e?i.value:"");i.value="⇚",i.value=o,t.prevInput=e?"":"​",i.selectionStart=1,i.selectionEnd=o.length,r.selForContextMenu=n.doc.sel}}function v(){if(t.contextMenuPending==v&&(t.contextMenuPending=!1,t.wrapper.style.cssText=f,i.style.cssText=h,a&&l<9&&r.scrollbars.setScrollTop(r.scroller.scrollTop=c),null!=i.selectionStart)){(!a||a&&l<9)&&g();var e=0,o=function(){r.selForContextMenu==n.doc.sel&&0==i.selectionStart&&i.selectionEnd>0&&"​"==t.prevInput?ei(n,lo)(n):e++<10?r.detectingSelectAll=setTimeout(o,500):(r.selForContextMenu=null,r.input.reset())};r.detectingSelectAll=setTimeout(o,200)}}},$a.prototype.readOnlyChanged=function(e){e||this.reset(),this.textarea.disabled="nocursor"==e,this.textarea.readOnly=!!e},$a.prototype.setUneditable=function(){},$a.prototype.needsContentAttribute=!1,function(e){var t=e.optionHandlers;function n(n,r,i,o){e.defaults[n]=r,i&&(t[n]=o?function(e,t,n){n!=ka&&i(e,t,n)}:i)}e.defineOption=n,e.Init=ka,n("value","",(function(e,t){return e.setValue(t)}),!0),n("mode",null,(function(e,t){e.doc.modeOption=t,Di(e)}),!0),n("indentUnit",2,Di,!0),n("indentWithTabs",!1),n("smartIndent",!0),n("tabSize",4,(function(e){Fi(e),Wn(e),dr(e)}),!0),n("lineSeparator",null,(function(e,t){if(e.doc.lineSep=t,t){var n=[],r=e.doc.first;e.doc.iter((function(e){for(var i=0;;){var o=e.text.indexOf(t,i);if(-1==o)break;i=o+t.length,n.push(et(r,o))}r++}));for(var i=n.length-1;i>=0;i--)mo(e.doc,t,n[i],et(n[i].line,n[i].ch+t.length))}})),n("specialChars",/[\u0000-\u001f\u007f-\u009f\u00ad\u061c\u200b-\u200c\u200e\u200f\u2028\u2029\ufeff\ufff9-\ufffc]/g,(function(e,t,n){e.state.specialChars=new RegExp(t.source+(t.test("\t")?"":"|\t"),"g"),n!=ka&&e.refresh()})),n("specialCharPlaceholder",Qt,(function(e){return e.refresh()}),!0),n("electricChars",!0),n("inputStyle",v?"contenteditable":"textarea",(function(){throw new Error("inputStyle can not (yet) be changed in a running editor")}),!0),n("spellcheck",!1,(function(e,t){return e.getInputField().spellcheck=t}),!0),n("autocorrect",!1,(function(e,t){return e.getInputField().autocorrect=t}),!0),n("autocapitalize",!1,(function(e,t){return e.getInputField().autocapitalize=t}),!0),n("rtlMoveVisually",!b),n("wholeLineUpdateBefore",!0),n("theme","default",(function(e){wa(e),mi(e)}),!0),n("keyMap","default",(function(e,t,n){var r=Zo(t),i=n!=ka&&Zo(n);i&&i.detach&&i.detach(e,r),r.attach&&r.attach(e,i||null)})),n("extraKeys",null),n("configureMouse",null),n("lineWrapping",!1,Ta,!0),n("gutters",[],(function(e,t){e.display.gutterSpecs=fi(t,e.options.lineNumbers),mi(e)}),!0),n("fixedGutter",!0,(function(e,t){e.display.gutters.style.left=t?ar(e.display)+"px":"0",e.refresh()}),!0),n("coverGutterNextToScrollbar",!1,(function(e){return Br(e)}),!0),n("scrollbarStyle","native",(function(e){Ur(e),Br(e),e.display.scrollbars.setScrollTop(e.doc.scrollTop),e.display.scrollbars.setScrollLeft(e.doc.scrollLeft)}),!0),n("lineNumbers",!1,(function(e,t){e.display.gutterSpecs=fi(e.options.gutters,t),mi(e)}),!0),n("firstLineNumber",1,mi,!0),n("lineNumberFormatter",(function(e){return e}),mi,!0),n("showCursorWhenSelecting",!1,gr,!0),n("resetSelectionOnContextMenu",!0),n("lineWiseCopyCut",!0),n("pasteLinesPerSelection",!0),n("selectionsMayTouch",!1),n("readOnly",!1,(function(e,t){"nocursor"==t&&(Lr(e),e.display.input.blur()),e.display.input.readOnlyChanged(t)})),n("screenReaderLabel",null,(function(e,t){t=""===t?null:t,e.display.input.screenReaderLabelChanged(t)})),n("disableInput",!1,(function(e,t){t||e.display.input.reset()}),!0),n("dragDrop",!0,La),n("allowDropFileTypes",null),n("cursorBlinkRate",530),n("cursorScrollMargin",0),n("cursorHeight",1,gr,!0),n("singleCursorHeightPerLine",!0,gr,!0),n("workTime",100),n("workDelay",100),n("flattenSpans",!0,Fi,!0),n("addModeClass",!1,Fi,!0),n("pollInterval",100),n("undoDepth",200,(function(e,t){return e.doc.history.undoDepth=t})),n("historyEventDelay",1250),n("viewportMargin",10,(function(e){return e.refresh()}),!0),n("maxHighlightLength",1e4,Fi,!0),n("moveInputWithCursor",!0,(function(e,t){t||e.display.input.resetPosition()})),n("tabindex",null,(function(e,t){return e.display.input.getField().tabIndex=t||""})),n("autofocus",null),n("direction","ltr",(function(e,t){return e.doc.setDirection(t)}),!0),n("phrases",null)}(Ma),function(e){var t=e.optionHandlers,n=e.helpers={};e.prototype={constructor:e,focus:function(){window.focus(),this.display.input.focus()},setOption:function(e,n){var r=this.options,i=r[e];r[e]==n&&"mode"!=e||(r[e]=n,t.hasOwnProperty(e)&&ei(this,t[e])(this,n,i),pe(this,"optionChange",this,e))},getOption:function(e){return this.options[e]},getDoc:function(){return this.doc},addKeyMap:function(e,t){this.state.keyMaps[t?"push":"unshift"](Zo(e))},removeKeyMap:function(e){for(var t=this.state.keyMaps,n=0;n<t.length;++n)if(t[n]==e||t[n].name==e)return t.splice(n,1),!0},addOverlay:ti((function(t,n){var r=t.token?t:e.getMode(this.options,t);if(r.startState)throw new Error("Overlays may not be stateful.");!function(e,t,n){for(var r=0,i=n(t);r<e.length&&n(e[r])<=i;)r++;e.splice(r,0,t)}(this.state.overlays,{mode:r,modeSpec:t,opaque:n&&n.opaque,priority:n&&n.priority||0},(function(e){return e.priority})),this.state.modeGen++,dr(this)})),removeOverlay:ti((function(e){for(var t=this.state.overlays,n=0;n<t.length;++n){var r=t[n].modeSpec;if(r==e||"string"==typeof e&&r.name==e)return t.splice(n,1),this.state.modeGen++,void dr(this)}})),indentLine:ti((function(e,t,n){"string"!=typeof t&&"number"!=typeof t&&(t=null==t?this.options.smartIndent?"smart":"prev":t?"add":"subtract"),Qe(this.doc,e)&&Na(this,e,t,n)})),indentSelection:ti((function(e){for(var t=this.doc.sel.ranges,n=-1,r=0;r<t.length;r++){var i=t[r];if(i.empty())i.head.line>n&&(Na(this,i.head.line,e,!0),n=i.head.line,r==this.doc.sel.primIndex&&Fr(this));else{var o=i.from(),a=i.to(),l=Math.max(n,o.line);n=Math.min(this.lastLine(),a.line-(a.ch?0:1))+1;for(var s=l;s<n;++s)Na(this,s,e);var c=this.doc.sel.ranges;0==o.ch&&t.length==c.length&&c[r].from().ch>0&&Zi(this.doc,r,new Ci(o,c[r].to()),j)}}})),getTokenAt:function(e,t){return xt(this,e,t)},getLineTokens:function(e,t){return xt(this,et(e),t,!0)},getTokenTypeAt:function(e){e=lt(this.doc,e);var t,n=ht(this,Ge(this.doc,e.line)),r=0,i=(n.length-1)/2,o=e.ch;if(0==o)t=n[2];else for(;;){var a=r+i>>1;if((a?n[2*a-1]:0)>=o)i=a;else{if(!(n[2*a+1]<o)){t=n[2*a+2];break}r=a+1}}var l=t?t.indexOf("overlay "):-1;return l<0?t:0==l?null:t.slice(0,l-1)},getModeAt:function(t){var n=this.doc.mode;return n.innerMode?e.innerMode(n,this.getTokenAt(t).state).mode:n},getHelper:function(e,t){return this.getHelpers(e,t)[0]},getHelpers:function(e,t){var r=[];if(!n.hasOwnProperty(t))return r;var i=n[t],o=this.getModeAt(e);if("string"==typeof o[t])i[o[t]]&&r.push(i[o[t]]);else if(o[t])for(var a=0;a<o[t].length;a++){var l=i[o[t][a]];l&&r.push(l)}else o.helperType&&i[o.helperType]?r.push(i[o.helperType]):i[o.name]&&r.push(i[o.name]);for(var s=0;s<i._global.length;s++){var c=i._global[s];c.pred(o,this)&&-1==W(r,c.val)&&r.push(c.val)}return r},getStateAfter:function(e,t){var n=this.doc;return ft(this,(e=at(n,null==e?n.first+n.size-1:e))+1,t).state},cursorCoords:function(e,t){var n=this.doc.sel.primary();return Vn(this,null==e?n.head:"object"==typeof e?lt(this.doc,e):e?n.from():n.to(),t||"page")},charCoords:function(e,t){return Gn(this,lt(this.doc,e),t||"page")},coordsChar:function(e,t){return Zn(this,(e=$n(this,e,t||"page")).left,e.top)},lineAtHeight:function(e,t){return e=$n(this,{top:e,left:0},t||"page").top,Ye(this.doc,e+this.display.viewOffset)},heightAtLine:function(e,t,n){var r,i=!1;if("number"==typeof e){var o=this.doc.first+this.doc.size-1;e<this.doc.first?e=this.doc.first:e>o&&(e=o,i=!0),r=Ge(this.doc,e)}else r=e;return Un(this,r,{top:0,left:0},t||"page",n||i).top+(i?this.doc.height-qt(r):0)},defaultTextHeight:function(){return rr(this.display)},defaultCharWidth:function(){return ir(this.display)},getViewport:function(){return{from:this.display.viewFrom,to:this.display.viewTo}},addWidget:function(e,t,n,r,i){var o,a,l,s=this.display,c=(e=Vn(this,lt(this.doc,e))).bottom,u=e.left;if(t.style.position="absolute",t.setAttribute("cm-ignore-events","true"),this.display.input.setUneditable(t),s.sizer.appendChild(t),"over"==r)c=e.top;else if("above"==r||"near"==r){var d=Math.max(s.wrapper.clientHeight,this.doc.height),h=Math.max(s.sizer.clientWidth,s.lineSpace.clientWidth);("above"==r||e.bottom+t.offsetHeight>d)&&e.top>t.offsetHeight?c=e.top-t.offsetHeight:e.bottom+t.offsetHeight<=d&&(c=e.bottom),u+t.offsetWidth>h&&(u=h-t.offsetWidth)}t.style.top=c+"px",t.style.left=t.style.right="","right"==i?(u=s.sizer.clientWidth-t.offsetWidth,t.style.right="0px"):("left"==i?u=0:"middle"==i&&(u=(s.sizer.clientWidth-t.offsetWidth)/2),t.style.left=u+"px"),n&&(o=this,a={left:u,top:c,right:u+t.offsetWidth,bottom:c+t.offsetHeight},null!=(l=Nr(o,a)).scrollTop&&zr(o,l.scrollTop),null!=l.scrollLeft&&Rr(o,l.scrollLeft))},triggerOnKeyDown:ti(ua),triggerOnKeyPress:ti(ha),triggerOnKeyUp:da,triggerOnMouseDown:ti(ga),execCommand:function(e){if(ta.hasOwnProperty(e))return ta[e].call(null,this)},triggerElectric:ti((function(e){Ia(this,e)})),findPosH:function(e,t,n,r){var i=1;t<0&&(i=-1,t=-t);for(var o=lt(this.doc,e),a=0;a<t&&!(o=_a(this.doc,o,i,n,r)).hitSide;++a);return o},moveH:ti((function(e,t){var n=this;this.extendSelectionsBy((function(r){return n.display.shift||n.doc.extend||r.empty()?_a(n.doc,r.head,e,t,n.options.rtlMoveVisually):e<0?r.from():r.to()}),U)})),deleteH:ti((function(e,t){var n=this.doc.sel,r=this.doc;n.somethingSelected()?r.replaceSelection("",null,"+delete"):Yo(this,(function(n){var i=_a(r,n.head,e,t,!1);return e<0?{from:i,to:n.head}:{from:n.head,to:i}}))})),findPosV:function(e,t,n,r){var i=1,o=r;t<0&&(i=-1,t=-t);for(var a=lt(this.doc,e),l=0;l<t;++l){var s=Vn(this,a,"div");if(null==o?o=s.left:s.left=o,(a=Pa(this,s,i,n)).hitSide)break}return a},moveV:ti((function(e,t){var n=this,r=this.doc,i=[],o=!this.display.shift&&!r.extend&&r.sel.somethingSelected();if(r.extendSelectionsBy((function(a){if(o)return e<0?a.from():a.to();var l=Vn(n,a.head,"div");null!=a.goalColumn&&(l.left=a.goalColumn),i.push(l.left);var s=Pa(n,l,e,t);return"page"==t&&a==r.sel.primary()&&Dr(n,Gn(n,s,"div").top-l.top),s}),U),i.length)for(var a=0;a<r.sel.ranges.length;a++)r.sel.ranges[a].goalColumn=i[a]})),findWordAt:function(e){var t=Ge(this.doc,e.line).text,n=e.ch,r=e.ch;if(t){var i=this.getHelper(e,"wordChars");"before"!=e.sticky&&r!=t.length||!n?++r:--n;for(var o=t.charAt(n),a=ee(o,i)?function(e){return ee(e,i)}:/\s/.test(o)?function(e){return/\s/.test(e)}:function(e){return!/\s/.test(e)&&!ee(e)};n>0&&a(t.charAt(n-1));)--n;for(;r<t.length&&a(t.charAt(r));)++r}return new Ci(et(e.line,n),et(e.line,r))},toggleOverwrite:function(e){null!=e&&e==this.state.overwrite||((this.state.overwrite=!this.state.overwrite)?O(this.display.cursorDiv,"CodeMirror-overwrite"):T(this.display.cursorDiv,"CodeMirror-overwrite"),pe(this,"overwriteToggle",this,this.state.overwrite))},hasFocus:function(){return this.display.input.getField()==E()},isReadOnly:function(){return!(!this.options.readOnly&&!this.doc.cantEdit)},scrollTo:ti((function(e,t){Er(this,e,t)})),getScrollInfo:function(){var e=this.display.scroller;return{left:e.scrollLeft,top:e.scrollTop,height:e.scrollHeight-Tn(this)-this.display.barHeight,width:e.scrollWidth-Tn(this)-this.display.barWidth,clientHeight:An(this),clientWidth:Mn(this)}},scrollIntoView:ti((function(e,t){null==e?(e={from:this.doc.sel.primary().head,to:null},null==t&&(t=this.options.cursorScrollMargin)):"number"==typeof e?e={from:et(e,0),to:null}:null==e.from&&(e={from:e,to:null}),e.to||(e.to=e.from),e.margin=t||0,null!=e.from.line?function(e,t){Or(e),e.curOp.scrollToPos=t}(this,e):Ir(this,e.from,e.to,e.margin)})),setSize:ti((function(e,t){var n=this,r=function(e){return"number"==typeof e||/^\d+$/.test(String(e))?e+"px":e};null!=e&&(this.display.wrapper.style.width=r(e)),null!=t&&(this.display.wrapper.style.height=r(t)),this.options.lineWrapping&&Pn(this);var i=this.display.viewFrom;this.doc.iter(i,this.display.viewTo,(function(e){if(e.widgets)for(var t=0;t<e.widgets.length;t++)if(e.widgets[t].noHScroll){hr(n,i,"widget");break}++i})),this.curOp.forceUpdate=!0,pe(this,"refresh",this)})),operation:function(e){return Jr(this,e)},startOperation:function(){return Gr(this)},endOperation:function(){return Vr(this)},refresh:ti((function(){var e=this.display.cachedTextHeight;dr(this),this.curOp.forceUpdate=!0,Wn(this),Er(this,this.doc.scrollLeft,this.doc.scrollTop),ci(this.display),(null==e||Math.abs(e-rr(this.display))>.5||this.options.lineWrapping)&&sr(this),pe(this,"refresh",this)})),swapDoc:ti((function(e){var t=this.doc;return t.cm=null,this.state.selectingText&&this.state.selectingText(),zi(this,e),Wn(this),this.display.input.reset(),Er(this,e.scrollLeft,e.scrollTop),this.curOp.forceScroll=!0,sn(this,"swapDoc",this,t),t})),phrase:function(e){var t=this.options.phrases;return t&&Object.prototype.hasOwnProperty.call(t,e)?t[e]:e},getInputField:function(){return this.display.input.getField()},getWrapperElement:function(){return this.display.wrapper},getScrollerElement:function(){return this.display.scroller},getGutterElement:function(){return this.display.gutters}},xe(e),e.registerHelper=function(t,r,i){n.hasOwnProperty(t)||(n[t]=e[t]={_global:[]}),n[t][r]=i},e.registerGlobalHelper=function(t,r,i,o){e.registerHelper(t,r,o),n[t]._global.push({pred:i,val:o})}}(Ma);var Ga="iter insert remove copy getEditor constructor".split(" ");for(var Va in Fo.prototype)Fo.prototype.hasOwnProperty(Va)&&W(Ga,Va)<0&&(Ma.prototype[Va]=function(e){return function(){return e.apply(this.doc,arguments)}}(Fo.prototype[Va]));return xe(Fo),Ma.inputStyles={textarea:$a,contenteditable:Wa},Ma.defineMode=function(e){Ma.defaults.mode||"null"==e||(Ma.defaults.mode=e),Re.apply(this,arguments)},Ma.defineMIME=function(e,t){He[e]=t},Ma.defineMode("null",(function(){return{token:function(e){return e.skipToEnd()}}})),Ma.defineMIME("text/plain","null"),Ma.defineExtension=function(e,t){Ma.prototype[e]=t},Ma.defineDocExtension=function(e,t){Fo.prototype[e]=t},Ma.fromTextArea=function(e,t){if((t=t?R(t):{}).value=e.value,!t.tabindex&&e.tabIndex&&(t.tabindex=e.tabIndex),!t.placeholder&&e.placeholder&&(t.placeholder=e.placeholder),null==t.autofocus){var n=E();t.autofocus=n==e||null!=e.getAttribute("autofocus")&&n==document.body}function r(){e.value=l.getValue()}var i;if(e.form&&(de(e.form,"submit",r),!t.leaveSubmitMethodAlone)){var o=e.form;i=o.submit;try{var a=o.submit=function(){r(),o.submit=i,o.submit(),o.submit=a}}catch(e){}}t.finishInit=function(n){n.save=r,n.getTextArea=function(){return e},n.toTextArea=function(){n.toTextArea=isNaN,r(),e.parentNode.removeChild(n.getWrapperElement()),e.style.display="",e.form&&(fe(e.form,"submit",r),t.leaveSubmitMethodAlone||"function"!=typeof e.form.submit||(e.form.submit=i))}},e.style.display="none";var l=Ma((function(t){return e.parentNode.insertBefore(t,e.nextSibling)}),t);return l},function(e){e.off=fe,e.on=de,e.wheelEventPixels=bi,e.Doc=Fo,e.splitLines=Fe,e.countColumn=_,e.findColumn=$,e.isWordChar=J,e.Pass=B,e.signal=pe,e.Line=Gt,e.changeEnd=Ti,e.scrollbarModel=qr,e.Pos=et,e.cmpPos=tt,e.modes=ze,e.mimeModes=He,e.resolveMode=_e,e.getMode=Pe,e.modeExtensions=We,e.extendMode=Be,e.copyState=je,e.startState=Ue,e.innerMode=qe,e.commands=ta,e.keyMap=qo,e.keyName=Xo,e.isModifierKey=Vo,e.lookupKey=Go,e.normalizeKeyMap=$o,e.StringStream=$e,e.SharedTextMarker=Mo,e.TextMarker=Lo,e.LineWidget=ko,e.e_preventDefault=ye,e.e_stopPropagation=be,e.e_stop=ke,e.addClass=O,e.contains=F,e.rmClass=T,e.keyNames=Po}(Ma),Ma.version="5.58.1",Ma}))},{}],10:[function(e,t,n){var r;r=function(e){"use strict";var t=/^((?:(?:aaas?|about|acap|adiumxtra|af[ps]|aim|apt|attachment|aw|beshare|bitcoin|bolo|callto|cap|chrome(?:-extension)?|cid|coap|com-eventbrite-attendee|content|crid|cvs|data|dav|dict|dlna-(?:playcontainer|playsingle)|dns|doi|dtn|dvb|ed2k|facetime|feed|file|finger|fish|ftp|geo|gg|git|gizmoproject|go|gopher|gtalk|h323|hcp|https?|iax|icap|icon|im|imap|info|ipn|ipp|irc[6s]?|iris(?:\.beep|\.lwz|\.xpc|\.xpcs)?|itms|jar|javascript|jms|keyparc|lastfm|ldaps?|magnet|mailto|maps|market|message|mid|mms|ms-help|msnim|msrps?|mtqp|mumble|mupdate|mvn|news|nfs|nih?|nntp|notes|oid|opaquelocktoken|palm|paparazzi|platform|pop|pres|proxy|psyc|query|res(?:ource)?|rmi|rsync|rtmp|rtsp|secondlife|service|session|sftp|sgn|shttp|sieve|sips?|skype|sm[bs]|snmp|soap\.beeps?|soldat|spotify|ssh|steam|svn|tag|teamspeak|tel(?:net)?|tftp|things|thismessage|tip|tn3270|tv|udp|unreal|urn|ut2004|vemmi|ventrilo|view-source|webcal|wss?|wtai|wyciwyg|xcon(?:-userid)?|xfire|xmlrpc\.beeps?|xmpp|xri|ymsgr|z39\.50[rs]?):(?:\/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]|\([^\s()<>]*\))+(?:\([^\s()<>]*\)|[^\s`*!()\[\]{};:'".,<>?«»“”‘’]))/i;e.defineMode("gfm",(function(n,r){var i=0,o={startState:function(){return{code:!1,codeBlock:!1,ateSpace:!1}},copyState:function(e){return{code:e.code,codeBlock:e.codeBlock,ateSpace:e.ateSpace}},token:function(e,n){if(n.combineTokens=null,n.codeBlock)return e.match(/^```+/)?(n.codeBlock=!1,null):(e.skipToEnd(),null);if(e.sol()&&(n.code=!1),e.sol()&&e.match(/^```+/))return e.skipToEnd(),n.codeBlock=!0,null;if("`"===e.peek()){e.next();var o=e.pos;e.eatWhile("`");var a=1+e.pos-o;return n.code?a===i&&(n.code=!1):(i=a,n.code=!0),null}if(n.code)return e.next(),null;if(e.eatSpace())return n.ateSpace=!0,null;if((e.sol()||n.ateSpace)&&(n.ateSpace=!1,!1!==r.gitHubSpice)){if(e.match(/^(?:[a-zA-Z0-9\-_]+\/)?(?:[a-zA-Z0-9\-_]+@)?(?=.{0,6}\d)(?:[a-f0-9]{7,40}\b)/))return n.combineTokens=!0,"link";if(e.match(/^(?:[a-zA-Z0-9\-_]+\/)?(?:[a-zA-Z0-9\-_]+)?#[0-9]+\b/))return n.combineTokens=!0,"link"}return e.match(t)&&"]("!=e.string.slice(e.start-2,e.start)&&(0==e.start||/\W/.test(e.string.charAt(e.start-1)))?(n.combineTokens=!0,"link"):(e.next(),null)},blankLine:function(e){return e.code=!1,null}},a={taskLists:!0,strikethrough:!0,emoji:!0};for(var l in r)a[l]=r[l];return a.name="markdown",e.overlayMode(e.getMode(n,a),o)}),"markdown"),e.defineMIME("text/x-gfm","gfm")},"object"==typeof n&&"object"==typeof t?r(e("../../lib/codemirror"),e("../markdown/markdown"),e("../../addon/mode/overlay")):r(CodeMirror)},{"../../addon/mode/overlay":6,"../../lib/codemirror":9,"../markdown/markdown":11}],11:[function(e,t,n){var r;r=function(e){"use strict";e.defineMode("markdown",(function(t,n){var r=e.getMode(t,"text/html"),i="null"==r.name;void 0===n.highlightFormatting&&(n.highlightFormatting=!1),void 0===n.maxBlockquoteDepth&&(n.maxBlockquoteDepth=0),void 0===n.taskLists&&(n.taskLists=!1),void 0===n.strikethrough&&(n.strikethrough=!1),void 0===n.emoji&&(n.emoji=!1),void 0===n.fencedCodeBlockHighlighting&&(n.fencedCodeBlockHighlighting=!0),void 0===n.fencedCodeBlockDefaultMode&&(n.fencedCodeBlockDefaultMode="text/plain"),void 0===n.xml&&(n.xml=!0),void 0===n.tokenTypeOverrides&&(n.tokenTypeOverrides={});var o={header:"header",code:"comment",quote:"quote",list1:"variable-2",list2:"variable-3",list3:"keyword",hr:"hr",image:"image",imageAltText:"image-alt-text",imageMarker:"image-marker",formatting:"formatting",linkInline:"link",linkEmail:"link",linkText:"link",linkHref:"string",em:"em",strong:"strong",strikethrough:"strikethrough",emoji:"builtin"};for(var a in o)o.hasOwnProperty(a)&&n.tokenTypeOverrides[a]&&(o[a]=n.tokenTypeOverrides[a]);var l=/^([*\-_])(?:\s*\1){2,}\s*$/,s=/^(?:[*\-+]|^[0-9]+([.)]))\s+/,c=/^\[(x| )\](?=\s)/i,u=n.allowAtxHeaderWithoutSpace?/^(#+)/:/^(#+)(?: |$)/,d=/^ {0,3}(?:\={1,}|-{2,})\s*$/,h=/^[^#!\[\]*_\\<>` "'(~:]+/,f=/^(~~~+|```+)[ \t]*([\w\/+#-]*)[^\n`]*$/,p=/^\s*\[[^\]]+?\]:.*$/,m=/[!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~\xA1\xA7\xAB\xB6\xB7\xBB\xBF\u037E\u0387\u055A-\u055F\u0589\u058A\u05BE\u05C0\u05C3\u05C6\u05F3\u05F4\u0609\u060A\u060C\u060D\u061B\u061E\u061F\u066A-\u066D\u06D4\u0700-\u070D\u07F7-\u07F9\u0830-\u083E\u085E\u0964\u0965\u0970\u0AF0\u0DF4\u0E4F\u0E5A\u0E5B\u0F04-\u0F12\u0F14\u0F3A-\u0F3D\u0F85\u0FD0-\u0FD4\u0FD9\u0FDA\u104A-\u104F\u10FB\u1360-\u1368\u1400\u166D\u166E\u169B\u169C\u16EB-\u16ED\u1735\u1736\u17D4-\u17D6\u17D8-\u17DA\u1800-\u180A\u1944\u1945\u1A1E\u1A1F\u1AA0-\u1AA6\u1AA8-\u1AAD\u1B5A-\u1B60\u1BFC-\u1BFF\u1C3B-\u1C3F\u1C7E\u1C7F\u1CC0-\u1CC7\u1CD3\u2010-\u2027\u2030-\u2043\u2045-\u2051\u2053-\u205E\u207D\u207E\u208D\u208E\u2308-\u230B\u2329\u232A\u2768-\u2775\u27C5\u27C6\u27E6-\u27EF\u2983-\u2998\u29D8-\u29DB\u29FC\u29FD\u2CF9-\u2CFC\u2CFE\u2CFF\u2D70\u2E00-\u2E2E\u2E30-\u2E42\u3001-\u3003\u3008-\u3011\u3014-\u301F\u3030\u303D\u30A0\u30FB\uA4FE\uA4FF\uA60D-\uA60F\uA673\uA67E\uA6F2-\uA6F7\uA874-\uA877\uA8CE\uA8CF\uA8F8-\uA8FA\uA8FC\uA92E\uA92F\uA95F\uA9C1-\uA9CD\uA9DE\uA9DF\uAA5C-\uAA5F\uAADE\uAADF\uAAF0\uAAF1\uABEB\uFD3E\uFD3F\uFE10-\uFE19\uFE30-\uFE52\uFE54-\uFE61\uFE63\uFE68\uFE6A\uFE6B\uFF01-\uFF03\uFF05-\uFF0A\uFF0C-\uFF0F\uFF1A\uFF1B\uFF1F\uFF20\uFF3B-\uFF3D\uFF3F\uFF5B\uFF5D\uFF5F-\uFF65]|\uD800[\uDD00-\uDD02\uDF9F\uDFD0]|\uD801\uDD6F|\uD802[\uDC57\uDD1F\uDD3F\uDE50-\uDE58\uDE7F\uDEF0-\uDEF6\uDF39-\uDF3F\uDF99-\uDF9C]|\uD804[\uDC47-\uDC4D\uDCBB\uDCBC\uDCBE-\uDCC1\uDD40-\uDD43\uDD74\uDD75\uDDC5-\uDDC9\uDDCD\uDDDB\uDDDD-\uDDDF\uDE38-\uDE3D\uDEA9]|\uD805[\uDCC6\uDDC1-\uDDD7\uDE41-\uDE43\uDF3C-\uDF3E]|\uD809[\uDC70-\uDC74]|\uD81A[\uDE6E\uDE6F\uDEF5\uDF37-\uDF3B\uDF44]|\uD82F\uDC9F|\uD836[\uDE87-\uDE8B]/;function g(e,t,n){return t.f=t.inline=n,n(e,t)}function v(e,t,n){return t.f=t.block=n,n(e,t)}function x(t){if(t.linkTitle=!1,t.linkHref=!1,t.linkText=!1,t.em=!1,t.strong=!1,t.strikethrough=!1,t.quote=0,t.indentedCode=!1,t.f==b){var n=i;if(!n){var o=e.innerMode(r,t.htmlState);n="xml"==o.mode.name&&null===o.state.tagStart&&!o.state.context&&o.state.tokenize.isInText}n&&(t.f=S,t.block=y,t.htmlState=null)}return t.trailingSpace=0,t.trailingSpaceNewLine=!1,t.prevLine=t.thisLine,t.thisLine={stream:null},null}function y(r,i){var a,h=r.column()===i.indentation,m=!(a=i.prevLine.stream)||!/\S/.test(a.string),v=i.indentedCode,x=i.prevLine.hr,y=!1!==i.list,b=(i.listStack[i.listStack.length-1]||0)+3;i.indentedCode=!1;var C=i.indentation;if(null===i.indentationDiff&&(i.indentationDiff=i.indentation,y)){for(i.list=null;C<i.listStack[i.listStack.length-1];)i.listStack.pop(),i.listStack.length?i.indentation=i.listStack[i.listStack.length-1]:i.list=!1;!1!==i.list&&(i.indentationDiff=C-i.listStack[i.listStack.length-1])}var S=!(m||x||i.prevLine.header||y&&v||i.prevLine.fencedCodeEnd),L=(!1===i.list||x||m)&&i.indentation<=b&&r.match(l),T=null;if(i.indentationDiff>=4&&(v||i.prevLine.fencedCodeEnd||i.prevLine.header||m))return r.skipToEnd(),i.indentedCode=!0,o.code;if(r.eatSpace())return null;if(h&&i.indentation<=b&&(T=r.match(u))&&T[1].length<=6)return i.quote=0,i.header=T[1].length,i.thisLine.header=!0,n.highlightFormatting&&(i.formatting="header"),i.f=i.inline,k(i);if(i.indentation<=b&&r.eat(">"))return i.quote=h?1:i.quote+1,n.highlightFormatting&&(i.formatting="quote"),r.eatSpace(),k(i);if(!L&&!i.setext&&h&&i.indentation<=b&&(T=r.match(s))){var M=T[1]?"ol":"ul";return i.indentation=C+r.current().length,i.list=!0,i.quote=0,i.listStack.push(i.indentation),i.em=!1,i.strong=!1,i.code=!1,i.strikethrough=!1,n.taskLists&&r.match(c,!1)&&(i.taskList=!0),i.f=i.inline,n.highlightFormatting&&(i.formatting=["list","list-"+M]),k(i)}return h&&i.indentation<=b&&(T=r.match(f,!0))?(i.quote=0,i.fencedEndRE=new RegExp(T[1]+"+ *$"),i.localMode=n.fencedCodeBlockHighlighting&&function(n){if(e.findModeByName){var r=e.findModeByName(n);r&&(n=r.mime||r.mimes[0])}var i=e.getMode(t,n);return"null"==i.name?null:i}(T[2]||n.fencedCodeBlockDefaultMode),i.localMode&&(i.localState=e.startState(i.localMode)),i.f=i.block=w,n.highlightFormatting&&(i.formatting="code-block"),i.code=-1,k(i)):i.setext||!(S&&y||i.quote||!1!==i.list||i.code||L||p.test(r.string))&&(T=r.lookAhead(1))&&(T=T.match(d))?(i.setext?(i.header=i.setext,i.setext=0,r.skipToEnd(),n.highlightFormatting&&(i.formatting="header")):(i.header="="==T[0].charAt(0)?1:2,i.setext=i.header),i.thisLine.header=!0,i.f=i.inline,k(i)):L?(r.skipToEnd(),i.hr=!0,i.thisLine.hr=!0,o.hr):"["===r.peek()?g(r,i,A):g(r,i,i.inline)}function b(t,n){var o=r.token(t,n.htmlState);if(!i){var a=e.innerMode(r,n.htmlState);("xml"==a.mode.name&&null===a.state.tagStart&&!a.state.context&&a.state.tokenize.isInText||n.md_inside&&t.current().indexOf(">")>-1)&&(n.f=S,n.block=y,n.htmlState=null)}return o}function w(e,t){var r,i=t.listStack[t.listStack.length-1]||0,a=t.indentation<i,l=i+3;return t.fencedEndRE&&t.indentation<=l&&(a||e.match(t.fencedEndRE))?(n.highlightFormatting&&(t.formatting="code-block"),a||(r=k(t)),t.localMode=t.localState=null,t.block=y,t.f=S,t.fencedEndRE=null,t.code=0,t.thisLine.fencedCodeEnd=!0,a?v(e,t,t.block):r):t.localMode?t.localMode.token(e,t.localState):(e.skipToEnd(),o.code)}function k(e){var t=[];if(e.formatting){t.push(o.formatting),"string"==typeof e.formatting&&(e.formatting=[e.formatting]);for(var r=0;r<e.formatting.length;r++)t.push(o.formatting+"-"+e.formatting[r]),"header"===e.formatting[r]&&t.push(o.formatting+"-"+e.formatting[r]+"-"+e.header),"quote"===e.formatting[r]&&(!n.maxBlockquoteDepth||n.maxBlockquoteDepth>=e.quote?t.push(o.formatting+"-"+e.formatting[r]+"-"+e.quote):t.push("error"))}if(e.taskOpen)return t.push("meta"),t.length?t.join(" "):null;if(e.taskClosed)return t.push("property"),t.length?t.join(" "):null;if(e.linkHref?t.push(o.linkHref,"url"):(e.strong&&t.push(o.strong),e.em&&t.push(o.em),e.strikethrough&&t.push(o.strikethrough),e.emoji&&t.push(o.emoji),e.linkText&&t.push(o.linkText),e.code&&t.push(o.code),e.image&&t.push(o.image),e.imageAltText&&t.push(o.imageAltText,"link"),e.imageMarker&&t.push(o.imageMarker)),e.header&&t.push(o.header,o.header+"-"+e.header),e.quote&&(t.push(o.quote),!n.maxBlockquoteDepth||n.maxBlockquoteDepth>=e.quote?t.push(o.quote+"-"+e.quote):t.push(o.quote+"-"+n.maxBlockquoteDepth)),!1!==e.list){var i=(e.listStack.length-1)%3;i?1===i?t.push(o.list2):t.push(o.list3):t.push(o.list1)}return e.trailingSpaceNewLine?t.push("trailing-space-new-line"):e.trailingSpace&&t.push("trailing-space-"+(e.trailingSpace%2?"a":"b")),t.length?t.join(" "):null}function C(e,t){if(e.match(h,!0))return k(t)}function S(t,i){var a=i.text(t,i);if(void 0!==a)return a;if(i.list)return i.list=null,k(i);if(i.taskList)return" "===t.match(c,!0)[1]?i.taskOpen=!0:i.taskClosed=!0,n.highlightFormatting&&(i.formatting="task"),i.taskList=!1,k(i);if(i.taskOpen=!1,i.taskClosed=!1,i.header&&t.match(/^#+$/,!0))return n.highlightFormatting&&(i.formatting="header"),k(i);var l=t.next();if(i.linkTitle){i.linkTitle=!1;var s=l;"("===l&&(s=")");var u="^\\s*(?:[^"+(s=(s+"").replace(/([.?*+^\[\]\\(){}|-])/g,"\\$1"))+"\\\\]+|\\\\\\\\|\\\\.)"+s;if(t.match(new RegExp(u),!0))return o.linkHref}if("`"===l){var d=i.formatting;n.highlightFormatting&&(i.formatting="code"),t.eatWhile("`");var h=t.current().length;if(0!=i.code||i.quote&&1!=h){if(h==i.code){var f=k(i);return i.code=0,f}return i.formatting=d,k(i)}return i.code=h,k(i)}if(i.code)return k(i);if("\\"===l&&(t.next(),n.highlightFormatting)){var p=k(i),g=o.formatting+"-escape";return p?p+" "+g:g}if("!"===l&&t.match(/\[[^\]]*\] ?(?:\(|\[)/,!1))return i.imageMarker=!0,i.image=!0,n.highlightFormatting&&(i.formatting="image"),k(i);if("["===l&&i.imageMarker&&t.match(/[^\]]*\](\(.*?\)| ?\[.*?\])/,!1))return i.imageMarker=!1,i.imageAltText=!0,n.highlightFormatting&&(i.formatting="image"),k(i);if("]"===l&&i.imageAltText){n.highlightFormatting&&(i.formatting="image");var p=k(i);return i.imageAltText=!1,i.image=!1,i.inline=i.f=T,p}if("["===l&&!i.image)return i.linkText&&t.match(/^.*?\]/)||(i.linkText=!0,n.highlightFormatting&&(i.formatting="link")),k(i);if("]"===l&&i.linkText){n.highlightFormatting&&(i.formatting="link");var p=k(i);return i.linkText=!1,i.inline=i.f=t.match(/\(.*?\)| ?\[.*?\]/,!1)?T:S,p}if("<"===l&&t.match(/^(https?|ftps?):\/\/(?:[^\\>]|\\.)+>/,!1))return i.f=i.inline=L,n.highlightFormatting&&(i.formatting="link"),(p=k(i))?p+=" ":p="",p+o.linkInline;if("<"===l&&t.match(/^[^> \\]+@(?:[^\\>]|\\.)+>/,!1))return i.f=i.inline=L,n.highlightFormatting&&(i.formatting="link"),(p=k(i))?p+=" ":p="",p+o.linkEmail;if(n.xml&&"<"===l&&t.match(/^(!--|\?|!\[CDATA\[|[a-z][a-z0-9-]*(?:\s+[a-z_:.\-]+(?:\s*=\s*[^>]+)?)*\s*(?:>|$))/i,!1)){var x=t.string.indexOf(">",t.pos);if(-1!=x){var y=t.string.substring(t.start,x);/markdown\s*=\s*('|"){0,1}1('|"){0,1}/.test(y)&&(i.md_inside=!0)}return t.backUp(1),i.htmlState=e.startState(r),v(t,i,b)}if(n.xml&&"<"===l&&t.match(/^\/\w*?>/))return i.md_inside=!1,"tag";if("*"===l||"_"===l){for(var w=1,C=1==t.pos?" ":t.string.charAt(t.pos-2);w<3&&t.eat(l);)w++;var M=t.peek()||" ",A=!/\s/.test(M)&&(!m.test(M)||/\s/.test(C)||m.test(C)),N=!/\s/.test(C)&&(!m.test(C)||/\s/.test(M)||m.test(M)),D=null,F=null;if(w%2&&(i.em||!A||"*"!==l&&N&&!m.test(C)?i.em!=l||!N||"*"!==l&&A&&!m.test(M)||(D=!1):D=!0),w>1&&(i.strong||!A||"*"!==l&&N&&!m.test(C)?i.strong!=l||!N||"*"!==l&&A&&!m.test(M)||(F=!1):F=!0),null!=F||null!=D)return n.highlightFormatting&&(i.formatting=null==D?"strong":null==F?"em":"strong em"),!0===D&&(i.em=l),!0===F&&(i.strong=l),f=k(i),!1===D&&(i.em=!1),!1===F&&(i.strong=!1),f}else if(" "===l&&(t.eat("*")||t.eat("_"))){if(" "===t.peek())return k(i);t.backUp(1)}if(n.strikethrough)if("~"===l&&t.eatWhile(l)){if(i.strikethrough)return n.highlightFormatting&&(i.formatting="strikethrough"),f=k(i),i.strikethrough=!1,f;if(t.match(/^[^\s]/,!1))return i.strikethrough=!0,n.highlightFormatting&&(i.formatting="strikethrough"),k(i)}else if(" "===l&&t.match(/^~~/,!0)){if(" "===t.peek())return k(i);t.backUp(2)}if(n.emoji&&":"===l&&t.match(/^(?:[a-z_\d+][a-z_\d+-]*|\-[a-z_\d+][a-z_\d+-]*):/)){i.emoji=!0,n.highlightFormatting&&(i.formatting="emoji");var E=k(i);return i.emoji=!1,E}return" "===l&&(t.match(/^ +$/,!1)?i.trailingSpace++:i.trailingSpace&&(i.trailingSpaceNewLine=!0)),k(i)}function L(e,t){if(">"===e.next()){t.f=t.inline=S,n.highlightFormatting&&(t.formatting="link");var r=k(t);return r?r+=" ":r="",r+o.linkInline}return e.match(/^[^>]+/,!0),o.linkInline}function T(e,t){if(e.eatSpace())return null;var r,i=e.next();return"("===i||"["===i?(t.f=t.inline=(r="("===i?")":"]",function(e,t){if(e.next()===r){t.f=t.inline=S,n.highlightFormatting&&(t.formatting="link-string");var i=k(t);return t.linkHref=!1,i}return e.match(M[r]),t.linkHref=!0,k(t)}),n.highlightFormatting&&(t.formatting="link-string"),t.linkHref=!0,k(t)):"error"}var M={")":/^(?:[^\\\(\)]|\\.|\((?:[^\\\(\)]|\\.)*\))*?(?=\))/,"]":/^(?:[^\\\[\]]|\\.|\[(?:[^\\\[\]]|\\.)*\])*?(?=\])/};function A(e,t){return e.match(/^([^\]\\]|\\.)*\]:/,!1)?(t.f=N,e.next(),n.highlightFormatting&&(t.formatting="link"),t.linkText=!0,k(t)):g(e,t,S)}function N(e,t){if(e.match(/^\]:/,!0)){t.f=t.inline=D,n.highlightFormatting&&(t.formatting="link");var r=k(t);return t.linkText=!1,r}return e.match(/^([^\]\\]|\\.)+/,!0),o.linkText}function D(e,t){return e.eatSpace()?null:(e.match(/^[^\s]+/,!0),void 0===e.peek()?t.linkTitle=!0:e.match(/^(?:\s+(?:"(?:[^"\\]|\\\\|\\.)+"|'(?:[^'\\]|\\\\|\\.)+'|\((?:[^)\\]|\\\\|\\.)+\)))?/,!0),t.f=t.inline=S,o.linkHref+" url")}var F={startState:function(){return{f:y,prevLine:{stream:null},thisLine:{stream:null},block:y,htmlState:null,indentation:0,inline:S,text:C,formatting:!1,linkText:!1,linkHref:!1,linkTitle:!1,code:0,em:!1,strong:!1,header:0,setext:0,hr:!1,taskList:!1,list:!1,listStack:[],quote:0,trailingSpace:0,trailingSpaceNewLine:!1,strikethrough:!1,emoji:!1,fencedEndRE:null}},copyState:function(t){return{f:t.f,prevLine:t.prevLine,thisLine:t.thisLine,block:t.block,htmlState:t.htmlState&&e.copyState(r,t.htmlState),indentation:t.indentation,localMode:t.localMode,localState:t.localMode?e.copyState(t.localMode,t.localState):null,inline:t.inline,text:t.text,formatting:!1,linkText:t.linkText,linkTitle:t.linkTitle,linkHref:t.linkHref,code:t.code,em:t.em,strong:t.strong,strikethrough:t.strikethrough,emoji:t.emoji,header:t.header,setext:t.setext,hr:t.hr,taskList:t.taskList,list:t.list,listStack:t.listStack.slice(0),quote:t.quote,indentedCode:t.indentedCode,trailingSpace:t.trailingSpace,trailingSpaceNewLine:t.trailingSpaceNewLine,md_inside:t.md_inside,fencedEndRE:t.fencedEndRE}},token:function(e,t){if(t.formatting=!1,e!=t.thisLine.stream){if(t.header=0,t.hr=!1,e.match(/^\s*$/,!0))return x(t),null;if(t.prevLine=t.thisLine,t.thisLine={stream:e},t.taskList=!1,t.trailingSpace=0,t.trailingSpaceNewLine=!1,!t.localState&&(t.f=t.block,t.f!=b)){var n=e.match(/^\s*/,!0)[0].replace(/\t/g,"    ").length;if(t.indentation=n,t.indentationDiff=null,n>0)return null}}return t.f(e,t)},innerMode:function(e){return e.block==b?{state:e.htmlState,mode:r}:e.localState?{state:e.localState,mode:e.localMode}:{state:e,mode:F}},indent:function(t,n,i){return t.block==b&&r.indent?r.indent(t.htmlState,n,i):t.localState&&t.localMode.indent?t.localMode.indent(t.localState,n,i):e.Pass},blankLine:x,getType:k,blockCommentStart:"\x3c!--",blockCommentEnd:"--\x3e",closeBrackets:"()[]{}''\"\"``",fold:"markdown"};return F}),"xml"),e.defineMIME("text/markdown","markdown"),e.defineMIME("text/x-markdown","markdown")},"object"==typeof n&&"object"==typeof t?r(e("../../lib/codemirror"),e("../xml/xml"),e("../meta")):r(CodeMirror)},{"../../lib/codemirror":9,"../meta":12,"../xml/xml":13}],12:[function(e,t,n){(function(e){"use strict";e.modeInfo=[{name:"APL",mime:"text/apl",mode:"apl",ext:["dyalog","apl"]},{name:"PGP",mimes:["application/pgp","application/pgp-encrypted","application/pgp-keys","application/pgp-signature"],mode:"asciiarmor",ext:["asc","pgp","sig"]},{name:"ASN.1",mime:"text/x-ttcn-asn",mode:"asn.1",ext:["asn","asn1"]},{name:"Asterisk",mime:"text/x-asterisk",mode:"asterisk",file:/^extensions\.conf$/i},{name:"Brainfuck",mime:"text/x-brainfuck",mode:"brainfuck",ext:["b","bf"]},{name:"C",mime:"text/x-csrc",mode:"clike",ext:["c","h","ino"]},{name:"C++",mime:"text/x-c++src",mode:"clike",ext:["cpp","c++","cc","cxx","hpp","h++","hh","hxx"],alias:["cpp"]},{name:"Cobol",mime:"text/x-cobol",mode:"cobol",ext:["cob","cpy"]},{name:"C#",mime:"text/x-csharp",mode:"clike",ext:["cs"],alias:["csharp","cs"]},{name:"Clojure",mime:"text/x-clojure",mode:"clojure",ext:["clj","cljc","cljx"]},{name:"ClojureScript",mime:"text/x-clojurescript",mode:"clojure",ext:["cljs"]},{name:"Closure Stylesheets (GSS)",mime:"text/x-gss",mode:"css",ext:["gss"]},{name:"CMake",mime:"text/x-cmake",mode:"cmake",ext:["cmake","cmake.in"],file:/^CMakeLists\.txt$/},{name:"CoffeeScript",mimes:["application/vnd.coffeescript","text/coffeescript","text/x-coffeescript"],mode:"coffeescript",ext:["coffee"],alias:["coffee","coffee-script"]},{name:"Common Lisp",mime:"text/x-common-lisp",mode:"commonlisp",ext:["cl","lisp","el"],alias:["lisp"]},{name:"Cypher",mime:"application/x-cypher-query",mode:"cypher",ext:["cyp","cypher"]},{name:"Cython",mime:"text/x-cython",mode:"python",ext:["pyx","pxd","pxi"]},{name:"Crystal",mime:"text/x-crystal",mode:"crystal",ext:["cr"]},{name:"CSS",mime:"text/css",mode:"css",ext:["css"]},{name:"CQL",mime:"text/x-cassandra",mode:"sql",ext:["cql"]},{name:"D",mime:"text/x-d",mode:"d",ext:["d"]},{name:"Dart",mimes:["application/dart","text/x-dart"],mode:"dart",ext:["dart"]},{name:"diff",mime:"text/x-diff",mode:"diff",ext:["diff","patch"]},{name:"Django",mime:"text/x-django",mode:"django"},{name:"Dockerfile",mime:"text/x-dockerfile",mode:"dockerfile",file:/^Dockerfile$/},{name:"DTD",mime:"application/xml-dtd",mode:"dtd",ext:["dtd"]},{name:"Dylan",mime:"text/x-dylan",mode:"dylan",ext:["dylan","dyl","intr"]},{name:"EBNF",mime:"text/x-ebnf",mode:"ebnf"},{name:"ECL",mime:"text/x-ecl",mode:"ecl",ext:["ecl"]},{name:"edn",mime:"application/edn",mode:"clojure",ext:["edn"]},{name:"Eiffel",mime:"text/x-eiffel",mode:"eiffel",ext:["e"]},{name:"Elm",mime:"text/x-elm",mode:"elm",ext:["elm"]},{name:"Embedded Javascript",mime:"application/x-ejs",mode:"htmlembedded",ext:["ejs"]},{name:"Embedded Ruby",mime:"application/x-erb",mode:"htmlembedded",ext:["erb"]},{name:"Erlang",mime:"text/x-erlang",mode:"erlang",ext:["erl"]},{name:"Esper",mime:"text/x-esper",mode:"sql"},{name:"Factor",mime:"text/x-factor",mode:"factor",ext:["factor"]},{name:"FCL",mime:"text/x-fcl",mode:"fcl"},{name:"Forth",mime:"text/x-forth",mode:"forth",ext:["forth","fth","4th"]},{name:"Fortran",mime:"text/x-fortran",mode:"fortran",ext:["f","for","f77","f90","f95"]},{name:"F#",mime:"text/x-fsharp",mode:"mllike",ext:["fs"],alias:["fsharp"]},{name:"Gas",mime:"text/x-gas",mode:"gas",ext:["s"]},{name:"Gherkin",mime:"text/x-feature",mode:"gherkin",ext:["feature"]},{name:"GitHub Flavored Markdown",mime:"text/x-gfm",mode:"gfm",file:/^(readme|contributing|history)\.md$/i},{name:"Go",mime:"text/x-go",mode:"go",ext:["go"]},{name:"Groovy",mime:"text/x-groovy",mode:"groovy",ext:["groovy","gradle"],file:/^Jenkinsfile$/},{name:"HAML",mime:"text/x-haml",mode:"haml",ext:["haml"]},{name:"Haskell",mime:"text/x-haskell",mode:"haskell",ext:["hs"]},{name:"Haskell (Literate)",mime:"text/x-literate-haskell",mode:"haskell-literate",ext:["lhs"]},{name:"Haxe",mime:"text/x-haxe",mode:"haxe",ext:["hx"]},{name:"HXML",mime:"text/x-hxml",mode:"haxe",ext:["hxml"]},{name:"ASP.NET",mime:"application/x-aspx",mode:"htmlembedded",ext:["aspx"],alias:["asp","aspx"]},{name:"HTML",mime:"text/html",mode:"htmlmixed",ext:["html","htm","handlebars","hbs"],alias:["xhtml"]},{name:"HTTP",mime:"message/http",mode:"http"},{name:"IDL",mime:"text/x-idl",mode:"idl",ext:["pro"]},{name:"Pug",mime:"text/x-pug",mode:"pug",ext:["jade","pug"],alias:["jade"]},{name:"Java",mime:"text/x-java",mode:"clike",ext:["java"]},{name:"Java Server Pages",mime:"application/x-jsp",mode:"htmlembedded",ext:["jsp"],alias:["jsp"]},{name:"JavaScript",mimes:["text/javascript","text/ecmascript","application/javascript","application/x-javascript","application/ecmascript"],mode:"javascript",ext:["js"],alias:["ecmascript","js","node"]},{name:"JSON",mimes:["application/json","application/x-json"],mode:"javascript",ext:["json","map"],alias:["json5"]},{name:"JSON-LD",mime:"application/ld+json",mode:"javascript",ext:["jsonld"],alias:["jsonld"]},{name:"JSX",mime:"text/jsx",mode:"jsx",ext:["jsx"]},{name:"Jinja2",mime:"text/jinja2",mode:"jinja2",ext:["j2","jinja","jinja2"]},{name:"Julia",mime:"text/x-julia",mode:"julia",ext:["jl"]},{name:"Kotlin",mime:"text/x-kotlin",mode:"clike",ext:["kt"]},{name:"LESS",mime:"text/x-less",mode:"css",ext:["less"]},{name:"LiveScript",mime:"text/x-livescript",mode:"livescript",ext:["ls"],alias:["ls"]},{name:"Lua",mime:"text/x-lua",mode:"lua",ext:["lua"]},{name:"Markdown",mime:"text/x-markdown",mode:"markdown",ext:["markdown","md","mkd"]},{name:"mIRC",mime:"text/mirc",mode:"mirc"},{name:"MariaDB SQL",mime:"text/x-mariadb",mode:"sql"},{name:"Mathematica",mime:"text/x-mathematica",mode:"mathematica",ext:["m","nb","wl","wls"]},{name:"Modelica",mime:"text/x-modelica",mode:"modelica",ext:["mo"]},{name:"MUMPS",mime:"text/x-mumps",mode:"mumps",ext:["mps"]},{name:"MS SQL",mime:"text/x-mssql",mode:"sql"},{name:"mbox",mime:"application/mbox",mode:"mbox",ext:["mbox"]},{name:"MySQL",mime:"text/x-mysql",mode:"sql"},{name:"Nginx",mime:"text/x-nginx-conf",mode:"nginx",file:/nginx.*\.conf$/i},{name:"NSIS",mime:"text/x-nsis",mode:"nsis",ext:["nsh","nsi"]},{name:"NTriples",mimes:["application/n-triples","application/n-quads","text/n-triples"],mode:"ntriples",ext:["nt","nq"]},{name:"Objective-C",mime:"text/x-objectivec",mode:"clike",ext:["m"],alias:["objective-c","objc"]},{name:"Objective-C++",mime:"text/x-objectivec++",mode:"clike",ext:["mm"],alias:["objective-c++","objc++"]},{name:"OCaml",mime:"text/x-ocaml",mode:"mllike",ext:["ml","mli","mll","mly"]},{name:"Octave",mime:"text/x-octave",mode:"octave",ext:["m"]},{name:"Oz",mime:"text/x-oz",mode:"oz",ext:["oz"]},{name:"Pascal",mime:"text/x-pascal",mode:"pascal",ext:["p","pas"]},{name:"PEG.js",mime:"null",mode:"pegjs",ext:["jsonld"]},{name:"Perl",mime:"text/x-perl",mode:"perl",ext:["pl","pm"]},{name:"PHP",mimes:["text/x-php","application/x-httpd-php","application/x-httpd-php-open"],mode:"php",ext:["php","php3","php4","php5","php7","phtml"]},{name:"Pig",mime:"text/x-pig",mode:"pig",ext:["pig"]},{name:"Plain Text",mime:"text/plain",mode:"null",ext:["txt","text","conf","def","list","log"]},{name:"PLSQL",mime:"text/x-plsql",mode:"sql",ext:["pls"]},{name:"PostgreSQL",mime:"text/x-pgsql",mode:"sql"},{name:"PowerShell",mime:"application/x-powershell",mode:"powershell",ext:["ps1","psd1","psm1"]},{name:"Properties files",mime:"text/x-properties",mode:"properties",ext:["properties","ini","in"],alias:["ini","properties"]},{name:"ProtoBuf",mime:"text/x-protobuf",mode:"protobuf",ext:["proto"]},{name:"Python",mime:"text/x-python",mode:"python",ext:["BUILD","bzl","py","pyw"],file:/^(BUCK|BUILD)$/},{name:"Puppet",mime:"text/x-puppet",mode:"puppet",ext:["pp"]},{name:"Q",mime:"text/x-q",mode:"q",ext:["q"]},{name:"R",mime:"text/x-rsrc",mode:"r",ext:["r","R"],alias:["rscript"]},{name:"reStructuredText",mime:"text/x-rst",mode:"rst",ext:["rst"],alias:["rst"]},{name:"RPM Changes",mime:"text/x-rpm-changes",mode:"rpm"},{name:"RPM Spec",mime:"text/x-rpm-spec",mode:"rpm",ext:["spec"]},{name:"Ruby",mime:"text/x-ruby",mode:"ruby",ext:["rb"],alias:["jruby","macruby","rake","rb","rbx"]},{name:"Rust",mime:"text/x-rustsrc",mode:"rust",ext:["rs"]},{name:"SAS",mime:"text/x-sas",mode:"sas",ext:["sas"]},{name:"Sass",mime:"text/x-sass",mode:"sass",ext:["sass"]},{name:"Scala",mime:"text/x-scala",mode:"clike",ext:["scala"]},{name:"Scheme",mime:"text/x-scheme",mode:"scheme",ext:["scm","ss"]},{name:"SCSS",mime:"text/x-scss",mode:"css",ext:["scss"]},{name:"Shell",mimes:["text/x-sh","application/x-sh"],mode:"shell",ext:["sh","ksh","bash"],alias:["bash","sh","zsh"],file:/^PKGBUILD$/},{name:"Sieve",mime:"application/sieve",mode:"sieve",ext:["siv","sieve"]},{name:"Slim",mimes:["text/x-slim","application/x-slim"],mode:"slim",ext:["slim"]},{name:"Smalltalk",mime:"text/x-stsrc",mode:"smalltalk",ext:["st"]},{name:"Smarty",mime:"text/x-smarty",mode:"smarty",ext:["tpl"]},{name:"Solr",mime:"text/x-solr",mode:"solr"},{name:"SML",mime:"text/x-sml",mode:"mllike",ext:["sml","sig","fun","smackspec"]},{name:"Soy",mime:"text/x-soy",mode:"soy",ext:["soy"],alias:["closure template"]},{name:"SPARQL",mime:"application/sparql-query",mode:"sparql",ext:["rq","sparql"],alias:["sparul"]},{name:"Spreadsheet",mime:"text/x-spreadsheet",mode:"spreadsheet",alias:["excel","formula"]},{name:"SQL",mime:"text/x-sql",mode:"sql",ext:["sql"]},{name:"SQLite",mime:"text/x-sqlite",mode:"sql"},{name:"Squirrel",mime:"text/x-squirrel",mode:"clike",ext:["nut"]},{name:"Stylus",mime:"text/x-styl",mode:"stylus",ext:["styl"]},{name:"Swift",mime:"text/x-swift",mode:"swift",ext:["swift"]},{name:"sTeX",mime:"text/x-stex",mode:"stex"},{name:"LaTeX",mime:"text/x-latex",mode:"stex",ext:["text","ltx","tex"],alias:["tex"]},{name:"SystemVerilog",mime:"text/x-systemverilog",mode:"verilog",ext:["v","sv","svh"]},{name:"Tcl",mime:"text/x-tcl",mode:"tcl",ext:["tcl"]},{name:"Textile",mime:"text/x-textile",mode:"textile",ext:["textile"]},{name:"TiddlyWiki",mime:"text/x-tiddlywiki",mode:"tiddlywiki"},{name:"Tiki wiki",mime:"text/tiki",mode:"tiki"},{name:"TOML",mime:"text/x-toml",mode:"toml",ext:["toml"]},{name:"Tornado",mime:"text/x-tornado",mode:"tornado"},{name:"troff",mime:"text/troff",mode:"troff",ext:["1","2","3","4","5","6","7","8","9"]},{name:"TTCN",mime:"text/x-ttcn",mode:"ttcn",ext:["ttcn","ttcn3","ttcnpp"]},{name:"TTCN_CFG",mime:"text/x-ttcn-cfg",mode:"ttcn-cfg",ext:["cfg"]},{name:"Turtle",mime:"text/turtle",mode:"turtle",ext:["ttl"]},{name:"TypeScript",mime:"application/typescript",mode:"javascript",ext:["ts"],alias:["ts"]},{name:"TypeScript-JSX",mime:"text/typescript-jsx",mode:"jsx",ext:["tsx"],alias:["tsx"]},{name:"Twig",mime:"text/x-twig",mode:"twig"},{name:"Web IDL",mime:"text/x-webidl",mode:"webidl",ext:["webidl"]},{name:"VB.NET",mime:"text/x-vb",mode:"vb",ext:["vb"]},{name:"VBScript",mime:"text/vbscript",mode:"vbscript",ext:["vbs"]},{name:"Velocity",mime:"text/velocity",mode:"velocity",ext:["vtl"]},{name:"Verilog",mime:"text/x-verilog",mode:"verilog",ext:["v"]},{name:"VHDL",mime:"text/x-vhdl",mode:"vhdl",ext:["vhd","vhdl"]},{name:"Vue.js Component",mimes:["script/x-vue","text/x-vue"],mode:"vue",ext:["vue"]},{name:"XML",mimes:["application/xml","text/xml"],mode:"xml",ext:["xml","xsl","xsd","svg"],alias:["rss","wsdl","xsd"]},{name:"XQuery",mime:"application/xquery",mode:"xquery",ext:["xy","xquery"]},{name:"Yacas",mime:"text/x-yacas",mode:"yacas",ext:["ys"]},{name:"YAML",mimes:["text/x-yaml","text/yaml"],mode:"yaml",ext:["yaml","yml"],alias:["yml"]},{name:"Z80",mime:"text/x-z80",mode:"z80",ext:["z80"]},{name:"mscgen",mime:"text/x-mscgen",mode:"mscgen",ext:["mscgen","mscin","msc"]},{name:"xu",mime:"text/x-xu",mode:"mscgen",ext:["xu"]},{name:"msgenny",mime:"text/x-msgenny",mode:"mscgen",ext:["msgenny"]}];for(var t=0;t<e.modeInfo.length;t++){var n=e.modeInfo[t];n.mimes&&(n.mime=n.mimes[0])}e.findModeByMIME=function(t){t=t.toLowerCase();for(var n=0;n<e.modeInfo.length;n++){var r=e.modeInfo[n];if(r.mime==t)return r;if(r.mimes)for(var i=0;i<r.mimes.length;i++)if(r.mimes[i]==t)return r}return/\+xml$/.test(t)?e.findModeByMIME("application/xml"):/\+json$/.test(t)?e.findModeByMIME("application/json"):void 0},e.findModeByExtension=function(t){t=t.toLowerCase();for(var n=0;n<e.modeInfo.length;n++){var r=e.modeInfo[n];if(r.ext)for(var i=0;i<r.ext.length;i++)if(r.ext[i]==t)return r}},e.findModeByFileName=function(t){for(var n=0;n<e.modeInfo.length;n++){var r=e.modeInfo[n];if(r.file&&r.file.test(t))return r}var i=t.lastIndexOf("."),o=i>-1&&t.substring(i+1,t.length);if(o)return e.findModeByExtension(o)},e.findModeByName=function(t){t=t.toLowerCase();for(var n=0;n<e.modeInfo.length;n++){var r=e.modeInfo[n];if(r.name.toLowerCase()==t)return r;if(r.alias)for(var i=0;i<r.alias.length;i++)if(r.alias[i].toLowerCase()==t)return r}}})("object"==typeof n&&"object"==typeof t?e("../lib/codemirror"):CodeMirror)},{"../lib/codemirror":9}],13:[function(e,t,n){(function(e){"use strict";var t={autoSelfClosers:{area:!0,base:!0,br:!0,col:!0,command:!0,embed:!0,frame:!0,hr:!0,img:!0,input:!0,keygen:!0,link:!0,meta:!0,param:!0,source:!0,track:!0,wbr:!0,menuitem:!0},implicitlyClosed:{dd:!0,li:!0,optgroup:!0,option:!0,p:!0,rp:!0,rt:!0,tbody:!0,td:!0,tfoot:!0,th:!0,tr:!0},contextGrabbers:{dd:{dd:!0,dt:!0},dt:{dd:!0,dt:!0},li:{li:!0},option:{option:!0,optgroup:!0},optgroup:{optgroup:!0},p:{address:!0,article:!0,aside:!0,blockquote:!0,dir:!0,div:!0,dl:!0,fieldset:!0,footer:!0,form:!0,h1:!0,h2:!0,h3:!0,h4:!0,h5:!0,h6:!0,header:!0,hgroup:!0,hr:!0,menu:!0,nav:!0,ol:!0,p:!0,pre:!0,section:!0,table:!0,ul:!0},rp:{rp:!0,rt:!0},rt:{rp:!0,rt:!0},tbody:{tbody:!0,tfoot:!0},td:{td:!0,th:!0},tfoot:{tbody:!0},th:{td:!0,th:!0},thead:{tbody:!0,tfoot:!0},tr:{tr:!0}},doNotIndent:{pre:!0},allowUnquoted:!0,allowMissing:!0,caseFold:!0},n={autoSelfClosers:{},implicitlyClosed:{},contextGrabbers:{},doNotIndent:{},allowUnquoted:!1,allowMissing:!1,allowMissingTagName:!1,caseFold:!1};e.defineMode("xml",(function(r,i){var o,a,l=r.indentUnit,s={},c=i.htmlMode?t:n;for(var u in c)s[u]=c[u];for(var u in i)s[u]=i[u];function d(e,t){function n(n){return t.tokenize=n,n(e,t)}var r=e.next();return"<"==r?e.eat("!")?e.eat("[")?e.match("CDATA[")?n(f("atom","]]>")):null:e.match("--")?n(f("comment","--\x3e")):e.match("DOCTYPE",!0,!0)?(e.eatWhile(/[\w\._\-]/),n(p(1))):null:e.eat("?")?(e.eatWhile(/[\w\._\-]/),t.tokenize=f("meta","?>"),"meta"):(o=e.eat("/")?"closeTag":"openTag",t.tokenize=h,"tag bracket"):"&"==r?(e.eat("#")?e.eat("x")?e.eatWhile(/[a-fA-F\d]/)&&e.eat(";"):e.eatWhile(/[\d]/)&&e.eat(";"):e.eatWhile(/[\w\.\-:]/)&&e.eat(";"))?"atom":"error":(e.eatWhile(/[^&<]/),null)}function h(e,t){var n,r,i=e.next();if(">"==i||"/"==i&&e.eat(">"))return t.tokenize=d,o=">"==i?"endTag":"selfcloseTag","tag bracket";if("="==i)return o="equals",null;if("<"==i){t.tokenize=d,t.state=x,t.tagName=t.tagStart=null;var a=t.tokenize(e,t);return a?a+" tag error":"tag error"}return/[\'\"]/.test(i)?(t.tokenize=(n=i,(r=function(e,t){for(;!e.eol();)if(e.next()==n){t.tokenize=h;break}return"string"}).isInAttribute=!0,r),t.stringStartCol=e.column(),t.tokenize(e,t)):(e.match(/^[^\s\u00a0=<>\"\']*[^\s\u00a0=<>\"\'\/]/),"word")}function f(e,t){return function(n,r){for(;!n.eol();){if(n.match(t)){r.tokenize=d;break}n.next()}return e}}function p(e){return function(t,n){for(var r;null!=(r=t.next());){if("<"==r)return n.tokenize=p(e+1),n.tokenize(t,n);if(">"==r){if(1==e){n.tokenize=d;break}return n.tokenize=p(e-1),n.tokenize(t,n)}}return"meta"}}function m(e,t,n){this.prev=e.context,this.tagName=t,this.indent=e.indented,this.startOfLine=n,(s.doNotIndent.hasOwnProperty(t)||e.context&&e.context.noIndent)&&(this.noIndent=!0)}function g(e){e.context&&(e.context=e.context.prev)}function v(e,t){for(var n;;){if(!e.context)return;if(n=e.context.tagName,!s.contextGrabbers.hasOwnProperty(n)||!s.contextGrabbers[n].hasOwnProperty(t))return;g(e)}}function x(e,t,n){return"openTag"==e?(n.tagStart=t.column(),y):"closeTag"==e?b:x}function y(e,t,n){return"word"==e?(n.tagName=t.current(),a="tag",C):s.allowMissingTagName&&"endTag"==e?(a="tag bracket",C(e,0,n)):(a="error",y)}function b(e,t,n){if("word"==e){var r=t.current();return n.context&&n.context.tagName!=r&&s.implicitlyClosed.hasOwnProperty(n.context.tagName)&&g(n),n.context&&n.context.tagName==r||!1===s.matchClosing?(a="tag",w):(a="tag error",k)}return s.allowMissingTagName&&"endTag"==e?(a="tag bracket",w(e,0,n)):(a="error",k)}function w(e,t,n){return"endTag"!=e?(a="error",w):(g(n),x)}function k(e,t,n){return a="error",w(e,0,n)}function C(e,t,n){if("word"==e)return a="attribute",S;if("endTag"==e||"selfcloseTag"==e){var r=n.tagName,i=n.tagStart;return n.tagName=n.tagStart=null,"selfcloseTag"==e||s.autoSelfClosers.hasOwnProperty(r)?v(n,r):(v(n,r),n.context=new m(n,r,i==n.indented)),x}return a="error",C}function S(e,t,n){return"equals"==e?L:(s.allowMissing||(a="error"),C(e,0,n))}function L(e,t,n){return"string"==e?T:"word"==e&&s.allowUnquoted?(a="string",C):(a="error",C(e,0,n))}function T(e,t,n){return"string"==e?T:C(e,0,n)}return d.isInText=!0,{startState:function(e){var t={tokenize:d,state:x,indented:e||0,tagName:null,tagStart:null,context:null};return null!=e&&(t.baseIndent=e),t},token:function(e,t){if(!t.tagName&&e.sol()&&(t.indented=e.indentation()),e.eatSpace())return null;o=null;var n=t.tokenize(e,t);return(n||o)&&"comment"!=n&&(a=null,t.state=t.state(o||n,e,t),a&&(n="error"==a?n+" error":a)),n},indent:function(t,n,r){var i=t.context;if(t.tokenize.isInAttribute)return t.tagStart==t.indented?t.stringStartCol+1:t.indented+l;if(i&&i.noIndent)return e.Pass;if(t.tokenize!=h&&t.tokenize!=d)return r?r.match(/^(\s*)/)[0].length:0;if(t.tagName)return!1!==s.multilineTagIndentPastTag?t.tagStart+t.tagName.length+2:t.tagStart+l*(s.multilineTagIndentFactor||1);if(s.alignCDATA&&/<!\[CDATA\[/.test(n))return 0;var o=n&&/^<(\/)?([\w_:\.-]*)/.exec(n);if(o&&o[1])for(;i;){if(i.tagName==o[2]){i=i.prev;break}if(!s.implicitlyClosed.hasOwnProperty(i.tagName))break;i=i.prev}else if(o)for(;i;){var a=s.contextGrabbers[i.tagName];if(!a||!a.hasOwnProperty(o[2]))break;i=i.prev}for(;i&&i.prev&&!i.startOfLine;)i=i.prev;return i?i.indent+l:t.baseIndent||0},electricInput:/<\/[\s\w:]+>$/,blockCommentStart:"\x3c!--",blockCommentEnd:"--\x3e",configuration:s.htmlMode?"html":"xml",helperType:s.htmlMode?"html":"xml",skipAttribute:function(e){e.state==L&&(e.state=C)},xmlCurrentTag:function(e){return e.tagName?{name:e.tagName,close:"closeTag"==e.type}:null},xmlCurrentContext:function(e){for(var t=[],n=e.context;n;n=n.prev)n.tagName&&t.push(n.tagName);return t.reverse()}}})),e.defineMIME("text/xml","xml"),e.defineMIME("application/xml","xml"),e.mimeModes.hasOwnProperty("text/html")||e.defineMIME("text/html",{name:"xml",htmlMode:!0})})("object"==typeof n&&"object"==typeof t?e("../../lib/codemirror"):CodeMirror)},{"../../lib/codemirror":9}],14:[function(e,t,n){!function(e,r){"object"==typeof n&&void 0!==t?t.exports=r():(e=e||self).marked=r()}(this,(function(){"use strict";function e(e,t){for(var n=0;n<t.length;n++){var r=t[n];r.enumerable=r.enumerable||!1,r.configurable=!0,"value"in r&&(r.writable=!0),Object.defineProperty(e,r.key,r)}}function t(e,t){(null==t||t>e.length)&&(t=e.length);for(var n=0,r=new Array(t);n<t;n++)r[n]=e[n];return r}function n(e,n){var r;if("undefined"==typeof Symbol||null==e[Symbol.iterator]){if(Array.isArray(e)||(r=function(e,n){if(e){if("string"==typeof e)return t(e,n);var r=Object.prototype.toString.call(e).slice(8,-1);return"Object"===r&&e.constructor&&(r=e.constructor.name),"Map"===r||"Set"===r?Array.from(e):"Arguments"===r||/^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(r)?t(e,n):void 0}}(e))||n&&e&&"number"==typeof e.length){r&&(e=r);var i=0;return function(){return i>=e.length?{done:!0}:{done:!1,value:e[i++]}}}throw new TypeError("Invalid attempt to iterate non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")}return(r=e[Symbol.iterator]()).next.bind(r)}var r=function(e,t){return e(t={exports:{}},t.exports),t.exports}((function(e){function t(){return{baseUrl:null,breaks:!1,gfm:!0,headerIds:!0,headerPrefix:"",highlight:null,langPrefix:"language-",mangle:!0,pedantic:!1,renderer:null,sanitize:!1,sanitizer:null,silent:!1,smartLists:!1,smartypants:!1,tokenizer:null,walkTokens:null,xhtml:!1}}e.exports={defaults:{baseUrl:null,breaks:!1,gfm:!0,headerIds:!0,headerPrefix:"",highlight:null,langPrefix:"language-",mangle:!0,pedantic:!1,renderer:null,sanitize:!1,sanitizer:null,silent:!1,smartLists:!1,smartypants:!1,tokenizer:null,walkTokens:null,xhtml:!1},getDefaults:t,changeDefaults:function(t){e.exports.defaults=t}}})),i=(r.defaults,r.getDefaults,r.changeDefaults,/[&<>"']/),o=/[&<>"']/g,a=/[<>"']|&(?!#?\w+;)/,l=/[<>"']|&(?!#?\w+;)/g,s={"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;"},c=function(e){return s[e]};var u=/&(#(?:\d+)|(?:#x[0-9A-Fa-f]+)|(?:\w+));?/gi;function d(e){return e.replace(u,(function(e,t){return"colon"===(t=t.toLowerCase())?":":"#"===t.charAt(0)?"x"===t.charAt(1)?String.fromCharCode(parseInt(t.substring(2),16)):String.fromCharCode(+t.substring(1)):""}))}var h=/(^|[^\[])\^/g;var f=/[^\w:]/g,p=/^$|^[a-z][a-z0-9+.-]*:|^[?#]/i;var m={},g=/^[^:]+:\/*[^/]*$/,v=/^([^:]+:)[\s\S]*$/,x=/^([^:]+:\/*[^/]*)[\s\S]*$/;function y(e,t){m[" "+e]||(g.test(e)?m[" "+e]=e+"/":m[" "+e]=b(e,"/",!0));var n=-1===(e=m[" "+e]).indexOf(":");return"//"===t.substring(0,2)?n?t:e.replace(v,"$1")+t:"/"===t.charAt(0)?n?t:e.replace(x,"$1")+t:e+t}function b(e,t,n){var r=e.length;if(0===r)return"";for(var i=0;i<r;){var o=e.charAt(r-i-1);if(o!==t||n){if(o===t||!n)break;i++}else i++}return e.substr(0,r-i)}var w=function(e,t){if(t){if(i.test(e))return e.replace(o,c)}else if(a.test(e))return e.replace(l,c);return e},k=d,C=function(e,t){e=e.source||e,t=t||"";var n={replace:function(t,r){return r=(r=r.source||r).replace(h,"$1"),e=e.replace(t,r),n},getRegex:function(){return new RegExp(e,t)}};return n},S=function(e,t,n){if(e){var r;try{r=decodeURIComponent(d(n)).replace(f,"").toLowerCase()}catch(e){return null}if(0===r.indexOf("javascript:")||0===r.indexOf("vbscript:")||0===r.indexOf("data:"))return null}t&&!p.test(n)&&(n=y(t,n));try{n=encodeURI(n).replace(/%25/g,"%")}catch(e){return null}return n},L={exec:function(){}},T=function(e){for(var t,n,r=1;r<arguments.length;r++)for(n in t=arguments[r])Object.prototype.hasOwnProperty.call(t,n)&&(e[n]=t[n]);return e},M=function(e,t){var n=e.replace(/\|/g,(function(e,t,n){for(var r=!1,i=t;--i>=0&&"\\"===n[i];)r=!r;return r?"|":" |"})).split(/ \|/),r=0;if(n.length>t)n.splice(t);else for(;n.length<t;)n.push("");for(;r<n.length;r++)n[r]=n[r].trim().replace(/\\\|/g,"|");return n},A=b,N=function(e,t){if(-1===e.indexOf(t[1]))return-1;for(var n=e.length,r=0,i=0;i<n;i++)if("\\"===e[i])i++;else if(e[i]===t[0])r++;else if(e[i]===t[1]&&--r<0)return i;return-1},D=function(e){e&&e.sanitize&&!e.silent&&console.warn("marked(): sanitize and sanitizer parameters are deprecated since version 0.7.0, should not be used and will be removed in the future. Read more here: https://marked.js.org/#/USING_ADVANCED.md#options")},F=r.defaults,E=A,O=M,I=w,z=N;function H(e,t,n){var r=t.href,i=t.title?I(t.title):null,o=e[1].replace(/\\([\[\]])/g,"$1");return"!"!==e[0].charAt(0)?{type:"link",raw:n,href:r,title:i,text:o}:{type:"image",raw:n,href:r,title:i,text:I(o)}}var R=function(){function e(e){this.options=e||F}var t=e.prototype;return t.space=function(e){var t=this.rules.block.newline.exec(e);if(t)return t[0].length>1?{type:"space",raw:t[0]}:{raw:"\n"}},t.code=function(e,t){var n=this.rules.block.code.exec(e);if(n){var r=t[t.length-1];if(r&&"paragraph"===r.type)return{raw:n[0],text:n[0].trimRight()};var i=n[0].replace(/^ {4}/gm,"");return{type:"code",raw:n[0],codeBlockStyle:"indented",text:this.options.pedantic?i:E(i,"\n")}}},t.fences=function(e){var t=this.rules.block.fences.exec(e);if(t){var n=t[0],r=function(e,t){var n=e.match(/^(\s+)(?:```)/);if(null===n)return t;var r=n[1];return t.split("\n").map((function(e){var t=e.match(/^\s+/);return null===t?e:t[0].length>=r.length?e.slice(r.length):e})).join("\n")}(n,t[3]||"");return{type:"code",raw:n,lang:t[2]?t[2].trim():t[2],text:r}}},t.heading=function(e){var t=this.rules.block.heading.exec(e);if(t)return{type:"heading",raw:t[0],depth:t[1].length,text:t[2]}},t.nptable=function(e){var t=this.rules.block.nptable.exec(e);if(t){var n={type:"table",header:O(t[1].replace(/^ *| *\| *$/g,"")),align:t[2].replace(/^ *|\| *$/g,"").split(/ *\| */),cells:t[3]?t[3].replace(/\n$/,"").split("\n"):[],raw:t[0]};if(n.header.length===n.align.length){var r,i=n.align.length;for(r=0;r<i;r++)/^ *-+: *$/.test(n.align[r])?n.align[r]="right":/^ *:-+: *$/.test(n.align[r])?n.align[r]="center":/^ *:-+ *$/.test(n.align[r])?n.align[r]="left":n.align[r]=null;for(i=n.cells.length,r=0;r<i;r++)n.cells[r]=O(n.cells[r],n.header.length);return n}}},t.hr=function(e){var t=this.rules.block.hr.exec(e);if(t)return{type:"hr",raw:t[0]}},t.blockquote=function(e){var t=this.rules.block.blockquote.exec(e);if(t){var n=t[0].replace(/^ *> ?/gm,"");return{type:"blockquote",raw:t[0],text:n}}},t.list=function(e){var t=this.rules.block.list.exec(e);if(t){for(var n,r,i,o,a,l,s,c=t[0],u=t[2],d=u.length>1,h=")"===u[u.length-1],f={type:"list",raw:c,ordered:d,start:d?+u.slice(0,-1):"",loose:!1,items:[]},p=t[0].match(this.rules.block.item),m=!1,g=p.length,v=0;v<g;v++)c=n=p[v],r=n.length,~(n=n.replace(/^ *([*+-]|\d+[.)]) */,"")).indexOf("\n ")&&(r-=n.length,n=this.options.pedantic?n.replace(/^ {1,4}/gm,""):n.replace(new RegExp("^ {1,"+r+"}","gm"),"")),v!==g-1&&(i=this.rules.block.bullet.exec(p[v+1])[0],(d?1===i.length||!h&&")"===i[i.length-1]:i.length>1||this.options.smartLists&&i!==u)&&(o=p.slice(v+1).join("\n"),f.raw=f.raw.substring(0,f.raw.length-o.length),v=g-1)),a=m||/\n\n(?!\s*$)/.test(n),v!==g-1&&(m="\n"===n.charAt(n.length-1),a||(a=m)),a&&(f.loose=!0),s=void 0,(l=/^\[[ xX]\] /.test(n))&&(s=" "!==n[1],n=n.replace(/^\[[ xX]\] +/,"")),f.items.push({type:"list_item",raw:c,task:l,checked:s,loose:a,text:n});return f}},t.html=function(e){var t=this.rules.block.html.exec(e);if(t)return{type:this.options.sanitize?"paragraph":"html",raw:t[0],pre:!this.options.sanitizer&&("pre"===t[1]||"script"===t[1]||"style"===t[1]),text:this.options.sanitize?this.options.sanitizer?this.options.sanitizer(t[0]):I(t[0]):t[0]}},t.def=function(e){var t=this.rules.block.def.exec(e);if(t)return t[3]&&(t[3]=t[3].substring(1,t[3].length-1)),{tag:t[1].toLowerCase().replace(/\s+/g," "),raw:t[0],href:t[2],title:t[3]}},t.table=function(e){var t=this.rules.block.table.exec(e);if(t){var n={type:"table",header:O(t[1].replace(/^ *| *\| *$/g,"")),align:t[2].replace(/^ *|\| *$/g,"").split(/ *\| */),cells:t[3]?t[3].replace(/\n$/,"").split("\n"):[]};if(n.header.length===n.align.length){n.raw=t[0];var r,i=n.align.length;for(r=0;r<i;r++)/^ *-+: *$/.test(n.align[r])?n.align[r]="right":/^ *:-+: *$/.test(n.align[r])?n.align[r]="center":/^ *:-+ *$/.test(n.align[r])?n.align[r]="left":n.align[r]=null;for(i=n.cells.length,r=0;r<i;r++)n.cells[r]=O(n.cells[r].replace(/^ *\| *| *\| *$/g,""),n.header.length);return n}}},t.lheading=function(e){var t=this.rules.block.lheading.exec(e);if(t)return{type:"heading",raw:t[0],depth:"="===t[2].charAt(0)?1:2,text:t[1]}},t.paragraph=function(e){var t=this.rules.block.paragraph.exec(e);if(t)return{type:"paragraph",raw:t[0],text:"\n"===t[1].charAt(t[1].length-1)?t[1].slice(0,-1):t[1]}},t.text=function(e,t){var n=this.rules.block.text.exec(e);if(n){var r=t[t.length-1];return r&&"text"===r.type?{raw:n[0],text:n[0]}:{type:"text",raw:n[0],text:n[0]}}},t.escape=function(e){var t=this.rules.inline.escape.exec(e);if(t)return{type:"escape",raw:t[0],text:I(t[1])}},t.tag=function(e,t,n){var r=this.rules.inline.tag.exec(e);if(r)return!t&&/^<a /i.test(r[0])?t=!0:t&&/^<\/a>/i.test(r[0])&&(t=!1),!n&&/^<(pre|code|kbd|script)(\s|>)/i.test(r[0])?n=!0:n&&/^<\/(pre|code|kbd|script)(\s|>)/i.test(r[0])&&(n=!1),{type:this.options.sanitize?"text":"html",raw:r[0],inLink:t,inRawBlock:n,text:this.options.sanitize?this.options.sanitizer?this.options.sanitizer(r[0]):I(r[0]):r[0]}},t.link=function(e){var t=this.rules.inline.link.exec(e);if(t){var n=z(t[2],"()");if(n>-1){var r=(0===t[0].indexOf("!")?5:4)+t[1].length+n;t[2]=t[2].substring(0,n),t[0]=t[0].substring(0,r).trim(),t[3]=""}var i=t[2],o="";if(this.options.pedantic){var a=/^([^'"]*[^\s])\s+(['"])(.*)\2/.exec(i);a?(i=a[1],o=a[3]):o=""}else o=t[3]?t[3].slice(1,-1):"";return H(t,{href:(i=i.trim().replace(/^<([\s\S]*)>$/,"$1"))?i.replace(this.rules.inline._escapes,"$1"):i,title:o?o.replace(this.rules.inline._escapes,"$1"):o},t[0])}},t.reflink=function(e,t){var n;if((n=this.rules.inline.reflink.exec(e))||(n=this.rules.inline.nolink.exec(e))){var r=(n[2]||n[1]).replace(/\s+/g," ");if(!(r=t[r.toLowerCase()])||!r.href){var i=n[0].charAt(0);return{type:"text",raw:i,text:i}}return H(n,r,n[0])}},t.strong=function(e,t,n){void 0===n&&(n="");var r=this.rules.inline.strong.start.exec(e);if(r&&(!r[1]||r[1]&&(""===n||this.rules.inline.punctuation.exec(n)))){t=t.slice(-1*e.length);var i,o="**"===r[0]?this.rules.inline.strong.endAst:this.rules.inline.strong.endUnd;for(o.lastIndex=0;null!=(r=o.exec(t));)if(i=this.rules.inline.strong.middle.exec(t.slice(0,r.index+3)))return{type:"strong",raw:e.slice(0,i[0].length),text:e.slice(2,i[0].length-2)}}},t.em=function(e,t,n){void 0===n&&(n="");var r=this.rules.inline.em.start.exec(e);if(r&&(!r[1]||r[1]&&(""===n||this.rules.inline.punctuation.exec(n)))){t=t.slice(-1*e.length);var i,o="*"===r[0]?this.rules.inline.em.endAst:this.rules.inline.em.endUnd;for(o.lastIndex=0;null!=(r=o.exec(t));)if(i=this.rules.inline.em.middle.exec(t.slice(0,r.index+2)))return{type:"em",raw:e.slice(0,i[0].length),text:e.slice(1,i[0].length-1)}}},t.codespan=function(e){var t=this.rules.inline.code.exec(e);if(t){var n=t[2].replace(/\n/g," "),r=/[^ ]/.test(n),i=n.startsWith(" ")&&n.endsWith(" ");return r&&i&&(n=n.substring(1,n.length-1)),n=I(n,!0),{type:"codespan",raw:t[0],text:n}}},t.br=function(e){var t=this.rules.inline.br.exec(e);if(t)return{type:"br",raw:t[0]}},t.del=function(e){var t=this.rules.inline.del.exec(e);if(t)return{type:"del",raw:t[0],text:t[1]}},t.autolink=function(e,t){var n,r,i=this.rules.inline.autolink.exec(e);if(i)return r="@"===i[2]?"mailto:"+(n=I(this.options.mangle?t(i[1]):i[1])):n=I(i[1]),{type:"link",raw:i[0],text:n,href:r,tokens:[{type:"text",raw:n,text:n}]}},t.url=function(e,t){var n;if(n=this.rules.inline.url.exec(e)){var r,i;if("@"===n[2])i="mailto:"+(r=I(this.options.mangle?t(n[0]):n[0]));else{var o;do{o=n[0],n[0]=this.rules.inline._backpedal.exec(n[0])[0]}while(o!==n[0]);r=I(n[0]),i="www."===n[1]?"http://"+r:r}return{type:"link",raw:n[0],text:r,href:i,tokens:[{type:"text",raw:r,text:r}]}}},t.inlineText=function(e,t,n){var r,i=this.rules.inline.text.exec(e);if(i)return r=t?this.options.sanitize?this.options.sanitizer?this.options.sanitizer(i[0]):I(i[0]):i[0]:I(this.options.smartypants?n(i[0]):i[0]),{type:"text",raw:i[0],text:r}},e}(),_=L,P=C,W=T,B={newline:/^\n+/,code:/^( {4}[^\n]+\n*)+/,fences:/^ {0,3}(`{3,}(?=[^`\n]*\n)|~{3,})([^\n]*)\n(?:|([\s\S]*?)\n)(?: {0,3}\1[~`]* *(?:\n+|$)|$)/,hr:/^ {0,3}((?:- *){3,}|(?:_ *){3,}|(?:\* *){3,})(?:\n+|$)/,heading:/^ {0,3}(#{1,6}) +([^\n]*?)(?: +#+)? *(?:\n+|$)/,blockquote:/^( {0,3}> ?(paragraph|[^\n]*)(?:\n|$))+/,list:/^( {0,3})(bull) [\s\S]+?(?:hr|def|\n{2,}(?! )(?!\1bull )\n*|\s*$)/,html:"^ {0,3}(?:<(script|pre|style)[\\s>][\\s\\S]*?(?:</\\1>[^\\n]*\\n+|$)|comment[^\\n]*(\\n+|$)|<\\?[\\s\\S]*?(?:\\?>\\n*|$)|<![A-Z][\\s\\S]*?(?:>\\n*|$)|<!\\[CDATA\\[[\\s\\S]*?(?:\\]\\]>\\n*|$)|</?(tag)(?: +|\\n|/?>)[\\s\\S]*?(?:\\n{2,}|$)|<(?!script|pre|style)([a-z][\\w-]*)(?:attribute)*? */?>(?=[ \\t]*(?:\\n|$))[\\s\\S]*?(?:\\n{2,}|$)|</(?!script|pre|style)[a-z][\\w-]*\\s*>(?=[ \\t]*(?:\\n|$))[\\s\\S]*?(?:\\n{2,}|$))",def:/^ {0,3}\[(label)\]: *\n? *<?([^\s>]+)>?(?:(?: +\n? *| *\n *)(title))? *(?:\n+|$)/,nptable:_,table:_,lheading:/^([^\n]+)\n {0,3}(=+|-+) *(?:\n+|$)/,_paragraph:/^([^\n]+(?:\n(?!hr|heading|lheading|blockquote|fences|list|html)[^\n]+)*)/,text:/^[^\n]+/,_label:/(?!\s*\])(?:\\[\[\]]|[^\[\]])+/,_title:/(?:"(?:\\"?|[^"\\])*"|'[^'\n]*(?:\n[^'\n]+)*\n?'|\([^()]*\))/};B.def=P(B.def).replace("label",B._label).replace("title",B._title).getRegex(),B.bullet=/(?:[*+-]|\d{1,9}[.)])/,B.item=/^( *)(bull) ?[^\n]*(?:\n(?!\1bull ?)[^\n]*)*/,B.item=P(B.item,"gm").replace(/bull/g,B.bullet).getRegex(),B.list=P(B.list).replace(/bull/g,B.bullet).replace("hr","\\n+(?=\\1?(?:(?:- *){3,}|(?:_ *){3,}|(?:\\* *){3,})(?:\\n+|$))").replace("def","\\n+(?="+B.def.source+")").getRegex(),B._tag="address|article|aside|base|basefont|blockquote|body|caption|center|col|colgroup|dd|details|dialog|dir|div|dl|dt|fieldset|figcaption|figure|footer|form|frame|frameset|h[1-6]|head|header|hr|html|iframe|legend|li|link|main|menu|menuitem|meta|nav|noframes|ol|optgroup|option|p|param|section|source|summary|table|tbody|td|tfoot|th|thead|title|tr|track|ul",B._comment=/<!--(?!-?>)[\s\S]*?(?:-->|$)/,B.html=P(B.html,"i").replace("comment",B._comment).replace("tag",B._tag).replace("attribute",/ +[a-zA-Z:_][\w.:-]*(?: *= *"[^"\n]*"| *= *'[^'\n]*'| *= *[^\s"'=<>`]+)?/).getRegex(),B.paragraph=P(B._paragraph).replace("hr",B.hr).replace("heading"," {0,3}#{1,6} ").replace("|lheading","").replace("blockquote"," {0,3}>").replace("fences"," {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list"," {0,3}(?:[*+-]|1[.)]) ").replace("html","</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|!--)").replace("tag",B._tag).getRegex(),B.blockquote=P(B.blockquote).replace("paragraph",B.paragraph).getRegex(),B.normal=W({},B),B.gfm=W({},B.normal,{nptable:"^ *([^|\\n ].*\\|.*)\\n {0,3}([-:]+ *\\|[-| :]*)(?:\\n((?:(?!\\n|hr|heading|blockquote|code|fences|list|html).*(?:\\n|$))*)\\n*|$)",table:"^ *\\|(.+)\\n {0,3}\\|?( *[-:]+[-| :]*)(?:\\n *((?:(?!\\n|hr|heading|blockquote|code|fences|list|html).*(?:\\n|$))*)\\n*|$)"}),B.gfm.nptable=P(B.gfm.nptable).replace("hr",B.hr).replace("heading"," {0,3}#{1,6} ").replace("blockquote"," {0,3}>").replace("code"," {4}[^\\n]").replace("fences"," {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list"," {0,3}(?:[*+-]|1[.)]) ").replace("html","</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|!--)").replace("tag",B._tag).getRegex(),B.gfm.table=P(B.gfm.table).replace("hr",B.hr).replace("heading"," {0,3}#{1,6} ").replace("blockquote"," {0,3}>").replace("code"," {4}[^\\n]").replace("fences"," {0,3}(?:`{3,}(?=[^`\\n]*\\n)|~{3,})[^\\n]*\\n").replace("list"," {0,3}(?:[*+-]|1[.)]) ").replace("html","</?(?:tag)(?: +|\\n|/?>)|<(?:script|pre|style|!--)").replace("tag",B._tag).getRegex(),B.pedantic=W({},B.normal,{html:P("^ *(?:comment *(?:\\n|\\s*$)|<(tag)[\\s\\S]+?</\\1> *(?:\\n{2,}|\\s*$)|<tag(?:\"[^\"]*\"|'[^']*'|\\s[^'\"/>\\s]*)*?/?> *(?:\\n{2,}|\\s*$))").replace("comment",B._comment).replace(/tag/g,"(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\\b)\\w+(?!:|[^\\w\\s@]*@)\\b").getRegex(),def:/^ *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +(["(][^\n]+[")]))? *(?:\n+|$)/,heading:/^ *(#{1,6}) *([^\n]+?) *(?:#+ *)?(?:\n+|$)/,fences:_,paragraph:P(B.normal._paragraph).replace("hr",B.hr).replace("heading"," *#{1,6} *[^\n]").replace("lheading",B.lheading).replace("blockquote"," {0,3}>").replace("|fences","").replace("|list","").replace("|html","").getRegex()});var j={escape:/^\\([!"#$%&'()*+,\-./:;<=>?@\[\]\\^_`{|}~])/,autolink:/^<(scheme:[^\s\x00-\x1f<>]*|email)>/,url:_,tag:"^comment|^</[a-zA-Z][\\w:-]*\\s*>|^<[a-zA-Z][\\w-]*(?:attribute)*?\\s*/?>|^<\\?[\\s\\S]*?\\?>|^<![a-zA-Z]+\\s[\\s\\S]*?>|^<!\\[CDATA\\[[\\s\\S]*?\\]\\]>",link:/^!?\[(label)\]\(\s*(href)(?:\s+(title))?\s*\)/,reflink:/^!?\[(label)\]\[(?!\s*\])((?:\\[\[\]]?|[^\[\]\\])+)\]/,nolink:/^!?\[(?!\s*\])((?:\[[^\[\]]*\]|\\[\[\]]|[^\[\]])*)\](?:\[\])?/,reflinkSearch:"reflink|nolink(?!\\()",strong:{start:/^(?:(\*\*(?=[*punctuation]))|\*\*)(?![\s])|__/,middle:/^\*\*(?:(?:(?!overlapSkip)(?:[^*]|\\\*)|overlapSkip)|\*(?:(?!overlapSkip)(?:[^*]|\\\*)|overlapSkip)*?\*)+?\*\*$|^__(?![\s])((?:(?:(?!overlapSkip)(?:[^_]|\\_)|overlapSkip)|_(?:(?!overlapSkip)(?:[^_]|\\_)|overlapSkip)*?_)+?)__$/,endAst:/[^punctuation\s]\*\*(?!\*)|[punctuation]\*\*(?!\*)(?:(?=[punctuation_\s]|$))/,endUnd:/[^\s]__(?!_)(?:(?=[punctuation*\s])|$)/},em:{start:/^(?:(\*(?=[punctuation]))|\*)(?![*\s])|_/,middle:/^\*(?:(?:(?!overlapSkip)(?:[^*]|\\\*)|overlapSkip)|\*(?:(?!overlapSkip)(?:[^*]|\\\*)|overlapSkip)*?\*)+?\*$|^_(?![_\s])(?:(?:(?!overlapSkip)(?:[^_]|\\_)|overlapSkip)|_(?:(?!overlapSkip)(?:[^_]|\\_)|overlapSkip)*?_)+?_$/,endAst:/[^punctuation\s]\*(?!\*)|[punctuation]\*(?!\*)(?:(?=[punctuation_\s]|$))/,endUnd:/[^\s]_(?!_)(?:(?=[punctuation*\s])|$)/},code:/^(`+)([^`]|[^`][\s\S]*?[^`])\1(?!`)/,br:/^( {2,}|\\)\n(?!\s*$)/,del:_,text:/^(`+|[^`])(?:(?= {2,}\n)|[\s\S]*?(?:(?=[\\<!\[`*]|\b_|$)|[^ ](?= {2,}\n)))/,punctuation:/^([\s*punctuation])/,_punctuation:"!\"#$%&'()+\\-.,/:;<=>?@\\[\\]`^{|}~"};j.punctuation=P(j.punctuation).replace(/punctuation/g,j._punctuation).getRegex(),j._blockSkip="\\[[^\\]]*?\\]\\([^\\)]*?\\)|`[^`]*?`|<[^>]*?>",j._overlapSkip="__[^_]*?__|\\*\\*\\[^\\*\\]*?\\*\\*",j._comment=P(B._comment).replace("(?:--\x3e|$)","--\x3e").getRegex(),j.em.start=P(j.em.start).replace(/punctuation/g,j._punctuation).getRegex(),j.em.middle=P(j.em.middle).replace(/punctuation/g,j._punctuation).replace(/overlapSkip/g,j._overlapSkip).getRegex(),j.em.endAst=P(j.em.endAst,"g").replace(/punctuation/g,j._punctuation).getRegex(),j.em.endUnd=P(j.em.endUnd,"g").replace(/punctuation/g,j._punctuation).getRegex(),j.strong.start=P(j.strong.start).replace(/punctuation/g,j._punctuation).getRegex(),j.strong.middle=P(j.strong.middle).replace(/punctuation/g,j._punctuation).replace(/overlapSkip/g,j._overlapSkip).getRegex(),j.strong.endAst=P(j.strong.endAst,"g").replace(/punctuation/g,j._punctuation).getRegex(),j.strong.endUnd=P(j.strong.endUnd,"g").replace(/punctuation/g,j._punctuation).getRegex(),j.blockSkip=P(j._blockSkip,"g").getRegex(),j.overlapSkip=P(j._overlapSkip,"g").getRegex(),j._escapes=/\\([!"#$%&'()*+,\-./:;<=>?@\[\]\\^_`{|}~])/g,j._scheme=/[a-zA-Z][a-zA-Z0-9+.-]{1,31}/,j._email=/[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+(@)[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+(?![-_])/,j.autolink=P(j.autolink).replace("scheme",j._scheme).replace("email",j._email).getRegex(),j._attribute=/\s+[a-zA-Z:_][\w.:-]*(?:\s*=\s*"[^"]*"|\s*=\s*'[^']*'|\s*=\s*[^\s"'=<>`]+)?/,j.tag=P(j.tag).replace("comment",j._comment).replace("attribute",j._attribute).getRegex(),j._label=/(?:\[(?:\\.|[^\[\]\\])*\]|\\.|`[^`]*`|[^\[\]\\`])*?/,j._href=/<(?:\\[<>]?|[^\s<>\\])*>|[^\s\x00-\x1f]*/,j._title=/"(?:\\"?|[^"\\])*"|'(?:\\'?|[^'\\])*'|\((?:\\\)?|[^)\\])*\)/,j.link=P(j.link).replace("label",j._label).replace("href",j._href).replace("title",j._title).getRegex(),j.reflink=P(j.reflink).replace("label",j._label).getRegex(),j.reflinkSearch=P(j.reflinkSearch,"g").replace("reflink",j.reflink).replace("nolink",j.nolink).getRegex(),j.normal=W({},j),j.pedantic=W({},j.normal,{strong:{start:/^__|\*\*/,middle:/^__(?=\S)([\s\S]*?\S)__(?!_)|^\*\*(?=\S)([\s\S]*?\S)\*\*(?!\*)/,endAst:/\*\*(?!\*)/g,endUnd:/__(?!_)/g},em:{start:/^_|\*/,middle:/^()\*(?=\S)([\s\S]*?\S)\*(?!\*)|^_(?=\S)([\s\S]*?\S)_(?!_)/,endAst:/\*(?!\*)/g,endUnd:/_(?!_)/g},link:P(/^!?\[(label)\]\((.*?)\)/).replace("label",j._label).getRegex(),reflink:P(/^!?\[(label)\]\s*\[([^\]]*)\]/).replace("label",j._label).getRegex()}),j.gfm=W({},j.normal,{escape:P(j.escape).replace("])","~|])").getRegex(),_extended_email:/[A-Za-z0-9._+-]+(@)[a-zA-Z0-9-_]+(?:\.[a-zA-Z0-9-_]*[a-zA-Z0-9])+(?![-_])/,url:/^((?:ftp|https?):\/\/|www\.)(?:[a-zA-Z0-9\-]+\.?)+[^\s<]*|^email/,_backpedal:/(?:[^?!.,:;*_~()&]+|\([^)]*\)|&(?![a-zA-Z0-9]+;$)|[?!.,:;*_~)]+(?!$))+/,del:/^~+(?=\S)([\s\S]*?\S)~+/,text:/^(`+|[^`])(?:(?= {2,}\n)|[\s\S]*?(?:(?=[\\<!\[`*~]|\b_|https?:\/\/|ftp:\/\/|www\.|$)|[^ ](?= {2,}\n)|[^a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-](?=[a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-]+@))|(?=[a-zA-Z0-9.!#$%&'*+\/=?_`{\|}~-]+@))/}),j.gfm.url=P(j.gfm.url,"i").replace("email",j.gfm._extended_email).getRegex(),j.breaks=W({},j.gfm,{br:P(j.br).replace("{2,}","*").getRegex(),text:P(j.gfm.text).replace("\\b_","\\b_| {2,}\\n").replace(/\{2,\}/g,"*").getRegex()});var q={block:B,inline:j},U=r.defaults,$=q.block,G=q.inline;function V(e){return e.replace(/---/g,"—").replace(/--/g,"–").replace(/(^|[-\u2014/(\[{"\s])'/g,"$1‘").replace(/'/g,"’").replace(/(^|[-\u2014/(\[{\u2018\s])"/g,"$1“").replace(/"/g,"”").replace(/\.{3}/g,"…")}function K(e){var t,n,r="",i=e.length;for(t=0;t<i;t++)n=e.charCodeAt(t),Math.random()>.5&&(n="x"+n.toString(16)),r+="&#"+n+";";return r}var X=function(){function t(e){this.tokens=[],this.tokens.links=Object.create(null),this.options=e||U,this.options.tokenizer=this.options.tokenizer||new R,this.tokenizer=this.options.tokenizer,this.tokenizer.options=this.options;var t={block:$.normal,inline:G.normal};this.options.pedantic?(t.block=$.pedantic,t.inline=G.pedantic):this.options.gfm&&(t.block=$.gfm,this.options.breaks?t.inline=G.breaks:t.inline=G.gfm),this.tokenizer.rules=t}t.lex=function(e,n){return new t(n).lex(e)},t.lexInline=function(e,n){return new t(n).inlineTokens(e)};var n,r,i,o=t.prototype;return o.lex=function(e){return e=e.replace(/\r\n|\r/g,"\n").replace(/\t/g,"    "),this.blockTokens(e,this.tokens,!0),this.inline(this.tokens),this.tokens},o.blockTokens=function(e,t,n){var r,i,o,a;for(void 0===t&&(t=[]),void 0===n&&(n=!0),e=e.replace(/^ +$/gm,"");e;)if(r=this.tokenizer.space(e))e=e.substring(r.raw.length),r.type&&t.push(r);else if(r=this.tokenizer.code(e,t))e=e.substring(r.raw.length),r.type?t.push(r):((a=t[t.length-1]).raw+="\n"+r.raw,a.text+="\n"+r.text);else if(r=this.tokenizer.fences(e))e=e.substring(r.raw.length),t.push(r);else if(r=this.tokenizer.heading(e))e=e.substring(r.raw.length),t.push(r);else if(r=this.tokenizer.nptable(e))e=e.substring(r.raw.length),t.push(r);else if(r=this.tokenizer.hr(e))e=e.substring(r.raw.length),t.push(r);else if(r=this.tokenizer.blockquote(e))e=e.substring(r.raw.length),r.tokens=this.blockTokens(r.text,[],n),t.push(r);else if(r=this.tokenizer.list(e)){for(e=e.substring(r.raw.length),o=r.items.length,i=0;i<o;i++)r.items[i].tokens=this.blockTokens(r.items[i].text,[],!1);t.push(r)}else if(r=this.tokenizer.html(e))e=e.substring(r.raw.length),t.push(r);else if(n&&(r=this.tokenizer.def(e)))e=e.substring(r.raw.length),this.tokens.links[r.tag]||(this.tokens.links[r.tag]={href:r.href,title:r.title});else if(r=this.tokenizer.table(e))e=e.substring(r.raw.length),t.push(r);else if(r=this.tokenizer.lheading(e))e=e.substring(r.raw.length),t.push(r);else if(n&&(r=this.tokenizer.paragraph(e)))e=e.substring(r.raw.length),t.push(r);else if(r=this.tokenizer.text(e,t))e=e.substring(r.raw.length),r.type?t.push(r):((a=t[t.length-1]).raw+="\n"+r.raw,a.text+="\n"+r.text);else if(e){var l="Infinite loop on byte: "+e.charCodeAt(0);if(this.options.silent){console.error(l);break}throw new Error(l)}return t},o.inline=function(e){var t,n,r,i,o,a,l=e.length;for(t=0;t<l;t++)switch((a=e[t]).type){case"paragraph":case"text":case"heading":a.tokens=[],this.inlineTokens(a.text,a.tokens);break;case"table":for(a.tokens={header:[],cells:[]},i=a.header.length,n=0;n<i;n++)a.tokens.header[n]=[],this.inlineTokens(a.header[n],a.tokens.header[n]);for(i=a.cells.length,n=0;n<i;n++)for(o=a.cells[n],a.tokens.cells[n]=[],r=0;r<o.length;r++)a.tokens.cells[n][r]=[],this.inlineTokens(o[r],a.tokens.cells[n][r]);break;case"blockquote":this.inline(a.tokens);break;case"list":for(i=a.items.length,n=0;n<i;n++)this.inline(a.items[n].tokens)}return e},o.inlineTokens=function(e,t,n,r,i){var o;void 0===t&&(t=[]),void 0===n&&(n=!1),void 0===r&&(r=!1),void 0===i&&(i="");var a,l=e;if(this.tokens.links){var s=Object.keys(this.tokens.links);if(s.length>0)for(;null!=(a=this.tokenizer.rules.inline.reflinkSearch.exec(l));)s.includes(a[0].slice(a[0].lastIndexOf("[")+1,-1))&&(l=l.slice(0,a.index)+"["+"a".repeat(a[0].length-2)+"]"+l.slice(this.tokenizer.rules.inline.reflinkSearch.lastIndex))}for(;null!=(a=this.tokenizer.rules.inline.blockSkip.exec(l));)l=l.slice(0,a.index)+"["+"a".repeat(a[0].length-2)+"]"+l.slice(this.tokenizer.rules.inline.blockSkip.lastIndex);for(;e;)if(o=this.tokenizer.escape(e))e=e.substring(o.raw.length),t.push(o);else if(o=this.tokenizer.tag(e,n,r))e=e.substring(o.raw.length),n=o.inLink,r=o.inRawBlock,t.push(o);else if(o=this.tokenizer.link(e))e=e.substring(o.raw.length),"link"===o.type&&(o.tokens=this.inlineTokens(o.text,[],!0,r)),t.push(o);else if(o=this.tokenizer.reflink(e,this.tokens.links))e=e.substring(o.raw.length),"link"===o.type&&(o.tokens=this.inlineTokens(o.text,[],!0,r)),t.push(o);else if(o=this.tokenizer.strong(e,l,i))e=e.substring(o.raw.length),o.tokens=this.inlineTokens(o.text,[],n,r),t.push(o);else if(o=this.tokenizer.em(e,l,i))e=e.substring(o.raw.length),o.tokens=this.inlineTokens(o.text,[],n,r),t.push(o);else if(o=this.tokenizer.codespan(e))e=e.substring(o.raw.length),t.push(o);else if(o=this.tokenizer.br(e))e=e.substring(o.raw.length),t.push(o);else if(o=this.tokenizer.del(e))e=e.substring(o.raw.length),o.tokens=this.inlineTokens(o.text,[],n,r),t.push(o);else if(o=this.tokenizer.autolink(e,K))e=e.substring(o.raw.length),t.push(o);else if(n||!(o=this.tokenizer.url(e,K))){if(o=this.tokenizer.inlineText(e,r,V))e=e.substring(o.raw.length),i=o.raw.slice(-1),t.push(o);else if(e){var c="Infinite loop on byte: "+e.charCodeAt(0);if(this.options.silent){console.error(c);break}throw new Error(c)}}else e=e.substring(o.raw.length),t.push(o);return t},n=t,i=[{key:"rules",get:function(){return{block:$,inline:G}}}],(r=null)&&e(n.prototype,r),i&&e(n,i),t}(),Z=r.defaults,Y=S,Q=w,J=function(){function e(e){this.options=e||Z}var t=e.prototype;return t.code=function(e,t,n){var r=(t||"").match(/\S*/)[0];if(this.options.highlight){var i=this.options.highlight(e,r);null!=i&&i!==e&&(n=!0,e=i)}return r?'<pre><code class="'+this.options.langPrefix+Q(r,!0)+'">'+(n?e:Q(e,!0))+"</code></pre>\n":"<pre><code>"+(n?e:Q(e,!0))+"</code></pre>\n"},t.blockquote=function(e){return"<blockquote>\n"+e+"</blockquote>\n"},t.html=function(e){return e},t.heading=function(e,t,n,r){return this.options.headerIds?"<h"+t+' id="'+this.options.headerPrefix+r.slug(n)+'">'+e+"</h"+t+">\n":"<h"+t+">"+e+"</h"+t+">\n"},t.hr=function(){return this.options.xhtml?"<hr/>\n":"<hr>\n"},t.list=function(e,t,n){var r=t?"ol":"ul";return"<"+r+(t&&1!==n?' start="'+n+'"':"")+">\n"+e+"</"+r+">\n"},t.listitem=function(e){return"<li>"+e+"</li>\n"},t.checkbox=function(e){return"<input "+(e?'checked="" ':"")+'disabled="" type="checkbox"'+(this.options.xhtml?" /":"")+"> "},t.paragraph=function(e){return"<p>"+e+"</p>\n"},t.table=function(e,t){return t&&(t="<tbody>"+t+"</tbody>"),"<table>\n<thead>\n"+e+"</thead>\n"+t+"</table>\n"},t.tablerow=function(e){return"<tr>\n"+e+"</tr>\n"},t.tablecell=function(e,t){var n=t.header?"th":"td";return(t.align?"<"+n+' align="'+t.align+'">':"<"+n+">")+e+"</"+n+">\n"},t.strong=function(e){return"<strong>"+e+"</strong>"},t.em=function(e){return"<em>"+e+"</em>"},t.codespan=function(e){return"<code>"+e+"</code>"},t.br=function(){return this.options.xhtml?"<br/>":"<br>"},t.del=function(e){return"<del>"+e+"</del>"},t.link=function(e,t,n){if(null===(e=Y(this.options.sanitize,this.options.baseUrl,e)))return n;var r='<a href="'+Q(e)+'"';return t&&(r+=' title="'+t+'"'),r+=">"+n+"</a>"},t.image=function(e,t,n){if(null===(e=Y(this.options.sanitize,this.options.baseUrl,e)))return n;var r='<img src="'+e+'" alt="'+n+'"';return t&&(r+=' title="'+t+'"'),r+=this.options.xhtml?"/>":">"},t.text=function(e){return e},e}(),ee=function(){function e(){}var t=e.prototype;return t.strong=function(e){return e},t.em=function(e){return e},t.codespan=function(e){return e},t.del=function(e){return e},t.html=function(e){return e},t.text=function(e){return e},t.link=function(e,t,n){return""+n},t.image=function(e,t,n){return""+n},t.br=function(){return""},e}(),te=function(){function e(){this.seen={}}var t=e.prototype;return t.serialize=function(e){return e.toLowerCase().trim().replace(/<[!\/a-z].*?>/gi,"").replace(/[\u2000-\u206F\u2E00-\u2E7F\\'!"#$%&()*+,./:;<=>?@[\]^`{|}~]/g,"").replace(/\s/g,"-")},t.getNextSafeSlug=function(e,t){var n=e,r=0;if(this.seen.hasOwnProperty(n)){r=this.seen[e];do{n=e+"-"+ ++r}while(this.seen.hasOwnProperty(n))}return t||(this.seen[e]=r,this.seen[n]=0),n},t.slug=function(e,t){void 0===t&&(t={});var n=this.serialize(e);return this.getNextSafeSlug(n,t.dryrun)},e}(),ne=r.defaults,re=k,ie=function(){function e(e){this.options=e||ne,this.options.renderer=this.options.renderer||new J,this.renderer=this.options.renderer,this.renderer.options=this.options,this.textRenderer=new ee,this.slugger=new te}e.parse=function(t,n){return new e(n).parse(t)},e.parseInline=function(t,n){return new e(n).parseInline(t)};var t=e.prototype;return t.parse=function(e,t){void 0===t&&(t=!0);var n,r,i,o,a,l,s,c,u,d,h,f,p,m,g,v,x,y,b="",w=e.length;for(n=0;n<w;n++)switch((d=e[n]).type){case"space":continue;case"hr":b+=this.renderer.hr();continue;case"heading":b+=this.renderer.heading(this.parseInline(d.tokens),d.depth,re(this.parseInline(d.tokens,this.textRenderer)),this.slugger);continue;case"code":b+=this.renderer.code(d.text,d.lang,d.escaped);continue;case"table":for(c="",s="",o=d.header.length,r=0;r<o;r++)s+=this.renderer.tablecell(this.parseInline(d.tokens.header[r]),{header:!0,align:d.align[r]});for(c+=this.renderer.tablerow(s),u="",o=d.cells.length,r=0;r<o;r++){for(s="",a=(l=d.tokens.cells[r]).length,i=0;i<a;i++)s+=this.renderer.tablecell(this.parseInline(l[i]),{header:!1,align:d.align[i]});u+=this.renderer.tablerow(s)}b+=this.renderer.table(c,u);continue;case"blockquote":u=this.parse(d.tokens),b+=this.renderer.blockquote(u);continue;case"list":for(h=d.ordered,f=d.start,p=d.loose,o=d.items.length,u="",r=0;r<o;r++)v=(g=d.items[r]).checked,x=g.task,m="",g.task&&(y=this.renderer.checkbox(v),p?g.tokens.length>0&&"text"===g.tokens[0].type?(g.tokens[0].text=y+" "+g.tokens[0].text,g.tokens[0].tokens&&g.tokens[0].tokens.length>0&&"text"===g.tokens[0].tokens[0].type&&(g.tokens[0].tokens[0].text=y+" "+g.tokens[0].tokens[0].text)):g.tokens.unshift({type:"text",text:y}):m+=y),m+=this.parse(g.tokens,p),u+=this.renderer.listitem(m,x,v);b+=this.renderer.list(u,h,f);continue;case"html":b+=this.renderer.html(d.text);continue;case"paragraph":b+=this.renderer.paragraph(this.parseInline(d.tokens));continue;case"text":for(u=d.tokens?this.parseInline(d.tokens):d.text;n+1<w&&"text"===e[n+1].type;)u+="\n"+((d=e[++n]).tokens?this.parseInline(d.tokens):d.text);b+=t?this.renderer.paragraph(u):u;continue;default:var k='Token with "'+d.type+'" type was not found.';if(this.options.silent)return void console.error(k);throw new Error(k)}return b},t.parseInline=function(e,t){t=t||this.renderer;var n,r,i="",o=e.length;for(n=0;n<o;n++)switch((r=e[n]).type){case"escape":i+=t.text(r.text);break;case"html":i+=t.html(r.text);break;case"link":i+=t.link(r.href,r.title,this.parseInline(r.tokens,t));break;case"image":i+=t.image(r.href,r.title,r.text);break;case"strong":i+=t.strong(this.parseInline(r.tokens,t));break;case"em":i+=t.em(this.parseInline(r.tokens,t));break;case"codespan":i+=t.codespan(r.text);break;case"br":i+=t.br();break;case"del":i+=t.del(this.parseInline(r.tokens,t));break;case"text":i+=t.text(r.text);break;default:var a='Token with "'+r.type+'" type was not found.';if(this.options.silent)return void console.error(a);throw new Error(a)}return i},e}(),oe=T,ae=D,le=w,se=r.getDefaults,ce=r.changeDefaults,ue=r.defaults;function de(e,t,n){if(null==e)throw new Error("marked(): input parameter is undefined or null");if("string"!=typeof e)throw new Error("marked(): input parameter is of type "+Object.prototype.toString.call(e)+", string expected");if("function"==typeof t&&(n=t,t=null),t=oe({},de.defaults,t||{}),ae(t),n){var r,i=t.highlight;try{r=X.lex(e,t)}catch(e){return n(e)}var o=function(e){var o;if(!e)try{o=ie.parse(r,t)}catch(t){e=t}return t.highlight=i,e?n(e):n(null,o)};if(!i||i.length<3)return o();if(delete t.highlight,!r.length)return o();var a=0;return de.walkTokens(r,(function(e){"code"===e.type&&(a++,setTimeout((function(){i(e.text,e.lang,(function(t,n){if(t)return o(t);null!=n&&n!==e.text&&(e.text=n,e.escaped=!0),0===--a&&o()}))}),0))})),void(0===a&&o())}try{var l=X.lex(e,t);return t.walkTokens&&de.walkTokens(l,t.walkTokens),ie.parse(l,t)}catch(e){if(e.message+="\nPlease report this to https://github.com/markedjs/marked.",t.silent)return"<p>An error occurred:</p><pre>"+le(e.message+"",!0)+"</pre>";throw e}}return de.options=de.setOptions=function(e){return oe(de.defaults,e),ce(de.defaults),de},de.getDefaults=se,de.defaults=ue,de.use=function(e){var t=oe({},e);if(e.renderer&&function(){var n=de.defaults.renderer||new J,r=function(t){var r=n[t];n[t]=function(){for(var i=arguments.length,o=new Array(i),a=0;a<i;a++)o[a]=arguments[a];var l=e.renderer[t].apply(n,o);return!1===l&&(l=r.apply(n,o)),l}};for(var i in e.renderer)r(i);t.renderer=n}(),e.tokenizer&&function(){var n=de.defaults.tokenizer||new R,r=function(t){var r=n[t];n[t]=function(){for(var i=arguments.length,o=new Array(i),a=0;a<i;a++)o[a]=arguments[a];var l=e.tokenizer[t].apply(n,o);return!1===l&&(l=r.apply(n,o)),l}};for(var i in e.tokenizer)r(i);t.tokenizer=n}(),e.walkTokens){var n=de.defaults.walkTokens;t.walkTokens=function(t){e.walkTokens(t),n&&n(t)}}de.setOptions(t)},de.walkTokens=function(e,t){for(var r,i=n(e);!(r=i()).done;){var o=r.value;switch(t(o),o.type){case"table":for(var a,l=n(o.tokens.header);!(a=l()).done;){var s=a.value;de.walkTokens(s,t)}for(var c,u=n(o.tokens.cells);!(c=u()).done;)for(var d,h=n(c.value);!(d=h()).done;){var f=d.value;de.walkTokens(f,t)}break;case"list":de.walkTokens(o.items,t);break;default:o.tokens&&de.walkTokens(o.tokens,t)}}},de.parseInline=function(e,t){if(null==e)throw new Error("marked.parseInline(): input parameter is undefined or null");if("string"!=typeof e)throw new Error("marked.parseInline(): input parameter is of type "+Object.prototype.toString.call(e)+", string expected");t=oe({},de.defaults,t||{}),ae(t);try{var n=X.lexInline(e,t);return t.walkTokens&&de.walkTokens(n,t.walkTokens),ie.parseInline(n,t)}catch(e){if(e.message+="\nPlease report this to https://github.com/markedjs/marked.",t.silent)return"<p>An error occurred:</p><pre>"+le(e.message+"",!0)+"</pre>";throw e}},de.Parser=ie,de.parser=ie.parse,de.Renderer=J,de.TextRenderer=ee,de.Lexer=X,de.lexer=X.lex,de.Tokenizer=R,de.Slugger=te,de.parse=de,de}))},{}],15:[function(e,t,n){(function(n){var r;!function(){"use strict";(r=function(e,t,r,i){i=i||{},this.dictionary=null,this.rules={},this.dictionaryTable={},this.compoundRules=[],this.compoundRuleCodes={},this.replacementTable=[],this.flags=i.flags||{},this.memoized={},this.loaded=!1;var o,a,l,s,c,u=this;function d(e,t){var n=u._readFile(e,null,i.asyncLoad);i.asyncLoad?n.then((function(e){t(e)})):t(n)}function h(e){t=e,r&&p()}function f(e){r=e,t&&p()}function p(){for(u.rules=u._parseAFF(t),u.compoundRuleCodes={},a=0,s=u.compoundRules.length;a<s;a++){var e=u.compoundRules[a];for(l=0,c=e.length;l<c;l++)u.compoundRuleCodes[e[l]]=[]}for(a in"ONLYINCOMPOUND"in u.flags&&(u.compoundRuleCodes[u.flags.ONLYINCOMPOUND]=[]),u.dictionaryTable=u._parseDIC(r),u.compoundRuleCodes)0===u.compoundRuleCodes[a].length&&delete u.compoundRuleCodes[a];for(a=0,s=u.compoundRules.length;a<s;a++){var n=u.compoundRules[a],o="";for(l=0,c=n.length;l<c;l++){var d=n[l];d in u.compoundRuleCodes?o+="("+u.compoundRuleCodes[d].join("|")+")":o+=d}u.compoundRules[a]=new RegExp(o,"i")}u.loaded=!0,i.asyncLoad&&i.loadedCallback&&i.loadedCallback(u)}return e&&(u.dictionary=e,t&&r?p():"undefined"!=typeof window&&"chrome"in window&&"extension"in window.chrome&&"getURL"in window.chrome.extension?(o=i.dictionaryPath?i.dictionaryPath:"typo/dictionaries",t||d(chrome.extension.getURL(o+"/"+e+"/"+e+".aff"),h),r||d(chrome.extension.getURL(o+"/"+e+"/"+e+".dic"),f)):(o=i.dictionaryPath?i.dictionaryPath:void 0!==n?n+"/dictionaries":"./dictionaries",t||d(o+"/"+e+"/"+e+".aff",h),r||d(o+"/"+e+"/"+e+".dic",f))),this}).prototype={load:function(e){for(var t in e)e.hasOwnProperty(t)&&(this[t]=e[t]);return this},_readFile:function(t,n,r){if(n=n||"utf8","undefined"!=typeof XMLHttpRequest){var i,o=new XMLHttpRequest;return o.open("GET",t,r),r&&(i=new Promise((function(e,t){o.onload=function(){200===o.status?e(o.responseText):t(o.statusText)},o.onerror=function(){t(o.statusText)}}))),o.overrideMimeType&&o.overrideMimeType("text/plain; charset="+n),o.send(null),r?i:o.responseText}if(void 0!==e){var a=e("fs");try{if(a.existsSync(t))return a.readFileSync(t,n);console.log("Path "+t+" does not exist.")}catch(e){return console.log(e),""}}},_parseAFF:function(e){var t,n,r,i,o,a,l,s={},c=(e=this._removeAffixComments(e)).split(/\r?\n/);for(i=0,a=c.length;i<a;i++){var u=(t=c[i]).split(/\s+/),d=u[0];if("PFX"==d||"SFX"==d){var h=u[1],f=u[2],p=[];for(o=i+1,l=i+1+(n=parseInt(u[3],10));o<l;o++){var m=(r=c[o].split(/\s+/))[2],g=r[3].split("/"),v=g[0];"0"===v&&(v="");var x=this.parseRuleCodes(g[1]),y=r[4],b={};b.add=v,x.length>0&&(b.continuationClasses=x),"."!==y&&(b.match="SFX"===d?new RegExp(y+"$"):new RegExp("^"+y)),"0"!=m&&(b.remove="SFX"===d?new RegExp(m+"$"):m),p.push(b)}s[h]={type:d,combineable:"Y"==f,entries:p},i+=n}else if("COMPOUNDRULE"===d){for(o=i+1,l=i+1+(n=parseInt(u[1],10));o<l;o++)r=(t=c[o]).split(/\s+/),this.compoundRules.push(r[1]);i+=n}else"REP"===d?3===(r=t.split(/\s+/)).length&&this.replacementTable.push([r[1],r[2]]):this.flags[d]=u[1]}return s},_removeAffixComments:function(e){return e=(e=(e=(e=e.replace(/^\s*#.*$/gm,"")).replace(/^\s\s*/m,"").replace(/\s\s*$/m,"")).replace(/\n{2,}/g,"\n")).replace(/^\s\s*/,"").replace(/\s\s*$/,"")},_parseDIC:function(e){var t=(e=this._removeDicComments(e)).split(/\r?\n/),n={};function r(e,t){n.hasOwnProperty(e)||(n[e]=null),t.length>0&&(null===n[e]&&(n[e]=[]),n[e].push(t))}for(var i=1,o=t.length;i<o;i++){var a=t[i];if(a){var l=a.split("/",2),s=l[0];if(l.length>1){var c=this.parseRuleCodes(l[1]);"NEEDAFFIX"in this.flags&&-1!=c.indexOf(this.flags.NEEDAFFIX)||r(s,c);for(var u=0,d=c.length;u<d;u++){var h=c[u],f=this.rules[h];if(f)for(var p=this._applyRule(s,f),m=0,g=p.length;m<g;m++){var v=p[m];if(r(v,[]),f.combineable)for(var x=u+1;x<d;x++){var y=c[x],b=this.rules[y];if(b&&b.combineable&&f.type!=b.type)for(var w=this._applyRule(v,b),k=0,C=w.length;k<C;k++){r(w[k],[])}}}h in this.compoundRuleCodes&&this.compoundRuleCodes[h].push(s)}}else r(s.trim(),[])}}return n},_removeDicComments:function(e){return e=e.replace(/^\t.*$/gm,"")},parseRuleCodes:function(e){if(!e)return[];if(!("FLAG"in this.flags))return e.split("");if("long"===this.flags.FLAG){for(var t=[],n=0,r=e.length;n<r;n+=2)t.push(e.substr(n,2));return t}return"num"===this.flags.FLAG?e.split(","):void 0},_applyRule:function(e,t){for(var n=t.entries,r=[],i=0,o=n.length;i<o;i++){var a=n[i];if(!a.match||e.match(a.match)){var l=e;if(a.remove&&(l=l.replace(a.remove,"")),"SFX"===t.type?l+=a.add:l=a.add+l,r.push(l),"continuationClasses"in a)for(var s=0,c=a.continuationClasses.length;s<c;s++){var u=this.rules[a.continuationClasses[s]];u&&(r=r.concat(this._applyRule(l,u)))}}}return r},check:function(e){if(!this.loaded)throw"Dictionary not loaded.";var t=e.replace(/^\s\s*/,"").replace(/\s\s*$/,"");if(this.checkExact(t))return!0;if(t.toUpperCase()===t){var n=t[0]+t.substring(1).toLowerCase();if(this.hasFlag(n,"KEEPCASE"))return!1;if(this.checkExact(n))return!0}var r=t.toLowerCase();if(r!==t){if(this.hasFlag(r,"KEEPCASE"))return!1;if(this.checkExact(r))return!0}return!1},checkExact:function(e){if(!this.loaded)throw"Dictionary not loaded.";var t,n,r=this.dictionaryTable[e];if(void 0===r){if("COMPOUNDMIN"in this.flags&&e.length>=this.flags.COMPOUNDMIN)for(t=0,n=this.compoundRules.length;t<n;t++)if(e.match(this.compoundRules[t]))return!0}else{if(null===r)return!0;if("object"==typeof r)for(t=0,n=r.length;t<n;t++)if(!this.hasFlag(e,"ONLYINCOMPOUND",r[t]))return!0}return!1},hasFlag:function(e,t,n){if(!this.loaded)throw"Dictionary not loaded.";return!(!(t in this.flags)||(void 0===n&&(n=Array.prototype.concat.apply([],this.dictionaryTable[e])),!n||-1===n.indexOf(this.flags[t])))},alphabet:"",suggest:function(e,t){if(!this.loaded)throw"Dictionary not loaded.";if(t=t||5,this.memoized.hasOwnProperty(e)){var n=this.memoized[e].limit;if(t<=n||this.memoized[e].suggestions.length<n)return this.memoized[e].suggestions.slice(0,t)}if(this.check(e))return[];for(var r=0,i=this.replacementTable.length;r<i;r++){var o=this.replacementTable[r];if(-1!==e.indexOf(o[0])){var a=e.replace(o[0],o[1]);if(this.check(a))return[a]}}var l=this;function s(e,t){var n,r,i,o,a,s={};if("string"==typeof e){var c=e;(e={})[c]=!0}for(var c in e)for(n=0,i=c.length+1;n<i;n++){var u=[c.substring(0,n),c.substring(n)];if(u[1]&&(a=u[0]+u[1].substring(1),t&&!l.check(a)||(a in s?s[a]+=1:s[a]=1)),u[1].length>1&&u[1][1]!==u[1][0]&&(a=u[0]+u[1][1]+u[1][0]+u[1].substring(2),t&&!l.check(a)||(a in s?s[a]+=1:s[a]=1)),u[1])for(r=0,o=l.alphabet.length;r<o;r++)l.alphabet[r]!=u[1].substring(0,1)&&(a=u[0]+l.alphabet[r]+u[1].substring(1),t&&!l.check(a)||(a in s?s[a]+=1:s[a]=1));if(u[1])for(r=0,o=l.alphabet.length;r<o;r++)a=u[0]+l.alphabet[r]+u[1],t&&!l.check(a)||(a in s?s[a]+=1:s[a]=1)}return s}return l.alphabet="abcdefghijklmnopqrstuvwxyz",this.memoized[e]={suggestions:function(e){var n,r=s(e),i=s(r,!0);for(var o in r)l.check(o)&&(o in i?i[o]+=r[o]:i[o]=r[o]);var a=[];for(n in i)i.hasOwnProperty(n)&&a.push([n,i[n]]);a.sort((function(e,t){var n=e[1],r=t[1];return n<r?-1:n>r?1:t[0].localeCompare(e[0])})).reverse();var c=[],u="lowercase";e.toUpperCase()===e?u="uppercase":e.substr(0,1).toUpperCase()+e.substr(1).toLowerCase()===e&&(u="capitalized");var d=t;for(n=0;n<Math.min(d,a.length);n++)"uppercase"===u?a[n][0]=a[n][0].toUpperCase():"capitalized"===u&&(a[n][0]=a[n][0].substr(0,1).toUpperCase()+a[n][0].substr(1)),l.hasFlag(a[n][0],"NOSUGGEST")||-1!=c.indexOf(a[n][0])?d++:c.push(a[n][0]);return c}(e),limit:t},this.memoized[e].suggestions}}}(),void 0!==t&&(t.exports=r)}).call(this,"/node_modules/typo-js")},{fs:1}],16:[function(e,t,n){var r=e("codemirror");r.commands.tabAndIndentMarkdownList=function(e){var t=e.listSelections()[0].head;if(!1!==e.getStateAfter(t.line).list)e.execCommand("indentMore");else if(e.options.indentWithTabs)e.execCommand("insertTab");else{var n=Array(e.options.tabSize+1).join(" ");e.replaceSelection(n)}},r.commands.shiftTabAndUnindentMarkdownList=function(e){var t=e.listSelections()[0].head;if(!1!==e.getStateAfter(t.line).list)e.execCommand("indentLess");else if(e.options.indentWithTabs)e.execCommand("insertTab");else{var n=Array(e.options.tabSize+1).join(" ");e.replaceSelection(n)}}},{codemirror:9}],17:[function(e,t,n){"use strict";var r=e("codemirror");e("codemirror/addon/edit/continuelist.js"),e("./codemirror/tablist"),e("codemirror/addon/display/fullscreen.js"),e("codemirror/mode/markdown/markdown.js"),e("codemirror/addon/mode/overlay.js"),e("codemirror/addon/display/placeholder.js"),e("codemirror/addon/selection/mark-selection.js"),e("codemirror/addon/search/searchcursor.js"),e("codemirror/mode/gfm/gfm.js"),e("codemirror/mode/xml/xml.js");var i=e("codemirror-spell-checker"),o=e("marked/lib/marked"),a=/Mac/.test(navigator.platform),l=new RegExp(/(<a.*?https?:\/\/.*?[^a]>)+?/g),s={toggleBold:x,toggleItalic:y,drawLink:F,toggleHeadingSmaller:C,toggleHeadingBigger:S,drawImage:E,toggleBlockquote:k,toggleOrderedList:N,toggleUnorderedList:A,toggleCodeBlock:w,togglePreview:W,toggleStrikethrough:b,toggleHeading1:L,toggleHeading2:T,toggleHeading3:M,cleanBlock:D,drawTable:z,drawHorizontalRule:H,undo:R,redo:_,toggleSideBySide:P,toggleFullScreen:v},c={toggleBold:"Cmd-B",toggleItalic:"Cmd-I",drawLink:"Cmd-K",toggleHeadingSmaller:"Cmd-H",toggleHeadingBigger:"Shift-Cmd-H",cleanBlock:"Cmd-E",drawImage:"Cmd-Alt-I",toggleBlockquote:"Cmd-'",toggleOrderedList:"Cmd-Alt-L",toggleUnorderedList:"Cmd-L",toggleCodeBlock:"Cmd-Alt-C",togglePreview:"Cmd-P",toggleSideBySide:"F9",toggleFullScreen:"F11"},u=function(){var e,t=!1;return e=navigator.userAgent||navigator.vendor||window.opera,(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(e)||/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw-(n|u)|c55\/|capi|ccwa|cdm-|cell|chtm|cldc|cmd-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc-s|devi|dica|dmob|do(c|p)o|ds(12|-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(-|_)|g1 u|g560|gene|gf-5|g-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd-(m|p|t)|hei-|hi(pt|ta)|hp( i|ip)|hs-c|ht(c(-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i-(20|go|ma)|i230|iac( |-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|-[a-w])|libw|lynx|m1-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|-([1-8]|c))|phil|pire|pl(ay|uc)|pn-2|po(ck|rt|se)|prox|psio|pt-g|qa-a|qc(07|12|21|32|60|-[2-7]|i-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h-|oo|p-)|sdk\/|se(c(-|0|1)|47|mc|nd|ri)|sgh-|shar|sie(-|m)|sk-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h-|v-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl-|tdg-|tel(i|m)|tim-|t-mo|to(pl|sh)|ts(70|m-|m3|m5)|tx-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas-|your|zeto|zte-/i.test(e.substr(0,4)))&&(t=!0),t};function d(e){return e=a?e.replace("Ctrl","Cmd"):e.replace("Cmd","Ctrl")}function h(e,t,n,r){var i=f(e,!1,t,n,"button",r);i.className+=" easymde-dropdown";var o=document.createElement("div");o.className="easymde-dropdown-content";for(var a=0;a<e.children.length;a++){var l,s=e.children[a];l=f("string"==typeof s&&s in X?X[s]:s,!0,t,n,"button",r),o.appendChild(l)}return i.appendChild(o),i}function f(e,t,n,r,i,o){e=e||{};var l=document.createElement(i);l.className=e.name,l.setAttribute("type",i),n=null==n||n,e.name&&e.name in r&&(s[e.name]=e.action),e.title&&n&&(l.title=function(e,t,n){var r,i=e;t&&n[r=function(e){for(var t in s)if(s[t]===e)return t;return null}(t)]&&(i+=" ("+d(n[r])+")");return i}(e.title,e.action,r),a&&(l.title=l.title.replace("Ctrl","⌘"),l.title=l.title.replace("Alt","⌥"))),e.noDisable&&l.classList.add("no-disable"),e.noMobile&&l.classList.add("no-mobile");var c=[];void 0!==e.className&&(c=e.className.split(" "));for(var u=[],h=0;h<c.length;h++){var f=c[h];f.match(/^fa([srlb]|(-[\w-]*)|$)/)?u.push(f):l.classList.add(f)}l.tabIndex=-1;for(var p=document.createElement("i"),m=0;m<u.length;m++){var g=u[m];p.classList.add(g)}return l.appendChild(p),void 0!==e.icon&&(l.innerHTML=e.icon),e.action&&t&&("function"==typeof e.action?l.onclick=function(t){t.preventDefault(),e.action(o)}:"string"==typeof e.action&&(l.onclick=function(t){t.preventDefault(),window.open(e.action,"_blank")})),l}function p(){var e=document.createElement("i");return e.className="separator",e.innerHTML="|",e}function m(e,t){t=t||e.getCursor("start");var n=e.getTokenAt(t);if(!n.type)return{};for(var r,i,o=n.type.split(" "),a={},l=0;l<o.length;l++)"strong"===(r=o[l])?a.bold=!0:"variable-2"===r?(i=e.getLine(t.line),/^\s*\d+\.\s/.test(i)?a["ordered-list"]=!0:a["unordered-list"]=!0):"atom"===r?a.quote=!0:"em"===r?a.italic=!0:"quote"===r?a.quote=!0:"strikethrough"===r?a.strikethrough=!0:"comment"===r?a.code=!0:"link"===r?a.link=!0:"tag"===r?a.image=!0:r.match(/^header(-[1-6])?$/)&&(a[r.replace("header","heading")]=!0);return a}var g="";function v(e){var t=e.codemirror;t.setOption("fullScreen",!t.getOption("fullScreen")),t.getOption("fullScreen")?(g=document.body.style.overflow,document.body.style.overflow="hidden"):document.body.style.overflow=g;var n=t.getWrapperElement().nextSibling;if(/editor-preview-active-side/.test(n.className)&&P(e),e.options.onToggleFullScreen&&e.options.onToggleFullScreen(t.getOption("fullScreen")||!1),void 0!==e.options.maxHeight&&(t.getOption("fullScreen")?(t.getScrollerElement().style.removeProperty("height"),n.style.removeProperty("height")):(t.getScrollerElement().style.height=e.options.maxHeight,e.setPreviewMaxHeight())),/fullscreen/.test(e.toolbar_div.className)?e.toolbar_div.className=e.toolbar_div.className.replace(/\s*fullscreen\b/,""):e.toolbar_div.className+=" fullscreen",e.toolbarElements&&e.toolbarElements.fullscreen){var r=e.toolbarElements.fullscreen;/active/.test(r.className)?r.className=r.className.replace(/\s*active\s*/g,""):r.className+=" active"}}function x(e){U(e,"bold",e.options.blockStyles.bold)}function y(e){U(e,"italic",e.options.blockStyles.italic)}function b(e){U(e,"strikethrough","~~")}function w(e){var t=e.options.blockStyles.code;function n(e){if("object"!=typeof e)throw"fencing_line() takes a 'line' object (not a line number, or line text).  Got: "+typeof e+": "+e;return e.styles&&e.styles[2]&&-1!==e.styles[2].indexOf("formatting-code-block")}function r(e){return e.state.base.base||e.state.base}function i(e,t,i,o,a){i=i||e.getLineHandle(t),o=o||e.getTokenAt({line:t,ch:1}),a=a||!!i.text&&e.getTokenAt({line:t,ch:i.text.length-1});var l=o.type?o.type.split(" "):[];return a&&r(a).indentedCode?"indented":-1!==l.indexOf("comment")&&(r(o).fencedChars||r(a).fencedChars||n(i)?"fenced":"single")}var o,a,l,s=e.codemirror,c=s.getCursor("start"),u=s.getCursor("end"),d=s.getTokenAt({line:c.line,ch:c.ch||1}),h=s.getLineHandle(c.line),f=i(s,c.line,h,d);if("single"===f){var p=h.text.slice(0,c.ch).replace("`",""),m=h.text.slice(c.ch).replace("`","");s.replaceRange(p+m,{line:c.line,ch:0},{line:c.line,ch:99999999999999}),c.ch--,c!==u&&u.ch--,s.setSelection(c,u),s.focus()}else if("fenced"===f)if(c.line!==u.line||c.ch!==u.ch){for(o=c.line;o>=0&&!n(h=s.getLineHandle(o));o--);var g,v,x,y,b=r(s.getTokenAt({line:o,ch:1})).fencedChars;n(s.getLineHandle(c.line))?(g="",v=c.line):n(s.getLineHandle(c.line-1))?(g="",v=c.line-1):(g=b+"\n",v=c.line),n(s.getLineHandle(u.line))?(x="",y=u.line,0===u.ch&&(y+=1)):0!==u.ch&&n(s.getLineHandle(u.line+1))?(x="",y=u.line+1):(x=b+"\n",y=u.line+1),0===u.ch&&(y-=1),s.operation((function(){s.replaceRange(x,{line:y,ch:0},{line:y+(x?0:1),ch:0}),s.replaceRange(g,{line:v,ch:0},{line:v+(g?0:1),ch:0})})),s.setSelection({line:v+(g?1:0),ch:0},{line:y+(g?1:-1),ch:0}),s.focus()}else{var w=c.line;if(n(s.getLineHandle(c.line))&&("fenced"===i(s,c.line+1)?(o=c.line,w=c.line+1):(a=c.line,w=c.line-1)),void 0===o)for(o=w;o>=0&&!n(h=s.getLineHandle(o));o--);if(void 0===a)for(l=s.lineCount(),a=w;a<l&&!n(h=s.getLineHandle(a));a++);s.operation((function(){s.replaceRange("",{line:o,ch:0},{line:o+1,ch:0}),s.replaceRange("",{line:a-1,ch:0},{line:a,ch:0})})),s.focus()}else if("indented"===f){if(c.line!==u.line||c.ch!==u.ch)o=c.line,a=u.line,0===u.ch&&a--;else{for(o=c.line;o>=0;o--)if(!(h=s.getLineHandle(o)).text.match(/^\s*$/)&&"indented"!==i(s,o,h)){o+=1;break}for(l=s.lineCount(),a=c.line;a<l;a++)if(!(h=s.getLineHandle(a)).text.match(/^\s*$/)&&"indented"!==i(s,a,h)){a-=1;break}}var k=s.getLineHandle(a+1),C=k&&s.getTokenAt({line:a+1,ch:k.text.length-1});C&&r(C).indentedCode&&s.replaceRange("\n",{line:a+1,ch:0});for(var S=o;S<=a;S++)s.indentLine(S,"subtract");s.focus()}else{var L=c.line===u.line&&c.ch===u.ch&&0===c.ch,T=c.line!==u.line;L||T?function(e,t,n,r){var i=t.line+1,o=n.line+1,a=t.line!==n.line,l=r+"\n",s="\n"+r;a&&o++,a&&0===n.ch&&(s=r+"\n",o--),B(e,!1,[l,s]),e.setSelection({line:i,ch:0},{line:o,ch:0})}(s,c,u,t):B(s,!1,["`","`"])}}function k(e){q(e.codemirror,"quote")}function C(e){j(e.codemirror,"smaller")}function S(e){j(e.codemirror,"bigger")}function L(e){j(e.codemirror,void 0,1)}function T(e){j(e.codemirror,void 0,2)}function M(e){j(e.codemirror,void 0,3)}function A(e){q(e.codemirror,"unordered-list")}function N(e){q(e.codemirror,"ordered-list")}function D(e){!function(e){if(/editor-preview-active/.test(e.getWrapperElement().lastChild.className))return;for(var t,n=e.getCursor("start"),r=e.getCursor("end"),i=n.line;i<=r.line;i++)t=(t=e.getLine(i)).replace(/^[ ]*([# ]+|\*|-|[> ]+|[0-9]+(.|\)))[ ]*/,""),e.replaceRange(t,{line:i,ch:0},{line:i,ch:99999999999999})}(e.codemirror)}function F(e){var t=e.codemirror,n=m(t),r=e.options,i="https://";if(r.promptURLs&&!(i=prompt(r.promptTexts.link,"https://")))return!1;B(t,n.link,r.insertTexts.link,i)}function E(e){var t=e.codemirror,n=m(t),r=e.options,i="https://";if(r.promptURLs&&!(i=prompt(r.promptTexts.image,"https://")))return!1;B(t,n.image,r.insertTexts.image,i)}function O(e){e.openBrowseFileWindow()}function I(e,t){var n=e.codemirror,r=m(n),i=e.options,o=t.substr(t.lastIndexOf("/")+1);B(n,r.image,i.insertTexts.uploadedImage,t),e.updateStatusBar("upload-image",e.options.imageTexts.sbOnUploaded.replace("#image_name#",o)),setTimeout((function(){e.updateStatusBar("upload-image",e.options.imageTexts.sbInit)}),1e3)}function z(e){var t=e.codemirror,n=m(t),r=e.options;B(t,n.table,r.insertTexts.table)}function H(e){var t=e.codemirror,n=m(t),r=e.options;B(t,n.image,r.insertTexts.horizontalRule)}function R(e){var t=e.codemirror;t.undo(),t.focus()}function _(e){var t=e.codemirror;t.redo(),t.focus()}function P(e){var t=e.codemirror,n=t.getWrapperElement(),r=n.nextSibling,i=e.toolbarElements&&e.toolbarElements["side-by-side"],o=!1,a=[n.parentNode,e.gui.toolbar,n,r,e.gui.statusbar];/editor-preview-active-side/.test(r.className)?(t.getOption("sideBySideNoFullscreen")&&(t.setOption("sideBySideNoFullscreen",!1),a.forEach((function(e){!function(e){e.className=e.className.replace(/\s*sided--no-fullscreen\s*/g,"")}(e)}))),r.className=r.className.replace(/\s*editor-preview-active-side\s*/g,""),i&&(i.className=i.className.replace(/\s*active\s*/g,"")),n.className=n.className.replace(/\s*CodeMirror-sided\s*/g," ")):(setTimeout((function(){t.getOption("fullScreen")||(!1===e.options.sideBySideFullscreen?(t.setOption("sideBySideNoFullscreen",!0),a.forEach((function(e){!function(e){e.className+=" sided--no-fullscreen"}(e)}))):v(e)),r.className+=" editor-preview-active-side"}),1),i&&(i.className+=" active"),n.className+=" CodeMirror-sided",o=!0);var l=n.lastChild;if(/editor-preview-active/.test(l.className)){l.className=l.className.replace(/\s*editor-preview-active\s*/g,"");var s=e.toolbarElements.preview,c=e.toolbar_div;s.className=s.className.replace(/\s*active\s*/g,""),c.className=c.className.replace(/\s*disabled-for-preview*/g,"")}if(t.sideBySideRenderingFunction||(t.sideBySideRenderingFunction=function(){var t=e.options.previewRender(e.value(),r);null!=t&&(r.innerHTML=t)}),o){var u=e.options.previewRender(e.value(),r);null!=u&&(r.innerHTML=u),t.on("update",t.sideBySideRenderingFunction)}else t.off("update",t.sideBySideRenderingFunction);t.refresh()}function W(e){var t=e.codemirror,n=t.getWrapperElement(),r=e.toolbar_div,i=!!e.options.toolbar&&e.toolbarElements.preview,o=n.lastChild,a=t.getWrapperElement().nextSibling;if(/editor-preview-active-side/.test(a.className)&&P(e),!o||!/editor-preview-full/.test(o.className)){if((o=document.createElement("div")).className="editor-preview-full",e.options.previewClass)if(Array.isArray(e.options.previewClass))for(var l=0;l<e.options.previewClass.length;l++)o.className+=" "+e.options.previewClass[l];else"string"==typeof e.options.previewClass&&(o.className+=" "+e.options.previewClass);n.appendChild(o)}/editor-preview-active/.test(o.className)?(o.className=o.className.replace(/\s*editor-preview-active\s*/g,""),i&&(i.className=i.className.replace(/\s*active\s*/g,""),r.className=r.className.replace(/\s*disabled-for-preview*/g,""))):(setTimeout((function(){o.className+=" editor-preview-active"}),1),i&&(i.className+=" active",r.className+=" disabled-for-preview")),o.innerHTML=e.options.previewRender(e.value(),o)}function B(e,t,n,r){if(!/editor-preview-active/.test(e.getWrapperElement().lastChild.className)){var i,o=n[0],a=n[1],l={},s={};Object.assign(l,e.getCursor("start")),Object.assign(s,e.getCursor("end")),r&&(o=o.replace("#url#",r),a=a.replace("#url#",r)),t?(o=(i=e.getLine(l.line)).slice(0,l.ch),a=i.slice(l.ch),e.replaceRange(o+a,{line:l.line,ch:0})):(i=e.getSelection(),e.replaceSelection(o+i+a),l.ch+=o.length,l!==s&&(s.ch+=o.length)),e.setSelection(l,s),e.focus()}}function j(e,t,n){if(!/editor-preview-active/.test(e.getWrapperElement().lastChild.className)){for(var r=e.getCursor("start"),i=e.getCursor("end"),o=r.line;o<=i.line;o++)!function(r){var i=e.getLine(r),o=i.search(/[^#]/);i=void 0!==t?o<=0?"bigger"==t?"###### "+i:"# "+i:6==o&&"smaller"==t?i.substr(7):1==o&&"bigger"==t?i.substr(2):"bigger"==t?i.substr(1):"#"+i:1==n?o<=0?"# "+i:o==n?i.substr(o+1):"# "+i.substr(o+1):2==n?o<=0?"## "+i:o==n?i.substr(o+1):"## "+i.substr(o+1):o<=0?"### "+i:o==n?i.substr(o+1):"### "+i.substr(o+1),e.replaceRange(i,{line:r,ch:0},{line:r,ch:99999999999999})}(o);e.focus()}}function q(e,t){if(!/editor-preview-active/.test(e.getWrapperElement().lastChild.className)){for(var n=/^(\s*)(\*|-|\+|\d*\.)(\s+)/,r=/^\s*/,i=m(e),o=e.getCursor("start"),a=e.getCursor("end"),l={quote:/^(\s*)>\s+/,"unordered-list":n,"ordered-list":n},s=function(e,t,i){var o=n.exec(t),a=function(e,t){return{quote:">","unordered-list":"*","ordered-list":"%%i."}[e].replace("%%i",t)}(e,c);return null!==o?(function(e,t){var n=new RegExp({quote:">","unordered-list":"*","ordered-list":"\\d+."}[e]);return t&&n.test(t)}(e,o[2])&&(a=""),t=o[1]+a+o[3]+t.replace(r,"").replace(l[e],"$1")):0==i&&(t=a+" "+t),t},c=1,u=o.line;u<=a.line;u++)!function(n){var r=e.getLine(n);i[t]?r=r.replace(l[t],"$1"):("unordered-list"==t&&(r=s("ordered-list",r,!0)),r=s(t,r,!1),c+=1),e.replaceRange(r,{line:n,ch:0},{line:n,ch:99999999999999})}(u);e.focus()}}function U(e,t,n,r){if(!/editor-preview-active/.test(e.codemirror.getWrapperElement().lastChild.className)){r=void 0===r?n:r;var i,o=e.codemirror,a=m(o),l=n,s=r,c=o.getCursor("start"),u=o.getCursor("end");a[t]?(l=(i=o.getLine(c.line)).slice(0,c.ch),s=i.slice(c.ch),"bold"==t?(l=l.replace(/(\*\*|__)(?![\s\S]*(\*\*|__))/,""),s=s.replace(/(\*\*|__)/,"")):"italic"==t?(l=l.replace(/(\*|_)(?![\s\S]*(\*|_))/,""),s=s.replace(/(\*|_)/,"")):"strikethrough"==t&&(l=l.replace(/(\*\*|~~)(?![\s\S]*(\*\*|~~))/,""),s=s.replace(/(\*\*|~~)/,"")),o.replaceRange(l+s,{line:c.line,ch:0},{line:c.line,ch:99999999999999}),"bold"==t||"strikethrough"==t?(c.ch-=2,c!==u&&(u.ch-=2)):"italic"==t&&(c.ch-=1,c!==u&&(u.ch-=1))):(i=o.getSelection(),"bold"==t?i=(i=i.split("**").join("")).split("__").join(""):"italic"==t?i=(i=i.split("*").join("")).split("_").join(""):"strikethrough"==t&&(i=i.split("~~").join("")),o.replaceSelection(l+i+s),c.ch+=n.length,u.ch=c.ch+i.length),o.setSelection(c,u),o.focus()}}function $(e,t){if(Math.abs(e)<1024)return""+e+t[0];var n=0;do{e/=1024,++n}while(Math.abs(e)>=1024&&n<t.length);return""+e.toFixed(1)+t[n]}function G(e,t){for(var n in t)Object.prototype.hasOwnProperty.call(t,n)&&(t[n]instanceof Array?e[n]=t[n].concat(e[n]instanceof Array?e[n]:[]):null!==t[n]&&"object"==typeof t[n]&&t[n].constructor===Object?e[n]=G(e[n]||{},t[n]):e[n]=t[n]);return e}function V(e){for(var t=1;t<arguments.length;t++)e=G(e,arguments[t]);return e}function K(e){var t=e.match(/[a-zA-Z0-9_\u00A0-\u02AF\u0392-\u03c9\u0410-\u04F9]+|[\u4E00-\u9FFF\u3400-\u4dbf\uf900-\ufaff\u3040-\u309f\uac00-\ud7af]+/g),n=0;if(null===t)return n;for(var r=0;r<t.length;r++)t[r].charCodeAt(0)>=19968?n+=t[r].length:n+=1;return n}var X={bold:{name:"bold",action:x,className:"fa fa-bold",title:"Bold",default:!0},italic:{name:"italic",action:y,className:"fa fa-italic",title:"Italic",default:!0},strikethrough:{name:"strikethrough",action:b,className:"fa fa-strikethrough",title:"Strikethrough"},heading:{name:"heading",action:C,className:"fa fa-header fa-heading",title:"Heading",default:!0},"heading-smaller":{name:"heading-smaller",action:C,className:"fa fa-header fa-heading header-smaller",title:"Smaller Heading"},"heading-bigger":{name:"heading-bigger",action:S,className:"fa fa-header fa-heading header-bigger",title:"Bigger Heading"},"heading-1":{name:"heading-1",action:L,className:"fa fa-header fa-heading header-1",title:"Big Heading"},"heading-2":{name:"heading-2",action:T,className:"fa fa-header fa-heading header-2",title:"Medium Heading"},"heading-3":{name:"heading-3",action:M,className:"fa fa-header fa-heading header-3",title:"Small Heading"},"separator-1":{name:"separator-1"},code:{name:"code",action:w,className:"fa fa-code",title:"Code"},quote:{name:"quote",action:k,className:"fa fa-quote-left",title:"Quote",default:!0},"unordered-list":{name:"unordered-list",action:A,className:"fa fa-list-ul",title:"Generic List",default:!0},"ordered-list":{name:"ordered-list",action:N,className:"fa fa-list-ol",title:"Numbered List",default:!0},"clean-block":{name:"clean-block",action:D,className:"fa fa-eraser",title:"Clean block"},"separator-2":{name:"separator-2"},link:{name:"link",action:F,className:"fa fa-link",title:"Create Link",default:!0},image:{name:"image",action:E,className:"fa fa-image",title:"Insert Image",default:!0},"upload-image":{name:"upload-image",action:O,className:"fa fa-image",title:"Import an image"},table:{name:"table",action:z,className:"fa fa-table",title:"Insert Table"},"horizontal-rule":{name:"horizontal-rule",action:H,className:"fa fa-minus",title:"Insert Horizontal Line"},"separator-3":{name:"separator-3"},preview:{name:"preview",action:W,className:"fa fa-eye",noDisable:!0,title:"Toggle Preview",default:!0},"side-by-side":{name:"side-by-side",action:P,className:"fa fa-columns",noDisable:!0,noMobile:!0,title:"Toggle Side by Side",default:!0},fullscreen:{name:"fullscreen",action:v,className:"fa fa-arrows-alt",noDisable:!0,noMobile:!0,title:"Toggle Fullscreen",default:!0},"separator-4":{name:"separator-4"},guide:{name:"guide",action:"https://www.markdownguide.org/basic-syntax/",className:"fa fa-question-circle",noDisable:!0,title:"Markdown Guide",default:!0},"separator-5":{name:"separator-5"},undo:{name:"undo",action:R,className:"fa fa-undo",noDisable:!0,title:"Undo"},redo:{name:"redo",action:_,className:"fa fa-repeat fa-redo",noDisable:!0,title:"Redo"}},Z={link:["[","](#url#)"],image:["![](","#url#)"],uploadedImage:["![](#url#)",""],table:["","\n\n| Column 1 | Column 2 | Column 3 |\n| -------- | -------- | -------- |\n| Text     | Text     | Text     |\n\n"],horizontalRule:["","\n\n-----\n\n"]},Y={link:"URL for the link:",image:"URL of the image:"},Q={locale:"en-US",format:{hour:"2-digit",minute:"2-digit"}},J={bold:"**",code:"```",italic:"*"},ee={sbInit:"Attach files by drag and dropping or pasting from clipboard.",sbOnDragEnter:"Drop image to upload it.",sbOnDrop:"Uploading image #images_names#...",sbProgress:"Uploading #file_name#: #progress#%",sbOnUploaded:"Uploaded #image_name#",sizeUnits:" B, KB, MB"},te={noFileGiven:"You must select a file.",typeNotAllowed:"This image type is not allowed.",fileTooLarge:"Image #image_name# is too big (#image_size#).\nMaximum file size is #image_max_size#.",importError:"Something went wrong when uploading the image #image_name#."};function ne(e){(e=e||{}).parent=this;var t=!0;if(!1===e.autoDownloadFontAwesome&&(t=!1),!0!==e.autoDownloadFontAwesome)for(var n=document.styleSheets,r=0;r<n.length;r++)n[r].href&&n[r].href.indexOf("//maxcdn.bootstrapcdn.com/font-awesome/")>-1&&(t=!1);if(t){var i=document.createElement("link");i.rel="stylesheet",i.href="https://maxcdn.bootstrapcdn.com/font-awesome/latest/css/font-awesome.min.css",document.getElementsByTagName("head")[0].appendChild(i)}if(e.element)this.element=e.element;else if(null===e.element)return void console.log("EasyMDE: Error. No element was found.");if(void 0===e.toolbar)for(var o in e.toolbar=[],X)Object.prototype.hasOwnProperty.call(X,o)&&(-1!=o.indexOf("separator-")&&e.toolbar.push("|"),(!0===X[o].default||e.showIcons&&e.showIcons.constructor===Array&&-1!=e.showIcons.indexOf(o))&&e.toolbar.push(o));if(Object.prototype.hasOwnProperty.call(e,"previewClass")||(e.previewClass="editor-preview"),Object.prototype.hasOwnProperty.call(e,"status")||(e.status=["autosave","lines","words","cursor"],e.uploadImage&&e.status.unshift("upload-image")),e.previewRender||(e.previewRender=function(e){return this.parent.markdown(e)}),e.parsingConfig=V({highlightFormatting:!0},e.parsingConfig||{}),e.insertTexts=V({},Z,e.insertTexts||{}),e.promptTexts=V({},Y,e.promptTexts||{}),e.blockStyles=V({},J,e.blockStyles||{}),null!=e.autosave&&(e.autosave.timeFormat=V({},Q,e.autosave.timeFormat||{})),e.shortcuts=V({},c,e.shortcuts||{}),e.maxHeight=e.maxHeight||void 0,void 0!==e.maxHeight?e.minHeight=e.maxHeight:e.minHeight=e.minHeight||"300px",e.errorCallback=e.errorCallback||function(e){alert(e)},e.uploadImage=e.uploadImage||!1,e.imageMaxSize=e.imageMaxSize||2097152,e.imageAccept=e.imageAccept||"image/png, image/jpeg",e.imageTexts=V({},ee,e.imageTexts||{}),e.errorMessages=V({},te,e.errorMessages||{}),null!=e.autosave&&null!=e.autosave.unique_id&&""!=e.autosave.unique_id&&(e.autosave.uniqueId=e.autosave.unique_id),e.overlayMode&&void 0===e.overlayMode.combine&&(e.overlayMode.combine=!0),this.options=e,this.render(),!e.initialValue||this.options.autosave&&!0===this.options.autosave.foundSavedValue||this.value(e.initialValue),e.uploadImage){var a=this;this.codemirror.on("dragenter",(function(e,t){a.updateStatusBar("upload-image",a.options.imageTexts.sbOnDragEnter),t.stopPropagation(),t.preventDefault()})),this.codemirror.on("dragend",(function(e,t){a.updateStatusBar("upload-image",a.options.imageTexts.sbInit),t.stopPropagation(),t.preventDefault()})),this.codemirror.on("dragleave",(function(e,t){a.updateStatusBar("upload-image",a.options.imageTexts.sbInit),t.stopPropagation(),t.preventDefault()})),this.codemirror.on("dragover",(function(e,t){a.updateStatusBar("upload-image",a.options.imageTexts.sbOnDragEnter),t.stopPropagation(),t.preventDefault()})),this.codemirror.on("drop",(function(t,n){n.stopPropagation(),n.preventDefault(),e.imageUploadFunction?a.uploadImagesUsingCustomFunction(e.imageUploadFunction,n.dataTransfer.files):a.uploadImages(n.dataTransfer.files)})),this.codemirror.on("paste",(function(t,n){e.imageUploadFunction?a.uploadImagesUsingCustomFunction(e.imageUploadFunction,n.clipboardData.files):a.uploadImages(n.clipboardData.files)}))}}function re(){if("object"!=typeof localStorage)return!1;try{localStorage.setItem("smde_localStorage",1),localStorage.removeItem("smde_localStorage")}catch(e){return!1}return!0}ne.prototype.uploadImages=function(e,t,n){if(0!==e.length){for(var r=[],i=0;i<e.length;i++)r.push(e[i].name),this.uploadImage(e[i],t,n);this.updateStatusBar("upload-image",this.options.imageTexts.sbOnDrop.replace("#images_names#",r.join(", ")))}},ne.prototype.uploadImagesUsingCustomFunction=function(e,t){if(0!==t.length){for(var n=[],r=0;r<t.length;r++)n.push(t[r].name),this.uploadImageUsingCustomFunction(e,t[r]);this.updateStatusBar("upload-image",this.options.imageTexts.sbOnDrop.replace("#images_names#",n.join(", ")))}},ne.prototype.updateStatusBar=function(e,t){if(this.gui.statusbar){var n=this.gui.statusbar.getElementsByClassName(e);1===n.length?this.gui.statusbar.getElementsByClassName(e)[0].textContent=t:0===n.length?console.log("EasyMDE: status bar item "+e+" was not found."):console.log("EasyMDE: Several status bar items named "+e+" was found.")}},ne.prototype.markdown=function(e){if(o){var t;if(t=this.options&&this.options.renderingConfig&&this.options.renderingConfig.markedOptions?this.options.renderingConfig.markedOptions:{},this.options&&this.options.renderingConfig&&!1===this.options.renderingConfig.singleLineBreaks?t.breaks=!1:t.breaks=!0,this.options&&this.options.renderingConfig&&!0===this.options.renderingConfig.codeSyntaxHighlighting){var n=this.options.renderingConfig.hljs||window.hljs;n&&(t.highlight=function(e,t){return t&&n.getLanguage(t)?n.highlight(t,e).value:n.highlightAuto(e).value})}o.setOptions(t);var r=o(e);return this.options.renderingConfig&&"function"==typeof this.options.renderingConfig.sanitizerFunction&&(r=this.options.renderingConfig.sanitizerFunction.call(this,r)),r=function(e){for(var t=(new DOMParser).parseFromString(e,"text/html"),n=t.getElementsByTagName("li"),r=0;r<n.length;r++)for(var i=n[r],o=0;o<i.children.length;o++){var a=i.children[o];a instanceof HTMLInputElement&&"checkbox"===a.type&&(i.style.marginLeft="-1.5em",i.style.listStyleType="none")}return t.documentElement.innerHTML}(r=function(e){for(var t;null!==(t=l.exec(e));){var n=t[0];if(-1===n.indexOf("target=")){var r=n.replace(/>$/,' target="_blank">');e=e.replace(n,r)}}return e}(r))}},ne.prototype.render=function(e){if(e||(e=this.element||document.getElementsByTagName("textarea")[0]),!this._rendered||this._rendered!==e){this.element=e;var t,n,o=this.options,a=this,l={};for(var c in o.shortcuts)null!==o.shortcuts[c]&&null!==s[c]&&function(e){l[d(o.shortcuts[e])]=function(){var t=s[e];"function"==typeof t?t(a):"string"==typeof t&&window.open(t,"_blank")}}(c);if(l.Enter="newlineAndIndentContinueMarkdownList",l.Tab="tabAndIndentMarkdownList",l["Shift-Tab"]="shiftTabAndUnindentMarkdownList",l.Esc=function(e){e.getOption("fullScreen")&&v(a)},this.documentOnKeyDown=function(e){27==(e=e||window.event).keyCode&&a.codemirror.getOption("fullScreen")&&v(a)},document.addEventListener("keydown",this.documentOnKeyDown,!1),o.overlayMode?(r.defineMode("overlay-mode",(function(e){return r.overlayMode(r.getMode(e,!1!==o.spellChecker?"spell-checker":"gfm"),o.overlayMode.mode,o.overlayMode.combine)})),t="overlay-mode",(n=o.parsingConfig).gitHubSpice=!1,!1!==o.spellChecker?(n.name="gfm",i({codeMirrorInstance:r})):!1!==o.spellChecker&&(t="spell-checker",(n=o.parsingConfig).name="gfm",n.gitHubSpice=!1,i({codeMirrorInstance:r}))):((t=o.parsingConfig).name="gfm",t.gitHubSpice=!1),this.codemirror=r.fromTextArea(e,{mode:t,backdrop:n,theme:null!=o.theme?o.theme:"easymde",tabSize:null!=o.tabSize?o.tabSize:2,indentUnit:null!=o.tabSize?o.tabSize:2,indentWithTabs:!1!==o.indentWithTabs,lineNumbers:!1,autofocus:!0===o.autofocus,extraKeys:l,lineWrapping:!1!==o.lineWrapping,allowDropFileTypes:["text/plain"],placeholder:o.placeholder||e.getAttribute("placeholder")||"",styleSelectedText:null!=o.styleSelectedText?o.styleSelectedText:!u(),configureMouse:function(e,t,n){return{addNew:!1}},inputStyle:null!=o.inputStyle?o.inputStyle:u()?"contenteditable":"textarea",spellcheck:null==o.nativeSpellcheck||o.nativeSpellcheck}),this.codemirror.getScrollerElement().style.minHeight=o.minHeight,void 0!==o.maxHeight&&(this.codemirror.getScrollerElement().style.height=o.maxHeight),!0===o.forceSync){var h=this.codemirror;h.on("change",(function(){h.save()}))}this.gui={};var f=document.createElement("div");f.classList.add("EasyMDEContainer");var p=this.codemirror.getWrapperElement();p.parentNode.insertBefore(f,p),f.appendChild(p),!1!==o.toolbar&&(this.gui.toolbar=this.createToolbar()),!1!==o.status&&(this.gui.statusbar=this.createStatusbar()),null!=o.autosave&&!0===o.autosave.enabled&&(this.autosave(),this.codemirror.on("change",(function(){clearTimeout(a._autosave_timeout),a._autosave_timeout=setTimeout((function(){a.autosave()}),a.options.autosave.submit_delay||a.options.autosave.delay||1e3)}))),this.codemirror.on("update",(function(){g()})),this.onWindowResize=function(){g()},window.addEventListener("resize",this.onWindowResize,!0),this.gui.sideBySide=this.createSideBySide(),this._rendered=this.element;var m=this.codemirror;setTimeout(function(){m.refresh()}.bind(m),0)}function g(){function e(e,t){return e<window.getComputedStyle(document.querySelector(".CodeMirror-sizer")).width.replace("px","")?t+"px":t/e*100+"%"}!1!==o.previewImagesInEditor&&f.querySelectorAll(".cm-formatting-image").forEach((function(t){var n=t.parentElement;if(n.hasAttribute("data-img-src")){var r=n.getAttribute("data-img-src"),i=+n.getAttribute("data-img-width"),o=+n.getAttribute("data-img-height");n.setAttribute("style","--bg-image:url("+r+");--width:"+i+"px;--height:"+e(i,o))}else{var a=n.innerText.match("\\((.*)\\)");if(a&&a.length>=2){var l=document.createElement("img");l.onload=function(){n.setAttribute("data-img-src",a[1]),n.setAttribute("data-img-width",l.naturalWidth),n.setAttribute("data-img-height",l.naturalHeight),n.setAttribute("style","--bg-image:url("+a[1]+");--width:"+l.naturalWidth+"px;--height:"+e(l.naturalWidth,l.naturalHeight))},l.src=a[1]}}}))}},ne.prototype.cleanup=function(){window.removeEventListener(this.onWindowResize),document.removeEventListener("keydown",this.documentOnKeyDown)},ne.prototype.autosave=function(){if(re()){var e=this;if(null==this.options.autosave.uniqueId||""==this.options.autosave.uniqueId)return void console.log("EasyMDE: You must set a uniqueId to use the autosave feature");!0!==this.options.autosave.binded&&(null!=e.element.form&&null!=e.element.form&&e.element.form.addEventListener("submit",(function(){clearTimeout(e.autosaveTimeoutId),e.autosaveTimeoutId=void 0,localStorage.removeItem("smde_"+e.options.autosave.uniqueId)})),this.options.autosave.binded=!0),!0!==this.options.autosave.loaded&&("string"==typeof localStorage.getItem("smde_"+this.options.autosave.uniqueId)&&""!=localStorage.getItem("smde_"+this.options.autosave.uniqueId)&&(this.codemirror.setValue(localStorage.getItem("smde_"+this.options.autosave.uniqueId)),this.options.autosave.foundSavedValue=!0),this.options.autosave.loaded=!0);var t=e.value();""!==t?localStorage.setItem("smde_"+this.options.autosave.uniqueId,t):localStorage.removeItem("smde_"+this.options.autosave.uniqueId);var n=document.getElementById("autosaved");if(null!=n&&null!=n&&""!=n){var r=new Date,i=new Intl.DateTimeFormat([this.options.autosave.timeFormat.locale,"en-US"],this.options.autosave.timeFormat.format).format(r),o=null==this.options.autosave.text?"Autosaved: ":this.options.autosave.text;n.innerHTML=o+i}}else console.log("EasyMDE: localStorage not available, cannot autosave")},ne.prototype.clearAutosavedValue=function(){if(re()){if(null==this.options.autosave||null==this.options.autosave.uniqueId||""==this.options.autosave.uniqueId)return void console.log("EasyMDE: You must set a uniqueId to clear the autosave value");localStorage.removeItem("smde_"+this.options.autosave.uniqueId)}else console.log("EasyMDE: localStorage not available, cannot autosave")},ne.prototype.openBrowseFileWindow=function(e,t){var n=this,r=this.gui.toolbar.getElementsByClassName("imageInput")[0];r.click(),r.addEventListener("change",(function i(o){n.options.imageUploadFunction?n.uploadImagesUsingCustomFunction(n.options.imageUploadFunction,o.target.files):n.uploadImages(o.target.files,e,t),r.removeEventListener("change",i)}))},ne.prototype.uploadImage=function(e,t,n){var r=this;function i(e){r.updateStatusBar("upload-image",e),setTimeout((function(){r.updateStatusBar("upload-image",r.options.imageTexts.sbInit)}),1e4),n&&"function"==typeof n&&n(e),r.options.errorCallback(e)}function o(t){var n=r.options.imageTexts.sizeUnits.split(",");return t.replace("#image_name#",e.name).replace("#image_size#",$(e.size,n)).replace("#image_max_size#",$(r.options.imageMaxSize,n))}if(t=t||function(e){I(r,e)},e.size>this.options.imageMaxSize)i(o(this.options.errorMessages.fileTooLarge));else{var a=new FormData;a.append("image",e),r.options.imageCSRFToken&&a.append("csrfmiddlewaretoken",r.options.imageCSRFToken);var l=new XMLHttpRequest;l.upload.onprogress=function(t){if(t.lengthComputable){var n=""+Math.round(100*t.loaded/t.total);r.updateStatusBar("upload-image",r.options.imageTexts.sbProgress.replace("#file_name#",e.name).replace("#progress#",n))}},l.open("POST",this.options.imageUploadEndpoint),l.onload=function(){try{var e=JSON.parse(this.responseText)}catch(e){return console.error("EasyMDE: The server did not return a valid json."),void i(o(r.options.errorMessages.importError))}200===this.status&&e&&!e.error&&e.data&&e.data.filePath?t(window.location.origin+"/"+e.data.filePath):e.error&&e.error in r.options.errorMessages?i(o(r.options.errorMessages[e.error])):e.error?i(o(e.error)):(console.error("EasyMDE: Received an unexpected response after uploading the image."+this.status+" ("+this.statusText+")"),i(o(r.options.errorMessages.importError)))},l.onerror=function(e){console.error("EasyMDE: An unexpected error occurred when trying to upload the image."+e.target.status+" ("+e.target.statusText+")"),i(r.options.errorMessages.importError)},l.send(a)}},ne.prototype.uploadImageUsingCustomFunction=function(e,t){var n=this;e.apply(this,[t,function(e){I(n,e)},function(e){var r=function(e){var r=n.options.imageTexts.sizeUnits.split(",");return e.replace("#image_name#",t.name).replace("#image_size#",$(t.size,r)).replace("#image_max_size#",$(n.options.imageMaxSize,r))}(e);n.updateStatusBar("upload-image",r),setTimeout((function(){n.updateStatusBar("upload-image",n.options.imageTexts.sbInit)}),1e4),n.options.errorCallback(r)}])},ne.prototype.setPreviewMaxHeight=function(){var e=this.codemirror.getWrapperElement(),t=e.nextSibling,n=parseInt(window.getComputedStyle(e).paddingTop),r=parseInt(window.getComputedStyle(e).borderTopWidth),i=(parseInt(this.options.maxHeight)+2*n+2*r).toString()+"px";t.style.height=i},ne.prototype.createSideBySide=function(){var e=this.codemirror,t=e.getWrapperElement(),n=t.nextSibling;if(!n||!/editor-preview-side/.test(n.className)){if((n=document.createElement("div")).className="editor-preview-side",this.options.previewClass)if(Array.isArray(this.options.previewClass))for(var r=0;r<this.options.previewClass.length;r++)n.className+=" "+this.options.previewClass[r];else"string"==typeof this.options.previewClass&&(n.className+=" "+this.options.previewClass);t.parentNode.insertBefore(n,t.nextSibling)}if(void 0!==this.options.maxHeight&&this.setPreviewMaxHeight(),!1===this.options.syncSideBySidePreviewScroll)return n;var i=!1,o=!1;return e.on("scroll",(function(e){if(i)i=!1;else{o=!0;var t=e.getScrollInfo().height-e.getScrollInfo().clientHeight,r=parseFloat(e.getScrollInfo().top)/t,a=(n.scrollHeight-n.clientHeight)*r;n.scrollTop=a}})),n.onscroll=function(){if(o)o=!1;else{i=!0;var t=n.scrollHeight-n.clientHeight,r=parseFloat(n.scrollTop)/t,a=(e.getScrollInfo().height-e.getScrollInfo().clientHeight)*r;e.scrollTo(0,a)}},n},ne.prototype.createToolbar=function(e){if((e=e||this.options.toolbar)&&0!==e.length){var t;for(t=0;t<e.length;t++)null!=X[e[t]]&&(e[t]=X[e[t]]);var n=document.createElement("div");n.className="editor-toolbar";var r=this,i={};for(r.toolbar=e,t=0;t<e.length;t++)if(("guide"!=e[t].name||!1!==r.options.toolbarGuideIcon)&&!(r.options.hideIcons&&-1!=r.options.hideIcons.indexOf(e[t].name)||("fullscreen"==e[t].name||"side-by-side"==e[t].name)&&u())){if("|"===e[t]){for(var o=!1,a=t+1;a<e.length;a++)"|"===e[a]||r.options.hideIcons&&-1!=r.options.hideIcons.indexOf(e[a].name)||(o=!0);if(!o)continue}!function(e){var t;if(t="|"===e?p():e.children?h(e,r.options.toolbarTips,r.options.shortcuts,r):f(e,!0,r.options.toolbarTips,r.options.shortcuts,"button",r),i[e.name||e]=t,n.appendChild(t),"upload-image"===e.name){var o=document.createElement("input");o.className="imageInput",o.type="file",o.multiple=!0,o.name="image",o.accept=r.options.imageAccept,o.style.display="none",o.style.opacity=0,n.appendChild(o)}}(e[t])}r.toolbar_div=n,r.toolbarElements=i;var l=this.codemirror;l.on("cursorActivity",(function(){var e=m(l);for(var t in i)!function(t){var n=i[t];e[t]?n.className+=" active":"fullscreen"!=t&&"side-by-side"!=t&&(n.className=n.className.replace(/\s*active\s*/g,""))}(t)}));var s=l.getWrapperElement();return s.parentNode.insertBefore(n,s),n}},ne.prototype.createStatusbar=function(e){e=e||this.options.status;var t=this.options,n=this.codemirror;if(e&&0!==e.length){var r,i,o,a,l=[];for(r=0;r<e.length;r++)if(i=void 0,o=void 0,a=void 0,"object"==typeof e[r])l.push({className:e[r].className,defaultValue:e[r].defaultValue,onUpdate:e[r].onUpdate,onActivity:e[r].onActivity});else{var s=e[r];"words"===s?(a=function(e){e.innerHTML=K(n.getValue())},i=function(e){e.innerHTML=K(n.getValue())}):"lines"===s?(a=function(e){e.innerHTML=n.lineCount()},i=function(e){e.innerHTML=n.lineCount()}):"cursor"===s?(a=function(e){e.innerHTML="0:0"},o=function(e){var t=n.getCursor();e.innerHTML=t.line+":"+t.ch}):"autosave"===s?a=function(e){null!=t.autosave&&!0===t.autosave.enabled&&e.setAttribute("id","autosaved")}:"upload-image"===s&&(a=function(e){e.innerHTML=t.imageTexts.sbInit}),l.push({className:s,defaultValue:a,onUpdate:i,onActivity:o})}var c=document.createElement("div");for(c.className="editor-statusbar",r=0;r<l.length;r++){var u=l[r],d=document.createElement("span");d.className=u.className,"function"==typeof u.defaultValue&&u.defaultValue(d),"function"==typeof u.onUpdate&&this.codemirror.on("update",function(e,t){return function(){t.onUpdate(e)}}(d,u)),"function"==typeof u.onActivity&&this.codemirror.on("cursorActivity",function(e,t){return function(){t.onActivity(e)}}(d,u)),c.appendChild(d)}var h=this.codemirror.getWrapperElement();return h.parentNode.insertBefore(c,h.nextSibling),c}},ne.prototype.value=function(e){var t=this.codemirror;if(void 0===e)return t.getValue();if(t.getDoc().setValue(e),this.isPreviewActive()){var n=t.getWrapperElement().lastChild;n.innerHTML=this.options.previewRender(e,n)}return this},ne.toggleBold=x,ne.toggleItalic=y,ne.toggleStrikethrough=b,ne.toggleBlockquote=k,ne.toggleHeadingSmaller=C,ne.toggleHeadingBigger=S,ne.toggleHeading1=L,ne.toggleHeading2=T,ne.toggleHeading3=M,ne.toggleCodeBlock=w,ne.toggleUnorderedList=A,ne.toggleOrderedList=N,ne.cleanBlock=D,ne.drawLink=F,ne.drawImage=E,ne.drawUploadedImage=O,ne.drawTable=z,ne.drawHorizontalRule=H,ne.undo=R,ne.redo=_,ne.togglePreview=W,ne.toggleSideBySide=P,ne.toggleFullScreen=v,ne.prototype.toggleBold=function(){x(this)},ne.prototype.toggleItalic=function(){y(this)},ne.prototype.toggleStrikethrough=function(){b(this)},ne.prototype.toggleBlockquote=function(){k(this)},ne.prototype.toggleHeadingSmaller=function(){C(this)},ne.prototype.toggleHeadingBigger=function(){S(this)},ne.prototype.toggleHeading1=function(){L(this)},ne.prototype.toggleHeading2=function(){T(this)},ne.prototype.toggleHeading3=function(){M(this)},ne.prototype.toggleCodeBlock=function(){w(this)},ne.prototype.toggleUnorderedList=function(){A(this)},ne.prototype.toggleOrderedList=function(){N(this)},ne.prototype.cleanBlock=function(){D(this)},ne.prototype.drawLink=function(){F(this)},ne.prototype.drawImage=function(){E(this)},ne.prototype.drawUploadedImage=function(){O(this)},ne.prototype.drawTable=function(){z(this)},ne.prototype.drawHorizontalRule=function(){H(this)},ne.prototype.undo=function(){R(this)},ne.prototype.redo=function(){_(this)},ne.prototype.togglePreview=function(){W(this)},ne.prototype.toggleSideBySide=function(){P(this)},ne.prototype.toggleFullScreen=function(){v(this)},ne.prototype.isPreviewActive=function(){var e=this.codemirror.getWrapperElement().lastChild;return/editor-preview-active/.test(e.className)},ne.prototype.isSideBySideActive=function(){var e=this.codemirror.getWrapperElement().nextSibling;return/editor-preview-active-side/.test(e.className)},ne.prototype.isFullscreenActive=function(){return this.codemirror.getOption("fullScreen")},ne.prototype.getState=function(){return m(this.codemirror)},ne.prototype.toTextArea=function(){var e=this.codemirror,t=e.getWrapperElement(),n=t.parentNode;n&&(this.gui.toolbar&&n.removeChild(this.gui.toolbar),this.gui.statusbar&&n.removeChild(this.gui.statusbar),this.gui.sideBySide&&n.removeChild(this.gui.sideBySide)),n.parentNode.insertBefore(t,n),n.remove(),e.toTextArea(),this.autosaveTimeoutId&&(clearTimeout(this.autosaveTimeoutId),this.autosaveTimeoutId=void 0,this.clearAutosavedValue())},t.exports=ne},{"./codemirror/tablist":16,codemirror:9,"codemirror-spell-checker":2,"codemirror/addon/display/fullscreen.js":3,"codemirror/addon/display/placeholder.js":4,"codemirror/addon/edit/continuelist.js":5,"codemirror/addon/mode/overlay.js":6,"codemirror/addon/search/searchcursor.js":7,"codemirror/addon/selection/mark-selection.js":8,"codemirror/mode/gfm/gfm.js":10,"codemirror/mode/markdown/markdown.js":11,"codemirror/mode/xml/xml.js":13,"marked/lib/marked":14}]},{},[17])(17)}));
// CodeMirror, copyright (c) by Marijn Haverbeke and others
// Distributed under an MIT license: https://codemirror.net/LICENSE

// This is CodeMirror (https://codemirror.net), a code editor
// implemented in JavaScript on top of the browser's DOM.
//
// You can find some technical background for some of the code below
// at http://marijnhaverbeke.nl/blog/#cm-internals .

(function (global, factory) {
  typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
  typeof define === 'function' && define.amd ? define(factory) :
  (global = global || self, global.CodeMirror = factory());
}(this, (function () { 'use strict';

  // Kludges for bugs and behavior differences that can't be feature
  // detected are enabled based on userAgent etc sniffing.
  var userAgent = navigator.userAgent;
  var platform = navigator.platform;

  var gecko = /gecko\/\d/i.test(userAgent);
  var ie_upto10 = /MSIE \d/.test(userAgent);
  var ie_11up = /Trident\/(?:[7-9]|\d{2,})\..*rv:(\d+)/.exec(userAgent);
  var edge = /Edge\/(\d+)/.exec(userAgent);
  var ie = ie_upto10 || ie_11up || edge;
  var ie_version = ie && (ie_upto10 ? document.documentMode || 6 : +(edge || ie_11up)[1]);
  var webkit = !edge && /WebKit\//.test(userAgent);
  var qtwebkit = webkit && /Qt\/\d+\.\d+/.test(userAgent);
  var chrome = !edge && /Chrome\//.test(userAgent);
  var presto = /Opera\//.test(userAgent);
  var safari = /Apple Computer/.test(navigator.vendor);
  var mac_geMountainLion = /Mac OS X 1\d\D([8-9]|\d\d)\D/.test(userAgent);
  var phantom = /PhantomJS/.test(userAgent);

  var ios = !edge && /AppleWebKit/.test(userAgent) && /Mobile\/\w+/.test(userAgent);
  var android = /Android/.test(userAgent);
  // This is woefully incomplete. Suggestions for alternative methods welcome.
  var mobile = ios || android || /webOS|BlackBerry|Opera Mini|Opera Mobi|IEMobile/i.test(userAgent);
  var mac = ios || /Mac/.test(platform);
  var chromeOS = /\bCrOS\b/.test(userAgent);
  var windows = /win/i.test(platform);

  var presto_version = presto && userAgent.match(/Version\/(\d*\.\d*)/);
  if (presto_version) { presto_version = Number(presto_version[1]); }
  if (presto_version && presto_version >= 15) { presto = false; webkit = true; }
  // Some browsers use the wrong event properties to signal cmd/ctrl on OS X
  var flipCtrlCmd = mac && (qtwebkit || presto && (presto_version == null || presto_version < 12.11));
  var captureRightClick = gecko || (ie && ie_version >= 9);

  function classTest(cls) { return new RegExp("(^|\\s)" + cls + "(?:$|\\s)\\s*") }

  var rmClass = function(node, cls) {
    var current = node.className;
    var match = classTest(cls).exec(current);
    if (match) {
      var after = current.slice(match.index + match[0].length);
      node.className = current.slice(0, match.index) + (after ? match[1] + after : "");
    }
  };

  function removeChildren(e) {
    for (var count = e.childNodes.length; count > 0; --count)
      { e.removeChild(e.firstChild); }
    return e
  }

  function removeChildrenAndAdd(parent, e) {
    return removeChildren(parent).appendChild(e)
  }

  function elt(tag, content, className, style) {
    var e = document.createElement(tag);
    if (className) { e.className = className; }
    if (style) { e.style.cssText = style; }
    if (typeof content == "string") { e.appendChild(document.createTextNode(content)); }
    else if (content) { for (var i = 0; i < content.length; ++i) { e.appendChild(content[i]); } }
    return e
  }
  // wrapper for elt, which removes the elt from the accessibility tree
  function eltP(tag, content, className, style) {
    var e = elt(tag, content, className, style);
    e.setAttribute("role", "presentation");
    return e
  }

  var range;
  if (document.createRange) { range = function(node, start, end, endNode) {
    var r = document.createRange();
    r.setEnd(endNode || node, end);
    r.setStart(node, start);
    return r
  }; }
  else { range = function(node, start, end) {
    var r = document.body.createTextRange();
    try { r.moveToElementText(node.parentNode); }
    catch(e) { return r }
    r.collapse(true);
    r.moveEnd("character", end);
    r.moveStart("character", start);
    return r
  }; }

  function contains(parent, child) {
    if (child.nodeType == 3) // Android browser always returns false when child is a textnode
      { child = child.parentNode; }
    if (parent.contains)
      { return parent.contains(child) }
    do {
      if (child.nodeType == 11) { child = child.host; }
      if (child == parent) { return true }
    } while (child = child.parentNode)
  }

  function activeElt() {
    // IE and Edge may throw an "Unspecified Error" when accessing document.activeElement.
    // IE < 10 will throw when accessed while the page is loading or in an iframe.
    // IE > 9 and Edge will throw when accessed in an iframe if document.body is unavailable.
    var activeElement;
    try {
      activeElement = document.activeElement;
    } catch(e) {
      activeElement = document.body || null;
    }
    while (activeElement && activeElement.shadowRoot && activeElement.shadowRoot.activeElement)
      { activeElement = activeElement.shadowRoot.activeElement; }
    return activeElement
  }

  function addClass(node, cls) {
    var current = node.className;
    if (!classTest(cls).test(current)) { node.className += (current ? " " : "") + cls; }
  }
  function joinClasses(a, b) {
    var as = a.split(" ");
    for (var i = 0; i < as.length; i++)
      { if (as[i] && !classTest(as[i]).test(b)) { b += " " + as[i]; } }
    return b
  }

  var selectInput = function(node) { node.select(); };
  if (ios) // Mobile Safari apparently has a bug where select() is broken.
    { selectInput = function(node) { node.selectionStart = 0; node.selectionEnd = node.value.length; }; }
  else if (ie) // Suppress mysterious IE10 errors
    { selectInput = function(node) { try { node.select(); } catch(_e) {} }; }

  function bind(f) {
    var args = Array.prototype.slice.call(arguments, 1);
    return function(){return f.apply(null, args)}
  }

  function copyObj(obj, target, overwrite) {
    if (!target) { target = {}; }
    for (var prop in obj)
      { if (obj.hasOwnProperty(prop) && (overwrite !== false || !target.hasOwnProperty(prop)))
        { target[prop] = obj[prop]; } }
    return target
  }

  // Counts the column offset in a string, taking tabs into account.
  // Used mostly to find indentation.
  function countColumn(string, end, tabSize, startIndex, startValue) {
    if (end == null) {
      end = string.search(/[^\s\u00a0]/);
      if (end == -1) { end = string.length; }
    }
    for (var i = startIndex || 0, n = startValue || 0;;) {
      var nextTab = string.indexOf("\t", i);
      if (nextTab < 0 || nextTab >= end)
        { return n + (end - i) }
      n += nextTab - i;
      n += tabSize - (n % tabSize);
      i = nextTab + 1;
    }
  }

  var Delayed = function() {
    this.id = null;
    this.f = null;
    this.time = 0;
    this.handler = bind(this.onTimeout, this);
  };
  Delayed.prototype.onTimeout = function (self) {
    self.id = 0;
    if (self.time <= +new Date) {
      self.f();
    } else {
      setTimeout(self.handler, self.time - +new Date);
    }
  };
  Delayed.prototype.set = function (ms, f) {
    this.f = f;
    var time = +new Date + ms;
    if (!this.id || time < this.time) {
      clearTimeout(this.id);
      this.id = setTimeout(this.handler, ms);
      this.time = time;
    }
  };

  function indexOf(array, elt) {
    for (var i = 0; i < array.length; ++i)
      { if (array[i] == elt) { return i } }
    return -1
  }

  // Number of pixels added to scroller and sizer to hide scrollbar
  var scrollerGap = 50;

  // Returned or thrown by various protocols to signal 'I'm not
  // handling this'.
  var Pass = {toString: function(){return "CodeMirror.Pass"}};

  // Reused option objects for setSelection & friends
  var sel_dontScroll = {scroll: false}, sel_mouse = {origin: "*mouse"}, sel_move = {origin: "+move"};

  // The inverse of countColumn -- find the offset that corresponds to
  // a particular column.
  function findColumn(string, goal, tabSize) {
    for (var pos = 0, col = 0;;) {
      var nextTab = string.indexOf("\t", pos);
      if (nextTab == -1) { nextTab = string.length; }
      var skipped = nextTab - pos;
      if (nextTab == string.length || col + skipped >= goal)
        { return pos + Math.min(skipped, goal - col) }
      col += nextTab - pos;
      col += tabSize - (col % tabSize);
      pos = nextTab + 1;
      if (col >= goal) { return pos }
    }
  }

  var spaceStrs = [""];
  function spaceStr(n) {
    while (spaceStrs.length <= n)
      { spaceStrs.push(lst(spaceStrs) + " "); }
    return spaceStrs[n]
  }

  function lst(arr) { return arr[arr.length-1] }

  function map(array, f) {
    var out = [];
    for (var i = 0; i < array.length; i++) { out[i] = f(array[i], i); }
    return out
  }

  function insertSorted(array, value, score) {
    var pos = 0, priority = score(value);
    while (pos < array.length && score(array[pos]) <= priority) { pos++; }
    array.splice(pos, 0, value);
  }

  function nothing() {}

  function createObj(base, props) {
    var inst;
    if (Object.create) {
      inst = Object.create(base);
    } else {
      nothing.prototype = base;
      inst = new nothing();
    }
    if (props) { copyObj(props, inst); }
    return inst
  }

  var nonASCIISingleCaseWordChar = /[\u00df\u0587\u0590-\u05f4\u0600-\u06ff\u3040-\u309f\u30a0-\u30ff\u3400-\u4db5\u4e00-\u9fcc\uac00-\ud7af]/;
  function isWordCharBasic(ch) {
    return /\w/.test(ch) || ch > "\x80" &&
      (ch.toUpperCase() != ch.toLowerCase() || nonASCIISingleCaseWordChar.test(ch))
  }
  function isWordChar(ch, helper) {
    if (!helper) { return isWordCharBasic(ch) }
    if (helper.source.indexOf("\\w") > -1 && isWordCharBasic(ch)) { return true }
    return helper.test(ch)
  }

  function isEmpty(obj) {
    for (var n in obj) { if (obj.hasOwnProperty(n) && obj[n]) { return false } }
    return true
  }

  // Extending unicode characters. A series of a non-extending char +
  // any number of extending chars is treated as a single unit as far
  // as editing and measuring is concerned. This is not fully correct,
  // since some scripts/fonts/browsers also treat other configurations
  // of code points as a group.
  var extendingChars = /[\u0300-\u036f\u0483-\u0489\u0591-\u05bd\u05bf\u05c1\u05c2\u05c4\u05c5\u05c7\u0610-\u061a\u064b-\u065e\u0670\u06d6-\u06dc\u06de-\u06e4\u06e7\u06e8\u06ea-\u06ed\u0711\u0730-\u074a\u07a6-\u07b0\u07eb-\u07f3\u0816-\u0819\u081b-\u0823\u0825-\u0827\u0829-\u082d\u0900-\u0902\u093c\u0941-\u0948\u094d\u0951-\u0955\u0962\u0963\u0981\u09bc\u09be\u09c1-\u09c4\u09cd\u09d7\u09e2\u09e3\u0a01\u0a02\u0a3c\u0a41\u0a42\u0a47\u0a48\u0a4b-\u0a4d\u0a51\u0a70\u0a71\u0a75\u0a81\u0a82\u0abc\u0ac1-\u0ac5\u0ac7\u0ac8\u0acd\u0ae2\u0ae3\u0b01\u0b3c\u0b3e\u0b3f\u0b41-\u0b44\u0b4d\u0b56\u0b57\u0b62\u0b63\u0b82\u0bbe\u0bc0\u0bcd\u0bd7\u0c3e-\u0c40\u0c46-\u0c48\u0c4a-\u0c4d\u0c55\u0c56\u0c62\u0c63\u0cbc\u0cbf\u0cc2\u0cc6\u0ccc\u0ccd\u0cd5\u0cd6\u0ce2\u0ce3\u0d3e\u0d41-\u0d44\u0d4d\u0d57\u0d62\u0d63\u0dca\u0dcf\u0dd2-\u0dd4\u0dd6\u0ddf\u0e31\u0e34-\u0e3a\u0e47-\u0e4e\u0eb1\u0eb4-\u0eb9\u0ebb\u0ebc\u0ec8-\u0ecd\u0f18\u0f19\u0f35\u0f37\u0f39\u0f71-\u0f7e\u0f80-\u0f84\u0f86\u0f87\u0f90-\u0f97\u0f99-\u0fbc\u0fc6\u102d-\u1030\u1032-\u1037\u1039\u103a\u103d\u103e\u1058\u1059\u105e-\u1060\u1071-\u1074\u1082\u1085\u1086\u108d\u109d\u135f\u1712-\u1714\u1732-\u1734\u1752\u1753\u1772\u1773\u17b7-\u17bd\u17c6\u17c9-\u17d3\u17dd\u180b-\u180d\u18a9\u1920-\u1922\u1927\u1928\u1932\u1939-\u193b\u1a17\u1a18\u1a56\u1a58-\u1a5e\u1a60\u1a62\u1a65-\u1a6c\u1a73-\u1a7c\u1a7f\u1b00-\u1b03\u1b34\u1b36-\u1b3a\u1b3c\u1b42\u1b6b-\u1b73\u1b80\u1b81\u1ba2-\u1ba5\u1ba8\u1ba9\u1c2c-\u1c33\u1c36\u1c37\u1cd0-\u1cd2\u1cd4-\u1ce0\u1ce2-\u1ce8\u1ced\u1dc0-\u1de6\u1dfd-\u1dff\u200c\u200d\u20d0-\u20f0\u2cef-\u2cf1\u2de0-\u2dff\u302a-\u302f\u3099\u309a\ua66f-\ua672\ua67c\ua67d\ua6f0\ua6f1\ua802\ua806\ua80b\ua825\ua826\ua8c4\ua8e0-\ua8f1\ua926-\ua92d\ua947-\ua951\ua980-\ua982\ua9b3\ua9b6-\ua9b9\ua9bc\uaa29-\uaa2e\uaa31\uaa32\uaa35\uaa36\uaa43\uaa4c\uaab0\uaab2-\uaab4\uaab7\uaab8\uaabe\uaabf\uaac1\uabe5\uabe8\uabed\udc00-\udfff\ufb1e\ufe00-\ufe0f\ufe20-\ufe26\uff9e\uff9f]/;
  function isExtendingChar(ch) { return ch.charCodeAt(0) >= 768 && extendingChars.test(ch) }

  // Returns a number from the range [`0`; `str.length`] unless `pos` is outside that range.
  function skipExtendingChars(str, pos, dir) {
    while ((dir < 0 ? pos > 0 : pos < str.length) && isExtendingChar(str.charAt(pos))) { pos += dir; }
    return pos
  }

  // Returns the value from the range [`from`; `to`] that satisfies
  // `pred` and is closest to `from`. Assumes that at least `to`
  // satisfies `pred`. Supports `from` being greater than `to`.
  function findFirst(pred, from, to) {
    // At any point we are certain `to` satisfies `pred`, don't know
    // whether `from` does.
    var dir = from > to ? -1 : 1;
    for (;;) {
      if (from == to) { return from }
      var midF = (from + to) / 2, mid = dir < 0 ? Math.ceil(midF) : Math.floor(midF);
      if (mid == from) { return pred(mid) ? from : to }
      if (pred(mid)) { to = mid; }
      else { from = mid + dir; }
    }
  }

  // BIDI HELPERS

  function iterateBidiSections(order, from, to, f) {
    if (!order) { return f(from, to, "ltr", 0) }
    var found = false;
    for (var i = 0; i < order.length; ++i) {
      var part = order[i];
      if (part.from < to && part.to > from || from == to && part.to == from) {
        f(Math.max(part.from, from), Math.min(part.to, to), part.level == 1 ? "rtl" : "ltr", i);
        found = true;
      }
    }
    if (!found) { f(from, to, "ltr"); }
  }

  var bidiOther = null;
  function getBidiPartAt(order, ch, sticky) {
    var found;
    bidiOther = null;
    for (var i = 0; i < order.length; ++i) {
      var cur = order[i];
      if (cur.from < ch && cur.to > ch) { return i }
      if (cur.to == ch) {
        if (cur.from != cur.to && sticky == "before") { found = i; }
        else { bidiOther = i; }
      }
      if (cur.from == ch) {
        if (cur.from != cur.to && sticky != "before") { found = i; }
        else { bidiOther = i; }
      }
    }
    return found != null ? found : bidiOther
  }

  // Bidirectional ordering algorithm
  // See http://unicode.org/reports/tr9/tr9-13.html for the algorithm
  // that this (partially) implements.

  // One-char codes used for character types:
  // L (L):   Left-to-Right
  // R (R):   Right-to-Left
  // r (AL):  Right-to-Left Arabic
  // 1 (EN):  European Number
  // + (ES):  European Number Separator
  // % (ET):  European Number Terminator
  // n (AN):  Arabic Number
  // , (CS):  Common Number Separator
  // m (NSM): Non-Spacing Mark
  // b (BN):  Boundary Neutral
  // s (B):   Paragraph Separator
  // t (S):   Segment Separator
  // w (WS):  Whitespace
  // N (ON):  Other Neutrals

  // Returns null if characters are ordered as they appear
  // (left-to-right), or an array of sections ({from, to, level}
  // objects) in the order in which they occur visually.
  var bidiOrdering = (function() {
    // Character types for codepoints 0 to 0xff
    var lowTypes = "bbbbbbbbbtstwsbbbbbbbbbbbbbbssstwNN%%%NNNNNN,N,N1111111111NNNNNNNLLLLLLLLLLLLLLLLLLLLLLLLLLNNNNNNLLLLLLLLLLLLLLLLLLLLLLLLLLNNNNbbbbbbsbbbbbbbbbbbbbbbbbbbbbbbbbb,N%%%%NNNNLNNNNN%%11NLNNN1LNNNNNLLLLLLLLLLLLLLLLLLLLLLLNLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLN";
    // Character types for codepoints 0x600 to 0x6f9
    var arabicTypes = "nnnnnnNNr%%r,rNNmmmmmmmmmmmrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrmmmmmmmmmmmmmmmmmmmmmnnnnnnnnnn%nnrrrmrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrmmmmmmmnNmmmmmmrrmmNmmmmrr1111111111";
    function charType(code) {
      if (code <= 0xf7) { return lowTypes.charAt(code) }
      else if (0x590 <= code && code <= 0x5f4) { return "R" }
      else if (0x600 <= code && code <= 0x6f9) { return arabicTypes.charAt(code - 0x600) }
      else if (0x6ee <= code && code <= 0x8ac) { return "r" }
      else if (0x2000 <= code && code <= 0x200b) { return "w" }
      else if (code == 0x200c) { return "b" }
      else { return "L" }
    }

    var bidiRE = /[\u0590-\u05f4\u0600-\u06ff\u0700-\u08ac]/;
    var isNeutral = /[stwN]/, isStrong = /[LRr]/, countsAsLeft = /[Lb1n]/, countsAsNum = /[1n]/;

    function BidiSpan(level, from, to) {
      this.level = level;
      this.from = from; this.to = to;
    }

    return function(str, direction) {
      var outerType = direction == "ltr" ? "L" : "R";

      if (str.length == 0 || direction == "ltr" && !bidiRE.test(str)) { return false }
      var len = str.length, types = [];
      for (var i = 0; i < len; ++i)
        { types.push(charType(str.charCodeAt(i))); }

      // W1. Examine each non-spacing mark (NSM) in the level run, and
      // change the type of the NSM to the type of the previous
      // character. If the NSM is at the start of the level run, it will
      // get the type of sor.
      for (var i$1 = 0, prev = outerType; i$1 < len; ++i$1) {
        var type = types[i$1];
        if (type == "m") { types[i$1] = prev; }
        else { prev = type; }
      }

      // W2. Search backwards from each instance of a European number
      // until the first strong type (R, L, AL, or sor) is found. If an
      // AL is found, change the type of the European number to Arabic
      // number.
      // W3. Change all ALs to R.
      for (var i$2 = 0, cur = outerType; i$2 < len; ++i$2) {
        var type$1 = types[i$2];
        if (type$1 == "1" && cur == "r") { types[i$2] = "n"; }
        else if (isStrong.test(type$1)) { cur = type$1; if (type$1 == "r") { types[i$2] = "R"; } }
      }

      // W4. A single European separator between two European numbers
      // changes to a European number. A single common separator between
      // two numbers of the same type changes to that type.
      for (var i$3 = 1, prev$1 = types[0]; i$3 < len - 1; ++i$3) {
        var type$2 = types[i$3];
        if (type$2 == "+" && prev$1 == "1" && types[i$3+1] == "1") { types[i$3] = "1"; }
        else if (type$2 == "," && prev$1 == types[i$3+1] &&
                 (prev$1 == "1" || prev$1 == "n")) { types[i$3] = prev$1; }
        prev$1 = type$2;
      }

      // W5. A sequence of European terminators adjacent to European
      // numbers changes to all European numbers.
      // W6. Otherwise, separators and terminators change to Other
      // Neutral.
      for (var i$4 = 0; i$4 < len; ++i$4) {
        var type$3 = types[i$4];
        if (type$3 == ",") { types[i$4] = "N"; }
        else if (type$3 == "%") {
          var end = (void 0);
          for (end = i$4 + 1; end < len && types[end] == "%"; ++end) {}
          var replace = (i$4 && types[i$4-1] == "!") || (end < len && types[end] == "1") ? "1" : "N";
          for (var j = i$4; j < end; ++j) { types[j] = replace; }
          i$4 = end - 1;
        }
      }

      // W7. Search backwards from each instance of a European number
      // until the first strong type (R, L, or sor) is found. If an L is
      // found, then change the type of the European number to L.
      for (var i$5 = 0, cur$1 = outerType; i$5 < len; ++i$5) {
        var type$4 = types[i$5];
        if (cur$1 == "L" && type$4 == "1") { types[i$5] = "L"; }
        else if (isStrong.test(type$4)) { cur$1 = type$4; }
      }

      // N1. A sequence of neutrals takes the direction of the
      // surrounding strong text if the text on both sides has the same
      // direction. European and Arabic numbers act as if they were R in
      // terms of their influence on neutrals. Start-of-level-run (sor)
      // and end-of-level-run (eor) are used at level run boundaries.
      // N2. Any remaining neutrals take the embedding direction.
      for (var i$6 = 0; i$6 < len; ++i$6) {
        if (isNeutral.test(types[i$6])) {
          var end$1 = (void 0);
          for (end$1 = i$6 + 1; end$1 < len && isNeutral.test(types[end$1]); ++end$1) {}
          var before = (i$6 ? types[i$6-1] : outerType) == "L";
          var after = (end$1 < len ? types[end$1] : outerType) == "L";
          var replace$1 = before == after ? (before ? "L" : "R") : outerType;
          for (var j$1 = i$6; j$1 < end$1; ++j$1) { types[j$1] = replace$1; }
          i$6 = end$1 - 1;
        }
      }

      // Here we depart from the documented algorithm, in order to avoid
      // building up an actual levels array. Since there are only three
      // levels (0, 1, 2) in an implementation that doesn't take
      // explicit embedding into account, we can build up the order on
      // the fly, without following the level-based algorithm.
      var order = [], m;
      for (var i$7 = 0; i$7 < len;) {
        if (countsAsLeft.test(types[i$7])) {
          var start = i$7;
          for (++i$7; i$7 < len && countsAsLeft.test(types[i$7]); ++i$7) {}
          order.push(new BidiSpan(0, start, i$7));
        } else {
          var pos = i$7, at = order.length, isRTL = direction == "rtl" ? 1 : 0;
          for (++i$7; i$7 < len && types[i$7] != "L"; ++i$7) {}
          for (var j$2 = pos; j$2 < i$7;) {
            if (countsAsNum.test(types[j$2])) {
              if (pos < j$2) { order.splice(at, 0, new BidiSpan(1, pos, j$2)); at += isRTL; }
              var nstart = j$2;
              for (++j$2; j$2 < i$7 && countsAsNum.test(types[j$2]); ++j$2) {}
              order.splice(at, 0, new BidiSpan(2, nstart, j$2));
              at += isRTL;
              pos = j$2;
            } else { ++j$2; }
          }
          if (pos < i$7) { order.splice(at, 0, new BidiSpan(1, pos, i$7)); }
        }
      }
      if (direction == "ltr") {
        if (order[0].level == 1 && (m = str.match(/^\s+/))) {
          order[0].from = m[0].length;
          order.unshift(new BidiSpan(0, 0, m[0].length));
        }
        if (lst(order).level == 1 && (m = str.match(/\s+$/))) {
          lst(order).to -= m[0].length;
          order.push(new BidiSpan(0, len - m[0].length, len));
        }
      }

      return direction == "rtl" ? order.reverse() : order
    }
  })();

  // Get the bidi ordering for the given line (and cache it). Returns
  // false for lines that are fully left-to-right, and an array of
  // BidiSpan objects otherwise.
  function getOrder(line, direction) {
    var order = line.order;
    if (order == null) { order = line.order = bidiOrdering(line.text, direction); }
    return order
  }

  // EVENT HANDLING

  // Lightweight event framework. on/off also work on DOM nodes,
  // registering native DOM handlers.

  var noHandlers = [];

  var on = function(emitter, type, f) {
    if (emitter.addEventListener) {
      emitter.addEventListener(type, f, false);
    } else if (emitter.attachEvent) {
      emitter.attachEvent("on" + type, f);
    } else {
      var map = emitter._handlers || (emitter._handlers = {});
      map[type] = (map[type] || noHandlers).concat(f);
    }
  };

  function getHandlers(emitter, type) {
    return emitter._handlers && emitter._handlers[type] || noHandlers
  }

  function off(emitter, type, f) {
    if (emitter.removeEventListener) {
      emitter.removeEventListener(type, f, false);
    } else if (emitter.detachEvent) {
      emitter.detachEvent("on" + type, f);
    } else {
      var map = emitter._handlers, arr = map && map[type];
      if (arr) {
        var index = indexOf(arr, f);
        if (index > -1)
          { map[type] = arr.slice(0, index).concat(arr.slice(index + 1)); }
      }
    }
  }

  function signal(emitter, type /*, values...*/) {
    var handlers = getHandlers(emitter, type);
    if (!handlers.length) { return }
    var args = Array.prototype.slice.call(arguments, 2);
    for (var i = 0; i < handlers.length; ++i) { handlers[i].apply(null, args); }
  }

  // The DOM events that CodeMirror handles can be overridden by
  // registering a (non-DOM) handler on the editor for the event name,
  // and preventDefault-ing the event in that handler.
  function signalDOMEvent(cm, e, override) {
    if (typeof e == "string")
      { e = {type: e, preventDefault: function() { this.defaultPrevented = true; }}; }
    signal(cm, override || e.type, cm, e);
    return e_defaultPrevented(e) || e.codemirrorIgnore
  }

  function signalCursorActivity(cm) {
    var arr = cm._handlers && cm._handlers.cursorActivity;
    if (!arr) { return }
    var set = cm.curOp.cursorActivityHandlers || (cm.curOp.cursorActivityHandlers = []);
    for (var i = 0; i < arr.length; ++i) { if (indexOf(set, arr[i]) == -1)
      { set.push(arr[i]); } }
  }

  function hasHandler(emitter, type) {
    return getHandlers(emitter, type).length > 0
  }

  // Add on and off methods to a constructor's prototype, to make
  // registering events on such objects more convenient.
  function eventMixin(ctor) {
    ctor.prototype.on = function(type, f) {on(this, type, f);};
    ctor.prototype.off = function(type, f) {off(this, type, f);};
  }

  // Due to the fact that we still support jurassic IE versions, some
  // compatibility wrappers are needed.

  function e_preventDefault(e) {
    if (e.preventDefault) { e.preventDefault(); }
    else { e.returnValue = false; }
  }
  function e_stopPropagation(e) {
    if (e.stopPropagation) { e.stopPropagation(); }
    else { e.cancelBubble = true; }
  }
  function e_defaultPrevented(e) {
    return e.defaultPrevented != null ? e.defaultPrevented : e.returnValue == false
  }
  function e_stop(e) {e_preventDefault(e); e_stopPropagation(e);}

  function e_target(e) {return e.target || e.srcElement}
  function e_button(e) {
    var b = e.which;
    if (b == null) {
      if (e.button & 1) { b = 1; }
      else if (e.button & 2) { b = 3; }
      else if (e.button & 4) { b = 2; }
    }
    if (mac && e.ctrlKey && b == 1) { b = 3; }
    return b
  }

  // Detect drag-and-drop
  var dragAndDrop = function() {
    // There is *some* kind of drag-and-drop support in IE6-8, but I
    // couldn't get it to work yet.
    if (ie && ie_version < 9) { return false }
    var div = elt('div');
    return "draggable" in div || "dragDrop" in div
  }();

  var zwspSupported;
  function zeroWidthElement(measure) {
    if (zwspSupported == null) {
      var test = elt("span", "\u200b");
      removeChildrenAndAdd(measure, elt("span", [test, document.createTextNode("x")]));
      if (measure.firstChild.offsetHeight != 0)
        { zwspSupported = test.offsetWidth <= 1 && test.offsetHeight > 2 && !(ie && ie_version < 8); }
    }
    var node = zwspSupported ? elt("span", "\u200b") :
      elt("span", "\u00a0", null, "display: inline-block; width: 1px; margin-right: -1px");
    node.setAttribute("cm-text", "");
    return node
  }

  // Feature-detect IE's crummy client rect reporting for bidi text
  var badBidiRects;
  function hasBadBidiRects(measure) {
    if (badBidiRects != null) { return badBidiRects }
    var txt = removeChildrenAndAdd(measure, document.createTextNode("A\u062eA"));
    var r0 = range(txt, 0, 1).getBoundingClientRect();
    var r1 = range(txt, 1, 2).getBoundingClientRect();
    removeChildren(measure);
    if (!r0 || r0.left == r0.right) { return false } // Safari returns null in some cases (#2780)
    return badBidiRects = (r1.right - r0.right < 3)
  }

  // See if "".split is the broken IE version, if so, provide an
  // alternative way to split lines.
  var splitLinesAuto = "\n\nb".split(/\n/).length != 3 ? function (string) {
    var pos = 0, result = [], l = string.length;
    while (pos <= l) {
      var nl = string.indexOf("\n", pos);
      if (nl == -1) { nl = string.length; }
      var line = string.slice(pos, string.charAt(nl - 1) == "\r" ? nl - 1 : nl);
      var rt = line.indexOf("\r");
      if (rt != -1) {
        result.push(line.slice(0, rt));
        pos += rt + 1;
      } else {
        result.push(line);
        pos = nl + 1;
      }
    }
    return result
  } : function (string) { return string.split(/\r\n?|\n/); };

  var hasSelection = window.getSelection ? function (te) {
    try { return te.selectionStart != te.selectionEnd }
    catch(e) { return false }
  } : function (te) {
    var range;
    try {range = te.ownerDocument.selection.createRange();}
    catch(e) {}
    if (!range || range.parentElement() != te) { return false }
    return range.compareEndPoints("StartToEnd", range) != 0
  };

  var hasCopyEvent = (function () {
    var e = elt("div");
    if ("oncopy" in e) { return true }
    e.setAttribute("oncopy", "return;");
    return typeof e.oncopy == "function"
  })();

  var badZoomedRects = null;
  function hasBadZoomedRects(measure) {
    if (badZoomedRects != null) { return badZoomedRects }
    var node = removeChildrenAndAdd(measure, elt("span", "x"));
    var normal = node.getBoundingClientRect();
    var fromRange = range(node, 0, 1).getBoundingClientRect();
    return badZoomedRects = Math.abs(normal.left - fromRange.left) > 1
  }

  // Known modes, by name and by MIME
  var modes = {}, mimeModes = {};

  // Extra arguments are stored as the mode's dependencies, which is
  // used by (legacy) mechanisms like loadmode.js to automatically
  // load a mode. (Preferred mechanism is the require/define calls.)
  function defineMode(name, mode) {
    if (arguments.length > 2)
      { mode.dependencies = Array.prototype.slice.call(arguments, 2); }
    modes[name] = mode;
  }

  function defineMIME(mime, spec) {
    mimeModes[mime] = spec;
  }

  // Given a MIME type, a {name, ...options} config object, or a name
  // string, return a mode config object.
  function resolveMode(spec) {
    if (typeof spec == "string" && mimeModes.hasOwnProperty(spec)) {
      spec = mimeModes[spec];
    } else if (spec && typeof spec.name == "string" && mimeModes.hasOwnProperty(spec.name)) {
      var found = mimeModes[spec.name];
      if (typeof found == "string") { found = {name: found}; }
      spec = createObj(found, spec);
      spec.name = found.name;
    } else if (typeof spec == "string" && /^[\w\-]+\/[\w\-]+\+xml$/.test(spec)) {
      return resolveMode("application/xml")
    } else if (typeof spec == "string" && /^[\w\-]+\/[\w\-]+\+json$/.test(spec)) {
      return resolveMode("application/json")
    }
    if (typeof spec == "string") { return {name: spec} }
    else { return spec || {name: "null"} }
  }

  // Given a mode spec (anything that resolveMode accepts), find and
  // initialize an actual mode object.
  function getMode(options, spec) {
    spec = resolveMode(spec);
    var mfactory = modes[spec.name];
    if (!mfactory) { return getMode(options, "text/plain") }
    var modeObj = mfactory(options, spec);
    if (modeExtensions.hasOwnProperty(spec.name)) {
      var exts = modeExtensions[spec.name];
      for (var prop in exts) {
        if (!exts.hasOwnProperty(prop)) { continue }
        if (modeObj.hasOwnProperty(prop)) { modeObj["_" + prop] = modeObj[prop]; }
        modeObj[prop] = exts[prop];
      }
    }
    modeObj.name = spec.name;
    if (spec.helperType) { modeObj.helperType = spec.helperType; }
    if (spec.modeProps) { for (var prop$1 in spec.modeProps)
      { modeObj[prop$1] = spec.modeProps[prop$1]; } }

    return modeObj
  }

  // This can be used to attach properties to mode objects from
  // outside the actual mode definition.
  var modeExtensions = {};
  function extendMode(mode, properties) {
    var exts = modeExtensions.hasOwnProperty(mode) ? modeExtensions[mode] : (modeExtensions[mode] = {});
    copyObj(properties, exts);
  }

  function copyState(mode, state) {
    if (state === true) { return state }
    if (mode.copyState) { return mode.copyState(state) }
    var nstate = {};
    for (var n in state) {
      var val = state[n];
      if (val instanceof Array) { val = val.concat([]); }
      nstate[n] = val;
    }
    return nstate
  }

  // Given a mode and a state (for that mode), find the inner mode and
  // state at the position that the state refers to.
  function innerMode(mode, state) {
    var info;
    while (mode.innerMode) {
      info = mode.innerMode(state);
      if (!info || info.mode == mode) { break }
      state = info.state;
      mode = info.mode;
    }
    return info || {mode: mode, state: state}
  }

  function startState(mode, a1, a2) {
    return mode.startState ? mode.startState(a1, a2) : true
  }

  // STRING STREAM

  // Fed to the mode parsers, provides helper functions to make
  // parsers more succinct.

  var StringStream = function(string, tabSize, lineOracle) {
    this.pos = this.start = 0;
    this.string = string;
    this.tabSize = tabSize || 8;
    this.lastColumnPos = this.lastColumnValue = 0;
    this.lineStart = 0;
    this.lineOracle = lineOracle;
  };

  StringStream.prototype.eol = function () {return this.pos >= this.string.length};
  StringStream.prototype.sol = function () {return this.pos == this.lineStart};
  StringStream.prototype.peek = function () {return this.string.charAt(this.pos) || undefined};
  StringStream.prototype.next = function () {
    if (this.pos < this.string.length)
      { return this.string.charAt(this.pos++) }
  };
  StringStream.prototype.eat = function (match) {
    var ch = this.string.charAt(this.pos);
    var ok;
    if (typeof match == "string") { ok = ch == match; }
    else { ok = ch && (match.test ? match.test(ch) : match(ch)); }
    if (ok) {++this.pos; return ch}
  };
  StringStream.prototype.eatWhile = function (match) {
    var start = this.pos;
    while (this.eat(match)){}
    return this.pos > start
  };
  StringStream.prototype.eatSpace = function () {
    var start = this.pos;
    while (/[\s\u00a0]/.test(this.string.charAt(this.pos))) { ++this.pos; }
    return this.pos > start
  };
  StringStream.prototype.skipToEnd = function () {this.pos = this.string.length;};
  StringStream.prototype.skipTo = function (ch) {
    var found = this.string.indexOf(ch, this.pos);
    if (found > -1) {this.pos = found; return true}
  };
  StringStream.prototype.backUp = function (n) {this.pos -= n;};
  StringStream.prototype.column = function () {
    if (this.lastColumnPos < this.start) {
      this.lastColumnValue = countColumn(this.string, this.start, this.tabSize, this.lastColumnPos, this.lastColumnValue);
      this.lastColumnPos = this.start;
    }
    return this.lastColumnValue - (this.lineStart ? countColumn(this.string, this.lineStart, this.tabSize) : 0)
  };
  StringStream.prototype.indentation = function () {
    return countColumn(this.string, null, this.tabSize) -
      (this.lineStart ? countColumn(this.string, this.lineStart, this.tabSize) : 0)
  };
  StringStream.prototype.match = function (pattern, consume, caseInsensitive) {
    if (typeof pattern == "string") {
      var cased = function (str) { return caseInsensitive ? str.toLowerCase() : str; };
      var substr = this.string.substr(this.pos, pattern.length);
      if (cased(substr) == cased(pattern)) {
        if (consume !== false) { this.pos += pattern.length; }
        return true
      }
    } else {
      var match = this.string.slice(this.pos).match(pattern);
      if (match && match.index > 0) { return null }
      if (match && consume !== false) { this.pos += match[0].length; }
      return match
    }
  };
  StringStream.prototype.current = function (){return this.string.slice(this.start, this.pos)};
  StringStream.prototype.hideFirstChars = function (n, inner) {
    this.lineStart += n;
    try { return inner() }
    finally { this.lineStart -= n; }
  };
  StringStream.prototype.lookAhead = function (n) {
    var oracle = this.lineOracle;
    return oracle && oracle.lookAhead(n)
  };
  StringStream.prototype.baseToken = function () {
    var oracle = this.lineOracle;
    return oracle && oracle.baseToken(this.pos)
  };

  // Find the line object corresponding to the given line number.
  function getLine(doc, n) {
    n -= doc.first;
    if (n < 0 || n >= doc.size) { throw new Error("There is no line " + (n + doc.first) + " in the document.") }
    var chunk = doc;
    while (!chunk.lines) {
      for (var i = 0;; ++i) {
        var child = chunk.children[i], sz = child.chunkSize();
        if (n < sz) { chunk = child; break }
        n -= sz;
      }
    }
    return chunk.lines[n]
  }

  // Get the part of a document between two positions, as an array of
  // strings.
  function getBetween(doc, start, end) {
    var out = [], n = start.line;
    doc.iter(start.line, end.line + 1, function (line) {
      var text = line.text;
      if (n == end.line) { text = text.slice(0, end.ch); }
      if (n == start.line) { text = text.slice(start.ch); }
      out.push(text);
      ++n;
    });
    return out
  }
  // Get the lines between from and to, as array of strings.
  function getLines(doc, from, to) {
    var out = [];
    doc.iter(from, to, function (line) { out.push(line.text); }); // iter aborts when callback returns truthy value
    return out
  }

  // Update the height of a line, propagating the height change
  // upwards to parent nodes.
  function updateLineHeight(line, height) {
    var diff = height - line.height;
    if (diff) { for (var n = line; n; n = n.parent) { n.height += diff; } }
  }

  // Given a line object, find its line number by walking up through
  // its parent links.
  function lineNo(line) {
    if (line.parent == null) { return null }
    var cur = line.parent, no = indexOf(cur.lines, line);
    for (var chunk = cur.parent; chunk; cur = chunk, chunk = chunk.parent) {
      for (var i = 0;; ++i) {
        if (chunk.children[i] == cur) { break }
        no += chunk.children[i].chunkSize();
      }
    }
    return no + cur.first
  }

  // Find the line at the given vertical position, using the height
  // information in the document tree.
  function lineAtHeight(chunk, h) {
    var n = chunk.first;
    outer: do {
      for (var i$1 = 0; i$1 < chunk.children.length; ++i$1) {
        var child = chunk.children[i$1], ch = child.height;
        if (h < ch) { chunk = child; continue outer }
        h -= ch;
        n += child.chunkSize();
      }
      return n
    } while (!chunk.lines)
    var i = 0;
    for (; i < chunk.lines.length; ++i) {
      var line = chunk.lines[i], lh = line.height;
      if (h < lh) { break }
      h -= lh;
    }
    return n + i
  }

  function isLine(doc, l) {return l >= doc.first && l < doc.first + doc.size}

  function lineNumberFor(options, i) {
    return String(options.lineNumberFormatter(i + options.firstLineNumber))
  }

  // A Pos instance represents a position within the text.
  function Pos(line, ch, sticky) {
    if ( sticky === void 0 ) sticky = null;

    if (!(this instanceof Pos)) { return new Pos(line, ch, sticky) }
    this.line = line;
    this.ch = ch;
    this.sticky = sticky;
  }

  // Compare two positions, return 0 if they are the same, a negative
  // number when a is less, and a positive number otherwise.
  function cmp(a, b) { return a.line - b.line || a.ch - b.ch }

  function equalCursorPos(a, b) { return a.sticky == b.sticky && cmp(a, b) == 0 }

  function copyPos(x) {return Pos(x.line, x.ch)}
  function maxPos(a, b) { return cmp(a, b) < 0 ? b : a }
  function minPos(a, b) { return cmp(a, b) < 0 ? a : b }

  // Most of the external API clips given positions to make sure they
  // actually exist within the document.
  function clipLine(doc, n) {return Math.max(doc.first, Math.min(n, doc.first + doc.size - 1))}
  function clipPos(doc, pos) {
    if (pos.line < doc.first) { return Pos(doc.first, 0) }
    var last = doc.first + doc.size - 1;
    if (pos.line > last) { return Pos(last, getLine(doc, last).text.length) }
    return clipToLen(pos, getLine(doc, pos.line).text.length)
  }
  function clipToLen(pos, linelen) {
    var ch = pos.ch;
    if (ch == null || ch > linelen) { return Pos(pos.line, linelen) }
    else if (ch < 0) { return Pos(pos.line, 0) }
    else { return pos }
  }
  function clipPosArray(doc, array) {
    var out = [];
    for (var i = 0; i < array.length; i++) { out[i] = clipPos(doc, array[i]); }
    return out
  }

  var SavedContext = function(state, lookAhead) {
    this.state = state;
    this.lookAhead = lookAhead;
  };

  var Context = function(doc, state, line, lookAhead) {
    this.state = state;
    this.doc = doc;
    this.line = line;
    this.maxLookAhead = lookAhead || 0;
    this.baseTokens = null;
    this.baseTokenPos = 1;
  };

  Context.prototype.lookAhead = function (n) {
    var line = this.doc.getLine(this.line + n);
    if (line != null && n > this.maxLookAhead) { this.maxLookAhead = n; }
    return line
  };

  Context.prototype.baseToken = function (n) {
    if (!this.baseTokens) { return null }
    while (this.baseTokens[this.baseTokenPos] <= n)
      { this.baseTokenPos += 2; }
    var type = this.baseTokens[this.baseTokenPos + 1];
    return {type: type && type.replace(/( |^)overlay .*/, ""),
            size: this.baseTokens[this.baseTokenPos] - n}
  };

  Context.prototype.nextLine = function () {
    this.line++;
    if (this.maxLookAhead > 0) { this.maxLookAhead--; }
  };

  Context.fromSaved = function (doc, saved, line) {
    if (saved instanceof SavedContext)
      { return new Context(doc, copyState(doc.mode, saved.state), line, saved.lookAhead) }
    else
      { return new Context(doc, copyState(doc.mode, saved), line) }
  };

  Context.prototype.save = function (copy) {
    var state = copy !== false ? copyState(this.doc.mode, this.state) : this.state;
    return this.maxLookAhead > 0 ? new SavedContext(state, this.maxLookAhead) : state
  };


  // Compute a style array (an array starting with a mode generation
  // -- for invalidation -- followed by pairs of end positions and
  // style strings), which is used to highlight the tokens on the
  // line.
  function highlightLine(cm, line, context, forceToEnd) {
    // A styles array always starts with a number identifying the
    // mode/overlays that it is based on (for easy invalidation).
    var st = [cm.state.modeGen], lineClasses = {};
    // Compute the base array of styles
    runMode(cm, line.text, cm.doc.mode, context, function (end, style) { return st.push(end, style); },
            lineClasses, forceToEnd);
    var state = context.state;

    // Run overlays, adjust style array.
    var loop = function ( o ) {
      context.baseTokens = st;
      var overlay = cm.state.overlays[o], i = 1, at = 0;
      context.state = true;
      runMode(cm, line.text, overlay.mode, context, function (end, style) {
        var start = i;
        // Ensure there's a token end at the current position, and that i points at it
        while (at < end) {
          var i_end = st[i];
          if (i_end > end)
            { st.splice(i, 1, end, st[i+1], i_end); }
          i += 2;
          at = Math.min(end, i_end);
        }
        if (!style) { return }
        if (overlay.opaque) {
          st.splice(start, i - start, end, "overlay " + style);
          i = start + 2;
        } else {
          for (; start < i; start += 2) {
            var cur = st[start+1];
            st[start+1] = (cur ? cur + " " : "") + "overlay " + style;
          }
        }
      }, lineClasses);
      context.state = state;
      context.baseTokens = null;
      context.baseTokenPos = 1;
    };

    for (var o = 0; o < cm.state.overlays.length; ++o) loop( o );

    return {styles: st, classes: lineClasses.bgClass || lineClasses.textClass ? lineClasses : null}
  }

  function getLineStyles(cm, line, updateFrontier) {
    if (!line.styles || line.styles[0] != cm.state.modeGen) {
      var context = getContextBefore(cm, lineNo(line));
      var resetState = line.text.length > cm.options.maxHighlightLength && copyState(cm.doc.mode, context.state);
      var result = highlightLine(cm, line, context);
      if (resetState) { context.state = resetState; }
      line.stateAfter = context.save(!resetState);
      line.styles = result.styles;
      if (result.classes) { line.styleClasses = result.classes; }
      else if (line.styleClasses) { line.styleClasses = null; }
      if (updateFrontier === cm.doc.highlightFrontier)
        { cm.doc.modeFrontier = Math.max(cm.doc.modeFrontier, ++cm.doc.highlightFrontier); }
    }
    return line.styles
  }

  function getContextBefore(cm, n, precise) {
    var doc = cm.doc, display = cm.display;
    if (!doc.mode.startState) { return new Context(doc, true, n) }
    var start = findStartLine(cm, n, precise);
    var saved = start > doc.first && getLine(doc, start - 1).stateAfter;
    var context = saved ? Context.fromSaved(doc, saved, start) : new Context(doc, startState(doc.mode), start);

    doc.iter(start, n, function (line) {
      processLine(cm, line.text, context);
      var pos = context.line;
      line.stateAfter = pos == n - 1 || pos % 5 == 0 || pos >= display.viewFrom && pos < display.viewTo ? context.save() : null;
      context.nextLine();
    });
    if (precise) { doc.modeFrontier = context.line; }
    return context
  }

  // Lightweight form of highlight -- proceed over this line and
  // update state, but don't save a style array. Used for lines that
  // aren't currently visible.
  function processLine(cm, text, context, startAt) {
    var mode = cm.doc.mode;
    var stream = new StringStream(text, cm.options.tabSize, context);
    stream.start = stream.pos = startAt || 0;
    if (text == "") { callBlankLine(mode, context.state); }
    while (!stream.eol()) {
      readToken(mode, stream, context.state);
      stream.start = stream.pos;
    }
  }

  function callBlankLine(mode, state) {
    if (mode.blankLine) { return mode.blankLine(state) }
    if (!mode.innerMode) { return }
    var inner = innerMode(mode, state);
    if (inner.mode.blankLine) { return inner.mode.blankLine(inner.state) }
  }

  function readToken(mode, stream, state, inner) {
    for (var i = 0; i < 10; i++) {
      if (inner) { inner[0] = innerMode(mode, state).mode; }
      var style = mode.token(stream, state);
      if (stream.pos > stream.start) { return style }
    }
    throw new Error("Mode " + mode.name + " failed to advance stream.")
  }

  var Token = function(stream, type, state) {
    this.start = stream.start; this.end = stream.pos;
    this.string = stream.current();
    this.type = type || null;
    this.state = state;
  };

  // Utility for getTokenAt and getLineTokens
  function takeToken(cm, pos, precise, asArray) {
    var doc = cm.doc, mode = doc.mode, style;
    pos = clipPos(doc, pos);
    var line = getLine(doc, pos.line), context = getContextBefore(cm, pos.line, precise);
    var stream = new StringStream(line.text, cm.options.tabSize, context), tokens;
    if (asArray) { tokens = []; }
    while ((asArray || stream.pos < pos.ch) && !stream.eol()) {
      stream.start = stream.pos;
      style = readToken(mode, stream, context.state);
      if (asArray) { tokens.push(new Token(stream, style, copyState(doc.mode, context.state))); }
    }
    return asArray ? tokens : new Token(stream, style, context.state)
  }

  function extractLineClasses(type, output) {
    if (type) { for (;;) {
      var lineClass = type.match(/(?:^|\s+)line-(background-)?(\S+)/);
      if (!lineClass) { break }
      type = type.slice(0, lineClass.index) + type.slice(lineClass.index + lineClass[0].length);
      var prop = lineClass[1] ? "bgClass" : "textClass";
      if (output[prop] == null)
        { output[prop] = lineClass[2]; }
      else if (!(new RegExp("(?:^|\\s)" + lineClass[2] + "(?:$|\\s)")).test(output[prop]))
        { output[prop] += " " + lineClass[2]; }
    } }
    return type
  }

  // Run the given mode's parser over a line, calling f for each token.
  function runMode(cm, text, mode, context, f, lineClasses, forceToEnd) {
    var flattenSpans = mode.flattenSpans;
    if (flattenSpans == null) { flattenSpans = cm.options.flattenSpans; }
    var curStart = 0, curStyle = null;
    var stream = new StringStream(text, cm.options.tabSize, context), style;
    var inner = cm.options.addModeClass && [null];
    if (text == "") { extractLineClasses(callBlankLine(mode, context.state), lineClasses); }
    while (!stream.eol()) {
      if (stream.pos > cm.options.maxHighlightLength) {
        flattenSpans = false;
        if (forceToEnd) { processLine(cm, text, context, stream.pos); }
        stream.pos = text.length;
        style = null;
      } else {
        style = extractLineClasses(readToken(mode, stream, context.state, inner), lineClasses);
      }
      if (inner) {
        var mName = inner[0].name;
        if (mName) { style = "m-" + (style ? mName + " " + style : mName); }
      }
      if (!flattenSpans || curStyle != style) {
        while (curStart < stream.start) {
          curStart = Math.min(stream.start, curStart + 5000);
          f(curStart, curStyle);
        }
        curStyle = style;
      }
      stream.start = stream.pos;
    }
    while (curStart < stream.pos) {
      // Webkit seems to refuse to render text nodes longer than 57444
      // characters, and returns inaccurate measurements in nodes
      // starting around 5000 chars.
      var pos = Math.min(stream.pos, curStart + 5000);
      f(pos, curStyle);
      curStart = pos;
    }
  }

  // Finds the line to start with when starting a parse. Tries to
  // find a line with a stateAfter, so that it can start with a
  // valid state. If that fails, it returns the line with the
  // smallest indentation, which tends to need the least context to
  // parse correctly.
  function findStartLine(cm, n, precise) {
    var minindent, minline, doc = cm.doc;
    var lim = precise ? -1 : n - (cm.doc.mode.innerMode ? 1000 : 100);
    for (var search = n; search > lim; --search) {
      if (search <= doc.first) { return doc.first }
      var line = getLine(doc, search - 1), after = line.stateAfter;
      if (after && (!precise || search + (after instanceof SavedContext ? after.lookAhead : 0) <= doc.modeFrontier))
        { return search }
      var indented = countColumn(line.text, null, cm.options.tabSize);
      if (minline == null || minindent > indented) {
        minline = search - 1;
        minindent = indented;
      }
    }
    return minline
  }

  function retreatFrontier(doc, n) {
    doc.modeFrontier = Math.min(doc.modeFrontier, n);
    if (doc.highlightFrontier < n - 10) { return }
    var start = doc.first;
    for (var line = n - 1; line > start; line--) {
      var saved = getLine(doc, line).stateAfter;
      // change is on 3
      // state on line 1 looked ahead 2 -- so saw 3
      // test 1 + 2 < 3 should cover this
      if (saved && (!(saved instanceof SavedContext) || line + saved.lookAhead < n)) {
        start = line + 1;
        break
      }
    }
    doc.highlightFrontier = Math.min(doc.highlightFrontier, start);
  }

  // Optimize some code when these features are not used.
  var sawReadOnlySpans = false, sawCollapsedSpans = false;

  function seeReadOnlySpans() {
    sawReadOnlySpans = true;
  }

  function seeCollapsedSpans() {
    sawCollapsedSpans = true;
  }

  // TEXTMARKER SPANS

  function MarkedSpan(marker, from, to) {
    this.marker = marker;
    this.from = from; this.to = to;
  }

  // Search an array of spans for a span matching the given marker.
  function getMarkedSpanFor(spans, marker) {
    if (spans) { for (var i = 0; i < spans.length; ++i) {
      var span = spans[i];
      if (span.marker == marker) { return span }
    } }
  }
  // Remove a span from an array, returning undefined if no spans are
  // left (we don't store arrays for lines without spans).
  function removeMarkedSpan(spans, span) {
    var r;
    for (var i = 0; i < spans.length; ++i)
      { if (spans[i] != span) { (r || (r = [])).push(spans[i]); } }
    return r
  }
  // Add a span to a line.
  function addMarkedSpan(line, span) {
    line.markedSpans = line.markedSpans ? line.markedSpans.concat([span]) : [span];
    span.marker.attachLine(line);
  }

  // Used for the algorithm that adjusts markers for a change in the
  // document. These functions cut an array of spans at a given
  // character position, returning an array of remaining chunks (or
  // undefined if nothing remains).
  function markedSpansBefore(old, startCh, isInsert) {
    var nw;
    if (old) { for (var i = 0; i < old.length; ++i) {
      var span = old[i], marker = span.marker;
      var startsBefore = span.from == null || (marker.inclusiveLeft ? span.from <= startCh : span.from < startCh);
      if (startsBefore || span.from == startCh && marker.type == "bookmark" && (!isInsert || !span.marker.insertLeft)) {
        var endsAfter = span.to == null || (marker.inclusiveRight ? span.to >= startCh : span.to > startCh)
        ;(nw || (nw = [])).push(new MarkedSpan(marker, span.from, endsAfter ? null : span.to));
      }
    } }
    return nw
  }
  function markedSpansAfter(old, endCh, isInsert) {
    var nw;
    if (old) { for (var i = 0; i < old.length; ++i) {
      var span = old[i], marker = span.marker;
      var endsAfter = span.to == null || (marker.inclusiveRight ? span.to >= endCh : span.to > endCh);
      if (endsAfter || span.from == endCh && marker.type == "bookmark" && (!isInsert || span.marker.insertLeft)) {
        var startsBefore = span.from == null || (marker.inclusiveLeft ? span.from <= endCh : span.from < endCh)
        ;(nw || (nw = [])).push(new MarkedSpan(marker, startsBefore ? null : span.from - endCh,
                                              span.to == null ? null : span.to - endCh));
      }
    } }
    return nw
  }

  // Given a change object, compute the new set of marker spans that
  // cover the line in which the change took place. Removes spans
  // entirely within the change, reconnects spans belonging to the
  // same marker that appear on both sides of the change, and cuts off
  // spans partially within the change. Returns an array of span
  // arrays with one element for each line in (after) the change.
  function stretchSpansOverChange(doc, change) {
    if (change.full) { return null }
    var oldFirst = isLine(doc, change.from.line) && getLine(doc, change.from.line).markedSpans;
    var oldLast = isLine(doc, change.to.line) && getLine(doc, change.to.line).markedSpans;
    if (!oldFirst && !oldLast) { return null }

    var startCh = change.from.ch, endCh = change.to.ch, isInsert = cmp(change.from, change.to) == 0;
    // Get the spans that 'stick out' on both sides
    var first = markedSpansBefore(oldFirst, startCh, isInsert);
    var last = markedSpansAfter(oldLast, endCh, isInsert);

    // Next, merge those two ends
    var sameLine = change.text.length == 1, offset = lst(change.text).length + (sameLine ? startCh : 0);
    if (first) {
      // Fix up .to properties of first
      for (var i = 0; i < first.length; ++i) {
        var span = first[i];
        if (span.to == null) {
          var found = getMarkedSpanFor(last, span.marker);
          if (!found) { span.to = startCh; }
          else if (sameLine) { span.to = found.to == null ? null : found.to + offset; }
        }
      }
    }
    if (last) {
      // Fix up .from in last (or move them into first in case of sameLine)
      for (var i$1 = 0; i$1 < last.length; ++i$1) {
        var span$1 = last[i$1];
        if (span$1.to != null) { span$1.to += offset; }
        if (span$1.from == null) {
          var found$1 = getMarkedSpanFor(first, span$1.marker);
          if (!found$1) {
            span$1.from = offset;
            if (sameLine) { (first || (first = [])).push(span$1); }
          }
        } else {
          span$1.from += offset;
          if (sameLine) { (first || (first = [])).push(span$1); }
        }
      }
    }
    // Make sure we didn't create any zero-length spans
    if (first) { first = clearEmptySpans(first); }
    if (last && last != first) { last = clearEmptySpans(last); }

    var newMarkers = [first];
    if (!sameLine) {
      // Fill gap with whole-line-spans
      var gap = change.text.length - 2, gapMarkers;
      if (gap > 0 && first)
        { for (var i$2 = 0; i$2 < first.length; ++i$2)
          { if (first[i$2].to == null)
            { (gapMarkers || (gapMarkers = [])).push(new MarkedSpan(first[i$2].marker, null, null)); } } }
      for (var i$3 = 0; i$3 < gap; ++i$3)
        { newMarkers.push(gapMarkers); }
      newMarkers.push(last);
    }
    return newMarkers
  }

  // Remove spans that are empty and don't have a clearWhenEmpty
  // option of false.
  function clearEmptySpans(spans) {
    for (var i = 0; i < spans.length; ++i) {
      var span = spans[i];
      if (span.from != null && span.from == span.to && span.marker.clearWhenEmpty !== false)
        { spans.splice(i--, 1); }
    }
    if (!spans.length) { return null }
    return spans
  }

  // Used to 'clip' out readOnly ranges when making a change.
  function removeReadOnlyRanges(doc, from, to) {
    var markers = null;
    doc.iter(from.line, to.line + 1, function (line) {
      if (line.markedSpans) { for (var i = 0; i < line.markedSpans.length; ++i) {
        var mark = line.markedSpans[i].marker;
        if (mark.readOnly && (!markers || indexOf(markers, mark) == -1))
          { (markers || (markers = [])).push(mark); }
      } }
    });
    if (!markers) { return null }
    var parts = [{from: from, to: to}];
    for (var i = 0; i < markers.length; ++i) {
      var mk = markers[i], m = mk.find(0);
      for (var j = 0; j < parts.length; ++j) {
        var p = parts[j];
        if (cmp(p.to, m.from) < 0 || cmp(p.from, m.to) > 0) { continue }
        var newParts = [j, 1], dfrom = cmp(p.from, m.from), dto = cmp(p.to, m.to);
        if (dfrom < 0 || !mk.inclusiveLeft && !dfrom)
          { newParts.push({from: p.from, to: m.from}); }
        if (dto > 0 || !mk.inclusiveRight && !dto)
          { newParts.push({from: m.to, to: p.to}); }
        parts.splice.apply(parts, newParts);
        j += newParts.length - 3;
      }
    }
    return parts
  }

  // Connect or disconnect spans from a line.
  function detachMarkedSpans(line) {
    var spans = line.markedSpans;
    if (!spans) { return }
    for (var i = 0; i < spans.length; ++i)
      { spans[i].marker.detachLine(line); }
    line.markedSpans = null;
  }
  function attachMarkedSpans(line, spans) {
    if (!spans) { return }
    for (var i = 0; i < spans.length; ++i)
      { spans[i].marker.attachLine(line); }
    line.markedSpans = spans;
  }

  // Helpers used when computing which overlapping collapsed span
  // counts as the larger one.
  function extraLeft(marker) { return marker.inclusiveLeft ? -1 : 0 }
  function extraRight(marker) { return marker.inclusiveRight ? 1 : 0 }

  // Returns a number indicating which of two overlapping collapsed
  // spans is larger (and thus includes the other). Falls back to
  // comparing ids when the spans cover exactly the same range.
  function compareCollapsedMarkers(a, b) {
    var lenDiff = a.lines.length - b.lines.length;
    if (lenDiff != 0) { return lenDiff }
    var aPos = a.find(), bPos = b.find();
    var fromCmp = cmp(aPos.from, bPos.from) || extraLeft(a) - extraLeft(b);
    if (fromCmp) { return -fromCmp }
    var toCmp = cmp(aPos.to, bPos.to) || extraRight(a) - extraRight(b);
    if (toCmp) { return toCmp }
    return b.id - a.id
  }

  // Find out whether a line ends or starts in a collapsed span. If
  // so, return the marker for that span.
  function collapsedSpanAtSide(line, start) {
    var sps = sawCollapsedSpans && line.markedSpans, found;
    if (sps) { for (var sp = (void 0), i = 0; i < sps.length; ++i) {
      sp = sps[i];
      if (sp.marker.collapsed && (start ? sp.from : sp.to) == null &&
          (!found || compareCollapsedMarkers(found, sp.marker) < 0))
        { found = sp.marker; }
    } }
    return found
  }
  function collapsedSpanAtStart(line) { return collapsedSpanAtSide(line, true) }
  function collapsedSpanAtEnd(line) { return collapsedSpanAtSide(line, false) }

  function collapsedSpanAround(line, ch) {
    var sps = sawCollapsedSpans && line.markedSpans, found;
    if (sps) { for (var i = 0; i < sps.length; ++i) {
      var sp = sps[i];
      if (sp.marker.collapsed && (sp.from == null || sp.from < ch) && (sp.to == null || sp.to > ch) &&
          (!found || compareCollapsedMarkers(found, sp.marker) < 0)) { found = sp.marker; }
    } }
    return found
  }

  // Test whether there exists a collapsed span that partially
  // overlaps (covers the start or end, but not both) of a new span.
  // Such overlap is not allowed.
  function conflictingCollapsedRange(doc, lineNo, from, to, marker) {
    var line = getLine(doc, lineNo);
    var sps = sawCollapsedSpans && line.markedSpans;
    if (sps) { for (var i = 0; i < sps.length; ++i) {
      var sp = sps[i];
      if (!sp.marker.collapsed) { continue }
      var found = sp.marker.find(0);
      var fromCmp = cmp(found.from, from) || extraLeft(sp.marker) - extraLeft(marker);
      var toCmp = cmp(found.to, to) || extraRight(sp.marker) - extraRight(marker);
      if (fromCmp >= 0 && toCmp <= 0 || fromCmp <= 0 && toCmp >= 0) { continue }
      if (fromCmp <= 0 && (sp.marker.inclusiveRight && marker.inclusiveLeft ? cmp(found.to, from) >= 0 : cmp(found.to, from) > 0) ||
          fromCmp >= 0 && (sp.marker.inclusiveRight && marker.inclusiveLeft ? cmp(found.from, to) <= 0 : cmp(found.from, to) < 0))
        { return true }
    } }
  }

  // A visual line is a line as drawn on the screen. Folding, for
  // example, can cause multiple logical lines to appear on the same
  // visual line. This finds the start of the visual line that the
  // given line is part of (usually that is the line itself).
  function visualLine(line) {
    var merged;
    while (merged = collapsedSpanAtStart(line))
      { line = merged.find(-1, true).line; }
    return line
  }

  function visualLineEnd(line) {
    var merged;
    while (merged = collapsedSpanAtEnd(line))
      { line = merged.find(1, true).line; }
    return line
  }

  // Returns an array of logical lines that continue the visual line
  // started by the argument, or undefined if there are no such lines.
  function visualLineContinued(line) {
    var merged, lines;
    while (merged = collapsedSpanAtEnd(line)) {
      line = merged.find(1, true).line
      ;(lines || (lines = [])).push(line);
    }
    return lines
  }

  // Get the line number of the start of the visual line that the
  // given line number is part of.
  function visualLineNo(doc, lineN) {
    var line = getLine(doc, lineN), vis = visualLine(line);
    if (line == vis) { return lineN }
    return lineNo(vis)
  }

  // Get the line number of the start of the next visual line after
  // the given line.
  function visualLineEndNo(doc, lineN) {
    if (lineN > doc.lastLine()) { return lineN }
    var line = getLine(doc, lineN), merged;
    if (!lineIsHidden(doc, line)) { return lineN }
    while (merged = collapsedSpanAtEnd(line))
      { line = merged.find(1, true).line; }
    return lineNo(line) + 1
  }

  // Compute whether a line is hidden. Lines count as hidden when they
  // are part of a visual line that starts with another line, or when
  // they are entirely covered by collapsed, non-widget span.
  function lineIsHidden(doc, line) {
    var sps = sawCollapsedSpans && line.markedSpans;
    if (sps) { for (var sp = (void 0), i = 0; i < sps.length; ++i) {
      sp = sps[i];
      if (!sp.marker.collapsed) { continue }
      if (sp.from == null) { return true }
      if (sp.marker.widgetNode) { continue }
      if (sp.from == 0 && sp.marker.inclusiveLeft && lineIsHiddenInner(doc, line, sp))
        { return true }
    } }
  }
  function lineIsHiddenInner(doc, line, span) {
    if (span.to == null) {
      var end = span.marker.find(1, true);
      return lineIsHiddenInner(doc, end.line, getMarkedSpanFor(end.line.markedSpans, span.marker))
    }
    if (span.marker.inclusiveRight && span.to == line.text.length)
      { return true }
    for (var sp = (void 0), i = 0; i < line.markedSpans.length; ++i) {
      sp = line.markedSpans[i];
      if (sp.marker.collapsed && !sp.marker.widgetNode && sp.from == span.to &&
          (sp.to == null || sp.to != span.from) &&
          (sp.marker.inclusiveLeft || span.marker.inclusiveRight) &&
          lineIsHiddenInner(doc, line, sp)) { return true }
    }
  }

  // Find the height above the given line.
  function heightAtLine(lineObj) {
    lineObj = visualLine(lineObj);

    var h = 0, chunk = lineObj.parent;
    for (var i = 0; i < chunk.lines.length; ++i) {
      var line = chunk.lines[i];
      if (line == lineObj) { break }
      else { h += line.height; }
    }
    for (var p = chunk.parent; p; chunk = p, p = chunk.parent) {
      for (var i$1 = 0; i$1 < p.children.length; ++i$1) {
        var cur = p.children[i$1];
        if (cur == chunk) { break }
        else { h += cur.height; }
      }
    }
    return h
  }

  // Compute the character length of a line, taking into account
  // collapsed ranges (see markText) that might hide parts, and join
  // other lines onto it.
  function lineLength(line) {
    if (line.height == 0) { return 0 }
    var len = line.text.length, merged, cur = line;
    while (merged = collapsedSpanAtStart(cur)) {
      var found = merged.find(0, true);
      cur = found.from.line;
      len += found.from.ch - found.to.ch;
    }
    cur = line;
    while (merged = collapsedSpanAtEnd(cur)) {
      var found$1 = merged.find(0, true);
      len -= cur.text.length - found$1.from.ch;
      cur = found$1.to.line;
      len += cur.text.length - found$1.to.ch;
    }
    return len
  }

  // Find the longest line in the document.
  function findMaxLine(cm) {
    var d = cm.display, doc = cm.doc;
    d.maxLine = getLine(doc, doc.first);
    d.maxLineLength = lineLength(d.maxLine);
    d.maxLineChanged = true;
    doc.iter(function (line) {
      var len = lineLength(line);
      if (len > d.maxLineLength) {
        d.maxLineLength = len;
        d.maxLine = line;
      }
    });
  }

  // LINE DATA STRUCTURE

  // Line objects. These hold state related to a line, including
  // highlighting info (the styles array).
  var Line = function(text, markedSpans, estimateHeight) {
    this.text = text;
    attachMarkedSpans(this, markedSpans);
    this.height = estimateHeight ? estimateHeight(this) : 1;
  };

  Line.prototype.lineNo = function () { return lineNo(this) };
  eventMixin(Line);

  // Change the content (text, markers) of a line. Automatically
  // invalidates cached information and tries to re-estimate the
  // line's height.
  function updateLine(line, text, markedSpans, estimateHeight) {
    line.text = text;
    if (line.stateAfter) { line.stateAfter = null; }
    if (line.styles) { line.styles = null; }
    if (line.order != null) { line.order = null; }
    detachMarkedSpans(line);
    attachMarkedSpans(line, markedSpans);
    var estHeight = estimateHeight ? estimateHeight(line) : 1;
    if (estHeight != line.height) { updateLineHeight(line, estHeight); }
  }

  // Detach a line from the document tree and its markers.
  function cleanUpLine(line) {
    line.parent = null;
    detachMarkedSpans(line);
  }

  // Convert a style as returned by a mode (either null, or a string
  // containing one or more styles) to a CSS style. This is cached,
  // and also looks for line-wide styles.
  var styleToClassCache = {}, styleToClassCacheWithMode = {};
  function interpretTokenStyle(style, options) {
    if (!style || /^\s*$/.test(style)) { return null }
    var cache = options.addModeClass ? styleToClassCacheWithMode : styleToClassCache;
    return cache[style] ||
      (cache[style] = style.replace(/\S+/g, "cm-$&"))
  }

  // Render the DOM representation of the text of a line. Also builds
  // up a 'line map', which points at the DOM nodes that represent
  // specific stretches of text, and is used by the measuring code.
  // The returned object contains the DOM node, this map, and
  // information about line-wide styles that were set by the mode.
  function buildLineContent(cm, lineView) {
    // The padding-right forces the element to have a 'border', which
    // is needed on Webkit to be able to get line-level bounding
    // rectangles for it (in measureChar).
    var content = eltP("span", null, null, webkit ? "padding-right: .1px" : null);
    var builder = {pre: eltP("pre", [content], "CodeMirror-line"), content: content,
                   col: 0, pos: 0, cm: cm,
                   trailingSpace: false,
                   splitSpaces: cm.getOption("lineWrapping")};
    lineView.measure = {};

    // Iterate over the logical lines that make up this visual line.
    for (var i = 0; i <= (lineView.rest ? lineView.rest.length : 0); i++) {
      var line = i ? lineView.rest[i - 1] : lineView.line, order = (void 0);
      builder.pos = 0;
      builder.addToken = buildToken;
      // Optionally wire in some hacks into the token-rendering
      // algorithm, to deal with browser quirks.
      if (hasBadBidiRects(cm.display.measure) && (order = getOrder(line, cm.doc.direction)))
        { builder.addToken = buildTokenBadBidi(builder.addToken, order); }
      builder.map = [];
      var allowFrontierUpdate = lineView != cm.display.externalMeasured && lineNo(line);
      insertLineContent(line, builder, getLineStyles(cm, line, allowFrontierUpdate));
      if (line.styleClasses) {
        if (line.styleClasses.bgClass)
          { builder.bgClass = joinClasses(line.styleClasses.bgClass, builder.bgClass || ""); }
        if (line.styleClasses.textClass)
          { builder.textClass = joinClasses(line.styleClasses.textClass, builder.textClass || ""); }
      }

      // Ensure at least a single node is present, for measuring.
      if (builder.map.length == 0)
        { builder.map.push(0, 0, builder.content.appendChild(zeroWidthElement(cm.display.measure))); }

      // Store the map and a cache object for the current logical line
      if (i == 0) {
        lineView.measure.map = builder.map;
        lineView.measure.cache = {};
      } else {
  (lineView.measure.maps || (lineView.measure.maps = [])).push(builder.map)
        ;(lineView.measure.caches || (lineView.measure.caches = [])).push({});
      }
    }

    // See issue #2901
    if (webkit) {
      var last = builder.content.lastChild;
      if (/\bcm-tab\b/.test(last.className) || (last.querySelector && last.querySelector(".cm-tab")))
        { builder.content.className = "cm-tab-wrap-hack"; }
    }

    signal(cm, "renderLine", cm, lineView.line, builder.pre);
    if (builder.pre.className)
      { builder.textClass = joinClasses(builder.pre.className, builder.textClass || ""); }

    return builder
  }

  function defaultSpecialCharPlaceholder(ch) {
    var token = elt("span", "\u2022", "cm-invalidchar");
    token.title = "\\u" + ch.charCodeAt(0).toString(16);
    token.setAttribute("aria-label", token.title);
    return token
  }

  // Build up the DOM representation for a single token, and add it to
  // the line map. Takes care to render special characters separately.
  function buildToken(builder, text, style, startStyle, endStyle, css, attributes) {
    if (!text) { return }
    var displayText = builder.splitSpaces ? splitSpaces(text, builder.trailingSpace) : text;
    var special = builder.cm.state.specialChars, mustWrap = false;
    var content;
    if (!special.test(text)) {
      builder.col += text.length;
      content = document.createTextNode(displayText);
      builder.map.push(builder.pos, builder.pos + text.length, content);
      if (ie && ie_version < 9) { mustWrap = true; }
      builder.pos += text.length;
    } else {
      content = document.createDocumentFragment();
      var pos = 0;
      while (true) {
        special.lastIndex = pos;
        var m = special.exec(text);
        var skipped = m ? m.index - pos : text.length - pos;
        if (skipped) {
          var txt = document.createTextNode(displayText.slice(pos, pos + skipped));
          if (ie && ie_version < 9) { content.appendChild(elt("span", [txt])); }
          else { content.appendChild(txt); }
          builder.map.push(builder.pos, builder.pos + skipped, txt);
          builder.col += skipped;
          builder.pos += skipped;
        }
        if (!m) { break }
        pos += skipped + 1;
        var txt$1 = (void 0);
        if (m[0] == "\t") {
          var tabSize = builder.cm.options.tabSize, tabWidth = tabSize - builder.col % tabSize;
          txt$1 = content.appendChild(elt("span", spaceStr(tabWidth), "cm-tab"));
          txt$1.setAttribute("role", "presentation");
          txt$1.setAttribute("cm-text", "\t");
          builder.col += tabWidth;
        } else if (m[0] == "\r" || m[0] == "\n") {
          txt$1 = content.appendChild(elt("span", m[0] == "\r" ? "\u240d" : "\u2424", "cm-invalidchar"));
          txt$1.setAttribute("cm-text", m[0]);
          builder.col += 1;
        } else {
          txt$1 = builder.cm.options.specialCharPlaceholder(m[0]);
          txt$1.setAttribute("cm-text", m[0]);
          if (ie && ie_version < 9) { content.appendChild(elt("span", [txt$1])); }
          else { content.appendChild(txt$1); }
          builder.col += 1;
        }
        builder.map.push(builder.pos, builder.pos + 1, txt$1);
        builder.pos++;
      }
    }
    builder.trailingSpace = displayText.charCodeAt(text.length - 1) == 32;
    if (style || startStyle || endStyle || mustWrap || css || attributes) {
      var fullStyle = style || "";
      if (startStyle) { fullStyle += startStyle; }
      if (endStyle) { fullStyle += endStyle; }
      var token = elt("span", [content], fullStyle, css);
      if (attributes) {
        for (var attr in attributes) { if (attributes.hasOwnProperty(attr) && attr != "style" && attr != "class")
          { token.setAttribute(attr, attributes[attr]); } }
      }
      return builder.content.appendChild(token)
    }
    builder.content.appendChild(content);
  }

  // Change some spaces to NBSP to prevent the browser from collapsing
  // trailing spaces at the end of a line when rendering text (issue #1362).
  function splitSpaces(text, trailingBefore) {
    if (text.length > 1 && !/  /.test(text)) { return text }
    var spaceBefore = trailingBefore, result = "";
    for (var i = 0; i < text.length; i++) {
      var ch = text.charAt(i);
      if (ch == " " && spaceBefore && (i == text.length - 1 || text.charCodeAt(i + 1) == 32))
        { ch = "\u00a0"; }
      result += ch;
      spaceBefore = ch == " ";
    }
    return result
  }

  // Work around nonsense dimensions being reported for stretches of
  // right-to-left text.
  function buildTokenBadBidi(inner, order) {
    return function (builder, text, style, startStyle, endStyle, css, attributes) {
      style = style ? style + " cm-force-border" : "cm-force-border";
      var start = builder.pos, end = start + text.length;
      for (;;) {
        // Find the part that overlaps with the start of this text
        var part = (void 0);
        for (var i = 0; i < order.length; i++) {
          part = order[i];
          if (part.to > start && part.from <= start) { break }
        }
        if (part.to >= end) { return inner(builder, text, style, startStyle, endStyle, css, attributes) }
        inner(builder, text.slice(0, part.to - start), style, startStyle, null, css, attributes);
        startStyle = null;
        text = text.slice(part.to - start);
        start = part.to;
      }
    }
  }

  function buildCollapsedSpan(builder, size, marker, ignoreWidget) {
    var widget = !ignoreWidget && marker.widgetNode;
    if (widget) { builder.map.push(builder.pos, builder.pos + size, widget); }
    if (!ignoreWidget && builder.cm.display.input.needsContentAttribute) {
      if (!widget)
        { widget = builder.content.appendChild(document.createElement("span")); }
      widget.setAttribute("cm-marker", marker.id);
    }
    if (widget) {
      builder.cm.display.input.setUneditable(widget);
      builder.content.appendChild(widget);
    }
    builder.pos += size;
    builder.trailingSpace = false;
  }

  // Outputs a number of spans to make up a line, taking highlighting
  // and marked text into account.
  function insertLineContent(line, builder, styles) {
    var spans = line.markedSpans, allText = line.text, at = 0;
    if (!spans) {
      for (var i$1 = 1; i$1 < styles.length; i$1+=2)
        { builder.addToken(builder, allText.slice(at, at = styles[i$1]), interpretTokenStyle(styles[i$1+1], builder.cm.options)); }
      return
    }

    var len = allText.length, pos = 0, i = 1, text = "", style, css;
    var nextChange = 0, spanStyle, spanEndStyle, spanStartStyle, collapsed, attributes;
    for (;;) {
      if (nextChange == pos) { // Update current marker set
        spanStyle = spanEndStyle = spanStartStyle = css = "";
        attributes = null;
        collapsed = null; nextChange = Infinity;
        var foundBookmarks = [], endStyles = (void 0);
        for (var j = 0; j < spans.length; ++j) {
          var sp = spans[j], m = sp.marker;
          if (m.type == "bookmark" && sp.from == pos && m.widgetNode) {
            foundBookmarks.push(m);
          } else if (sp.from <= pos && (sp.to == null || sp.to > pos || m.collapsed && sp.to == pos && sp.from == pos)) {
            if (sp.to != null && sp.to != pos && nextChange > sp.to) {
              nextChange = sp.to;
              spanEndStyle = "";
            }
            if (m.className) { spanStyle += " " + m.className; }
            if (m.css) { css = (css ? css + ";" : "") + m.css; }
            if (m.startStyle && sp.from == pos) { spanStartStyle += " " + m.startStyle; }
            if (m.endStyle && sp.to == nextChange) { (endStyles || (endStyles = [])).push(m.endStyle, sp.to); }
            // support for the old title property
            // https://github.com/codemirror/CodeMirror/pull/5673
            if (m.title) { (attributes || (attributes = {})).title = m.title; }
            if (m.attributes) {
              for (var attr in m.attributes)
                { (attributes || (attributes = {}))[attr] = m.attributes[attr]; }
            }
            if (m.collapsed && (!collapsed || compareCollapsedMarkers(collapsed.marker, m) < 0))
              { collapsed = sp; }
          } else if (sp.from > pos && nextChange > sp.from) {
            nextChange = sp.from;
          }
        }
        if (endStyles) { for (var j$1 = 0; j$1 < endStyles.length; j$1 += 2)
          { if (endStyles[j$1 + 1] == nextChange) { spanEndStyle += " " + endStyles[j$1]; } } }

        if (!collapsed || collapsed.from == pos) { for (var j$2 = 0; j$2 < foundBookmarks.length; ++j$2)
          { buildCollapsedSpan(builder, 0, foundBookmarks[j$2]); } }
        if (collapsed && (collapsed.from || 0) == pos) {
          buildCollapsedSpan(builder, (collapsed.to == null ? len + 1 : collapsed.to) - pos,
                             collapsed.marker, collapsed.from == null);
          if (collapsed.to == null) { return }
          if (collapsed.to == pos) { collapsed = false; }
        }
      }
      if (pos >= len) { break }

      var upto = Math.min(len, nextChange);
      while (true) {
        if (text) {
          var end = pos + text.length;
          if (!collapsed) {
            var tokenText = end > upto ? text.slice(0, upto - pos) : text;
            builder.addToken(builder, tokenText, style ? style + spanStyle : spanStyle,
                             spanStartStyle, pos + tokenText.length == nextChange ? spanEndStyle : "", css, attributes);
          }
          if (end >= upto) {text = text.slice(upto - pos); pos = upto; break}
          pos = end;
          spanStartStyle = "";
        }
        text = allText.slice(at, at = styles[i++]);
        style = interpretTokenStyle(styles[i++], builder.cm.options);
      }
    }
  }


  // These objects are used to represent the visible (currently drawn)
  // part of the document. A LineView may correspond to multiple
  // logical lines, if those are connected by collapsed ranges.
  function LineView(doc, line, lineN) {
    // The starting line
    this.line = line;
    // Continuing lines, if any
    this.rest = visualLineContinued(line);
    // Number of logical lines in this visual line
    this.size = this.rest ? lineNo(lst(this.rest)) - lineN + 1 : 1;
    this.node = this.text = null;
    this.hidden = lineIsHidden(doc, line);
  }

  // Create a range of LineView objects for the given lines.
  function buildViewArray(cm, from, to) {
    var array = [], nextPos;
    for (var pos = from; pos < to; pos = nextPos) {
      var view = new LineView(cm.doc, getLine(cm.doc, pos), pos);
      nextPos = pos + view.size;
      array.push(view);
    }
    return array
  }

  var operationGroup = null;

  function pushOperation(op) {
    if (operationGroup) {
      operationGroup.ops.push(op);
    } else {
      op.ownsGroup = operationGroup = {
        ops: [op],
        delayedCallbacks: []
      };
    }
  }

  function fireCallbacksForOps(group) {
    // Calls delayed callbacks and cursorActivity handlers until no
    // new ones appear
    var callbacks = group.delayedCallbacks, i = 0;
    do {
      for (; i < callbacks.length; i++)
        { callbacks[i].call(null); }
      for (var j = 0; j < group.ops.length; j++) {
        var op = group.ops[j];
        if (op.cursorActivityHandlers)
          { while (op.cursorActivityCalled < op.cursorActivityHandlers.length)
            { op.cursorActivityHandlers[op.cursorActivityCalled++].call(null, op.cm); } }
      }
    } while (i < callbacks.length)
  }

  function finishOperation(op, endCb) {
    var group = op.ownsGroup;
    if (!group) { return }

    try { fireCallbacksForOps(group); }
    finally {
      operationGroup = null;
      endCb(group);
    }
  }

  var orphanDelayedCallbacks = null;

  // Often, we want to signal events at a point where we are in the
  // middle of some work, but don't want the handler to start calling
  // other methods on the editor, which might be in an inconsistent
  // state or simply not expect any other events to happen.
  // signalLater looks whether there are any handlers, and schedules
  // them to be executed when the last operation ends, or, if no
  // operation is active, when a timeout fires.
  function signalLater(emitter, type /*, values...*/) {
    var arr = getHandlers(emitter, type);
    if (!arr.length) { return }
    var args = Array.prototype.slice.call(arguments, 2), list;
    if (operationGroup) {
      list = operationGroup.delayedCallbacks;
    } else if (orphanDelayedCallbacks) {
      list = orphanDelayedCallbacks;
    } else {
      list = orphanDelayedCallbacks = [];
      setTimeout(fireOrphanDelayed, 0);
    }
    var loop = function ( i ) {
      list.push(function () { return arr[i].apply(null, args); });
    };

    for (var i = 0; i < arr.length; ++i)
      loop( i );
  }

  function fireOrphanDelayed() {
    var delayed = orphanDelayedCallbacks;
    orphanDelayedCallbacks = null;
    for (var i = 0; i < delayed.length; ++i) { delayed[i](); }
  }

  // When an aspect of a line changes, a string is added to
  // lineView.changes. This updates the relevant part of the line's
  // DOM structure.
  function updateLineForChanges(cm, lineView, lineN, dims) {
    for (var j = 0; j < lineView.changes.length; j++) {
      var type = lineView.changes[j];
      if (type == "text") { updateLineText(cm, lineView); }
      else if (type == "gutter") { updateLineGutter(cm, lineView, lineN, dims); }
      else if (type == "class") { updateLineClasses(cm, lineView); }
      else if (type == "widget") { updateLineWidgets(cm, lineView, dims); }
    }
    lineView.changes = null;
  }

  // Lines with gutter elements, widgets or a background class need to
  // be wrapped, and have the extra elements added to the wrapper div
  function ensureLineWrapped(lineView) {
    if (lineView.node == lineView.text) {
      lineView.node = elt("div", null, null, "position: relative");
      if (lineView.text.parentNode)
        { lineView.text.parentNode.replaceChild(lineView.node, lineView.text); }
      lineView.node.appendChild(lineView.text);
      if (ie && ie_version < 8) { lineView.node.style.zIndex = 2; }
    }
    return lineView.node
  }

  function updateLineBackground(cm, lineView) {
    var cls = lineView.bgClass ? lineView.bgClass + " " + (lineView.line.bgClass || "") : lineView.line.bgClass;
    if (cls) { cls += " CodeMirror-linebackground"; }
    if (lineView.background) {
      if (cls) { lineView.background.className = cls; }
      else { lineView.background.parentNode.removeChild(lineView.background); lineView.background = null; }
    } else if (cls) {
      var wrap = ensureLineWrapped(lineView);
      lineView.background = wrap.insertBefore(elt("div", null, cls), wrap.firstChild);
      cm.display.input.setUneditable(lineView.background);
    }
  }

  // Wrapper around buildLineContent which will reuse the structure
  // in display.externalMeasured when possible.
  function getLineContent(cm, lineView) {
    var ext = cm.display.externalMeasured;
    if (ext && ext.line == lineView.line) {
      cm.display.externalMeasured = null;
      lineView.measure = ext.measure;
      return ext.built
    }
    return buildLineContent(cm, lineView)
  }

  // Redraw the line's text. Interacts with the background and text
  // classes because the mode may output tokens that influence these
  // classes.
  function updateLineText(cm, lineView) {
    var cls = lineView.text.className;
    var built = getLineContent(cm, lineView);
    if (lineView.text == lineView.node) { lineView.node = built.pre; }
    lineView.text.parentNode.replaceChild(built.pre, lineView.text);
    lineView.text = built.pre;
    if (built.bgClass != lineView.bgClass || built.textClass != lineView.textClass) {
      lineView.bgClass = built.bgClass;
      lineView.textClass = built.textClass;
      updateLineClasses(cm, lineView);
    } else if (cls) {
      lineView.text.className = cls;
    }
  }

  function updateLineClasses(cm, lineView) {
    updateLineBackground(cm, lineView);
    if (lineView.line.wrapClass)
      { ensureLineWrapped(lineView).className = lineView.line.wrapClass; }
    else if (lineView.node != lineView.text)
      { lineView.node.className = ""; }
    var textClass = lineView.textClass ? lineView.textClass + " " + (lineView.line.textClass || "") : lineView.line.textClass;
    lineView.text.className = textClass || "";
  }

  function updateLineGutter(cm, lineView, lineN, dims) {
    if (lineView.gutter) {
      lineView.node.removeChild(lineView.gutter);
      lineView.gutter = null;
    }
    if (lineView.gutterBackground) {
      lineView.node.removeChild(lineView.gutterBackground);
      lineView.gutterBackground = null;
    }
    if (lineView.line.gutterClass) {
      var wrap = ensureLineWrapped(lineView);
      lineView.gutterBackground = elt("div", null, "CodeMirror-gutter-background " + lineView.line.gutterClass,
                                      ("left: " + (cm.options.fixedGutter ? dims.fixedPos : -dims.gutterTotalWidth) + "px; width: " + (dims.gutterTotalWidth) + "px"));
      cm.display.input.setUneditable(lineView.gutterBackground);
      wrap.insertBefore(lineView.gutterBackground, lineView.text);
    }
    var markers = lineView.line.gutterMarkers;
    if (cm.options.lineNumbers || markers) {
      var wrap$1 = ensureLineWrapped(lineView);
      var gutterWrap = lineView.gutter = elt("div", null, "CodeMirror-gutter-wrapper", ("left: " + (cm.options.fixedGutter ? dims.fixedPos : -dims.gutterTotalWidth) + "px"));
      cm.display.input.setUneditable(gutterWrap);
      wrap$1.insertBefore(gutterWrap, lineView.text);
      if (lineView.line.gutterClass)
        { gutterWrap.className += " " + lineView.line.gutterClass; }
      if (cm.options.lineNumbers && (!markers || !markers["CodeMirror-linenumbers"]))
        { lineView.lineNumber = gutterWrap.appendChild(
          elt("div", lineNumberFor(cm.options, lineN),
              "CodeMirror-linenumber CodeMirror-gutter-elt",
              ("left: " + (dims.gutterLeft["CodeMirror-linenumbers"]) + "px; width: " + (cm.display.lineNumInnerWidth) + "px"))); }
      if (markers) { for (var k = 0; k < cm.display.gutterSpecs.length; ++k) {
        var id = cm.display.gutterSpecs[k].className, found = markers.hasOwnProperty(id) && markers[id];
        if (found)
          { gutterWrap.appendChild(elt("div", [found], "CodeMirror-gutter-elt",
                                     ("left: " + (dims.gutterLeft[id]) + "px; width: " + (dims.gutterWidth[id]) + "px"))); }
      } }
    }
  }

  function updateLineWidgets(cm, lineView, dims) {
    if (lineView.alignable) { lineView.alignable = null; }
    var isWidget = classTest("CodeMirror-linewidget");
    for (var node = lineView.node.firstChild, next = (void 0); node; node = next) {
      next = node.nextSibling;
      if (isWidget.test(node.className)) { lineView.node.removeChild(node); }
    }
    insertLineWidgets(cm, lineView, dims);
  }

  // Build a line's DOM representation from scratch
  function buildLineElement(cm, lineView, lineN, dims) {
    var built = getLineContent(cm, lineView);
    lineView.text = lineView.node = built.pre;
    if (built.bgClass) { lineView.bgClass = built.bgClass; }
    if (built.textClass) { lineView.textClass = built.textClass; }

    updateLineClasses(cm, lineView);
    updateLineGutter(cm, lineView, lineN, dims);
    insertLineWidgets(cm, lineView, dims);
    return lineView.node
  }

  // A lineView may contain multiple logical lines (when merged by
  // collapsed spans). The widgets for all of them need to be drawn.
  function insertLineWidgets(cm, lineView, dims) {
    insertLineWidgetsFor(cm, lineView.line, lineView, dims, true);
    if (lineView.rest) { for (var i = 0; i < lineView.rest.length; i++)
      { insertLineWidgetsFor(cm, lineView.rest[i], lineView, dims, false); } }
  }

  function insertLineWidgetsFor(cm, line, lineView, dims, allowAbove) {
    if (!line.widgets) { return }
    var wrap = ensureLineWrapped(lineView);
    for (var i = 0, ws = line.widgets; i < ws.length; ++i) {
      var widget = ws[i], node = elt("div", [widget.node], "CodeMirror-linewidget" + (widget.className ? " " + widget.className : ""));
      if (!widget.handleMouseEvents) { node.setAttribute("cm-ignore-events", "true"); }
      positionLineWidget(widget, node, lineView, dims);
      cm.display.input.setUneditable(node);
      if (allowAbove && widget.above)
        { wrap.insertBefore(node, lineView.gutter || lineView.text); }
      else
        { wrap.appendChild(node); }
      signalLater(widget, "redraw");
    }
  }

  function positionLineWidget(widget, node, lineView, dims) {
    if (widget.noHScroll) {
  (lineView.alignable || (lineView.alignable = [])).push(node);
      var width = dims.wrapperWidth;
      node.style.left = dims.fixedPos + "px";
      if (!widget.coverGutter) {
        width -= dims.gutterTotalWidth;
        node.style.paddingLeft = dims.gutterTotalWidth + "px";
      }
      node.style.width = width + "px";
    }
    if (widget.coverGutter) {
      node.style.zIndex = 5;
      node.style.position = "relative";
      if (!widget.noHScroll) { node.style.marginLeft = -dims.gutterTotalWidth + "px"; }
    }
  }

  function widgetHeight(widget) {
    if (widget.height != null) { return widget.height }
    var cm = widget.doc.cm;
    if (!cm) { return 0 }
    if (!contains(document.body, widget.node)) {
      var parentStyle = "position: relative;";
      if (widget.coverGutter)
        { parentStyle += "margin-left: -" + cm.display.gutters.offsetWidth + "px;"; }
      if (widget.noHScroll)
        { parentStyle += "width: " + cm.display.wrapper.clientWidth + "px;"; }
      removeChildrenAndAdd(cm.display.measure, elt("div", [widget.node], null, parentStyle));
    }
    return widget.height = widget.node.parentNode.offsetHeight
  }

  // Return true when the given mouse event happened in a widget
  function eventInWidget(display, e) {
    for (var n = e_target(e); n != display.wrapper; n = n.parentNode) {
      if (!n || (n.nodeType == 1 && n.getAttribute("cm-ignore-events") == "true") ||
          (n.parentNode == display.sizer && n != display.mover))
        { return true }
    }
  }

  // POSITION MEASUREMENT

  function paddingTop(display) {return display.lineSpace.offsetTop}
  function paddingVert(display) {return display.mover.offsetHeight - display.lineSpace.offsetHeight}
  function paddingH(display) {
    if (display.cachedPaddingH) { return display.cachedPaddingH }
    var e = removeChildrenAndAdd(display.measure, elt("pre", "x", "CodeMirror-line-like"));
    var style = window.getComputedStyle ? window.getComputedStyle(e) : e.currentStyle;
    var data = {left: parseInt(style.paddingLeft), right: parseInt(style.paddingRight)};
    if (!isNaN(data.left) && !isNaN(data.right)) { display.cachedPaddingH = data; }
    return data
  }

  function scrollGap(cm) { return scrollerGap - cm.display.nativeBarWidth }
  function displayWidth(cm) {
    return cm.display.scroller.clientWidth - scrollGap(cm) - cm.display.barWidth
  }
  function displayHeight(cm) {
    return cm.display.scroller.clientHeight - scrollGap(cm) - cm.display.barHeight
  }

  // Ensure the lineView.wrapping.heights array is populated. This is
  // an array of bottom offsets for the lines that make up a drawn
  // line. When lineWrapping is on, there might be more than one
  // height.
  function ensureLineHeights(cm, lineView, rect) {
    var wrapping = cm.options.lineWrapping;
    var curWidth = wrapping && displayWidth(cm);
    if (!lineView.measure.heights || wrapping && lineView.measure.width != curWidth) {
      var heights = lineView.measure.heights = [];
      if (wrapping) {
        lineView.measure.width = curWidth;
        var rects = lineView.text.firstChild.getClientRects();
        for (var i = 0; i < rects.length - 1; i++) {
          var cur = rects[i], next = rects[i + 1];
          if (Math.abs(cur.bottom - next.bottom) > 2)
            { heights.push((cur.bottom + next.top) / 2 - rect.top); }
        }
      }
      heights.push(rect.bottom - rect.top);
    }
  }

  // Find a line map (mapping character offsets to text nodes) and a
  // measurement cache for the given line number. (A line view might
  // contain multiple lines when collapsed ranges are present.)
  function mapFromLineView(lineView, line, lineN) {
    if (lineView.line == line)
      { return {map: lineView.measure.map, cache: lineView.measure.cache} }
    for (var i = 0; i < lineView.rest.length; i++)
      { if (lineView.rest[i] == line)
        { return {map: lineView.measure.maps[i], cache: lineView.measure.caches[i]} } }
    for (var i$1 = 0; i$1 < lineView.rest.length; i$1++)
      { if (lineNo(lineView.rest[i$1]) > lineN)
        { return {map: lineView.measure.maps[i$1], cache: lineView.measure.caches[i$1], before: true} } }
  }

  // Render a line into the hidden node display.externalMeasured. Used
  // when measurement is needed for a line that's not in the viewport.
  function updateExternalMeasurement(cm, line) {
    line = visualLine(line);
    var lineN = lineNo(line);
    var view = cm.display.externalMeasured = new LineView(cm.doc, line, lineN);
    view.lineN = lineN;
    var built = view.built = buildLineContent(cm, view);
    view.text = built.pre;
    removeChildrenAndAdd(cm.display.lineMeasure, built.pre);
    return view
  }

  // Get a {top, bottom, left, right} box (in line-local coordinates)
  // for a given character.
  function measureChar(cm, line, ch, bias) {
    return measureCharPrepared(cm, prepareMeasureForLine(cm, line), ch, bias)
  }

  // Find a line view that corresponds to the given line number.
  function findViewForLine(cm, lineN) {
    if (lineN >= cm.display.viewFrom && lineN < cm.display.viewTo)
      { return cm.display.view[findViewIndex(cm, lineN)] }
    var ext = cm.display.externalMeasured;
    if (ext && lineN >= ext.lineN && lineN < ext.lineN + ext.size)
      { return ext }
  }

  // Measurement can be split in two steps, the set-up work that
  // applies to the whole line, and the measurement of the actual
  // character. Functions like coordsChar, that need to do a lot of
  // measurements in a row, can thus ensure that the set-up work is
  // only done once.
  function prepareMeasureForLine(cm, line) {
    var lineN = lineNo(line);
    var view = findViewForLine(cm, lineN);
    if (view && !view.text) {
      view = null;
    } else if (view && view.changes) {
      updateLineForChanges(cm, view, lineN, getDimensions(cm));
      cm.curOp.forceUpdate = true;
    }
    if (!view)
      { view = updateExternalMeasurement(cm, line); }

    var info = mapFromLineView(view, line, lineN);
    return {
      line: line, view: view, rect: null,
      map: info.map, cache: info.cache, before: info.before,
      hasHeights: false
    }
  }

  // Given a prepared measurement object, measures the position of an
  // actual character (or fetches it from the cache).
  function measureCharPrepared(cm, prepared, ch, bias, varHeight) {
    if (prepared.before) { ch = -1; }
    var key = ch + (bias || ""), found;
    if (prepared.cache.hasOwnProperty(key)) {
      found = prepared.cache[key];
    } else {
      if (!prepared.rect)
        { prepared.rect = prepared.view.text.getBoundingClientRect(); }
      if (!prepared.hasHeights) {
        ensureLineHeights(cm, prepared.view, prepared.rect);
        prepared.hasHeights = true;
      }
      found = measureCharInner(cm, prepared, ch, bias);
      if (!found.bogus) { prepared.cache[key] = found; }
    }
    return {left: found.left, right: found.right,
            top: varHeight ? found.rtop : found.top,
            bottom: varHeight ? found.rbottom : found.bottom}
  }

  var nullRect = {left: 0, right: 0, top: 0, bottom: 0};

  function nodeAndOffsetInLineMap(map, ch, bias) {
    var node, start, end, collapse, mStart, mEnd;
    // First, search the line map for the text node corresponding to,
    // or closest to, the target character.
    for (var i = 0; i < map.length; i += 3) {
      mStart = map[i];
      mEnd = map[i + 1];
      if (ch < mStart) {
        start = 0; end = 1;
        collapse = "left";
      } else if (ch < mEnd) {
        start = ch - mStart;
        end = start + 1;
      } else if (i == map.length - 3 || ch == mEnd && map[i + 3] > ch) {
        end = mEnd - mStart;
        start = end - 1;
        if (ch >= mEnd) { collapse = "right"; }
      }
      if (start != null) {
        node = map[i + 2];
        if (mStart == mEnd && bias == (node.insertLeft ? "left" : "right"))
          { collapse = bias; }
        if (bias == "left" && start == 0)
          { while (i && map[i - 2] == map[i - 3] && map[i - 1].insertLeft) {
            node = map[(i -= 3) + 2];
            collapse = "left";
          } }
        if (bias == "right" && start == mEnd - mStart)
          { while (i < map.length - 3 && map[i + 3] == map[i + 4] && !map[i + 5].insertLeft) {
            node = map[(i += 3) + 2];
            collapse = "right";
          } }
        break
      }
    }
    return {node: node, start: start, end: end, collapse: collapse, coverStart: mStart, coverEnd: mEnd}
  }

  function getUsefulRect(rects, bias) {
    var rect = nullRect;
    if (bias == "left") { for (var i = 0; i < rects.length; i++) {
      if ((rect = rects[i]).left != rect.right) { break }
    } } else { for (var i$1 = rects.length - 1; i$1 >= 0; i$1--) {
      if ((rect = rects[i$1]).left != rect.right) { break }
    } }
    return rect
  }

  function measureCharInner(cm, prepared, ch, bias) {
    var place = nodeAndOffsetInLineMap(prepared.map, ch, bias);
    var node = place.node, start = place.start, end = place.end, collapse = place.collapse;

    var rect;
    if (node.nodeType == 3) { // If it is a text node, use a range to retrieve the coordinates.
      for (var i$1 = 0; i$1 < 4; i$1++) { // Retry a maximum of 4 times when nonsense rectangles are returned
        while (start && isExtendingChar(prepared.line.text.charAt(place.coverStart + start))) { --start; }
        while (place.coverStart + end < place.coverEnd && isExtendingChar(prepared.line.text.charAt(place.coverStart + end))) { ++end; }
        if (ie && ie_version < 9 && start == 0 && end == place.coverEnd - place.coverStart)
          { rect = node.parentNode.getBoundingClientRect(); }
        else
          { rect = getUsefulRect(range(node, start, end).getClientRects(), bias); }
        if (rect.left || rect.right || start == 0) { break }
        end = start;
        start = start - 1;
        collapse = "right";
      }
      if (ie && ie_version < 11) { rect = maybeUpdateRectForZooming(cm.display.measure, rect); }
    } else { // If it is a widget, simply get the box for the whole widget.
      if (start > 0) { collapse = bias = "right"; }
      var rects;
      if (cm.options.lineWrapping && (rects = node.getClientRects()).length > 1)
        { rect = rects[bias == "right" ? rects.length - 1 : 0]; }
      else
        { rect = node.getBoundingClientRect(); }
    }
    if (ie && ie_version < 9 && !start && (!rect || !rect.left && !rect.right)) {
      var rSpan = node.parentNode.getClientRects()[0];
      if (rSpan)
        { rect = {left: rSpan.left, right: rSpan.left + charWidth(cm.display), top: rSpan.top, bottom: rSpan.bottom}; }
      else
        { rect = nullRect; }
    }

    var rtop = rect.top - prepared.rect.top, rbot = rect.bottom - prepared.rect.top;
    var mid = (rtop + rbot) / 2;
    var heights = prepared.view.measure.heights;
    var i = 0;
    for (; i < heights.length - 1; i++)
      { if (mid < heights[i]) { break } }
    var top = i ? heights[i - 1] : 0, bot = heights[i];
    var result = {left: (collapse == "right" ? rect.right : rect.left) - prepared.rect.left,
                  right: (collapse == "left" ? rect.left : rect.right) - prepared.rect.left,
                  top: top, bottom: bot};
    if (!rect.left && !rect.right) { result.bogus = true; }
    if (!cm.options.singleCursorHeightPerLine) { result.rtop = rtop; result.rbottom = rbot; }

    return result
  }

  // Work around problem with bounding client rects on ranges being
  // returned incorrectly when zoomed on IE10 and below.
  function maybeUpdateRectForZooming(measure, rect) {
    if (!window.screen || screen.logicalXDPI == null ||
        screen.logicalXDPI == screen.deviceXDPI || !hasBadZoomedRects(measure))
      { return rect }
    var scaleX = screen.logicalXDPI / screen.deviceXDPI;
    var scaleY = screen.logicalYDPI / screen.deviceYDPI;
    return {left: rect.left * scaleX, right: rect.right * scaleX,
            top: rect.top * scaleY, bottom: rect.bottom * scaleY}
  }

  function clearLineMeasurementCacheFor(lineView) {
    if (lineView.measure) {
      lineView.measure.cache = {};
      lineView.measure.heights = null;
      if (lineView.rest) { for (var i = 0; i < lineView.rest.length; i++)
        { lineView.measure.caches[i] = {}; } }
    }
  }

  function clearLineMeasurementCache(cm) {
    cm.display.externalMeasure = null;
    removeChildren(cm.display.lineMeasure);
    for (var i = 0; i < cm.display.view.length; i++)
      { clearLineMeasurementCacheFor(cm.display.view[i]); }
  }

  function clearCaches(cm) {
    clearLineMeasurementCache(cm);
    cm.display.cachedCharWidth = cm.display.cachedTextHeight = cm.display.cachedPaddingH = null;
    if (!cm.options.lineWrapping) { cm.display.maxLineChanged = true; }
    cm.display.lineNumChars = null;
  }

  function pageScrollX() {
    // Work around https://bugs.chromium.org/p/chromium/issues/detail?id=489206
    // which causes page_Offset and bounding client rects to use
    // different reference viewports and invalidate our calculations.
    if (chrome && android) { return -(document.body.getBoundingClientRect().left - parseInt(getComputedStyle(document.body).marginLeft)) }
    return window.pageXOffset || (document.documentElement || document.body).scrollLeft
  }
  function pageScrollY() {
    if (chrome && android) { return -(document.body.getBoundingClientRect().top - parseInt(getComputedStyle(document.body).marginTop)) }
    return window.pageYOffset || (document.documentElement || document.body).scrollTop
  }

  function widgetTopHeight(lineObj) {
    var height = 0;
    if (lineObj.widgets) { for (var i = 0; i < lineObj.widgets.length; ++i) { if (lineObj.widgets[i].above)
      { height += widgetHeight(lineObj.widgets[i]); } } }
    return height
  }

  // Converts a {top, bottom, left, right} box from line-local
  // coordinates into another coordinate system. Context may be one of
  // "line", "div" (display.lineDiv), "local"./null (editor), "window",
  // or "page".
  function intoCoordSystem(cm, lineObj, rect, context, includeWidgets) {
    if (!includeWidgets) {
      var height = widgetTopHeight(lineObj);
      rect.top += height; rect.bottom += height;
    }
    if (context == "line") { return rect }
    if (!context) { context = "local"; }
    var yOff = heightAtLine(lineObj);
    if (context == "local") { yOff += paddingTop(cm.display); }
    else { yOff -= cm.display.viewOffset; }
    if (context == "page" || context == "window") {
      var lOff = cm.display.lineSpace.getBoundingClientRect();
      yOff += lOff.top + (context == "window" ? 0 : pageScrollY());
      var xOff = lOff.left + (context == "window" ? 0 : pageScrollX());
      rect.left += xOff; rect.right += xOff;
    }
    rect.top += yOff; rect.bottom += yOff;
    return rect
  }

  // Coverts a box from "div" coords to another coordinate system.
  // Context may be "window", "page", "div", or "local"./null.
  function fromCoordSystem(cm, coords, context) {
    if (context == "div") { return coords }
    var left = coords.left, top = coords.top;
    // First move into "page" coordinate system
    if (context == "page") {
      left -= pageScrollX();
      top -= pageScrollY();
    } else if (context == "local" || !context) {
      var localBox = cm.display.sizer.getBoundingClientRect();
      left += localBox.left;
      top += localBox.top;
    }

    var lineSpaceBox = cm.display.lineSpace.getBoundingClientRect();
    return {left: left - lineSpaceBox.left, top: top - lineSpaceBox.top}
  }

  function charCoords(cm, pos, context, lineObj, bias) {
    if (!lineObj) { lineObj = getLine(cm.doc, pos.line); }
    return intoCoordSystem(cm, lineObj, measureChar(cm, lineObj, pos.ch, bias), context)
  }

  // Returns a box for a given cursor position, which may have an
  // 'other' property containing the position of the secondary cursor
  // on a bidi boundary.
  // A cursor Pos(line, char, "before") is on the same visual line as `char - 1`
  // and after `char - 1` in writing order of `char - 1`
  // A cursor Pos(line, char, "after") is on the same visual line as `char`
  // and before `char` in writing order of `char`
  // Examples (upper-case letters are RTL, lower-case are LTR):
  //     Pos(0, 1, ...)
  //     before   after
  // ab     a|b     a|b
  // aB     a|B     aB|
  // Ab     |Ab     A|b
  // AB     B|A     B|A
  // Every position after the last character on a line is considered to stick
  // to the last character on the line.
  function cursorCoords(cm, pos, context, lineObj, preparedMeasure, varHeight) {
    lineObj = lineObj || getLine(cm.doc, pos.line);
    if (!preparedMeasure) { preparedMeasure = prepareMeasureForLine(cm, lineObj); }
    function get(ch, right) {
      var m = measureCharPrepared(cm, preparedMeasure, ch, right ? "right" : "left", varHeight);
      if (right) { m.left = m.right; } else { m.right = m.left; }
      return intoCoordSystem(cm, lineObj, m, context)
    }
    var order = getOrder(lineObj, cm.doc.direction), ch = pos.ch, sticky = pos.sticky;
    if (ch >= lineObj.text.length) {
      ch = lineObj.text.length;
      sticky = "before";
    } else if (ch <= 0) {
      ch = 0;
      sticky = "after";
    }
    if (!order) { return get(sticky == "before" ? ch - 1 : ch, sticky == "before") }

    function getBidi(ch, partPos, invert) {
      var part = order[partPos], right = part.level == 1;
      return get(invert ? ch - 1 : ch, right != invert)
    }
    var partPos = getBidiPartAt(order, ch, sticky);
    var other = bidiOther;
    var val = getBidi(ch, partPos, sticky == "before");
    if (other != null) { val.other = getBidi(ch, other, sticky != "before"); }
    return val
  }

  // Used to cheaply estimate the coordinates for a position. Used for
  // intermediate scroll updates.
  function estimateCoords(cm, pos) {
    var left = 0;
    pos = clipPos(cm.doc, pos);
    if (!cm.options.lineWrapping) { left = charWidth(cm.display) * pos.ch; }
    var lineObj = getLine(cm.doc, pos.line);
    var top = heightAtLine(lineObj) + paddingTop(cm.display);
    return {left: left, right: left, top: top, bottom: top + lineObj.height}
  }

  // Positions returned by coordsChar contain some extra information.
  // xRel is the relative x position of the input coordinates compared
  // to the found position (so xRel > 0 means the coordinates are to
  // the right of the character position, for example). When outside
  // is true, that means the coordinates lie outside the line's
  // vertical range.
  function PosWithInfo(line, ch, sticky, outside, xRel) {
    var pos = Pos(line, ch, sticky);
    pos.xRel = xRel;
    if (outside) { pos.outside = outside; }
    return pos
  }

  // Compute the character position closest to the given coordinates.
  // Input must be lineSpace-local ("div" coordinate system).
  function coordsChar(cm, x, y) {
    var doc = cm.doc;
    y += cm.display.viewOffset;
    if (y < 0) { return PosWithInfo(doc.first, 0, null, -1, -1) }
    var lineN = lineAtHeight(doc, y), last = doc.first + doc.size - 1;
    if (lineN > last)
      { return PosWithInfo(doc.first + doc.size - 1, getLine(doc, last).text.length, null, 1, 1) }
    if (x < 0) { x = 0; }

    var lineObj = getLine(doc, lineN);
    for (;;) {
      var found = coordsCharInner(cm, lineObj, lineN, x, y);
      var collapsed = collapsedSpanAround(lineObj, found.ch + (found.xRel > 0 || found.outside > 0 ? 1 : 0));
      if (!collapsed) { return found }
      var rangeEnd = collapsed.find(1);
      if (rangeEnd.line == lineN) { return rangeEnd }
      lineObj = getLine(doc, lineN = rangeEnd.line);
    }
  }

  function wrappedLineExtent(cm, lineObj, preparedMeasure, y) {
    y -= widgetTopHeight(lineObj);
    var end = lineObj.text.length;
    var begin = findFirst(function (ch) { return measureCharPrepared(cm, preparedMeasure, ch - 1).bottom <= y; }, end, 0);
    end = findFirst(function (ch) { return measureCharPrepared(cm, preparedMeasure, ch).top > y; }, begin, end);
    return {begin: begin, end: end}
  }

  function wrappedLineExtentChar(cm, lineObj, preparedMeasure, target) {
    if (!preparedMeasure) { preparedMeasure = prepareMeasureForLine(cm, lineObj); }
    var targetTop = intoCoordSystem(cm, lineObj, measureCharPrepared(cm, preparedMeasure, target), "line").top;
    return wrappedLineExtent(cm, lineObj, preparedMeasure, targetTop)
  }

  // Returns true if the given side of a box is after the given
  // coordinates, in top-to-bottom, left-to-right order.
  function boxIsAfter(box, x, y, left) {
    return box.bottom <= y ? false : box.top > y ? true : (left ? box.left : box.right) > x
  }

  function coordsCharInner(cm, lineObj, lineNo, x, y) {
    // Move y into line-local coordinate space
    y -= heightAtLine(lineObj);
    var preparedMeasure = prepareMeasureForLine(cm, lineObj);
    // When directly calling `measureCharPrepared`, we have to adjust
    // for the widgets at this line.
    var widgetHeight = widgetTopHeight(lineObj);
    var begin = 0, end = lineObj.text.length, ltr = true;

    var order = getOrder(lineObj, cm.doc.direction);
    // If the line isn't plain left-to-right text, first figure out
    // which bidi section the coordinates fall into.
    if (order) {
      var part = (cm.options.lineWrapping ? coordsBidiPartWrapped : coordsBidiPart)
                   (cm, lineObj, lineNo, preparedMeasure, order, x, y);
      ltr = part.level != 1;
      // The awkward -1 offsets are needed because findFirst (called
      // on these below) will treat its first bound as inclusive,
      // second as exclusive, but we want to actually address the
      // characters in the part's range
      begin = ltr ? part.from : part.to - 1;
      end = ltr ? part.to : part.from - 1;
    }

    // A binary search to find the first character whose bounding box
    // starts after the coordinates. If we run across any whose box wrap
    // the coordinates, store that.
    var chAround = null, boxAround = null;
    var ch = findFirst(function (ch) {
      var box = measureCharPrepared(cm, preparedMeasure, ch);
      box.top += widgetHeight; box.bottom += widgetHeight;
      if (!boxIsAfter(box, x, y, false)) { return false }
      if (box.top <= y && box.left <= x) {
        chAround = ch;
        boxAround = box;
      }
      return true
    }, begin, end);

    var baseX, sticky, outside = false;
    // If a box around the coordinates was found, use that
    if (boxAround) {
      // Distinguish coordinates nearer to the left or right side of the box
      var atLeft = x - boxAround.left < boxAround.right - x, atStart = atLeft == ltr;
      ch = chAround + (atStart ? 0 : 1);
      sticky = atStart ? "after" : "before";
      baseX = atLeft ? boxAround.left : boxAround.right;
    } else {
      // (Adjust for extended bound, if necessary.)
      if (!ltr && (ch == end || ch == begin)) { ch++; }
      // To determine which side to associate with, get the box to the
      // left of the character and compare it's vertical position to the
      // coordinates
      sticky = ch == 0 ? "after" : ch == lineObj.text.length ? "before" :
        (measureCharPrepared(cm, preparedMeasure, ch - (ltr ? 1 : 0)).bottom + widgetHeight <= y) == ltr ?
        "after" : "before";
      // Now get accurate coordinates for this place, in order to get a
      // base X position
      var coords = cursorCoords(cm, Pos(lineNo, ch, sticky), "line", lineObj, preparedMeasure);
      baseX = coords.left;
      outside = y < coords.top ? -1 : y >= coords.bottom ? 1 : 0;
    }

    ch = skipExtendingChars(lineObj.text, ch, 1);
    return PosWithInfo(lineNo, ch, sticky, outside, x - baseX)
  }

  function coordsBidiPart(cm, lineObj, lineNo, preparedMeasure, order, x, y) {
    // Bidi parts are sorted left-to-right, and in a non-line-wrapping
    // situation, we can take this ordering to correspond to the visual
    // ordering. This finds the first part whose end is after the given
    // coordinates.
    var index = findFirst(function (i) {
      var part = order[i], ltr = part.level != 1;
      return boxIsAfter(cursorCoords(cm, Pos(lineNo, ltr ? part.to : part.from, ltr ? "before" : "after"),
                                     "line", lineObj, preparedMeasure), x, y, true)
    }, 0, order.length - 1);
    var part = order[index];
    // If this isn't the first part, the part's start is also after
    // the coordinates, and the coordinates aren't on the same line as
    // that start, move one part back.
    if (index > 0) {
      var ltr = part.level != 1;
      var start = cursorCoords(cm, Pos(lineNo, ltr ? part.from : part.to, ltr ? "after" : "before"),
                               "line", lineObj, preparedMeasure);
      if (boxIsAfter(start, x, y, true) && start.top > y)
        { part = order[index - 1]; }
    }
    return part
  }

  function coordsBidiPartWrapped(cm, lineObj, _lineNo, preparedMeasure, order, x, y) {
    // In a wrapped line, rtl text on wrapping boundaries can do things
    // that don't correspond to the ordering in our `order` array at
    // all, so a binary search doesn't work, and we want to return a
    // part that only spans one line so that the binary search in
    // coordsCharInner is safe. As such, we first find the extent of the
    // wrapped line, and then do a flat search in which we discard any
    // spans that aren't on the line.
    var ref = wrappedLineExtent(cm, lineObj, preparedMeasure, y);
    var begin = ref.begin;
    var end = ref.end;
    if (/\s/.test(lineObj.text.charAt(end - 1))) { end--; }
    var part = null, closestDist = null;
    for (var i = 0; i < order.length; i++) {
      var p = order[i];
      if (p.from >= end || p.to <= begin) { continue }
      var ltr = p.level != 1;
      var endX = measureCharPrepared(cm, preparedMeasure, ltr ? Math.min(end, p.to) - 1 : Math.max(begin, p.from)).right;
      // Weigh against spans ending before this, so that they are only
      // picked if nothing ends after
      var dist = endX < x ? x - endX + 1e9 : endX - x;
      if (!part || closestDist > dist) {
        part = p;
        closestDist = dist;
      }
    }
    if (!part) { part = order[order.length - 1]; }
    // Clip the part to the wrapped line.
    if (part.from < begin) { part = {from: begin, to: part.to, level: part.level}; }
    if (part.to > end) { part = {from: part.from, to: end, level: part.level}; }
    return part
  }

  var measureText;
  // Compute the default text height.
  function textHeight(display) {
    if (display.cachedTextHeight != null) { return display.cachedTextHeight }
    if (measureText == null) {
      measureText = elt("pre", null, "CodeMirror-line-like");
      // Measure a bunch of lines, for browsers that compute
      // fractional heights.
      for (var i = 0; i < 49; ++i) {
        measureText.appendChild(document.createTextNode("x"));
        measureText.appendChild(elt("br"));
      }
      measureText.appendChild(document.createTextNode("x"));
    }
    removeChildrenAndAdd(display.measure, measureText);
    var height = measureText.offsetHeight / 50;
    if (height > 3) { display.cachedTextHeight = height; }
    removeChildren(display.measure);
    return height || 1
  }

  // Compute the default character width.
  function charWidth(display) {
    if (display.cachedCharWidth != null) { return display.cachedCharWidth }
    var anchor = elt("span", "xxxxxxxxxx");
    var pre = elt("pre", [anchor], "CodeMirror-line-like");
    removeChildrenAndAdd(display.measure, pre);
    var rect = anchor.getBoundingClientRect(), width = (rect.right - rect.left) / 10;
    if (width > 2) { display.cachedCharWidth = width; }
    return width || 10
  }

  // Do a bulk-read of the DOM positions and sizes needed to draw the
  // view, so that we don't interleave reading and writing to the DOM.
  function getDimensions(cm) {
    var d = cm.display, left = {}, width = {};
    var gutterLeft = d.gutters.clientLeft;
    for (var n = d.gutters.firstChild, i = 0; n; n = n.nextSibling, ++i) {
      var id = cm.display.gutterSpecs[i].className;
      left[id] = n.offsetLeft + n.clientLeft + gutterLeft;
      width[id] = n.clientWidth;
    }
    return {fixedPos: compensateForHScroll(d),
            gutterTotalWidth: d.gutters.offsetWidth,
            gutterLeft: left,
            gutterWidth: width,
            wrapperWidth: d.wrapper.clientWidth}
  }

  // Computes display.scroller.scrollLeft + display.gutters.offsetWidth,
  // but using getBoundingClientRect to get a sub-pixel-accurate
  // result.
  function compensateForHScroll(display) {
    return display.scroller.getBoundingClientRect().left - display.sizer.getBoundingClientRect().left
  }

  // Returns a function that estimates the height of a line, to use as
  // first approximation until the line becomes visible (and is thus
  // properly measurable).
  function estimateHeight(cm) {
    var th = textHeight(cm.display), wrapping = cm.options.lineWrapping;
    var perLine = wrapping && Math.max(5, cm.display.scroller.clientWidth / charWidth(cm.display) - 3);
    return function (line) {
      if (lineIsHidden(cm.doc, line)) { return 0 }

      var widgetsHeight = 0;
      if (line.widgets) { for (var i = 0; i < line.widgets.length; i++) {
        if (line.widgets[i].height) { widgetsHeight += line.widgets[i].height; }
      } }

      if (wrapping)
        { return widgetsHeight + (Math.ceil(line.text.length / perLine) || 1) * th }
      else
        { return widgetsHeight + th }
    }
  }

  function estimateLineHeights(cm) {
    var doc = cm.doc, est = estimateHeight(cm);
    doc.iter(function (line) {
      var estHeight = est(line);
      if (estHeight != line.height) { updateLineHeight(line, estHeight); }
    });
  }

  // Given a mouse event, find the corresponding position. If liberal
  // is false, it checks whether a gutter or scrollbar was clicked,
  // and returns null if it was. forRect is used by rectangular
  // selections, and tries to estimate a character position even for
  // coordinates beyond the right of the text.
  function posFromMouse(cm, e, liberal, forRect) {
    var display = cm.display;
    if (!liberal && e_target(e).getAttribute("cm-not-content") == "true") { return null }

    var x, y, space = display.lineSpace.getBoundingClientRect();
    // Fails unpredictably on IE[67] when mouse is dragged around quickly.
    try { x = e.clientX - space.left; y = e.clientY - space.top; }
    catch (e$1) { return null }
    var coords = coordsChar(cm, x, y), line;
    if (forRect && coords.xRel > 0 && (line = getLine(cm.doc, coords.line).text).length == coords.ch) {
      var colDiff = countColumn(line, line.length, cm.options.tabSize) - line.length;
      coords = Pos(coords.line, Math.max(0, Math.round((x - paddingH(cm.display).left) / charWidth(cm.display)) - colDiff));
    }
    return coords
  }

  // Find the view element corresponding to a given line. Return null
  // when the line isn't visible.
  function findViewIndex(cm, n) {
    if (n >= cm.display.viewTo) { return null }
    n -= cm.display.viewFrom;
    if (n < 0) { return null }
    var view = cm.display.view;
    for (var i = 0; i < view.length; i++) {
      n -= view[i].size;
      if (n < 0) { return i }
    }
  }

  // Updates the display.view data structure for a given change to the
  // document. From and to are in pre-change coordinates. Lendiff is
  // the amount of lines added or subtracted by the change. This is
  // used for changes that span multiple lines, or change the way
  // lines are divided into visual lines. regLineChange (below)
  // registers single-line changes.
  function regChange(cm, from, to, lendiff) {
    if (from == null) { from = cm.doc.first; }
    if (to == null) { to = cm.doc.first + cm.doc.size; }
    if (!lendiff) { lendiff = 0; }

    var display = cm.display;
    if (lendiff && to < display.viewTo &&
        (display.updateLineNumbers == null || display.updateLineNumbers > from))
      { display.updateLineNumbers = from; }

    cm.curOp.viewChanged = true;

    if (from >= display.viewTo) { // Change after
      if (sawCollapsedSpans && visualLineNo(cm.doc, from) < display.viewTo)
        { resetView(cm); }
    } else if (to <= display.viewFrom) { // Change before
      if (sawCollapsedSpans && visualLineEndNo(cm.doc, to + lendiff) > display.viewFrom) {
        resetView(cm);
      } else {
        display.viewFrom += lendiff;
        display.viewTo += lendiff;
      }
    } else if (from <= display.viewFrom && to >= display.viewTo) { // Full overlap
      resetView(cm);
    } else if (from <= display.viewFrom) { // Top overlap
      var cut = viewCuttingPoint(cm, to, to + lendiff, 1);
      if (cut) {
        display.view = display.view.slice(cut.index);
        display.viewFrom = cut.lineN;
        display.viewTo += lendiff;
      } else {
        resetView(cm);
      }
    } else if (to >= display.viewTo) { // Bottom overlap
      var cut$1 = viewCuttingPoint(cm, from, from, -1);
      if (cut$1) {
        display.view = display.view.slice(0, cut$1.index);
        display.viewTo = cut$1.lineN;
      } else {
        resetView(cm);
      }
    } else { // Gap in the middle
      var cutTop = viewCuttingPoint(cm, from, from, -1);
      var cutBot = viewCuttingPoint(cm, to, to + lendiff, 1);
      if (cutTop && cutBot) {
        display.view = display.view.slice(0, cutTop.index)
          .concat(buildViewArray(cm, cutTop.lineN, cutBot.lineN))
          .concat(display.view.slice(cutBot.index));
        display.viewTo += lendiff;
      } else {
        resetView(cm);
      }
    }

    var ext = display.externalMeasured;
    if (ext) {
      if (to < ext.lineN)
        { ext.lineN += lendiff; }
      else if (from < ext.lineN + ext.size)
        { display.externalMeasured = null; }
    }
  }

  // Register a change to a single line. Type must be one of "text",
  // "gutter", "class", "widget"
  function regLineChange(cm, line, type) {
    cm.curOp.viewChanged = true;
    var display = cm.display, ext = cm.display.externalMeasured;
    if (ext && line >= ext.lineN && line < ext.lineN + ext.size)
      { display.externalMeasured = null; }

    if (line < display.viewFrom || line >= display.viewTo) { return }
    var lineView = display.view[findViewIndex(cm, line)];
    if (lineView.node == null) { return }
    var arr = lineView.changes || (lineView.changes = []);
    if (indexOf(arr, type) == -1) { arr.push(type); }
  }

  // Clear the view.
  function resetView(cm) {
    cm.display.viewFrom = cm.display.viewTo = cm.doc.first;
    cm.display.view = [];
    cm.display.viewOffset = 0;
  }

  function viewCuttingPoint(cm, oldN, newN, dir) {
    var index = findViewIndex(cm, oldN), diff, view = cm.display.view;
    if (!sawCollapsedSpans || newN == cm.doc.first + cm.doc.size)
      { return {index: index, lineN: newN} }
    var n = cm.display.viewFrom;
    for (var i = 0; i < index; i++)
      { n += view[i].size; }
    if (n != oldN) {
      if (dir > 0) {
        if (index == view.length - 1) { return null }
        diff = (n + view[index].size) - oldN;
        index++;
      } else {
        diff = n - oldN;
      }
      oldN += diff; newN += diff;
    }
    while (visualLineNo(cm.doc, newN) != newN) {
      if (index == (dir < 0 ? 0 : view.length - 1)) { return null }
      newN += dir * view[index - (dir < 0 ? 1 : 0)].size;
      index += dir;
    }
    return {index: index, lineN: newN}
  }

  // Force the view to cover a given range, adding empty view element
  // or clipping off existing ones as needed.
  function adjustView(cm, from, to) {
    var display = cm.display, view = display.view;
    if (view.length == 0 || from >= display.viewTo || to <= display.viewFrom) {
      display.view = buildViewArray(cm, from, to);
      display.viewFrom = from;
    } else {
      if (display.viewFrom > from)
        { display.view = buildViewArray(cm, from, display.viewFrom).concat(display.view); }
      else if (display.viewFrom < from)
        { display.view = display.view.slice(findViewIndex(cm, from)); }
      display.viewFrom = from;
      if (display.viewTo < to)
        { display.view = display.view.concat(buildViewArray(cm, display.viewTo, to)); }
      else if (display.viewTo > to)
        { display.view = display.view.slice(0, findViewIndex(cm, to)); }
    }
    display.viewTo = to;
  }

  // Count the number of lines in the view whose DOM representation is
  // out of date (or nonexistent).
  function countDirtyView(cm) {
    var view = cm.display.view, dirty = 0;
    for (var i = 0; i < view.length; i++) {
      var lineView = view[i];
      if (!lineView.hidden && (!lineView.node || lineView.changes)) { ++dirty; }
    }
    return dirty
  }

  function updateSelection(cm) {
    cm.display.input.showSelection(cm.display.input.prepareSelection());
  }

  function prepareSelection(cm, primary) {
    if ( primary === void 0 ) primary = true;

    var doc = cm.doc, result = {};
    var curFragment = result.cursors = document.createDocumentFragment();
    var selFragment = result.selection = document.createDocumentFragment();

    for (var i = 0; i < doc.sel.ranges.length; i++) {
      if (!primary && i == doc.sel.primIndex) { continue }
      var range = doc.sel.ranges[i];
      if (range.from().line >= cm.display.viewTo || range.to().line < cm.display.viewFrom) { continue }
      var collapsed = range.empty();
      if (collapsed || cm.options.showCursorWhenSelecting)
        { drawSelectionCursor(cm, range.head, curFragment); }
      if (!collapsed)
        { drawSelectionRange(cm, range, selFragment); }
    }
    return result
  }

  // Draws a cursor for the given range
  function drawSelectionCursor(cm, head, output) {
    var pos = cursorCoords(cm, head, "div", null, null, !cm.options.singleCursorHeightPerLine);

    var cursor = output.appendChild(elt("div", "\u00a0", "CodeMirror-cursor"));
    cursor.style.left = pos.left + "px";
    cursor.style.top = pos.top + "px";
    cursor.style.height = Math.max(0, pos.bottom - pos.top) * cm.options.cursorHeight + "px";

    if (pos.other) {
      // Secondary cursor, shown when on a 'jump' in bi-directional text
      var otherCursor = output.appendChild(elt("div", "\u00a0", "CodeMirror-cursor CodeMirror-secondarycursor"));
      otherCursor.style.display = "";
      otherCursor.style.left = pos.other.left + "px";
      otherCursor.style.top = pos.other.top + "px";
      otherCursor.style.height = (pos.other.bottom - pos.other.top) * .85 + "px";
    }
  }

  function cmpCoords(a, b) { return a.top - b.top || a.left - b.left }

  // Draws the given range as a highlighted selection
  function drawSelectionRange(cm, range, output) {
    var display = cm.display, doc = cm.doc;
    var fragment = document.createDocumentFragment();
    var padding = paddingH(cm.display), leftSide = padding.left;
    var rightSide = Math.max(display.sizerWidth, displayWidth(cm) - display.sizer.offsetLeft) - padding.right;
    var docLTR = doc.direction == "ltr";

    function add(left, top, width, bottom) {
      if (top < 0) { top = 0; }
      top = Math.round(top);
      bottom = Math.round(bottom);
      fragment.appendChild(elt("div", null, "CodeMirror-selected", ("position: absolute; left: " + left + "px;\n                             top: " + top + "px; width: " + (width == null ? rightSide - left : width) + "px;\n                             height: " + (bottom - top) + "px")));
    }

    function drawForLine(line, fromArg, toArg) {
      var lineObj = getLine(doc, line);
      var lineLen = lineObj.text.length;
      var start, end;
      function coords(ch, bias) {
        return charCoords(cm, Pos(line, ch), "div", lineObj, bias)
      }

      function wrapX(pos, dir, side) {
        var extent = wrappedLineExtentChar(cm, lineObj, null, pos);
        var prop = (dir == "ltr") == (side == "after") ? "left" : "right";
        var ch = side == "after" ? extent.begin : extent.end - (/\s/.test(lineObj.text.charAt(extent.end - 1)) ? 2 : 1);
        return coords(ch, prop)[prop]
      }

      var order = getOrder(lineObj, doc.direction);
      iterateBidiSections(order, fromArg || 0, toArg == null ? lineLen : toArg, function (from, to, dir, i) {
        var ltr = dir == "ltr";
        var fromPos = coords(from, ltr ? "left" : "right");
        var toPos = coords(to - 1, ltr ? "right" : "left");

        var openStart = fromArg == null && from == 0, openEnd = toArg == null && to == lineLen;
        var first = i == 0, last = !order || i == order.length - 1;
        if (toPos.top - fromPos.top <= 3) { // Single line
          var openLeft = (docLTR ? openStart : openEnd) && first;
          var openRight = (docLTR ? openEnd : openStart) && last;
          var left = openLeft ? leftSide : (ltr ? fromPos : toPos).left;
          var right = openRight ? rightSide : (ltr ? toPos : fromPos).right;
          add(left, fromPos.top, right - left, fromPos.bottom);
        } else { // Multiple lines
          var topLeft, topRight, botLeft, botRight;
          if (ltr) {
            topLeft = docLTR && openStart && first ? leftSide : fromPos.left;
            topRight = docLTR ? rightSide : wrapX(from, dir, "before");
            botLeft = docLTR ? leftSide : wrapX(to, dir, "after");
            botRight = docLTR && openEnd && last ? rightSide : toPos.right;
          } else {
            topLeft = !docLTR ? leftSide : wrapX(from, dir, "before");
            topRight = !docLTR && openStart && first ? rightSide : fromPos.right;
            botLeft = !docLTR && openEnd && last ? leftSide : toPos.left;
            botRight = !docLTR ? rightSide : wrapX(to, dir, "after");
          }
          add(topLeft, fromPos.top, topRight - topLeft, fromPos.bottom);
          if (fromPos.bottom < toPos.top) { add(leftSide, fromPos.bottom, null, toPos.top); }
          add(botLeft, toPos.top, botRight - botLeft, toPos.bottom);
        }

        if (!start || cmpCoords(fromPos, start) < 0) { start = fromPos; }
        if (cmpCoords(toPos, start) < 0) { start = toPos; }
        if (!end || cmpCoords(fromPos, end) < 0) { end = fromPos; }
        if (cmpCoords(toPos, end) < 0) { end = toPos; }
      });
      return {start: start, end: end}
    }

    var sFrom = range.from(), sTo = range.to();
    if (sFrom.line == sTo.line) {
      drawForLine(sFrom.line, sFrom.ch, sTo.ch);
    } else {
      var fromLine = getLine(doc, sFrom.line), toLine = getLine(doc, sTo.line);
      var singleVLine = visualLine(fromLine) == visualLine(toLine);
      var leftEnd = drawForLine(sFrom.line, sFrom.ch, singleVLine ? fromLine.text.length + 1 : null).end;
      var rightStart = drawForLine(sTo.line, singleVLine ? 0 : null, sTo.ch).start;
      if (singleVLine) {
        if (leftEnd.top < rightStart.top - 2) {
          add(leftEnd.right, leftEnd.top, null, leftEnd.bottom);
          add(leftSide, rightStart.top, rightStart.left, rightStart.bottom);
        } else {
          add(leftEnd.right, leftEnd.top, rightStart.left - leftEnd.right, leftEnd.bottom);
        }
      }
      if (leftEnd.bottom < rightStart.top)
        { add(leftSide, leftEnd.bottom, null, rightStart.top); }
    }

    output.appendChild(fragment);
  }

  // Cursor-blinking
  function restartBlink(cm) {
    if (!cm.state.focused) { return }
    var display = cm.display;
    clearInterval(display.blinker);
    var on = true;
    display.cursorDiv.style.visibility = "";
    if (cm.options.cursorBlinkRate > 0)
      { display.blinker = setInterval(function () {
        if (!cm.hasFocus()) { onBlur(cm); }
        display.cursorDiv.style.visibility = (on = !on) ? "" : "hidden";
      }, cm.options.cursorBlinkRate); }
    else if (cm.options.cursorBlinkRate < 0)
      { display.cursorDiv.style.visibility = "hidden"; }
  }

  function ensureFocus(cm) {
    if (!cm.state.focused) { cm.display.input.focus(); onFocus(cm); }
  }

  function delayBlurEvent(cm) {
    cm.state.delayingBlurEvent = true;
    setTimeout(function () { if (cm.state.delayingBlurEvent) {
      cm.state.delayingBlurEvent = false;
      onBlur(cm);
    } }, 100);
  }

  function onFocus(cm, e) {
    if (cm.state.delayingBlurEvent) { cm.state.delayingBlurEvent = false; }

    if (cm.options.readOnly == "nocursor") { return }
    if (!cm.state.focused) {
      signal(cm, "focus", cm, e);
      cm.state.focused = true;
      addClass(cm.display.wrapper, "CodeMirror-focused");
      // This test prevents this from firing when a context
      // menu is closed (since the input reset would kill the
      // select-all detection hack)
      if (!cm.curOp && cm.display.selForContextMenu != cm.doc.sel) {
        cm.display.input.reset();
        if (webkit) { setTimeout(function () { return cm.display.input.reset(true); }, 20); } // Issue #1730
      }
      cm.display.input.receivedFocus();
    }
    restartBlink(cm);
  }
  function onBlur(cm, e) {
    if (cm.state.delayingBlurEvent) { return }

    if (cm.state.focused) {
      signal(cm, "blur", cm, e);
      cm.state.focused = false;
      rmClass(cm.display.wrapper, "CodeMirror-focused");
    }
    clearInterval(cm.display.blinker);
    setTimeout(function () { if (!cm.state.focused) { cm.display.shift = false; } }, 150);
  }

  // Read the actual heights of the rendered lines, and update their
  // stored heights to match.
  function updateHeightsInViewport(cm) {
    var display = cm.display;
    var prevBottom = display.lineDiv.offsetTop;
    for (var i = 0; i < display.view.length; i++) {
      var cur = display.view[i], wrapping = cm.options.lineWrapping;
      var height = (void 0), width = 0;
      if (cur.hidden) { continue }
      if (ie && ie_version < 8) {
        var bot = cur.node.offsetTop + cur.node.offsetHeight;
        height = bot - prevBottom;
        prevBottom = bot;
      } else {
        var box = cur.node.getBoundingClientRect();
        height = box.bottom - box.top;
        // Check that lines don't extend past the right of the current
        // editor width
        if (!wrapping && cur.text.firstChild)
          { width = cur.text.firstChild.getBoundingClientRect().right - box.left - 1; }
      }
      var diff = cur.line.height - height;
      if (diff > .005 || diff < -.005) {
        updateLineHeight(cur.line, height);
        updateWidgetHeight(cur.line);
        if (cur.rest) { for (var j = 0; j < cur.rest.length; j++)
          { updateWidgetHeight(cur.rest[j]); } }
      }
      if (width > cm.display.sizerWidth) {
        var chWidth = Math.ceil(width / charWidth(cm.display));
        if (chWidth > cm.display.maxLineLength) {
          cm.display.maxLineLength = chWidth;
          cm.display.maxLine = cur.line;
          cm.display.maxLineChanged = true;
        }
      }
    }
  }

  // Read and store the height of line widgets associated with the
  // given line.
  function updateWidgetHeight(line) {
    if (line.widgets) { for (var i = 0; i < line.widgets.length; ++i) {
      var w = line.widgets[i], parent = w.node.parentNode;
      if (parent) { w.height = parent.offsetHeight; }
    } }
  }

  // Compute the lines that are visible in a given viewport (defaults
  // the the current scroll position). viewport may contain top,
  // height, and ensure (see op.scrollToPos) properties.
  function visibleLines(display, doc, viewport) {
    var top = viewport && viewport.top != null ? Math.max(0, viewport.top) : display.scroller.scrollTop;
    top = Math.floor(top - paddingTop(display));
    var bottom = viewport && viewport.bottom != null ? viewport.bottom : top + display.wrapper.clientHeight;

    var from = lineAtHeight(doc, top), to = lineAtHeight(doc, bottom);
    // Ensure is a {from: {line, ch}, to: {line, ch}} object, and
    // forces those lines into the viewport (if possible).
    if (viewport && viewport.ensure) {
      var ensureFrom = viewport.ensure.from.line, ensureTo = viewport.ensure.to.line;
      if (ensureFrom < from) {
        from = ensureFrom;
        to = lineAtHeight(doc, heightAtLine(getLine(doc, ensureFrom)) + display.wrapper.clientHeight);
      } else if (Math.min(ensureTo, doc.lastLine()) >= to) {
        from = lineAtHeight(doc, heightAtLine(getLine(doc, ensureTo)) - display.wrapper.clientHeight);
        to = ensureTo;
      }
    }
    return {from: from, to: Math.max(to, from + 1)}
  }

  // SCROLLING THINGS INTO VIEW

  // If an editor sits on the top or bottom of the window, partially
  // scrolled out of view, this ensures that the cursor is visible.
  function maybeScrollWindow(cm, rect) {
    if (signalDOMEvent(cm, "scrollCursorIntoView")) { return }

    var display = cm.display, box = display.sizer.getBoundingClientRect(), doScroll = null;
    if (rect.top + box.top < 0) { doScroll = true; }
    else if (rect.bottom + box.top > (window.innerHeight || document.documentElement.clientHeight)) { doScroll = false; }
    if (doScroll != null && !phantom) {
      var scrollNode = elt("div", "\u200b", null, ("position: absolute;\n                         top: " + (rect.top - display.viewOffset - paddingTop(cm.display)) + "px;\n                         height: " + (rect.bottom - rect.top + scrollGap(cm) + display.barHeight) + "px;\n                         left: " + (rect.left) + "px; width: " + (Math.max(2, rect.right - rect.left)) + "px;"));
      cm.display.lineSpace.appendChild(scrollNode);
      scrollNode.scrollIntoView(doScroll);
      cm.display.lineSpace.removeChild(scrollNode);
    }
  }

  // Scroll a given position into view (immediately), verifying that
  // it actually became visible (as line heights are accurately
  // measured, the position of something may 'drift' during drawing).
  function scrollPosIntoView(cm, pos, end, margin) {
    if (margin == null) { margin = 0; }
    var rect;
    if (!cm.options.lineWrapping && pos == end) {
      // Set pos and end to the cursor positions around the character pos sticks to
      // If pos.sticky == "before", that is around pos.ch - 1, otherwise around pos.ch
      // If pos == Pos(_, 0, "before"), pos and end are unchanged
      pos = pos.ch ? Pos(pos.line, pos.sticky == "before" ? pos.ch - 1 : pos.ch, "after") : pos;
      end = pos.sticky == "before" ? Pos(pos.line, pos.ch + 1, "before") : pos;
    }
    for (var limit = 0; limit < 5; limit++) {
      var changed = false;
      var coords = cursorCoords(cm, pos);
      var endCoords = !end || end == pos ? coords : cursorCoords(cm, end);
      rect = {left: Math.min(coords.left, endCoords.left),
              top: Math.min(coords.top, endCoords.top) - margin,
              right: Math.max(coords.left, endCoords.left),
              bottom: Math.max(coords.bottom, endCoords.bottom) + margin};
      var scrollPos = calculateScrollPos(cm, rect);
      var startTop = cm.doc.scrollTop, startLeft = cm.doc.scrollLeft;
      if (scrollPos.scrollTop != null) {
        updateScrollTop(cm, scrollPos.scrollTop);
        if (Math.abs(cm.doc.scrollTop - startTop) > 1) { changed = true; }
      }
      if (scrollPos.scrollLeft != null) {
        setScrollLeft(cm, scrollPos.scrollLeft);
        if (Math.abs(cm.doc.scrollLeft - startLeft) > 1) { changed = true; }
      }
      if (!changed) { break }
    }
    return rect
  }

  // Scroll a given set of coordinates into view (immediately).
  function scrollIntoView(cm, rect) {
    var scrollPos = calculateScrollPos(cm, rect);
    if (scrollPos.scrollTop != null) { updateScrollTop(cm, scrollPos.scrollTop); }
    if (scrollPos.scrollLeft != null) { setScrollLeft(cm, scrollPos.scrollLeft); }
  }

  // Calculate a new scroll position needed to scroll the given
  // rectangle into view. Returns an object with scrollTop and
  // scrollLeft properties. When these are undefined, the
  // vertical/horizontal position does not need to be adjusted.
  function calculateScrollPos(cm, rect) {
    var display = cm.display, snapMargin = textHeight(cm.display);
    if (rect.top < 0) { rect.top = 0; }
    var screentop = cm.curOp && cm.curOp.scrollTop != null ? cm.curOp.scrollTop : display.scroller.scrollTop;
    var screen = displayHeight(cm), result = {};
    if (rect.bottom - rect.top > screen) { rect.bottom = rect.top + screen; }
    var docBottom = cm.doc.height + paddingVert(display);
    var atTop = rect.top < snapMargin, atBottom = rect.bottom > docBottom - snapMargin;
    if (rect.top < screentop) {
      result.scrollTop = atTop ? 0 : rect.top;
    } else if (rect.bottom > screentop + screen) {
      var newTop = Math.min(rect.top, (atBottom ? docBottom : rect.bottom) - screen);
      if (newTop != screentop) { result.scrollTop = newTop; }
    }

    var screenleft = cm.curOp && cm.curOp.scrollLeft != null ? cm.curOp.scrollLeft : display.scroller.scrollLeft;
    var screenw = displayWidth(cm) - (cm.options.fixedGutter ? display.gutters.offsetWidth : 0);
    var tooWide = rect.right - rect.left > screenw;
    if (tooWide) { rect.right = rect.left + screenw; }
    if (rect.left < 10)
      { result.scrollLeft = 0; }
    else if (rect.left < screenleft)
      { result.scrollLeft = Math.max(0, rect.left - (tooWide ? 0 : 10)); }
    else if (rect.right > screenw + screenleft - 3)
      { result.scrollLeft = rect.right + (tooWide ? 0 : 10) - screenw; }
    return result
  }

  // Store a relative adjustment to the scroll position in the current
  // operation (to be applied when the operation finishes).
  function addToScrollTop(cm, top) {
    if (top == null) { return }
    resolveScrollToPos(cm);
    cm.curOp.scrollTop = (cm.curOp.scrollTop == null ? cm.doc.scrollTop : cm.curOp.scrollTop) + top;
  }

  // Make sure that at the end of the operation the current cursor is
  // shown.
  function ensureCursorVisible(cm) {
    resolveScrollToPos(cm);
    var cur = cm.getCursor();
    cm.curOp.scrollToPos = {from: cur, to: cur, margin: cm.options.cursorScrollMargin};
  }

  function scrollToCoords(cm, x, y) {
    if (x != null || y != null) { resolveScrollToPos(cm); }
    if (x != null) { cm.curOp.scrollLeft = x; }
    if (y != null) { cm.curOp.scrollTop = y; }
  }

  function scrollToRange(cm, range) {
    resolveScrollToPos(cm);
    cm.curOp.scrollToPos = range;
  }

  // When an operation has its scrollToPos property set, and another
  // scroll action is applied before the end of the operation, this
  // 'simulates' scrolling that position into view in a cheap way, so
  // that the effect of intermediate scroll commands is not ignored.
  function resolveScrollToPos(cm) {
    var range = cm.curOp.scrollToPos;
    if (range) {
      cm.curOp.scrollToPos = null;
      var from = estimateCoords(cm, range.from), to = estimateCoords(cm, range.to);
      scrollToCoordsRange(cm, from, to, range.margin);
    }
  }

  function scrollToCoordsRange(cm, from, to, margin) {
    var sPos = calculateScrollPos(cm, {
      left: Math.min(from.left, to.left),
      top: Math.min(from.top, to.top) - margin,
      right: Math.max(from.right, to.right),
      bottom: Math.max(from.bottom, to.bottom) + margin
    });
    scrollToCoords(cm, sPos.scrollLeft, sPos.scrollTop);
  }

  // Sync the scrollable area and scrollbars, ensure the viewport
  // covers the visible area.
  function updateScrollTop(cm, val) {
    if (Math.abs(cm.doc.scrollTop - val) < 2) { return }
    if (!gecko) { updateDisplaySimple(cm, {top: val}); }
    setScrollTop(cm, val, true);
    if (gecko) { updateDisplaySimple(cm); }
    startWorker(cm, 100);
  }

  function setScrollTop(cm, val, forceScroll) {
    val = Math.max(0, Math.min(cm.display.scroller.scrollHeight - cm.display.scroller.clientHeight, val));
    if (cm.display.scroller.scrollTop == val && !forceScroll) { return }
    cm.doc.scrollTop = val;
    cm.display.scrollbars.setScrollTop(val);
    if (cm.display.scroller.scrollTop != val) { cm.display.scroller.scrollTop = val; }
  }

  // Sync scroller and scrollbar, ensure the gutter elements are
  // aligned.
  function setScrollLeft(cm, val, isScroller, forceScroll) {
    val = Math.max(0, Math.min(val, cm.display.scroller.scrollWidth - cm.display.scroller.clientWidth));
    if ((isScroller ? val == cm.doc.scrollLeft : Math.abs(cm.doc.scrollLeft - val) < 2) && !forceScroll) { return }
    cm.doc.scrollLeft = val;
    alignHorizontally(cm);
    if (cm.display.scroller.scrollLeft != val) { cm.display.scroller.scrollLeft = val; }
    cm.display.scrollbars.setScrollLeft(val);
  }

  // SCROLLBARS

  // Prepare DOM reads needed to update the scrollbars. Done in one
  // shot to minimize update/measure roundtrips.
  function measureForScrollbars(cm) {
    var d = cm.display, gutterW = d.gutters.offsetWidth;
    var docH = Math.round(cm.doc.height + paddingVert(cm.display));
    return {
      clientHeight: d.scroller.clientHeight,
      viewHeight: d.wrapper.clientHeight,
      scrollWidth: d.scroller.scrollWidth, clientWidth: d.scroller.clientWidth,
      viewWidth: d.wrapper.clientWidth,
      barLeft: cm.options.fixedGutter ? gutterW : 0,
      docHeight: docH,
      scrollHeight: docH + scrollGap(cm) + d.barHeight,
      nativeBarWidth: d.nativeBarWidth,
      gutterWidth: gutterW
    }
  }

  var NativeScrollbars = function(place, scroll, cm) {
    this.cm = cm;
    var vert = this.vert = elt("div", [elt("div", null, null, "min-width: 1px")], "CodeMirror-vscrollbar");
    var horiz = this.horiz = elt("div", [elt("div", null, null, "height: 100%; min-height: 1px")], "CodeMirror-hscrollbar");
    vert.tabIndex = horiz.tabIndex = -1;
    place(vert); place(horiz);

    on(vert, "scroll", function () {
      if (vert.clientHeight) { scroll(vert.scrollTop, "vertical"); }
    });
    on(horiz, "scroll", function () {
      if (horiz.clientWidth) { scroll(horiz.scrollLeft, "horizontal"); }
    });

    this.checkedZeroWidth = false;
    // Need to set a minimum width to see the scrollbar on IE7 (but must not set it on IE8).
    if (ie && ie_version < 8) { this.horiz.style.minHeight = this.vert.style.minWidth = "18px"; }
  };

  NativeScrollbars.prototype.update = function (measure) {
    var needsH = measure.scrollWidth > measure.clientWidth + 1;
    var needsV = measure.scrollHeight > measure.clientHeight + 1;
    var sWidth = measure.nativeBarWidth;

    if (needsV) {
      this.vert.style.display = "block";
      this.vert.style.bottom = needsH ? sWidth + "px" : "0";
      var totalHeight = measure.viewHeight - (needsH ? sWidth : 0);
      // A bug in IE8 can cause this value to be negative, so guard it.
      this.vert.firstChild.style.height =
        Math.max(0, measure.scrollHeight - measure.clientHeight + totalHeight) + "px";
    } else {
      this.vert.style.display = "";
      this.vert.firstChild.style.height = "0";
    }

    if (needsH) {
      this.horiz.style.display = "block";
      this.horiz.style.right = needsV ? sWidth + "px" : "0";
      this.horiz.style.left = measure.barLeft + "px";
      var totalWidth = measure.viewWidth - measure.barLeft - (needsV ? sWidth : 0);
      this.horiz.firstChild.style.width =
        Math.max(0, measure.scrollWidth - measure.clientWidth + totalWidth) + "px";
    } else {
      this.horiz.style.display = "";
      this.horiz.firstChild.style.width = "0";
    }

    if (!this.checkedZeroWidth && measure.clientHeight > 0) {
      if (sWidth == 0) { this.zeroWidthHack(); }
      this.checkedZeroWidth = true;
    }

    return {right: needsV ? sWidth : 0, bottom: needsH ? sWidth : 0}
  };

  NativeScrollbars.prototype.setScrollLeft = function (pos) {
    if (this.horiz.scrollLeft != pos) { this.horiz.scrollLeft = pos; }
    if (this.disableHoriz) { this.enableZeroWidthBar(this.horiz, this.disableHoriz, "horiz"); }
  };

  NativeScrollbars.prototype.setScrollTop = function (pos) {
    if (this.vert.scrollTop != pos) { this.vert.scrollTop = pos; }
    if (this.disableVert) { this.enableZeroWidthBar(this.vert, this.disableVert, "vert"); }
  };

  NativeScrollbars.prototype.zeroWidthHack = function () {
    var w = mac && !mac_geMountainLion ? "12px" : "18px";
    this.horiz.style.height = this.vert.style.width = w;
    this.horiz.style.pointerEvents = this.vert.style.pointerEvents = "none";
    this.disableHoriz = new Delayed;
    this.disableVert = new Delayed;
  };

  NativeScrollbars.prototype.enableZeroWidthBar = function (bar, delay, type) {
    bar.style.pointerEvents = "auto";
    function maybeDisable() {
      // To find out whether the scrollbar is still visible, we
      // check whether the element under the pixel in the bottom
      // right corner of the scrollbar box is the scrollbar box
      // itself (when the bar is still visible) or its filler child
      // (when the bar is hidden). If it is still visible, we keep
      // it enabled, if it's hidden, we disable pointer events.
      var box = bar.getBoundingClientRect();
      var elt = type == "vert" ? document.elementFromPoint(box.right - 1, (box.top + box.bottom) / 2)
          : document.elementFromPoint((box.right + box.left) / 2, box.bottom - 1);
      if (elt != bar) { bar.style.pointerEvents = "none"; }
      else { delay.set(1000, maybeDisable); }
    }
    delay.set(1000, maybeDisable);
  };

  NativeScrollbars.prototype.clear = function () {
    var parent = this.horiz.parentNode;
    parent.removeChild(this.horiz);
    parent.removeChild(this.vert);
  };

  var NullScrollbars = function () {};

  NullScrollbars.prototype.update = function () { return {bottom: 0, right: 0} };
  NullScrollbars.prototype.setScrollLeft = function () {};
  NullScrollbars.prototype.setScrollTop = function () {};
  NullScrollbars.prototype.clear = function () {};

  function updateScrollbars(cm, measure) {
    if (!measure) { measure = measureForScrollbars(cm); }
    var startWidth = cm.display.barWidth, startHeight = cm.display.barHeight;
    updateScrollbarsInner(cm, measure);
    for (var i = 0; i < 4 && startWidth != cm.display.barWidth || startHeight != cm.display.barHeight; i++) {
      if (startWidth != cm.display.barWidth && cm.options.lineWrapping)
        { updateHeightsInViewport(cm); }
      updateScrollbarsInner(cm, measureForScrollbars(cm));
      startWidth = cm.display.barWidth; startHeight = cm.display.barHeight;
    }
  }

  // Re-synchronize the fake scrollbars with the actual size of the
  // content.
  function updateScrollbarsInner(cm, measure) {
    var d = cm.display;
    var sizes = d.scrollbars.update(measure);

    d.sizer.style.paddingRight = (d.barWidth = sizes.right) + "px";
    d.sizer.style.paddingBottom = (d.barHeight = sizes.bottom) + "px";
    d.heightForcer.style.borderBottom = sizes.bottom + "px solid transparent";

    if (sizes.right && sizes.bottom) {
      d.scrollbarFiller.style.display = "block";
      d.scrollbarFiller.style.height = sizes.bottom + "px";
      d.scrollbarFiller.style.width = sizes.right + "px";
    } else { d.scrollbarFiller.style.display = ""; }
    if (sizes.bottom && cm.options.coverGutterNextToScrollbar && cm.options.fixedGutter) {
      d.gutterFiller.style.display = "block";
      d.gutterFiller.style.height = sizes.bottom + "px";
      d.gutterFiller.style.width = measure.gutterWidth + "px";
    } else { d.gutterFiller.style.display = ""; }
  }

  var scrollbarModel = {"native": NativeScrollbars, "null": NullScrollbars};

  function initScrollbars(cm) {
    if (cm.display.scrollbars) {
      cm.display.scrollbars.clear();
      if (cm.display.scrollbars.addClass)
        { rmClass(cm.display.wrapper, cm.display.scrollbars.addClass); }
    }

    cm.display.scrollbars = new scrollbarModel[cm.options.scrollbarStyle](function (node) {
      cm.display.wrapper.insertBefore(node, cm.display.scrollbarFiller);
      // Prevent clicks in the scrollbars from killing focus
      on(node, "mousedown", function () {
        if (cm.state.focused) { setTimeout(function () { return cm.display.input.focus(); }, 0); }
      });
      node.setAttribute("cm-not-content", "true");
    }, function (pos, axis) {
      if (axis == "horizontal") { setScrollLeft(cm, pos); }
      else { updateScrollTop(cm, pos); }
    }, cm);
    if (cm.display.scrollbars.addClass)
      { addClass(cm.display.wrapper, cm.display.scrollbars.addClass); }
  }

  // Operations are used to wrap a series of changes to the editor
  // state in such a way that each change won't have to update the
  // cursor and display (which would be awkward, slow, and
  // error-prone). Instead, display updates are batched and then all
  // combined and executed at once.

  var nextOpId = 0;
  // Start a new operation.
  function startOperation(cm) {
    cm.curOp = {
      cm: cm,
      viewChanged: false,      // Flag that indicates that lines might need to be redrawn
      startHeight: cm.doc.height, // Used to detect need to update scrollbar
      forceUpdate: false,      // Used to force a redraw
      updateInput: 0,       // Whether to reset the input textarea
      typing: false,           // Whether this reset should be careful to leave existing text (for compositing)
      changeObjs: null,        // Accumulated changes, for firing change events
      cursorActivityHandlers: null, // Set of handlers to fire cursorActivity on
      cursorActivityCalled: 0, // Tracks which cursorActivity handlers have been called already
      selectionChanged: false, // Whether the selection needs to be redrawn
      updateMaxLine: false,    // Set when the widest line needs to be determined anew
      scrollLeft: null, scrollTop: null, // Intermediate scroll position, not pushed to DOM yet
      scrollToPos: null,       // Used to scroll to a specific position
      focus: false,
      id: ++nextOpId           // Unique ID
    };
    pushOperation(cm.curOp);
  }

  // Finish an operation, updating the display and signalling delayed events
  function endOperation(cm) {
    var op = cm.curOp;
    if (op) { finishOperation(op, function (group) {
      for (var i = 0; i < group.ops.length; i++)
        { group.ops[i].cm.curOp = null; }
      endOperations(group);
    }); }
  }

  // The DOM updates done when an operation finishes are batched so
  // that the minimum number of relayouts are required.
  function endOperations(group) {
    var ops = group.ops;
    for (var i = 0; i < ops.length; i++) // Read DOM
      { endOperation_R1(ops[i]); }
    for (var i$1 = 0; i$1 < ops.length; i$1++) // Write DOM (maybe)
      { endOperation_W1(ops[i$1]); }
    for (var i$2 = 0; i$2 < ops.length; i$2++) // Read DOM
      { endOperation_R2(ops[i$2]); }
    for (var i$3 = 0; i$3 < ops.length; i$3++) // Write DOM (maybe)
      { endOperation_W2(ops[i$3]); }
    for (var i$4 = 0; i$4 < ops.length; i$4++) // Read DOM
      { endOperation_finish(ops[i$4]); }
  }

  function endOperation_R1(op) {
    var cm = op.cm, display = cm.display;
    maybeClipScrollbars(cm);
    if (op.updateMaxLine) { findMaxLine(cm); }

    op.mustUpdate = op.viewChanged || op.forceUpdate || op.scrollTop != null ||
      op.scrollToPos && (op.scrollToPos.from.line < display.viewFrom ||
                         op.scrollToPos.to.line >= display.viewTo) ||
      display.maxLineChanged && cm.options.lineWrapping;
    op.update = op.mustUpdate &&
      new DisplayUpdate(cm, op.mustUpdate && {top: op.scrollTop, ensure: op.scrollToPos}, op.forceUpdate);
  }

  function endOperation_W1(op) {
    op.updatedDisplay = op.mustUpdate && updateDisplayIfNeeded(op.cm, op.update);
  }

  function endOperation_R2(op) {
    var cm = op.cm, display = cm.display;
    if (op.updatedDisplay) { updateHeightsInViewport(cm); }

    op.barMeasure = measureForScrollbars(cm);

    // If the max line changed since it was last measured, measure it,
    // and ensure the document's width matches it.
    // updateDisplay_W2 will use these properties to do the actual resizing
    if (display.maxLineChanged && !cm.options.lineWrapping) {
      op.adjustWidthTo = measureChar(cm, display.maxLine, display.maxLine.text.length).left + 3;
      cm.display.sizerWidth = op.adjustWidthTo;
      op.barMeasure.scrollWidth =
        Math.max(display.scroller.clientWidth, display.sizer.offsetLeft + op.adjustWidthTo + scrollGap(cm) + cm.display.barWidth);
      op.maxScrollLeft = Math.max(0, display.sizer.offsetLeft + op.adjustWidthTo - displayWidth(cm));
    }

    if (op.updatedDisplay || op.selectionChanged)
      { op.preparedSelection = display.input.prepareSelection(); }
  }

  function endOperation_W2(op) {
    var cm = op.cm;

    if (op.adjustWidthTo != null) {
      cm.display.sizer.style.minWidth = op.adjustWidthTo + "px";
      if (op.maxScrollLeft < cm.doc.scrollLeft)
        { setScrollLeft(cm, Math.min(cm.display.scroller.scrollLeft, op.maxScrollLeft), true); }
      cm.display.maxLineChanged = false;
    }

    var takeFocus = op.focus && op.focus == activeElt();
    if (op.preparedSelection)
      { cm.display.input.showSelection(op.preparedSelection, takeFocus); }
    if (op.updatedDisplay || op.startHeight != cm.doc.height)
      { updateScrollbars(cm, op.barMeasure); }
    if (op.updatedDisplay)
      { setDocumentHeight(cm, op.barMeasure); }

    if (op.selectionChanged) { restartBlink(cm); }

    if (cm.state.focused && op.updateInput)
      { cm.display.input.reset(op.typing); }
    if (takeFocus) { ensureFocus(op.cm); }
  }

  function endOperation_finish(op) {
    var cm = op.cm, display = cm.display, doc = cm.doc;

    if (op.updatedDisplay) { postUpdateDisplay(cm, op.update); }

    // Abort mouse wheel delta measurement, when scrolling explicitly
    if (display.wheelStartX != null && (op.scrollTop != null || op.scrollLeft != null || op.scrollToPos))
      { display.wheelStartX = display.wheelStartY = null; }

    // Propagate the scroll position to the actual DOM scroller
    if (op.scrollTop != null) { setScrollTop(cm, op.scrollTop, op.forceScroll); }

    if (op.scrollLeft != null) { setScrollLeft(cm, op.scrollLeft, true, true); }
    // If we need to scroll a specific position into view, do so.
    if (op.scrollToPos) {
      var rect = scrollPosIntoView(cm, clipPos(doc, op.scrollToPos.from),
                                   clipPos(doc, op.scrollToPos.to), op.scrollToPos.margin);
      maybeScrollWindow(cm, rect);
    }

    // Fire events for markers that are hidden/unidden by editing or
    // undoing
    var hidden = op.maybeHiddenMarkers, unhidden = op.maybeUnhiddenMarkers;
    if (hidden) { for (var i = 0; i < hidden.length; ++i)
      { if (!hidden[i].lines.length) { signal(hidden[i], "hide"); } } }
    if (unhidden) { for (var i$1 = 0; i$1 < unhidden.length; ++i$1)
      { if (unhidden[i$1].lines.length) { signal(unhidden[i$1], "unhide"); } } }

    if (display.wrapper.offsetHeight)
      { doc.scrollTop = cm.display.scroller.scrollTop; }

    // Fire change events, and delayed event handlers
    if (op.changeObjs)
      { signal(cm, "changes", cm, op.changeObjs); }
    if (op.update)
      { op.update.finish(); }
  }

  // Run the given function in an operation
  function runInOp(cm, f) {
    if (cm.curOp) { return f() }
    startOperation(cm);
    try { return f() }
    finally { endOperation(cm); }
  }
  // Wraps a function in an operation. Returns the wrapped function.
  function operation(cm, f) {
    return function() {
      if (cm.curOp) { return f.apply(cm, arguments) }
      startOperation(cm);
      try { return f.apply(cm, arguments) }
      finally { endOperation(cm); }
    }
  }
  // Used to add methods to editor and doc instances, wrapping them in
  // operations.
  function methodOp(f) {
    return function() {
      if (this.curOp) { return f.apply(this, arguments) }
      startOperation(this);
      try { return f.apply(this, arguments) }
      finally { endOperation(this); }
    }
  }
  function docMethodOp(f) {
    return function() {
      var cm = this.cm;
      if (!cm || cm.curOp) { return f.apply(this, arguments) }
      startOperation(cm);
      try { return f.apply(this, arguments) }
      finally { endOperation(cm); }
    }
  }

  // HIGHLIGHT WORKER

  function startWorker(cm, time) {
    if (cm.doc.highlightFrontier < cm.display.viewTo)
      { cm.state.highlight.set(time, bind(highlightWorker, cm)); }
  }

  function highlightWorker(cm) {
    var doc = cm.doc;
    if (doc.highlightFrontier >= cm.display.viewTo) { return }
    var end = +new Date + cm.options.workTime;
    var context = getContextBefore(cm, doc.highlightFrontier);
    var changedLines = [];

    doc.iter(context.line, Math.min(doc.first + doc.size, cm.display.viewTo + 500), function (line) {
      if (context.line >= cm.display.viewFrom) { // Visible
        var oldStyles = line.styles;
        var resetState = line.text.length > cm.options.maxHighlightLength ? copyState(doc.mode, context.state) : null;
        var highlighted = highlightLine(cm, line, context, true);
        if (resetState) { context.state = resetState; }
        line.styles = highlighted.styles;
        var oldCls = line.styleClasses, newCls = highlighted.classes;
        if (newCls) { line.styleClasses = newCls; }
        else if (oldCls) { line.styleClasses = null; }
        var ischange = !oldStyles || oldStyles.length != line.styles.length ||
          oldCls != newCls && (!oldCls || !newCls || oldCls.bgClass != newCls.bgClass || oldCls.textClass != newCls.textClass);
        for (var i = 0; !ischange && i < oldStyles.length; ++i) { ischange = oldStyles[i] != line.styles[i]; }
        if (ischange) { changedLines.push(context.line); }
        line.stateAfter = context.save();
        context.nextLine();
      } else {
        if (line.text.length <= cm.options.maxHighlightLength)
          { processLine(cm, line.text, context); }
        line.stateAfter = context.line % 5 == 0 ? context.save() : null;
        context.nextLine();
      }
      if (+new Date > end) {
        startWorker(cm, cm.options.workDelay);
        return true
      }
    });
    doc.highlightFrontier = context.line;
    doc.modeFrontier = Math.max(doc.modeFrontier, context.line);
    if (changedLines.length) { runInOp(cm, function () {
      for (var i = 0; i < changedLines.length; i++)
        { regLineChange(cm, changedLines[i], "text"); }
    }); }
  }

  // DISPLAY DRAWING

  var DisplayUpdate = function(cm, viewport, force) {
    var display = cm.display;

    this.viewport = viewport;
    // Store some values that we'll need later (but don't want to force a relayout for)
    this.visible = visibleLines(display, cm.doc, viewport);
    this.editorIsHidden = !display.wrapper.offsetWidth;
    this.wrapperHeight = display.wrapper.clientHeight;
    this.wrapperWidth = display.wrapper.clientWidth;
    this.oldDisplayWidth = displayWidth(cm);
    this.force = force;
    this.dims = getDimensions(cm);
    this.events = [];
  };

  DisplayUpdate.prototype.signal = function (emitter, type) {
    if (hasHandler(emitter, type))
      { this.events.push(arguments); }
  };
  DisplayUpdate.prototype.finish = function () {
    for (var i = 0; i < this.events.length; i++)
      { signal.apply(null, this.events[i]); }
  };

  function maybeClipScrollbars(cm) {
    var display = cm.display;
    if (!display.scrollbarsClipped && display.scroller.offsetWidth) {
      display.nativeBarWidth = display.scroller.offsetWidth - display.scroller.clientWidth;
      display.heightForcer.style.height = scrollGap(cm) + "px";
      display.sizer.style.marginBottom = -display.nativeBarWidth + "px";
      display.sizer.style.borderRightWidth = scrollGap(cm) + "px";
      display.scrollbarsClipped = true;
    }
  }

  function selectionSnapshot(cm) {
    if (cm.hasFocus()) { return null }
    var active = activeElt();
    if (!active || !contains(cm.display.lineDiv, active)) { return null }
    var result = {activeElt: active};
    if (window.getSelection) {
      var sel = window.getSelection();
      if (sel.anchorNode && sel.extend && contains(cm.display.lineDiv, sel.anchorNode)) {
        result.anchorNode = sel.anchorNode;
        result.anchorOffset = sel.anchorOffset;
        result.focusNode = sel.focusNode;
        result.focusOffset = sel.focusOffset;
      }
    }
    return result
  }

  function restoreSelection(snapshot) {
    if (!snapshot || !snapshot.activeElt || snapshot.activeElt == activeElt()) { return }
    snapshot.activeElt.focus();
    if (!/^(INPUT|TEXTAREA)$/.test(snapshot.activeElt.nodeName) &&
        snapshot.anchorNode && contains(document.body, snapshot.anchorNode) && contains(document.body, snapshot.focusNode)) {
      var sel = window.getSelection(), range = document.createRange();
      range.setEnd(snapshot.anchorNode, snapshot.anchorOffset);
      range.collapse(false);
      sel.removeAllRanges();
      sel.addRange(range);
      sel.extend(snapshot.focusNode, snapshot.focusOffset);
    }
  }

  // Does the actual updating of the line display. Bails out
  // (returning false) when there is nothing to be done and forced is
  // false.
  function updateDisplayIfNeeded(cm, update) {
    var display = cm.display, doc = cm.doc;

    if (update.editorIsHidden) {
      resetView(cm);
      return false
    }

    // Bail out if the visible area is already rendered and nothing changed.
    if (!update.force &&
        update.visible.from >= display.viewFrom && update.visible.to <= display.viewTo &&
        (display.updateLineNumbers == null || display.updateLineNumbers >= display.viewTo) &&
        display.renderedView == display.view && countDirtyView(cm) == 0)
      { return false }

    if (maybeUpdateLineNumberWidth(cm)) {
      resetView(cm);
      update.dims = getDimensions(cm);
    }

    // Compute a suitable new viewport (from & to)
    var end = doc.first + doc.size;
    var from = Math.max(update.visible.from - cm.options.viewportMargin, doc.first);
    var to = Math.min(end, update.visible.to + cm.options.viewportMargin);
    if (display.viewFrom < from && from - display.viewFrom < 20) { from = Math.max(doc.first, display.viewFrom); }
    if (display.viewTo > to && display.viewTo - to < 20) { to = Math.min(end, display.viewTo); }
    if (sawCollapsedSpans) {
      from = visualLineNo(cm.doc, from);
      to = visualLineEndNo(cm.doc, to);
    }

    var different = from != display.viewFrom || to != display.viewTo ||
      display.lastWrapHeight != update.wrapperHeight || display.lastWrapWidth != update.wrapperWidth;
    adjustView(cm, from, to);

    display.viewOffset = heightAtLine(getLine(cm.doc, display.viewFrom));
    // Position the mover div to align with the current scroll position
    cm.display.mover.style.top = display.viewOffset + "px";

    var toUpdate = countDirtyView(cm);
    if (!different && toUpdate == 0 && !update.force && display.renderedView == display.view &&
        (display.updateLineNumbers == null || display.updateLineNumbers >= display.viewTo))
      { return false }

    // For big changes, we hide the enclosing element during the
    // update, since that speeds up the operations on most browsers.
    var selSnapshot = selectionSnapshot(cm);
    if (toUpdate > 4) { display.lineDiv.style.display = "none"; }
    patchDisplay(cm, display.updateLineNumbers, update.dims);
    if (toUpdate > 4) { display.lineDiv.style.display = ""; }
    display.renderedView = display.view;
    // There might have been a widget with a focused element that got
    // hidden or updated, if so re-focus it.
    restoreSelection(selSnapshot);

    // Prevent selection and cursors from interfering with the scroll
    // width and height.
    removeChildren(display.cursorDiv);
    removeChildren(display.selectionDiv);
    display.gutters.style.height = display.sizer.style.minHeight = 0;

    if (different) {
      display.lastWrapHeight = update.wrapperHeight;
      display.lastWrapWidth = update.wrapperWidth;
      startWorker(cm, 400);
    }

    display.updateLineNumbers = null;

    return true
  }

  function postUpdateDisplay(cm, update) {
    var viewport = update.viewport;

    for (var first = true;; first = false) {
      if (!first || !cm.options.lineWrapping || update.oldDisplayWidth == displayWidth(cm)) {
        // Clip forced viewport to actual scrollable area.
        if (viewport && viewport.top != null)
          { viewport = {top: Math.min(cm.doc.height + paddingVert(cm.display) - displayHeight(cm), viewport.top)}; }
        // Updated line heights might result in the drawn area not
        // actually covering the viewport. Keep looping until it does.
        update.visible = visibleLines(cm.display, cm.doc, viewport);
        if (update.visible.from >= cm.display.viewFrom && update.visible.to <= cm.display.viewTo)
          { break }
      } else if (first) {
        update.visible = visibleLines(cm.display, cm.doc, viewport);
      }
      if (!updateDisplayIfNeeded(cm, update)) { break }
      updateHeightsInViewport(cm);
      var barMeasure = measureForScrollbars(cm);
      updateSelection(cm);
      updateScrollbars(cm, barMeasure);
      setDocumentHeight(cm, barMeasure);
      update.force = false;
    }

    update.signal(cm, "update", cm);
    if (cm.display.viewFrom != cm.display.reportedViewFrom || cm.display.viewTo != cm.display.reportedViewTo) {
      update.signal(cm, "viewportChange", cm, cm.display.viewFrom, cm.display.viewTo);
      cm.display.reportedViewFrom = cm.display.viewFrom; cm.display.reportedViewTo = cm.display.viewTo;
    }
  }

  function updateDisplaySimple(cm, viewport) {
    var update = new DisplayUpdate(cm, viewport);
    if (updateDisplayIfNeeded(cm, update)) {
      updateHeightsInViewport(cm);
      postUpdateDisplay(cm, update);
      var barMeasure = measureForScrollbars(cm);
      updateSelection(cm);
      updateScrollbars(cm, barMeasure);
      setDocumentHeight(cm, barMeasure);
      update.finish();
    }
  }

  // Sync the actual display DOM structure with display.view, removing
  // nodes for lines that are no longer in view, and creating the ones
  // that are not there yet, and updating the ones that are out of
  // date.
  function patchDisplay(cm, updateNumbersFrom, dims) {
    var display = cm.display, lineNumbers = cm.options.lineNumbers;
    var container = display.lineDiv, cur = container.firstChild;

    function rm(node) {
      var next = node.nextSibling;
      // Works around a throw-scroll bug in OS X Webkit
      if (webkit && mac && cm.display.currentWheelTarget == node)
        { node.style.display = "none"; }
      else
        { node.parentNode.removeChild(node); }
      return next
    }

    var view = display.view, lineN = display.viewFrom;
    // Loop over the elements in the view, syncing cur (the DOM nodes
    // in display.lineDiv) with the view as we go.
    for (var i = 0; i < view.length; i++) {
      var lineView = view[i];
      if (lineView.hidden) ; else if (!lineView.node || lineView.node.parentNode != container) { // Not drawn yet
        var node = buildLineElement(cm, lineView, lineN, dims);
        container.insertBefore(node, cur);
      } else { // Already drawn
        while (cur != lineView.node) { cur = rm(cur); }
        var updateNumber = lineNumbers && updateNumbersFrom != null &&
          updateNumbersFrom <= lineN && lineView.lineNumber;
        if (lineView.changes) {
          if (indexOf(lineView.changes, "gutter") > -1) { updateNumber = false; }
          updateLineForChanges(cm, lineView, lineN, dims);
        }
        if (updateNumber) {
          removeChildren(lineView.lineNumber);
          lineView.lineNumber.appendChild(document.createTextNode(lineNumberFor(cm.options, lineN)));
        }
        cur = lineView.node.nextSibling;
      }
      lineN += lineView.size;
    }
    while (cur) { cur = rm(cur); }
  }

  function updateGutterSpace(display) {
    var width = display.gutters.offsetWidth;
    display.sizer.style.marginLeft = width + "px";
  }

  function setDocumentHeight(cm, measure) {
    cm.display.sizer.style.minHeight = measure.docHeight + "px";
    cm.display.heightForcer.style.top = measure.docHeight + "px";
    cm.display.gutters.style.height = (measure.docHeight + cm.display.barHeight + scrollGap(cm)) + "px";
  }

  // Re-align line numbers and gutter marks to compensate for
  // horizontal scrolling.
  function alignHorizontally(cm) {
    var display = cm.display, view = display.view;
    if (!display.alignWidgets && (!display.gutters.firstChild || !cm.options.fixedGutter)) { return }
    var comp = compensateForHScroll(display) - display.scroller.scrollLeft + cm.doc.scrollLeft;
    var gutterW = display.gutters.offsetWidth, left = comp + "px";
    for (var i = 0; i < view.length; i++) { if (!view[i].hidden) {
      if (cm.options.fixedGutter) {
        if (view[i].gutter)
          { view[i].gutter.style.left = left; }
        if (view[i].gutterBackground)
          { view[i].gutterBackground.style.left = left; }
      }
      var align = view[i].alignable;
      if (align) { for (var j = 0; j < align.length; j++)
        { align[j].style.left = left; } }
    } }
    if (cm.options.fixedGutter)
      { display.gutters.style.left = (comp + gutterW) + "px"; }
  }

  // Used to ensure that the line number gutter is still the right
  // size for the current document size. Returns true when an update
  // is needed.
  function maybeUpdateLineNumberWidth(cm) {
    if (!cm.options.lineNumbers) { return false }
    var doc = cm.doc, last = lineNumberFor(cm.options, doc.first + doc.size - 1), display = cm.display;
    if (last.length != display.lineNumChars) {
      var test = display.measure.appendChild(elt("div", [elt("div", last)],
                                                 "CodeMirror-linenumber CodeMirror-gutter-elt"));
      var innerW = test.firstChild.offsetWidth, padding = test.offsetWidth - innerW;
      display.lineGutter.style.width = "";
      display.lineNumInnerWidth = Math.max(innerW, display.lineGutter.offsetWidth - padding) + 1;
      display.lineNumWidth = display.lineNumInnerWidth + padding;
      display.lineNumChars = display.lineNumInnerWidth ? last.length : -1;
      display.lineGutter.style.width = display.lineNumWidth + "px";
      updateGutterSpace(cm.display);
      return true
    }
    return false
  }

  function getGutters(gutters, lineNumbers) {
    var result = [], sawLineNumbers = false;
    for (var i = 0; i < gutters.length; i++) {
      var name = gutters[i], style = null;
      if (typeof name != "string") { style = name.style; name = name.className; }
      if (name == "CodeMirror-linenumbers") {
        if (!lineNumbers) { continue }
        else { sawLineNumbers = true; }
      }
      result.push({className: name, style: style});
    }
    if (lineNumbers && !sawLineNumbers) { result.push({className: "CodeMirror-linenumbers", style: null}); }
    return result
  }

  // Rebuild the gutter elements, ensure the margin to the left of the
  // code matches their width.
  function renderGutters(display) {
    var gutters = display.gutters, specs = display.gutterSpecs;
    removeChildren(gutters);
    display.lineGutter = null;
    for (var i = 0; i < specs.length; ++i) {
      var ref = specs[i];
      var className = ref.className;
      var style = ref.style;
      var gElt = gutters.appendChild(elt("div", null, "CodeMirror-gutter " + className));
      if (style) { gElt.style.cssText = style; }
      if (className == "CodeMirror-linenumbers") {
        display.lineGutter = gElt;
        gElt.style.width = (display.lineNumWidth || 1) + "px";
      }
    }
    gutters.style.display = specs.length ? "" : "none";
    updateGutterSpace(display);
  }

  function updateGutters(cm) {
    renderGutters(cm.display);
    regChange(cm);
    alignHorizontally(cm);
  }

  // The display handles the DOM integration, both for input reading
  // and content drawing. It holds references to DOM nodes and
  // display-related state.

  function Display(place, doc, input, options) {
    var d = this;
    this.input = input;

    // Covers bottom-right square when both scrollbars are present.
    d.scrollbarFiller = elt("div", null, "CodeMirror-scrollbar-filler");
    d.scrollbarFiller.setAttribute("cm-not-content", "true");
    // Covers bottom of gutter when coverGutterNextToScrollbar is on
    // and h scrollbar is present.
    d.gutterFiller = elt("div", null, "CodeMirror-gutter-filler");
    d.gutterFiller.setAttribute("cm-not-content", "true");
    // Will contain the actual code, positioned to cover the viewport.
    d.lineDiv = eltP("div", null, "CodeMirror-code");
    // Elements are added to these to represent selection and cursors.
    d.selectionDiv = elt("div", null, null, "position: relative; z-index: 1");
    d.cursorDiv = elt("div", null, "CodeMirror-cursors");
    // A visibility: hidden element used to find the size of things.
    d.measure = elt("div", null, "CodeMirror-measure");
    // When lines outside of the viewport are measured, they are drawn in this.
    d.lineMeasure = elt("div", null, "CodeMirror-measure");
    // Wraps everything that needs to exist inside the vertically-padded coordinate system
    d.lineSpace = eltP("div", [d.measure, d.lineMeasure, d.selectionDiv, d.cursorDiv, d.lineDiv],
                      null, "position: relative; outline: none");
    var lines = eltP("div", [d.lineSpace], "CodeMirror-lines");
    // Moved around its parent to cover visible view.
    d.mover = elt("div", [lines], null, "position: relative");
    // Set to the height of the document, allowing scrolling.
    d.sizer = elt("div", [d.mover], "CodeMirror-sizer");
    d.sizerWidth = null;
    // Behavior of elts with overflow: auto and padding is
    // inconsistent across browsers. This is used to ensure the
    // scrollable area is big enough.
    d.heightForcer = elt("div", null, null, "position: absolute; height: " + scrollerGap + "px; width: 1px;");
    // Will contain the gutters, if any.
    d.gutters = elt("div", null, "CodeMirror-gutters");
    d.lineGutter = null;
    // Actual scrollable element.
    d.scroller = elt("div", [d.sizer, d.heightForcer, d.gutters], "CodeMirror-scroll");
    d.scroller.setAttribute("tabIndex", "-1");
    // The element in which the editor lives.
    d.wrapper = elt("div", [d.scrollbarFiller, d.gutterFiller, d.scroller], "CodeMirror");

    // Work around IE7 z-index bug (not perfect, hence IE7 not really being supported)
    if (ie && ie_version < 8) { d.gutters.style.zIndex = -1; d.scroller.style.paddingRight = 0; }
    if (!webkit && !(gecko && mobile)) { d.scroller.draggable = true; }

    if (place) {
      if (place.appendChild) { place.appendChild(d.wrapper); }
      else { place(d.wrapper); }
    }

    // Current rendered range (may be bigger than the view window).
    d.viewFrom = d.viewTo = doc.first;
    d.reportedViewFrom = d.reportedViewTo = doc.first;
    // Information about the rendered lines.
    d.view = [];
    d.renderedView = null;
    // Holds info about a single rendered line when it was rendered
    // for measurement, while not in view.
    d.externalMeasured = null;
    // Empty space (in pixels) above the view
    d.viewOffset = 0;
    d.lastWrapHeight = d.lastWrapWidth = 0;
    d.updateLineNumbers = null;

    d.nativeBarWidth = d.barHeight = d.barWidth = 0;
    d.scrollbarsClipped = false;

    // Used to only resize the line number gutter when necessary (when
    // the amount of lines crosses a boundary that makes its width change)
    d.lineNumWidth = d.lineNumInnerWidth = d.lineNumChars = null;
    // Set to true when a non-horizontal-scrolling line widget is
    // added. As an optimization, line widget aligning is skipped when
    // this is false.
    d.alignWidgets = false;

    d.cachedCharWidth = d.cachedTextHeight = d.cachedPaddingH = null;

    // Tracks the maximum line length so that the horizontal scrollbar
    // can be kept static when scrolling.
    d.maxLine = null;
    d.maxLineLength = 0;
    d.maxLineChanged = false;

    // Used for measuring wheel scrolling granularity
    d.wheelDX = d.wheelDY = d.wheelStartX = d.wheelStartY = null;

    // True when shift is held down.
    d.shift = false;

    // Used to track whether anything happened since the context menu
    // was opened.
    d.selForContextMenu = null;

    d.activeTouch = null;

    d.gutterSpecs = getGutters(options.gutters, options.lineNumbers);
    renderGutters(d);

    input.init(d);
  }

  // Since the delta values reported on mouse wheel events are
  // unstandardized between browsers and even browser versions, and
  // generally horribly unpredictable, this code starts by measuring
  // the scroll effect that the first few mouse wheel events have,
  // and, from that, detects the way it can convert deltas to pixel
  // offsets afterwards.
  //
  // The reason we want to know the amount a wheel event will scroll
  // is that it gives us a chance to update the display before the
  // actual scrolling happens, reducing flickering.

  var wheelSamples = 0, wheelPixelsPerUnit = null;
  // Fill in a browser-detected starting value on browsers where we
  // know one. These don't have to be accurate -- the result of them
  // being wrong would just be a slight flicker on the first wheel
  // scroll (if it is large enough).
  if (ie) { wheelPixelsPerUnit = -.53; }
  else if (gecko) { wheelPixelsPerUnit = 15; }
  else if (chrome) { wheelPixelsPerUnit = -.7; }
  else if (safari) { wheelPixelsPerUnit = -1/3; }

  function wheelEventDelta(e) {
    var dx = e.wheelDeltaX, dy = e.wheelDeltaY;
    if (dx == null && e.detail && e.axis == e.HORIZONTAL_AXIS) { dx = e.detail; }
    if (dy == null && e.detail && e.axis == e.VERTICAL_AXIS) { dy = e.detail; }
    else if (dy == null) { dy = e.wheelDelta; }
    return {x: dx, y: dy}
  }
  function wheelEventPixels(e) {
    var delta = wheelEventDelta(e);
    delta.x *= wheelPixelsPerUnit;
    delta.y *= wheelPixelsPerUnit;
    return delta
  }

  function onScrollWheel(cm, e) {
    var delta = wheelEventDelta(e), dx = delta.x, dy = delta.y;

    var display = cm.display, scroll = display.scroller;
    // Quit if there's nothing to scroll here
    var canScrollX = scroll.scrollWidth > scroll.clientWidth;
    var canScrollY = scroll.scrollHeight > scroll.clientHeight;
    if (!(dx && canScrollX || dy && canScrollY)) { return }

    // Webkit browsers on OS X abort momentum scrolls when the target
    // of the scroll event is removed from the scrollable element.
    // This hack (see related code in patchDisplay) makes sure the
    // element is kept around.
    if (dy && mac && webkit) {
      outer: for (var cur = e.target, view = display.view; cur != scroll; cur = cur.parentNode) {
        for (var i = 0; i < view.length; i++) {
          if (view[i].node == cur) {
            cm.display.currentWheelTarget = cur;
            break outer
          }
        }
      }
    }

    // On some browsers, horizontal scrolling will cause redraws to
    // happen before the gutter has been realigned, causing it to
    // wriggle around in a most unseemly way. When we have an
    // estimated pixels/delta value, we just handle horizontal
    // scrolling entirely here. It'll be slightly off from native, but
    // better than glitching out.
    if (dx && !gecko && !presto && wheelPixelsPerUnit != null) {
      if (dy && canScrollY)
        { updateScrollTop(cm, Math.max(0, scroll.scrollTop + dy * wheelPixelsPerUnit)); }
      setScrollLeft(cm, Math.max(0, scroll.scrollLeft + dx * wheelPixelsPerUnit));
      // Only prevent default scrolling if vertical scrolling is
      // actually possible. Otherwise, it causes vertical scroll
      // jitter on OSX trackpads when deltaX is small and deltaY
      // is large (issue #3579)
      if (!dy || (dy && canScrollY))
        { e_preventDefault(e); }
      display.wheelStartX = null; // Abort measurement, if in progress
      return
    }

    // 'Project' the visible viewport to cover the area that is being
    // scrolled into view (if we know enough to estimate it).
    if (dy && wheelPixelsPerUnit != null) {
      var pixels = dy * wheelPixelsPerUnit;
      var top = cm.doc.scrollTop, bot = top + display.wrapper.clientHeight;
      if (pixels < 0) { top = Math.max(0, top + pixels - 50); }
      else { bot = Math.min(cm.doc.height, bot + pixels + 50); }
      updateDisplaySimple(cm, {top: top, bottom: bot});
    }

    if (wheelSamples < 20) {
      if (display.wheelStartX == null) {
        display.wheelStartX = scroll.scrollLeft; display.wheelStartY = scroll.scrollTop;
        display.wheelDX = dx; display.wheelDY = dy;
        setTimeout(function () {
          if (display.wheelStartX == null) { return }
          var movedX = scroll.scrollLeft - display.wheelStartX;
          var movedY = scroll.scrollTop - display.wheelStartY;
          var sample = (movedY && display.wheelDY && movedY / display.wheelDY) ||
            (movedX && display.wheelDX && movedX / display.wheelDX);
          display.wheelStartX = display.wheelStartY = null;
          if (!sample) { return }
          wheelPixelsPerUnit = (wheelPixelsPerUnit * wheelSamples + sample) / (wheelSamples + 1);
          ++wheelSamples;
        }, 200);
      } else {
        display.wheelDX += dx; display.wheelDY += dy;
      }
    }
  }

  // Selection objects are immutable. A new one is created every time
  // the selection changes. A selection is one or more non-overlapping
  // (and non-touching) ranges, sorted, and an integer that indicates
  // which one is the primary selection (the one that's scrolled into
  // view, that getCursor returns, etc).
  var Selection = function(ranges, primIndex) {
    this.ranges = ranges;
    this.primIndex = primIndex;
  };

  Selection.prototype.primary = function () { return this.ranges[this.primIndex] };

  Selection.prototype.equals = function (other) {
    if (other == this) { return true }
    if (other.primIndex != this.primIndex || other.ranges.length != this.ranges.length) { return false }
    for (var i = 0; i < this.ranges.length; i++) {
      var here = this.ranges[i], there = other.ranges[i];
      if (!equalCursorPos(here.anchor, there.anchor) || !equalCursorPos(here.head, there.head)) { return false }
    }
    return true
  };

  Selection.prototype.deepCopy = function () {
    var out = [];
    for (var i = 0; i < this.ranges.length; i++)
      { out[i] = new Range(copyPos(this.ranges[i].anchor), copyPos(this.ranges[i].head)); }
    return new Selection(out, this.primIndex)
  };

  Selection.prototype.somethingSelected = function () {
    for (var i = 0; i < this.ranges.length; i++)
      { if (!this.ranges[i].empty()) { return true } }
    return false
  };

  Selection.prototype.contains = function (pos, end) {
    if (!end) { end = pos; }
    for (var i = 0; i < this.ranges.length; i++) {
      var range = this.ranges[i];
      if (cmp(end, range.from()) >= 0 && cmp(pos, range.to()) <= 0)
        { return i }
    }
    return -1
  };

  var Range = function(anchor, head) {
    this.anchor = anchor; this.head = head;
  };

  Range.prototype.from = function () { return minPos(this.anchor, this.head) };
  Range.prototype.to = function () { return maxPos(this.anchor, this.head) };
  Range.prototype.empty = function () { return this.head.line == this.anchor.line && this.head.ch == this.anchor.ch };

  // Take an unsorted, potentially overlapping set of ranges, and
  // build a selection out of it. 'Consumes' ranges array (modifying
  // it).
  function normalizeSelection(cm, ranges, primIndex) {
    var mayTouch = cm && cm.options.selectionsMayTouch;
    var prim = ranges[primIndex];
    ranges.sort(function (a, b) { return cmp(a.from(), b.from()); });
    primIndex = indexOf(ranges, prim);
    for (var i = 1; i < ranges.length; i++) {
      var cur = ranges[i], prev = ranges[i - 1];
      var diff = cmp(prev.to(), cur.from());
      if (mayTouch && !cur.empty() ? diff > 0 : diff >= 0) {
        var from = minPos(prev.from(), cur.from()), to = maxPos(prev.to(), cur.to());
        var inv = prev.empty() ? cur.from() == cur.head : prev.from() == prev.head;
        if (i <= primIndex) { --primIndex; }
        ranges.splice(--i, 2, new Range(inv ? to : from, inv ? from : to));
      }
    }
    return new Selection(ranges, primIndex)
  }

  function simpleSelection(anchor, head) {
    return new Selection([new Range(anchor, head || anchor)], 0)
  }

  // Compute the position of the end of a change (its 'to' property
  // refers to the pre-change end).
  function changeEnd(change) {
    if (!change.text) { return change.to }
    return Pos(change.from.line + change.text.length - 1,
               lst(change.text).length + (change.text.length == 1 ? change.from.ch : 0))
  }

  // Adjust a position to refer to the post-change position of the
  // same text, or the end of the change if the change covers it.
  function adjustForChange(pos, change) {
    if (cmp(pos, change.from) < 0) { return pos }
    if (cmp(pos, change.to) <= 0) { return changeEnd(change) }

    var line = pos.line + change.text.length - (change.to.line - change.from.line) - 1, ch = pos.ch;
    if (pos.line == change.to.line) { ch += changeEnd(change).ch - change.to.ch; }
    return Pos(line, ch)
  }

  function computeSelAfterChange(doc, change) {
    var out = [];
    for (var i = 0; i < doc.sel.ranges.length; i++) {
      var range = doc.sel.ranges[i];
      out.push(new Range(adjustForChange(range.anchor, change),
                         adjustForChange(range.head, change)));
    }
    return normalizeSelection(doc.cm, out, doc.sel.primIndex)
  }

  function offsetPos(pos, old, nw) {
    if (pos.line == old.line)
      { return Pos(nw.line, pos.ch - old.ch + nw.ch) }
    else
      { return Pos(nw.line + (pos.line - old.line), pos.ch) }
  }

  // Used by replaceSelections to allow moving the selection to the
  // start or around the replaced test. Hint may be "start" or "around".
  function computeReplacedSel(doc, changes, hint) {
    var out = [];
    var oldPrev = Pos(doc.first, 0), newPrev = oldPrev;
    for (var i = 0; i < changes.length; i++) {
      var change = changes[i];
      var from = offsetPos(change.from, oldPrev, newPrev);
      var to = offsetPos(changeEnd(change), oldPrev, newPrev);
      oldPrev = change.to;
      newPrev = to;
      if (hint == "around") {
        var range = doc.sel.ranges[i], inv = cmp(range.head, range.anchor) < 0;
        out[i] = new Range(inv ? to : from, inv ? from : to);
      } else {
        out[i] = new Range(from, from);
      }
    }
    return new Selection(out, doc.sel.primIndex)
  }

  // Used to get the editor into a consistent state again when options change.

  function loadMode(cm) {
    cm.doc.mode = getMode(cm.options, cm.doc.modeOption);
    resetModeState(cm);
  }

  function resetModeState(cm) {
    cm.doc.iter(function (line) {
      if (line.stateAfter) { line.stateAfter = null; }
      if (line.styles) { line.styles = null; }
    });
    cm.doc.modeFrontier = cm.doc.highlightFrontier = cm.doc.first;
    startWorker(cm, 100);
    cm.state.modeGen++;
    if (cm.curOp) { regChange(cm); }
  }

  // DOCUMENT DATA STRUCTURE

  // By default, updates that start and end at the beginning of a line
  // are treated specially, in order to make the association of line
  // widgets and marker elements with the text behave more intuitive.
  function isWholeLineUpdate(doc, change) {
    return change.from.ch == 0 && change.to.ch == 0 && lst(change.text) == "" &&
      (!doc.cm || doc.cm.options.wholeLineUpdateBefore)
  }

  // Perform a change on the document data structure.
  function updateDoc(doc, change, markedSpans, estimateHeight) {
    function spansFor(n) {return markedSpans ? markedSpans[n] : null}
    function update(line, text, spans) {
      updateLine(line, text, spans, estimateHeight);
      signalLater(line, "change", line, change);
    }
    function linesFor(start, end) {
      var result = [];
      for (var i = start; i < end; ++i)
        { result.push(new Line(text[i], spansFor(i), estimateHeight)); }
      return result
    }

    var from = change.from, to = change.to, text = change.text;
    var firstLine = getLine(doc, from.line), lastLine = getLine(doc, to.line);
    var lastText = lst(text), lastSpans = spansFor(text.length - 1), nlines = to.line - from.line;

    // Adjust the line structure
    if (change.full) {
      doc.insert(0, linesFor(0, text.length));
      doc.remove(text.length, doc.size - text.length);
    } else if (isWholeLineUpdate(doc, change)) {
      // This is a whole-line replace. Treated specially to make
      // sure line objects move the way they are supposed to.
      var added = linesFor(0, text.length - 1);
      update(lastLine, lastLine.text, lastSpans);
      if (nlines) { doc.remove(from.line, nlines); }
      if (added.length) { doc.insert(from.line, added); }
    } else if (firstLine == lastLine) {
      if (text.length == 1) {
        update(firstLine, firstLine.text.slice(0, from.ch) + lastText + firstLine.text.slice(to.ch), lastSpans);
      } else {
        var added$1 = linesFor(1, text.length - 1);
        added$1.push(new Line(lastText + firstLine.text.slice(to.ch), lastSpans, estimateHeight));
        update(firstLine, firstLine.text.slice(0, from.ch) + text[0], spansFor(0));
        doc.insert(from.line + 1, added$1);
      }
    } else if (text.length == 1) {
      update(firstLine, firstLine.text.slice(0, from.ch) + text[0] + lastLine.text.slice(to.ch), spansFor(0));
      doc.remove(from.line + 1, nlines);
    } else {
      update(firstLine, firstLine.text.slice(0, from.ch) + text[0], spansFor(0));
      update(lastLine, lastText + lastLine.text.slice(to.ch), lastSpans);
      var added$2 = linesFor(1, text.length - 1);
      if (nlines > 1) { doc.remove(from.line + 1, nlines - 1); }
      doc.insert(from.line + 1, added$2);
    }

    signalLater(doc, "change", doc, change);
  }

  // Call f for all linked documents.
  function linkedDocs(doc, f, sharedHistOnly) {
    function propagate(doc, skip, sharedHist) {
      if (doc.linked) { for (var i = 0; i < doc.linked.length; ++i) {
        var rel = doc.linked[i];
        if (rel.doc == skip) { continue }
        var shared = sharedHist && rel.sharedHist;
        if (sharedHistOnly && !shared) { continue }
        f(rel.doc, shared);
        propagate(rel.doc, doc, shared);
      } }
    }
    propagate(doc, null, true);
  }

  // Attach a document to an editor.
  function attachDoc(cm, doc) {
    if (doc.cm) { throw new Error("This document is already in use.") }
    cm.doc = doc;
    doc.cm = cm;
    estimateLineHeights(cm);
    loadMode(cm);
    setDirectionClass(cm);
    if (!cm.options.lineWrapping) { findMaxLine(cm); }
    cm.options.mode = doc.modeOption;
    regChange(cm);
  }

  function setDirectionClass(cm) {
  (cm.doc.direction == "rtl" ? addClass : rmClass)(cm.display.lineDiv, "CodeMirror-rtl");
  }

  function directionChanged(cm) {
    runInOp(cm, function () {
      setDirectionClass(cm);
      regChange(cm);
    });
  }

  function History(startGen) {
    // Arrays of change events and selections. Doing something adds an
    // event to done and clears undo. Undoing moves events from done
    // to undone, redoing moves them in the other direction.
    this.done = []; this.undone = [];
    this.undoDepth = Infinity;
    // Used to track when changes can be merged into a single undo
    // event
    this.lastModTime = this.lastSelTime = 0;
    this.lastOp = this.lastSelOp = null;
    this.lastOrigin = this.lastSelOrigin = null;
    // Used by the isClean() method
    this.generation = this.maxGeneration = startGen || 1;
  }

  // Create a history change event from an updateDoc-style change
  // object.
  function historyChangeFromChange(doc, change) {
    var histChange = {from: copyPos(change.from), to: changeEnd(change), text: getBetween(doc, change.from, change.to)};
    attachLocalSpans(doc, histChange, change.from.line, change.to.line + 1);
    linkedDocs(doc, function (doc) { return attachLocalSpans(doc, histChange, change.from.line, change.to.line + 1); }, true);
    return histChange
  }

  // Pop all selection events off the end of a history array. Stop at
  // a change event.
  function clearSelectionEvents(array) {
    while (array.length) {
      var last = lst(array);
      if (last.ranges) { array.pop(); }
      else { break }
    }
  }

  // Find the top change event in the history. Pop off selection
  // events that are in the way.
  function lastChangeEvent(hist, force) {
    if (force) {
      clearSelectionEvents(hist.done);
      return lst(hist.done)
    } else if (hist.done.length && !lst(hist.done).ranges) {
      return lst(hist.done)
    } else if (hist.done.length > 1 && !hist.done[hist.done.length - 2].ranges) {
      hist.done.pop();
      return lst(hist.done)
    }
  }

  // Register a change in the history. Merges changes that are within
  // a single operation, or are close together with an origin that
  // allows merging (starting with "+") into a single event.
  function addChangeToHistory(doc, change, selAfter, opId) {
    var hist = doc.history;
    hist.undone.length = 0;
    var time = +new Date, cur;
    var last;

    if ((hist.lastOp == opId ||
         hist.lastOrigin == change.origin && change.origin &&
         ((change.origin.charAt(0) == "+" && hist.lastModTime > time - (doc.cm ? doc.cm.options.historyEventDelay : 500)) ||
          change.origin.charAt(0) == "*")) &&
        (cur = lastChangeEvent(hist, hist.lastOp == opId))) {
      // Merge this change into the last event
      last = lst(cur.changes);
      if (cmp(change.from, change.to) == 0 && cmp(change.from, last.to) == 0) {
        // Optimized case for simple insertion -- don't want to add
        // new changesets for every character typed
        last.to = changeEnd(change);
      } else {
        // Add new sub-event
        cur.changes.push(historyChangeFromChange(doc, change));
      }
    } else {
      // Can not be merged, start a new event.
      var before = lst(hist.done);
      if (!before || !before.ranges)
        { pushSelectionToHistory(doc.sel, hist.done); }
      cur = {changes: [historyChangeFromChange(doc, change)],
             generation: hist.generation};
      hist.done.push(cur);
      while (hist.done.length > hist.undoDepth) {
        hist.done.shift();
        if (!hist.done[0].ranges) { hist.done.shift(); }
      }
    }
    hist.done.push(selAfter);
    hist.generation = ++hist.maxGeneration;
    hist.lastModTime = hist.lastSelTime = time;
    hist.lastOp = hist.lastSelOp = opId;
    hist.lastOrigin = hist.lastSelOrigin = change.origin;

    if (!last) { signal(doc, "historyAdded"); }
  }

  function selectionEventCanBeMerged(doc, origin, prev, sel) {
    var ch = origin.charAt(0);
    return ch == "*" ||
      ch == "+" &&
      prev.ranges.length == sel.ranges.length &&
      prev.somethingSelected() == sel.somethingSelected() &&
      new Date - doc.history.lastSelTime <= (doc.cm ? doc.cm.options.historyEventDelay : 500)
  }

  // Called whenever the selection changes, sets the new selection as
  // the pending selection in the history, and pushes the old pending
  // selection into the 'done' array when it was significantly
  // different (in number of selected ranges, emptiness, or time).
  function addSelectionToHistory(doc, sel, opId, options) {
    var hist = doc.history, origin = options && options.origin;

    // A new event is started when the previous origin does not match
    // the current, or the origins don't allow matching. Origins
    // starting with * are always merged, those starting with + are
    // merged when similar and close together in time.
    if (opId == hist.lastSelOp ||
        (origin && hist.lastSelOrigin == origin &&
         (hist.lastModTime == hist.lastSelTime && hist.lastOrigin == origin ||
          selectionEventCanBeMerged(doc, origin, lst(hist.done), sel))))
      { hist.done[hist.done.length - 1] = sel; }
    else
      { pushSelectionToHistory(sel, hist.done); }

    hist.lastSelTime = +new Date;
    hist.lastSelOrigin = origin;
    hist.lastSelOp = opId;
    if (options && options.clearRedo !== false)
      { clearSelectionEvents(hist.undone); }
  }

  function pushSelectionToHistory(sel, dest) {
    var top = lst(dest);
    if (!(top && top.ranges && top.equals(sel)))
      { dest.push(sel); }
  }

  // Used to store marked span information in the history.
  function attachLocalSpans(doc, change, from, to) {
    var existing = change["spans_" + doc.id], n = 0;
    doc.iter(Math.max(doc.first, from), Math.min(doc.first + doc.size, to), function (line) {
      if (line.markedSpans)
        { (existing || (existing = change["spans_" + doc.id] = {}))[n] = line.markedSpans; }
      ++n;
    });
  }

  // When un/re-doing restores text containing marked spans, those
  // that have been explicitly cleared should not be restored.
  function removeClearedSpans(spans) {
    if (!spans) { return null }
    var out;
    for (var i = 0; i < spans.length; ++i) {
      if (spans[i].marker.explicitlyCleared) { if (!out) { out = spans.slice(0, i); } }
      else if (out) { out.push(spans[i]); }
    }
    return !out ? spans : out.length ? out : null
  }

  // Retrieve and filter the old marked spans stored in a change event.
  function getOldSpans(doc, change) {
    var found = change["spans_" + doc.id];
    if (!found) { return null }
    var nw = [];
    for (var i = 0; i < change.text.length; ++i)
      { nw.push(removeClearedSpans(found[i])); }
    return nw
  }

  // Used for un/re-doing changes from the history. Combines the
  // result of computing the existing spans with the set of spans that
  // existed in the history (so that deleting around a span and then
  // undoing brings back the span).
  function mergeOldSpans(doc, change) {
    var old = getOldSpans(doc, change);
    var stretched = stretchSpansOverChange(doc, change);
    if (!old) { return stretched }
    if (!stretched) { return old }

    for (var i = 0; i < old.length; ++i) {
      var oldCur = old[i], stretchCur = stretched[i];
      if (oldCur && stretchCur) {
        spans: for (var j = 0; j < stretchCur.length; ++j) {
          var span = stretchCur[j];
          for (var k = 0; k < oldCur.length; ++k)
            { if (oldCur[k].marker == span.marker) { continue spans } }
          oldCur.push(span);
        }
      } else if (stretchCur) {
        old[i] = stretchCur;
      }
    }
    return old
  }

  // Used both to provide a JSON-safe object in .getHistory, and, when
  // detaching a document, to split the history in two
  function copyHistoryArray(events, newGroup, instantiateSel) {
    var copy = [];
    for (var i = 0; i < events.length; ++i) {
      var event = events[i];
      if (event.ranges) {
        copy.push(instantiateSel ? Selection.prototype.deepCopy.call(event) : event);
        continue
      }
      var changes = event.changes, newChanges = [];
      copy.push({changes: newChanges});
      for (var j = 0; j < changes.length; ++j) {
        var change = changes[j], m = (void 0);
        newChanges.push({from: change.from, to: change.to, text: change.text});
        if (newGroup) { for (var prop in change) { if (m = prop.match(/^spans_(\d+)$/)) {
          if (indexOf(newGroup, Number(m[1])) > -1) {
            lst(newChanges)[prop] = change[prop];
            delete change[prop];
          }
        } } }
      }
    }
    return copy
  }

  // The 'scroll' parameter given to many of these indicated whether
  // the new cursor position should be scrolled into view after
  // modifying the selection.

  // If shift is held or the extend flag is set, extends a range to
  // include a given position (and optionally a second position).
  // Otherwise, simply returns the range between the given positions.
  // Used for cursor motion and such.
  function extendRange(range, head, other, extend) {
    if (extend) {
      var anchor = range.anchor;
      if (other) {
        var posBefore = cmp(head, anchor) < 0;
        if (posBefore != (cmp(other, anchor) < 0)) {
          anchor = head;
          head = other;
        } else if (posBefore != (cmp(head, other) < 0)) {
          head = other;
        }
      }
      return new Range(anchor, head)
    } else {
      return new Range(other || head, head)
    }
  }

  // Extend the primary selection range, discard the rest.
  function extendSelection(doc, head, other, options, extend) {
    if (extend == null) { extend = doc.cm && (doc.cm.display.shift || doc.extend); }
    setSelection(doc, new Selection([extendRange(doc.sel.primary(), head, other, extend)], 0), options);
  }

  // Extend all selections (pos is an array of selections with length
  // equal the number of selections)
  function extendSelections(doc, heads, options) {
    var out = [];
    var extend = doc.cm && (doc.cm.display.shift || doc.extend);
    for (var i = 0; i < doc.sel.ranges.length; i++)
      { out[i] = extendRange(doc.sel.ranges[i], heads[i], null, extend); }
    var newSel = normalizeSelection(doc.cm, out, doc.sel.primIndex);
    setSelection(doc, newSel, options);
  }

  // Updates a single range in the selection.
  function replaceOneSelection(doc, i, range, options) {
    var ranges = doc.sel.ranges.slice(0);
    ranges[i] = range;
    setSelection(doc, normalizeSelection(doc.cm, ranges, doc.sel.primIndex), options);
  }

  // Reset the selection to a single range.
  function setSimpleSelection(doc, anchor, head, options) {
    setSelection(doc, simpleSelection(anchor, head), options);
  }

  // Give beforeSelectionChange handlers a change to influence a
  // selection update.
  function filterSelectionChange(doc, sel, options) {
    var obj = {
      ranges: sel.ranges,
      update: function(ranges) {
        this.ranges = [];
        for (var i = 0; i < ranges.length; i++)
          { this.ranges[i] = new Range(clipPos(doc, ranges[i].anchor),
                                     clipPos(doc, ranges[i].head)); }
      },
      origin: options && options.origin
    };
    signal(doc, "beforeSelectionChange", doc, obj);
    if (doc.cm) { signal(doc.cm, "beforeSelectionChange", doc.cm, obj); }
    if (obj.ranges != sel.ranges) { return normalizeSelection(doc.cm, obj.ranges, obj.ranges.length - 1) }
    else { return sel }
  }

  function setSelectionReplaceHistory(doc, sel, options) {
    var done = doc.history.done, last = lst(done);
    if (last && last.ranges) {
      done[done.length - 1] = sel;
      setSelectionNoUndo(doc, sel, options);
    } else {
      setSelection(doc, sel, options);
    }
  }

  // Set a new selection.
  function setSelection(doc, sel, options) {
    setSelectionNoUndo(doc, sel, options);
    addSelectionToHistory(doc, doc.sel, doc.cm ? doc.cm.curOp.id : NaN, options);
  }

  function setSelectionNoUndo(doc, sel, options) {
    if (hasHandler(doc, "beforeSelectionChange") || doc.cm && hasHandler(doc.cm, "beforeSelectionChange"))
      { sel = filterSelectionChange(doc, sel, options); }

    var bias = options && options.bias ||
      (cmp(sel.primary().head, doc.sel.primary().head) < 0 ? -1 : 1);
    setSelectionInner(doc, skipAtomicInSelection(doc, sel, bias, true));

    if (!(options && options.scroll === false) && doc.cm)
      { ensureCursorVisible(doc.cm); }
  }

  function setSelectionInner(doc, sel) {
    if (sel.equals(doc.sel)) { return }

    doc.sel = sel;

    if (doc.cm) {
      doc.cm.curOp.updateInput = 1;
      doc.cm.curOp.selectionChanged = true;
      signalCursorActivity(doc.cm);
    }
    signalLater(doc, "cursorActivity", doc);
  }

  // Verify that the selection does not partially select any atomic
  // marked ranges.
  function reCheckSelection(doc) {
    setSelectionInner(doc, skipAtomicInSelection(doc, doc.sel, null, false));
  }

  // Return a selection that does not partially select any atomic
  // ranges.
  function skipAtomicInSelection(doc, sel, bias, mayClear) {
    var out;
    for (var i = 0; i < sel.ranges.length; i++) {
      var range = sel.ranges[i];
      var old = sel.ranges.length == doc.sel.ranges.length && doc.sel.ranges[i];
      var newAnchor = skipAtomic(doc, range.anchor, old && old.anchor, bias, mayClear);
      var newHead = skipAtomic(doc, range.head, old && old.head, bias, mayClear);
      if (out || newAnchor != range.anchor || newHead != range.head) {
        if (!out) { out = sel.ranges.slice(0, i); }
        out[i] = new Range(newAnchor, newHead);
      }
    }
    return out ? normalizeSelection(doc.cm, out, sel.primIndex) : sel
  }

  function skipAtomicInner(doc, pos, oldPos, dir, mayClear) {
    var line = getLine(doc, pos.line);
    if (line.markedSpans) { for (var i = 0; i < line.markedSpans.length; ++i) {
      var sp = line.markedSpans[i], m = sp.marker;

      // Determine if we should prevent the cursor being placed to the left/right of an atomic marker
      // Historically this was determined using the inclusiveLeft/Right option, but the new way to control it
      // is with selectLeft/Right
      var preventCursorLeft = ("selectLeft" in m) ? !m.selectLeft : m.inclusiveLeft;
      var preventCursorRight = ("selectRight" in m) ? !m.selectRight : m.inclusiveRight;

      if ((sp.from == null || (preventCursorLeft ? sp.from <= pos.ch : sp.from < pos.ch)) &&
          (sp.to == null || (preventCursorRight ? sp.to >= pos.ch : sp.to > pos.ch))) {
        if (mayClear) {
          signal(m, "beforeCursorEnter");
          if (m.explicitlyCleared) {
            if (!line.markedSpans) { break }
            else {--i; continue}
          }
        }
        if (!m.atomic) { continue }

        if (oldPos) {
          var near = m.find(dir < 0 ? 1 : -1), diff = (void 0);
          if (dir < 0 ? preventCursorRight : preventCursorLeft)
            { near = movePos(doc, near, -dir, near && near.line == pos.line ? line : null); }
          if (near && near.line == pos.line && (diff = cmp(near, oldPos)) && (dir < 0 ? diff < 0 : diff > 0))
            { return skipAtomicInner(doc, near, pos, dir, mayClear) }
        }

        var far = m.find(dir < 0 ? -1 : 1);
        if (dir < 0 ? preventCursorLeft : preventCursorRight)
          { far = movePos(doc, far, dir, far.line == pos.line ? line : null); }
        return far ? skipAtomicInner(doc, far, pos, dir, mayClear) : null
      }
    } }
    return pos
  }

  // Ensure a given position is not inside an atomic range.
  function skipAtomic(doc, pos, oldPos, bias, mayClear) {
    var dir = bias || 1;
    var found = skipAtomicInner(doc, pos, oldPos, dir, mayClear) ||
        (!mayClear && skipAtomicInner(doc, pos, oldPos, dir, true)) ||
        skipAtomicInner(doc, pos, oldPos, -dir, mayClear) ||
        (!mayClear && skipAtomicInner(doc, pos, oldPos, -dir, true));
    if (!found) {
      doc.cantEdit = true;
      return Pos(doc.first, 0)
    }
    return found
  }

  function movePos(doc, pos, dir, line) {
    if (dir < 0 && pos.ch == 0) {
      if (pos.line > doc.first) { return clipPos(doc, Pos(pos.line - 1)) }
      else { return null }
    } else if (dir > 0 && pos.ch == (line || getLine(doc, pos.line)).text.length) {
      if (pos.line < doc.first + doc.size - 1) { return Pos(pos.line + 1, 0) }
      else { return null }
    } else {
      return new Pos(pos.line, pos.ch + dir)
    }
  }

  function selectAll(cm) {
    cm.setSelection(Pos(cm.firstLine(), 0), Pos(cm.lastLine()), sel_dontScroll);
  }

  // UPDATING

  // Allow "beforeChange" event handlers to influence a change
  function filterChange(doc, change, update) {
    var obj = {
      canceled: false,
      from: change.from,
      to: change.to,
      text: change.text,
      origin: change.origin,
      cancel: function () { return obj.canceled = true; }
    };
    if (update) { obj.update = function (from, to, text, origin) {
      if (from) { obj.from = clipPos(doc, from); }
      if (to) { obj.to = clipPos(doc, to); }
      if (text) { obj.text = text; }
      if (origin !== undefined) { obj.origin = origin; }
    }; }
    signal(doc, "beforeChange", doc, obj);
    if (doc.cm) { signal(doc.cm, "beforeChange", doc.cm, obj); }

    if (obj.canceled) {
      if (doc.cm) { doc.cm.curOp.updateInput = 2; }
      return null
    }
    return {from: obj.from, to: obj.to, text: obj.text, origin: obj.origin}
  }

  // Apply a change to a document, and add it to the document's
  // history, and propagating it to all linked documents.
  function makeChange(doc, change, ignoreReadOnly) {
    if (doc.cm) {
      if (!doc.cm.curOp) { return operation(doc.cm, makeChange)(doc, change, ignoreReadOnly) }
      if (doc.cm.state.suppressEdits) { return }
    }

    if (hasHandler(doc, "beforeChange") || doc.cm && hasHandler(doc.cm, "beforeChange")) {
      change = filterChange(doc, change, true);
      if (!change) { return }
    }

    // Possibly split or suppress the update based on the presence
    // of read-only spans in its range.
    var split = sawReadOnlySpans && !ignoreReadOnly && removeReadOnlyRanges(doc, change.from, change.to);
    if (split) {
      for (var i = split.length - 1; i >= 0; --i)
        { makeChangeInner(doc, {from: split[i].from, to: split[i].to, text: i ? [""] : change.text, origin: change.origin}); }
    } else {
      makeChangeInner(doc, change);
    }
  }

  function makeChangeInner(doc, change) {
    if (change.text.length == 1 && change.text[0] == "" && cmp(change.from, change.to) == 0) { return }
    var selAfter = computeSelAfterChange(doc, change);
    addChangeToHistory(doc, change, selAfter, doc.cm ? doc.cm.curOp.id : NaN);

    makeChangeSingleDoc(doc, change, selAfter, stretchSpansOverChange(doc, change));
    var rebased = [];

    linkedDocs(doc, function (doc, sharedHist) {
      if (!sharedHist && indexOf(rebased, doc.history) == -1) {
        rebaseHist(doc.history, change);
        rebased.push(doc.history);
      }
      makeChangeSingleDoc(doc, change, null, stretchSpansOverChange(doc, change));
    });
  }

  // Revert a change stored in a document's history.
  function makeChangeFromHistory(doc, type, allowSelectionOnly) {
    var suppress = doc.cm && doc.cm.state.suppressEdits;
    if (suppress && !allowSelectionOnly) { return }

    var hist = doc.history, event, selAfter = doc.sel;
    var source = type == "undo" ? hist.done : hist.undone, dest = type == "undo" ? hist.undone : hist.done;

    // Verify that there is a useable event (so that ctrl-z won't
    // needlessly clear selection events)
    var i = 0;
    for (; i < source.length; i++) {
      event = source[i];
      if (allowSelectionOnly ? event.ranges && !event.equals(doc.sel) : !event.ranges)
        { break }
    }
    if (i == source.length) { return }
    hist.lastOrigin = hist.lastSelOrigin = null;

    for (;;) {
      event = source.pop();
      if (event.ranges) {
        pushSelectionToHistory(event, dest);
        if (allowSelectionOnly && !event.equals(doc.sel)) {
          setSelection(doc, event, {clearRedo: false});
          return
        }
        selAfter = event;
      } else if (suppress) {
        source.push(event);
        return
      } else { break }
    }

    // Build up a reverse change object to add to the opposite history
    // stack (redo when undoing, and vice versa).
    var antiChanges = [];
    pushSelectionToHistory(selAfter, dest);
    dest.push({changes: antiChanges, generation: hist.generation});
    hist.generation = event.generation || ++hist.maxGeneration;

    var filter = hasHandler(doc, "beforeChange") || doc.cm && hasHandler(doc.cm, "beforeChange");

    var loop = function ( i ) {
      var change = event.changes[i];
      change.origin = type;
      if (filter && !filterChange(doc, change, false)) {
        source.length = 0;
        return {}
      }

      antiChanges.push(historyChangeFromChange(doc, change));

      var after = i ? computeSelAfterChange(doc, change) : lst(source);
      makeChangeSingleDoc(doc, change, after, mergeOldSpans(doc, change));
      if (!i && doc.cm) { doc.cm.scrollIntoView({from: change.from, to: changeEnd(change)}); }
      var rebased = [];

      // Propagate to the linked documents
      linkedDocs(doc, function (doc, sharedHist) {
        if (!sharedHist && indexOf(rebased, doc.history) == -1) {
          rebaseHist(doc.history, change);
          rebased.push(doc.history);
        }
        makeChangeSingleDoc(doc, change, null, mergeOldSpans(doc, change));
      });
    };

    for (var i$1 = event.changes.length - 1; i$1 >= 0; --i$1) {
      var returned = loop( i$1 );

      if ( returned ) return returned.v;
    }
  }

  // Sub-views need their line numbers shifted when text is added
  // above or below them in the parent document.
  function shiftDoc(doc, distance) {
    if (distance == 0) { return }
    doc.first += distance;
    doc.sel = new Selection(map(doc.sel.ranges, function (range) { return new Range(
      Pos(range.anchor.line + distance, range.anchor.ch),
      Pos(range.head.line + distance, range.head.ch)
    ); }), doc.sel.primIndex);
    if (doc.cm) {
      regChange(doc.cm, doc.first, doc.first - distance, distance);
      for (var d = doc.cm.display, l = d.viewFrom; l < d.viewTo; l++)
        { regLineChange(doc.cm, l, "gutter"); }
    }
  }

  // More lower-level change function, handling only a single document
  // (not linked ones).
  function makeChangeSingleDoc(doc, change, selAfter, spans) {
    if (doc.cm && !doc.cm.curOp)
      { return operation(doc.cm, makeChangeSingleDoc)(doc, change, selAfter, spans) }

    if (change.to.line < doc.first) {
      shiftDoc(doc, change.text.length - 1 - (change.to.line - change.from.line));
      return
    }
    if (change.from.line > doc.lastLine()) { return }

    // Clip the change to the size of this doc
    if (change.from.line < doc.first) {
      var shift = change.text.length - 1 - (doc.first - change.from.line);
      shiftDoc(doc, shift);
      change = {from: Pos(doc.first, 0), to: Pos(change.to.line + shift, change.to.ch),
                text: [lst(change.text)], origin: change.origin};
    }
    var last = doc.lastLine();
    if (change.to.line > last) {
      change = {from: change.from, to: Pos(last, getLine(doc, last).text.length),
                text: [change.text[0]], origin: change.origin};
    }

    change.removed = getBetween(doc, change.from, change.to);

    if (!selAfter) { selAfter = computeSelAfterChange(doc, change); }
    if (doc.cm) { makeChangeSingleDocInEditor(doc.cm, change, spans); }
    else { updateDoc(doc, change, spans); }
    setSelectionNoUndo(doc, selAfter, sel_dontScroll);

    if (doc.cantEdit && skipAtomic(doc, Pos(doc.firstLine(), 0)))
      { doc.cantEdit = false; }
  }

  // Handle the interaction of a change to a document with the editor
  // that this document is part of.
  function makeChangeSingleDocInEditor(cm, change, spans) {
    var doc = cm.doc, display = cm.display, from = change.from, to = change.to;

    var recomputeMaxLength = false, checkWidthStart = from.line;
    if (!cm.options.lineWrapping) {
      checkWidthStart = lineNo(visualLine(getLine(doc, from.line)));
      doc.iter(checkWidthStart, to.line + 1, function (line) {
        if (line == display.maxLine) {
          recomputeMaxLength = true;
          return true
        }
      });
    }

    if (doc.sel.contains(change.from, change.to) > -1)
      { signalCursorActivity(cm); }

    updateDoc(doc, change, spans, estimateHeight(cm));

    if (!cm.options.lineWrapping) {
      doc.iter(checkWidthStart, from.line + change.text.length, function (line) {
        var len = lineLength(line);
        if (len > display.maxLineLength) {
          display.maxLine = line;
          display.maxLineLength = len;
          display.maxLineChanged = true;
          recomputeMaxLength = false;
        }
      });
      if (recomputeMaxLength) { cm.curOp.updateMaxLine = true; }
    }

    retreatFrontier(doc, from.line);
    startWorker(cm, 400);

    var lendiff = change.text.length - (to.line - from.line) - 1;
    // Remember that these lines changed, for updating the display
    if (change.full)
      { regChange(cm); }
    else if (from.line == to.line && change.text.length == 1 && !isWholeLineUpdate(cm.doc, change))
      { regLineChange(cm, from.line, "text"); }
    else
      { regChange(cm, from.line, to.line + 1, lendiff); }

    var changesHandler = hasHandler(cm, "changes"), changeHandler = hasHandler(cm, "change");
    if (changeHandler || changesHandler) {
      var obj = {
        from: from, to: to,
        text: change.text,
        removed: change.removed,
        origin: change.origin
      };
      if (changeHandler) { signalLater(cm, "change", cm, obj); }
      if (changesHandler) { (cm.curOp.changeObjs || (cm.curOp.changeObjs = [])).push(obj); }
    }
    cm.display.selForContextMenu = null;
  }

  function replaceRange(doc, code, from, to, origin) {
    var assign;

    if (!to) { to = from; }
    if (cmp(to, from) < 0) { (assign = [to, from], from = assign[0], to = assign[1]); }
    if (typeof code == "string") { code = doc.splitLines(code); }
    makeChange(doc, {from: from, to: to, text: code, origin: origin});
  }

  // Rebasing/resetting history to deal with externally-sourced changes

  function rebaseHistSelSingle(pos, from, to, diff) {
    if (to < pos.line) {
      pos.line += diff;
    } else if (from < pos.line) {
      pos.line = from;
      pos.ch = 0;
    }
  }

  // Tries to rebase an array of history events given a change in the
  // document. If the change touches the same lines as the event, the
  // event, and everything 'behind' it, is discarded. If the change is
  // before the event, the event's positions are updated. Uses a
  // copy-on-write scheme for the positions, to avoid having to
  // reallocate them all on every rebase, but also avoid problems with
  // shared position objects being unsafely updated.
  function rebaseHistArray(array, from, to, diff) {
    for (var i = 0; i < array.length; ++i) {
      var sub = array[i], ok = true;
      if (sub.ranges) {
        if (!sub.copied) { sub = array[i] = sub.deepCopy(); sub.copied = true; }
        for (var j = 0; j < sub.ranges.length; j++) {
          rebaseHistSelSingle(sub.ranges[j].anchor, from, to, diff);
          rebaseHistSelSingle(sub.ranges[j].head, from, to, diff);
        }
        continue
      }
      for (var j$1 = 0; j$1 < sub.changes.length; ++j$1) {
        var cur = sub.changes[j$1];
        if (to < cur.from.line) {
          cur.from = Pos(cur.from.line + diff, cur.from.ch);
          cur.to = Pos(cur.to.line + diff, cur.to.ch);
        } else if (from <= cur.to.line) {
          ok = false;
          break
        }
      }
      if (!ok) {
        array.splice(0, i + 1);
        i = 0;
      }
    }
  }

  function rebaseHist(hist, change) {
    var from = change.from.line, to = change.to.line, diff = change.text.length - (to - from) - 1;
    rebaseHistArray(hist.done, from, to, diff);
    rebaseHistArray(hist.undone, from, to, diff);
  }

  // Utility for applying a change to a line by handle or number,
  // returning the number and optionally registering the line as
  // changed.
  function changeLine(doc, handle, changeType, op) {
    var no = handle, line = handle;
    if (typeof handle == "number") { line = getLine(doc, clipLine(doc, handle)); }
    else { no = lineNo(handle); }
    if (no == null) { return null }
    if (op(line, no) && doc.cm) { regLineChange(doc.cm, no, changeType); }
    return line
  }

  // The document is represented as a BTree consisting of leaves, with
  // chunk of lines in them, and branches, with up to ten leaves or
  // other branch nodes below them. The top node is always a branch
  // node, and is the document object itself (meaning it has
  // additional methods and properties).
  //
  // All nodes have parent links. The tree is used both to go from
  // line numbers to line objects, and to go from objects to numbers.
  // It also indexes by height, and is used to convert between height
  // and line object, and to find the total height of the document.
  //
  // See also http://marijnhaverbeke.nl/blog/codemirror-line-tree.html

  function LeafChunk(lines) {
    this.lines = lines;
    this.parent = null;
    var height = 0;
    for (var i = 0; i < lines.length; ++i) {
      lines[i].parent = this;
      height += lines[i].height;
    }
    this.height = height;
  }

  LeafChunk.prototype = {
    chunkSize: function() { return this.lines.length },

    // Remove the n lines at offset 'at'.
    removeInner: function(at, n) {
      for (var i = at, e = at + n; i < e; ++i) {
        var line = this.lines[i];
        this.height -= line.height;
        cleanUpLine(line);
        signalLater(line, "delete");
      }
      this.lines.splice(at, n);
    },

    // Helper used to collapse a small branch into a single leaf.
    collapse: function(lines) {
      lines.push.apply(lines, this.lines);
    },

    // Insert the given array of lines at offset 'at', count them as
    // having the given height.
    insertInner: function(at, lines, height) {
      this.height += height;
      this.lines = this.lines.slice(0, at).concat(lines).concat(this.lines.slice(at));
      for (var i = 0; i < lines.length; ++i) { lines[i].parent = this; }
    },

    // Used to iterate over a part of the tree.
    iterN: function(at, n, op) {
      for (var e = at + n; at < e; ++at)
        { if (op(this.lines[at])) { return true } }
    }
  };

  function BranchChunk(children) {
    this.children = children;
    var size = 0, height = 0;
    for (var i = 0; i < children.length; ++i) {
      var ch = children[i];
      size += ch.chunkSize(); height += ch.height;
      ch.parent = this;
    }
    this.size = size;
    this.height = height;
    this.parent = null;
  }

  BranchChunk.prototype = {
    chunkSize: function() { return this.size },

    removeInner: function(at, n) {
      this.size -= n;
      for (var i = 0; i < this.children.length; ++i) {
        var child = this.children[i], sz = child.chunkSize();
        if (at < sz) {
          var rm = Math.min(n, sz - at), oldHeight = child.height;
          child.removeInner(at, rm);
          this.height -= oldHeight - child.height;
          if (sz == rm) { this.children.splice(i--, 1); child.parent = null; }
          if ((n -= rm) == 0) { break }
          at = 0;
        } else { at -= sz; }
      }
      // If the result is smaller than 25 lines, ensure that it is a
      // single leaf node.
      if (this.size - n < 25 &&
          (this.children.length > 1 || !(this.children[0] instanceof LeafChunk))) {
        var lines = [];
        this.collapse(lines);
        this.children = [new LeafChunk(lines)];
        this.children[0].parent = this;
      }
    },

    collapse: function(lines) {
      for (var i = 0; i < this.children.length; ++i) { this.children[i].collapse(lines); }
    },

    insertInner: function(at, lines, height) {
      this.size += lines.length;
      this.height += height;
      for (var i = 0; i < this.children.length; ++i) {
        var child = this.children[i], sz = child.chunkSize();
        if (at <= sz) {
          child.insertInner(at, lines, height);
          if (child.lines && child.lines.length > 50) {
            // To avoid memory thrashing when child.lines is huge (e.g. first view of a large file), it's never spliced.
            // Instead, small slices are taken. They're taken in order because sequential memory accesses are fastest.
            var remaining = child.lines.length % 25 + 25;
            for (var pos = remaining; pos < child.lines.length;) {
              var leaf = new LeafChunk(child.lines.slice(pos, pos += 25));
              child.height -= leaf.height;
              this.children.splice(++i, 0, leaf);
              leaf.parent = this;
            }
            child.lines = child.lines.slice(0, remaining);
            this.maybeSpill();
          }
          break
        }
        at -= sz;
      }
    },

    // When a node has grown, check whether it should be split.
    maybeSpill: function() {
      if (this.children.length <= 10) { return }
      var me = this;
      do {
        var spilled = me.children.splice(me.children.length - 5, 5);
        var sibling = new BranchChunk(spilled);
        if (!me.parent) { // Become the parent node
          var copy = new BranchChunk(me.children);
          copy.parent = me;
          me.children = [copy, sibling];
          me = copy;
       } else {
          me.size -= sibling.size;
          me.height -= sibling.height;
          var myIndex = indexOf(me.parent.children, me);
          me.parent.children.splice(myIndex + 1, 0, sibling);
        }
        sibling.parent = me.parent;
      } while (me.children.length > 10)
      me.parent.maybeSpill();
    },

    iterN: function(at, n, op) {
      for (var i = 0; i < this.children.length; ++i) {
        var child = this.children[i], sz = child.chunkSize();
        if (at < sz) {
          var used = Math.min(n, sz - at);
          if (child.iterN(at, used, op)) { return true }
          if ((n -= used) == 0) { break }
          at = 0;
        } else { at -= sz; }
      }
    }
  };

  // Line widgets are block elements displayed above or below a line.

  var LineWidget = function(doc, node, options) {
    if (options) { for (var opt in options) { if (options.hasOwnProperty(opt))
      { this[opt] = options[opt]; } } }
    this.doc = doc;
    this.node = node;
  };

  LineWidget.prototype.clear = function () {
    var cm = this.doc.cm, ws = this.line.widgets, line = this.line, no = lineNo(line);
    if (no == null || !ws) { return }
    for (var i = 0; i < ws.length; ++i) { if (ws[i] == this) { ws.splice(i--, 1); } }
    if (!ws.length) { line.widgets = null; }
    var height = widgetHeight(this);
    updateLineHeight(line, Math.max(0, line.height - height));
    if (cm) {
      runInOp(cm, function () {
        adjustScrollWhenAboveVisible(cm, line, -height);
        regLineChange(cm, no, "widget");
      });
      signalLater(cm, "lineWidgetCleared", cm, this, no);
    }
  };

  LineWidget.prototype.changed = function () {
      var this$1 = this;

    var oldH = this.height, cm = this.doc.cm, line = this.line;
    this.height = null;
    var diff = widgetHeight(this) - oldH;
    if (!diff) { return }
    if (!lineIsHidden(this.doc, line)) { updateLineHeight(line, line.height + diff); }
    if (cm) {
      runInOp(cm, function () {
        cm.curOp.forceUpdate = true;
        adjustScrollWhenAboveVisible(cm, line, diff);
        signalLater(cm, "lineWidgetChanged", cm, this$1, lineNo(line));
      });
    }
  };
  eventMixin(LineWidget);

  function adjustScrollWhenAboveVisible(cm, line, diff) {
    if (heightAtLine(line) < ((cm.curOp && cm.curOp.scrollTop) || cm.doc.scrollTop))
      { addToScrollTop(cm, diff); }
  }

  function addLineWidget(doc, handle, node, options) {
    var widget = new LineWidget(doc, node, options);
    var cm = doc.cm;
    if (cm && widget.noHScroll) { cm.display.alignWidgets = true; }
    changeLine(doc, handle, "widget", function (line) {
      var widgets = line.widgets || (line.widgets = []);
      if (widget.insertAt == null) { widgets.push(widget); }
      else { widgets.splice(Math.min(widgets.length - 1, Math.max(0, widget.insertAt)), 0, widget); }
      widget.line = line;
      if (cm && !lineIsHidden(doc, line)) {
        var aboveVisible = heightAtLine(line) < doc.scrollTop;
        updateLineHeight(line, line.height + widgetHeight(widget));
        if (aboveVisible) { addToScrollTop(cm, widget.height); }
        cm.curOp.forceUpdate = true;
      }
      return true
    });
    if (cm) { signalLater(cm, "lineWidgetAdded", cm, widget, typeof handle == "number" ? handle : lineNo(handle)); }
    return widget
  }

  // TEXTMARKERS

  // Created with markText and setBookmark methods. A TextMarker is a
  // handle that can be used to clear or find a marked position in the
  // document. Line objects hold arrays (markedSpans) containing
  // {from, to, marker} object pointing to such marker objects, and
  // indicating that such a marker is present on that line. Multiple
  // lines may point to the same marker when it spans across lines.
  // The spans will have null for their from/to properties when the
  // marker continues beyond the start/end of the line. Markers have
  // links back to the lines they currently touch.

  // Collapsed markers have unique ids, in order to be able to order
  // them, which is needed for uniquely determining an outer marker
  // when they overlap (they may nest, but not partially overlap).
  var nextMarkerId = 0;

  var TextMarker = function(doc, type) {
    this.lines = [];
    this.type = type;
    this.doc = doc;
    this.id = ++nextMarkerId;
  };

  // Clear the marker.
  TextMarker.prototype.clear = function () {
    if (this.explicitlyCleared) { return }
    var cm = this.doc.cm, withOp = cm && !cm.curOp;
    if (withOp) { startOperation(cm); }
    if (hasHandler(this, "clear")) {
      var found = this.find();
      if (found) { signalLater(this, "clear", found.from, found.to); }
    }
    var min = null, max = null;
    for (var i = 0; i < this.lines.length; ++i) {
      var line = this.lines[i];
      var span = getMarkedSpanFor(line.markedSpans, this);
      if (cm && !this.collapsed) { regLineChange(cm, lineNo(line), "text"); }
      else if (cm) {
        if (span.to != null) { max = lineNo(line); }
        if (span.from != null) { min = lineNo(line); }
      }
      line.markedSpans = removeMarkedSpan(line.markedSpans, span);
      if (span.from == null && this.collapsed && !lineIsHidden(this.doc, line) && cm)
        { updateLineHeight(line, textHeight(cm.display)); }
    }
    if (cm && this.collapsed && !cm.options.lineWrapping) { for (var i$1 = 0; i$1 < this.lines.length; ++i$1) {
      var visual = visualLine(this.lines[i$1]), len = lineLength(visual);
      if (len > cm.display.maxLineLength) {
        cm.display.maxLine = visual;
        cm.display.maxLineLength = len;
        cm.display.maxLineChanged = true;
      }
    } }

    if (min != null && cm && this.collapsed) { regChange(cm, min, max + 1); }
    this.lines.length = 0;
    this.explicitlyCleared = true;
    if (this.atomic && this.doc.cantEdit) {
      this.doc.cantEdit = false;
      if (cm) { reCheckSelection(cm.doc); }
    }
    if (cm) { signalLater(cm, "markerCleared", cm, this, min, max); }
    if (withOp) { endOperation(cm); }
    if (this.parent) { this.parent.clear(); }
  };

  // Find the position of the marker in the document. Returns a {from,
  // to} object by default. Side can be passed to get a specific side
  // -- 0 (both), -1 (left), or 1 (right). When lineObj is true, the
  // Pos objects returned contain a line object, rather than a line
  // number (used to prevent looking up the same line twice).
  TextMarker.prototype.find = function (side, lineObj) {
    if (side == null && this.type == "bookmark") { side = 1; }
    var from, to;
    for (var i = 0; i < this.lines.length; ++i) {
      var line = this.lines[i];
      var span = getMarkedSpanFor(line.markedSpans, this);
      if (span.from != null) {
        from = Pos(lineObj ? line : lineNo(line), span.from);
        if (side == -1) { return from }
      }
      if (span.to != null) {
        to = Pos(lineObj ? line : lineNo(line), span.to);
        if (side == 1) { return to }
      }
    }
    return from && {from: from, to: to}
  };

  // Signals that the marker's widget changed, and surrounding layout
  // should be recomputed.
  TextMarker.prototype.changed = function () {
      var this$1 = this;

    var pos = this.find(-1, true), widget = this, cm = this.doc.cm;
    if (!pos || !cm) { return }
    runInOp(cm, function () {
      var line = pos.line, lineN = lineNo(pos.line);
      var view = findViewForLine(cm, lineN);
      if (view) {
        clearLineMeasurementCacheFor(view);
        cm.curOp.selectionChanged = cm.curOp.forceUpdate = true;
      }
      cm.curOp.updateMaxLine = true;
      if (!lineIsHidden(widget.doc, line) && widget.height != null) {
        var oldHeight = widget.height;
        widget.height = null;
        var dHeight = widgetHeight(widget) - oldHeight;
        if (dHeight)
          { updateLineHeight(line, line.height + dHeight); }
      }
      signalLater(cm, "markerChanged", cm, this$1);
    });
  };

  TextMarker.prototype.attachLine = function (line) {
    if (!this.lines.length && this.doc.cm) {
      var op = this.doc.cm.curOp;
      if (!op.maybeHiddenMarkers || indexOf(op.maybeHiddenMarkers, this) == -1)
        { (op.maybeUnhiddenMarkers || (op.maybeUnhiddenMarkers = [])).push(this); }
    }
    this.lines.push(line);
  };

  TextMarker.prototype.detachLine = function (line) {
    this.lines.splice(indexOf(this.lines, line), 1);
    if (!this.lines.length && this.doc.cm) {
      var op = this.doc.cm.curOp
      ;(op.maybeHiddenMarkers || (op.maybeHiddenMarkers = [])).push(this);
    }
  };
  eventMixin(TextMarker);

  // Create a marker, wire it up to the right lines, and
  function markText(doc, from, to, options, type) {
    // Shared markers (across linked documents) are handled separately
    // (markTextShared will call out to this again, once per
    // document).
    if (options && options.shared) { return markTextShared(doc, from, to, options, type) }
    // Ensure we are in an operation.
    if (doc.cm && !doc.cm.curOp) { return operation(doc.cm, markText)(doc, from, to, options, type) }

    var marker = new TextMarker(doc, type), diff = cmp(from, to);
    if (options) { copyObj(options, marker, false); }
    // Don't connect empty markers unless clearWhenEmpty is false
    if (diff > 0 || diff == 0 && marker.clearWhenEmpty !== false)
      { return marker }
    if (marker.replacedWith) {
      // Showing up as a widget implies collapsed (widget replaces text)
      marker.collapsed = true;
      marker.widgetNode = eltP("span", [marker.replacedWith], "CodeMirror-widget");
      if (!options.handleMouseEvents) { marker.widgetNode.setAttribute("cm-ignore-events", "true"); }
      if (options.insertLeft) { marker.widgetNode.insertLeft = true; }
    }
    if (marker.collapsed) {
      if (conflictingCollapsedRange(doc, from.line, from, to, marker) ||
          from.line != to.line && conflictingCollapsedRange(doc, to.line, from, to, marker))
        { throw new Error("Inserting collapsed marker partially overlapping an existing one") }
      seeCollapsedSpans();
    }

    if (marker.addToHistory)
      { addChangeToHistory(doc, {from: from, to: to, origin: "markText"}, doc.sel, NaN); }

    var curLine = from.line, cm = doc.cm, updateMaxLine;
    doc.iter(curLine, to.line + 1, function (line) {
      if (cm && marker.collapsed && !cm.options.lineWrapping && visualLine(line) == cm.display.maxLine)
        { updateMaxLine = true; }
      if (marker.collapsed && curLine != from.line) { updateLineHeight(line, 0); }
      addMarkedSpan(line, new MarkedSpan(marker,
                                         curLine == from.line ? from.ch : null,
                                         curLine == to.line ? to.ch : null));
      ++curLine;
    });
    // lineIsHidden depends on the presence of the spans, so needs a second pass
    if (marker.collapsed) { doc.iter(from.line, to.line + 1, function (line) {
      if (lineIsHidden(doc, line)) { updateLineHeight(line, 0); }
    }); }

    if (marker.clearOnEnter) { on(marker, "beforeCursorEnter", function () { return marker.clear(); }); }

    if (marker.readOnly) {
      seeReadOnlySpans();
      if (doc.history.done.length || doc.history.undone.length)
        { doc.clearHistory(); }
    }
    if (marker.collapsed) {
      marker.id = ++nextMarkerId;
      marker.atomic = true;
    }
    if (cm) {
      // Sync editor state
      if (updateMaxLine) { cm.curOp.updateMaxLine = true; }
      if (marker.collapsed)
        { regChange(cm, from.line, to.line + 1); }
      else if (marker.className || marker.startStyle || marker.endStyle || marker.css ||
               marker.attributes || marker.title)
        { for (var i = from.line; i <= to.line; i++) { regLineChange(cm, i, "text"); } }
      if (marker.atomic) { reCheckSelection(cm.doc); }
      signalLater(cm, "markerAdded", cm, marker);
    }
    return marker
  }

  // SHARED TEXTMARKERS

  // A shared marker spans multiple linked documents. It is
  // implemented as a meta-marker-object controlling multiple normal
  // markers.
  var SharedTextMarker = function(markers, primary) {
    this.markers = markers;
    this.primary = primary;
    for (var i = 0; i < markers.length; ++i)
      { markers[i].parent = this; }
  };

  SharedTextMarker.prototype.clear = function () {
    if (this.explicitlyCleared) { return }
    this.explicitlyCleared = true;
    for (var i = 0; i < this.markers.length; ++i)
      { this.markers[i].clear(); }
    signalLater(this, "clear");
  };

  SharedTextMarker.prototype.find = function (side, lineObj) {
    return this.primary.find(side, lineObj)
  };
  eventMixin(SharedTextMarker);

  function markTextShared(doc, from, to, options, type) {
    options = copyObj(options);
    options.shared = false;
    var markers = [markText(doc, from, to, options, type)], primary = markers[0];
    var widget = options.widgetNode;
    linkedDocs(doc, function (doc) {
      if (widget) { options.widgetNode = widget.cloneNode(true); }
      markers.push(markText(doc, clipPos(doc, from), clipPos(doc, to), options, type));
      for (var i = 0; i < doc.linked.length; ++i)
        { if (doc.linked[i].isParent) { return } }
      primary = lst(markers);
    });
    return new SharedTextMarker(markers, primary)
  }

  function findSharedMarkers(doc) {
    return doc.findMarks(Pos(doc.first, 0), doc.clipPos(Pos(doc.lastLine())), function (m) { return m.parent; })
  }

  function copySharedMarkers(doc, markers) {
    for (var i = 0; i < markers.length; i++) {
      var marker = markers[i], pos = marker.find();
      var mFrom = doc.clipPos(pos.from), mTo = doc.clipPos(pos.to);
      if (cmp(mFrom, mTo)) {
        var subMark = markText(doc, mFrom, mTo, marker.primary, marker.primary.type);
        marker.markers.push(subMark);
        subMark.parent = marker;
      }
    }
  }

  function detachSharedMarkers(markers) {
    var loop = function ( i ) {
      var marker = markers[i], linked = [marker.primary.doc];
      linkedDocs(marker.primary.doc, function (d) { return linked.push(d); });
      for (var j = 0; j < marker.markers.length; j++) {
        var subMarker = marker.markers[j];
        if (indexOf(linked, subMarker.doc) == -1) {
          subMarker.parent = null;
          marker.markers.splice(j--, 1);
        }
      }
    };

    for (var i = 0; i < markers.length; i++) loop( i );
  }

  var nextDocId = 0;
  var Doc = function(text, mode, firstLine, lineSep, direction) {
    if (!(this instanceof Doc)) { return new Doc(text, mode, firstLine, lineSep, direction) }
    if (firstLine == null) { firstLine = 0; }

    BranchChunk.call(this, [new LeafChunk([new Line("", null)])]);
    this.first = firstLine;
    this.scrollTop = this.scrollLeft = 0;
    this.cantEdit = false;
    this.cleanGeneration = 1;
    this.modeFrontier = this.highlightFrontier = firstLine;
    var start = Pos(firstLine, 0);
    this.sel = simpleSelection(start);
    this.history = new History(null);
    this.id = ++nextDocId;
    this.modeOption = mode;
    this.lineSep = lineSep;
    this.direction = (direction == "rtl") ? "rtl" : "ltr";
    this.extend = false;

    if (typeof text == "string") { text = this.splitLines(text); }
    updateDoc(this, {from: start, to: start, text: text});
    setSelection(this, simpleSelection(start), sel_dontScroll);
  };

  Doc.prototype = createObj(BranchChunk.prototype, {
    constructor: Doc,
    // Iterate over the document. Supports two forms -- with only one
    // argument, it calls that for each line in the document. With
    // three, it iterates over the range given by the first two (with
    // the second being non-inclusive).
    iter: function(from, to, op) {
      if (op) { this.iterN(from - this.first, to - from, op); }
      else { this.iterN(this.first, this.first + this.size, from); }
    },

    // Non-public interface for adding and removing lines.
    insert: function(at, lines) {
      var height = 0;
      for (var i = 0; i < lines.length; ++i) { height += lines[i].height; }
      this.insertInner(at - this.first, lines, height);
    },
    remove: function(at, n) { this.removeInner(at - this.first, n); },

    // From here, the methods are part of the public interface. Most
    // are also available from CodeMirror (editor) instances.

    getValue: function(lineSep) {
      var lines = getLines(this, this.first, this.first + this.size);
      if (lineSep === false) { return lines }
      return lines.join(lineSep || this.lineSeparator())
    },
    setValue: docMethodOp(function(code) {
      var top = Pos(this.first, 0), last = this.first + this.size - 1;
      makeChange(this, {from: top, to: Pos(last, getLine(this, last).text.length),
                        text: this.splitLines(code), origin: "setValue", full: true}, true);
      if (this.cm) { scrollToCoords(this.cm, 0, 0); }
      setSelection(this, simpleSelection(top), sel_dontScroll);
    }),
    replaceRange: function(code, from, to, origin) {
      from = clipPos(this, from);
      to = to ? clipPos(this, to) : from;
      replaceRange(this, code, from, to, origin);
    },
    getRange: function(from, to, lineSep) {
      var lines = getBetween(this, clipPos(this, from), clipPos(this, to));
      if (lineSep === false) { return lines }
      return lines.join(lineSep || this.lineSeparator())
    },

    getLine: function(line) {var l = this.getLineHandle(line); return l && l.text},

    getLineHandle: function(line) {if (isLine(this, line)) { return getLine(this, line) }},
    getLineNumber: function(line) {return lineNo(line)},

    getLineHandleVisualStart: function(line) {
      if (typeof line == "number") { line = getLine(this, line); }
      return visualLine(line)
    },

    lineCount: function() {return this.size},
    firstLine: function() {return this.first},
    lastLine: function() {return this.first + this.size - 1},

    clipPos: function(pos) {return clipPos(this, pos)},

    getCursor: function(start) {
      var range = this.sel.primary(), pos;
      if (start == null || start == "head") { pos = range.head; }
      else if (start == "anchor") { pos = range.anchor; }
      else if (start == "end" || start == "to" || start === false) { pos = range.to(); }
      else { pos = range.from(); }
      return pos
    },
    listSelections: function() { return this.sel.ranges },
    somethingSelected: function() {return this.sel.somethingSelected()},

    setCursor: docMethodOp(function(line, ch, options) {
      setSimpleSelection(this, clipPos(this, typeof line == "number" ? Pos(line, ch || 0) : line), null, options);
    }),
    setSelection: docMethodOp(function(anchor, head, options) {
      setSimpleSelection(this, clipPos(this, anchor), clipPos(this, head || anchor), options);
    }),
    extendSelection: docMethodOp(function(head, other, options) {
      extendSelection(this, clipPos(this, head), other && clipPos(this, other), options);
    }),
    extendSelections: docMethodOp(function(heads, options) {
      extendSelections(this, clipPosArray(this, heads), options);
    }),
    extendSelectionsBy: docMethodOp(function(f, options) {
      var heads = map(this.sel.ranges, f);
      extendSelections(this, clipPosArray(this, heads), options);
    }),
    setSelections: docMethodOp(function(ranges, primary, options) {
      if (!ranges.length) { return }
      var out = [];
      for (var i = 0; i < ranges.length; i++)
        { out[i] = new Range(clipPos(this, ranges[i].anchor),
                           clipPos(this, ranges[i].head)); }
      if (primary == null) { primary = Math.min(ranges.length - 1, this.sel.primIndex); }
      setSelection(this, normalizeSelection(this.cm, out, primary), options);
    }),
    addSelection: docMethodOp(function(anchor, head, options) {
      var ranges = this.sel.ranges.slice(0);
      ranges.push(new Range(clipPos(this, anchor), clipPos(this, head || anchor)));
      setSelection(this, normalizeSelection(this.cm, ranges, ranges.length - 1), options);
    }),

    getSelection: function(lineSep) {
      var ranges = this.sel.ranges, lines;
      for (var i = 0; i < ranges.length; i++) {
        var sel = getBetween(this, ranges[i].from(), ranges[i].to());
        lines = lines ? lines.concat(sel) : sel;
      }
      if (lineSep === false) { return lines }
      else { return lines.join(lineSep || this.lineSeparator()) }
    },
    getSelections: function(lineSep) {
      var parts = [], ranges = this.sel.ranges;
      for (var i = 0; i < ranges.length; i++) {
        var sel = getBetween(this, ranges[i].from(), ranges[i].to());
        if (lineSep !== false) { sel = sel.join(lineSep || this.lineSeparator()); }
        parts[i] = sel;
      }
      return parts
    },
    replaceSelection: function(code, collapse, origin) {
      var dup = [];
      for (var i = 0; i < this.sel.ranges.length; i++)
        { dup[i] = code; }
      this.replaceSelections(dup, collapse, origin || "+input");
    },
    replaceSelections: docMethodOp(function(code, collapse, origin) {
      var changes = [], sel = this.sel;
      for (var i = 0; i < sel.ranges.length; i++) {
        var range = sel.ranges[i];
        changes[i] = {from: range.from(), to: range.to(), text: this.splitLines(code[i]), origin: origin};
      }
      var newSel = collapse && collapse != "end" && computeReplacedSel(this, changes, collapse);
      for (var i$1 = changes.length - 1; i$1 >= 0; i$1--)
        { makeChange(this, changes[i$1]); }
      if (newSel) { setSelectionReplaceHistory(this, newSel); }
      else if (this.cm) { ensureCursorVisible(this.cm); }
    }),
    undo: docMethodOp(function() {makeChangeFromHistory(this, "undo");}),
    redo: docMethodOp(function() {makeChangeFromHistory(this, "redo");}),
    undoSelection: docMethodOp(function() {makeChangeFromHistory(this, "undo", true);}),
    redoSelection: docMethodOp(function() {makeChangeFromHistory(this, "redo", true);}),

    setExtending: function(val) {this.extend = val;},
    getExtending: function() {return this.extend},

    historySize: function() {
      var hist = this.history, done = 0, undone = 0;
      for (var i = 0; i < hist.done.length; i++) { if (!hist.done[i].ranges) { ++done; } }
      for (var i$1 = 0; i$1 < hist.undone.length; i$1++) { if (!hist.undone[i$1].ranges) { ++undone; } }
      return {undo: done, redo: undone}
    },
    clearHistory: function() {
      var this$1 = this;

      this.history = new History(this.history.maxGeneration);
      linkedDocs(this, function (doc) { return doc.history = this$1.history; }, true);
    },

    markClean: function() {
      this.cleanGeneration = this.changeGeneration(true);
    },
    changeGeneration: function(forceSplit) {
      if (forceSplit)
        { this.history.lastOp = this.history.lastSelOp = this.history.lastOrigin = null; }
      return this.history.generation
    },
    isClean: function (gen) {
      return this.history.generation == (gen || this.cleanGeneration)
    },

    getHistory: function() {
      return {done: copyHistoryArray(this.history.done),
              undone: copyHistoryArray(this.history.undone)}
    },
    setHistory: function(histData) {
      var hist = this.history = new History(this.history.maxGeneration);
      hist.done = copyHistoryArray(histData.done.slice(0), null, true);
      hist.undone = copyHistoryArray(histData.undone.slice(0), null, true);
    },

    setGutterMarker: docMethodOp(function(line, gutterID, value) {
      return changeLine(this, line, "gutter", function (line) {
        var markers = line.gutterMarkers || (line.gutterMarkers = {});
        markers[gutterID] = value;
        if (!value && isEmpty(markers)) { line.gutterMarkers = null; }
        return true
      })
    }),

    clearGutter: docMethodOp(function(gutterID) {
      var this$1 = this;

      this.iter(function (line) {
        if (line.gutterMarkers && line.gutterMarkers[gutterID]) {
          changeLine(this$1, line, "gutter", function () {
            line.gutterMarkers[gutterID] = null;
            if (isEmpty(line.gutterMarkers)) { line.gutterMarkers = null; }
            return true
          });
        }
      });
    }),

    lineInfo: function(line) {
      var n;
      if (typeof line == "number") {
        if (!isLine(this, line)) { return null }
        n = line;
        line = getLine(this, line);
        if (!line) { return null }
      } else {
        n = lineNo(line);
        if (n == null) { return null }
      }
      return {line: n, handle: line, text: line.text, gutterMarkers: line.gutterMarkers,
              textClass: line.textClass, bgClass: line.bgClass, wrapClass: line.wrapClass,
              widgets: line.widgets}
    },

    addLineClass: docMethodOp(function(handle, where, cls) {
      return changeLine(this, handle, where == "gutter" ? "gutter" : "class", function (line) {
        var prop = where == "text" ? "textClass"
                 : where == "background" ? "bgClass"
                 : where == "gutter" ? "gutterClass" : "wrapClass";
        if (!line[prop]) { line[prop] = cls; }
        else if (classTest(cls).test(line[prop])) { return false }
        else { line[prop] += " " + cls; }
        return true
      })
    }),
    removeLineClass: docMethodOp(function(handle, where, cls) {
      return changeLine(this, handle, where == "gutter" ? "gutter" : "class", function (line) {
        var prop = where == "text" ? "textClass"
                 : where == "background" ? "bgClass"
                 : where == "gutter" ? "gutterClass" : "wrapClass";
        var cur = line[prop];
        if (!cur) { return false }
        else if (cls == null) { line[prop] = null; }
        else {
          var found = cur.match(classTest(cls));
          if (!found) { return false }
          var end = found.index + found[0].length;
          line[prop] = cur.slice(0, found.index) + (!found.index || end == cur.length ? "" : " ") + cur.slice(end) || null;
        }
        return true
      })
    }),

    addLineWidget: docMethodOp(function(handle, node, options) {
      return addLineWidget(this, handle, node, options)
    }),
    removeLineWidget: function(widget) { widget.clear(); },

    markText: function(from, to, options) {
      return markText(this, clipPos(this, from), clipPos(this, to), options, options && options.type || "range")
    },
    setBookmark: function(pos, options) {
      var realOpts = {replacedWith: options && (options.nodeType == null ? options.widget : options),
                      insertLeft: options && options.insertLeft,
                      clearWhenEmpty: false, shared: options && options.shared,
                      handleMouseEvents: options && options.handleMouseEvents};
      pos = clipPos(this, pos);
      return markText(this, pos, pos, realOpts, "bookmark")
    },
    findMarksAt: function(pos) {
      pos = clipPos(this, pos);
      var markers = [], spans = getLine(this, pos.line).markedSpans;
      if (spans) { for (var i = 0; i < spans.length; ++i) {
        var span = spans[i];
        if ((span.from == null || span.from <= pos.ch) &&
            (span.to == null || span.to >= pos.ch))
          { markers.push(span.marker.parent || span.marker); }
      } }
      return markers
    },
    findMarks: function(from, to, filter) {
      from = clipPos(this, from); to = clipPos(this, to);
      var found = [], lineNo = from.line;
      this.iter(from.line, to.line + 1, function (line) {
        var spans = line.markedSpans;
        if (spans) { for (var i = 0; i < spans.length; i++) {
          var span = spans[i];
          if (!(span.to != null && lineNo == from.line && from.ch >= span.to ||
                span.from == null && lineNo != from.line ||
                span.from != null && lineNo == to.line && span.from >= to.ch) &&
              (!filter || filter(span.marker)))
            { found.push(span.marker.parent || span.marker); }
        } }
        ++lineNo;
      });
      return found
    },
    getAllMarks: function() {
      var markers = [];
      this.iter(function (line) {
        var sps = line.markedSpans;
        if (sps) { for (var i = 0; i < sps.length; ++i)
          { if (sps[i].from != null) { markers.push(sps[i].marker); } } }
      });
      return markers
    },

    posFromIndex: function(off) {
      var ch, lineNo = this.first, sepSize = this.lineSeparator().length;
      this.iter(function (line) {
        var sz = line.text.length + sepSize;
        if (sz > off) { ch = off; return true }
        off -= sz;
        ++lineNo;
      });
      return clipPos(this, Pos(lineNo, ch))
    },
    indexFromPos: function (coords) {
      coords = clipPos(this, coords);
      var index = coords.ch;
      if (coords.line < this.first || coords.ch < 0) { return 0 }
      var sepSize = this.lineSeparator().length;
      this.iter(this.first, coords.line, function (line) { // iter aborts when callback returns a truthy value
        index += line.text.length + sepSize;
      });
      return index
    },

    copy: function(copyHistory) {
      var doc = new Doc(getLines(this, this.first, this.first + this.size),
                        this.modeOption, this.first, this.lineSep, this.direction);
      doc.scrollTop = this.scrollTop; doc.scrollLeft = this.scrollLeft;
      doc.sel = this.sel;
      doc.extend = false;
      if (copyHistory) {
        doc.history.undoDepth = this.history.undoDepth;
        doc.setHistory(this.getHistory());
      }
      return doc
    },

    linkedDoc: function(options) {
      if (!options) { options = {}; }
      var from = this.first, to = this.first + this.size;
      if (options.from != null && options.from > from) { from = options.from; }
      if (options.to != null && options.to < to) { to = options.to; }
      var copy = new Doc(getLines(this, from, to), options.mode || this.modeOption, from, this.lineSep, this.direction);
      if (options.sharedHist) { copy.history = this.history
      ; }(this.linked || (this.linked = [])).push({doc: copy, sharedHist: options.sharedHist});
      copy.linked = [{doc: this, isParent: true, sharedHist: options.sharedHist}];
      copySharedMarkers(copy, findSharedMarkers(this));
      return copy
    },
    unlinkDoc: function(other) {
      if (other instanceof CodeMirror) { other = other.doc; }
      if (this.linked) { for (var i = 0; i < this.linked.length; ++i) {
        var link = this.linked[i];
        if (link.doc != other) { continue }
        this.linked.splice(i, 1);
        other.unlinkDoc(this);
        detachSharedMarkers(findSharedMarkers(this));
        break
      } }
      // If the histories were shared, split them again
      if (other.history == this.history) {
        var splitIds = [other.id];
        linkedDocs(other, function (doc) { return splitIds.push(doc.id); }, true);
        other.history = new History(null);
        other.history.done = copyHistoryArray(this.history.done, splitIds);
        other.history.undone = copyHistoryArray(this.history.undone, splitIds);
      }
    },
    iterLinkedDocs: function(f) {linkedDocs(this, f);},

    getMode: function() {return this.mode},
    getEditor: function() {return this.cm},

    splitLines: function(str) {
      if (this.lineSep) { return str.split(this.lineSep) }
      return splitLinesAuto(str)
    },
    lineSeparator: function() { return this.lineSep || "\n" },

    setDirection: docMethodOp(function (dir) {
      if (dir != "rtl") { dir = "ltr"; }
      if (dir == this.direction) { return }
      this.direction = dir;
      this.iter(function (line) { return line.order = null; });
      if (this.cm) { directionChanged(this.cm); }
    })
  });

  // Public alias.
  Doc.prototype.eachLine = Doc.prototype.iter;

  // Kludge to work around strange IE behavior where it'll sometimes
  // re-fire a series of drag-related events right after the drop (#1551)
  var lastDrop = 0;

  function onDrop(e) {
    var cm = this;
    clearDragCursor(cm);
    if (signalDOMEvent(cm, e) || eventInWidget(cm.display, e))
      { return }
    e_preventDefault(e);
    if (ie) { lastDrop = +new Date; }
    var pos = posFromMouse(cm, e, true), files = e.dataTransfer.files;
    if (!pos || cm.isReadOnly()) { return }
    // Might be a file drop, in which case we simply extract the text
    // and insert it.
    if (files && files.length && window.FileReader && window.File) {
      var n = files.length, text = Array(n), read = 0;
      var markAsReadAndPasteIfAllFilesAreRead = function () {
        if (++read == n) {
          operation(cm, function () {
            pos = clipPos(cm.doc, pos);
            var change = {from: pos, to: pos,
                          text: cm.doc.splitLines(
                              text.filter(function (t) { return t != null; }).join(cm.doc.lineSeparator())),
                          origin: "paste"};
            makeChange(cm.doc, change);
            setSelectionReplaceHistory(cm.doc, simpleSelection(clipPos(cm.doc, pos), clipPos(cm.doc, changeEnd(change))));
          })();
        }
      };
      var readTextFromFile = function (file, i) {
        if (cm.options.allowDropFileTypes &&
            indexOf(cm.options.allowDropFileTypes, file.type) == -1) {
          markAsReadAndPasteIfAllFilesAreRead();
          return
        }
        var reader = new FileReader;
        reader.onerror = function () { return markAsReadAndPasteIfAllFilesAreRead(); };
        reader.onload = function () {
          var content = reader.result;
          if (/[\x00-\x08\x0e-\x1f]{2}/.test(content)) {
            markAsReadAndPasteIfAllFilesAreRead();
            return
          }
          text[i] = content;
          markAsReadAndPasteIfAllFilesAreRead();
        };
        reader.readAsText(file);
      };
      for (var i = 0; i < files.length; i++) { readTextFromFile(files[i], i); }
    } else { // Normal drop
      // Don't do a replace if the drop happened inside of the selected text.
      if (cm.state.draggingText && cm.doc.sel.contains(pos) > -1) {
        cm.state.draggingText(e);
        // Ensure the editor is re-focused
        setTimeout(function () { return cm.display.input.focus(); }, 20);
        return
      }
      try {
        var text$1 = e.dataTransfer.getData("Text");
        if (text$1) {
          var selected;
          if (cm.state.draggingText && !cm.state.draggingText.copy)
            { selected = cm.listSelections(); }
          setSelectionNoUndo(cm.doc, simpleSelection(pos, pos));
          if (selected) { for (var i$1 = 0; i$1 < selected.length; ++i$1)
            { replaceRange(cm.doc, "", selected[i$1].anchor, selected[i$1].head, "drag"); } }
          cm.replaceSelection(text$1, "around", "paste");
          cm.display.input.focus();
        }
      }
      catch(e$1){}
    }
  }

  function onDragStart(cm, e) {
    if (ie && (!cm.state.draggingText || +new Date - lastDrop < 100)) { e_stop(e); return }
    if (signalDOMEvent(cm, e) || eventInWidget(cm.display, e)) { return }

    e.dataTransfer.setData("Text", cm.getSelection());
    e.dataTransfer.effectAllowed = "copyMove";

    // Use dummy image instead of default browsers image.
    // Recent Safari (~6.0.2) have a tendency to segfault when this happens, so we don't do it there.
    if (e.dataTransfer.setDragImage && !safari) {
      var img = elt("img", null, null, "position: fixed; left: 0; top: 0;");
      img.src = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";
      if (presto) {
        img.width = img.height = 1;
        cm.display.wrapper.appendChild(img);
        // Force a relayout, or Opera won't use our image for some obscure reason
        img._top = img.offsetTop;
      }
      e.dataTransfer.setDragImage(img, 0, 0);
      if (presto) { img.parentNode.removeChild(img); }
    }
  }

  function onDragOver(cm, e) {
    var pos = posFromMouse(cm, e);
    if (!pos) { return }
    var frag = document.createDocumentFragment();
    drawSelectionCursor(cm, pos, frag);
    if (!cm.display.dragCursor) {
      cm.display.dragCursor = elt("div", null, "CodeMirror-cursors CodeMirror-dragcursors");
      cm.display.lineSpace.insertBefore(cm.display.dragCursor, cm.display.cursorDiv);
    }
    removeChildrenAndAdd(cm.display.dragCursor, frag);
  }

  function clearDragCursor(cm) {
    if (cm.display.dragCursor) {
      cm.display.lineSpace.removeChild(cm.display.dragCursor);
      cm.display.dragCursor = null;
    }
  }

  // These must be handled carefully, because naively registering a
  // handler for each editor will cause the editors to never be
  // garbage collected.

  function forEachCodeMirror(f) {
    if (!document.getElementsByClassName) { return }
    var byClass = document.getElementsByClassName("CodeMirror"), editors = [];
    for (var i = 0; i < byClass.length; i++) {
      var cm = byClass[i].CodeMirror;
      if (cm) { editors.push(cm); }
    }
    if (editors.length) { editors[0].operation(function () {
      for (var i = 0; i < editors.length; i++) { f(editors[i]); }
    }); }
  }

  var globalsRegistered = false;
  function ensureGlobalHandlers() {
    if (globalsRegistered) { return }
    registerGlobalHandlers();
    globalsRegistered = true;
  }
  function registerGlobalHandlers() {
    // When the window resizes, we need to refresh active editors.
    var resizeTimer;
    on(window, "resize", function () {
      if (resizeTimer == null) { resizeTimer = setTimeout(function () {
        resizeTimer = null;
        forEachCodeMirror(onResize);
      }, 100); }
    });
    // When the window loses focus, we want to show the editor as blurred
    on(window, "blur", function () { return forEachCodeMirror(onBlur); });
  }
  // Called when the window resizes
  function onResize(cm) {
    var d = cm.display;
    // Might be a text scaling operation, clear size caches.
    d.cachedCharWidth = d.cachedTextHeight = d.cachedPaddingH = null;
    d.scrollbarsClipped = false;
    cm.setSize();
  }

  var keyNames = {
    3: "Pause", 8: "Backspace", 9: "Tab", 13: "Enter", 16: "Shift", 17: "Ctrl", 18: "Alt",
    19: "Pause", 20: "CapsLock", 27: "Esc", 32: "Space", 33: "PageUp", 34: "PageDown", 35: "End",
    36: "Home", 37: "Left", 38: "Up", 39: "Right", 40: "Down", 44: "PrintScrn", 45: "Insert",
    46: "Delete", 59: ";", 61: "=", 91: "Mod", 92: "Mod", 93: "Mod",
    106: "*", 107: "=", 109: "-", 110: ".", 111: "/", 145: "ScrollLock",
    173: "-", 186: ";", 187: "=", 188: ",", 189: "-", 190: ".", 191: "/", 192: "`", 219: "[", 220: "\\",
    221: "]", 222: "'", 224: "Mod", 63232: "Up", 63233: "Down", 63234: "Left", 63235: "Right", 63272: "Delete",
    63273: "Home", 63275: "End", 63276: "PageUp", 63277: "PageDown", 63302: "Insert"
  };

  // Number keys
  for (var i = 0; i < 10; i++) { keyNames[i + 48] = keyNames[i + 96] = String(i); }
  // Alphabetic keys
  for (var i$1 = 65; i$1 <= 90; i$1++) { keyNames[i$1] = String.fromCharCode(i$1); }
  // Function keys
  for (var i$2 = 1; i$2 <= 12; i$2++) { keyNames[i$2 + 111] = keyNames[i$2 + 63235] = "F" + i$2; }

  var keyMap = {};

  keyMap.basic = {
    "Left": "goCharLeft", "Right": "goCharRight", "Up": "goLineUp", "Down": "goLineDown",
    "End": "goLineEnd", "Home": "goLineStartSmart", "PageUp": "goPageUp", "PageDown": "goPageDown",
    "Delete": "delCharAfter", "Backspace": "delCharBefore", "Shift-Backspace": "delCharBefore",
    "Tab": "defaultTab", "Shift-Tab": "indentAuto",
    "Enter": "newlineAndIndent", "Insert": "toggleOverwrite",
    "Esc": "singleSelection"
  };
  // Note that the save and find-related commands aren't defined by
  // default. User code or addons can define them. Unknown commands
  // are simply ignored.
  keyMap.pcDefault = {
    "Ctrl-A": "selectAll", "Ctrl-D": "deleteLine", "Ctrl-Z": "undo", "Shift-Ctrl-Z": "redo", "Ctrl-Y": "redo",
    "Ctrl-Home": "goDocStart", "Ctrl-End": "goDocEnd", "Ctrl-Up": "goLineUp", "Ctrl-Down": "goLineDown",
    "Ctrl-Left": "goGroupLeft", "Ctrl-Right": "goGroupRight", "Alt-Left": "goLineStart", "Alt-Right": "goLineEnd",
    "Ctrl-Backspace": "delGroupBefore", "Ctrl-Delete": "delGroupAfter", "Ctrl-S": "save", "Ctrl-F": "find",
    "Ctrl-G": "findNext", "Shift-Ctrl-G": "findPrev", "Shift-Ctrl-F": "replace", "Shift-Ctrl-R": "replaceAll",
    "Ctrl-[": "indentLess", "Ctrl-]": "indentMore",
    "Ctrl-U": "undoSelection", "Shift-Ctrl-U": "redoSelection", "Alt-U": "redoSelection",
    "fallthrough": "basic"
  };
  // Very basic readline/emacs-style bindings, which are standard on Mac.
  keyMap.emacsy = {
    "Ctrl-F": "goCharRight", "Ctrl-B": "goCharLeft", "Ctrl-P": "goLineUp", "Ctrl-N": "goLineDown",
    "Alt-F": "goWordRight", "Alt-B": "goWordLeft", "Ctrl-A": "goLineStart", "Ctrl-E": "goLineEnd",
    "Ctrl-V": "goPageDown", "Shift-Ctrl-V": "goPageUp", "Ctrl-D": "delCharAfter", "Ctrl-H": "delCharBefore",
    "Alt-D": "delWordAfter", "Alt-Backspace": "delWordBefore", "Ctrl-K": "killLine", "Ctrl-T": "transposeChars",
    "Ctrl-O": "openLine"
  };
  keyMap.macDefault = {
    "Cmd-A": "selectAll", "Cmd-D": "deleteLine", "Cmd-Z": "undo", "Shift-Cmd-Z": "redo", "Cmd-Y": "redo",
    "Cmd-Home": "goDocStart", "Cmd-Up": "goDocStart", "Cmd-End": "goDocEnd", "Cmd-Down": "goDocEnd", "Alt-Left": "goGroupLeft",
    "Alt-Right": "goGroupRight", "Cmd-Left": "goLineLeft", "Cmd-Right": "goLineRight", "Alt-Backspace": "delGroupBefore",
    "Ctrl-Alt-Backspace": "delGroupAfter", "Alt-Delete": "delGroupAfter", "Cmd-S": "save", "Cmd-F": "find",
    "Cmd-G": "findNext", "Shift-Cmd-G": "findPrev", "Cmd-Alt-F": "replace", "Shift-Cmd-Alt-F": "replaceAll",
    "Cmd-[": "indentLess", "Cmd-]": "indentMore", "Cmd-Backspace": "delWrappedLineLeft", "Cmd-Delete": "delWrappedLineRight",
    "Cmd-U": "undoSelection", "Shift-Cmd-U": "redoSelection", "Ctrl-Up": "goDocStart", "Ctrl-Down": "goDocEnd",
    "fallthrough": ["basic", "emacsy"]
  };
  keyMap["default"] = mac ? keyMap.macDefault : keyMap.pcDefault;

  // KEYMAP DISPATCH

  function normalizeKeyName(name) {
    var parts = name.split(/-(?!$)/);
    name = parts[parts.length - 1];
    var alt, ctrl, shift, cmd;
    for (var i = 0; i < parts.length - 1; i++) {
      var mod = parts[i];
      if (/^(cmd|meta|m)$/i.test(mod)) { cmd = true; }
      else if (/^a(lt)?$/i.test(mod)) { alt = true; }
      else if (/^(c|ctrl|control)$/i.test(mod)) { ctrl = true; }
      else if (/^s(hift)?$/i.test(mod)) { shift = true; }
      else { throw new Error("Unrecognized modifier name: " + mod) }
    }
    if (alt) { name = "Alt-" + name; }
    if (ctrl) { name = "Ctrl-" + name; }
    if (cmd) { name = "Cmd-" + name; }
    if (shift) { name = "Shift-" + name; }
    return name
  }

  // This is a kludge to keep keymaps mostly working as raw objects
  // (backwards compatibility) while at the same time support features
  // like normalization and multi-stroke key bindings. It compiles a
  // new normalized keymap, and then updates the old object to reflect
  // this.
  function normalizeKeyMap(keymap) {
    var copy = {};
    for (var keyname in keymap) { if (keymap.hasOwnProperty(keyname)) {
      var value = keymap[keyname];
      if (/^(name|fallthrough|(de|at)tach)$/.test(keyname)) { continue }
      if (value == "...") { delete keymap[keyname]; continue }

      var keys = map(keyname.split(" "), normalizeKeyName);
      for (var i = 0; i < keys.length; i++) {
        var val = (void 0), name = (void 0);
        if (i == keys.length - 1) {
          name = keys.join(" ");
          val = value;
        } else {
          name = keys.slice(0, i + 1).join(" ");
          val = "...";
        }
        var prev = copy[name];
        if (!prev) { copy[name] = val; }
        else if (prev != val) { throw new Error("Inconsistent bindings for " + name) }
      }
      delete keymap[keyname];
    } }
    for (var prop in copy) { keymap[prop] = copy[prop]; }
    return keymap
  }

  function lookupKey(key, map, handle, context) {
    map = getKeyMap(map);
    var found = map.call ? map.call(key, context) : map[key];
    if (found === false) { return "nothing" }
    if (found === "...") { return "multi" }
    if (found != null && handle(found)) { return "handled" }

    if (map.fallthrough) {
      if (Object.prototype.toString.call(map.fallthrough) != "[object Array]")
        { return lookupKey(key, map.fallthrough, handle, context) }
      for (var i = 0; i < map.fallthrough.length; i++) {
        var result = lookupKey(key, map.fallthrough[i], handle, context);
        if (result) { return result }
      }
    }
  }

  // Modifier key presses don't count as 'real' key presses for the
  // purpose of keymap fallthrough.
  function isModifierKey(value) {
    var name = typeof value == "string" ? value : keyNames[value.keyCode];
    return name == "Ctrl" || name == "Alt" || name == "Shift" || name == "Mod"
  }

  function addModifierNames(name, event, noShift) {
    var base = name;
    if (event.altKey && base != "Alt") { name = "Alt-" + name; }
    if ((flipCtrlCmd ? event.metaKey : event.ctrlKey) && base != "Ctrl") { name = "Ctrl-" + name; }
    if ((flipCtrlCmd ? event.ctrlKey : event.metaKey) && base != "Mod") { name = "Cmd-" + name; }
    if (!noShift && event.shiftKey && base != "Shift") { name = "Shift-" + name; }
    return name
  }

  // Look up the name of a key as indicated by an event object.
  function keyName(event, noShift) {
    if (presto && event.keyCode == 34 && event["char"]) { return false }
    var name = keyNames[event.keyCode];
    if (name == null || event.altGraphKey) { return false }
    // Ctrl-ScrollLock has keyCode 3, same as Ctrl-Pause,
    // so we'll use event.code when available (Chrome 48+, FF 38+, Safari 10.1+)
    if (event.keyCode == 3 && event.code) { name = event.code; }
    return addModifierNames(name, event, noShift)
  }

  function getKeyMap(val) {
    return typeof val == "string" ? keyMap[val] : val
  }

  // Helper for deleting text near the selection(s), used to implement
  // backspace, delete, and similar functionality.
  function deleteNearSelection(cm, compute) {
    var ranges = cm.doc.sel.ranges, kill = [];
    // Build up a set of ranges to kill first, merging overlapping
    // ranges.
    for (var i = 0; i < ranges.length; i++) {
      var toKill = compute(ranges[i]);
      while (kill.length && cmp(toKill.from, lst(kill).to) <= 0) {
        var replaced = kill.pop();
        if (cmp(replaced.from, toKill.from) < 0) {
          toKill.from = replaced.from;
          break
        }
      }
      kill.push(toKill);
    }
    // Next, remove those actual ranges.
    runInOp(cm, function () {
      for (var i = kill.length - 1; i >= 0; i--)
        { replaceRange(cm.doc, "", kill[i].from, kill[i].to, "+delete"); }
      ensureCursorVisible(cm);
    });
  }

  function moveCharLogically(line, ch, dir) {
    var target = skipExtendingChars(line.text, ch + dir, dir);
    return target < 0 || target > line.text.length ? null : target
  }

  function moveLogically(line, start, dir) {
    var ch = moveCharLogically(line, start.ch, dir);
    return ch == null ? null : new Pos(start.line, ch, dir < 0 ? "after" : "before")
  }

  function endOfLine(visually, cm, lineObj, lineNo, dir) {
    if (visually) {
      if (cm.doc.direction == "rtl") { dir = -dir; }
      var order = getOrder(lineObj, cm.doc.direction);
      if (order) {
        var part = dir < 0 ? lst(order) : order[0];
        var moveInStorageOrder = (dir < 0) == (part.level == 1);
        var sticky = moveInStorageOrder ? "after" : "before";
        var ch;
        // With a wrapped rtl chunk (possibly spanning multiple bidi parts),
        // it could be that the last bidi part is not on the last visual line,
        // since visual lines contain content order-consecutive chunks.
        // Thus, in rtl, we are looking for the first (content-order) character
        // in the rtl chunk that is on the last line (that is, the same line
        // as the last (content-order) character).
        if (part.level > 0 || cm.doc.direction == "rtl") {
          var prep = prepareMeasureForLine(cm, lineObj);
          ch = dir < 0 ? lineObj.text.length - 1 : 0;
          var targetTop = measureCharPrepared(cm, prep, ch).top;
          ch = findFirst(function (ch) { return measureCharPrepared(cm, prep, ch).top == targetTop; }, (dir < 0) == (part.level == 1) ? part.from : part.to - 1, ch);
          if (sticky == "before") { ch = moveCharLogically(lineObj, ch, 1); }
        } else { ch = dir < 0 ? part.to : part.from; }
        return new Pos(lineNo, ch, sticky)
      }
    }
    return new Pos(lineNo, dir < 0 ? lineObj.text.length : 0, dir < 0 ? "before" : "after")
  }

  function moveVisually(cm, line, start, dir) {
    var bidi = getOrder(line, cm.doc.direction);
    if (!bidi) { return moveLogically(line, start, dir) }
    if (start.ch >= line.text.length) {
      start.ch = line.text.length;
      start.sticky = "before";
    } else if (start.ch <= 0) {
      start.ch = 0;
      start.sticky = "after";
    }
    var partPos = getBidiPartAt(bidi, start.ch, start.sticky), part = bidi[partPos];
    if (cm.doc.direction == "ltr" && part.level % 2 == 0 && (dir > 0 ? part.to > start.ch : part.from < start.ch)) {
      // Case 1: We move within an ltr part in an ltr editor. Even with wrapped lines,
      // nothing interesting happens.
      return moveLogically(line, start, dir)
    }

    var mv = function (pos, dir) { return moveCharLogically(line, pos instanceof Pos ? pos.ch : pos, dir); };
    var prep;
    var getWrappedLineExtent = function (ch) {
      if (!cm.options.lineWrapping) { return {begin: 0, end: line.text.length} }
      prep = prep || prepareMeasureForLine(cm, line);
      return wrappedLineExtentChar(cm, line, prep, ch)
    };
    var wrappedLineExtent = getWrappedLineExtent(start.sticky == "before" ? mv(start, -1) : start.ch);

    if (cm.doc.direction == "rtl" || part.level == 1) {
      var moveInStorageOrder = (part.level == 1) == (dir < 0);
      var ch = mv(start, moveInStorageOrder ? 1 : -1);
      if (ch != null && (!moveInStorageOrder ? ch >= part.from && ch >= wrappedLineExtent.begin : ch <= part.to && ch <= wrappedLineExtent.end)) {
        // Case 2: We move within an rtl part or in an rtl editor on the same visual line
        var sticky = moveInStorageOrder ? "before" : "after";
        return new Pos(start.line, ch, sticky)
      }
    }

    // Case 3: Could not move within this bidi part in this visual line, so leave
    // the current bidi part

    var searchInVisualLine = function (partPos, dir, wrappedLineExtent) {
      var getRes = function (ch, moveInStorageOrder) { return moveInStorageOrder
        ? new Pos(start.line, mv(ch, 1), "before")
        : new Pos(start.line, ch, "after"); };

      for (; partPos >= 0 && partPos < bidi.length; partPos += dir) {
        var part = bidi[partPos];
        var moveInStorageOrder = (dir > 0) == (part.level != 1);
        var ch = moveInStorageOrder ? wrappedLineExtent.begin : mv(wrappedLineExtent.end, -1);
        if (part.from <= ch && ch < part.to) { return getRes(ch, moveInStorageOrder) }
        ch = moveInStorageOrder ? part.from : mv(part.to, -1);
        if (wrappedLineExtent.begin <= ch && ch < wrappedLineExtent.end) { return getRes(ch, moveInStorageOrder) }
      }
    };

    // Case 3a: Look for other bidi parts on the same visual line
    var res = searchInVisualLine(partPos + dir, dir, wrappedLineExtent);
    if (res) { return res }

    // Case 3b: Look for other bidi parts on the next visual line
    var nextCh = dir > 0 ? wrappedLineExtent.end : mv(wrappedLineExtent.begin, -1);
    if (nextCh != null && !(dir > 0 && nextCh == line.text.length)) {
      res = searchInVisualLine(dir > 0 ? 0 : bidi.length - 1, dir, getWrappedLineExtent(nextCh));
      if (res) { return res }
    }

    // Case 4: Nowhere to move
    return null
  }

  // Commands are parameter-less actions that can be performed on an
  // editor, mostly used for keybindings.
  var commands = {
    selectAll: selectAll,
    singleSelection: function (cm) { return cm.setSelection(cm.getCursor("anchor"), cm.getCursor("head"), sel_dontScroll); },
    killLine: function (cm) { return deleteNearSelection(cm, function (range) {
      if (range.empty()) {
        var len = getLine(cm.doc, range.head.line).text.length;
        if (range.head.ch == len && range.head.line < cm.lastLine())
          { return {from: range.head, to: Pos(range.head.line + 1, 0)} }
        else
          { return {from: range.head, to: Pos(range.head.line, len)} }
      } else {
        return {from: range.from(), to: range.to()}
      }
    }); },
    deleteLine: function (cm) { return deleteNearSelection(cm, function (range) { return ({
      from: Pos(range.from().line, 0),
      to: clipPos(cm.doc, Pos(range.to().line + 1, 0))
    }); }); },
    delLineLeft: function (cm) { return deleteNearSelection(cm, function (range) { return ({
      from: Pos(range.from().line, 0), to: range.from()
    }); }); },
    delWrappedLineLeft: function (cm) { return deleteNearSelection(cm, function (range) {
      var top = cm.charCoords(range.head, "div").top + 5;
      var leftPos = cm.coordsChar({left: 0, top: top}, "div");
      return {from: leftPos, to: range.from()}
    }); },
    delWrappedLineRight: function (cm) { return deleteNearSelection(cm, function (range) {
      var top = cm.charCoords(range.head, "div").top + 5;
      var rightPos = cm.coordsChar({left: cm.display.lineDiv.offsetWidth + 100, top: top}, "div");
      return {from: range.from(), to: rightPos }
    }); },
    undo: function (cm) { return cm.undo(); },
    redo: function (cm) { return cm.redo(); },
    undoSelection: function (cm) { return cm.undoSelection(); },
    redoSelection: function (cm) { return cm.redoSelection(); },
    goDocStart: function (cm) { return cm.extendSelection(Pos(cm.firstLine(), 0)); },
    goDocEnd: function (cm) { return cm.extendSelection(Pos(cm.lastLine())); },
    goLineStart: function (cm) { return cm.extendSelectionsBy(function (range) { return lineStart(cm, range.head.line); },
      {origin: "+move", bias: 1}
    ); },
    goLineStartSmart: function (cm) { return cm.extendSelectionsBy(function (range) { return lineStartSmart(cm, range.head); },
      {origin: "+move", bias: 1}
    ); },
    goLineEnd: function (cm) { return cm.extendSelectionsBy(function (range) { return lineEnd(cm, range.head.line); },
      {origin: "+move", bias: -1}
    ); },
    goLineRight: function (cm) { return cm.extendSelectionsBy(function (range) {
      var top = cm.cursorCoords(range.head, "div").top + 5;
      return cm.coordsChar({left: cm.display.lineDiv.offsetWidth + 100, top: top}, "div")
    }, sel_move); },
    goLineLeft: function (cm) { return cm.extendSelectionsBy(function (range) {
      var top = cm.cursorCoords(range.head, "div").top + 5;
      return cm.coordsChar({left: 0, top: top}, "div")
    }, sel_move); },
    goLineLeftSmart: function (cm) { return cm.extendSelectionsBy(function (range) {
      var top = cm.cursorCoords(range.head, "div").top + 5;
      var pos = cm.coordsChar({left: 0, top: top}, "div");
      if (pos.ch < cm.getLine(pos.line).search(/\S/)) { return lineStartSmart(cm, range.head) }
      return pos
    }, sel_move); },
    goLineUp: function (cm) { return cm.moveV(-1, "line"); },
    goLineDown: function (cm) { return cm.moveV(1, "line"); },
    goPageUp: function (cm) { return cm.moveV(-1, "page"); },
    goPageDown: function (cm) { return cm.moveV(1, "page"); },
    goCharLeft: function (cm) { return cm.moveH(-1, "char"); },
    goCharRight: function (cm) { return cm.moveH(1, "char"); },
    goColumnLeft: function (cm) { return cm.moveH(-1, "column"); },
    goColumnRight: function (cm) { return cm.moveH(1, "column"); },
    goWordLeft: function (cm) { return cm.moveH(-1, "word"); },
    goGroupRight: function (cm) { return cm.moveH(1, "group"); },
    goGroupLeft: function (cm) { return cm.moveH(-1, "group"); },
    goWordRight: function (cm) { return cm.moveH(1, "word"); },
    delCharBefore: function (cm) { return cm.deleteH(-1, "codepoint"); },
    delCharAfter: function (cm) { return cm.deleteH(1, "char"); },
    delWordBefore: function (cm) { return cm.deleteH(-1, "word"); },
    delWordAfter: function (cm) { return cm.deleteH(1, "word"); },
    delGroupBefore: function (cm) { return cm.deleteH(-1, "group"); },
    delGroupAfter: function (cm) { return cm.deleteH(1, "group"); },
    indentAuto: function (cm) { return cm.indentSelection("smart"); },
    indentMore: function (cm) { return cm.indentSelection("add"); },
    indentLess: function (cm) { return cm.indentSelection("subtract"); },
    insertTab: function (cm) { return cm.replaceSelection("\t"); },
    insertSoftTab: function (cm) {
      var spaces = [], ranges = cm.listSelections(), tabSize = cm.options.tabSize;
      for (var i = 0; i < ranges.length; i++) {
        var pos = ranges[i].from();
        var col = countColumn(cm.getLine(pos.line), pos.ch, tabSize);
        spaces.push(spaceStr(tabSize - col % tabSize));
      }
      cm.replaceSelections(spaces);
    },
    defaultTab: function (cm) {
      if (cm.somethingSelected()) { cm.indentSelection("add"); }
      else { cm.execCommand("insertTab"); }
    },
    // Swap the two chars left and right of each selection's head.
    // Move cursor behind the two swapped characters afterwards.
    //
    // Doesn't consider line feeds a character.
    // Doesn't scan more than one line above to find a character.
    // Doesn't do anything on an empty line.
    // Doesn't do anything with non-empty selections.
    transposeChars: function (cm) { return runInOp(cm, function () {
      var ranges = cm.listSelections(), newSel = [];
      for (var i = 0; i < ranges.length; i++) {
        if (!ranges[i].empty()) { continue }
        var cur = ranges[i].head, line = getLine(cm.doc, cur.line).text;
        if (line) {
          if (cur.ch == line.length) { cur = new Pos(cur.line, cur.ch - 1); }
          if (cur.ch > 0) {
            cur = new Pos(cur.line, cur.ch + 1);
            cm.replaceRange(line.charAt(cur.ch - 1) + line.charAt(cur.ch - 2),
                            Pos(cur.line, cur.ch - 2), cur, "+transpose");
          } else if (cur.line > cm.doc.first) {
            var prev = getLine(cm.doc, cur.line - 1).text;
            if (prev) {
              cur = new Pos(cur.line, 1);
              cm.replaceRange(line.charAt(0) + cm.doc.lineSeparator() +
                              prev.charAt(prev.length - 1),
                              Pos(cur.line - 1, prev.length - 1), cur, "+transpose");
            }
          }
        }
        newSel.push(new Range(cur, cur));
      }
      cm.setSelections(newSel);
    }); },
    newlineAndIndent: function (cm) { return runInOp(cm, function () {
      var sels = cm.listSelections();
      for (var i = sels.length - 1; i >= 0; i--)
        { cm.replaceRange(cm.doc.lineSeparator(), sels[i].anchor, sels[i].head, "+input"); }
      sels = cm.listSelections();
      for (var i$1 = 0; i$1 < sels.length; i$1++)
        { cm.indentLine(sels[i$1].from().line, null, true); }
      ensureCursorVisible(cm);
    }); },
    openLine: function (cm) { return cm.replaceSelection("\n", "start"); },
    toggleOverwrite: function (cm) { return cm.toggleOverwrite(); }
  };


  function lineStart(cm, lineN) {
    var line = getLine(cm.doc, lineN);
    var visual = visualLine(line);
    if (visual != line) { lineN = lineNo(visual); }
    return endOfLine(true, cm, visual, lineN, 1)
  }
  function lineEnd(cm, lineN) {
    var line = getLine(cm.doc, lineN);
    var visual = visualLineEnd(line);
    if (visual != line) { lineN = lineNo(visual); }
    return endOfLine(true, cm, line, lineN, -1)
  }
  function lineStartSmart(cm, pos) {
    var start = lineStart(cm, pos.line);
    var line = getLine(cm.doc, start.line);
    var order = getOrder(line, cm.doc.direction);
    if (!order || order[0].level == 0) {
      var firstNonWS = Math.max(start.ch, line.text.search(/\S/));
      var inWS = pos.line == start.line && pos.ch <= firstNonWS && pos.ch;
      return Pos(start.line, inWS ? 0 : firstNonWS, start.sticky)
    }
    return start
  }

  // Run a handler that was bound to a key.
  function doHandleBinding(cm, bound, dropShift) {
    if (typeof bound == "string") {
      bound = commands[bound];
      if (!bound) { return false }
    }
    // Ensure previous input has been read, so that the handler sees a
    // consistent view of the document
    cm.display.input.ensurePolled();
    var prevShift = cm.display.shift, done = false;
    try {
      if (cm.isReadOnly()) { cm.state.suppressEdits = true; }
      if (dropShift) { cm.display.shift = false; }
      done = bound(cm) != Pass;
    } finally {
      cm.display.shift = prevShift;
      cm.state.suppressEdits = false;
    }
    return done
  }

  function lookupKeyForEditor(cm, name, handle) {
    for (var i = 0; i < cm.state.keyMaps.length; i++) {
      var result = lookupKey(name, cm.state.keyMaps[i], handle, cm);
      if (result) { return result }
    }
    return (cm.options.extraKeys && lookupKey(name, cm.options.extraKeys, handle, cm))
      || lookupKey(name, cm.options.keyMap, handle, cm)
  }

  // Note that, despite the name, this function is also used to check
  // for bound mouse clicks.

  var stopSeq = new Delayed;

  function dispatchKey(cm, name, e, handle) {
    var seq = cm.state.keySeq;
    if (seq) {
      if (isModifierKey(name)) { return "handled" }
      if (/\'$/.test(name))
        { cm.state.keySeq = null; }
      else
        { stopSeq.set(50, function () {
          if (cm.state.keySeq == seq) {
            cm.state.keySeq = null;
            cm.display.input.reset();
          }
        }); }
      if (dispatchKeyInner(cm, seq + " " + name, e, handle)) { return true }
    }
    return dispatchKeyInner(cm, name, e, handle)
  }

  function dispatchKeyInner(cm, name, e, handle) {
    var result = lookupKeyForEditor(cm, name, handle);

    if (result == "multi")
      { cm.state.keySeq = name; }
    if (result == "handled")
      { signalLater(cm, "keyHandled", cm, name, e); }

    if (result == "handled" || result == "multi") {
      e_preventDefault(e);
      restartBlink(cm);
    }

    return !!result
  }

  // Handle a key from the keydown event.
  function handleKeyBinding(cm, e) {
    var name = keyName(e, true);
    if (!name) { return false }

    if (e.shiftKey && !cm.state.keySeq) {
      // First try to resolve full name (including 'Shift-'). Failing
      // that, see if there is a cursor-motion command (starting with
      // 'go') bound to the keyname without 'Shift-'.
      return dispatchKey(cm, "Shift-" + name, e, function (b) { return doHandleBinding(cm, b, true); })
          || dispatchKey(cm, name, e, function (b) {
               if (typeof b == "string" ? /^go[A-Z]/.test(b) : b.motion)
                 { return doHandleBinding(cm, b) }
             })
    } else {
      return dispatchKey(cm, name, e, function (b) { return doHandleBinding(cm, b); })
    }
  }

  // Handle a key from the keypress event
  function handleCharBinding(cm, e, ch) {
    return dispatchKey(cm, "'" + ch + "'", e, function (b) { return doHandleBinding(cm, b, true); })
  }

  var lastStoppedKey = null;
  function onKeyDown(e) {
    var cm = this;
    if (e.target && e.target != cm.display.input.getField()) { return }
    cm.curOp.focus = activeElt();
    if (signalDOMEvent(cm, e)) { return }
    // IE does strange things with escape.
    if (ie && ie_version < 11 && e.keyCode == 27) { e.returnValue = false; }
    var code = e.keyCode;
    cm.display.shift = code == 16 || e.shiftKey;
    var handled = handleKeyBinding(cm, e);
    if (presto) {
      lastStoppedKey = handled ? code : null;
      // Opera has no cut event... we try to at least catch the key combo
      if (!handled && code == 88 && !hasCopyEvent && (mac ? e.metaKey : e.ctrlKey))
        { cm.replaceSelection("", null, "cut"); }
    }
    if (gecko && !mac && !handled && code == 46 && e.shiftKey && !e.ctrlKey && document.execCommand)
      { document.execCommand("cut"); }

    // Turn mouse into crosshair when Alt is held on Mac.
    if (code == 18 && !/\bCodeMirror-crosshair\b/.test(cm.display.lineDiv.className))
      { showCrossHair(cm); }
  }

  function showCrossHair(cm) {
    var lineDiv = cm.display.lineDiv;
    addClass(lineDiv, "CodeMirror-crosshair");

    function up(e) {
      if (e.keyCode == 18 || !e.altKey) {
        rmClass(lineDiv, "CodeMirror-crosshair");
        off(document, "keyup", up);
        off(document, "mouseover", up);
      }
    }
    on(document, "keyup", up);
    on(document, "mouseover", up);
  }

  function onKeyUp(e) {
    if (e.keyCode == 16) { this.doc.sel.shift = false; }
    signalDOMEvent(this, e);
  }

  function onKeyPress(e) {
    var cm = this;
    if (e.target && e.target != cm.display.input.getField()) { return }
    if (eventInWidget(cm.display, e) || signalDOMEvent(cm, e) || e.ctrlKey && !e.altKey || mac && e.metaKey) { return }
    var keyCode = e.keyCode, charCode = e.charCode;
    if (presto && keyCode == lastStoppedKey) {lastStoppedKey = null; e_preventDefault(e); return}
    if ((presto && (!e.which || e.which < 10)) && handleKeyBinding(cm, e)) { return }
    var ch = String.fromCharCode(charCode == null ? keyCode : charCode);
    // Some browsers fire keypress events for backspace
    if (ch == "\x08") { return }
    if (handleCharBinding(cm, e, ch)) { return }
    cm.display.input.onKeyPress(e);
  }

  var DOUBLECLICK_DELAY = 400;

  var PastClick = function(time, pos, button) {
    this.time = time;
    this.pos = pos;
    this.button = button;
  };

  PastClick.prototype.compare = function (time, pos, button) {
    return this.time + DOUBLECLICK_DELAY > time &&
      cmp(pos, this.pos) == 0 && button == this.button
  };

  var lastClick, lastDoubleClick;
  function clickRepeat(pos, button) {
    var now = +new Date;
    if (lastDoubleClick && lastDoubleClick.compare(now, pos, button)) {
      lastClick = lastDoubleClick = null;
      return "triple"
    } else if (lastClick && lastClick.compare(now, pos, button)) {
      lastDoubleClick = new PastClick(now, pos, button);
      lastClick = null;
      return "double"
    } else {
      lastClick = new PastClick(now, pos, button);
      lastDoubleClick = null;
      return "single"
    }
  }

  // A mouse down can be a single click, double click, triple click,
  // start of selection drag, start of text drag, new cursor
  // (ctrl-click), rectangle drag (alt-drag), or xwin
  // middle-click-paste. Or it might be a click on something we should
  // not interfere with, such as a scrollbar or widget.
  function onMouseDown(e) {
    var cm = this, display = cm.display;
    if (signalDOMEvent(cm, e) || display.activeTouch && display.input.supportsTouch()) { return }
    display.input.ensurePolled();
    display.shift = e.shiftKey;

    if (eventInWidget(display, e)) {
      if (!webkit) {
        // Briefly turn off draggability, to allow widgets to do
        // normal dragging things.
        display.scroller.draggable = false;
        setTimeout(function () { return display.scroller.draggable = true; }, 100);
      }
      return
    }
    if (clickInGutter(cm, e)) { return }
    var pos = posFromMouse(cm, e), button = e_button(e), repeat = pos ? clickRepeat(pos, button) : "single";
    window.focus();

    // #3261: make sure, that we're not starting a second selection
    if (button == 1 && cm.state.selectingText)
      { cm.state.selectingText(e); }

    if (pos && handleMappedButton(cm, button, pos, repeat, e)) { return }

    if (button == 1) {
      if (pos) { leftButtonDown(cm, pos, repeat, e); }
      else if (e_target(e) == display.scroller) { e_preventDefault(e); }
    } else if (button == 2) {
      if (pos) { extendSelection(cm.doc, pos); }
      setTimeout(function () { return display.input.focus(); }, 20);
    } else if (button == 3) {
      if (captureRightClick) { cm.display.input.onContextMenu(e); }
      else { delayBlurEvent(cm); }
    }
  }

  function handleMappedButton(cm, button, pos, repeat, event) {
    var name = "Click";
    if (repeat == "double") { name = "Double" + name; }
    else if (repeat == "triple") { name = "Triple" + name; }
    name = (button == 1 ? "Left" : button == 2 ? "Middle" : "Right") + name;

    return dispatchKey(cm,  addModifierNames(name, event), event, function (bound) {
      if (typeof bound == "string") { bound = commands[bound]; }
      if (!bound) { return false }
      var done = false;
      try {
        if (cm.isReadOnly()) { cm.state.suppressEdits = true; }
        done = bound(cm, pos) != Pass;
      } finally {
        cm.state.suppressEdits = false;
      }
      return done
    })
  }

  function configureMouse(cm, repeat, event) {
    var option = cm.getOption("configureMouse");
    var value = option ? option(cm, repeat, event) : {};
    if (value.unit == null) {
      var rect = chromeOS ? event.shiftKey && event.metaKey : event.altKey;
      value.unit = rect ? "rectangle" : repeat == "single" ? "char" : repeat == "double" ? "word" : "line";
    }
    if (value.extend == null || cm.doc.extend) { value.extend = cm.doc.extend || event.shiftKey; }
    if (value.addNew == null) { value.addNew = mac ? event.metaKey : event.ctrlKey; }
    if (value.moveOnDrag == null) { value.moveOnDrag = !(mac ? event.altKey : event.ctrlKey); }
    return value
  }

  function leftButtonDown(cm, pos, repeat, event) {
    if (ie) { setTimeout(bind(ensureFocus, cm), 0); }
    else { cm.curOp.focus = activeElt(); }

    var behavior = configureMouse(cm, repeat, event);

    var sel = cm.doc.sel, contained;
    if (cm.options.dragDrop && dragAndDrop && !cm.isReadOnly() &&
        repeat == "single" && (contained = sel.contains(pos)) > -1 &&
        (cmp((contained = sel.ranges[contained]).from(), pos) < 0 || pos.xRel > 0) &&
        (cmp(contained.to(), pos) > 0 || pos.xRel < 0))
      { leftButtonStartDrag(cm, event, pos, behavior); }
    else
      { leftButtonSelect(cm, event, pos, behavior); }
  }

  // Start a text drag. When it ends, see if any dragging actually
  // happen, and treat as a click if it didn't.
  function leftButtonStartDrag(cm, event, pos, behavior) {
    var display = cm.display, moved = false;
    var dragEnd = operation(cm, function (e) {
      if (webkit) { display.scroller.draggable = false; }
      cm.state.draggingText = false;
      off(display.wrapper.ownerDocument, "mouseup", dragEnd);
      off(display.wrapper.ownerDocument, "mousemove", mouseMove);
      off(display.scroller, "dragstart", dragStart);
      off(display.scroller, "drop", dragEnd);
      if (!moved) {
        e_preventDefault(e);
        if (!behavior.addNew)
          { extendSelection(cm.doc, pos, null, null, behavior.extend); }
        // Work around unexplainable focus problem in IE9 (#2127) and Chrome (#3081)
        if ((webkit && !safari) || ie && ie_version == 9)
          { setTimeout(function () {display.wrapper.ownerDocument.body.focus({preventScroll: true}); display.input.focus();}, 20); }
        else
          { display.input.focus(); }
      }
    });
    var mouseMove = function(e2) {
      moved = moved || Math.abs(event.clientX - e2.clientX) + Math.abs(event.clientY - e2.clientY) >= 10;
    };
    var dragStart = function () { return moved = true; };
    // Let the drag handler handle this.
    if (webkit) { display.scroller.draggable = true; }
    cm.state.draggingText = dragEnd;
    dragEnd.copy = !behavior.moveOnDrag;
    // IE's approach to draggable
    if (display.scroller.dragDrop) { display.scroller.dragDrop(); }
    on(display.wrapper.ownerDocument, "mouseup", dragEnd);
    on(display.wrapper.ownerDocument, "mousemove", mouseMove);
    on(display.scroller, "dragstart", dragStart);
    on(display.scroller, "drop", dragEnd);

    delayBlurEvent(cm);
    setTimeout(function () { return display.input.focus(); }, 20);
  }

  function rangeForUnit(cm, pos, unit) {
    if (unit == "char") { return new Range(pos, pos) }
    if (unit == "word") { return cm.findWordAt(pos) }
    if (unit == "line") { return new Range(Pos(pos.line, 0), clipPos(cm.doc, Pos(pos.line + 1, 0))) }
    var result = unit(cm, pos);
    return new Range(result.from, result.to)
  }

  // Normal selection, as opposed to text dragging.
  function leftButtonSelect(cm, event, start, behavior) {
    var display = cm.display, doc = cm.doc;
    e_preventDefault(event);

    var ourRange, ourIndex, startSel = doc.sel, ranges = startSel.ranges;
    if (behavior.addNew && !behavior.extend) {
      ourIndex = doc.sel.contains(start);
      if (ourIndex > -1)
        { ourRange = ranges[ourIndex]; }
      else
        { ourRange = new Range(start, start); }
    } else {
      ourRange = doc.sel.primary();
      ourIndex = doc.sel.primIndex;
    }

    if (behavior.unit == "rectangle") {
      if (!behavior.addNew) { ourRange = new Range(start, start); }
      start = posFromMouse(cm, event, true, true);
      ourIndex = -1;
    } else {
      var range = rangeForUnit(cm, start, behavior.unit);
      if (behavior.extend)
        { ourRange = extendRange(ourRange, range.anchor, range.head, behavior.extend); }
      else
        { ourRange = range; }
    }

    if (!behavior.addNew) {
      ourIndex = 0;
      setSelection(doc, new Selection([ourRange], 0), sel_mouse);
      startSel = doc.sel;
    } else if (ourIndex == -1) {
      ourIndex = ranges.length;
      setSelection(doc, normalizeSelection(cm, ranges.concat([ourRange]), ourIndex),
                   {scroll: false, origin: "*mouse"});
    } else if (ranges.length > 1 && ranges[ourIndex].empty() && behavior.unit == "char" && !behavior.extend) {
      setSelection(doc, normalizeSelection(cm, ranges.slice(0, ourIndex).concat(ranges.slice(ourIndex + 1)), 0),
                   {scroll: false, origin: "*mouse"});
      startSel = doc.sel;
    } else {
      replaceOneSelection(doc, ourIndex, ourRange, sel_mouse);
    }

    var lastPos = start;
    function extendTo(pos) {
      if (cmp(lastPos, pos) == 0) { return }
      lastPos = pos;

      if (behavior.unit == "rectangle") {
        var ranges = [], tabSize = cm.options.tabSize;
        var startCol = countColumn(getLine(doc, start.line).text, start.ch, tabSize);
        var posCol = countColumn(getLine(doc, pos.line).text, pos.ch, tabSize);
        var left = Math.min(startCol, posCol), right = Math.max(startCol, posCol);
        for (var line = Math.min(start.line, pos.line), end = Math.min(cm.lastLine(), Math.max(start.line, pos.line));
             line <= end; line++) {
          var text = getLine(doc, line).text, leftPos = findColumn(text, left, tabSize);
          if (left == right)
            { ranges.push(new Range(Pos(line, leftPos), Pos(line, leftPos))); }
          else if (text.length > leftPos)
            { ranges.push(new Range(Pos(line, leftPos), Pos(line, findColumn(text, right, tabSize)))); }
        }
        if (!ranges.length) { ranges.push(new Range(start, start)); }
        setSelection(doc, normalizeSelection(cm, startSel.ranges.slice(0, ourIndex).concat(ranges), ourIndex),
                     {origin: "*mouse", scroll: false});
        cm.scrollIntoView(pos);
      } else {
        var oldRange = ourRange;
        var range = rangeForUnit(cm, pos, behavior.unit);
        var anchor = oldRange.anchor, head;
        if (cmp(range.anchor, anchor) > 0) {
          head = range.head;
          anchor = minPos(oldRange.from(), range.anchor);
        } else {
          head = range.anchor;
          anchor = maxPos(oldRange.to(), range.head);
        }
        var ranges$1 = startSel.ranges.slice(0);
        ranges$1[ourIndex] = bidiSimplify(cm, new Range(clipPos(doc, anchor), head));
        setSelection(doc, normalizeSelection(cm, ranges$1, ourIndex), sel_mouse);
      }
    }

    var editorSize = display.wrapper.getBoundingClientRect();
    // Used to ensure timeout re-tries don't fire when another extend
    // happened in the meantime (clearTimeout isn't reliable -- at
    // least on Chrome, the timeouts still happen even when cleared,
    // if the clear happens after their scheduled firing time).
    var counter = 0;

    function extend(e) {
      var curCount = ++counter;
      var cur = posFromMouse(cm, e, true, behavior.unit == "rectangle");
      if (!cur) { return }
      if (cmp(cur, lastPos) != 0) {
        cm.curOp.focus = activeElt();
        extendTo(cur);
        var visible = visibleLines(display, doc);
        if (cur.line >= visible.to || cur.line < visible.from)
          { setTimeout(operation(cm, function () {if (counter == curCount) { extend(e); }}), 150); }
      } else {
        var outside = e.clientY < editorSize.top ? -20 : e.clientY > editorSize.bottom ? 20 : 0;
        if (outside) { setTimeout(operation(cm, function () {
          if (counter != curCount) { return }
          display.scroller.scrollTop += outside;
          extend(e);
        }), 50); }
      }
    }

    function done(e) {
      cm.state.selectingText = false;
      counter = Infinity;
      // If e is null or undefined we interpret this as someone trying
      // to explicitly cancel the selection rather than the user
      // letting go of the mouse button.
      if (e) {
        e_preventDefault(e);
        display.input.focus();
      }
      off(display.wrapper.ownerDocument, "mousemove", move);
      off(display.wrapper.ownerDocument, "mouseup", up);
      doc.history.lastSelOrigin = null;
    }

    var move = operation(cm, function (e) {
      if (e.buttons === 0 || !e_button(e)) { done(e); }
      else { extend(e); }
    });
    var up = operation(cm, done);
    cm.state.selectingText = up;
    on(display.wrapper.ownerDocument, "mousemove", move);
    on(display.wrapper.ownerDocument, "mouseup", up);
  }

  // Used when mouse-selecting to adjust the anchor to the proper side
  // of a bidi jump depending on the visual position of the head.
  function bidiSimplify(cm, range) {
    var anchor = range.anchor;
    var head = range.head;
    var anchorLine = getLine(cm.doc, anchor.line);
    if (cmp(anchor, head) == 0 && anchor.sticky == head.sticky) { return range }
    var order = getOrder(anchorLine);
    if (!order) { return range }
    var index = getBidiPartAt(order, anchor.ch, anchor.sticky), part = order[index];
    if (part.from != anchor.ch && part.to != anchor.ch) { return range }
    var boundary = index + ((part.from == anchor.ch) == (part.level != 1) ? 0 : 1);
    if (boundary == 0 || boundary == order.length) { return range }

    // Compute the relative visual position of the head compared to the
    // anchor (<0 is to the left, >0 to the right)
    var leftSide;
    if (head.line != anchor.line) {
      leftSide = (head.line - anchor.line) * (cm.doc.direction == "ltr" ? 1 : -1) > 0;
    } else {
      var headIndex = getBidiPartAt(order, head.ch, head.sticky);
      var dir = headIndex - index || (head.ch - anchor.ch) * (part.level == 1 ? -1 : 1);
      if (headIndex == boundary - 1 || headIndex == boundary)
        { leftSide = dir < 0; }
      else
        { leftSide = dir > 0; }
    }

    var usePart = order[boundary + (leftSide ? -1 : 0)];
    var from = leftSide == (usePart.level == 1);
    var ch = from ? usePart.from : usePart.to, sticky = from ? "after" : "before";
    return anchor.ch == ch && anchor.sticky == sticky ? range : new Range(new Pos(anchor.line, ch, sticky), head)
  }


  // Determines whether an event happened in the gutter, and fires the
  // handlers for the corresponding event.
  function gutterEvent(cm, e, type, prevent) {
    var mX, mY;
    if (e.touches) {
      mX = e.touches[0].clientX;
      mY = e.touches[0].clientY;
    } else {
      try { mX = e.clientX; mY = e.clientY; }
      catch(e$1) { return false }
    }
    if (mX >= Math.floor(cm.display.gutters.getBoundingClientRect().right)) { return false }
    if (prevent) { e_preventDefault(e); }

    var display = cm.display;
    var lineBox = display.lineDiv.getBoundingClientRect();

    if (mY > lineBox.bottom || !hasHandler(cm, type)) { return e_defaultPrevented(e) }
    mY -= lineBox.top - display.viewOffset;

    for (var i = 0; i < cm.display.gutterSpecs.length; ++i) {
      var g = display.gutters.childNodes[i];
      if (g && g.getBoundingClientRect().right >= mX) {
        var line = lineAtHeight(cm.doc, mY);
        var gutter = cm.display.gutterSpecs[i];
        signal(cm, type, cm, line, gutter.className, e);
        return e_defaultPrevented(e)
      }
    }
  }

  function clickInGutter(cm, e) {
    return gutterEvent(cm, e, "gutterClick", true)
  }

  // CONTEXT MENU HANDLING

  // To make the context menu work, we need to briefly unhide the
  // textarea (making it as unobtrusive as possible) to let the
  // right-click take effect on it.
  function onContextMenu(cm, e) {
    if (eventInWidget(cm.display, e) || contextMenuInGutter(cm, e)) { return }
    if (signalDOMEvent(cm, e, "contextmenu")) { return }
    if (!captureRightClick) { cm.display.input.onContextMenu(e); }
  }

  function contextMenuInGutter(cm, e) {
    if (!hasHandler(cm, "gutterContextMenu")) { return false }
    return gutterEvent(cm, e, "gutterContextMenu", false)
  }

  function themeChanged(cm) {
    cm.display.wrapper.className = cm.display.wrapper.className.replace(/\s*cm-s-\S+/g, "") +
      cm.options.theme.replace(/(^|\s)\s*/g, " cm-s-");
    clearCaches(cm);
  }

  var Init = {toString: function(){return "CodeMirror.Init"}};

  var defaults = {};
  var optionHandlers = {};

  function defineOptions(CodeMirror) {
    var optionHandlers = CodeMirror.optionHandlers;

    function option(name, deflt, handle, notOnInit) {
      CodeMirror.defaults[name] = deflt;
      if (handle) { optionHandlers[name] =
        notOnInit ? function (cm, val, old) {if (old != Init) { handle(cm, val, old); }} : handle; }
    }

    CodeMirror.defineOption = option;

    // Passed to option handlers when there is no old value.
    CodeMirror.Init = Init;

    // These two are, on init, called from the constructor because they
    // have to be initialized before the editor can start at all.
    option("value", "", function (cm, val) { return cm.setValue(val); }, true);
    option("mode", null, function (cm, val) {
      cm.doc.modeOption = val;
      loadMode(cm);
    }, true);

    option("indentUnit", 2, loadMode, true);
    option("indentWithTabs", false);
    option("smartIndent", true);
    option("tabSize", 4, function (cm) {
      resetModeState(cm);
      clearCaches(cm);
      regChange(cm);
    }, true);

    option("lineSeparator", null, function (cm, val) {
      cm.doc.lineSep = val;
      if (!val) { return }
      var newBreaks = [], lineNo = cm.doc.first;
      cm.doc.iter(function (line) {
        for (var pos = 0;;) {
          var found = line.text.indexOf(val, pos);
          if (found == -1) { break }
          pos = found + val.length;
          newBreaks.push(Pos(lineNo, found));
        }
        lineNo++;
      });
      for (var i = newBreaks.length - 1; i >= 0; i--)
        { replaceRange(cm.doc, val, newBreaks[i], Pos(newBreaks[i].line, newBreaks[i].ch + val.length)); }
    });
    option("specialChars", /[\u0000-\u001f\u007f-\u009f\u00ad\u061c\u200b-\u200c\u200e\u200f\u2028\u2029\ufeff\ufff9-\ufffc]/g, function (cm, val, old) {
      cm.state.specialChars = new RegExp(val.source + (val.test("\t") ? "" : "|\t"), "g");
      if (old != Init) { cm.refresh(); }
    });
    option("specialCharPlaceholder", defaultSpecialCharPlaceholder, function (cm) { return cm.refresh(); }, true);
    option("electricChars", true);
    option("inputStyle", mobile ? "contenteditable" : "textarea", function () {
      throw new Error("inputStyle can not (yet) be changed in a running editor") // FIXME
    }, true);
    option("spellcheck", false, function (cm, val) { return cm.getInputField().spellcheck = val; }, true);
    option("autocorrect", false, function (cm, val) { return cm.getInputField().autocorrect = val; }, true);
    option("autocapitalize", false, function (cm, val) { return cm.getInputField().autocapitalize = val; }, true);
    option("rtlMoveVisually", !windows);
    option("wholeLineUpdateBefore", true);

    option("theme", "default", function (cm) {
      themeChanged(cm);
      updateGutters(cm);
    }, true);
    option("keyMap", "default", function (cm, val, old) {
      var next = getKeyMap(val);
      var prev = old != Init && getKeyMap(old);
      if (prev && prev.detach) { prev.detach(cm, next); }
      if (next.attach) { next.attach(cm, prev || null); }
    });
    option("extraKeys", null);
    option("configureMouse", null);

    option("lineWrapping", false, wrappingChanged, true);
    option("gutters", [], function (cm, val) {
      cm.display.gutterSpecs = getGutters(val, cm.options.lineNumbers);
      updateGutters(cm);
    }, true);
    option("fixedGutter", true, function (cm, val) {
      cm.display.gutters.style.left = val ? compensateForHScroll(cm.display) + "px" : "0";
      cm.refresh();
    }, true);
    option("coverGutterNextToScrollbar", false, function (cm) { return updateScrollbars(cm); }, true);
    option("scrollbarStyle", "native", function (cm) {
      initScrollbars(cm);
      updateScrollbars(cm);
      cm.display.scrollbars.setScrollTop(cm.doc.scrollTop);
      cm.display.scrollbars.setScrollLeft(cm.doc.scrollLeft);
    }, true);
    option("lineNumbers", false, function (cm, val) {
      cm.display.gutterSpecs = getGutters(cm.options.gutters, val);
      updateGutters(cm);
    }, true);
    option("firstLineNumber", 1, updateGutters, true);
    option("lineNumberFormatter", function (integer) { return integer; }, updateGutters, true);
    option("showCursorWhenSelecting", false, updateSelection, true);

    option("resetSelectionOnContextMenu", true);
    option("lineWiseCopyCut", true);
    option("pasteLinesPerSelection", true);
    option("selectionsMayTouch", false);

    option("readOnly", false, function (cm, val) {
      if (val == "nocursor") {
        onBlur(cm);
        cm.display.input.blur();
      }
      cm.display.input.readOnlyChanged(val);
    });

    option("screenReaderLabel", null, function (cm, val) {
      val = (val === '') ? null : val;
      cm.display.input.screenReaderLabelChanged(val);
    });

    option("disableInput", false, function (cm, val) {if (!val) { cm.display.input.reset(); }}, true);
    option("dragDrop", true, dragDropChanged);
    option("allowDropFileTypes", null);

    option("cursorBlinkRate", 530);
    option("cursorScrollMargin", 0);
    option("cursorHeight", 1, updateSelection, true);
    option("singleCursorHeightPerLine", true, updateSelection, true);
    option("workTime", 100);
    option("workDelay", 100);
    option("flattenSpans", true, resetModeState, true);
    option("addModeClass", false, resetModeState, true);
    option("pollInterval", 100);
    option("undoDepth", 200, function (cm, val) { return cm.doc.history.undoDepth = val; });
    option("historyEventDelay", 1250);
    option("viewportMargin", 10, function (cm) { return cm.refresh(); }, true);
    option("maxHighlightLength", 10000, resetModeState, true);
    option("moveInputWithCursor", true, function (cm, val) {
      if (!val) { cm.display.input.resetPosition(); }
    });

    option("tabindex", null, function (cm, val) { return cm.display.input.getField().tabIndex = val || ""; });
    option("autofocus", null);
    option("direction", "ltr", function (cm, val) { return cm.doc.setDirection(val); }, true);
    option("phrases", null);
  }

  function dragDropChanged(cm, value, old) {
    var wasOn = old && old != Init;
    if (!value != !wasOn) {
      var funcs = cm.display.dragFunctions;
      var toggle = value ? on : off;
      toggle(cm.display.scroller, "dragstart", funcs.start);
      toggle(cm.display.scroller, "dragenter", funcs.enter);
      toggle(cm.display.scroller, "dragover", funcs.over);
      toggle(cm.display.scroller, "dragleave", funcs.leave);
      toggle(cm.display.scroller, "drop", funcs.drop);
    }
  }

  function wrappingChanged(cm) {
    if (cm.options.lineWrapping) {
      addClass(cm.display.wrapper, "CodeMirror-wrap");
      cm.display.sizer.style.minWidth = "";
      cm.display.sizerWidth = null;
    } else {
      rmClass(cm.display.wrapper, "CodeMirror-wrap");
      findMaxLine(cm);
    }
    estimateLineHeights(cm);
    regChange(cm);
    clearCaches(cm);
    setTimeout(function () { return updateScrollbars(cm); }, 100);
  }

  // A CodeMirror instance represents an editor. This is the object
  // that user code is usually dealing with.

  function CodeMirror(place, options) {
    var this$1 = this;

    if (!(this instanceof CodeMirror)) { return new CodeMirror(place, options) }

    this.options = options = options ? copyObj(options) : {};
    // Determine effective options based on given values and defaults.
    copyObj(defaults, options, false);

    var doc = options.value;
    if (typeof doc == "string") { doc = new Doc(doc, options.mode, null, options.lineSeparator, options.direction); }
    else if (options.mode) { doc.modeOption = options.mode; }
    this.doc = doc;

    var input = new CodeMirror.inputStyles[options.inputStyle](this);
    var display = this.display = new Display(place, doc, input, options);
    display.wrapper.CodeMirror = this;
    themeChanged(this);
    if (options.lineWrapping)
      { this.display.wrapper.className += " CodeMirror-wrap"; }
    initScrollbars(this);

    this.state = {
      keyMaps: [],  // stores maps added by addKeyMap
      overlays: [], // highlighting overlays, as added by addOverlay
      modeGen: 0,   // bumped when mode/overlay changes, used to invalidate highlighting info
      overwrite: false,
      delayingBlurEvent: false,
      focused: false,
      suppressEdits: false, // used to disable editing during key handlers when in readOnly mode
      pasteIncoming: -1, cutIncoming: -1, // help recognize paste/cut edits in input.poll
      selectingText: false,
      draggingText: false,
      highlight: new Delayed(), // stores highlight worker timeout
      keySeq: null,  // Unfinished key sequence
      specialChars: null
    };

    if (options.autofocus && !mobile) { display.input.focus(); }

    // Override magic textarea content restore that IE sometimes does
    // on our hidden textarea on reload
    if (ie && ie_version < 11) { setTimeout(function () { return this$1.display.input.reset(true); }, 20); }

    registerEventHandlers(this);
    ensureGlobalHandlers();

    startOperation(this);
    this.curOp.forceUpdate = true;
    attachDoc(this, doc);

    if ((options.autofocus && !mobile) || this.hasFocus())
      { setTimeout(function () {
        if (this$1.hasFocus() && !this$1.state.focused) { onFocus(this$1); }
      }, 20); }
    else
      { onBlur(this); }

    for (var opt in optionHandlers) { if (optionHandlers.hasOwnProperty(opt))
      { optionHandlers[opt](this, options[opt], Init); } }
    maybeUpdateLineNumberWidth(this);
    if (options.finishInit) { options.finishInit(this); }
    for (var i = 0; i < initHooks.length; ++i) { initHooks[i](this); }
    endOperation(this);
    // Suppress optimizelegibility in Webkit, since it breaks text
    // measuring on line wrapping boundaries.
    if (webkit && options.lineWrapping &&
        getComputedStyle(display.lineDiv).textRendering == "optimizelegibility")
      { display.lineDiv.style.textRendering = "auto"; }
  }

  // The default configuration options.
  CodeMirror.defaults = defaults;
  // Functions to run when options are changed.
  CodeMirror.optionHandlers = optionHandlers;

  // Attach the necessary event handlers when initializing the editor
  function registerEventHandlers(cm) {
    var d = cm.display;
    on(d.scroller, "mousedown", operation(cm, onMouseDown));
    // Older IE's will not fire a second mousedown for a double click
    if (ie && ie_version < 11)
      { on(d.scroller, "dblclick", operation(cm, function (e) {
        if (signalDOMEvent(cm, e)) { return }
        var pos = posFromMouse(cm, e);
        if (!pos || clickInGutter(cm, e) || eventInWidget(cm.display, e)) { return }
        e_preventDefault(e);
        var word = cm.findWordAt(pos);
        extendSelection(cm.doc, word.anchor, word.head);
      })); }
    else
      { on(d.scroller, "dblclick", function (e) { return signalDOMEvent(cm, e) || e_preventDefault(e); }); }
    // Some browsers fire contextmenu *after* opening the menu, at
    // which point we can't mess with it anymore. Context menu is
    // handled in onMouseDown for these browsers.
    on(d.scroller, "contextmenu", function (e) { return onContextMenu(cm, e); });
    on(d.input.getField(), "contextmenu", function (e) {
      if (!d.scroller.contains(e.target)) { onContextMenu(cm, e); }
    });

    // Used to suppress mouse event handling when a touch happens
    var touchFinished, prevTouch = {end: 0};
    function finishTouch() {
      if (d.activeTouch) {
        touchFinished = setTimeout(function () { return d.activeTouch = null; }, 1000);
        prevTouch = d.activeTouch;
        prevTouch.end = +new Date;
      }
    }
    function isMouseLikeTouchEvent(e) {
      if (e.touches.length != 1) { return false }
      var touch = e.touches[0];
      return touch.radiusX <= 1 && touch.radiusY <= 1
    }
    function farAway(touch, other) {
      if (other.left == null) { return true }
      var dx = other.left - touch.left, dy = other.top - touch.top;
      return dx * dx + dy * dy > 20 * 20
    }
    on(d.scroller, "touchstart", function (e) {
      if (!signalDOMEvent(cm, e) && !isMouseLikeTouchEvent(e) && !clickInGutter(cm, e)) {
        d.input.ensurePolled();
        clearTimeout(touchFinished);
        var now = +new Date;
        d.activeTouch = {start: now, moved: false,
                         prev: now - prevTouch.end <= 300 ? prevTouch : null};
        if (e.touches.length == 1) {
          d.activeTouch.left = e.touches[0].pageX;
          d.activeTouch.top = e.touches[0].pageY;
        }
      }
    });
    on(d.scroller, "touchmove", function () {
      if (d.activeTouch) { d.activeTouch.moved = true; }
    });
    on(d.scroller, "touchend", function (e) {
      var touch = d.activeTouch;
      if (touch && !eventInWidget(d, e) && touch.left != null &&
          !touch.moved && new Date - touch.start < 300) {
        var pos = cm.coordsChar(d.activeTouch, "page"), range;
        if (!touch.prev || farAway(touch, touch.prev)) // Single tap
          { range = new Range(pos, pos); }
        else if (!touch.prev.prev || farAway(touch, touch.prev.prev)) // Double tap
          { range = cm.findWordAt(pos); }
        else // Triple tap
          { range = new Range(Pos(pos.line, 0), clipPos(cm.doc, Pos(pos.line + 1, 0))); }
        cm.setSelection(range.anchor, range.head);
        cm.focus();
        e_preventDefault(e);
      }
      finishTouch();
    });
    on(d.scroller, "touchcancel", finishTouch);

    // Sync scrolling between fake scrollbars and real scrollable
    // area, ensure viewport is updated when scrolling.
    on(d.scroller, "scroll", function () {
      if (d.scroller.clientHeight) {
        updateScrollTop(cm, d.scroller.scrollTop);
        setScrollLeft(cm, d.scroller.scrollLeft, true);
        signal(cm, "scroll", cm);
      }
    });

    // Listen to wheel events in order to try and update the viewport on time.
    on(d.scroller, "mousewheel", function (e) { return onScrollWheel(cm, e); });
    on(d.scroller, "DOMMouseScroll", function (e) { return onScrollWheel(cm, e); });

    // Prevent wrapper from ever scrolling
    on(d.wrapper, "scroll", function () { return d.wrapper.scrollTop = d.wrapper.scrollLeft = 0; });

    d.dragFunctions = {
      enter: function (e) {if (!signalDOMEvent(cm, e)) { e_stop(e); }},
      over: function (e) {if (!signalDOMEvent(cm, e)) { onDragOver(cm, e); e_stop(e); }},
      start: function (e) { return onDragStart(cm, e); },
      drop: operation(cm, onDrop),
      leave: function (e) {if (!signalDOMEvent(cm, e)) { clearDragCursor(cm); }}
    };

    var inp = d.input.getField();
    on(inp, "keyup", function (e) { return onKeyUp.call(cm, e); });
    on(inp, "keydown", operation(cm, onKeyDown));
    on(inp, "keypress", operation(cm, onKeyPress));
    on(inp, "focus", function (e) { return onFocus(cm, e); });
    on(inp, "blur", function (e) { return onBlur(cm, e); });
  }

  var initHooks = [];
  CodeMirror.defineInitHook = function (f) { return initHooks.push(f); };

  // Indent the given line. The how parameter can be "smart",
  // "add"/null, "subtract", or "prev". When aggressive is false
  // (typically set to true for forced single-line indents), empty
  // lines are not indented, and places where the mode returns Pass
  // are left alone.
  function indentLine(cm, n, how, aggressive) {
    var doc = cm.doc, state;
    if (how == null) { how = "add"; }
    if (how == "smart") {
      // Fall back to "prev" when the mode doesn't have an indentation
      // method.
      if (!doc.mode.indent) { how = "prev"; }
      else { state = getContextBefore(cm, n).state; }
    }

    var tabSize = cm.options.tabSize;
    var line = getLine(doc, n), curSpace = countColumn(line.text, null, tabSize);
    if (line.stateAfter) { line.stateAfter = null; }
    var curSpaceString = line.text.match(/^\s*/)[0], indentation;
    if (!aggressive && !/\S/.test(line.text)) {
      indentation = 0;
      how = "not";
    } else if (how == "smart") {
      indentation = doc.mode.indent(state, line.text.slice(curSpaceString.length), line.text);
      if (indentation == Pass || indentation > 150) {
        if (!aggressive) { return }
        how = "prev";
      }
    }
    if (how == "prev") {
      if (n > doc.first) { indentation = countColumn(getLine(doc, n-1).text, null, tabSize); }
      else { indentation = 0; }
    } else if (how == "add") {
      indentation = curSpace + cm.options.indentUnit;
    } else if (how == "subtract") {
      indentation = curSpace - cm.options.indentUnit;
    } else if (typeof how == "number") {
      indentation = curSpace + how;
    }
    indentation = Math.max(0, indentation);

    var indentString = "", pos = 0;
    if (cm.options.indentWithTabs)
      { for (var i = Math.floor(indentation / tabSize); i; --i) {pos += tabSize; indentString += "\t";} }
    if (pos < indentation) { indentString += spaceStr(indentation - pos); }

    if (indentString != curSpaceString) {
      replaceRange(doc, indentString, Pos(n, 0), Pos(n, curSpaceString.length), "+input");
      line.stateAfter = null;
      return true
    } else {
      // Ensure that, if the cursor was in the whitespace at the start
      // of the line, it is moved to the end of that space.
      for (var i$1 = 0; i$1 < doc.sel.ranges.length; i$1++) {
        var range = doc.sel.ranges[i$1];
        if (range.head.line == n && range.head.ch < curSpaceString.length) {
          var pos$1 = Pos(n, curSpaceString.length);
          replaceOneSelection(doc, i$1, new Range(pos$1, pos$1));
          break
        }
      }
    }
  }

  // This will be set to a {lineWise: bool, text: [string]} object, so
  // that, when pasting, we know what kind of selections the copied
  // text was made out of.
  var lastCopied = null;

  function setLastCopied(newLastCopied) {
    lastCopied = newLastCopied;
  }

  function applyTextInput(cm, inserted, deleted, sel, origin) {
    var doc = cm.doc;
    cm.display.shift = false;
    if (!sel) { sel = doc.sel; }

    var recent = +new Date - 200;
    var paste = origin == "paste" || cm.state.pasteIncoming > recent;
    var textLines = splitLinesAuto(inserted), multiPaste = null;
    // When pasting N lines into N selections, insert one line per selection
    if (paste && sel.ranges.length > 1) {
      if (lastCopied && lastCopied.text.join("\n") == inserted) {
        if (sel.ranges.length % lastCopied.text.length == 0) {
          multiPaste = [];
          for (var i = 0; i < lastCopied.text.length; i++)
            { multiPaste.push(doc.splitLines(lastCopied.text[i])); }
        }
      } else if (textLines.length == sel.ranges.length && cm.options.pasteLinesPerSelection) {
        multiPaste = map(textLines, function (l) { return [l]; });
      }
    }

    var updateInput = cm.curOp.updateInput;
    // Normal behavior is to insert the new text into every selection
    for (var i$1 = sel.ranges.length - 1; i$1 >= 0; i$1--) {
      var range = sel.ranges[i$1];
      var from = range.from(), to = range.to();
      if (range.empty()) {
        if (deleted && deleted > 0) // Handle deletion
          { from = Pos(from.line, from.ch - deleted); }
        else if (cm.state.overwrite && !paste) // Handle overwrite
          { to = Pos(to.line, Math.min(getLine(doc, to.line).text.length, to.ch + lst(textLines).length)); }
        else if (paste && lastCopied && lastCopied.lineWise && lastCopied.text.join("\n") == textLines.join("\n"))
          { from = to = Pos(from.line, 0); }
      }
      var changeEvent = {from: from, to: to, text: multiPaste ? multiPaste[i$1 % multiPaste.length] : textLines,
                         origin: origin || (paste ? "paste" : cm.state.cutIncoming > recent ? "cut" : "+input")};
      makeChange(cm.doc, changeEvent);
      signalLater(cm, "inputRead", cm, changeEvent);
    }
    if (inserted && !paste)
      { triggerElectric(cm, inserted); }

    ensureCursorVisible(cm);
    if (cm.curOp.updateInput < 2) { cm.curOp.updateInput = updateInput; }
    cm.curOp.typing = true;
    cm.state.pasteIncoming = cm.state.cutIncoming = -1;
  }

  function handlePaste(e, cm) {
    var pasted = e.clipboardData && e.clipboardData.getData("Text");
    if (pasted) {
      e.preventDefault();
      if (!cm.isReadOnly() && !cm.options.disableInput)
        { runInOp(cm, function () { return applyTextInput(cm, pasted, 0, null, "paste"); }); }
      return true
    }
  }

  function triggerElectric(cm, inserted) {
    // When an 'electric' character is inserted, immediately trigger a reindent
    if (!cm.options.electricChars || !cm.options.smartIndent) { return }
    var sel = cm.doc.sel;

    for (var i = sel.ranges.length - 1; i >= 0; i--) {
      var range = sel.ranges[i];
      if (range.head.ch > 100 || (i && sel.ranges[i - 1].head.line == range.head.line)) { continue }
      var mode = cm.getModeAt(range.head);
      var indented = false;
      if (mode.electricChars) {
        for (var j = 0; j < mode.electricChars.length; j++)
          { if (inserted.indexOf(mode.electricChars.charAt(j)) > -1) {
            indented = indentLine(cm, range.head.line, "smart");
            break
          } }
      } else if (mode.electricInput) {
        if (mode.electricInput.test(getLine(cm.doc, range.head.line).text.slice(0, range.head.ch)))
          { indented = indentLine(cm, range.head.line, "smart"); }
      }
      if (indented) { signalLater(cm, "electricInput", cm, range.head.line); }
    }
  }

  function copyableRanges(cm) {
    var text = [], ranges = [];
    for (var i = 0; i < cm.doc.sel.ranges.length; i++) {
      var line = cm.doc.sel.ranges[i].head.line;
      var lineRange = {anchor: Pos(line, 0), head: Pos(line + 1, 0)};
      ranges.push(lineRange);
      text.push(cm.getRange(lineRange.anchor, lineRange.head));
    }
    return {text: text, ranges: ranges}
  }

  function disableBrowserMagic(field, spellcheck, autocorrect, autocapitalize) {
    field.setAttribute("autocorrect", autocorrect ? "" : "off");
    field.setAttribute("autocapitalize", autocapitalize ? "" : "off");
    field.setAttribute("spellcheck", !!spellcheck);
  }

  function hiddenTextarea() {
    var te = elt("textarea", null, null, "position: absolute; bottom: -1em; padding: 0; width: 1px; height: 1em; outline: none");
    var div = elt("div", [te], null, "overflow: hidden; position: relative; width: 3px; height: 0px;");
    // The textarea is kept positioned near the cursor to prevent the
    // fact that it'll be scrolled into view on input from scrolling
    // our fake cursor out of view. On webkit, when wrap=off, paste is
    // very slow. So make the area wide instead.
    if (webkit) { te.style.width = "1000px"; }
    else { te.setAttribute("wrap", "off"); }
    // If border: 0; -- iOS fails to open keyboard (issue #1287)
    if (ios) { te.style.border = "1px solid black"; }
    disableBrowserMagic(te);
    return div
  }

  // The publicly visible API. Note that methodOp(f) means
  // 'wrap f in an operation, performed on its `this` parameter'.

  // This is not the complete set of editor methods. Most of the
  // methods defined on the Doc type are also injected into
  // CodeMirror.prototype, for backwards compatibility and
  // convenience.

  function addEditorMethods(CodeMirror) {
    var optionHandlers = CodeMirror.optionHandlers;

    var helpers = CodeMirror.helpers = {};

    CodeMirror.prototype = {
      constructor: CodeMirror,
      focus: function(){window.focus(); this.display.input.focus();},

      setOption: function(option, value) {
        var options = this.options, old = options[option];
        if (options[option] == value && option != "mode") { return }
        options[option] = value;
        if (optionHandlers.hasOwnProperty(option))
          { operation(this, optionHandlers[option])(this, value, old); }
        signal(this, "optionChange", this, option);
      },

      getOption: function(option) {return this.options[option]},
      getDoc: function() {return this.doc},

      addKeyMap: function(map, bottom) {
        this.state.keyMaps[bottom ? "push" : "unshift"](getKeyMap(map));
      },
      removeKeyMap: function(map) {
        var maps = this.state.keyMaps;
        for (var i = 0; i < maps.length; ++i)
          { if (maps[i] == map || maps[i].name == map) {
            maps.splice(i, 1);
            return true
          } }
      },

      addOverlay: methodOp(function(spec, options) {
        var mode = spec.token ? spec : CodeMirror.getMode(this.options, spec);
        if (mode.startState) { throw new Error("Overlays may not be stateful.") }
        insertSorted(this.state.overlays,
                     {mode: mode, modeSpec: spec, opaque: options && options.opaque,
                      priority: (options && options.priority) || 0},
                     function (overlay) { return overlay.priority; });
        this.state.modeGen++;
        regChange(this);
      }),
      removeOverlay: methodOp(function(spec) {
        var overlays = this.state.overlays;
        for (var i = 0; i < overlays.length; ++i) {
          var cur = overlays[i].modeSpec;
          if (cur == spec || typeof spec == "string" && cur.name == spec) {
            overlays.splice(i, 1);
            this.state.modeGen++;
            regChange(this);
            return
          }
        }
      }),

      indentLine: methodOp(function(n, dir, aggressive) {
        if (typeof dir != "string" && typeof dir != "number") {
          if (dir == null) { dir = this.options.smartIndent ? "smart" : "prev"; }
          else { dir = dir ? "add" : "subtract"; }
        }
        if (isLine(this.doc, n)) { indentLine(this, n, dir, aggressive); }
      }),
      indentSelection: methodOp(function(how) {
        var ranges = this.doc.sel.ranges, end = -1;
        for (var i = 0; i < ranges.length; i++) {
          var range = ranges[i];
          if (!range.empty()) {
            var from = range.from(), to = range.to();
            var start = Math.max(end, from.line);
            end = Math.min(this.lastLine(), to.line - (to.ch ? 0 : 1)) + 1;
            for (var j = start; j < end; ++j)
              { indentLine(this, j, how); }
            var newRanges = this.doc.sel.ranges;
            if (from.ch == 0 && ranges.length == newRanges.length && newRanges[i].from().ch > 0)
              { replaceOneSelection(this.doc, i, new Range(from, newRanges[i].to()), sel_dontScroll); }
          } else if (range.head.line > end) {
            indentLine(this, range.head.line, how, true);
            end = range.head.line;
            if (i == this.doc.sel.primIndex) { ensureCursorVisible(this); }
          }
        }
      }),

      // Fetch the parser token for a given character. Useful for hacks
      // that want to inspect the mode state (say, for completion).
      getTokenAt: function(pos, precise) {
        return takeToken(this, pos, precise)
      },

      getLineTokens: function(line, precise) {
        return takeToken(this, Pos(line), precise, true)
      },

      getTokenTypeAt: function(pos) {
        pos = clipPos(this.doc, pos);
        var styles = getLineStyles(this, getLine(this.doc, pos.line));
        var before = 0, after = (styles.length - 1) / 2, ch = pos.ch;
        var type;
        if (ch == 0) { type = styles[2]; }
        else { for (;;) {
          var mid = (before + after) >> 1;
          if ((mid ? styles[mid * 2 - 1] : 0) >= ch) { after = mid; }
          else if (styles[mid * 2 + 1] < ch) { before = mid + 1; }
          else { type = styles[mid * 2 + 2]; break }
        } }
        var cut = type ? type.indexOf("overlay ") : -1;
        return cut < 0 ? type : cut == 0 ? null : type.slice(0, cut - 1)
      },

      getModeAt: function(pos) {
        var mode = this.doc.mode;
        if (!mode.innerMode) { return mode }
        return CodeMirror.innerMode(mode, this.getTokenAt(pos).state).mode
      },

      getHelper: function(pos, type) {
        return this.getHelpers(pos, type)[0]
      },

      getHelpers: function(pos, type) {
        var found = [];
        if (!helpers.hasOwnProperty(type)) { return found }
        var help = helpers[type], mode = this.getModeAt(pos);
        if (typeof mode[type] == "string") {
          if (help[mode[type]]) { found.push(help[mode[type]]); }
        } else if (mode[type]) {
          for (var i = 0; i < mode[type].length; i++) {
            var val = help[mode[type][i]];
            if (val) { found.push(val); }
          }
        } else if (mode.helperType && help[mode.helperType]) {
          found.push(help[mode.helperType]);
        } else if (help[mode.name]) {
          found.push(help[mode.name]);
        }
        for (var i$1 = 0; i$1 < help._global.length; i$1++) {
          var cur = help._global[i$1];
          if (cur.pred(mode, this) && indexOf(found, cur.val) == -1)
            { found.push(cur.val); }
        }
        return found
      },

      getStateAfter: function(line, precise) {
        var doc = this.doc;
        line = clipLine(doc, line == null ? doc.first + doc.size - 1: line);
        return getContextBefore(this, line + 1, precise).state
      },

      cursorCoords: function(start, mode) {
        var pos, range = this.doc.sel.primary();
        if (start == null) { pos = range.head; }
        else if (typeof start == "object") { pos = clipPos(this.doc, start); }
        else { pos = start ? range.from() : range.to(); }
        return cursorCoords(this, pos, mode || "page")
      },

      charCoords: function(pos, mode) {
        return charCoords(this, clipPos(this.doc, pos), mode || "page")
      },

      coordsChar: function(coords, mode) {
        coords = fromCoordSystem(this, coords, mode || "page");
        return coordsChar(this, coords.left, coords.top)
      },

      lineAtHeight: function(height, mode) {
        height = fromCoordSystem(this, {top: height, left: 0}, mode || "page").top;
        return lineAtHeight(this.doc, height + this.display.viewOffset)
      },
      heightAtLine: function(line, mode, includeWidgets) {
        var end = false, lineObj;
        if (typeof line == "number") {
          var last = this.doc.first + this.doc.size - 1;
          if (line < this.doc.first) { line = this.doc.first; }
          else if (line > last) { line = last; end = true; }
          lineObj = getLine(this.doc, line);
        } else {
          lineObj = line;
        }
        return intoCoordSystem(this, lineObj, {top: 0, left: 0}, mode || "page", includeWidgets || end).top +
          (end ? this.doc.height - heightAtLine(lineObj) : 0)
      },

      defaultTextHeight: function() { return textHeight(this.display) },
      defaultCharWidth: function() { return charWidth(this.display) },

      getViewport: function() { return {from: this.display.viewFrom, to: this.display.viewTo}},

      addWidget: function(pos, node, scroll, vert, horiz) {
        var display = this.display;
        pos = cursorCoords(this, clipPos(this.doc, pos));
        var top = pos.bottom, left = pos.left;
        node.style.position = "absolute";
        node.setAttribute("cm-ignore-events", "true");
        this.display.input.setUneditable(node);
        display.sizer.appendChild(node);
        if (vert == "over") {
          top = pos.top;
        } else if (vert == "above" || vert == "near") {
          var vspace = Math.max(display.wrapper.clientHeight, this.doc.height),
          hspace = Math.max(display.sizer.clientWidth, display.lineSpace.clientWidth);
          // Default to positioning above (if specified and possible); otherwise default to positioning below
          if ((vert == 'above' || pos.bottom + node.offsetHeight > vspace) && pos.top > node.offsetHeight)
            { top = pos.top - node.offsetHeight; }
          else if (pos.bottom + node.offsetHeight <= vspace)
            { top = pos.bottom; }
          if (left + node.offsetWidth > hspace)
            { left = hspace - node.offsetWidth; }
        }
        node.style.top = top + "px";
        node.style.left = node.style.right = "";
        if (horiz == "right") {
          left = display.sizer.clientWidth - node.offsetWidth;
          node.style.right = "0px";
        } else {
          if (horiz == "left") { left = 0; }
          else if (horiz == "middle") { left = (display.sizer.clientWidth - node.offsetWidth) / 2; }
          node.style.left = left + "px";
        }
        if (scroll)
          { scrollIntoView(this, {left: left, top: top, right: left + node.offsetWidth, bottom: top + node.offsetHeight}); }
      },

      triggerOnKeyDown: methodOp(onKeyDown),
      triggerOnKeyPress: methodOp(onKeyPress),
      triggerOnKeyUp: onKeyUp,
      triggerOnMouseDown: methodOp(onMouseDown),

      execCommand: function(cmd) {
        if (commands.hasOwnProperty(cmd))
          { return commands[cmd].call(null, this) }
      },

      triggerElectric: methodOp(function(text) { triggerElectric(this, text); }),

      findPosH: function(from, amount, unit, visually) {
        var dir = 1;
        if (amount < 0) { dir = -1; amount = -amount; }
        var cur = clipPos(this.doc, from);
        for (var i = 0; i < amount; ++i) {
          cur = findPosH(this.doc, cur, dir, unit, visually);
          if (cur.hitSide) { break }
        }
        return cur
      },

      moveH: methodOp(function(dir, unit) {
        var this$1 = this;

        this.extendSelectionsBy(function (range) {
          if (this$1.display.shift || this$1.doc.extend || range.empty())
            { return findPosH(this$1.doc, range.head, dir, unit, this$1.options.rtlMoveVisually) }
          else
            { return dir < 0 ? range.from() : range.to() }
        }, sel_move);
      }),

      deleteH: methodOp(function(dir, unit) {
        var sel = this.doc.sel, doc = this.doc;
        if (sel.somethingSelected())
          { doc.replaceSelection("", null, "+delete"); }
        else
          { deleteNearSelection(this, function (range) {
            var other = findPosH(doc, range.head, dir, unit, false);
            return dir < 0 ? {from: other, to: range.head} : {from: range.head, to: other}
          }); }
      }),

      findPosV: function(from, amount, unit, goalColumn) {
        var dir = 1, x = goalColumn;
        if (amount < 0) { dir = -1; amount = -amount; }
        var cur = clipPos(this.doc, from);
        for (var i = 0; i < amount; ++i) {
          var coords = cursorCoords(this, cur, "div");
          if (x == null) { x = coords.left; }
          else { coords.left = x; }
          cur = findPosV(this, coords, dir, unit);
          if (cur.hitSide) { break }
        }
        return cur
      },

      moveV: methodOp(function(dir, unit) {
        var this$1 = this;

        var doc = this.doc, goals = [];
        var collapse = !this.display.shift && !doc.extend && doc.sel.somethingSelected();
        doc.extendSelectionsBy(function (range) {
          if (collapse)
            { return dir < 0 ? range.from() : range.to() }
          var headPos = cursorCoords(this$1, range.head, "div");
          if (range.goalColumn != null) { headPos.left = range.goalColumn; }
          goals.push(headPos.left);
          var pos = findPosV(this$1, headPos, dir, unit);
          if (unit == "page" && range == doc.sel.primary())
            { addToScrollTop(this$1, charCoords(this$1, pos, "div").top - headPos.top); }
          return pos
        }, sel_move);
        if (goals.length) { for (var i = 0; i < doc.sel.ranges.length; i++)
          { doc.sel.ranges[i].goalColumn = goals[i]; } }
      }),

      // Find the word at the given position (as returned by coordsChar).
      findWordAt: function(pos) {
        var doc = this.doc, line = getLine(doc, pos.line).text;
        var start = pos.ch, end = pos.ch;
        if (line) {
          var helper = this.getHelper(pos, "wordChars");
          if ((pos.sticky == "before" || end == line.length) && start) { --start; } else { ++end; }
          var startChar = line.charAt(start);
          var check = isWordChar(startChar, helper)
            ? function (ch) { return isWordChar(ch, helper); }
            : /\s/.test(startChar) ? function (ch) { return /\s/.test(ch); }
            : function (ch) { return (!/\s/.test(ch) && !isWordChar(ch)); };
          while (start > 0 && check(line.charAt(start - 1))) { --start; }
          while (end < line.length && check(line.charAt(end))) { ++end; }
        }
        return new Range(Pos(pos.line, start), Pos(pos.line, end))
      },

      toggleOverwrite: function(value) {
        if (value != null && value == this.state.overwrite) { return }
        if (this.state.overwrite = !this.state.overwrite)
          { addClass(this.display.cursorDiv, "CodeMirror-overwrite"); }
        else
          { rmClass(this.display.cursorDiv, "CodeMirror-overwrite"); }

        signal(this, "overwriteToggle", this, this.state.overwrite);
      },
      hasFocus: function() { return this.display.input.getField() == activeElt() },
      isReadOnly: function() { return !!(this.options.readOnly || this.doc.cantEdit) },

      scrollTo: methodOp(function (x, y) { scrollToCoords(this, x, y); }),
      getScrollInfo: function() {
        var scroller = this.display.scroller;
        return {left: scroller.scrollLeft, top: scroller.scrollTop,
                height: scroller.scrollHeight - scrollGap(this) - this.display.barHeight,
                width: scroller.scrollWidth - scrollGap(this) - this.display.barWidth,
                clientHeight: displayHeight(this), clientWidth: displayWidth(this)}
      },

      scrollIntoView: methodOp(function(range, margin) {
        if (range == null) {
          range = {from: this.doc.sel.primary().head, to: null};
          if (margin == null) { margin = this.options.cursorScrollMargin; }
        } else if (typeof range == "number") {
          range = {from: Pos(range, 0), to: null};
        } else if (range.from == null) {
          range = {from: range, to: null};
        }
        if (!range.to) { range.to = range.from; }
        range.margin = margin || 0;

        if (range.from.line != null) {
          scrollToRange(this, range);
        } else {
          scrollToCoordsRange(this, range.from, range.to, range.margin);
        }
      }),

      setSize: methodOp(function(width, height) {
        var this$1 = this;

        var interpret = function (val) { return typeof val == "number" || /^\d+$/.test(String(val)) ? val + "px" : val; };
        if (width != null) { this.display.wrapper.style.width = interpret(width); }
        if (height != null) { this.display.wrapper.style.height = interpret(height); }
        if (this.options.lineWrapping) { clearLineMeasurementCache(this); }
        var lineNo = this.display.viewFrom;
        this.doc.iter(lineNo, this.display.viewTo, function (line) {
          if (line.widgets) { for (var i = 0; i < line.widgets.length; i++)
            { if (line.widgets[i].noHScroll) { regLineChange(this$1, lineNo, "widget"); break } } }
          ++lineNo;
        });
        this.curOp.forceUpdate = true;
        signal(this, "refresh", this);
      }),

      operation: function(f){return runInOp(this, f)},
      startOperation: function(){return startOperation(this)},
      endOperation: function(){return endOperation(this)},

      refresh: methodOp(function() {
        var oldHeight = this.display.cachedTextHeight;
        regChange(this);
        this.curOp.forceUpdate = true;
        clearCaches(this);
        scrollToCoords(this, this.doc.scrollLeft, this.doc.scrollTop);
        updateGutterSpace(this.display);
        if (oldHeight == null || Math.abs(oldHeight - textHeight(this.display)) > .5 || this.options.lineWrapping)
          { estimateLineHeights(this); }
        signal(this, "refresh", this);
      }),

      swapDoc: methodOp(function(doc) {
        var old = this.doc;
        old.cm = null;
        // Cancel the current text selection if any (#5821)
        if (this.state.selectingText) { this.state.selectingText(); }
        attachDoc(this, doc);
        clearCaches(this);
        this.display.input.reset();
        scrollToCoords(this, doc.scrollLeft, doc.scrollTop);
        this.curOp.forceScroll = true;
        signalLater(this, "swapDoc", this, old);
        return old
      }),

      phrase: function(phraseText) {
        var phrases = this.options.phrases;
        return phrases && Object.prototype.hasOwnProperty.call(phrases, phraseText) ? phrases[phraseText] : phraseText
      },

      getInputField: function(){return this.display.input.getField()},
      getWrapperElement: function(){return this.display.wrapper},
      getScrollerElement: function(){return this.display.scroller},
      getGutterElement: function(){return this.display.gutters}
    };
    eventMixin(CodeMirror);

    CodeMirror.registerHelper = function(type, name, value) {
      if (!helpers.hasOwnProperty(type)) { helpers[type] = CodeMirror[type] = {_global: []}; }
      helpers[type][name] = value;
    };
    CodeMirror.registerGlobalHelper = function(type, name, predicate, value) {
      CodeMirror.registerHelper(type, name, value);
      helpers[type]._global.push({pred: predicate, val: value});
    };
  }

  // Used for horizontal relative motion. Dir is -1 or 1 (left or
  // right), unit can be "codepoint", "char", "column" (like char, but
  // doesn't cross line boundaries), "word" (across next word), or
  // "group" (to the start of next group of word or
  // non-word-non-whitespace chars). The visually param controls
  // whether, in right-to-left text, direction 1 means to move towards
  // the next index in the string, or towards the character to the right
  // of the current position. The resulting position will have a
  // hitSide=true property if it reached the end of the document.
  function findPosH(doc, pos, dir, unit, visually) {
    var oldPos = pos;
    var origDir = dir;
    var lineObj = getLine(doc, pos.line);
    var lineDir = visually && doc.direction == "rtl" ? -dir : dir;
    function findNextLine() {
      var l = pos.line + lineDir;
      if (l < doc.first || l >= doc.first + doc.size) { return false }
      pos = new Pos(l, pos.ch, pos.sticky);
      return lineObj = getLine(doc, l)
    }
    function moveOnce(boundToLine) {
      var next;
      if (unit == "codepoint") {
        var ch = lineObj.text.charCodeAt(pos.ch + (unit > 0 ? 0 : -1));
        if (isNaN(ch)) { next = null; }
        else { next = new Pos(pos.line, Math.max(0, Math.min(lineObj.text.length, pos.ch + dir * (ch >= 0xD800 && ch < 0xDC00 ? 2 : 1))),
                            -dir); }
      } else if (visually) {
        next = moveVisually(doc.cm, lineObj, pos, dir);
      } else {
        next = moveLogically(lineObj, pos, dir);
      }
      if (next == null) {
        if (!boundToLine && findNextLine())
          { pos = endOfLine(visually, doc.cm, lineObj, pos.line, lineDir); }
        else
          { return false }
      } else {
        pos = next;
      }
      return true
    }

    if (unit == "char" || unit == "codepoint") {
      moveOnce();
    } else if (unit == "column") {
      moveOnce(true);
    } else if (unit == "word" || unit == "group") {
      var sawType = null, group = unit == "group";
      var helper = doc.cm && doc.cm.getHelper(pos, "wordChars");
      for (var first = true;; first = false) {
        if (dir < 0 && !moveOnce(!first)) { break }
        var cur = lineObj.text.charAt(pos.ch) || "\n";
        var type = isWordChar(cur, helper) ? "w"
          : group && cur == "\n" ? "n"
          : !group || /\s/.test(cur) ? null
          : "p";
        if (group && !first && !type) { type = "s"; }
        if (sawType && sawType != type) {
          if (dir < 0) {dir = 1; moveOnce(); pos.sticky = "after";}
          break
        }

        if (type) { sawType = type; }
        if (dir > 0 && !moveOnce(!first)) { break }
      }
    }
    var result = skipAtomic(doc, pos, oldPos, origDir, true);
    if (equalCursorPos(oldPos, result)) { result.hitSide = true; }
    return result
  }

  // For relative vertical movement. Dir may be -1 or 1. Unit can be
  // "page" or "line". The resulting position will have a hitSide=true
  // property if it reached the end of the document.
  function findPosV(cm, pos, dir, unit) {
    var doc = cm.doc, x = pos.left, y;
    if (unit == "page") {
      var pageSize = Math.min(cm.display.wrapper.clientHeight, window.innerHeight || document.documentElement.clientHeight);
      var moveAmount = Math.max(pageSize - .5 * textHeight(cm.display), 3);
      y = (dir > 0 ? pos.bottom : pos.top) + dir * moveAmount;

    } else if (unit == "line") {
      y = dir > 0 ? pos.bottom + 3 : pos.top - 3;
    }
    var target;
    for (;;) {
      target = coordsChar(cm, x, y);
      if (!target.outside) { break }
      if (dir < 0 ? y <= 0 : y >= doc.height) { target.hitSide = true; break }
      y += dir * 5;
    }
    return target
  }

  // CONTENTEDITABLE INPUT STYLE

  var ContentEditableInput = function(cm) {
    this.cm = cm;
    this.lastAnchorNode = this.lastAnchorOffset = this.lastFocusNode = this.lastFocusOffset = null;
    this.polling = new Delayed();
    this.composing = null;
    this.gracePeriod = false;
    this.readDOMTimeout = null;
  };

  ContentEditableInput.prototype.init = function (display) {
      var this$1 = this;

    var input = this, cm = input.cm;
    var div = input.div = display.lineDiv;
    disableBrowserMagic(div, cm.options.spellcheck, cm.options.autocorrect, cm.options.autocapitalize);

    function belongsToInput(e) {
      for (var t = e.target; t; t = t.parentNode) {
        if (t == div) { return true }
        if (/\bCodeMirror-(?:line)?widget\b/.test(t.className)) { break }
      }
      return false
    }

    on(div, "paste", function (e) {
      if (!belongsToInput(e) || signalDOMEvent(cm, e) || handlePaste(e, cm)) { return }
      // IE doesn't fire input events, so we schedule a read for the pasted content in this way
      if (ie_version <= 11) { setTimeout(operation(cm, function () { return this$1.updateFromDOM(); }), 20); }
    });

    on(div, "compositionstart", function (e) {
      this$1.composing = {data: e.data, done: false};
    });
    on(div, "compositionupdate", function (e) {
      if (!this$1.composing) { this$1.composing = {data: e.data, done: false}; }
    });
    on(div, "compositionend", function (e) {
      if (this$1.composing) {
        if (e.data != this$1.composing.data) { this$1.readFromDOMSoon(); }
        this$1.composing.done = true;
      }
    });

    on(div, "touchstart", function () { return input.forceCompositionEnd(); });

    on(div, "input", function () {
      if (!this$1.composing) { this$1.readFromDOMSoon(); }
    });

    function onCopyCut(e) {
      if (!belongsToInput(e) || signalDOMEvent(cm, e)) { return }
      if (cm.somethingSelected()) {
        setLastCopied({lineWise: false, text: cm.getSelections()});
        if (e.type == "cut") { cm.replaceSelection("", null, "cut"); }
      } else if (!cm.options.lineWiseCopyCut) {
        return
      } else {
        var ranges = copyableRanges(cm);
        setLastCopied({lineWise: true, text: ranges.text});
        if (e.type == "cut") {
          cm.operation(function () {
            cm.setSelections(ranges.ranges, 0, sel_dontScroll);
            cm.replaceSelection("", null, "cut");
          });
        }
      }
      if (e.clipboardData) {
        e.clipboardData.clearData();
        var content = lastCopied.text.join("\n");
        // iOS exposes the clipboard API, but seems to discard content inserted into it
        e.clipboardData.setData("Text", content);
        if (e.clipboardData.getData("Text") == content) {
          e.preventDefault();
          return
        }
      }
      // Old-fashioned briefly-focus-a-textarea hack
      var kludge = hiddenTextarea(), te = kludge.firstChild;
      cm.display.lineSpace.insertBefore(kludge, cm.display.lineSpace.firstChild);
      te.value = lastCopied.text.join("\n");
      var hadFocus = document.activeElement;
      selectInput(te);
      setTimeout(function () {
        cm.display.lineSpace.removeChild(kludge);
        hadFocus.focus();
        if (hadFocus == div) { input.showPrimarySelection(); }
      }, 50);
    }
    on(div, "copy", onCopyCut);
    on(div, "cut", onCopyCut);
  };

  ContentEditableInput.prototype.screenReaderLabelChanged = function (label) {
    // Label for screenreaders, accessibility
    if(label) {
      this.div.setAttribute('aria-label', label);
    } else {
      this.div.removeAttribute('aria-label');
    }
  };

  ContentEditableInput.prototype.prepareSelection = function () {
    var result = prepareSelection(this.cm, false);
    result.focus = document.activeElement == this.div;
    return result
  };

  ContentEditableInput.prototype.showSelection = function (info, takeFocus) {
    if (!info || !this.cm.display.view.length) { return }
    if (info.focus || takeFocus) { this.showPrimarySelection(); }
    this.showMultipleSelections(info);
  };

  ContentEditableInput.prototype.getSelection = function () {
    return this.cm.display.wrapper.ownerDocument.getSelection()
  };

  ContentEditableInput.prototype.showPrimarySelection = function () {
    var sel = this.getSelection(), cm = this.cm, prim = cm.doc.sel.primary();
    var from = prim.from(), to = prim.to();

    if (cm.display.viewTo == cm.display.viewFrom || from.line >= cm.display.viewTo || to.line < cm.display.viewFrom) {
      sel.removeAllRanges();
      return
    }

    var curAnchor = domToPos(cm, sel.anchorNode, sel.anchorOffset);
    var curFocus = domToPos(cm, sel.focusNode, sel.focusOffset);
    if (curAnchor && !curAnchor.bad && curFocus && !curFocus.bad &&
        cmp(minPos(curAnchor, curFocus), from) == 0 &&
        cmp(maxPos(curAnchor, curFocus), to) == 0)
      { return }

    var view = cm.display.view;
    var start = (from.line >= cm.display.viewFrom && posToDOM(cm, from)) ||
        {node: view[0].measure.map[2], offset: 0};
    var end = to.line < cm.display.viewTo && posToDOM(cm, to);
    if (!end) {
      var measure = view[view.length - 1].measure;
      var map = measure.maps ? measure.maps[measure.maps.length - 1] : measure.map;
      end = {node: map[map.length - 1], offset: map[map.length - 2] - map[map.length - 3]};
    }

    if (!start || !end) {
      sel.removeAllRanges();
      return
    }

    var old = sel.rangeCount && sel.getRangeAt(0), rng;
    try { rng = range(start.node, start.offset, end.offset, end.node); }
    catch(e) {} // Our model of the DOM might be outdated, in which case the range we try to set can be impossible
    if (rng) {
      if (!gecko && cm.state.focused) {
        sel.collapse(start.node, start.offset);
        if (!rng.collapsed) {
          sel.removeAllRanges();
          sel.addRange(rng);
        }
      } else {
        sel.removeAllRanges();
        sel.addRange(rng);
      }
      if (old && sel.anchorNode == null) { sel.addRange(old); }
      else if (gecko) { this.startGracePeriod(); }
    }
    this.rememberSelection();
  };

  ContentEditableInput.prototype.startGracePeriod = function () {
      var this$1 = this;

    clearTimeout(this.gracePeriod);
    this.gracePeriod = setTimeout(function () {
      this$1.gracePeriod = false;
      if (this$1.selectionChanged())
        { this$1.cm.operation(function () { return this$1.cm.curOp.selectionChanged = true; }); }
    }, 20);
  };

  ContentEditableInput.prototype.showMultipleSelections = function (info) {
    removeChildrenAndAdd(this.cm.display.cursorDiv, info.cursors);
    removeChildrenAndAdd(this.cm.display.selectionDiv, info.selection);
  };

  ContentEditableInput.prototype.rememberSelection = function () {
    var sel = this.getSelection();
    this.lastAnchorNode = sel.anchorNode; this.lastAnchorOffset = sel.anchorOffset;
    this.lastFocusNode = sel.focusNode; this.lastFocusOffset = sel.focusOffset;
  };

  ContentEditableInput.prototype.selectionInEditor = function () {
    var sel = this.getSelection();
    if (!sel.rangeCount) { return false }
    var node = sel.getRangeAt(0).commonAncestorContainer;
    return contains(this.div, node)
  };

  ContentEditableInput.prototype.focus = function () {
    if (this.cm.options.readOnly != "nocursor") {
      if (!this.selectionInEditor() || document.activeElement != this.div)
        { this.showSelection(this.prepareSelection(), true); }
      this.div.focus();
    }
  };
  ContentEditableInput.prototype.blur = function () { this.div.blur(); };
  ContentEditableInput.prototype.getField = function () { return this.div };

  ContentEditableInput.prototype.supportsTouch = function () { return true };

  ContentEditableInput.prototype.receivedFocus = function () {
    var input = this;
    if (this.selectionInEditor())
      { this.pollSelection(); }
    else
      { runInOp(this.cm, function () { return input.cm.curOp.selectionChanged = true; }); }

    function poll() {
      if (input.cm.state.focused) {
        input.pollSelection();
        input.polling.set(input.cm.options.pollInterval, poll);
      }
    }
    this.polling.set(this.cm.options.pollInterval, poll);
  };

  ContentEditableInput.prototype.selectionChanged = function () {
    var sel = this.getSelection();
    return sel.anchorNode != this.lastAnchorNode || sel.anchorOffset != this.lastAnchorOffset ||
      sel.focusNode != this.lastFocusNode || sel.focusOffset != this.lastFocusOffset
  };

  ContentEditableInput.prototype.pollSelection = function () {
    if (this.readDOMTimeout != null || this.gracePeriod || !this.selectionChanged()) { return }
    var sel = this.getSelection(), cm = this.cm;
    // On Android Chrome (version 56, at least), backspacing into an
    // uneditable block element will put the cursor in that element,
    // and then, because it's not editable, hide the virtual keyboard.
    // Because Android doesn't allow us to actually detect backspace
    // presses in a sane way, this code checks for when that happens
    // and simulates a backspace press in this case.
    if (android && chrome && this.cm.display.gutterSpecs.length && isInGutter(sel.anchorNode)) {
      this.cm.triggerOnKeyDown({type: "keydown", keyCode: 8, preventDefault: Math.abs});
      this.blur();
      this.focus();
      return
    }
    if (this.composing) { return }
    this.rememberSelection();
    var anchor = domToPos(cm, sel.anchorNode, sel.anchorOffset);
    var head = domToPos(cm, sel.focusNode, sel.focusOffset);
    if (anchor && head) { runInOp(cm, function () {
      setSelection(cm.doc, simpleSelection(anchor, head), sel_dontScroll);
      if (anchor.bad || head.bad) { cm.curOp.selectionChanged = true; }
    }); }
  };

  ContentEditableInput.prototype.pollContent = function () {
    if (this.readDOMTimeout != null) {
      clearTimeout(this.readDOMTimeout);
      this.readDOMTimeout = null;
    }

    var cm = this.cm, display = cm.display, sel = cm.doc.sel.primary();
    var from = sel.from(), to = sel.to();
    if (from.ch == 0 && from.line > cm.firstLine())
      { from = Pos(from.line - 1, getLine(cm.doc, from.line - 1).length); }
    if (to.ch == getLine(cm.doc, to.line).text.length && to.line < cm.lastLine())
      { to = Pos(to.line + 1, 0); }
    if (from.line < display.viewFrom || to.line > display.viewTo - 1) { return false }

    var fromIndex, fromLine, fromNode;
    if (from.line == display.viewFrom || (fromIndex = findViewIndex(cm, from.line)) == 0) {
      fromLine = lineNo(display.view[0].line);
      fromNode = display.view[0].node;
    } else {
      fromLine = lineNo(display.view[fromIndex].line);
      fromNode = display.view[fromIndex - 1].node.nextSibling;
    }
    var toIndex = findViewIndex(cm, to.line);
    var toLine, toNode;
    if (toIndex == display.view.length - 1) {
      toLine = display.viewTo - 1;
      toNode = display.lineDiv.lastChild;
    } else {
      toLine = lineNo(display.view[toIndex + 1].line) - 1;
      toNode = display.view[toIndex + 1].node.previousSibling;
    }

    if (!fromNode) { return false }
    var newText = cm.doc.splitLines(domTextBetween(cm, fromNode, toNode, fromLine, toLine));
    var oldText = getBetween(cm.doc, Pos(fromLine, 0), Pos(toLine, getLine(cm.doc, toLine).text.length));
    while (newText.length > 1 && oldText.length > 1) {
      if (lst(newText) == lst(oldText)) { newText.pop(); oldText.pop(); toLine--; }
      else if (newText[0] == oldText[0]) { newText.shift(); oldText.shift(); fromLine++; }
      else { break }
    }

    var cutFront = 0, cutEnd = 0;
    var newTop = newText[0], oldTop = oldText[0], maxCutFront = Math.min(newTop.length, oldTop.length);
    while (cutFront < maxCutFront && newTop.charCodeAt(cutFront) == oldTop.charCodeAt(cutFront))
      { ++cutFront; }
    var newBot = lst(newText), oldBot = lst(oldText);
    var maxCutEnd = Math.min(newBot.length - (newText.length == 1 ? cutFront : 0),
                             oldBot.length - (oldText.length == 1 ? cutFront : 0));
    while (cutEnd < maxCutEnd &&
           newBot.charCodeAt(newBot.length - cutEnd - 1) == oldBot.charCodeAt(oldBot.length - cutEnd - 1))
      { ++cutEnd; }
    // Try to move start of change to start of selection if ambiguous
    if (newText.length == 1 && oldText.length == 1 && fromLine == from.line) {
      while (cutFront && cutFront > from.ch &&
             newBot.charCodeAt(newBot.length - cutEnd - 1) == oldBot.charCodeAt(oldBot.length - cutEnd - 1)) {
        cutFront--;
        cutEnd++;
      }
    }

    newText[newText.length - 1] = newBot.slice(0, newBot.length - cutEnd).replace(/^\u200b+/, "");
    newText[0] = newText[0].slice(cutFront).replace(/\u200b+$/, "");

    var chFrom = Pos(fromLine, cutFront);
    var chTo = Pos(toLine, oldText.length ? lst(oldText).length - cutEnd : 0);
    if (newText.length > 1 || newText[0] || cmp(chFrom, chTo)) {
      replaceRange(cm.doc, newText, chFrom, chTo, "+input");
      return true
    }
  };

  ContentEditableInput.prototype.ensurePolled = function () {
    this.forceCompositionEnd();
  };
  ContentEditableInput.prototype.reset = function () {
    this.forceCompositionEnd();
  };
  ContentEditableInput.prototype.forceCompositionEnd = function () {
    if (!this.composing) { return }
    clearTimeout(this.readDOMTimeout);
    this.composing = null;
    this.updateFromDOM();
    this.div.blur();
    this.div.focus();
  };
  ContentEditableInput.prototype.readFromDOMSoon = function () {
      var this$1 = this;

    if (this.readDOMTimeout != null) { return }
    this.readDOMTimeout = setTimeout(function () {
      this$1.readDOMTimeout = null;
      if (this$1.composing) {
        if (this$1.composing.done) { this$1.composing = null; }
        else { return }
      }
      this$1.updateFromDOM();
    }, 80);
  };

  ContentEditableInput.prototype.updateFromDOM = function () {
      var this$1 = this;

    if (this.cm.isReadOnly() || !this.pollContent())
      { runInOp(this.cm, function () { return regChange(this$1.cm); }); }
  };

  ContentEditableInput.prototype.setUneditable = function (node) {
    node.contentEditable = "false";
  };

  ContentEditableInput.prototype.onKeyPress = function (e) {
    if (e.charCode == 0 || this.composing) { return }
    e.preventDefault();
    if (!this.cm.isReadOnly())
      { operation(this.cm, applyTextInput)(this.cm, String.fromCharCode(e.charCode == null ? e.keyCode : e.charCode), 0); }
  };

  ContentEditableInput.prototype.readOnlyChanged = function (val) {
    this.div.contentEditable = String(val != "nocursor");
  };

  ContentEditableInput.prototype.onContextMenu = function () {};
  ContentEditableInput.prototype.resetPosition = function () {};

  ContentEditableInput.prototype.needsContentAttribute = true;

  function posToDOM(cm, pos) {
    var view = findViewForLine(cm, pos.line);
    if (!view || view.hidden) { return null }
    var line = getLine(cm.doc, pos.line);
    var info = mapFromLineView(view, line, pos.line);

    var order = getOrder(line, cm.doc.direction), side = "left";
    if (order) {
      var partPos = getBidiPartAt(order, pos.ch);
      side = partPos % 2 ? "right" : "left";
    }
    var result = nodeAndOffsetInLineMap(info.map, pos.ch, side);
    result.offset = result.collapse == "right" ? result.end : result.start;
    return result
  }

  function isInGutter(node) {
    for (var scan = node; scan; scan = scan.parentNode)
      { if (/CodeMirror-gutter-wrapper/.test(scan.className)) { return true } }
    return false
  }

  function badPos(pos, bad) { if (bad) { pos.bad = true; } return pos }

  function domTextBetween(cm, from, to, fromLine, toLine) {
    var text = "", closing = false, lineSep = cm.doc.lineSeparator(), extraLinebreak = false;
    function recognizeMarker(id) { return function (marker) { return marker.id == id; } }
    function close() {
      if (closing) {
        text += lineSep;
        if (extraLinebreak) { text += lineSep; }
        closing = extraLinebreak = false;
      }
    }
    function addText(str) {
      if (str) {
        close();
        text += str;
      }
    }
    function walk(node) {
      if (node.nodeType == 1) {
        var cmText = node.getAttribute("cm-text");
        if (cmText) {
          addText(cmText);
          return
        }
        var markerID = node.getAttribute("cm-marker"), range;
        if (markerID) {
          var found = cm.findMarks(Pos(fromLine, 0), Pos(toLine + 1, 0), recognizeMarker(+markerID));
          if (found.length && (range = found[0].find(0)))
            { addText(getBetween(cm.doc, range.from, range.to).join(lineSep)); }
          return
        }
        if (node.getAttribute("contenteditable") == "false") { return }
        var isBlock = /^(pre|div|p|li|table|br)$/i.test(node.nodeName);
        if (!/^br$/i.test(node.nodeName) && node.textContent.length == 0) { return }

        if (isBlock) { close(); }
        for (var i = 0; i < node.childNodes.length; i++)
          { walk(node.childNodes[i]); }

        if (/^(pre|p)$/i.test(node.nodeName)) { extraLinebreak = true; }
        if (isBlock) { closing = true; }
      } else if (node.nodeType == 3) {
        addText(node.nodeValue.replace(/\u200b/g, "").replace(/\u00a0/g, " "));
      }
    }
    for (;;) {
      walk(from);
      if (from == to) { break }
      from = from.nextSibling;
      extraLinebreak = false;
    }
    return text
  }

  function domToPos(cm, node, offset) {
    var lineNode;
    if (node == cm.display.lineDiv) {
      lineNode = cm.display.lineDiv.childNodes[offset];
      if (!lineNode) { return badPos(cm.clipPos(Pos(cm.display.viewTo - 1)), true) }
      node = null; offset = 0;
    } else {
      for (lineNode = node;; lineNode = lineNode.parentNode) {
        if (!lineNode || lineNode == cm.display.lineDiv) { return null }
        if (lineNode.parentNode && lineNode.parentNode == cm.display.lineDiv) { break }
      }
    }
    for (var i = 0; i < cm.display.view.length; i++) {
      var lineView = cm.display.view[i];
      if (lineView.node == lineNode)
        { return locateNodeInLineView(lineView, node, offset) }
    }
  }

  function locateNodeInLineView(lineView, node, offset) {
    var wrapper = lineView.text.firstChild, bad = false;
    if (!node || !contains(wrapper, node)) { return badPos(Pos(lineNo(lineView.line), 0), true) }
    if (node == wrapper) {
      bad = true;
      node = wrapper.childNodes[offset];
      offset = 0;
      if (!node) {
        var line = lineView.rest ? lst(lineView.rest) : lineView.line;
        return badPos(Pos(lineNo(line), line.text.length), bad)
      }
    }

    var textNode = node.nodeType == 3 ? node : null, topNode = node;
    if (!textNode && node.childNodes.length == 1 && node.firstChild.nodeType == 3) {
      textNode = node.firstChild;
      if (offset) { offset = textNode.nodeValue.length; }
    }
    while (topNode.parentNode != wrapper) { topNode = topNode.parentNode; }
    var measure = lineView.measure, maps = measure.maps;

    function find(textNode, topNode, offset) {
      for (var i = -1; i < (maps ? maps.length : 0); i++) {
        var map = i < 0 ? measure.map : maps[i];
        for (var j = 0; j < map.length; j += 3) {
          var curNode = map[j + 2];
          if (curNode == textNode || curNode == topNode) {
            var line = lineNo(i < 0 ? lineView.line : lineView.rest[i]);
            var ch = map[j] + offset;
            if (offset < 0 || curNode != textNode) { ch = map[j + (offset ? 1 : 0)]; }
            return Pos(line, ch)
          }
        }
      }
    }
    var found = find(textNode, topNode, offset);
    if (found) { return badPos(found, bad) }

    // FIXME this is all really shaky. might handle the few cases it needs to handle, but likely to cause problems
    for (var after = topNode.nextSibling, dist = textNode ? textNode.nodeValue.length - offset : 0; after; after = after.nextSibling) {
      found = find(after, after.firstChild, 0);
      if (found)
        { return badPos(Pos(found.line, found.ch - dist), bad) }
      else
        { dist += after.textContent.length; }
    }
    for (var before = topNode.previousSibling, dist$1 = offset; before; before = before.previousSibling) {
      found = find(before, before.firstChild, -1);
      if (found)
        { return badPos(Pos(found.line, found.ch + dist$1), bad) }
      else
        { dist$1 += before.textContent.length; }
    }
  }

  // TEXTAREA INPUT STYLE

  var TextareaInput = function(cm) {
    this.cm = cm;
    // See input.poll and input.reset
    this.prevInput = "";

    // Flag that indicates whether we expect input to appear real soon
    // now (after some event like 'keypress' or 'input') and are
    // polling intensively.
    this.pollingFast = false;
    // Self-resetting timeout for the poller
    this.polling = new Delayed();
    // Used to work around IE issue with selection being forgotten when focus moves away from textarea
    this.hasSelection = false;
    this.composing = null;
  };

  TextareaInput.prototype.init = function (display) {
      var this$1 = this;

    var input = this, cm = this.cm;
    this.createField(display);
    var te = this.textarea;

    display.wrapper.insertBefore(this.wrapper, display.wrapper.firstChild);

    // Needed to hide big blue blinking cursor on Mobile Safari (doesn't seem to work in iOS 8 anymore)
    if (ios) { te.style.width = "0px"; }

    on(te, "input", function () {
      if (ie && ie_version >= 9 && this$1.hasSelection) { this$1.hasSelection = null; }
      input.poll();
    });

    on(te, "paste", function (e) {
      if (signalDOMEvent(cm, e) || handlePaste(e, cm)) { return }

      cm.state.pasteIncoming = +new Date;
      input.fastPoll();
    });

    function prepareCopyCut(e) {
      if (signalDOMEvent(cm, e)) { return }
      if (cm.somethingSelected()) {
        setLastCopied({lineWise: false, text: cm.getSelections()});
      } else if (!cm.options.lineWiseCopyCut) {
        return
      } else {
        var ranges = copyableRanges(cm);
        setLastCopied({lineWise: true, text: ranges.text});
        if (e.type == "cut") {
          cm.setSelections(ranges.ranges, null, sel_dontScroll);
        } else {
          input.prevInput = "";
          te.value = ranges.text.join("\n");
          selectInput(te);
        }
      }
      if (e.type == "cut") { cm.state.cutIncoming = +new Date; }
    }
    on(te, "cut", prepareCopyCut);
    on(te, "copy", prepareCopyCut);

    on(display.scroller, "paste", function (e) {
      if (eventInWidget(display, e) || signalDOMEvent(cm, e)) { return }
      if (!te.dispatchEvent) {
        cm.state.pasteIncoming = +new Date;
        input.focus();
        return
      }

      // Pass the `paste` event to the textarea so it's handled by its event listener.
      var event = new Event("paste");
      event.clipboardData = e.clipboardData;
      te.dispatchEvent(event);
    });

    // Prevent normal selection in the editor (we handle our own)
    on(display.lineSpace, "selectstart", function (e) {
      if (!eventInWidget(display, e)) { e_preventDefault(e); }
    });

    on(te, "compositionstart", function () {
      var start = cm.getCursor("from");
      if (input.composing) { input.composing.range.clear(); }
      input.composing = {
        start: start,
        range: cm.markText(start, cm.getCursor("to"), {className: "CodeMirror-composing"})
      };
    });
    on(te, "compositionend", function () {
      if (input.composing) {
        input.poll();
        input.composing.range.clear();
        input.composing = null;
      }
    });
  };

  TextareaInput.prototype.createField = function (_display) {
    // Wraps and hides input textarea
    this.wrapper = hiddenTextarea();
    // The semihidden textarea that is focused when the editor is
    // focused, and receives input.
    this.textarea = this.wrapper.firstChild;
  };

  TextareaInput.prototype.screenReaderLabelChanged = function (label) {
    // Label for screenreaders, accessibility
    if(label) {
      this.textarea.setAttribute('aria-label', label);
    } else {
      this.textarea.removeAttribute('aria-label');
    }
  };

  TextareaInput.prototype.prepareSelection = function () {
    // Redraw the selection and/or cursor
    var cm = this.cm, display = cm.display, doc = cm.doc;
    var result = prepareSelection(cm);

    // Move the hidden textarea near the cursor to prevent scrolling artifacts
    if (cm.options.moveInputWithCursor) {
      var headPos = cursorCoords(cm, doc.sel.primary().head, "div");
      var wrapOff = display.wrapper.getBoundingClientRect(), lineOff = display.lineDiv.getBoundingClientRect();
      result.teTop = Math.max(0, Math.min(display.wrapper.clientHeight - 10,
                                          headPos.top + lineOff.top - wrapOff.top));
      result.teLeft = Math.max(0, Math.min(display.wrapper.clientWidth - 10,
                                           headPos.left + lineOff.left - wrapOff.left));
    }

    return result
  };

  TextareaInput.prototype.showSelection = function (drawn) {
    var cm = this.cm, display = cm.display;
    removeChildrenAndAdd(display.cursorDiv, drawn.cursors);
    removeChildrenAndAdd(display.selectionDiv, drawn.selection);
    if (drawn.teTop != null) {
      this.wrapper.style.top = drawn.teTop + "px";
      this.wrapper.style.left = drawn.teLeft + "px";
    }
  };

  // Reset the input to correspond to the selection (or to be empty,
  // when not typing and nothing is selected)
  TextareaInput.prototype.reset = function (typing) {
    if (this.contextMenuPending || this.composing) { return }
    var cm = this.cm;
    if (cm.somethingSelected()) {
      this.prevInput = "";
      var content = cm.getSelection();
      this.textarea.value = content;
      if (cm.state.focused) { selectInput(this.textarea); }
      if (ie && ie_version >= 9) { this.hasSelection = content; }
    } else if (!typing) {
      this.prevInput = this.textarea.value = "";
      if (ie && ie_version >= 9) { this.hasSelection = null; }
    }
  };

  TextareaInput.prototype.getField = function () { return this.textarea };

  TextareaInput.prototype.supportsTouch = function () { return false };

  TextareaInput.prototype.focus = function () {
    if (this.cm.options.readOnly != "nocursor" && (!mobile || activeElt() != this.textarea)) {
      try { this.textarea.focus(); }
      catch (e) {} // IE8 will throw if the textarea is display: none or not in DOM
    }
  };

  TextareaInput.prototype.blur = function () { this.textarea.blur(); };

  TextareaInput.prototype.resetPosition = function () {
    this.wrapper.style.top = this.wrapper.style.left = 0;
  };

  TextareaInput.prototype.receivedFocus = function () { this.slowPoll(); };

  // Poll for input changes, using the normal rate of polling. This
  // runs as long as the editor is focused.
  TextareaInput.prototype.slowPoll = function () {
      var this$1 = this;

    if (this.pollingFast) { return }
    this.polling.set(this.cm.options.pollInterval, function () {
      this$1.poll();
      if (this$1.cm.state.focused) { this$1.slowPoll(); }
    });
  };

  // When an event has just come in that is likely to add or change
  // something in the input textarea, we poll faster, to ensure that
  // the change appears on the screen quickly.
  TextareaInput.prototype.fastPoll = function () {
    var missed = false, input = this;
    input.pollingFast = true;
    function p() {
      var changed = input.poll();
      if (!changed && !missed) {missed = true; input.polling.set(60, p);}
      else {input.pollingFast = false; input.slowPoll();}
    }
    input.polling.set(20, p);
  };

  // Read input from the textarea, and update the document to match.
  // When something is selected, it is present in the textarea, and
  // selected (unless it is huge, in which case a placeholder is
  // used). When nothing is selected, the cursor sits after previously
  // seen text (can be empty), which is stored in prevInput (we must
  // not reset the textarea when typing, because that breaks IME).
  TextareaInput.prototype.poll = function () {
      var this$1 = this;

    var cm = this.cm, input = this.textarea, prevInput = this.prevInput;
    // Since this is called a *lot*, try to bail out as cheaply as
    // possible when it is clear that nothing happened. hasSelection
    // will be the case when there is a lot of text in the textarea,
    // in which case reading its value would be expensive.
    if (this.contextMenuPending || !cm.state.focused ||
        (hasSelection(input) && !prevInput && !this.composing) ||
        cm.isReadOnly() || cm.options.disableInput || cm.state.keySeq)
      { return false }

    var text = input.value;
    // If nothing changed, bail.
    if (text == prevInput && !cm.somethingSelected()) { return false }
    // Work around nonsensical selection resetting in IE9/10, and
    // inexplicable appearance of private area unicode characters on
    // some key combos in Mac (#2689).
    if (ie && ie_version >= 9 && this.hasSelection === text ||
        mac && /[\uf700-\uf7ff]/.test(text)) {
      cm.display.input.reset();
      return false
    }

    if (cm.doc.sel == cm.display.selForContextMenu) {
      var first = text.charCodeAt(0);
      if (first == 0x200b && !prevInput) { prevInput = "\u200b"; }
      if (first == 0x21da) { this.reset(); return this.cm.execCommand("undo") }
    }
    // Find the part of the input that is actually new
    var same = 0, l = Math.min(prevInput.length, text.length);
    while (same < l && prevInput.charCodeAt(same) == text.charCodeAt(same)) { ++same; }

    runInOp(cm, function () {
      applyTextInput(cm, text.slice(same), prevInput.length - same,
                     null, this$1.composing ? "*compose" : null);

      // Don't leave long text in the textarea, since it makes further polling slow
      if (text.length > 1000 || text.indexOf("\n") > -1) { input.value = this$1.prevInput = ""; }
      else { this$1.prevInput = text; }

      if (this$1.composing) {
        this$1.composing.range.clear();
        this$1.composing.range = cm.markText(this$1.composing.start, cm.getCursor("to"),
                                           {className: "CodeMirror-composing"});
      }
    });
    return true
  };

  TextareaInput.prototype.ensurePolled = function () {
    if (this.pollingFast && this.poll()) { this.pollingFast = false; }
  };

  TextareaInput.prototype.onKeyPress = function () {
    if (ie && ie_version >= 9) { this.hasSelection = null; }
    this.fastPoll();
  };

  TextareaInput.prototype.onContextMenu = function (e) {
    var input = this, cm = input.cm, display = cm.display, te = input.textarea;
    if (input.contextMenuPending) { input.contextMenuPending(); }
    var pos = posFromMouse(cm, e), scrollPos = display.scroller.scrollTop;
    if (!pos || presto) { return } // Opera is difficult.

    // Reset the current text selection only if the click is done outside of the selection
    // and 'resetSelectionOnContextMenu' option is true.
    var reset = cm.options.resetSelectionOnContextMenu;
    if (reset && cm.doc.sel.contains(pos) == -1)
      { operation(cm, setSelection)(cm.doc, simpleSelection(pos), sel_dontScroll); }

    var oldCSS = te.style.cssText, oldWrapperCSS = input.wrapper.style.cssText;
    var wrapperBox = input.wrapper.offsetParent.getBoundingClientRect();
    input.wrapper.style.cssText = "position: static";
    te.style.cssText = "position: absolute; width: 30px; height: 30px;\n      top: " + (e.clientY - wrapperBox.top - 5) + "px; left: " + (e.clientX - wrapperBox.left - 5) + "px;\n      z-index: 1000; background: " + (ie ? "rgba(255, 255, 255, .05)" : "transparent") + ";\n      outline: none; border-width: 0; outline: none; overflow: hidden; opacity: .05; filter: alpha(opacity=5);";
    var oldScrollY;
    if (webkit) { oldScrollY = window.scrollY; } // Work around Chrome issue (#2712)
    display.input.focus();
    if (webkit) { window.scrollTo(null, oldScrollY); }
    display.input.reset();
    // Adds "Select all" to context menu in FF
    if (!cm.somethingSelected()) { te.value = input.prevInput = " "; }
    input.contextMenuPending = rehide;
    display.selForContextMenu = cm.doc.sel;
    clearTimeout(display.detectingSelectAll);

    // Select-all will be greyed out if there's nothing to select, so
    // this adds a zero-width space so that we can later check whether
    // it got selected.
    function prepareSelectAllHack() {
      if (te.selectionStart != null) {
        var selected = cm.somethingSelected();
        var extval = "\u200b" + (selected ? te.value : "");
        te.value = "\u21da"; // Used to catch context-menu undo
        te.value = extval;
        input.prevInput = selected ? "" : "\u200b";
        te.selectionStart = 1; te.selectionEnd = extval.length;
        // Re-set this, in case some other handler touched the
        // selection in the meantime.
        display.selForContextMenu = cm.doc.sel;
      }
    }
    function rehide() {
      if (input.contextMenuPending != rehide) { return }
      input.contextMenuPending = false;
      input.wrapper.style.cssText = oldWrapperCSS;
      te.style.cssText = oldCSS;
      if (ie && ie_version < 9) { display.scrollbars.setScrollTop(display.scroller.scrollTop = scrollPos); }

      // Try to detect the user choosing select-all
      if (te.selectionStart != null) {
        if (!ie || (ie && ie_version < 9)) { prepareSelectAllHack(); }
        var i = 0, poll = function () {
          if (display.selForContextMenu == cm.doc.sel && te.selectionStart == 0 &&
              te.selectionEnd > 0 && input.prevInput == "\u200b") {
            operation(cm, selectAll)(cm);
          } else if (i++ < 10) {
            display.detectingSelectAll = setTimeout(poll, 500);
          } else {
            display.selForContextMenu = null;
            display.input.reset();
          }
        };
        display.detectingSelectAll = setTimeout(poll, 200);
      }
    }

    if (ie && ie_version >= 9) { prepareSelectAllHack(); }
    if (captureRightClick) {
      e_stop(e);
      var mouseup = function () {
        off(window, "mouseup", mouseup);
        setTimeout(rehide, 20);
      };
      on(window, "mouseup", mouseup);
    } else {
      setTimeout(rehide, 50);
    }
  };

  TextareaInput.prototype.readOnlyChanged = function (val) {
    if (!val) { this.reset(); }
    this.textarea.disabled = val == "nocursor";
    this.textarea.readOnly = !!val;
  };

  TextareaInput.prototype.setUneditable = function () {};

  TextareaInput.prototype.needsContentAttribute = false;

  function fromTextArea(textarea, options) {
    options = options ? copyObj(options) : {};
    options.value = textarea.value;
    if (!options.tabindex && textarea.tabIndex)
      { options.tabindex = textarea.tabIndex; }
    if (!options.placeholder && textarea.placeholder)
      { options.placeholder = textarea.placeholder; }
    // Set autofocus to true if this textarea is focused, or if it has
    // autofocus and no other element is focused.
    if (options.autofocus == null) {
      var hasFocus = activeElt();
      options.autofocus = hasFocus == textarea ||
        textarea.getAttribute("autofocus") != null && hasFocus == document.body;
    }

    function save() {textarea.value = cm.getValue();}

    var realSubmit;
    if (textarea.form) {
      on(textarea.form, "submit", save);
      // Deplorable hack to make the submit method do the right thing.
      if (!options.leaveSubmitMethodAlone) {
        var form = textarea.form;
        realSubmit = form.submit;
        try {
          var wrappedSubmit = form.submit = function () {
            save();
            form.submit = realSubmit;
            form.submit();
            form.submit = wrappedSubmit;
          };
        } catch(e) {}
      }
    }

    options.finishInit = function (cm) {
      cm.save = save;
      cm.getTextArea = function () { return textarea; };
      cm.toTextArea = function () {
        cm.toTextArea = isNaN; // Prevent this from being ran twice
        save();
        textarea.parentNode.removeChild(cm.getWrapperElement());
        textarea.style.display = "";
        if (textarea.form) {
          off(textarea.form, "submit", save);
          if (!options.leaveSubmitMethodAlone && typeof textarea.form.submit == "function")
            { textarea.form.submit = realSubmit; }
        }
      };
    };

    textarea.style.display = "none";
    var cm = CodeMirror(function (node) { return textarea.parentNode.insertBefore(node, textarea.nextSibling); },
      options);
    return cm
  }

  function addLegacyProps(CodeMirror) {
    CodeMirror.off = off;
    CodeMirror.on = on;
    CodeMirror.wheelEventPixels = wheelEventPixels;
    CodeMirror.Doc = Doc;
    CodeMirror.splitLines = splitLinesAuto;
    CodeMirror.countColumn = countColumn;
    CodeMirror.findColumn = findColumn;
    CodeMirror.isWordChar = isWordCharBasic;
    CodeMirror.Pass = Pass;
    CodeMirror.signal = signal;
    CodeMirror.Line = Line;
    CodeMirror.changeEnd = changeEnd;
    CodeMirror.scrollbarModel = scrollbarModel;
    CodeMirror.Pos = Pos;
    CodeMirror.cmpPos = cmp;
    CodeMirror.modes = modes;
    CodeMirror.mimeModes = mimeModes;
    CodeMirror.resolveMode = resolveMode;
    CodeMirror.getMode = getMode;
    CodeMirror.modeExtensions = modeExtensions;
    CodeMirror.extendMode = extendMode;
    CodeMirror.copyState = copyState;
    CodeMirror.startState = startState;
    CodeMirror.innerMode = innerMode;
    CodeMirror.commands = commands;
    CodeMirror.keyMap = keyMap;
    CodeMirror.keyName = keyName;
    CodeMirror.isModifierKey = isModifierKey;
    CodeMirror.lookupKey = lookupKey;
    CodeMirror.normalizeKeyMap = normalizeKeyMap;
    CodeMirror.StringStream = StringStream;
    CodeMirror.SharedTextMarker = SharedTextMarker;
    CodeMirror.TextMarker = TextMarker;
    CodeMirror.LineWidget = LineWidget;
    CodeMirror.e_preventDefault = e_preventDefault;
    CodeMirror.e_stopPropagation = e_stopPropagation;
    CodeMirror.e_stop = e_stop;
    CodeMirror.addClass = addClass;
    CodeMirror.contains = contains;
    CodeMirror.rmClass = rmClass;
    CodeMirror.keyNames = keyNames;
  }

  // EDITOR CONSTRUCTOR

  defineOptions(CodeMirror);

  addEditorMethods(CodeMirror);

  // Set up methods on CodeMirror's prototype to redirect to the editor's document.
  var dontDelegate = "iter insert remove copy getEditor constructor".split(" ");
  for (var prop in Doc.prototype) { if (Doc.prototype.hasOwnProperty(prop) && indexOf(dontDelegate, prop) < 0)
    { CodeMirror.prototype[prop] = (function(method) {
      return function() {return method.apply(this.doc, arguments)}
    })(Doc.prototype[prop]); } }

  eventMixin(Doc);
  CodeMirror.inputStyles = {"textarea": TextareaInput, "contenteditable": ContentEditableInput};

  // Extra arguments are stored as the mode's dependencies, which is
  // used by (legacy) mechanisms like loadmode.js to automatically
  // load a mode. (Preferred mechanism is the require/define calls.)
  CodeMirror.defineMode = function(name/*, mode, …*/) {
    if (!CodeMirror.defaults.mode && name != "null") { CodeMirror.defaults.mode = name; }
    defineMode.apply(this, arguments);
  };

  CodeMirror.defineMIME = defineMIME;

  // Minimal default mode.
  CodeMirror.defineMode("null", function () { return ({token: function (stream) { return stream.skipToEnd(); }}); });
  CodeMirror.defineMIME("text/plain", "null");

  // EXTENSIONS

  CodeMirror.defineExtension = function (name, func) {
    CodeMirror.prototype[name] = func;
  };
  CodeMirror.defineDocExtension = function (name, func) {
    Doc.prototype[name] = func;
  };

  CodeMirror.fromTextArea = fromTextArea;

  addLegacyProps(CodeMirror);

  CodeMirror.version = "5.58.1";

  return CodeMirror;

})));

/*!
 * FilePond 4.20.1
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

!function(e,t){"object"==typeof exports&&"undefined"!=typeof module?t(exports):"function"==typeof define&&define.amd?define(["exports"],t):t((e=e||self).FilePond={})}(this,function(e){"use strict";var t=function(e,t){for(var n in e)e.hasOwnProperty(n)&&t(n,e[n])},n=function(e){var n={};return t(e,function(t){!function(e,t,n){"function"!=typeof n?Object.defineProperty(e,t,Object.assign({},n)):e[t]=n}(n,t,e[t])}),n},r=function(e,t){var n=arguments.length>2&&void 0!==arguments[2]?arguments[2]:null;if(null===n)return e.getAttribute(t)||e.hasAttribute(t);e.setAttribute(t,n)},o=["svg","path"],i=function(e){return o.includes(e)},a=function(e,n){var o=arguments.length>2&&void 0!==arguments[2]?arguments[2]:{};"object"==typeof n&&(o=n,n=null);var a=i(e)?document.createElementNS("http://www.w3.org/2000/svg",e):document.createElement(e);return n&&(i(e)?r(a,"class",n):a.className=n),t(o,function(e,t){r(a,e,t)}),a},u=function(e,t){return function(e,n){return void 0!==n?t.splice(n,0,e):t.push(e),e}},s=function(e,t){return function(n){return t.splice(t.indexOf(n),1),n.element.parentNode&&e.removeChild(n.element),n}},l="undefined"!=typeof window&&void 0!==window.document,c=function(){return l},f="children"in(c()?a("svg"):{})?function(e){return e.children.length}:function(e){return e.childNodes.length},d=function(e,t,n,r){var o=n[0]||e.left,i=n[1]||e.top,a=o+e.width,u=i+e.height*(r[1]||1),s={element:Object.assign({},e),inner:{left:e.left,top:e.top,right:e.right,bottom:e.bottom},outer:{left:o,top:i,right:a,bottom:u}};return t.filter(function(e){return!e.isRectIgnored()}).map(function(e){return e.rect}).forEach(function(e){p(s.inner,Object.assign({},e.inner)),p(s.outer,Object.assign({},e.outer))}),E(s.inner),s.outer.bottom+=s.element.marginBottom,s.outer.right+=s.element.marginRight,E(s.outer),s},p=function(e,t){t.top+=e.top,t.right+=e.left,t.bottom+=e.top,t.left+=e.left,t.bottom>e.bottom&&(e.bottom=t.bottom),t.right>e.right&&(e.right=t.right)},E=function(e){e.width=e.right-e.left,e.height=e.bottom-e.top},_=function(e){return"number"==typeof e},T=function(e){return e<.5?2*e*e:(4-2*e)*e-1},I={spring:function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},t=e.stiffness,r=void 0===t?.5:t,o=e.damping,i=void 0===o?.75:o,a=e.mass,u=void 0===a?10:a,s=null,l=null,c=0,f=!1,d=n({interpolate:function(e,t){if(!f){if(!_(s)||!_(l))return f=!0,void(c=0);(function(e,t,n){var r=arguments.length>3&&void 0!==arguments[3]?arguments[3]:.001;return Math.abs(e-t)<r&&Math.abs(n)<r})(l+=c+=-(l-s)*r/u,s,c*=i)||t?(l=s,c=0,f=!0,d.onupdate(l),d.oncomplete(l)):d.onupdate(l)}},target:{set:function(e){if(_(e)&&!_(l)&&(l=e),null===s&&(s=e,l=e),l===(s=e)||void 0===s)return f=!0,c=0,d.onupdate(l),void d.oncomplete(l);f=!1},get:function(){return s}},resting:{get:function(){return f}},onupdate:function(e){},oncomplete:function(e){}});return d},tween:function(){var e,t,r=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},o=r.duration,i=void 0===o?500:o,a=r.easing,u=void 0===a?T:a,s=r.delay,l=void 0===s?0:s,c=null,f=!0,d=!1,p=null,E=n({interpolate:function(n,r){f||null===p||(null===c&&(c=n),n-c<l||((e=n-c-l)>=i||r?(e=1,t=d?0:1,E.onupdate(t*p),E.oncomplete(t*p),f=!0):(t=e/i,E.onupdate((e>=0?u(d?1-t:t):0)*p))))},target:{get:function(){return d?0:p},set:function(e){if(null===p)return p=e,E.onupdate(e),void E.oncomplete(e);e<p?(p=1,d=!0):(d=!1,p=e),f=!1,c=null}},resting:{get:function(){return f}},onupdate:function(e){},oncomplete:function(e){}});return E}},v=function(e,t,n){var r=e[t]&&"object"==typeof e[t][n]?e[t][n]:e[t]||e,o="string"==typeof r?r:r.type,i="object"==typeof r?Object.assign({},r):{};return I[o]?I[o](i):null},m=function(e,t,n){var r=arguments.length>3&&void 0!==arguments[3]&&arguments[3];(t=Array.isArray(t)?t:[t]).forEach(function(t){e.forEach(function(e){var o=e,i=function(){return n[e]},a=function(t){return n[e]=t};"object"==typeof e&&(o=e.key,i=e.getter||i,a=e.setter||a),t[o]&&!r||(t[o]={get:i,set:a})})})},g=function(e){return null!=e},h={opacity:1,scaleX:1,scaleY:1,translateX:0,translateY:0,rotateX:0,rotateY:0,rotateZ:0,originX:0,originY:0},R=function(e,t){if(Object.keys(e).length!==Object.keys(t).length)return!0;for(var n in t)if(t[n]!==e[n])return!0;return!1},O=function(e,t){var n=t.opacity,r=t.perspective,o=t.translateX,i=t.translateY,a=t.scaleX,u=t.scaleY,s=t.rotateX,l=t.rotateY,c=t.rotateZ,f=t.originX,d=t.originY,p=t.width,E=t.height,_="",T="";(g(f)||g(d))&&(T+="transform-origin: "+(f||0)+"px "+(d||0)+"px;"),g(r)&&(_+="perspective("+r+"px) "),(g(o)||g(i))&&(_+="translate3d("+(o||0)+"px, "+(i||0)+"px, 0) "),(g(a)||g(u))&&(_+="scale3d("+(g(a)?a:1)+", "+(g(u)?u:1)+", 1) "),g(c)&&(_+="rotateZ("+c+"rad) "),g(s)&&(_+="rotateX("+s+"rad) "),g(l)&&(_+="rotateY("+l+"rad) "),_.length&&(T+="transform:"+_+";"),g(n)&&(T+="opacity:"+n+";",0===n&&(T+="visibility:hidden;"),n<1&&(T+="pointer-events:none;")),g(E)&&(T+="height:"+E+"px;"),g(p)&&(T+="width:"+p+"px;");var I=e.elementCurrentStyle||"";T.length===I.length&&T===I||(e.style.cssText=T,e.elementCurrentStyle=T)},y={styles:function(e){var t=e.mixinConfig,n=e.viewProps,r=e.viewInternalAPI,o=e.viewExternalAPI,i=e.view,a=Object.assign({},n),u={};m(t,[r,o],n);var s=function(){return i.rect?d(i.rect,i.childViews,[n.translateX||0,n.translateY||0],[n.scaleX||0,n.scaleY||0]):null};return r.rect={get:s},o.rect={get:s},t.forEach(function(e){n[e]=void 0===a[e]?h[e]:a[e]}),{write:function(){if(R(u,n))return O(i.element,n),Object.assign(u,Object.assign({},n)),!0},destroy:function(){}}},listeners:function(e){e.mixinConfig,e.viewProps,e.viewInternalAPI;var t,n=e.viewExternalAPI,r=(e.viewState,e.view),o=[],i=(t=r.element,function(e,n){t.addEventListener(e,n)}),a=function(e){return function(t,n){e.removeEventListener(t,n)}}(r.element);return n.on=function(e,t){o.push({type:e,fn:t}),i(e,t)},n.off=function(e,t){o.splice(o.findIndex(function(n){return n.type===e&&n.fn===t}),1),a(e,t)},{write:function(){return!0},destroy:function(){o.forEach(function(e){a(e.type,e.fn)})}}},animations:function(e){var n=e.mixinConfig,r=e.viewProps,o=e.viewInternalAPI,i=e.viewExternalAPI,a=Object.assign({},r),u=[];return t(n,function(e,t){var n=v(t);n&&(n.onupdate=function(t){r[e]=t},n.target=a[e],m([{key:e,setter:function(e){n.target!==e&&(n.target=e)},getter:function(){return r[e]}}],[o,i],r,!0),u.push(n))}),{write:function(e){var t=document.hidden,n=!0;return u.forEach(function(r){r.resting||(n=!1),r.interpolate(e,t)}),n},destroy:function(){}}},apis:function(e){var t=e.mixinConfig,n=e.viewProps,r=e.viewExternalAPI;m(t,r,n)}},D=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{},n=arguments.length>2&&void 0!==arguments[2]?arguments[2]:{};return t.layoutCalculated||(e.paddingTop=parseInt(n.paddingTop,10)||0,e.marginTop=parseInt(n.marginTop,10)||0,e.marginRight=parseInt(n.marginRight,10)||0,e.marginBottom=parseInt(n.marginBottom,10)||0,e.marginLeft=parseInt(n.marginLeft,10)||0,t.layoutCalculated=!0),e.left=t.offsetLeft||0,e.top=t.offsetTop||0,e.width=t.offsetWidth||0,e.height=t.offsetHeight||0,e.right=e.left+e.width,e.bottom=e.top+e.height,e.scrollTop=t.scrollTop,e.hidden=null===t.offsetParent,e},S=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},t=e.tag,r=void 0===t?"div":t,o=e.name,i=void 0===o?null:o,l=e.attributes,c=void 0===l?{}:l,p=e.read,E=void 0===p?function(){}:p,_=e.write,T=void 0===_?function(){}:_,I=e.create,v=void 0===I?function(){}:I,m=e.destroy,g=void 0===m?function(){}:m,h=e.filterFrameActionsForChild,R=void 0===h?function(e,t){return t}:h,O=e.didCreateView,S=void 0===O?function(){}:O,A=e.didWriteView,b=void 0===A?function(){}:A,P=e.ignoreRect,L=void 0!==P&&P,M=e.ignoreRectUpdate,w=void 0!==M&&M,C=e.mixins,N=void 0===C?[]:C;return function(e){var t,o=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{},l=a(r,"filepond--"+i,c),p=window.getComputedStyle(l,null),_=D(),I=null,m=!1,h=[],O=[],A={},P={},M=[T],C=[E],G=[g],U=function(){return l},B=function(){return h.concat()},q=function(){return I||(I=d(_,h,[0,0],[1,1]))},V={element:{get:U},style:{get:function(){return p}},childViews:{get:B}},F=Object.assign({},V,{rect:{get:q},ref:{get:function(){return A}},is:function(e){return i===e},appendChild:(t=l,function(e,n){void 0!==n&&t.children[n]?t.insertBefore(e,t.children[n]):t.appendChild(e)}),createChildView:function(e){return function(t,n){return t(e,n)}}(e),linkView:function(e){return h.push(e),e},unlinkView:function(e){h.splice(h.indexOf(e),1)},appendChildView:u(0,h),removeChildView:s(l,h),registerWriter:function(e){return M.push(e)},registerReader:function(e){return C.push(e)},registerDestroyer:function(e){return G.push(e)},invalidateLayout:function(){return l.layoutCalculated=!1},dispatch:e.dispatch,query:e.query}),x={element:{get:U},childViews:{get:B},rect:{get:q},resting:{get:function(){return m}},isRectIgnored:function(){return L},_read:function(){I=null,h.forEach(function(e){return e._read()}),!(w&&_.width&&_.height)&&D(_,l,p);var e={root:j,props:o,rect:_};C.forEach(function(t){return t(e)})},_write:function(e,t,n){var r=0===t.length;return M.forEach(function(i){!1===i({props:o,root:j,actions:t,timestamp:e,shouldOptimize:n})&&(r=!1)}),O.forEach(function(t){!1===t.write(e)&&(r=!1)}),h.filter(function(e){return!!e.element.parentNode}).forEach(function(o){o._write(e,R(o,t),n)||(r=!1)}),h.forEach(function(o,i){o.element.parentNode||(j.appendChild(o.element,i),o._read(),o._write(e,R(o,t),n),r=!1)}),m=r,b({props:o,root:j,actions:t,timestamp:e}),r},_destroy:function(){O.forEach(function(e){return e.destroy()}),G.forEach(function(e){e({root:j,props:o})}),h.forEach(function(e){return e._destroy()})}},Y=Object.assign({},V,{rect:{get:function(){return _}}});Object.keys(N).sort(function(e,t){return"styles"===e?1:"styles"===t?-1:0}).forEach(function(e){var t=y[e]({mixinConfig:N[e],viewProps:o,viewState:P,viewInternalAPI:F,viewExternalAPI:x,view:n(Y)});t&&O.push(t)});var j=n(F);v({root:j,props:o});var k=f(l);return h.forEach(function(e,t){j.appendChild(e.element,k+t)}),S(j),n(x)}},A=function(e,t){return function(n){var r=n.root,o=n.props,i=n.actions,a=void 0===i?[]:i,u=n.timestamp,s=n.shouldOptimize;a.filter(function(t){return e[t.type]}).forEach(function(t){return e[t.type]({root:r,props:o,action:t.data,timestamp:u,shouldOptimize:s})}),t&&t({root:r,props:o,actions:a,timestamp:u,shouldOptimize:s})}},b=function(e,t){return t.parentNode.insertBefore(e,t)},P=function(e,t){return t.parentNode.insertBefore(e,t.nextSibling)},L=function(e){return Array.isArray(e)},M=function(e){return null==e},w=function(e){return e.trim()},C=function(e){return""+e},N=function(e){return"boolean"==typeof e},G=function(e){return N(e)?e:"true"===e},U=function(e){return"string"==typeof e},B=function(e){return _(e)?e:U(e)?C(e).replace(/[a-z]+/gi,""):0},q=function(e){return parseInt(B(e),10)},V=function(e){return parseFloat(B(e))},F=function(e){return _(e)&&isFinite(e)&&Math.floor(e)===e},x=function(e){if(F(e))return e;var t=C(e).trim();return/MB$/i.test(t)?(t=t.replace(/MB$i/,"").trim(),1e3*q(t)*1e3):/KB/i.test(t)?(t=t.replace(/KB$i/,"").trim(),1e3*q(t)):q(t)},Y=function(e){return"function"==typeof e},j={process:"POST",patch:"PATCH",revert:"DELETE",fetch:"GET",restore:"GET",load:"GET"},k=function(e,t,n,r,o){if(null===t)return null;if("function"==typeof t)return t;var i={url:"GET"===n||"PATCH"===n?"?"+e+"=":"",method:n,headers:o,withCredentials:!1,timeout:r,onload:null,ondata:null,onerror:null};if(U(t))return i.url=t,i;if(Object.assign(i,t),U(i.headers)){var a=i.headers.split(/:(.+)/);i.headers={header:a[0],value:a[1]}}return i.withCredentials=G(i.withCredentials),i},H=function(e){return"object"==typeof e&&null!==e},X=function(e){return L(e)?"array":function(e){return null===e}(e)?"null":F(e)?"int":/^[0-9]+ ?(?:GB|MB|KB)$/gi.test(e)?"bytes":function(e){return H(e)&&U(e.url)&&H(e.process)&&H(e.revert)&&H(e.restore)&&H(e.fetch)}(e)?"api":typeof e},W={array:function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:",";return M(e)?[]:L(e)?e:C(e).split(t).map(w).filter(function(e){return e.length})},boolean:G,int:function(e){return"bytes"===X(e)?x(e):q(e)},number:V,float:V,bytes:x,string:function(e){return Y(e)?e:C(e)},function:function(e){return function(e){for(var t=self,n=e.split("."),r=null;r=n.shift();)if(!(t=t[r]))return null;return t}(e)},serverapi:function(e){return(r={}).url=U(n=e)?n:n.url||"",r.timeout=n.timeout?parseInt(n.timeout,10):0,r.headers=n.headers?n.headers:{},t(j,function(e){r[e]=k(e,n[e],j[e],r.timeout,r.headers)}),r.remove=n.remove||null,delete r.headers,r;var n,r},object:function(e){try{return JSON.parse(e.replace(/{\s*'/g,'{"').replace(/'\s*}/g,'"}').replace(/'\s*:/g,'":').replace(/:\s*'/g,':"').replace(/,\s*'/g,',"').replace(/'\s*,/g,'",'))}catch(e){return null}}},z=function(e,t,n){if(e===t)return e;var r,o=X(e);if(o!==n){var i=(r=e,W[n](r));if(o=X(i),null===i)throw'Trying to assign value with incorrect type to "'+option+'", allowed type: "'+n+'"';e=i}return e},Q=function(e){var r={};return t(e,function(t){var n,o,i,a=e[t];r[t]=(n=a[0],o=a[1],i=n,{enumerable:!0,get:function(){return i},set:function(e){i=z(e,n,o)}})}),n(r)},$=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:"-";return e.split(/(?=[A-Z])/).map(function(e){return e.toLowerCase()}).join(t)},Z=function(e){return function(n,r,o){var i={};return t(e,function(e){var t=$(e,"_").toUpperCase();i["SET_"+t]=function(r){try{o.options[e]=r.value}catch(e){}n("DID_SET_"+t,{value:o.options[e]})}}),i}},K=function(e){return function(n){var r={};return t(e,function(e){r["GET_"+$(e,"_").toUpperCase()]=function(t){return n.options[e]}}),r}},J=1,ee=2,te=3,ne=4,re=5,oe=function(){return Math.random().toString(36).substr(2,9)};function ie(e){this.wrapped=e}function ae(e){var t,n;function r(t,n){try{var i=e[t](n),a=i.value,u=a instanceof ie;Promise.resolve(u?a.wrapped:a).then(function(e){u?r("next",e):o(i.done?"return":"normal",e)},function(e){r("throw",e)})}catch(e){o("throw",e)}}function o(e,o){switch(e){case"return":t.resolve({value:o,done:!0});break;case"throw":t.reject(o);break;default:t.resolve({value:o,done:!1})}(t=t.next)?r(t.key,t.arg):n=null}this._invoke=function(e,o){return new Promise(function(i,a){var u={key:e,arg:o,resolve:i,reject:a,next:null};n?n=n.next=u:(t=n=u,r(e,o))})},"function"!=typeof e.return&&(this.return=void 0)}function ue(e,t){if(null==e)return{};var n,r,o=function(e,t){if(null==e)return{};var n,r,o={},i=Object.keys(e);for(r=0;r<i.length;r++)n=i[r],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(r=0;r<i.length;r++)n=i[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}"function"==typeof Symbol&&Symbol.asyncIterator&&(ae.prototype[Symbol.asyncIterator]=function(){return this}),ae.prototype.next=function(e){return this._invoke("next",e)},ae.prototype.throw=function(e){return this._invoke("throw",e)},ae.prototype.return=function(e){return this._invoke("return",e)};function se(e){return function(e){if(Array.isArray(e)){for(var t=0,n=new Array(e.length);t<e.length;t++)n[t]=e[t];return n}}(e)||le(e)||function(){throw new TypeError("Invalid attempt to spread non-iterable instance")}()}function le(e){if(Symbol.iterator in Object(e)||"[object Arguments]"===Object.prototype.toString.call(e))return Array.from(e)}var ce,fe,de=function(e,t){return e.splice(t,1)},pe=function(){var e=[],t=function(t,n){de(e,e.findIndex(function(e){return e.event===t&&(e.cb===n||!n)}))},n=function(t,n,r){e.filter(function(e){return e.event===t}).map(function(e){return e.cb}).forEach(function(e){return function(e,t){t?e():document.hidden?Promise.resolve(1).then(e):setTimeout(e,0)}(function(){return e.apply(void 0,se(n))},r)})};return{fireSync:function(e){for(var t=arguments.length,r=new Array(t>1?t-1:0),o=1;o<t;o++)r[o-1]=arguments[o];n(e,r,!0)},fire:function(e){for(var t=arguments.length,r=new Array(t>1?t-1:0),o=1;o<t;o++)r[o-1]=arguments[o];n(e,r,!1)},on:function(t,n){e.push({event:t,cb:n})},onOnce:function(n,r){e.push({event:n,cb:function(){t(n,r),r.apply(void 0,arguments)}})},off:t}},Ee=function(e,t,n){Object.getOwnPropertyNames(e).filter(function(e){return!n.includes(e)}).forEach(function(n){return Object.defineProperty(t,n,Object.getOwnPropertyDescriptor(e,n))})},_e=["fire","process","revert","load","on","off","onOnce","retryLoad","extend","archive","archived","release","released","requestProcessing","freeze"],Te=function(e){var t={};return Ee(e,t,_e),t},Ie={INIT:1,IDLE:2,PROCESSING_QUEUED:9,PROCESSING:3,PROCESSING_COMPLETE:5,PROCESSING_ERROR:6,PROCESSING_REVERT_ERROR:10,LOADING:7,LOAD_ERROR:8},ve={INPUT:1,LIMBO:2,LOCAL:3},me=function(e){return/[^0-9]+/.exec(e)},ge=function(){return me(1.1.toLocaleString())[0]},he={BOOLEAN:"boolean",INT:"int",NUMBER:"number",STRING:"string",ARRAY:"array",OBJECT:"object",FUNCTION:"function",ACTION:"action",SERVER_API:"serverapi",REGEX:"regex"},Re=[],Oe=function(e,t,n){return new Promise(function(r,o){var i=Re.filter(function(t){return t.key===e}).map(function(e){return e.cb});if(0!==i.length){var a=i.shift();i.reduce(function(e,t){return e.then(function(e){return t(e,n)})},a(t,n)).then(function(e){return r(e)}).catch(function(e){return o(e)})}else r(t)})},ye=function(e,t,n){return Re.filter(function(t){return t.key===e}).map(function(e){return e.cb(t,n)})},De=function(e,t){return Re.push({key:e,cb:t})},Se=function(){return Object.assign({},Ae)},Ae={id:[null,he.STRING],name:["filepond",he.STRING],disabled:[!1,he.BOOLEAN],className:[null,he.STRING],required:[!1,he.BOOLEAN],captureMethod:[null,he.STRING],allowSyncAcceptAttribute:[!0,he.BOOLEAN],allowDrop:[!0,he.BOOLEAN],allowBrowse:[!0,he.BOOLEAN],allowPaste:[!0,he.BOOLEAN],allowMultiple:[!1,he.BOOLEAN],allowReplace:[!0,he.BOOLEAN],allowRevert:[!0,he.BOOLEAN],allowRemove:[!0,he.BOOLEAN],allowProcess:[!0,he.BOOLEAN],allowReorder:[!1,he.BOOLEAN],allowDirectoriesOnly:[!1,he.BOOLEAN],forceRevert:[!1,he.BOOLEAN],maxFiles:[null,he.INT],checkValidity:[!1,he.BOOLEAN],itemInsertLocationFreedom:[!0,he.BOOLEAN],itemInsertLocation:["before",he.STRING],itemInsertInterval:[75,he.INT],dropOnPage:[!1,he.BOOLEAN],dropOnElement:[!0,he.BOOLEAN],dropValidation:[!1,he.BOOLEAN],ignoredFiles:[[".ds_store","thumbs.db","desktop.ini"],he.ARRAY],instantUpload:[!0,he.BOOLEAN],maxParallelUploads:[2,he.INT],chunkUploads:[!1,he.BOOLEAN],chunkForce:[!1,he.BOOLEAN],chunkSize:[5e6,he.INT],chunkRetryDelays:[[500,1e3,3e3],he.Array],server:[null,he.SERVER_API],labelDecimalSeparator:[ge(),he.STRING],labelThousandsSeparator:[(ce=ge(),fe=1e3.toLocaleString(),fe!==1e3.toString()?me(fe)[0]:"."===ce?",":"."),he.STRING],labelIdle:['Drag & Drop your files or <span class="filepond--label-action">Browse</span>',he.STRING],labelInvalidField:["Field contains invalid files",he.STRING],labelFileWaitingForSize:["Waiting for size",he.STRING],labelFileSizeNotAvailable:["Size not available",he.STRING],labelFileCountSingular:["file in list",he.STRING],labelFileCountPlural:["files in list",he.STRING],labelFileLoading:["Loading",he.STRING],labelFileAdded:["Added",he.STRING],labelFileLoadError:["Error during load",he.STRING],labelFileRemoved:["Removed",he.STRING],labelFileRemoveError:["Error during remove",he.STRING],labelFileProcessing:["Uploading",he.STRING],labelFileProcessingComplete:["Upload complete",he.STRING],labelFileProcessingAborted:["Upload cancelled",he.STRING],labelFileProcessingError:["Error during upload",he.STRING],labelFileProcessingRevertError:["Error during revert",he.STRING],labelTapToCancel:["tap to cancel",he.STRING],labelTapToRetry:["tap to retry",he.STRING],labelTapToUndo:["tap to undo",he.STRING],labelButtonRemoveItem:["Remove",he.STRING],labelButtonAbortItemLoad:["Abort",he.STRING],labelButtonRetryItemLoad:["Retry",he.STRING],labelButtonAbortItemProcessing:["Cancel",he.STRING],labelButtonUndoItemProcessing:["Undo",he.STRING],labelButtonRetryItemProcessing:["Retry",he.STRING],labelButtonProcessItem:["Upload",he.STRING],iconRemove:['<svg width="26" height="26" viewBox="0 0 26 26" xmlns="http://www.w3.org/2000/svg"><path d="M11.586 13l-2.293 2.293a1 1 0 0 0 1.414 1.414L13 14.414l2.293 2.293a1 1 0 0 0 1.414-1.414L14.414 13l2.293-2.293a1 1 0 0 0-1.414-1.414L13 11.586l-2.293-2.293a1 1 0 0 0-1.414 1.414L11.586 13z" fill="currentColor" fill-rule="nonzero"/></svg>',he.STRING],iconProcess:['<svg width="26" height="26" viewBox="0 0 26 26" xmlns="http://www.w3.org/2000/svg"><path d="M14 10.414v3.585a1 1 0 0 1-2 0v-3.585l-1.293 1.293a1 1 0 0 1-1.414-1.415l3-3a1 1 0 0 1 1.414 0l3 3a1 1 0 0 1-1.414 1.415L14 10.414zM9 18a1 1 0 0 1 0-2h8a1 1 0 0 1 0 2H9z" fill="currentColor" fill-rule="evenodd"/></svg>',he.STRING],iconRetry:['<svg width="26" height="26" viewBox="0 0 26 26" xmlns="http://www.w3.org/2000/svg"><path d="M10.81 9.185l-.038.02A4.997 4.997 0 0 0 8 13.683a5 5 0 0 0 5 5 5 5 0 0 0 5-5 1 1 0 0 1 2 0A7 7 0 1 1 9.722 7.496l-.842-.21a.999.999 0 1 1 .484-1.94l3.23.806c.535.133.86.675.73 1.21l-.804 3.233a.997.997 0 0 1-1.21.73.997.997 0 0 1-.73-1.21l.23-.928v-.002z" fill="currentColor" fill-rule="nonzero"/></svg>',he.STRING],iconUndo:['<svg width="26" height="26" viewBox="0 0 26 26" xmlns="http://www.w3.org/2000/svg"><path d="M9.185 10.81l.02-.038A4.997 4.997 0 0 1 13.683 8a5 5 0 0 1 5 5 5 5 0 0 1-5 5 1 1 0 0 0 0 2A7 7 0 1 0 7.496 9.722l-.21-.842a.999.999 0 1 0-1.94.484l.806 3.23c.133.535.675.86 1.21.73l3.233-.803a.997.997 0 0 0 .73-1.21.997.997 0 0 0-1.21-.73l-.928.23-.002-.001z" fill="currentColor" fill-rule="nonzero"/></svg>',he.STRING],iconDone:['<svg width="26" height="26" viewBox="0 0 26 26" xmlns="http://www.w3.org/2000/svg"><path d="M18.293 9.293a1 1 0 0 1 1.414 1.414l-7.002 7a1 1 0 0 1-1.414 0l-3.998-4a1 1 0 1 1 1.414-1.414L12 15.586l6.294-6.293z" fill="currentColor" fill-rule="nonzero"/></svg>',he.STRING],oninit:[null,he.FUNCTION],onwarning:[null,he.FUNCTION],onerror:[null,he.FUNCTION],onactivatefile:[null,he.FUNCTION],oninitfile:[null,he.FUNCTION],onaddfilestart:[null,he.FUNCTION],onaddfileprogress:[null,he.FUNCTION],onaddfile:[null,he.FUNCTION],onprocessfilestart:[null,he.FUNCTION],onprocessfileprogress:[null,he.FUNCTION],onprocessfileabort:[null,he.FUNCTION],onprocessfilerevert:[null,he.FUNCTION],onprocessfile:[null,he.FUNCTION],onprocessfiles:[null,he.FUNCTION],onremovefile:[null,he.FUNCTION],onpreparefile:[null,he.FUNCTION],onupdatefiles:[null,he.FUNCTION],onreorderfiles:[null,he.FUNCTION],beforeDropFile:[null,he.FUNCTION],beforeAddFile:[null,he.FUNCTION],beforeRemoveFile:[null,he.FUNCTION],stylePanelLayout:[null,he.STRING],stylePanelAspectRatio:[null,he.STRING],styleItemPanelAspectRatio:[null,he.STRING],styleButtonRemoveItemPosition:["left",he.STRING],styleButtonProcessItemPosition:["right",he.STRING],styleLoadIndicatorPosition:["right",he.STRING],styleProgressIndicatorPosition:["right",he.STRING],styleButtonRemoveItemAlign:[!1,he.BOOLEAN],files:[[],he.ARRAY]},be=function(e,t){return M(t)?e[0]||null:F(t)?e[t]||null:("object"==typeof t&&(t=t.id),e.find(function(e){return e.id===t})||null)},Pe=function(e){if(M(e))return e;if(/:/.test(e)){var t=e.split(":");return t[1]/t[0]}return parseFloat(e)},Le=function(e){return e.filter(function(e){return!e.archived})},Me={EMPTY:0,IDLE:1,ERROR:2,BUSY:3,READY:4},we=[Ie.LOAD_ERROR,Ie.PROCESSING_ERROR,Ie.PROCESSING_REVERT_ERROR],Ce=[Ie.LOADING,Ie.PROCESSING,Ie.PROCESSING_QUEUED,Ie.INIT],Ne=[Ie.PROCESSING_COMPLETE],Ge=function(e){return we.includes(e.status)},Ue=function(e){return Ce.includes(e.status)},Be=function(e){return Ne.includes(e.status)},qe=function(e){return{GET_STATUS:function(){var t=Le(e.items),n=Me.EMPTY,r=Me.ERROR,o=Me.BUSY,i=Me.IDLE,a=Me.READY;return 0===t.length?n:t.some(Ge)?r:t.some(Ue)?o:t.some(Be)?a:i},GET_ITEM:function(t){return be(e.items,t)},GET_ACTIVE_ITEM:function(t){return be(Le(e.items),t)},GET_ACTIVE_ITEMS:function(){return Le(e.items)},GET_ITEMS:function(){return e.items},GET_ITEM_NAME:function(t){var n=be(e.items,t);return n?n.filename:null},GET_ITEM_SIZE:function(t){var n=be(e.items,t);return n?n.fileSize:null},GET_STYLES:function(){return Object.keys(e.options).filter(function(e){return/^style/.test(e)}).map(function(t){return{name:t,value:e.options[t]}})},GET_PANEL_ASPECT_RATIO:function(){return/circle/.test(e.options.stylePanelLayout)?1:Pe(e.options.stylePanelAspectRatio)},GET_ITEM_PANEL_ASPECT_RATIO:function(){return e.options.styleItemPanelAspectRatio},GET_ITEMS_BY_STATUS:function(t){return Le(e.items).filter(function(e){return e.status===t})},GET_TOTAL_ITEMS:function(){return Le(e.items).length},IS_ASYNC:function(){return H(e.options.server)&&(H(e.options.server.process)||Y(e.options.server.process))}}},Ve=function(e,t,n){return Math.max(Math.min(n,e),t)},Fe=function(e){return/^\s*data:([a-z]+\/[a-z0-9-+.]+(;[a-z-]+=[a-z0-9-]+)?)?(;base64)?,([a-z0-9!$&',()*+;=\-._~:@\/?%\s]*)\s*$/i.test(e)},xe=function(e){return e.split("/").pop().split("?").shift()},Ye=function(e){return e.split(".").pop()},je=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:"";return(t+e).slice(-t.length)},ke=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:new Date;return e.getFullYear()+"-"+je(e.getMonth()+1,"00")+"-"+je(e.getDate(),"00")+"_"+je(e.getHours(),"00")+"-"+je(e.getMinutes(),"00")+"-"+je(e.getSeconds(),"00")},He=function(e,t){var n=arguments.length>2&&void 0!==arguments[2]?arguments[2]:null,r=arguments.length>3&&void 0!==arguments[3]?arguments[3]:null,o="string"==typeof n?e.slice(0,e.size,n):e.slice(0,e.size,e.type);return o.lastModifiedDate=new Date,e._relativePath&&(o._relativePath=e._relativePath),U(t)||(t=ke()),t&&null===r&&Ye(t)?o.name=t:(r=r||function(e){if("string"!=typeof e)return"";var t=e.split("/").pop();return/svg/.test(t)?"svg":/zip|compressed/.test(t)?"zip":/plain/.test(t)?"txt":/msword/.test(t)?"doc":/[a-z]+/.test(t)?"jpeg"===t?"jpg":t:""}(o.type),o.name=t+(r?"."+r:"")),o},Xe=function(e,t){var n=window.BlobBuilder=window.BlobBuilder||window.WebKitBlobBuilder||window.MozBlobBuilder||window.MSBlobBuilder;if(n){var r=new n;return r.append(e),r.getBlob(t)}return new Blob([e],{type:t})},We=function(e){return(/^data:(.+);/.exec(e)||[])[1]||null},ze=function(e){var t=We(e);return function(e,t){for(var n=new ArrayBuffer(e.length),r=new Uint8Array(n),o=0;o<e.length;o++)r[o]=e.charCodeAt(o);return Xe(n,t)}(function(e){return atob(function(e){return e.split(",")[1].replace(/\s/g,"")}(e))}(e),t)},Qe=function(e){if(!/^content-disposition:/i.test(e))return null;var t=e.split(/filename=|filename\*=.+''/).splice(1).map(function(e){return e.trim().replace(/^["']|[;"']{0,2}$/g,"")}).filter(function(e){return e.length});return t.length?decodeURI(t[t.length-1]):null},$e=function(e){if(/content-length:/i.test(e)){var t=e.match(/[0-9]+/)[0];return t?parseInt(t,10):null}return null},Ze=function(e){return/x-content-transfer-id:/i.test(e)&&(e.split(":")[1]||"").trim()||null},Ke=function(e){var t={source:null,name:null,size:null},n=e.split("\n"),r=!0,o=!1,i=void 0;try{for(var a,u=n[Symbol.iterator]();!(r=(a=u.next()).done);r=!0){var s=a.value,l=Qe(s);if(l)t.name=l;else{var c=$e(s);if(c)t.size=c;else{var f=Ze(s);f&&(t.source=f)}}}}catch(e){o=!0,i=e}finally{try{r||null==u.return||u.return()}finally{if(o)throw i}}return t},Je=function(e){var t={source:null,complete:!1,progress:0,size:null,timestamp:null,duration:0,request:null},n=function(n){e?(t.timestamp=Date.now(),t.request=e(n,function(e){t.duration=Date.now()-t.timestamp,t.complete=!0,e instanceof Blob&&(e=He(e,e.name||xe(n))),r.fire("load",e instanceof Blob?e:e?e.body:null)},function(e){r.fire("error","string"==typeof e?{type:"error",code:0,body:e}:e)},function(e,n,o){o&&(t.size=o),t.duration=Date.now()-t.timestamp,e?(t.progress=n/o,r.fire("progress",t.progress)):t.progress=null},function(){r.fire("abort")},function(e){var n=Ke("string"==typeof e?e:e.headers);r.fire("meta",{size:t.size||n.size,filename:n.name,source:n.source})})):r.fire("error",{type:"error",body:"Can't load URL",code:400})},r=Object.assign({},pe(),{setSource:function(e){return t.source=e},getProgress:function(){return t.progress},abort:function(){t.request&&t.request.abort&&t.request.abort()},load:function(){var e,o,i=t.source;r.fire("init",i),i instanceof File?r.fire("load",i):i instanceof Blob?r.fire("load",He(i,i.name)):Fe(i)?r.fire("load",He(ze(i),e,null,o)):n(i)}});return r},et=function(e){return/GET|HEAD/.test(e)},tt=function(e,t,n){var r={onheaders:function(){},onprogress:function(){},onload:function(){},ontimeout:function(){},onerror:function(){},onabort:function(){},abort:function(){o=!0,a.abort()}},o=!1,i=!1;n=Object.assign({method:"POST",headers:{},withCredentials:!1},n),t=encodeURI(t),et(n.method)&&e&&(t=""+t+encodeURIComponent("string"==typeof e?e:JSON.stringify(e)));var a=new XMLHttpRequest;return(et(n.method)?a:a.upload).onprogress=function(e){o||r.onprogress(e.lengthComputable,e.loaded,e.total)},a.onreadystatechange=function(){a.readyState<2||4===a.readyState&&0===a.status||i||(i=!0,r.onheaders(a))},a.onload=function(){a.status>=200&&a.status<300?r.onload(a):r.onerror(a)},a.onerror=function(){return r.onerror(a)},a.onabort=function(){o=!0,r.onabort()},a.ontimeout=function(){return r.ontimeout(a)},a.open(n.method,t,!0),F(n.timeout)&&(a.timeout=n.timeout),Object.keys(n.headers).forEach(function(e){var t=unescape(encodeURIComponent(n.headers[e]));a.setRequestHeader(e,t)}),n.responseType&&(a.responseType=n.responseType),n.withCredentials&&(a.withCredentials=!0),a.send(e),r},nt=function(e,t,n,r){return{type:e,code:t,body:n,headers:r}},rt=function(e){return function(t){e(nt("error",0,"Timeout",t.getAllResponseHeaders()))}},ot=function(e){return/\?/.test(e)},it=function(){for(var e="",t=arguments.length,n=new Array(t),r=0;r<t;r++)n[r]=arguments[r];return n.forEach(function(t){e+=ot(e)&&ot(t)?t.replace(/\?/,"&"):t}),e},at=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:"",t=arguments.length>1?arguments[1]:void 0;if("function"==typeof t)return t;if(!t||!U(t.url))return null;var n=t.onload||function(e){return e},r=t.onerror||function(e){return null};return function(o,i,a,u,s,l){var c=tt(o,it(e,t.url),Object.assign({},t,{responseType:"blob"}));return c.onload=function(e){var r=e.getAllResponseHeaders(),a=Ke(r).name||xe(o);i(nt("load",e.status,"HEAD"===t.method?null:He(n(e.response),a),r))},c.onerror=function(e){a(nt("error",e.status,r(e.response)||e.statusText,e.getAllResponseHeaders()))},c.onheaders=function(e){l(nt("headers",e.status,null,e.getAllResponseHeaders()))},c.ontimeout=rt(a),c.onprogress=u,c.onabort=s,c}},ut=0,st=1,lt=2,ct=3,ft=4,dt=function(e,t,n,r,o,i,a,u,s,l,c){for(var f=[],d=c.chunkTransferId,p=c.chunkServer,E=c.chunkSize,_=c.chunkRetryDelays,T={serverId:d,aborted:!1},I=t.ondata||function(e){return e},v=t.onload||function(e,t){return"HEAD"===t?e.getResponseHeader("Upload-Offset"):e.response},m=t.onerror||function(e){return null},g=Math.floor(r.size/E),h=0;h<=g;h++){var R=h*E,O=r.slice(R,R+E,"application/offset+octet-stream");f[h]={index:h,size:O.size,offset:R,data:O,file:r,progress:0,retries:se(_),status:ut,error:null,request:null,timeout:null}}var y,D,S,A,b=function(e){return e.status===ut||e.status===ct},P=function(t){if(!T.aborted)if(t=t||f.find(b)){t.status=lt,t.progress=null;var n=p.ondata||function(e){return e},o=p.onerror||function(e){return null},u=it(e,p.url,T.serverId),l="function"==typeof p.headers?p.headers(t):Object.assign({},p.headers,{"Content-Type":"application/offset+octet-stream","Upload-Offset":t.offset,"Upload-Length":r.size,"Upload-Name":r.name}),c=t.request=tt(n(t.data),u,Object.assign({},p,{headers:l}));c.onload=function(){t.status=st,t.request=null,w()},c.onprogress=function(e,n,r){t.progress=e?n:null,M()},c.onerror=function(e){t.status=ct,t.request=null,t.error=o(e.response)||e.statusText,L(t)||a(nt("error",e.status,o(e.response)||e.statusText,e.getAllResponseHeaders()))},c.ontimeout=function(e){t.status=ct,t.request=null,L(t)||rt(a)(e)},c.onabort=function(){t.status=ut,t.request=null,s()}}else f.every(function(e){return e.status===st})&&i(T.serverId)},L=function(e){return 0!==e.retries.length&&(e.status=ft,clearTimeout(e.timeout),e.timeout=setTimeout(function(){P(e)},e.retries.shift()),!0)},M=function(){var e=f.reduce(function(e,t){return null===e||null===t.progress?null:e+t.progress},0);if(null===e)return u(!1,0,0);var t=f.reduce(function(e,t){return e+t.size},0);u(!0,e,t)},w=function(){f.filter(function(e){return e.status===lt}).length>=1||P()};return T.serverId?(y=function(e){T.aborted||(f.filter(function(t){return t.offset<e}).forEach(function(e){e.status=st,e.progress=e.size}),w())},D=it(e,p.url,T.serverId),S={headers:"function"==typeof t.headers?t.headers(T.serverId):Object.assign({},t.headers),method:"HEAD"},(A=tt(null,D,S)).onload=function(e){return y(v(e,S.method))},A.onerror=function(e){return a(nt("error",e.status,m(e.response)||e.statusText,e.getAllResponseHeaders()))},A.ontimeout=rt(a)):function(i){var u=new FormData;H(o)&&u.append(n,JSON.stringify(o));var s="function"==typeof t.headers?t.headers(r,o):Object.assign({},t.headers,{"Upload-Length":r.size}),l=Object.assign({},t,{headers:s}),c=tt(I(u),it(e,t.url),l);c.onload=function(e){return i(v(e,l.method))},c.onerror=function(e){return a(nt("error",e.status,m(e.response)||e.statusText,e.getAllResponseHeaders()))},c.ontimeout=rt(a)}(function(e){T.aborted||(l(e),T.serverId=e,w())}),{abort:function(){T.aborted=!0,f.forEach(function(e){clearTimeout(e.timeout),e.request&&e.request.abort()})}}},pt=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:"",t=arguments.length>1?arguments[1]:void 0,n=arguments.length>2?arguments[2]:void 0,r=arguments.length>3?arguments[3]:void 0;return"function"==typeof t?function(){for(var e=arguments.length,o=new Array(e),i=0;i<e;i++)o[i]=arguments[i];return t.apply(void 0,[n].concat(o,[r]))}:t&&U(t.url)?function(e,t,n,r){return function(o,i,a,u,s,l,c){if(o){var f=r.chunkUploads,d=f&&o.size>r.chunkSize,p=f&&(d||r.chunkForce);if(o instanceof Blob&&p)return dt(e,t,n,o,i,a,u,s,l,c,r);var E=t.ondata||function(e){return e},_=t.onload||function(e){return e},T=t.onerror||function(e){return null},I=new FormData;H(i)&&I.append(n,JSON.stringify(i)),(o instanceof Blob?[{name:null,file:o}]:o).forEach(function(e){I.append(n,e.file,null===e.name?e.file.name:""+e.name+e.file.name)});var v=tt(E(I),it(e,t.url),t);return v.onload=function(e){a(nt("load",e.status,_(e.response),e.getAllResponseHeaders()))},v.onerror=function(e){u(nt("error",e.status,T(e.response)||e.statusText,e.getAllResponseHeaders()))},v.ontimeout=rt(u),v.onprogress=s,v.onabort=l,v}}}(e,t,n,r):null},Et=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:"",t=arguments.length>1?arguments[1]:void 0;if("function"==typeof t)return t;if(!t||!U(t.url))return function(e,t){return t()};var n=t.onload||function(e){return e},r=t.onerror||function(e){return null};return function(o,i,a){var u=tt(o,e+t.url,t);return u.onload=function(e){i(nt("load",e.status,n(e.response),e.getAllResponseHeaders()))},u.onerror=function(e){a(nt("error",e.status,r(e.response)||e.statusText,e.getAllResponseHeaders()))},u.ontimeout=rt(a),u}},_t=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:0,t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:1;return e+Math.random()*(t-e)},Tt=function(e){var t={complete:!1,perceivedProgress:0,perceivedPerformanceUpdater:null,progress:null,timestamp:null,perceivedDuration:0,duration:0,request:null,response:null},n=function(){t.request&&(t.perceivedPerformanceUpdater.clear(),t.request.abort&&t.request.abort(),t.complete=!0)},r=Object.assign({},pe(),{process:function(n,o){var i=function(){0!==t.duration&&null!==t.progress&&r.fire("progress",r.getProgress())},a=function(){t.complete=!0,r.fire("load-perceived",t.response.body)};r.fire("start"),t.timestamp=Date.now(),t.perceivedPerformanceUpdater=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:1e3,n=(arguments.length>2&&void 0!==arguments[2]&&arguments[2],arguments.length>3&&void 0!==arguments[3]?arguments[3]:25),r=arguments.length>4&&void 0!==arguments[4]?arguments[4]:250,o=null,i=Date.now();return function a(){var u=Date.now()-i,s=_t(n,r);u+s>t&&(s=u+s-t);var l=u/t;l>=1||document.hidden?e(1):(e(l),o=setTimeout(a,s))}(),{clear:function(){clearTimeout(o)}}}(function(e){t.perceivedProgress=e,t.perceivedDuration=Date.now()-t.timestamp,i(),t.response&&1===t.perceivedProgress&&!t.complete&&a()},_t(750,1500)),t.request=e(n,o,function(e){t.response=H(e)?e:{type:"load",code:200,body:""+e,headers:{}},t.duration=Date.now()-t.timestamp,t.progress=1,r.fire("load",t.response.body),1===t.perceivedProgress&&a()},function(e){t.perceivedPerformanceUpdater.clear(),r.fire("error",H(e)?e:{type:"error",code:0,body:""+e})},function(e,n,r){t.duration=Date.now()-t.timestamp,t.progress=e?n/r:null,i()},function(){t.perceivedPerformanceUpdater.clear(),r.fire("abort",t.response?t.response.body:null)},function(e){r.fire("transfer",e)})},abort:n,getProgress:function(){return t.progress?Math.min(t.progress,t.perceivedProgress):null},getDuration:function(){return Math.min(t.duration,t.perceivedDuration)},reset:function(){n(),t.complete=!1,t.perceivedProgress=0,t.progress=0,t.timestamp=null,t.perceivedDuration=0,t.duration=0,t.request=null,t.response=null}});return r},It=function(e){return e.substr(0,e.lastIndexOf("."))||e},vt=function(e){return!!(e instanceof File||e instanceof Blob&&e.name)},mt=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:null,t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:null,r=arguments.length>2&&void 0!==arguments[2]?arguments[2]:null,o=oe(),i={archived:!1,frozen:!1,released:!1,source:null,file:r,serverFileReference:t,transferId:null,processingAborted:!1,status:t?Ie.PROCESSING_COMPLETE:Ie.INIT,activeLoader:null,activeProcessor:null},a=null,u={},s=function(e){return i.status=e},l=function(e){if(!i.released&&!i.frozen){for(var t=arguments.length,n=new Array(t>1?t-1:0),r=1;r<t;r++)n[r-1]=arguments[r];f.fire.apply(f,[e].concat(n))}},c=function(e,t,n){var r=e.split("."),o=r[0],i=r.pop(),a=u;r.forEach(function(e){return a=a[e]}),JSON.stringify(a[i])!==JSON.stringify(t)&&(a[i]=t,n||l("metadata-update",{key:o,value:u[o]}))},f=Object.assign({id:{get:function(){return o}},origin:{get:function(){return e}},serverId:{get:function(){return i.serverFileReference}},transferId:{get:function(){return i.transferId}},status:{get:function(){return i.status}},filename:{get:function(){return i.file.name}},filenameWithoutExtension:{get:function(){return It(i.file.name)}},fileExtension:{get:function(){return Ye(i.file.name)}},fileType:{get:function(){return i.file.type}},fileSize:{get:function(){return i.file.size}},file:{get:function(){return i.file}},relativePath:{get:function(){return i.file._relativePath}},source:{get:function(){return i.source}},getMetadata:function(e){return function e(t){if(!H(t))return t;var n=L(t)?[]:{};for(var r in t)if(t.hasOwnProperty(r)){var o=t[r];n[r]=o&&H(o)?e(o):o}return n}(e?u[e]:u)},setMetadata:function(e,t,n){if(H(e)){var r=e;return Object.keys(r).forEach(function(e){c(e,r[e],t)}),e}return c(e,t,n),t},extend:function(e,t){return d[e]=t},abortLoad:function(){i.activeLoader?i.activeLoader.abort():(s(Ie.INIT),l("load-abort"))},retryLoad:function(){i.activeLoader&&i.activeLoader.load()},requestProcessing:function(){i.processingAborted=!1,s(Ie.PROCESSING_QUEUED)},abortProcessing:function(){return new Promise(function(e){if(!i.activeProcessor)return i.processingAborted=!0,s(Ie.IDLE),l("process-abort"),void e();a=function(){e()},i.activeProcessor.abort()})},load:function(t,n,r){i.source=t,f.fireSync("init"),i.file?f.fireSync("load-skip"):(i.file=function(e){var t=[e.name,e.size,e.type];return e instanceof Blob||Fe(e)?t[0]=e.name||ke():Fe(e)?(t[1]=e.length,t[2]=We(e)):U(e)&&(t[0]=xe(e),t[1]=0,t[2]="application/octet-stream"),{name:t[0],size:t[1],type:t[2]}}(t),n.on("init",function(){l("load-init")}),n.on("meta",function(t){i.file.size=t.size,i.file.filename=t.filename,t.source&&(e=ve.LIMBO,i.serverFileReference=t.source,i.status=Ie.PROCESSING_COMPLETE),l("load-meta")}),n.on("progress",function(e){s(Ie.LOADING),l("load-progress",e)}),n.on("error",function(e){s(Ie.LOAD_ERROR),l("load-request-error",e)}),n.on("abort",function(){s(Ie.INIT),l("load-abort")}),n.on("load",function(t){i.activeLoader=null;var n=function(t){i.file=vt(t)?t:i.file,e===ve.LIMBO&&i.serverFileReference?s(Ie.PROCESSING_COMPLETE):s(Ie.IDLE),l("load")};i.serverFileReference?n(t):r(t,n,function(e){i.file=t,l("load-meta"),s(Ie.LOAD_ERROR),l("load-file-error",e)})}),n.setSource(t),i.activeLoader=n,n.load())},process:function e(t,n){if(i.processingAborted)i.processingAborted=!1;else if(s(Ie.PROCESSING),a=null,i.file instanceof Blob){t.on("load",function(e){i.transferId=null,i.serverFileReference=e}),t.on("transfer",function(e){i.transferId=e}),t.on("load-perceived",function(e){i.activeProcessor=null,i.transferId=null,i.serverFileReference=e,s(Ie.PROCESSING_COMPLETE),l("process-complete",e)}),t.on("start",function(){l("process-start")}),t.on("error",function(e){i.activeProcessor=null,s(Ie.PROCESSING_ERROR),l("process-error",e)}),t.on("abort",function(e){i.activeProcessor=null,i.transferId=null,i.serverFileReference=e,s(Ie.IDLE),l("process-abort"),a&&a()}),t.on("progress",function(e){l("process-progress",e)});var r=console.error;n(i.file,function(e){i.archived||t.process(e,Object.assign({},u))},r),i.activeProcessor=t}else f.on("load",function(){e(t,n)})},revert:function(e,t){return new Promise(function(n,r){null!==i.serverFileReference?(e(i.serverFileReference,function(){i.serverFileReference=null,n()},function(e){t?(s(Ie.PROCESSING_REVERT_ERROR),l("process-revert-error"),r(e)):n()}),s(Ie.IDLE),l("process-revert")):n()})}},pe(),{freeze:function(){return i.frozen=!0},release:function(){return i.released=!0},released:{get:function(){return i.released}},archive:function(){return i.archived=!0},archived:{get:function(){return i.archived}}}),d=n(f);return d},gt=function(e,t){var n=function(e,t){return M(t)?0:U(t)?e.findIndex(function(e){return e.id===t}):-1}(e,t);if(!(n<0))return e[n]||null},ht=function(e,t,n,r,o,i){var a=tt(null,e,{method:"GET",responseType:"blob"});return a.onload=function(n){var r=n.getAllResponseHeaders(),o=Ke(r).name||xe(e);t(nt("load",n.status,He(n.response,o),r))},a.onerror=function(e){n(nt("error",e.status,e.statusText,e.getAllResponseHeaders()))},a.onheaders=function(e){i(nt("headers",e.status,null,e.getAllResponseHeaders()))},a.ontimeout=rt(n),a.onprogress=r,a.onabort=o,a},Rt=function(e){return 0===e.indexOf("//")&&(e=location.protocol+e),e.toLowerCase().replace("blob:","").replace(/([a-z])?:\/\//,"$1").split("/")[0]},Ot=function(e){return function(){return Y(e)?e.apply(void 0,arguments):e}},yt=function(e,t){clearTimeout(t.listUpdateTimeout),t.listUpdateTimeout=setTimeout(function(){e("DID_UPDATE_ITEMS",{items:Le(t.items)})},0)},Dt=function(e){for(var t=arguments.length,n=new Array(t>1?t-1:0),r=1;r<t;r++)n[r-1]=arguments[r];return new Promise(function(t){if(!e)return t(!0);var r=e.apply(void 0,n);return null==r?t(!0):"boolean"==typeof r?t(r):void("function"==typeof r.then&&r.then(t))})},St=function(e,t){e.items.sort(function(e,n){return t(Te(e),Te(n))})},At=function(e,t){return function(){var n=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},r=n.query,o=n.success,i=void 0===o?function(){}:o,a=n.failure,u=void 0===a?function(){}:a,s=ue(n,["query","success","failure"]),l=be(e.items,r);l?t(l,i,u,s||{}):u({error:nt("error",0,"Item not found"),file:null})}},bt=function(e,n,r){return{ABORT_ALL:function(){Le(r.items).forEach(function(e){e.freeze(),e.abortLoad(),e.abortProcessing()})},DID_SET_FILES:function(t){var n=t.value,o=(void 0===n?[]:n).map(function(e){return{source:e.source?e.source:e,options:e.options}}),i=Le(r.items);i.forEach(function(t){o.find(function(e){return e.source===t.source||e.source===t.file})||e("REMOVE_ITEM",{query:t,remove:!1})}),i=Le(r.items),o.forEach(function(t,n){i.find(function(e){return e.source===t.source||e.file===t.source})||e("ADD_ITEM",Object.assign({},t,{interactionMethod:re,index:n}))})},DID_UPDATE_ITEM_METADATA:function(t){var o=t.id;clearTimeout(r.itemUpdateTimeout),r.itemUpdateTimeout=setTimeout(function(){var t=gt(r.items,o);if(n("IS_ASYNC")){var i,a=function(){setTimeout(function(){e("REQUEST_ITEM_PROCESSING",{query:o})},32)};return t.status===Ie.PROCESSING_COMPLETE?(i=r.options.instantUpload,void t.revert(Et(r.options.server.url,r.options.server.revert),n("GET_FORCE_REVERT")).then(i?a:function(){}).catch(function(){})):t.status===Ie.PROCESSING?function(e){t.abortProcessing().then(e?a:function(){})}(r.options.instantUpload):void(r.options.instantUpload&&a())}Oe("SHOULD_PREPARE_OUTPUT",!1,{item:t,query:n}).then(function(n){n&&e("REQUEST_PREPARE_OUTPUT",{query:o,item:t,success:function(t){e("DID_PREPARE_OUTPUT",{id:o,file:t})}},!0)})},0)},MOVE_ITEM:function(e){var t=e.query,n=e.index,o=be(r.items,t);if(o){var i=r.items.indexOf(o);i!==(n=Ve(n,0,r.items.length-1))&&r.items.splice(n,0,r.items.splice(i,1)[0])}},SORT:function(t){var o=t.compare;St(r,o),e("DID_SORT_ITEMS",{items:n("GET_ACTIVE_ITEMS")})},ADD_ITEMS:function(t){var r=t.items,o=t.index,i=t.interactionMethod,a=t.success,u=void 0===a?function(){}:a,s=t.failure,l=void 0===s?function(){}:s,c=o;if(-1===o||void 0===o){var f=n("GET_ITEM_INSERT_LOCATION"),d=n("GET_TOTAL_ITEMS");c="before"===f?0:d}var p=n("GET_IGNORED_FILES"),E=r.filter(function(e){return vt(e)?!p.includes(e.name.toLowerCase()):!M(e)}).map(function(t){return new Promise(function(n,r){e("ADD_ITEM",{interactionMethod:i,source:t.source||t,success:n,failure:r,index:c++,options:t.options||{}})})});Promise.all(E).then(u).catch(l)},ADD_ITEM:function(t){var o=t.source,i=t.index,a=void 0===i?-1:i,u=t.interactionMethod,s=t.success,l=void 0===s?function(){}:s,c=t.failure,f=void 0===c?function(){}:c,d=t.options,p=void 0===d?{}:d;if(M(o))f({error:nt("error",0,"No source"),file:null});else if(!vt(o)||!r.options.ignoredFiles.includes(o.name.toLowerCase())){if(!function(e){var t=Le(e.items).length;if(!e.options.allowMultiple)return 0===t;var n=e.options.maxFiles;return null===n||t<n}(r)){if(r.options.allowMultiple||!r.options.allowMultiple&&!r.options.allowReplace){var E=nt("warning",0,"Max files");return e("DID_THROW_MAX_FILES",{source:o,error:E}),void f({error:E,file:null})}var _=Le(r.items)[0];if(_.status===Ie.PROCESSING_COMPLETE||_.status===Ie.PROCESSING_REVERT_ERROR){var T=n("GET_FORCE_REVERT");if(_.revert(Et(r.options.server.url,r.options.server.revert),T).then(function(){T&&e("ADD_ITEM",{source:o,index:a,interactionMethod:u,success:l,failure:f,options:p})}).catch(function(){}),T)return}e("REMOVE_ITEM",{query:_.id})}var I="local"===p.type?ve.LOCAL:"limbo"===p.type?ve.LIMBO:ve.INPUT,v=mt(I,I===ve.INPUT?null:o,p.file);Object.keys(p.metadata||{}).forEach(function(e){v.setMetadata(e,p.metadata[e])}),ye("DID_CREATE_ITEM",v,{query:n,dispatch:e});var m=n("GET_ITEM_INSERT_LOCATION");r.options.itemInsertLocationFreedom||(a="before"===m?-1:r.items.length),function(e,t,n){M(t)||(void 0===n?e.push(t):function(e,t,n){e.splice(t,0,n)}(e,n=Ve(n,0,e.length),t))}(r.items,v,a),Y(m)&&o&&St(r,m);var g=v.id;v.on("init",function(){e("DID_INIT_ITEM",{id:g})}),v.on("load-init",function(){e("DID_START_ITEM_LOAD",{id:g})}),v.on("load-meta",function(){e("DID_UPDATE_ITEM_META",{id:g})}),v.on("load-progress",function(t){e("DID_UPDATE_ITEM_LOAD_PROGRESS",{id:g,progress:t})}),v.on("load-request-error",function(t){var n=Ot(r.options.labelFileLoadError)(t);if(t.code>=400&&t.code<500)return e("DID_THROW_ITEM_INVALID",{id:g,error:t,status:{main:n,sub:t.code+" ("+t.body+")"}}),void f({error:t,file:Te(v)});e("DID_THROW_ITEM_LOAD_ERROR",{id:g,error:t,status:{main:n,sub:r.options.labelTapToRetry}})}),v.on("load-file-error",function(t){e("DID_THROW_ITEM_INVALID",{id:g,error:t.status,status:t.status}),f({error:t.status,file:Te(v)})}),v.on("load-abort",function(){e("REMOVE_ITEM",{query:g})}),v.on("load-skip",function(){e("COMPLETE_LOAD_ITEM",{query:g,item:v,data:{source:o,success:l}})}),v.on("load",function(){var t=function(t){t?(v.on("metadata-update",function(t){e("DID_UPDATE_ITEM_METADATA",{id:g,change:t})}),Oe("SHOULD_PREPARE_OUTPUT",!1,{item:v,query:n}).then(function(t){var n=function(){e("COMPLETE_LOAD_ITEM",{query:g,item:v,data:{source:o,success:l}}),yt(e,r)};t?e("REQUEST_PREPARE_OUTPUT",{query:g,item:v,success:function(t){e("DID_PREPARE_OUTPUT",{id:g,file:t}),n()}},!0):n()})):e("REMOVE_ITEM",{query:g})};Oe("DID_LOAD_ITEM",v,{query:n,dispatch:e}).then(function(){Dt(n("GET_BEFORE_ADD_FILE"),Te(v)).then(t)}).catch(function(){t(!1)})}),v.on("process-start",function(){e("DID_START_ITEM_PROCESSING",{id:g})}),v.on("process-progress",function(t){e("DID_UPDATE_ITEM_PROCESS_PROGRESS",{id:g,progress:t})}),v.on("process-error",function(t){e("DID_THROW_ITEM_PROCESSING_ERROR",{id:g,error:t,status:{main:Ot(r.options.labelFileProcessingError)(t),sub:r.options.labelTapToRetry}})}),v.on("process-revert-error",function(t){e("DID_THROW_ITEM_PROCESSING_REVERT_ERROR",{id:g,error:t,status:{main:Ot(r.options.labelFileProcessingRevertError)(t),sub:r.options.labelTapToRetry}})}),v.on("process-complete",function(t){e("DID_COMPLETE_ITEM_PROCESSING",{id:g,error:null,serverFileReference:t}),e("DID_DEFINE_VALUE",{id:g,value:t})}),v.on("process-abort",function(){e("DID_ABORT_ITEM_PROCESSING",{id:g})}),v.on("process-revert",function(){e("DID_REVERT_ITEM_PROCESSING",{id:g}),e("DID_DEFINE_VALUE",{id:g,value:null})}),e("DID_ADD_ITEM",{id:g,index:a,interactionMethod:u}),yt(e,r);var h=r.options.server||{},R=h.url,O=h.load,y=h.restore,D=h.fetch;v.load(o,Je(I===ve.INPUT?U(o)&&function(e){return(e.indexOf(":")>-1||e.indexOf("//")>-1)&&Rt(location.href)!==Rt(e)}(o)&&D?at(R,D):ht:at(R,I===ve.LIMBO?y:O)),function(e,t,r){Oe("LOAD_FILE",e,{query:n}).then(t).catch(r)})}},REQUEST_PREPARE_OUTPUT:function(e){var t=e.item,r=e.success,o=e.failure,i=void 0===o?function(){}:o,a={error:nt("error",0,"Item not found"),file:null};if(t.archived)return i(a);Oe("PREPARE_OUTPUT",t.file,{query:n,item:t}).then(function(e){Oe("COMPLETE_PREPARE_OUTPUT",e,{query:n,item:t}).then(function(e){if(t.archived)return i(a);r(e)})})},COMPLETE_LOAD_ITEM:function(t){var o=t.item,i=t.data,a=i.success,u=i.source,s=n("GET_ITEM_INSERT_LOCATION");if(Y(s)&&u&&St(r,s),e("DID_LOAD_ITEM",{id:o.id,error:null,serverFileReference:o.origin===ve.INPUT?null:u}),a(Te(o)),o.origin!==ve.LOCAL)return o.origin===ve.LIMBO?(e("DID_COMPLETE_ITEM_PROCESSING",{id:o.id,error:null,serverFileReference:u}),void e("DID_DEFINE_VALUE",{id:o.id,value:u})):void(n("IS_ASYNC")&&r.options.instantUpload&&e("REQUEST_ITEM_PROCESSING",{query:o.id}));e("DID_LOAD_LOCAL_ITEM",{id:o.id})},RETRY_ITEM_LOAD:At(r,function(e){e.retryLoad()}),REQUEST_ITEM_PREPARE:At(r,function(t,n,r){e("REQUEST_PREPARE_OUTPUT",{query:t.id,item:t,success:function(r){e("DID_PREPARE_OUTPUT",{id:t.id,file:r}),n({file:t,output:r})},failure:r},!0)}),REQUEST_ITEM_PROCESSING:At(r,function(t,o,i){if(t.status===Ie.IDLE||t.status===Ie.PROCESSING_ERROR)t.status!==Ie.PROCESSING_QUEUED&&(t.requestProcessing(),e("DID_REQUEST_ITEM_PROCESSING",{id:t.id}),e("PROCESS_ITEM",{query:t,success:o,failure:i},!0));else{var a=function(){return e("REQUEST_ITEM_PROCESSING",{query:t,success:o,failure:i})},u=function(){return document.hidden?a():setTimeout(a,32)};t.status===Ie.PROCESSING_COMPLETE||t.status===Ie.PROCESSING_REVERT_ERROR?t.revert(Et(r.options.server.url,r.options.server.revert),n("GET_FORCE_REVERT")).then(u).catch(function(){}):t.status===Ie.PROCESSING&&t.abortProcessing().then(u)}}),PROCESS_ITEM:At(r,function(t,o,i){var a=n("GET_MAX_PARALLEL_UPLOADS");if(n("GET_ITEMS_BY_STATUS",Ie.PROCESSING).length!==a){if(t.status!==Ie.PROCESSING){var u=function t(){var n=r.processingQueue.shift();if(n){var o=n.id,i=n.success,a=n.failure,u=be(r.items,o);u&&!u.archived?e("PROCESS_ITEM",{query:o,success:i,failure:a},!0):t()}};t.onOnce("process-complete",function(){o(Te(t)),u(),n("GET_ITEMS_BY_STATUS",Ie.PROCESSING_COMPLETE).length===r.items.length&&e("DID_COMPLETE_ITEM_PROCESSING_ALL")}),t.onOnce("process-error",function(e){i({error:e,file:Te(t)}),u()});var s=r.options;t.process(Tt(pt(s.server.url,s.server.process,s.name,{chunkTransferId:t.transferId,chunkServer:s.server.patch,chunkUploads:s.chunkUploads,chunkForce:s.chunkForce,chunkSize:s.chunkSize,chunkRetryDelays:s.chunkRetryDelays})),function(r,o,i){Oe("PREPARE_OUTPUT",r,{query:n,item:t}).then(function(n){e("DID_PREPARE_OUTPUT",{id:t.id,file:n}),o(n)}).catch(i)})}}else r.processingQueue.push({id:t.id,success:o,failure:i})}),RETRY_ITEM_PROCESSING:At(r,function(t){e("REQUEST_ITEM_PROCESSING",{query:t})}),REQUEST_REMOVE_ITEM:At(r,function(t){Dt(n("GET_BEFORE_REMOVE_FILE"),Te(t)).then(function(n){n&&e("REMOVE_ITEM",{query:t})})}),RELEASE_ITEM:At(r,function(e){e.release()}),REMOVE_ITEM:At(r,function(t,o,i,a){var u=function(){var n=t.id;gt(r.items,n).archive(),e("DID_REMOVE_ITEM",{error:null,id:n,item:t}),yt(e,r),o(Te(t))},s=r.options.server;t.origin===ve.LOCAL&&s&&Y(s.remove)&&!1!==a.remove?(e("DID_START_ITEM_REMOVE",{id:t.id}),s.remove(t.source,function(){return u()},function(n){e("DID_THROW_ITEM_REMOVE_ERROR",{id:t.id,error:nt("error",0,n,null),status:{main:Ot(r.options.labelFileRemoveError)(n),sub:r.options.labelTapToRetry}})})):(a.revert&&t.origin!==ve.LOCAL&&null!==t.serverId&&t.revert(Et(r.options.server.url,r.options.server.revert),n("GET_FORCE_REVERT")),u())}),ABORT_ITEM_LOAD:At(r,function(e){e.abortLoad()}),ABORT_ITEM_PROCESSING:At(r,function(t){t.serverId?e("REVERT_ITEM_PROCESSING",{id:t.id}):t.abortProcessing().then(function(){r.options.instantUpload&&e("REMOVE_ITEM",{query:t.id})})}),REQUEST_REVERT_ITEM_PROCESSING:At(r,function(t){if(r.options.instantUpload){var o=function(n){n&&e("REVERT_ITEM_PROCESSING",{query:t})},i=n("GET_BEFORE_REMOVE_FILE");if(!i)return o(!0);var a=i(Te(t));return null==a?o(!0):"boolean"==typeof a?o(a):void("function"==typeof a.then&&a.then(o))}e("REVERT_ITEM_PROCESSING",{query:t})}),REVERT_ITEM_PROCESSING:At(r,function(t){t.revert(Et(r.options.server.url,r.options.server.revert),n("GET_FORCE_REVERT")).then(function(){(r.options.instantUpload||function(e){return!vt(e.file)}(t))&&e("REMOVE_ITEM",{query:t.id})}).catch(function(){})}),SET_OPTIONS:function(n){var r=n.options;t(r,function(t,n){e("SET_"+$(t,"_").toUpperCase(),{value:n})})}}},Pt=function(e){return e},Lt=function(e){return document.createElement(e)},Mt=function(e,t){var n=e.childNodes[0];n?t!==n.nodeValue&&(n.nodeValue=t):(n=document.createTextNode(t),e.appendChild(n))},wt=function(e,t,n,r){var o=(r%360-90)*Math.PI/180;return{x:e+n*Math.cos(o),y:t+n*Math.sin(o)}},Ct=function(e,t,n,r,o){var i=1;return o>r&&o-r<=.5&&(i=0),r>o&&r-o>=.5&&(i=0),function(e,t,n,r,o,i){var a=wt(e,t,n,o),u=wt(e,t,n,r);return["M",a.x,a.y,"A",n,n,0,i,0,u.x,u.y].join(" ")}(e,t,n,360*Math.min(.9999,r),360*Math.min(.9999,o),i)},Nt=S({tag:"div",name:"progress-indicator",ignoreRectUpdate:!0,ignoreRect:!0,create:function(e){var t=e.root,n=e.props;n.spin=!1,n.progress=0,n.opacity=0;var r=a("svg");t.ref.path=a("path",{"stroke-width":2,"stroke-linecap":"round"}),r.appendChild(t.ref.path),t.ref.svg=r,t.appendChild(r)},write:function(e){var t=e.root,n=e.props;if(0!==n.opacity){n.align&&(t.element.dataset.align=n.align);var o=parseInt(r(t.ref.path,"stroke-width"),10),i=.5*t.rect.element.width,a=0,u=0;n.spin?(a=0,u=.5):(a=0,u=n.progress);var s=Ct(i,i,i-o,a,u);r(t.ref.path,"d",s),r(t.ref.path,"stroke-opacity",n.spin||n.progress>0?1:0)}},mixins:{apis:["progress","spin","align"],styles:["opacity"],animations:{opacity:{type:"tween",duration:500},progress:{type:"spring",stiffness:.95,damping:.65,mass:10}}}}),Gt=S({tag:"button",attributes:{type:"button"},ignoreRect:!0,ignoreRectUpdate:!0,name:"file-action-button",mixins:{apis:["label"],styles:["translateX","translateY","scaleX","scaleY","opacity"],animations:{scaleX:"spring",scaleY:"spring",translateX:"spring",translateY:"spring",opacity:{type:"tween",duration:250}},listeners:!0},create:function(e){var t=e.root,n=e.props;t.element.innerHTML=(n.icon||"")+"<span>"+n.label+"</span>",n.isDisabled=!1},write:function(e){var t=e.root,n=e.props,o=n.isDisabled,i=t.query("GET_DISABLED")||0===n.opacity;i&&!o?(n.isDisabled=!0,r(t.element,"disabled","disabled")):!i&&o&&(n.isDisabled=!1,t.element.removeAttribute("disabled"))}}),Ut=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:".";return(e=Math.round(Math.abs(e)))<1e3?e+" bytes":e<qt?Math.floor(e/Bt)+" KB":e<Vt?Ft(e/qt,1,t)+" MB":Ft(e/Vt,2,t)+" GB"},Bt=1e3,qt=1e6,Vt=1e9,Ft=function(e,t,n){return e.toFixed(t).split(".").filter(function(e){return"0"!==e}).join(n)},xt=function(e){var t=e.root,n=e.props;Mt(t.ref.fileSize,Ut(t.query("GET_ITEM_SIZE",n.id))),Mt(t.ref.fileName,Pt(t.query("GET_ITEM_NAME",n.id)))},Yt=function(e){var t=e.root,n=e.props;F(t.query("GET_ITEM_SIZE",n.id))||Mt(t.ref.fileSize,t.query("GET_LABEL_FILE_SIZE_NOT_AVAILABLE"))},jt=S({name:"file-info",ignoreRect:!0,ignoreRectUpdate:!0,write:A({DID_LOAD_ITEM:xt,DID_UPDATE_ITEM_META:xt,DID_THROW_ITEM_LOAD_ERROR:Yt,DID_THROW_ITEM_INVALID:Yt}),didCreateView:function(e){ye("CREATE_VIEW",Object.assign({},e,{view:e}))},create:function(e){var t=e.root,n=e.props,o=Lt("span");o.className="filepond--file-info-main",r(o,"aria-hidden","true"),t.appendChild(o),t.ref.fileName=o;var i=Lt("span");i.className="filepond--file-info-sub",t.appendChild(i),t.ref.fileSize=i,Mt(i,t.query("GET_LABEL_FILE_WAITING_FOR_SIZE")),Mt(o,Pt(t.query("GET_ITEM_NAME",n.id)))},mixins:{styles:["translateX","translateY"],animations:{translateX:"spring",translateY:"spring"}}}),kt=function(e){return Math.round(100*e)},Ht=function(e){var t=e.root,n=e.action,r=null===n.progress?t.query("GET_LABEL_FILE_LOADING"):t.query("GET_LABEL_FILE_LOADING")+" "+kt(n.progress)+"%";Mt(t.ref.main,r),Mt(t.ref.sub,t.query("GET_LABEL_TAP_TO_CANCEL"))},Xt=function(e){var t=e.root;Mt(t.ref.main,""),Mt(t.ref.sub,"")},Wt=function(e){var t=e.root,n=e.action;Mt(t.ref.main,n.status.main),Mt(t.ref.sub,n.status.sub)},zt=S({name:"file-status",ignoreRect:!0,ignoreRectUpdate:!0,write:A({DID_LOAD_ITEM:Xt,DID_REVERT_ITEM_PROCESSING:Xt,DID_REQUEST_ITEM_PROCESSING:function(e){var t=e.root;Mt(t.ref.main,t.query("GET_LABEL_FILE_PROCESSING")),Mt(t.ref.sub,t.query("GET_LABEL_TAP_TO_CANCEL"))},DID_ABORT_ITEM_PROCESSING:function(e){var t=e.root;Mt(t.ref.main,t.query("GET_LABEL_FILE_PROCESSING_ABORTED")),Mt(t.ref.sub,t.query("GET_LABEL_TAP_TO_RETRY"))},DID_COMPLETE_ITEM_PROCESSING:function(e){var t=e.root;Mt(t.ref.main,t.query("GET_LABEL_FILE_PROCESSING_COMPLETE")),Mt(t.ref.sub,t.query("GET_LABEL_TAP_TO_UNDO"))},DID_UPDATE_ITEM_PROCESS_PROGRESS:function(e){var t=e.root,n=e.action,r=null===n.progress?t.query("GET_LABEL_FILE_PROCESSING"):t.query("GET_LABEL_FILE_PROCESSING")+" "+kt(n.progress)+"%";Mt(t.ref.main,r),Mt(t.ref.sub,t.query("GET_LABEL_TAP_TO_CANCEL"))},DID_UPDATE_ITEM_LOAD_PROGRESS:Ht,DID_THROW_ITEM_LOAD_ERROR:Wt,DID_THROW_ITEM_INVALID:Wt,DID_THROW_ITEM_PROCESSING_ERROR:Wt,DID_THROW_ITEM_PROCESSING_REVERT_ERROR:Wt,DID_THROW_ITEM_REMOVE_ERROR:Wt}),didCreateView:function(e){ye("CREATE_VIEW",Object.assign({},e,{view:e}))},create:function(e){var t=e.root,n=Lt("span");n.className="filepond--file-status-main",t.appendChild(n),t.ref.main=n;var r=Lt("span");r.className="filepond--file-status-sub",t.appendChild(r),t.ref.sub=r,Ht({root:t,action:{progress:null}})},mixins:{styles:["translateX","translateY","opacity"],animations:{opacity:{type:"tween",duration:250},translateX:"spring",translateY:"spring"}}}),Qt={AbortItemLoad:{label:"GET_LABEL_BUTTON_ABORT_ITEM_LOAD",action:"ABORT_ITEM_LOAD",className:"filepond--action-abort-item-load",align:"LOAD_INDICATOR_POSITION"},RetryItemLoad:{label:"GET_LABEL_BUTTON_RETRY_ITEM_LOAD",action:"RETRY_ITEM_LOAD",icon:"GET_ICON_RETRY",className:"filepond--action-retry-item-load",align:"BUTTON_PROCESS_ITEM_POSITION"},RemoveItem:{label:"GET_LABEL_BUTTON_REMOVE_ITEM",action:"REQUEST_REMOVE_ITEM",icon:"GET_ICON_REMOVE",className:"filepond--action-remove-item",align:"BUTTON_REMOVE_ITEM_POSITION"},ProcessItem:{label:"GET_LABEL_BUTTON_PROCESS_ITEM",action:"REQUEST_ITEM_PROCESSING",icon:"GET_ICON_PROCESS",className:"filepond--action-process-item",align:"BUTTON_PROCESS_ITEM_POSITION"},AbortItemProcessing:{label:"GET_LABEL_BUTTON_ABORT_ITEM_PROCESSING",action:"ABORT_ITEM_PROCESSING",className:"filepond--action-abort-item-processing",align:"BUTTON_PROCESS_ITEM_POSITION"},RetryItemProcessing:{label:"GET_LABEL_BUTTON_RETRY_ITEM_PROCESSING",action:"RETRY_ITEM_PROCESSING",icon:"GET_ICON_RETRY",className:"filepond--action-retry-item-processing",align:"BUTTON_PROCESS_ITEM_POSITION"},RevertItemProcessing:{label:"GET_LABEL_BUTTON_UNDO_ITEM_PROCESSING",action:"REQUEST_REVERT_ITEM_PROCESSING",icon:"GET_ICON_UNDO",className:"filepond--action-revert-item-processing",align:"BUTTON_PROCESS_ITEM_POSITION"}},$t=[];t(Qt,function(e){$t.push(e)});var Zt,Kt=function(e){var t=e.ref.buttonRemoveItem.rect.element;return t.hidden?null:t.width+t.left},Jt=function(e){return e.ref.buttonAbortItemLoad.rect.element.width},en=function(e){return Math.floor(e.ref.buttonRemoveItem.rect.element.height/4)},tn=function(e){return Math.floor(e.ref.buttonRemoveItem.rect.element.left/2)},nn=function(e){return e.query("GET_STYLE_BUTTON_REMOVE_ITEM_POSITION")},rn={buttonAbortItemLoad:{opacity:0},buttonRetryItemLoad:{opacity:0},buttonRemoveItem:{opacity:0},buttonProcessItem:{opacity:0},buttonAbortItemProcessing:{opacity:0},buttonRetryItemProcessing:{opacity:0},buttonRevertItemProcessing:{opacity:0},loadProgressIndicator:{opacity:0,align:function(e){return e.query("GET_STYLE_LOAD_INDICATOR_POSITION")}},processProgressIndicator:{opacity:0,align:function(e){return e.query("GET_STYLE_PROGRESS_INDICATOR_POSITION")}},processingCompleteIndicator:{opacity:0,scaleX:.75,scaleY:.75},info:{translateX:0,translateY:0,opacity:0},status:{translateX:0,translateY:0,opacity:0}},on={buttonRemoveItem:{opacity:1},buttonProcessItem:{opacity:1},info:{translateX:Kt},status:{translateX:Kt}},an={buttonAbortItemProcessing:{opacity:1},processProgressIndicator:{opacity:1},status:{opacity:1}},un={DID_THROW_ITEM_INVALID:{buttonRemoveItem:{opacity:1},info:{translateX:Kt},status:{translateX:Kt,opacity:1}},DID_START_ITEM_LOAD:{buttonAbortItemLoad:{opacity:1},loadProgressIndicator:{opacity:1},status:{opacity:1}},DID_THROW_ITEM_LOAD_ERROR:{buttonRetryItemLoad:{opacity:1},buttonRemoveItem:{opacity:1},info:{translateX:Kt},status:{opacity:1}},DID_START_ITEM_REMOVE:{processProgressIndicator:{opacity:1,align:nn},info:{translateX:Kt},status:{opacity:0}},DID_THROW_ITEM_REMOVE_ERROR:{processProgressIndicator:{opacity:0,align:nn},buttonRemoveItem:{opacity:1},info:{translateX:Kt},status:{opacity:1,translateX:Kt}},DID_LOAD_ITEM:on,DID_LOAD_LOCAL_ITEM:{buttonRemoveItem:{opacity:1},info:{translateX:Kt},status:{translateX:Kt}},DID_START_ITEM_PROCESSING:an,DID_REQUEST_ITEM_PROCESSING:an,DID_UPDATE_ITEM_PROCESS_PROGRESS:an,DID_COMPLETE_ITEM_PROCESSING:{buttonRevertItemProcessing:{opacity:1},info:{opacity:1},status:{opacity:1}},DID_THROW_ITEM_PROCESSING_ERROR:{buttonRemoveItem:{opacity:1},buttonRetryItemProcessing:{opacity:1},status:{opacity:1},info:{translateX:Kt}},DID_THROW_ITEM_PROCESSING_REVERT_ERROR:{buttonRevertItemProcessing:{opacity:1},status:{opacity:1},info:{opacity:1}},DID_ABORT_ITEM_PROCESSING:{buttonRemoveItem:{opacity:1},buttonProcessItem:{opacity:1},info:{translateX:Kt},status:{opacity:1}},DID_REVERT_ITEM_PROCESSING:on},sn=S({create:function(e){var t=e.root;t.element.innerHTML=t.query("GET_ICON_DONE")},name:"processing-complete-indicator",ignoreRect:!0,mixins:{styles:["scaleX","scaleY","opacity"],animations:{scaleX:"spring",scaleY:"spring",opacity:{type:"tween",duration:250}}}}),ln=A({DID_SET_LABEL_BUTTON_ABORT_ITEM_PROCESSING:function(e){var t=e.root,n=e.action;t.ref.buttonAbortItemProcessing.label=n.value},DID_SET_LABEL_BUTTON_ABORT_ITEM_LOAD:function(e){var t=e.root,n=e.action;t.ref.buttonAbortItemLoad.label=n.value},DID_SET_LABEL_BUTTON_ABORT_ITEM_REMOVAL:function(e){var t=e.root,n=e.action;t.ref.buttonAbortItemRemoval.label=n.value},DID_REQUEST_ITEM_PROCESSING:function(e){var t=e.root;t.ref.processProgressIndicator.spin=!0,t.ref.processProgressIndicator.progress=0},DID_START_ITEM_LOAD:function(e){var t=e.root;t.ref.loadProgressIndicator.spin=!0,t.ref.loadProgressIndicator.progress=0},DID_START_ITEM_REMOVE:function(e){var t=e.root;t.ref.processProgressIndicator.spin=!0,t.ref.processProgressIndicator.progress=0},DID_UPDATE_ITEM_LOAD_PROGRESS:function(e){var t=e.root,n=e.action;t.ref.loadProgressIndicator.spin=!1,t.ref.loadProgressIndicator.progress=n.progress},DID_UPDATE_ITEM_PROCESS_PROGRESS:function(e){var t=e.root,n=e.action;t.ref.processProgressIndicator.spin=!1,t.ref.processProgressIndicator.progress=n.progress}}),cn=S({create:function(e){var n,r=e.root,o=e.props.id,i=r.query("GET_ALLOW_REVERT"),a=r.query("GET_ALLOW_REMOVE"),u=r.query("GET_ALLOW_PROCESS"),s=r.query("GET_INSTANT_UPLOAD"),l=r.query("IS_ASYNC"),c=r.query("GET_STYLE_BUTTON_REMOVE_ITEM_ALIGN");l?u&&!i?n=function(e){return!/RevertItemProcessing/.test(e)}:!u&&i?n=function(e){return!/ProcessItem|RetryItemProcessing|AbortItemProcessing/.test(e)}:u||i||(n=function(e){return!/Process/.test(e)}):n=function(e){return!/Process/.test(e)};var f=n?$t.filter(n):$t.concat();if(s&&i&&(Qt.RevertItemProcessing.label="GET_LABEL_BUTTON_REMOVE_ITEM",Qt.RevertItemProcessing.icon="GET_ICON_REMOVE"),l&&!i){var d=un.DID_COMPLETE_ITEM_PROCESSING;d.info.translateX=tn,d.info.translateY=en,d.status.translateY=en,d.processingCompleteIndicator={opacity:1,scaleX:1,scaleY:1}}if(l&&!u&&(["DID_START_ITEM_PROCESSING","DID_REQUEST_ITEM_PROCESSING","DID_UPDATE_ITEM_PROCESS_PROGRESS","DID_THROW_ITEM_PROCESSING_ERROR"].forEach(function(e){un[e].status.translateY=en}),un.DID_THROW_ITEM_PROCESSING_ERROR.status.translateX=Jt),c&&i){Qt.RevertItemProcessing.align="BUTTON_REMOVE_ITEM_POSITION";var p=un.DID_COMPLETE_ITEM_PROCESSING;p.info.translateX=Kt,p.status.translateY=en,p.processingCompleteIndicator={opacity:1,scaleX:1,scaleY:1}}a||(Qt.RemoveItem.disabled=!0),t(Qt,function(e,t){var n=r.createChildView(Gt,{label:r.query(t.label),icon:r.query(t.icon),opacity:0});f.includes(e)&&r.appendChildView(n),t.disabled&&(n.element.setAttribute("disabled","disabled"),n.element.setAttribute("hidden","hidden")),n.element.dataset.align=r.query("GET_STYLE_"+t.align),n.element.classList.add(t.className),n.on("click",function(e){e.stopPropagation(),t.disabled||r.dispatch(t.action,{query:o})}),r.ref["button"+e]=n}),r.ref.processingCompleteIndicator=r.appendChildView(r.createChildView(sn)),r.ref.processingCompleteIndicator.element.dataset.align=r.query("GET_STYLE_BUTTON_PROCESS_ITEM_POSITION"),r.ref.info=r.appendChildView(r.createChildView(jt,{id:o})),r.ref.status=r.appendChildView(r.createChildView(zt,{id:o}));var E=r.appendChildView(r.createChildView(Nt,{opacity:0,align:r.query("GET_STYLE_LOAD_INDICATOR_POSITION")}));E.element.classList.add("filepond--load-indicator"),r.ref.loadProgressIndicator=E;var _=r.appendChildView(r.createChildView(Nt,{opacity:0,align:r.query("GET_STYLE_PROGRESS_INDICATOR_POSITION")}));_.element.classList.add("filepond--process-indicator"),r.ref.processProgressIndicator=_,r.ref.activeStyles=[]},write:function(e){var n=e.root,r=e.actions,o=e.props;ln({root:n,actions:r,props:o});var i=r.concat().filter(function(e){return/^DID_/.test(e.type)}).reverse().find(function(e){return un[e.type]});if(i){n.ref.activeStyles=[];var a=un[i.type];t(rn,function(e,r){var o=n.ref[e];t(r,function(t,r){var i=a[e]&&void 0!==a[e][t]?a[e][t]:r;n.ref.activeStyles.push({control:o,key:t,value:i})})})}n.ref.activeStyles.forEach(function(e){var t=e.control,r=e.key,o=e.value;t[r]="function"==typeof o?o(n):o})},didCreateView:function(e){ye("CREATE_VIEW",Object.assign({},e,{view:e}))},name:"file"}),fn=S({create:function(e){var t=e.root,n=e.props;t.ref.fileName=Lt("legend"),t.appendChild(t.ref.fileName),t.ref.file=t.appendChildView(t.createChildView(cn,{id:n.id})),t.ref.data=!1},ignoreRect:!0,write:A({DID_LOAD_ITEM:function(e){var t=e.root,n=e.props;Mt(t.ref.fileName,Pt(t.query("GET_ITEM_NAME",n.id)))}}),didCreateView:function(e){ye("CREATE_VIEW",Object.assign({},e,{view:e}))},tag:"fieldset",name:"file-wrapper"}),dn={type:"spring",damping:.6,mass:7},pn=function(e,t,n){var r=S({name:"panel-"+t.name+" filepond--"+n,mixins:t.mixins,ignoreRectUpdate:!0}),o=e.createChildView(r,t.props);e.ref[t.name]=e.appendChildView(o)},En=S({name:"panel",write:function(e){var t=e.root,n=e.props;if(null!==t.ref.scalable&&n.scalable===t.ref.scalable||(t.ref.scalable=!N(n.scalable)||n.scalable,t.element.dataset.scalable=t.ref.scalable),n.height){var r=t.ref.top.rect.element,o=t.ref.bottom.rect.element,i=Math.max(r.height+o.height,n.height);t.ref.center.translateY=r.height,t.ref.center.scaleY=(i-r.height-o.height)/100,t.ref.bottom.translateY=i-o.height}},create:function(e){var t=e.root,n=e.props;[{name:"top"},{name:"center",props:{translateY:null,scaleY:null},mixins:{animations:{scaleY:dn},styles:["translateY","scaleY"]}},{name:"bottom",props:{translateY:null},mixins:{animations:{translateY:dn},styles:["translateY"]}}].forEach(function(e){pn(t,e,n.name)}),t.element.classList.add("filepond--"+n.name),t.ref.scalable=null},ignoreRect:!0,mixins:{apis:["height","scalable"]}}),_n={type:"spring",stiffness:.75,damping:.45,mass:10},Tn={DID_START_ITEM_LOAD:"busy",DID_UPDATE_ITEM_LOAD_PROGRESS:"loading",DID_THROW_ITEM_INVALID:"load-invalid",DID_THROW_ITEM_LOAD_ERROR:"load-error",DID_LOAD_ITEM:"idle",DID_THROW_ITEM_REMOVE_ERROR:"remove-error",DID_START_ITEM_REMOVE:"busy",DID_START_ITEM_PROCESSING:"busy processing",DID_REQUEST_ITEM_PROCESSING:"busy processing",DID_UPDATE_ITEM_PROCESS_PROGRESS:"processing",DID_COMPLETE_ITEM_PROCESSING:"processing-complete",DID_THROW_ITEM_PROCESSING_ERROR:"processing-error",DID_THROW_ITEM_PROCESSING_REVERT_ERROR:"processing-revert-error",DID_ABORT_ITEM_PROCESSING:"cancelled",DID_REVERT_ITEM_PROCESSING:"idle"},In=A({DID_UPDATE_PANEL_HEIGHT:function(e){var t=e.root,n=e.action;t.height=n.height}}),vn=A({DID_GRAB_ITEM:function(e){var t=e.root;e.props.dragOrigin={x:t.translateX,y:t.translateY}},DID_DRAG_ITEM:function(e){e.root.element.dataset.dragState="drag"},DID_DROP_ITEM:function(e){var t=e.root,n=e.props;n.dragOffset=null,n.dragOrigin=null,t.element.dataset.dragState="drop"}},function(e){var t=e.root,n=e.actions,r=e.props,o=e.shouldOptimize;"drop"===t.element.dataset.dragState&&t.scaleX<=1&&(t.element.dataset.dragState="idle");var i=n.concat().filter(function(e){return/^DID_/.test(e.type)}).reverse().find(function(e){return Tn[e.type]});i&&i.type!==r.currentState&&(r.currentState=i.type,t.element.dataset.filepondItemState=Tn[r.currentState]||"");var a=t.query("GET_ITEM_PANEL_ASPECT_RATIO")||t.query("GET_PANEL_ASPECT_RATIO");a?o||(t.height=t.rect.element.width*a):(In({root:t,actions:n,props:r}),!t.height&&t.ref.container.rect.element.height>0&&(t.height=t.ref.container.rect.element.height)),o&&(t.ref.panel.height=null),t.ref.panel.height=t.height}),mn=S({create:function(e){var t=e.root,n=e.props;t.ref.handleClick=function(e){return t.dispatch("DID_ACTIVATE_ITEM",{id:n.id})},t.element.id="filepond--item-"+n.id,t.element.addEventListener("click",t.ref.handleClick),t.ref.container=t.appendChildView(t.createChildView(fn,{id:n.id})),t.ref.panel=t.appendChildView(t.createChildView(En,{name:"item-panel"})),t.ref.panel.height=null,n.markedForRemoval=!1,t.query("GET_ALLOW_REORDER")&&(t.element.dataset.dragState="idle",t.element.addEventListener("pointerdown",function(e){if(e.isPrimary){var r=!1,o=e.pageX,i=e.pageY;n.dragOrigin={x:t.translateX,y:t.translateY},n.dragCenter={x:e.offsetX,y:e.offsetY};var a,u,s,l=(a=t.query("GET_ACTIVE_ITEMS"),u=a.map(function(e){return e.id}),s=void 0,{setIndex:function(e){s=e},getIndex:function(){return s},getItemIndex:function(e){return u.indexOf(e.id)}});t.dispatch("DID_GRAB_ITEM",{id:n.id,dragState:l});var c=function(e){e.isPrimary&&(e.stopPropagation(),e.preventDefault(),n.dragOffset={x:e.pageX-o,y:e.pageY-i},n.dragOffset.x*n.dragOffset.x+n.dragOffset.y*n.dragOffset.y>16&&!r&&(r=!0,t.element.removeEventListener("click",t.ref.handleClick)),t.dispatch("DID_DRAG_ITEM",{id:n.id,dragState:l}))};document.addEventListener("pointermove",c),document.addEventListener("pointerup",function e(a){a.isPrimary&&(document.removeEventListener("pointermove",c),document.removeEventListener("pointerup",e),n.dragOffset={x:a.pageX-o,y:a.pageY-i},t.dispatch("DID_DROP_ITEM",{id:n.id,dragState:l}),r&&setTimeout(function(){return t.element.addEventListener("click",t.ref.handleClick)},0))})}}))},write:vn,destroy:function(e){var t=e.root,n=e.props;t.element.removeEventListener("click",t.ref.handleClick),t.dispatch("RELEASE_ITEM",{query:n.id})},tag:"li",name:"item",mixins:{apis:["id","interactionMethod","markedForRemoval","spawnDate","dragCenter","dragOrigin","dragOffset"],styles:["translateX","translateY","scaleX","scaleY","opacity","height"],animations:{scaleX:"spring",scaleY:"spring",translateX:_n,translateY:_n,opacity:{type:"tween",duration:150}}}}),gn=function(e,t,n){if(n){var r=e.rect.element.width,o=t.length,i=null;if(0===o||n.top<t[0].rect.element.top)return-1;var a=t[0].rect.element,u=a.marginLeft+a.marginRight,s=a.width+u,l=Math.round(r/s);if(1===l){for(var c=0;c<o;c++){var f=t[c],d=f.rect.outer.top+.5*f.rect.element.height;if(n.top<d)return c}return o}for(var p=a.marginTop+a.marginBottom,E=a.height+p,_=0;_<o;_++){var T=_%l*s,I=Math.floor(_/l)*E,v=I-a.marginTop,m=T+s,g=I+E+a.marginBottom;if(n.top<g&&n.top>v){if(n.left<m)return _;i=_!==o-1?_:null}}return null!==i?i:o}},hn={height:0,width:0,get getHeight(){return this.height},set setHeight(e){0!==this.height&&0!==e||(this.height=e)},get getWidth(){return this.width},set setWidth(e){0!==this.width&&0!==e||(this.width=e)},setDimensions:function(e,t){0!==this.height&&0!==e||(this.height=e),0!==this.width&&0!==t||(this.width=t)}},Rn=function(e,t,n){var r=arguments.length>3&&void 0!==arguments[3]?arguments[3]:0,o=arguments.length>4&&void 0!==arguments[4]?arguments[4]:1;e.dragOffset?(e.translateX=null,e.translateY=null,e.translateX=e.dragOrigin.x+e.dragOffset.x,e.translateY=e.dragOrigin.y+e.dragOffset.y,e.scaleX=1.025,e.scaleY=1.025):(e.translateX=t,e.translateY=n,Date.now()>e.spawnDate&&(0===e.opacity&&On(e,t,n,r,o),e.scaleX=1,e.scaleY=1,e.opacity=1))},On=function(e,t,n,r,o){e.interactionMethod===re?(e.translateX=null,e.translateX=t,e.translateY=null,e.translateY=n):e.interactionMethod===ee?(e.translateX=null,e.translateX=t-20*r,e.translateY=null,e.translateY=n-10*o,e.scaleX=.8,e.scaleY=.8):e.interactionMethod===te?(e.translateY=null,e.translateY=n-30):e.interactionMethod===J&&(e.translateX=null,e.translateX=t-30,e.translateY=null)},yn=function(e){return e.rect.element.height+.5*e.rect.element.marginBottom+.5*e.rect.element.marginTop},Dn=A({DID_ADD_ITEM:function(e){var t=e.root,n=e.action,r=n.id,o=n.index,i=n.interactionMethod;t.ref.addIndex=o;var a=Date.now(),u=a,s=1;if(i!==re){s=0;var l=t.query("GET_ITEM_INSERT_INTERVAL"),c=a-t.ref.lastItemSpanwDate;u=c<l?a+(l-c):a}t.ref.lastItemSpanwDate=u,t.appendChildView(t.createChildView(mn,{spawnDate:u,id:r,opacity:s,interactionMethod:i}),o)},DID_REMOVE_ITEM:function(e){var t=e.root,n=e.action.id,r=t.childViews.find(function(e){return e.id===n});r&&(r.scaleX=.9,r.scaleY=.9,r.opacity=0,r.markedForRemoval=!0)},DID_DRAG_ITEM:function(e){var t=e.root,n=e.action,r=n.id,o=n.dragState,i=t.query("GET_ITEM",{id:r}),a=t.childViews.find(function(e){return e.id===r}),u=t.childViews.length,s=o.getItemIndex(i);if(a){var l,c=a.dragOrigin.x+a.dragOffset.x+a.dragCenter.x,f=a.dragOrigin.y+a.dragOffset.y+a.dragCenter.y,d=yn(a),p=(l=a).rect.element.width+.5*l.rect.element.marginLeft+.5*l.rect.element.marginRight,E=Math.floor(t.rect.outer.width/p);E>u&&(E=u);var _=Math.floor(u/E+1);hn.setHeight=d*_,hn.setWidth=p*E;var T={y:Math.floor(f/d),x:Math.floor(c/p),getGridIndex:function(){return f>hn.getHeight||f<0||c>hn.getWidth||c<0?s:this.y*E+this.x},getColIndex:function(){for(var e=t.query("GET_ACTIVE_ITEMS"),n=t.childViews.filter(function(e){return e.rect.element.height}),r=e.map(function(e){return n.find(function(t){return t.id===e.id})}),o=r.findIndex(function(e){return e===a}),i=yn(a),u=r.length,s=u,l=0,c=0,d=0,p=0;p<u;p++)if(l=yn(r[p]),f<(c=(d=c)+l)){if(o>p){if(f<d+i){s=p;break}continue}s=p;break}return s}},I=E>1?T.getGridIndex():T.getColIndex();t.dispatch("MOVE_ITEM",{query:a,index:I});var v=o.getIndex();if(void 0===v||v!==I){if(o.setIndex(I),void 0===v)return;t.dispatch("DID_REORDER_ITEMS",{items:t.query("GET_ACTIVE_ITEMS"),origin:s,target:I})}}}}),Sn=S({create:function(e){var t=e.root;r(t.element,"role","list"),t.ref.lastItemSpanwDate=Date.now()},write:function(e){var t=e.root,n=e.props,r=e.actions,o=e.shouldOptimize;Dn({root:t,props:n,actions:r});var i=n.dragCoordinates,a=t.rect.element.width,u=t.childViews.filter(function(e){return e.rect.element.height}),s=t.query("GET_ACTIVE_ITEMS").map(function(e){return u.find(function(t){return t.id===e.id})}).filter(function(e){return e}),l=i?gn(t,s,i):null,c=t.ref.addIndex||null;t.ref.addIndex=null;var f=0,d=0,p=0;if(0!==s.length){var E=s[0].rect.element,_=E.marginTop+E.marginBottom,T=E.marginLeft+E.marginRight,I=E.width+T,v=E.height+_,m=Math.round(a/I);if(1===m){var g=0,h=0;s.forEach(function(e,t){if(l){var n=t-l;h=-2===n?.25*-_:-1===n?.75*-_:0===n?.75*_:1===n?.25*_:0}o&&(e.translateX=null,e.translateY=null),e.markedForRemoval||Rn(e,0,g+h);var r=(e.rect.element.height+_)*(e.markedForRemoval?e.opacity:1);g+=r})}else{var R=0,O=0;s.forEach(function(e,t){t===l&&(f=1),t===c&&(p+=1),e.markedForRemoval&&e.opacity<.5&&(d-=1);var n=t+p+f+d,r=n%m,i=Math.floor(n/m),a=r*I,u=i*v,s=Math.sign(a-R),E=Math.sign(u-O);R=a,O=u,e.markedForRemoval||(o&&(e.translateX=null,e.translateY=null),Rn(e,a,u,s,E))})}}},tag:"ul",name:"list",didWriteView:function(e){var t=e.root;t.childViews.filter(function(e){return e.markedForRemoval&&0===e.opacity&&e.resting}).forEach(function(e){e._destroy(),t.removeChildView(e)})},filterFrameActionsForChild:function(e,t){return t.filter(function(t){return!t.data||!t.data.id||e.id===t.data.id})},mixins:{apis:["dragCoordinates"]}}),An=A({DID_DRAG:function(e){var t=e.root,n=e.props,r=e.action;t.query("GET_ITEM_INSERT_LOCATION_FREEDOM")&&(n.dragCoordinates={left:r.position.scopeLeft-t.ref.list.rect.element.left,top:r.position.scopeTop-(t.rect.outer.top+t.rect.element.marginTop+t.rect.element.scrollTop)})},DID_END_DRAG:function(e){e.props.dragCoordinates=null}}),bn=S({create:function(e){var t=e.root,n=e.props;t.ref.list=t.appendChildView(t.createChildView(Sn)),n.dragCoordinates=null,n.overflowing=!1},write:function(e){var t=e.root,n=e.props,r=e.actions;if(An({root:t,props:n,actions:r}),t.ref.list.dragCoordinates=n.dragCoordinates,n.overflowing&&!n.overflow&&(n.overflowing=!1,t.element.dataset.state="",t.height=null),n.overflow){var o=Math.round(n.overflow);o!==t.height&&(n.overflowing=!0,t.element.dataset.state="overflow",t.height=o)}},name:"list-scroller",mixins:{apis:["overflow","dragCoordinates"],styles:["height","translateY"],animations:{translateY:"spring"}}}),Pn=function(e,t,n){var o=arguments.length>3&&void 0!==arguments[3]?arguments[3]:"";n?r(e,t,o):e.removeAttribute(t)},Ln=function(e){var t=e.root,n=e.action;t.query("GET_ALLOW_SYNC_ACCEPT_ATTRIBUTE")&&Pn(t.element,"accept",!!n.value,n.value?n.value.join(","):"")},Mn=function(e){var t=e.root,n=e.action;Pn(t.element,"multiple",n.value)},wn=function(e){var t=e.root,n=e.action;Pn(t.element,"webkitdirectory",n.value)},Cn=function(e){var t=e.root,n=t.query("GET_DISABLED"),r=t.query("GET_ALLOW_BROWSE"),o=n||!r;Pn(t.element,"disabled",o)},Nn=function(e){var t=e.root;e.action.value?0===t.query("GET_TOTAL_ITEMS")&&Pn(t.element,"required",!0):Pn(t.element,"required",!1)},Gn=function(e){var t=e.root,n=e.action;Pn(t.element,"capture",!!n.value,!0===n.value?"":n.value)},Un=function(e){var t=e.root,n=t.element;t.query("GET_TOTAL_ITEMS")>0?(Pn(n,"required",!1),Pn(n,"name",!1)):(Pn(n,"name",!0,t.query("GET_NAME")),t.query("GET_CHECK_VALIDITY")&&n.setCustomValidity(""),t.query("GET_REQUIRED")&&Pn(n,"required",!0))},Bn=S({tag:"input",name:"browser",ignoreRect:!0,ignoreRectUpdate:!0,attributes:{type:"file"},create:function(e){var t=e.root,n=e.props;t.element.id="filepond--browser-"+n.id,r(t.element,"name",t.query("GET_NAME")),r(t.element,"aria-controls","filepond--assistant-"+n.id),r(t.element,"aria-labelledby","filepond--drop-label-"+n.id),Ln({root:t,action:{value:t.query("GET_ACCEPTED_FILE_TYPES")}}),Mn({root:t,action:{value:t.query("GET_ALLOW_MULTIPLE")}}),wn({root:t,action:{value:t.query("GET_ALLOW_DIRECTORIES_ONLY")}}),Cn({root:t}),Nn({root:t,action:{value:t.query("GET_REQUIRED")}}),Gn({root:t,action:{value:t.query("GET_CAPTURE_METHOD")}}),t.ref.handleChange=function(e){if(t.element.value){var r=Array.from(t.element.files).map(function(e){return e._relativePath=e.webkitRelativePath,e});setTimeout(function(){n.onload(r),function(e){if(e&&""!==e.value){try{e.value=""}catch(e){}if(e.value){var t=Lt("form"),n=e.parentNode,r=e.nextSibling;t.appendChild(e),t.reset(),r?n.insertBefore(e,r):n.appendChild(e)}}}(t.element)},250)}},t.element.addEventListener("change",t.ref.handleChange)},destroy:function(e){var t=e.root;t.element.removeEventListener("change",t.ref.handleChange)},write:A({DID_LOAD_ITEM:Un,DID_REMOVE_ITEM:Un,DID_THROW_ITEM_INVALID:function(e){var t=e.root;t.query("GET_CHECK_VALIDITY")&&t.element.setCustomValidity(t.query("GET_LABEL_INVALID_FIELD"))},DID_SET_DISABLED:Cn,DID_SET_ALLOW_BROWSE:Cn,DID_SET_ALLOW_DIRECTORIES_ONLY:wn,DID_SET_ALLOW_MULTIPLE:Mn,DID_SET_ACCEPTED_FILE_TYPES:Ln,DID_SET_CAPTURE_METHOD:Gn,DID_SET_REQUIRED:Nn})}),qn=13,Vn=32,Fn=function(e,t){e.innerHTML=t;var n=e.querySelector(".filepond--label-action");return n&&r(n,"tabindex","0"),t},xn=S({name:"drop-label",ignoreRect:!0,create:function(e){var t=e.root,n=e.props,o=Lt("label");r(o,"for","filepond--browser-"+n.id),r(o,"id","filepond--drop-label-"+n.id),r(o,"aria-hidden","true"),t.ref.handleKeyDown=function(e){(e.keyCode===qn||e.keyCode===Vn)&&(e.preventDefault(),t.ref.label.click())},t.ref.handleClick=function(e){e.target===o||o.contains(e.target)||t.ref.label.click()},o.addEventListener("keydown",t.ref.handleKeyDown),t.element.addEventListener("click",t.ref.handleClick),Fn(o,n.caption),t.appendChild(o),t.ref.label=o},destroy:function(e){var t=e.root;t.ref.label.addEventListener("keydown",t.ref.handleKeyDown),t.element.removeEventListener("click",t.ref.handleClick)},write:A({DID_SET_LABEL_IDLE:function(e){var t=e.root,n=e.action;Fn(t.ref.label,n.value)}}),mixins:{styles:["opacity","translateX","translateY"],animations:{opacity:{type:"tween",duration:150},translateX:"spring",translateY:"spring"}}}),Yn=S({name:"drip-blob",ignoreRect:!0,mixins:{styles:["translateX","translateY","scaleX","scaleY","opacity"],animations:{scaleX:"spring",scaleY:"spring",translateX:"spring",translateY:"spring",opacity:{type:"tween",duration:250}}}}),jn=A({DID_DRAG:function(e){var t=e.root,n=e.action;t.ref.blob?(t.ref.blob.translateX=n.position.scopeLeft,t.ref.blob.translateY=n.position.scopeTop,t.ref.blob.scaleX=1,t.ref.blob.scaleY=1,t.ref.blob.opacity=1):function(e){var t=e.root,n=.5*t.rect.element.width,r=.5*t.rect.element.height;t.ref.blob=t.appendChildView(t.createChildView(Yn,{opacity:0,scaleX:2.5,scaleY:2.5,translateX:n,translateY:r}))}({root:t})},DID_DROP:function(e){var t=e.root;t.ref.blob&&(t.ref.blob.scaleX=2.5,t.ref.blob.scaleY=2.5,t.ref.blob.opacity=0)},DID_END_DRAG:function(e){var t=e.root;t.ref.blob&&(t.ref.blob.opacity=0)}}),kn=S({ignoreRect:!0,ignoreRectUpdate:!0,name:"drip",write:function(e){var t=e.root,n=e.props,r=e.actions;jn({root:t,props:n,actions:r});var o=t.ref.blob;0===r.length&&o&&0===o.opacity&&(t.removeChildView(o),t.ref.blob=null)}}),Hn=function(e,t){return e.ref.fields[t]},Xn=function(e){e.query("GET_ACTIVE_ITEMS").forEach(function(t){e.ref.fields[t.id]&&e.element.appendChild(e.ref.fields[t.id])})},Wn=function(e){var t=e.root;return Xn(t)},zn=A({DID_SET_DISABLED:function(e){var t=e.root;t.element.disabled=t.query("GET_DISABLED")},DID_ADD_ITEM:function(e){var t=e.root,n=e.action,r=Lt("input");r.type="hidden",r.name=t.query("GET_NAME"),r.disabled=t.query("GET_DISABLED"),t.ref.fields[n.id]=r,Xn(t)},DID_LOAD_ITEM:function(e){var t=e.root,n=e.action,r=Hn(t,n.id);r&&null!==n.serverFileReference&&(r.value=n.serverFileReference)},DID_REMOVE_ITEM:function(e){var t=e.root,n=e.action,r=Hn(t,n.id);r&&(r.parentNode.removeChild(r),delete t.ref.fields[n.id])},DID_DEFINE_VALUE:function(e){var t=e.root,n=e.action,r=Hn(t,n.id);r&&(null===n.value?r.removeAttribute("value"):r.value=n.value,Xn(t))},DID_REORDER_ITEMS:Wn,DID_SORT_ITEMS:Wn}),Qn=S({tag:"fieldset",name:"data",create:function(e){return e.root.ref.fields={}},write:zn,ignoreRect:!0}),$n=["jpg","jpeg","png","gif","bmp","webp","svg","tiff"],Zn=["css","csv","html","txt"],Kn={zip:"zip|compressed",epub:"application/epub+zip"},Jn=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:"";return e=e.toLowerCase(),$n.includes(e)?"image/"+("jpg"===e?"jpeg":"svg"===e?"svg+xml":e):Zn.includes(e)?"text/"+e:Kn[e]||""},er=function(e){return new Promise(function(t,n){var r=cr(e);if(r.length&&!tr(e))return t(r);nr(e).then(t)})},tr=function(e){return!!e.files&&e.files.length>0},nr=function(e){return new Promise(function(t,n){var r=(e.items?Array.from(e.items):[]).filter(function(e){return rr(e)}).map(function(e){return or(e)});r.length?Promise.all(r).then(function(e){var n=[];e.forEach(function(e){n.push.apply(n,e)}),t(n.filter(function(e){return e}).map(function(e){return e._relativePath||(e._relativePath=e.webkitRelativePath),e}))}).catch(console.error):t(e.files?Array.from(e.files):[])})},rr=function(e){if(sr(e)){var t=lr(e);if(t)return t.isFile||t.isDirectory}return"file"===e.kind},or=function(e){return new Promise(function(t,n){ur(e)?ir(lr(e)).then(t).catch(n):t([e.getAsFile()])})},ir=function(e){return new Promise(function(t,n){var r=[],o=0,i=0,a=function(){0===i&&0===o&&t(r)};!function e(t){o++;var u=t.createReader();!function t(){u.readEntries(function(n){if(0===n.length)return o--,void a();n.forEach(function(t){t.isDirectory?e(t):(i++,t.file(function(e){var n=ar(e);t.fullPath&&(n._relativePath=t.fullPath),r.push(n),i--,a()}))}),t()},n)}()}(e)})},ar=function(e){if(e.type.length)return e;var t=e.lastModifiedDate,n=e.name;return(e=e.slice(0,e.size,Jn(Ye(e.name)))).name=n,e.lastModifiedDate=t,e},ur=function(e){return sr(e)&&(lr(e)||{}).isDirectory},sr=function(e){return"webkitGetAsEntry"in e},lr=function(e){return e.webkitGetAsEntry()},cr=function(e){var t=[];try{if((t=dr(e)).length)return t;t=fr(e)}catch(e){}return t},fr=function(e){var t=e.getData("url");return"string"==typeof t&&t.length?[t]:[]},dr=function(e){var t=e.getData("text/html");if("string"==typeof t&&t.length){var n=t.match(/src\s*=\s*"(.+?)"/);if(n)return[n[1]]}return[]},pr=[],Er=function(e){return{pageLeft:e.pageX,pageTop:e.pageY,scopeLeft:e.offsetX||e.layerX,scopeTop:e.offsetY||e.layerY}},_r=function(e){var t=pr.find(function(t){return t.element===e});if(t)return t;var n=Tr(e);return pr.push(n),n},Tr=function(e){var n=[],r={dragenter:gr,dragover:hr,dragleave:Or,drop:Rr},o={};t(r,function(t,r){o[t]=r(e,n),e.addEventListener(t,o[t],!1)});var i={element:e,addListener:function(a){return n.push(a),function(){n.splice(n.indexOf(a),1),0===n.length&&(pr.splice(pr.indexOf(i),1),t(r,function(t){e.removeEventListener(t,o[t],!1)}))}}};return i},Ir=function(e,t){var n,r=function(e,t){return"elementFromPoint"in e||(e=document),e.elementFromPoint(t.x,t.y)}("getRootNode"in(n=t)?n.getRootNode():document,{x:e.pageX-window.pageXOffset,y:e.pageY-window.pageYOffset});return r===t||t.contains(r)},vr=null,mr=function(e,t){try{e.dropEffect=t}catch(e){}},gr=function(e,t){return function(e){e.preventDefault(),vr=e.target,t.forEach(function(t){var n=t.element,r=t.onenter;Ir(e,n)&&(t.state="enter",r(Er(e)))})}},hr=function(e,t){return function(e){e.preventDefault();var n=e.dataTransfer;er(n).then(function(r){var o=!1;t.some(function(t){var i=t.filterElement,a=t.element,u=t.onenter,s=t.onexit,l=t.ondrag,c=t.allowdrop;mr(n,"copy");var f=c(r);if(f)if(Ir(e,a)){if(o=!0,null===t.state)return t.state="enter",void u(Er(e));if(t.state="over",i&&!f)return void mr(n,"none");l(Er(e))}else i&&!o&&mr(n,"none"),t.state&&(t.state=null,s(Er(e)));else mr(n,"none")})})}},Rr=function(e,t){return function(e){e.preventDefault();var n=e.dataTransfer;er(n).then(function(n){t.forEach(function(t){var r=t.filterElement,o=t.element,i=t.ondrop,a=t.onexit,u=t.allowdrop;if(t.state=null,!r||Ir(e,o))return u(n)?void i(Er(e),n):a(Er(e))})})}},Or=function(e,t){return function(e){vr===e.target&&t.forEach(function(t){var n=t.onexit;t.state=null,n(Er(e))})}},yr=function(e,t,n){e.classList.add("filepond--hopper");var r=n.catchesDropsOnPage,o=n.requiresDropOnElement,i=n.filterItems,a=void 0===i?function(e){return e}:i,u=function(e,t,n){var r=_r(t),o={element:e,filterElement:n,state:null,ondrop:function(){},onenter:function(){},ondrag:function(){},onexit:function(){},onload:function(){},allowdrop:function(){}};return o.destroy=r.addListener(o),o}(e,r?document.documentElement:e,o),s="",l="";u.allowdrop=function(e){return t(a(e))},u.ondrop=function(e,n){var r=a(n);t(r)?(l="drag-drop",c.onload(r,e)):c.ondragend(e)},u.ondrag=function(e){c.ondrag(e)},u.onenter=function(e){l="drag-over",c.ondragstart(e)},u.onexit=function(e){l="drag-exit",c.ondragend(e)};var c={updateHopperState:function(){s!==l&&(e.dataset.hopperState=l,s=l)},onload:function(){},ondragstart:function(){},ondrag:function(){},ondragend:function(){},destroy:function(){u.destroy()}};return c},Dr=!1,Sr=[],Ar=function(e){var t=document.activeElement;if(t&&/textarea|input/i.test(t.nodeName)){for(var n=!1,r=t;r!==document.body;){if(r.classList.contains("filepond--root")){n=!0;break}r=r.parentNode}if(!n)return}er(e.clipboardData).then(function(e){e.length&&Sr.forEach(function(t){return t(e)})})},br=function(){var e=function(e){t.onload(e)},t={destroy:function(){var t;t=e,de(Sr,Sr.indexOf(t)),0===Sr.length&&(document.removeEventListener("paste",Ar),Dr=!1)},onload:function(){}};return function(e){Sr.includes(e)||(Sr.push(e),Dr||(Dr=!0,document.addEventListener("paste",Ar)))}(e),t},Pr=null,Lr=null,Mr=[],wr=function(e,t){e.element.textContent=t},Cr=function(e,t,n){var r=e.query("GET_TOTAL_ITEMS");wr(e,n+" "+t+", "+r+" "+(1===r?e.query("GET_LABEL_FILE_COUNT_SINGULAR"):e.query("GET_LABEL_FILE_COUNT_PLURAL"))),clearTimeout(Lr),Lr=setTimeout(function(){!function(e){e.element.textContent=""}(e)},1500)},Nr=function(e){return e.element.parentNode.contains(document.activeElement)},Gr=function(e){var t=e.root,n=e.action,r=t.query("GET_ITEM",n.id).filename,o=t.query("GET_LABEL_FILE_PROCESSING_ABORTED");wr(t,r+" "+o)},Ur=function(e){var t=e.root,n=e.action,r=t.query("GET_ITEM",n.id).filename;wr(t,n.status.main+" "+r+" "+n.status.sub)},Br=S({create:function(e){var t=e.root,n=e.props;t.element.id="filepond--assistant-"+n.id,r(t.element,"role","status"),r(t.element,"aria-live","polite"),r(t.element,"aria-relevant","additions")},ignoreRect:!0,ignoreRectUpdate:!0,write:A({DID_LOAD_ITEM:function(e){var t=e.root,n=e.action;if(Nr(t)){t.element.textContent="";var r=t.query("GET_ITEM",n.id);Mr.push(r.filename),clearTimeout(Pr),Pr=setTimeout(function(){Cr(t,Mr.join(", "),t.query("GET_LABEL_FILE_ADDED")),Mr.length=0},750)}},DID_REMOVE_ITEM:function(e){var t=e.root,n=e.action;if(Nr(t)){var r=n.item;Cr(t,r.filename,t.query("GET_LABEL_FILE_REMOVED"))}},DID_COMPLETE_ITEM_PROCESSING:function(e){var t=e.root,n=e.action,r=t.query("GET_ITEM",n.id).filename,o=t.query("GET_LABEL_FILE_PROCESSING_COMPLETE");wr(t,r+" "+o)},DID_ABORT_ITEM_PROCESSING:Gr,DID_REVERT_ITEM_PROCESSING:Gr,DID_THROW_ITEM_REMOVE_ERROR:Ur,DID_THROW_ITEM_LOAD_ERROR:Ur,DID_THROW_ITEM_INVALID:Ur,DID_THROW_ITEM_PROCESSING_ERROR:Ur}),tag:"span",name:"assistant"}),qr=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:"-";return e.replace(new RegExp(t+".","g"),function(e){return e.charAt(1).toUpperCase()})},Vr=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:16,n=!(arguments.length>2&&void 0!==arguments[2])||arguments[2],r=Date.now(),o=null;return function(){for(var i=arguments.length,a=new Array(i),u=0;u<i;u++)a[u]=arguments[u];clearTimeout(o);var s=Date.now()-r,l=function(){r=Date.now(),e.apply(void 0,a)};s<t?n||(o=setTimeout(l,t-s)):l()}},Fr=function(e){return e.preventDefault()},xr=function(e){var t=e.ref.list.childViews[0].childViews[0];return t?{top:t.rect.element.marginTop,bottom:t.rect.element.marginBottom}:{top:0,bottom:0}},Yr=function(e){var t=0,n=0,r=e.ref.list,o=r.childViews[0],i=o.childViews.filter(function(e){return e.rect.element.height}),a=e.query("GET_ACTIVE_ITEMS").map(function(e){return i.find(function(t){return t.id===e.id})}).filter(function(e){return e});if(0===a.length)return{visual:t,bounds:n};var u=o.rect.element.width,s=gn(o,a,r.dragCoordinates),l=a[0].rect.element,c=l.marginTop+l.marginBottom,f=l.marginLeft+l.marginRight,d=l.width+f,p=l.height+c,E=void 0!==s&&s>=0?1:0,_=a.find(function(e){return e.markedForRemoval&&e.opacity<.45})?-1:0,T=a.length+E+_,I=Math.round(u/d);return 1===I?a.forEach(function(e){var r=e.rect.element.height+c;n+=r,t+=r*e.opacity}):(n=Math.ceil(T/I)*p,t=n),{visual:t,bounds:n}},jr=function(e){var t=e.ref.measureHeight||null;return{cappedHeight:parseInt(e.style.maxHeight,10)||null,fixedHeight:0===t?null:t}},kr=function(e,t){var n=e.query("GET_ALLOW_REPLACE"),r=e.query("GET_ALLOW_MULTIPLE"),o=e.query("GET_TOTAL_ITEMS"),i=e.query("GET_MAX_FILES"),a=t.length;return!r&&a>1||!!(F(i=r?i:n?i:1)&&o+a>i)&&(e.dispatch("DID_THROW_MAX_FILES",{source:t,error:nt("warning",0,"Max files")}),!0)},Hr=function(e,t,n){var r=e.childViews[0];return gn(r,t,{left:n.scopeLeft-r.rect.element.left,top:n.scopeTop-(e.rect.outer.top+e.rect.element.marginTop+e.rect.element.scrollTop)})},Xr=function(e){var t=e.query("GET_ALLOW_DROP"),n=e.query("GET_DISABLED"),r=t&&!n;if(r&&!e.ref.hopper){var o=yr(e.element,function(t){if(kr(e,t))return!1;var n=e.query("GET_BEFORE_DROP_FILE")||function(){return!0};return!e.query("GET_DROP_VALIDATION")||t.every(function(t){return ye("ALLOW_HOPPER_ITEM",t,{query:e.query}).every(function(e){return!0===e})&&n(t)})},{filterItems:function(t){var n=e.query("GET_IGNORED_FILES");return t.filter(function(e){return!vt(e)||!n.includes(e.name.toLowerCase())})},catchesDropsOnPage:e.query("GET_DROP_ON_PAGE"),requiresDropOnElement:e.query("GET_DROP_ON_ELEMENT")});o.onload=function(t,n){var r=e.ref.list.childViews[0].childViews.filter(function(e){return e.rect.element.height}),o=e.query("GET_ACTIVE_ITEMS").map(function(e){return r.find(function(t){return t.id===e.id})}).filter(function(e){return e});e.dispatch("ADD_ITEMS",{items:t,index:Hr(e.ref.list,o,n),interactionMethod:ee}),e.dispatch("DID_DROP",{position:n}),e.dispatch("DID_END_DRAG",{position:n})},o.ondragstart=function(t){e.dispatch("DID_START_DRAG",{position:t})},o.ondrag=Vr(function(t){e.dispatch("DID_DRAG",{position:t})}),o.ondragend=function(t){e.dispatch("DID_END_DRAG",{position:t})},e.ref.hopper=o,e.ref.drip=e.appendChildView(e.createChildView(kn))}else!r&&e.ref.hopper&&(e.ref.hopper.destroy(),e.ref.hopper=null,e.removeChildView(e.ref.drip))},Wr=function(e,t){var n=e.query("GET_ALLOW_BROWSE"),r=e.query("GET_DISABLED"),o=n&&!r;o&&!e.ref.browser?e.ref.browser=e.appendChildView(e.createChildView(Bn,Object.assign({},t,{onload:function(t){if(kr(e,t))return!1;e.dispatch("ADD_ITEMS",{items:t,index:-1,interactionMethod:te})}})),0):!o&&e.ref.browser&&(e.removeChildView(e.ref.browser),e.ref.browser=null)},zr=function(e){var t=e.query("GET_ALLOW_PASTE"),n=e.query("GET_DISABLED"),r=t&&!n;r&&!e.ref.paster?(e.ref.paster=br(),e.ref.paster.onload=function(t){e.dispatch("ADD_ITEMS",{items:t,index:-1,interactionMethod:ne})}):!r&&e.ref.paster&&(e.ref.paster.destroy(),e.ref.paster=null)},Qr=A({DID_SET_ALLOW_BROWSE:function(e){var t=e.root,n=e.props;Wr(t,n)},DID_SET_ALLOW_DROP:function(e){var t=e.root;Xr(t)},DID_SET_ALLOW_PASTE:function(e){var t=e.root;zr(t)},DID_SET_DISABLED:function(e){var t=e.root,n=e.props;Xr(t),zr(t),Wr(t,n),t.query("GET_DISABLED")?t.element.dataset.disabled="disabled":t.element.removeAttribute("data-disabled")}}),$r=S({name:"root",read:function(e){var t=e.root;t.ref.measure&&(t.ref.measureHeight=t.ref.measure.offsetHeight)},create:function(e){var t=e.root,n=e.props,r=t.query("GET_ID");r&&(t.element.id=r);var o=t.query("GET_CLASS_NAME");o&&o.split(" ").filter(function(e){return e.length}).forEach(function(e){t.element.classList.add(e)}),t.ref.label=t.appendChildView(t.createChildView(xn,Object.assign({},n,{translateY:null,caption:t.query("GET_LABEL_IDLE")}))),t.ref.list=t.appendChildView(t.createChildView(bn,{translateY:null})),t.ref.panel=t.appendChildView(t.createChildView(En,{name:"panel-root"})),t.ref.assistant=t.appendChildView(t.createChildView(Br,Object.assign({},n))),t.ref.data=t.appendChildView(t.createChildView(Qn,Object.assign({},n))),t.ref.measure=Lt("div"),t.ref.measure.style.height="100%",t.element.appendChild(t.ref.measure),t.ref.bounds=null,t.query("GET_STYLES").filter(function(e){return!M(e.value)}).map(function(e){var n=e.name,r=e.value;t.element.dataset[n]=r}),t.ref.widthPrevious=null,t.ref.widthUpdated=Vr(function(){t.ref.updateHistory=[],t.dispatch("DID_RESIZE_ROOT")},250),t.ref.previousAspectRatio=null,t.ref.updateHistory=[];var i=window.matchMedia("(pointer: fine) and (hover: hover)").matches,a="PointerEvent"in window;t.query("GET_ALLOW_REORDER")&&a&&!i&&(t.element.addEventListener("touchmove",Fr,{passive:!1}),t.element.addEventListener("gesturestart",Fr))},write:function(e){var t=e.root,n=e.props,r=e.actions;if(Qr({root:t,props:n,actions:r}),r.filter(function(e){return/^DID_SET_STYLE_/.test(e.type)}).filter(function(e){return!M(e.data.value)}).map(function(e){var n=e.type,r=e.data,o=qr(n.substr(8).toLowerCase(),"_");t.element.dataset[o]=r.value,t.invalidateLayout()}),!t.rect.element.hidden){t.rect.element.width!==t.ref.widthPrevious&&(t.ref.widthPrevious=t.rect.element.width,t.ref.widthUpdated());var o=t.ref.bounds;o||(o=t.ref.bounds=jr(t),t.element.removeChild(t.ref.measure),t.ref.measure=null);var i=t.ref,a=i.hopper,u=i.label,s=i.list,l=i.panel;a&&a.updateHopperState();var c=t.query("GET_PANEL_ASPECT_RATIO"),f=t.query("GET_ALLOW_MULTIPLE"),d=t.query("GET_TOTAL_ITEMS"),p=d===(f?t.query("GET_MAX_FILES")||1e6:1),E=r.find(function(e){return"DID_ADD_ITEM"===e.type});if(p&&E){var _=E.data.interactionMethod;u.opacity=0,f?u.translateY=-40:_===J?u.translateX=40:u.translateY=_===te?40:30}else p||(u.opacity=1,u.translateX=0,u.translateY=0);var T=xr(t),I=Yr(t),v=u.rect.element.height,m=!f||p?0:v,g=p?s.rect.element.marginTop:0,h=0===d?0:s.rect.element.marginBottom,R=m+g+I.visual+h,O=m+g+I.bounds+h;if(s.translateY=Math.max(0,m-s.rect.element.marginTop)-T.top,c){var y=t.rect.element.width,D=y*c;c!==t.ref.previousAspectRatio&&(t.ref.previousAspectRatio=c,t.ref.updateHistory=[]);var S=t.ref.updateHistory;if(S.push(y),S.length>4)for(var A=S.length,b=A-10,P=0,L=A;L>=b;L--)if(S[L]===S[L-2]&&P++,P>=2)return;l.scalable=!1,l.height=D;var w=D-m-(h-T.bottom)-(p?g:0);I.visual>w?s.overflow=w:s.overflow=null,t.height=D}else if(o.fixedHeight){l.scalable=!1;var C=o.fixedHeight-m-(h-T.bottom)-(p?g:0);I.visual>C?s.overflow=C:s.overflow=null}else if(o.cappedHeight){var N=R>=o.cappedHeight,G=Math.min(o.cappedHeight,R);l.scalable=!0,l.height=N?G:G-T.top-T.bottom;var U=G-m-(h-T.bottom)-(p?g:0);R>o.cappedHeight&&I.visual>U?s.overflow=U:s.overflow=null,t.height=Math.min(o.cappedHeight,O-T.top-T.bottom)}else{var B=d>0?T.top+T.bottom:0;l.scalable=!0,l.height=Math.max(v,R-B),t.height=Math.max(v,O-B)}}},destroy:function(e){var t=e.root;t.ref.paster&&t.ref.paster.destroy(),t.ref.hopper&&t.ref.hopper.destroy(),t.element.removeEventListener("touchmove",Fr),t.element.removeEventListener("gesturestart",Fr)},mixins:{styles:["height"]}}),Zr=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},r=null,o=Se(),i=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:[],n=arguments.length>2&&void 0!==arguments[2]?arguments[2]:[],r=Object.assign({},e),o=[],i=[],a=function(e,t,n){!n||document.hidden?(c[e]&&c[e](t),o.push({type:e,data:t})):i.push({type:e,data:t})},u=function(e){for(var t,n=arguments.length,r=new Array(n>1?n-1:0),o=1;o<n;o++)r[o-1]=arguments[o];return l[e]?(t=l)[e].apply(t,r):null},s={getState:function(){return Object.assign({},r)},processActionQueue:function(){var e=[].concat(o);return o.length=0,e},processDispatchQueue:function(){var e=[].concat(i);i.length=0,e.forEach(function(e){var t=e.type,n=e.data;a(t,n)})},dispatch:a,query:u},l={};t.forEach(function(e){l=Object.assign({},e(r),{},l)});var c={};return n.forEach(function(e){c=Object.assign({},e(a,u,r),{},c)}),s}({items:[],listUpdateTimeout:null,itemUpdateTimeout:null,processingQueue:[],options:Q(o)},[qe,K(o)],[bt,Z(o)]);i.dispatch("SET_OPTIONS",{options:e});var a=function(){document.hidden||i.dispatch("KICK")};document.addEventListener("visibilitychange",a);var u=null,s=!1,l=!1,c=null,f=null,d=function(){s||(s=!0),clearTimeout(u),u=setTimeout(function(){s=!1,c=null,f=null,l&&(l=!1,i.dispatch("DID_STOP_RESIZE"))},500)};window.addEventListener("resize",d);var p=$r(i,{id:oe()}),E=!1,T=!1,I={_read:function(){s&&(f=window.innerWidth,c||(c=f),l||f===c||(i.dispatch("DID_START_RESIZE"),l=!0)),T&&E&&(E=null===p.element.offsetParent),E||(p._read(),T=p.rect.element.hidden)},_write:function(e){var t,n=i.processActionQueue().filter(function(e){return!/^SET_/.test(e.type)});E&&!n.length||(h(n),E=p._write(e,n,l),(t=i.query("GET_ITEMS")).forEach(function(e,n){e.released&&de(t,n)}),E&&i.processDispatchQueue())}},v=function(e){return function(t){var n={type:e};if(!t)return n;if(t.hasOwnProperty("error")&&(n.error=t.error?Object.assign({},t.error):null),t.status&&(n.status=Object.assign({},t.status)),t.file&&(n.output=t.file),t.source)n.file=t.source;else if(t.item||t.id){var r=t.item?t.item:i.query("GET_ITEM",t.id);n.file=r?Te(r):null}return t.items&&(n.items=t.items.map(Te)),/progress/.test(e)&&(n.progress=t.progress),t.hasOwnProperty("origin")&&t.hasOwnProperty("target")&&(n.origin=t.origin,n.target=t.target),n}},m={DID_DESTROY:v("destroy"),DID_INIT:v("init"),DID_THROW_MAX_FILES:v("warning"),DID_INIT_ITEM:v("initfile"),DID_START_ITEM_LOAD:v("addfilestart"),DID_UPDATE_ITEM_LOAD_PROGRESS:v("addfileprogress"),DID_LOAD_ITEM:v("addfile"),DID_THROW_ITEM_INVALID:[v("error"),v("addfile")],DID_THROW_ITEM_LOAD_ERROR:[v("error"),v("addfile")],DID_THROW_ITEM_REMOVE_ERROR:[v("error"),v("removefile")],DID_PREPARE_OUTPUT:v("preparefile"),DID_START_ITEM_PROCESSING:v("processfilestart"),DID_UPDATE_ITEM_PROCESS_PROGRESS:v("processfileprogress"),DID_ABORT_ITEM_PROCESSING:v("processfileabort"),DID_COMPLETE_ITEM_PROCESSING:v("processfile"),DID_COMPLETE_ITEM_PROCESSING_ALL:v("processfiles"),DID_REVERT_ITEM_PROCESSING:v("processfilerevert"),DID_THROW_ITEM_PROCESSING_ERROR:[v("error"),v("processfile")],DID_REMOVE_ITEM:v("removefile"),DID_UPDATE_ITEMS:v("updatefiles"),DID_ACTIVATE_ITEM:v("activatefile"),DID_REORDER_ITEMS:v("reorderfiles")},g=function(e){var t=Object.assign({pond:A},e);delete t.type,p.element.dispatchEvent(new CustomEvent("FilePond:"+e.type,{detail:t,bubbles:!0,cancelable:!0,composed:!0}));var n=[];e.hasOwnProperty("error")&&n.push(e.error),e.hasOwnProperty("file")&&n.push(e.file);var r=["type","error","file"];Object.keys(e).filter(function(e){return!r.includes(e)}).forEach(function(t){return n.push(e[t])}),A.fire.apply(A,[e.type].concat(n));var o=i.query("GET_ON"+e.type.toUpperCase());o&&o.apply(void 0,n)},h=function(e){e.length&&e.filter(function(e){return m[e.type]}).forEach(function(e){var t=m[e.type];(Array.isArray(t)?t:[t]).forEach(function(t){"DID_INIT_ITEM"===e.type?g(t(e.data)):setTimeout(function(){g(t(e.data))},0)})})},R=function(e){return new Promise(function(t,n){i.dispatch("REQUEST_ITEM_PREPARE",{query:e,success:function(e){t(e)},failure:function(e){n(e)}})})},O=function(e,t){var n;return"object"!=typeof e||(n=e).file&&n.id||t||(t=e,e=void 0),i.dispatch("REMOVE_ITEM",Object.assign({},t,{query:e})),null===i.query("GET_ACTIVE_ITEM",e)},y=function(){for(var e=arguments.length,t=new Array(e),n=0;n<e;n++)t[n]=arguments[n];return new Promise(function(e,n){var r=[],o={};if(L(t[0]))r.push.apply(r,t[0]),Object.assign(o,t[1]||{});else{var a=t[t.length-1];"object"!=typeof a||a instanceof Blob||Object.assign(o,t.pop()),r.push.apply(r,t)}i.dispatch("ADD_ITEMS",{items:r,index:o.index,interactionMethod:J,success:e,failure:n})})},D=function(){return i.query("GET_ACTIVE_ITEMS")},S=function(e){return new Promise(function(t,n){i.dispatch("REQUEST_ITEM_PROCESSING",{query:e,success:function(e){t(e)},failure:function(e){n(e)}})})},A=Object.assign({},pe(),{},I,{},function(e,n){var r={};return t(n,function(t){r[t]={get:function(){return e.getState().options[t]},set:function(n){e.dispatch("SET_"+$(t,"_").toUpperCase(),{value:n})}}}),r}(i,o),{setOptions:function(e){return i.dispatch("SET_OPTIONS",{options:e})},addFile:function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{};return new Promise(function(n,r){y([{source:e,options:t}],{index:t.index}).then(function(e){return n(e&&e[0])}).catch(r)})},addFiles:y,getFile:function(e){return i.query("GET_ACTIVE_ITEM",e)},processFile:S,prepareFile:R,removeFile:O,moveFile:function(e,t){return i.dispatch("MOVE_ITEM",{query:e,index:t})},getFiles:D,processFiles:function(){for(var e=arguments.length,t=new Array(e),n=0;n<e;n++)t[n]=arguments[n];var r=Array.isArray(t[0])?t[0]:t;if(!r.length){var o=D().filter(function(e){return!(e.status===Ie.IDLE&&e.origin===ve.LOCAL)&&e.status!==Ie.PROCESSING&&e.status!==Ie.PROCESSING_COMPLETE&&e.status!==Ie.PROCESSING_REVERT_ERROR});return Promise.all(o.map(S))}return Promise.all(r.map(S))},removeFiles:function(){for(var e=arguments.length,t=new Array(e),n=0;n<e;n++)t[n]=arguments[n];var r,o=Array.isArray(t[0])?t[0]:t;"object"==typeof o[o.length-1]?r=o.pop():Array.isArray(t[0])&&(r=t[1]);var i=D();return o.length?o.map(function(e){return _(e)?i[e]?i[e].id:null:e}).filter(function(e){return e}).map(function(e){return O(e,r)}):Promise.all(i.map(function(e){return O(e,r)}))},prepareFiles:function(){for(var e=arguments.length,t=new Array(e),n=0;n<e;n++)t[n]=arguments[n];var r=Array.isArray(t[0])?t[0]:t,o=r.length?r:D();return Promise.all(o.map(R))},sort:function(e){return i.dispatch("SORT",{compare:e})},browse:function(){var e=p.element.querySelector("input[type=file]");e&&e.click()},destroy:function(){A.fire("destroy",p.element),i.dispatch("ABORT_ALL"),p._destroy(),window.removeEventListener("resize",d),document.removeEventListener("visibilitychange",a),i.dispatch("DID_DESTROY")},insertBefore:function(e){return b(p.element,e)},insertAfter:function(e){return P(p.element,e)},appendTo:function(e){return e.appendChild(p.element)},replaceElement:function(e){b(p.element,e),e.parentNode.removeChild(e),r=e},restoreElement:function(){r&&(P(r,p.element),p.element.parentNode.removeChild(p.element),r=null)},isAttachedTo:function(e){return p.element===e||r===e},element:{get:function(){return p.element}},status:{get:function(){return i.query("GET_STATUS")}}});return i.dispatch("DID_INIT"),n(A)},Kr=function(){var e=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},n={};return t(Se(),function(e,t){n[e]=t[0]}),Zr(Object.assign({},n,{},e))},Jr=function(e){var n=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{},o=[];t(e.attributes,function(t){o.push(e.attributes[t])});var i=o.filter(function(e){return e.name}).reduce(function(t,n){var o,i=r(e,n.name);return t[(o=n.name,qr(o.replace(/^data-/,"")))]=i===n.name||i,t},{});return function e(n,r){t(r,function(r,o){t(n,function(e,t){var i=new RegExp(r);if(i.test(e)&&(delete n[e],!1!==o))if(U(o))n[o]=t;else{var a,u=o.group;H(o)&&!n[u]&&(n[u]={}),n[u][(a=e.replace(i,""),a.charAt(0).toLowerCase()+a.slice(1))]=t}}),o.mapping&&e(n[o.group],o.mapping)})}(i,n),i},eo=function(){return(arguments.length<=0?void 0:arguments[0])instanceof HTMLElement?function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{},n={"^class$":"className","^multiple$":"allowMultiple","^capture$":"captureMethod","^webkitdirectory$":"allowDirectoriesOnly","^server":{group:"server",mapping:{"^process":{group:"process"},"^revert":{group:"revert"},"^fetch":{group:"fetch"},"^restore":{group:"restore"},"^load":{group:"load"}}},"^type$":!1,"^files$":!1};ye("SET_ATTRIBUTE_TO_OPTION_MAP",n);var r=Object.assign({},t),o=Jr("FIELDSET"===e.nodeName?e.querySelector("input[type=file]"):e,n);Object.keys(o).forEach(function(e){H(o[e])?(H(r[e])||(r[e]={}),Object.assign(r[e],o[e])):r[e]=o[e]}),r.files=(t.files||[]).concat(Array.from(e.querySelectorAll("input:not([type=file])")).map(function(e){return{source:e.value,options:{type:e.dataset.type}}}));var i=Kr(r);return e.files&&Array.from(e.files).forEach(function(e){i.addFile(e)}),i.replaceElement(e),i}.apply(void 0,arguments):Kr.apply(void 0,arguments)},to=["fire","_read","_write"],no=function(e){var t={};return Ee(e,t,to),t},ro=function(e,t){return e.replace(/(?:{([a-zA-Z]+)})/g,function(e,n){return t[n]})},oo=function(e){var t=new Blob(["(",e.toString(),")()"],{type:"application/javascript"}),n=URL.createObjectURL(t),r=new Worker(n);return{transfer:function(e,t){},post:function(e,t,n){var o=oe();r.onmessage=function(e){e.data.id===o&&t(e.data.message)},r.postMessage({id:o,message:e},n)},terminate:function(){r.terminate(),URL.revokeObjectURL(n)}}},io=function(e){return new Promise(function(t,n){var r=new Image;r.onload=function(){t(r)},r.onerror=function(e){n(e)},r.src=e})},ao=function(e,t){var n=e.slice(0,e.size,e.type);return n.lastModifiedDate=e.lastModifiedDate,n.name=t,n},uo=function(e){return ao(e,e.name)},so=[],lo=function(e){if(!so.includes(e)){so.push(e);var n,r=e({addFilter:De,utils:{Type:he,forin:t,isString:U,isFile:vt,toNaturalFileSize:Ut,replaceInString:ro,getExtensionFromFilename:Ye,getFilenameWithoutExtension:It,guesstimateMimeType:Jn,getFileFromBlob:He,getFilenameFromURL:xe,createRoute:A,createWorker:oo,createView:S,createItemAPI:Te,loadImage:io,copyFile:uo,renameFile:ao,createBlob:Xe,applyFilterChain:Oe,text:Mt,getNumericAspectRatioFromString:Pe},views:{fileActionButton:Gt}});n=r.options,Object.assign(Ae,n)}},co=(Zt=c()&&!("[object OperaMini]"===Object.prototype.toString.call(window.operamini))&&"visibilityState"in document&&"Promise"in window&&"slice"in Blob.prototype&&"URL"in window&&"createObjectURL"in window.URL&&"performance"in window&&("supports"in(window.CSS||{})||/MSIE|Trident/.test(window.navigator.userAgent)),function(){return Zt}),fo={apps:[]},po=function(){};if(e.Status={},e.FileStatus={},e.FileOrigin={},e.OptionTypes={},e.create=po,e.destroy=po,e.parse=po,e.find=po,e.registerPlugin=po,e.getOptions=po,e.setOptions=po,co()){!function(e,t){var n=arguments.length>2&&void 0!==arguments[2]?arguments[2]:60,r="__framePainter";if(window[r])return window[r].readers.push(e),void window[r].writers.push(t);window[r]={readers:[e],writers:[t]};var o=window[r],i=1e3/n,a=null,u=null,s=null,l=null,c=function(){document.hidden?(s=function(){return window.setTimeout(function(){return f(performance.now())},i)},l=function(){return window.clearTimeout(u)}):(s=function(){return window.requestAnimationFrame(f)},l=function(){return window.cancelAnimationFrame(u)})};document.addEventListener("visibilitychange",function(){l&&l(),c(),f(performance.now())});var f=function e(t){u=s(e),a||(a=t);var n=t-a;n<=i||(a=t-n%i,o.readers.forEach(function(e){return e()}),o.writers.forEach(function(e){return e(t)}))};c(),f(performance.now())}(function(){fo.apps.forEach(function(e){return e._read()})},function(e){fo.apps.forEach(function(t){return t._write(e)})});var Eo=function t(){document.dispatchEvent(new CustomEvent("FilePond:loaded",{detail:{supported:co,create:e.create,destroy:e.destroy,parse:e.parse,find:e.find,registerPlugin:e.registerPlugin,setOptions:e.setOptions}})),document.removeEventListener("DOMContentLoaded",t)};"loading"!==document.readyState?setTimeout(function(){return Eo()},0):document.addEventListener("DOMContentLoaded",Eo);var _o=function(){return t(Se(),function(t,n){e.OptionTypes[t]=n[1]})};e.Status=Object.assign({},Me),e.FileOrigin=Object.assign({},ve),e.FileStatus=Object.assign({},Ie),e.OptionTypes={},_o(),e.create=function(){var t=eo.apply(void 0,arguments);return t.on("destroy",e.destroy),fo.apps.push(t),no(t)},e.destroy=function(e){var t=fo.apps.findIndex(function(t){return t.isAttachedTo(e)});return t>=0&&(fo.apps.splice(t,1)[0].restoreElement(),!0)},e.parse=function(t){return Array.from(t.querySelectorAll(".filepond")).filter(function(e){return!fo.apps.find(function(t){return t.isAttachedTo(e)})}).map(function(t){return e.create(t)})},e.find=function(e){var t=fo.apps.find(function(t){return t.isAttachedTo(e)});return t?no(t):null},e.registerPlugin=function(){for(var e=arguments.length,t=new Array(e),n=0;n<e;n++)t[n]=arguments[n];t.forEach(lo),_o()},e.getOptions=function(){var e={};return t(Se(),function(t,n){e[t]=n[0]}),e},e.setOptions=function(n){return H(n)&&(fo.apps.forEach(function(e){e.setOptions(n)}),function(e){t(e,function(e,t){Ae[e]&&(Ae[e][0]=z(t,Ae[e][0],Ae[e][1]))})}(n)),e.getOptions()}}e.supported=co,Object.defineProperty(e,"__esModule",{value:!0})});

/*!
 * FilePondPluginImagePreview 4.6.4
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

!function(e,t){"object"==typeof exports&&"undefined"!=typeof module?module.exports=t():"function"==typeof define&&define.amd?define(t):(e=e||self).FilePondPluginImagePreview=t()}(this,function(){"use strict";function e(e){this.wrapped=e}function t(t){var i,r;function a(i,r){try{var o=t[i](r),c=o.value,s=c instanceof e;Promise.resolve(s?c.wrapped:c).then(function(e){s?a("next",e):n(o.done?"return":"normal",e)},function(e){a("throw",e)})}catch(e){n("throw",e)}}function n(e,t){switch(e){case"return":i.resolve({value:t,done:!0});break;case"throw":i.reject(t);break;default:i.resolve({value:t,done:!1})}(i=i.next)?a(i.key,i.arg):r=null}this._invoke=function(e,t){return new Promise(function(n,o){var c={key:e,arg:t,resolve:n,reject:o,next:null};r?r=r.next=c:(i=r=c,a(e,t))})},"function"!=typeof t.return&&(this.return=void 0)}"function"==typeof Symbol&&Symbol.asyncIterator&&(t.prototype[Symbol.asyncIterator]=function(){return this}),t.prototype.next=function(e){return this._invoke("next",e)},t.prototype.throw=function(e){return this._invoke("throw",e)},t.prototype.return=function(e){return this._invoke("return",e)};function i(e,t){return r(e)||function(e,t){var i=[],r=!0,a=!1,n=void 0;try{for(var o,c=e[Symbol.iterator]();!(r=(o=c.next()).done)&&(i.push(o.value),!t||i.length!==t);r=!0);}catch(e){a=!0,n=e}finally{try{r||null==c.return||c.return()}finally{if(a)throw n}}return i}(e,t)||a()}function r(e){if(Array.isArray(e))return e}function a(){throw new TypeError("Invalid attempt to destructure non-iterable instance")}var n=function(e,t){return s(e.x*t,e.y*t)},o=function(e,t){return s(e.x+t.x,e.y+t.y)},c=function(e,t,i){var r=Math.cos(t),a=Math.sin(t),n=s(e.x-i.x,e.y-i.y);return s(i.x+r*n.x-a*n.y,i.y+a*n.x+r*n.y)},s=function(){return{x:arguments.length>0&&void 0!==arguments[0]?arguments[0]:0,y:arguments.length>1&&void 0!==arguments[1]?arguments[1]:0}},h=function(e,t){var i=arguments.length>2&&void 0!==arguments[2]?arguments[2]:1,r=arguments.length>3?arguments[3]:void 0;return"string"==typeof e?parseFloat(e)*i:"number"==typeof e?e*(r?t[r]:Math.min(t.width,t.height)):void 0},u=function(e){return null!=e},l=function(e,t){return Object.keys(t).forEach(function(i){return e.setAttribute(i,t[i])})},d=function(e,t){var i=document.createElementNS("http://www.w3.org/2000/svg",e);return t&&l(i,t),i},f={contain:"xMidYMid meet",cover:"xMidYMid slice"},g={left:"start",center:"middle",right:"end"},p=function(e){return function(t){return d(e,{id:t.id})}},m={image:function(e){var t=d("image",{id:e.id,"stroke-linecap":"round","stroke-linejoin":"round",opacity:"0"});return t.onload=function(){t.setAttribute("opacity",e.opacity||1)},t.setAttributeNS("http://www.w3.org/1999/xlink","xlink:href",e.src),t},rect:p("rect"),ellipse:p("ellipse"),text:p("text"),path:p("path"),line:function(e){var t=d("g",{id:e.id,"stroke-linecap":"round","stroke-linejoin":"round"}),i=d("line");t.appendChild(i);var r=d("path");t.appendChild(r);var a=d("path");return t.appendChild(a),t}},y={rect:function(e){return l(e,Object.assign({},e.rect,e.styles))},ellipse:function(e){var t=e.rect.x+.5*e.rect.width,i=e.rect.y+.5*e.rect.height,r=.5*e.rect.width,a=.5*e.rect.height;return l(e,Object.assign({cx:t,cy:i,rx:r,ry:a},e.styles))},image:function(e,t){l(e,Object.assign({},e.rect,e.styles,{preserveAspectRatio:f[t.fit]||"none"}))},text:function(e,t,i,r){var a=h(t.fontSize,i,r),n=t.fontFamily||"sans-serif",o=t.fontWeight||"normal",c=g[t.textAlign]||"start";l(e,Object.assign({},e.rect,e.styles,{"stroke-width":0,"font-weight":o,"font-size":a,"font-family":n,"text-anchor":c})),e.text!==t.text&&(e.text=t.text,e.textContent=t.text.length?t.text:" ")},path:function(e,t,i,r){var a;l(e,Object.assign({},e.styles,{fill:"none",d:(a=t.points.map(function(e){return{x:h(e.x,i,r,"width"),y:h(e.y,i,r,"height")}}),a.map(function(e,t){return"".concat(0===t?"M":"L"," ").concat(e.x," ").concat(e.y)}).join(" "))}))},line:function(e,t,i,r){l(e,Object.assign({},e.rect,e.styles,{fill:"none"}));var a=e.childNodes[0],u=e.childNodes[1],d=e.childNodes[2],f=e.rect,g={x:e.rect.x+e.rect.width,y:e.rect.y+e.rect.height};if(l(a,{x1:f.x,y1:f.y,x2:g.x,y2:g.y}),t.lineDecoration){u.style.display="none",d.style.display="none";var p=function(e){var t=Math.sqrt(e.x*e.x+e.y*e.y);return 0===t?{x:0,y:0}:s(e.x/t,e.y/t)}({x:g.x-f.x,y:g.y-f.y}),m=h(.05,i,r);if(-1!==t.lineDecoration.indexOf("arrow-begin")){var y=n(p,m),E=o(f,y),v=c(f,2,E),w=c(f,-2,E);l(u,{style:"display:block;",d:"M".concat(v.x,",").concat(v.y," L").concat(f.x,",").concat(f.y," L").concat(w.x,",").concat(w.y)})}if(-1!==t.lineDecoration.indexOf("arrow-end")){var _=n(p,-m),I=o(g,_),M=c(g,2,I),x=c(g,-2,I);l(d,{style:"display:block;",d:"M".concat(M.x,",").concat(M.y," L").concat(g.x,",").concat(g.y," L").concat(x.x,",").concat(x.y)})}}}},E=function(e,t,i,r,a){"path"!==t&&(e.rect=function(e,t){var i=arguments.length>2&&void 0!==arguments[2]?arguments[2]:1,r=h(e.x,t,i,"width")||h(e.left,t,i,"width"),a=h(e.y,t,i,"height")||h(e.top,t,i,"height"),n=h(e.width,t,i,"width"),o=h(e.height,t,i,"height"),c=h(e.right,t,i,"width"),s=h(e.bottom,t,i,"height");return u(a)||(a=u(o)&&u(s)?t.height-o-s:s),u(r)||(r=u(n)&&u(c)?t.width-n-c:c),u(n)||(n=u(r)&&u(c)?t.width-r-c:0),u(o)||(o=u(a)&&u(s)?t.height-a-s:0),{x:r||0,y:a||0,width:n||0,height:o||0}}(i,r,a)),e.styles=function(e,t,i){var r=e.borderStyle||e.lineStyle||"solid",a=e.backgroundColor||e.fontColor||"transparent",n=e.borderColor||e.lineColor||"transparent",o=h(e.borderWidth||e.lineWidth,t,i);return{"stroke-linecap":e.lineCap||"round","stroke-linejoin":e.lineJoin||"round","stroke-width":o||0,"stroke-dasharray":"string"==typeof r?"":r.map(function(e){return h(e,t,i)}).join(","),stroke:n,fill:a,opacity:e.opacity||1}}(i,r,a),y[t](e,i,r,a)},v=["x","y","left","top","right","bottom","width","height"],w=function(e){var t=i(e,2),r=t[0],a=t[1],n=a.points?{}:v.reduce(function(e,t){var i;return e[t]="string"==typeof(i=a[t])&&/%/.test(i)?parseFloat(i)/100:i,e},{});return[r,Object.assign({zIndex:0},a,n)]},_=function(e,t){return e[1].zIndex>t[1].zIndex?1:e[1].zIndex<t[1].zIndex?-1:0},I=function(e){return e.utils.createView({name:"image-preview-markup",tag:"svg",ignoreRect:!0,mixins:{apis:["width","height","crop","markup","resize","dirty"]},write:function(e){var t=e.root,r=e.props;if(r.dirty){var a=r.crop,n=r.resize,o=r.markup,c=r.width,s=r.height,h=a.width,u=a.height;if(n){var l=n.size,d=l&&l.width,f=l&&l.height,g=n.mode,p=n.upscale;d&&!f&&(f=d),f&&!d&&(d=f);var y=h<d&&u<f;if(!y||y&&p){var v,I=d/h,M=f/u;if("force"===g)h=d,u=f;else"cover"===g?v=Math.max(I,M):"contain"===g&&(v=Math.min(I,M)),h*=v,u*=v}}var x={width:c,height:s};t.element.setAttribute("width",x.width),t.element.setAttribute("height",x.height);var T=Math.min(c/h,s/u);t.element.innerHTML="";var A=t.query("GET_IMAGE_PREVIEW_MARKUP_FILTER");o.filter(A).map(w).sort(_).forEach(function(e){var r=i(e,2),a=r[0],n=r[1],o=function(e,t){return m[e](t)}(a,n);E(o,a,n,x,T),t.element.appendChild(o)})}}})},M=function(e,t){return{x:e,y:t}},x=function(e,t){return M(e.x-t.x,e.y-t.y)},T=function(e,t){return Math.sqrt(function(e,t){return function(e,t){return e.x*t.x+e.y*t.y}(x(e,t),x(e,t))}(e,t))},A=function(e,t){var i=e,r=t,a=1.5707963267948966-t,n=Math.sin(1.5707963267948966),o=Math.sin(r),c=Math.sin(a),s=Math.cos(a),h=i/n;return M(s*(h*o),s*(h*c))},R=function(e,t,i,r){var a=r.x>.5?1-r.x:r.x,n=r.y>.5?1-r.y:r.y,o=2*a*e.width,c=2*n*e.height,s=function(e,t){var i=e.width,r=e.height,a=A(i,t),n=A(r,t),o=M(e.x+Math.abs(a.x),e.y-Math.abs(a.y)),c=M(e.x+e.width+Math.abs(n.y),e.y+Math.abs(n.x)),s=M(e.x-Math.abs(n.y),e.y+e.height-Math.abs(n.x));return{width:T(o,c),height:T(o,s)}}(t,i);return Math.max(s.width/o,s.height/c)},P=function(e,t){var i=e.width,r=i*t;return r>e.height&&(i=(r=e.height)/t),{x:.5*(e.width-i),y:.5*(e.height-r),width:i,height:r}},C=function(e){var t=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{},i=t.zoom,r=t.rotation,a=t.center,n=t.aspectRatio;n||(n=e.height/e.width);var o=function(e,t){var i=arguments.length>2&&void 0!==arguments[2]?arguments[2]:1,r=e.height/e.width,a=t,n=1,o=r;o>a&&(n=(o=a)/r);var c=Math.max(1/n,a/o),s=e.width/(i*c*n);return{width:s,height:s*t}}(e,n,i),c={x:.5*o.width,y:.5*o.height},s={x:0,y:0,width:o.width,height:o.height,center:c},h=void 0===t.scaleToFit||t.scaleToFit,u=i*R(e,P(s,n),r,h?a:{x:.5,y:.5});return{widthFloat:o.width/u,heightFloat:o.height/u,width:Math.round(o.width/u),height:Math.round(o.height/u)}},k={type:"spring",stiffness:.5,damping:.45,mass:10},D=function(e){return e.utils.createView({name:"image-clip",tag:"div",ignoreRect:!0,mixins:{apis:["crop","markup","resize","width","height","dirty","background"],styles:["width","height","opacity"],animations:{opacity:{type:"tween",duration:250}}},didWriteView:function(e){var t=e.root,i=e.props;i.background&&(t.element.style.backgroundColor=i.background)},create:function(t){var i=t.root,r=t.props;i.ref.image=i.appendChildView(i.createChildView(function(e){return e.utils.createView({name:"image-canvas-wrapper",tag:"div",ignoreRect:!0,mixins:{apis:["crop","width","height"],styles:["originX","originY","translateX","translateY","scaleX","scaleY","rotateZ"],animations:{originX:k,originY:k,scaleX:k,scaleY:k,translateX:k,translateY:k,rotateZ:k}},create:function(t){var i=t.root,r=t.props;r.width=r.image.width,r.height=r.image.height,i.ref.bitmap=i.appendChildView(i.createChildView(function(e){return e.utils.createView({name:"image-bitmap",ignoreRect:!0,mixins:{styles:["scaleX","scaleY"]},create:function(e){var t=e.root,i=e.props;t.appendChild(i.image)}})}(e),{image:r.image}))},write:function(e){var t=e.root,i=e.props.crop.flip,r=t.ref.bitmap;r.scaleX=i.horizontal?-1:1,r.scaleY=i.vertical?-1:1}})}(e),Object.assign({},r))),i.ref.createMarkup=function(){i.ref.markup||(i.ref.markup=i.appendChildView(i.createChildView(I(e),Object.assign({},r))))},i.ref.destroyMarkup=function(){i.ref.markup&&(i.removeChildView(i.ref.markup),i.ref.markup=null)};var a=i.query("GET_IMAGE_PREVIEW_TRANSPARENCY_INDICATOR");null!==a&&(i.element.dataset.transparencyIndicator="grid"===a?a:"color")},write:function(e){var t=e.root,i=e.props,r=e.shouldOptimize,a=i.crop,n=i.markup,o=i.resize,c=i.dirty,s=i.width,h=i.height;t.ref.image.crop=a;var u={x:0,y:0,width:s,height:h,center:{x:.5*s,y:.5*h}},l={width:t.ref.image.width,height:t.ref.image.height},d={x:a.center.x*l.width,y:a.center.y*l.height},f={x:u.center.x-l.width*a.center.x,y:u.center.y-l.height*a.center.y},g=2*Math.PI+a.rotation%(2*Math.PI),p=a.aspectRatio||l.height/l.width,m=void 0===a.scaleToFit||a.scaleToFit,y=R(l,P(u,p),g,m?a.center:{x:.5,y:.5}),E=a.zoom*y;n&&n.length?(t.ref.createMarkup(),t.ref.markup.width=s,t.ref.markup.height=h,t.ref.markup.resize=o,t.ref.markup.dirty=c,t.ref.markup.markup=n,t.ref.markup.crop=C(l,a)):t.ref.markup&&t.ref.destroyMarkup();var v=t.ref.image;if(r)return v.originX=null,v.originY=null,v.translateX=null,v.translateY=null,v.rotateZ=null,v.scaleX=null,void(v.scaleY=null);v.originX=d.x,v.originY=d.y,v.translateX=f.x,v.translateY=f.y,v.rotateZ=g,v.scaleX=E,v.scaleY=E}})},G='<svg width="500" height="200" viewBox="0 0 500 200" preserveAspectRatio="none">\n    <defs>\n        <radialGradient id="gradient-__UID__" cx=".5" cy="1.25" r="1.15">\n            <stop offset=\'50%\' stop-color=\'#000000\'/>\n            <stop offset=\'56%\' stop-color=\'#0a0a0a\'/>\n            <stop offset=\'63%\' stop-color=\'#262626\'/>\n            <stop offset=\'69%\' stop-color=\'#4f4f4f\'/>\n            <stop offset=\'75%\' stop-color=\'#808080\'/>\n            <stop offset=\'81%\' stop-color=\'#b1b1b1\'/>\n            <stop offset=\'88%\' stop-color=\'#dadada\'/>\n            <stop offset=\'94%\' stop-color=\'#f6f6f6\'/>\n            <stop offset=\'100%\' stop-color=\'#ffffff\'/>\n        </radialGradient>\n        <mask id="mask-__UID__">\n            <rect x="0" y="0" width="500" height="200" fill="url(#gradient-__UID__)"></rect>\n        </mask>\n    </defs>\n    <rect x="0" width="500" height="200" fill="currentColor" mask="url(#mask-__UID__)"></rect>\n</svg>',V=!1,O=0,b=function(){self.onmessage=function(e){createImageBitmap(e.data.message.file).then(function(t){self.postMessage({id:e.data.id,message:t},[t])})}},S=function(){self.onmessage=function(e){for(var t=e.data.message.imageData,i=e.data.message.colorMatrix,r=t.data,a=r.length,n=i[0],o=i[1],c=i[2],s=i[3],h=i[4],u=i[5],l=i[6],d=i[7],f=i[8],g=i[9],p=i[10],m=i[11],y=i[12],E=i[13],v=i[14],w=i[15],_=i[16],I=i[17],M=i[18],x=i[19],T=0,A=0,R=0,P=0,C=0;T<a;T+=4)A=r[T]/255,R=r[T+1]/255,P=r[T+2]/255,C=r[T+3]/255,r[T]=Math.max(0,Math.min(255*(A*n+R*o+P*c+C*s+h),255)),r[T+1]=Math.max(0,Math.min(255*(A*u+R*l+P*d+C*f+g),255)),r[T+2]=Math.max(0,Math.min(255*(A*p+R*m+P*y+C*E+v),255)),r[T+3]=Math.max(0,Math.min(255*(A*w+R*_+P*I+C*M+x),255));self.postMessage({id:e.data.id,message:t},[t.data.buffer])}},L={1:function(){return[1,0,0,1,0,0]},2:function(e){return[-1,0,0,1,e,0]},3:function(e,t){return[-1,0,0,-1,e,t]},4:function(e,t){return[1,0,0,-1,0,t]},5:function(){return[0,1,1,0,0,0]},6:function(e,t){return[0,1,-1,0,t,0]},7:function(e,t){return[0,-1,-1,0,t,e]},8:function(e){return[0,-1,1,0,0,e]}},N=function(e,t,i,r){t=Math.round(t),i=Math.round(i);var a=document.createElement("canvas");a.width=t,a.height=i;var n=a.getContext("2d");if(r>=5&&r<=8){var o=[i,t];t=o[0],i=o[1]}return function(e,t,i,r){-1!==r&&e.transform.apply(e,L[r](t,i))}(n,t,i,r),n.drawImage(e,0,0,t,i),a},W=function(e){return/^image/.test(e.type)&&!/svg/.test(e.type)},z=function(e){var t=Math.min(10/e.width,10/e.height),i=document.createElement("canvas"),r=i.getContext("2d"),a=i.width=Math.ceil(e.width*t),n=i.height=Math.ceil(e.height*t);r.drawImage(e,0,0,a,n);var o=null;try{o=r.getImageData(0,0,a,n).data}catch(e){return null}for(var c=o.length,s=0,h=0,u=0,l=0;l<c;l+=4)s+=o[l]*o[l],h+=o[l+1]*o[l+1],u+=o[l+2]*o[l+2];return{r:s=H(s,c),g:h=H(h,c),b:u=H(u,c)}},H=function(e,t){return Math.floor(Math.sqrt(e/(t/4)))},q=function(e){var t=e.utils.createView({name:"image-preview-overlay",tag:"div",ignoreRect:!0,create:function(e){var t=e.root,i=e.props;!V&&document.querySelector("base")&&(G=G.replace(/url\(\#/g,"url("+window.location.href.replace(window.location.hash,"")+"#"),V=!0),O++,t.element.classList.add("filepond--image-preview-overlay-".concat(i.status)),t.element.innerHTML=G.replace(/__UID__/g,O)},mixins:{styles:["opacity"],animations:{opacity:{type:"spring",mass:25}}}}),i=function(e){return e.utils.createView({name:"image-preview",tag:"div",ignoreRect:!0,mixins:{apis:["image","crop","markup","resize","dirty","background"],styles:["translateY","scaleX","scaleY","opacity"],animations:{scaleX:k,scaleY:k,translateY:k,opacity:{type:"tween",duration:400}}},create:function(t){var i=t.root,r=t.props;i.ref.clip=i.appendChildView(i.createChildView(D(e),{id:r.id,image:r.image,crop:r.crop,markup:r.markup,resize:r.resize,dirty:r.dirty,background:r.background}))},write:function(e){var t=e.root,i=e.props,r=e.shouldOptimize,a=t.ref.clip,n=i.image,o=i.crop,c=i.markup,s=i.resize,h=i.dirty;if(a.crop=o,a.markup=c,a.resize=s,a.dirty=h,a.opacity=r?0:1,!r&&!t.rect.element.hidden){var u=n.height/n.width,l=o.aspectRatio||u,d=t.rect.inner.width,f=t.rect.inner.height,g=t.query("GET_IMAGE_PREVIEW_HEIGHT"),p=t.query("GET_IMAGE_PREVIEW_MIN_HEIGHT"),m=t.query("GET_IMAGE_PREVIEW_MAX_HEIGHT"),y=t.query("GET_PANEL_ASPECT_RATIO"),E=t.query("GET_ALLOW_MULTIPLE");y&&!E&&(g=d*y,l=y);var v=null!==g?g:Math.max(p,Math.min(d*l,m)),w=v/l;w>d&&(v=(w=d)*l),v>f&&(v=f,w=f/l),a.width=w,a.height=v}}})}(e),r=e.utils.createWorker,a=function(e,t,i){return new Promise(function(a){e.ref.imageData||(e.ref.imageData=i.getContext("2d").getImageData(0,0,i.width,i.height));var n=function(e){var t;try{t=new ImageData(e.width,e.height)}catch(i){t=document.createElement("canvas").getContext("2d").createImageData(e.width,e.height)}return t.data.set(new Uint8ClampedArray(e.data)),t}(e.ref.imageData);if(!t||20!==t.length)return i.getContext("2d").putImageData(n,0,0),a();var o=r(S);o.post({imageData:n,colorMatrix:t},function(e){i.getContext("2d").putImageData(e,0,0),o.terminate(),a()},[n.data.buffer])})},n=function(e){var t=e.root,r=e.props,a=e.image,n=r.id,o=t.query("GET_ITEM",{id:n});if(o){var c,s,h=o.getMetadata("crop")||{center:{x:.5,y:.5},flip:{horizontal:!1,vertical:!1},zoom:1,rotation:0,aspectRatio:null},u=t.query("GET_IMAGE_TRANSFORM_CANVAS_BACKGROUND_COLOR"),l=!1;t.query("GET_IMAGE_PREVIEW_MARKUP_SHOW")&&(c=o.getMetadata("markup")||[],s=o.getMetadata("resize"),l=!0);var d=t.appendChildView(t.createChildView(i,{id:n,image:a,crop:h,resize:s,markup:c,dirty:l,background:u,opacity:0,scaleX:1.15,scaleY:1.15,translateY:15}),t.childViews.length);t.ref.images.push(d),d.opacity=1,d.scaleX=1,d.scaleY=1,d.translateY=0,setTimeout(function(){t.dispatch("DID_IMAGE_PREVIEW_SHOW",{id:n})},250)}},o=function(e){var t=e.root;t.ref.overlayShadow.opacity=1,t.ref.overlayError.opacity=0,t.ref.overlaySuccess.opacity=0},c=function(e){var t=e.root;t.ref.overlayShadow.opacity=.25,t.ref.overlayError.opacity=1};return e.utils.createView({name:"image-preview-wrapper",create:function(e){var i=e.root;i.ref.images=[],i.ref.imageData=null,i.ref.imageViewBin=[],i.ref.overlayShadow=i.appendChildView(i.createChildView(t,{opacity:0,status:"idle"})),i.ref.overlaySuccess=i.appendChildView(i.createChildView(t,{opacity:0,status:"success"})),i.ref.overlayError=i.appendChildView(i.createChildView(t,{opacity:0,status:"failure"}))},styles:["height"],apis:["height"],destroy:function(e){e.root.ref.images.forEach(function(e){e.image.width=1,e.image.height=1})},didWriteView:function(e){e.root.ref.images.forEach(function(e){e.dirty=!1})},write:e.utils.createRoute({DID_IMAGE_PREVIEW_DRAW:function(e){var t=e.root,i=t.ref.images[t.ref.images.length-1];i.translateY=0,i.scaleX=1,i.scaleY=1,i.opacity=1},DID_IMAGE_PREVIEW_CONTAINER_CREATE:function(e){var t=e.root,i=e.props.id,r=t.query("GET_ITEM",i);if(r){var a,n,o,c=URL.createObjectURL(r.file);a=c,n=function(e,r){t.dispatch("DID_IMAGE_PREVIEW_CALCULATE_SIZE",{id:i,width:e,height:r})},(o=new Image).onload=function(){var e=o.naturalWidth,t=o.naturalHeight;o=null,n(e,t)},o.src=a}},DID_FINISH_CALCULATE_PREVIEWSIZE:function(e){var t=e.root,i=e.props,o=i.id,c=t.query("GET_ITEM",o);if(c){var s,h,u=URL.createObjectURL(c.file),l=function(){var e;(e=u,new Promise(function(t,i){var r=new Image;r.crossOrigin="Anonymous",r.onload=function(){t(r)},r.onerror=function(e){i(e)},r.src=e})).then(d)},d=function(e){URL.revokeObjectURL(u);var r=(c.getMetadata("exif")||{}).orientation||-1,o=e.width,s=e.height;if(r>=5&&r<=8){var h=[s,o];o=h[0],s=h[1]}var l=Math.max(1,.75*window.devicePixelRatio),d=t.query("GET_IMAGE_PREVIEW_ZOOM_FACTOR")*l,f=s/o,g=t.rect.element.width,p=t.rect.element.height,m=g,y=m*f;f>1?y=(m=Math.min(o,g*d))*f:m=(y=Math.min(s,p*d))/f;var E=N(e,m,y,r),v=function(){var r=t.query("GET_IMAGE_PREVIEW_CALCULATE_AVERAGE_IMAGE_COLOR")?z(data):null;c.setMetadata("color",r,!0),"close"in e&&e.close(),t.ref.overlayShadow.opacity=1,n({root:t,props:i,image:E})},w=c.getMetadata("filter");w?a(t,w,E).then(v):v()};if(s=c.file,!(((h=window.navigator.userAgent.match(/Firefox\/([0-9]+)\./))?parseInt(h[1]):null)<=58)&&"createImageBitmap"in window&&W(s)){var f=r(b);f.post({file:c.file},function(e){f.terminate(),e?d(e):l()})}else l()}},DID_UPDATE_ITEM_METADATA:function(e){var t=e.root,i=e.props,r=e.action;if(/crop|filter|markup|resize/.test(r.change.key)&&t.ref.images.length){var o=t.query("GET_ITEM",{id:i.id});if(o)if(/filter/.test(r.change.key)){var c=t.ref.images[t.ref.images.length-1];a(t,r.change.value,c.image)}else{if(/crop|markup|resize/.test(r.change.key)){var s=o.getMetadata("crop"),h=t.ref.images[t.ref.images.length-1];if(Math.abs(s.aspectRatio-h.crop.aspectRatio)>1e-5){var u=function(e){var t=e.root,i=t.ref.images.shift();return i.opacity=0,i.translateY=-15,t.ref.imageViewBin.push(i),i}({root:t});n({root:t,props:i,image:(l=u.image,(d=d||document.createElement("canvas")).width=l.width,d.height=l.height,d.getContext("2d").drawImage(l,0,0),d)})}else!function(e){var t=e.root,i=e.props,r=t.query("GET_ITEM",{id:i.id});if(r){var a=t.ref.images[t.ref.images.length-1];a.crop=r.getMetadata("crop"),a.background=t.query("GET_IMAGE_TRANSFORM_CANVAS_BACKGROUND_COLOR"),t.query("GET_IMAGE_PREVIEW_MARKUP_SHOW")&&(a.dirty=!0,a.resize=r.getMetadata("resize"),a.markup=r.getMetadata("markup"))}}({root:t,props:i})}var l,d}}},DID_THROW_ITEM_LOAD_ERROR:c,DID_THROW_ITEM_PROCESSING_ERROR:c,DID_THROW_ITEM_INVALID:c,DID_COMPLETE_ITEM_PROCESSING:function(e){var t=e.root;t.ref.overlayShadow.opacity=.25,t.ref.overlaySuccess.opacity=1},DID_START_ITEM_PROCESSING:o,DID_REVERT_ITEM_PROCESSING:o},function(e){var t=e.root,i=t.ref.imageViewBin.filter(function(e){return 0===e.opacity});t.ref.imageViewBin=t.ref.imageViewBin.filter(function(e){return e.opacity>0}),i.forEach(function(e){return function(e,t){e.removeChildView(t),t.image.width=1,t.image.height=1,t._destroy()}(t,e)}),i.length=0})})},F=function(e){var t=e.addFilter,i=e.utils,r=i.Type,a=i.createRoute,n=i.isFile,o=q(e);return t("CREATE_VIEW",function(e){var t=e.is,i=e.view,r=e.query;if(t("file")&&r("GET_ALLOW_IMAGE_PREVIEW")){var c=function(e){e.root.ref.shouldRescale=!0};i.registerWriter(a({DID_RESIZE_ROOT:c,DID_STOP_RESIZE:c,DID_LOAD_ITEM:function(e){var t=e.root,a=e.props.id,c=r("GET_ITEM",a);if(c&&n(c.file)&&!c.archived){var s=c.file;if(function(e){return/^image/.test(e.type)}(s)&&r("GET_IMAGE_PREVIEW_FILTER_ITEM")(c)){var h="createImageBitmap"in(window||{}),u=r("GET_IMAGE_PREVIEW_MAX_FILE_SIZE");if(!(!h&&u&&s.size>u)){t.ref.imagePreview=i.appendChildView(i.createChildView(o,{id:a}));var l=t.query("GET_IMAGE_PREVIEW_HEIGHT");l&&t.dispatch("DID_UPDATE_PANEL_HEIGHT",{id:c.id,height:l});var d=!h&&s.size>r("GET_IMAGE_PREVIEW_MAX_INSTANT_PREVIEW_FILE_SIZE");t.dispatch("DID_IMAGE_PREVIEW_CONTAINER_CREATE",{id:a},d)}}}},DID_IMAGE_PREVIEW_CALCULATE_SIZE:function(e){var t=e.root,i=e.action;t.ref.imageWidth=i.width,t.ref.imageHeight=i.height,t.ref.shouldRescale=!0,t.ref.shouldDrawPreview=!0,t.dispatch("KICK")},DID_UPDATE_ITEM_METADATA:function(e){var t=e.root;"crop"===e.action.change.key&&(t.ref.shouldRescale=!0)}},function(e){var t=e.root,i=e.props;t.ref.imagePreview&&(t.rect.element.hidden||(t.ref.shouldRescale&&(!function(e,t){if(e.ref.imagePreview){var i=t.id,r=e.query("GET_ITEM",{id:i});if(r){var a=e.query("GET_PANEL_ASPECT_RATIO"),n=e.query("GET_ITEM_PANEL_ASPECT_RATIO"),o=e.query("GET_IMAGE_PREVIEW_HEIGHT");if(!(a||n||o)){var c=e.ref,s=c.imageWidth,h=c.imageHeight;if(s&&h){var u=e.query("GET_IMAGE_PREVIEW_MIN_HEIGHT"),l=e.query("GET_IMAGE_PREVIEW_MAX_HEIGHT"),d=(r.getMetadata("exif")||{}).orientation||-1;if(d>=5&&d<=8){var f=[h,s];s=f[0],h=f[1]}if(!W(r.file)||e.query("GET_IMAGE_PREVIEW_UPSCALE")){var g=2048/s;s*=g,h*=g}var p=h/s,m=(r.getMetadata("crop")||{}).aspectRatio||p,y=Math.max(u,Math.min(h,l)),E=e.rect.element.width,v=Math.min(E*m,y);e.dispatch("DID_UPDATE_PANEL_HEIGHT",{id:r.id,height:v})}}}}}(t,i),t.ref.shouldRescale=!1),t.ref.shouldDrawPreview&&(requestAnimationFrame(function(){t.dispatch("DID_FINISH_CALCULATE_PREVIEWSIZE",{id:i.id})}),t.ref.shouldDrawPreview=!1)))}))}}),{options:{allowImagePreview:[!0,r.BOOLEAN],imagePreviewFilterItem:[function(){return!0},r.FUNCTION],imagePreviewHeight:[null,r.INT],imagePreviewMinHeight:[44,r.INT],imagePreviewMaxHeight:[256,r.INT],imagePreviewMaxFileSize:[null,r.INT],imagePreviewZoomFactor:[2,r.INT],imagePreviewUpscale:[!1,r.BOOLEAN],imagePreviewMaxInstantPreviewFileSize:[1e6,r.INT],imagePreviewTransparencyIndicator:[null,r.STRING],imagePreviewCalculateAverageImageColor:[!1,r.BOOLEAN],imagePreviewMarkupShow:[!0,r.BOOLEAN],imagePreviewMarkupFilter:[function(){return!0},r.FUNCTION]}}};return"undefined"!=typeof window&&void 0!==window.document&&document.dispatchEvent(new CustomEvent("FilePond:pluginloaded",{detail:F})),F});

/*!
 * FilePondPluginFileValidateSize 2.2.1
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

!function(e,i){"object"==typeof exports&&"undefined"!=typeof module?module.exports=i():"function"==typeof define&&define.amd?define(i):(e=e||self).FilePondPluginFileValidateSize=i()}(this,function(){"use strict";var e=function(e){var i=e.addFilter,l=e.utils,E=l.Type,n=l.replaceInString,_=l.toNaturalFileSize;return i("ALLOW_HOPPER_ITEM",function(e,i){var l=i.query;if(!l("GET_ALLOW_FILE_SIZE_VALIDATION"))return!0;var E=l("GET_MAX_FILE_SIZE");if(null!==E&&e.size>=E)return!1;var n=l("GET_MIN_FILE_SIZE");return!(null!==n&&e.size<=n)}),i("LOAD_FILE",function(e,i){var l=i.query;return new Promise(function(i,E){if(!l("GET_ALLOW_FILE_SIZE_VALIDATION"))return i(e);var t=l("GET_FILE_VALIDATE_SIZE_FILTER");if(t&&!t(e))return i(e);var a=l("GET_MAX_FILE_SIZE");if(null!==a&&e.size>=a)E({status:{main:l("GET_LABEL_MAX_FILE_SIZE_EXCEEDED"),sub:n(l("GET_LABEL_MAX_FILE_SIZE"),{filesize:_(a)})}});else{var I=l("GET_MIN_FILE_SIZE");if(null!==I&&e.size<=I)E({status:{main:l("GET_LABEL_MIN_FILE_SIZE_EXCEEDED"),sub:n(l("GET_LABEL_MIN_FILE_SIZE"),{filesize:_(I)})}});else{var u=l("GET_MAX_TOTAL_FILE_SIZE");if(null!==u)if(l("GET_ACTIVE_ITEMS").reduce(function(e,i){return e+i.fileSize},0)>u)return void E({status:{main:l("GET_LABEL_MAX_TOTAL_FILE_SIZE_EXCEEDED"),sub:n(l("GET_LABEL_MAX_TOTAL_FILE_SIZE"),{filesize:_(u)})}});i(e)}}})}),{options:{allowFileSizeValidation:[!0,E.BOOLEAN],maxFileSize:[null,E.INT],minFileSize:[null,E.INT],maxTotalFileSize:[null,E.INT],fileValidateSizeFilter:[null,E.FUNCTION],labelMinFileSizeExceeded:["File is too small",E.STRING],labelMinFileSize:["Minimum file size is {filesize}",E.STRING],labelMaxFileSizeExceeded:["File is too large",E.STRING],labelMaxFileSize:["Maximum file size is {filesize}",E.STRING],labelMaxTotalFileSizeExceeded:["Maximum total size exceeded",E.STRING],labelMaxTotalFileSize:["Maximum total file size is {filesize}",E.STRING]}}};return"undefined"!=typeof window&&void 0!==window.document&&document.dispatchEvent(new CustomEvent("FilePond:pluginloaded",{detail:e})),e});

/*!
 * FilePondPluginFileValidateType 1.2.5
 * Licensed under MIT, https://opensource.org/licenses/MIT/
 * Please visit https://pqina.nl/filepond/ for details.
 */

/* eslint-disable */

!function(e,t){"object"==typeof exports&&"undefined"!=typeof module?module.exports=t():"function"==typeof define&&define.amd?define(t):(e=e||self).FilePondPluginFileValidateType=t()}(this,function(){"use strict";var e=function(e){var t=e.addFilter,n=e.utils,i=n.Type,T=n.isString,E=n.replaceInString,l=n.guesstimateMimeType,o=n.getExtensionFromFilename,a=n.getFilenameFromURL,r=function(e,t){return e.some(function(e){return/\*$/.test(e)?(n=e,(/^[^\/]+/.exec(t)||[]).pop()===n.slice(0,-2)):e===t;var n})},u=function(e,t,n){if(0===t.length)return!0;var i=function(e){var t="";if(T(e)){var n=a(e),i=o(n);i&&(t=l(i))}else t=e.type;return t}(e);return n?new Promise(function(T,E){n(e,i).then(function(e){r(t,e)?T():E()}).catch(E)}):r(t,i)};return t("SET_ATTRIBUTE_TO_OPTION_MAP",function(e){return Object.assign(e,{accept:"acceptedFileTypes"})}),t("ALLOW_HOPPER_ITEM",function(e,t){var n=t.query;return!n("GET_ALLOW_FILE_TYPE_VALIDATION")||u(e,n("GET_ACCEPTED_FILE_TYPES"))}),t("LOAD_FILE",function(e,t){var n=t.query;return new Promise(function(t,i){if(n("GET_ALLOW_FILE_TYPE_VALIDATION")){var T=n("GET_ACCEPTED_FILE_TYPES"),l=n("GET_FILE_VALIDATE_TYPE_DETECT_TYPE"),o=u(e,T,l),a=function(){var e,t=T.map((e=n("GET_FILE_VALIDATE_TYPE_LABEL_EXPECTED_TYPES_MAP"),function(t){return null!==e[t]&&(e[t]||t)})).filter(function(e){return!1!==e});i({status:{main:n("GET_LABEL_FILE_TYPE_NOT_ALLOWED"),sub:E(n("GET_FILE_VALIDATE_TYPE_LABEL_EXPECTED_TYPES"),{allTypes:t.join(", "),allButLastType:t.slice(0,-1).join(", "),lastType:t[t.length-1]})}})};if("boolean"==typeof o)return o?t(e):a();o.then(function(){t(e)}).catch(a)}else t(e)})}),{options:{allowFileTypeValidation:[!0,i.BOOLEAN],acceptedFileTypes:[[],i.ARRAY],labelFileTypeNotAllowed:["File is of invalid type",i.STRING],fileValidateTypeLabelExpectedTypes:["Expects {allButLastType} or {lastType}",i.STRING],fileValidateTypeLabelExpectedTypesMap:[{},i.OBJECT],fileValidateTypeDetectType:[null,i.FUNCTION]}}};return"undefined"!=typeof window&&void 0!==window.document&&document.dispatchEvent(new CustomEvent("FilePond:pluginloaded",{detail:e})),e});
