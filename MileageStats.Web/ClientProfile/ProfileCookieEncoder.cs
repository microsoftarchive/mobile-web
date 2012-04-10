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
using System.Web;
using System.Web.Script.Serialization;

namespace MileageStats.Web.ClientProfile
{
    public class ProfileCookieEncoder : IProfileCookieEncoder
    {
        public IDictionary<string, string> GetDeviceCapabilities(HttpCookie profileCookie)
        {
            var value = HttpUtility.UrlDecode(profileCookie.Value);

            try
            {
                // Parses the http cookie with the device capabilities that
                // was encoded as json
                var serializer = new JavaScriptSerializer();
                var clientProfile = serializer.DeserializeObject(value) as IDictionary<string, object>;

                var capabilities = new Dictionary<string, string>();
                foreach (var key in clientProfile.Keys)
                {
                    capabilities.Add(key, clientProfile[key].ToString());
                }

                return capabilities;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The profile cookie could not be parsed", ex);
            }
        }
    }
}
