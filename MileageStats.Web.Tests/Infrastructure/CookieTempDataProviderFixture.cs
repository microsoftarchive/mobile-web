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
using System.Linq;
using System.Text;
using Xunit;
using MileageStats.Web.Infrastructure;
using System.Web;
using Moq;
using System.Web.Mvc;

namespace MileageStats.Web.Tests.Infrastructure
{
    public class CookieTempDataProviderFixture
    {
        [Fact]
        public void WhenConstructingWithNullHttpContext_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CookieTempDataProvider(null));
        }

        [Fact]
        public void WhenLoadingDataFromEmptyCookie_ThenReturnsEmptyDictionary()
        {
            var cookie = new HttpCookie("__ControllerTempData");

            cookie.Value = string.Empty;
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(r => r.Cookies).Returns(cookies);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            var provider = new CookieTempDataProvider(httpContextMock.Object);

            var tempData = provider.LoadTempData(null);
            Assert.NotNull(tempData);
            Assert.Equal(0, tempData.Count);
        }

        [Fact]
        public void WhenLoadTempDataFromUnexistingCookie_ThenReturnsEmptyTempDataDictionary()
        {
            var cookies = new HttpCookieCollection();

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(r => r.Cookies).Returns(cookies);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            var provider = new CookieTempDataProvider(httpContextMock.Object);

            var tempData = provider.LoadTempData(null);
            
            Assert.NotNull(tempData);
            Assert.Equal(0, tempData.Count);
        }

        [Fact]
        public void WhenLoadingTempDatFromValidCookie_ThenReturnsDictionaryWithEntries()
        {
            var cookie = new HttpCookie("__ControllerTempData");
            var initialTempData = new Dictionary<string, object>();
            initialTempData.Add("WhatIsInHere?", "Stuff");
            cookie.Value = CookieTempDataProvider.SerializeToBase64EncodedString(initialTempData);
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(r => r.Cookies).Returns(cookies);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            httpContextMock.Setup(c => c.Response).Returns((HttpResponseBase)null);

            var provider = new CookieTempDataProvider(httpContextMock.Object);

            var tempData = provider.LoadTempData(null);
            Assert.Equal("Stuff", tempData["WhatIsInHere?"]);
        }

        [Fact]
        public void WhenSavingDictionary_ThenDataGetsSerializedInCookie()
        {
            var cookies = new HttpCookieCollection();
            var responseMock = new Mock<HttpResponseBase>();
            responseMock.Setup(r => r.Cookies).Returns(cookies);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);

            var provider = new CookieTempDataProvider(httpContextMock.Object);
            var tempData = new Dictionary<string, object>();
            
            tempData.Add("Testing", "Turn it up to 11");
            tempData.Add("Testing2", 1.23);

            provider.SaveTempData(null, tempData);
            var cookie = cookies["__ControllerTempData"];
            var serialized = cookie.Value;
            var deserializedTempData = CookieTempDataProvider.DeserializeTempData(serialized);
            
            Assert.Equal("Turn it up to 11", deserializedTempData["Testing"]);
            Assert.Equal(1.23, deserializedTempData["Testing2"]);
        }
    }

}
