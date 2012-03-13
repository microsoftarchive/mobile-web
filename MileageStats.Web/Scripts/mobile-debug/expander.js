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
    mstats.expander = function (require) {
        
        var $ = require('$');
        
        var parent = 'dt',
            child = 'dd';
        
        function attach(view) {
            var header = view.find(parent);
            
            header.next(child).toggle();
            
            header.click(function (e) {
				e.preventDefault();
				$(this).next(child).toggle();
			});
        }

        return {
            attach: attach
        };
    };
} (this.mstats = this.mstats || {}));