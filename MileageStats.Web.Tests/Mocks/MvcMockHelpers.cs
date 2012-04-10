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
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using System.Web.Caching;

//
// Courtesy of Scott Hanselman: http://www.hanselman.com/blog/ASPNETMVCSessionAtMix08TDDAndMvcMockHelpers.aspx
//        and Daniel Cazzulino: http://www.clariusconsulting.net/blogs/kzu/
//

namespace MileageStats.Web.Tests.Mocks
{
    public static class MvcMockHelpers
    {
        public static HttpContextBase FakeHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            return context.Object;
        }

        public static HttpContextBase FakeHttpContext(string url)
        {
            HttpContextBase context = FakeHttpContext();
            context.Request.SetupRequestUrl(url);
            return context;
        }

        public static void SetFakeControllerContext(this Controller controller)
        {
            var httpContext = FakeHttpContext();
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()),
                                                              controller);
            controller.ControllerContext = context;
        }

        public static void SetHttpBrowserCapabilities(this HttpRequestBase request,
                                                      HttpBrowserCapabilitiesBase httpBrowserCapabilities)
        {
            Mock.Get(request)
                .Setup(p => p.Browser)
                .Returns(httpBrowserCapabilities);
        }

        public static void SetHttpCookies(this HttpResponseBase response,
            HttpCookieCollection cookies)
        {
            Mock.Get(response)
                .SetupGet(r => r.Cookies)
                .Returns(cookies);
        }

        public static void SetHttpCookies(this HttpRequestBase request,
           HttpCookieCollection cookies)
        {
            Mock.Get(request)
                .SetupGet(r => r.Cookies)
                .Returns(cookies);
        }

        private static string GetUrlFileName(string url)
        {
            if (url.Contains("?"))
                return url.Substring(0, url.IndexOf("?"));
            else
                return url;
        }

        private static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                NameValueCollection parameters = new NameValueCollection();

                string[] parts = url.Split("?".ToCharArray());
                string[] keys = parts[1].Split("&".ToCharArray());

                foreach (string key in keys)
                {
                    string[] part = key.Split("=".ToCharArray());
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            else
            {
                return null;
            }
        }

        public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }


        public static void SetAjaxRequest(this HttpRequestBase request)
        {
            var headers = new NameValueCollection();

            Mock.Get(request).SetupGet(x => x.Headers).Returns(headers);            

            headers["X-Requested-With"] = "XMLHttpRequest";           
        }

        public static void SetJsonRequest(this HttpRequestBase request)
        {          
            Mock.Get(request)
                .SetupGet(req => req.ContentType)
                .Returns("application/json");
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            // rlb:  we need fully qualified uris.
            //if (!url.StartsWith("~/"))
            //    throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));

            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(GetUrlFileName(url));

            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);

            mock.SetupGet(req => req.Url)
                .Returns(new Uri(url, UriKind.RelativeOrAbsolute));

            mock.SetupGet(req => req.UrlReferrer)
                .Returns(new Uri("http://referrer", UriKind.Absolute));
        }
    }
}