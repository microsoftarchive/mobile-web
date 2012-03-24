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

        expect(8);
        var module = app.charts(mocks.create());
        var testSubmitEvent = { preventDefault: function () { ok(true, "Expect that preventDefault is called."); } };

        var chartImage = { attr: function (attributeName, attributeValue) {
            if (attributeName == 'src') {
                equal(0, attributeValue.indexOf('testchartimageurl'), 'Expect testchartimageurl to be at index zero of src attrib value');
                ok(attributeValue.indexOf('&ChartName=testchartname') > 0, 'Expect ChartName constraint');
                ok(attributeValue.indexOf('&StartDate=teststartdate') > 0, 'Expect StartDate constraint');
                ok(attributeValue.indexOf('&EndDate=testenddate') > 0, 'Expect EndDate constraint');
            } else {
                equal(attributeName, 'style');
                equal(attributeValue, '');
            }
        }
        };
        var mockView = {
            find: function (selector) {
                switch (selector) {
                    case '#ChartRefreshButton':
                        return {
                            click: function (clickEventSubscription) {
                                if (clickEventSubscription) {
                                    clickEventSubscription();
                                } else {
                                    ok(true, 'Expect click() called in postrender.');
                                }
                            }
                        };
                    case 'form':
                        return {
                            submit: function (submitEventSubscription) {
                                submitEventSubscription(testSubmitEvent);
                            }
                        };
                    case 'select[name=ChartName] option:selected':
                        return {
                            val: function () { return 'testchartname'; }
                        };
                    case 'select[name=StartDate] option:selected':
                        return {
                            val: function () { return 'teststartdate'; }
                        };
                    case 'select[name=EndDate] option:selected':
                        return {
                            val: function () { return 'testenddate'; }
                        };
                    case 'input:checkbox[name=VehicleIds]':
                        return {
                            each: function (fn) { return { value: 123 }; }
                        };
                    case '#GetChartImageUrl':
                        return {
                            val: function () { return 'testchartimageurl'; }
                        };
                    case '#chartimage':
                        return chartImage;

                    default: result = 'unknown';
                }

            }
        };

        module.postrender({}, mockView);

    });

} (window.specs = window.specs || {}, window.mstats));