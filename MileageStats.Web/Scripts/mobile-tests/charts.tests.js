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

(function (specs, app) {

    module('charts specs');

    test('charts module constructs itself', function () {
        var module = app.charts(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('charts subscribes to button click and searches for form values in the given view. also subscribes to submit event and cancels default', function () {

        expect(6);
        var module = app.charts(mocks.create());
        var clickEventHandler;
        var submitEventHandler;
        var testSubmitEventCancelled = false;
        var testSubmitEvent = { preventDefault: function () { testSubmitEventCancelled = true; } };

        var chartImage = { attr: function (attributeName, attributeValue) {
            equal(attributeName, 'src');
            equal(0, attributeValue.indexOf('testchartimageurl'));
            ok(attributeValue.indexOf('&ChartName=testchartname') > 0);
            ok(attributeValue.indexOf('&StartDate=teststartdate') > 0);
            ok(attributeValue.indexOf('&EndDate=testenddate') > 0);
        }
        };
        var mockView = {
            find: function (selector) {
                switch (selector) {
                    case '#ChartRefreshButton':
                        return {
                            click: function (clickEventSubscription) {
                                clickEventHandler = clickEventSubscription;
                            }
                        }; break;
                    case 'form':
                        return {
                            submit: function (submitEventSubscription) {
                                submitEventHandler = submitEventSubscription;
                            }
                        }; break;
                    case 'select[name=ChartName] option:selected':
                        return {
                            val: function () { return 'testchartname'; }
                        }; break;
                    case 'select[name=StartDate] option:selected':
                        return {
                            val: function () { return 'teststartdate'; }
                        }; break;
                    case 'select[name=EndDate] option:selected':
                        return {
                            val: function () { return 'testenddate'; }
                        }; break;
                    case 'input:checkbox[name=VehicleIds]:checked':
                        return {
                            each: function (fn) { return { value: 123 }; }
                        }; break;
                    case '#GetChartImageUrl':
                        return {
                            val: function () { return 'testchartimageurl'; }
                        }; break;
                    case '#chartimage':
                        return chartImage; break;

                    default: result = 'unknown';
                }

            }
        };

        module.postrender({}, mockView);

        clickEventHandler();
        submitEventHandler(testSubmitEvent);
        ok(testSubmitEventCancelled, 'submit event not cancelled.');

    });

} (window.specs = window.specs || {}, window.mstats));