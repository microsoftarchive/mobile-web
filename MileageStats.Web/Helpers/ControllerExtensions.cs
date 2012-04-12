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
using System.Web.Mvc;
using MileageStats.Domain.Contracts;

namespace MileageStats.Web
{
    /// <summary>
    /// Extension methods to Controllers within the MileageStats application.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Adds a model error for each validation result from the business service.
        /// </summary>
        /// <param name="validationResults">The validation results from a business service.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="defaultErrorKey">The default key to use if a field is not specified in a business service validation result.</param>
        public static void AddModelErrors(this Controller controller, IEnumerable<ValidationResult> validationResults, string defaultErrorKey = null)
        {
            if (validationResults != null)
            {
                foreach (var validationResult in validationResults)
                {
                    if (!string.IsNullOrEmpty(validationResult.MemberName))
                    {
                        controller.ModelState.AddModelError(validationResult.MemberName, validationResult.Message);
                    }
                    else if (defaultErrorKey != null)
                    {
                        controller.ModelState.AddModelError(defaultErrorKey, validationResult.Message);
                    }
                    else
                    {
                        controller.ModelState.AddModelError(string.Empty, validationResult.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a model error for each validation result from the business service.
        /// </summary>
        /// <param name="validationResults">The validation results from a business service.</param>
        /// <param name="modelState">The model state dictionary used to add errors.</param>
        /// <param name="defaultErrorKey">The default key to use if a field is not specified in a business service validation result.</param>
        public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<ValidationResult> validationResults, string defaultErrorKey = null)
        {
            if (validationResults == null) return;

            foreach (var validationResult in validationResults)
            {
                string key = validationResult.MemberName ?? defaultErrorKey ?? string.Empty;
                modelState.AddModelError(key, validationResult.Message);
            }
        }

        public static void SetConfirmationMessage(this Controller controller, string message)
        {
            controller.TempData["confirm"] = message;
        }

        public static void SetAlertMessage(this Controller controller, string message)
        {
            controller.TempData["alert"] = message;
        }
    }
}