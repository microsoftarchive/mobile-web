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

// Vehicle Charts Widget
// Provides Average Fuel Efficiency, Total Distance, and Total Cost charts
// All 3 Charts are plotted along with titles.
(function (mstats, $) {

    var cache = {};
    
    if ($.jqplot && $.jqplot.config) {
        $.jqplot.config.enablePlugins = true;
    }
    
    function constructChart(containerId, data) {
         $.jqplot(containerId, data, {
                    axes: {
                        xaxis: {
                            renderer: $.jqplot.DateAxisRenderer,
                            tickInterval: '3 months',
                            pad: 1.5,
                            rendererOptions: {
                                tickRenderer: $.jqplot.CanvasAxisTickRenderer
                            },
                            tickOptions: { formatString: '%m-%y', fontSize: '8pt', fontFamily: 'Tahoma', angle: 0, fontWeight: 'normal', fontStretch: 1 }
                        }
                    },
                    axesDefaults: { useSeriesColor: true },
                    cursor: {
                        show: false,
                        showVerticalLine: false,
                        showHorizontalLine: false,
                        showCursorLegend: false,
                        showTooltip: false,
                        zoom: false,
                        intersectionThreshold: 6
                    },
                    legend: { location: 's', show: false },
                    seriesDefaults: { showMarker: true },
                    series: [{ lineWidth: 2, markerOptions: { style: 'filledCircle', size: 6}}]
                });
    }
    
    $.widget('mstats.vehicleCharts', {
        // default options
        options: {
            // Default to $.ajax when sendRequest is undefined.
            // The extra function indirection allows $.ajax substitution because 
            // the widget framework is using the real $.ajax during options initialization.
            sendRequest: function (ajaxOptions) { $.ajax(ajaxOptions); }
        },

        // The client-side data model for the chart (independency of jqPlot)
        chartData: [],
            
        // Creates the widget
        _create: function () {
            this._fetchChartData();
        },

        // Retrieve the chart data and updates the slider.
        _fetchChartData: function () {
            var that = this,
                chartsTarget = this.element;
            if (chartsTarget) {
                that.options.sendRequest({
                    url: chartsTarget.data('chart-url'),
                    type: "POST",
                    success: function (data) {
                        that._updateChartData(data);
                        that._plotCharts();
                    }
                });
            }
        },

        // Updates the chart data to normalize it into a client-side data model.
        _updateChartData: function (data) {
            if (!data || !data.Entries || !data.Entries.length) {
                return;
            }

            var lastSeriesId = -1,
                vehicleIndex = -1,
                dataPointIndex = 0,
                fuelEfficiencyData = [],
                distanceData = [],
                costData = [],
                numEntries = data.Entries.length,
                entry,
                i,
                month,
                yearAndMonth;

            // Chart data contains 3 sets of vehicle data  (fuel efficiency, distance, and cost)
            // Each set contains a collection of vehicle data.
            // Each vehicle data is a collection of X,Y data points.
            for (i = 0; i < numEntries; i += 1) {
                entry = data.Entries[i];
                // Each time we encounter a new ID, we start a new data set for the vehicle.
                if (lastSeriesId !== entry.Id) {
                    vehicleIndex += 1;
                    dataPointIndex = 0;
                    fuelEfficiencyData[vehicleIndex] = { id: entry.Id, name: entry.Name, series: [] };
                    distanceData[vehicleIndex] = { id: entry.Id, name: entry.Name, series: [] };
                    costData[vehicleIndex] = { id: entry.Id, name: entry.Name, series: [] };
                }

                // Year and Month is the X axis value for each data point.
                // We format to something that the chart can handle for a date-based axis.
                month = entry.Month.toString();
                if (entry.Month < 10) {
                    month = '0' + month;
                }
                yearAndMonth = entry.Year.toString() + '-' + month + '-01';

                // We set each data point per vehicle.
                fuelEfficiencyData[vehicleIndex].series[dataPointIndex] = [yearAndMonth, entry.AverageFuelEfficiency];
                distanceData[vehicleIndex].series[dataPointIndex] = [yearAndMonth, entry.TotalDistance];
                costData[vehicleIndex].series[dataPointIndex] = [yearAndMonth, entry.TotalCost];

                dataPointIndex += 1;
                lastSeriesId = entry.Id;
            }

            this.chartData = [fuelEfficiencyData, distanceData, costData];
        },

        _plotCharts: function () {
            var chartData = this.chartData;
            if (chartData && (chartData.length > 0)) {
                this._plotChart('#vehicle-fuel-efficiency-chart', chartData[0]);
            }
            if (chartData && (chartData.length > 1)) {
                this._plotChart('#vehicle-distance-chart', chartData[1]);
            }
            if (chartData && (chartData.length > 2)) {
                this._plotChart('#vehicle-cost-chart', chartData[2]);
            }
        },
            
        // plots a chart given a target ID, title and a series of client-side data
        _plotChart: function (targetId, data) {
            var target,
                seriesData,
                cacheKey,
                containerId;
            
            if (!data || !data.length) {
                return;
            }

            target = this.element.find(targetId);
            target.empty();
            
            // since generating the chart is an experience operation
            // we cache the DOM elements for the chart
            cacheKey = this.element.data('chart-url') + targetId;
            
            if(cache[cacheKey]) {
                cache[cacheKey].appendTo(target);
            } else {
                containerId = targetId.slice(1) + '-container';
                cache[cacheKey] = $('<div></div>').attr('id', containerId).addClass('display-chart').appendTo(target);
                
                // We convert the client-side data to a jqPlot data format
                seriesData = this._getJQPlotSeries(data);
            
                if (!seriesData || !seriesData.values.length) {
                    return;
                }

                constructChart(containerId, seriesData.values);
            }                
        },

        // Converts to jqPlot data format, and provides vehicle selection and date-range for the data.
        _getJQPlotSeries: function (seriesData) {
            var seriesValues = [],
                numSeries = seriesData.length,
                data,
                index = 0,
                i;

            // In order to pick the right series color when some vehicles are not selected, we track
            // the index of those added vs. the index of the item in the seris.            
            for (i = 0; i < numSeries; i += 1) {
                
                data = seriesData[i].series;
                
                if (data.length !== 0) {
                    // Slice is more start and length than start and end here, so we add one.
                    seriesValues[index] = data;
                    index += 1;
                }
            }

            return { values: seriesValues };
        }

    });
} (this.mstats, jQuery));
