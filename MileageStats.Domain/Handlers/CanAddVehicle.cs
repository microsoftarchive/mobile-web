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

using System.Collections.Generic;
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class CanAddVehicle
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly CanValidateVehicleYearMakeAndModel _validateVehicleYearMakeAndModel;

        public const int MaxNumberOfVehiclesPerUser = 10;

        public CanAddVehicle(IVehicleRepository vehicleRepository, CanValidateVehicleYearMakeAndModel validateVehicleYearMakeAndModel)
        {
            _vehicleRepository = vehicleRepository;
            _validateVehicleYearMakeAndModel = validateVehicleYearMakeAndModel;
        }

        public virtual IEnumerable<ValidationResult> Execute(int userId, ICreateVehicleCommand vehicle)
        {
            var vehicles = _vehicleRepository.GetVehicles(userId);

            if (vehicles.Count() >= MaxNumberOfVehiclesPerUser)
            {
                yield return new ValidationResult(Resources.TooManyVehicles);
            }

            foreach (var result in _validateVehicleYearMakeAndModel.Execute(vehicle))
            {
                yield return result;
            }
        }
    }
}