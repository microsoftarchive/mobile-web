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
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;


namespace MileageStats.Domain.Handlers
{
    public class AddFillupToVehicle
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFillupRepository _fillupRepository;

        public AddFillupToVehicle(IVehicleRepository vehicleRepository, IFillupRepository fillupRepository)
        {
            _vehicleRepository = vehicleRepository;
            _fillupRepository = fillupRepository;
        }

        public virtual void Execute(int userId, int vehicleId, ICreateFillupEntryCommand newFillup)
        {
            if (newFillup == null) throw new ArgumentNullException("newFillup");

            try
            {
                var vehicle = _vehicleRepository.GetVehicle(userId, vehicleId);

                if (vehicle != null)
                {
                    newFillup.VehicleId = vehicleId;
                    var fillup = newFillup;

                    var entity = ToEntry(fillup);
                    AdjustSurroundingFillupEntries(entity);

                    _fillupRepository.Create(userId, vehicleId, entity);

                    newFillup.Distance = entity.Distance;   // update calculated value
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new UnauthorizedException(Resources.UnableToAddFillupToVehicleExceptionMessage, ex);
            }
        }

        private void AdjustSurroundingFillupEntries(FillupEntry newFillup)
        {
            if (newFillup == null) throw new ArgumentNullException("newFillup");

            var fillups = _fillupRepository.GetFillups(newFillup.VehicleId);

            // Prior fillups are ordered descending so that FirstOrDefault() chooses the one closest to the new fillup.
            // Secondary ordering is by entry ID ensure a consistent ordering/
            var priorFillup = fillups
                .OrderByDescending(f => f.Date).ThenByDescending(f => f.FillupEntryId)
                .FirstOrDefault(f => (f.Date <= newFillup.Date) && (f.FillupEntryId != newFillup.FillupEntryId));

            // Prior fillups are ordered ascending that FirstOrDefault() chooses the one closest to the new fillup.
            // Secondary ordering is by entry ID ensure a consistent ordering.
            var nextFillup = fillups
                .OrderBy(f => f.Date).ThenBy(f => f.FillupEntryId)
                .FirstOrDefault(f => (f.Date >= newFillup.Date) && (f.FillupEntryId != newFillup.FillupEntryId));

            CalculateInterFillupStatistics(newFillup, priorFillup);
            CalculateInterFillupStatistics(nextFillup, newFillup);
        }

        private static void CalculateInterFillupStatistics(FillupEntry fillup, FillupEntry priorFillup)
        {
            if (priorFillup != null && fillup != null)
            {
                fillup.Distance = Math.Abs(fillup.Odometer - priorFillup.Odometer);
            }
        }

        private static FillupEntry ToEntry(ICreateFillupEntryCommand source)
        {
            if (source == null)
            {
                return null;
            }

            var fillup = new FillupEntry
                             {
                                 FillupEntryId = source.FillupEntryId,
                                 Date = source.Date,
                                 Distance = source.Distance,
                                 Odometer = source.Odometer,
                                 PricePerUnit = source.PricePerUnit.Value,
                                 Remarks = source.Remarks,
                                 TotalUnits = source.TotalUnits.Value,
                                 TransactionFee = source.TransactionFee.Value,
                                 VehicleId = source.VehicleId,
                                 Vendor = source.Vendor ?? source.Location,
                                 UnitOfMeasure = source.UnitOfMeasure
                             };
            return fillup;
        }
    }
}