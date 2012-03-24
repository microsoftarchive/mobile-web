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
using Moq;
using Xunit;
using System;
using MileageStats.Web.MobileProfiler.ClientProfile;
using WURFL;
using System.Collections.Generic;

namespace MileageStats.Web.MobileProfiler.Tests
{
    public class MobileCapabilitiesProviderFixture
    {
        [Fact]
        public void WhenConstructingWithNullWURLManager_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MobileCapabilitiesProvider(null,
                new Mock<IProfileCookieEncoder>().Object));
        }

        [Fact]
        public void WhenConstructingWithNullProfileCookieEncoder_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MobileCapabilitiesProvider(new Mock<IWURFLManager>().Object,
                null));
        }

        [Fact]
        public void WhenCapabilitiesRequestedFromWURFL_ThenWURLManagerIsInvoked()
        {
            var device = new Mock<IDevice>();

            device.Setup(d => d.GetCapabilities())
                .Returns(new Dictionary<string, string> { { "capKey", "capValue" } });

            var manager = new Mock<IWURFLManager>();
            manager.Setup(m => m.GetDeviceForRequest("android"))
                .Returns(device.Object);

            var request = Mock.Of<HttpRequestBase>(r => r.UserAgent == "android");

            var caps = MobileCapabilitiesProvider.DetermineCapsByWurfl(request, manager.Object);

            Assert.True(caps["capKey"] == "capValue");
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

            Assert.True(caps.Count == 0);
        }

        [Fact]
        public void WhenCapabilitiesRequested_ThenCapabilitiesShouldBeAggregated()
        {
            var cookie = new HttpCookie("profile");
            var cookies = new HttpCookieCollection();

            var encoder = new Mock<IProfileCookieEncoder>();

            encoder.Setup(e => e.GetDeviceCapabilities(cookie))
                .Returns(new Dictionary<string, string> { { "profileK", "profileV" } });

            cookies.Add(cookie);

            var device = new Mock<IDevice>();

            device.Setup(d => d.GetCapabilities())
                .Returns(new Dictionary<string, string> { { "wurlfK", "wurlfV" } });

            var manager = new Mock<IWURFLManager>();
            manager.Setup(m => m.GetDeviceForRequest("unimportant stuff; Windows Phone OS 7.5; more stuff"))
                .Returns(device.Object);

            var request = new Mock<HttpRequestBase>();

            request
              .SetupGet(x => x.UserAgent)
              .Returns("unimportant stuff; Windows Phone OS 7.5; more stuff");

            request.SetupGet(r => r.Cookies)
                .Returns(cookies);

            var provider = new MobileCapabilitiesProvider(manager.Object, encoder.Object);
            var caps = provider.GetBrowserCapabilities(request.Object);

            Assert.True(caps["wurlfK"] == "wurlfV");
            Assert.True(caps["profileK"] == "profileV");
            Assert.Equal(caps[AllCapabilities.MobileDevice], "true");
        }

        [Fact]
        public void WhenUserAgentContaintWP7_ThenReportAsWirelessDevice()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; Windows Phone OS 7.5; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsByAlgorithm(request.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "true");
        }

        [Fact]
        public void WhenUserAgentContaintWP7_ThenReportStandardXHRSupport()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; Windows Phone OS 7.5; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsByAlgorithm(request.Object);

            Assert.Equal(caps[AllCapabilities.XHRType], "standard");
        }

        [Fact]
        public void WhenUserAgentContaintWP7_ThenReportJavascriptSupport()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; Windows Phone OS 7.5; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsByAlgorithm(request.Object);

            Assert.Equal(caps[AllCapabilities.Javascript], "true");
        }

        [Fact]
        public void WhenUserAgentContaintWP7_ThenReportDOMManipulationSupport()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; Windows Phone OS 7.5; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineCapsByAlgorithm(request.Object);

            Assert.Equal(caps[AllCapabilities.DOMManipulation], "true");
        }

        [Fact]
        public void WhenUserAgentContaintWinNT_ThenReportAsNOTWirelessDevice()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; Windows NT; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineIsMobileByWhiteList(request.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "false");
        }

        [Fact]
        public void WhenUserAgentContainMacintosh_ThenReportAsNOTWirelessDevice()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; (Macintosh; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineIsMobileByWhiteList(request.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "false");
        }

        [Fact]
        public void WhenUserAgentContainUnknownPlatform_ThenReportAsWirelessDevice()
        {
            var request = new Mock<HttpRequestBase>();
            request
                .SetupGet(x => x.UserAgent)
                .Returns("unimportant stuff; previously unknown platform; more stuff");

            var caps = MobileCapabilitiesProvider.DetermineIsMobileByWhiteList(request.Object);

            Assert.Equal(caps[AllCapabilities.MobileDevice], "true");
        }
    }
}