﻿import tippy from 'tippy.js';

window.addEventListener('load', function () {
    window.wikiAutosuggestXHR = new XMLHttpRequest();

    var search = this.document.getElementById("searchInput");
    if (search) {
        search.addEventListener('keyup', function (event) {
            const input = event.target;
            if (input.value.length < 3) {
                return;
            }
            var searchSuggestions = document.getElementById("searchSuggestions");
            if (searchSuggestions) {
                window.wikiAutosuggestXHR.abort();
                window.wikiAutosuggestXHR.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                        var response = JSON.parse(this.responseText);
                        searchSuggestions.innerHTML = "";
                        response.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item;
                            searchSuggestions.appendChild(option);
                        });
                    }
                };
                window.wikiAutosuggestXHR.open("GET", "/wiki/api/suggest?search=" + encodeURIComponent(input.value), true);
                window.wikiAutosuggestXHR.send();
            }
        });
    }
});

window.wikimvc = {
    tmr: -1,
    showPreview: async function (e, link) {
        e = e || window.event;
        const target = e.currentTarget;

        if (target == null || target._tippy) {
            return;
        }

        var formData = new FormData();
        formData.append('link', link);

        let t = tippy(target, {
            content: 'Loading preview...',
            delay: [1500, null],
            placement: 'auto',
            allowHTML: true,
            onCreate(instance) {
                instance._isFetching = false;
                instance._loaded = false;
            },
            onShow(instance) {
                if (instance._isFetching || instance._loaded) {
                    return;
                }

                instance._isFetching = true;

                fetch('/wiki/api/preview', {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'Accept': 'application/json'
                    }
                })
                    .then(response => response.json())
                    .then(json => {
                        if (json.length < 1) {
                            instance.hide();
                            instance.disable();
                        } else {
                            instance.setContent(json);
                        }
                    })
                    .catch(error => {
                        // console.error(error);
                        instance.setContent('Preview failed to load');
                    })
                    .finally(() => {
                        instance._loaded = true;
                        instance._isFetching = false;
                        instance.setProps({
                            delay: [750, null],
                        });
                    });
            }
        });

        window.wikimvc.tmr = setTimeout(function () {
            t.show();
        }, 1500);
    },

    hidePreview: function () {
        if (window.wikimvc.tmr !== -1) {
            clearTimeout(window.wikimvc.tmr);
            window.wikimvc.tmr = -1;
        }
    },

    showHideMessage: function (div) {
        let height = parseInt(getComputedStyle(div).getPropertyValue("height"));
        let maxHeight = parseInt(getComputedStyle(div).getPropertyValue("max-height"));
        if (height < maxHeight) {
            div.parentElement.classList.remove("collapsible");
        }
    },
};
