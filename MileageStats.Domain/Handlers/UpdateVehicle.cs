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
using System.Net;
using System.Web;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class UpdateVehicle
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehiclePhotoRepository _photoRepository;

        public UpdateVehicle(IVehicleRepository vehicleRepository, IVehiclePhotoRepository photoRepository)
        {
            _vehicleRepository = vehicleRepository;
            _photoRepository = photoRepository;
        }

        public virtual void Execute(int userId, ICreateVehicleCommand vehicleForm, HttpPostedFileBase photoFile)
        {
            try
            {
                var existing = _vehicleRepository.GetVehicle(userId, vehicleForm.VehicleId);
                int? photoId = null;

                if (existing != null)
                {
                    if (photoFile != null)
                    {
                        if (existing.PhotoId > 0)
                        {
                            _photoRepository.Delete(existing.PhotoId);
                        }

                        var dataPhoto = photoFile.ConvertToEntity();

                        // should we put this in its own try block?
                        _photoRepository.Create(vehicleForm.VehicleId, dataPhoto);
                        photoId = dataPhoto.VehiclePhotoId = dataPhoto.VehiclePhotoId;
                    }

                    var vehicle = vehicleForm.ConvertToEntity(userId, includeVehicleId: true);
                    vehicle.PhotoId = (photoId != null) ? photoId.Value : existing.PhotoId;
                    _vehicleRepository.Update(vehicle);
                }
                else
                {
                    throw new HttpException((int) HttpStatusCode.NotFound, Resources.CannotFindVehicleToUpdateExceptionMessage);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new UnauthorizedException(Resources.UnableToUpdateVehicleExceptionMessage, ex);
            }
        }
    }
}