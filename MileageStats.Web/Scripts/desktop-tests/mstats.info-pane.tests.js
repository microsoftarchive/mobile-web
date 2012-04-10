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
    module('InfoPane Tests', {
        setup: function () {
            $('#qunit-fixture').append('<div id="info"><div/></div>');
        }
    });

    test('when moveOnScreenFromRight is called, then infoPane is shown', function () {
        expect(2);
        var info = $('#info').infoPane();
        info.infoPane('moveOnScreenFromRight');
        setTimeout(function () {
            ok($('#info').not(':hidden'), 'infoPane is visible');
            equal($('#info').css('left'), '260px', 'infoPane is positioned correctly');
            start();
        }, 1500);
        stop();
    });

    test('when moveOnScreenFromRight is called, then visible is set to true', function () {
        expect(1);
        var info = $('#info').infoPane();
        info.infoPane('moveOnScreenFromRight');
        setTimeout(function () {
            equal(info.infoPane('option', 'visible'), true, 'visible is true');
            start();
        }, 1200);
        stop();
    });

    test('when moveOffScreenToRight is called, then infoPane is hidden', function () {
        expect(2);
        var info = $('#info').infoPane({ visible: true });
        info.infoPane('moveOffScreenToRight');
        setTimeout(function () {
            ok(info.is(':hidden'), 'infoPane is hidden');
            equal(info.css('left'), '640px', 'infoPane is positioned correctly');
            start();
        }, 1200);
        stop();
    });

    test('when moveOffScreenToRight is called, then visible is set to false', function () {
        expect(1);
        var info = $('#info').infoPane({ visible: true });
        info.infoPane('moveOffScreenToRight');
        setTimeout(function () {
            equal(info.infoPane('option', 'visible'), false, 'visible is false');
            start();
        }, 1200);
        stop();
    });

    test('when created, then attached details widget', function () {
        expect(1);
        $('#info').infoPane();
        equal($('#details-pane').length, 1, 'vehicle details setup');
    });

    test('when created, then attached fillups widget', function () {
        expect(1);
        $('#info').infoPane();
        equal($('#fillups-pane').length, 1, 'fillups widget is setup');
    });

    test('when created, then attached reminders widget', function () {
        expect(1);
        $('#info').infoPane();
        equal($('#reminders-pane').length, 1, 'reminders widget is setup');
    });

    test('when created, then defaults to details as the activePane', function () {
        expect(1);
        var info = $('#info').infoPane();
        equal(info.infoPane('option', 'activePane'), 'details', 'defaults to details');
    });

} (jQuery));