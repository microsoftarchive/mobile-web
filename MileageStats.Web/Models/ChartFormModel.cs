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
using System.Web.Mvc;
using MileageStats.Domain.Models;
using MileageStats.Domain.Properties;

namespace MileageStats.Web.Models
{
    public class ChartFormModel
    {
        public ChartFormModel()
        {
            VehicleIds = new List<int>();
        }
        public int UserId { get; set; }
        public string ChartName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<int> VehicleIds { get; set; }
        public IList<int> Positions { get; set; } 
        public VehicleModel[] AllVehicleModels { get; set; }
    }
}