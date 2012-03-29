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
using MileageStats.Web.Infrastructure;

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

        public static string RouteValue<TModel>(this HtmlHelper<TModel> helper, string key)
        {
            var value = helper.ViewContext.RouteData.Values[key];
            return (value == null) ? string.Empty : value.ToString();
        }

        public static IDisposable EnableUnobtrusiveValidation<TModel>(this HtmlHelper<TModel> helper)
        {
            // the data-dash attributes for MVC's unobtrustive validation 
            // are not emitted unless the following code is executed
            // generally, this is handled by the BeginForm helper
            // however, in order to render a form action that was compatiable
            // with the Mustache version of the view, we had to implement
            // this logic manually.
            // this is not required in with MVC 4:
            // http://bradwilson.typepad.com/blog/2010/10/mvc3-unobtrusive-validation.html

            // in general, this approach should be used with caution.
            helper.ViewContext.FormContext = new FormContext { FormId = "form" };
            return new DelegatingDisposable(helper.ViewContext.OutputClientValidation);
        }
    }
}