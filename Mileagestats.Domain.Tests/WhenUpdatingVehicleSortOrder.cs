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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenUpdatingVehicleSortOrder
    {
        private const int UserId = 99;
        private readonly Mock<IVehicleRepository> _vehicleRepo;

        public WhenUpdatingVehicleSortOrder()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
        }

        [Fact]
        public void ThenDelegatesToVehicleRepositoryForEachVehicle()
        {
            var vehicleOne = new Vehicle {VehicleId = 1, Name = "oldName", SortOrder = 0};
            var vehicleTwo = new Vehicle {VehicleId = 2, Name = "oldName", SortOrder = 1};
            var vehicleThree = new Vehicle {VehicleId = 3, Name = "oldName", SortOrder = 2};

            _vehicleRepo.Setup(r => r.GetVehicle(It.IsAny<int>(), 1)).Returns(vehicleOne);
            _vehicleRepo.Setup(r => r.GetVehicle(It.IsAny<int>(), 2)).Returns(vehicleTwo);
            _vehicleRepo.Setup(r => r.GetVehicle(It.IsAny<int>(), 3)).Returns(vehicleThree);

            var newOrder = new[] {3, 2, 1};

            var handler = new UpdateVehicleSortOrder(_vehicleRepo.Object);
            handler.Execute(UserId, newOrder);

            _vehicleRepo.Verify(r => r.Update(vehicleOne), Times.Once());
            _vehicleRepo.Verify(r => r.Update(vehicleTwo), Times.Once());
            _vehicleRepo.Verify(r => r.Update(vehicleThree), Times.Once());
        }

        [Fact]
        public void ThenUpdatesSortOrderForEachVehicle()
        {
            var vehicleOne = new Vehicle {VehicleId = 1, Name = "oldName", SortOrder = 0};
            var vehicleTwo = new Vehicle {VehicleId = 2, Name = "oldName", SortOrder = 1};
            var vehicleThree = new Vehicle {VehicleId = 3, Name = "oldName", SortOrder = 2};

            _vehicleRepo.Setup(vr => vr.GetVehicle(It.IsAny<int>(), 1)).Returns(vehicleOne);
            _vehicleRepo.Setup(vr => vr.GetVehicle(It.IsAny<int>(), 2)).Returns(vehicleTwo);
            _vehicleRepo.Setup(vr => vr.GetVehicle(It.IsAny<int>(), 3)).Returns(vehicleThree);

            var newOrder = new[] {3, 2, 1};
            var handler = new UpdateVehicleSortOrder(_vehicleRepo.Object);
            handler.Execute(UserId, newOrder);

            Assert.Equal(2, vehicleOne.SortOrder);
            Assert.Equal(1, vehicleTwo.SortOrder);
            Assert.Equal(0, vehicleThree.SortOrder);
        }

        [Fact]
        public void AndRepositoryFails_ThenThrowsBusinessServicesException()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(UserId, It.IsAny<int>()))
                .Returns(new Vehicle());

            _vehicleRepo
                .Setup(b => b.Update(It.IsAny<Vehicle>()))
                .Throws(new InvalidOperationException());

            var newOrder = new[] {3, 2, 1};

            var handler = new UpdateVehicleSortOrder(_vehicleRepo.Object);

            var exception = Assert.Throws<UnauthorizedException>(() => handler.Execute(UserId, newOrder));
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }
    }
}