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
using System.Linq;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenGettingReminders
    {
        private const int DefaultUserId = 99;
        private const int DefaultVehicleId = 77;
        private const int CurrentOdometer = 10000;
        private readonly Mock<IReminderRepository> _reminderRepository;

        public WhenGettingReminders()
        {
            _reminderRepository = new Mock<IReminderRepository>();
        }

        [Fact]
        public void WhenGettingAllReminders_ThenReturnsAllUnfulfilledReminders()
        {
            _reminderRepository
                .Setup(r => r.GetRemindersForVehicle(DefaultVehicleId))
                .Returns(GetListOfReminders);

            var handler = new GetUnfulfilledRemindersForVehicle(_reminderRepository.Object);

            var reminders = handler.Execute(DefaultUserId, DefaultVehicleId, CurrentOdometer);

            Assert.Equal(7, reminders.Count());
        }

        [Fact]
        public void WhenGettingOverdueRemindersForVehicle_ThenSortByDateAndDistance()
        {
            var today = DateTime.UtcNow;

            var reminders = new[]
                                {
                                    new Reminder
                                        {
                                            ReminderId = 1,
                                            DueDate = today.AddDays(5),
                                            DueDistance = CurrentOdometer + 10000,
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 2,
                                            DueDate = today.AddDays(4),
                                            DueDistance = CurrentOdometer + 10000,
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 3,
                                            DueDate = today.AddDays(5),
                                            DueDistance = CurrentOdometer + 499,
                                        },
                                };

            _reminderRepository
                    .Setup(r => r.GetOverdueReminders(DefaultVehicleId, today, CurrentOdometer))
                    .Returns(reminders);

            var handler = new GetOverdueRemindersForVehicle(_reminderRepository.Object);

            var result = handler.Execute(DefaultVehicleId, today, CurrentOdometer)
                .ToArray();

            Assert.Equal(2, result[0].ReminderId);
            Assert.Equal(3, result[1].ReminderId);
            Assert.Equal(1, result[2].ReminderId);
        }

        [Fact]
        public void WhenGettingAllReminders_ThenReturnsAllReminders()
        {
            _reminderRepository
                .Setup(r => r.GetRemindersForVehicle(DefaultVehicleId))
                .Returns(GetListOfReminders);

            var handler = new GetAllRemindersForVehicle(_reminderRepository.Object);

            var reminders = handler.Execute(DefaultVehicleId);

            Assert.Equal(8, reminders.Count());
            Assert.True(reminders.Last().IsFulfilled);
        }

        private static IEnumerable<Reminder> GetListOfReminders()
        {
            var currentDate = DateTime.UtcNow;
            var reminders = new List<Reminder>
                                {
                                    new Reminder
                                        {
                                            ReminderId = 1,
                                            DueDate = currentDate.AddDays(300),
                                            DueDistance = CurrentOdometer + 10000,
                                            Title = "Future Reminder"
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 2,
                                            DueDate = currentDate.AddDays(15),
                                            DueDistance = CurrentOdometer + 10000,
                                            Title = "Upcoming by Date Reminder"
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 3,
                                            DueDate = currentDate.AddDays(300),
                                            DueDistance = CurrentOdometer + 499,
                                            Title = "Upcoming by Mileage Reminder"
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 4,
                                            DueDate = currentDate.AddDays(-5),
                                            DueDistance = CurrentOdometer + 10000,
                                            Title = "Overdue By Date"
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 5,
                                            DueDate = currentDate.AddDays(30),
                                            DueDistance = CurrentOdometer - 10,
                                            Title = "OverDue By Mileage"
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 6,
                                            DueDate = DateTime.UtcNow.AddDays(-1),
                                            DueDistance = CurrentOdometer - 1,
                                            Title = "Overdue by date and mileage"
                                        },
                                    new Reminder
                                        {
                                            ReminderId = 7,
                                            DueDate = DateTime.UtcNow.AddDays(45),
                                            DueDistance = null,
                                            Title = "Not Upcoming or Overdue and DueDistance null"
                                        },
                                };

            var fulfilledReminder = new Reminder
                                        {
                                            ReminderId = 7,
                                            DueDate = DateTime.UtcNow.AddDays(-1),
                                            DueDistance = CurrentOdometer - 1,
                                            Title = "Fulfilled reminder",
                                            IsFulfilled = true
                                        };

            reminders.Add(fulfilledReminder);
            return reminders.AsEnumerable();
        }
    }
}