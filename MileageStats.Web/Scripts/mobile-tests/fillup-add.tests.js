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

    module('fillup add specs');

    test('fillup module constructs itself', function () {
        var module = app.fillupAdd(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('fillup module should invoke unobtrusive validation', function () {

        expect(1);

        var m = mocks.create();
        var mockForm = m.$();
        mockForm.submit = function () { };

        m.$.validator = {
            unobtrusive: {
                parse: function (form) {
                    ok(true);
                }
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockForm, {});
    });

    test('fillup module should attach submit handler on form', function () {

        expect(1);

        var m = mocks.create();
        
        m.$.validator = {
            unobtrusive: {
                parse: function (form) {
                }
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, m.$('view'), {});

        ok(m.tracked.contains('form.submit()'));

    });

    test('fillup module should search for a form tag', function () {

        expect(1);

        var m = mocks.create();
        
        m.$.validator = {
            unobtrusive: {
                parse: function (form) {
                }
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, m.$('view'), {});

        ok(m.tracked.contains('view.find(form)'));
    });

} (window.specs = window.specs || {}, window.mstats));