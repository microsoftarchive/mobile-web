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

    module('expander specs');

    var stubFunction = function () {
    };

    var verifyFunction = function() { ok(true); };

    test('expander module constructs itself', function () {
        var module = app.expander(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('expander searches for "headers" in the given view', function () {

        var module = app.expander(mocks.create());
        var header = '';

        var view = {
            find: function (selector) {
                header = selector;
                return {
                    next: function () {
                        return {
                            toggle: stubFunction
                        };
                    },
                    click: stubFunction
                };
            }
        };

        module.attach(view);

        equal(header, 'dl.widget dt');
    });

    test('expander toggles the "child" elements for the "headers" in the given view', function () {

        expect(2);

        var module = app.expander(mocks.create());
        var child = '';

        var view = {
            find: function () {
                return {
                    next: function (selector) {
                        child = selector;
                        return {
                            toggle: verifyFunction
                        };
                    },
                    click: stubFunction
                };
            }
        };

        module.attach(view);

        equal(child, 'dd');
    });

    test('expander attaches a click handler to "headers"', function () {
        expect(1);

        var m = mocks.create();
        var module = app.expander(m);

        var view = {
            find: function () {
                return {
                    next: function () {
                        return {
                            toggle: stubFunction
                        };
                    },
                    click: function (handler) {
                        equal(typeof handler, 'function');
                    }
                };
            }
        };

        module.attach(view);
    });

    test('expander should toggle children when the header is clicked', function () {

        expect(2);
        var m = mocks.create();
        var module = app.expander(m);

        var view = {
            find: function () {
                return {
                    next: function () {
                        return {
                            toggle: stubFunction
                        };
                    },
                    click: function (handler) {
                        handler.call('child', {
                            preventDefault: verifyFunction
                        });
                    }
                };
            }
        };

        module.attach(view);

        ok(m.tracked.contains('dd.toggle()'));
    });

} (window.specs = window.specs || {}, window.mstats));