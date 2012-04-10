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
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace MileageStats.Web
{

    /// <summary>
    /// This class allows ASP.NET MVC to use to Unity for resolving dependencies. 
    /// For more information see http://msdn.com/library/system.web.mvc.idependencyresolver
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _unity;

        public UnityDependencyResolver(IUnityContainer unity)
        {
            _unity = unity;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _unity.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                // By definition of IDependencyResolver contract,
                // this should return null if it cannot be found.
                // http://msdn.com/library/system.web.mvc.idependencyresolver.getservice
                Debug.WriteLine(string.Format("Unable to resolve request for {0}, returning null", serviceType.Name));
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _unity.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                // By definition of IDependencyResolver contract,
                // this should return an empty collection if it cannot be found.
                // http://msdn.com/library/system.web.mvc.idependencyresolver.getservices
                Debug.WriteLine(string.Format("Unable to resolve request for a collection of {0}, returning an empty collection", serviceType.Name));
                return new object[0];
            }
        }
    }
}