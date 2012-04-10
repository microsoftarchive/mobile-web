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
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MileageStats.Web.Helpers.Mustache
{
    internal class DisposableEnumerable<T> : IEnumerable<T>
    {
        private readonly ViewContext _viewContext;
        private readonly string _sectionName;
        private readonly IEnumerable<T> _source;

        public DisposableEnumerable(ViewContext viewContext, string sectionName,
                                    Func<IEnumerable<T>> getItems = null)
        {
            _viewContext = viewContext;
            _sectionName = sectionName;
            _source = (getItems != null) ? getItems() : new List<T> {default(T)};

            if (string.IsNullOrEmpty(_sectionName)) return;
                
            string beginSection = string.Format("{{{{#{0}}}}}", _sectionName);
            _viewContext.Writer.WriteLine(beginSection);
        }

        private void WhenDisposing()
        {
            if (string.IsNullOrEmpty(_sectionName)) return;

            string endSection = string.Format("{{{{/{0}}}}}", _sectionName);
            _viewContext.Writer.WriteLine(endSection);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DisposableEnumerator<T>(_source.GetEnumerator(), WhenDisposing);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DisposableEnumerator<T>(_source.GetEnumerator(), WhenDisposing);
        }
    }
}