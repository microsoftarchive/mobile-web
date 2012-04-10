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
using System.Linq;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;


namespace MileageStats.Domain.Handlers
{
    public class GetVehicleById
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFillupRepository _fillupRepository;

        public GetVehicleById(IVehicleRepository vehicleRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _fillupRepository = fillupRepository;
        }

        public virtual VehicleModel Execute(int userId, int vehicleId)
        {
            Vehicle vehicle = _vehicleRepository.GetVehicle(userId, vehicleId);

            if (vehicle == null)
                return null;

            var fillups = _fillupRepository.GetFillups(vehicle.VehicleId).OrderBy(f => f.Odometer);
            var statistics = CalculateStatistics.Calculate(fillups);

            return new VehicleModel(vehicle, statistics);
        }
    }
}