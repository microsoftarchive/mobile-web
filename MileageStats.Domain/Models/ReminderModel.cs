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

namespace MileageStats.Domain.Models
{
    public class ReminderModel
    {
        private int? lastVehicleOdometer;

        public bool IsOverdue
        {
            get
            {
                int odometer = this.lastVehicleOdometer == null ? 0 : (int) this.lastVehicleOdometer;
                return ((this.IsFulfilled == false)
                        && ((this.DueDate.HasValue && this.DueDate.Value.Date <= DateTime.UtcNow.Date)
                            || (this.DueDistance.HasValue && this.DueDistance <= odometer)));
            }
        }

        public int ReminderId { get; set; }

        public int VehicleId { get; set; }

        public string Title { get; set; }

        public string Remarks { get; set; }

        public DateTime? DueDate { get; set; }

        public int? DueDistance { get; set; }

        public bool IsFulfilled { get; set; }

        public void UpdateLastVehicleOdometer(int? odometer)
        {
            this.lastVehicleOdometer = odometer;
        }
    }
}