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
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class JsonVehicleViewModel
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sort order relative to other vehicles.
        /// </summary>
        /// <value>
        /// A positive number up to 10,000 or zero.
        /// </value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the manufacturing year of the vehicle.
        /// </summary>
        /// <value>
        /// An integer after 1896.
        /// </value>    
        public int? Year { get; set; }

        /// <summary>
        /// Gets or sets the make of the vehicle (e.g. Toyota, Ford).
        /// </summary>
        /// <value>
        /// A string.
        /// </value>
        public string MakeName { get; set; }

        /// <summary>
        /// Gets or sets the model of the vehicle (e.g. Camry, Fiesta)
        /// </summary>
        /// <value>
        /// A string.
        /// </value>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets the highest odometer value recorded for the vehicle.
        /// </summary>
        public int? Odometer { get; set; }

        /// <summary>
        /// The id of the vehicle's photo
        /// </summary>
        public int PhotoId { get; set; }

        /// <summary>
        /// Gets the statistics for the entire lifetime of this vehicle.
        /// </summary>
        public JsonStatisticsViewModel LifeTimeStatistics { get; set; }

        /// <summary>
        /// Gets the statistics for the last 12 months for this vehicle.
        /// </summary>
        public JsonStatisticsViewModel LastTwelveMonthsStatistics { get; set; }

        /// <summary>
        /// Gets or sets the overdue reminders for this vehicle.
        /// </summary>    
        public IEnumerable<ReminderSummaryModel> OverdueReminders { get; set; }
    }
}