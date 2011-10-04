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
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;

namespace MileageStats.Data.InMemory
{
    public class VehicleManufacturerRepository : IVehicleManufacturerRepository
    {
        private static readonly Dictionary<int, Dictionary<string, List<string>>> _yearsMakesAndModels;
        private static readonly IList<VehicleManufacturerInfo> _data = new List<VehicleManufacturerInfo>(); 

       static VehicleManufacturerRepository()
       {
           AddSampleVehicles();
           _yearsMakesAndModels = ExtractMakesAndModels(_data);
       }

        private static void AddSampleVehicles()
        {
            _data.Add(new VehicleManufacturerInfo { Year = 1997, MakeName = "Honda", ModelName = "Accord LX" });
            _data.Add(new VehicleManufacturerInfo { Year = 2003, MakeName = "BMW", ModelName = "330xi" });

            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Audi", ModelName = "A4" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Audi", ModelName = "A6" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Audi", ModelName = "A8" });

            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "BMW", ModelName = "330i" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "BMW", ModelName = "335i" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "BMW", ModelName = "550i" });

            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Honda", ModelName = "Accord" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Honda", ModelName = "CRV" });

            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Toyota", ModelName = "Prius" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Toyota", ModelName = "Sienna" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Toyota", ModelName = "Tacoma" });
            _data.Add(new VehicleManufacturerInfo { Year = 2010, MakeName = "Toyota", ModelName = "Tundra" });

            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Chevrolet", ModelName = "Camero" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Chevrolet", ModelName = "Colorado" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Chevrolet", ModelName = "Corevette" });

            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Dodge", ModelName = "Challenger" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Dodge", ModelName = "Grand Caravan" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Dodge", ModelName = "Viper" });

            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Ford", ModelName = "Explorer" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Ford", ModelName = "Focus" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Ford", ModelName = "Fusion" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Ford", ModelName = "Mustang" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Ford", ModelName = "Taurus" });

            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Jeep", ModelName = "Grand Cherokee" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Jeep", ModelName = "Liberty" });
            _data.Add(new VehicleManufacturerInfo { Year = 2011, MakeName = "Jeep", ModelName = "Wrangler" });
        }

        private static Dictionary<int, Dictionary<string, List<string>>> ExtractMakesAndModels(IEnumerable<VehicleManufacturerInfo> infos)
        {
            // Sort by year, make, and model
            var infoList = new List<VehicleManufacturerInfo>(infos);
            infoList.Sort((x, y) =>
            {
                // Compare Year
                int result = x.Year.CompareTo(y.Year);
                if (result != 0)
                {
                    return result;
                }

                // Compare Make
                result = string.Compare(x.MakeName, y.MakeName);
                if (result != 0)
                {
                    return result;
                }

                // Compare Model
                return string.Compare(x.ModelName, y.ModelName);
            });

            // Load into a dictionary of dictionaries for high-performance lookup.
            var yearsMakesAndModels = new Dictionary<int, Dictionary<string, List<string>>>();

            foreach (var info in infoList)
            {
                AddYearMakeModel(info.Year, info.MakeName, info.ModelName, yearsMakesAndModels);
            }

            return yearsMakesAndModels;
        }

        // A helper routine to ensure uniqueness in the year, make, model hierarchy.
        private static void AddYearMakeModel(int year, string make, string model, IDictionary<int, Dictionary<string, List<string>>> yearsMakesAndModels)
        {
            //Ensure makes exist for year
            Dictionary<string, List<string>> makesAndModels;
            if (!yearsMakesAndModels.TryGetValue(year, out makesAndModels))
            {
                makesAndModels = new Dictionary<string, List<string>>(StringComparer.CurrentCultureIgnoreCase);
                yearsMakesAndModels.Add(year, makesAndModels);
            }

            // Ensure models exist for a make
            List<string> models;
            if (!makesAndModels.TryGetValue(make, out models))
            {
                models = new List<string>();
                makesAndModels.Add(make, models);
            }

            // Ensure model in the list
            if (!models.Contains(model))
            {
                models.Add(model);
            }
        }

        public int[] GetYears()
        {
            return _yearsMakesAndModels.Keys.ToArray();
        }

        public string[] GetMakes(int year)
        {
            if (!_yearsMakesAndModels.ContainsKey(year)) return null;

            return _yearsMakesAndModels[year].Keys
                .Distinct()
                .OrderBy(value => value)
                .ToArray();
        }

        public string[] GetModels(int year, string make)
        {
            if(!_yearsMakesAndModels.ContainsKey(year) || !_yearsMakesAndModels[year].ContainsKey(make)) return null;

            return _yearsMakesAndModels[year][make]
                .Distinct()
                .OrderBy(value => value)
                .ToArray();
        }

        public bool IsValidYear(int year)
        {
            return _yearsMakesAndModels.ContainsKey(year);
        }

        public bool IsValidMake(int year, string make)
        {
            Dictionary<string, List<string>> makesAndModels;
            if (_yearsMakesAndModels.TryGetValue(year, out makesAndModels))
            {
                return makesAndModels.ContainsKey(make);
            }

            return false;
        }

        public bool IsValidModel(int year, string make, string model)
        {
            Dictionary<string, List<string>> makesAndModels;
            if (_yearsMakesAndModels.TryGetValue(year, out makesAndModels))
            {
                List<string> models;
                if (makesAndModels.TryGetValue(make, out models))
                {
                    return models.Contains(model, StringComparer.OrdinalIgnoreCase);
                }
            }

            return false;
        }
    }
}