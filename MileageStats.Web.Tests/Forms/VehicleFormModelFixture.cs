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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MileageStats.Web.Models;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class VehicleFormModelFixture
    {
        [Fact]
        public void WhenConstructed_ThenSuccessful()
        {
            var actual = new VehicleFormModel();

            Assert.NotNull(actual);
            Assert.Equal(0, actual.VehicleId);
            Assert.Null(actual.Name);
        }

        [Fact]
        public void WhenIdSet_ThenValueUpdated()
        {
            var target = new VehicleFormModel();

            target.VehicleId = 4;

            int actual = target.VehicleId;
            Assert.Equal(4, actual);
        }

        [Fact]
        public void WhenNameSet_ThenValueUpdated()
        {
            var target = new VehicleFormModel();

            target.Name = "Name";

            string actual = target.Name;
            Assert.Equal("Name", actual);
        }

        [Fact]
        public void WhenNameSetToNull_ThenUpdatesValue()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";

            target.Name = null;

            string actual = target.Name;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenNameSetToValidValue_ThenValidationPasses()
        {
            var target = new VehicleFormModel();

            target.Name = "Name";

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenNameSetToNull_ThenValidationFails()
        {
            var target = new VehicleFormModel();

            target.Name = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Name", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenNameSetToEmpty_ThenValidationFails()
        {
            var target = new VehicleFormModel();

            target.Name = string.Empty;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Name", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenNameSetTo101Characters_ThenValidationFails()
        {
            var target = new VehicleFormModel();

            target.Name = new string('1', 101);

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Name", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenSortOrderSet_ThenValueUpdated()
        {
            var target = new VehicleFormModel();

            target.SortOrder = 1;

            int actual = target.SortOrder;
            Assert.Equal(1, actual);
        }

        [Fact]
        public void WhenSortOrderSetToZero_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.SortOrder = 0;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenSortOrderSetToPositive_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.SortOrder = 5;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenSortOrderSetToNegative_ThenValidationFails()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.SortOrder = -3;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("SortOrder", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenYearSet_ThenValueUpdated()
        {
            var target = new VehicleFormModel();

            target.Year = 1971;

            int? actual = target.Year;
            Assert.True(actual.HasValue);
            Assert.Equal(1971, actual.Value);
        }

        [Fact]
        public void WhenYearSetToNull_ThenUpdatesValue()
        {
            var target = new VehicleFormModel();
            target.Year = 1971;

            target.Year = null;

            int? actual = target.Year;
            Assert.False(actual.HasValue);
        }

        [Fact]
        public void WhenMakeNameSet_ThenValueUpdated()
        {
            var target = new VehicleFormModel();

            target.MakeName = "MakeName";

            string actual = target.MakeName;
            Assert.Equal("MakeName", actual);
        }

        [Fact]
        public void WhenMakeNameSetToNull_ThenUpdatesValue()
        {
            var target = new VehicleFormModel();
            target.MakeName = "MakeName";

            target.MakeName = null;

            string actual = target.MakeName;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenMakeNameSetToValidValue_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.MakeName = "MakeName";

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenMakeNameSetToNull_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.MakeName = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenMakeNameSetToEmpty_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.MakeName = string.Empty;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenMakeNameSetTo51Characters_ThenValidationFails()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.MakeName = new string('1', 51);

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("MakeName", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenModelNameSet_ThenValueUpdated()
        {
            var target = new VehicleFormModel();

            target.ModelName = "ModelName";

            string actual = target.ModelName;
            Assert.Equal("ModelName", actual);
        }

        [Fact]
        public void WhenModelNameSetToNull_ThenUpdatesValue()
        {
            var target = new VehicleFormModel();
            target.ModelName = "ModelName";

            target.ModelName = null;

            string actual = target.ModelName;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenModelNameSetToValidValue_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.ModelName = "ModelName";

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenModelNameSetToNull_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.ModelName = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenModelNameSetToEmpty_ThenValidationPasses()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.ModelName = string.Empty;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenModelNameSetTo51Characters_ThenValidationFails()
        {
            var target = new VehicleFormModel();
            target.Name = "Name";
            target.ModelName = new string('1', 51);

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("ModelName", validationResults[0].MemberNames.First());
        }
    }
}