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
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Moq;
using Xunit;

namespace Mileagestats.Domain.Tests
{
    public class WhenGetFleetSummaryStatistics
    {
        private const int DefaultUserId = 99;

        [Fact]
        public void WhenGetFleetSummaryStatistics_CalculatesAcrossVehicles()
        {
            var fillups1 = new[]
                            {
                                new FillupEntry {Date = DateTime.UtcNow.AddDays(-1), PricePerUnit = 1.0, TotalUnits = 10},
                                new FillupEntry {Date = DateTime.UtcNow, PricePerUnit = 1.0, TotalUnits = 10}
                            };

            var fillups2 = new[]
                            {
                                new FillupEntry {Date = DateTime.UtcNow.AddDays(-1), PricePerUnit = 1.0, TotalUnits = 10},
                                new FillupEntry {Date = DateTime.UtcNow, PricePerUnit = 1.0, TotalUnits = 10},
                                new FillupEntry
                                    {
                                        Date = DateTime.UtcNow,
                                        PricePerUnit = 1.0,
                                        TotalUnits = 10,
                                        TransactionFee = 5.0
                                    }
                            };

            var vehicle1 = new Vehicle { VehicleId = 1 };
            var vehicle2 = new Vehicle { VehicleId = 2 };

            var mockVehicleRepository = new Mock<IVehicleRepository>();
            var mockFillupsRepository = new Mock<IFillupRepository>();

            mockVehicleRepository
                .Setup(x => x.GetVehicles(DefaultUserId))
                .Returns(new[] { vehicle1, vehicle2 });

            mockFillupsRepository
                .Setup(x => x.GetFillups(vehicle1.VehicleId))
                .Returns(fillups1);

            mockFillupsRepository
                .Setup(x => x.GetFillups(vehicle2.VehicleId))
                .Returns(fillups2);

            var handler = new GetFleetSummaryStatistics(mockVehicleRepository.Object,mockFillupsRepository.Object);

            var actual = handler.Execute(DefaultUserId);

            Assert.NotNull(actual);
            Assert.Equal(30, actual.TotalFuelCost);
            Assert.Equal(35, actual.TotalCost);
        }
    }
}