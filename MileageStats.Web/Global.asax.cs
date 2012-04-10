/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using MileageStats.Web.Capabilities;
using MileageStats.Web.ClientProfile;
using MileageStats.Web.Models;
using MileageStats.Web.Authentication;
using MileageStats.Web.UnityExtensions;
using System.Linq;
using System.Collections.Generic;
using System.Web.Configuration;
using MileageStats.Domain.Handlers;
using MileageStats.Web.Infrastructure;
using System.Net;
using MileageStats.Domain.Contracts;


namespace MileageStats.Web
{
    public class MvcApplication : HttpApplication
    {
        private static IUnityContainer container;
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorFilter(new Dictionary<Type, HttpStatusCode>()
                {
                    { typeof(UnauthorizedException), HttpStatusCode.Forbidden }
                })
                {
                    View = "_Error"
                }); 
        }

        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.([iI][cC][oO]|[gG][iI][fF])(/.*)?" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                 "JsonMakesForYearRoute", // Route name
                 "Vehicle/MakesForYear", // URL with parameters
                 new { action = "MakesForYear", controller = "ModelAndMake" } // Parameter defaults
                 );

            routes.MapRoute(
                 "JsonModelsForMakeRoute", // Route name
                 "Vehicle/ModelsForMake", // URL with parameters
                 new { action = "ModelsForMake", controller = "ModelAndMake" } // Parameter defaults
                 );

            routes.MapRoute(
                "VehiclePhotoRoute", // Route name
                "Vehicle/Photo/{vehiclePhotoId}", // URL with parameters
                new { controller = "VehiclePhoto", action = "GetVehiclePhoto", vehiclePhotoId = UrlParameter.Optional } // Parameter defaults
                );

            routes.MapRoute(
                "VehicleFuelEfficiencyChartRoute",
                "Vehicle/FuelEfficiencyChart/{userId}/{vehicleId}",
                new { controller = "VehicleChart", action = "FuelEfficiencyChart" }
                );

            routes.MapRoute(
                "JsonGetVehicleStatisticSeriesRoute",
                "Vehicle/JsonGetVehicleStatisticSeries/{id}",
                new { controller = "VehicleChart", action = "JsonGetVehicleStatisticSeries", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                "VehicleTotalDistanceChartRoute",
                "Vehicle/TotalDistanceChart/{userId}/{vehicleId}",
                new { controller = "VehicleChart", action = "TotalDistanceChart" }
                );

            routes.MapRoute(
                "VehicleTotalCostChartRoute",
                "Vehicle/TotalCostChart/{userId}/{vehicleId}",
                new { controller = "VehicleChart", action = "TotalCostChart" }
                );

            routes.MapRoute(
                "ListRoute", // Route name
                "Vehicle/{vehicleId}/{controller}/List", // URL with parameters
                new { action = "List", vehicleId = UrlParameter.Optional } // Parameter defaults );
                );

            routes.MapRoute(
                "ListPartialRoute", // Route name
                "Vehicle/{vehicleId}/{controller}/ListPartial", // URL with parameters
                new { action = "ListPartial", vehicleId = UrlParameter.Optional } // Parameter defaults
                );

            routes.MapRoute(
                "JsonListRoute", // Route name
                "{controller}/JsonList/{vehicleId}", // URL with parameters
                new { action = "JsonList" } // Parameter defaults
                );

            routes.MapRoute(
                "FulfillRoute", // Route name
                "Vehicle/{vehicleId}/Reminder/{id}/Fulfill",
                new { action = "Fulfill", controller = "Reminder" }
                );
            
            routes.MapRoute(
                "AddRoute", // Route name
                "Vehicle/{vehicleId}/{controller}/Add",
                new {action = "Add"}
                );

            routes.MapRoute(
                "VehicleDetailsRoute",
                "Vehicle/{vehicleId}/Details",
                new { controller = "Vehicle", action = "Details" }
            );

            routes.MapRoute(
                "VehicleEditRoute",
                "Vehicle/{vehicleId}/Edit",
                new { controller = "Vehicle", action = "Edit" }
            );

            routes.MapRoute(
              "DetailsRoute",
              "Vehicle/{vehicleId}/{controller}/{id}/Details",
              new { action = "Details" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Dashboard", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        public override void Init()
        {
            PostAuthenticateRequest += PostAuthenticateRequestHandler;
            
            EndRequest += EndRequestHandler;

            base.Init();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializeDependencyInjectionContainer();

            // Sets a custom controller factory for overriding the default settings like the TempDataProvider.
            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory());

            // Injects the custom BrowserCapabilitiesProvider into the ASP.NET pipeline.
            HttpCapabilitiesBase.BrowserCapabilitiesProvider = container.Resolve<MobileCapabilitiesProvider>();

            // Injects the custom metadata provider.
            ModelMetadataProviders.Current = new CustomMetadataProvider();
        }

        private void EndRequestHandler(object sender, EventArgs e)
        {
            // This is a workaround since subscribing to HttpContext.Current.ApplicationInstance.EndRequest 
            // from HttpContext.Current.ApplicationInstance.BeginRequest does not work. 
            IEnumerable<UnityHttpContextPerRequestLifetimeManager> perRequestManagers = 
                container.Registrations
                    .Select(r => r.LifetimeManager)
                    .OfType<UnityHttpContextPerRequestLifetimeManager>()
                    .ToArray();

            foreach (var manager in perRequestManagers)
            {
                manager.Dispose();
            }
        }

        private void PostAuthenticateRequestHandler(object sender, EventArgs e)
        {
            var formsAuthentication = ServiceLocator.Current.GetInstance<IFormsAuthentication>();
            var ticket = formsAuthentication.GetAuthenticationTicket(new HttpContextWrapper(HttpContext.Current));

            if (ticket != null)
            {
                var mileageStatsIdentity = new MileageStatsIdentity(ticket);

                //Implemented workaround for the scenario where the user is not found in the repository
                // but the cookie exists.
                var getUser = ServiceLocator.Current.GetInstance<GetUserByClaimId>();
                if (getUser.Execute(mileageStatsIdentity.Name) == null)
                {
                    formsAuthentication.Signout();

                    if (Context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        Context.Response.End();
                    }
                    else
                    {
                        Context.Response.Redirect("~/Auth/Index", true);
                    }
                }
                else
                {
                    Context.User = new GenericPrincipal(mileageStatsIdentity, null);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "This should survive the lifetime of the application.")]
        private static void InitializeDependencyInjectionContainer()
        {
            container = new UnityContainerFactory().CreateConfiguredContainer();            
            var serviceLocator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            container.RegisterInstance<IProfileManifestRepository>(
                new XmlProfileManifestRepository("~/ClientProfile/",
                    (path) => HttpContext.Current.Server.MapPath(path)));
        }
    }
}