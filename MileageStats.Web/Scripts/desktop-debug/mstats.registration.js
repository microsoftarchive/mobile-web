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

	$.widget('mstats.registration', {

		options: {
			// Default to $.ajax when sendRequest is undefined.
			// The extra function indirection allows $.ajax substitution because 
			// the widget framework is using the real $.ajax during options initialization.
			sendRequest: function (ajaxOptions) { $.ajax(ajaxOptions); },

			// option providing the event publisher used for communicating with 
			// the status widget.
			publish: function () { mstats.log('The publish option on registration has not been set'); },

			displayNameChanged: null
		},

		_create: function () {
			this._bindNavigation();
		},

		_bindNavigation: function () {
			var that = this;

			this.element.delegate('form', 'submit.registration', function (event) {
				that.saveProfile();
				event.preventDefault();
			});
		},

		saveProfile: function () {
			var that = this,
                elem = this.element,
                formData = {
                	UserId: elem.find('#UserId').val(),
                	AuthorizationId: elem.find('#AuthorizationId').val(),
                	DisplayName: elem.find('#DisplayName').val(),
                	Country: elem.find('#Country').val(),
                	PostalCode: elem.find('#PostalCode').val(),
                	__RequestVerificationToken: elem.find('input[name=__RequestVerificationToken]').val()
                };

			this._showSavingMessage();

			this.options.sendRequest({
				url: this.options.dataUrl,
				data: formData,
				cache: false,
				success: function () {
					that._startHidingWidget();
					that._showSavedMessage();
					// we update the username after successfully the updating the profile
					that._trigger('displayNameChanged', null, { displayName: formData.DisplayName });
				},
				error: function () {
					that._showSavingErrorMessage();
				}
			});
		},

		/********************************************************
		* Status Methods               
		********************************************************/
		_publishStatus: function (status) {
			this.options.publish(mstats.events.status, status);
		},

		// show the status widget in the saving state
		_showSavingMessage: function () {
			this._publishStatus({
				type: 'saving',
				message: 'Saving registration information...',
				duration: 5000
			});
		},

		// show the status widget in the saved state
		_showSavedMessage: function () {
			this._publishStatus({
				type: 'saved',
				message: 'Registration saved',
				duration: 5000
			});
		},

		// show the status widget in the error state after a failed save
		_showSavingErrorMessage: function () {
			this._publishStatus({
				type: 'saveError',
				message: 'An error occurred while saving your registration. ' +
                         'Please try again.',
				duration: 5000
			});
		},

		// hide the form in the widget
		_startHidingWidget: function () {
			this.element
                .find('#registration-content')
                    .slideUp('slow')
                    .end()
                .delay(2000)
                .slideUp('slow');
		}
	});

} (this.mstats, jQuery));