import * as signalR from "@microsoft/signalr";
import tippy from 'tippy.js';

const sendButton = document.getElementById('wiki-talk-send');
if (sendButton) {
    sendButton.disabled = true;
}
const chatInput = document.getElementById("wiki-talk-newmessage-input");

window.wikimvcchat = {
    connection: null,
    messageListUl: null,
    userNamespace: "Users",

    init: function (url, userNamespace, topicId, messages) {
        window.wikimvcchat.userNamespace = userNamespace;

        let messagesDiv = document.getElementById("wiki-talk-messages");
        if (messagesDiv == null) {
            return;
        }
        let ul = document.createElement("ul");
        window.wikimvcchat.messageListUl = ul;

        messages = JSON.parse(messages) || [];
        for (let i = 0; i < messages.length; i++) {
            window.wikimvcchat.addMessage(messages[i]);
        }

        if (url && url.length && topicId && topicId.length) {
            window.wikimvcchat.connection = new signalR
                .HubConnectionBuilder()
                .withUrl(url)
                .withAutomaticReconnect()
                .build();

            window.wikimvcchat.connection.on("Receive", window.wikimvcchat.addMessage);

            window.wikimvcchat.connection.on("ReceiveReaction", window.wikimvcchat.addReaction);

            window.wikimvcchat.connection.start().then(function () {
                sendButton.disabled = false;
            }).catch(function (err) {
                //return console.error(err.toString());
                return console.error("An error occurred while connecting to chat");
            });

            sendButton.addEventListener("click", function (event) {
                let message = chatInput.value;
                if (message && message.length) {
                    connection.invoke("Send", {
                        markdown: message,
                        topicId: topicId,
                    }).catch(function (err) {
                        //return console.error(err.toString());
                        return console.error("An error occurred while sending a chat message");
                    });
                }
                event.preventDefault();
                event.stopPropagation();
            });
        }
    },

    addMessage: function (message) {
        if (message == null || message.content == null || message.content.length == null || !(message.content.length > 0)) {
            return;
        }

        document.getElementById("wiki-talk-nomessages").remove();

        let messageLi = document.createElement("li");
        messageLi.id = `wiki-message-${message.id}`;

        let messageDiv = document.createElement("div");
        messageLi.appendChild(messageDiv);
        messageDiv.classList.add("wiki-message", "collapsible", "collapsed");

        let messageHeader = document.createElement("div");
        messageDiv.appendChild(messageHeader);
        messageHeader.classList.add("wiki-message-header");

        let sender = document.createElement("span");
        messageHeader.appendChild(sender);
        sender.classList.add("wiki-message-sender");

        let username = document.createTextNode(message.senderName);

        if (message.senderExists) {
            let userLink = document.createElement("a");
            sender.appendChild(userLink);
            userLink.classList.add("wiki-username", "wiki-username-link");
            userLink.href = `/${window.wikimvcchat.userNamespace}:${message.senderId}`;
            userLink.title = `Visit the user page for ${message.senderName}`
            userLink.appendChild(username);
        } else {
            let userSpan = document.createElement("span");
            sender.appendChild(userSpan);
            userLink.classList.add("wiki-username", "wiki-username-nolink");
            userLink.appendChild(username);
        }

        let reactionsSpan = document.createElement("span");
        messageHeader.appendChild(reactionsSpan);
        reactionsSpan.classList.add("wiki-message-reactions");

        let reactionsPositive = [];
        let reactionsFunny = [];
        let reactionsSurprise = [];
        let reactionsSad = [];
        let reactionsNegative = [];
        for (let i = 0; i < message.reactions.length; i++) {
            switch (message.reactions[i].type) {
                case 0:
                    reactionsPositive.push(message.reactions[i]);
                    break;
                case 1:
                    reactionsNegative.push(message.reactions[i]);
                    break;
                case 2:
                    reactionsFunny.push(message.reactions[i]);
                    break;
                case 3:
                    reactionsSad.push(message.reactions[i]);
                    break;
                case 4:
                    reactionsSurprise.push(message.reactions[i]);
                    break;
            }
        }
        window.wikimvcchat.addReactionList(message.id, reactionsSpan, reactionsPositive, "positive", 0);
        window.wikimvcchat.addReactionList(message.id, reactionsSpan, reactionsPositive, "funny", 2);
        window.wikimvcchat.addReactionList(message.id, reactionsSpan, reactionsPositive, "surprise", 4);
        window.wikimvcchat.addReactionList(message.id, reactionsSpan, reactionsPositive, "sad", 3);
        window.wikimvcchat.addReactionList(message.id, reactionsSpan, reactionsPositive, "negative", 1);

        let messageTimestamp = document.createElement("span");
        messageHeader.appendChild(messageTimestamp);
        messageTimestamp.classList.add("wiki-message-timestamp");

        let timestamp = document.createTextNode(new Date((message.timestamp / 10000) - 2208988800000).toLocaleString());
        messageTimestamp.appendChild(timestamp);

        let messageContent = document.createElement("div");
        messageDiv.appendChild(messageContent);
        messageContent.classList.add("wiki-message-content");

        let content = document.createTextNode(message.content);
        messageContent.appendChild(content);

        let toggler = document.createElement("div");
        messageDiv.appendChild(toggler);
        toggler.classList.add("wiki-message-toggler");

        let toggleLink = document.createElement("a");
        toggler.appendChild(toggleLink);
        toggleLink.href = "javascript: void(0);";
        toggleLink.onclick = function (event) {
            event.target.parentElement.parentElement.classList.toggle("collapsed");
        }

        window.wikimvcchat.messageListUl.appendChild(messageLi);

        let height = parseInt(getComputedStyle(messageContent).getPropertyValue("height"));
        let maxHeight = parseInt(getComputedStyle(messageContent).getPropertyValue("max-height"));
        if (height < maxHeight) {
            messageDiv.classList.remove("collapsible");
        }
    },

    addReaction: function (reaction) {
        if (reaction == null || reaction.messageId == null) {
            return;
        }

        let typeName = "";
        switch (reaction.type) {
            case 0:
                typeName = "positive";
                break;
            case 1:
                typeName = "negative";
                break;
            case 2:
                typeName = "funny";
                break;
            case 3:
                typeName = "sad";
                break;
            case 4:
                typeName = "surprise";
                break;
            default:
        }
        let reactions = document.getElementById(`wiki-message-reactions-${typeName}-${reaction.messageId}`);
        if (reactions == null) {
            return;
        }

        let countSpan = reactions.getElementsByClassName("wiki-message-reactions-count")[0];
        let count = parseInt(countSpan.textContent);
        count++;
        countSpan.textContent = count.toLocaleString();

        let reactionList = reactions.getElementsByClassName("wiki-message-reaction-list")[0];
        window.wikimvcchat.addReactionToList(reactionList, reaction);
    },

    addReactionList: function (message, span, list, typeName, typeNum) {
        let reactionTypeSpan = document.createElement("span");
        span.appendChild(reactionTypeSpan);
        reactionTypeSpan.id = `wiki-message-reactions-${typeName}-${message.id}`;

        let reactionIconSpan = document.createElement("span");
        reactionTypeSpan.appendChild(reactionIconSpan);
        reactionIconSpan.classList.add(`wiki-message-reactions-${typeName}`);
        reactionIconSpan.onclick = function (event) {
            window.wikimvcchat.connection.invoke("SendReaction", {
                messageId: message.id,
                topicId: message.topicId,
                type: typeNum,
            }).catch(function (err) {
                //return console.error(`An error occurred while attempting to send a chat reaction: ${err}`);
                return console.error("An error occurred while attempting to send a chat reaction");
            });
            event.preventDefault();
            event.stopPropagation();
        };

        let reactionCountSpan = document.createElement("span");
        reactionTypeSpan.appendChild(reactionCountSpan);
        reactionCountSpan.classList.add("wiki-message-reactions-count");

        let reactionCount = document.createTextNode(list.length.toLocaleString());
        reactionCountSpan.appendChild(reactionCount);

        if (list.length === 0) {
            return;
        }

        list.sort(function (a, b) {
            return a.timestamp - b.timestamp;
        });

        let reactionList = document.createElement("ul");
        reactionTypeSpan.appendChild(reactionList);
        reactionList.classList.add("wiki-message-reaction-list");
        reactionList.style.display = "none";
        for (let i = 0; i < list.length; i++) {
            window.wikimvcchat.addReactionToList(reactionList, list[i]);
        }

        tippy(reactionTypeSpan, {
            content(reference) {
                return reference.getElementsByClassName("wiki-message-reaction-list")[0].innerHTML;
            },
            placement: 'auto',
            allowHTML: true
        });
    },

    addReactionToList(ul, reaction) {
        let reactionListItem = document.createElement("li");
        ul.appendChild(reactionListItem);

        let username = document.createTextNode(reaction.senderName);
        if (reaction.senderExists) {
            let userLink = document.createElement("a");
            reactionListItem.appendChild(userLink);
            userLink.classList.add("wiki-username", "wiki-username-link");
            userLink.href = `/${window.wikimvcchat.userNamespace}:${reaction.senderId}`;
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

        let timestamp = document.createTextNode(new Date((list[i].timestamp / 10000) - 2208988800000).toLocaleString());
        reactionTimestamp.appendChild(timestamp);
    }
};
