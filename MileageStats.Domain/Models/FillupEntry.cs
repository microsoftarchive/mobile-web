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
using MileageStats.Domain.Contracts;


namespace MileageStats.Domain.Models
{
    public enum FillupUnits
    {
        Gallons,
        Litres
    }

    public class FillupEntry : IHasIdentity
    {
        private static int tempKey;

        public FillupEntry()
        {
            UnitOfMeasure = FillupUnits.Gallons;
            Date = DateTime.UtcNow;
            FillupEntryId = --tempKey;
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
        public DateTime Date { get; set; }

        /// <summary>
        /// Odometer reading for the fillup.
        /// </summary>
        public int Odometer { get; set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        public double PricePerUnit { get; set; }

        /// <summary>
        /// Total number of units.
        /// </summary>
        public double TotalUnits { get; set; }

        public FillupUnits UnitOfMeasure { get; set; }

        public string Vendor { get; set; }

        public double TransactionFee { get; set; }

        public string Remarks { get; set; }

        /// <summary>
        /// Total cost of fillup.
        /// </summary>
        public double TotalCost
        {
            get { return (PricePerUnit*TotalUnits) + TransactionFee; }
        }

        #region Cached Calculations

        /// <summary>
        /// Gets or sets the distance from last fillup.  
        /// </summary>
        public int? Distance { get; set; }

        #endregion

        #region IHasIdentity Members

        public int Id
        {
            get { return FillupEntryId; }
            set { FillupEntryId = value; }
        }

        #endregion
    }
}