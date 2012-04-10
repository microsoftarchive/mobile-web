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
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenGettingVehicles
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IFillupRepository> _fillupRepo;

        private const int UserId = 99;

        public WhenGettingVehicles()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _fillupRepo = new Mock<IFillupRepository>();
        }

        [Fact]
        public void ByUserIdForUserWithNoVehicles_ThenReturnsEmptyCollection()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicles(UserId))
                .Returns(new Vehicle[] { })
                .Verifiable();

            var handler = new GetVehicleListForUser(_vehicleRepo.Object, _fillupRepo.Object);

            var result = handler.Execute(UserId);

            _vehicleRepo.Verify();
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void ByUserIdForUser_ThenReturnsVehicles()
        {
            var vehicles = new List<Vehicle> { new Vehicle() };

            _vehicleRepo
                .Setup(vr => vr.GetVehicles(UserId))
                .Returns(vehicles);

            var result = new GetVehicleListForUser(_vehicleRepo.Object, _fillupRepo.Object).Execute(UserId);

            Assert.Equal(vehicles.Count, result.Count());
        }

        [Fact]
        public void ByUserIdForUser_ThenReturnsVehiclesInSortedOrder()
        {
            var vehicle01 = new Vehicle { Name = "first", VehicleId = 4, SortOrder = 1 };
            var vehicle02 = new Vehicle { Name = "second", VehicleId = 1, SortOrder = 3 };
            var vehicle03 = new Vehicle { Name = "third", VehicleId = 2, SortOrder = 2 };

            var vehicles = new List<Vehicle> { vehicle03, vehicle02, vehicle01 };

            _vehicleRepo
                .Setup(vr => vr.GetVehicles(UserId))
                .Returns(vehicles);

            var handler = new GetVehicleListForUser(_vehicleRepo.Object, _fillupRepo.Object);

            var result = handler.Execute(UserId);
            var acutal = result.ToArray();

            Assert.Equal(vehicle01.Name, acutal[0].Name);
            Assert.Equal(vehicle03.Name, acutal[1].Name);
            Assert.Equal(vehicle02.Name, acutal[2].Name);
        }
    }
}