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

	// this module searches the provided jQuery object 
	// for a form element, it takes the first one and 
	// then sets up the unobstrusive validation that is
	// part of ASP.NET MVC 3. after that, it wires an
	// event handler for submission of the form that
	// results in an AJAX request to the url specified
	// in the form's action.
	// this has an implicit depedency on the jQuery 
	// validation plugin

	mstats.formSubmitter = function (require) {

		var $ = require('$'),
			notifications = require('notifications');

		function validate(form) {
			// here we look for the validation object that has been
			// attached to the form. this assumes the present of 
			// jQuery validation and MVC's unobtrusive validation scripts.
			var validationInfo = $(form).data('unobtrusiveValidation');
			return !validationInfo || !validationInfo.validate || validationInfo.validate();
		}

		function displayErrors(errors) {
			var item;
			var errorList;
			var msg;

			// we want to visually associate any errors returned from
			// server. (note that this differs from the client-side
			// validation)
			// this makes assumption about the structure of the markup

			for (item in errors) {
				// `item` is the name of the field with the input error
				// `errorList` is the list of validation errors associated 
				// with that field
				errorList = errors[item];
				msg = '';

				for (var i = 0; i < errorList.length; i++) {
					msg = msg + errorList[i];
				}

				// `data-valmsg-for` is an attribute that is emit by
				// the unobtrusive validation code
				$('[data-valmsg-for="' + item + '"]').html(msg);
			}
		}

		function onSuccess(callback) {
			// we create this function in a closure
			// in order to trap the callback
			return function (res, status, xhr) {
				if (res.Errors) {
					displayErrors(res.Errors);
				} else {
					notifications.send(res);
					callback(res);
				}
			};
		}

		function attachFormSubmission(el, callback) {

			var form = el.find('form').first(),
                action = form.attr('action');

			$.validator.unobtrusive.parse(form);

			form.submit(function (evt) {

				evt.preventDefault();

				if (!validate(this)) {
					return;
				}

				var input = form.serialize();

				$.ajax({
					dataType: 'json',
					data: input,
					type: 'POST',
					url: action,
					success: onSuccess(callback)
				});
				return false;
			});
		};

		return {
			attach: attachFormSubmission
		};

	};
} (this.mstats = this.mstats || {}));
