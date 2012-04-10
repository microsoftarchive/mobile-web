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

    module('router specs', {
        setup: function () {
            defaultMocks = {
                transition: {
                    to: function (target, defaultRegion, namedParametersPattern, callback) { callback(); }
                },
                window: { onhashchange: function () { } },
                mstats: {
                    rooturl: '/'
                },
                document: $('<div/>')[0]
            };
        }
    });

    test('router module constructs itself', function () {
        var router = app.router(mocks.create(defaultMocks));

        ok(router != undefined, true);
        equal(typeof router, 'object');
    });

    test('router delegates to transition when hash changed', function () {
        expect(1);

        defaultMocks.transition = {
            to: function () {
                var view = arguments[1];
                equal(view, '#view');
            }
        };
        var m = mocks.create(defaultMocks);

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // Simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();
    });

    test('router modifies the href on anchor tags matching registered routes', function () {
        expect(1);

        var dom = $('<div><a href="/my/route"/></div>');
        defaultMocks.document = dom;
        var m = mocks.create(defaultMocks);

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // Simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();

        // assert
        equal(dom.find('a').attr('href'), '/#/my/route');
    });

    test('router modifies the href on anchor tags matching registered routes with named args', function () {
        expect(1);

        var dom = $('<div><a href="/my/route/1"/></div>');

        defaultMocks.document = dom;
        var m = mocks.create(defaultMocks);

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id');

        // Simulate hash change
        router.register('/something/unrelated');
        m.window.location = {
            hash: '#/something/unrelated'
        };
        m.window.onhashchange();

        //assert
        equal(dom.find('a').attr('href'), '/#/my/route/1');
    });

    test('router invoke transition when first initialized if # is in the url', function () {
        expect(1);

        var transition_invoked = false;

        defaultMocks.transition = {
            to: function () { transition_invoked = true; }
        };
        var m = mocks.create(defaultMocks);
        var module = app.router(m);

        module.setDefaultRegion('#view');
        module.register('/inital/page');

        // Simulate hash change
        m.window.location = {
            hash: '#/inital/page'
        };

        module.initialize();

        // assert
        ok(transition_invoked);
    });

    test('router will not invoke transition when first initialized if no # is in the url', function () {

        var transition_not_invoked = true;

        defaultMocks.transition = {
            to: function () { transition_not_invoked = false; }
        };
        var m = mocks.create(defaultMocks);

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // Simulate hash change
        m.window.location = {
            hash: '',
            pathname: ''
        };

        router.initialize();

        ok(transition_not_invoked);
    });

    test('router invoke transition with the correct params for the target', function () {

        expect(1);

        defaultMocks.transition = {
            to: function (target, defaultRegion, namedParametersPattern, callback) {
                equal(target.params.id, 123);
            }
        };

        var m = mocks.create(defaultMocks);

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id');

        // Simulate hash change
        m.window.location = {
            hash: '#/my/route/123'
        };

        router.initialize();
    });

    test('router resets initial model during initialized if # is not the root', function () {

        expect(2);

        var initialModel = {};

        var m = mocks.create(defaultMocks);
        m.mstats.initialModel = initialModel;

        var router = app.router(m);
        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // Simulate hash change
        m.window.location = {
            hash: '#/inital/page'
        };

        equal(m.mstats.initialModel, initialModel);

        router.initialize();

        // assert
        equal(m.mstats.initialModel, null);
    });

    test('router does not resets initial model during initialized if # is the root', function () {

        expect(1);

        var initialModel = {};

        var m = mocks.create(defaultMocks);
        m.mstats.initialModel = initialModel;

        var router = app.router(m);
        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // Simulate hash change
        m.window.location = {
            hash: '#/'
        };

        router.initialize();

        // assert
        equal(m.mstats.initialModel, initialModel);
    });

    test('router should respect root url when overriding links', function () {
        expect(1);

        var dom = $('<div><a href="/virtual.directory/my/route"/></div>');

        defaultMocks.document = dom;
        defaultMocks.rootUrl = '/virtual.directory/';
        var m = mocks.create(defaultMocks);

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // Simulate hash change
        router.register('/something/unrelated');
        m.window.location = {
            hash: '#/something/unrelated'
        };
        m.window.onhashchange();

        //assert
        equal(dom.find('a').attr('href'), '/virtual.directory/#/my/route');
    });

} (window.specs = window.specs || {}, window.mstats));