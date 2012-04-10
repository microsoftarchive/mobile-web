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

using System.Collections.Generic;
using System.Linq;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;

namespace MileageStats.Domain.Handlers
{
    public class GetOverdueRemindersForVehicle
    {
        private readonly IReminderRepository _reminderRepository;

        public GetOverdueRemindersForVehicle(IReminderRepository reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

        public virtual IEnumerable<ReminderSummaryModel> Execute(int vehicleId, System.DateTime dueDate, int odometer)
        {
            return from reminder in _reminderRepository.GetOverdueReminders(vehicleId, dueDate, odometer)
                   orderby reminder.DueDistance
                   orderby reminder.DueDate
                   select new ReminderSummaryModel(reminder);
        }
    }
}