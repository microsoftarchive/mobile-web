/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
ARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

(window.mstats = window.mstats || {}).router = function (require) {

    var templating = require('Mustache'),
        window = require('window'),
        log = require('log'),
        $ = require('$');

    var routes = {},
        defaultRegion = '#region-to-replace';

    if (!('onhashchange' in window)) {
        log('hashchange event not supported!');
    }

    window.onhashchange = function () {

        var route = window.location.hash.replace('#', '');
        var registration = routes[route];

        if (registration) {
            transitionTo(registration);
        } else {
            log('unhandled route: ' + route);
        }
    };

    function transitionTo(registration) {
        var region = registration.region || defaultRegion;
        var route = registration.route;
        var callback = registration.callback;

        var template = getTemplateFor(route);

        function success(data) {
            var view = templating.to_html(template, data);
            $(region).append(view);
            overrideLinks();
            if (callback) callback(null, data, view);
        }

        $(region).empty();

        if (registration.fetch) {
            $.ajax({
                dataType: 'json',
                type: 'GET',
                url: route,
                success: success
            });
        } else {
            // do we ever need to render a template with default values?
            success({});
        }
    }

    function getTemplateFor(route) {
        // results could be cached
        if (route.substring(0, 1) === '/') route = route.replace('/', '');
        var id = '#' + route.replace(/\//g, '-').toLowerCase();
        return $(id).html();
    }

    function register(route, registration) {
        registration = registration || {};
        // assume that we'll fetch data unless we're told otherwise
        if (!('fetch' in registration)) registration.fetch = true;
        if (!('route' in registration)) registration.route = route;

        routes[route] = registration;
    }

    function overrideLinks() {
        // this could be scoped, so that it doesn't search the entire page
        var route;
        for (route in routes) {
            $('a[href="' + route + '"]').attr('href', '#' + route);
        }
    }

    return {
        register: register,
        setDefaultRegion: function (val) {
            defaultRegion = val;
        },
        initialize: overrideLinks
    };

};