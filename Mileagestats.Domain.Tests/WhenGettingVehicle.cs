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
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenGettingVehicle
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IFillupRepository> _fillupRepo;

        private const int UserId = 99;
        private const int DefaultVehicleId = 1;

        public WhenGettingVehicle()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _fillupRepo= new Mock<IFillupRepository>();
        }

        [Fact]
        public void ThenVehicleReturned()
        {
            var vehicle = new Vehicle { VehicleId = 1, Name = "vehicle" };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, vehicle.VehicleId))
                .Returns(vehicle);

            var handler = new GetVehicleById(_vehicleRepo.Object, _fillupRepo.Object);

            var retrievedVehicle = handler.Execute(UserId, vehicle.VehicleId);

            Assert.NotNull(retrievedVehicle);
            Assert.Equal("vehicle", retrievedVehicle.Name);
        }

        [Fact]
        public void ThenVehiclePhotoIdSet()
        {
            const int photoId = 99;

            var vehicle = new Vehicle { VehicleId = DefaultVehicleId, Name = "vehicle", PhotoId = photoId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, DefaultVehicleId))
                .Returns(vehicle);

            var handler = new GetVehicleById(_vehicleRepo.Object, _fillupRepo.Object);

            var retrievedVehicle = handler.Execute(UserId, vehicle.VehicleId);

            Assert.Equal(photoId, retrievedVehicle.PhotoId);
        }

        [Fact]
        public void ForOtherUser_ThenNullReturned()
        {
            const int notTheCurrentUserId = UserId + 1;

            var vehicle = new Vehicle { VehicleId = DefaultVehicleId, Name = "vehicle" };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(notTheCurrentUserId, DefaultVehicleId))
                .Returns((Vehicle)null);

            var handler = new GetVehicleById(_vehicleRepo.Object, _fillupRepo.Object);
            var retrievedVehicle = handler.Execute(notTheCurrentUserId, vehicle.VehicleId);

            Assert.Null(retrievedVehicle);
        }

        [Fact]
        public void WhenGettingVehicle_ThenVehicleStatisticsReturned()
        {
            var fillups = new List<FillupEntry>
                              {
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddDays(-10),
                                          Odometer = 500,
                                          PricePerUnit = 10.0,
                                          TotalUnits = 10.0
                                      },
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddDays(-5),
                                          Odometer = 1000,
                                          PricePerUnit = 10.0,
                                          TotalUnits = 10.0
                                      }
                              };

            var vehicle = new Vehicle { VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, DefaultVehicleId))
                .Returns(vehicle);

            _fillupRepo
                .Setup(r => r.GetFillups(DefaultVehicleId))
                .Returns(fillups);

            var handler = new GetVehicleById(_vehicleRepo.Object, _fillupRepo.Object);
            var retrievedVehicle = handler.Execute(UserId, vehicle.VehicleId);

            Assert.NotNull(retrievedVehicle);
            Assert.Equal(1000, retrievedVehicle.Odometer);
        }

        [Fact]
        public void WhenGettingVehicleWithOneFillup_ThenVehicleOdometerReflectsFillup()
        {
            var fillups = new List<FillupEntry>
                              {
                                  new FillupEntry
                                      {
                                          Date = DateTime.UtcNow.AddDays(-10),
                                          Odometer = 500,
                                          PricePerUnit = 10.0,
                                          TotalUnits = 10.0
                                      },
                              };

            var vehicle = new Vehicle { VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, DefaultVehicleId))
                .Returns(vehicle);

            _fillupRepo
                .Setup(r => r.GetFillups(DefaultVehicleId))
                .Returns(fillups);

            var handler = new GetVehicleById(_vehicleRepo.Object, _fillupRepo.Object);
            var retrievedVehicle = handler.Execute(UserId, vehicle.VehicleId);

            Assert.Equal(500, retrievedVehicle.Odometer);
        }
    }
}