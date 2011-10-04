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
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Data.InMemory.Tests.Repositories
{
    public class VehiclePhotoRepositoryFixture
    {
        [Fact]
        public void WhenConstuctedWithUnitOfWork_ThenSuccessful()
        {
            var actual = new VehiclePhotoRepository();

            Assert.NotNull(actual);
        }

        [Fact]
        public void WhenCreateCalled_ThenPhotoPersists()
        {
            var repository = new VehiclePhotoRepository();
            var photo = new VehiclePhoto
                            {
                                ImageMimeType = "image/jpeg",
                                Image = new byte[1]
                            };
            repository.Create(1, photo);

            var repository2 = new VehiclePhotoRepository();
            Assert.NotNull(repository2.Get(1));
        }

        [Fact]
        public void WhenDeleteCalled_ThenPhotoNuked()
        {
            const int vehicleId = 1;
            var repository = new VehiclePhotoRepository();
            var photo = new VehiclePhoto
                            {
                                ImageMimeType = "image/jpeg",
                                Image = new byte[1]
                            };
            repository.Create(vehicleId, photo);
            repository.Delete(photo.Id);

            var repository2 = new VehiclePhotoRepository();
            Assert.Null(repository2.Get(photo.Id));
        }

        [Fact]
        public void WhenDeleteCalledForNonexistentPhoto_ThenThrows()
        {
            var repository = new VehiclePhotoRepository();

            Assert.Throws<InvalidOperationException>(() => repository.Delete(12345));
        }

        [Fact]
        public void WhenGetCalled_ThenReturnsPhoto()
        {
            const int vehicleId = 99;
            new VehiclePhotoRepository()
                .Create(
                    vehicleId,
                    new VehiclePhoto
                        {
                            ImageMimeType = "image/jpeg",
                            Image = new byte[1]
                        });


            var target = new VehiclePhotoRepository();

            var actual = target.Get(vehicleId);
            Assert.NotNull(actual);
        }

        [Fact]
        public void WhenGetCalledForNonExistantPhoto_ThenReturnsNull()
        {
            var target = new VehiclePhotoRepository();

            Assert.Null(target.Get(1200));
        }
    }
}