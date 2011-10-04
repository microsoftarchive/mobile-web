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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class FulfillReminder
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public FulfillReminder(IReminderRepository reminderRepository, IVehicleRepository vehicleRepository)
        {
            _reminderRepository = reminderRepository;
            _vehicleRepository = vehicleRepository;
        }

        public virtual void Execute(int userId, int reminderId)
        {
            var reminder = _reminderRepository.GetReminder(reminderId);

            if (!_vehicleRepository.GetVehicles(userId)
                .Any(v => v.Id == reminder.VehicleId))
            {
                throw new UnauthorizedException(Resources.UpdateReminderUnauthorized);
            }

            reminder.IsFulfilled = true;
            
            _reminderRepository.Update(reminder);
        }
    }
}