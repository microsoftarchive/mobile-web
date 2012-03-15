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

    module('transition specs');

    test('transition module constructs itself', function () {
        var module = app.transition(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('module infers template id from route', function () {

        var m = mocks.create();
        var transition = app.transition(m);

        transition.to({ registration: { route: '/my/route'} }, '#view');

        //assert
        ok(m.tracked.contains('#my-route.html()'));
    });

    test('module invokes an ajax calls by default to url when registered to fetch', function () {

        var m = mocks.create();
        var transition = app.transition(m);

        transition.to({
            url: '/my/route',
            registration: {
             route: '/my/route',
             fetch: true
         }
        }, '#view');

        //assert
           ok(m.tracked.contains('ajax: /my/route?format=json'));
    });

    test('module appends format to url before making an ajax calls registered to fetch', function () {

		var m = mocks.create();
		var transition = app.transition(m);

		transition.to({
       		url: '/my/route',
       		registration: {
       			route: '/my/route',
       			fetch: true
       		}
		}, '#view');

		//assert
		ok(m.tracked.contains('ajax: /my/route?format=json'));
    });

    test('module will not invoke an ajax call when registrater not to fetch', function () {

        var m = mocks.create();
        var transition = app.transition(m);

        transition.to({
            url: '/my/route',
            registration: {
                route: '/my/route',
                fetch: false
            }
        }, '#view');

        //assert
        ok(!m.tracked.contains('ajax: /my/route'));
    });

    test('module matches invokes correct ajax url when route has a named arg', function () {

        var m = mocks.create();
        var transition = app.transition(m);

        transition.to({
            url: '/my/route/1',
            registration: {
                route: '/my/route/:id',
                fetch: true
            }
        }, '#view');


        //assert
         ok(m.tracked.contains('ajax: /my/route/1?format=json'));
    });

    test('module matches template with a named arg when the correct regex is passed', function () {

        var m = mocks.create();
        var transition = app.transition(m);

        transition.to({
            url: '/my/route/1',
            registration: {
                route: '/my/route/:id',
                fetch: true
            }
        }, '#view', /:(\w)*/g);
        // this regex is hard-coded into the router

        ok(m.tracked.contains('#my-route.html()'));
    });

    test('module matches template with an embedded named arg when the correct regex is passed', function () {

        var m = mocks.create();
        var transition = app.transition(m);

        transition.to({
            url: '/my/route/1/more',
            registration: {
                route: '/my/route/:id/more',
                fetch: true
            }
        }, '#view', /:(\w)*/g);
        // this regex is hard-coded into the router

        //assert
        ok(m.tracked.contains('#my-route-more.html()'));
    });


} (window.specs = window.specs || {}, window.mstats));