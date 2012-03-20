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

(function (specs, app) {

    module('router specs');

    var default_mocks = {
        transition: {
            to: function (target, defaultRegion, namedParametersPattern, callback) { callback(); }
        },
        window: { onhashchange: function () { } },
        mstats: {
            rooturl: '/'
        }
    };

    test('router module constructs itself', function () {
        var router = app.router(mocks.create(default_mocks));

        ok(router != undefined, true);
        equal(typeof router, 'object');
    });

    test('router delegates to transition when hash changed', function () {

        expect(1);

        var m = mocks.create({
            transition: {
                to: function () {
                    var view = arguments[1];
                    equal(view, '#view');
                }
            },
            window: { onhashchange: function () { } },
            mstats: {
                rooturl: '/'
            }
        });

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();


    });

    test('router modifies the href on anchor tags matching registered routes', function () {
        var overriddenLink = '';

        var m = mocks.create(default_mocks,
            {
                // when attr is called inside the module
                // we expect it to be called with a
                // modified url that includes a #
                '$.attr': function (name, value) {
                    if (!value) return '/my/route';
                    overriddenLink = value;
                }
            });

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();

        // assert
        equal(overriddenLink, '/#/my/route');
    });

    test('router modifies the href on anchor tags matching registered routes with named args', function () {
        var overriddenLink = '';
        var m = mocks.create(default_mocks,
            {
                '$.attr': function (name, value) {
                    if (!value) return '/my/route/1';
                    overriddenLink = value;
                }
            }
        );

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route/x'
        };
        m.window.onhashchange();

        //assert
        equal(overriddenLink, '/#/my/route/1');
    });

    test('router invoke transition when first initialized if # is in the url', function () {

        expect(1);

        var transition_invoked = false;

        var m = mocks.create({
            transition: {
                to: function () { transition_invoked = true; }
            },
            window: { onhashchange: function () { } },
            mstats: {
                rooturl: '/'
            }
        });

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // simulate hash change
        m.window.location = {
            hash: '#/inital/page'
        };

        router.initialize();

        // assert
        ok(transition_invoked);
    });

    test('router will not invoke transition when first initialized if no # is in the url', function () {

        var transition_not_invoked = true;

        var m = mocks.create({
            transition: {
                to: function () { transition_not_invoked = false; }
            },
            window: { onhashchange: function () { } },
            mstats: {
                rooturl: '/'
            }
        });
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // simulate hash change
        m.window.location = {
            hash: '',
            pathname: ''
        };

        router.initialize();

        ok(transition_not_invoked);
    });

    test('router invoke transition with the correct params for the target', function () {

        expect(1);

        var m = mocks.create({
            transition: {
                to: function (target, defaultRegion, namedParametersPattern, callback) {
                    equal(target.params.id, 123);
                }
            },
            window: { onhashchange: function () { } },
            mstats: {
                rooturl: '/'
            }
        });

        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route/123'
        };

        router.initialize();
    });

    test('router resets initial model during initialized if # is not the root', function () {

        expect(2);

        var initialModel = {};

        var m = mocks.create({
            transition: {
                to: function () { }
            },
            window: { onhashchange: function () { } },
            mstats: {
                rooturl: '/',
                initialModel: initialModel
            }
        });

        var router = app.router(m);
        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // simulate hash change
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

        var m = mocks.create({
            transition: {
                to: function () { }
            },
            window: { onhashchange: function () { } },
            mstats: {
                rooturl: '/',
                initialModel: initialModel
            }
        });

        var router = app.router(m);
        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // simulate hash change
        m.window.location = {
            hash: '#/'
        };

        router.initialize();

        // assert
        equal(m.mstats.initialModel, initialModel);
    });
} (window.specs = window.specs || {}, window.mstats));