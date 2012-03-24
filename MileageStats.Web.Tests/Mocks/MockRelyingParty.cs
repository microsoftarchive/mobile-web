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

using System.Web.Mvc;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using MileageStats.Web.Authentication;
using Moq;

namespace MileageStats.Web.Tests.Mocks
{
    internal class MockRelyingParty : IOpenIdRelyingParty
    {
        private readonly IAuthenticationResponse response;

        public MockRelyingParty()
        {
            var mock = new Mock<IAuthenticationResponse>();
            mock.SetupGet(r => r.Status).Returns(AuthenticationStatus.Authenticated);
            this.response = mock.Object;
        }

        IAuthenticationResponse IOpenIdRelyingParty.GetResponse()
        {
            return this.response;
        }

        public ActionResult RedirectToProvider(string providerUrl, string returnUrl, FetchRequest fetch)
        {
            return new EmptyResult();
        }

        public Mock<IAuthenticationResponse> ResponseMock
        {
            get { return Mock.Get(this.response); }
        }
    }
}