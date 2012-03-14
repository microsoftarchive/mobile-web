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

    module('charts specs');

    test('charts module constructs itself', function () {
        var module = app.charts(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('charts searches for "headers" in the given view', function () {

        var module = app.charts(mocks.create());

        var mockView = {
            find: function (selector) {
                switch (selector) {
                    case '#ChartRefreshButton':
                        return {
                            click: function (clickEventSubscription) {
                                
                            },
                            val: function() { return 'testchartname' }
                        }; break;
                    default: result = 'unknown';
                }

                if (selector === '#reminderMenu_1') find_invoked++;
                return {
                    addClass: function () {
                    }
                };
            }
        };

        module.postrender({ Model: {
            VehicleListViewModel: {
                Vehicles: [{ VehicleId: 1}]
            },
            ImminentReminders: [{ VehicleId: 1 }, { VehicleId: 1 }, { VehicleId: 1}]
        }
        }, mockView);


    });
    ;


} (window.specs = window.specs || {}, window.mstats));