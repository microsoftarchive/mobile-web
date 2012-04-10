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
using System.Collections.Generic;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using Moq;
using Xunit;
using MileageStats.Domain.Handlers;

namespace MileageStats.Domain.Tests
{
    public class WhenGettingCountryList
    {
        [Fact]
        public void WhenGettingCountryList_ThenReturnsCountryNames()
        {
            var countryRepositoryMock = new Mock<ICountryRepository>();
            countryRepositoryMock.Setup(c => c.GetAll()).Returns(new List<Country>() {new Country()});
            var services = new GetCountries(countryRepositoryMock.Object);

            var countries = services.Execute();

            Assert.NotNull(countries);
            Assert.Equal(1, countries.Count);
        }

        [Fact]
        public void WhenGettingCountryListAndRepositoryReturnsNoRecords_ThenReturnsEmptyCollection()
        {
            var countryRepositoryMock = new Mock<ICountryRepository>();
            countryRepositoryMock.Setup(c => c.GetAll()).Returns(new List<Country>());
            var services = new GetCountries(countryRepositoryMock.Object);

            var countries = services.Execute();

            Assert.Empty(countries);
        }
    }
}