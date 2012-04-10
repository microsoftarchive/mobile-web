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

(function (specs, app) {

    module('charts specs');

    test('charts module constructs itself', function () {
        var module = app.charts(mocks.create());

        ok(module != undefined, true);
        equal(typeof module, 'object');
    });

    test('charts subscribes to button click and searches for form values in the given view', function () {

        var view = $('<div><input type="hidden" id="GetChartImageUrl" value="testchartimageurl" /><select id="ChartName" name="ChartName"><option selected="selected" value="testchartname"></option></select><img id="chartimage" style="display:none" /><input name="VehicleIds" type="checkbox" value="1" checked></input><input name="VehicleIds" type="checkbox" value="2" checked></input><select id="StartDate" name="StartDate"><option selected="selected" value="teststartdate"></option></select><select id="EndDate" name="EndDate"><option value="testenddate"></option></select><button id="ChartRefreshButton" type="submit"></button></div>');
        var module = app.charts(mocks.create());

        module.postrender({}, view);

        view.find('#ChartRefreshButton').trigger('click');
        var chartSource = view.find('#chartimage').attr("src");
        equal(0, chartSource.indexOf('testchartimageurl'), 'Expect testchartimageurl to be at index zero of src attrib value');
        ok(chartSource.indexOf('&ChartName=testchartname') > 0, 'Expect ChartName constraint');
        ok(chartSource.indexOf('&StartDate=teststartdate') > 0, 'Expect StartDate constraint');
        ok(chartSource.indexOf('&EndDate=testenddate') > 0, 'Expect EndDate constraint');
        ok(chartSource.indexOf('&VehicleIds=1') > 0, 'Expect Vehicle 1');
        ok(chartSource.indexOf('&VehicleIds=2') > 0, 'Expect Vehicle 2');
        ok(chartSource.indexOf('&Positions=0') > 0, 'Expect First Position');
        ok(chartSource.indexOf('&Positions=1') > 0, 'Expect Second Position');

        var chartStyle = view.find('#chartimage').attr("style");
        equal(chartStyle, undefined);

    });

    test('charts pass correct position of checked vehicles', function () {

        var view = $('<div><input type="hidden" id="GetChartImageUrl" value="testchartimageurl" /><select id="ChartName" name="ChartName"><option selected="selected" value="testchartname"></option></select><img id="chartimage" style="display:none" /><input name="VehicleIds" type="checkbox" value="VehicleId1" checked></input><input name="VehicleIds" type="checkbox" value="VehicleId2" ></input><input name="VehicleIds" type="checkbox" value="VehicleId3" checked></input><select id="StartDate" name="StartDate"><option selected="selected" value="teststartdate"></option></select><select id="EndDate" name="EndDate"><option value="testenddate"></option></select><button id="ChartRefreshButton" type="submit"></button></div>');
        var module = app.charts(mocks.create());

        module.postrender({}, view);

        view.find('#ChartRefreshButton').trigger('click');
        var chartSource = view.find('#chartimage').attr("src");
        equal(0, chartSource.indexOf('testchartimageurl'), 'Expect testchartimageurl to be at index zero of src attrib value');
        ok(chartSource.indexOf('&ChartName=testchartname') > 0, 'Expect ChartName constraint');
        ok(chartSource.indexOf('&StartDate=teststartdate') > 0, 'Expect StartDate constraint');
        ok(chartSource.indexOf('&EndDate=testenddate') > 0, 'Expect EndDate constraint');
        ok(chartSource.indexOf('&VehicleIds=VehicleId1') > 0, 'Expect Vehicle 1');
        ok(chartSource.indexOf('&VehicleIds=VehicleId2') < 0, 'Do Not Expect Vehicle 2');
        ok(chartSource.indexOf('&VehicleIds=VehicleId3') > 0, 'Expect Vehicle 3');
        ok(chartSource.indexOf('&Positions=0') > 0, 'Expect First Position');
        ok(chartSource.indexOf('&Positions=1') < 0, 'Do Not Second Position');
        ok(chartSource.indexOf('&Positions=2') > 0, 'Expect Third Position');

    });

    test('charts set default values', function () {

        var view = $('<div><img id="chartimage" style="display:none" /><input name="VehicleIds" type="checkbox" value="1"></input><input name="VehicleIds" type="checkbox" value="2"></input></input><button id="ChartRefreshButton" type="submit"></button></div>');
        var module = app.charts(mocks.create());

        module.postrender({}, view);

        view.find('#ChartRefreshButton').trigger('click');
        var chartSource = view.find('#chartimage').attr("src");
        ok(chartSource.indexOf('&VehicleIds=1') > 0, 'Expect Vehicle 1');
        ok(chartSource.indexOf('&VehicleIds=2') < 0, 'Do Not Expect Vehicle 2');

    });

} (window.specs = window.specs || {}, window.mstats));