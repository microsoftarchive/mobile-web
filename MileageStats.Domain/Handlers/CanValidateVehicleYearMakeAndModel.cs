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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class CanValidateVehicleYearMakeAndModel
    {
        private readonly IVehicleManufacturerRepository _manufacturerRepository;

        public CanValidateVehicleYearMakeAndModel(IVehicleManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public virtual IEnumerable<ValidationResult> Execute(ICreateVehicleCommand vehicle)
        {
            bool isYearSet = vehicle.Year.HasValue;
            bool isMakeSet = !string.IsNullOrEmpty(vehicle.MakeName);
            bool isModelSet = !string.IsNullOrEmpty(vehicle.ModelName);

            bool isYearValid = true;
            bool isMakeValid = true;
            bool isModelValid = true;

            // Make requires a year
            if ((!isYearSet) && isMakeSet)
            {
                yield return new ValidationResult("MakeName", Resources.VehicleMissingYearForMake);
                isMakeValid = false;
            }

            // Model requires a year and make
            if (isModelSet)
            {
                if (!isYearSet)
                {
                    yield return new ValidationResult("ModelName", Resources.VehicleMissingYearForModel);
                    isModelValid = false;
                }
                else if (!isMakeSet)
                {
                    yield return new ValidationResult("ModelName", Resources.VehicleMissingMakeForModel);
                    isModelValid = false;
                }
            }

            // Validate Year value (if not already invalid)
            if (isYearValid)
            {
                isYearValid = ((!isYearSet) || _manufacturerRepository.IsValidYear(vehicle.Year.Value));
                if (!isYearValid)
                {
                    yield return new ValidationResult("Year", Resources.VehicleYearInvalid);
                }
            }

            // Validate Make value (if not already invalid)
            if (isMakeValid)
            {
                isMakeValid = ((!isMakeSet) ||
                               (isYearSet &&
                                _manufacturerRepository.IsValidMake(vehicle.Year.Value, vehicle.MakeName)));
                if (!isMakeValid)
                {
                    yield return new ValidationResult("MakeName", Resources.VehicleMakeInvalid);
                }
            }

            // Validate Model value (if not already invalid)
            if (isModelValid)
            {
                isModelValid = ((!isModelSet) ||
                                (isYearSet && isMakeSet &&
                                 _manufacturerRepository.IsValidModel(vehicle.Year.Value, vehicle.MakeName,
                                                                      vehicle.ModelName)));
                if (!isModelValid)
                {
                    yield return new ValidationResult("ModelName", Resources.VehicleModelInvalid);
                }
            }
        }
    }
}