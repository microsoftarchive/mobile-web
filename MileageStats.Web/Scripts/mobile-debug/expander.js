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

(function ($) {
	$.fn.expander = function (options) {

		var defaults = {
			header: 'dt',
			children: 'dd'
		}; 

		var options = $.extend(defaults, options);

		this.each(function () {
			var header = $(this).children(options.header);

			header.next(options.children).toggle();

			header.click(function (e) {
				e.preventDefault();
				$(this).next(options.children).toggle();
			});
		});
	};
})($);

