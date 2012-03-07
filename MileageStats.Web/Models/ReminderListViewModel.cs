using System;
using System.Linq;
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class ReminderListViewModel
    {
        public ReminderStatus Status { get; set; }
        public IGrouping<ReminderStatus, ReminderSummaryModel> Reminders { get; set; } 
    }
}