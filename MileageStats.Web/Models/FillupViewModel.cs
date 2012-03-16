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
using System.ComponentModel.DataAnnotations;
using MileageStats.Domain.Properties;

namespace MileageStats.Web.Models
{
    public class FillupViewModel
    {
        public FillupViewModel(FillupEntry entry)
        {
            FillupEntryId = entry.FillupEntryId;
            Date = String.Format("{0:MMMM d, yyyy}", entry.Date);
            DateShort = String.Format("{0:MM/dd/yyyy}", entry.Date);
            TotalUnits = String.Format("{0:#00.000}", entry.TotalUnits);
            Odometer = String.Format("{0:N0}", entry.Odometer);
            TransactionFee = String.Format("{0:C}", entry.TransactionFee);
            PricePerUnit = String.Format("{0:C}", entry.PricePerUnit);
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
        /// Date of the fillup. in MM/dd/yyyy format
        /// </summary>
        public string DateShort { get; private set; }

        /// <summary>
        /// Odometer reading for the fillup.
        /// </summary>
        public string Odometer { get; private  set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        [Display(Name = "FillupEntryPricePerUnitLabelText", ResourceType = typeof(Resources))]
        public string PricePerUnit { get; private  set; }

        /// <summary>
        /// Total number of units.
        /// </summary>
        [Display(Name = "FillupEntryTotalUnitsLabelText", ResourceType = typeof(Resources))]
        public string TotalUnits { get; private  set; }

        /// <summary>
        /// Name of the gas station
        /// </summary>
        [Display(Name = "FillUpEntryVendorLabelText", ResourceType = typeof(Resources))]
        public string Vendor { get; private  set; }

        /// <summary>
        /// Any additional transaction fees
        /// </summary>
        [Display(Name = "FillupEntryTransactionFeeLabelText", ResourceType = typeof(Resources))]
        public string TransactionFee { get; private  set; }

        /// <summary>
        /// Optional remarks for this fillup
        /// </summary>
        public string Remarks { get; private  set; }

        /// <summary>
        /// Total cost of the fillup (includes transaction fee)
        /// </summary>
        [Display(Name = "FillupEntryTotalCostLabelText", ResourceType = typeof(Resources))]
        public string TotalCost { get; private  set; }
    }
}