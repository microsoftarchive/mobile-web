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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MileageStats.Web.Helpers
{
    public static class MustacheTemplateExtensions
    {
        private const string UseMustacheFlag = "__use_mustache";

        public static void RenderAsMustacheTemplate(this HtmlHelper helper, string view)
        {
            helper.RenderPartial(view, new ViewDataDictionary { { UseMustacheFlag, "" } });
        }

        public static bool IsMustache<TModel>(this HtmlHelper<TModel> helper)
        {
            return helper.ViewData.ContainsKey(UseMustacheFlag);
        }

        public static MvcHtmlString Mustache<TModel,TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel,TProperty>> getter)
        {
            if(helper.ViewData.Model == null)
            {
                var name = ExpressionHelper.GetExpressionText(getter);
                return new MvcHtmlString(string.Format("{{{{{0}}}}}",name));

            }

            var fn = getter.Compile();
            var value = fn(helper.ViewData.Model);
            if (value != null)
            {
                return new MvcHtmlString(value.ToString());
            }
            return new MvcHtmlString(null);
        }

        public static MvcHtmlString Mustache<TModel, TProperty>(this HtmlHelper helper, TModel model, Expression<Func<TModel, TProperty>> getter)
        {
            if (helper.ViewData.Model == null)
            {
                var name = ExpressionHelper.GetExpressionText(getter);
                return new MvcHtmlString(string.Format("{{{{{0}}}}}", name));

            }
            
            var fn = getter.Compile();
            var value = fn(model);
            if (value != null)
            {
                return new MvcHtmlString(value.ToString());
            }
            return new MvcHtmlString(null);
        }

        public interface ISectionRenderer<out T> : IDisposable
        {
            IEnumerable<T> Items { get; }
        }

        private class MustacheSectionRenderer<T> : ISectionRenderer<T>
        {
            private bool _disposed;
            private readonly string _sectionName;
            private readonly TextWriter _writer;
            private readonly Func<IEnumerable<T>> _getItems;

            public MustacheSectionRenderer(ViewContext viewContext, string sectionName, Func<IEnumerable<T>> getItems = null)
            {
                if (viewContext == null)
                {
                    throw new ArgumentNullException("viewContext");
                }

                _sectionName = sectionName;
                _getItems = getItems;
                _writer = viewContext.Writer;
            }

            public IEnumerable<T> Items
            {
                get { return _getItems(); }
            } 

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    var endSection = string.Format("{{{{/{0}}}}}", _sectionName);
                    _writer.Write(endSection);
                }
            }

        }

        private class NoOpDisposable<T> : ISectionRenderer<T>
        {
            public void Dispose()
            {
            }

            public IEnumerable<T> Items
            {
                get { return new List<T> { default(T) }; }
            }
        }

        public static ISectionRenderer<TProperty> MustacheSection<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> getter)
        {
            if (helper.ViewData.Model == null)
            {
                var name = ExpressionHelper.GetExpressionText(getter);
                var beginSection = string.Format("{{{{#{0}}}}}", name);
                helper.ViewContext.Writer.WriteLine(beginSection);

                //todo: check if TProperty is IEnumerable and construct the appropriate Func to pass into the renderer

                return new MustacheSectionRenderer<TProperty>(helper.ViewContext, name);
            }
            else
            {
                return new NoOpDisposable<TProperty>();
            }

        }

        public static IEnumerable<TProperty> MustacheLoop<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, IEnumerable<TProperty>>> getter)
        {
            if (helper.ViewData.Model == null)
            {
                return new List<TProperty>{default(TProperty)};

            }
            else
            {
                var fn = getter.Compile();
                var value = fn(helper.ViewData.Model);
                return value;
            }
        }

        public static MvcHtmlString MTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> getter, object htlmAttributes)
        {
            if (helper.ViewData.Model == null)
            {
                var name = ExpressionHelper.GetExpressionText(getter);
                return helper.TextBox(name, string.Format("{{{{{0}}}}}", name), htlmAttributes);
            }
            else
            {
                return helper.TextBoxFor(getter, htlmAttributes);
            }
        }

        public static MvcHtmlString MDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> getter, SelectList list, string optionLabel = null, object htlmAttributes = null)
        {
            if (helper.ViewData.Model == null)
            {
                var name = ExpressionHelper.GetExpressionText(getter);
                var mlist = new SelectList(new List<SelectListItem> { new SelectListItem { Value = "{{value}}", Text = "{{text}}" } }, "Value", "Text");
                var html = helper.DropDownList(name, mlist, htlmAttributes)
                    .ToHtmlString()
                    .Replace("<option", "{{#" + name + "}}<option")
                    .Replace("</option>", "</option>{{/" + name + "}}")
                    .Replace(">{{text}}</option", " {{#selected}}selected{{/selected}}>{{text}}</option");

                return MvcHtmlString.Create(html);
            }
            else
            {
                return helper.DropDownListFor(getter, list, optionLabel, htlmAttributes);
            }

        }
    }
}