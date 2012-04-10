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

    module('expander specs');

    test('expander module constructs itself', function () {
        var module = app.expander(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('expander applies "closed" class to widgets in the given view', function () {

        expect(1);

        var module = app.expander(mocks.create());

        var view = $('#qunit-fixture').html('<div><dl class="widget"><dt/><dd>content</dd></dl></div>');

        module.attach(view);
        equal(view.find('.widget').attr('class'), 'widget closed');
    });

    test('expander should remove "closed" class when the header is clicked', function () {
        expect(1);

        var module = app.expander(mocks.create());

        var view = $('#qunit-fixture').html('<div><dl class="widget"><dt/><dd>content</dd></dl></div>');
        module.attach(view);

        view.find('dt').click();
        equal(view.find('.widget').attr('class'), 'widget');
    });

    test('expander should not react when a child element is clicked', function () {
        expect(1);

        var module = app.expander(mocks.create());

        var view = $('#qunit-fixture').html('<div><dl class="widget"><dt/><dd>content</dd></dl></div>');
        module.attach(view);
        view.find('dt').click(); // simulate opening the widget
        
        view.find('dd').click(); // simulate clicking a child
        equal(view.find('.widget').attr('class'), 'widget');
    });

} (window.specs = window.specs || {}, window.mstats));