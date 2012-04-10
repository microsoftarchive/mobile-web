/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

(function (specs, app) {

    var defaultMocks;

    module('transition specs', {
        setup: function () {
            defaultMocks = {
                mstats: {
                    rooturl: '/'
                },
                document: $('<div/>')[0],
                ajax: {
                    request: function (options) { options.success(); }
                }
            };

        },
        teardown: function () {
        }
    });

    test('transition module constructs itself', function () {
        var module = app.transition(mocks.create(defaultMocks));

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('module infers template id from route', function () {
        expect(1);

        var dom = $('<div><script id="my-route" type="text/html">template</script><div id="view">old content</div></div>');
        defaultMocks.document = dom;
        var m = mocks.create(defaultMocks);
        var transition = app.transition(m);

        transition.to({ registration: { route: '/my/route'} }, '#view');

        var content = dom.find('#view').html();
        equal(content, 'template');
    });

    test('module invokes an ajax calls by default to url when registered to fetch', function () {
        expect(1);

        defaultMocks.ajax.request = function(options) {
            equal(options.url, '/my/route?format=json');
        };
        
        var transition = app.transition(mocks.create(defaultMocks));

        transition.to({
            url: '/my/route',
            registration: {
                route: '/my/route',
                fetch: true
            }
        }, '#view');
    });

    test('module appends format to url before making an ajax calls registered to fetch', function () {
        expect(1);

        defaultMocks.ajax.request = function (options) {
            ok(options.url.indexOf('format=json') > -1);
        };
        
        var transition = app.transition(mocks.create(defaultMocks));

        transition.to({
            url: '/my/route',
            registration: {
                route: '/my/route',
                fetch: true
            }
        }, '#view');
    });

    test('module will not invoke an ajax call when registrater not to fetch', function () {

        defaultMocks.ajax.request = function (options) {
            ok(false, 'ajax should not be invoked');
        };

        var transition = app.transition(mocks.create(defaultMocks));

        transition.to({
            url: '/my/route',
            registration: {
                route: '/my/route',
                fetch: false
            }
        }, '#view');
    });

    test('module matches invokes correct ajax url when route has a named arg', function () {
        expect(1);

        defaultMocks.ajax.request = function (options) {
            equal(options.url, '/my/route/1?format=json');
        };

        var transition = app.transition(mocks.create(defaultMocks));

        transition.to({
            url: '/my/route/1',
            registration: {
                route: '/my/route/:id',
                fetch: true
            }
        }, '#view');
    });

    test('module matches template with a named arg when the correct regex is passed', function () {
        expect(1);

        var dom = $('<div> <script id="my-route" type="text/html">template</script> <div id="view">old content</div> </div>');
        defaultMocks.document = dom;

        var transition = app.transition(mocks.create(defaultMocks));

        transition.to({
            url: '/my/route/1',
            registration: {
                route: '/my/route/:id'
            }
        }, '#view', /:(\w)*/g);
        // This regex is hard-coded into the router.

        var content = dom.find('#view').html();
        equal(content, 'template');
    });

    test('module matches template with an embedded named arg when the correct regex is passed', function () {
        expect(1);

        var dom = $('<div> <script id="my-route-more" type="text/html">template</script> <div id="view">old content</div> </div>');
        defaultMocks.document = dom;

        var transition = app.transition(mocks.create(defaultMocks));

        transition.to({
            url: '/my/route/1/more',
            registration: {
                route: '/my/route/:id/more'
            }
        }, '#view', /:(\w)*/g);
        // This regex is hard-coded into the router.

        var content = dom.find('#view').html();
        equal(content, 'template');
    });


} (window.specs = window.specs || {}, window.mstats));