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

    module('formSubmitter specs');

    test('formSubmitter module constructs itself', function () {

        var module = app.formSubmitter(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('formSubmitter module should invoke unobtrusive validation', function () {

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

        var module = app.formSubmitter(m);
        module.attach(mockForm, function () {
        });
    });

    test('formSubmitter module should attach submit handler on form', function () {

        expect(1);

        var m = mocks.create();

        m.$.validator = stubUnobtrusive;

        var module = app.formSubmitter(m);
        module.attach(m.$('view'), {});

        ok(m.tracked.contains('form.submit()'));

    });

    test('formSubmitter module should prevent default event when form is submitted', function () {

        expect(1);

        var m = mocks.create({},
            {
                '$.data': getValidatorThatReturns(false)
            });

        m.$.validator = stubUnobtrusive;

        var mockView = whenFormSubmitted(function (submission) {
            submission({
                preventDefault: function () { ok(true); }
            });
        });

        var module = app.formSubmitter(m);
        module.attach(mockView, {});
    });

    test('formSubmitter module should serialize form when form is valid', function () {

        expect(1);

        var m = mocks.create({},
            {
                '$.data': getValidatorThatReturns(true),
                '$.submit': function (handler) {
                    handler({ preventDefault: function () { }
                    });
                }
            });

        m.$.validator = stubUnobtrusive;

        var module = app.formSubmitter(m);
        module.attach(m.$('view'), function () {

        });
        ok(m.tracked.contains('form.serialize()'));
    });


    test('formSubmitter module should invoke a callback after the form is successfully submitted', function () {

        expect(1);

        var m = mocks.create({},
            {
                '$.data': getValidatorThatReturns(true),
                '$.submit': function (handler) {
                    handler({ preventDefault: function () { }
                    });
                }
            });

        m.$.validator = stubUnobtrusive;

        var module = app.formSubmitter(m);
        module.attach(m.$('view'), function () {
            ok(true);
        });
    });

    test('formSubmitter module should submit via ajax form when form is valid', function () {

        expect(1);

        var m = mocks.create({},
            {
                '$.attr': function () {
                    return 'url/to/post/form/to';
                },
                '$.data': getValidatorThatReturns(true),
                '$.submit': function (handler) {
                    handler({ preventDefault: function () { }
                    });
                }
            });

        m.$.validator = stubUnobtrusive;

        var module = app.formSubmitter(m);
        module.attach(m.$('view'), function () {
        });

        ok(m.tracked.contains('ajax: url/to/post/form/to'));
    });

    // represents the api that the formSubmitter module
    // expects to be present whenever `attach` is invoked
    var stubUnobtrusive = {
        unobtrusive: {
            parse: function (form) {
            }
        }
    };

    // provides a mock element for attach the submit handler,
    // the handler passed in receives a function as an argument
    function whenFormSubmitted(handler) {
        return {
            find: function () {
                return {
                    first: function () {
                        return {
                            attr: function () {
                            },
                            submit: handler
                        };
                    }
                };
            }
        };
    }

    // returns a "validator" object, whose validate method
    // returns the value specified
    function getValidatorThatReturns(isValid) {
        return function () {
            return {
                validate: function (form) {
                    return isValid;
                }
            };
        };
    }

} (window.specs = window.specs || {}, window.mstats));