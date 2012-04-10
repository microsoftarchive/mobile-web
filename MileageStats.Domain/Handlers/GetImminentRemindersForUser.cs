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
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;


namespace MileageStats.Domain.Handlers
{
    public class GetImminentRemindersForUser
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IReminderRepository _reminderRepository;
        private readonly IFillupRepository _fillupRepository;

        private const int UpcomingReminderOdometerTolerance = 500;
        private const int UpcomingReminderDaysTolerance = 15;

        public GetImminentRemindersForUser(IVehicleRepository vehicleRepository, IReminderRepository reminderRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _reminderRepository = reminderRepository;
            _fillupRepository = fillupRepository;
        }

        public virtual IEnumerable<ImminentReminderModel> Execute(int userId, DateTime dueDate, int selectedVehicle = 0)
        {
            var vehicles = _vehicleRepository.GetVehicles(userId);

            var reminders = new List<AggregatedData>();

            //NOTE: the structure of the data forces us into a SELECT N+1 scenario
            foreach (var vehicle in vehicles)
            {
                var fillups = _fillupRepository.GetFillups(vehicle.VehicleId);
                var stats = CalculateStatistics.Calculate(fillups);
                var odometer = stats.Odometer ?? 0;

                var overdue = _reminderRepository
                    .GetOverdueReminders(vehicle.VehicleId, dueDate, odometer)
                    .Select(r => new AggregatedData
                                {
                                    IsOverdue = true,
                                    Reminder = r,
                                    Vehicle = vehicle
                                })
                     .ToList();

                var end = dueDate.AddDays(UpcomingReminderDaysTolerance).Date;
                var start = dueDate.Date;
                
                var upcoming =  _reminderRepository
                    .GetUpcomingReminders(vehicle.VehicleId, start, end, odometer, UpcomingReminderOdometerTolerance)
                    .Select(r => new AggregatedData
                                {
                                    IsOverdue = false,
                                    Reminder = r,
                                    Vehicle = vehicle
                                })
                      // we need to exclude reminders that qualify as both overdue and upcoming
                      // performing this check in memory is less than desirable
                     .Where( r=> !overdue.Any(o => o.Reminder.ReminderId == r.Reminder.ReminderId))
                     .ToList();

                reminders.AddRange(overdue);
                reminders.AddRange(upcoming);
            }

            var result = reminders
                .Select(x => new ImminentReminderModel(x.Vehicle, x.Reminder, x.IsOverdue))
                .ToList();

                result.Sort(ImminentReminderModel.CompareImminentReminders);

            return result;
        }

        class AggregatedData
        {
            public Reminder Reminder { get; set; }
            public Vehicle Vehicle { get; set; }
            public bool IsOverdue { get; set; }
        }
    }
}