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

    test('expander module constructs itself', function () {
        var module = app.expander(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('expander toggles the "child" elements for the "headers" in the given view', function () {

        expect(1);

        var module = app.expander(mocks.create());

        var view = $('#qunit-fixture').html('<div><dl class="widget"><dt/><dd>content</dd></dl></div>');

        module.attach(view);
        equal(view.find('dd').css('display'), 'none');
    });

    test('expander should toggle children when the header is clicked', function () {
        expect(1);

        var module = app.expander(mocks.create());

        var view = $('#qunit-fixture').html('<div><dl class="widget"><dt/><dd>content</dd></dl></div>');
        module.attach(view);

        view.find('dt').click();
        equal(view.find('dd').css('display'), 'block');
    });

} (window.specs = window.specs || {}, window.mstats));