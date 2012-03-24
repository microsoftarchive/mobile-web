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
        delayLength = 400;

    $.widget('mstats.summaryPane', {

        options: {
            visible : true,
            
            sendRequest: function (ajaxOptions) { $.ajax(ajaxOptions); },

            publish: function () {
                mstats.log('The publish option on summaryPane has not been set');
            },

            invalidateData: function () {
                mstats.log('The invalidateData option on summaryPane has not been set');
            },
            
            header: null
        },
            
        registration: null,
        imminentRemindersPane: null,
        statisticsPane: null,

        // Initialize the widget
        _create: function () {
            this._setupRegistrationWidget();
            this._setupStatisticsWidget();
            this._setupRemindersWidget();

            mstats.setupWidgetInvocationMethods(this, this, ['statisticsPane', 'registration', 'imminentRemindersPane']);
            mstats.setupWidgetInvocationMethods(this, this.options, ['header']);
        },

        // initialize the registration widget
        _setupRegistrationWidget: function () {
            var that = this,
                elem = this.element.find('#registration');
            
            this.registration = elem.registration({
                dataUrl: elem.data('url'),
                publish: this.options.publish,
                sendRequest: this.options.sendRequest,
                displayNameChanged: function (event, args) {
                    that._header('option', 'displayName', args.displayName);
                }
            });
        },

        // initialize the statistics widget
        _setupStatisticsWidget: function () {
            var elem = this.element.find('#statistics');
            
            this.statisticsPane = elem.statisticsPane({
                sendRequest: this.options.sendRequest,
                dataUrl: elem.data('url'),
                invalidateData: this.options.invalidateData,
                templateId: '#fleet-statistics-template'
            });
        },

        // initialize the reminders widget
        _setupRemindersWidget: function () {
            var elem = this.element.find('#reminders');
            
            this.imminentRemindersPane = elem.imminentRemindersPane({
                sendRequest: this.options.sendRequest,
                dataUrl: elem.data('url'),
                invalidateData: this.options.invalidateData,
                templateId: '#summary-imminent-reminders-template'
            });
        },

        // move the whole summary pane off-screen
        moveOffScreen: function () {
            var that = this;
            
            if(!this.options.visible) { return; }

            this.element
                .delay(delayLength)
                .animate({
                    left: '-=350',
                    opacity: 0
                }, {
                    duration: animationDuration,
                    complete: function () {
                        that.element.hide();
                    }
                });
            this._setOption('visible', false);
        },

        // move the whole summary pane on-screen
        moveOnScreen: function () {
            
            if(this.options.visible) { return; }
            
            this.element
                .css('opacity', 0)
                .animate({
                    opacity: 1,
                    left: '+=350' 
                }, animationDuration)
                .show();
            this._setOption('visible', true);
        },

        requeryStatistics: function () {
            this._statisticsPane('requeryData');
        },

        requeryReminders: function () {
            this._imminentRemindersPane('requeryData');
        },

        // cleanup 
        destroy: function () {
            $.Widget.prototype.destroy.call(this); // default destroy

            this._statisticsPane('destroy');
            this._registration('destroy');
            this._imminentRemindersPane('destroy');
        }

    });

}(this.mstats, jQuery));