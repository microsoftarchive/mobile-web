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
using MileageStats.Web.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenAddingReminder
    {
        private const int DefaultUserId = 99;
        private const int DefaultVehicleId = 77;
        private const int CurrentOdometer = 10000;
        private readonly Mock<IReminderRepository> _reminderRepository;
        private readonly Mock<IVehicleRepository> _vehicleRepository;

        public WhenAddingReminder()
        {
            _vehicleRepository = new Mock<IVehicleRepository>();
            _reminderRepository = new Mock<IReminderRepository>();
        }

        [Fact]
        public void WhenAddingReminder_ThenDelegatesToReminderRepository()
        {
            var vehicle = new Vehicle { VehicleId = DefaultVehicleId, Name = "vehicle" };

            _vehicleRepository
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(vehicle);

            var handler = new AddReminderToVehicle(_vehicleRepository.Object, _reminderRepository.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, new ReminderFormModel());

            _reminderRepository
                .Verify(r => r.Create(DefaultVehicleId, It.IsAny<Reminder>()), Times.Once());
        }

        [Fact]
        public void WhenAddingReminder_ThenUpdatesServiceReminder()
        {
            const int newReminderId = 456;

            var vehicle = new Vehicle { VehicleId = DefaultVehicleId, Name = "vehicle" };

            _vehicleRepository
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(vehicle);

            _reminderRepository
                .Setup(r => r.Create(DefaultVehicleId, It.IsAny<Reminder>()))
                .Callback(new Action<int, Reminder>((vehicleId, reminder) =>
                                                        {
                                                            // represents the entity created internally
                                                            reminder.ReminderId = newReminderId;
                                                            reminder.VehicleId = DefaultVehicleId;
                                                        }));

            var formModel = new ReminderFormModel();

            var handler = new AddReminderToVehicle(_vehicleRepository.Object, _reminderRepository.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, formModel);

            Assert.Equal(newReminderId, formModel.ReminderId);
            Assert.Equal(DefaultVehicleId, formModel.VehicleId);
        }
    }
}