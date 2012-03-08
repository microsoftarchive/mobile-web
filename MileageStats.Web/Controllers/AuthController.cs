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
using System.Web.Mvc;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.Practices.Unity;
using MileageStats.Web.Authentication;
using MileageStats.Domain.Handlers;
using System.Web.UI;
using MileageStats.Web.Helpers;
using MileageStats.Web.Properties;

namespace MileageStats.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IOpenIdRelyingParty _relyingParty;
        private readonly IFormsAuthentication _formsAuthentication;
        private readonly CreateUser _createUser;
        private readonly GetUserByClaimId _getUserByClaimId;

        [InjectionConstructor]
        public AuthController(
            IOpenIdRelyingParty relyingParty, 
            IFormsAuthentication formsAuthentication,
            CreateUser createUser,
            GetUserByClaimId getUserByClaimId)
        {
            _relyingParty = relyingParty;
            _formsAuthentication = formsAuthentication;
            _createUser = createUser;
            _getUserByClaimId = getUserByClaimId;
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

#if DEBUG
            // NOTE: When we are in debug mode, we are using a mock OpenId provider.
            // For convenience we fill in the url for the fake provider
            ViewData["providerUrl"] = "http://oturner.myidprovider.org/";
#endif
            return View();
        }        

        public ActionResult SignInWithProvider(string providerUrl, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(providerUrl))
            {
                return RedirectToAction("Index");
            }

            var fetch = new FetchRequest();
            var returnUrl = Url.ToPublicUrl("SignInResponse", "Auth");
            try
            {
                TempData["rememberMe"] = rememberMe;

                return _relyingParty.RedirectToProvider(providerUrl, returnUrl, fetch);
            }
            catch (Exception)
            {
                this.SetAlertMessage(Messages.AuthController_SignIn_UnableToAuthenticateWithProvider);
                return RedirectToAction("SignIn");
            }
        }

        public ActionResult SignInResponse()
        {
            var response = _relyingParty.GetResponse();

            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    var user = _getUserByClaimId.Execute(response.ClaimedIdentifier);
                    if (user == null)
                    {
                        user = _createUser.Execute(response.ClaimedIdentifier);
                    }

                    var isPersistent = (TempData.ContainsKey("rememberMe") && (bool)TempData["rememberMe"]);

                    var ticket = UserAuthenticationTicketBuilder.CreateAuthenticationTicket(user, DateTime.Now, isPersistent);
                    
                    _formsAuthentication.SetAuthCookie(HttpContext, ticket);

                    return RedirectToAction("Index", "Dashboard");

                case AuthenticationStatus.Canceled:

                    this.SetConfirmationMessage(Messages.AuthController_CanceledAuthentication);

                    return RedirectToAction("SignIn");

                case AuthenticationStatus.Failed:
                    this.SetAlertMessage(response.Exception.Message);
                    return RedirectToAction("SignIn");

                default:
                    this.SetAlertMessage(Messages.AuthController_SignIn_UnableToAuthenticateWithProvider);
                    return RedirectToAction("SignIn");
            }
        }

        public ActionResult SignOut()
        {
            _formsAuthentication.Signout();

            return RedirectToAction("Index");
        }
    }
}