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

using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString DayPickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper, 
            Expression<Func<TModel, TProperty>> expression, DateTime date, bool required)
        {
            var days = new List<SelectListItem>();
            
            if (!required)
            {
                days.Add(new SelectListItem());
            }

            for (int i = 1; i < 32; i++)
            {
                days.Add(new SelectListItem
                                {
                                    Value = i.ToString(),
                                    Text = i.ToString(),
                                    Selected = (i == date.Day)
                                });
            }

            return helper.DropDownListFor(expression, days);
        }

        public static IHtmlString MonthPickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, DateTime date, bool required)
        {
            var months = new List<SelectListItem>();
            
            if (!required)
            {
                months.Add(new SelectListItem());
            }

            for (int i = 1; i < 13; i++)
            {
                months.Add(new SelectListItem
                               {
                                   Value = i.ToString(),
                                   Text = string.Format("{0:MMM}", new DateTime(date.Year, i, 1)),
                                   Selected = (i == date.Month)
                               });
            }

            return helper.DropDownListFor(expression, months);
        }

        public static IHtmlString YearPickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, TProperty>> expression, DateTime date, bool required)
        {
            var year = DateTime.Now.Year;

            var years = new List<SelectListItem>();

            if (!required)
            {
                years.Add(new SelectListItem());
            }

            for (int i = year-1; i < year+3; i++)
            {
                years.Add(new SelectListItem
                                {
                                    Value = i.ToString(),
                                    Text = i.ToString(),
                                    Selected = (i == date.Year)
                                });
            }
            
            return helper.DropDownListFor(expression, years);
        }

        public static IHtmlString PriorMonthYearPickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, DateTime? selectedDate, bool required)
        {
            var months = new List<SelectListItem>();

            if (!required)
            {
                months.Add(new SelectListItem());
            }

            for (int i = 0; i > -10; i--)
            {
                var date = DateTime.Now.AddMonths(i);
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                months.Add(new SelectListItem
                               {
                                   Value = firstDayOfMonth.ToShortDateString(),
                                   Text = string.Format("{0:MMMM yyyy}", firstDayOfMonth),
                                   Selected = (selectedDate.HasValue ? firstDayOfMonth == selectedDate.Value : false)
                               });
            }
            return helper.DropDownListFor(expression, months);
        }

        public static IHtmlString PriorMonthEndYearPickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, DateTime? selectedDate, bool required)
        {
            var months = new List<SelectListItem>();

            if (!required)
            {
                months.Add(new SelectListItem());
            }

            for (int i = 0; i > -10; i--)
            {
                var date = DateTime.Now.AddMonths(i);
                var nextMonth = date.AddMonths(1);
                var firstDayOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                var lastDayOfThisMonth = firstDayOfNextMonth.AddDays(-1);
                months.Add(new SelectListItem
                {
                    Value = lastDayOfThisMonth.ToShortDateString(),
                    Text = string.Format("{0:MMMM yyyy}", lastDayOfThisMonth),
                    Selected = (selectedDate.HasValue ? lastDayOfThisMonth == selectedDate.Value : false)
                });
            }
            return helper.DropDownListFor(expression, months);
        }

        public static IHtmlString FixedValueTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return FixedValueTextBoxFor(helper, expression, null, htmlAttributes);
        }

        public static IHtmlString FixedValueTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            return helper.TextBox(name, (value == null) ? "" : value, htmlAttributes);
        }

        
    }
}