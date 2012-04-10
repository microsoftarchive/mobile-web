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

using System;
using System.Collections.Generic;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class VehicleStatisticsFixture
    {
        [Fact]
        public void WhenConstructed_ThenIntialized()
        {
            var target = CalculateStatistics.Calculate(new FillupEntry[]{});

            Assert.NotNull(target);
            Assert.Equal(0.0, target.AverageCostPerMonth);
            Assert.Equal(0.0, target.AverageCostToDrive);
            Assert.Equal(0.0, target.AverageFillupPrice);
            Assert.Equal(0.0, target.AverageFuelEfficiency);
            Assert.Null(target.Name);
            Assert.Null(target.Odometer);
            Assert.Equal(0.0, target.TotalCost);
            Assert.Equal(0.0, target.TotalDistance);
            Assert.Equal(0.0, target.TotalFuelCost);
            Assert.Equal(0.0, target.TotalUnits);
        }

        [Fact]
        public void WhenNameSet_ThenValueUpdated()
        {
            var target = new VehicleStatisticsModel {Name = "Name"};

            Assert.Equal("Name", target.Name);
        }

        [Fact]
        public void WhenVehiclePopulated_ThenOdometerCalculated()
        {
            var fillups = new[]
                              {
                                  new FillupEntry {Date = DateTime.UtcNow.AddMonths(-5), Odometer = 10000},
                                  new FillupEntry {Date = DateTime.UtcNow.AddMonths(-4), Odometer = 12000},
                                  new FillupEntry {Date = DateTime.UtcNow.AddMonths(-3), Odometer = 14000},
                                  new FillupEntry {Date = DateTime.UtcNow.AddMonths(-3), Odometer = 16000}
                              };

            var target = CalculateStatistics.Calculate(fillups);


            Assert.Equal(16000, target.Odometer);
        }

        [Fact]
        public void WhenVehiclePopulated_ThenTotalDistanceCalculated()
        {
            var fillups = new[]
                              {
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddMonths(-5),
                                          Odometer = 10000,
                                          Distance = null
                                      },
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddMonths(-4),
                                          Odometer = 12000,
                                          Distance = 2000
                                      },
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddMonths(-3),
                                          Odometer = 14000,
                                          Distance = 2000
                                      },
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddMonths(-3),
                                          Odometer = 16000,
                                          Distance = 2000
                                      }
                              };

            var target = CalculateStatistics.Calculate(fillups);

            Assert.Equal(16000 - 10000, target.TotalDistance);
        }

        [Fact]
        public void WhenVehiclePopulated_ThenTotalUnitsCalculated()
        {
            var fillups = new[]
                            {
                                new FillupEntry()
                                    {Date = DateTime.UtcNow.AddMonths(-5), TotalUnits = 20.5},
                                new FillupEntry()
                                    {Date = DateTime.UtcNow.AddMonths(-4), TotalUnits = 22},
                                new FillupEntry()
                                    {Date = DateTime.UtcNow.AddMonths(-3), TotalUnits = 36.253},
                                new FillupEntry()
                                    {Date = DateTime.UtcNow.AddMonths(-3), TotalUnits = 21.55}
                            };

            var target = CalculateStatistics.Calculate(fillups, includeFirst:false);

            Assert.Equal(79.8, Math.Round(target.TotalUnits, 2));
        }


        [Fact]
        public void WhenVehiclePopulated_ThenTotalFuelCostCalculated()
        {
            var fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry() { Date = DateTime.UtcNow.AddMonths(-5), PricePerUnit = 2.95, TotalUnits = 20.5, });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry() { Date = DateTime.UtcNow.AddMonths(-3), PricePerUnit = 1.90, TotalUnits = 36.253 });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst:false);

            Assert.Equal(206.5, Math.Round(target.TotalFuelCost, 2));
        }

        [Fact]
        public void WhenVehiclePopulated_ThenTotalCostCalculated()
        {
            List<FillupEntry> fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-5),
                                PricePerUnit = 2.95,
                                TotalUnits = 20.5,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                PricePerUnit = 1.90,
                                TotalUnits = 36.253
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst: false);
            // Ignore the cost of the first fillup, otherwise stats are off
            double totalCost = (22 * 3.16 + 36.253 * 1.90 + 21.55 * 3.16 + 2.9 + 1.13);
            Assert.Equal(Math.Round(totalCost, 2), Math.Round(target.TotalCost, 2));
        }

        [Fact]
        public void WhenVehiclePopulated_ThenAverageFillupPriceCalculated()
        {
            var fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry() { Date = DateTime.UtcNow.AddMonths(-5), PricePerUnit = 2.95, TotalUnits = 20.5, });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry() { Date = DateTime.UtcNow.AddMonths(-3), PricePerUnit = 1.90, TotalUnits = 36.253 });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst:false);

            Assert.Equal(2.59, Math.Round(target.AverageFillupPrice, 2));
        }

        [Fact]
        public void WhenVehiclePopulated_ThenAverageFuelEfficiencyCalculated()
        {
            List<FillupEntry> fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-5),
                                Odometer = 10000,
                                Distance = null,
                                PricePerUnit = 2.95,
                                TotalUnits = 20.5,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                Odometer = 10220,
                                Distance = 220,
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 11000,
                                Distance = 780,
                                PricePerUnit = 1.90,
                                TotalUnits = 36.253
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 11131,
                                Distance = 131,
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst: false);
            double totalDistance = 11131 - 10000;
            // Ignore the cost of the first fillup, otherwise stats are off
            double totalFuel = 22 + 36.253 + 21.55;

            double efficiency = Math.Round(totalDistance / totalFuel, 2);
            Assert.Equal(efficiency, Math.Round(target.AverageFuelEfficiency, 2));
        }

        [Fact]
        public void WhenVehiclePopulated_ThenAverageCostToDriveCalculated()
        {
            List<FillupEntry> fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-5),
                                Odometer = 10000,
                                Distance = null,
                                PricePerUnit = 2.95,
                                TotalUnits = 20.5,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                Odometer = 10220,
                                Distance = 220,
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 11000,
                                Distance = 780,
                                PricePerUnit = 1.90,
                                TotalUnits = 36.253
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 11132,
                                Distance = 132,
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst: false);
            double totalDistance = 11132 - 10000;
            // Ignore the cost of the first fillup, otherwise stats are off
            double totalCost = 22 * 3.16 + 36.253 * 1.90 + 21.55 * 3.16 + 2.9 + 1.13;
            double costPerDistance = Math.Round(totalCost / totalDistance, 2);
            Assert.Equal(costPerDistance, Math.Round(target.AverageCostToDrive, 2));
        }

        [Fact]
        public void WhenVehiclePopulated_ThenAverageCostPerMonthIsCalculated()
        {
            List<FillupEntry> fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-5),
                                Odometer = 10000,
                                Distance = null,
                                PricePerUnit = 2.95,
                                TotalUnits = 20.5,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                Odometer = 10220,
                                Distance = 220,
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 11000,
                                Distance = 780,
                                PricePerUnit = 1.90,
                                TotalUnits = 36.253
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 11132,
                                Distance = 132,
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst: false);
            // Ignore the cost of the first fillup, otherwise stats are off
            double totalCost = Math.Round(22 * 3.16 + 36.253 * 1.90 + 21.55 * 3.16 + 2.90 + 1.13, 2);
            double avgCostPerMonth = Math.Round(totalCost / 5, 2);
            Assert.Equal(avgCostPerMonth, Math.Round(target.AverageCostPerMonth, 2));
        }

        [Fact]
        public void WhenVehiclePopulatedWithIntraMonthData_ThenAverageCostPerMonthCalculatedAtOneMonth()
        {
            List<FillupEntry> fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddDays(-5),
                                Odometer = 10000,
                                Distance = null,
                                PricePerUnit = 2.95,
                                TotalUnits = 20.5,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddDays(-4),
                                Odometer = 10220,
                                Distance = 220,
                                PricePerUnit = 3.16,
                                TotalUnits = 22,
                                TransactionFee = 2.90
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddDays(-3),
                                Odometer = 11000,
                                Distance = 780,
                                PricePerUnit = 1.90,
                                TotalUnits = 36.253
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddDays(-3),
                                Odometer = 11132,
                                Distance = 132,
                                PricePerUnit = 3.16,
                                TotalUnits = 21.55,
                                TransactionFee = 1.13
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst: false);
            // Ignore the cost of the first fillup, otherwise stats are off
            double totalGasCost = (22 * 3.16 + 36.253 * 1.90 + 21.55 * 3.16);
            double totalCost = Math.Round(totalGasCost + +2.90 + 1.13, 2);
            Assert.Equal((totalCost) / 1,
                         Math.Round(target.AverageCostPerMonth, 2));
            Assert.Equal(Math.Round(target.TotalCost, 2), Math.Round(target.AverageCostPerMonth, 2));
        }

        [Fact]
        public void WhenIncludeFirstFillupTrue_ThenFirstFillupIncluded()
        {
            List<FillupEntry> fillups = new List<FillupEntry>();

            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-5),
                                Odometer = 10000,
                                Distance = null,
                                PricePerUnit = 1,
                                TotalUnits = 10,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-4),
                                Odometer = 11000,
                                Distance = 1000,
                                PricePerUnit = 1,
                                TotalUnits = 10,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-3),
                                Odometer = 12000,
                                Distance = 1000,
                                PricePerUnit = 1,
                                TotalUnits = 10,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-2),
                                Odometer = 13000,
                                Distance = 1000,
                                PricePerUnit = 1,
                                TotalUnits = 10,
                            });
            fillups.Add(new FillupEntry()
                            {
                                Date = DateTime.UtcNow.AddMonths(-1),
                                Odometer = 14000,
                                Distance = 1000,
                                PricePerUnit = 1,
                                TotalUnits = 10,
                            });

            var target = CalculateStatistics.Calculate(fillups, includeFirst: true);

            double totalDistance = 14000 - 10000;
            // Ignore the cost of the first fillup, otherwise stats are off
            double totalCost = 10 * 5;
            double fuelCost = 10 * 5;
            double totalFuel = 10 * 5;
            double avgCostPerMonth = Math.Round(totalCost / 5, 2);
            double avgCostToDrive = Math.Round(totalCost / totalDistance, 2);
            double avgEfficiency = Math.Round(totalDistance / totalFuel, 2);

            Assert.Equal(avgCostPerMonth, Math.Round(target.AverageCostPerMonth, 2));
            Assert.Equal(avgCostToDrive, Math.Round(target.AverageCostToDrive, 2));
            Assert.Equal(1, Math.Round(target.AverageFillupPrice, 2));
            Assert.Equal(avgEfficiency, Math.Round(target.AverageFuelEfficiency, 2));
            Assert.Equal(14000, target.Odometer);
            Assert.Equal(totalCost, Math.Round(target.TotalCost, 2));
            Assert.Equal(totalDistance, target.TotalDistance);
            Assert.Equal(fuelCost, Math.Round(target.TotalFuelCost, 2));
            Assert.Equal(totalFuel, Math.Round(target.TotalUnits, 2));
        }
    }
}