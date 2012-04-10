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
using System.Web.Mvc;
using MileageStats.Web.ClientProfile;
using MileageStats.Web.ClientProfile.Model;
using System.Web.Caching;

namespace MileageStats.Web.Controllers
{
    public class MobileProfileController : Controller
    {
        private static object ProfileLock = new object(); 

        IProfileManifestRepository _profileRepository;
        IProfileCookieEncoder _encoder;

        public MobileProfileController(IProfileManifestRepository profileRepository, IProfileCookieEncoder encoder)
        {
            _profileRepository = profileRepository;
            _encoder = encoder;
        }
        
        public ActionResult ProfileScript()
        {
            var model = GetProfile();

            var profileCookie = Request.Cookies["profile"];

            if (profileCookie != null)
            {
                var clientProfile = _encoder.GetDeviceCapabilities(profileCookie);

                if (clientProfile.ContainsKey("id"))
                {
                    var parts = clientProfile["id"].Split('-');
                    if (parts.Length == 1 || parts[1] != model.Version)
                    {
                        // The cookie version does not match, so it needs 
                        // to be refreshed.
                        Request.Cookies.Remove("profile");
                    }
                }
                else
                {
                    // The cookie does not contain an ID, so it needs to be
                    // refreshed.
                    Request.Cookies.Remove("profile");
                }
            }

            Response.ContentType = "text/javascript";
            
            return PartialView("Profile.js", model);
        }

        /// <summary>
        /// Tries to get the profile from the cache before parsing the file on disk.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private ProfileManifest GetProfile()
        {
            var model = (this.HttpContext.Cache != null) ? this.HttpContext.Cache["profile"] as ProfileManifest : null;
            if (model == null)
            {
                lock (ProfileLock)
                {
                    model = (this.HttpContext.Cache != null) ? HttpContext.Cache["profile"] as ProfileManifest : null;
                    if (model == null)
                    {
                        model = this._profileRepository.GetProfile("generic");
                        if (HttpContext.Cache != null)
                        {
                            HttpContext.Cache.Add("profile",
                                model,
                                new CacheDependency(this._profileRepository.GetManifestPath("generic")),
                                Cache.NoAbsoluteExpiration,
                                TimeSpan.FromHours(1),
                                CacheItemPriority.Normal,
                                null);
                        }
                    }
                }
            }

            return model;
        }
    }
}
