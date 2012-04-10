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
using System.ComponentModel.DataAnnotations;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AtLeastOneNonNullPropertyValidationAttribute : ValidationAttribute
    {
        private readonly string[] propertyNames;

        public AtLeastOneNonNullPropertyValidationAttribute(params string[] propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (propertyName.Length < 2)
                throw new ArgumentException("There should be at least two properties specified.", "propertyName");
            this.propertyNames = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Since this attribute can only be applied to a class level, value will always be
            // the object under test.
            var instance = value;

            var targetType = instance.GetType();
            foreach (var propertyName in this.propertyNames)
            {
                var propertyInfo = targetType.GetProperty(propertyName);
                var propertyValue = propertyInfo.GetValue(instance, null);
                if (propertyValue != null) return ValidationResult.Success;
            }

            var errorMessage = this.ErrorMessageString;
            if (String.IsNullOrEmpty(errorMessage))
            {
                errorMessage = Resources.AtLeastOneNonNullPropertyValidationAttribute_IsValid_OnePropertyMustHaveValue;
            }

            return new ValidationResult(errorMessage, this.propertyNames);
        }
    }
}