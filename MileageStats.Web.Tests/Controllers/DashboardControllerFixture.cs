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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Models;
using MileageStats.Domain.Handlers;
using Moq;
using Xunit;
using System.Web.Mvc;
using MileageStats.Web.Controllers;
using System.Web.Routing;
using MileageStats.Web.Models;
using MileageStats.Web.Tests.Mocks;
using MileageStats.Domain.Contracts;

namespace MileageStats.Web.Tests.Controllers
{
    public class DashboardControllerFixture
    {
        private const int NoVehicleSelectedId = 0;
        private const int DefaultVehicleId = 99;

        public DashboardControllerFixture()
        {
            serviceLocator = new Mock<IServiceLocator>();

            defaultUser = new User { AuthorizationId = "TestClaimsIdentifier", UserId = 5 };
            defaultUserInfo = new UserInfo
                                  {
                                      ClaimsIdentifier = defaultUser.AuthorizationId,
                                      UserId = defaultUser.UserId
                                  };

            userServicesMock = new Mock<GetUserByClaimId>(null);
            userServicesMock
                .Setup(s => s.Execute(defaultUser.AuthorizationId))
                .Returns(defaultUser);

            countryServicesMock = new Mock<GetCountries>(null);
            countryServicesMock
                .Setup(s => s.Execute())
                .Returns(new ReadOnlyCollection<string>(new List<string>(new[]{"country1", "country2"})));

            this.chartDataService = new Mock<IChartDataService>();
        }

        private Mock<GetUserByClaimId> userServicesMock { get; set; }
        private Mock<GetCountries> countryServicesMock { get; set; }
        private Mock<IServiceLocator> serviceLocator { get; set; }
        private UserInfo defaultUserInfo { get; set; }
        private User defaultUser { get; set; }
        private Mock<IChartDataService> chartDataService { get; set; }

        [Fact]
        public void WhenRequestingDashboard_ThenInvokeHandlers()
        {
            var list = MockHandlerFor(
                () => new Mock<GetVehicleListForUser>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(new VehicleModel[] { })
                            .Verifiable("handler wasn't invoked.")
                );

            var reminders = MockHandlerFor(
                () => new Mock<GetImminentRemindersForUser>(null, null, null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId, It.IsAny<DateTime>(), NoVehicleSelectedId))
                            .Verifiable("handler wasn't invoked.")
                );

            var statistics = MockHandlerFor(
                () => new Mock<GetFleetSummaryStatistics>(null, null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Verifiable("handler wasn't invoked.")
                );

            var controller = GetTestableDashboardController();
            var context = controller.MockRequestForMediaType("text/html");

            var proxy = (ITestableContentTypeAwareResult)controller.Index();
            var result = proxy.GetActionResultFor(context.Object);

            Assert.IsType<ViewResult>(result);

            list.Verify();
            reminders.Verify();
            statistics.Verify();
        }

         [Fact]
        public void WhenRequestingDashboard_ThenViewModelContainsImminentReminders()
        {
            var vehicle1 = new Vehicle { VehicleId = 1 };
            var vehicle2 = new Vehicle { VehicleId = 2 };

            var reminders = new[]
                                {
                                    new ImminentReminderModel(vehicle1, new Reminder {ReminderId = 45}, false),
                                    new ImminentReminderModel(vehicle2, new Reminder {ReminderId = 67}, true),
                                };

            MockHandlerFor(
                () => new Mock<GetImminentRemindersForUser>(null, null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId, It.IsAny<DateTime>(), NoVehicleSelectedId))
                            .Returns(reminders)
                );

            MockHandlerFor(
                () => new Mock<GetVehicleListForUser>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(new VehicleModel[]{})
                );

            MockHandlerFor(
                () => new Mock<GetFleetSummaryStatistics>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                );

            var controller = GetTestableDashboardController();

            var result = controller.Index();
            var model = result.Extract<DashboardViewModel>();

            Assert.Equal(2, model.ImminentReminders.Count());

            ImminentReminderModel[] list = model.ImminentReminders.ToArray();

            Assert.Equal(45, list[0].Reminder.ReminderId);
            Assert.Equal(67, list[1].Reminder.ReminderId);
        }

       

        [Fact]
        public void WhenRequestingDashboard_ThenViewModelContainsVehiclesList()
        {
            var vehicleList = new VehicleModel[] { };

            MockHandlerFor(
                () => new Mock<GetVehicleListForUser>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(vehicleList)
                );

            MockHandlerFor(
                () => new Mock<GetImminentRemindersForUser>(null, null, null),
                mock => mock.Setup(h => h.Execute(defaultUser.UserId, It.IsAny<DateTime>(), NoVehicleSelectedId))
                );

            MockHandlerFor(
                () => new Mock<GetFleetSummaryStatistics>(null,null),
                mock => mock.Setup(h => h.Execute(defaultUser.UserId))
                );

            var controller = GetTestableDashboardController();

            var result = controller.Index();
            var model = result.Extract<DashboardViewModel>();
            var actual = model.VehicleListViewModel.Vehicles.ToArray();

            Assert.NotNull(actual);
            Assert.False(model.VehicleListViewModel.IsCollapsed);
        }

