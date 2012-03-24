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

using MileageStats.Domain.Models;

namespace MileageStats.Domain.Contracts.Data
{
    /// <summary>
    /// A repository for vehicle photos.
    /// </summary>
    /// <remarks>
    /// This repository allows for working with vehicle photos directly without requiring loading a user or vehicle.
    /// </remarks>
    public interface IVehiclePhotoRepository
    {
        /// <summary>
        /// Gets the vehicle photo for the specified ID.
        /// </summary>
        /// <param name="id">The ID of the vehicle photo.</param>
        /// <returns>A vehicle photo if found; otherwise null.</returns>
        VehiclePhoto Get(int id);

        /// <summary>
        /// Creates a vehicle photo
        /// </summary>
        /// <param name="vehicleId">The vehicle the photo is for</param>
        /// <param name="photo">The photo to create</param>
        void Create(int vehicleId, VehiclePhoto photo);

        /// <summary>
        /// Deletes a vehicle photo
        /// </summary>
        /// <param name="photoId">The photo to delete</param>
        void Delete(int photoId);
    }
}