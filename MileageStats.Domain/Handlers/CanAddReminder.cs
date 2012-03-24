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

using System.Collections.Generic;
using System.Globalization;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class CanAddReminder
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFillupRepository _fillupRepository;

        public CanAddReminder(IVehicleRepository vehicleRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _fillupRepository = fillupRepository;
        }

        public virtual IEnumerable<ValidationResult> Execute(int userId, ICreateReminderCommand reminder)
        {
            var foundVehicle = _vehicleRepository.GetVehicle(userId, reminder.VehicleId);

            if (foundVehicle == null)
            {
                yield return new ValidationResult(Resources.VehicleNotFound);
            }
            else
            {
                var fillups = _fillupRepository.GetFillups(reminder.VehicleId);
                var stats = CalculateStatistics.Calculate(fillups);
                var odometer = stats.Odometer;

                if ((reminder.DueDistance.HasValue) && (reminder.DueDistance.Value <= odometer))
                {
                    yield return new ValidationResult(
                        "DueDistance",
                        string.Format(CultureInfo.CurrentUICulture, Resources.DueDistanceNotGreaterThanOdometer, odometer));
                }
            }
        }
    }
}