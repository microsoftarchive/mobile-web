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
using System.Web.Mvc;

namespace MileageStats.Web
{
    public static class UrlExtensions
    {
        // occasionally we need to generate a url that is publicly accessible
        // for example, in the case of authenticating with a 3rd party we need to provide
        // a return url for the 3rd party invoke. 
        // in some circumstances, such as hosting the app with a service provider that handles
        // load balances by forwarding the request internally on different ports, the standard 
        // approach for generating a url in ASP.NET may use one of the internal ports. 
        // ToPublicUrl helps us a create a url that will be publically accessible.
        public static string ToPublicUrl(this UrlHelper urlHelper, string action, string controller)
        {
            var uri = new Uri(urlHelper.Action(action, controller), UriKind.Relative);
            return ToPublicUrl(urlHelper, uri);
        }
        
        public static string ToPublicUrl(this UrlHelper urlHelper, Uri relativeUri)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            return ToPublicUrl(request,relativeUri);
        }

        public static string ToPublicUrl(this HttpRequestBase request, Uri relativeUri)
        {
            var url = request.Url;
            return ToPublicUrl(url.Host,url.Scheme,url.Port,request.IsLocal, relativeUri);
        }

        public static string ToPublicUrl(this HttpRequest request, Uri relativeUri)
        {
            var url = request.Url;
            return ToPublicUrl(url.Host, url.Scheme, url.Port, request.IsLocal, relativeUri);
        }

        //note: this assume that port 80 will always be used when not running locally
        private static string ToPublicUrl(string host, string scheme, int port, bool isLocal, Uri relativeUri)
        {
            var uriBuilder = new UriBuilder
            {
                Host = host,
                Path = "/",
                Port = 80,
                Scheme = scheme,
            };

            if (isLocal)
            {
                uriBuilder.Port = port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}