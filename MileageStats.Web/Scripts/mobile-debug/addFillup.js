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

(window.mstats = window.mstats || { }).addFillup = function(require) {

    function months() {
        var names = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var today = new Date(),
            i = 0,
            len = names.length,
            out = [];

        for (; i < len; i++) {
            out.push(
                {
                    value: i,
                    text: names[i],
                    selected: today.getMonth() === i
                }
            );
        }

        return out;
    }

    function dates() {
        var today = new Date(),
            i = 0,
            len = 31,
            out = [];

        for (; i < len; i++) {
            out.push(
                {
                    value: i,
                    text: i,
                    selected: today.getDate() === i
                }
            );
        }

        return out;
    }

    function years() {
        var year = new Date().getFullYear(),
            i = -2,
            out = [];

        for (; i < 1; i++) {
            out.push(
                {
                    value: year + i,
                    text: year + i,
                    selected: i === 0
                }
            );
        }

        return out;
    }

    function prerender(data) {
        data.DateMonth = months();
        data.DateDay = dates();
        data.DateYear = years();
        return data;
    }

    return {
        fetch: false,
        prerender: prerender
    };
};