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

        public static MvcHtmlString ViewBag<TModel>(this MustacheHelper<TModel> helper, string key)
        {
            var s = (helper.IsRenderingMustache())
                        ? string.Format("{{{{__view__.{0}}}}}", key)
                        : helper.ViewData[key].ToString();

            return new MvcHtmlString(s);
        }

        public static MvcHtmlString Value<TModel, TProperty>(
            this MustacheHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> getter)
        {
            return Value(helper, helper.ViewData.Model, getter);
        }

        public static MvcHtmlString Value<TPageModel, TModel, TProperty>(
            this MustacheHelper<TPageModel> helper,
            TModel model,
            Expression<Func<TModel, TProperty>> getter)
        {
            string s;

            if (helper.IsRenderingMustache())
            {
                string name = ExpressionHelper.GetExpressionText(getter);
                s = string.Format("{{{{{0}}}}}", name);
            } else
            {
                var fn = getter.Compile();
                var value = fn(model);
                s = value != null ? value.ToString() : string.Empty;
            }
            return new MvcHtmlString(s);
        }

        public static string RouteValue<TModel>(this MustacheHelper<TModel> helper, string key)
        {
            if (helper.IsRenderingMustache())
            {
                return string.Format("{{{{__route__.{0}}}}}", key);
            }

            var value = helper.HtmlHelper.ViewContext.RouteData.Values[key];

            return (value == null) ? string.Empty : value.ToString();
        }

        public static IEnumerable<TProperty> Loop<TModel, TProperty>(
            this MustacheHelper<TModel> helper,
            Expression<Func<TModel,
            IEnumerable<TProperty>>> getter)
        {
            return Loop(helper, helper.ViewData.Model, getter);
        }

        public static IEnumerable<TProperty> Loop<TPageModel, TModel, TProperty>(
            this MustacheHelper<TPageModel> helper,
            TModel model,
            Expression<Func<TModel,
            IEnumerable<TProperty>>> getter)
        {
            string name = null;
            Func<IEnumerable<TProperty>> getEnumerable;

            var fn = getter.Compile();
            if (helper.IsRenderingMustache())
            {
                name = ExpressionHelper.GetExpressionText(getter);
                if (string.IsNullOrEmpty(name)) name = "model";
                getEnumerable = () => new List<TProperty> { default(TProperty) };
            }
            else
            {
                // this is an example of partial application of a function
                getEnumerable = () => fn(model);
            }

            return new DisposableEnumerable<TProperty>(helper.ViewContext, name, getEnumerable);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this MustacheHelper<TModel> helper,
                                                                  Expression<Func<TModel, TProperty>> getter,
                                                                  object htlmAttributes)
        {
            if (helper.IsRenderingMustache())
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
                                                                       IEnumerable<SelectListItem> list, string optionLabel = null,
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
    
        private static bool IsRenderingMustache<TModel>(this MustacheHelper<TModel> helper)
        {
            return (helper.ViewData.Model == null);
        }
    }
}