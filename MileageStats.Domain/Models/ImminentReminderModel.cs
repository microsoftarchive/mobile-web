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

namespace MileageStats.Domain.Models
{
    public class ImminentReminderModel
    {
        private readonly Vehicle _vehicle;
        private readonly Reminder _reminder;
        private readonly ReminderSummaryModel _summary;

        public ImminentReminderModel(Vehicle vehicle, Reminder reminder, bool isOverdue)
        {
            IsOverdue = isOverdue;
            _vehicle = vehicle;
            _reminder = reminder;
            _summary = new ReminderSummaryModel(reminder);
        }

        public int VehicleId { get { return _vehicle.VehicleId; } }
        public string VehicleMakeName { get { return _vehicle.MakeName; } }
        public string VehicleModelName { get { return _vehicle.ModelName; } }
        public ReminderSummaryModel Reminder { get { return _summary; } }
        public bool IsOverdue { get; private set; }
        internal Reminder Inner { get { return _reminder; } }
        public string FormattedDueDate
        {
            get
            {
                return _reminder.DueDate.HasValue
                           ? String.Format("{0:d MMM yyyy}", _reminder.DueDate)
                           : null;
            }
        }

        #region Comparison Methods

        public static int CompareImminentReminders(ImminentReminderModel x, ImminentReminderModel y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x != null && y == null)
            {
                return -1; // I want non-null values first
            }
            else if (x == null && y != null)
            {
                return 1; // I want non-null values first
            }
            else
            {
                return CompareReminders(x.Inner, y.Inner);
            }
        }

        private static int CompareReminders(Reminder x, Reminder y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x != null && y == null)
            {
                return -1; // I want non-null values first
            }
            else if (x == null && y != null)
            {
                return 1; // I want non-null values first
            }
            else
            {
                int result = CompareDueDate(x, y);

                if (result == 0)
                {
                    result = CompareDueDistance(x, y);

                    if (result == 0)
                    {
                        result = x.ReminderId.CompareTo(y.ReminderId);
                    }
                }

                return result;
            }
        }

        private static int CompareDueDate(Reminder x, Reminder y)
        {
            if (!x.DueDate.HasValue && !y.DueDate.HasValue)
            {
                return 0;
            }
            else if (x.DueDate.HasValue && !y.DueDate.HasValue)
            {
                return -1; // I want non-null values first
            }
            else if (!x.DueDate.HasValue && y.DueDate.HasValue)
            {
                return 1; // I want non-null values first
            }
            else
            {
                return x.DueDate.Value.CompareTo(y.DueDate.Value);
            }
        }

        private static int CompareDueDistance(Reminder x, Reminder y)
        {
            if (!x.DueDistance.HasValue && !y.DueDistance.HasValue)
            {
                return 0;
            }
            else if (x.DueDistance.HasValue && !y.DueDistance.HasValue)
            {
                return -1; // I want non-null values first
            }
            else if (!x.DueDistance.HasValue && y.DueDistance.HasValue)
            {
                return 1; // I want non-null values first
            }
            else
            {
                return x.DueDistance.Value.CompareTo(y.DueDistance.Value);
            }
        }

        #endregion
    }
}