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
using MileageStats.Domain.Contracts.Data;

namespace MileageStats.Domain.Handlers
{
    public class GetYearsMakesAndModels
    {
        private readonly IVehicleManufacturerRepository _manufacturerRepository;

        public GetYearsMakesAndModels(IVehicleManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public virtual Tuple<int[], string[], string[]> Execute(int? filteredToYear = null, string filteredByMake = null)
        {
            int[] years = _manufacturerRepository.GetYears();
            string[] makes = null;
            string[] models = null;

            if ((years != null) && (years.Length > 0))
            {
                // If the user specified a year, then look up the makes.
                // Otherwise, use the makes from the first year in the list.
                int selectedYear = -1;

                if ((filteredToYear != null) &&
                    (_manufacturerRepository.IsValidYear(filteredToYear.Value)))
                {
                    selectedYear = filteredToYear.Value;
                }

                makes = _manufacturerRepository.GetMakes(selectedYear);

                // if the user specified a year and a make, then look up the models.
                if ((makes != null) && (makes.Length > 0))
                {
                    string selectedMake = string.Empty;

                    if ((!string.IsNullOrEmpty(filteredByMake)) &&
                        (_manufacturerRepository.IsValidMake(selectedYear, filteredByMake)))
                    {
                        selectedMake = filteredByMake;
                    }

                    models = _manufacturerRepository.GetModels(selectedYear, selectedMake);
                }
            }

            if (years == null)
            {
                years = new int[] {};
            }

            if (makes == null)
            {
                makes = new string[] {};
            }

            if (models == null)
            {
                models = new string[] {};
            }

            return new Tuple<int[], string[], string[]>(years, makes, models);
        }
    }
}