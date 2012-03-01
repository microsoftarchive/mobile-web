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
using System.Collections.ObjectModel;
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;


namespace MileageStats.Domain.Handlers
{
    public class GetFillupsForVehicle
    {
        private readonly IFillupRepository _fillupRepository;

        public GetFillupsForVehicle(IFillupRepository fillupRepository)
        {
            _fillupRepository = fillupRepository;
        }
        public virtual IEnumerable<FillupEntry> Execute(int vehicleId)
        {
            try
            {
                var fillups = _fillupRepository
                    .GetFillups(vehicleId)
                    .OrderBy(f => f.Date)
                    .ToList();

                return new ReadOnlyCollection<FillupEntry>(fillups);
            }
            catch (InvalidOperationException ex)
            {
                throw new UnauthorizedException(Resources.UnableToRetireveFillupsExceptionMessage, ex);
            }
        }
    }
}