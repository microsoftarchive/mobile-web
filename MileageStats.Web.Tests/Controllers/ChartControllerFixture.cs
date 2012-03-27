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
using System.Text;
using Xunit;
using MileageStats.Web.Controllers;
using Moq;
using MileageStats.Domain.Handlers;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Models;
using MileageStats.Web.Tests.Mocks;
using MileageStats.Web.Models;
using System.Web.Mvc;
using System.Web;
using System.Web.Hosting;

namespace MileageStats.Web.Tests.Controllers
{
    public class ChartControllerFixture
    {
        private readonly User _defaultUser;

        private readonly Mock<GetUserByClaimId> _userService;
        private readonly Mock<IServiceLocator> _serviceLocator;
        private readonly Mock<IChartDataService> _chartDataService;

        public ChartControllerFixture()
        {
            this._userService = new Mock<GetUserByClaimId>(null);
            this._serviceLocator = new Mock<IServiceLocator>();
            this._chartDataService = new Mock<IChartDataService>();
            this._defaultUser = new User { Id = 5, DisplayName = "test", AuthorizationId = "test" };
        }

        [Fact]
        public void WhenConstructingChartControllerWithNullChartDataService_ThenItThrowsNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ChartController(_userService.Object, null, _serviceLocator.Object));
        }

        [Fact]
        public void WhenRequestingIndex_ThenContentTypeAwareResultIsReturned()
        {
            var handler = new Mock<GetVehicleListForUser>(null, null);
            
            MockHandlerFor(
               () => new Mock<GetVehicleListForUser>(null, null),
               x => x
                        .Setup(h => h.Execute(_defaultUser.UserId))
                        .Returns(new VehicleModel[] { new VehicleModel(new Vehicle
                        {
                            Id = 1
                        }, new VehicleStatisticsModel())}));

            var controller = GetTestableChartController();
            var result = controller.Index(new ChartFormModel());

            Assert.IsType<ContentTypeAwareResult>(result);
            Assert.IsType<ChartFormModel>(((ContentTypeAwareResult)result).Model);
            Assert.True(((ContentTypeAwareResult)result).Model.AllVehicleModels.Length == 1);
        }

        [Fact]
        public void WhenRequestingTotalCost_ThenViewPageIsReturned()
        {
            var controller = GetTestableChartController();
            var result = controller.TotalCost();

            Assert.IsType<ViewResult>(result);
            Assert.IsType<ChartFormModel>(((ViewResult)result).Model);
        }

        [Fact]
        public void WhenRequestingTotalDistance_ThenViewPageIsReturned()
        {
            var controller = GetTestableChartController();
            var result = controller.TotalCost();

            Assert.IsType<ViewResult>(result);
            Assert.IsType<ChartFormModel>(((ViewResult)result).Model);
        }

        [Fact]
        public void WhenRequestingChartImageForFuelEfficiencyInMobile_ThenChartImageIsReturned()
        {
            var browserCapabilities = new Mock<HttpBrowserCapabilitiesBase>();
            browserCapabilities.SetupGet(b => b.IsMobileDevice).Returns(true);

            var controller = GetTestableChartController();
            controller.Request.SetHttpBrowserCapabilities(browserCapabilities.Object);

            var data = new StatisticSeries();
            data.Entries.Add(new StatisticSeriesEntry
            {
                AverageFuelEfficiency = 10,
                Month = 12,
                Name = "test",
                TotalCost = 20,
                TotalDistance = 30,
                Year = 2012
            });
            
            this._chartDataService.Setup(d => d.CalculateSeriesForVehicle(_defaultUser.UserId, 1, 
                It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(data);

            var result = controller.GetChartImage(new ChartFormModel
                {
                    UserId = _defaultUser.UserId,
                    VehicleIds = new int[] { 1 },
                    ChartName = "FuelEfficiency",
                    StartDate = DateTime.Now.Subtract(TimeSpan.FromHours(5)),
                    EndDate = DateTime.Now
                });
            
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public void WhenRequestingChartImageForTotalDistanceInMobile_ThenChartImageIsReturned()
        {
            var browserCapabilities = new Mock<HttpBrowserCapabilitiesBase>();
            browserCapabilities.SetupGet(b => b.IsMobileDevice).Returns(true);

            var controller = GetTestableChartController();
            controller.Request.SetHttpBrowserCapabilities(browserCapabilities.Object);

            var data = new StatisticSeries();
            data.Entries.Add(new StatisticSeriesEntry
            {
                AverageFuelEfficiency = 10,
                Month = 12,
                Name = "test",
                TotalCost = 20,
                TotalDistance = 30,
                Year = 2012
            });

            this._chartDataService.Setup(d => d.CalculateSeriesForVehicle(_defaultUser.UserId, 1,
                It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(data);

            var result = controller.GetChartImage(new ChartFormModel
            {
                UserId = _defaultUser.UserId,
                VehicleIds = new int[] { 1 },
                ChartName = "TotalDistance",
                StartDate = DateTime.Now.Subtract(TimeSpan.FromHours(5)),
                EndDate = DateTime.Now
            });

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public void WhenRequestingChartImageForTotalCostInMobile_ThenChartImageIsReturned()
        {
            var browserCapabilities = new Mock<HttpBrowserCapabilitiesBase>();
            browserCapabilities.SetupGet(b => b.IsMobileDevice).Returns(true);

            var controller = GetTestableChartController();
            controller.Request.SetHttpBrowserCapabilities(browserCapabilities.Object);

            var data = new StatisticSeries();
            data.Entries.Add(new StatisticSeriesEntry
            {
                AverageFuelEfficiency = 10,
                Month = 12,
                Name = "test",
                TotalCost = 20,
                TotalDistance = 30,
                Year = 2012
            });

            this._chartDataService.Setup(d => d.CalculateSeriesForVehicle(_defaultUser.UserId, 1,
                It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(data);

            var result = controller.GetChartImage(new ChartFormModel
            {
                UserId = _defaultUser.UserId,
                VehicleIds = new int[] { 1 },
                ChartName = "TotalCost",
                StartDate = DateTime.Now.Subtract(TimeSpan.FromHours(5)),
                EndDate = DateTime.Now
            });

            Assert.IsType<FileContentResult>(result);
        }

        private ChartController GetTestableChartController()
        {
            _userService.Setup(u => u.Execute("test")).Returns(_defaultUser);
            
            var controller = new ChartController(_userService.Object, _chartDataService.Object, _serviceLocator.Object);
            controller.SetFakeControllerContext();
            controller.SetUserIdentity(new MileageStatsIdentity(_defaultUser.AuthorizationId,
                                                                _defaultUser.DisplayName,
                                                                _defaultUser.UserId));
            
            return controller;
        }

        private Mock<T> MockHandlerFor<T>(Func<Mock<T>> create, Action<Mock<T>> setup = null) where T : class
        {
            return this._serviceLocator.MockHandlerFor(create, setup);
        }
    }
}
