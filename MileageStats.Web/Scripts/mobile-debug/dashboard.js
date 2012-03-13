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

(function (mstats) {
    mstats.dashboard = function (require) {

        function highlightImminentReminders(res, view) {
            
            var vehicles = res.Model.VehicleListViewModel.Vehicles,
                reminders = res.Model.ImminentReminders,
                vehicle,
                reminder,
                i, j;

            for (i = 0; i < vehicles.length; i++) {
                vehicle = vehicles[i];

                for (j = 0; j < reminders.length; j++) {
                    reminder = reminders[j];
                    if (reminder.VehicleId == vehicle.VehicleId) {
                        var el = view.find('#reminderMenu_' + reminder.VehicleId);
                        el.addClass('flag');
                        break;
                    }
                }
            }
        }

        return {
            postrender: highlightImminentReminders
        };
    };
} (this.mstats = this.mstats || {}));