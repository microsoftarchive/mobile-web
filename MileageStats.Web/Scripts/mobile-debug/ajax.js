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

(function (mstats) {
    mstats.ajax = function (require) {

        var rootUrl = require('rootUrl'),
            $ = require('$');

        var cache = {},
        namedParametersPattern = /:(\w)*/g;

        var invalidations = {
            '/Vehicle/Add': ['/', '/Charts'],
            '/Vehicle/:id/Fillup/Add': ['/', '/Charts', '/Vehicle/:id/Fillup/List', '/Vehicle/:id/Details'],
            '/Vehicle/:id/Edit': ['/', '/Charts', '/Vehicle/:id/Details'],
            '/Vehicle/Delete/:id': ['/', '/Charts', '/Vehicle/:id/Details'],
            '/Vehicle/:id/Reminder/Add': ['/', '/Vehicle/:id/Reminder/ListByGroup'],
            '/Vehicle/:id/Reminder/:reminderId/Fulfill': ['/', '/Vehicle/:id/Reminder/ListByGroup', '/Vehicle/:id/Reminder/:reminderId/Details']
        };

        function buildRegExpForMatching(route) {
            var pattern = route.replace(/\//g, '\\/').replace(namedParametersPattern, '(\\w+)') + '$';
            return new RegExp(pattern);
        }

        function get(token) {
            return cache[token];
        }

        function set(token, payload) {
            cache[token] = payload;
        }

        function clear(token) {
            if (cache[token]) {
                delete cache[token];
            }
        }

        function enforceInvalidations(url) {
            var entry,
                re,
                params,
                routesToRemove,
                actualUrls;

            for (entry in invalidations) {
                re = buildRegExpForMatching(entry);
                if (url.match(re)) {
                    params = extractedNameParameters(re, entry, url);
                    routesToRemove = invalidations[entry];
                    actualUrls = combineValuesWithRoutes(params, routesToRemove);
                    invalidate(actualUrls);
                }
            }
        }

        function invalidate(entries) {
            forEach(entries, clear);
        }

        function extractedNameParameters(re, entry, url) {
            var named, i;
            var values = url.match(re);
            var names = entry.match(namedParametersPattern);

            var params = {};

            if (names) {
                for (i = 0; i < names.length; i++) {
                    named = ':' + names[i].slice(1);
                    params[named] = values[i + 1];
                }                
            }

            return params;
        }

        function combineValuesWithRoutes(params, routes) {

            var urls = [];

            forEach(routes, function (route) {
                var name;
                for (name in params) {
                    route = route.replace(name, params[name]);
                    route = route.replace('/', rootUrl);
                }
                urls.push(route);
            });

            return urls;
        }

        function forEach(array, op) {
            var len = array.length,
                index = 0;

            for (; index < len; index++) {
                op(array[index]);
            }
        }

        function request(options) {
            var url = options.url.replace('?format=json', ''),
                success = options.success,
                cachedData = get(url);

            if (cachedData) {
                options.success(cachedData);
                return;
            }

            options.success = function (data) {
                set(url, data);
                success(data);
            };

            $.ajax(options);
        }

        function post(options) {
            var url = options.url,
                success = options.success;

            options.success = function (data) {
                enforceInvalidations(url);
                success(data);
            };

            $.ajax(options);
        }

        return {
            request: request,
            post: post
        };
    };
} (this.mstats = this.mstats || {}));