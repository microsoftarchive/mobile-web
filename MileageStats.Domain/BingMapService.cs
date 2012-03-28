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
using System.Configuration;
using System.Linq;
using System.Text;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.SearchService;

namespace MileageStats.Domain
{
    public class BingMapService : IMapService
    {
        public IEnumerable<string> SearchKeywordLocation(string keyword, double latitude, double longitude)
        {
            var results = new List<string>();

            var searchRequest = new SearchRequest();

            // Set the credentials using a valid Bing Maps key
            searchRequest.Credentials = new SearchService.Credentials();
            searchRequest.Credentials.ApplicationId = GetBingMapsApplicationKey();

            //Create the search query
            var ssQuery = new StructuredSearchQuery();
            ssQuery.Keyword = keyword;
            ssQuery.Location = string.Format("{0}, {1}", latitude, longitude);
            searchRequest.StructuredQuery = ssQuery;

            //Make the search request 
            SearchResponse searchResponse;
            using (var searchService = new SearchServiceClient("BasicHttpBinding_ISearchService"))
            {
                searchResponse = searchService.Search(searchRequest);
            }

            foreach (var searchResult in searchResponse.ResultSets[0].Results)
            {
                results.Add(string.Format("{0} ({1})", searchResult.Name, searchResult.Distance));
            }
            return results;
        }

        private string GetBingMapsApplicationKey()
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("BingMapsApplicationKey") ||
                ConfigurationManager.AppSettings["BingMapsApplicationKey"] == "Insert Bing Maps Application Key here.")
            {
                throw new ArgumentException("Valid Bing Maps Application Key is missing. Please update appSettings in web.config with key BingMapsApplicationKey and appropriate value.");
            }
            return ConfigurationManager.AppSettings["BingMapsApplicationKey"];
        }
    }
}
