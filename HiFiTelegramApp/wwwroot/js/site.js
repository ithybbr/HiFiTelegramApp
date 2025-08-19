document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.nav-link[data-url]')
        .forEach(link => {
            link.addEventListener('click', function () {
                console.log('Link clicked:', this.dataset.url);
                loadAjax(this.dataset.url);
            });
        });
});
function links() {
    document.querySelectorAll('.link[data-url]')
        .forEach(link => {
            link.addEventListener('click', function () {
                console.log('Link clicked:', this.dataset.url);
                loadAjax(this.dataset.url);
            });
        });
}
var search = false;
function loadSearch() {
    document.querySelector('.search #search-input').addEventListener('input', function () {
        console.log('Search input changed:', this.value);
        $.ajax({
            url: "/Home/search",
            data: { query: this.value},
            type: 'GET',
            success: function (data) {
                var temp = $('<div>').html(data);
                var rows = temp.find('li').map(function () { return this.outerHTML; }).get();
                clusterize.update(rows);
                search = true;
            },
            failure: function (xhr, status, error) {
                console.error('Error appending data:', status, error);
            }
        })
    }, 450);
}

function disableButton() {
    setTimeout(() => {
        showSpinner();
        this.disabled = true;
        this.style.color = "#666";
        // then re-enable later if you like…
        setTimeout(() => {
            this.disabled = false;
            console.log("Button activated");
            hideSpinner();
        }, 15000);
    }, 0);
}
function showSpinner() { document.getElementById('loading-spinner').style.display = 'block'; }
function hideSpinner() { document.getElementById('loading-spinner').style.display = 'none'; }

var clusterize;
function loadAjax(url) {
    showSpinner();
    console.log('Loading URL:', url);
    $.ajax({
        url: url,
        type: 'GET',
        success: function (html) {
            search = false; // Reset search state
            try {
                activeCss.disabled = true;
            }
            catch (e) {
                console.error('Error removing previous CSS:', e);
            }
            var css = url.split('/')[2].concat('-css');
            console.log('Loading CSS:', css);
            try {
                enableCSS(css);
            }
            catch (e) {
                console.error('Error enabling CSS:', e);
            }
            $('#append-ajax').html(html);
            if (url == "/Home/artists") {
                var data = ['<li>…</li>'];
                clusterize = new Clusterize({
                    row: data,
                    scrollId: 'scrollArea',
                    contentId: 'contentArea',
                    callbacks: {
                        scrollingProgress: function (progress) {
                            if (!search && progress > 60) {
                                appendClusterize("/Home/list");
                            }
                        }
                    }
                });
                clusterize.clear();
                appendClusterize("/Home/list");
            }
            else {
            }
            try {
                loadSearch();
            }
            catch (e) {
                console.error('Error loading search functionality:', e);
            }
            loadFavorites();
            links();
            hideSpinner();
        },
        failure: function (xhr, status, error) {
            console.error('Error loading data:', status, error);
            hideSpinner();
        }
    })
}
function appendClusterize(url) {
    console.log('Appending URL:', url);
    console.log('Current rows amount:', clusterize.getRowsAmount());
    $.ajax({
        url: url,
        data: { offset: clusterize.getRowsAmount()},
        type: 'GET',
        success: function (data) {
            var temp = $('<div>').html(data);
            var rows = temp.find('li').map(function () { return this.outerHTML; }).get();
            clusterize.append(rows);
            links();
        },
        failure: function (xhr, status, error) {
            console.error('Error appending data:', status, error);
        }
    })
}
//$(document).on('submit', 'form', function (e) {
//    e.preventDefault();
//});
var activeCss;
function enableCSS(css) {
    activeCss = document.getElementById(css);
    activeCss.disabled = false;
}
function loadFavorites() {
    if (document.querySelector('#favorites')) {
        console
        $.ajax({
            url: '/favorite/favorites',
            method: 'GET',
            success: function (data) {
                $('.list-group').html(data);
            },
            error: function (error) {
                console.error('Error fetching favorites:', error);
            }
        })
    }
}