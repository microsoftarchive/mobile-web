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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenCanAddFillup
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IFillupRepository> _fillupRepo;
        private const int DefaultUserId = 99;
        private const int DefaultVehicleId = 88;

        public WhenCanAddFillup()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _fillupRepo = new Mock<IFillupRepository>();
        }

        [Fact]
        public void WhenCanAddFillup_ThenReturnsEmptyCollection()
        {
            var fillUp = new FillupEntryFormModel
                             {
                                 Date = DateTime.UtcNow,
                                 TotalUnits = 10,
                                 PricePerUnit = 1.0,
                             };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(new Vehicle { VehicleId = DefaultVehicleId, Name = "Vehicle" });

            _fillupRepo
                .Setup(x => x.GetFillups(It.IsAny<int>()))
                .Returns(new List<FillupEntry>());

            var handler = new CanAddFillup(_vehicleRepo.Object, _fillupRepo.Object);
            var actual = handler.Execute(DefaultUserId, DefaultVehicleId, fillUp);

            Assert.Empty(actual);
        }

        [Fact]
        public void WhenCanAddFillupWithInvalidVehicleId_ThenReturnsValidationResult()
        {
            var fillUp = new FillupEntryFormModel
                             {
                                 Date = DateTime.UtcNow,
                                 TotalUnits = 10,
                                 PricePerUnit = 1.0,
                             };

            var fillUps = new List<FillupEntry>();
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns((Vehicle)null);

            _fillupRepo
                .Setup(x => x.GetFillups(DefaultVehicleId))
                .Returns(fillUps);

            var handler = new CanAddFillup(_vehicleRepo.Object, _fillupRepo.Object);
            var actual = handler.Execute(DefaultUserId, DefaultVehicleId, fillUp);

            var actualList = new List<ValidationResult>(actual);
            Assert.Equal(1, actualList.Count);
            Assert.Contains("not found", actualList[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WhenCanAddFillupWithInvalidFillupOdometer_ThenReturnsValidationResult()
        {
            var fillUp = new FillupEntryFormModel
                             {
                                 Date = DateTime.UtcNow,
                                 TotalUnits = 10,
                                 PricePerUnit = 1.0,
                                 Odometer = 1500 //less than prior fillup
                             };

            var fillUps = new List<FillupEntry>
                              {
                                  new FillupEntry
                                      {
                                          FillupEntryId = 1,
                                          Date = DateTime.UtcNow.AddDays(-10),
                                          Odometer = 1000
                                      },
                                  new FillupEntry
                                      {
                                          FillupEntryId = 2,
                                          Date = DateTime.UtcNow.AddDays(-1),
                                          Odometer = 2000
                                      }
                              };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(new Vehicle { VehicleId = DefaultVehicleId, Name = "Vehicle" });

            _fillupRepo
                .Setup(x => x.GetFillups(DefaultVehicleId))
                .Returns(fillUps);

            var handler = new CanAddFillup(_vehicleRepo.Object, _fillupRepo.Object);
            var actual = handler.Execute(DefaultUserId, DefaultVehicleId, fillUp);

            var actualList = new List<ValidationResult>(actual);
            Assert.Equal(1, actualList.Count);
            Assert.Contains("odometer ", actualList[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}