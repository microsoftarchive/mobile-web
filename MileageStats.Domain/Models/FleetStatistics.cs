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

namespace MileageStats.Domain.Models
{
    /// <summary>
    /// Provides aggregate statistics across a set of vehicle statistics
    /// </summary>
    public class FleetStatistics : IVehicleStatistics
    {
        private readonly IEnumerable<VehicleStatisticsModel> _fleetVehicleStatistics;

        private double averageFillupPrice;
        private double averageFuelEfficiency;
        private double averageCostToDrive;
        private double averageCostPerMonth;
        private int? odometer;
        private double totalCost;
        private int totalDistance;
        private double totalFuelCost;
        private double totalUnits;

        private bool areStatisticsCalculated;

        public FleetStatistics(IEnumerable<VehicleStatisticsModel> fleetVehicleStatistics)
        {
            if (fleetVehicleStatistics == null)
            {
                throw new ArgumentNullException("vehicleStatistics");
            }

            this._fleetVehicleStatistics = fleetVehicleStatistics;
        }

        #region Properties

        /// <summary>
        /// Gets the average cost to fill up per unit.
        /// </summary>
        public double AverageFillupPrice
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(averageFillupPrice);
            }
        }

        /// <summary>
        /// Gets the average fuel efficiency (e.g. Miles/Gallon or Kilomter / Litre)
        /// </summary>
        public double AverageFuelEfficiency
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(averageFuelEfficiency);
            }
        }

        /// <summary>
        /// Gets the average cost to drive per distance (e.g. $/Mile or €/Kilometer)
        /// </summary>
        public double AverageCostToDrive
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(averageCostToDrive);
            }
        }

        /// <summary>
        /// Gets the average cost to drive per month (e.g. $/Month or €/Month) between the first entry and today.
        /// </summary>
        public double AverageCostPerMonth
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(averageCostPerMonth);
            }
        }

        /// <summary>
        /// Gets the highest odometer value recorded.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        public int? Odometer
        {
            get
            {
                EnsureStatisticsCalculated();
                return odometer;
            }
        }

        /// <summary>
        /// Gets the total vehicle distance traveled for fillup entries.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        public int TotalDistance
        {
            get
            {
                EnsureStatisticsCalculated();
                return totalDistance;
            }
        }

        /// <summary>
        /// Gets the total cost of all fillup entries, not including transaction fees.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        public double TotalFuelCost
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(totalFuelCost);
            }
        }

        /// <summary>
        /// Gets the total units consumed based on all fillup entries.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        public double TotalUnits
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(totalUnits);
            }
        }

        /// <summary>
        /// Gets the total cost of all fillup entries including transaction fees and service entries.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        public double TotalCost
        {
            get
            {
                EnsureStatisticsCalculated();
                return RoundToTwoDecimals(totalCost);
            }
        }

        #endregion

        /// <summary>
        /// Marks the statistics for recalculation.
        /// </summary>
        public void Recalculate()
        {
            areStatisticsCalculated = false;
        }

        private void EnsureStatisticsCalculated()
        {
            if (!areStatisticsCalculated)
            {
                Calculate();
            }
        }

        private void Calculate()
        {
            averageFillupPrice = 0.0;
            averageFuelEfficiency = 0.0;
            averageCostToDrive = 0.0;
            averageCostPerMonth = 0.0;
            odometer = null;
            totalCost = 0.0;
            totalDistance = 0;
            totalFuelCost = 0.0;
            totalUnits = 0.0;

            int vehicleCount = 0;
            int maxOdometer = 0;
            double totalAverageCostToDrive = 0.0;
            double totalAverageCostPerMonth = 0.0;
            double totalAverageFillupPrice = 0.0;
            double totalAverageFuelEfficiency = 0.0;

            foreach (var vehicleStatistics in _fleetVehicleStatistics)
            {
                maxOdometer = (vehicleStatistics.Odometer.HasValue)
                                  ? Math.Max(maxOdometer, vehicleStatistics.Odometer.Value)
                                  : maxOdometer;

                totalCost += vehicleStatistics.TotalCost;
                totalDistance += vehicleStatistics.TotalDistance;
                totalFuelCost += vehicleStatistics.TotalFuelCost;
                totalUnits += vehicleStatistics.TotalUnits;
                totalAverageCostPerMonth += vehicleStatistics.AverageCostPerMonth;
                totalAverageCostToDrive += vehicleStatistics.AverageCostToDrive;
                totalAverageFillupPrice += vehicleStatistics.AverageFillupPrice;
                totalAverageFuelEfficiency += vehicleStatistics.AverageFuelEfficiency;

                vehicleCount++;
            }

            if (vehicleCount > 0)
            {
                averageCostToDrive = totalAverageCostToDrive/(double) vehicleCount;
                averageCostPerMonth = totalAverageCostPerMonth/(double) vehicleCount;
                averageFillupPrice = totalAverageFillupPrice/(double) vehicleCount;
                averageFuelEfficiency = totalAverageFuelEfficiency/(double) vehicleCount;
            }

            odometer = (maxOdometer != 0) ? maxOdometer : (int?) null;

            areStatisticsCalculated = true;
        }

        private static double RoundToTwoDecimals(double number)
        {
            return Math.Round(number, 2);
        }
    }

}