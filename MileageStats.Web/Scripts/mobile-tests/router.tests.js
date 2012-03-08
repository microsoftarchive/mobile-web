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

    test('router module constructs itself', function () {

        var router = app.router(mocks.create());

        ok(router != undefined, true);
        equal(typeof router, 'object');
    });

    test('router replaces view when hash changed', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();

        //assert
        ok(m.tracked.contains('empty: #view'));
        ok(m.tracked.contains('append: #view'));
        ok(m.tracked.indexOf('empty: #view') < m.tracked.indexOf('append: #view'), 'should empty before appending');
    });

    test('router infers template id from route', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();

        //assert
        ok(m.tracked.contains('html: #my-route'));
    });

    test('router invokes an ajax calls by default to url', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();

        //assert
        ok(m.tracked.contains('ajax: /my/route'));
    });

    test('router will not invoke an ajax call when registration accordingly', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route', { fetch: false });

        // simulate hash change
        m.window.location = {
            hash: '#/my/route'
        };
        m.window.onhashchange();

        //assert
        ok(!m.tracked.contains('ajax: /my/route'));
    });

    test('router modifies the href on anchor tags matching registered routes', function () {
        var overriddenLink = '';

        var m = mocks.create(
            {
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

    test('router applies expander to all widget containers', function () {
		var overriddenLink = '';

		var m = mocks.create();
		var router = app.router(m);

		router.setDefaultRegion('#view');
		router.register('/my/route');

		// simulate hash change
		m.window.location = {
       		hash: '#/my/route'
		};
		m.window.onhashchange();

		ok(m.tracked.contains('expander: dl.widget'));
    });

    test('router modifies the href on anchor tags matching registered routes with named args', function () {
        var overriddenLink = '';
        var m = mocks.create(
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

    test('router matches invokes correct ajax url when route has a named arg', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route/1'
        };
        m.window.onhashchange();

        //assert
        ok(m.tracked.contains('ajax: /my/route/1'));
    });

    test('router matches template with a named arg', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route/1'
        };
        m.window.onhashchange();

        //assert
        ok(m.tracked.contains('html: #my-route'));
    });

    test('router matches template with an embedded named arg', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/my/route/:id/more');

        // simulate hash change
        m.window.location = {
            hash: '#/my/route/1/more'
        };
        m.window.onhashchange();

        //assert
        ok(m.tracked.contains('html: #my-route-more'));
    });

    test('router invoke transition when first initialized if # is in the url', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // simulate hash change
        m.window.location = {
            hash: '#/inital/page'
        };

        router.initialize();

        // assert
        // this assert is that we clearing the region
        // it is one of many things that happen during
        // a transition
        ok(m.tracked.contains('empty: #view'));
        ok(m.tracked.length > 2);
    });

    test('router will not invoke transition when first initialized if no # is in the url', function () {

        var m = mocks.create();
        var router = app.router(m);

        router.setDefaultRegion('#view');
        router.register('/inital/page');

        // simulate hash change
        m.window.location = {
            hash: ''
        };

        router.initialize();

        // assert
        // should only contain the calls related to 
        // overriding links
        // this assertion is somewhat fragile
        // but it is sometimes diffiuclt to make
        // a negative assertion
        ok(m.tracked.contains('attr: a[href] item'));
        equal(m.tracked.length, 1);
    });

} (window.specs = window.specs || {}, window.mstats));