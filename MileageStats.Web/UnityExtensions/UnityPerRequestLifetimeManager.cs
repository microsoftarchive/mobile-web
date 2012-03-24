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

namespace MileageStats.Web.UnityExtensions
{
    public class UnityPerRequestLifetimeManager : Microsoft.Practices.Unity.LifetimeManager, IDisposable
    {
        private readonly IPerRequestStore _contextStore;
        private readonly Guid key = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of UnityPerRequestLifetimeManager with a per-request store.
        /// </summary>
        /// <param name="contextStore"></param>
        public UnityPerRequestLifetimeManager(IPerRequestStore contextStore)
        {
            _contextStore = contextStore;
            _contextStore.EndRequest += EndRequestHandler;
        }

        public override object GetValue()
        {
            return _contextStore.GetValue(key);
        }

        public override void SetValue(object newValue)
        {
            _contextStore.SetValue(key, newValue);
        }

        public override void RemoveValue()
        {
            var oldValue = _contextStore.GetValue(key);
            _contextStore.RemoveValue(key);

            var disposable = oldValue as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            RemoveValue();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void EndRequestHandler(object sender, EventArgs e)
        {
            RemoveValue();
        }
    }
}