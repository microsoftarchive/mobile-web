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
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;


namespace MileageStats.Domain.Handlers
{
    public class GetOverdueRemindersForUser
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IReminderRepository _reminderRepository;
        private readonly IFillupRepository _fillupRepository;

        public GetOverdueRemindersForUser(IVehicleRepository vehicleRepository, IReminderRepository reminderRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _reminderRepository = reminderRepository;
            _fillupRepository = fillupRepository;
        }

        static ReminderModel ToViewModel(Reminder source)
        {
            if (source == null)
            {
                return null;
            }

            return new ReminderModel
                       {
                           DueDate = source.DueDate,
                           DueDistance = source.DueDistance,
                           Remarks = source.Remarks,
                           ReminderId = source.ReminderId,
                           Title = source.Title,
                           VehicleId = source.VehicleId,
                           IsFulfilled = source.IsFulfilled
                       };
        }

        public virtual IEnumerable<ReminderModel> Execute(int userId)
        {
            // this results in a SELECT N+1 scenario
            try
            {
                var vehicles = _vehicleRepository.GetVehicles(userId);

                var reminders = (from vehicle in vehicles
                                 let fillups = _fillupRepository.GetFillups(vehicle.VehicleId)
                                 let stats = CalculateStatistics.Calculate(fillups)
                                 let odometer = stats.Odometer
                                 from reminder in _reminderRepository.GetRemindersForVehicle(vehicle.VehicleId)
                                 select new {viewmodel = ToViewModel(reminder), odometer})
                                .ToList();

                foreach (var item in reminders)
                {
                    item.viewmodel.UpdateLastVehicleOdometer(item.odometer);
                }

                var overdueReminders = reminders
                    .Select( x=> x.viewmodel)
                    .Where(f => !f.IsFulfilled && f.IsOverdue)
                    .OrderBy(f => f.DueDistance)
                    .ThenBy(f => f.DueDate)
                    .ToList();

                return overdueReminders.ToList();
            }
            catch (InvalidOperationException ex)
            {
                throw new UnauthorizedException(Resources.UnableToRetrieveOverdueReminders, ex);
            }
        }
    }
}