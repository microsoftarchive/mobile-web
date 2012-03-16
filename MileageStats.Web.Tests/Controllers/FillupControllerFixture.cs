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
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Controllers;
using MileageStats.Web.Models;
using MileageStats.Web.Tests.Mocks;
using Moq;
using Xunit;
using FillupEntry = MileageStats.Domain.Models.FillupEntry;
using Vehicle = MileageStats.Domain.Models.Vehicle;

namespace MileageStats.Web.Tests.Controllers
{
    public class FillupControllerFixture
    {
        private const int NoVehicleSelectedId = 0;
        private readonly Mock<GetUserByClaimId> userServicesMock;
        private readonly UserInfo defaultUserInfo;
        private readonly Mock<IServiceLocator> serviceLocator;

        const int defaultFillupId = 55;
        const int defaultVehicleId = 99;

        public FillupControllerFixture()
        {
            serviceLocator = new Mock<IServiceLocator>();
            userServicesMock = new Mock<GetUserByClaimId>(null);
            defaultUserInfo = new UserInfo { ClaimsIdentifier = "TestClaimsIdentifier", UserId = 5 };
        }

        [Fact]
        public void WhenRequestingFillup_ThenReturnsFillupView()
        {
            var controller = GetTestableFillupController();

            MockFillupForDefaultVehicle();

            MockHandlerFor(() => new Mock<GetVehicleById>(null, null))
                .Setup(h => h.Execute(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new VehicleModel(null, null));
                
            MockHandlerFor(() => new Mock<GetVehicleListForUser>(null,null));

            MockEmptyFillupsForDefaultVehicle();

            var result = controller.Details(defaultVehicleId, defaultFillupId);

            Assert.NotNull(result);
        }

        [Fact]
        public void WhenRequestingFillup_ThenReturnsProvidesViewModelToView()
        {
            MockFillupForDefaultVehicle();

            MockHandlerFor(() => new Mock<GetVehicleListForUser>(null,null));
            MockHandlerFor(() => new Mock<GetVehicleById>(null, null))
                .Setup(h => h.Execute(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new VehicleModel(null, null));

            MockEmptyFillupsForDefaultVehicle();

            var controller = GetTestableFillupController();
            var result = controller.Details(defaultVehicleId, defaultFillupId);
            var model = result.Extract<FillupViewModel>();

            Assert.NotNull(model);
        }

        [Fact]
        public void WhenRequestingFillup_ThenReturnsProvidesFillupsInViewModel()
        {
            var fillupEntries = new[]
                                    {
                                        new FillupEntry {VehicleId = defaultVehicleId},
                                        new FillupEntry {VehicleId = defaultVehicleId}
                                    };

            MockFillupForDefaultVehicle();

            MockHandlerFor(() => new Mock<GetVehicleListForUser>(null,null));

            MockHandlerFor(
                () => new Mock<GetFillupsForVehicle>(null),
                x => x
                         .Setup(h => h.Execute(defaultVehicleId))
                         .Returns(new List<FillupEntry>(fillupEntries)));

            MockHandlerFor(() => new Mock<GetVehicleById>(null, null))
                .Setup(h => h.Execute(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new VehicleModel(null, null));

            var controller = GetTestableFillupController();

            var result = controller.Details(defaultVehicleId, defaultFillupId);
            var model = result.Extract<FillupViewModel>();

            Assert.NotNull(model);
        }

        [Fact]
        public void WhenRequestingVehicleFillups_ReturnsViewWithViewResult()
        {
            MockVehicleListWithVehicles(defaultVehicleId);
            MockFillupsForDefaultVehicle();
            MockHandlerFor(() => new Mock<GetVehicleById>(null, null))
                .Setup(h => h.Execute(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new VehicleModel(null, null));

            var controller = GetTestableFillupController();

            var actual = controller.List(defaultVehicleId);
            var model = actual.Extract<List<FillupListViewModel>>();

            Assert.NotNull(actual);
            Assert.NotNull(model);
        }

        [Fact]
        public void WhenRequestingVehicleFillups_ReturnsViewWithPopulatedViewModel()
        {
            MockVehicleListWithVehicles(defaultVehicleId);
            MockFillupsForDefaultVehicle();
            MockHandlerFor(() => new Mock<GetVehicleById>(null, null))
                .Setup(h => h.Execute(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new VehicleModel(null, null));

            var controller = GetTestableFillupController();

            var result = controller.List(defaultVehicleId);
            var model = result.Extract<List<FillupListViewModel>>();
            Assert.Equal(1, model.Count()); // expect 1 group of fillups
            Assert.Equal(3, model.First().Fillups.Count()); // expect 3 members in that group
        }

        [Fact]
        public void WhenAddingFillupGet_ShowsFillupEntryView()
        {
            MockHandlerFor(
                 () => new Mock<GetVehicleById>(null, null),
                 x => x
                          .Setup(h => h.Execute(defaultUserInfo.UserId, defaultVehicleId))
                          .Returns(new VehicleModel(new Vehicle{VehicleId = defaultVehicleId}, new VehicleStatisticsModel())));

            MockFillupsForDefaultVehicle();

            var controller = GetTestableFillupController();

            var result = controller.Add(defaultVehicleId);

            Assert.IsType(typeof(ContentTypeAwareResult), result);
        }

        [Fact]
        public void WhenAddingFillupGet_ProvidesPrePopulatedModel()
        {
            // this test has some unnessary setup, just to reflect what is happening with data access
            // ultimately, the data access should be improved to remove the extra calls to the db

            var fillups = new List<FillupEntry>
                           {
                               new FillupEntry
                                   {
                                       VehicleId = defaultVehicleId, 
                                       FillupEntryId = defaultFillupId,
                                       Odometer = 500
                                   },
                           };

            // this is where the actual odometer reading originates
            var statistics = CalculateStatistics.Calculate(fillups);

            // fillups is not required on the vehicle for this test to pass
            var vehicles = new List<VehicleModel>
                               {
                                   new VehicleModel(new Vehicle {VehicleId = defaultVehicleId}, statistics )
                               };

            MockHandlerFor(
                () => new Mock<GetVehicleById>(null,null),
                x => x
                         .Setup(h => h.Execute(defaultUserInfo.UserId, defaultVehicleId))
                         .Returns(vehicles[0]));

            // this test will pass even if this handler returns the wrong set of fillups
            MockHandlerFor(
                () => new Mock<GetFillupsForVehicle>(null),
                x => x
                         .Setup(h => h.Execute(defaultVehicleId))
                         .Returns(fillups));

            var controller = GetTestableFillupController();

            var result = controller.Add(defaultVehicleId);
            var model = result.Extract<FillupEntryFormModel>();

            Assert.NotNull(model);
            Assert.Equal(500, model.Odometer);
        }

        [Fact]
        public void WhenAddingFillupPostExecutes_SendsToServicesTier()
        {
            var fillupEntry = new FillupEntryFormModel
                                  {
                                      VehicleId = defaultVehicleId,
                                      Date = DateTime.Now,
                                      Odometer = 50,
                                      PricePerUnit = 1.25d,
                                      TotalUnits = 10.0d
                                  };

            MockHandlerFor(
                () => new Mock<CanAddFillup>(null, null),
                x => x
                         .Setup(h => h.Execute(defaultUserInfo.UserId, defaultVehicleId, fillupEntry))
                         .Returns(new ValidationResult[] { }));

            var handler = MockHandlerFor(
                () => new Mock<AddFillupToVehicle>(null, null),
                x => x
                         .Setup(h => h.Execute(defaultUserInfo.UserId, defaultVehicleId, fillupEntry))
                         .Verifiable("handler was not invoked"));

            MockVehicleListWithVehicles(defaultVehicleId);

            var controller = GetTestableFillupController();
            controller.Add(defaultVehicleId, fillupEntry);

            handler.Verify();
        }

        [Fact]
        public void WhenAddingFillupPostExecutes_RedirectsToFillupList()
        {
            var fillupEntry = new FillupEntryFormModel
            {
                VehicleId = defaultVehicleId,
                Date = DateTime.Now,
                Odometer = 50,
                PricePerUnit = 1.25d,
                TotalUnits = 10.0d
            };

            MockHandlerFor(
                () => new Mock<CanAddFillup>(null, null),
                x => x
                         .Setup(h => h.Execute(defaultUserInfo.UserId, defaultVehicleId, fillupEntry))
                         .Returns(new ValidationResult[] { }));

            MockHandlerFor(() => new Mock<AddFillupToVehicle>(null, null));

            MockVehicleListWithVehicles(defaultVehicleId);

            var controller = GetTestableFillupController();
            var context = controller.MockRequestForMediaType("text/html");
            var proxy = (ITestableContentTypeAwareResult)controller.Add(defaultVehicleId, fillupEntry);
            var result = proxy.GetActionResultFor(context.Object) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("List", result.RouteValues["action"]);
            Assert.Equal("Fillup", result.RouteValues["controller"]);
        }

        // returns controller with mocks
        private FillupController GetTestableFillupController()
        {
            var controller = new FillupController(userServicesMock.Object, serviceLocator.Object);
            controller.SetFakeControllerContext();
            controller.SetUserIdentity(new MileageStatsIdentity("TestUser", defaultUserInfo.DisplayName,
                                                                       defaultUserInfo.UserId));
            return controller;
        }

        Mock<T> MockHandlerFor<T>(Func<Mock<T>> create, Action<Mock<T>> setup = null) where T : class
        {
            return serviceLocator.MockHandlerFor(create, setup);
        }

        private void MockVehicleListWithVehicles(int selectedVehicle)
        {
            MockHandlerFor(
                () => new Mock<GetVehicleListForUser>(null,null),
                x => x
                         .Setup(h => h.Execute(defaultUserInfo.UserId))
                         .Returns(defaultVehicleId.StandardVehicleList()));
        }

        private void MockFillupsForDefaultVehicle()
        {
            var list = new List<FillupEntry>
                           {
                               new FillupEntry {VehicleId = defaultVehicleId, FillupEntryId = defaultFillupId},
                               new FillupEntry {VehicleId = defaultVehicleId, FillupEntryId = defaultFillupId + 1},
                               new FillupEntry {VehicleId = defaultVehicleId, FillupEntryId = defaultFillupId + 2},
                           };

            MockHandlerFor(
                () => new Mock<GetFillupsForVehicle>(null),
                x => x
                         .Setup(h => h.Execute(defaultVehicleId))
                         .Returns(list));
        }

        private void MockEmptyFillupsForDefaultVehicle()
        {
            MockHandlerFor(
                () => new Mock<GetFillupsForVehicle>(null),
                x => x
                         .Setup(h => h.Execute(defaultVehicleId))
                         .Returns(new List<FillupEntry>()));
        }

        private void MockFillupForDefaultVehicle()
        {
            MockHandlerFor(
                () => new Mock<GetFillupById>(null),
                x => x
                         .Setup(h => h.Execute(defaultFillupId))
                         .Returns(new FillupEntry { VehicleId = defaultVehicleId }));
        }
    }
}