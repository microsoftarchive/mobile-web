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

using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        // a utility function for merging a 'target' dictionary into the 'source'
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
        {
            if (target == null || source == null) return;

            foreach (var key in target.Keys)
            {
                if (source.ContainsKey(key))
                {
                    source[key] = target[key];
                }
                else
                {
                    source.Add(key, target[key]);
                }
            }
        }

        public static void Merge(this IDictionary source, IDictionary target)
        {
            if (target == null || source == null) return;

            foreach (var key in target.Keys)
            {
                if (source.Contains(key))
                {
                    source[key] = target[key];
                }
                else
                {
                    source.Add(key, target[key]);
                }
            }
        }

        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            var value = default(TValue);

            if (source.TryGetValue(key, out value))
            {
                return value;
            }

            return default(TValue);
        }

        public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, string value)
        {
            return source.ContainsKey(key) &&
                source[key].ToString().Equals(value, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool ContainsValue(this IDictionary source, string key, string value)
        {
            return source.Contains(key) &&
                source[key].ToString().Equals(value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}