using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Web;
using Moq;

namespace MileageStats.Web.Tests.Helpers
{
    public class BrowserExtensionsFixture
    {
        [Fact]
        public void WhenCleanOverrridenBrowser_ThenViewSwitcherCookieIsRemoved()
        {
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("ViewSwitcher", "mobile"));

            var browser = Mock.Of<HttpContextBase>(h => h.Response.Cookies == cookies);
            
            BrowserExtensions.ClearOverriddenBrowser(browser);

            Assert.True(cookies.Count == 0);
        }

        [Fact]
        public void WhenSetOverriddenBrowserToMobile_ThenViewSwitcherCookiesIsSetToTrue()
        {
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("ViewSwitcher", "mobile"));

            var browser = Mock.Of<HttpContextBase>(h => h.Response.Cookies == cookies);

            BrowserExtensions.SetOverriddenBrowser(browser, BrowserOverride.Mobile);

            Assert.True(cookies["ViewSwitcher"]["Mobile"] == "true");
        }

        [Fact]
        public void WhenSetOverriddenBrowserToDesktop_ThenViewSwitcherCookiesIsSetToFalse()
        {
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("ViewSwitcher", "mobile"));

            var browser = Mock.Of<HttpContextBase>(h => h.Response.Cookies == cookies);

            BrowserExtensions.SetOverriddenBrowser(browser, BrowserOverride.Desktop);

            Assert.True(cookies["ViewSwitcher"]["Mobile"] == "false");
        }

        [Fact]
        public void WhenGettingOverridenBrowser_ThenViewSwitcherCookieIsParsed()
        {
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("ViewSwitcher"));

            cookies["ViewSwitcher"]["Mobile"] = "true";

            var browser = Mock.Of<HttpContextBase>(h => h.Request.Cookies == cookies && h.Request.Browser.IsMobileDevice == false);

            var value = BrowserExtensions.GetOverridenBrowser (browser);

            Assert.Equal(BrowserOverride.Mobile, value);
        }

    }
}
