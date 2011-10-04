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
using System.Collections.Generic;
using System.Linq;
using MileageStats.Domain.Models;

namespace MileageStats.Domain.Handlers
{
    public static class CalculateStatistics
    {
        public static VehicleStatisticsModel Calculate(IEnumerable<FillupEntry> fillUps, bool includeFirst = true)
        {
            if (!fillUps.Any()) return new VehicleStatisticsModel();

            var firstFillUp = fillUps.OrderBy(x => x.Date).FirstOrDefault();

            double totalFuelCost = 0.0;
            double totalUnits = 0.0;
            double totalCost = 0.0;
            int totalDistance = 0;

            foreach (var fillUp in fillUps)
            {
                if (includeFirst || fillUp != firstFillUp)
                {
                    totalFuelCost += fillUp.PricePerUnit*fillUp.TotalUnits;
                    totalUnits += fillUp.TotalUnits;
                    totalCost += fillUp.TotalCost;
                    totalDistance += fillUp.Distance ?? 0;
                }
            }

            var odometer = fillUps.Max(x => x.Odometer);

            var earliestEntryDate = fillUps.Min(x => x.Date).ToUniversalTime();
            var today = DateTime.UtcNow.Date;
            var totalMonths = CalculateDifferenceInMonths(earliestEntryDate.Date, today.Date);

            return new VehicleStatisticsModel(totalFuelCost, totalUnits, totalCost, totalDistance, odometer, totalMonths);
        }

        private static int CalculateDifferenceInMonths(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) return 0;
            int years = (endDate.Year - startDate.Year);
            int months = (12 * years) + endDate.Month - startDate.Month;
            // In the case where it has not yet been a full month of fill up, default to 1
            return Math.Max(1, months);
        }
    }
}