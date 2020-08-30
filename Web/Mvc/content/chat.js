import * as signalR from "@microsoft/signalr";
import EmojiButton from "@joeattardi/emoji-button";
import tippy from 'tippy.js';
import { createPopper } from '@popperjs/core';

window.wikimvcchat = {
    connected: false,
    connection: null,
    editor: null,
    emojiBox: null,
    messageListUl: null,
    messages: [],
    tenorAPIKey: null,
    tenorSearchNext: null,
    tenorSearchTerm: '',
    userNamespace: "Users",
    prefix: "Wiki",

    init: function (url, prefix, userNamespace, tenorAPIKey, topicId, messages) {
        window.wikimvcchat.prefix = prefix;
        window.wikimvcchat.userNamespace = userNamespace;
        window.wikimvcchat.tenorAPIKey = tenorAPIKey;

        window.wikimvcchat.messageListUl = document.getElementById("wiki-talk-message-list");
        if (window.wikimvcchat.messageListUl == null) {
            return;
        }

        const chatInput = document.getElementById("wiki-talk-newmessage-input");
        const emojiButton = document.getElementById("wiki-talk-emoji");
        const editorButton = document.getElementById("wiki-talk-editor");
        const sendButton = document.getElementById('wiki-talk-send');
        if (sendButton) {
            sendButton.completelyDisabled = true;
            sendButton.disabled = true;
        }

        messages = JSON.parse(messages) || [];
        for (let i = 0; i < messages.length; i++) {
            window.wikimvcchat.addMessage(topicId, messages[i]);
        }
        if (messages.length > 1) {
            let messages = document.getElementsByClassName("wiki-message");
            let last = true;
            for (let i = messages.length - 1; i >= 0; i--) {
                let message = messages.item(i);
                if (message.classList.contains("expanded")) {
                    if (last) {
                        last = false;
                        continue;
                    }
                    message.classList.remove("expanded");
                }
            }
        }

        if (url && url.length && topicId && topicId.length) {
            window.wikimvcchat.connection = new signalR
                .HubConnectionBuilder()
                .withUrl(url)
                .withAutomaticReconnect()
                .build();

            window.wikimvcchat.connection.on("Receive", function (m) {
                window.wikimvcchat.addMessage(topicId, m);
            });

            window.wikimvcchat.connection.start().then(function () {
                window.wikimvcchat.connection.invoke("JoinTopic", topicId)
                    .then(function () {
                        window.wikimvcchat.connected = true;
                        if (sendButton) {
                            sendButton.completelyDisabled = false;
                        }
                        let readOnlyMessages = document.getElementsByClassName("wiki-message-readonly");
                        for (var i = 0; i < readOnlyMessages.length; i++) {
                            let readOnlyMessage = readOnlyMessages.item(i);
                            readOnlyMessage.classList.remove("wiki-message-readonly");
                        }
                    })
                    .catch(function (err) {
                        //return console.error(err);
                        return console.error("An error occurred while connecting to chat");
                    });
            }).catch(function (err) {
                //return console.error(err);
                return console.error("An error occurred while connecting to chat");
            });

            window.wikimvcchat.picker = new EmojiButton({
                theme: 'dark',
            });
            window.wikimvcchat.picker.on('emoji', emoji => {
                if (window.wikimvcchat.emojiBox != null) {
                    if (window.wikimvcchat.emojiBox.attachedEditor) {
                        window.wikimvcchat.emojiBox.attachedEditor.value(window.wikimvcchat.emojiBox.attachedEditor.value() + emoji);
                    } else {
                        window.wikimvcchat.emojiBox.value += emoji;
                    }
                    window.wikimvcchat.emojiBox.focus();
                    if (window.wikimvcchat.emojiBox.setSelectionRange) {
                        window.wikimvcchat.emojiBox.setSelectionRange(window.wikimvcchat.emojiBox.value.length, window.wikimvcchat.emojiBox.value.length);
                    }
                    window.wikimvcchat.emojiSendButton.disabled = false;
                }
            });

            if (chatInput) {
                chatInput.addEventListener("input", function () {
                    if (sendButton && !sendButton.completelyDisabled) {
                        sendButton.disabled = !chatInput.value || !(chatInput.value.length > 0);
                    }
                });

                chatInput.addEventListener("keypress", function (event) {
                    if (event.keyCode === 13 && !event.shiftKey) {
                        if (chatInput.attachedEditor) {
                            window.wikimvcchat.sendMessage(topicId, chatInput.attachedEditor.value());
                            chatInput.attachedEditor.value("");
                        } else {
                            window.wikimvcchat.sendMessage(topicId, chatInput.value);
                            chatInput.value = "";
                        }
                        event.preventDefault();
                        event.stopPropagation();
                    }
                });

                if (emojiButton) {
                    emojiButton.addEventListener("click", function () {
                        window.wikimvcchat.emojiBox = chatInput;
                        window.wikimvcchat.emojiSendButton = sendButton;
                        picker.togglePicker(emojiButton);
                    });
                }

                if (editorButton) {
                    editorButton.addEventListener("click", function () {
                        if (chatInput.attachedEditor == null) {
                            chatInput.attachedEditor = new EasyMDE({
                                element: chatInput,
                                indentWithTabs: false,
                                placeholder: "Add a new message",
                                tabSize: 4,
                                toolbar: ['bold', 'italic', '|', 'heading-1', 'heading-2', 'heading-3', '|', 'unordered-list', 'ordered-list', '|', 'link', 'image']
                            });
                        } else {
                            chatInput.attachedEditor.toTextArea();
                            chatInput.attachedEditor = null;
                        }
                    });
                }

                if (sendButton) {
                    sendButton.addEventListener("click", function (event) {
                        if (chatInput.attachedEditor) {
                            window.wikimvcchat.sendMessage(topicId, chatInput.attachedEditor.value());
                            chatInput.attachedEditor.value("");
                        } else {
                            window.wikimvcchat.sendMessage(topicId, chatInput.value);
                            chatInput.value = "";
                        }
                        event.preventDefault();
                        event.stopPropagation();
                    });
                }

                if (tenorAPIKey && tenorAPIKey.length) {
                    let gifButton = document.getElementById("wiki-talk-gif");
                    let tenorContainer = document.getElementById("wiki-talk-tenor");
                    window.wikimvcchat.tenorBox = createPopper(gifButton, tenorContainer, {
                        placement: 'auto',
                        modifiers: [
                            {
                                name: 'offset',
                                options: {
                                    offset: [0, 8],
                                },
                            },
                            {
                                name: 'preventOverflow',
                            },
                            {
                                name: 'flip',
                                options: {
                                    allowedAutoPlacements: ['top', 'bottom'],
                                    fallbackPlacements: ['top', 'bottom'],
                                },
                            },
                        ],
                    });
                    window.wikimvcchat.tenorBox.onHide = function () {
                        document.getElementById("wiki-talk-tenor").removeAttribute("data-show");
                    };
                    window.wikimvcchat.tenorBox.onShow = function (textBox, sendButton) {
                        window.wikimvcchat.tenorBox.editor = textBox;
                        window.wikimvcchat.tenorBox.sendButton = sendButton;

                        document.getElementById("wiki-talk-tenor").setAttribute("data-show", '');
                        document.getElementById("wiki-talk-tenor-search-text").focus();

                        if (window.wikimvcchat.tenorBox._isLoading || window.wikimvcchat.tenorBox._isLoaded) {
                            return;
                        }

                        window.wikimvcchat.tenorBox._isLoading = true;

                        document.getElementById("wiki-talk-tenor-content").innerHTML = "";

                        let lang = (navigator.languages || ["en"])[0];
                        let url = "https://api.tenor.com/v1/trending?key=" + window.wikimvcchat.tenorAPIKey
                            + "&locale=" + lang
                            + "&contentfilter=low"
                            + "&media_filter=basic"
                            + "&limit=" + 6;
                        fetch(url)
                            .then(r => r.json())
                            .then(window.wikimvcchat.tenorSearchCallback)
                            .finally(() => {
                                let url = "https://api.tenor.com/v1/categories?key=" + window.wikimvcchat.tenorAPIKey
                                    + "&locale=" + lang
                                    + "&contentfilter=low";
                                fetch(url)
                                    .then(r => r.json())
                                    .then(window.wikimvcchat.tenorCategoriesCallback)
                                    .finally(() => {
                                        window.wikimvcchat.tenorBox._isLoading = false;
                                        window.wikimvcchat.tenorBox._isLoaded = true;
                                    });
                            });
                    };
                    gifButton.addEventListener('click', function (event) {
                        if (tenorContainer.getAttribute("data-show") == null) {
                            window.wikimvcchat.tenorBox.onShow(
                                document.getElementById("wiki-talk-newmessage-input"),
                                document.getElementById('wiki-talk-send')
                            );
                        } else {
                            window.wikimvcchat.tenorBox.onHide();
                        }
                    });
                    tenorContainer.addEventListener('focusout', function (event) {
                        if (!event.relatedTarget || event.relatedTarget.closest("#wiki-talk-tenor") != tenorContainer) {
                            window.wikimvcchat.tenorBox.onHide();
                        }
                    })

                    let search = document.getElementById("wiki-talk-tenor-search-text");
                    let lang = (navigator.languages || ["en"])[0];
                    let url = "https://api.tenor.com/v1/trending_terms?key=" + window.wikimvcchat.tenorAPIKey
                        + "&locale=" + lang
                        + "&limit=" + 5;
                    fetch(url)
                        .then(r => r.json())
                        .then(response => {
                            let suggestionList = document.getElementById("wiki-talk-tenor-suggestions");
                            if (suggestionList.innerHTML.length === 0) {
                                let predictedWords = response["results"];

                                for (let i = 0; i < predictedWords.length; i++) {
                                    let option = document.createElement("option");
                                    suggestionList.appendChild(option);
                                    option.value = predictedWords[i];
                                }
                            }
                        });

                    window.wikiTenorAutosuggestXHR = new XMLHttpRequest();
                    search.addEventListener('keyup', function (event) {
                        const input = event.target;
                        let content = document.getElementById("wiki-talk-tenor-content");
                        if (event.keyCode == 13) {
                            content.innerHTML = "";
                            window.wikimvcchat.tenorSearch(input.value);
                        }

                        if (input.value.length < 3) {
                            return;
                        }
                        window.wikiTenorAutosuggestXHR.abort();
                        window.wikiTenorAutosuggestXHR.onreadystatechange = function () {
                            if (this.readyState == 4 && this.status == 200) {
                                let response = JSON.parse(this.responseText);

                                let predictedWords = response["results"];

                                let suggestionList = document.getElementById("wiki-talk-tenor-suggestions");
                                suggestionList.innerHTML = "";
                                for (let i = 0; i < predictedWords.length; i++) {
                                    let option = document.createElement("option");
                                    suggestionList.appendChild(option);
                                    option.value = predictedWords[i];
                                }
                            }
                        };
                        let lang = (navigator.languages || ["en"])[0];
                        let url = "https://api.tenor.com/v1/autocomplete?key=" + window.wikimvcchat.tenorAPIKey
                            + "&q=" + input.value
                            + "&locale=" + lang
                            + "&limit=" + 5;
                        window.wikiTenorAutosuggestXHR.open("GET", url, true);
                        window.wikiTenorAutosuggestXHR.send();
                    });

                    let content = document.getElementById("wiki-talk-tenor-content");
                    content.addEventListener("scroll", function (event) {
                        event = event || window.event;
                        let target = event.target || event.srcElement;
                        let child = target.querySelector("div:last-child");
                        if (child
                            && window.wikimvcchat.tenorSearchTerm
                            && window.wikimvcchat.tenorSearchTerm.length
                            && !target.infiniteScrollStarted
                            && target.scrollTop + target.clientHeight > child.offsetTop) {
                            target.infiniteScrollStarted = true;
                            window.wikimvcchat.tenorSearch(window.wikimvcchat.tenorSearchTerm);
                        }
                    });
                }
            }
        }
    },

    addMessage: function (topicId, message) {
        if (message == null || message.content == null || message.content.length == null || !(message.content.length > 0)) {
            return;
        }

        let content = window.wikimvcchat.getContent(message);

        let isReply = message.replyMessageId != null && message.replyMessageId.length != null && message.replyMessageId.length > 0;
        let emojiRegExp = /(?:<p>)(?:\uD83D(?:\uDD73\uFE0F?|\uDC41(?:(?:\uFE0F(?:\u200D\uD83D\uDDE8\uFE0F?)?|\u200D\uD83D\uDDE8\uFE0F?))?|[\uDDE8\uDDEF]\uFE0F?|\uDC4B(?:\uD83C[\uDFFB-\uDFFF])?|\uDD90(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\uDD96\uDC4C\uDC48\uDC49\uDC46\uDD95\uDC47\uDC4D\uDC4E\uDC4A\uDC4F\uDE4C\uDC50\uDE4F\uDC85\uDCAA\uDC42\uDC43\uDC76\uDC66\uDC67](?:\uD83C[\uDFFB-\uDFFF])?|\uDC71(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?)|\u200D(?:[\u2640\u2642]\uFE0F?)))?|\uDC68(?:(?:\uD83C(?:\uDFFB(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFC-\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFC(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB\uDFFD-\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFD(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFE(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFD\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFF(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFE]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?)|\u200D(?:\uD83E[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD]|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC68\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)|\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)))))?|\uDC69(?:(?:\uD83C(?:\uDFFB(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFC-\uDFFF]|\uDC68\uD83C[\uDFFC-\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFC(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB\uDFFD-\uDFFF]|\uDC68\uD83C[\uDFFB\uDFFD-\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFD(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|\uDC68\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFE(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB-\uDFFD\uDFFF]|\uDC68\uD83C[\uDFFB-\uDFFD\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFF(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB-\uDFFE]|\uDC68\uD83C[\uDFFB-\uDFFE])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?)|\u200D(?:\uD83E[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD]|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])|\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])))))?|[\uDC74\uDC75](?:\uD83C[\uDFFB-\uDFFF])?|[\uDE4D\uDE4E\uDE45\uDE46\uDC81\uDE4B\uDE47\uDC6E](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD75(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC82\uDC77](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDC78(?:\uD83C[\uDFFB-\uDFFF])?|\uDC73(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC72\uDC70\uDC7C](?:\uD83C[\uDFFB-\uDFFF])?|[\uDC86\uDC87\uDEB6](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC83\uDD7A](?:\uD83C[\uDFFB-\uDFFF])?|\uDD74(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\uDC6F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDEA3\uDEB4\uDEB5](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDEC0\uDECC\uDC6D\uDC6B\uDC6C](?:\uD83C[\uDFFB-\uDFFF])?|\uDDE3\uFE0F?|\uDC15(?:\u200D\uD83E\uDDBA)?|[\uDC3F\uDD4A\uDD77\uDD78\uDDFA\uDEE3\uDEE4\uDEE2\uDEF3\uDEE5\uDEE9\uDEF0\uDECE\uDD70\uDD79\uDDBC\uDD76\uDECD\uDDA5\uDDA8\uDDB1\uDDB2\uDCFD\uDD6F\uDDDE\uDDF3\uDD8B\uDD8A\uDD8C\uDD8D\uDDC2\uDDD2\uDDD3\uDD87\uDDC3\uDDC4\uDDD1\uDDDD\uDEE0\uDDE1\uDEE1\uDDDC\uDECF\uDECB\uDD49]\uFE0F?|[\uDE00\uDE03\uDE04\uDE01\uDE06\uDE05\uDE02\uDE42\uDE43\uDE09\uDE0A\uDE07\uDE0D\uDE18\uDE17\uDE1A\uDE19\uDE0B\uDE1B-\uDE1D\uDE10\uDE11\uDE36\uDE0F\uDE12\uDE44\uDE2C\uDE0C\uDE14\uDE2A\uDE34\uDE37\uDE35\uDE0E\uDE15\uDE1F\uDE41\uDE2E\uDE2F\uDE32\uDE33\uDE26-\uDE28\uDE30\uDE25\uDE22\uDE2D\uDE31\uDE16\uDE23\uDE1E\uDE13\uDE29\uDE2B\uDE24\uDE21\uDE20\uDE08\uDC7F\uDC80\uDCA9\uDC79-\uDC7B\uDC7D\uDC7E\uDE3A\uDE38\uDE39\uDE3B-\uDE3D\uDE40\uDE3F\uDE3E\uDE48-\uDE4A\uDC8B\uDC8C\uDC98\uDC9D\uDC96\uDC97\uDC93\uDC9E\uDC95\uDC9F\uDC94\uDC9B\uDC9A\uDC99\uDC9C\uDDA4\uDCAF\uDCA2\uDCA5\uDCAB\uDCA6\uDCA8\uDCA3\uDCAC\uDCAD\uDCA4\uDC40\uDC45\uDC44\uDC8F\uDC91\uDC6A\uDC64\uDC65\uDC63\uDC35\uDC12\uDC36\uDC29\uDC3A\uDC31\uDC08\uDC2F\uDC05\uDC06\uDC34\uDC0E\uDC2E\uDC02-\uDC04\uDC37\uDC16\uDC17\uDC3D\uDC0F\uDC11\uDC10\uDC2A\uDC2B\uDC18\uDC2D\uDC01\uDC00\uDC39\uDC30\uDC07\uDC3B\uDC28\uDC3C\uDC3E\uDC14\uDC13\uDC23-\uDC27\uDC38\uDC0A\uDC22\uDC0D\uDC32\uDC09\uDC33\uDC0B\uDC2C\uDC1F-\uDC21\uDC19\uDC1A\uDC0C\uDC1B-\uDC1E\uDC90\uDCAE\uDD2A\uDDFE\uDDFB\uDC92\uDDFC\uDDFD\uDD4C\uDED5\uDD4D\uDD4B\uDC88\uDE82-\uDE8A\uDE9D\uDE9E\uDE8B-\uDE8E\uDE90-\uDE9C\uDEF5\uDEFA\uDEB2\uDEF4\uDEF9\uDE8F\uDEA8\uDEA5\uDEA6\uDED1\uDEA7\uDEF6\uDEA4\uDEA2\uDEEB\uDEEC\uDCBA\uDE81\uDE9F-\uDEA1\uDE80\uDEF8\uDD5B\uDD67\uDD50\uDD5C\uDD51\uDD5D\uDD52\uDD5E\uDD53\uDD5F\uDD54\uDD60\uDD55\uDD61\uDD56\uDD62\uDD57\uDD63\uDD58\uDD64\uDD59\uDD65\uDD5A\uDD66\uDD25\uDCA7\uDEF7\uDD2E\uDC53-\uDC62\uDC51\uDC52\uDCFF\uDC84\uDC8D\uDC8E\uDD07-\uDD0A\uDCE2\uDCE3\uDCEF\uDD14\uDD15\uDCFB\uDCF1\uDCF2\uDCDE-\uDCE0\uDD0B\uDD0C\uDCBB\uDCBD-\uDCC0\uDCFA\uDCF7-\uDCF9\uDCFC\uDD0D\uDD0E\uDCA1\uDD26\uDCD4-\uDCDA\uDCD3\uDCD2\uDCC3\uDCDC\uDCC4\uDCF0\uDCD1\uDD16\uDCB0\uDCB4-\uDCB8\uDCB3\uDCB9\uDCB1\uDCB2\uDCE7-\uDCE9\uDCE4-\uDCE6\uDCEB\uDCEA\uDCEC-\uDCEE\uDCDD\uDCBC\uDCC1\uDCC2\uDCC5-\uDCD0\uDD12\uDD13\uDD0F-\uDD11\uDD28\uDD2B\uDD27\uDD29\uDD17\uDD2C\uDD2D\uDCE1\uDC89\uDC8A\uDEAA\uDEBD\uDEBF\uDEC1\uDED2\uDEAC\uDDFF\uDEAE\uDEB0\uDEB9-\uDEBC\uDEBE\uDEC2-\uDEC5\uDEB8\uDEAB\uDEB3\uDEAD\uDEAF\uDEB1\uDEB7\uDCF5\uDD1E\uDD03\uDD04\uDD19-\uDD1D\uDED0\uDD4E\uDD2F\uDD00-\uDD02\uDD3C\uDD3D\uDD05\uDD06\uDCF6\uDCF3\uDCF4\uDD31\uDCDB\uDD30\uDD1F-\uDD24\uDD34\uDFE0-\uDFE2\uDD35\uDFE3-\uDFE5\uDFE7-\uDFE9\uDFE6\uDFEA\uDFEB\uDD36-\uDD3B\uDCA0\uDD18\uDD33\uDD32\uDEA9])|\uD83E(?:[\uDD1A\uDD0F\uDD1E\uDD1F\uDD18\uDD19\uDD1B\uDD1C\uDD32\uDD33\uDDB5\uDDB6\uDDBB\uDDD2](?:\uD83C[\uDFFB-\uDFFF])?|\uDDD1(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?)|\u200D(?:\uD83E(?:\uDD1D\u200D\uD83E\uDDD1|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?)))?|[\uDDD4\uDDD3](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDCF\uDD26\uDD37](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDD34\uDDD5\uDD35\uDD30\uDD31\uDD36](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDB8\uDDB9\uDDD9-\uDDDD](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDDDE\uDDDF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDDCD\uDDCE\uDDD6\uDDD7\uDD38](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD3C(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDD3D\uDD3E\uDD39\uDDD8](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDD23\uDD70\uDD29\uDD2A\uDD11\uDD17\uDD2D\uDD2B\uDD14\uDD10\uDD28\uDD25\uDD24\uDD12\uDD15\uDD22\uDD2E\uDD27\uDD75\uDD76\uDD74\uDD2F\uDD20\uDD73\uDD13\uDDD0\uDD7A\uDD71\uDD2C\uDD21\uDD16\uDDE1\uDD0E\uDD0D\uDD1D\uDDBE\uDDBF\uDDE0\uDDB7\uDDB4\uDD3A\uDDB0\uDDB1\uDDB3\uDDB2\uDD8D\uDDA7\uDDAE\uDD8A\uDD9D\uDD81\uDD84\uDD93\uDD8C\uDD99\uDD92\uDD8F\uDD9B\uDD94\uDD87\uDDA5\uDDA6\uDDA8\uDD98\uDDA1\uDD83\uDD85\uDD86\uDDA2\uDD89\uDDA9\uDD9A\uDD9C\uDD8E\uDD95\uDD96\uDD88\uDD8B\uDD97\uDD82\uDD9F\uDDA0\uDD40\uDD6D\uDD5D\uDD65\uDD51\uDD54\uDD55\uDD52\uDD6C\uDD66\uDDC4\uDDC5\uDD5C\uDD50\uDD56\uDD68\uDD6F\uDD5E\uDDC7\uDDC0\uDD69\uDD53\uDD6A\uDD59\uDDC6\uDD5A\uDD58\uDD63\uDD57\uDDC8\uDDC2\uDD6B\uDD6E\uDD5F-\uDD61\uDD80\uDD9E\uDD90\uDD91\uDDAA\uDDC1\uDD67\uDD5B\uDD42\uDD43\uDD64\uDDC3\uDDC9\uDDCA\uDD62\uDD44\uDDED\uDDF1\uDDBD\uDDBC\uDE82\uDDF3\uDE90\uDDE8\uDDE7\uDD47-\uDD49\uDD4E\uDD4F\uDD4D\uDD4A\uDD4B\uDD45\uDD3F\uDD4C\uDE80\uDE81\uDDFF\uDDE9\uDDF8\uDDF5\uDDF6\uDD7D\uDD7C\uDDBA\uDDE3-\uDDE6\uDD7B\uDE71-\uDE73\uDD7E\uDD7F\uDE70\uDDE2\uDE95\uDD41\uDDEE\uDE94\uDDFE\uDE93\uDDAF\uDDF0\uDDF2\uDDEA-\uDDEC\uDE78-\uDE7A\uDE91\uDE92\uDDF4\uDDF7\uDDF9-\uDDFD\uDDEF])|[\u263A\u2639\u2620\u2763\u2764]\uFE0F?|\u270B(?:\uD83C[\uDFFB-\uDFFF])?|[\u270C\u261D](?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\u270A(?:\uD83C[\uDFFB-\uDFFF])?|\u270D(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\uD83C(?:\uDF85(?:\uD83C[\uDFFB-\uDFFF])?|\uDFC3(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC7\uDFC2](?:\uD83C[\uDFFB-\uDFFF])?|\uDFCC(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC4\uDFCA](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDFCB(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFF5\uDF36\uDF7D\uDFD4-\uDFD6\uDFDC-\uDFDF\uDFDB\uDFD7\uDFD8\uDFDA\uDFD9\uDFCE\uDFCD\uDF21\uDF24-\uDF2C\uDF97\uDF9F\uDF96\uDF99-\uDF9B\uDF9E\uDFF7\uDD70\uDD71\uDD7E\uDD7F\uDE02\uDE37]\uFE0F?|\uDFF4(?:(?:\u200D\u2620\uFE0F?|\uDB40\uDC67\uDB40\uDC62\uDB40(?:\uDC65\uDB40\uDC6E\uDB40\uDC67\uDB40\uDC7F|\uDC73\uDB40\uDC63\uDB40\uDC74\uDB40\uDC7F|\uDC77\uDB40\uDC6C\uDB40\uDC73\uDB40\uDC7F)))?|\uDFF3(?:(?:\uFE0F(?:\u200D\uD83C\uDF08)?|\u200D\uD83C\uDF08))?|\uDDE6\uD83C[\uDDE8-\uDDEC\uDDEE\uDDF1\uDDF2\uDDF4\uDDF6-\uDDFA\uDDFC\uDDFD\uDDFF]|\uDDE7\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEF\uDDF1-\uDDF4\uDDF6-\uDDF9\uDDFB\uDDFC\uDDFE\uDDFF]|\uDDE8\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDEE\uDDF0-\uDDF5\uDDF7\uDDFA-\uDDFF]|\uDDE9\uD83C[\uDDEA\uDDEC\uDDEF\uDDF0\uDDF2\uDDF4\uDDFF]|\uDDEA\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDED\uDDF7-\uDDFA]|\uDDEB\uD83C[\uDDEE-\uDDF0\uDDF2\uDDF4\uDDF7]|\uDDEC\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEE\uDDF1-\uDDF3\uDDF5-\uDDFA\uDDFC\uDDFE]|\uDDED\uD83C[\uDDF0\uDDF2\uDDF3\uDDF7\uDDF9\uDDFA]|\uDDEE\uD83C[\uDDE8-\uDDEA\uDDF1-\uDDF4\uDDF6-\uDDF9]|\uDDEF\uD83C[\uDDEA\uDDF2\uDDF4\uDDF5]|\uDDF0\uD83C[\uDDEA\uDDEC-\uDDEE\uDDF2\uDDF3\uDDF5\uDDF7\uDDFC\uDDFE\uDDFF]|\uDDF1\uD83C[\uDDE6-\uDDE8\uDDEE\uDDF0\uDDF7-\uDDFB\uDDFE]|\uDDF2\uD83C[\uDDE6\uDDE8-\uDDED\uDDF0-\uDDFF]|\uDDF3\uD83C[\uDDE6\uDDE8\uDDEA-\uDDEC\uDDEE\uDDF1\uDDF4\uDDF5\uDDF7\uDDFA\uDDFF]|\uDDF4\uD83C\uDDF2|\uDDF5\uD83C[\uDDE6\uDDEA-\uDDED\uDDF0-\uDDF3\uDDF7-\uDDF9\uDDFC\uDDFE]|\uDDF6\uD83C\uDDE6|\uDDF7\uD83C[\uDDEA\uDDF4\uDDF8\uDDFA\uDDFC]|\uDDF8\uD83C[\uDDE6-\uDDEA\uDDEC-\uDDF4\uDDF7-\uDDF9\uDDFB\uDDFD-\uDDFF]|\uDDF9\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDED\uDDEF-\uDDF4\uDDF7\uDDF9\uDDFB\uDDFC\uDDFF]|\uDDFA\uD83C[\uDDE6\uDDEC\uDDF2\uDDF3\uDDF8\uDDFE\uDDFF]|\uDDFB\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDEE\uDDF3\uDDFA]|\uDDFC\uD83C[\uDDEB\uDDF8]|\uDDFD\uD83C\uDDF0|\uDDFE\uD83C[\uDDEA\uDDF9]|\uDDFF\uD83C[\uDDE6\uDDF2\uDDFC]|[\uDFFB-\uDFFF\uDF38-\uDF3C\uDF37\uDF31-\uDF35\uDF3E-\uDF43\uDF47-\uDF53\uDF45\uDF46\uDF3D\uDF44\uDF30\uDF5E\uDF56\uDF57\uDF54\uDF5F\uDF55\uDF2D-\uDF2F\uDF73\uDF72\uDF7F\uDF71\uDF58-\uDF5D\uDF60\uDF62-\uDF65\uDF61\uDF66-\uDF6A\uDF82\uDF70\uDF6B-\uDF6F\uDF7C\uDF75\uDF76\uDF7E\uDF77-\uDF7B\uDF74\uDFFA\uDF0D-\uDF10\uDF0B\uDFE0-\uDFE6\uDFE8-\uDFED\uDFEF\uDFF0\uDF01\uDF03-\uDF07\uDF09\uDFA0-\uDFA2\uDFAA\uDF11-\uDF20\uDF0C\uDF00\uDF08\uDF02\uDF0A\uDF83\uDF84\uDF86-\uDF8B\uDF8D-\uDF91\uDF80\uDF81\uDFAB\uDFC6\uDFC5\uDFC0\uDFD0\uDFC8\uDFC9\uDFBE\uDFB3\uDFCF\uDFD1-\uDFD3\uDFF8\uDFA3\uDFBD\uDFBF\uDFAF\uDFB1\uDFAE\uDFB0\uDFB2\uDCCF\uDC04\uDFB4\uDFAD\uDFA8\uDF92\uDFA9\uDF93\uDFBC\uDFB5\uDFB6\uDFA4\uDFA7\uDFB7-\uDFBB\uDFA5\uDFAC\uDFEE\uDFF9\uDFE7\uDFA6\uDD8E\uDD91-\uDD9A\uDE01\uDE36\uDE2F\uDE50\uDE39\uDE1A\uDE32\uDE51\uDE38\uDE34\uDE33\uDE3A\uDE35\uDFC1\uDF8C])|\u26F7\uFE0F?|\u26F9(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\u2618\u26F0\u26E9\u2668\u26F4\u2708\u23F1\u23F2\u2600\u2601\u26C8\u2602\u26F1\u2744\u2603\u2604\u26F8\u2660\u2665\u2666\u2663\u265F\u26D1\u260E\u2328\u2709\u270F\u2712\u2702\u26CF\u2692\u2694\u2699\u2696\u26D3\u2697\u26B0\u26B1\u26A0\u2622\u2623\u2B06\u2197\u27A1\u2198\u2B07\u2199\u2B05\u2196\u2195\u2194\u21A9\u21AA\u2934\u2935\u269B\u2721\u2638\u262F\u271D\u2626\u262A\u262E\u25B6\u23ED\u23EF\u25C0\u23EE\u23F8-\u23FA\u23CF\u2640\u2642\u2695\u267E\u267B\u269C\u2611\u2714\u2716\u303D\u2733\u2734\u2747\u203C\u2049\u3030\u00A9\u00AE\u2122]\uFE0F?|[\u0023\u002A\u0030-\u0039](?:\uFE0F\u20E3|\u20E3)|[\u2139\u24C2\u3297\u3299\u25FC\u25FB\u25AA\u25AB]\uFE0F?|[\u2615\u26EA\u26F2\u26FA\u26FD\u2693\u26F5\u231B\u23F3\u231A\u23F0\u2B50\u26C5\u2614\u26A1\u26C4\u2728\u26BD\u26BE\u26F3\u267F\u26D4\u2648-\u2653\u26CE\u23E9-\u23EC\u2B55\u2705\u274C\u274E\u2795-\u2797\u27B0\u27BF\u2753-\u2755\u2757\u26AB\u26AA\u2B1B\u2B1C\u25FE\u25FD])(?:<\/p>)\s*/;
        if (isReply && emojiRegExp.test(content)) {
            let emoji = content.trim();
            if (emoji.startsWith("<p>")) {
                emoji = emoji.substr(3);
            }
            if (emoji.endsWith("</p>")) {
                emoji = emoji.substr(0, emoji.length - 4);
            }
            this.addReaction(message, emoji);
            return;
        }

        let nomessages = document.getElementById("wiki-talk-nomessages");
        if (nomessages != null) {
            nomessages.remove();
        }

        let messageLi = document.createElement("li");
        messageLi.id = `wiki-message-${message.id}`;
        messageLi.classList.add("wiki-message", "collapsible", "collapsed");

        let messageHeader = document.createElement("div");
        messageLi.appendChild(messageHeader);
        messageHeader.classList.add("wiki-message-header");

        let sender = document.createElement("span");
        messageHeader.appendChild(sender);
        sender.classList.add("wiki-message-sender");

        let username = document.createTextNode(message.senderName);

        if (message.senderExists) {
            let userLink = document.createElement("a");
            sender.appendChild(userLink);
            userLink.classList.add("wiki-username", "wiki-username-link", `wiki-username-${message.senderId}`);
            if (message.senderIsAdmin) {
                userLink.classList.add("wiki-username-admin");
            }
            if (!message.senderPageExists) {
                userLink.classList.add("wiki-link-missing");
            }
            userLink.href = `/${window.wikimvcchat.prefix}/${window.wikimvcchat.userNamespace}:${message.senderId}`;
            userLink.title = `Visit the user page for ${message.senderName}`
            userLink.appendChild(username);
        } else {
            let userSpan = document.createElement("span");
            sender.appendChild(userSpan);
            userLink.classList.add("wiki-username", "wiki-username-nolink");
            userLink.appendChild(username);
        }

        let messageTimestamp = document.createElement("span");
        messageHeader.appendChild(messageTimestamp);
        messageTimestamp.classList.add("wiki-message-timestamp");

        let timestamp = document.createTextNode(new Date((message.timestampTicks / 10000) - 62135596800000).toLocaleString());
        messageTimestamp.appendChild(timestamp);

        let reactionsSpan = document.createElement("div");
        messageHeader.appendChild(reactionsSpan);
        reactionsSpan.id = `wiki-message-reactions-${message.id}`;
        reactionsSpan.classList.add("wiki-message-reactions");

        let messageBody = document.createElement("div");
        messageLi.appendChild(messageBody);
        messageBody.classList.add("wiki-message-body");
        if (!window.wikimvcchat.connected) {
            messageBody.classList.add("wiki-message-readonly");
        }

        let messageContent = document.createElement("div");
        messageBody.appendChild(messageContent);
        messageContent.classList.add("wiki-message-content");

        messageContent.innerHTML = content;

        let collapseToggler = document.createElement("div");
        messageBody.appendChild(collapseToggler);
        collapseToggler.classList.add("wiki-message-collapse-toggler");

        let toggleLink = document.createElement("a");
        collapseToggler.appendChild(toggleLink);
        toggleLink.href = "javascript: void(0);";
        toggleLink.onclick = function (event) {
            event.target.parentElement.parentElement.parentElement.classList.toggle("collapsed");
        }

        if (!isReply) {
            let messageFooter = document.createElement("div");
            messageLi.appendChild(messageFooter);
            messageFooter.classList.add("wiki-message-footer");

            let threadToggler = document.createElement("div");
            messageFooter.appendChild(threadToggler);
            threadToggler.classList.add("wiki-message-thread-toggler");
            threadToggler.onclick = function (event) {
                event.target.closest(".wiki-message-thread").classList.toggle("expanded");
            }

            let threadToggleLink = document.createElement("a");
            threadToggler.appendChild(threadToggleLink);
            threadToggleLink.href = "javascript: void(0);";

            let threadToggleIcon = document.createElement("span");
            threadToggler.appendChild(threadToggleIcon);
            threadToggleIcon.innerHTML = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"12\" height=\"12\"><path d=\"M4 1.533v9.671l4.752-4.871z\" /></svg >";

            let replyToggle = document.createElement("a");
            messageFooter.appendChild(replyToggle);
            replyToggle.id = `wiki-talk-reply-toggle-${message.id}`;
            replyToggle.classList.add("wiki-talk-reply-toggle");
            replyToggle.href = "javascript: void(0);";
            replyToggle.textContent = "Reply";
            replyToggle.onclick = function (event) {
                let reply = document.getElementById(`wiki-talk-reply-${message.id}`);
                if (reply.classList.contains("collapsed")) {
                    document.getElementById(`wiki-talk-reply-${message.id}`).classList.remove("collapsed");
                    document.getElementById(`wiki-talk-message-input-${message.id}`).focus();
                } else {
                    document.getElementById(`wiki-talk-reply-${message.id}`).classList.add("collapsed");
                }
            }

            let replyDiv = document.createElement("div");
            messageFooter.appendChild(replyDiv);
            replyDiv.id = `wiki-talk-reply-${message.id}`;
            replyDiv.classList.add("wiki-talk-reply", "collapsed");

            let formTextArea = document.createElement("textarea");
            replyDiv.appendChild(formTextArea);
            formTextArea.id = `wiki-talk-message-input-${message.id}`;
            formTextArea.classList.add("form-control");
            formTextArea.placeholder = "Reply";
            formTextArea.addEventListener("keypress", function (event) {
                if (event.keyCode === 13 && !event.shiftKey) {
                    event = event || window.event;
                    let target = event.target || event.srcElement;
                    let id = target.id;
                    if (id == null || id.length == null || id.length <= 24) {
                        return;
                    }
                    id = id.substr(24);
                    let reply;
                    if (target.attachedEditor) {
                        reply = target.attachedEditor.value();
                        target.attachedEditor.value("");
                    } else {
                        reply = target.value;
                        target.value = "";
                    }
                    if (reply == null || reply.length == null || !(reply.length > 0)) {
                        return;
                    }
                    window.wikimvcchat.connection.invoke("Send", {
                        markdown: reply,
                        messageId: id,
                        topicId: topicId,
                    }).catch(function (err) {
                        //return console.error(err.toString());
                        return console.error("An error occurred while sending a chat message");
                    });
                }
            });

            let replyControlRow = document.createElement("div");
            replyDiv.appendChild(replyControlRow);
            replyControlRow.classList.add("wiki-message-controls");

            let replyControlButtons = document.createElement("div");
            replyControlRow.appendChild(replyControlButtons);

            window.wikimvcchat.addReactionButton(topicId, replyControlButtons, "👍");
            window.wikimvcchat.addReactionButton(topicId, replyControlButtons, "👎");

            let anyEmojiButton = document.createElement("button");
            replyControlButtons.appendChild(anyEmojiButton);
            anyEmojiButton.id = `wiki-talk-message-emoji-${message.id}`;
            anyEmojiButton.classList.add("btn", "btn-sm", "btn-outline-light");
            anyEmojiButton.textContent = "😀";
            anyEmojiButton.addEventListener("click", function (event) {
                event = event || window.event;
                let target = event.target || event.srcElement;
                let id = target.id;
                if (id == null || id.length == null || id.length <= 24) {
                    return;
                }
                id = id.substr(24);
                window.wikimvcchat.emojiBox = document.getElementById(`wiki-talk-message-input-${id}`);
                window.wikimvcchat.emojiButton = document.getElementById(`wiki-talk-reply-button-${id}`);
                window.wikimvcchat.picker.togglePicker(anyEmojiButton);
            });

            let gifButton = document.createElement("button");
            replyControlButtons.appendChild(gifButton);
            gifButton.id = `wiki-talk-message-gif-${message.id}`;
            gifButton.classList.add("btn", "btn-sm", "btn-outline-light");
            gifButton.textContent = "GIF";
            gifButton.addEventListener('click', function (event) {
                let tenorContainer = document.getElementById("wiki-talk-tenor");
                if (tenorContainer.getAttribute("data-show") == null) {
                    event = event || window.event;
                    let target = event.target || event.srcElement;
                    let id = target.id;
                    if (id == null || id.length == null || id.length <= 22) {
                        return;
                    }
                    id = id.substr(22);
                    window.wikimvcchat.tenorBox.onShow(
                        document.getElementById(`wiki-talk-message-input-${id}`),
                        document.getElementById(`wiki-talk-reply-button-${id}`));
                } else {
                    window.wikimvcchat.tenorBox.onHide();
                }
            });

            let editorButton = document.createElement("button");
            replyControlButtons.appendChild(editorButton);
            editorButton.id = `wiki-talk-message-editor-${message.id}`;
            editorButton.classList.add("btn", "btn-sm", "btn-outline-light");
            editorButton.textContent = "✏️";
            editorButton.addEventListener("click", function (event) {
                event = event || window.event;
                let target = event.target || event.srcElement;
                let id = target.id;
                if (id == null || id.length == null || id.length <= 25) {
                    return;
                }
                id = id.substr(25);
                let chatInput = document.getElementById(`wiki-talk-message-input-${id}`);
                if (chatInput.attachedEditor == null) {
                    chatInput.attachedEditor = new EasyMDE({
                        element: chatInput,
                        indentWithTabs: false,
                        placeholder: "Add a new message",
                        tabSize: 4,
                        toolbar: ['bold', 'italic', '|', 'heading-1', 'heading-2', 'heading-3', '|', 'unordered-list', 'ordered-list', '|', 'link', 'image']
                    });
                } else {
                    chatInput.attachedEditor.toTextArea();
                    chatInput.attachedEditor = null;
                }
            });

            let replyButton = document.createElement("button");
            replyControlRow.appendChild(replyButton);
            replyButton.id = `wiki-talk-reply-button-${message.id}`;
            replyButton.type = 'button';
            replyButton.classList.add("btn", "btn-primary", "btn-sm");
            replyButton.textContent = "Post";
            replyButton.disabled = true;
            replyButton.addEventListener("click", function (event) {
                event = event || window.event;
                let target = event.target || event.srcElement;
                let id = target.id;
                if (id == null || id.length == null || id.length <= 23) {
                    return;
                }
                id = id.substr(23);
                let chatInput = document.getElementById(`wiki-talk-message-input-${id}`);
                if (chatInput == null) {
                    return;
                }
                let reply;
                if (chatInput.attachedEditor) {
                    reply = chatInput.attachedEditor.value();
                    chatInput.attachedEditor.value("");
                } else {
                    reply = chatInput.value;
                    chatInput.value = "";
                }
                if (reply == null || reply.length == null || !(reply.length > 0)) {
                    return;
                }
                window.wikimvcchat.connection.invoke("Send", {
                    markdown: reply,
                    messageId: id,
                    topicId: topicId,
                }).catch(function (err) {
                    //return console.error(err.toString());
                    return console.error("An error occurred while sending a chat message");
                });
                event.preventDefault();
                event.stopPropagation();
            });
            formTextArea.addEventListener("input", function (event) {
                event = event || window.event;
                let target = event.target || event.srcElement;
                let id = target.id;
                if (id == null || id.length == null || id.length <= 24) {
                    return;
                }
                id = id.substr(24);
                let replyBtn = document.getElementById(`wiki-talk-reply-button-${id}`);
                if (replyBtn == null) {
                    return;
                }
                replyBtn.disabled = !target.value || !(target.value.length > 0);
            });

            window.wikimvcchat.messageListUl.appendChild(messageLi);
        } else {
            let parent = document.getElementById(`wiki-message-${message.replyMessageId}`);
            if (parent != null) {
                messageLi.classList.add("wiki-message-reply");

                parent.classList.add("wiki-message-thread", "expanded");

                let childMessageLists = parent.getElementsByTagName("ul");
                let childMessageList;
                if (childMessageLists.length === 0) {
                    childMessageList = document.createElement("ul");
                    let parentFooters = parent.getElementsByClassName("wiki-message-footer");
                    if (parentFooters.length > 0) {
                        let parentFooter = parentFooters[0];
                        parent.insertBefore(childMessageList, parentFooter);
                    } else {
                        parent.appendChild(childMessageList);
                    }
                    childMessageList.classList.add("wiki-message-replies");
                } else {
                    childMessageList = childMessageLists[0];
                }
                childMessageList.appendChild(messageLi);
            }
        }

        let height = parseInt(getComputedStyle(messageContent).getPropertyValue("height"));
        let maxHeight = parseInt(getComputedStyle(messageContent).getPropertyValue("max-height"));
        if (height < maxHeight) {
            messageLi.classList.remove("collapsible");
        }
    },

    addReaction: function (reaction, emoji) {
        if (reaction == null || reaction.id == null || reaction.replyMessageId == null && reaction.replyMessageId.length == null && !(reaction.replyMessageId.length > 0)) {
            return;
        }

        let parentReactionSpan = document.getElementById(`wiki-message-reactions-${reaction.replyMessageId}`);
        if (parentReactionSpan == null) {
            return;
        }

        let typeName = window.wikimvcchat.getCharCode(emoji).toString();
        let reactions = document.getElementById(`wiki-message-reactions-${typeName}-${reaction.replyMessageId}`);
        if (reactions == null) {
            window.wikimvcchat.addReactionList(reaction.replyMessageId, reaction.topicId, parentReactionSpan, typeName, emoji);
        }

        window.wikimvcchat.addReactionToList(reaction, typeName);
    },

    addReactionButton: function (topicId, span, emoji) {
        let replyButton = document.createElement("button");
        span.appendChild(replyButton);
        replyButton.classList.add("wiki-message-reply-emoji", "btn", "btn-sm", "btn-outline-light");
        replyButton.textContent = emoji;
        replyButton.addEventListener("click", function (event) {
            event = event || window.event;
            let target = event.target || event.srcElement;
            let parent = target.parentElement.parentElement.parentElement;
            let id = parent.id;
            if (id == null || id.length == null || id.length <= 16) {
                return;
            }
            id = id.substr(16);
            window.wikimvcchat.connection.invoke("Send", {
                markdown: emoji,
                messageId: id,
                topicId: topicId,
            }).catch(function (err) {
                //return console.error(err.toString());
                return console.error("An error occurred while sending a chat message");
            });
            event.preventDefault();
            event.stopPropagation();
        });
    },

    addReactionList: function (messageId, topicId, span, typeName, emoji) {
        let reactionTypeParentSpan = document.createElement("span");
        span.appendChild(reactionTypeParentSpan);

        let reactionTypeSpan = document.createElement("span");
        reactionTypeParentSpan.appendChild(reactionTypeSpan);
        reactionTypeSpan.id = `wiki-message-reactions-${typeName}-${messageId}`;

        let reactionIconSpan = document.createElement("span");
        reactionTypeSpan.appendChild(reactionIconSpan);
        reactionIconSpan.classList.add("wiki-message-reaction-icon");
        reactionIconSpan.textContent = emoji;
        reactionIconSpan.onclick = function (event) {
            window.wikimvcchat.connection.invoke("Send", {
                markdown: emoji,
                messageId: messageId,
                topicId: topicId,
            }).catch(function (err) {
                //return console.error(`An error occurred while attempting to send a chat reaction: ${err}`);
                return console.error("An error occurred while attempting to send a chat reaction");
            });
            event.preventDefault();
            event.stopPropagation();
        };

        let reactionCountSpan = document.createElement("span");
        reactionTypeSpan.appendChild(reactionCountSpan);
        reactionCountSpan.id = `wiki-message-reaction-count-${typeName}-${messageId}`;
        reactionCountSpan.classList.add("wiki-message-reaction-count");

        let reactionCount = document.createTextNode("0");
        reactionCountSpan.appendChild(reactionCount);

        let reactionList = document.createElement("ul");
        reactionTypeSpan.appendChild(reactionList);
        reactionList.id = `wiki-message-reaction-list-${typeName}-${messageId}`;
        reactionList.classList.add("wiki-message-reaction-list");
        reactionList.style.display = "none";

        tippy(reactionTypeSpan, {
            placement: 'auto',
            allowHTML: true,
            interactive: true,
            onShow(instance) {
                instance.setContent(document.getElementById(`wiki-message-reaction-list-${typeName}-${messageId}`).innerHTML);
            },
        });
    },

    addReactionToList(reaction, typeName) {
        let countSpan = document.getElementById(`wiki-message-reaction-count-${typeName}-${reaction.replyMessageId}`);
        let count = parseInt(countSpan.textContent);
        count++;
        countSpan.textContent = count.toLocaleString();
        if (count === 1) {
            countSpan.classList.add("wiki-message-reaction-count-one");
        } else {
            countSpan.classList.remove("wiki-message-reaction-count-one");
        }

        let reactionList = document.getElementById(`wiki-message-reaction-list-${typeName}-${reaction.replyMessageId}`);
        let reactionListItem = document.createElement("li");
        reactionList.appendChild(reactionListItem);

        let username = document.createTextNode(reaction.senderName);
        if (reaction.senderExists) {
            let userLink = document.createElement("a");
            reactionListItem.appendChild(userLink);
            userLink.classList.add("wiki-username", "wiki-username-link");
            if (!reaction.senderPageExists) {
                userLink.classList.add("wiki-link-missing");
            }
            userLink.href = `/${window.wikimvcchat.prefix}/${window.wikimvcchat.userNamespace}:${reaction.senderId}`;
            userLink.title = `Visit the user page for ${reaction.senderName}`;
            userLink.appendChild(username);
        } else {
            let userSpan = document.createElement("span");
            reactionListItem.appendChild(userSpan);
            userSpan.classList.add("wiki-username", "wiki-username-nolink");
            userSpan.appendChild(username);
        }

        let reactionTimestamp = document.createElement("span");
        reactionListItem.appendChild(reactionTimestamp);
        reactionTimestamp.classList.add("wiki-message-timestamp");

        let timestamp = document.createTextNode(new Date((reaction.timestampTicks / 10000) - 62135596800000).toLocaleString());
        reactionTimestamp.appendChild(timestamp);
    },

    getCharCode: function (emoji) {
        if (!emoji.length) {
            return 0;
        }
        if (emoji.length === 1) {
            return emoji.charCodeAt(0);
        }
        let c = ((emoji.charCodeAt(0) - 0xD800) * 0x400)
            + (emoji.charCodeAt(1) - 0xDC00) + 0x10000;
        if (c < 0) {
            return emoji.charCodeAt(0);
        }
        return c;
    },

    getContent: function (message) {
        const txtArea = document.createElement('textarea');
        txtArea.innerHTML = message.content;
        return txtArea.value;
    },

    sendMessage: function (topicId, message) {
        if (message && message.length) {
            let msg = message.trim();
            if (msg.length > 0) {
                window.wikimvcchat.connection.invoke("Send", {
                    markdown: msg,
                    topicId: topicId,
                }).catch(function (err) {
                    return console.error(err);
                    //return console.error("An error occurred while sending a chat message");
                });
            }
        }
    },

    tenorCategoriesCallback: function (response) {
        let categories = response["tags"];

        let contentContainer = document.getElementById("wiki-talk-tenor-content");

        for (let i = 0; i < categories.length; i++) {
            let item = document.createElement("div");
            contentContainer.appendChild(item);
            item.classList.add("wiki-talk-tenor-item");
            item.categoryPath = categories[i]["path"];

            let img = document.createElement("img");
            item.appendChild(img);
            img.src = categories[i]["image"];

            let caption = document.createElement("span");
            item.appendChild(caption);
            caption.textContent = categories[i]["name"];

            item.addEventListener("click", function (event) {
                event = event || window.event;
                let target = event.target || event.srcElement;
                let item = target.closest("div");
                let contentContainer = document.getElementById("wiki-talk-tenor-content");
                contentContainer.innerHTML = "";
                fetch(item.categoryPath)
                    .then(r => r.json())
                    .then(window.wikimvcchat.tenorSearchCallback);
            })
        }
    },

    tenorSearch: function (search_term) {
        if (window.wikimvcchat.tenorSearchTerm != search_term) {
            window.wikimvcchat.tenorSearchTerm = search_term;
            window.wikimvcchat.tenorSearchNext = null;
        }

        let searchSuggestionsContainer = document.getElementById("wiki-talk-tenor-search-suggestions");
        searchSuggestionsContainer.innerHTML = "";

        let lang = (navigator.languages || ["en"])[0];
        let url = "https://api.tenor.com/v1/search?q=" + search_term
            + "&key=" + window.wikimvcchat.tenorAPIKey
            + "&locale=" + lang
            + "&contentfilter=low"
            + "&media_filter=basic"
            + "&limit=" + 10;
        if (window.wikimvcchat.tenorSearchNext != null) {
            url += "&pos=" + window.wikimvcchat.tenorSearchNext;
        }

        // using default locale of en_US
        fetch(url)
            .then(r => r.json())
            .then(window.wikimvcchat.tenorSearchCallback);

        url = "https://api.tenor.com/v1/search_suggestions?key=" + window.wikimvcchat.tenorAPIKey
            + "&locale=" + lang
            + "&limit=" + 5;
        fetch(url)
            .then(r => r.json())
            .then(window.wikimvcchat.tenorSearchSuggestionCallback);
    },

    tenorSearchCallback: function (response) {
        window.wikimvcchat.tenorSearchNext = response["next"];
        let results = response["results"];

        let contentContainer = document.getElementById("wiki-talk-tenor-content");

        for (let i = 0; i < results.length; i++) {
            let item = document.createElement("div");
            contentContainer.appendChild(item);
            item.classList.add("wiki-talk-tenor-item");
            item.tenorFullUrl = results[i]["media"][0]["tinygif"]["url"];

            let img = document.createElement("img");
            item.appendChild(img);
            img.src = results[i]["media"][0]["nanogif"]["url"];

            item.addEventListener("click", function (event) {
                event = event || window.event;
                let target = event.target || event.srcElement;
                let item = target.closest("div");
                window.wikimvcchat.tenorBox.onHide();
                let link = `[![${results[i]["title"]}](${item.tenorFullUrl})](${results[i]["url"]})`;
                if (window.wikimvcchat.tenorBox.editor.attachedEditor) {
                    window.wikimvcchat.tenorBox.editor.attachedEditor.value(window.wikimvcchat.tenorBox.editor.attachedEditor.value() + link);
                } else {
                    window.wikimvcchat.tenorBox.editor.value += link;
                }
                window.wikimvcchat.tenorBox.sendButton.disabled = false;

                if (window.wikimvcchat.tenorSearchTerm && window.wikimvcchat.tenorSearchTerm.length) {
                    window.wikimvcchat.tenorShare(window.wikimvcchat.tenorSearchTerm, results[i]["id"]);
                }
            })
        }

        setTimeout(() => {
            contentContainer.infiniteScrollStarted = false;
        }, 500);
    },

    tenorSearchSuggestionCallback: function (response) {
        let predictions = response["results"];

        let searchSuggestionsContainer = document.getElementById("wiki-talk-tenor-search-suggestions");

        for (let i = 0; i < predictions.length; i++) {
            let suggestion = document.createElement("div");
            searchSuggestionsContainer.appendChild(suggestion);
            suggestion.classList.add("wiki-talk-tenor-search-suggestion");

            let suggestionText = document.createElement("span");
            suggestion.appendChild(suggestionText);
            suggestionText.textContent = predictions[i];
        }
    },

    tenorShare: function (search_term, id) {
        let lang = (navigator.languages || ["en"])[0];
        fetch("https://api.tenor.com/v1/registershare?id=" + id
            + "&key=" + window.wikimvcchat.tenorAPIKey
            + "&q=" + search_term
            + "&locale=" + lang);
    },
};
