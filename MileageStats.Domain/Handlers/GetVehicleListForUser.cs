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
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;


namespace MileageStats.Domain.Handlers
{
    public class GetVehicleListForUser
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFillupRepository _fillupRepository;

        public GetVehicleListForUser(IVehicleRepository vehicleRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _fillupRepository = fillupRepository;
        }

        public virtual IEnumerable<VehicleModel> Execute(int userId)
        {
            IEnumerable<Vehicle> vehicleData = null;

            try
            {
                vehicleData = _vehicleRepository.GetVehicles(userId);
            }
            catch (InvalidOperationException e)
            {
                throw new BusinessServicesException("Unable to retrieve vehicle from database.", e);
            }

            var vehicles = from vehicle in vehicleData
                           let fillups = _fillupRepository.GetFillups(vehicle.VehicleId).OrderBy(f => f.Odometer)
                           let statistics = CalculateStatistics.Calculate(fillups, includeFirst: false)
                           orderby vehicle.SortOrder
                           select new VehicleModel(vehicle, statistics);

            return vehicles;
        }
    }
}