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

using System;
using MileageStats.Domain.Models;


namespace MileageStats.Domain.Contracts
{
    public interface ICreateFillupEntryCommand
    {
        /// <summary>
        /// Identifier for FillupEntry.  Should be unique once persisted.
        /// </summary>
        int FillupEntryId { get; set; }

        /// <summary>
        /// Identifier for the Vehicle this fillup is related to.  
        /// </summary>
        int VehicleId { get; set; }

        /// <summary>
        /// Date of the fillup.
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// Odometer reading for the fillup.
        /// </summary>
        int Odometer { get; set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        double? PricePerUnit { get; set; }

        /// <summary>
        /// Total number of units.
        /// </summary>
        double? TotalUnits { get; set; }

        FillupUnits UnitOfMeasure { get; set; }

        string Vendor { get; set; }

        string Location { get; set; }

        double? TransactionFee { get; set; }

        string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the distance from last fillup.  This is a cached value
        /// and is not expected to be set directly.
        /// </summary>        
        int? Distance { get; set; }
    }
}