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

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {}
            }
        });
        var module = app.fillupAdd(m);

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });


    test('fillup module should invoke formSubmitter with the correct arguments', function () {

        expect(2);

        var m = mocks.create({
            formSubmitter: {
                attach: function (el, callback) {
                    equal(typeof el, 'object');
                    equal(typeof callback, 'function');
                }
            },
            navigator: {
                geolocation: {}
            }
        });

        var mockView = m.$('view');
        mockView.find = function (selector) {
            switch (selector) {
                case 'input:checkbox[name=use-api-location]':
                    return { click: function () { } };
                default:
                    return m.$(selector);
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockView, {});

    });

    test('fillup module attaches to click event of use-api-location checkbox', function () {

        expect(1);

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {}
            }
        });

        var mockView = m.$('view');
        mockView.find = function (selector) {
            switch (selector) {
                case 'input:checkbox[name=use-api-location]':
                    return { click: function () {
                        ok(true, "Expect to subscribe to client event of checkbox.");
                    }
                    };
                default:
                    return m.$(selector);
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockView, {});

    });

    test('fillup module disables Vendor controls by default', function () {

        expect(2);

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {}
            }
        });

        var mockView = m.$('view');
        mockView.find = function (selector) {
            switch (selector) {
                case 'input:checkbox[name=use-api-location]':
                    return {
                        click: function () { }
                    };
                case 'select[name=Location]':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "disabled");
                            equal(attributeValue, "disabled");
                        }
                    };
                default:
                    return m.$(selector);
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockView, {});

    });

    test('fillup module enables Location and disables Vendor controls if use-api-location is checked', function () {

        expect(6);

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {
                    getCurrentPosition: function (callback) {
                        callback({
                            coords: {
                                latitude: "123",
                                longitude: "456"
                            }
                        });
                    }
                }
            }
        });

        $.getJSON = function (url, callback) {
            callback(["value1", "value2"]);
        };

        var mockView = m.$('view');
        mockView.find = function (selector) {
            switch (selector) {
                case 'input:checkbox[name=use-api-location]':
                    return {
                        click: function (clickEventSubscription) {
                            clickEventSubscription();
                        },
                        is: function (valueToCheck) {
                            equal(':checked', valueToCheck);
                            return true;
                        }
                    };
                case 'input[name=Vendor]':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "disabled");
                            equal(attributeValue, "disabled");
                        }
                    };
                case 'select[name=Location]':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "disabled");
                            equal(attributeValue, "disabled");
                        },
                        removeAttr: function (attributeName) {
                            equal(attributeName, "disabled");
                        },
                        append: function () {
                        }
                    };
                default:
                    return m.$(selector);
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockView, {});

    });

    test('fillup module disables Location and enables Vendor controls if use-api-location is unchecked', function () {

        expect(6);

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {}
            }
        });

        var mockView = m.$('view');
        mockView.find = function (selector) {
            switch (selector) {
                case 'input:checkbox[name=use-api-location]':
                    return {
                        click: function (clickEventSubscription) {
                            clickEventSubscription();
                        },
                        is: function (valueToCheck) {
                            equal(':checked', valueToCheck);
                            return false;
                        }
                    };
                case 'input[name=Vendor]':
                    return {
                        removeAttr: function (attributeName) {
                            equal(attributeName, "disabled");
                        }
                    };

                case 'select[name=Location]':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "disabled");
                            equal(attributeValue, "disabled");
                        }
                    };
                default:
                    return m.$(selector);
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockView, {});

    });

    test('fillup module hides Location controls by if GeoLocation API not available', function () {

        expect(6);

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {}
        });

        var mockView = m.$('view');
        mockView.find = function (selector) {
            switch (selector) {
                case 'input:checkbox[name=use-api-location]':
                    return {
                        click: function () {
                        }
                    };
                case '#GeoLocationSelect':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "style");
                            equal(attributeValue, "display:none");
                        }
                    };
                case '#GeoLocationCheckbox':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "style");
                            equal(attributeValue, "display:none");
                        }
                    };
                case 'label[for=new-location]':
                    return {
                        attr: function (attributeName, attributeValue) {
                            equal(attributeName, "style");
                            equal(attributeValue, "display:none");
                        }
                    };
                default:
                    return m.$(selector);
            }
        };

        var module = app.fillupAdd(m);
        module.postrender({}, mockView, {});

    });

} (window.specs = window.specs || {}, window.mstats));