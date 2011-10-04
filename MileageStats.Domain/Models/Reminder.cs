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
using MileageStats.Domain.Contracts;


namespace MileageStats.Domain.Models
{
    /// <summary>
    /// A reminder for a user to service their vehicle.
    /// </summary>
    public class Reminder : IHasIdentity
    {
        
        public int Id { get { return ReminderId; } set { ReminderId = value; } }

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
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the remarks for the reminder.
        /// </summary>
        /// <value>
        /// A string.
        /// </value>        
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the date the reminder is due.
        /// </summary>
        /// <value>
        /// A DateTime (UTC) or null.
        /// </value>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the distance the reminder is due when reached
        /// </summary>
        /// <value>
        /// A number or null.
        /// </value>
        public int? DueDistance { get; set; }

        /// <summary>
        /// Gets a value indicating whether the reminder has been fulfilled.
        /// </summary>
        /// <value>
        /// <c>true</c> if fulfilled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFulfilled { get; set; }

        public bool? IsOverdue { get; private set; }

        public void CalculateIsOverdue(int odometer)
        {
            IsOverdue = false;
            if(DueDate.HasValue && DueDate.Value < DateTime.UtcNow) IsOverdue = true;
            if (DueDistance.HasValue && DueDistance.Value < odometer) IsOverdue = true;
        }
    }
}