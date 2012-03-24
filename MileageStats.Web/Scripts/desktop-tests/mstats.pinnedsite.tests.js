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

    module('MileageStats Pinned Site Widget Tests', {
        setup: function () {
            this.savedAjax = $.ajax;
            this.savedDocumentMode = document.documentMode;
            this.savedIsPinned = mstats.pinnedSite.isPinned;

            $.ajax = function () { };
            document.documentMode = 9;
            // Unable to set window.external.msIsSiteMode (built into browser), so we override isPinned
            mstats.pinnedSite.isPinned = function () { 
                return true; 
            };

            $('#qunit-fixture').append(
                '<div>' +
                    '<div id="pinnedSiteImage" />' +
                    '<div id="pinnedSiteCallout" style="display:none" />' +
                    '</div>'
            );
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            document.documentMode = this.savedDocumentMode;
            mstats.pinnedSite.isPinned = this.savedIsPinned;
        }
    });

    test('when document undefined, then fails silently', function () {
        expect(1);

        document.documentMode = undefined;

        mstats.pinnedSite.initializePinnedSiteImage();

        ok(true, 'pinnedSite.initialized failed silently');
    });

    test('when pinned sites not enabled, then fails silently', function () {
        expect(1);       

        mstats.pinnedSite.isPinned = function () { return false; };

        mstats.pinnedSite.initializePinnedSiteImage();

        ok(true, 'pinnedSite.initialized failed silently');
    });

    test('when pinned sites enabled, then mouseover shows pinnedSiteCallout ', function () {
        expect(1);

        mstats.pinnedSite.isPinned = function () { return false; };
        mstats.pinnedSite.initializePinnedSiteImage();

        $('#pinnedSiteImage').simulate('mouseover');

        ok(!$('#pinnedSiteCallout').is(':hidden'), 'pinnedSiteCallout is shown');
    });

    test('when pinned sites enabled, then mousedown hides pinnedSiteCallout ', function () {
        expect(1);

        mstats.pinnedSite.isPinned = function () { return false; };
        mstats.pinnedSite.initializePinnedSiteImage();

        $('#pinnedSiteCallout').show();

        $('#pinnedSiteImage').simulate('mousedown');

        ok($('#pinnedSiteCallout').is(':hidden'), 'pinnedSiteCallout is shown');
    });

    test('when pinned sites enabled, then mouseout hides pinnedSiteCallout ', function () {
        expect(1);

        mstats.pinnedSite.isPinned = function () { return false; };
        mstats.pinnedSite.initializePinnedSiteImage();

        $('#pinnedSiteCallout').show();

        $('#pinnedSiteImage').simulate('mouseout');

        ok($('#pinnedSiteCallout').is(':hidden'), 'pinnedSiteCallout is shown');
    });

    test('when pinned sites enabled, then ajax method called ', function () {
        expect(1);

        var sendRequest = function (options) {
            equal(options.url, '/reminder/overduelist/', 'Ajax URL is for overdue list of reminders');
        };

        mstats.pinnedSite.intializeData(sendRequest);
    });

    // Note: Difficult to test success handler since window.external is not overridable.

}(jQuery));
