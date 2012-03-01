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

using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class StatisticsViewModel
    {
        public StatisticsViewModel(FleetStatistics inner)
        {
            AverageFillupPrice = string.Format("{0:c}", inner.AverageFillupPrice);
            AverageFuelEfficiency = string.Format("{0:0}", inner.AverageFuelEfficiency);
            AverageCostToDrive = string.Format("{0:0}¢", inner.AverageCostToDrive * 100);
            AverageCostPerMonth = string.Format("{0:$0,0}", inner.AverageCostPerMonth);
            Odometer = string.Format("{0:0}", inner.Odometer);
            TotalDistance = string.Format("{0:0}", inner.TotalDistance);
            TotalFuelCost = string.Format("{0:$0,0}", inner.TotalFuelCost);
            TotalUnits = string.Format("{0:0}", inner.TotalUnits);
            TotalCost = string.Format("{0:$0,0}", inner.TotalCost);
        }

        public string AverageFillupPrice { get; private set; }
        public string AverageFuelEfficiency { get; private set; }
        public string AverageCostToDrive { get; private set; }
        public string AverageCostPerMonth { get; private set; }
        public string Odometer { get; private set; }
        public string TotalDistance { get; private set; }
        public string TotalFuelCost { get; private set; }
        public string TotalUnits { get; private set; }
        public string TotalCost { get; private set; }
    }
}