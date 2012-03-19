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
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Controllers;
using MileageStats.Web.Models;
using MileageStats.Web.Tests.Mocks;
using Moq;
using Xunit;
using System.Web;

namespace MileageStats.Web.Tests.Controllers
{
    public class VehicleControllerFixture
    {
        private const int NoVehicleSelectedId = 0;
        private const int DefaultVehicleId = 99;

        public VehicleControllerFixture()
        {
            serviceLocator = new Mock<IServiceLocator>();

            chartDataServiceMock = new Mock<IChartDataService>();

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
        }

        private Mock<GetUserByClaimId> userServicesMock { get; set; }
        private Mock<IChartDataService> chartDataServiceMock { get; set; }
        private Mock<IServiceLocator> serviceLocator { get; set; }
        private UserInfo defaultUserInfo { get; set; }
        private User defaultUser { get; set; }

        [Fact]
        public void WhenUpdatingVehicleSortOrder_InvokesHandler()
        {
            var handler = MockHandlerFor<UpdateVehicleSortOrder>();

            const string newOrder = "3,2,1";
            var sortOrder = new UpdateVehicleSortOrderViewModel { SortOrder = newOrder };

            var controller = GetTestableVehicleController();
            controller.UpdateSortOrder(sortOrder);

            handler.Verify(x => x.Execute(defaultUserInfo.UserId, new[] { 3, 2, 1 }));
        }

        

        [Fact]
        public void WhenJsonList_ThenReturnsVehicles()
        {
            var vehicles = new[]
                               {
                                   new VehicleModel(new Vehicle {Name = "test"}, new VehicleStatisticsModel())
                               };

            MockHandlerFor<GetVehicleListForUser>(
                mock => mock
                            .Setup(h => h.Execute(It.IsAny<int>()))
                            .Returns(vehicles)
                );

            var controller = GetTestableVehicleController();

            JsonResult result = controller.JsonList();
            Assert.IsType<JsonResult>(result);

            var data = (IList<JsonVehicleViewModel>)result.Data;
            Assert.NotNull(data);

            Assert.Equal(vehicles.First().Name, data.First().Name);
        }

        [Fact]
        public void WhenJsonDetailsCalled_ThenReturnsVehicle()
        {
            var vehicle = new Vehicle
                              {
                                  VehicleId = DefaultVehicleId,
                                  Name = "test",
                                  MakeName = "make",
                                  ModelName = "model",
                                  Year = 2010,
                                  PhotoId = 12,
                                  SortOrder = 1
                              };

            MockHandlerFor<GetVehicleById>(
                x => x.Setup(h => h.Execute(It.IsAny<int>(), DefaultVehicleId))
                    .Returns(new VehicleModel(vehicle, new VehicleStatisticsModel())));

            MockHandlerFor<GetOverdueRemindersForVehicle>(
                x => x.Setup(h => h.Execute(DefaultVehicleId, It.IsAny<DateTime>(), 0))
                    .Returns(new List<ReminderSummaryModel> { new ReminderSummaryModel(new Reminder{DueDate = DateTime.UtcNow.AddDays(-1)}) }));

            var controller = GetTestableVehicleController();

            JsonResult result = controller.JsonDetails(DefaultVehicleId);
            var data = (JsonVehicleViewModel)result.Data;

            Assert.Equal(DefaultVehicleId, data.VehicleId);
            Assert.Equal("test", data.Name);
            Assert.Equal("make", data.MakeName);
            Assert.Equal("model", data.ModelName);
            Assert.Equal(2010, data.Year);
            Assert.Equal(12, data.PhotoId);
            Assert.Equal(1, data.SortOrder);
            Assert.NotEmpty(data.OverdueReminders);
        }

        [Fact]
        public void WhenAddActionExecuted_ThenViewModelContainsVehicleFormViewModel()
        {
            MockVehicleList();
            MockDefaultYearMakeModel();

            var controller = GetTestableVehicleController();
            ActionResult result = controller.Add();
            var model = result.Extract<VehicleFormModel>();

            Assert.NotNull(model);
        }

        [Fact]
        public void WhenListPartialActionExecuted_ThenVehicleListSetToCollapsed()
        {
            MockDefaultYearMakeModel();
            MockVehicleList();

            TestableVehicleController controller = GetTestableVehicleController();
            ActionResult result = controller.ListPartial(0, true);
            var model = result.Extract<VehicleListViewModel>();

            Assert.True(model.IsCollapsed);
        }

        [Fact]
        public void WhenAddVehicleActionExecutedWithInvalidVehicle_ViewModelContainsVehicleFormViewModel()
        {
            MockVehicleList();
            MockDefaultYearMakeModel();

            TestableVehicleController controller = GetTestableVehicleController();
            controller.ModelState.AddModelError("bad", "bad");

            ActionResult result = controller.Add(new VehicleFormModel(), null, "Save");
            var model = result.Extract<VehicleFormModel>();

            Assert.NotNull(model);
        }

