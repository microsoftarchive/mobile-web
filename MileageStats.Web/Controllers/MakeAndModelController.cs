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

using System.Web.Mvc;
using MileageStats.Domain.Handlers;
using Microsoft.Practices.ServiceLocation;

namespace MileageStats.Web.Controllers
{
    public class MakeAndModelController : BaseController
    {
        public MakeAndModelController(
            GetUserByClaimId getUser,
            IServiceLocator serviceLocator)
            : base(getUser, serviceLocator)
        {
        }

        [HttpPost]
        [Authorize]
        public JsonResult MakesForYear(int year)
        {
            var result = Using<GetYearsMakesAndModels>().Execute(filteredToYear: year);
            return Json(result.Item2);
        }

        [HttpPost]
        [Authorize]
        public JsonResult ModelsForMake(int year, string make)
        {
            var result = Using<GetYearsMakesAndModels>().Execute(filteredToYear: year,
                filteredByMake: make);

            return Json(result.Item3);
        }
    }
}
