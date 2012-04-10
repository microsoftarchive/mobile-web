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
using MileageStats.Domain.Validators;
using Xunit;

namespace MileageStats.Domain.Tests.Validators
{
    public class TextMultilineInputValidatorAttributeFixture
    {
        [Fact]
        public void WhenTextInputLineHasMultipleLines_ThenPassesValidation()
        {
            var validator = new TextMultilineValidatorAttribute();

            validator.Validate("\r\r\n\r", "TestName");
        }

        [Fact]
        public void WhenTextInputLineHasMultipleValidLines_ThenPassesValidation()
        {
            var validator = new TextMultilineValidatorAttribute();

            validator.Validate("ABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz_'-,.", "TestName");
        }

        [Fact]
        public void WhenTextInputLineHasAnInvalidLine_ThenFailsValidation()
        {
            var validator = new TextMultilineValidatorAttribute();

            Assert.Throws<ValidationException>(() =>
                                               validator.Validate(
                                                   "ABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz$", "TestName"));
        }

        [Fact]
        public void WhenTextInputLineHasDoubleHyphen_ThenFailsValidation()
        {
            var validator = new TextMultilineValidatorAttribute();

            Assert.Throws<ValidationException>(() =>
                                               validator.Validate(
                                                   "ABCDEF--GHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwx--yz$", "TestName"));
        }
    }
}