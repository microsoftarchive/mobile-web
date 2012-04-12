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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Xunit;
using System.Web;

namespace MileageStats.Web.MobileProfiler.Tests
{
    public class MobileBrowserCapabilitiesFixture
    {
        [Fact]
        public void WhenMobileDeviceCapabilitiesPassed_ThenIsMobileDeviceReturnsTrue()
        {
            var deviceCapabilities = new Dictionary<string, string> 
            {
                { AllCapabilities.MobileDevice, "true" }
            };

            var capabilities = new MobileBrowserCapabilities(deviceCapabilities);
            
            Assert.True(capabilities.IsMobileDevice);
        }

        [Fact]
        public void WhenDOMManipulationRequested_ThenMobileDeviceCapabilityIsReturned()
        {
            var deviceCapabilities = new Dictionary<string, string> 
            {
                {AllCapabilities.DOMManipulation, "true"},
            };

            var capabilities = new MobileBrowserCapabilities(deviceCapabilities);

            Assert.Equal("true", capabilities[AllCapabilities.DOMManipulation]);
        }

        [Fact]
        public void WhenXHRRequested_ThenMobileDeviceCapabilityIsReturned()
        {
            var deviceCapabilities = new Dictionary<string, string> 
            {
                {AllCapabilities.XHR, "1"},
            };

            var capabilities = new MobileBrowserCapabilities(deviceCapabilities);

            Assert.True(capabilities.SupportsXmlHttp);
        }

        [Fact]
        public void WhenXHRNotFound_ThenXHRTypeCapabilityIsUsed()
        {
            var deviceCapabilities = new Dictionary<string, string> 
            {
                {AllCapabilities.XHRType, "standard"},
            };

            var capabilities = new MobileBrowserCapabilities(deviceCapabilities);

            Assert.True(capabilities.SupportsXmlHttp);
        }
    }
}
