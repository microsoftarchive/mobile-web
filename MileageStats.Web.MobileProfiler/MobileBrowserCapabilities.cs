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

using System.Collections.Generic;
using System.Web;
using System.Collections;

namespace MileageStats.Web.MobileProfiler
{
    public class MobileBrowserCapabilities : HttpBrowserCapabilities
    {
        readonly IDictionary<string, string> _deviceCapabilities;
        readonly IDictionary _baseCapabilities;

        public MobileBrowserCapabilities(IDictionary baseCapabilities, IDictionary<string, string> deviceCapabilities)
        {
            _deviceCapabilities = deviceCapabilities;
            _baseCapabilities = baseCapabilities;
        }

        public override string this[string key]
        {
            get
            {
                return GetDeviceCapability(key);
            }
        }

        protected virtual string GetDeviceCapability(string key)
        {
            switch (key)
            {
                case "javascript":
                    return _deviceCapabilities.TryGetValue(AllCapabilities.Javascript);
                case "supportsXmlHttp":
                    {
                        if (_deviceCapabilities.ContainsKey(AllCapabilities.XHR) &&
                            _deviceCapabilities[AllCapabilities.XHR] == "1")
                        {
                            return "true";
                        }
                        else if (_deviceCapabilities.ContainsKey(AllCapabilities.XHRType) &&
                            !_deviceCapabilities[AllCapabilities.XHRType].Equals("none"))
                        {
                            return "true";
                        }

                        return null;
                    }
                case "isMobileDevice":
                    return _deviceCapabilities.TryGetValue(AllCapabilities.MobileDevice);
                case "cookies":
                    return _deviceCapabilities.TryGetValue(AllCapabilities.Cookies);
                case "screenPixelsWidth":
                    {
                        if (_deviceCapabilities.ContainsKey(AllCapabilities.Width))
                        {
                            return _deviceCapabilities[AllCapabilities.Width];
                        }
                        return AllCapabilities.DefaultWidth.ToString();
                    }
                default:
                    {
                        string capability = null;

                        if (_deviceCapabilities.TryGetValue(key, out capability))
                            return capability;

                        return (string)_baseCapabilities[key];
                    }
            }
        }
    }
}
