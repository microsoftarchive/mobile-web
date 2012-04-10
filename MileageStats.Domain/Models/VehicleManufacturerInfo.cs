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

namespace MileageStats.Domain.Models
{
    public class VehicleManufacturerInfo
    {
        /// <summary>
        /// Gets or sets the entity ID of information.
        /// </summary>
        /// <value>
        /// An integer identifying the entity.
        /// </value>
        public int VehicleManufacturerInfoId { get; set; }

        /// <summary>
        /// Gets or sets the manufacturing year of the vehicle.
        /// </summary>
        /// <value>
        /// An integer after 1896.
        /// </value>        
        public int Year { get; set; }

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
    }
}