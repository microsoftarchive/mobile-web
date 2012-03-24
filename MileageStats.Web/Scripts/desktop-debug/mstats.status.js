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
    
    var priorities = {
        saveError: 1,
        saved: 2,
        saving: 3,
        loadError: 4
    };

    $.widget('mstats.status', {
        options: {
            duration: 3000,
            subscribe: function () { mstats.log('The subscribe option on status has not been set'); }
        },

        _create: function () {
            // handle global status events
            this.options.subscribe(mstats.events.status, this._statusSubscription, this);
        },

        currentStatus: null,

        _statusSubscription: function (status) {
            var that = this,
                current = this.currentStatus;

            status.priority = this._getPriority(status);

            // cancel displaying the current message if its priority is lower than
            // the new message. (the lower the int the higher priority)
            if (current && (status.priority < current.priority)) {
                clearTimeout(current.timer);
            }

            current = status;

            this.element.text(status.message).show();

            // set the message for the duration
            current.timer = setTimeout(function () {
                that.element.fadeOut();
                that.currentStatus = null;
            }, status.duration || this.options.duration);
        },

        _getPriority: function (status) {
            return priorities[status.type];
        },

        destroy: function () {
            if (this.currentStatus) {
                clearTimeout(this.currentStatus.timer);
            }
            $.Widget.prototype.destroy.call(this);
        }

    });

} (this.mstats = this.mstats || {}, jQuery));