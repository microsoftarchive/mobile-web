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

namespace MileageStats.Domain.Contracts.Data
{
    /// <summary>
    /// A repository of vehicle year/make/model lookup data.
    /// </summary>
    public interface IVehicleManufacturerRepository
    {
        /// <summary>
        /// Gets the manufacturer years for all vehicles.
        /// </summary>
        /// <returns></returns>
        int[] GetYears();

        /// <summary>
        /// Gets the vehicle makes for the specified manufacturer year.
        /// </summary>
        /// <param name="year">The year the vehicle was manufactured.</param>
        /// <returns>A list of makes for that year (if found); otherwise null.</returns>
        string[] GetMakes(int year);

        /// <summary>
        /// Gets the vehicle models for the specified manufacturer year and make.
        /// </summary>
        /// <param name="year">The year the vehicle was manufactured.</param>
        /// <param name="make">The make of the vehicle.</param>
        /// <returns>A list of models for that year and make; otherwise null.</returns>
        string[] GetModels(int year, string make);

        /// <summary>
        /// Determines whether the specified year is valid.
        /// </summary>
        /// <param name="year">The year to check.</param>
        /// <returns>
        ///   <c>true</c> if the year is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidYear(int year);

        /// <summary>
        /// Determines whether the specified year and make are valid.
        /// </summary>
        /// <param name="year">The year to check.</param>
        /// <param name="make">The make to check.</param>
        /// <returns>
        ///   <c>true</c> if the year and make is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidMake(int year, string make);

        /// <summary>
        /// Determines whether the specified year, make, and model are valid.
        /// </summary>
        /// <param name="year">The year to check.</param>
        /// <param name="make">The make to check.</param>
        /// <param name="model">The model to check.</param>
        /// <returns>
        ///   <c>true</c> if the year, make, and model is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidModel(int year, string make, string model);
    }
}