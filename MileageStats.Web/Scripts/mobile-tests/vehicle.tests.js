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

    module('vehicle add/edit specs');

    var stubFormSubmitter = {
        attach: function () {
        }
    };

    var default_mocks = {
        confirm: function () { return true; },
        formSubmitter: stubFormSubmitter
    };

    test('vehicle module constructs itself', function () {
        var module = app.vehicleAdd(mocks.create(default_mocks));

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('vehicle module gets Makes when Year changes', function () {

        expect(5);

        var m = mocks.create(default_mocks);

        $.post = function (url, data, callback) {
            equal(url.indexOf('makesUrl'), 0, "Expect to see Makes URL");
            equal(data.year, 1990);
            callback(["MakeName1", "MakeName2"]);
        };

        var view = $('<div><div id="vehicleEditForm" data-makes-url="makesUrl"><select id="Year"><option value="1990"/></select><select id="MakeName"></select></div></div>');

        var module = app.vehicleAdd(m);
        module.postrender({}, view, {});

        view.find('#Year').change();

        equal(view.find('#MakeName').children().size(), 2);
        equal(view.find('#MakeName').children('option[value=MakeName1]').size(), 1);
        equal(view.find('#MakeName').children('option[value=MakeName2]').size(), 1);
    });

    test('vehicle module gets Models when Makes changes', function () {

        expect(5);

        var m = mocks.create(default_mocks);

        $.post = function (url, data, callback) {
            equal(url.indexOf('modelsUrl'), 0, "Expect to see Models URL");
            equal(data.year, 1990);
            callback(["Model1", "Model2"]);
        };

        var view = $('<div><div id="vehicleEditForm" data-models-url="modelsUrl"><select id="Year"><option value="1990"/></select><select id="MakeName"><option value="testMakeName"/></select><select id="ModelName"></select></div></div>');

        var module = app.vehicleAdd(m);
        module.postrender({}, view, {});

        view.find('#MakeName').change();

        equal(view.find('#ModelName').children().size(), 2);
        equal(view.find('#ModelName').children('option[value=Model1]').size(), 1);
        equal(view.find('#ModelName').children('option[value=Model2]').size(), 1);
    });

    test('vehicle module should attach to click event for DeleteVehicleButton and cancel event if not confirmed', function () {

        expect(2);

        var clickEvent = { preventDefault: function () { ok(true, "Trying to cancel click event."); } };
        var m = mocks.create({
            confirm: function () { return false; },
            formSubmitter: stubFormSubmitter
        });
        var mockView = m.$();

        mockView.find = function (selector) {
            switch (selector) {
                case 'form':
                    return {
                        first: function () { return mockView; }
                    };
                case '#vehicleEditForm':
                    return mockView;
                case '#DeleteVehicleButton':
                    return { click: function (subscription) {
                        ok(true, "Trying to attach to DeleteButton click");
                        subscription(clickEvent);
                    }
                    };
                case '#Year':
                    return { change: function () { }
                    };
                case '#MakeName':
                    return { change: function () { } };
                default:
                    return m.$(selector);
            }
        };

        var module = app.vehicleAdd(m);
        module.postrender({}, mockView, {});

    });

    test('vehicle module should invoke formSubmitter with the correct arguments', function () {

        expect(4);

        var m = mocks.create({
            confirm: function () { return true; },
            formSubmitter: {
                attach: function (el, callback, selector) {
                    equal(typeof el, 'object');
                    equal(typeof callback, 'function');
                }
            }
        });

        var view = $('<div></div>');

        var module = app.vehicleAdd(m);
        module.postrender({}, view, {});

    });

    test('vehicle module should redirect to / after deleting a vehicle', function () {

        expect(1);

        var m = mocks.create({
            window: { location: { hash:'something-else' } },
            confirm: function () { return true; },
            formSubmitter: {
                attach: function (el, callback, selector) {
                    if (selector !== '#DeleteVehicleForm') return;
                    callback();
                    equal(m.window.location.hash, '/');
                }
            }
        });

        var view = $('<div></div>');

        var module = app.vehicleAdd(m);
        module.postrender({}, view, {});

    });

} (window.specs = window.specs || {}, window.mstats));