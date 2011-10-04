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
using System.Web.Mvc;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;

namespace MileageStats.Web.Helpers
{
    public static class VehicleHtmlHelperExtensions
    {
        public static MvcHtmlString AverageFuelEfficiencyText(this HtmlHelper helper, VehicleModel vehicle)
        {
            double raw = vehicle.Statistics.AverageFuelEfficiency;
            double averageFuelEfficiency = Math.Round(raw);

            string averageFuelEfficiencyText = string.Format("{0:N0}", averageFuelEfficiency);

            if (averageFuelEfficiency >= 99000)
            {
                averageFuelEfficiencyText = "99k+";
            }
            else if (averageFuelEfficiency >= 10000)
            {
                averageFuelEfficiencyText = string.Format("{0:N1}k", averageFuelEfficiency/1000);
            }

            return MvcHtmlString.Create(averageFuelEfficiencyText);
        }

        public static MvcHtmlString AverageFuelEfficiencyMagnitude(this HtmlHelper helper, VehicleModel vehicle)
        {
            double raw = vehicle.Statistics.AverageFuelEfficiency;
            double averageFuelEfficiency = Math.Round(raw);
            string averageFuelEfficiencyMagnitude = "";

            if (averageFuelEfficiency >= 1000)
            {
                averageFuelEfficiencyMagnitude = "thousands";
            }
            else if (averageFuelEfficiency >= 100)
            {
                averageFuelEfficiencyMagnitude = "hundreds";
            }

            return MvcHtmlString.Create(averageFuelEfficiencyMagnitude);
        }

        public static string CssClassForTile(this HtmlHelper helper, VehicleListViewModel list, VehicleModel vehicle)
        {
            var selectedItem = list.Vehicles.SelectedItem;

            var shouldCompact = (list.IsCollapsed && selectedItem == null)
                ||( selectedItem != null && vehicle.VehicleId != selectedItem.VehicleId);

            return shouldCompact ? "compact" : String.Empty;
        }
    }
}