"use strict";
var routes = [
    {
        path: '/kategori/:slug/',
        async: function (routeTo, routeFrom, resolve, reject) {
            var slug = routeTo.params.slug;

            // Framework7'nin AJAX isteði yapmasýný engelle
            window.location.href = "/kategori/" + slug;
        }
    },

    {
        path: '/urun/:slug/',
        async: function (routeTo, routeFrom, resolve, reject) {
            var slug = routeTo.params.slug;
            window.location.href = "/urun/" + slug;
        }
    },

    {
        path: '/secim',
        async: function (routeTo, routeFrom, resolve, reject) {
            var slug = routeTo.params.slug;
            window.location.href = "/secim/";
        }
    },
    {
        path: '/main',
        async: function (routeTo, routeFrom, resolve, reject) {
            var slug = routeTo.params.slug;
            window.location.href = "/main/";
        }
    },
    {
        path: '/kategoriler',
        async: function (routeTo, routeFrom, resolve, reject) {
            var slug = routeTo.params.slug;
            window.location.href = "/kategoriler/";
        }
    },
    {
        path: '/iletisim',
        async: function (routeTo, routeFrom, resolve, reject) {
            var slug = routeTo.params.slug;
            window.location.href = "/iletisim/";
        }
    },
];
