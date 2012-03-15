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

(window.mstats = window.mstats || {}).charts = function (require) {

    function postrender(model, el) {

        el.find('#ChartRefreshButton').click(function () {
            var chartNameCriteria = "&ChartName=" + el.find('select[name=ChartName] option:selected').val();
            var vehicleIdCriteria = "";
            var startDateCriteria = "&StartDate=" + el.find('select[name=StartDate] option:selected').val();
            var endDateCriteria = "&EndDate=" + el.find('select[name=EndDate] option:selected').val();
            el.find('input:checkbox[name=VehicleIds]:checked').each(function (index) {
                vehicleIdCriteria = vehicleIdCriteria + "&VehicleIds=" + this.value;
            });
            var getChartImageUrl = el.find('#GetChartImageUrl').val();
            var fullChartUrl = getChartImageUrl + chartNameCriteria + startDateCriteria + endDateCriteria + vehicleIdCriteria;

            el.find("#chartimage").attr("src", fullChartUrl);

        });

        el.find('form').submit(function (event) {
            event.preventDefault();
        });
    }

    return {
        postrender: postrender
    };

};