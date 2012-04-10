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

using System.ComponentModel.DataAnnotations;
using System.Linq;
using MileageStats.Domain.Validators;
using Xunit;

namespace MileageStats.Domain.Tests.Validators
{
    public class AtLeastOnePropertyValidatorFixture
    {
        [Fact]
        public void WhenAllValuesNull_ValidationFails()
        {
            var validator = new AtLeastOneNonNullPropertyValidationAttribute("FirstValue", "SecondValue");
            var instance = new ForValidation
                               {
                                   FirstValue = null,
                                   SecondValue = null
                               };
            var context = new ValidationContext(instance, null, null);
            ValidationResult result = validator.GetValidationResult(instance, context);

            Assert.NotSame(ValidationResult.Success, result);
            Assert.NotNull(result.ErrorMessage);
            Assert.False(result.MemberNames.Except(new[] {"FirstValue", "SecondValue"}).Any());
        }

        [Fact]
        public void WhenOneValueSet_ValidationPasses()
        {
            var validator = new AtLeastOneNonNullPropertyValidationAttribute("FirstValue", "SecondValue");
            var instance = new ForValidation
                               {
                                   FirstValue = 1,
                                   SecondValue = null
                               };
            var context = new ValidationContext(instance, null, null);
            ValidationResult result = validator.GetValidationResult(instance, context);

            Assert.Same(ValidationResult.Success, result);
        }

        [Fact]
        public void WhenAllValuesSet_ValidationPasses()
        {
            var validator = new AtLeastOneNonNullPropertyValidationAttribute("FirstValue", "SecondValue");
            var instance = new ForValidation
                               {
                                   FirstValue = 1,
                                   SecondValue = 2
                               };
            var context = new ValidationContext(instance, null, null);
            ValidationResult result = validator.GetValidationResult(instance, context);

            Assert.Same(ValidationResult.Success, result);
        }

        #region Nested type: ForValidation

        private class ForValidation
        {
            public int? FirstValue { get; set; }
            public int? SecondValue { get; set; }
        }

        #endregion
    }
}