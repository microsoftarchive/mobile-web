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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MileageStats.Web.Helpers.Mustache;

namespace MileageStats.Web.Helpers
{
    public static class MustacheTemplateExtensions
    {
        private const string UseMustacheFlag = "__use_mustache";

        public static void RenderAsMustacheTemplate(this HtmlHelper helper, string view)
        {
            helper.RenderPartial(view, new ViewDataDictionary {{UseMustacheFlag, ""}});
        }

        public static bool IsRenderingForMustache<TModel>(this HtmlHelper<TModel> helper)
        {
            return helper.ViewData.ContainsKey(UseMustacheFlag);
        }

        public static MvcHtmlString Value<TModel, TProperty>(this MustacheHelper<TModel> helper,
                                                             Expression<Func<TModel, TProperty>> getter)
        {
            if (helper.ViewData.Model == null)
            {
                string name = ExpressionHelper.GetExpressionText(getter);
                return new MvcHtmlString(string.Format("{{{{{0}}}}}", name));
            }
            
            var fn = getter.Compile();
            var value = fn(helper.ViewData.Model);
            if (value != null)
            {
                return new MvcHtmlString(value.ToString());
            }
            return  new MvcHtmlString(null);
        }

        public static MvcHtmlString Value<TPageModel, TModel, TProperty>(this MustacheHelper<TPageModel> helper,
                                                                         TModel model,
                                                                         Expression<Func<TModel, TProperty>> getter)
        {
            if (helper.ViewData.Model == null)
            {
                string name = ExpressionHelper.GetExpressionText(getter);
                return new MvcHtmlString(string.Format("{{{{{0}}}}}", name));
            }
            else
            {
                var fn = getter.Compile();
                var value = fn(model);
                if (value != null)
                {
                    return new MvcHtmlString(value.ToString());
                }
                return new MvcHtmlString(null);
            }
        }

        public static IEnumerable<TProperty> Loop<TModel, TProperty>(this MustacheHelper<TModel> helper,
                                                                     Expression<Func<TModel, IEnumerable<TProperty>>> getter)
        {
            string name = null;
            Func<IEnumerable<TProperty>> getEnumerable;

            var fn = getter.Compile();
            if (helper.ViewData.Model == null)
            {
                name = ExpressionHelper.GetExpressionText(getter);
                getEnumerable = () => new List<TProperty> { default(TProperty) };
            } else
            {
                // this is an example of partial application of a function
                getEnumerable = () => fn(helper.ViewData.Model);
            }

            return new DisposableEnumerable<TProperty>(helper.ViewContext, name, getEnumerable);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this MustacheHelper<TModel> helper,
                                                                  Expression<Func<TModel, TProperty>> getter,
                                                                  object htlmAttributes)
        {
            if (helper.ViewData.Model == null)
            {
                string name = ExpressionHelper.GetExpressionText(getter);
                return helper.HtmlHelper.TextBox(name, string.Format("{{{{{0}}}}}", name), htlmAttributes);
            }
            else
            {
                return helper.HtmlHelper.TextBoxFor(getter, htlmAttributes);
            }
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this MustacheHelper<TModel> helper,
                                                                       Expression<Func<TModel, TProperty>> getter,
                                                                       SelectList list, string optionLabel = null,
                                                                       object htlmAttributes = null)
        {
            if (helper.ViewData.Model == null)
            {
                string name = ExpressionHelper.GetExpressionText(getter);
                var mlist =
                    new SelectList(
                        new List<SelectListItem> {new SelectListItem {Value = "{{value}}", Text = "{{text}}"}}, "Value",
                        "Text");
                string html = helper.HtmlHelper.DropDownList(name, mlist, htlmAttributes)
                    .ToHtmlString()
                    .Replace("<option", "{{#" + name + "}}<option")
                    .Replace("</option>", "</option>{{/" + name + "}}")
                    .Replace(">{{text}}</option", " {{#selected}}selected{{/selected}}>{{text}}</option");

                return MvcHtmlString.Create(html);
            }
            else
            {
                return helper.HtmlHelper.DropDownListFor(getter, list, optionLabel, htlmAttributes);
            }
        }
    }
}