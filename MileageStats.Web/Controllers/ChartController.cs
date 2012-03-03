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

using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Models;
using MileageStats.Web.MobileProfiler;
using MileageStats.Web.Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using MileageStats.Domain.Properties;
using MileageStats.Domain.Handlers;

namespace MileageStats.Web.Controllers
{
    public class ChartController : BaseController
    {
        private const int DESKTOP_CHART_WIDTH = 800;
        private const int DESKTOP_CHART_HEIGHT = 450;
        private const String CHARTS_THEME = @"<Chart> 
<Legends> 
<Legend _Template_=""All"" Docking=""Bottom"" LegendStyle=""Table"" Alignment=""Center""> 
</Legend> 
    </Legends>                           
</Chart>"; 

        private readonly IChartDataService chartDataService;
        private readonly MobileCapabilitiesProvider mobileCapabilitiesProvider;

        public ChartController(GetUserByClaimId getUser, IChartDataService chartDataService, IServiceLocator serviceLocator, MobileCapabilitiesProvider mobileCapabilitiesProvider)
            : base(getUser, serviceLocator)
        {
            this.chartDataService = chartDataService;
            this.mobileCapabilitiesProvider = mobileCapabilitiesProvider;
        }

        [Authorize]
        public ActionResult Index(ChartFormModel chartFormModel)
        {
            chartFormModel.UserId = this.CurrentUser.Id;
                
            return SetupChartFormModel(chartFormModel);
        }

        private ActionResult SetupChartFormModel(ChartFormModel chartFormModel)
        {
            var allCharts = new List<SelectListItem>();
            allCharts.Add(new SelectListItem { Text = @Resources.ChartController_AverageFuelEfficiencyChart_Title, Value = "FuelEfficiency" });
            allCharts.Add(new SelectListItem { Text = @Resources.ChartController_TotalDistance_Title, Value = "TotalDistance" });
            allCharts.Add(new SelectListItem { Text = @Resources.ChartController_TotalCost_Title, Value = "TotalCost" });
            chartFormModel.AllCharts = allCharts;

            chartFormModel.AllVehicleModels = Using<GetVehicleListForUser>()
                .Execute(chartFormModel.UserId).ToArray();

            return View(chartFormModel);
        }

        [Authorize]
        public ActionResult TotalCost()
        {
            return View(new ChartFormModel { UserId = this.CurrentUser.Id });
        }

        [Authorize]
        public ActionResult TotalDistance()
        {
            return View(new ChartFormModel { UserId = this.CurrentUser.Id });
        }

        //
        // Note: This method is intentionally not authorized to support secondary image retrievals from some browsers.
        public ActionResult FuelEfficiencyChart(int id)
        {
            return GetChartImage(new ChartFormModel
                                 {
                                     UserId = id,
                                     ChartName = "FuelEfficiency"
                                 });
        }

        //
        // Note: This method is intentionally not authorized to support secondary image retrievals from some browsers.
        public ActionResult TotalDistanceChart(int id)
        {
            return GetChartImage(new ChartFormModel
                                 {
                                     UserId = id,
                                     ChartName = "TotalDistance"
                                 });
        }

        //
        // Note: This method is intentionally not authorized to support secondary image retrievals from some browsers.
        public ActionResult TotalCostChart(int id)
        {
            return GetChartImage(new ChartFormModel
                                 {
                                     UserId = id,
                                     ChartName = "TotalCost"
                                 });
        }

        public ActionResult GetChartImage(ChartFormModel chartFormModel)
        {
            if (chartFormModel == null) return null;

            byte[] chartBytes = null;

            switch (chartFormModel.ChartName)
            {
                case "FuelEfficiency":
                    chartBytes = GetChartBytes(chartFormModel.UserId, x => x.AverageFuelEfficiency, Resources.ChartController_AverageFuelEfficiencyChart_Title, chartFormModel);
                    break;

                case "TotalDistance":
                    chartBytes = GetChartBytes(chartFormModel.UserId, x => x.TotalDistance, Resources.ChartController_TotalDistance_Title, chartFormModel);
                    break;

                case "TotalCost":
                    chartBytes = GetChartBytes(chartFormModel.UserId, x => x.TotalCost, Resources.ChartController_TotalCost_Title, chartFormModel);
                    break;
            }


            if (chartBytes != null)
            {
                return new FileContentResult(chartBytes, "image/jpeg");
            }

            return new FilePathResult(this.Url.Content("~/Content/trans_pixel.gif"), "image/gif");
        }

