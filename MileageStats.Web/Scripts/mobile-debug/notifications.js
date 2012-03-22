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

(function (mstats) {

    // this module persist message notifications across different
    // ajax calls, so they can be correctly rendered in the views

    mstats.notifications = function (require) {

        var alertMessage = null,
			confirmMessage = null;

        function log(model) {
            alertMessage = model.FlashAlert || null;
            confirmMessage = model.FlashConfirm || null;
        }

        function renderTo(el) {
            UpdateFlashMessage(confirmMessage, "confirm", el);
            confirmMessage = null;
            UpdateFlashMessage(alertMessage, "alert", el);
            alertMessage = null;
        }

        return {
            log: log,
            renderTo: renderTo
        };

    };

    function UpdateFlashMessage(flashMessage, flashMessageType, context) {
        var flashContainer = context.find('#flash' + flashMessageType);
        if (flashMessage) {
            if (flashContainer.length > 0) {
                flashContainer.text(flashMessage);
            } else {
                var newContainer = $('<div><p id="flash' + flashMessageType + '">' + flashMessage + '</p></div>')
                        .addClass('flash ' + flashMessageType);

                context.find('nav').first().after(newContainer);
            }
        } else {
            //Clear out flash message
            if (flashContainer.length > 0) {
                flashContainer.text('');
            }
        }
    }
} (this.mstats = this.mstats || {}));