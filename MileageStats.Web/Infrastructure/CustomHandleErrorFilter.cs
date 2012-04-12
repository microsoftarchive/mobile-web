using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace MileageStats.Web.Infrastructure
{
    /// <summary>
    /// The default error filter in MVC always returns a server internal error (505) for any possible error 
    /// that ocurred on the server. However, there are certain errors that should be treated differently 
    /// such as a bad request when the client sent some invalid data or a not found error when the requested resource did not
    /// exist.  
    /// This filter allows mapping exceptions to more meaningful error status codes. 
    /// </summary>
    public class CustomHandleErrorFilter : FilterAttribute, IExceptionFilter
    {
        public CustomHandleErrorFilter(IDictionary<Type, HttpStatusCode> mappings)
        {
            this.Mappings = mappings;
            this.View = "_Error";
        }

        /// <summary>
        /// The mapping between an exception type and a status code.
        /// </summary>
        public IDictionary<Type, HttpStatusCode> Mappings
        {
            get;
            private set;
        }

        /// <summary>
        /// The view that needs to be rendered when an exception occurrs.
        /// </summary>
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
                // The error view is only set for http requests originated by the browser, and not Ajax calls.
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
                // HttpExceptions raised in the code are used directly with no mapping.
                statusCode = ((HttpException)filterContext.Exception).GetHttpCode();
            }
            else if (this.Mappings.ContainsKey(filterContext.Exception.GetType()))
            {
                //Regular exceptions are mapped to a status code.
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