        [Fact]
        public void WhenRequestingDashboard_ThenViewModelContainsFleetSummaryStatistics()
        {
            var fleetSummaryStatistics = new FleetStatistics(new VehicleStatisticsModel[0]);

            MockHandlerFor(
                () => new Mock<GetFleetSummaryStatistics>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(fleetSummaryStatistics)
                );

            MockHandlerFor(
                () => new Mock<GetVehicleListForUser>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(new VehicleModel[] { })
                );

            MockHandlerFor(
                () => new Mock<GetImminentRemindersForUser>(null, null, null),
                mock => mock.Setup(h => h.Execute(defaultUser.UserId, It.IsAny<DateTime>(), NoVehicleSelectedId))
                );

            var controller = GetTestableDashboardController();

            var result = controller.Index();
            var model = result.Extract<DashboardViewModel>();

            Assert.NotNull(model);
            Assert.NotNull(model.FleetSummaryStatistics);
        }

        [Fact]
        public void WhenRequestingDashboard_ThenReturnsUser()
        {
            MockHandlers();

            var controller = GetTestableDashboardController();
            ActionResult result = controller.Index();
            var model = result.Extract<DashboardViewModel>();

            Assert.NotNull(model);
            Assert.NotNull(model.User);
            Assert.Same(defaultUser, model.User);
        }

        [Fact]
        public void WhenJsonFleetStats_ThenReturnsFleetStatistics()
        {
            var fleetSummaryStatistics = new FleetStatistics(new VehicleStatisticsModel[0]);

            MockHandlerFor(
                () => new Mock<GetFleetSummaryStatistics>(null,null),
                x => x.Setup(h => h.Execute(defaultUser.UserId))
                         .Returns(fleetSummaryStatistics)
                );

            var controller = GetTestableDashboardController();
            JsonResult result = controller.JsonFleetStatistics();

            Assert.NotNull(result);

            Assert.Same(fleetSummaryStatistics, result.Data);
        }

        [Fact]
        public void WhenJsonGetFleetStatisticSeries_ThenReturnsJsonData()
        {
            var controller = GetTestableDashboardController();

            var series = new StatisticSeries();

            this.chartDataService.Setup(x => x.CalculateSeriesForUser(defaultUserInfo.UserId, null, null)).Returns(
                series);

            ActionResult actual = controller.JsonGetFleetStatisticSeries();

            var actualJsonResult = actual as JsonResult;
            Assert.NotNull(actualJsonResult);
            Assert.Same(series, actualJsonResult.Data);
        }

        private Mock<T> MockHandlerFor<T>(Func<Mock<T>> create, Action<Mock<T>> setup = null) where T : class
        {
            return serviceLocator.MockHandlerFor(create, setup);
        }

        // returns controller with mocks
        private TestableDashboardController GetTestableDashboardController()
        {
            var c = new TestableDashboardController(userServicesMock.Object, countryServicesMock.Object, serviceLocator.Object, chartDataService.Object);
            c.SetFakeControllerContext();
            c.SetUserIdentity(new MileageStatsIdentity(defaultUserInfo.ClaimsIdentifier,
                                                       defaultUserInfo.DisplayName,
                                                       defaultUserInfo.UserId));

            c.InvokeInitialize(c.ControllerContext.RequestContext);

            return c;
        }

        private void MockHandlers()
        {
            MockStatistics();

            MockVehicleList();

            MockImminentReminders();
        }

        private void MockVehicleList()
        {
            var vm = new[] { new VehicleModel(new Vehicle(), new VehicleStatisticsModel()) };

            MockHandlerFor(
                () => new Mock<GetVehicleListForUser>(null, null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(vm)
                );
        }

        private void MockImminentReminders()
        {
            MockHandlerFor(
                () => new Mock<GetImminentRemindersForUser>(null, null, null),
                mock => mock.Setup(h => h.Execute(defaultUser.UserId, It.IsAny<DateTime>(), NoVehicleSelectedId))
                );
        }

        private void MockStatistics()
        {
            var fleetSummaryStatistics = new FleetStatistics(new VehicleStatisticsModel[0]);

            MockHandlerFor(
                () => new Mock<GetFleetSummaryStatistics>(null,null),
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(fleetSummaryStatistics)
                );
        }
    }

    internal class TestableDashboardController : DashboardController
    {
        public TestableDashboardController(GetUserByClaimId userServices, 
                                           GetCountries getCountries,
                                           IServiceLocator serviceLocator,
                                            IChartDataService chartDataService)
            : base(userServices, getCountries, serviceLocator, chartDataService)
        {
        }

        public void InvokeInitialize(RequestContext context)
        {
            base.Initialize(context);
        }
    
    }
}
