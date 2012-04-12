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
using MileageStats.Web.MobileProfiler;

namespace System.Web
{
    public static class HttpBrowserCapabilitiesExtensions
    {
        public static bool SupportsJSON(this HttpBrowserCapabilitiesBase httpBrowser)
        {
            return httpBrowser[AllCapabilities.JSON] == "1" ||
                httpBrowser[AllCapabilities.JSON] == "true";
        }

        public static bool SupportsDOMManipulation(this HttpBrowserCapabilitiesBase httpBrowser)
        {
            return httpBrowser[AllCapabilities.DOMManipulation] == "true";
        }

        public static bool SupportsHashChangeEvent(this HttpBrowserCapabilitiesBase httpBrowser)
        {
            return httpBrowser[AllCapabilities.HashChange] == "true";
        }

        public static bool IsWow(this HttpBrowserCapabilitiesBase httpBrowser)
        {
            return httpBrowser.IsMobileDevice &&
                   httpBrowser.SupportsJSON() &&
                   //httpBrowser.SupportsDOMManipulation() &&
                   httpBrowser.SupportsXmlHttp && 
                   httpBrowser.SupportsHashChangeEvent();
        }
    }
}
