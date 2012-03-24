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

namespace MileageStats.Web.Models
{
    public class VehicleFormModel : ICreateVehicleCommand
    {
        /// <summary>
        /// Gets or sets the entity ID of vehicle.
        /// </summary>
        /// <value>
        /// An integer identifying the entity.
        /// </value>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the name of the vehicle.
        /// </summary>
        /// <value>
        /// A string.
        /// </value>
        [StringLength(20, ErrorMessageResourceName = "VehicleNameStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextLineInputValidator]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "VehicleNameRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "VehicleNameLabelText", ResourceType = typeof(Resources))]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sort order relative to other vehicles.
        /// </summary>
        /// <value>
        /// A positive number up to 10,000 or zero.
        /// </value>
        [Range(0, 10000, ErrorMessageResourceName = "VehicleSortOrderRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "VehicleSortOrderLabelText", ResourceType = typeof(Resources))]
        public int SortOrder { get; set; }

        private int? _year;

        /// <summary>
        /// Gets or sets the manufacturing year of the vehicle.
        /// </summary>
        /// <value>
        /// An integer after 1896.
        /// </value>    
        [Range(1900, 2100, ErrorMessageResourceName = "VehicleYearRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "VehicleYearLabelText", ResourceType = typeof(Resources))]
        public int? Year
        {
            get { return _year; }
            set { _year = value; if (_year == null) MakeName = null; }
        }

        private string _makeName;

        /// <summary>
        /// Gets or sets the make of the vehicle (e.g. Toyota, Ford).
        /// </summary>
        /// <value>
        /// A string.
        /// </value>
        [StringLength(50, ErrorMessageResourceName = "VehicleMakeStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextLineInputValidator]
        [Display(Name = "VehicleMakeLabelText", ResourceType = typeof(Resources))]
        public string MakeName
        {
            get { return _makeName; }
            set { _makeName = value; if (_makeName == null) ModelName = null; }
        }

        /// <summary>
        /// Gets or sets the model of the vehicle (e.g. Camry, Fiesta)
        /// </summary>
        /// <value>
        /// A string.
        /// </value>
        [StringLength(50, ErrorMessageResourceName = "VehicleModelStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextLineInputValidator]
        [Display(Name = "VehicleModelLabelText", ResourceType = typeof(Resources))]
        public string ModelName { get; set; }

        public string Action { get; set; }

        public string CancelUrl { get; set; }
    }
}