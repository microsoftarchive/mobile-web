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
using MileageStats.Domain.Properties;
using MileageStats.Domain.Validators;
using MileageStats.Web.Infrastructure;

namespace MileageStats.Web.Models
{
    /// <summary>
    /// A reminder for a user to service their vehicle.
    /// </summary>
    [AtLeastOneNonNullPropertyValidation("DueDate", "DueDistance", ErrorMessage = "Valid Due Date or Due Distance is required.")]
    public class ReminderFormModel : ICreateReminderCommand
    {
        private int? lastVehicleOdometer;

        private DateTime? date;

        /// <summary>
        /// Gets or sets the entity ID of reminder.
        /// </summary>
        /// <value>
        /// An integer identifying the entity.
        /// </value>
        public int ReminderId { get; set; }

        /// <summary>
        /// Gets or sets the vehicle the reminder is for.
        /// </summary>
        /// <value>
        /// An integer identifying the entity.
        /// </value>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the title of the reminder.
        /// </summary>
        /// <value>
        /// A string.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ReminderTitleRequired", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(50, ErrorMessageResourceName = "ReminderTitleStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextLineInputValidator]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the remarks for the reminder.
        /// </summary>
        /// <value>
        /// A string.
        /// </value>        
        [StringLength(250, ErrorMessageResourceName = "ReminderRemarksStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [TextMultilineValidator]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the date the reminder is due.
        /// </summary>
        /// <value>
        /// A DateTime (UTC) or null.
        /// </value>
        [Display(Name = "ReminderDueDateLabelText", ResourceType = typeof(Resources))]
        [StoreRestrictedDate]
        [FutureDate]
        public DateTime? DueDate
        {
            get
            {
                if (this.date.HasValue)
                    return this.date.Value;

                if (!string.IsNullOrEmpty(this.DueDateDay) &&
                    !string.IsNullOrEmpty(this.DueDateMonth) &&
                    !string.IsNullOrEmpty(this.DueDateYear))
                {
                    DateTime date;
                    if (DateTime.TryParse(string.Join("/", this.DueDateYear, this.DueDateMonth, this.DueDateDay), out date))
                    {
                        this.date = date;
                    }
                }

                return this.date;
            }
            set
            {
                this.date = value;
            }
        }

        public string DueDateYear { get; set; }
        
        public string DueDateMonth { get; set; }
        
        public string DueDateDay { get; set; }

        /// <summary>
        /// Gets or sets the distance the reminder is due when reached
        /// </summary>
        /// <value>
        /// A number or null.
        /// </value>
        [Range(0, 1000000, ErrorMessageResourceName = "ReminderDueDistanceRangeValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "ReminderDueDistanceLabelText", ResourceType = typeof(Resources))]
        [InputType("number", "7", "1", "0")]
        public int? DueDistance { get; set; }

        /// <summary>
        /// Gets a value indicating whether the reminder has been fulfilled.
        /// </summary>
        /// <value>
        /// <c>true</c> if fulfilled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFulfilled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the reminder is overdue (cached value based on vehicle odometer)
        /// </summary>
        /// <value>
        ///   <c>true</c> if is overdue; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverdue
        {
            get
            {
                int odometer = this.lastVehicleOdometer == null ? 0 : (int)this.lastVehicleOdometer;
                return ((this.IsFulfilled == false)
                        && ((this.DueDate.HasValue && this.DueDate.Value.Date <= DateTime.UtcNow.Date)
                            || (this.DueDistance.HasValue && this.DueDistance <= odometer)));

                
            }
        }

        public void UpdateLastVehicleOdometer(int? odometer)
        {
            lastVehicleOdometer = odometer;
        }
    }
}