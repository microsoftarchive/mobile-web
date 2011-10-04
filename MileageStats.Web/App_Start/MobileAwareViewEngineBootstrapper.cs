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
using System.IO;
using System.Web;
using System.Web.Mvc;
using MileageStats.Web.App_Start;

[assembly: WebActivator.PreApplicationStartMethod(typeof (MobileAwareViewEngineBootstrapper), "Start")]

namespace MileageStats.Web.App_Start
{
    public static class MobileAwareViewEngineBootstrapper
    {
        public static void Start()
        {
            ViewEngines.Engines.Insert(0, new MobileCapableRazorViewEngine());
        }
    }
    
    // this sample is derived from by Scott Hanselman and Peter Mournfield
    // for background, see this post:
    // http://www.hanselman.com/blog/NuGetPackageOfTheWeek10NewMobileViewEnginesForASPNETMVC3SpeccompatibleWithASPNETMVC4.aspx

    // the view engine defined here is inserted at the head of the chain of
    // view engines like so:
    //
    // ViewEngines.Engines.Insert(0, new MobileCapableRazorViewEngine());
    //
    // in this sample, the view engine is inserted using WebActivator, using
    // the bootstrapper in the App_Start folder. More info on WebActivator here:
    // https://bitbucket.org/davidebbo/webactivator/wiki/Home
    // alternatively, it could also be inserted in the Global.asax.cs in Application_Start

    public class MobileCapableRazorViewEngine : RazorViewEngine
    {
        // this identifier is added to the name of the view that the engine looks
        // for, if the provided condition is met
        private readonly string _viewModifier;

        // a function that accepts the http context and
        // returns true if this view engine should be used
        private readonly Func<HttpContextBase, bool> _contextCondition;

        // by default, the condition for using this view engine
        // is that ASP.NET believes that the request is from
        // a mobile browser
        public MobileCapableRazorViewEngine()
            : this("Mobile", context => context.Request.Browser.IsMobileDevice)
        {
        }

        #region additional constructors

        // the following constructors allow this view engine to customized further.
        // the constructors are not used in this sample, but included to demonstrate one
        // way to provide extensibility.
        //
        // for example, to include an additional set of views specific to iPhone
        // you could setup another instance of the view engine using:
        //
        // ViewEngines.Engines.Insert(0,
        //      new MobileCapableRazorViewEngine(
        //          "iPhone", 
        //          ctx => ctx.Request.UserAgent.IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) >= 0
        //      )
        // );

        public MobileCapableRazorViewEngine(string viewModifier)
            : this(viewModifier, context => context.Request.Browser.IsMobileDevice)
        {
        }

        public MobileCapableRazorViewEngine(string viewModifier, Func<HttpContextBase, bool> contextCondition)
        {
            _viewModifier = viewModifier;
            _contextCondition = contextCondition;
        }
        #endregion

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName,
                                                    string masterName, bool useCache)
        {
            return NewFindView(controllerContext, viewName, null, useCache, false);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return NewFindView(controllerContext, partialViewName, null, useCache, true);
        }

        // this logic is used by both FindView and FindPartial, it uses _viewModifier to attempt to find a match 
        private ViewEngineResult NewFindView(ControllerContext controllerContext, string viewName, string masterName,
                                                bool useCache, bool isPartialView)
        {
            if (!_contextCondition(controllerContext.HttpContext))
            {
                // this result contains a list of places we attempted to look for a view
                // since the condition to use the engine wasn't met, we didn't look anywhere
                // hence the empty array
                return new ViewEngineResult(new string[] { });
            }

            // we look for the cookie that is set in the ViewSwitcher controller
            var viewSwitcher = controllerContext.RequestContext.HttpContext.Request.Cookies["ViewSwitcher"];

            // if the mobile flag has been explicitly turned off, then
            // we'll pass to the next view engine in the chain
            // also note that we'll never check for this flag unless 
            // the initial condition for using the engine has been met
            if (viewSwitcher != null && viewSwitcher["Mobile"] != null && String.Equals(viewSwitcher["Mobile"], "false"))
            {
                // again, since conditions were not met, we return an empty array for the 'attempts'
                return new ViewEngineResult(new string[] { });
            }

            // all of the necessary conditions have been met, we can be constructing the name 
            // of the view we'd like to use
            return FindModifiedView(controllerContext, viewName, masterName, useCache, isPartialView);
        }

        private ViewEngineResult FindModifiedView(ControllerContext controllerContext, string viewName, string masterName,
                                                    bool useCache, bool isPartialView)
        {
            // Get the name of the controller from the path
            string controller = controllerContext.RouteData.Values["controller"].ToString();
            string area = "";

            if (controllerContext.RouteData.DataTokens.ContainsKey("area"))
                area = controllerContext.RouteData.DataTokens["area"].ToString();

            // Apply the view modifier
            var newViewName = string.Format("{0}.{1}", viewName, _viewModifier);

            // Create the key for caching purposes          
            string keyPath = Path.Combine(area, controller, newViewName);

            string cacheLocation = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, keyPath);

            // Try the cache          
            if (useCache)
            {
                //If using the cache, check to see if the location is cached.                              
                if (!string.IsNullOrWhiteSpace(cacheLocation))
                {
                    if (isPartialView)
                    {
                        return new ViewEngineResult(CreatePartialView(controllerContext, cacheLocation), this);
                    }
                    else
                    {
                        return new ViewEngineResult(CreateView(controllerContext, cacheLocation, masterName), this);
                    }
                }
            }
            string[] locationFormats = string.IsNullOrEmpty(area) ? ViewLocationFormats : AreaViewLocationFormats;

            // for each of the paths defined, format the string and see if that path exists. When found, cache it.          
            foreach (string rootPath in locationFormats)
            {
                string currentPath = string.IsNullOrEmpty(area)
                                            ? string.Format(rootPath, newViewName, controller)
                                            : string.Format(rootPath, newViewName, controller, area);

                if (FileExists(controllerContext, currentPath))
                {
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, keyPath, currentPath);

                    if (isPartialView)
                    {
                        return new ViewEngineResult(CreatePartialView(controllerContext, currentPath), this);
                    }
                    else
                    {
                        return new ViewEngineResult(CreateView(controllerContext, currentPath, masterName), this);
                    }
                }
            }
            return new ViewEngineResult(new string[] { }); // we found nothing and we pretend we looked nowhere
        }
    }

}