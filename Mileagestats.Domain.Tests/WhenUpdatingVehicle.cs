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
using System.Web;
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
    public class WhenUpdatingVehicle
    {
        private const int UserId = 99;
        private const int DefaultVehicleId = 1;
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IVehiclePhotoRepository> _photoRepo;

        public WhenUpdatingVehicle()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _photoRepo = new Mock<IVehiclePhotoRepository>();
        }

        [Fact]
        public void ThenDelegatesToVehicleRepository()
        {
            var vehicleForm = new VehicleFormModel { VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, DefaultVehicleId))
                .Returns(new Vehicle());

            var handler = new UpdateVehicle(_vehicleRepo.Object, _photoRepo.Object);
            handler.Execute(UserId, vehicleForm, null);

            _vehicleRepo.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once());
        }

        [Fact]
        public void WihtNewPhoto_ThenDelegatesToPhotoRepositoryAddNewPhoto()
        {
            var vehicleForm = new VehicleFormModel { VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(r => r.GetVehicle(UserId, DefaultVehicleId))
                .Returns(new Vehicle { VehicleId = DefaultVehicleId });
            
            var newPhotoFile = Mock.MockPhotoStream().Object;

            var handler = new UpdateVehicle(_vehicleRepo.Object, _photoRepo.Object);
            handler.Execute(UserId, vehicleForm, newPhotoFile);

            _photoRepo.Verify(r => r.Create(DefaultVehicleId, It.IsAny<VehiclePhoto>()), Times.Once());
        }

        [Fact]
        public void WithExistingPhoto_ThenDelegatesToPhotoRepositoryToDeleteOldPhoto()
        {
            const int vehiclePhotoId = 300;
            var vehicleForm = new VehicleFormModel { VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, DefaultVehicleId))
                .Returns(new Vehicle { VehicleId = DefaultVehicleId, PhotoId = vehiclePhotoId });

            var newPhotoFile = Mock.MockPhotoStream().Object;

            var handler = new UpdateVehicle(_vehicleRepo.Object, _photoRepo.Object);
            handler.Execute(UserId, vehicleForm, newPhotoFile);

            _photoRepo.Verify(r => r.Delete(vehiclePhotoId), Times.Once());
        }

        [Fact]
        public void ForOtherUser_ThenThrows()
        {
            const int anotherUserId = 87;

            var vehicleForm = new VehicleFormModel { Name = "vehicle", VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(anotherUserId, DefaultVehicleId))
                .Throws(new InvalidOperationException());

            var handler = new UpdateVehicle(_vehicleRepo.Object, _photoRepo.Object);

            Assert.Throws<UnauthorizedException>(() => handler.Execute(anotherUserId, vehicleForm, null));
        }

        [Fact]
        public void ThatAndVehicleDoesNotExist_ThenThrowsNonExistent_ThenThrows()
        {
            // this is the same test as FromOtherUsers_ThenThrows
            const int nonExistentVehicleId = 87;
            var vehicleForm = new VehicleFormModel { Name = "vehicle", VehicleId = nonExistentVehicleId };

            // the repo throws an exception when it can't find a match with both the user and the vehicle
            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, It.IsAny<int>()))
                .Returns((Vehicle)null);

            var handler = new UpdateVehicle(_vehicleRepo.Object, _photoRepo.Object);

            Assert.Throws<HttpException>(() => handler.Execute(UserId, vehicleForm, null));
        }

        [Fact]
        public void Throws_ThenWrapsException()
        {
            var vehicleForm = new VehicleFormModel { Name = "vehicle", VehicleId = DefaultVehicleId };

            _vehicleRepo
                .Setup(vr => vr.GetVehicle(UserId, DefaultVehicleId))
                .Throws(new InvalidOperationException());

            var handler = new UpdateVehicle(_vehicleRepo.Object, _photoRepo.Object);

            Exception ex = Assert.Throws<UnauthorizedException>(() => handler.Execute(UserId, vehicleForm, null));

            Assert.NotNull(ex.InnerException);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

   
    }
}