        [Fact]
        public void WhenAddVehicleActionExecutedWithValidVehicle_ThenRedirectsToVehicleDetails()
        {
            var vehicleForm = new VehicleFormModel();

            MockVehicleList();
            MockDefaultYearMakeModel();

            MockHandlerFor<CanAddVehicle>();
            MockHandlerFor<CreateVehicle>(
                x => x.Setup(h => h.Execute(defaultUser.UserId, vehicleForm, null)));

            var controller = GetTestableVehicleController();

            ActionResult result = controller.Add(vehicleForm, null, "Save");

            Assert.IsAssignableFrom<ContentTypeAwareResult>(result);
        }

        [Fact]
        public void WhenAddVehicleActionExecutedWithValidVehicle_ThenVehicleIsCreated()
        {
            var vehicleForm = new VehicleFormModel();

            MockVehicleList();
            MockDefaultYearMakeModel();

            MockHandlerFor<CanAddVehicle>();
            var handler = MockHandlerFor<CreateVehicle>(
                x => x.Setup(h => h.Execute(defaultUser.UserId, vehicleForm, null))
                    .Verifiable("handler not invoked"));

            var controller = GetTestableVehicleController();
            controller.Add(vehicleForm, null, "Save");

            handler.Verify();
        }

        [Fact]
        public void WhenRequestingVehicleDetails_ThenSetsViewModel()
        {
            var vehicle = new Vehicle { VehicleId = DefaultVehicleId };
            var list = new[] { new VehicleModel(vehicle, new VehicleStatisticsModel()) };

            MockHandlerFor<GetVehicleListForUser>(
                x => x.Setup(h => h.Execute(defaultUser.UserId))
                    .Returns(list));

            MockHandlerFor<GetOverdueRemindersForVehicle>();

            TestableVehicleController controller = GetTestableVehicleController();
            ActionResult result = controller.Details(DefaultVehicleId);

            var model = result.Extract<VehicleDetailsViewModel>();
            Assert.NotNull(model);
        }

        [Fact]
        public void WhenRequestingVehicleDetailsWithoutAValidVehicle_ThenThrows()
        {
            var list = new VehicleModel[] { };

            MockHandlerFor<GetVehicleListForUser>(
                x => x.Setup(h => h.Execute(defaultUser.UserId))
                    .Returns(list));

            MockHandlerFor<GetOverdueRemindersForVehicle>();

            var controller = GetTestableVehicleController();

            Assert.Throws<HttpException>(() => { controller.Details(DefaultVehicleId); });
        }

        [Fact]
        public void WhenRequestingVehicleDetails_ThenSetsVehicleListInViewModel()
        {
            MockHandlerFor<GetVehicleListForUser>(
                x => x.Setup(h => h.Execute(defaultUser.UserId))
                    .Returns(DefaultVehicleId.StandardVehicleList()));

            MockHandlerFor<GetOverdueRemindersForVehicle>();

            var controller = GetTestableVehicleController();
            ActionResult result = controller.Details(DefaultVehicleId);
            var model = result.Extract<VehicleDetailsViewModel>();

            Assert.Equal(3, model.VehicleList.Vehicles.Count());
        }

        [Fact]
        public void WhenRequestingVehicleDetails_ThenSetsVehicleListToCollapsedView()
        {
            MockHandlerFor<GetVehicleListForUser>(
                x => x.Setup(h => h.Execute(defaultUser.UserId))
                    .Returns(DefaultVehicleId.StandardVehicleList()));

            MockHandlerFor<GetOverdueRemindersForVehicle>();

            var controller = GetTestableVehicleController();
            ActionResult result = controller.Details(DefaultVehicleId);
            var model = result.Extract<VehicleDetailsViewModel>();

            Assert.True(model.VehicleList.IsCollapsed);
        }

        [Fact]
        public void WhenRequestingVehicleDetails_ThenSetsSelectedVehicle()
        {
            MockHandlerFor<GetVehicleListForUser>(
                x => x.Setup(h => h.Execute(defaultUser.UserId))
                    .Returns(DefaultVehicleId.StandardVehicleList()));

            MockHandlerFor<GetOverdueRemindersForVehicle>();

            var controller = GetTestableVehicleController();
            ActionResult result = controller.Details(DefaultVehicleId);
            var model = result.Extract<VehicleDetailsViewModel>();

            Assert.Equal(DefaultVehicleId, model.Vehicle.VehicleId);
            Assert.Equal(DefaultVehicleId, model.VehicleList.SelectedVehicle.VehicleId);
        }

