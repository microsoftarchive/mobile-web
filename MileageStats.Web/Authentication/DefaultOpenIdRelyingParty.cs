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
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using MileageStats.Web.Helpers;

namespace MileageStats.Web.Authentication
{
    internal class DefaultOpenIdRelyingParty : IOpenIdRelyingParty, IDisposable
    {
        private readonly OpenIdRelyingParty _relyingParty = new OpenIdRelyingParty();

        public IAuthenticationResponse GetResponse()
        {
            return _relyingParty.GetResponse();
        }

        public ActionResult RedirectToProvider(string providerUrl, string returnUrl, FetchRequest fetch)
        {
            var baseUrl = HttpContext.Current.Request.ToPublicUrl(new Uri("/", UriKind.Relative));
            var realm = new Realm(baseUrl);
            var authenticationRequest = _relyingParty.CreateRequest(providerUrl, realm, new Uri(returnUrl,UriKind.Absolute));
            authenticationRequest.AddExtension(fetch);

            return new OutgoingRequestActionResult(authenticationRequest.RedirectingResponse);
        }

        public virtual void Dispose()
        {
            _relyingParty.Dispose();
        }
    }
}