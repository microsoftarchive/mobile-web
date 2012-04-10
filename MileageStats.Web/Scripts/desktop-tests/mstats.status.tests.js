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

(function ($) {

    module('Status Widget Tests', {
        setup: function () {
            $('#qunit-fixture').append('<div id="notification"/>');
        },
        teardown: function () {

        }
    });

    test('when initialized, then listens for status messages', function () {
        expect(1);
        $('#notification').status({
            subscribe: function (status) {
                ok(status, 'widget subscribed to status message');
            }
        });
        mstats.pubsub.publish(mstats.events.status, { type: 'saving' });
    });

    test('when status message published, then message is displayed', function () {
        expect(1);
        var msg = 'Saved';

        $('#notification').status({
            subscribe: mstats.pubsub.subscribe
        });
        
        mstats.pubsub.publish(mstats.events.status, {
            type: 'saved',
            duration: 3000,
            message: msg
        });

        setTimeout(function () {
            equal($('#notification').text(), msg, 'message initially displayed');
            start();
        }, 200); // give it a 200ms chance to set the message
        stop();
    });

    test('when status message published, then hides after duration expires', function () {
        expect(2);
        var msg = 'Saved';

        $('#notification').status({
            subscribe: mstats.pubsub.subscribe
        });
        
        mstats.pubsub.publish(mstats.events.status, {
            type: 'saved',
            duration: 2000,
            message: msg
        });

        setTimeout(function () {
            equal($('#notification').text(), msg, 'message initially displayed');
            setTimeout(function () {
                ok($('#notification').is(':hidden'), 'widget hid message after duration');
                start();
            }, 1500);
        }, 1000);
        stop();
    });

    test('when "saved" message published, then hides "saving" message', function () {
        expect(2);
        var savingMsg = 'Saving ...',
            savedMsg = 'Saved';

        $('#notification').status({
            subscribe: mstats.pubsub.subscribe
        });
        
        mstats.pubsub.publish(mstats.events.status, {
            type: 'saving',
            //duration: 3000,
            message: savingMsg
        });

        setTimeout(function () {
                
            equal($('#notification').text(), savingMsg, 'message initially displayed');

            // preempt 'saving' with a 'saved' message
            mstats.pubsub.publish(mstats.events.status, {
                type: 'saved',
                duration: 2000,
                message: savedMsg
            });

            // assert the saving message is now showing
            setTimeout(function () {
                equal($('#notification').text(), savedMsg, 'saved message displayed on top of saving');
                start();
            }, 1500); // wait until we're sure the "saved" message is showing

        }, 1500); // wait until after the default 1000ms delay for showing "saving"
        stop();
    });

}(jQuery));