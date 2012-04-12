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
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;

namespace MileageStats.Web.Controllers
{
    /// <summary>
    /// Provides base for controllers that need authorization and user information.
    /// </summary>
    /// <remarks>
    /// This base controller largely provides common methods to recover authorized user information.
    /// </remarks>
    public abstract class BaseController : Controller
    {
        protected readonly GetUserByClaimId _getUserByClaimId;
        private readonly IServiceLocator _serviceLocator;

        public BaseController(GetUserByClaimId getUserByClaimId, IServiceLocator serviceLocator)
        {
            if (getUserByClaimId == null) throw new ArgumentNullException("getUserByClaimId");
            
            _getUserByClaimId = getUserByClaimId;
            _serviceLocator = serviceLocator;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var vehicleId = filterContext.RouteData.Values["vehicleId"];
            if (vehicleId != null && string.IsNullOrEmpty(ViewBag.VehicleName))
            {
                ViewBag.VehicleName = GetVehicleName(int.Parse(vehicleId.ToString()));
            }

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Retrieves the CurrentUserId as stored in the <see cref="MileageStatsIdentity"/>.
        /// </summary>
        /// <remarks>
        /// Using this method requires the user to be authorized.
        /// </remarks>
        protected int CurrentUserId
        {
            get { return MileageStatsIdentity(User).UserId; }
        }

        private User _currentUser;

        /// <summary>
        /// Returns the current user or recovers the user from the <see cref="MileageStatsIdentity"/>.
        /// </summary>
        /// <remarks>
        /// Using this method requires the user to be authorized.
        /// </remarks>
        public User CurrentUser
        {
            get
            {
                return _currentUser ??
                       (_currentUser = GetUserFromIdentity(_getUserByClaimId, MileageStatsIdentity(User)));
            }
        }

        protected T Using<T>() where T: class
        {
            var handler = _serviceLocator.GetInstance<T>();
            if (handler == null)
            {
                throw new NullReferenceException("Unable to resolve type with service locator; type " + typeof (T).Name);
            }
            return handler;
        }

        private string GetVehicleName(int vehicleId)
        {
            try
            {
                var vehicle = Using<GetVehicleListForUser>()
                    .Execute(CurrentUserId).First(v => v.VehicleId == vehicleId);

                return vehicle == null ? string.Empty : vehicle.Name;
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }

        }

        private static MileageStatsIdentity MileageStatsIdentity(IPrincipal principal)
        {
            return (MileageStatsIdentity)principal.Identity;
        }

        private static User GetUserFromIdentity(GetUserByClaimId getUser, MileageStatsIdentity identity)
        {
            var user = getUser.Execute(identity.Name);
            return user;
        }
    }
}