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
    module('MileageStats Summary Pane Widget Tests', {
        setup: function () {
            $('#qunit-fixture').append(
                '<div id="summary" class="article framed">' +
                    '<div id="summary-content">' +
                        '<div id="registration">' +
                            '<div id="registration-content"/>' +
                        '</div>' +

                        '<div id="statistics" class="statistics section">' +
                            '<div id="statistics-content"/>' +
                        '</div>' +

                        '<div class="section" id="reminders">' +
                            '<div id="summary-reminders-content"/>' +
                        '</div>' +
                    '</div>' +
                    '</div>'
            );
        },
        teardown: function () {
        }
    });

    test('when moveOffscreen is called, then moves off-screen and hides.', function () {
        expect(2);
        var newOpacity, newLeft;

        var widget = $('#summary').summaryPane();

        widget.summaryPane('moveOffScreen');

        // wait for delay to complete
        setTimeout(function () {
            forceCompletionOfAllAnimations();

            newOpacity = parseInt(widget.css('opacity'), 10);
            newLeft = parseInt(widget.css('left'), 10);

            equal(newOpacity, 0, 'opacity set');
            equal(newLeft, -350, 'moved to position');
            start();
        }, 500);
        stop();
    });

    test('when moveOffscreen is called, then sets visible to false.', function () {
        expect(1);
        var summary = $('#summary').summaryPane();

        summary.summaryPane('moveOffScreen');

        // wait for delay to complete
        setTimeout(function () {
            forceCompletionOfAllAnimations();

            equal(summary.summaryPane('option', 'visible'), false, 'set visible to false');
            start();
        }, 500);
        stop();
    });


    test('when moveOnscreen is called, then shows and moves on-screen.', function () {
        expect(2);
        var newOpacity, newLeft;

        var widget = $('#summary').summaryPane();
        widget.summaryPane('moveOffScreen');
        
        // wait for delay to complete
        setTimeout(function () {
            forceCompletionOfAllAnimations();
        }, 500);

        // move back on screen
       widget.summaryPane('moveOnScreen');

        // wait for delay to complete
        setTimeout(function () {
            forceCompletionOfAllAnimations();

            newOpacity = parseInt(widget.css('opacity'), 10);
            newLeft = widget.css('left');

            equal(newOpacity, 1, 'opacity set');
            equal(newLeft, '0px', 'moved to position');
            start();
        }, 500);
        stop();
    });

    test('when moveOnscreen is called, then sets visible to true.', function () {
        expect(1);
        var summary = $('#summary').summaryPane();

        summary.summaryPane('moveOffScreen');

        // wait for delay to complete
        setTimeout(function () {
            forceCompletionOfAllAnimations();
        }, 500);

        // move back on screen
        summary.summaryPane('moveOnScreen');

        // wait for delay to complete
        setTimeout(function () {
            forceCompletionOfAllAnimations();

            equal(summary.summaryPane('option', 'visible'), true, 'set visible to true');
            start();
        }, 500);
        stop();
    });


    test('when created, then attached registration widget', function () {
        expect(1);
        var widget = $('#summary').summaryPane();
        equal(widget.length, 1, 'registration setup');
    });

    test('when created, then attached fleet statistics widget', function () {
        expect(1);
        $('#summary').summaryPane();
        var statisticsPane = $('#statistics');
        var templateId = statisticsPane.statisticsPane('option','templateId');
        equal(templateId, '#fleet-statistics-template', 'statistics setup');
    });

    test('when created, then attached reminders widget', function () {
        expect(1);
        $('#summary').summaryPane();
        var remindersPane = $('#reminders');
        var templateId = remindersPane.imminentRemindersPane('option','templateId');
        equal(templateId, '#summary-imminent-reminders-template', 'reminders setup');
    });

    test('when created, then defaults to on screen', function () {
        expect(1);
        var summary = $('#summary').summaryPane();
        equal(summary.summaryPane('option', 'visible'), true, 'defaults to off screen');
    });
}(jQuery));