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

(function (mstats, $) {
    $.widget('mstats.imminentRemindersPane', {
        options: {
            // Default to $.ajax when sendRequest is undefined.
            // The extra function indirection allows $.ajax substitution because 
            // the widget framework is using the real $.ajax during options initialization.
            sendRequest: function (ajaxOptions) { $.ajax(ajaxOptions); },

            // invalidateData allows the imminentReminders to invalidate data
            // stored in the data cache.  
            invalidateData: function () { mstats.log('The invalidateData option on imminentReminders has not been set'); },

            // option providing the event publisher used for communicating with the status widget.
            publish: function() { mstats.log('The publish option on imminentReminders has not been set'); }
        },

        _create: function () {
            this._getImminentReminderData();
        },

        _applyTemplate: function (data) {
            if (!$(this.options.templateId).length) {
                mstats.log('Cannot apply templates as there is no template defined.');
                return;
            }

            // Wrapped to make it easier to template with header data.
            var wrappedData = { ReminderList: data.Model.model };
            this.element.find('#summary-reminders-content')
                .html($(this.options.templateId).tmpl(wrappedData, {
                    createRemindersLink: function (vehicleId) {
                        var state = $.bbq.getState() || {},
                            newUrlBase = mstats.getBaseUrl();
                        state.layout = 'reminders';
                        state.vid = vehicleId;

                        return $.param.fragment(newUrlBase, state);
                    }
                }));
        },

        _getImminentReminderData: function () {
            var that = this;
            this._hideReminders();

            this.options.sendRequest({
                url: this.options.dataUrl,
                success: function (data) {

                    that._applyTemplate(data);
                    that._showReminders();

                    // ensure that setOption is called with the provided options
                    that.option(that.options);
                },
                error: function () {
                    that._hideReminders();
                    that._showErrorMessage();
                }
            });
        },

        _showReminders: function () {
            this.element.find('#summary-reminders-content').show();
        },

        _hideReminders: function () {
            this.element.find('#summary-reminders-content').hide();
        },

        refreshData: function () {
            this._getImminentReminderData();
        },

        requeryData: function () {
            this.options.invalidateData(this.options.dataUrl);
            this.refreshData();
        },

        /********************************************************
        * Status Methods               
        ********************************************************/
        _publishStatus: function (status) {
            this.options.publish(mstats.events.status, status);
        },

        // hide the vehicles list and show the status widget in the error state
        _showErrorMessage: function () {
            this._publishStatus({
                type: 'loadError',
                message: 'An error occurred while loading the summary reminders data.  Please try again.',
                duration: 10000
            });
        }
    });

}(this.mstats, jQuery));
