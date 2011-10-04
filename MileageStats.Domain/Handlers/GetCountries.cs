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
using System.Text;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using MileageStats.Domain.Contracts.Data;

namespace MileageStats.Domain.Handlers
{
    public class GetCountries
    {
        private readonly ICountryRepository countryRepository;

        public GetCountries(ICountryRepository countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        public virtual ReadOnlyCollection<string> Execute()
        {
            var countriesList = this.countryRepository.GetAll().Select(c => c.Name).ToList();
            return new ReadOnlyCollection<string>(countriesList);
        }

        public SelectList GetCountrySelectList()
        {
            var countryNames = this.Execute()
                .Select(country => new { text = country, value = country });

            return new SelectList(countryNames, "value", "text");
        }
    }
}
