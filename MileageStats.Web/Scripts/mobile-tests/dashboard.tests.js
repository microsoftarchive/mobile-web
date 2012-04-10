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

    module('dashboard specs');

    var mockExpander = {
        expander: {
            attach: function () {
            }
        }
    };

    test('dashboard module constructs itself', function () {
        var module = app.dashboard(mocks.create(mockExpander));

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('dashboard module should flag an element associated with a vehicle that has an overdue reminder', function () {

        expect(1);

        var view = $('<div><div id="reminderMenu_99"/></div>');

        var response = {
            Model: {
                VehicleListViewModel: {
                    Vehicles: [{ VehicleId: 99}]
                },
                ImminentReminders: [{ VehicleId: 99}]
            }
        };

        var module = app.dashboard(mocks.create(mockExpander));
        module.postrender(response, view);

        ok(view.find('#reminderMenu_99').hasClass('flag'));
    });

    test('dashboard module should flag every vehicle that has an associated overdue reminder', function () {

        var view = $('<div><div id="reminderMenu_1" /><div id="reminderMenu_2" /></div>');

        var module = app.dashboard(mocks.create(mockExpander));

        module.postrender({ Model: {
            VehicleListViewModel: {
                Vehicles: [{ VehicleId: 1 }, { VehicleId: 2}]
            },
            ImminentReminders: [{ VehicleId: 1 }, { VehicleId: 1 }, { VehicleId: 2}]
        }
        }, view);

        var class1 = view.find('#reminderMenu_1').attr('class');
        var class2 = view.find('#reminderMenu_2').attr('class');
        equal(class1, 'flag');
        equal(class2, 'flag');
    });

    test('dashboard module should invoke expander module', function () {

        expect(1);

        // The view and response are inconsequential for this test
        var view = $('<div/>');

        var response = {
            Model: {
                VehicleListViewModel: { Vehicles: [] },
                ImminentReminders: []
            }
        };

        var module = app.dashboard(mocks.create({
            expander: {
                attach: function () {
                    ok(true);
                }
            }
        }));

        module.postrender(response, view);
    });

} (window.specs = window.specs || {}, window.mstats));