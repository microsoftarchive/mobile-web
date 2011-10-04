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

namespace MileageStats.Web.MobileProfiler.Tests
{
    public class MobileCapabilitiesProviderFixture
    {
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