using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

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
    }
}