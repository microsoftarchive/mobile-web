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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Validators;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Models
{
    public class User : IHasIdentity
    {
        public int Id { get { return UserId; } set { UserId = value; } }

        /// <summary>
        /// Gets or sets the identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        [StringLength(15, ErrorMessageResourceName = "UserDisplayNameStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextLineInputValidator]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "UserDisplayNameRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "UserDisplayNameLabelText", ResourceType = typeof(Resources))]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the authorization identifier for the user.
        /// </summary>
        [Required(ErrorMessageResourceName = "UserAuthorizationIdRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "UserAuthorizationIdLabelText", ResourceType = typeof(Resources))]
        public string AuthorizationId { get; set; }

        /// <summary>
        /// Gets or sets the country for the user.
        /// </summary>
        [StringLength(50, ErrorMessageResourceName = "UserCountryStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextLineInputValidator]
        [Display(Name = "UserCountryLabelText", ResourceType = typeof(Resources))]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has completed or dismissed their profile registration.
        /// </summary>
        public bool HasRegistered { get; set; }
    }
}