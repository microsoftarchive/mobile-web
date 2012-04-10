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
using Xunit;
using System.Web.Mvc;
using MileageStats.Domain.Models;
using MileageStats.Domain.Handlers;
using Moq;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Web.Controllers;
using System.Web.Routing;
using MileageStats.Web.Tests.Mocks;
using MileageStats.Web.Models;

namespace MileageStats.Web.Tests.Controllers
{
    public class VehiclePhotoControllerFixture
    {
        public VehiclePhotoControllerFixture()
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
        public void WhenPhotoForValidPhoto_ThenPhotoReturned()
        {
            const int photoId = 33;

            MockHandlerFor(
                () => new Mock<GetVehiclePhoto>(null),
                x => x
                         .Setup(h => h.Execute(photoId))
                         .Returns(new VehiclePhoto
                         {
                             Image = new byte[] { },
                             ImageMimeType = "something"
                         }));

            var controller = GetTestableImageController();
            var result = (FileStreamResult)controller.GetVehiclePhoto(photoId);

            Assert.NotNull(result.FileStream);
        }

        [Fact]
        public void WhenPhotoForInvalidPhoto_ThenDefaultPhotoReturned()
        {
            const int photoId = 33;

            MockHandlerFor(
                () => new Mock<GetVehiclePhoto>(null),
                x => x
                         .Setup(h => h.Execute(photoId))
                         .Returns<VehiclePhoto>(null));

            var controller = GetTestableImageController();

            Mock.Get(controller.HttpContext)
                .SetupGet(x => x.Request.ApplicationPath)
                .Returns("/Something");

            Mock.Get(controller.HttpContext)
                .Setup(x => x.Response.ApplyAppPathModifier(It.IsAny<string>()))
                .Returns("/Something/Content/vehicle.png");

            var result = (FilePathResult)controller.GetVehiclePhoto(photoId);
            Assert.Contains("Something/Content/vehicle.png", result.FileName);
        }

        private Mock<T> MockHandlerFor<T>(Func<Mock<T>> create, Action<Mock<T>> setup = null) where T : class
        {
            return serviceLocator.MockHandlerFor(create, setup);
        }

        // returns controller with mocks
        private TestableImageController GetTestableImageController()
        {
            var c = new TestableImageController(userServicesMock.Object, serviceLocator.Object);
            c.SetFakeControllerContext();
            c.SetUserIdentity(new MileageStatsIdentity(defaultUserInfo.ClaimsIdentifier,
                                                       defaultUserInfo.DisplayName,
                                                       defaultUserInfo.UserId));

            c.InvokeInitialize(c.ControllerContext.RequestContext);

            return c;
        }

        internal class TestableImageController : VehiclePhotoController
        {
            public TestableImageController(GetUserByClaimId userServices,
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
