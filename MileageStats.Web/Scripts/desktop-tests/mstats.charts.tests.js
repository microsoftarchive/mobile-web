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

    module('MileageStats Charts Widget Tests', {
        setup: function () {
            this.savedAjax = $.ajax;
            this.savedjqplot = $.jqplot;

            $.jqplot = {
                config: {
                    enablePlugins: true
                }
            };

            $('#qunit-fixture').append(
                '<div id="main-chart" data-chart-url="JsonGetFleetStatisticSeries")">' +
                    '<div>' +
                        '<div id="nav">' +
                            '<a id="fuel-efficiency-link">Average Fuel Efficiency</a> |' +
                            '<a id="distance-link">Total Distance</a> | ' +
                            '<a id="cost-link">Total Cost</a>' +
                        '</div>' +                        
                        '<div id="main-chart-plot" />' +                                            
                        '<div id="vehicle-selection">' +
                            '<div id="vehicle-selection-list">' +
                                '<input type="checkbox" checked="checked" name="1" value="1" />' +
                                '<input type="checkbox" checked="checked" name="2" value="2" />' +
                                '<input type="checkbox" checked="checked" name="3" value="3" />' +
                            '</div>' +
                        '</div>' +
                        '<div id="date-range-selection">' +                            
                            '<div id="slider" /> ' +                                                                  
                            '<div id="label">' +
                                '<div id="lower" />' +                                   
                                '<div id="upper" />' +                                   
                            '</div>'  +
                        '</div>' +                        
                        '<div id="unavailable-message"></div>' +                       
                    '</div>' +
                    '</div>'
            );
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $.jqplot = this.savedjqplot;
            $('#main-chart').charts('destroy');
        }
    });

    test('when widget is attached to the #main-chart element, then prefetches chart data', function () {
        expect(1);

        $('#main-chart').charts({
            sendRequest: function (options) {
                if (options.url === 'JsonGetFleetStatisticSeries') {
                    ok(true, 'sendRequest called with data-chart-url');
                }
            }
        });
    });

    test('when moveOnScreenFromRight is called, then visible is set to true', function () {
        expect(1);
        var chart = $('#main-chart').charts();
        chart.charts('moveOnScreenFromRight');
        setTimeout(function () {
            equal(chart.charts('option', 'visible'), true, 'visible is true');
            start();
        }, 1200);
        stop();
    });

    test('when moveOffScreenToRight is called, then moved to position', function () {
        expect(1);
        var chart = $('#main-chart').charts();
        chart.charts('moveOnScreenFromRight');

        setTimeout(function () {
            chart.charts('moveOffScreenToRight');
            setTimeout(function () {
                equal(chart.css('left'), '500px', 'charts are positioned correctly');
                start();
            }, 1200);
        }, 1200);
        stop();
    });

    test('when moveOffScreenToRight is called, then visible is set to false', function () {
        expect(1);
        var chart = $('#main-chart').charts();
        chart.charts('moveOffScreenToRight');
        setTimeout(function () {
            equal(chart.charts('option', 'visible'), false, 'visible is false');
            start();
        }, 1200);
        stop();
    });

    test('when widget gets chart data, then parses chart data', function () {
        expect(16);

        var jsonData = $.parseJSON('{"Entries":[' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":4,"AverageFuelEfficiency":20,"TotalDistance":310,"TotalCost":58.12},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":5,"AverageFuelEfficiency":22.5,"TotalDistance":360,"TotalCost":60.5},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":6,"AverageFuelEfficiency":18.33,"TotalDistance":220,"TotalCost":43.8},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":7,"AverageFuelEfficiency":18.24,"TotalDistance":310,"TotalCost":58.65},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":8,"AverageFuelEfficiency":20,"TotalDistance":360,"TotalCost":67.5},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":9,"AverageFuelEfficiency":21.21,"TotalDistance":350,"TotalCost":62.18},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":10,"AverageFuelEfficiency":21.38,"TotalDistance":340,"TotalCost":62.46},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":11,"AverageFuelEfficiency":21.8,"TotalDistance":375,"TotalCost":60.8},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":12,"AverageFuelEfficiency":22.78,"TotalDistance":410,"TotalCost":66.7},' +
            '{"Id":1,"Name":"Fast Rod","Year":2011,"Month":1,"AverageFuelEfficiency":21.26,"TotalDistance":270,"TotalCost":47.44},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":4,"AverageFuelEfficiency":22.22,"TotalDistance":310,"TotalCost":52.31},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":5,"AverageFuelEfficiency":25,"TotalDistance":360,"TotalCost":54.5},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":6,"AverageFuelEfficiency":20.37,"TotalDistance":220,"TotalCost":39.42},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":7,"AverageFuelEfficiency":20.26,"TotalDistance":310,"TotalCost":52.78},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":8,"AverageFuelEfficiency":22.22,"TotalDistance":360,"TotalCost":60.75},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":9,"AverageFuelEfficiency":23.57,"TotalDistance":350,"TotalCost":55.99},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":10,"AverageFuelEfficiency":23.76,"TotalDistance":340,"TotalCost":56.26},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":11,"AverageFuelEfficiency":24.22,"TotalDistance":375,"TotalCost":54.78},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":12,"AverageFuelEfficiency":25.31,"TotalDistance":410,"TotalCost":60.13},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2011,"Month":1,"AverageFuelEfficiency":23.62,"TotalDistance":270,"TotalCost":42.74},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":4,"AverageFuelEfficiency":16.67,"TotalDistance":310,"TotalCost":69.75},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":5,"AverageFuelEfficiency":18.75,"TotalDistance":360,"TotalCost":72.5},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":6,"AverageFuelEfficiency":15.28,"TotalDistance":220,"TotalCost":52.56},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":7,"AverageFuelEfficiency":15.2,"TotalDistance":310,"TotalCost":70.38},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":8,"AverageFuelEfficiency":16.67,"TotalDistance":360,"TotalCost":81},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":9,"AverageFuelEfficiency":17.68,"TotalDistance":350,"TotalCost":74.55},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":10,"AverageFuelEfficiency":17.82,"TotalDistance":340,"TotalCost":74.86},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":11,"AverageFuelEfficiency":18.17,"TotalDistance":375,"TotalCost":72.84},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":12,"AverageFuelEfficiency":18.98,"TotalDistance":410,"TotalCost":79.84},' +
            '{"Id":3,"Name":"Mud Lover","Year":2011,"Month":1,"AverageFuelEfficiency":17.72,"TotalDistance":270,"TotalCost":56.84}]}');

        $('#main-chart').charts({
            sendRequest: function (options) {
                options.success(jsonData);

                var chartData = $('#main-chart').charts('getChartData');

                equal(chartData.length, 3, 'Chart data is correct length');

                equal(chartData[0].length, 3, 'Chart data fuel efficiency has correct vehicle length');
                equal(chartData[1].length, 3, 'Chart data distance has correct vehicle length');
                equal(chartData[2].length, 3, 'Chart data cost has correct vehicle length');

                equal(chartData[0][0].name, 'Fast Rod', 'Chart data fuel efficiency vehicle 1 has correct name');
                equal(chartData[1][1].name, 'Soccer Mom\u0027s Ride', 'Chart data distance vehicle 2 has correct name');
                equal(chartData[2][2].name, 'Mud Lover', 'Chart data cost vehicle 3 has correct name');

                equal(chartData[0][0].series.length, 11, 'Chart data fuel efficiency vehicle 1 has correct series length');
                equal(chartData[1][1].series.length, 11, 'Chart data distance vehicle 2 has correct series length');
                equal(chartData[2][2].series.length, 11, 'Chart data cost vehicle 3 has correct series length');

                equal(chartData[0][2].series[4][0], '2010-07-01', 'Chart data fuel efficiency vehicle 3 series[4] has correct year and month');
                equal(chartData[1][1].series[7][0], '2010-10-01', 'Chart data distance vehicle 2 series[7] has correct year and month');
                equal(chartData[2][1].series[5][0], '2010-08-01', 'Chart data cost vehicle 2 series[5] has correct year and month');

                equal(chartData[0][2].series[4][1], 15.2, 'Chart data fuel efficiency vehicle 3 series[4] has correct data');
                equal(chartData[1][1].series[7][1], 340, 'Chart data distance vehicle 2 series[7] has correct data');
                equal(chartData[2][1].series[5][1], 60.75, 'Chart data cost vehicle 2 series[5] has correct data');


            }
        });
    });

    test('when widget gets no chart data, then sets chart data to empty array', function () {
        expect(1);

        $('#main-chart').charts({
            sendRequest: function (options) {
                options.success(undefined);

                var chartData = $('#main-chart').charts('getChartData');

                equal(chartData.length, 0, 'Chart data is correct length');               
            }
        });
    });

    test('when widget gets mstats.events.mainChart.showFuelEfficiency, then plots chart', function () {
        expect(8);

        var jsonData = $.parseJSON('{"Entries":[' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":4,"AverageFuelEfficiency":20,"TotalDistance":310,"TotalCost":58.12},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":5,"AverageFuelEfficiency":22.5,"TotalDistance":360,"TotalCost":60.5},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":4,"AverageFuelEfficiency":22.22,"TotalDistance":310,"TotalCost":52.31},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":5,"AverageFuelEfficiency":25,"TotalDistance":360,"TotalCost":54.5},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":4,"AverageFuelEfficiency":16.67,"TotalDistance":310,"TotalCost":69.75},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":5,"AverageFuelEfficiency":18.75,"TotalDistance":360,"TotalCost":72.5}]}');

        $.jqplot = function (elementId, seriesData, options) {
            equal(elementId, 'mstats-main-chart-container', 'jqplot called with correct element ID');
            equal(seriesData.length, 3, 'jqplot called with correct series data length');

            equal(seriesData[0][2][0], '2010-05-01', 'jqplot called with correct series data');
            equal(seriesData[0][2][1], 22.5, 'jqplot called with correct series data');
            equal(seriesData[1][1][0], '2010-04-01', 'jqplot called with correct series data');
            equal(seriesData[1][1][1], 22.22, 'jqplot called with correct series data');
            equal(seriesData[2][2][0], '2010-05-01', 'jqplot called with correct series data');
            equal(seriesData[2][2][1], 18.75, 'jqplot called with correct series data');            
        };

        $('#main-chart').charts({
            visible: true,
            sendRequest: function (options) {
                options.success(jsonData);
            }
        });

        $('#main-chart #fuel-efficiency-link').simulate('click');        
    });

    test('when widget gets mstats.events.mainChart.showDistance, then plots chart', function () {
        expect(8);

        var jsonData = $.parseJSON('{"Entries":[' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":4,"AverageFuelEfficiency":20,"TotalDistance":310,"TotalCost":58.12},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":5,"AverageFuelEfficiency":22.5,"TotalDistance":360,"TotalCost":60.5},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":4,"AverageFuelEfficiency":22.22,"TotalDistance":310,"TotalCost":52.31},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":5,"AverageFuelEfficiency":25,"TotalDistance":360,"TotalCost":54.5},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":4,"AverageFuelEfficiency":16.67,"TotalDistance":310,"TotalCost":69.75},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":5,"AverageFuelEfficiency":18.75,"TotalDistance":360,"TotalCost":72.5}]}');

        $.jqplot = function (elementId, seriesData, options) {
            equal(elementId, 'mstats-main-chart-container', 'jqplot called with correct element ID');
            equal(seriesData.length, 3, 'jqplot called with correct series data length');

            equal(seriesData[0][2][0], '2010-05-01', 'jqplot called with correct series data');
            equal(seriesData[0][2][1], 360, 'jqplot called with correct series data');
            equal(seriesData[1][1][0], '2010-04-01', 'jqplot called with correct series data');
            equal(seriesData[1][1][1], 310, 'jqplot called with correct series data');
            equal(seriesData[2][2][0], '2010-05-01', 'jqplot called with correct series data');
            equal(seriesData[2][2][1], 360, 'jqplot called with correct series data');            
        };

        var widget = $('#main-chart').charts({
            visible: false,
            sendRequest: function (options) {
                options.success(jsonData);
            }
        });

        widget.charts('option', 'visible', true);

        $('#main-chart #distance-link').simulate('click');        
    });

    test('when widget gets mstats.events.mainChart.showCost, then plots chart', function () {
        expect(8);

        var jsonData = $.parseJSON('{"Entries":[' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":4,"AverageFuelEfficiency":20,"TotalDistance":310,"TotalCost":58.12},' +
            '{"Id":1,"Name":"Fast Rod","Year":2010,"Month":5,"AverageFuelEfficiency":22.5,"TotalDistance":360,"TotalCost":60.5},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":4,"AverageFuelEfficiency":22.22,"TotalDistance":310,"TotalCost":52.31},' +
            '{"Id":2,"Name":"Soccer Mom\u0027s Ride","Year":2010,"Month":5,"AverageFuelEfficiency":25,"TotalDistance":360,"TotalCost":54.5},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":3,"AverageFuelEfficiency":0,"TotalDistance":0,"TotalCost":0},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":4,"AverageFuelEfficiency":16.67,"TotalDistance":310,"TotalCost":69.75},' +
            '{"Id":3,"Name":"Mud Lover","Year":2010,"Month":5,"AverageFuelEfficiency":18.75,"TotalDistance":360,"TotalCost":72.5}]}');

        $.jqplot = function (elementId, seriesData, options) {
            equal(elementId, 'mstats-main-chart-container', 'jqplot called with correct element ID');
            equal(seriesData.length, 3, 'jqplot called with correct series data length');

            equal(seriesData[0][2][0], '2010-05-01', 'jqplot called with correct series data');
            equal(seriesData[0][2][1], 60.5, 'jqplot called with correct series data');
            equal(seriesData[1][1][0], '2010-04-01', 'jqplot called with correct series data');
            equal(seriesData[1][1][1], 52.31, 'jqplot called with correct series data');
            equal(seriesData[2][2][0], '2010-05-01', 'jqplot called with correct series data');
            equal(seriesData[2][2][1], 72.5, 'jqplot called with correct series data');            
        };

        var widget = $('#main-chart').charts({
            visible: false,                
            sendRequest: function (options) {
                options.success(jsonData);
            }
        });
        
        widget.charts('option', 'visible', true);

        $('#main-chart #cost-link').simulate('click');
    });

    test('when refreshData is called, then sendRequest is called', function () {
        expect(1);

        $('#main-chart').charts({
            sendRequest: function (options) { options.success({}); }
        });

        $('#main-chart').charts('option', 'sendRequest', function () {
            ok(true, 'sendRequest was called properly');
        });

        $('#main-chart').charts('refreshData');
    });

    test('when refreshData is called, then cached data is not invalidated', function () {
        expect(0);

        $('#main-chart').charts({
            sendRequest: function (options) { options.success({}); },
            invalidateData: function () {
                ok(false, 'invalidateData was called properly');
            }
        });

        $('#main-chart').charts('refreshData');
    });

    test('when requeryData is called, then cached data is invalidated', function () {
        expect(1);

        $('#main-chart').charts({
            sendRequest: function (options) { options.success({}); },
            invalidateData: function () {
                ok(true, 'invalidateData was called properly');
            }
        });

        $('#main-chart').charts('requeryData');
    });

    test('when requeryData is called, then sendRequest is invoked', function () {
        expect(1);

        $('#main-chart').charts({
            sendRequest: function (options) { options.success({}); },
            invalidateData: function () { }
        });

        $('#main-chart').charts('option', 'sendRequest', function () {
            ok(true, 'sendRequest was called properly');
        });

        $('#main-chart').charts('requeryData');
    });

}(jQuery));
