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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Messages;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace MileageStats.Web.Authentication
{
    /// <summary>
    /// A fake OpenIdRelyingParty to provide an 'offline' authentication capability.
    /// This is a sample only to allow YOU TO easily run the RI without needing an openID.
    /// Do _not_ use this in a production application.
    /// </summary>
    /// <remarks>
    /// This simple approach was done as an alternative to creating a working, but local,
    /// OpenId provider.
    /// </remarks>    
    public class LocalFakeRelyingParty : IOpenIdRelyingParty
    {
        public IAuthenticationResponse GetResponse()
        {
            return new MockAuthenticationResponse(HttpContext.Current);
        }

        public ActionResult RedirectToProvider(string providerUrl, string returnUrl, FetchRequest fetch)
        {
            return new RedirectToRouteResult(new RouteValueDictionary(new
                                                                          {
                                                                              Controller = "MockAuthenticator",
                                                                              Action = "Index",
                                                                              ProviderUrl = providerUrl,
                                                                              ReturnUrl = returnUrl
                                                                          }));
        }

        private class MockAuthenticationResponse : IAuthenticationResponse
        {
            private readonly HttpContext _httpContext;

            public MockAuthenticationResponse(HttpContext httpContext)
            {
                if (httpContext == null) throw new ArgumentNullException("httpContext");
                this._httpContext = httpContext;
            }

            public string GetCallbackArgument(string key)
            {
                throw new NotImplementedException();
            }

            public string GetUntrustedCallbackArgument(string key)
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, string> GetCallbackArguments()
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, string> GetUntrustedCallbackArguments()
            {
                throw new NotImplementedException();
            }

            public T GetExtension<T>() where T : IOpenIdMessageExtension
            {
                return (T) this.GetExtension(typeof(T));
            }

            public IOpenIdMessageExtension GetExtension(Type extensionType)
            {
                if (extensionType == typeof(FetchResponse))
                {
                    var response = new FetchResponse();

                    return response;
                }

                return null;
            }

            public T GetUntrustedExtension<T>() where T : IOpenIdMessageExtension
            {
                throw new NotImplementedException();
            }

            public IOpenIdMessageExtension GetUntrustedExtension(Type extensionType)
            {
                throw new NotImplementedException();
            }

            public Identifier ClaimedIdentifier
            {
                get
                {
                    var identifier = this._httpContext.Request.Form.Get("claimed_identifier");
                    return Identifier.Parse(identifier, true);
                }
            }

            public string FriendlyIdentifierForDisplay
            {
                get { return this._httpContext.Request.Form.Get("friendly_name"); }
            }

            public AuthenticationStatus Status
            {
                get { return AuthenticationStatus.Authenticated; }
            }

            public IProviderEndpoint Provider
            {
                get { throw new NotImplementedException(); }
            }

            public Exception Exception
            {
                get { return null; }
            }
        }
    }
}