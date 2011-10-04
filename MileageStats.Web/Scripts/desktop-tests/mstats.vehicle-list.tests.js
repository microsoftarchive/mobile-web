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

    module('MileageStats Vehicle List Widget', {
        setup: function () {
            this.savedAjax = $.ajax;
            $('#qunit-fixture').append('<div id="vehicles" class="article" data-list-url="/test/get/list/">' +
                '<div id="vehicle-list-content">' +
                     '<div class="framed command section">' +
                        '<div>' +
                            '<a href="" class="framed command" data-action="vehicle-add-selected">+ Add Vehicle</a>' +
                        '</div>' +
                    '</div>' +
                    '<div class="wrapper">' +
                        '<div class="vehicle" data-vehicle-id="4">' +
                            '<div class="content">' +
                                '<a data-action="vehicle-details-selected" href="/Vehicle/Details/4">' +
                                    '<div class="header">' +
                                        '<div class="overlay"></div>' +
                                        '<div class="data-model">' +
                                            '<span class="year">2002</span>' +
                                            '<span class="make">Wrangler</span>' +
                                            '<span class="model">Jeep</span>' +
                                        '</div>' +
                                        '<div class="data-name" data-field="vehicle-name">Mud Lover</div>' +
                                        '<div class="glass"></div>' +
                                    '</div>' +
                                '</a>' +
                                '<div class="actions">' +
                                    '<a class="avatar" data-action="vehicle-details-selected">' +
                                        '<img src="/Vehicle/Photo/" alt="Default Vehicle Photo" />' +
                                    '</a><div class="nav">' +
                                        '<a href="/Vehicle/Details/4" data-action="vehicle-details-selected" alt="details">' +
                                            '<div class="hover"></div>' +
                                            '<div class="active"></div>' +
                                            '<img alt="Details" src="/command-details.png")" />' +
                                            '<div class="glass"></div>' +
                                        '</a>' +
                                        '<a href="/Fillup/List/4" data-action="vehicle-fillups-selected" alt="fillups">' +
                                            '<div class="hover"></div>' +
                                            '<div class="active"></div>' +
                                            '<img alt="Fill ups" src="/command-fillups.png")" />' +
                                            '<div class="glass"></div>' +
                                        '</a>' +
                                        '<a href="/Reminder/List/4" data-action="vehicle-reminders-selected" alt="reminders">' +
                                            '<div class="hover"></div>' +
                                            '<div class="active"></div>' +
                                            '<img alt="Reminders" src="/command-reminders.png")" />' +
                                            '<div class="glass"></div>' +
                                        '</a></div></div>' +
                                '<div class="statistics footer">' +
                                    '<div class="statistic mile-per-gallon">' +
                                        '<div class="quantity">27</div><div class="unit">mpg</div>' +
                                    '</div>' +
                                    '<div class="statistic cost-per-mile">' +
                                        '<div class="quantity">15&cent;</div><div class="unit">per mile</div>' +
                                    '</div>' +
                                    '<div class="statistic cost-per-month">' +
                                        '<div class="quantity">$34</div><div class="unit">per month</div>' +
                                    '</div></div></div></div></div></div></div>' +
                                    '<script id="testTemplate" type="text/x-jquery-tmpl"><p>contents go here</p></script>');
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $('#vehicles').vehicleList('destroy');
        }
    });
    /****************************************************************
    * Navigation tests
    ****************************************************************/

    test('when widget is attached to the .mstats-vehicle-list element, then the list is made sortable', function () {
        expect(3);
        
        var widget = $('#vehicles').vehicleList();

        ok(widget.sortable('option', 'items') === ':mstats-tile:has(:mstats-vehicle)', 'the items the list sorts was scoped properly');
        ok(widget.sortable('option', 'containment') === '#vehicles', 'the sorting containment was set properly');
        ok(widget.sortable('option', 'handle') === '.header', 'the handle was set properly');
    });

    test('when moveOnScreen is called, then visible is set to true', function () {
        expect(1);
        
        var widget = $('#vehicles').vehicleList();
        
        widget.vehicleList('moveOnScreen');
        
        setTimeout(function () {
            equal(widget.vehicleList('option', 'visible'), true, 'visible is true');
            start();
        }, 1200);
        stop();
    });

    test('when moveOffScreen is called, then is moved', function () {
        expect(1);
        
        var widget = $('#vehicles').vehicleList();
        
        widget.vehicleList('moveOffScreen');
        
        setTimeout(function () {
            equal(widget.css('left'), '-600px', 'list is positioned correctly');
            start();
        }, 1200);
        stop();
    });

    test('when moveOffScreen is called, then visible is set to false', function () {
        expect(1);
        
        var list = $('#vehicles').vehicleList();
        
        list.vehicleList('moveOffScreen');
        
        setTimeout(function () {
            equal(list.vehicleList('option', 'visible'), false, 'visible is false');
            start();
        }, 1200);
        stop();
    });

    /****************************************************************
    * Go to Details Layout Tests
    ****************************************************************/
    test('when layout is set to details, then compact class is added', function () {
        expect(2);
        
        var widget = $('#vehicles').vehicleList();
        ok(!widget.hasClass('compact'), 'correctly defaults to expanded mode');
        
        widget.vehicleList('option', 'layout', 'details');
        ok(widget.hasClass('compact'), 'correctly set class to compact');
    });

    test('when layout is set to details, then sortable axis is set to vertical (y axis) only', function () {
        expect(2);
        
        var widget = $('#vehicles').vehicleList();
        equal(widget.sortable('option', 'axis'), false, 'axis properly defaults to false');
        
        widget.vehicleList('option', 'layout', 'details');
        equal(widget.sortable('option', 'axis'), 'y', 'axis was changed to only y');
    });


    /****************************************************************
    * Go to Dashboard Layout Tests
    ****************************************************************/
    test('when layout is set to dashboard, then compact class is removed', function () {
        expect(2);
        
        var widget = $('#vehicles').vehicleList();
        widget.vehicleList('option', 'layout', 'details');
        
        equal(widget.hasClass('compact'), true, 'actually in compact mode');
        
        widget.vehicleList('option', 'layout', 'dashboard');
        equal(widget.hasClass('compact'), false, 'correctly removed compact class');
    });

    test('when layout is set to dashboard, then sortable axis has no constraints (axis === false)', function () {
        expect(2);

        var widget = $('#vehicles').vehicleList({ layout: 'details' });
        equal(widget.sortable('option', 'axis'), 'y', 'axis is only y in compact mode');

        widget.vehicleList('option', 'layout', 'dashboard');
        equal(widget.sortable('option', 'axis'), false, 'axis was changed to false');
    });

    /****************************************************************
    * Vehicle Selected Tests
    ****************************************************************/
    test('when a vehicle is selected in compact mode during initialization, then the selected vehicle is shown expanded', function () {
        expect(1);
        $('#vehicles').vehicleList({ layout: 'details', selectedVehicleId: 3 });

        equal($('.vehicle[data-vehicle-id = 3 ]').first().hasClass('compact'), false, 'compact class not applied to selected vehicle');
    });

    test('when a vehicle is selected in compact mode, then the selected vehicle is shown expanded', function () {
        expect(1);
        var widget= $('#vehicles').vehicleList();
        widget.vehicleList('option', 'layout', 'details');

        widget.vehicleList('option', 'selectedVehicleId', 3);

        equal($('.vehicle[data-vehicle-id = 3 ]').first().hasClass('compact'), false, 'compact class not applied to selected vehicle');
    });

    /****************************************************************
    * Data Loading Tests
    ****************************************************************/

    test('when loading data errors out, then the widget ensures the vehicles are hidden', function () {
        expect(1);
        $('#vehicles').vehicleList({
            sendRequest: function (options) {
                options.error({});
            },
            templateId: '#testTemplate'
        });

        ok($(':mstats-tile:hidden').length > 0, 'vehicles are hidden');
    });

    test('when widget is attached to the .mstats-vehicle-list element, then widget calls sendRequest with dataUrl', function () {
        expect(1);

        var mockGetData = function (options) {
                equal(options.url, '/test/get/list/', 'Url was passed in');
            };

        // extracted url from data-list-url
        $('#vehicles').vehicleList({
            sendRequest: mockGetData,
            templateId: '#testTemplate'
        });
    });

    test('when refreshData is called, then sendRequest is called', function () {
        expect(1);

        $('#vehicles').vehicleList({
            sendRequest: function (options) {
                options.success({}); 
            },
            templateId: '#testTemplate'
        });

        $('#vehicles').vehicleList('option', 'sendRequest', function () {
            ok(true, 'sendRequest was called properly');
        });

        $('#vehicles').vehicleList('refreshData');
    });

    test('when refreshData is called, then cached data is not invalidated', function () {
        expect(0);

        $('#vehicles').vehicleList({
            sendRequest: function (options) { options.success({}); },
            invalidateData: function () {
                ok(false, 'invalidateData was called properly');
            }
        });

        $('#vehicles').vehicleList('refreshData');
    });

    test('when requeryData is called, then cached data is invalidated', function () {
        expect(1);

        $('#vehicles').vehicleList({
            sendRequest: function (options) { options.success({}); },
            invalidateData: function () {
                ok(true, 'invalidateData was called properly');
            },
            templateId: '#testTemplate'
        });

        $('#vehicles').vehicleList('requeryData');
    });

    test('when requeryData is called, then sendRequest is invoked', function () {
        expect(1);

        $('#vehicles').vehicleList({
            sendRequest: function (options) { options.success({}); },
            invalidateData: function () { },
            templateId: '#testTemplate'
        });

        $('#vehicles').vehicleList('option', 'sendRequest', function () {
            ok(true, 'sendRequest was called properly');
        });

        $('#vehicles').vehicleList('requeryData');
    });
    /****************************************************************
    * Data Templating Tests
    ****************************************************************/
    module('MileageStats Vehicle List Widget Template Tests', {
        setup: function () {
            $('#qunit-fixture').append(
                '<div id="vehicles" class="article"><div id="vehicle-list-content"/></div>' +
                    '<script id="testTemplate" type="text/x-jquery-tmpl"><p>contents go here</p></script>' +
                    '<script id="testTemplate2" type="text/x-jquery-tmpl"><p>other contents</p></script>'
            );
            this.savedAjax = $.ajax;
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $('#vehicles').vehicleList('destroy');
        }
    });

    test('when widget is attached to the .mstats-vehicle-list element and sendRequest not specified, then widget calls $.ajax', function () {
        expect(1);

        $.ajax = function () {
            ok(true, '$.ajax was called');
        };

        $('#vehicles').vehicleList({
            templateId: '#testTemplate'
            });
    });

    test('when widget is attached to the .mstats-vehicle-list element, then widget calls sendRequest', function () {
        expect(1);

        $('#vehicles').vehicleList({
            sendRequest: function (options) {
                ok(true, 'sendRequest was called');
            },
            templateId: '#testTemplate'
        });
    });

    test('when widget data is retrieved, then widget contents are replaced by the template', function () {
        expect(1);

        $('#vehicles').vehicleList({
            sendRequest: function (options) { options.success({}); },
            templateId: '#testTemplate'
        });

        equal($.trim($('#vehicle-list-content').html().toLowerCase()), $.trim($('#testTemplate').html().toLowerCase()), 'Template applied');
    });

    test('when refreshData is called, then template is re-applied', function () {
        expect(2);

        $('#vehicles').vehicleList({
            sendRequest: function (options) { options.success({}); },
            templateId: '#testTemplate'
        });

        equal($.trim($('#vehicle-list-content').html().toLowerCase()), $.trim($('#testTemplate').html().toLowerCase()), 'Template applied');

        $('#vehicles').vehicleList('option', 'templateId', '#testTemplate2');
        $('#vehicles').vehicleList('refreshData');

        equal($.trim($('#vehicle-list-content').html().toLowerCase()), $.trim($('#testTemplate2').html().toLowerCase()), 'Template applied');
    });

    /****************************************************************
    * Status Widget Tests
    ****************************************************************/
    test('when loading data is complete, then the vehicles are shown', function () {
        expect(1);
        $('#vehicles').vehicleList({
            sendRequest: function (options) { options.success({}); }
        });

        setTimeout(function () { // this allows the animation to finish
            ok($('.mstats-vehicle-list-content:hidden').length === 0, 'vehicles are shown');
            start();
        }, 2500);

        stop();
    });

    test('when loading data errors out, then triggers error status', function () {
        expect(2);
        var eventType = 'loadError';
        $('#vehicles').vehicleList({
            templateId: '#testTemplate',
            sendRequest: function (options) { options.error({}); },
            publish: function (event, status) {
                if (status.type === eventType) {
                    ok(status, 'status object passed to publisher');
                    equal(status.type, eventType, 'status is of type : ' + eventType);
                }
            }
        });
    });

    /****************************************************************
    * Vehicle Sorting Tests
    ****************************************************************/
    module('MileageStats Vehicle List Widget - Sorting Vehicles', {
        setup: function () {
            this.savedAjax = $.ajax;
            $('#qunit-fixture').append(
                '<div id="vehicles" class="article" data-sort-url="/Vehicle/UpdateSortOrder">' +
                    '<div id="vehicle-list-content">' +
                        '<div class="wrapper"><div data-vehicle-id="1" class="vehicle framed section" /></div>' +
                        '<div class="wrapper"><div data-vehicle-id="2" class="vehicle framed section" /></div>' +
                        '<div class="wrapper"><div data-vehicle-id="3" class="vehicle framed section" /></div>' +
                        '<div class="wrapper"><div data-vehicle-id="4" class="vehicle framed section" /></div>' +
                        '<div class="framed command section">' +
                            '<div>' +
                                '<a data-action="vehicle-add-selected" href="/Vehicle/Create">+ Add Vehicle</a>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                    '</div>'
            );
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $('#vehicles').vehicleList('destroy');
        }
    });

    test('when vehicles sorted then UpdateSortOrder called', function () {
        expect(1);

        var v = $('#vehicles').vehicleList({
                sendRequest: function () {
                    ok(true, 'UpdateSortOrder was called.');
                }
            });

        v.vehicleList('saveSortOrder');
    });

    test('when vehicles sorted then UpdateSortOrder called with sort order parameter', function () {
        expect(1);

        var v = $('#vehicles').vehicleList({
            sendRequest: function (options) {
                equal(options.data.SortOrder, "1,2,3,4");
            }
        });

        v.vehicleList('saveSortOrder');
    });

    test('when vehicles sorted then saving status is triggered', function () {
        expect(2);

        $.ajax = function (options) { };

        var eventType = 'saving',
            v = $('#vehicles').vehicleList({
                publish: function (event, status) {
                    if (status.type === eventType) {
                        ok(status, 'status object passed to publisher');
                        equal(status.type, eventType, 'status is of type : ' + eventType);
                    }
                }
            });

        v.vehicleList('saveSortOrder');
    });

    test('when vehicles sorted successfully, then the saved status is triggered', function () {
        expect(2);

        $.ajax = function (options) {
            if (options.url === '/Vehicle/UpdateSortOrder') {
                options.success();
            }
        };

        var eventType = 'saved',
            v = $('#vehicles').vehicleList({
                publish: function (event, status) {
                    if (status.type === eventType) {
                        ok(status, 'status object passed to publisher');
                        equal(status.type, eventType, 'status is of type : ' + eventType);
                    }
                }
            });

        v.vehicleList('saveSortOrder');
    });

    test('when vehicles sorted fails, then the error status is triggered', function () {
        expect(2);

        $.ajax = function (options) {
            if (options.url === '/Vehicle/UpdateSortOrder') {
                options.error();
            }
        };

        var eventType = 'saveError',
            v = $('#vehicles').vehicleList({
                publish: function (event, status) {
                    if (status.type === eventType) {
                        ok(status, 'status object passed to publisher');
                        equal(status.type, eventType, 'status is of type : ' + eventType);
                    }
                }
            });

        v.vehicleList('saveSortOrder');
    });


    module('Vehicle List Widget - Animation', {
        setup: function () {
            this.savedAjax = $.ajax;
            $('#qunit-fixture').append(
                '<div id="vehicles" class="article">' +
                    '<div id="vehicle-list-content">' +
                        '<div class="wrapper"><div data-vehicle-id="1" class="vehicle framed section" /></div>' +
                        '<div class="wrapper"><div data-vehicle-id="2" class="vehicle framed section" /></div>' +
                        '<div class="wrapper"><div data-vehicle-id="3" class="vehicle framed section" /></div>' +
                        '<div class="wrapper"><div data-vehicle-id="4" class="vehicle framed section" /></div>' +
                        '<div class="framed command section">' +
                            '<div>' +
                                '<a data-action="vehicle-add-selected" href="/Vehicle/Create">+ Add Vehicle</a>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                    '</div>'
            );
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $('#vehicles').vehicleList('destroy');
        }
    });

    test('when going to Details Layout, then moves all tiles to left', function () {
        expect(5);
            
        var v = $('#vehicles').vehicleList();
        v.vehicleList('goToDetailsLayout');

        setTimeout(function () {
            forceCompletionOfAllAnimations();
            $(':mstats-tile').each(function () {
                equal($(this).css('left'), '0px', 'animated to correct left position');
            });
            start();
        }, 500);
        stop();
    });

    test('when going to Details Layout, then adds compact class', function () {
        expect(1);
        var v = $('#vehicles').vehicleList();
        v.vehicleList('goToDetailsLayout');

        setTimeout(function () {
            ok($('#vehicles').hasClass('compact'), 'added compact class');
            start();
        }, 3000);
        stop();
    });

    test('when going to Dashboard Layout, then removes compact class', function () {
        expect(1);
        var v = $('#vehicles').vehicleList({ layout: 'details' });

        v.vehicleList('goToDashboardLayout');

        setTimeout(function () {
            forceCompletionOfAllAnimations();
            ok(!$('#vehicles').hasClass('compact'), 'removed compact class');
            start();
        }, 2000);
        stop();
    });

    test('when going to Dashboard Layout, then moves all tiles to right', function () {
        expect(5);
        var v = $('#vehicles').vehicleList({ layout: 'details' });

        v.vehicleList('goToDashboardLayout');

        setTimeout(function () {
            forceCompletionOfAllAnimations();
            $(':mstats-tile').each(function () {
                equal($(this).css('left'), '0px', 'animated to correct right position');
            });
            start();
        }, 1500);
        stop();
    });
}(jQuery));
