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

(function ($) {

    module('MileageStats PubSub');

    test('When publish, subscribers are fired', function () {
        expect(1);
        mstats.pubsub.subscribe('test', function () {
            ok(true, 'Yep, event is firing');
            start();
        });
        mstats.pubsub.publish('test');
        stop();
    });

    test('When data is supplied, it is passed from publisher to subscriber', function () {
        expect(1);
        var testdata = 'test data',
            testevent = 'test event';
        mstats.pubsub.subscribe(testevent, function (data) {
            equal(data, testdata, 'Correct data arrived through ' + testevent + ' in one piece.');
            start();
        });
        mstats.pubsub.publish(testevent, testdata);
        stop();
    });

    test('When unsubscribed from an event, future events are not fired', function () {
        expect(0);
        var testevent = 'some.event',
            handler = function () {
                ok(false, 'PubSub should no longer be subscribed here');
            };
        mstats.pubsub.subscribe(testevent, handler, this);
        mstats.pubsub.unsubscribe(testevent, handler, this);
        mstats.pubsub.publish(testevent);
    });

    test('When a subscribed callback throws, then other events are still fired', function () {
        expect(1);
        var testevent = 'some.eventWithError',
            badHandler = function () {
                throw "some error";
            },
            anotherHandler = function () {
                ok(true, 'anotherHandler was called after an error in another handler');
                start();
            };

        mstats.pubsub.subscribe(testevent, badHandler, this);
        mstats.pubsub.subscribe(testevent, anotherHandler, this);

        mstats.pubsub.publish(testevent);
        stop();
    });

} (jQuery));
