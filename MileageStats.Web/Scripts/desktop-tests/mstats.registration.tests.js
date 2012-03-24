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

    module('MileageStats Registration Widget Tests', {
        setup: function () {
            $('#qunit-fixture').append(
                '<div id="registration">' +
                    '<div id="registration-content">' +
                        '<form action="/Profile/Update" method="post">' +
                            '<input name="__RequestVerificationToken" type="hidden" value="empty"/>' + 
							'<input id="UserId" name="UserId" type="hidden" value="1" />' +
                            '<input id="AuthorizationId" name="AuthorizationId" type="hidden" value="Sample Token" />' +
                            '<input id="DisplayName" name="DisplayName" type="text" value="Sample DisplayName" />' + 
                            '<select id="Country" name="Country">' +
                                '<option value="">-- Select country --</option>' +
                                '<option selected="selected">Sample Country</option>' +
                                 '<option>Other Country</option>' +
                            '</select>' + 
                            '<input id="PostalCode" name="PostalCode" type="text" value="Sample PostalCode" />' + 
                            '<div>' +
                                '<button data-action="profile-save" type="submit">Save</button>' +
                            '</div>' +
                        '</form>' +
                    '</div>' +
                    '</div>'
            );
        }
    });

    test('when widget is attached to the #registration element, then submit is intercepted and calls sendRequest', function () {
        expect(1);

        $('#registration').registration({
            sendRequest: function (options) { 
                ok(true, 'sendRequest called on submit');            
            }
        });
        $('[data-action=profile-save]').first().click();
    });

    test('when widget is attached to the #registration element, then submit is intercepted and calls sendRequest with form data', function () {
        expect(5);

        $('#registration').registration({
            sendRequest: function (options) { 
                equal(options.data.UserId, '1', 'UserId passed to sendRequest');       
                equal(options.data.AuthorizationId, 'Sample Token', 'AuthorizationId passed to sendRequest');       
                equal(options.data.DisplayName, 'Sample DisplayName', 'DisplayName passed to sendRequest');       
                equal(options.data.Country, 'Sample Country', 'Country passed to sendRequest');       
                equal(options.data.PostalCode, 'Sample PostalCode', 'PostalCode passed to sendRequest');       
            }       
        });
        $('[data-action=profile-save]').first().click();
    });

    test('when form submitted, then status set to saving', function () {
        expect(2);
        var eventType = 'saving';

        $('#registration').registration({
            sendRequest: function (options) {
                setTimeout(function () {
                    // just kill some time so the test can finish before we change states
                    options.success({});
                }, 500);
            },
            publish: function (event, status) {
                if (status.type === eventType) {
                    ok(status, 'status object passed to publisher');
                    equal(status.type, eventType, 'status is of type : ' + eventType);
                }
            }
        });

        $('[data-action=profile-save]').first().click();
        forceCompletionOfAllAnimations();
    });

    test('when form submission is successful, then saved is triggered', function () {
        expect(2);
        var eventType = 'saved';
        $('#registration').registration({
            sendRequest: function (options) {
                options.success({});
            },
            publish: function (event, status) {
                if (status.type === eventType) {
                    ok(status, 'status object passed to publisher');
                    equal(status.type, eventType, 'status is of type : ' + eventType);
                }
            }
        });

        $('[data-action=profile-save]').first().click();
        forceCompletionOfAllAnimations();
    });

    test('when sendRequest errors out, then error status set to error', function () {
        expect(2);
        var eventType = 'saveError';

        $('#registration').registration({
            sendRequest: function (options) {
                options.error({});
            },
            publish: function (event, status) {
                if (status.type === eventType) {
                    ok(status, 'status object passed to publisher');
                    equal(status.type, eventType, 'status is of type : ' + eventType);
                }
            }
        });
        $('[data-action=profile-save]').first().click();
        forceCompletionOfAllAnimations();
    });

}(jQuery));