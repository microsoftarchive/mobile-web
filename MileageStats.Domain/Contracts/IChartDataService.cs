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

using MileageStats.Domain.Models;
using System;

namespace MileageStats.Domain.Contracts
{
    public interface IChartDataService
    {
        /// <summary>
        /// Calculates the series of statistics of all vehicles for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="startDate">The start date for filtering.</param>
        /// <param name="endDate">The end date for filtering.</param>
        /// <returns>A statistical series.</returns>
        StatisticSeries CalculateSeriesForUser(int userId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Calculates the series of statistics for the specified vehicle.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="startDate">The start date for filtering.</param>
        /// <param name="endDate">The end date for filtering.</param>
        /// <returns>A statistical series.</returns>
        StatisticSeries CalculateSeriesForVehicle(int userId, int vehicleId, DateTime? startDate, DateTime? endDate);
    }
}