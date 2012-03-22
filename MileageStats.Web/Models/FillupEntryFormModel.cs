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
using System.ComponentModel.DataAnnotations;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Models;
using MileageStats.Domain.Validators;
using MileageStats.Domain.Properties;
using MileageStats.Web.Infrastructure;

namespace MileageStats.Web.Models
{
    public class FillupEntryFormModel : ICreateFillupEntryCommand
    {
        private static int tempKey = 0;

        DateTime? date = null;

        public FillupEntryFormModel()
        {
            this.UnitOfMeasure = FillupUnits.Gallons;
            this.FillupEntryId = --tempKey;
        }

        /// <summary>
        /// Identifier for FillupEntry.  Should be unique once persisted.
        /// </summary>
        public int FillupEntryId { get; set; }

        /// <summary>
        /// Identifier for the Vehicle this fillup is related to.  
        /// </summary>
        public int VehicleId { get; set; }

        /// <summary>
        /// Date of the fillup.
        /// </summary>
        [Required(ErrorMessageResourceName = "FillupEntryDateRequired", ErrorMessageResourceType = typeof(Resources))]
        [PastDate]
        [StoreRestrictedDate]
        public DateTime? Date 
        { 
            get 
            {
                if (this.date.HasValue)
                    return this.date.Value;

                if (!string.IsNullOrEmpty(this.DateDay) &&
                    !string.IsNullOrEmpty(this.DateMonth) &&
                    !string.IsNullOrEmpty(this.DateYear))
                {
                    DateTime date;
                    if (DateTime.TryParse(string.Join("/", this.DateYear, this.DateMonth, this.DateDay), out date))
                    {
                        this.date = date;
                    }
                    else
                    {
                        return null;
                    }
                }
                
                return this.date.GetValueOrDefault(DateTime.Now);
            }
            set 
            { 
                this.date = value; 
            }
        }

        public string DateYear { get; set; }
        public string DateMonth { get; set; }
        public string DateDay { get; set; }

        /// <summary>
        /// Odometer reading for the fillup.
        /// </summary>
        [Required(ErrorMessageResourceName = "FillupEntryOdometerRequired", ErrorMessageResourceType = typeof(Resources))]
        [Range(1, 1000000, ErrorMessageResourceName = "FillupEntryOdometerRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [InputType("number", "7")]
        public int Odometer { get; set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        [Required(ErrorMessageResourceName = "FillupEntryPricePerUnitRequired", ErrorMessageResourceType = typeof(Resources))]
        [Range(0.1d, 100.0d, ErrorMessageResourceName = "FillupEntryPricePerUnitRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "FillupEntryPricePerUnitLabelText", ResourceType = typeof(Resources))]
        [InputType("number", "6", "0.01", "3.75")]
        public double? PricePerUnit { get; set; }

        /// <summary>
        /// Total number of units.
        /// </summary>
        [Required(ErrorMessageResourceName = "FillupEntryTotalUnitsRequired", ErrorMessageResourceType = typeof(Resources))]
        [Range(1.0d, 1000.0d, ErrorMessageResourceName = "FillupEntryTotalUnitsRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "FillupEntryTotalUnitsLabelText", ResourceType = typeof(Resources))]
        [InputType("number", "7", "0.001", "10.123")]
        public double? TotalUnits { get; set; }

        [Required(ErrorMessageResourceName = "FillUpEntryUnitOfMeasureRequired", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "FillupEntryUnitOfMeasureLabelText", ResourceType = typeof(Resources))]
        public FillupUnits UnitOfMeasure { get; set; }

        [TextLineInputValidator]
        [StringLength(20, ErrorMessageResourceName = "FillupEntryVendorStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "FillUpEntryVendorLabelText", ResourceType = typeof(Resources))]
        public string Vendor { get; set; }

        public string Location { get; set; }

        [Required(ErrorMessageResourceName = "FillupEntryTransactionFeeRequired", ErrorMessageResourceType = typeof(Resources))]
        [Range(0.0d, 100.0d, ErrorMessageResourceName = "FillupEntryTransactionFeeRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "FillupEntryTransactionFeeLabelText", ResourceType = typeof(Resources))]
        [InputType("number", "6", "0.01", "0.45")]
        public double? TransactionFee { get; set; }

        [TextMultilineValidator]
        [StringLength(250, ErrorMessageResourceName = "FillupEntryRemarksStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        public string Remarks { get; set; }

        #region Cached Calculations

        /// <summary>
        /// Gets or sets the distance from last fillup.  This is a cached value
        /// and is not expected to be set directly.
        /// </summary>        
        public int? Distance { get; set; }

        #endregion
    }
}