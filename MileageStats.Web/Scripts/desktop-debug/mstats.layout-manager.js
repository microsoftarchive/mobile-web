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

(function(mstats, $) {

    $.widget('mstats.layoutManager', {
            options: {
                layout: 'dashboard',
                subscribe: function () { mstats.log('The subscribe option on layoutManager has not been set'); },
                pinnedSite: null,
                charts: null,
                header: null,
                infoPane: null,
                summaryPane: null,
                vehicleList: null
            },

            _create: function() {
                var state = $.bbq.getState() || {  };

                // add on helper methods for invoking public methods on widgets
                mstats.setupWidgetInvocationMethods(this, this.options, ['charts', 'header', 'infoPane', 'summaryPane', 'vehicleList']);

                this._subscribeToGlobalEvents();
                this._subscribeToHashChange();

                if (state && state.layout) {
                    this._setOption('layout', state.layout);
                }

                this._changeLayout(state);
            },

            _subscribeToGlobalEvents: function() {
                var state = {  },
                    that = this;

                this.options.subscribe(mstats.events.vehicle.deleted,
                    function() {
                        // we need to refresh the vehicle list, summary reminders, 
                        // fleet statistics, and redirect to the dashboard
                        that._summaryPane('requeryStatistics');
                        that._summaryPane('requeryReminders');
                        that._vehicleList('requeryData');
                        that._charts('requeryData');
                        state.layout = 'dashboard';
                        $.bbq.pushState(state, 2);
                    }, this);

                this.options.subscribe(mstats.events.vehicle.reminders.fulfilled,
                    function() {
                        // we need to refresh the summary reminders, fleet statistics, 
                        // details pane, reminders pane and jump list
                        that._summaryPane('requeryStatistics');
                        that._summaryPane('requeryReminders');
                        that._infoPane('requeryVehicleDetails');
                        that._infoPane('requeryReminders');
                        that._charts('requeryData');

                        that.options.pinnedSite.requeryJumpList();
                    }, this);
            },

            _changeLayout: function(state) {
                this._setOption('layout', state.layout || 'dashboard');

                this._setupInfoPaneOptions(state);
                switch (this.options.layout) {
                case 'dashboard':
                    this._header('option', 'title', 'Dashboard');
                    this._goToDashboardLayout();
                    break;
                case 'charts':
                    this._goToChartsLayout();
                    this._header('option', 'title', 'Charts');
                    break;
                case 'details':
                    // The first time we transition to details, the selected vehicle data is not available,
                    // in that case the vehicleDetail widgets takes over the responsibility of setting the title
                    // because it contains additional information to display there
                    this._setHeaderForSelectedVehicle('Details');
                    this._goToDetailsLayout();
                    break;
                case 'fillups':
                    this._setHeaderForSelectedVehicle('Fill ups');
                    this._goToDetailsLayout();
                    break;
                case 'reminders':
                    this._setHeaderForSelectedVehicle('Reminders');
                    this._goToDetailsLayout();
                    break;
                }
            },

            _setHeaderForSelectedVehicle: function(titleBase) {
                var vehicleName = this._infoPane('getSelectedVehicleName'),
                    title = titleBase;

                if (vehicleName) {
                    title = title + " for " + vehicleName;
                }

                this._header('option', 'title', title);
            },

            _subscribeToHashChange: function() {
                var that = this;
                $(window).bind('hashchange.layoutManager', function(event) {
                    var state = $.deparam.fragment(true);
                    that._changeLayout(state);
                });
            },

            _setupInfoPaneOptions: function(state) {
                this._vehicleList('option', 'selectedVehicleId', state.vid);
                this._infoPane('option', 'selectedVehicleId', state.vid);
                this._infoPane('option', 'activePane', state.layout);
            },

            _goToChartsLayout: function() {
                this._summaryPane('moveOffScreen');
                this._vehicleList('moveOffScreen');
                this._infoPane('moveOffScreenToRight');
                this._charts('moveOnScreenFromRight');
            },

            _goToDetailsLayout: function() {
                this._charts('moveOffScreenToRight');
                this._summaryPane('moveOffScreen');
                this._infoPane('moveOnScreenFromRight');
                this._vehicleList('moveOnScreen');
                this._vehicleList('goToDetailsLayout');
            },

            _goToDashboardLayout: function() {
                this._charts('moveOffScreenToRight');
                this._summaryPane('moveOnScreen');
                this._vehicleList('moveOnScreen');
                this._infoPane('moveOffScreenToRight');
                this._vehicleList('goToDashboardLayout');
            },

            destroy: function() {
                // will unbind event handlers namespaced with the widget's name
                $.Widget.prototype.destroy.call(this);
            }
        });

}(this.mstats = this.mstats || {  }, jQuery));