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
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class FillupViewModel
    {
        public FillupViewModel(FillupEntry entry)
        {
            FillupEntryId = entry.FillupEntryId;
            Date = String.Format("{0:d MMM yyyy}", entry.Date);
            TotalUnits = String.Format("{0:#00.000}", entry.TotalUnits);
            Odometer = entry.Odometer;
            TransactionFee = String.Format("{0:C}", entry.TransactionFee);
            PricePerUnit = String.Format("{0:0.000}", entry.PricePerUnit);
            Remarks = entry.Remarks;
            Vendor = entry.Vendor;
            TotalCost = String.Format("{0:C}", entry.TotalCost);
        }

        /// <summary>
        /// Identifier for FillupEntry.  Should be unique once persisted.
        /// </summary>
        public int FillupEntryId { get; private  set; }

        /// <summary>
        /// Date of the fillup.
        /// </summary>
        public string Date { get; private  set; }

        /// <summary>
        /// Odometer reading for the fillup.
        /// </summary>
        public int Odometer { get; private  set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        public string PricePerUnit { get; private  set; }

        /// <summary>
        /// Total number of units.
        /// </summary>
        public string TotalUnits { get; private  set; }

        /// <summary>
        /// Name of the gas station
        /// </summary>
        public string Vendor { get; private  set; }

        /// <summary>
        /// Any additional transaction fees
        /// </summary>
        public string TransactionFee { get; private  set; }

        /// <summary>
        /// Optional remarks for this fillup
        /// </summary>
        public string Remarks { get; private  set; }

        /// <summary>
        /// Total cost of the fillup (includes transaction fee)
        /// </summary>
        public string TotalCost { get; private  set; }
    }
}