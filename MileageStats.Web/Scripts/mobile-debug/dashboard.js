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

(function (mstats, $) {
	mstats.dashboard = function () {
		return function (arg, data, view) {
			highlightImminentReminders(data, $(view));
		}

		function highlightImminentReminders(data, view) {
			for (var i = 0; i < data.VehicleListViewModel.Vehicles.length; i++) {
				var vehicle = data.VehicleListViewModel.Vehicles[i];

				for (var j = 0; j < data.ImminentReminders.length; j++) {
					var reminder = data.ImminentReminders[j];
					if (reminder.VehicleId == vehicle.VehicleId) {
						$('#reminderMenu_' + reminder.VehicleId).addClass('highlight');
						break;
					}
				}
			}
		}
	};
} (this.mstats = this.mstats || {}, jQuery));