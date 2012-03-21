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

    var ajax = $.ajax,
        validator = $.validator;

    module('formSubmitter specs', {
        setup: function () {
            $.validator = stubUnobtrusive;
        },
        teardown: function () {
            $.validator = validator;
            $.ajax = ajax;
        }
    });

    test('formSubmitter module constructs itself', function () {

        var module = app.formSubmitter(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('formSubmitter module should invoke unobtrusive validation', function () {

        expect(1);

        var form = $('<div><form/></div>');

        $.validator = {
            unobtrusive: {
                parse: function () {
                    ok(true);
                }
            }
        };

        var module = app.formSubmitter(mocks.create());
        module.attach(form, function () { });
    });

    test('formSubmitter module should prevent default event when form is submitted', function () {

        expect(2);

        var view = $('<div><form/></div>'),
            form = view.find('form');
        form.data('unobtrusiveValidation', getValidatorThatReturns(false));

        $.validator = stubUnobtrusive;

        var module = app.formSubmitter(mocks.create({}));

        form.submit(function (evt) {
            ok(!evt.isDefaultPrevented());
        });

        module.attach(view, {});

        form.submit(function (evt) {
            ok(evt.isDefaultPrevented());
        });

        form.triggerHandler('submit');
    });

    test('formSubmitter module should serialize form when form is valid', function () {

        expect(1);

        var view = $('<div><form><input type="text" name="field" value="value"/></form></div>'),
            form = view.find('form');
        form.data('unobtrusiveValidation', getValidatorThatReturns(true));

        $.validator = stubUnobtrusive;
        $.ajax = function (options) {
            equal(options.data, 'field=value');
        };

        var module = app.formSubmitter(mocks.create());
        module.attach(view, function () { });
        form.triggerHandler('submit');
    });

    test('formSubmitter module should invoke a callback after the form is successfully submitted', function () {

        expect(1);

        var view = $('<div><form/></div>'),
            form = view.find('form');
        form.find('form').data('unobtrusiveValidation', getValidatorThatReturns(true));

        $.ajax = function (options) {
            options.success({});
        };

        var module = app.formSubmitter(mocks.create());

        module.attach(view, function () {
            ok(true);
        });

        form.triggerHandler('submit');
    });

    test('formSubmitter module should submit via ajax form when form is valid', function () {

        expect(1);

        var view = $('<div><form action="url/to/post/form/to"/></div>'),
            form = view.find('form');
        form.find('form').data('unobtrusiveValidation', getValidatorThatReturns(true));

        $.ajax = function (options) {
            equal(options.url, 'url/to/post/form/to');
        };

        var module = app.formSubmitter(mocks.create());

        module.attach(view, function () { });

        form.triggerHandler('submit');
    });

    // represents the api that the formSubmitter module
    // expects to be present whenever `attach` is invoked
    var stubUnobtrusive = {
        unobtrusive: {
            parse: function (form) {
            }
        }
    };

    // returns a "validator" object, whose validate method
    // returns the value specified
    function getValidatorThatReturns(isValid) {
        return {
            validate: function (form) {
                return isValid;
            }
        };
    }

} (window.specs = window.specs || {}, window.mstats));