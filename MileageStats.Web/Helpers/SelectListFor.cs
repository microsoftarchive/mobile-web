using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MileageStats.Domain.Properties;

namespace MileageStats.Web.Helpers
{

    // assists with constructing select list for dates
    // each helper takes a function for setting the 
    // selected item on in the select list

    public static class SelectListFor
    {
        public static IEnumerable<SelectListItem> Month(Func<int, bool> isSelected)
        {
            return from i in Enumerable.Range(1, 12)
                   let value = i.ToString(CultureInfo.InvariantCulture)
                   let date = new DateTime(2000, i, 1)
                   select new SelectListItem
                              {
                                  Value = value,
                                  Text = string.Format("{0:MMM}", date),
                                  Selected = isSelected(i)
                              };
        }

        public static IEnumerable<SelectListItem> Date(Func<int, bool> isSelected)
        {
            return from i in Enumerable.Range(1, 31)
                   let value = i.ToString(CultureInfo.InvariantCulture)
                   select new SelectListItem
                   {
                       Value = value,
                       Text = value,
                       Selected = isSelected(i)
                   };
        }

        public static IEnumerable<SelectListItem> Year(Func<int, bool> isSelected)
        {
            var current = DateTime.Now.Year;
            return from i in Enumerable.Range(current - 2, 3)
                   let value = i.ToString(CultureInfo.InvariantCulture)
                   select new SelectListItem
                   {
                       Value = value,
                       Text = value,
                       Selected = isSelected(i)
                   };
        }

        public static IEnumerable<SelectListItem> PriorMonthStartYear(Func<DateTime, bool> isSelected)
        {
            var months = new List<SelectListItem>();

            for (int i = 0; i > -12; i--)
            {
                var date = DateTime.Now.AddMonths(i);
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                months.Add(new SelectListItem
                {
                    Value = firstDayOfMonth.ToShortDateString(),
                    Text = string.Format("{0:MMMM yyyy}", firstDayOfMonth),
                    Selected = isSelected(firstDayOfMonth)
                });
            }
            return months;
        }

        public static IEnumerable<SelectListItem> PriorMonthEndYear(Func<DateTime, bool> isSelected)
        {
            var months = new List<SelectListItem>();

            for (int i = 0; i > -12; i--)
            {
                var date = DateTime.Now.AddMonths(i);
                var nextMonth = date.AddMonths(1);
                var firstDayOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                var lastDayOfThisMonth = firstDayOfNextMonth.AddDays(-1);
                months.Add(new SelectListItem
                {
                    Value = lastDayOfThisMonth.ToShortDateString(),
                    Text = string.Format("{0:MMMM yyyy}", lastDayOfThisMonth),
                    Selected = isSelected(lastDayOfThisMonth)
                });
            }
            return months;
        } 

        public static IEnumerable<SelectListItem> Charts()
        {
            var allCharts = new List<SelectListItem>();
            allCharts.Add(new SelectListItem { Text = @Resources.ChartController_AverageFuelEfficiencyChart_Title, Value = "FuelEfficiency" });
            allCharts.Add(new SelectListItem { Text = @Resources.ChartController_TotalDistance_Title, Value = "TotalDistance" });
            allCharts.Add(new SelectListItem { Text = @Resources.ChartController_TotalCost_Title, Value = "TotalCost" });
            return allCharts;
        } 
    }
}