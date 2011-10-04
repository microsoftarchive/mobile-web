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
using System.Web.Security;
using MileageStats.Web.Authentication;
using MileageStats.Web.Tests.Mocks;
using Moq;
using Xunit;

namespace MileageStats.Web.Tests
{
    public class DefaultFormsAuthenticationFixture
    {
        [Fact]
        public void WhenSettingAuthenticationCookieFromTicket_ThenAddsToResponseCookie()
        {
            var cookies = new HttpCookieCollection();
            var formsAuth = new DefaultFormsAuthentication();
            var context = MvcMockHelpers.FakeHttpContext();
            var responseMock = Mock.Get(context.Response);
            responseMock.Setup(r => r.Cookies).Returns(cookies);

            formsAuth.SetAuthCookie(context, new FormsAuthenticationTicket("TestName", false, 55));

            Assert.NotNull(cookies[FormsAuthentication.FormsCookieName]);
            Assert.Equal("TestName",
                         FormsAuthentication.Decrypt(cookies[FormsAuthentication.FormsCookieName].Value).Name);
        }

        [Fact]
        public void WhenSettingAuthenticationCookieFromTicket_ThenSetsExpiryFromFormsAuthenticationTimeoutValue()
        {
            var cookies = new HttpCookieCollection();
            var formsAuth = new DefaultFormsAuthentication();
            var context = MvcMockHelpers.FakeHttpContext();
            var responseMock = Mock.Get(context.Response);
            responseMock.Setup(r => r.Cookies).Returns(cookies);
            
            formsAuth.SetAuthCookie(context, new FormsAuthenticationTicket("TestName", false, 55));

            var cookie = cookies[FormsAuthentication.FormsCookieName];
            var now = DateTime.Now;
            var formsTimeout = FormsAuthentication.Timeout;

            // assumes test can run +/- 1 minute
            Assert.True(cookie.Expires >= now.AddMinutes(formsTimeout.Minutes - 1));
            Assert.True(cookie.Expires <= now.AddMinutes(formsTimeout.Minutes + 1));
        }

        [Fact]
        public void WhenDecryptingAuthenticationTicket_TheReturnsTicket()
        {
            var formsAuth = new DefaultFormsAuthentication();
            var ticket = new FormsAuthenticationTicket("test", false, 10);
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var recoveredTicket = formsAuth.Decrypt(encryptedTicket);

            Assert.NotNull(recoveredTicket);
            Assert.Equal(ticket.Name, recoveredTicket.Name);
            Assert.Equal(ticket.Expiration, recoveredTicket.Expiration);
            Assert.Equal(ticket.IsPersistent, recoveredTicket.IsPersistent);
            Assert.Equal(ticket.IssueDate, recoveredTicket.IssueDate);
        }
    }
}