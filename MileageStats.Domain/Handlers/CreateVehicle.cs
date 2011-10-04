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
using System.Web;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class CreateVehicle
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehiclePhotoRepository _photoRepository;

        public CreateVehicle(IVehicleRepository vehicleRepository, IVehiclePhotoRepository photoRepository)
        {
            _vehicleRepository = vehicleRepository;
            _photoRepository = photoRepository;
        }

        public virtual int Execute(int userId, ICreateVehicleCommand vehicleForm, HttpPostedFileBase photoFile)
        {
            if (vehicleForm == null) throw new ArgumentNullException("vehicleForm");

            try
            {
                var vehicle = vehicleForm.ConvertToEntity(userId);
                _vehicleRepository.Create(userId, vehicle);

                if (photoFile != null)
                {
                    // the double reference between vehicle and photo is a potential source of pain
                    var photo = photoFile.ConvertToEntity();
                    _photoRepository.Create(vehicle.VehicleId, photo);
                    vehicle.PhotoId = photo.VehiclePhotoId;

                    _vehicleRepository.Update(vehicle);
                }

                return vehicle.Id;
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessServicesException(Resources.UnableToCreateVehicleExceptionMessage, ex);
            }
        }
    }
}