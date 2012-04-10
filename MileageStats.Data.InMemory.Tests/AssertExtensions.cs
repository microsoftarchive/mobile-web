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
using System.Linq;
using Xunit.Sdk;

namespace MileageStats.Data.InMemory.Tests
{
    public static class AssertExtensions
    {
        /// <summary>
        /// Compares two objects set of properties
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="properties"></param>
        public static void PropertiesAreEqual<T>(T object1, T object2, params string[] properties)
            where T : class
        {
            if (object1 == null) throw new ArgumentNullException("object1");
            if (object2 == null) throw new ArgumentNullException("object2");

            var compareProperties = typeof(T).GetProperties().Where(p => properties.Contains(p.Name));

            foreach (var property in compareProperties)
            {
                var object1PropertyValue = property.GetValue(object1, null);
                var object2PropertyValue = property.GetValue(object2, null);

                if (!object.Equals(object1PropertyValue, object2PropertyValue))
                {
                    throw new AssertActualExpectedException(object1PropertyValue, object2PropertyValue,
                                                            "Property values are not equal.");
                }
            }
        }
    }
}