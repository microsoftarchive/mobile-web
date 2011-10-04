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
using MileageStats.Domain.Handlers;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Web.Models;
using MileageStats.Domain.Contracts;

namespace MileageStats.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly GetCountries getCountries;
        private readonly IChartDataService chartDataService;

        public DashboardController(
            GetUserByClaimId getUser,
            GetCountries getCountries,
            IServiceLocator serviceLocator,
            IChartDataService chartDataService)
            : base(getUser, serviceLocator)
        {
            this.getCountries = getCountries;
            this.chartDataService = chartDataService;
        }

        [Authorize]
        public ActionResult Index()
        {
            var vehicles = Using<GetVehicleListForUser>()
                .Execute(CurrentUserId);

            var imminentReminders = Using<GetImminentRemindersForUser>()
                .Execute(CurrentUserId, DateTime.UtcNow);

            var statistics = Using<GetFleetSummaryStatistics>()
                .Execute(CurrentUserId);

            var model = new DashboardViewModel
            {
                User = CurrentUser,
                VehicleListViewModel = new VehicleListViewModel(vehicles),
                ImminentReminders = imminentReminders,
                FleetSummaryStatistics = statistics
            };

            ViewBag.CountryList = getCountries.GetCountrySelectList();

            return View(model);
        }

        // POST: /Vehicle/JsonFleetStatistics
        [HttpPost]
        [Authorize]
        public JsonResult JsonFleetStatistics()
        {
            var fleetSummaryStatistics = Using<GetFleetSummaryStatistics>().Execute(CurrentUserId);
            return Json(fleetSummaryStatistics);
        }

        [HttpPost]
        [Authorize]
        public ActionResult JsonGetFleetStatisticSeries()
        {
            var series = chartDataService.CalculateSeriesForUser(CurrentUserId, null, null);
            return Json(series);
        }
    }
}
