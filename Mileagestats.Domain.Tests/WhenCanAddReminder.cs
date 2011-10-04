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

using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;

using MileageStats.Web.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenCanAddReminder
    {
        private const int DefaultUserId = 99;
        private const int DefaultVehicleId = 77;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        readonly Mock<IFillupRepository> _fillupRepo = new Mock<IFillupRepository>();

        public WhenCanAddReminder()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        }

        [Fact]
        public void WhenCanAddReminder_ThenReturnsEmptyCollection()
        {
            var vehicle1 = new Vehicle { VehicleId = DefaultVehicleId, Name = "vehicle1" };

            _vehicleRepositoryMock
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(vehicle1);

            var handler = new CanAddReminder(_vehicleRepositoryMock.Object, _fillupRepo.Object);

            var formModel = new ReminderFormModel { VehicleId = DefaultVehicleId, Title = "Test", DueDistance = 20000 };

            var result = handler.Execute(DefaultUserId, formModel);

            Assert.Empty(result);
        }

        [Fact]
        public void WhenCanAddReminderWithInvalidVehicleId_ThenReturnsValidationResult()
        {
            const int nonExistentVehicleId = -1;

            _vehicleRepositoryMock
                .Setup(vr => vr.GetVehicle(DefaultUserId, nonExistentVehicleId))
                .Returns((Vehicle)null);

            var handler = new CanAddReminder(_vehicleRepositoryMock.Object, _fillupRepo.Object);

            var formModel = new ReminderFormModel { VehicleId = nonExistentVehicleId, Title = "Test", DueDistance = 20000 };

            var result = handler.Execute(DefaultUserId, formModel);

            Assert.NotEmpty(result);
        }

        [Fact]
        public void WhenCanAddReminderWithInvalidDueDistance_ThenReturnsValidationResult()
        {
            var vehicle1 = new Vehicle
                               {
                                   VehicleId = DefaultVehicleId,
                                   Name = "vehicle1"
                               };

            _vehicleRepositoryMock
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(vehicle1);

            _fillupRepo
                .Setup(r => r.GetFillups(DefaultVehicleId))
                .Returns(new[] {new FillupEntry {Odometer = 7000}});

            var handler = new CanAddReminder(_vehicleRepositoryMock.Object, _fillupRepo.Object);

            var formModel = new ReminderFormModel { VehicleId = DefaultVehicleId, Title = "Test", DueDistance = 5000 };

            var result = handler.Execute(DefaultUserId, formModel);

            Assert.NotEmpty(result);
        }
    }
}