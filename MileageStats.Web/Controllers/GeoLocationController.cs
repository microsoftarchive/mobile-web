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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MileageStats.Domain.Contracts;

namespace MileageStats.Web.Controllers
{
    public class GeoLocationController : Controller
    {
        private readonly IMapService mapService;

        public GeoLocationController(IMapService mapService)
        {
            this.mapService = mapService;
        }

        // GET: /GeoLocation/

        public JsonResult GetFillupStations(double latitude, double longitude)
        {
            return Json(this.mapService.SearchKeywordLocation("Gas Stations", latitude, longitude), JsonRequestBehavior.AllowGet);
        }        
    }
}
