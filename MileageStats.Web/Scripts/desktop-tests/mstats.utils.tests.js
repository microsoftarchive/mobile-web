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

   module('MileageStats Utilities -- Url Fixer', {
        setup: function () {
            this._savedRootUrl = mstats.rootUrl;
        },
        teardown: function () {
            mstats.rootUrl = this._savedRootUrl;
        }
    });

    test('When url fixer called with a complex root url and discriminator and undefined root url, then returns url with nothing pre-pended', function () {
        expect(1);

        mstats.rootUrl = undefined;

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction/value');

        equal(cleanUrl, '/testController/testAction/value', 'Url set properly');
    });

    test('When url fixer called without a root url, then returns url with nothing pre-pended', function () {
        expect(1);

        mstats.rootUrl = '';

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction');

        equal(cleanUrl, '/testController/testAction', 'Url set properly');
    });

    test('When url fixer called with "/" root url and url starts with a "/", then the url is properly formatted', function () {
        expect(1);

        mstats.rootUrl = '/';

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction/value');

        equal(cleanUrl, '/testController/testAction/value', 'Url set properly');
    });

    test('When url fixer called with a root url, then returns url with root pre-pended', function () {
        expect(1);
        mstats.rootUrl = '/SomeFolder';

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction');

        equal(cleanUrl, '/SomeFolder/testController/testAction', 'Url set properly');
    });

    test('When url fixer called with a complex root url, then returns url with the root pre-pended', function () {
        expect(1);

        mstats.rootUrl = '/A/B/C';

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction');

        equal(cleanUrl, '/A/B/C/testController/testAction', 'Url set properly');
    });

    test('When url fixer called with a complex root url and discriminator, then returns url with the root pre-pended', function () {
        expect(1);

        mstats.rootUrl = '/A/B/C';

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction/value');

        equal(cleanUrl, '/A/B/C/testController/testAction/value', 'Url set properly');
    });

    test('When url fixer called with "/" root url and url that does not start with a "/", then the url is properly formatted', function () {
        expect(1);
        mstats.rootUrl = '/';

        var cleanUrl = mstats.getRelativeEndpointUrl('testController/testAction/value');

        equal(cleanUrl, '/testController/testAction/value', 'Url set properly');
    });

    test('When url fixer called with a root url with a "/" at the end and url starts with a "/", then the url is properly formatted', function () {
        expect(1);
        mstats.rootUrl = 'test/';

        var cleanUrl = mstats.getRelativeEndpointUrl('/testController/testAction/value');

        equal(cleanUrl, '/test/testController/testAction/value', 'Url set properly');
    });

    test('When url fixer called with a root url with a "/" at the end and url that does not start with a "/", then the url is properly formatted', function () {
        expect(1);
        mstats.rootUrl = '/test/';

        var cleanUrl = mstats.getRelativeEndpointUrl('testController/testAction/value');

        equal(cleanUrl, '/test/testController/testAction/value', 'Url set properly');
    });

    test('When url fixer called with the root already prefixing the endpoint, just return the endpoint', function () {
        expect(1);
        mstats.rootUrl = '/somesub/';

        var cleanUrl = mstats.getRelativeEndpointUrl('/somesub/testController/testAction/value');

        equal(cleanUrl, '/somesub/testController/testAction/value', 'Url set properly');
    });

    module('MileageStats Utilities -- Make Money');

    test('When makeMoney is called with valid value, then same value returned as string', function () {
        expect(1);

        var value = 10.00,
            returnedValue = mstats.makeMoney(value);

        equal(returnedValue, '10.00', 'Value not changed');
    });

    test('When makeMoney is called with unpadded value, then same returned value is padded', function () {
        expect(1);

        var value = 10,
            returnedValue = mstats.makeMoney(value);

        equal(returnedValue, '10.00', 'Value updated');
    });

    test('When makeMoney is called with one decimal point value, then returned value is padded', function () {
        expect(1);

        var value = 10.1,
            returnedValue = mstats.makeMoney(value);

        equal(returnedValue, '10.10', 'Value updated');
    });

    test('When makeMoney is called with two decimal point value, then returned value is not padded', function () {
        expect(1);

        var value = 10.12,
            returnedValue = mstats.makeMoney(value);

        equal(returnedValue, '10.12', 'Value changed');
    });

    test('When makeMoney is called with three decimal point value, then returned value properly rounded', function () {
        expect(1);

        var returnedValue = mstats.makeMoney(10.110);
        equal(returnedValue, '10.11', 'Value rounded down');
    });

    test('When makeMoney is called with three decimal point value, then returned value properly rounded', function () {
        expect(1);

        var returnedValue = mstats.makeMoney(10.117);
        equal(returnedValue, '10.12', 'Value rounded up');
    });

    module('MileageStats Utilities -- Make MPG Display');

    test('When makeMPGDisplay is called with valid value, then same value returned as string', function () {
        expect(1);

        var value = 10,
            returnedValue = mstats.makeMPGDisplay(value);

        equal(returnedValue, '10', 'Value returned');
    });

    test('When makeMPGDisplay is called with decimal points, then returned value is rounded down', function () {
        expect(1);

        var value = 10.1,
            returnedValue = mstats.makeMPGDisplay(value);

        equal(returnedValue, '10', 'Value rounded');
    });

    test('When makeMPGDisplay is called with decimal points, then returned value is rounded down', function () {
        expect(1);

        var value = 10.7,
            returnedValue = mstats.makeMPGDisplay(value);

        equal(returnedValue, '11', 'Value rounded');
    });

    module('MileageStats Utilities -- Cost to Drive Display');

    test('When makeCostToDriveDisplay is called with valid cents value, then only cents are returned', function () {
        expect(1);

        var value = 0.10,
            returnedValue = mstats.makeCostToDriveDisplay(value);

        equal(returnedValue, '10', 'Value returned');
    });

    test('When makeCostToDriveDisplay is called with decimal cents value, then only rounded cents are returned', function () {
        expect(1);

        var value = 0.101,
            returnedValue = mstats.makeCostToDriveDisplay(value);

        equal(returnedValue, '10', 'Value returned');
    });


    test('When makeCostToDriveDisplay is called with decimal cents value, then only rounded cents are returned', function () {
        expect(1);

        var value = 0.107,
            returnedValue = mstats.makeCostToDriveDisplay(value);

        equal(returnedValue, '11', 'Value returned');
    });

    module('MileageStats Utilities -- Cost Per Month Display');

    test('When makeCostPerMonthDisplay is called with valid value, then only dollars are returned', function () {
        expect(1);

        var value = 11.00,
            returnedValue = mstats.makeCostPerMonthDisplay(value);

        equal(returnedValue, '11', 'Value returned');
    });

    test('When makeCostPerMonthDisplay is called with extra decimals, then only dollars are returned', function () {
        expect(1);

        var value = 11.01,
            returnedValue = mstats.makeCostPerMonthDisplay(value);

        equal(returnedValue, '11', 'Value returned');
    });

    test('When makeCostPerMonthDisplay is called with extra decimals, then only dollars are returned', function () {
        expect(1);

        var value = 11.71,
            returnedValue = mstats.makeCostPerMonthDisplay(value);

        equal(returnedValue, '12', 'Value returned');
    });
}(jQuery));