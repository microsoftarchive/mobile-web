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

(window.mstats = window.mstats || {}).transition = function (require) {

    var templating = require('Mustache'),
        rootUrl = require('rootUrl'),
        $ = require('$');

    var cssClassForTransition = 'swapping';

    function transitionTo(target, defaultRegion, namedParametersPattern, callback) {

        var registration = target.registration,
            region = registration.region || defaultRegion,
            host = $(region),
            route = registration.route;

        var template = getTemplateFor(route, namedParametersPattern);
        var onSuccess = success(host, template, target, callback);

        host.toggleClass(cssClassForTransition);

        if (registration.fetch) {

            $.ajax({
                dataType: 'json',
                type: 'GET',
                url: makeRelativeToRoot(target.url),
                success: onSuccess,
                error: error(host)
            });

        } else {
            // do we ever need to render a template with default values?
            onSuccess({});
        }
    }

    function success(host, template, target, callback) {

        var registration = target.registration;

        return function (model, status, xhr) {

            var view;

            // append route data to the model 
            // we use the well known name '__route__'
            // assuming that it will be unlikely to
            // collide with any existing properties
            model.__route__ = target.params;

            if (registration.prerender) {
                model = registration.prerender(model);
            }

            view = templating.to_html(template, model);
            host.empty();
            host.append(view);

            host.toggleClass(cssClassForTransition);

            if (callback) callback(null, data, view);
        };
    }

    function error(host) {

        return function (xhr, status, errorThrown) {
            host.empty();
            host.append('<div>' + status + ': ' + errorThrown + '</div>');
        };
    }

    function makeRelativeToRoot(url) {
        return (rootUrl + url).replace('//', '/');
    }

    function getTemplateFor(route, namedParametersPattern) {

        // templateId could be cached at registration

        var id = '#' + route
            .replace(namedParametersPattern, '')    // remove named parameters
            .replace(/^\//, '')                     // remove leading /
            .replace(/\//g, '-').toLowerCase()      // convert / to  -
            .replace(/--/g, '-')                    // collapse double -
            .replace(/-$/, ''); // remove trailing -

        var template = $(id);
        if (template.length === 0) {
            return '<h1>No Template Found!</h1><h2>' + id + '</h2>';
        }
        return template.html();
    }

    return {
        to: transitionTo
    };

};