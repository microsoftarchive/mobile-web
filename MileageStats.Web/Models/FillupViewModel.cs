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
    public class FillupViewModel
    {
        /// <summary>
        /// Identifier for FillupEntry.  Should be unique once persisted.
        /// </summary>
        public int FillupEntryId { get; set; }

        /// <summary>
        /// Date of the fillup.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Odometer reading for the fillup.
        /// </summary>
        public int Odometer { get; set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        public string PricePerUnit { get; set; }

        /// <summary>
        /// Total number of units.
        /// </summary>
        public string TotalUnits { get; set; }

        /// <summary>
        /// Name of the gas station
        /// </summary>
        public string Vendor { get; set; }

        /// <summary>
        /// Any additional transaction fees
        /// </summary>
        public string TransactionFee { get; set; }

        /// <summary>
        /// Optional remarks for this fillup
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Total cost of the fillup (includes transaction fee)
        /// </summary>
        public string TotalCost { get; set; }

    }
}