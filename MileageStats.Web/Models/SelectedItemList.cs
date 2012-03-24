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

namespace MileageStats.Web.Models
{
    public class SelectedItemList<T> : IEnumerable<T>
    {
        public T this[int i]
        {
            get { return List.ToArray()[i]; }
        }

        public SelectedItemList(IEnumerable<T> source, Func<IEnumerable<T>, T> setSelected)
            : this(source, setSelected(source))
        {
        }

        public SelectedItemList(IEnumerable<T> source) : this(source, default(T))
        {
        }

        public SelectedItemList(IEnumerable<T> source, T selectedItem)
        {
            List = source;
            SelectedItem = selectedItem;
        }

        public IEnumerable<T> List { get; private set; }
        public T SelectedItem { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}