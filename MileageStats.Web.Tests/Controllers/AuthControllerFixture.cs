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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Models;
using MileageStats.Web.Authentication;
using MileageStats.Web.Controllers;
using MileageStats.Web.Tests.Mocks;
using Moq;
using Xunit;
using MileageStats.Domain.Handlers;

namespace MileageStats.Web.Tests.Controllers
{
    public class AuthControllerFixture
    {
        [Fact]
        public void WhenSigningInWithoutSpecifyingProviderUrl_ThenRedirectsToOpenProvidedAction()
        {
            var relyingPartyMock = new Mock<IOpenIdRelyingParty>();
            relyingPartyMock
                .Setup(x => x.RedirectToProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FetchRequest>()))
                .Throws(new Exception("Relying party expects a url, a null or empty string will raise an exception."));

            TestableAuthController authController = GetTestableAuthController(relyingPartyMock.Object);

            var result = authController.SignInWithProvider(null) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void WhenSigningIn_ThenRedirectsToOpenProvidedAction()
        {
            string providerUrl = @"http://ssomeauthcontroller.com";

            TestableAuthController authController =
                GetTestableAuthController(OpenIdRelyingPartyBuilder.DefaultParty().Object);

            ActionResult response = authController.SignInWithProvider(providerUrl);

            // For the test we ensure that controller responsds with whatever the OpenIdRelyingParty facade returns.
            Assert.IsType(typeof (RedirectResult), response);
        }

        [Fact]
        public void WhenSigningIn_ThenSetsReturnUrlToCorrectSchema()
        {
            string providerUrl = @"http://ssomeauthcontroller.com";

            var relyingPartyMock = new Mock<IOpenIdRelyingParty>();

            TestableAuthController authController = GetTestableAuthController(relyingPartyMock.Object);

            Mock<HttpRequestBase> requestMock = Mock.Get(authController.Request);
            requestMock.SetupGet(r => r.Url)
                .Returns(new Uri(@"https://nothingmattersbutschema.com", UriKind.Absolute));

            ActionResult response = authController.SignInWithProvider(providerUrl);

            relyingPartyMock.Verify(x => x.RedirectToProvider(
                It.IsAny<string>(),
                It.Is<string>(s => s.StartsWith("https:")),
                It.IsAny<FetchRequest>()), "Incorrect schema applied for return URL.");
        }

        [Fact]
        public void WhenUserAuthenticatedAndRegistered_ThenRedirectsToHomeIndex()
        {
            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);
            
            getUser.Setup(u => u.Execute(It.IsAny<string>())).Returns(
                new User());

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder.DefaultParty().Object,
                new Mock<IFormsAuthentication>().Object,
                createUser.Object,
                getUser.Object
                );

            ActionResult response = authController.SignInResponse();

