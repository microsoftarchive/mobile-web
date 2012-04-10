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
    /**
    * PubSub is the messaging system used by 
    * the widgets to listen for and fire events
    */
    mstats.pubsub = (function () {
        var queue = [],
            that = {};

        /**
        * Executes all callbacks associated with eventName using the 
        * context provided in the subscription.
        *
        * @param {string} eventName  The name of the event to publish
        *                            Uses a dot notation for consistency
        * @param {object} data       Any data to be passed to the event
        *                            handler. Custom to the event.
        */

        that.publish = function (eventName, data) {
            var context, intervalId, idx = 0;
            if (queue[eventName]) {
                intervalId = setInterval(function () {
                    if (queue[eventName][idx]) {
                        try {
                            context = queue[eventName][idx].context || this;
                            queue[eventName][idx].callback.call(context, data);
                        } catch (e) {
                            // log the message for developers
                            mstats.log('An error occurred in one of the callbacks for the event "' + eventName + '"');
                            mstats.log('The error was: "' + e + '"');
                        }

                        idx += 1;
                    } else {
                        clearInterval(intervalId);
                    }
                }, 0);

            }
        };
        /**
        * Stores an event subscription. Subsequent event subscriptions
        * are always added (not overwritten). Use unsubscribe to remove
        * event subscriptions.
        *
        * @param {string} eventName  The name of the event to publish
        *                            Uses a dot notation for consistency
        * @param {function} callback The function to be called when the 
        *                            event is published.
        * @param {object} context    The context to execute the callback
        */
        that.subscribe = function (eventName, callback, context) {
            if (!queue[eventName]) {
                queue[eventName] = [];
            }
            queue[eventName].push({
                callback: callback,
                context: context
            });
        };
        /**
        * Removes an event subscription.
        *
        * @param {string} eventName  The name of the event to remove
        * @param {function} callback The function associated witht the
        *                            event. Used to ensure the correct
        *                            event is being removed.
        * @param {object} context    The context associated with the 
        *                            callback. Used to ensure the correct
        *                            event is being removed.
        */
        that.unsubscribe = function (eventName, callback, context) {
            if (queue[eventName]) {
                queue[eventName].pop({
                    callback: callback,
                    context: context
                });
            }
        };

        return that;
    } ());

} (this.mstats = this.mstats || {}, jQuery));
