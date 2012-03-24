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

    var animationDuration = 600,
        delayLength = 550;

    $.widget('mstats.infoPane', {
        options: {
            visible: false,
            activePane: 'details',
            selectedVehicleId: 0,
            sendRequest: function () {
                mstats.log('The sendRequest option on infoPane has not been set');
            },
            invalidateData: function () {
                mstats.log('The invalidateData option on infoPane has not been set');
            },
            publish: function () {
                mstats.log('The publish option on infoPane has not been set');
            },
            header: null
        },
        
        // these are widgets that are created internally    
        fillups: null,
        reminders: null,
        vehicleDetails: null,
            
        _create: function () {
            
            this._setupDetailsPane();
            this._setupRemindersPane();
            this._setupFillupsPane();
            
            // add on helper methods for invoking public methods on widgets
            mstats.setupWidgetInvocationMethods(this, this, ['fillups','reminders','vehicleDetails']);
            mstats.setupWidgetInvocationMethods(this, this.options, ['header']);

            this._bindNavigation();
        },

        // initialize the details pane
        _setupDetailsPane: function () {
            var elem = this.element,
                options = this.options;
            
            if (!elem.find('#details-pane').length) {
                elem.find('div:first')
                    .append('<div id="details-pane" class="tab opened section" />');
            }
            this.vehicleDetails = elem.find('#details-pane').vehicleDetails({
                sendRequest: options.sendRequest,
                dataUrl: elem.data('details-url'),
                templateId: '#mstats-vehicle-details-template',
                invalidateData: options.invalidateData,
                publish: options.publish,
                header: options.header
            });
        },

        // initialize the fillups pane
        _setupFillupsPane: function () {
            var elem = this.element,
                options = this.options;
            
            if (!elem.find('#fillups-pane').length) {
                elem.find('div:first')
                    .append('<div id="fillups-pane" class="tab section" />');
            }
            this.fillups = elem.find('#fillups-pane').fillups({
                sendRequest: options.sendRequest,
                dataUrl: elem.data('fillups-url'),
                templateId: '#mstats-fillups-template',
                invalidateData: options.invalidateData,
                publish: options.publish
            });
        },

        // initialize the reminders pane
        _setupRemindersPane: function () {
            var elem = this.element,
                options = this.options;
            
            if (!elem.find('#reminders-pane').length) {
                elem.find('div:first')
                    .append('<div id="reminders-pane" class="tab section" />');
            }
            this.reminders = elem.find('#reminders-pane').reminders({
                sendRequest: options.sendRequest,
                dataUrl: elem.data('reminders-url'),
                templateId: '#mstats-reminders-template',
                invalidateData: options.invalidateData,
                publish: options.publish
            });
        },

        _bindNavigation: function () {
            var that = this; // closure for the event handler below
            
            this.element.delegate('[data-info-nav]', 'click.infoPane', function (event) {
                var action = $(this).data('info-nav');
                that._setHashLayout(action);
                event.preventDefault();
            });
        },

        // handle setting options 
        _setOption: function (key, value) {
            $.Widget.prototype._setOption.apply(this, arguments);
            switch (key) {
                case 'selectedVehicleId':
                    value = parseInt(value, 10);
                    this._setSelectedVehicleId(value);
                    break;
                case 'activePane':
                    var methodName = '_' + value + 'Selected';
                    if(this[methodName]) { this[methodName](); }
                    break;
            }
        },
            
        getSelectedVehicleName: function () {
            return this._vehicleDetails('getSelectedVehicleName');
        },

        _setSelectedVehicleId: function (id) {
            this._vehicleDetails('option', 'selectedVehicleId', id);
            this._fillups('option', 'selectedVehicleId', id);
            this._reminders('option', 'selectedVehicleId', id);
        },

        _setOpen: function (o) {
            var key, val, elem;
            for (key in o) {
                if (o.hasOwnProperty(key)) {
                    val = o[key];
                    elem = this.element.find(key);
                    if (val) {
                        elem.addClass('opened', 300);
                    } else {
                        elem.removeClass('opened', 300);
                    }
                }
            }
        },

        _setHashLayout: function (newLayout) {
            var state = $.bbq.getState() || {};
            state.layout = newLayout;
            $.bbq.pushState(state, 2);
        },

        _detailsSelected: function () {
            this._header('option', 'title', 'Details');
            this._setOpen({
                '#details-pane': true,
                '#fillups-pane': false,
                '#reminders-pane': false
            });
        },

        _fillupsSelected: function () {
            this._header('option', 'title', 'Fill ups');
            this._setOpen({
                '#details-pane': true,
                '#fillups-pane': true,
                '#reminders-pane': false
            });
        },

        _remindersSelected: function () {
            this._header('option', 'title', 'Reminders');
            this._setOpen({
                '#details-pane': true,
                '#fillups-pane': true,
                '#reminders-pane': true
            });
        },

        moveOnScreenFromRight: function () {
            if(this.options.visible) { return; }
            
            this.element
                .removeClass('empty')
                .css({ left: 900, opacity: 0, position: 'relative' })
                .show()
                .delay(delayLength)
                .animate({ left: '-=640', opacity: 1 }, animationDuration);
            this._setOption('visible', true);
        },

        moveOffScreenToRight: function () {
            var that = this;
            
            if(!this.options.visible) { return; }
            
            this.element
                .css({ position: 'absolute', top: 0 })
                .animate({ left: '+=640', opacity: 0 }, {
                    duration: animationDuration,
                    complete: function () {
                            that.element.css({ position: 'relative' });
                            that.element.hide();
                        }});
            this.options.visible = false;
        },

        requeryVehicleDetails: function () {
            this._vehicleDetails('requeryData');
        },

        requeryFillups: function () {
            this._fillups('requeryData');
        },

        requeryReminders: function () {
           this._reminders('requeryData');
        },

        destroy: function () {
            this._vehicleDetails('destroy');
            this._fillups('destroy');
            this._reminders('destroy');
            
            $.Widget.prototype.destroy.call(this);
        }
    });

} (this.mstats, jQuery));
