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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MileageStats.Domain.Models;
using MileageStats.Web.Authentication;
using MileageStats.Web.Controllers;
using MileageStats.Web.Models;
using MileageStats.Web.Tests.Mocks;
using Moq;
using Xunit;
using System.Net;
using MileageStats.Domain.Handlers;

namespace MileageStats.Web.Tests.Controllers
{
    public class ProfileControllerFixture
    {
        private readonly Mock<GetCountries> countryServicesMock;
        private readonly Mock<UpdateUser> updateUserMock;
        private readonly User currentUser;
        private readonly Mock<IFormsAuthentication> formsAuthenticationMock;
        private readonly ProfileController profileController;
        private readonly Mock<GetUserByClaimId> userServicesMock;
        private User modifiedUser;

        public ProfileControllerFixture()
        {
            currentUser = new User {UserId = 1, Country = "argentina", DisplayName = "a display name"};
            formsAuthenticationMock = new Mock<IFormsAuthentication>();
            userServicesMock = new Mock<GetUserByClaimId>(null);
            updateUserMock = new Mock<UpdateUser>(null);

            userServicesMock
                .Setup(r => r.Execute(It.IsAny<string>()))
                .Returns(currentUser);

            updateUserMock.Setup(r => r.Execute(It.IsAny<User>()))
                .Callback((User u) => { modifiedUser = u; }).Verifiable();

            countryServicesMock = new Mock<GetCountries>(null);
            countryServicesMock
                .Setup(r => r.Execute())
                .Returns(() => new ReadOnlyCollection<string>(new List<string> {"a", "b"}));

            profileController = new ProfileController(
                updateUserMock.Object,
                userServicesMock.Object,
                countryServicesMock.Object,
                formsAuthenticationMock.Object);

            profileController.SetFakeControllerContext();
            profileController.SetUserIdentity(new MileageStatsIdentity("CleverClaimsId", "Test display name", 1));
        }

        [Fact]
        public void WhenDriverUpdatesProfile_ThenRepositoryIsUpdated()
        {
            var updatedUser = new User {DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1};

            ActionResult response = profileController.Edit(updatedUser);

            updateUserMock.Verify(r => r.Execute(It.IsAny<User>()));
            Assert.Same(updatedUser, modifiedUser);
        }

        [Fact]
        public void WhenDriverUpdatesProfile_ThenAuthenticationTicketIsSet()
        {
            formsAuthenticationMock.Setup(f => f.GetAuthenticationTicket(It.IsAny<HttpContextBase>())).Returns(
                new FormsAuthenticationTicket("user", false, 100));
            
            var updatedUser = new User {DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1};

            profileController.Edit(updatedUser);

            formsAuthenticationMock.Verify(
                a => a.SetAuthCookie(It.IsAny<HttpContextBase>(), It.IsAny<FormsAuthenticationTicket>()), Times.Once());
        }

        [Fact]
        public void WhenDriverUpdatesProfile_ThenHasRegisteredIsTrue()
        {
            var updatedUser = new User {DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1};

            profileController.Edit(updatedUser);

            Assert.True(updatedUser.HasRegistered);
        }

        [Fact]
        public void WhenDriverCancelsProfileUpdate_ThenHasRegisteredIsTrue()
        {
            var updatedUser = new User { DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1 };
            
            profileController.Edit(updatedUser, "cancel");

            Assert.True(updatedUser.HasRegistered);
        }

        [Fact]
        public void WhenDriverCancelsProfileUpdate_ThenProfileDataIsNotUpdated()
        {
            var updatedUser = new User { DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1 };

            profileController.Edit(updatedUser, "cancel");

            Assert.Equal(profileController.CurrentUser.DisplayName, modifiedUser.DisplayName);
            Assert.Equal(profileController.CurrentUser.Country, modifiedUser.Country);
        }

        [Fact]
        public void WhenDriverUpdatesProfile_ThenAuthenticationTicketIsSetWithUpdatedUserData()
        {
            formsAuthenticationMock.Setup(f => f.GetAuthenticationTicket(It.IsAny<HttpContextBase>())).Returns(
                new FormsAuthenticationTicket("user", false, 100));

            FormsAuthenticationTicket newTicket = null;
            formsAuthenticationMock
                .Setup(a => a.SetAuthCookie(It.IsAny<HttpContextBase>(), It.IsAny<FormsAuthenticationTicket>()))
                .Callback<HttpContextBase, FormsAuthenticationTicket>((c, a) => { newTicket = a; });

            var updatedUser = new User {DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1};

            profileController.Edit(updatedUser);

            Assert.Equal(updatedUser.AuthorizationId, newTicket.Name);
            UserInfo userInfo = UserInfo.FromString(newTicket.UserData);
            Assert.NotNull(userInfo);
            Assert.Equal(updatedUser.AuthorizationId, userInfo.ClaimsIdentifier);
            Assert.Equal(updatedUser.DisplayName, userInfo.DisplayName);
            Assert.Equal(updatedUser.UserId, userInfo.UserId);
        }

        [Fact]
        public void WhenDriverUpdatesProfileWithInvalidDataOnAjaxCall_ThenBadRequestIsReturned()
        {
            profileController.ModelState.AddModelError("key", "error message");
            profileController.HttpContext.Request.SetAjaxRequest();
            
            var updatedUser = new User { DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1 };

            var result = profileController.Edit(updatedUser);

            Assert.IsType<HttpStatusCodeResult>(result);
            Assert.Equal(400, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Fact]
        public void WhenDriverUpdatesProfileWithInvalidData_ThenEditViewIsReturned()
        {
            profileController.ModelState.AddModelError("key", "error message");

            var context = profileController.MockRequestForMediaType("text/html");

            var updatedUser = new User { DisplayName = "Updated name", AuthorizationId = "CleverClaimsId", UserId = 1 };

            var proxy = (ITestableContentTypeAwareResult)profileController.Edit(updatedUser);
            var result = proxy.GetActionResultFor(context.Object);

            Assert.IsType<ViewResult>(result);
        }


    }
}