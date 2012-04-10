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

(window.mstats = window.mstats || {}).fillupAdd = function (require) {

    var formSubmitter = require('formSubmitter');
    var navigator = require('navigator');

    var urlPattern = '/Vehicle/:vehicleId/Fillup/List';

    function postrender(model, el, context) {

        formSubmitter.attach(el, function (response) {
            window.location.hash = urlPattern.replace(':vehicleId', context.params.vehicleId);
        });

        var $fillupForm = el.find('#fillupForm');
        var $useGeoCoordinateToGetFillupStationsCheckbox = el.find('input:checkbox[name=use-api-location]');
        var $fillupStationSelect = el.find('select[name=Location]');
        var $fillupStationTextbox = el.find('input[name=Vendor]');
        var fillupStationsUrl = $fillupForm.data('fillup-stations-url');

        if (!navigator.geolocation) {
            el.find('#GeoLocationSelect').attr("style", "display:none");
            el.find('#GeoLocationCheckbox').attr("style", "display:none");
            el.find('label[for=new-location]').attr("style", "display:none");
        } else {
            $fillupStationSelect.attr('disabled', 'disabled');
        }

        $useGeoCoordinateToGetFillupStationsCheckbox.click(function () {
            if ($useGeoCoordinateToGetFillupStationsCheckbox.is(':checked') && navigator.geolocation) {
                $fillupStationTextbox.attr('disabled', 'disabled');
                navigator.geolocation.getCurrentPosition(function (position) {
                    //Get country name from GeoLocationController. 
                    var getGasStationsUrl = fillupStationsUrl + '?latitude=' + position.coords.latitude + "&longitude=" + position.coords.longitude;
                    $.getJSON(getGasStationsUrl, function (data) {
                        $fillupStationSelect.removeAttr('disabled');
                        updateList(data, $fillupStationSelect);
                    });
                });
            }
            else {
                $fillupStationTextbox.removeAttr('disabled');
                $fillupStationSelect.attr('disabled', 'disabled');
            }

        });
    }

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
        fetch: true,
        postrender: postrender
    };

};