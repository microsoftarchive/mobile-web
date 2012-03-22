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

	// this module persist message notifications across different
	// ajax calls, so they can be correctly rendered in the views

	mstats.notifications = function (require) {

		var alert = null,
			confirm = null;

		function log(model) {
			alert = model.FlashAlert || null;
			confirm = model.FlashConfirm || null;
		}

		function renderTo(el) {
			var container;

			if (confirm) {
				container = $('<div><p>' + confirm + '</p></div>')
							.addClass('flash confirm')

				el.find('nav').first().after(container)
				confirm = null;
			}

			if (alert) {
			    container = $('<div><p>' + alert + '</p></div>')
							.addClass('flash alert')

				el.find('nav').first().after(container);
				alert = null;
			}
		}

		return {
			log: log,
			renderTo: renderTo
		};

	};
} (this.mstats = this.mstats || {}));