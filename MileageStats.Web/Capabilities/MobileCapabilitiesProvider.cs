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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Configuration;
using System.Web.WebPages;
using MileageStats.Web.ClientProfile;

namespace MileageStats.Web.Capabilities
{
    // note: we break encapsulation and expose internal logic as public static readonly fields
    // in order to facilitate testing. this is primarily due to the difficulty in mocking
    // the http request object
    public class MobileCapabilitiesProvider : HttpCapabilitiesDefaultProvider
    {
        private readonly IProfileCookieEncoder _encoder;

        public MobileCapabilitiesProvider(IProfileCookieEncoder encoder)
        {
            if (encoder == null)
                throw new ArgumentNullException("encoder");

            _encoder = encoder;
        }

        public override HttpBrowserCapabilities GetBrowserCapabilities(HttpRequest request)
        {
            var httpContext = request.RequestContext.HttpContext;
            var browser = base.GetBrowserCapabilities(request);

            SetDefaults(browser);

            // The default HttpBrowserCapabilities is optimized to only
            // retrieved the values from the underlying dictionary once.
            // This means that if you examine it in the debugger prior to 
            // merging in the additional capabilities, you won't see
            // the new values when accessing them through the convenience
            // properties, even though the correct values are present in
            // the underlying dictionary.
            browser.Capabilities.Merge(GetAdditionalCapabilities(httpContext));

            return browser;
        }

        public virtual IDictionary GetAdditionalCapabilities(HttpContextBase context)
        {
            var capabilities = new Dictionary<string, string>();

            if (BrowserOverrideStores.Current.GetOverriddenUserAgent(context) != null) return capabilities;

            // the capabilities are merged in a specific order
            // because later sets override earlier sets
            capabilities.Merge(DetermineCapsBy3rdPartyDatabase(context));
            capabilities.Merge(DetermineCapsByProfilingClient(context.Request, _encoder));

            return capabilities;
        }

        public static readonly Func<HttpContextBase, IDictionary<string, string>> DetermineCapsBy3rdPartyDatabase =
            context =>
            {
                // We're not actually using a 3rd party database, but if you are (and you need
                // to manually merge in the capabilities) then you could do so here.
                // In this simulated database, we look for a known OS to determine if it is a desktop browser.
                // This is not meant to represent production code - only to simulate what
                // a 3rd party database would provide you with
                var caps = new Dictionary<string, string>();
                var ua = context.Request.UserAgent;
                if (ua.Contains("Windows NT") || ua.Contains("Macintosh") || ua.Contains("Windows+XP"))
                {
                    caps[AllCapabilities.MobileDevice] = "false";
                }
                else
                {
                    caps[AllCapabilities.MobileDevice] = "true";
                }
                return caps;
            };

        public static readonly Func<HttpRequestBase, IProfileCookieEncoder, IDictionary<string, string>> DetermineCapsByProfilingClient =
            (request, encoder) =>
            {
                // The profile cookie is parsed for getting the device capabilities inferred on
                // the client side
                var profileCookie = request.Cookies["profile"];

                return (profileCookie != null)
                        ? encoder.GetDeviceCapabilities(profileCookie)
                        : new Dictionary<string, string>();
            };

        private static void SetDefaults(HttpCapabilitiesBase source)
        {
            // We'll assume a reasonable width for the browser, but we'll
            // do so before any other capabilities are detected. This way
            // our default will be overriden if an actual value is found 
            // in one of our sources.
            if(!source.Capabilities.Contains(AllCapabilities.Width))
                source.Capabilities[AllCapabilities.Width] = AllCapabilities.DefaultWidth.ToString(CultureInfo.InvariantCulture);
        }
    }
}