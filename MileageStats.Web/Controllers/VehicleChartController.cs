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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Handlers;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Models;
using System.Web.Helpers;
using System.Diagnostics;

namespace MileageStats.Web.Controllers
{
    public class VehicleChartController : BaseController
    {
        private readonly IChartDataService chartDataService;

        public VehicleChartController(
            GetUserByClaimId getUser,
            IChartDataService chartDataService,
            IServiceLocator serviceLocator)
            : base(getUser, serviceLocator)
        {
            this.chartDataService = chartDataService;
        }

        //
        // GET: /Vehicle/FuelEfficiencyChart/5/1        
        // Note: This method is intentionally not authorized to support secondary image retrievals from some browsers.
        public ActionResult FuelEfficiencyChart(int userId, int vehicleId)
        {
            byte[] chartBytes = GetVehicleChartBytes(userId, vehicleId, x => x.AverageFuelEfficiency);

            if (chartBytes != null)
            {
                return new FileContentResult(chartBytes, "image/jpeg");
            }
            else
            {
                return new FilePathResult(Url.Content("~/Content/trans_pixel.gif"), "image/gif");
            }
        }

        //
        // GET: /Vehicle/TotalDistanceChart/5/1        
        // Note: This method is intentionally not authorized to support secondary image retrievals from some browsers.
        public ActionResult TotalDistanceChart(int userId, int vehicleId)
        {
            byte[] chartBytes = GetVehicleChartBytes(userId, vehicleId, x => x.TotalDistance);

            if (chartBytes != null)
            {
                return new FileContentResult(chartBytes, "image/jpeg");
            }
            else
            {
                return new FilePathResult(Url.Content("~/Content/trans_pixel.gif"), "image/gif");
            }
        }

        //
        // GET: /Vehicle/TotalCostChart/5/1     
        // Note: This method is intentionally not authorized to support secondary image retrievals from some browsers.
        public ActionResult TotalCostChart(int userId, int vehicleId)
        {
            byte[] chartBytes = GetVehicleChartBytes(userId, vehicleId, x => x.TotalCost);

            if (chartBytes != null)
            {
                return new FileContentResult(chartBytes, "image/jpeg");
            }
            else
            {
                return new FilePathResult(Url.Content("~/Content/trans_pixel.gif"), "image/gif");
            }
        }

        // POST: /Vehicle/JsonAverageFuelEfficiencyChart/1
        [HttpPost]
        [Authorize]
        public JsonResult JsonGetVehicleStatisticSeries(int id)
        {
            StatisticSeries chartData = chartDataService.CalculateSeriesForVehicle(CurrentUserId, id,
                                                                                   DateTime.UtcNow.AddMonths(-12), null);
            return Json(chartData);
        }

        private byte[] GetVehicleChartBytes(int userId, int vehicleId, Func<StatisticSeriesEntry, double> yValueAccessor)
        {
            Debug.Assert(yValueAccessor != null);

            StatisticSeries seriesData = chartDataService.CalculateSeriesForVehicle(userId, vehicleId,
                                                                                    DateTime.UtcNow.AddMonths(-12), null);
            var myChart = new Chart(width: 250, height: 120);

            if (ChartController.PlotSingleChartLine(myChart, seriesData.Entries, yValueAccessor))
            {
                return myChart.GetBytes("jpeg");
            }
            else
            {
                return null;
            }
        }

    }
}
