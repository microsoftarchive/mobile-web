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
using System.Web.Mvc;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Validators
{
    public class TextLineInputValidatorAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public TextLineInputValidatorAttribute() : base(Resources.TextLineInputValidatorRegEx)
        {
            this.ErrorMessage = Resources.InvalidInputCharacter;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = Resources.InvalidInputCharacter,
                ValidationType = "textlineinput"
            };

            rule.ValidationParameters.Add("pattern", Resources.TextLineInputValidatorRegEx);
            return new List<ModelClientValidationRule>() {rule};
        }
    }
}