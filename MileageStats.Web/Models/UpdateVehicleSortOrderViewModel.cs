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

namespace MileageStats.Web.Models
{
    public class UpdateVehicleSortOrderViewModel
    {
        public string SortOrder { get; set; }

        public int[] VehicleSortOrder
        {
            get
            {
                var newOrderStrings = this.SortOrder.Split(',');

                var newOrder = new int[newOrderStrings.Length];

                for (var i = 0; i < newOrderStrings.Length; i++)
                {
                    newOrder[i] = Convert.ToInt32(newOrderStrings[i]);
                }
                return newOrder;
            }
        }
    }
}