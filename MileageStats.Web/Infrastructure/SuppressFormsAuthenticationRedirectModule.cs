using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MileageStats.Web.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SuppressFormsAuthenticationRedirectModule), "Start")]

namespace MileageStats.Web.Infrastructure
{
    /// <summary>
    /// This module suppress the http redirects made by forms authentication for any ajax call. 
    /// It becomes handy in scenarios where several ajax calls were made after the user session expired, and
    /// a redirection to the login page may generate unexpected results on the client side.
    /// </summary>
    public class SuppressFormsAuthenticationRedirectModule : IHttpModule
    {
        private static readonly object SuppressAuthenticationKey = new Object();

        public static void SuppressAuthenticationRedirect(HttpContext context)
        {
            context.Items[SuppressAuthenticationKey] = true;
        }

        public static void SuppressAuthenticationRedirect(HttpContextBase context)
        {
            context.Items[SuppressAuthenticationKey] = true;
        }

        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += OnPostReleaseRequestState;
            context.EndRequest += OnEndRequest;
        }

        private void OnPostReleaseRequestState(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var response = context.Response;
            var request = context.Request;

            if (response.StatusCode == 401 && request.Headers["X-Requested-With"] ==
              "XMLHttpRequest")
            {
                SuppressAuthenticationRedirect(context.Context);
            }
        }

        private void OnEndRequest(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var response = context.Response;

            if (context.Context.Items.Contains(SuppressAuthenticationKey))
            {
                response.TrySkipIisCustomErrors = true;
                response.ClearContent();
                response.StatusCode = 401;
                response.RedirectLocation = null;
            }
        }

        public void Dispose()
        {
        }

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(
              typeof(SuppressFormsAuthenticationRedirectModule));
        }
    }
}