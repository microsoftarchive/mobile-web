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
using System.Linq;
using System.Text;
using Xunit;
using MileageStats.Web.Controllers;
using System.Web.Routing;
using MileageStats.Web.Tests.Mocks;
using Moq;
using System.Web;
using System.Web.Mvc;
using MileageStats.Web.Models;

namespace MileageStats.Web.Tests.Controllers
{
    public class ViewSwitcherControllerFixture
    {
        [Fact]
        public void WhenViewSwitchedToMobile_ThenViewSwitchCookieIsSetWithTrue()
        {
            var browserCapabilities = new Mock<HttpBrowserCapabilitiesBase>();
            browserCapabilities.SetupGet(b => b.IsMobileDevice).Returns(true);

            var controller = GetTestableViewSwitcherController();
            controller.Request.SetHttpBrowserCapabilities(browserCapabilities.Object);

            var cookies = new HttpCookieCollection();
            controller.Response.SetHttpCookies(cookies);

            cookies.Add(new HttpCookie("ViewSwitcher"));

            var result = controller.SwitchView(true, "http://test");

            Assert.NotNull(cookies["ViewSwitcher"]);
            Assert.Equal("true", cookies["ViewSwitcher"]["Mobile"]);
        }

        [Fact]
        public void WhenViewSwitchedToDesktop_ThenViewSwitchCookieIsSetWithFalse()
        {
            var browserCapabilities = new Mock<HttpBrowserCapabilitiesBase>();
            browserCapabilities.SetupGet(b => b.IsMobileDevice).Returns(true);

            var controller = GetTestableViewSwitcherController();
            controller.Request.SetHttpBrowserCapabilities(browserCapabilities.Object);

            var cookies = new HttpCookieCollection();
            controller.Response.SetHttpCookies(cookies);

            cookies.Add(new HttpCookie("ViewSwitcher"));

            var result = controller.SwitchView(false, "http://test");

            Assert.NotNull(cookies["ViewSwitcher"]);
            Assert.Equal("false", cookies["ViewSwitcher"]["Mobile"]);
        }

        // returns controller with mocks
        private TestableViewSwitcherController GetTestableViewSwitcherController()
        {
            var c = new TestableViewSwitcherController();
            c.SetFakeControllerContext();
            c.InvokeInitialize(c.ControllerContext.RequestContext);

            return c;
        }

        internal class TestableViewSwitcherController : ViewSwitcherController
        {
            public TestableViewSwitcherController()
                : base()
            {
            }

            public void InvokeInitialize(RequestContext context)
            {
                base.Initialize(context);
            }
        }
    }
}
