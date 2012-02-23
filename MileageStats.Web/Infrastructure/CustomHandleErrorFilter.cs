using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace MileageStats.Web.Infrastructure
{
    public class CustomHandleErrorFilter : FilterAttribute, IExceptionFilter
    {
        public CustomHandleErrorFilter(IDictionary<Type, HttpStatusCode> mappings)
        {
            this.Mappings = mappings;
            this.View = "_Error";
        }

        public IDictionary<Type, HttpStatusCode> Mappings
        {
            get;
            private set;
        }

        public string View
        {
            get;
            set;
        }

        public virtual void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = this.View,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            int statusCode = 500;
            if (filterContext.Exception is HttpException)
            {
                statusCode = ((HttpException)filterContext.Exception).GetHttpCode();
            }
            else if (this.Mappings.ContainsKey(filterContext.Exception.GetType()))
            {
                statusCode = (int)this.Mappings[filterContext.Exception.GetType()];
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.StatusDescription = filterContext.Exception.Message;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}