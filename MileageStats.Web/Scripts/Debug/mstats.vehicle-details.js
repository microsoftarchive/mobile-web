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
    $.widget('mstats.vehicleDetails', {
        // default options
        options: {
            // Default to $.ajax when sendRequest is undefined.
            // The extra function indirection allows $.ajax substitution because 
            // the widget framework is using the real $.ajax during options initialization.
            sendRequest: function (ajaxOptions) { $.ajax(ajaxOptions); },

            // option providing the event publisher used for communicating with the status widget.
            publish: function() { mstats.log('The publish option on vehicleDetails has not been set'); },

            // invalidateData allows the vehicleList to invalidate data
            // stored in the data cache.  
            invalidateData: function () { mstats.log('The invalidateData option on vehicleDetails has not been set'); },

            templateId: ''
        },

        selectedVehicleName: '',

        // Creates the widget, taking over the UI contained in it, the links in it, 
        // and adding the necessary behaviors.
        _create: function () {
            
            // add on helper methods for invoking public methods on widgets
            mstats.setupWidgetInvocationMethods(this, this.options, ['header']);
            
            this._bindNavigation();

            // ensure that setOption is called with the provided options
            // since the widget framework does not do this.
            this._setOptions(this.options);
        },

        _bindNavigation: function () {
            var that = this; // closure for the event handler below
            this.element.delegate('[data-action]', 'click.vehicleDetails', function (event) {

                var $this = $(this),
                    action = $this.data('action');

                switch (action) {
                    case 'vehicle-delete-selected':
                        that.deleteVehicle($this.data('action-url'));
                        event.preventDefault();
                        break;
                }
            });
        },

        // Gets the vehicle list data (via the sendRequest option) from the dataUrl option endpoint
        // This method also applies the data to the template provided in option.tempalteId
        _getVehicleData: function () {
            var that = this;

            // we invalidate the selected vehicle data first, in order to prevent other
            // widgets from using old data 
            this.selectedVehicleName = undefined;

            this._hideVehicleDetails();
            this.options.sendRequest({
                url: this._createRequestUrl(),
                success: function (data) {
                    var currentHeaderTitle;

                    if (!$(that.options.templateId).length) {
                        that._showVehicleDetails();
                        mstats.log('Vehicle Details: Cannot apply templates as there is no template defined.');
                        return;
                    }

                    that.element
                        .html($(that.options.templateId).tmpl(data, {
                            createRemindersLink: function (vehicleId) {
                                var state = $.bbq.getState() || {},
                                    newUrlBase = mstats.getBaseUrl();
                                state.layout = 'reminders';
                                state.vid = vehicleId;

                                return $.param.fragment(newUrlBase, state);
                            }
                        }));

                    that._attachCharts();

                    // we store the data for the currently selected vehicle 
                    // so that other widgets can potentially make use of it
                    that.selectedVehicleName = data.Name;

                    // when the data for the vehicle is first retrieve, an inital title is already
                    // set. However, we'd like to append the vehicle name to provide better context 
                    // for the user.
                    currentHeaderTitle = that._header('option', 'title') || '';
                    if (currentHeaderTitle.indexOf(' for ') === -1) {
                        that._header('option', 'title', currentHeaderTitle + ' for ' + data.Name);
                    }

                    // now animate them into view
                    that._showVehicleDetails();
                },
                error: function () {
                    that._hideVehicleDetails();
                    that._showErrorMessage();
                }
            });
        },

        _attachCharts: function () {
            this.element.find('#vehicle-charts').vehicleCharts({
                sendRequest: this.options.sendRequest,
                invalidateData: this.options.invalidateData
            });
        },

        _deleteVehicle: function (actionUrl) {
            var that = this;

            this.options.sendRequest({
                url: actionUrl,
                cache: false,
                success: function () {
                    that._showDeletedMessage();
                    that.options.publish(mstats.events.vehicle.deleted, {});
                },
                error: function () {
                    that._showDeleteErrorMessage();
                }
            });
        },

        _showVehicleDetails: function () {
            this.element.find('.content').show();
        },

        _hideVehicleDetails: function () {
            this.element.find('.content').hide();
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
                message: 'An error occurred while loading the requested data.  Please try again.',
                duration: 10000
            });
        },

        _showDeletingMessage: function () {
            this._publishStatus({
                type: 'saving',
                message: 'Deleting the selected vehicle ...',
                duration: 5000
            });
        },

        _showDeletedMessage: function () {
            this._publishStatus({
                type: 'saved',
                message: 'Vehicle deleted.',
                duration: 5000
            });
        },

        _showDeleteErrorMessage: function () {
            this._publishStatus({
                type: 'saveError',
                message: 'An error occurred while deleting the selected vehicle.  Please try again.',
                duration: 10000
            });
        },

        _createRequestUrl: function () {
            return this.options.dataUrl + '/' + this.options.selectedVehicleId;
        },

        // handle setting options 
        _setOption: function (key, value) {
            $.Widget.prototype._setOption.apply(this, arguments);
            if (value <= 0) {
                return;
            }
            switch (key) {
                case 'selectedVehicleId':
                    this.refreshData();
                    break;
            }
        },

        refreshData: function () {
            this._getVehicleData();
        },

        requeryData: function () {
            this.options.invalidateData(this._createRequestUrl());
            this.refreshData();
        },

        getSelectedVehicleName: function () {
            return this.selectedVehicleName || '';
        },

        deleteVehicle: function (actionUrl) {
            var shouldDelete = confirm('Really delete the selected vehicle?');
            if (shouldDelete) {
                this._deleteVehicle(actionUrl);
            }
        }
    });
} (this.mstats, jQuery));
