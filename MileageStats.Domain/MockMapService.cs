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
using MileageStats.Domain.Contracts;
namespace MileageStats.Domain
{
    public class MockMapService : IMapService
    {
        public IEnumerable<string> SearchKeywordLocation(string keyword, double latitude, double longitude)
        {
            var searchResultList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                searchResultList.Add(string.Format("{0}{1}", keyword, i));
            }
            return searchResultList;
        }

        public string ReverseGeoCodeLocationCountry(double latitude, double longitude)
        {
            return "United States";
        }
    }
}
