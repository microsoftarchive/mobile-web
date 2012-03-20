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

    var window = require('window'),
        rootUrl = require('rootUrl'),
	    transition = require('transition'),
        app = require('mstats'),
        $ = require('$');

    var routes = {},
        supportsHashChange = ('onhashchange' in window),
        defaultRegion = '#region-to-replace',
        namedParametersPattern = /:(\w)*/g;

    if (supportsHashChange) {
        window.onhashchange = function () {
            var route = window.location.hash.replace('#', ''),
                target = matchRoute(route);

            if (target) {
                transition.to(target, defaultRegion, namedParametersPattern, function () {
                    overrideLinks();
                });

            } else {
                // console.log('unhandled route: ' + route);
            }
        };
    }

    function register(route, registration) {
        registration = registration || {};

        // assume that we'll fetch data unless we're told otherwise
        if (!('fetch' in registration)) registration.fetch = true;
        if (!('route' in registration)) registration.route = route;

        registration.params = route.match(namedParametersPattern);
        registration.regexp = buildRegExpForMatching(route);

        routes[route] = registration;
    }

    function matchRoute(url) {

        var item,
            registration,
            match,
            result;

        for (item in routes) {
            registration = routes[item];
            match = url.match(registration.regexp);

            if (match !== null) {
                result = buildMatchResult(match, registration);
                result.url = match[0];
                return result;
            }
        }

        return false;
    }

    function buildMatchResult(match, registration) {

        var result = {
            registration: registration,
            params: {}
        },
            params = registration.params,
            named,
            i;

        if (!params) return result;

        for (i = 0; i < params.length; i++) {
            named = params[i].slice(1);
            result.params[named] = match[i + 1];
        }

        return result;
    }

    function buildRegExpForMatching(route) {
        var pattern = route.replace(/\//g, '\\/').replace(namedParametersPattern, '(\\w+)') + '$';
        return new RegExp(pattern);
    }

    function overrideLinks() {

        // rewriting the links this way is very heavy
        // another approach would be to do this check
        // in the click handler for the link

        $('a[href]').each(function (i, a) {
            var anchor = $(a);
            var match;
            var href = anchor.attr('href');
            if (href.indexOf('#') === -1 && (match = matchRoute(href))) {
                anchor.attr('href', rootUrl + '#' + match.url);
            }
        });
    }

    function initialize() {
        if (!supportsHashChange) return;

        var pathname = window.location.pathname;

        if (!window.location.hash) {
            // the initial hit of the page, w/o any hash
            window.location.hash = '#/' + pathname.replace(rootUrl, '');
            overrideLinks();
        } else if (window.location.hash === '#/') {
            // the root hash, probably the result of a refresh
            window.onhashchange();
        } else {
            // if the page is refresh, and the hash is something
            // other than the root, then we don't want to load 
            // the initial model
            app.initialModel = null;
            window.onhashchange();
        }
    }

    return {
        register: register,
        setDefaultRegion: function (val) {
            defaultRegion = val;
        },
        initialize: initialize
    };

};