        [Fact]
        public void WhenRequestingVehicleDetails_ThenSetsRemindersList()
        {
            const int selectedVehicleId = 99;
            const int odometer = 15000;

            var vehicle = new VehicleModel(
                new Vehicle { VehicleId = selectedVehicleId },
                new VehicleStatisticsModel(0, 0, 0, 0, odometer, 0)
                );

            var reminder = new ReminderSummaryModel(new Reminder());

            MockHandlerFor<GetVehicleListForUser>(
                m => m.Setup(h => h.Execute(defaultUser.UserId))
                    .Returns(new[] { vehicle }));

            var handler = MockHandlerFor<GetOverdueRemindersForVehicle>(
                m => m.Setup(h => h.Execute(selectedVehicleId, It.IsAny<DateTime>(), odometer))
                         .Returns(new[] { reminder })
                         .Verifiable("Did not get overdue reminders.")
                );

            TestableVehicleController controller = GetTestableVehicleController();
            ActionResult result = controller.Details(selectedVehicleId);

            var model = result.Extract<VehicleDetailsViewModel>();

            handler.Verify();
            Assert.NotNull(model.OverdueReminders);
            Assert.Same(reminder, model.OverdueReminders.First());
        }

        [Fact]
        public void WhenEditVehicleActionExecutedWithValidVehicle_ThenRedirectsToDetails()
        {
            MockVehicleList();
            MockDefaultYearMakeModel();
            MockHandlerFor<CanValidateVehicleYearMakeAndModel>();
            MockHandlerFor<UpdateVehicle>();

            var vehicle = new VehicleFormModel { VehicleId = DefaultVehicleId, Name = "test" };
            var form = new FormCollection { { "Save", "true" } };

            TestableVehicleController controller = GetTestableVehicleController();
            ActionResult result = controller.Edit(vehicle, null, "Save");

            Assert.IsType<RedirectToRouteResult>(result);
            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("Details", redirect.RouteValues["action"]);
            Assert.Equal("Vehicle", redirect.RouteValues["controller"]);
        }

        [Fact]
        public void WhenEditGetFormActionExecuted_ThenViewModelContainsVehicleListViewModel()
        {
            MockVehicleListWithVehicles();
            MockDefaultYearMakeModel();

            TestableVehicleController controller = GetTestableVehicleController();

            ActionResult result = controller.Edit(DefaultVehicleId);
            var model = result.Extract<VehicleFormModel>();

            Assert.IsType<VehicleFormModel>(model);
        }

        [Fact]
        public void WhenEditVehicleActionExecutedWithInValidVehicle_ViewModelContainsVehicleListViewModel()
        {
            MockVehicleListWithVehicles();
            MockDefaultYearMakeModel();

            TestableVehicleController controller = GetTestableVehicleController();
            controller.ModelState.AddModelError("bad", "bad");

            var vehicle = new VehicleFormModel { VehicleId = 1, Name = null };
            var form = new FormCollection { { "Save", "true" } };

            ActionResult result = controller.Edit(vehicle, null, "Save");
            var model = result.Extract<VehicleFormModel>();

            Assert.IsType<VehicleFormModel>(model);
        }

        [Fact]
        public void WhenVehicleDeleted_ThenCallsServicesDelete()
        {
            const int vehicleToDelete = 99;

            var handler = MockHandlerFor<DeleteVehicle>(
                mock => mock.Setup(h => h.Execute(defaultUser.UserId, vehicleToDelete))
                    .Verifiable("delete handler was not invoke")
                );

            TestableVehicleController controller = GetTestableVehicleController();

            controller.Delete(vehicleToDelete);

            handler.Verify();
        }

        private Mock<T> MockHandlerFor<T>(Action<Mock<T>> setup = null) where T : class
        {
            return serviceLocator.MockHandlerFor(TestHelpExtensions.Mock<T>, setup);
        }

        private void MockVehicleList()
        {
            var vm = new[] {  new VehicleModel(new Vehicle(), new VehicleStatisticsModel()) };

            MockHandlerFor<GetVehicleListForUser>(
                mock => mock
                            .Setup(h => h.Execute(defaultUser.UserId))
                            .Returns(vm)
                );
        }

        private void MockVehicleListWithVehicles()
        {
            MockHandlerFor<GetVehicleListForUser>(
                x => x.StandardSetup(defaultUser.UserId, DefaultVehicleId));
        }

        private void MockDefaultYearMakeModel()
        {
            MockHandlerFor<GetYearsMakesAndModels>(
                x => x
                         .Setup(h => h.Execute(null, null))
                         .Returns(YearsMakesAndModelsFor.YearWithoutMakes(2010))
                );
        }

        // returns controller with mocks
        private TestableVehicleController GetTestableVehicleController()
        {
            var c = new TestableVehicleController(userServicesMock.Object, serviceLocator.Object);
            c.SetFakeControllerContext();
            c.SetUserIdentity(new MileageStatsIdentity(defaultUserInfo.ClaimsIdentifier,
                                                       defaultUserInfo.DisplayName,
                                                       defaultUserInfo.UserId));

            c.InvokeInitialize(c.ControllerContext.RequestContext);

            return c;
        }
    }

    internal class TestableVehicleController : VehicleController
    {
        public TestableVehicleController(GetUserByClaimId userServices, IServiceLocator serviceLocator)
            : base(userServices, serviceLocator)
        {
        }

        public void InvokeInitialize(RequestContext context)
        {
            base.Initialize(context);
        }
    }
}