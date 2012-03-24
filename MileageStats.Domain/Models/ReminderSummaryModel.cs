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

namespace MileageStats.Domain.Models
{
    public class ReminderSummaryModel
    {
        private readonly Reminder _reminder;

        public ReminderSummaryModel(Reminder reminder)
        {
            _reminder = reminder;
        }

        public int ReminderId
        {
            get { return _reminder.ReminderId; }
        }

        public int VehicleId
        {
            get { return _reminder.VehicleId; }
        }

        public string Title
        {
            get { return _reminder.Title; }
        }

        public bool IsOverdue
        {
            get { return _reminder.IsOverdue.HasValue && _reminder.IsOverdue.Value; }
        }

        public bool IsFulfilled
        {
            get { return _reminder.IsFulfilled; }
        }

        public string DueOnFormatted
        {
            get
            {
                var msg = _reminder.DueDate == null
                                 ? string.Empty
                                 : String.Format("on {0:d MMM yyyy}", _reminder.DueDate);

                msg += _reminder.DueDate == null || _reminder.DueDistance == null
                           ? string.Empty
                           : " or ";

                msg += _reminder.DueDistance == null
                           ? string.Empty
                           : String.Format("at {0} miles", _reminder.DueDistance);

                return msg + ".";
            }
        }

        public string Remarks
        {
            get { return string.IsNullOrEmpty(_reminder.Remarks) 
                        ? "(not entered)"
                        : _reminder.Remarks; 
            }
        }

        public string DueDate
        {
            get { 
                return _reminder.DueDate.HasValue
                        ? String.Format("{0:d MMM yyyy}", _reminder.DueDate)
                        : "(not entered)"; 
            }
        }

        public string DueDistance
        {
            get { return _reminder.DueDistance.HasValue
                        ? _reminder.DueDistance.ToString()
                        : "(not entered)";
            }
        }

        public ReminderState Status
        {
            get
            {
                if (IsFulfilled) return ReminderState.Fulfilled;
                if (IsOverdue) return ReminderState.Overdue;
                return ReminderState.Active;
            }
        }
    }
}