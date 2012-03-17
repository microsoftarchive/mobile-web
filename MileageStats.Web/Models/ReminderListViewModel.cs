using System;
using System.Linq;
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class ReminderListViewModel
    {
        public ReminderState Status { get; set; }
        public IGrouping<ReminderState, ReminderSummaryModel> Reminders { get; set; } 
        public string StatusName 
        {
            get { return Status.ToString(); }
        }
    }
}