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
using System.Globalization;
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class CanAddFillup
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFillupRepository _fillupRepository;

        public CanAddFillup(IVehicleRepository vehicleRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _fillupRepository = fillupRepository;
        }

        public virtual IEnumerable<ValidationResult> Execute(int userId, int vehicleId, ICreateFillupEntryCommand fillup)
        {
            var foundVehicle = _vehicleRepository.GetVehicle(userId, vehicleId);

            if (foundVehicle == null)
            {
                yield return new ValidationResult(Resources.VehicleNotFound);
            }
            else
            {
                var fillups = _fillupRepository.GetFillups(vehicleId);

                if (!fillups.Any()) yield break;

                var priorFillup = fillups.LastOrDefault(f => f.Date <= fillup.Date);

                if ((priorFillup != null) && (priorFillup.Odometer >= fillup.Odometer))
                {
                    yield return new ValidationResult(
                        "Odometer",
                        string.Format(CultureInfo.CurrentUICulture,
                                      Resources.OdometerNotGreaterThanPrior,
                                      priorFillup.Odometer));
                }
            }
        }
    }
}
