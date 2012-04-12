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

using System.Web;
using MileageStats.Web.Capabilities;
using MileageStats.Web.ClientProfile;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;

namespace MileageStats.Web.Tests.Capabilities
{
    public class MobileCapabilitiesProviderFixture
    {
        [Fact]
        public void WhenConstructingWithNullProfileCookieEncoder_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MobileCapabilitiesProvider(null));
        }

        [Fact]
        public void WhenCapabilitiesRequestedFromClientProfile_ThenProfileCookieIsUsed()
        {
            var cookie = new HttpCookie("profile");
            var encoder = new Mock<IProfileCookieEncoder>();

            encoder.Setup(e => e.GetDeviceCapabilities(cookie))
                .Returns(new Dictionary<string, string> { { "capKey", "capValue" } });

            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);
            
            var request = Mock.Of<HttpRequestBase>(r => r.Cookies == cookies);

            var caps = MobileCapabilitiesProvider.DetermineCapsByProfilingClient(request, encoder.Object);

            Assert.True(caps["capKey"] == "capValue");
        }

        [Fact]
        public void WhenCapabilitiesRequestedFromClientProfileAndCookieNotFound_ThenEmptyListIsReturned()
        {
            var encoder = new Mock<IProfileCookieEncoder>();

            encoder.Setup(e => e.GetDeviceCapabilities(new HttpCookie("")));

            var cookies = new HttpCookieCollection();

            var request = Mock.Of<HttpRequestBase>(r => r.Cookies == cookies);

            var caps = MobileCapabilitiesProvider.DetermineCapsByProfilingClient(request, encoder.Object);

            Assert.Equal(0, caps.Count);
        }


        [Fact]
        public void WhenUserAgentContaintWinNT_ThenReportAsNOTWirelessDevice()
        {
            var context = new Mock<HttpContextBase>();
            context
                .SetupGet(x => x.Request.UserAgent)
                .Returns("unimportant stuff; Windows NT; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsBy3rdPartyDatabase(context.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "false");
        }

        [Fact]
        public void WhenUserAgentContainMacintosh_ThenReportAsNOTWirelessDevice()
        {
            var context = new Mock<HttpContextBase>();
            context
                .SetupGet(x => x.Request.UserAgent)
                .Returns("unimportant stuff; (Macintosh; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsBy3rdPartyDatabase(context.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "false");
        }

        [Fact]
        public void WhenUserAgentContainUnknownPlatform_ThenReportAsWirelessDevice()
        {
            var context = new Mock<HttpContextBase>();
            context
                .SetupGet(x => x.Request.UserAgent)
                .Returns("unimportant stuff; previously unknown platform; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsBy3rdPartyDatabase(context.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "true");
        }
    }
}