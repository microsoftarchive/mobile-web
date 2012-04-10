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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;


namespace MileageStats.Domain.Handlers
{
    public class AddReminderToVehicle
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IReminderRepository _reminderRepository;

        public AddReminderToVehicle(IVehicleRepository vehicleRepository, IReminderRepository reminderRepository)
        {
            _vehicleRepository = vehicleRepository;
            _reminderRepository = reminderRepository;
        }

        public virtual void Execute(int userId, int vehicleId, ICreateReminderCommand reminder)
        {
            if (reminder == null) throw new ArgumentNullException("reminder");
            try
            {
                var vehicle = _vehicleRepository.GetVehicle(userId, vehicleId);
                if (vehicle != null)
                {
                    reminder.VehicleId = vehicleId;
                    var entity = ToEntity(reminder);

                    _reminderRepository.Create(vehicleId, entity);

                    // Update reminder fields
                    reminder.ReminderId = entity.ReminderId;
                    reminder.VehicleId = entity.VehicleId;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new UnauthorizedException(Resources.UnableToAddReminderToVehicleExceptionMessage, ex);
            }
        }

        static Reminder ToEntity(ICreateReminderCommand source)
        {
            if (source == null)
            {
                return null;
            }

            return new Reminder
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
    }
}