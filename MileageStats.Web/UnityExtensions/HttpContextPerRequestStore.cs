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

namespace MileageStats.Web.UnityExtensions
{
    public class HttpContextPerRequestStore : IPerRequestStore
    {
        //public HttpContextPerRequestStore()
        //{
        //    if (HttpContext.Current.ApplicationInstance != null)
        //    {
        //        // Note: We'd like to do this, but you cannot sign up for the EndRequest 
        //        // from this application instance as it is actually different than the one 
        //        // the EndRequest handler is actually invoked from.
        //        //HttpContext.Current.ApplicationInstance.EndRequest += this.EndRequestHandler;
        //    }
        //}

        public object GetValue(object key)
        {
            return HttpContext.Current.Items[key];
        }

        public void SetValue(object key, object value)
        {
            HttpContext.Current.Items[key] = value;
        }

        public void RemoveValue(object key)
        {
            HttpContext.Current.Items.Remove(key);
        }

        private void EndRequestHandler(object sender, EventArgs e)
        {
            EventHandler handler = EndRequest;
            if (handler != null) handler(this, e);
        }

        public event EventHandler EndRequest;
    }
}