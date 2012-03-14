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
using System.Linq;
using System.Web.Mvc;

namespace MileageStats.Web
{
    public sealed class ContentTypeAwareResult : ActionResult, ITestableContentTypeAwareResult
    {
        private readonly dynamic _model;
        private Dictionary<string, Func<object,ViewDataDictionary, TempDataDictionary, ActionResult>> _supportedTypes;

        public Func<object, ViewDataDictionary, TempDataDictionary, ActionResult> WhenJson { get; set; }
        public Func<object, ViewDataDictionary, TempDataDictionary, ActionResult> WhenHtml { get; set; }

        public ContentTypeAwareResult()
            : this(null)
        {
            // convenience constructor for when there is no model
        }

        public ContentTypeAwareResult(object model)
        {
            _model = model;

            SetupDefaultActionResultProviders(model);
        }

        public dynamic Model { get { return _model; } }

        private static IDictionary<string,object> WrapArrayWithObject(dynamic model, TempDataDictionary tempData, ViewDataDictionary viewDataDictionary)
        {
            var output = new Dictionary<string, object>();

            // We want to avoid sending a JSON array as a response
            // See http://haacked.com/archive/2009/06/25/json-hijacking.aspx
            // In cases were our model would be converted to an array,
            // we wrap it in a object
            object wrappedModel = (typeof(IEnumerable)).IsInstanceOfType(model) ? new { model } : model;

            output["Model"] = wrappedModel;

            if (tempData.ContainsKey("alert"))
            {
                output["FlashAlert"] = tempData["alert"].ToString();
            }

            if (tempData.ContainsKey("confirm"))
            {
                output["FlashConfirm"] = tempData["confirm"].ToString();
            }
            
            if (viewDataDictionary.Any())
            {
                output["__view__"] = viewDataDictionary
                    .ToDictionary(viewDataKeyValuePair => viewDataKeyValuePair.Key,
                                  viewDataKeyValuePair => viewDataKeyValuePair.Value);
            }

            if (viewDataDictionary.ModelState.Any())
            {
                var errors = viewDataDictionary.ModelState
                    .Where(p => p.Value.Errors.Any());

                if(errors.Any())
                output["Errors"] = errors
                    .ToDictionary(p => p.Key, p => p.Value.Errors.Select(x => x.ErrorMessage).ToArray());
            }

            return output;
        }

        private void SetupDefaultActionResultProviders(object model)
        {
            // these should behave the same as calling the corresponding 
            // convenience method on Controller

            WhenJson = (x, v, t) => new JsonResult
                                     {
                                         Data = WrapArrayWithObject(model, t, v),
                                         JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                     };

            WhenHtml = (x,v,t) =>
                       {
                           var result = new ViewResult();
                           result.ViewData = v;
                           result.TempData = t;
                           if (model != null)
                           {
                               result.ViewData.Model = model;
                           }
                           return result;
                       };
        }

        private ActionResult GetActionResultFor(ControllerContext context)
        {
            // map supported content-types to a provider
            _supportedTypes = new Dictionary<string, Func<object, ViewDataDictionary, TempDataDictionary, ActionResult>>
                                  {
                                      {"application/json", WhenJson},
                                      {"text/html", WhenHtml}
                                  };

            // mime types can follow a form like:
            //  text/html; q=0.90
            // in those cases we want to discard the portion
            // after the semicolon when attempting to match
            var types = (from type in context.HttpContext.Request.AcceptTypes
                        select type.Split(';')[0])
                        .ToList();

            if (types.Count == 0)
            {
                // The format passed in the query string is used
                // when no accept header is found in the request

                var format = context.HttpContext.Request.QueryString["format"];
                var contentType = GetContentTypeForFormat(format);

                if (!string.IsNullOrEmpty(contentType))
                {
                    types.Add(contentType);
                }
            }

            var providers = from type in types
                            where _supportedTypes.ContainsKey(type)
                            select _supportedTypes[type];

            if (providers.Any())
            {
                //note: if more than one support type is found, what should we do?
                var getResult = providers.First();
                return getResult(_model, context.Controller.ViewData, context.Controller.TempData);
            }
            else
            {
                var msg = string.Format("An unsupported media type was requested. The supported content types are : {0}", String.Join(",", types));
                return new HttpStatusCodeResult(406, msg);
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            GetActionResultFor(context).ExecuteResult(context);
        }

        private static string GetContentTypeForFormat(string format)
        {
            if (format != null && format.Equals("json", StringComparison.CurrentCultureIgnoreCase))
            {
                return "application/json";
            }

            return null;
        }


        ActionResult ITestableContentTypeAwareResult.GetActionResultFor(ControllerContext context)
        {
            return GetActionResultFor(context);
        }
    }

    // this interface is used to hide a member of ContentTypeAwareResult
    // that is only meant to be used in unit tests.
    // we named the interface with the prefix "Testable" in order to make
    // the purpose more explicit
    public interface ITestableContentTypeAwareResult
    {
        ActionResult GetActionResultFor(ControllerContext context);
    }
}