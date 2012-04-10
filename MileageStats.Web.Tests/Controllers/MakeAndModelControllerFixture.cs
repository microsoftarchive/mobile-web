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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Moq;
using MileageStats.Domain.Models;
using MileageStats.Domain.Handlers;
using MileageStats.Web.Controllers;
using System.Web.Routing;
using MileageStats.Web.Models;
using MileageStats.Web.Tests.Mocks;
using Xunit;
using System.Web.Mvc;

namespace MileageStats.Web.Tests.Controllers
{
    public class MakeAndModelControllerFixture
    {
        public MakeAndModelControllerFixture()
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
        }

        private Mock<GetUserByClaimId> userServicesMock { get; set; }
        private Mock<IServiceLocator> serviceLocator { get; set; }
        private UserInfo defaultUserInfo { get; set; }
        private User defaultUser { get; set; }

        [Fact]
        public void WhenJsonMakesRequestedForDatafullYear_ThenReturnsAnyMakes()
        {
            const int yearSelected = 1984;

            MockHandlerFor(
                () => new Mock<GetYearsMakesAndModels>(null),
                x => x
                         .Setup(h => h.Execute(yearSelected, null))
                         .Returns(YearsMakesAndModelsFor.YearWithMakes(yearSelected, "ManufacturerA")));

            var controller = GetTestableMakeAndModelController();

            JsonResult result = controller.MakesForYear(yearSelected);

            var makeList = (string[])result.Data;
            Assert.Contains("ManufacturerA", makeList);
            Assert.Equal(1, makeList.Count());
        }

        [Fact]
        public void WhenJsonMakesRequestedForDatalessYear_ThenReturnsNoMakes()
        {
            const int yearSelected = 1985;

            MockHandlerFor(
                () => new Mock<GetYearsMakesAndModels>(null),
                x => x
                         .Setup(h => h.Execute(yearSelected, null))
                         .Returns(YearsMakesAndModelsFor.YearWithoutMakes(yearSelected)));

            var controller = GetTestableMakeAndModelController();

            JsonResult result = controller.MakesForYear(yearSelected);

            var makeList = (string[])result.Data;
            Assert.Equal(0, makeList.Count());
        }

        [Fact]
        public void WhenJsonModelRequestedForDatafullYearAndMakeRequested_ThenReturnsModels()
        {
            const int yearSelected = 1985;
            const string makeSelected = "ManufacturerA";

            MockHandlerFor(
                () => new Mock<GetYearsMakesAndModels>(null),
                x => x
                         .Setup(h => h.Execute(yearSelected, makeSelected))
                         .Returns(YearsMakesAndModelsFor.MakeWithModels(yearSelected, makeSelected, "Model1", "Model2")));

            var controller = GetTestableMakeAndModelController();
            JsonResult result = controller.ModelsForMake(yearSelected, makeSelected);

            var modelList = (string[])result.Data;
            Assert.Equal(2, modelList.Count());
            Assert.Contains("Model1", modelList);
            Assert.Contains("Model2", modelList);
        }

        [Fact]
        public void WhenJsonModelsRequestedForDatalessYearAndMake_ThenReturnsModels()
        {
            const int yearSelected = 1985;
            const string makeSelected = "ManufacturerA";

            MockHandlerFor(
                () => new Mock<GetYearsMakesAndModels>(null),
                x => x
                         .Setup(h => h.Execute(yearSelected, makeSelected))
                         .Returns(YearsMakesAndModelsFor.MakeWithModels(yearSelected, makeSelected)));

            var controller = GetTestableMakeAndModelController();

            JsonResult result = controller.ModelsForMake(yearSelected, makeSelected);

            var modelList = (string[])result.Data;
            Assert.Equal(0, modelList.Count());
        }

        private Mock<T> MockHandlerFor<T>(Func<Mock<T>> create, Action<Mock<T>> setup = null) where T : class
        {
            return serviceLocator.MockHandlerFor(create, setup);
        }

        // returns controller with mocks
        private TestableMakeAndModelController GetTestableMakeAndModelController()
        {
            var c = new TestableMakeAndModelController(userServicesMock.Object, serviceLocator.Object);
            c.SetFakeControllerContext();
            c.SetUserIdentity(new MileageStatsIdentity(defaultUserInfo.ClaimsIdentifier,
                                                       defaultUserInfo.DisplayName,
                                                       defaultUserInfo.UserId));

            c.InvokeInitialize(c.ControllerContext.RequestContext);

            return c;
        }

        internal class TestableMakeAndModelController : MakeAndModelController
        {
            public TestableMakeAndModelController(GetUserByClaimId userServices,
                                               IServiceLocator serviceLocator)
                : base(userServices, serviceLocator)
            {
            }

            public void InvokeInitialize(RequestContext context)
            {
                base.Initialize(context);
            }

        }
    }
}
