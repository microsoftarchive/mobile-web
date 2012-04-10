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

        var view = $('<div></div>');

        var module = app.fillupAdd(m);
        module.postrender({}, view, {});

    });

    test('fillup module disables Vendor controls by default', function () {

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {}
            }
        });

        var view = $('<div><input name="use-api-location" type="checkbox"></input><select name="Location"></select></div>');

        var module = app.fillupAdd(m);
        module.postrender({}, view, {});

        var locationSelect = view.find('select[name=Location]');
        equal(locationSelect.attr('disabled'), 'disabled');

    });

    test('fillup module enables Location and gets geolocation data calls service to get nearby fillups and populates Location', function () {

        expect(8    );

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {
                    getCurrentPosition: function (callback) {
                        ok(true, "Expect to call getCurrentPosition");
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
            ok(url.indexOf('?latitude=123') > 0, "Expect to see latitude value passed in QS.");
            ok(url.indexOf('&longitude=456') > 0, "Expect to see longitude value passed in QS.");
            callback(["value1", "value2"]);
        };

        var view = $('<div><input name="use-api-location" type="checkbox" checked></input><select name="Location"></select><input name="Vendor" type="text"></input></div>');

        var module = app.fillupAdd(m);
        module.postrender({}, view, {});

        view.find('input:checkbox[name=use-api-location]').trigger('click');

        equal(view.find('input[name=Vendor]').attr('disabled'), 'disabled');
        equal(view.find('select[name=Location]').attr('disabled'), undefined);

        equal(view.find('select[name=Location]').children().size(), 2);
        equal(view.find('select[name=Location]').children('option[value=value1]').size(), 1);
        equal(view.find('select[name=Location]').children('option[value=value2]').size(), 1);
    });

    test('fillup module disables Location and enables Vendor controls if use-api-location is unchecked', function () {

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {
                geolocation: {}
            }
        });

        var view = $('<div><input name="use-api-location" type="checkbox"></input><select name="Location"></select><input name="Vendor" type="text"></input></div>');

        var module = app.fillupAdd(m);
        module.postrender({}, view, {});

        view.find('input:checkbox[name=use-api-location]').trigger('click');

        equal(view.find('input[name=Vendor]').attr('disabled'), undefined);
        equal(view.find('select[name=Location]').attr('disabled'), 'disabled');

    });

    test('fillup module hides Location controls by if GeoLocation API not available', function () {

        var m = mocks.create({
            formSubmitter: {
                attach: function () { }
            },
            navigator: {}
        });

        var view = $('<div><li id="GeoLocationCheckbox"><input name="use-api-location" type="checkbox" checked></input></li><li id="GeoLocationSelect"></li><label for="new-location"></label></div>');

        var module = app.fillupAdd(m);
        module.postrender({}, view, {});

        equal(view.find('#GeoLocationSelect').attr('style'), "display: none;");
        equal(view.find('#GeoLocationCheckbox').attr('style'), "display: none;");
        equal(view.find('label[for=new-location]').attr('style'), "display: none;");

    });

} (window.specs = window.specs || {}, window.mstats));