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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Models;


namespace MileageStats.Domain.Handlers
{
    public static class ViewModelExtensions
    {
        public static VehiclePhoto ConvertToEntity(this HttpPostedFileBase photoFile)
        {
            var stream = photoFile.InputStream;
            var contentType = photoFile.ContentType;

            byte[] buffer = null;

            // load the stream as an Image to verify it is valid.
            var image = Image.FromStream(stream);

            // restream the image to prevent any dependency on photoFile.ContentLength.
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, new ImageFormat(image.RawFormat.Guid));
                buffer = memoryStream.ToArray();
            }
            return new VehiclePhoto { ImageMimeType = contentType, Image = buffer };
        }

        public static Vehicle ConvertToEntity(this ICreateVehicleCommand vehicleForm, int userId, bool includeVehicleId = false)
        {
            if (vehicleForm == null)
            {
                return null;
            }

            var vehicle = new Vehicle
                              {
                                  MakeName = vehicleForm.MakeName,
                                  ModelName = vehicleForm.ModelName,
                                  Name = vehicleForm.Name,
                                  SortOrder = vehicleForm.SortOrder,
                                  Year = vehicleForm.Year,
                                  UserId = userId
                              };

            if (includeVehicleId) vehicle.VehicleId = vehicleForm.VehicleId;

            return vehicle;
        }
    }
}