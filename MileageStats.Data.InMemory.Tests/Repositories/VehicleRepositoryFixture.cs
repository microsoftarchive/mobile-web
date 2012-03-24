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
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Data.InMemory.Tests.Repositories
{
    // This test fixture was intentionally written with an explicit dependency on 
    // the MileageStatsDbContext. 
    public class VehicleRepositoryFixture
    {
        private User defaultTestUser;

        public VehicleRepositoryFixture()
        {
            this.InitializeFixture();
        }

        private void InitializeFixture()
        {
            this.defaultTestUser = new User()
                                       {
                                           AuthorizationId = "TestAuthorizationId",
                                           DisplayName = "DefaultTestUser"
                                       };

            var repository = new UserRepository();
            repository.Create(this.defaultTestUser);
        }

        [Fact]
        public void WhenGetAllFromEmptyDatabase_ThenReturnsEmptyCollection()
        {
            var repository = new VehicleRepository();
            IEnumerable<Vehicle> actual = repository.GetVehicles(this.defaultTestUser.UserId);

            Assert.NotNull(actual);
            List<Vehicle> actualList = new List<Vehicle>(actual);
            Assert.Equal(0, actualList.Count);
        }


        [Fact]
        public void WhenVehicleAdded_ThenUpdatesRepository()
        {
            var repository = new VehicleRepository();
            var initialCount = repository.All().Count();

            var vehicle = new Vehicle {Name = "Vehicle"};

            repository.Create(defaultTestUser.UserId, vehicle);

            var actualList = new VehicleRepository().All().ToList();

            Assert.Equal(initialCount + 1, actualList.Count);
            Assert.Equal(vehicle.Name, actualList[initialCount].Name);
        }

        [Fact]
        public void WhenVehicleAdded_ThenPersists()
        {
            int userId = this.defaultTestUser.UserId;
            var repository = new VehicleRepository();

            Vehicle vehicle1 = new Vehicle {Name = "Vehicle1"};
            Vehicle vehicle2 = new Vehicle {Name = "Vehicle2"};

            repository.Create(userId, vehicle1);
            repository.Create(userId, vehicle2);

            // I use a new context and repository to verify the data was stored
            var repository2 = new VehicleRepository();

            var retrievedVehicles = repository2.GetVehicles(userId);

            Assert.NotNull(retrievedVehicles);
            List<Vehicle> actualList = new List<Vehicle>(retrievedVehicles);

            Assert.Equal(2, actualList.Count);
            Assert.Equal(vehicle1.Name, actualList[0].Name);
            Assert.Equal(vehicle2.Name, actualList[1].Name);
        }

        [Fact]
        public void WhenVehicleAdded_ThenUpdatesVehicleId()
        {
            const int userId = 999;
            var repository = new VehicleRepository();

            var vehicle = new Vehicle {Name = "Vehicle"};

            repository.Create(userId, vehicle);

            var retrievedVehicles = repository.GetVehicles(userId);

            Assert.NotNull(retrievedVehicles);
            var actualList = new List<Vehicle>(retrievedVehicles);

            Assert.Equal(1, actualList.Count);
            Assert.Equal(vehicle.VehicleId, actualList[0].VehicleId);
        }

        [Fact]
        public void WhenVehicleModified_ThenPersistsChange()
        {
            int userId = defaultTestUser.UserId;
            var repository = new VehicleRepository();

            var vehicle = new Vehicle { Name = "Vehicle", UserId = userId };
            repository.Create(userId, vehicle);

            // I use a new context and repository to verify the data was stored
            var repositoryForUpdate = new VehicleRepository();

            var retrievedVehicle = repositoryForUpdate.GetVehicles(userId).First();

            retrievedVehicle.Name = "Updated Vehicle Name";
            repositoryForUpdate.Update(retrievedVehicle);
            int updatedVehicleId = retrievedVehicle.VehicleId;

            var repositoryForVerify = new VehicleRepository();
            var updatedVehicle = repositoryForVerify.GetVehicle(userId, updatedVehicleId);

            Assert.Equal("Updated Vehicle Name", updatedVehicle.Name);
        }

        [Fact]
        public void WhenVehicleModifiedInSameContext_ThenPersistsChange()
        {
            int userId = this.defaultTestUser.UserId;
            var repository = new VehicleRepository();

            Vehicle vehicle = new Vehicle {Name = "Vehicle", UserId = userId};
            repository.Create(userId, vehicle);

            // I use a new context and repository to verify the data was stored
            var repositoryForUpdate = new VehicleRepository();

            var retrievedVehicle = repositoryForUpdate.GetVehicles(userId).First();

            retrievedVehicle.Name = "Updated Vehicle Name";
            repositoryForUpdate.Update(retrievedVehicle);
            int updatedVehicleId = retrievedVehicle.VehicleId;

            var repositoryForVerify = new VehicleRepository();
            var updatedVehicle = repositoryForVerify.GetVehicle(userId, updatedVehicleId);

            Assert.Equal("Updated Vehicle Name", updatedVehicle.Name);
        }

        [Fact]
        public void WhenGettingOtherUsersVehicle_ThenReturnsNull()
        {
            int userId = defaultTestUser.UserId;
            var repository = new VehicleRepository();
            const int unknownId = 4200;

            var vehicle = new Vehicle {Name = "Vehicle"};

            repository.Create(userId, vehicle);

            var repositoryForVerify = new VehicleRepository();
            Assert.Null(repositoryForVerify.GetVehicle(unknownId, vehicle.VehicleId));
        }

        [Fact]
        public void WhenVehicleDeleted_ThenPersists()
        {
            int userId = this.defaultTestUser.UserId;
            var repository = new VehicleRepository();

            Vehicle vehicle1 = new Vehicle {Name = "Vehicle1"};
            repository.Create(userId, vehicle1);

            var retrievedVehicles = repository.GetVehicles(userId);
            Assert.Equal(1, retrievedVehicles.Count());
            repository.Delete(retrievedVehicles.First().VehicleId);

            // I use a new context and repository to verify the vehicle was deleted
            var repository2 = new VehicleRepository();

            var verifyVehicles = repository2.GetVehicles(userId);

            Assert.NotNull(retrievedVehicles);
            Assert.Equal(0, verifyVehicles.Count());
        }
    }
}