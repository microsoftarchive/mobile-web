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

using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using MileageStats.Data;
using MileageStats.Data.InMemory;
using MileageStats.Domain;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Web.Authentication;
using MileageStats.Web.UnityExtensions;
using MileageStats.Web.MobileProfiler.ClientProfile;

namespace MileageStats.Web
{
    public class UnityContainerFactory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification = "Container has the scope of the application.")
        ]
        public IUnityContainer CreateConfiguredContainer()
        {
            var container = new UnityContainer();

            // We are choosing to perform most of the Unity configuration here,
            // rather than in the config file. The justification for this choice
            // is that:
            // * these registration rarely change
            // * the is no anticipated need to swap items at run-time
            // * performing the registrations in code allows the compiler to catch typos
            
            // register services
            RegisterPerRequest<IChartDataService, ChartDataService>(container);

            // register repositories
            RegisterPerRequest<IUserRepository, UserRepository>(container);
            RegisterPerRequest<ICountryRepository, CountryRepository>(container);
            RegisterPerRequest<IVehicleRepository, VehicleRepository>(container);
            RegisterPerRequest<IFillupRepository, FillupRepository>(container);
            RegisterPerRequest<IReminderRepository, ReminderRepository>(container);
            RegisterPerRequest<IVehiclePhotoRepository, VehiclePhotoRepository>(container);
            RegisterPerRequest<IVehicleManufacturerRepository, VehicleManufacturerRepository>(container);
            
            // register authorization components
            RegisterPerRequest<IFormsAuthentication, DefaultFormsAuthentication>(container);

            // register device profile components
            RegisterPerRequest<IProfileManifestRepository, XmlProfileManifestRepository>(container);
            RegisterPerRequest<IProfileCookieEncoder, ProfileCookieEncoder>(container);
            
            // register map service
            RegisterPerRequest<IMapService, MockMapService>(container);
            //RegisterPerRequest<IMapService, BingMapService>(container);

#if DEBUG
            RegisterPerRequest<IOpenIdRelyingParty, LocalFakeRelyingParty>(container);
#else
            RegisterPerRequest<IOpenIdRelyingParty, DefaultOpenIdRelyingParty>(container);
#endif
            return container;
        }

        // convenience method, makes the list of registrations easier to read
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification = "Container has the scope of the application.")
        ]
        private void RegisterPerRequest<TFrom, TTo>(IUnityContainer container) where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>(new UnityHttpContextPerRequestLifetimeManager());
        }
    }
}