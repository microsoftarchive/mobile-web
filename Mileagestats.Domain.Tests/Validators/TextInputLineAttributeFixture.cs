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

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MileageStats.Domain.Validators;
using Xunit;

namespace MileageStats.Domain.Tests.Validators
{
    public class TextLineValidatorAttributeFixture
    {
        [Fact]
        public void WhenTextInputLineHasValidCharacters_ThenPassesValidation()
        {
            var validator = new TextLineInputValidatorAttribute();

            validator.Validate(@"ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz_'-,.", "TestName");
        }

        [Fact]
        public void WhenTextInputLineHasMultipleLines_ThenFailsValidation()
        {
            var validator = new TextLineInputValidatorAttribute();

            Assert.Throws<ValidationException>(
                () => validator.Validate("ABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz", "TestName"));
        }

        [Fact]
        public void WhenTextInputLineHasInvalidCharacters_ThenFailsValidation()
        {
            var validator = new TextLineInputValidatorAttribute();

            Assert.Throws<ValidationException>(
                () => validator.Validate("AB<>C", "TestName"));
        }

        [Fact]
        public void WhenTextInputLineHasDoubleHyphen_ThenFailsValidation()
        {
            var validator = new TextLineInputValidatorAttribute();

            Assert.Throws<ValidationException>(
                () => validator.Validate("AB--C", "TestName"));
        }

        [Fact]
        public void WhenCreated_ThenImplementsIClientValidatable()
        {
            var validator = new TextLineInputValidatorAttribute();
            Assert.IsAssignableFrom<IClientValidatable>(validator);
        }

        [Fact]
        public void WhenGetClientValidationRules_ThenReturnsValidationRule()
        {
            var validationRules = new TextLineInputValidatorAttribute().GetClientValidationRules(null, null).ToList();
            Assert.Equal(1, validationRules.Count());
            Assert.Equal("textlineinput", validationRules[0].ValidationType);
            Assert.Equal("Only alpha-numeric characters and [.,_-'] are allowed.", validationRules[0].ErrorMessage);
            Assert.Equal(1, validationRules[0].ValidationParameters.Count);
            Assert.Equal(@"^(?!.*--)[A-Za-z0-9\.,'_ \-]*$", validationRules[0].ValidationParameters["pattern"]);
        }
    }
}