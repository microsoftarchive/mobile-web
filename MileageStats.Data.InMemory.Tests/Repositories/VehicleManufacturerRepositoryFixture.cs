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

using Xunit;

namespace MileageStats.Data.InMemory.Tests.Repositories
{
    public class VehicleManufacturerRepositoryFixture
    {
        [Fact]
        public void WhenConstructingWithResolver_ThenSuccessful()
        {
            var actual = new VehicleManufacturerRepository();

            Assert.NotNull(actual);
        }

        [Fact]
        public void WhenGetYears_ReturnsYears()
        {
            var target = new VehicleManufacturerRepository();

            int[] actual = target.GetYears();

            Assert.NotNull(actual);
            Assert.NotEqual(0, actual.Length);
        }

        [Fact]
        public void WhenGetMakesWithValidYear_ReturnsMakes()
        {
            var target = new VehicleManufacturerRepository();

            string[] actual = target.GetMakes(2011);

            Assert.NotNull(actual);
            Assert.NotEqual(0, actual.Length);
        }

        [Fact]
        public void WhenGetMakesWithInvalidYear_ReturnsNull()
        {
            var target = new VehicleManufacturerRepository();

            string[] actual = target.GetMakes(3211);

            Assert.Null(actual);
        }

        [Fact]
        public void WhenGetModelsWithValidYearAndMake_ReturnsModels()
        {
            var target = new VehicleManufacturerRepository();

            string[] actual = target.GetModels(2010, "BMW");

            Assert.NotNull(actual);
            Assert.NotEqual(0, actual.Length);
        }

        [Fact]
        public void WhenGetModelsWithInvalidYear_ReturnsNull()
        {
            var target = new VehicleManufacturerRepository();

            string[] actual = target.GetModels(3211, "BMW");

            Assert.Null(actual);
        }

        [Fact]
        public void WhenGetModelsWithInvalidMake_ReturnsNull()
        {
            var target = new VehicleManufacturerRepository();

            string[] actual = target.GetModels(2011, "HotWheels");

            Assert.Null(actual);
        }

        [Fact]
        public void WhenIsValidYearWithValidYear_ReturnsTrue()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidYear(2010);

            Assert.True(actual);
        }

        [Fact]
        public void WhenIsValidYearWithInvalidYear_ReturnsFalse()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidYear(3015);

            Assert.False(actual);
        }

        [Fact]
        public void WhenIsValidMakeWithValidYearAndMake_ReturnsTrue()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidMake(2010, "Audi");

            Assert.True(actual);
        }

        [Fact]
        public void WhenIsValidMakeWithInvalidYear_ReturnsFalse()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidMake(3022, "Audi");

            Assert.False(actual);
        }

        [Fact]
        public void WhenIsValidMakeWithInvalidMake_ReturnsFalse()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidMake(2010, "Innee");

            Assert.False(actual);
        }

        [Fact]
        public void WhenIsValidModelWithValidYearAndMake_ReturnsTrue()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidModel(2010, "Audi", "A4");

            Assert.True(actual);
        }

        [Fact]
        public void WhenIsValidModelWithInvalidYear_ReturnsFalse()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidModel(3022, "Audi", "A4");

            Assert.False(actual);
        }

        [Fact]
        public void WhenIsValidModelWithInvalidMake_ReturnsFalse()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidModel(2010, "Innee", "A4");

            Assert.False(actual);
        }

        [Fact]
        public void WhenIsValidModelWithInvalidModel_ReturnsFalse()
        {
            var target = new VehicleManufacturerRepository();

            bool actual = target.IsValidModel(2010, "Audi", "A422L16");

            Assert.False(actual);
        }
    }
}