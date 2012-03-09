using System;
using System.Linq;
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class ReminderListViewModel
    {
        public string Status { get; set; }
        public IGrouping<string, ReminderSummaryModel> Reminders { get; set; } 
    }
}