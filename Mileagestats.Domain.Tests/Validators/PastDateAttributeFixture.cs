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
using Xunit;
using MileageStats.Domain.Validators;
using System.ComponentModel.DataAnnotations;

namespace MileageStats.Domain.Tests.Validators
{
    public class PastDateValidationAttributeFixture
    {
        [Fact]
        public void WhenValueNull_ValidationSucceeds()
        {
            var validator = new PastDateAttribute();

            var instance = new object();

            var datetime = (DateTime?)null;

            var context = new ValidationContext(instance, null, null);
            ValidationResult result = validator.GetValidationResult(datetime, context);

            Assert.Same(ValidationResult.Success, result);
        }

        [Fact]
        public void WhenValuePastDate_ValidationSucceeds()
        {
            var validator = new PastDateAttribute();

            var instance = new object();

            var datetime = (DateTime?)DateTime.UtcNow.AddDays(-1);

            var context = new ValidationContext(instance, null, null);
            ValidationResult result = validator.GetValidationResult(datetime, context);

            Assert.Same(ValidationResult.Success, result);
        }

        [Fact]
        public void WhenValueToday_ValidationSucceeds()
        {
            var validator = new PastDateAttribute();

            var instance = new object();

            var datetime = (DateTime?)DateTime.UtcNow;

            var context = new ValidationContext(instance, null, null);
            ValidationResult result = validator.GetValidationResult(datetime, context);

            Assert.Same(ValidationResult.Success, result);
        }

        [Fact]
        public void WhenValueFuture_ValidationFails()
        {
            var validator = new PastDateAttribute();

            var instance = new object();

            var datetime = (DateTime?)DateTime.UtcNow.AddDays(2);

            var context = new ValidationContext(instance, null, null);
            context.MemberName = "MyDate";
            ValidationResult result = validator.GetValidationResult(datetime, context);

            Assert.NotSame(ValidationResult.Success, result);
            Assert.NotNull(result.ErrorMessage);            
        }
    }
}
