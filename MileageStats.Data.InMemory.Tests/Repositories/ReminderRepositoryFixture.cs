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
using System.Linq;
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Data.InMemory.Tests.Repositories
{
    public class ReminderRepositoryFixture
    {
        private User defaultTestUser;
        private Vehicle defaultVehicle;

        public ReminderRepositoryFixture()
        {
            InitializeFixture();
        }

        private void InitializeFixture()
        {
            defaultTestUser = new User()
                                       {
                                           AuthorizationId = "TestAuthorizationId",
                                           DisplayName = "DefaultTestUser"
                                       };

            var userRepository = new UserRepository();
            userRepository.Create(defaultTestUser);

            int userId = defaultTestUser.UserId;

            var vehicleRepository = new VehicleRepository();
            defaultVehicle = new Vehicle()
                                      {
                                          Name = "Test Vehicle"
                                      };
            vehicleRepository.Create(defaultTestUser.UserId, defaultVehicle);
        }

        [Fact]
        public void WhenCreatingReminder_ThenPersists()
        {
            var repository = new ReminderRepository();

            Reminder reminder = new Reminder()
                                    {
                                        DueDate = DateTime.UtcNow.AddDays(30),
                                        DueDistance = 1000,
                                        Title = "Test Reminder"
                                    };

            repository.Create(defaultVehicle.VehicleId, reminder);

            var repositoryForVerification = new ReminderRepository();
            var returnedReminder =
                repositoryForVerification.GetRemindersForVehicle(defaultVehicle.VehicleId).First();

            Assert.NotNull(returnedReminder);
            Assert.Equal("Test Reminder", returnedReminder.Title);
        }

        [Fact]
        public void WhenDeletingReminder_ThenPersists()
        {
            var repository = new ReminderRepository();

            Reminder reminder = new Reminder()
                                    {
                                        DueDate = DateTime.UtcNow.AddDays(30),
                                        DueDistance = 1000,
                                        Title = "Test Reminder"
                                    };

            repository.Create(defaultVehicle.VehicleId, reminder);

            var repositoryForDelete = new ReminderRepository();

            Assert.Equal(1, repositoryForDelete.GetRemindersForVehicle(defaultVehicle.VehicleId).Count());

            repositoryForDelete.Delete(reminder.ReminderId);

            var repositoryForVerification = new ReminderRepository();
            var returnedReminders = repositoryForVerification.GetRemindersForVehicle(defaultVehicle.VehicleId);

            Assert.NotNull(returnedReminders);
            Assert.Equal(0, returnedReminders.Count());
        }

        [Fact]
        public void WhenUpdatingReminder_ThenPersists()
        {
            var repository = new ReminderRepository();

            Reminder reminder = new Reminder()
                                    {
                                        DueDate = DateTime.UtcNow.AddDays(30),
                                        DueDistance = 1000,
                                        Title = "Test Reminder"
                                    };

            repository.Create(defaultVehicle.VehicleId, reminder);

            var repositoryForUpdate = new ReminderRepository();

            var reminderToUpdate = repositoryForUpdate.GetRemindersForVehicle(defaultVehicle.VehicleId).First();
            reminderToUpdate.Title = "updated";

            repositoryForUpdate.Update(reminderToUpdate);

            var repositoryForVerification = new ReminderRepository();
            var returnedReminder =
                repositoryForVerification.GetRemindersForVehicle(defaultVehicle.VehicleId).First();

            Assert.NotNull(returnedReminder);
            Assert.Equal("updated", returnedReminder.Title);
        }

        [Fact]
        public void WhenRetrievingUpcomingReminders_ThenRemindersInDueDistanceRangeRetrieved()
        {
            DateTime dateStart = DateTime.UtcNow;
            DateTime dateEnd = DateTime.UtcNow.AddDays(5);
            int odometer = 1000;
            int warningThreshold = 500;

            var repository = new ReminderRepository();

            // reminders just inside range
            var inRangeReminder1 = new Reminder()
                               {
                                   DueDate = dateEnd.AddDays(30),
                                   DueDistance = odometer + 1,
                                   Title = "UpcomingReminder"
                               };
            repository.Create(defaultVehicle.VehicleId, 
                                inRangeReminder1);

            var inRangeReminder2 = new Reminder()
                               {
                                   DueDate = dateEnd.AddDays(30),
                                   DueDistance = odometer + warningThreshold,
                                   Title = "UpcomingReminder1"
                               };
            repository.Create(defaultVehicle.VehicleId,
                    inRangeReminder2);

            // reminders just outside of range
            repository.Create(defaultVehicle.VehicleId,
                              new Reminder()
                              {
                                  DueDate = dateEnd.AddDays(30),
                                  DueDistance = odometer,
                                  Title = "OutsideRangeReminder1"
                              });

            repository.Create(defaultVehicle.VehicleId,
                              new Reminder()
                                  {
                                      DueDate = dateEnd.AddDays(30),
                                      DueDistance = odometer + warningThreshold + 1,
                                      Title = "OutsideRangeReminder2"
                                  });

            var reminders = repository.GetUpcomingReminders(
                defaultVehicle.VehicleId,
                dateStart,
                dateEnd,
                odometer,
                warningThreshold
                );

            Assert.Equal(2, reminders.Count());
            Assert.True(reminders.Any(r => r.ReminderId == inRangeReminder1.ReminderId));
            Assert.True(reminders.Any(r => r.ReminderId == inRangeReminder2.ReminderId));
        }

        [Fact]
        public void WhenRetrievingUpcomingReminders_ThenRemindersInDueDateRangeRetrieved()
        {
            DateTime dateStart = DateTime.UtcNow;
            DateTime dateEnd = DateTime.UtcNow.AddDays(5);
            int odometer = 1000;
            int warningThreshold = 500;
            int outsideOdometerRange = odometer + warningThreshold + 1;

            var repository = new ReminderRepository();

            // reminders just inside range
            var inRangeReminder1 = new Reminder()
                               {
                                   DueDate = dateStart.AddDays(1),
                                   DueDistance = outsideOdometerRange,
                                   Title = "UpcomingReminder"
                               };
            repository.Create(defaultVehicle.VehicleId,
                                inRangeReminder1);

            var inRangeReminder2 = new Reminder()
                                {
                                    DueDate = dateEnd.AddDays(-1),
                                    DueDistance = outsideOdometerRange,
                                    Title = "UpcomingReminder1"
                                };
            repository.Create(defaultVehicle.VehicleId,
                    inRangeReminder2);

            // reminders just outside of range
            repository.Create(defaultVehicle.VehicleId,
                              new Reminder()
                              {
                                  DueDate = dateStart.AddDays(-1),
                                  DueDistance = outsideOdometerRange,
                                  Title = "OutsideRangeReminder1"
                              });

            repository.Create(defaultVehicle.VehicleId,
                              new Reminder()
                              {
                                  DueDate = dateEnd.AddDays(1),
                                  DueDistance = outsideOdometerRange,
                                  Title = "OutsideRangeReminder2"
                              });

            var reminders = repository.GetUpcomingReminders(
                defaultVehicle.VehicleId,
                dateStart,
                dateEnd,
                odometer,
                warningThreshold
                );

            Assert.Equal(2, reminders.Count());
            Assert.True(reminders.Any(r => r.ReminderId == inRangeReminder1.ReminderId));
            Assert.True(reminders.Any(r => r.ReminderId == inRangeReminder2.ReminderId));

        }
    }
}