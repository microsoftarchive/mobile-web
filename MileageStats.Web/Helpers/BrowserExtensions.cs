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
using System.Web;

namespace MileageStats.Web
{
    public enum BrowserOverride
    {
        Desktop = 0,
        Mobile = 1,
    }

    public static class BrowserExtensions
    {
        public static void ClearOverriddenBrowser(this HttpContextBase httpContext)
        {
            httpContext.Response.Cookies.Remove("ViewSwitcher");
        }

        public static void SetOverriddenBrowser(this HttpContextBase httpContext, BrowserOverride browserOverride)
        {
            httpContext.Response.Cookies["ViewSwitcher"]["Mobile"] = (browserOverride == BrowserOverride.Mobile ? "true" : "false");
        }

        public static BrowserOverride GetOverridenBrowser(this HttpContextBase httpContext)
        {
            var request = httpContext.Request;

            var isMobileMode = request.Browser.IsMobileDevice;

            if (request.Cookies["ViewSwitcher"] != null && request.Cookies["ViewSwitcher"]["Mobile"] != null)
            {
                isMobileMode = String.Equals(request.Cookies["ViewSwitcher"]["Mobile"], "true");
            }

            return (isMobileMode) ? BrowserOverride.Mobile : BrowserOverride.Desktop;
        }
    }
}