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


(function (mstats, $) {

    mstats.VehicleDropDownMonitor = function VehicleDropDownMonitor(publish, sendRequest) {

        if (!(this instanceof VehicleDropDownMonitor)) {
            return new VehicleDropDownMonitor(publish, sendRequest);
        }

        var that = {};

        that.initialize = function () {
            var $vehicleEditForm = $('#vehicleEditForm'),
                $yearSelect = $('#Year', $vehicleEditForm),
                $makeSelect = $('#MakeName', $vehicleEditForm),
                $modelSelect = $('#ModelName', $vehicleEditForm),
                makesUrl = $vehicleEditForm.data('makes-url'),
                modelsUrl = $vehicleEditForm.data('models-url');

            $vehicleEditForm.find('input[name="UpdateMakes"]').remove()
            .end()
            .find('input[name="UpdateModels"]').remove();

            $yearSelect.change(function () {
                $makeSelect.children().not(':first').remove();
                $modelSelect.children().not(':first').remove();

                sendRequest({
                    url: makesUrl,
                    data: { year: $yearSelect.val() },
                    cache: false,
                    success: function (data) {
                        that._updateList(data, $makeSelect);
                    },
                    error: function () {
                        that._publishError('Could not load vehicle data lists.');
                    }
                });
            });

            $makeSelect.change(function () {
                $modelSelect.children().not(':first').remove();

                sendRequest({
                    url: modelsUrl,
                    cache: false,
                    data: { year: $yearSelect.val(), make: $makeSelect.val() },
                    success: function (data) {
                        that._updateList(data, $modelSelect);
                    },
                    error: function () {
                        that._publishError('Could not load vehicle data lists.');
                    }
                });
            });
        };

        that._updateList = function (data, $selectList) {
            $.each(data,
              function (key, value) {
                  $selectList.append(
                    $('<option></option>')
                    .attr('value', value)
                    .text(value)
                    );
              });
        };

        that._publishError = function (message) {
            publish(mstats.events.status,
               {
                   type: 'loadError',
                   message: message
               });
        };

        return that;
    };

} (this.mstats = this.mstats || {}, jQuery));