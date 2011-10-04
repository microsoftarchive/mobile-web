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

    module('Add Vehicle Drop Down Selections',
        {
            setup: function () {
                this.savedAjax = $.ajax;

                $('#qunit-fixture').append('<div id="test" />');

                $('#qunit-fixture').append(
                    '<div class="editor article">' +
                        '<form action="/Vehicle/Add" enctype="multipart/form-data" method="post">' +
                            '<div id="vehicleEditForm" class="editor-area" data-makes-url="/Vehicle/MakesForYear" data-models-url="/Vehicle/ModelsForMake">' +
                                '<div class="editor-row">' +
                                    '<label class="editor-label">' +
                                        '<label for="Year">Model Year</label>' +
                                    '</label>' +
                                    '<div class="editor-field">' +
                                        '<select class="editor-textbox" id="Year" name="Year">' +
                                            '<option value="">--Select a Year--</option>' +
                                            '<option>1997</option>' +
                                        '</select>' +
                                    '</div>' +
                                    '<input type="submit" name="UpdateMakes" />' +
                                '</div>' +
                                '<div class="editor-row">' +
                                    '<label class="editor-label">' +
                                        '<label for="MakeName">Make</label>' +
                                    '</label>' +
                                    '<div class="editor-field">' +
                                        '<select class="editor-textbox" id="MakeName" name="MakeName">' +
                                            '<option value="">--Select a Make--</option>' +
                                            '<option>Honda</option>' +
                                        '</select>' +
                                    '</div>' +
                                    '<input type="submit" name="UpdateModels" />' +
                                '</div>' +
                                '<div class="editor-row">' +
                                    '<label class="editor-label">' +
                                        '<label for="ModelName">Model</label>' +
                                    '</label>' +
                                    '<div class="editor-field">' +
                                        '<select class="editor-textbox" id="ModelName" name="ModelName">' +
                                            '<option value="">--Select a Model--</option>' +
                                            '<option>Accord</option>' +
                                        '</select>' +
                                    '</div>' +
                                '</div>' +
                            '</div>' +
                        '</form>' +
                    '</div>'
                );

            },
            teardown: function () {
                $.ajax = this.savedAjax;
            }
        }
    );

    test('When created, then removes update makes button', function () {
        expect(1);
        var monitor = new mstats.VehicleDropDownMonitor();
        monitor.initialize();
        ok($('input[name="UpdateMakes"]').length === 0);
    });

    test('When created, then removes update models button', function () {
        expect(1);
        var monitor = new mstats.VehicleDropDownMonitor();
        monitor.initialize();
        ok($('input[name="UpdateModels"]').length === 0);
    });

    test('When year changes, then requests data from endpoint', function () {
        var monitor, $year, sendRequest;

        expect(1);
        sendRequest = function (settings) {
            equal(settings.url, '/Vehicle/MakesForYear', 'Did not invoke the correct endpoint');
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        $year = $('#Year');
        $year.val($year.find('option:last').val());
        $year.change();
    });

    test('When year changes, then provides year for request', function () {
        var monitor, $year, sendRequest;

        expect(1);

        $year = $('#Year');
        $year.val($year.find('option:last').val());

        sendRequest = function (settings) {
            var data = settings.data;
            equal(data.year, $year.val());
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        $year.change();
    });

    test('After new makes returned on year change, then replaces make values.', function () {
        var monitor,
        sendRequest,
        $year,
        makeArray = ["ManufacturerA", "ManufacturerB", "ManufacturerC"],
        actualMakeValues = [],
        actualMakeText = [],
        actualElementsButFirst,
        totalExpectedTests = (makeArray.length * 2) + 1; // we check two arrays worth of items

        expect(totalExpectedTests);

        // Arrange
        $year = $('#Year');
        $year.val($year.find('option:last').val());

        sendRequest = function (settings) {
            settings.success(makeArray);
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        // Act
        $year.change();

        // Assert
        equal($('#MakeName').children().length, 4);

        actualElementsButFirst = $('#MakeName').children().not(':first');
        actualMakeValues = $.map(actualElementsButFirst, function (item) { return $(item).val() });
        actualMakeText = $.map(actualElementsButFirst, function (item) { return $(item).text() });

        // verify all values provided
        $.each(actualMakeValues, function (n, item) {
            equal(item, makeArray[n]);
        });

        // verify all text provided
        $.each(actualMakeText, function (n, item) {
            equal(item, makeArray[n]);
        });
    });

    test('After new makes returned on year change, then removes all model values.', function () {
        var monitor, $year, sendRequest;

        expect(1);

        $year = $('#Year');
        $year.val($year.find('option:last').val());


        sendRequest = function (settings) {
            settings.success(["ManufacturerA", "ManufacturerB", "ManufacturerC"]);
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        $year.change();

        equal($('#ModelName').children().length, 1);
    });

    test('When make changes, then requests data from endpoint', function () {
        var monitor, $make, sendRequest;

        expect(1);
        sendRequest = function (settings) {
            equal(settings.url, '/Vehicle/ModelsForMake', 'Did not invoke the correct endpoint');
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        $make = $('#MakeName');
        $make.change();
    });

    test('When make changes, then provides year and make on request', function () {
        var monitor, $make, sendRequest;

        expect(2);

        $('#Year').val($('#Year').find('option:last').val());
        $make = $('#MakeName');
        $make.val($make.children().last());

        sendRequest = function (settings) {
            var data = settings.data;
            equal(data.year, $('#Year').val());
            equal(data.make, $('#MakeName').val());
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        $make.change();
    });

    test('When new make selected, then model values updated', function () {
        var monitor, $makeSelect, sendRequest;

        expect(1);

        $('#Year').val($('#Year').find('option:last').val());

        $makeSelect = $('#MakeName');
        $makeSelect.val($makeSelect.children(':last'));

        sendRequest = function (settings) {
            settings.success(["ModelA", "ModelB", "ModelC"]);
        };

        monitor = new mstats.VehicleDropDownMonitor(undefined, sendRequest);
        monitor.initialize();

        $makeSelect.change();

        equal($('#ModelName').children().length, 4);
    });

    test('when make retrieval fails, publish error', function () {

        var monitor,
        sendRequest,
        $year,
        eventType = 'loadError';

        expect(2);

        $year = $('#Year');
        $year.val($year.find('option:last').val());

        sendRequest = function (settings) {
            settings.error(null, "Boom!", "");
        };

        monitor = new mstats.VehicleDropDownMonitor(function (eventName, status) {
            if (status.type === eventType) {
                ok(status, 'status object passed to publisher');
                equal(status.message, 'Could not load vehicle data lists.');
            }
        }, sendRequest);
        monitor.initialize();

        $year.change();

    });

    test('when model retrieval fails, publish error', function () {

        var monitor,
        sendRequest,
        $makeSelect,
        eventType = 'loadError';

        expect(2);

        $makeSelect = $('#MakeName');

        sendRequest = function (settings) {
            settings.error(null, "Boom!", "");
        };

        monitor = new mstats.VehicleDropDownMonitor(function (eventName, status) {
            if (status.type === eventType) {
                ok(status, 'status object passed to publisher');
                equal(status.message, 'Could not load vehicle data lists.');
            }
        }, sendRequest);
        monitor.initialize();

        $makeSelect.change();

    });

    test('when make retrieval failes, then clears make and model selects.', function () {
        var monitor,
        sendRequest,
        $year,
        eventType = 'loadError';

        expect(2);

        $year = $('#Year');

        sendRequest = function (settings) {
            settings.error(null, "Boom!", "");
        };

        monitor = new mstats.VehicleDropDownMonitor(function (eventName, status) { }, sendRequest);
        monitor.initialize();
        $year.change();

        equal($('#MakeName').children().length, 1)
        equal($('#ModelName').children().length, 1)
    });

    test('when make retrieval failes, clears model list', function () {

        var monitor,
        sendRequest,
        $makeSelect,
        eventType = 'loadError';

        expect(1);

        $makeSelect = $('#MakeName');

        sendRequest = function (settings) {
            settings.error(null, "Boom!", "");
        };

        monitor = new mstats.VehicleDropDownMonitor(function (eventName, status) { }, sendRequest);
        monitor.initialize();

        $makeSelect.change();

        equal($('#ModelName').children().length, 1);
    });
} (jQuery));