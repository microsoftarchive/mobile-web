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

namespace MileageStats.Web.Models
{
    public class JsonStatisticsViewModel
    {
        /// <summary>
        /// Gets or sets the name of this statistic set.
        /// </summary>
        /// <value>
        /// A string - commonly set to describe the range "Lifetime", "Last12Months", etc.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the average cost to fill up per unit.
        /// </summary>
        public double AverageFillupPrice { get; set; }

        /// <summary>
        /// Gets the average fuel efficiency (e.g. Miles/Gallon or Kilomter/Litre).
        /// </summary>
        public double AverageFuelEfficiency { get; set; }

        /// <summary>
        /// Gets the average cost to drive per distance (e.g. $/Mile or €/Kilometer).
        /// </summary>
        public double AverageCostToDrive { get; set; }

        /// <summary>
        /// Gets the average cost to drive per month (e.g. $/Month or €/Month) between the first entry and today.
        /// </summary>
        public double AverageCostPerMonth { get; set; }

        /// <summary>
        /// Gets the highest odometer value recorded.
        /// </summary>
        public int? Odometer { get; set; }

        /// <summary>
        /// Gets the total vehicle distance traveled for fillup entries.
        /// </summary>
        public int TotalDistance { get; set; }

        /// <summary>
        /// Gets the total cost of all fillup entries, not including transaction fees.
        /// </summary>
        public double TotalFuelCost { get; set; }

        /// <summary>
        /// Gets the total units consumed based on all fillup entries.
        /// </summary>
        public double TotalUnits { get; set; }

        /// <summary>
        /// Gets the total cost of all fillup entries, including transaction fees and service entries.
        /// </summary>
        public double TotalCost { get; set; }

    }
}