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

    var $ = require('$');

    function postrender(model, el) {

        el.find('#ChartRefreshButton').click(function () {
            el.find('input:checkbox[name=VehicleIds]:checked').each(function(index) {
                alert(this.value);
            });
        });

        el.find('form').submit(function (event) {
            event.preventDefault();
        });
    }

    return {
        postrender: postrender
    };

};