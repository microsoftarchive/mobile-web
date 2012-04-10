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
using System.Collections.Generic;
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    public class VehicleDetailsViewModel
    {
        private VehicleListViewModel _vehicleList;
        public VehicleListViewModel VehicleList
        {
            get { return _vehicleList; }
            set
            {
                _vehicleList = value;
                MakeConsistent();
            }
        }

        private VehicleModel _vehicle;
        public VehicleModel Vehicle
        {
            get { return _vehicle; }
            set
            {
                _vehicle = value;
                MakeConsistent();
                _statistics = new StatisticsViewModel(_vehicle.Statistics);
            }
        }

        private StatisticsViewModel _statistics;
        public StatisticsViewModel Statistics
        {
            get { return _statistics; }
            set
            {
                _statistics = value;
            }
        }

        public int UserId { get; set; }
        public IEnumerable<ReminderSummaryModel> OverdueReminders { get; set; }

        public int VehicleId { get; set; }

        public long DateTimeTicks { get; set; }

        void MakeConsistent()
        {
            if (Vehicle == null || VehicleList == null) return;

            VehicleList.Vehicles.SelectedItem = Vehicle;
        }
    }
}