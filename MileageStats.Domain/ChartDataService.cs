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
using System.Diagnostics;
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;

namespace MileageStats.Domain
{
    public class ChartDataService : IChartDataService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFillupRepository _fillupRepository;

        #region IChartDataService Members

        public StatisticSeries CalculateSeriesForUser(int userId, DateTime? startDate, DateTime? endDate)
        {
            var vehicles = _vehicleRepository.GetVehicles(userId);

            var series = new StatisticSeries();

            foreach (var vehicle in vehicles)
            {
                CalculateSeriesForVehicle(vehicle, series, startDate, endDate);
            }

            return series;
        }

        public StatisticSeries CalculateSeriesForVehicle(int userId, int vehicleId, DateTime? startDate,
                                                         DateTime? endDate)
        {
            var series = new StatisticSeries();
            CalculateSeriesForVehicle(userId, vehicleId, series, startDate, endDate);
            return series;
        }

        #endregion

        public ChartDataService(IVehicleRepository vehicleRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _fillupRepository = fillupRepository;
        }

        private void CalculateSeriesForVehicle(int userId, int vehicleId, StatisticSeries series, DateTime? startDate, DateTime? endDate)
        {
            var vehicle = _vehicleRepository.GetVehicle(userId, vehicleId);
            CalculateSeriesForVehicle(vehicle, series, startDate, endDate);
        }

        private void CalculateSeriesForVehicle(Vehicle vehicle, StatisticSeries series, DateTime? startDate, DateTime? endDate)
        {
            Debug.Assert(series != null);

            DateTime startFilterDate = startDate ?? DateTime.MinValue;
            DateTime endFilterDate = endDate ?? DateTime.UtcNow;

            var fillUps = _fillupRepository.GetFillups(vehicle.VehicleId);

            var fillupGroups = from fillUp in fillUps
                               where ((fillUp.Date >= startFilterDate) && (fillUp.Date <= endFilterDate))
                               group fillUp by new { Year = fillUp.Date.Year, Month = fillUp.Date.Month }
                                   into g
                                   orderby g.Key.Year, g.Key.Month
                                   select g;

            var firstFillUp = fillUps.OrderBy(x => x.Date).FirstOrDefault();

            VehicleStatisticsModel statistics;
            foreach (var fillupGroup in fillupGroups)
            {
                var includeFirstFillup = (fillupGroup.Key.Year != firstFillUp.Date.Year) ||
                                                    (fillupGroup.Key.Month != firstFillUp.Date.Month);

                statistics = CalculateStatistics.Calculate(fillupGroup, includeFirstFillup);

                Debug.Assert(firstFillUp != null);
                

                var seriesEntry = new StatisticSeriesEntry
                {
                    Id = vehicle.VehicleId,
                    Name = vehicle.Name,
                    Year = fillupGroup.Key.Year,
                    Month = fillupGroup.Key.Month,
                    AverageFuelEfficiency = Math.Round(statistics.AverageFuelEfficiency, 2),
                    TotalCost = Math.Round(statistics.TotalCost, 2),
                    TotalDistance = statistics.TotalDistance,
                };
                series.Entries.Add(seriesEntry);
            }            
        }
    }
}