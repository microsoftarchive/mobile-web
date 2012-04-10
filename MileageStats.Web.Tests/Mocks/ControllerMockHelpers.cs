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

using System.Security.Principal;
using System.Web.Mvc;
using Moq;

namespace MileageStats.Web.Tests.Mocks
{
    internal static class ControllerMockHelpers
    {
        /// <summary>
        /// Sets the controller User to a GenericPrincipal with the supplied identity
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="identity"></param>
        /// <remarks>
        /// Assumes that the Controller.HttpContext is a Mock/></remarks>
        public static void SetUserIdentity(this Controller controller, IIdentity identity)
        {
            Mock.Get(controller.HttpContext).Setup(x => x.User).Returns(new GenericPrincipal(identity, null));
        }
    }
}