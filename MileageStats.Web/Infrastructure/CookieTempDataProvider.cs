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
using System.Web.Mvc;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MileageStats.Web.Infrastructure
{
    /// <summary>
    /// This implementation replaces the built-in provider that uses the Http session
    /// for storing the temporary messages. All the Ajax calls to an MVC controller that requires
    /// the use of a session are serialized and cannot be executed in parallel. By disabling the session
    /// and using this implementation, better performance results can be obtained.
    /// </summary>
    public class CookieTempDataProvider : ITempDataProvider
    {
        internal const string TempDataCookieKey = "__ControllerTempData";
        
        HttpContextBase _httpContext;

        public CookieTempDataProvider(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            _httpContext = httpContext;
        }

        public HttpContextBase HttpContext
        {
            get
            {
                return _httpContext;
            }
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var cookie = _httpContext.Request.Cookies[TempDataCookieKey];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                var deserializedTempData = DeserializeTempData(cookie.Value);

                cookie.Expires = DateTime.MinValue;
                cookie.Value = string.Empty;

                if (_httpContext.Response != null && _httpContext.Response.Cookies != null)
                {
                    var responseCookie = _httpContext.Response.Cookies[TempDataCookieKey];
                    if (responseCookie != null)
                    {
                        cookie.Expires = DateTime.MinValue;
                        cookie.Value = string.Empty;
                    }
                }

                return deserializedTempData;
            }

            return new Dictionary<string, object>();
        }

        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            string cookieValue = SerializeToBase64EncodedString(values);

            var cookie = new HttpCookie(TempDataCookieKey);
            cookie.HttpOnly = true;
            cookie.Value = cookieValue;

            _httpContext.Response.Cookies.Add(cookie);
        }

        public static IDictionary<string, object> DeserializeTempData(string base64EncodedSerializedTempData)
        {
            var bytes = Convert.FromBase64String(base64EncodedSerializedTempData);
            using (var memoryStream = new MemoryStream(bytes))
            {
                var binFormatter = new BinaryFormatter();

                var tempData = binFormatter.Deserialize(memoryStream, null) as IDictionary<string, object>;

                return tempData;
            }
        }

        public static string SerializeToBase64EncodedString(IDictionary<string, object> values)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binFormatter = new BinaryFormatter();
                binFormatter.Serialize(memoryStream, values);
                                
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }

}