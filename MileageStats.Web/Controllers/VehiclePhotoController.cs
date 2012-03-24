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

using System.Web.Mvc;
using MileageStats.Domain.Handlers;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MileageStats.Web.Controllers
{
    public class VehiclePhotoController : BaseController
    {
        public VehiclePhotoController(
            GetUserByClaimId getUser,
            IServiceLocator serviceLocator)
            : base(getUser, serviceLocator)
        {
        }

        //
        // GET: /Vehicle/Image/5
        [SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification = "FileStreamResult disposes the stream.")]
        public ActionResult GetVehiclePhoto(int vehiclePhotoId)
        {
            var photo = Using<GetVehiclePhoto>().Execute(vehiclePhotoId);
            if(photo != null)
                return new FileStreamResult(new MemoryStream(photo.Image), photo.ImageMimeType);
            
            //use the default image
            return new FilePathResult(Url.Content("~/Content/vehicle.png"), "image/png");
        }

    }
}