            Assert.IsType(typeof (RedirectToRouteResult), response);
            var route = ((RedirectToRouteResult) response).RouteValues;
            Assert.Equal("Dashboard", route["controller"]);
            Assert.Equal("Index", route["action"]);
        }

        [Fact]
        public void WhenProviderRespondsAuthenticated_ThenSetsFormsAuthCookie()
        {
            const string claimIdentifier = @"http://username/";
            var formsAuthMock = new Mock<IFormsAuthentication>();
            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(u => u.Execute(It.IsAny<string>())).Returns(
                new User());

            formsAuthMock.Setup(f => f.SetAuthCookie(It.IsAny<HttpContextBase>(), It.IsAny<FormsAuthenticationTicket>()))
                .Verifiable();

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder
                    .DefaultParty()
                    .ReturnsClaimId(claimIdentifier)
                    .Object,
                formsAuthMock.Object,
                createUser.Object, 
                getUser.Object, 
                @"http://providerUrl.com");

            ActionResult response = authController.SignInResponse();

            formsAuthMock.Verify();
        }

        [Fact]
        public void WhenProviderRespondsAuthenticated_ThenSetsFormsAuthCookieNameToClaimIdentifier()
        {
            const string claimIdentifier = @"http://username/";
            const string friendlyName = "FriendlyName";

            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(ur => ur.Execute(It.IsAny<string>()))
                .Returns(new User
                             {
                                 DisplayName = friendlyName,
                                 AuthorizationId = claimIdentifier
                             })
                .Verifiable();

            var formsAuthMock = new Mock<IFormsAuthentication>();
            formsAuthMock.Setup(
                f =>
                f.SetAuthCookie(It.IsAny<HttpContextBase>(),
                                It.Is<FormsAuthenticationTicket>(t => t.Name == claimIdentifier))).Verifiable();

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder
                    .DefaultParty()
                    .ReturnsClaimId(claimIdentifier)
                    .ReturnFriendlyName(friendlyName)
                    .Object,
                    formsAuthMock.Object,
                    createUser.Object, 
                    getUser.Object,
                    @"http://providerUrl.com");

            ActionResult response = authController.SignInResponse();

            formsAuthMock.Verify();
        }

        [Fact]
        public void WhenProviderRespondsAuthenticated_ThenSerializesAdditionalUserInfoInUserData()
        {
            const string claimIdentifier = @"http://username/";
            FormsAuthenticationTicket ticket = null;

            var formsAuthMock = new Mock<IFormsAuthentication>();
            formsAuthMock.Setup(f => f.SetAuthCookie(It.IsAny<HttpContextBase>(), It.IsAny<FormsAuthenticationTicket>()))
                .Callback<HttpContextBase, FormsAuthenticationTicket>((h, t) => ticket = t);

            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(x => x.Execute(It.Is<string>(u => u == claimIdentifier)))
                .Returns(new User
                             {
                                 AuthorizationId = claimIdentifier,
                                 DisplayName = "TestDisplayName",
                                 UserId = 55,
                             });

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder
                    .DefaultParty()
                    .ReturnsClaimId(claimIdentifier)
                    .Object,
                formsAuthMock.Object,
                createUser.Object,
                getUser.Object,
                @"http://providerUrl.com");

            ActionResult response = authController.SignInResponse();

            // Assert
            UserInfo userInfo = UserInfo.FromString(ticket.UserData);
            Assert.NotNull(userInfo);
        }

        [Fact]
        public void WhenProviderRespondsAuthenticated_ThenSerializesNewUserIdInUserData()
        {
            const string claimIdentifier = @"http://username/";
            FormsAuthenticationTicket ticket = null;

            var formsAuthMock = new Mock<IFormsAuthentication>();
            formsAuthMock.Setup(f => f.SetAuthCookie(It.IsAny<HttpContextBase>(), It.IsAny<FormsAuthenticationTicket>()))
                .Callback<HttpContextBase, FormsAuthenticationTicket>((h, t) => ticket = t);

            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(x => x.Execute(It.Is<string>(u => u == claimIdentifier)))
                .Returns(new User
                             {
                                 AuthorizationId = claimIdentifier,
                                 DisplayName = "TestDisplayName",
                                 UserId = 55,
                             });

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder
                    .DefaultParty()
                    .ReturnsClaimId(claimIdentifier)
                    .Object,
                formsAuthMock.Object,
                createUser.Object,
                getUser.Object,
                @"http://providerUrl.com");

            ActionResult response = authController.SignInResponse();

            // Assert
            UserInfo userInfo = UserInfo.FromString(ticket.UserData);

            Assert.NotNull(userInfo);
            Assert.Equal(55, userInfo.UserId);
        }

        [Fact]
        public void WhenProviderRespondsAuthenticatedWithMissingMetadata_ThenSavesOnlyPopulatedMetadata()
        {
            var fetchResponse = new FetchResponse();

            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(ur => ur.Execute(It.IsAny<string>()))
                .Returns<User>(null);

            createUser.Setup(c => c.Execute(It.IsAny<string>()))
                .Returns(new User { })
                .Verifiable();

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder.DefaultParty()
                    .ReturnFriendlyName("BillyBaroo")
                    .ReturnFetchResponse(fetchResponse)
                    .Object,
                new Mock<IFormsAuthentication>().Object,
                createUser.Object,
                getUser.Object);

            authController.SignInResponse();

            createUser.VerifyAll();
        }

        [Fact]
        public void WhenProviderRespondsAuthenticatedAndSuppliesMetdata_ThenMetadataSavedToRepository()
        {
            var fetchResponse = new FetchResponse();
            
            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(ur => ur.Execute(It.IsAny<string>()))
                .Returns<User>(null);

            createUser.Setup(c => c.Execute(It.IsAny<string>()))
                .Returns(new User { })
                .Verifiable();

            TestableAuthController authController = GetTestableAuthController(
                OpenIdRelyingPartyBuilder.DefaultParty()
                    .ReturnFriendlyName("BillyBaroo")
                    .ReturnFetchResponse(fetchResponse)
                    .Object,
                new Mock<IFormsAuthentication>().Object,
                createUser.Object,
                getUser.Object);

            authController.SignInResponse();

            createUser.VerifyAll();
        }

        [Fact]
        public void WhenProviderRespondsFailedSignInAuthentication_ThenRedirectsToSignInAction()
        {
            var mockRelyingParty = new MockRelyingParty();
            mockRelyingParty.ResponseMock.SetupGet(r => r.Status).Returns(AuthenticationStatus.Failed);
            mockRelyingParty.ResponseMock.SetupGet(r => r.Exception).Returns(new Exception("Failed"));

            TestableAuthController authController = GetTestableAuthController(mockRelyingParty);

            ActionResult result = authController.SignInResponse();
            Assert.IsType(typeof (RedirectToRouteResult), result);
            Assert.Equal("SignIn", ((RedirectToRouteResult) result).RouteValues["action"]);
        }

        [Fact]
        public void WhenProviderRespondsFailedSignInAuthentication_ThenProvidesErrorMessage()
        {
            var exception = new ArgumentException("TestException");
            var relyingParty = new MockRelyingParty();
            relyingParty.ResponseMock.SetupGet(r => r.Status).Returns(AuthenticationStatus.Failed);
            relyingParty.ResponseMock.SetupGet(r => r.Exception).Returns(exception);

            TestableAuthController authController = GetTestableAuthController(relyingParty);

            authController.SignInResponse();
            Assert.Equal(exception.Message, authController.TempData["Message"]);
        }

        [Fact]
        public void WhenProviderRespondsCancelledAuthentication_ThenRedirectsToSignInAction()
        {
            var relyingParty = new MockRelyingParty();
            relyingParty.ResponseMock.SetupGet(r => r.Status).Returns(AuthenticationStatus.Canceled);

            TestableAuthController authController = GetTestableAuthController(relyingParty);

            ActionResult result = authController.SignInResponse();
            Assert.IsType(typeof (RedirectToRouteResult), result);
            Assert.Equal("SignIn", ((RedirectToRouteResult) result).RouteValues["action"]);
        }

        [Fact]
        public void WhenProviderRespondsWithAnythingElse_ThenRedirectsToSignInActionWithMessage()
        {
            var relyingParty = new MockRelyingParty();
            relyingParty.ResponseMock.SetupGet(r => r.Status).Returns(AuthenticationStatus.SetupRequired);

            TestableAuthController authController = GetTestableAuthController(relyingParty);

            ActionResult result = authController.SignInResponse();
            Assert.IsType(typeof (RedirectToRouteResult), result);
            Assert.Equal("SignIn", ((RedirectToRouteResult) result).RouteValues["action"]);
            Assert.NotNull(authController.TempData["Message"]);
        }

        [Fact]
        public void WhenProviderCreateRequestThrows_ThenRedirectsToSignInActionWithAMessage()
        {
            var relyingParty = new Mock<IOpenIdRelyingParty>();
            relyingParty
                .Setup(r => r.RedirectToProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FetchRequest>()))
                .Throws(new Exception("Some Provider Exception"));

            var authController = GetTestableAuthController(relyingParty.Object);

            var result = authController.SignInWithProvider(@"http://providerUrl");

            Assert.IsType(typeof (RedirectToRouteResult), result);
            Assert.Equal("SignIn", ((RedirectToRouteResult) result).RouteValues["action"]);
            Assert.NotNull(authController.TempData["Message"]);
        }

        [Fact]
        public void WhenUserAuthenticatedAndHasNoUserRecord_ThenCreatesOne()
        {
            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            getUser.Setup(r => r.Execute(It.IsAny<string>()))
                .Returns<User>(null)
                .Verifiable();

            createUser.Setup(c => c.Execute(It.IsAny<string>()))
                .Returns(new User { })
                .Verifiable();

            TestableAuthController authController = GetTestableAuthController(
                new MockRelyingParty(),
                new Mock<IFormsAuthentication>().Object,
                createUser.Object,
                getUser.Object
                );

            ActionResult result = authController.SignInResponse();

            createUser.VerifyAll();
        }

        [Fact]
        public void WhenUserSignsOut_ThenSignThemOut()
        {
            var createUser = new Mock<CreateUser>(null);
            var getUser = new Mock<GetUserByClaimId>(null);

            var formsMock = new Mock<IFormsAuthentication>();
            var authController = new AuthController(OpenIdRelyingPartyBuilder.DefaultParty().Object, formsMock.Object,
                                                    createUser.Object, getUser.Object);

            ActionResult response = authController.SignOut();

            formsMock.Verify(x => x.Signout(), Times.Once());
            Assert.IsType<RedirectToRouteResult>(response);
        }

        private static TestableAuthController GetTestableAuthController(IOpenIdRelyingParty relyingParty)
        {
            return GetTestableAuthController(relyingParty, new Mock<IFormsAuthentication>().Object,
                                             new Mock<CreateUser>(null).Object,
                                             new Mock<GetUserByClaimId>(null).Object);
        }

        private static TestableAuthController GetTestableAuthController(
            IOpenIdRelyingParty relyingParty,
            IFormsAuthentication formsAuth,
            CreateUser createUser,
            GetUserByClaimId getUser,
            string providerUrl = @"http:\\testprovider.com")
        {
            HttpContextBase contextMock = MvcMockHelpers.FakeHttpContext(providerUrl);
            var authController = new TestableAuthController(relyingParty, formsAuth, createUser, getUser);
            authController.ControllerContext = new ControllerContext(contextMock, new RouteData(), authController);
            authController.InvokeInitialize(authController.ControllerContext.RequestContext);

            // default routes
            var routes = new RouteCollection();
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = ""} // Parameter defaults
                );
            authController.Url = new UrlHelper(authController.ControllerContext.RequestContext,
                                               routes);

            return authController;
        }

        

        #region Nested type: OpenIdRelyingPartyBuilder

        public class OpenIdRelyingPartyBuilder
        {
            private readonly Mock<IAuthenticationRequest> authenticationRequest = new Mock<IAuthenticationRequest>();
            private readonly Mock<IAuthenticationResponse> authenticationResponse = new Mock<IAuthenticationResponse>();
            private readonly Mock<IOpenIdRelyingParty> relyingPartyMock = new Mock<IOpenIdRelyingParty>();
            private Action<FetchRequest> _fetchRequestAction = (fr) => { };
            private FetchResponse fetchResponse;
            private ActionResult providerRedirectResult = new RedirectResult(@"http://test.url/");

            private OpenIdRelyingPartyBuilder()
            {
                authenticationResponse.SetupGet(x => x.Status).Returns(AuthenticationStatus.Authenticated);
                authenticationResponse.Setup(x => x.GetExtension<FetchResponse>()).Returns(() => fetchResponse);
                authenticationResponse.SetupGet(x => x.ClaimedIdentifier).Returns("id");

                relyingPartyMock.Setup(
                    x => x.RedirectToProvider(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FetchRequest>()))
                    .Callback<string, string, FetchRequest>((p, u, fr) => _fetchRequestAction(fr))
                    .Returns(() => providerRedirectResult);
                relyingPartyMock.Setup(x => x.GetResponse()).Returns(authenticationResponse.Object);
            }

            public IOpenIdRelyingParty Object
            {
                get { return relyingPartyMock.Object; }
            }

            public static OpenIdRelyingPartyBuilder DefaultParty()
            {
                return new OpenIdRelyingPartyBuilder();
            }

            public OpenIdRelyingPartyBuilder OnRedirectToProvider(string providerUrl)
            {
                relyingPartyMock.Setup(
                    x => x.RedirectToProvider(providerUrl, It.IsAny<string>(), It.IsAny<FetchRequest>()))
                    .Callback<string, string, FetchRequest>((p, u, fr) => _fetchRequestAction(fr))
                    .Returns(() => providerRedirectResult);
                return this;
            }

            public OpenIdRelyingPartyBuilder ReturnsRedirectResult(ActionResult result)
            {
                providerRedirectResult = result;
                return this;
            }

            public OpenIdRelyingPartyBuilder OnFetchRequest(Action<FetchRequest> fetchRequestAction)
            {
                _fetchRequestAction = fetchRequestAction;
                return this;
            }

            internal OpenIdRelyingPartyBuilder ReturnFriendlyName(string friendlyName)
            {
                authenticationResponse.Setup(x => x.FriendlyIdentifierForDisplay).Returns(friendlyName);
                return this;
            }

            internal OpenIdRelyingPartyBuilder ReturnFetchResponse(FetchResponse response)
            {
                fetchResponse = response;
                return this;
            }

            internal OpenIdRelyingPartyBuilder AddResponseAttribute(string attributeName, string value)
            {
                if (fetchResponse == null)
                {
                    fetchResponse = new FetchResponse();
                }
                fetchResponse.Attributes.Add(attributeName, value);

                return this;
            }

            public OpenIdRelyingPartyBuilder ReturnsClaimId(string claimId)
            {
                authenticationResponse.SetupGet(r => r.ClaimedIdentifier).Returns(claimId);
                return this;
            }
        }

        #endregion

        #region Nested type: TestableAuthController

        public class TestableAuthController : AuthController
        {
            public TestableAuthController(IOpenIdRelyingParty mockRelyingParty, IFormsAuthentication formsAuth,
                                          CreateUser createUser, GetUserByClaimId getUser)
                : base(mockRelyingParty, formsAuth, createUser, getUser)
            {
            }

            public void InvokeInitialize(RequestContext context)
            {
                base.Initialize(context);
            }
        }

        #endregion
    }
}