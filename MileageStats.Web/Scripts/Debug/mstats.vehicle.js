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

    var shortAnimationLength = 600,
        longAnimationLength = 800;
    
    $.widget('mstats.vehicle', {

        options: {
            id: 0,
            collapsed: false
        },

        _create: function () {
            this._setOption('id', this.element.data('vehicle-id'));
            this._adjustNavigation();

            if (this.options.collapsed) {
                this._forceCollapse();
            }
        },

        _forceCollapse: function () {
            this.element
                .find('.mile-per-gallon')
                    .find('.quantity').hide().end()
                    .find('.unit').hide().end()
                    .end()
                .find('.cost-per-mile')
                    .find('.quantity').hide().end()
                    .find('.unit').hide().end()
                    .end()
                .find('.cost-per-month')
                    .find('.quantity').hide().end()
                    .find('.unit').hide().end()
                    .end()
                .find('.statistics')
                    .height(0)
                    .closest(':mstats-vehicle')
                        .addClass('compact');
        },

        // Take over link navigation in the widget to enable a
        // single page interface.  This will keep all links point to the dashboard
        // with the proper url hash set for the action.
        _adjustNavigation: function () {
            var that = this;
            
            this.element.find('[data-action]').each(function () {
                var $this = $(this),
                    action = $this.data('action'),
                    vehicleId = that.options.id,
                    state = $.bbq.getState() || {},
                    newUrlBase = mstats.getBaseUrl();

                state.vid = vehicleId;
                switch (action) {
                    case 'vehicle-details-selected':
                        state.layout = 'details';
                        break;
                    case 'vehicle-fillups-selected':
                        state.layout = 'fillups';
                        break;
                    case 'vehicle-reminders-selected':
                        state.layout = 'reminders';
                        break;
                    case 'vehicle-add-selected':
                        state.layout = 'addVehicle';
                        state.vid = undefined;
                        break;
                }
                $this.attr('href', $.param.fragment(newUrlBase, state));
            });
        },

        collapse: function () {
            if (this.options.collapsed) {
                return;
            }

            this.element
                .find('.mile-per-gallon')
                    .find('.quantity').fadeOut(shortAnimationLength).end()
                    .find('.unit').fadeOut(shortAnimationLength).end()
                    .end()
                .find('.cost-per-mile')
                    .find('.quantity').fadeOut(shortAnimationLength).end()
                    .find('.unit').fadeOut(shortAnimationLength).end()
                    .end()
                .find('.cost-per-month')
                    .find('.quantity').fadeOut(shortAnimationLength).end()
                    .find('.unit').fadeOut(shortAnimationLength).end()
                    .end()
                .find('.statistics')
                    .animate({
                        height: 0
                    }, {
                        duration: longAnimationLength,
                        easing: 'linear',
                        complete: function () {
                            $(this).closest(':mstats-vehicle')
                                .addClass('compact');
                        }
                    });

            this._setOption('collapsed', true);
        },

        expand: function () {
            if (!this.options.collapsed) {
                return;
            }

            this.element
                .removeClass('compact')
                .find('.statistics')
                    .animate(
                        { height: 115 },
                        { duration: longAnimationLength, easing: 'linear' }
                    )
                    .find('.mile-per-gallon')
                        .find('.quantity').fadeIn(shortAnimationLength).end()
                        .find('.unit').fadeIn(shortAnimationLength).end()
                        .end()
                    .find('.cost-per-mile')
                        .find('.quantity').fadeIn(shortAnimationLength).end()
                        .find('.unit').fadeIn(shortAnimationLength).end()
                        .end()
                    .find('.cost-per-month')
                        .find('.quantity').fadeIn(shortAnimationLength).end()
                        .find('.unit').fadeIn(shortAnimationLength).end()
                        .end();

            this._setOption('collapsed', false);
        }

    });
} (this.mstats, jQuery));
