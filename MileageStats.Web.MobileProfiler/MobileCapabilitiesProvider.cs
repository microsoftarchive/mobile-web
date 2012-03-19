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
using System.Web.Configuration;
using System.Web;
using WURFL;
using MileageStats.Web.MobileProfiler.ClientProfile;

namespace MileageStats.Web.MobileProfiler
{
    // note: we break encapsulation and expose internal logic as public static readonly fields
    // in order to facilitate testing. this is primarily due to the difficult in mocking
    // the http request object
    public class MobileCapabilitiesProvider : HttpCapabilitiesDefaultProvider
    {
        readonly IWURFLManager _manager;
        readonly IProfileCookieEncoder _encoder;

        public MobileCapabilitiesProvider(IWURFLManager manager, IProfileCookieEncoder encoder)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (encoder == null)
                throw new ArgumentNullException("encoder");

            _manager = manager;
            _encoder = encoder;
        }

        public static readonly Func<HttpRequestBase, IWURFLManager, IDictionary<string, string>> DetermineCapsByWurfl = (request, manager) =>
        {
            // The user agent string in the current http request is used for getting
            // the device capabilities from the WURFL database
            var device = manager.GetDeviceForRequest(request.UserAgent);
            return device.GetCapabilities();
        };

        public static readonly Func<HttpRequestBase, IProfileCookieEncoder, IDictionary<string, string>> DetermineCapsByProfilingClient = (request, encoder) =>
        {
            // The profile cookie is parsed for getting the device capabilities inferred on
            // the client side
            var profileCookie = request.Cookies["profile"];
            
            return (profileCookie != null)
                ? encoder.GetDeviceCapabilities(profileCookie)
                : new Dictionary<string, string>();
        };

        public static readonly Func<HttpRequestBase, IDictionary<string, string>> DetermineCapsByAlgorithm = request =>
        {
            // we have some special cases where we can identify a device as 'mobile' 
            // even though it does not currently have an entry in our user agent
            // database (WURFL).
            var caps = new Dictionary<string, string>();
            if (request.UserAgent.Contains("Windows Phone OS"))
            {
                caps[AllCapabilities.MobileDevice] = "true";
                caps[AllCapabilities.Javascript] = "true";
                caps[AllCapabilities.DOMManipulation] = "true";
                caps[AllCapabilities.XHRType] = "standard";
            }
            return caps;
        };

        public override HttpBrowserCapabilities GetBrowserCapabilities(HttpRequest request)
        {
            var @base = base.GetBrowserCapabilities(request);

            var capabilities = GetBrowserCapabilities(new HttpRequestWrapper(request));

            return new MobileBrowserCapabilities(@base.Capabilities, capabilities);
        }

        public virtual IDictionary<string, string> GetBrowserCapabilities(HttpRequestBase request)
        {
            // the capabilities are merged in a specific order
            // because later sets override earlier sets
            var capabilities = new Dictionary<string, string>();
            capabilities.Merge(DetermineCapsByWurfl(request, _manager));
            capabilities.Merge(DetermineCapsByProfilingClient(request, _encoder));
            capabilities.Merge(DetermineCapsByAlgorithm(request));
            capabilities.Merge(DetermineIsMobileByWhiteList(request));

            return capabilities;
        }

        public static readonly Func<HttpRequestBase, IDictionary<string, string>> DetermineIsMobileByWhiteList =
            request =>
                {
                    // Identify desktop OS to determine if it is a desktop browser
                    // This will probably need to be updated in the future to accomodate other 
                    // desktop browsers. 
                    // Also, will need to decide what experience (mobile/desktop) to give tablets
                    // that run on OS's that are considered "desktop"
                    var caps = new Dictionary<string, string>();
                    if (request.UserAgent.Contains("Windows NT") || request.UserAgent.Contains("(Macintosh;"))
                    {
                        caps[AllCapabilities.MobileDevice] = "false";
                    }
                    else
                    {
                        caps[AllCapabilities.MobileDevice] = "true";
                    }
                    return caps;
                };
    }
}
