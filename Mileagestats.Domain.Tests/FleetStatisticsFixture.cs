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
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class FleetStatisticsFixture
    {
        private FleetStatistics target;

        public FleetStatisticsFixture()
        {
            this.InitializeFixture();
        }

        private void InitializeFixture()
        {
            this.target = new FleetStatistics(new[]
                                                  {
                                                      CalculateStatistics.Calculate(new[]
                                                                                        {
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-5),
                                                                                                    Odometer = 1000,
                                                                                                    Distance = null,
                                                                                                    PricePerUnit = 1,
                                                                                                    TotalUnits = 10,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-4),
                                                                                                    Odometer = 2000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 1,
                                                                                                    TotalUnits = 10,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-3),
                                                                                                    Odometer = 3000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 1,
                                                                                                    TotalUnits = 10,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-2),
                                                                                                    Odometer = 4000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 1,
                                                                                                    TotalUnits = 10,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-1),
                                                                                                    Odometer = 5000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 1,
                                                                                                    TotalUnits = 10,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                        }, includeFirst: false),
                                                      CalculateStatistics.Calculate(new[]
                                                                                        {
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-6),
                                                                                                    Odometer = 1000,
                                                                                                    Distance = null,
                                                                                                    PricePerUnit = 3,
                                                                                                    TotalUnits = 20,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-5),
                                                                                                    Odometer = 2000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 3,
                                                                                                    TotalUnits = 20,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-3),
                                                                                                    Odometer = 3000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 3,
                                                                                                    TotalUnits = 20,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-3),
                                                                                                    Odometer = 4000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 3,
                                                                                                    TotalUnits = 20,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow,
                                                                                                    Odometer = 5000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 3,
                                                                                                    TotalUnits = 20,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                        }, includeFirst: false),
                                                      CalculateStatistics.Calculate(new[]
                                                                                        {
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-12),
                                                                                                    Odometer = 3000,
                                                                                                    Distance = null,
                                                                                                    PricePerUnit = 2,
                                                                                                    TotalUnits = 30,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-10),
                                                                                                    Odometer = 4000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 2,
                                                                                                    TotalUnits = 30,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-8),
                                                                                                    Odometer = 5000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 2,
                                                                                                    TotalUnits = 30,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-6),
                                                                                                    Odometer = 6000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 2,
                                                                                                    TotalUnits = 30,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                            new FillupEntry
                                                                                                {
                                                                                                    Date =
                                                                                                        DateTime.UtcNow.
                                                                                                        AddMonths(-4),
                                                                                                    Odometer = 7000,
                                                                                                    Distance = 1000,
                                                                                                    PricePerUnit = 2,
                                                                                                    TotalUnits = 30,
                                                                                                    TransactionFee = 1.5
                                                                                                },
                                                                                        }, includeFirst: false)
                                                  });
        }

        [Fact]
        public void WhenConstructed_ThenInitializes()
        {
            var actual = new FleetStatistics(new VehicleStatisticsModel[0]);

            Assert.Equal(0.0, actual.AverageCostPerMonth);
            Assert.Equal(0.0, actual.AverageCostToDrive);
            Assert.Equal(0.0, actual.AverageFillupPrice);
            Assert.Equal(0.0, actual.AverageFuelEfficiency);
            Assert.Null(actual.Odometer);
            Assert.Equal(0.0, actual.TotalCost);
            Assert.Equal(0, actual.TotalDistance);
            Assert.Equal(0.0, actual.TotalFuelCost);
            Assert.Equal(0.0, actual.TotalUnits);
        }

        [Fact]
        public void WhenConstructedWithNull_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(() => { new FleetStatistics(null); });
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesAverageFillupPrice()
        {
            // each vehicle statistic
            double expected = (1.0 + 2.0 + 3.0)/3.0;
            Assert.Equal(expected, this.target.AverageFillupPrice);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesAverageFuelEfficiency()
        {
            double expected = Math.Round(((4000.0/(10.0*4.0)) + (4000.0/(20.0*4.0)) + (4000.0/(30.0*4.0)))/3.0, 2);
            Assert.Equal(expected, this.target.AverageFuelEfficiency);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesAverageCostToDrive()
        {
            // each vehicle statistic + tranaction fees for each vehicle statistic
            double expectedTotalCost = (10*1*4) + (20*3*4) + (30*2*4) + (1.5*(4*3));
            double expectedTotalDistance = 4000 + 4000 + 4000;
            double expected = Math.Round(expectedTotalCost/expectedTotalDistance, 2);
            Assert.Equal(expected, this.target.AverageCostToDrive);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesAverageCostPerMonth()
        {
            double expected1 = ((10*1*4) + (1.5*4))/5;
            double expected2 = ((20*3*4) + (1.5*4))/6;
            double expected3 = ((30*2*4) + (1.5*4))/12;
            double expected = Math.Round(((expected1 + expected2 + expected3)/3.0), 2);
            Assert.Equal(expected, this.target.AverageCostPerMonth);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesOdometer()
        {
            int expected = 7000;
            Assert.Equal(expected, this.target.Odometer);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesTotalDistance()
        {
            // each vehicle statistic
            double expected = 4000 + 4000 + 4000;
            Assert.Equal(expected, this.target.TotalDistance);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesTotalFuelCost()
        {
            // each vehicle statistic
            double expected = (10*1*4) + (20*3*4) + (30*2*4);
            Assert.Equal(expected, this.target.TotalFuelCost);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesTotalUnits()
        {
            // each vehicle statistic
            double expected = (10*4) + (20*4) + (30*4);
            Assert.Equal(expected, this.target.TotalUnits);
        }

        [Fact]
        public void WhenConstructed_ThenCalculatesTotalCost()
        {
            // each vehicle statistic + tranaction fees for each vehicle statistic
            double expected = (10*1*4) + (20*3*4) + (30*2*4) + (1.5*(4*3));
            Assert.Equal(expected, this.target.TotalCost);
        }

        [Fact]
        public void WhenRecalculated_ThenCalculationsUpdated()
        {
        }
    }
}