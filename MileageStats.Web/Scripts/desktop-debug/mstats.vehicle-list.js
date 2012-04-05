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
    var animationLength = 600,
        delayLength = 400,
        dataUrl;

    $.widget('mstats.vehicleList', {
        // default options
        options: {
            // Default to $.ajax when sendRequest is undefined.
            // The extra function indirection allows $.ajax substitution because 
            // the widget framework is using the real $.ajax during options 
            // initialization.
            sendRequest: function (ajaxOptions) { $.ajax(ajaxOptions); },

            // invalidateData allows the vehicleList to invalidate data
            // stored in the data cache.  
            invalidateData: function () {
                mstats.log('no data invalidation function provided.');
            },

            // option controlling the compact state of the widget.
            layout: 'dashboard',

            // option providing the event publisher used for communicating with 
            // the status widget.
            publish: function () { mstats.log('The publish option on vehicleList has not been set'); },

            // option for the selectedVehicleId
            selectedVehicleId: 0,

            visible: true,

            isCollapsed: false
        },

        // Creates the widget, taking over the UI contained in it, the links in it, 
        // and adding the necessary behaviors.
        _create: function () {

            dataUrl = this.element.data('list-url');

            this._widgetizeVehicleTiles();
            this._bindEventHandlers();
            this._makeSortable();
            this._getVehicleData();

            // ensure that setOption is called with the provided options
            // since the widget framework does not do this.
            this._setOptions(this.options);

            if (this.options.layout !== 'dashboard') {
                this._setOption('isCollapsed', true);
                this._checkSelectedVehicleId();
            }
        },

        // Bind to necessary events within the vehicle list
        _bindEventHandlers: function () {
            var that = this;
            this.element.delegate('[data-action]', 'click.' + this.name, function () {
                that._setOption('selectedVehicleId', $(this).closest('.vehicle').data('vehicle-id'));
                // intentionally NOT calling preventDefault so the adjusted (bbq) links will work
            });
        },

        // add the tile widget to each tile, and the vehicle widget to each vehicle
        _widgetizeVehicleTiles: function () {
            this.element
                .find('#vehicle-list-content > div').tile() // widgetize tiles
                .find('.vehicle').vehicle(); // widgetize vehicles
        },

        // Gets the vehicle list data (via the sendRequest option) from the dataUrl 
        // option endpoint. This method also applies the data to the template 
        // provided in option.templateId
        _getVehicleData: function () {
            var that = this,
                $template = $(this.options.templateId);

            if (!$template.length) {
                this._showVehicles();
                mstats.log('Cannot apply templates as there is no template defined.');
                return;
            }

            this.options.sendRequest({
                url: dataUrl,
                success: function (data) {
                    var vehicleTemplate = $template.tmpl(data),
                        buttonTemplate = $('#mstats-add-vehicle-button-template').tmpl();
                    
                    that.element.find('#vehicle-list-content')
                        .html(vehicleTemplate)
                        .append(buttonTemplate);

                    that._widgetizeVehicleTiles();

                    // ensure that setOption is called with the provided options
                    // since the widget framework does not do this.
                    that._setOptions(that.options);

                    // now animate them into view
                    that._showVehicles();
                },
                error: function () {
                    that._hideVehicles();
                    that._showErrorMessage();
                }
            });
        },

        _hideVehicles: function () {
            this.element.find(':mstats-tile').hide();
        },

        // Show the vehicles list 
        _showVehicles: function () {
            // consider moving this to inside the tile widget
            this.element.find(':mstats-tile').show();
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
                message: 'An error occurred while loading the requested data. ' +
                         'Please try again.',
                duration: 10000
            });
        },

        // show the status widget in the saving state
        _showSaveMessage: function () {
            this._publishStatus({
                type: 'saving',
                message: 'Saving sort order ...'
            });
        },

        // show the status widget in the saved state
        _showSavedMessage: function () {
            this._publishStatus({
                type: 'saved',
                message: 'Sort order saved',
                duration: 5000
            });
        },

        // show the status widget in the error state after a failed save
        _showSaveErrorMessage: function () {
            this._publishStatus({
                type: 'saveError',
                message: 'An error occurred while saving the sort order. ' +
                         'Please reload the page and try again.',
                duration: 10000
            });
        },

        /********************************************************
        * Sorting Methods               
        ********************************************************/

        // add the sortable behavior to the vehicle list
        _makeSortable: function () {
            var that = this;
            
            this.element.sortable({
                axis: false,
                items: ':mstats-tile:has(:mstats-vehicle)',
                containment: '#vehicles',
                handle: '.header',
                stop: function () {
                    that.saveSortOrder();
                }
            });
        },

        // save the vehicle list sort order
        saveSortOrder: function () {
            var that = this,
                url = this.element.data('sort-url'),
                sortorder = [];

            this.element.find(':mstats-vehicle').each(function () {
                var id = $(this).data('vehicle-id');
                sortorder.push(id);
            });

            this._showSaveMessage();

            // Passing the sort order as a string avoids the need 
            // for a custom model binder on the server side.
            this.options.sendRequest({
                url: url,
                cache: false,
                data: {
                    SortOrder: sortorder.toString()
                },
                success: function () {
                    that._showSavedMessage();
                },
                error: function () {
                    that._showSaveErrorMessage();
                }
            });
        },

        // handle setting options 
        _setOption: function (key, value) {
            $.Widget.prototype._setOption.apply(this, arguments);
            if (value <= 0) {
                return;
            }
            switch (key) {
                case 'layout':
                    this._setLayoutOption(value);
                    break;
                case 'selectedVehicleId':
                    this._expandVehicle(value);
                    this._collapseVehicles();
                    break;
            }
        },

        _checkSelectedVehicleId: function () {
            var vid,
                state = $.bbq.getState() || {};

            if (state.vid) {
                vid = parseInt(state.vid, 10);
                if (vid !== this.options.selectedVehicleId) {
                    this._setOption('selectedVehicleId', state.vid);
                }
            }
        },


        // adjust the size of the widget between compact and expanded mode
        _setLayoutOption: function (layout) {
            switch (layout.toLowerCase()) {
                case 'dashboard':
                    // ensure any changes to styles are cleaned up before changing classes
                    this.element.removeAttr('style')
                        .removeClass('compact')
                        .sortable('option', 'axis', false);
                    break;
                case 'details':
                    // intentional fall through
                case 'reminder':
                    // intentional fall through
                case 'fillup':
                    // ensure any changes to styles are cleaned up before adding classes
                    this.element.removeAttr('style')
                        .addClass('compact')
                        .sortable('option', 'axis', 'y');
                    this._collapseVehicles();
                    break;
            }
        },

        /********************************************************
        * Animations
        ********************************************************/

        // collapse all but the selected vehicle
        _collapseVehicles: function () {
            var selected = this.options.selectedVehicleId;

            this.element.find(':mstats-vehicle').each(function () {
                var $this = $(this);
                if ($this.vehicle('option', 'id') !== selected) {
                    $this.vehicle('collapse');
                }
            });
        },

        // expand all the vehicles
        _expandVehicles: function () {
            this.element.find(':mstats-vehicle').each(function () {
                $(this).vehicle('expand');
            });
        },

        // expand only the specified vehicle
        _expandVehicle: function (id) {
            var selected = id;

            this.element.find(':mstats-vehicle').each(function () {
                var $this = $(this);
                if (($this.vehicle('option', 'id') === selected)
                        && $this.vehicle('option', 'collapsed')) {
                    $this.vehicle('expand');
                }
            });
        },

        // shrink the list to the one column size
        _narrowToSingleColumn: function () {
            var $element = this.element;
            
            if (!this.options.isCollapsed) {
                
                $element.delay(delayLength).animate({
                    left: 0
                }, {
                    duration: animationLength,
                    complete: function () {
                        $element.addClass('compact');
                    }
                });
            }
        },

        // grow the list to the two column size
        _expandToDoubleColumn: function () {
            if (this.options.isCollapsed) {
                this.element.removeClass('compact')
                    .animate({
                        left: 400
                    }, {
                        duration: animationLength
                    });
            }
        },

        _scrollToSelectedVehicle: function () {
            var that = this;
            
            this.element.find(':mstats-tile').each(function () {
                var id,
                    top,
                    $this = $(this);

                id = $this.find('.vehicle').vehicle('option', 'id');
                top = $this.offset().top;
                if (id === that.options.selectedVehicleId) {
                    if (top > 300) {
                        $('html, body').delay(delayLength)
                            .animate({ scrollTop: top }, 500);
                    }
                }
            });
        },

        /********************************************************
        * Public Methods               
        ********************************************************/
        // called externally (usually by the layoutManager widget)
        goToDetailsLayout: function () {
            var selectedVehicle,
                vid = 0,
                that = this,
                runningTop = 0,
                animationInfoArray,
                state = $.bbq.getState() || {};
                this._checkSelectedVehicleId();
                selectedVehicle = this.options.selectedVehicleId;

            if (!this.options.isCollapsed) {
                this.element.find(':mstats-tile').each(function () {
                    $(this).tile('beginAnimation');
                });

                this.element.find(':mstats-tile').each(function () {
                    var $this = $(this);
                    vid = $this.find('.vehicle').vehicle('option', 'id');
                    animationInfoArray = [{
                        position: { top: runningTop },
                        duration: animationLength
                    }, {
                        position: { left: 0 },
                        duration: animationLength
                    }];

                    $this.tile('moveTo', animationInfoArray, function () {
                        that.element.find(':mstats-tile').each(function () {
                            $(this).tile('endAnimation');
                        });
                    });

                    // calculate the runningTop for next time around
                    if (vid === selectedVehicle) {
                        runningTop += 321;
                    } else {
                        runningTop += 206;
                    }
                });

                this._narrowToSingleColumn();
            }

            this._scrollToSelectedVehicle();
            if (state && state.layout) {
                this.options.layout = state.layout;
            }

            this.options.isCollapsed = true;
        },

        // called externally (usually by the layoutManager widget)
        goToDashboardLayout: function () {
            var vid = 0,
                that = this,
                animationInfoArray;

            this.options.layout = 'dashboard';
            this.options.selectedVehicleId = 0;

            if (!this.options.isCollapsed) {
                return;
            }

            this.element.find(':mstats-tile').each(function () {
                $(this).tile('beginAnimation');
            });

            this._expandToDoubleColumn();

            this.element.find(':mstats-tile').each(function (index) {
                var $this = $(this);

                vid = $this.find('.vehicle').vehicle('option', 'id');
                animationInfoArray = [{
                    position: { left: index % 2 * 260 },
                    duration: animationLength
                }, {
                    position: { top: Math.floor(index / 2) * 320 },
                    duration: animationLength
                }];

                that._expandVehicles();

                $this.tile('moveTo', animationInfoArray, function () {
                    that.element.find(':mstats-tile').each(function () {
                        $(this).tile('endAnimation');
                    });
                });
            });

            this.options.isCollapsed = false;
        },

        moveOffScreen: function () {
            var that = this;
            
            if (!this.options.visible) { return; }
            
            this.element.css('position', 'absolute')
                .delay(delayLength)
                .animate({
                    left: '-=600',
                    opacity: 0
                }, {
                    duration: animationLength,
                    complete: function () {
                        that.element.hide();
                    }
                });
            this.options.visible = false;
        },

        moveOnScreen: function () {
            if (this.options.visible) {
                return;
            }

            this.element.css('opacity', 0)
                .animate({
                    opacity: 1,
                    left: '+=600'
                }, animationLength)
                .show();
            this._setOption('visible', true);
        },

        _invalidateData: function () {
            if ($.isFunction(this.options.invalidateData)) {
                this.options.invalidateData(dataUrl);
            }
        },

        refreshData: function () {
            this._getVehicleData();
        },

        requeryData: function () {
            this._invalidateData();
            this.refreshData();
        },

        // cleanup
        destroy: function () {
            this.element
                .sortable('destroy')
                .find(':mstats-tile').tile('destroy')
                .find(':mstats-vehicle').vehicle('destroy');

            $.Widget.prototype.destroy.call(this);
        }
    });
} (this.mstats = this.mstats || {}, jQuery));
