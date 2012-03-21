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

        var $ = require('$');

        var cache = {};

        function get(token) {
            return cache[token];
        }

        function set(token, payload) {
            cache[token] = payload;
        }

        function clear(token) {
            cache[token] = undefined;
        }

        function enforceInvalidations(url) {
            if (url.toLowerCase().indexOf('add') > -1 || url.toLowerCase().indexOf('edit') > -1) {
                cache = { };
            }
        }

        function request(options) {
            var url = options.url,
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