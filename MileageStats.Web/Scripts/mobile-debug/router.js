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
        rootUrl = require('rootUrl'),
        $ = require('$');

    var routes = {},
        defaultRegion = '#region-to-replace',
        namedParametersPattern = /:(\w)*/g;

    if (!('onhashchange' in window)) {
        //console.log('hashchange event not supported!');
    }

    window.onhashchange = function () {
        var route = window.location.hash.replace('#', ''),
            target = matchRoute(route);

        if (target) {
            transitionTo(target);
        } else {
            //console.log('unhandled route: ' + route);
        }
    };

    function register(route, registration) {
        registration = registration || {};

        // assume that we'll fetch data unless we're told otherwise
        if (!('fetch' in registration)) registration.fetch = true;
        if (!('route' in registration)) registration.route = route;

        registration.params = route.match(namedParametersPattern);
        registration.regexp = buildRegExpForMatching(route);

        routes[route] = registration;
    }

    function transitionTo(target) {
        var registration = target.registration;
        var region = registration.region || defaultRegion;
        var route = registration.route;
        var callback = registration.callback;

        var template = getTemplateFor(route);

        function success(data, status, xhr) {
            data.__route__ = target.params;

            if (registration.prerender) {
                data = registration.prerender(data);
            }

            var view = templating.to_html(template, data);
            $(region).append(view);
            overrideLinks();
            if (callback) callback(null, data, view);
        }

        function error(xhr, status, errorThrown) {
            $(region).append('<div>' + status + ': ' + errorThrown + '</div>');
        }

        $(region).empty();

        if (registration.fetch) {
            $.ajax({
                dataType: 'json',
                type: 'GET',
                url: makeRelativeToRoot(target.url),
                success: success,
                error: error
            });
        } else {
            // do we ever need to render a template with default values?
            success({});
        }
    }

    function makeRelativeToRoot(url) {
        return (rootUrl + url).replace('//', '/');
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

    function getTemplateFor(route) {

        // templateId could be cached at registration

        var id = '#' + route
            .replace(namedParametersPattern, '')    // remove named parameters
            .replace(/^\//, '')                     // remove leading /
            .replace(/\//g, '-').toLowerCase()      // convert / to  -
            .replace(/--/g, '-')                    // collapse double -
            .replace(/-$/, '');                   // remove trailing -

        var template = $(id);
        if (template.length === 0) {
            return '<h1>No Template Found!</h1><h2>' + id + '</h2>';
        }
        return template.html();
    }

    function buildRegExpForMatching(route) {
        var pattern = route.replace(/\//g, '\\/').replace(namedParametersPattern, '(\\w)+') + '$';
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
        if (!window.location.hash) {
            if (window.location.pathname === rootUrl) {
                window.location.hash = '#/';
            } else {
                overrideLinks();
            }
        } else {
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