        private byte[] GetChartBytes(int userId, Func<StatisticSeriesEntry, double> yValueAccessor, string chartTitle, ChartFormModel chartFormModel)
        {
            Debug.Assert(yValueAccessor != null);

            int chartWidth = DESKTOP_CHART_WIDTH;
            int chartHeight = DESKTOP_CHART_HEIGHT;

            StatisticSeries seriesData = new StatisticSeries();

            if (Request.Browser.IsMobileDevice)
            {
                var capabilities = mobileCapabilitiesProvider.GetBrowserCapabilities(this.Request);
                chartWidth = capabilities.ContainsKey(AllCapabilities.Width)
                                 ? int.Parse(capabilities[AllCapabilities.Width])
                                 : 320;
                //chartHeight = chartWidth*DESKTOP_CHART_HEIGHT/DESKTOP_CHART_WIDTH;

                foreach (var vehicleId in chartFormModel.VehicleIds)
                {
                    var seriesDataForVehicle = this.chartDataService.CalculateSeriesForVehicle(userId, vehicleId, chartFormModel.StartDate, chartFormModel.EndDate);
                    foreach (StatisticSeriesEntry statisticSeriesEntry in seriesDataForVehicle.Entries)
                    {
                        seriesData.Entries.Add(statisticSeriesEntry);
                    }
                }
            }
            else
            {
                seriesData = this.chartDataService.CalculateSeriesForUser(userId, DateTime.UtcNow.AddMonths(-12), null);                
            }
            
            var myChart = new Chart(chartWidth, chartHeight, CHARTS_THEME)
                .AddTitle(chartTitle)
                .AddLegend();

            if (PlotMultipleChartLine(myChart, seriesData.Entries, yValueAccessor))
            {
                return myChart.GetBytes("jpeg");
            }
            
            return null;
        }

        public static bool PlotMultipleChartLine(Chart chart, IEnumerable<StatisticSeriesEntry> seriesData, Func<StatisticSeriesEntry, double> yValueAccessor)
        {
            if (chart == null)
            {
                throw new ArgumentNullException("chart");
            }

            bool isDataPlotted = false;
            if (seriesData != null)
            {
                var entriesGroupedById = seriesData.GroupBy(x => x.Id);

                foreach (var entryGroup in entriesGroupedById)
                {
                    isDataPlotted |= PlotSingleChartLine(chart, entryGroup, yValueAccessor);
                }
            }

            return isDataPlotted;
        }

        public static bool PlotSingleChartLine(Chart chart, IEnumerable<StatisticSeriesEntry> seriesData, Func<StatisticSeriesEntry, double> yValueAccessor)
        {
            if (chart == null)
            {
                throw new ArgumentNullException("chart");
            }

            bool isDataPlotted = false;
            if (seriesData != null && seriesData.Count() > 0)
            {
                var xValues = new List<DateTime>();
                var yValues = new List<double>();

                // I add these as DateTime types as charts need them for proper sorting of the x axis.
                DateTime date = DateTime.UtcNow.Date;
                StatisticSeriesEntry lastEntry = null;
                foreach (var entry in seriesData)
                {
                    date = new DateTime(entry.Year, entry.Month, 1);
                    xValues.Add(date);
                    yValues.Add(yValueAccessor(entry));
                    lastEntry = entry;
                }

                // I add a previous data point when there is only a single one to help the graph draw a line.
                if (xValues.Count == 1)
                {
                    xValues.Insert(0, date.AddMonths(-1));
                    yValues.Insert(0, 0.0);
                }

                chart.AddSeries(chartType: "Line",
                   name: lastEntry.Name,
                   xValue: xValues.ToArray(),
                   yValues: yValues.ToArray());

                isDataPlotted = true;
            }

            return isDataPlotted;
        }
    }
}
