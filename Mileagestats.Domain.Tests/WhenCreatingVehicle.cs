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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;
using Moq;
using Xunit;
using Mock = MileageStats.Domain.Tests.Helpers.Mock;

namespace MileageStats.Domain.Tests
{
    public class WhenCreatingVehicle
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IVehiclePhotoRepository> _photoRepo;

        private const int UserId = 99;

        public WhenCreatingVehicle()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _photoRepo = new Mock<IVehiclePhotoRepository>();
        }

        [Fact]
        public void InvokesVehicleRepository()
        {
            var vehicleForm = new VehicleFormModel { Name = "vehicle" };

            var handler = new CreateVehicle(_vehicleRepo.Object, _photoRepo.Object);
            handler.Execute(UserId, vehicleForm, null);

            _vehicleRepo
                .Verify(r => r.Create(UserId, It.IsAny<Vehicle>()), Times.Once());
        }

        [Fact]
        public void WithAPhoto_ThenInvokesVehicleRepositoryToUpdatePhotoInfo()
        {
            var vehicleForm = new VehicleFormModel { Name = "vehicle" };
            var photoStream = Mock.MockPhotoStream();

            var handler = new CreateVehicle(_vehicleRepo.Object, _photoRepo.Object);
            handler.Execute(UserId, vehicleForm, photoStream.Object);

            _vehicleRepo
                .Verify(r => r.Create(UserId, It.IsAny<Vehicle>()), Times.Once());

            _vehicleRepo
                .Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once());
        }
    }
}