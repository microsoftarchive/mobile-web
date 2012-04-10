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

(function (mstats) {
	var ctr = function (require) {

	    var $ = require('$'),
	        window = require('window'),
            confirmPrompt = require('confirm'),
            formSubmitter = require('formSubmitter');

		var urlPattern = '/Vehicle/:vehicleId/Details';

		function postrender(model, el, context) {

			formSubmitter.attach(el, function (response) {
				window.location.hash = urlPattern.replace(':vehicleId', response.Model.VehicleId);
			});

			formSubmitter.attach(el, function (response) {
				window.location.hash = '/';
			}, '#DeleteVehicleForm');

			el.find('#DeleteVehicleButton').click(function (event) {
				var answer = confirmPrompt("Are you sure you want to delete this vehicle and all of it's related data?");
				if (!answer) {
					event.preventDefault();
				}
			});

			var $vehicleEditForm = el.find('#vehicleEditForm'),
                $yearSelect = el.find('#Year'),
                $makeSelect = el.find('#MakeName'),
                $modelSelect = el.find('#ModelName'),
                makesUrl = $vehicleEditForm.data('makes-url'),
                modelsUrl = $vehicleEditForm.data('models-url');

			$yearSelect.change(function () {
				$.post(makesUrl,
                { year: $yearSelect.val() },
                function (data) {
                	$makeSelect.children().not(':first').remove();
                	$modelSelect.children().not(':first').remove();
                	updateList(data, $makeSelect);
                });
			});

			$makeSelect.change(function () {
				$.post(modelsUrl,
                { year: $yearSelect.val(), make: $makeSelect.val() },
                function (data) {
                	$modelSelect.children().not(':first').remove();
                	updateList(data, $modelSelect);
                });
			});
		};

		function updateList(data, $selectList) {
			$.each(data,
              function (key, value) {
              	$selectList.append(
                    $('<option></option>')
                    .attr('value', value)
                    .text(value)
                    );
              });
		};

		return {
			postrender: postrender
		};
	};
	mstats.vehicleEdit = ctr;
	mstats.vehicleAdd = ctr;
} (this.mstats = this.mstats || {}));
