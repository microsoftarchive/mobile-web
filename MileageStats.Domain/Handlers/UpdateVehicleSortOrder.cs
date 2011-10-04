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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class UpdateVehicleSortOrder
    {
        private readonly IVehicleRepository _vehicleRepository;

        public UpdateVehicleSortOrder(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public virtual void Execute(int userId, int[] vehicleIds)
        {
            int order = 0;
            try
            {
                foreach (var id in vehicleIds)
                {
                    var vehicle = _vehicleRepository.GetVehicle(userId, id);
                    vehicle.SortOrder = order;
                    order++;

                    _vehicleRepository.Update(vehicle);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessServicesException(Resources.UnableToUpdateVehicleSortOrderExceptionMessage, ex);
            }
        }
    }
}
