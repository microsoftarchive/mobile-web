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
using System.Linq;
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Data.InMemory.Tests.Repositories
{
    public class FillupRepositoryFixture
    {
        private User defaultTestUser;
        private Vehicle defaultVehicle;

        public FillupRepositoryFixture()
        {
            InitializeFixture();
        }

        private void InitializeFixture()
        {
            defaultTestUser = new User
                                       {
                                           AuthorizationId = "TestAuthorizationId",
                                           DisplayName = "DefaultTestUser"
                                       };

            var userRepository = new UserRepository();
            userRepository.Create(defaultTestUser);

            int userId = defaultTestUser.UserId;

            var vehicleRepository = new VehicleRepository();
            defaultVehicle = new Vehicle
                                      {
                                          Name = "Test Vehicle"
                                      };
            vehicleRepository.Create(defaultTestUser.UserId, defaultVehicle);
        }

        [Fact]
        public void WhenAddingMinimalFillupEntry_ThenPersistsToTheDatabase()
        {
            var repository = new FillupRepository();

            var fillupEntry = new FillupEntry
                                  {
                                      Date = DateTime.Now,
                                      Odometer = 3000,
                                      PricePerUnit = 3.24d,
                                      TotalUnits = 13.01d,
                                  };

            repository.Create(defaultTestUser.UserId, defaultVehicle.VehicleId, fillupEntry);

            // Verification
            var repositoryForVerification = new FillupRepository();
            var retrievedFillup = repositoryForVerification.GetFillups(defaultVehicle.VehicleId).First();

            Assert.NotNull(retrievedFillup);
            Assert.Equal(fillupEntry.Date.ToShortDateString(), retrievedFillup.Date.ToShortDateString());
            // We only care that the dates map.
            AssertExtensions.PropertiesAreEqual(fillupEntry, retrievedFillup, "Odometer", "PricePerUnit", "TotalUnits");
        }


        [Fact]
        public void WhenAddingFillupEntry_ThenFillupAssignedNewId()
        {
            var repository = new FillupRepository();

            var fillupEntry = new FillupEntry
                                  {
                                      Date = DateTime.Now,
                                      Odometer = 3000,
                                      PricePerUnit = 3.24d,
                                      TotalUnits = 13.01d,
                                  };

            repository.Create(defaultTestUser.UserId, defaultVehicle.VehicleId, fillupEntry);

            Assert.NotEqual(0, fillupEntry.FillupEntryId);
        }

        [Fact]
        public void WhenGettingAllFillupsForNewVehicle_ThenReturnsEmptyCollection()
        {
            var repository = new FillupRepository();

            var fillups = repository.GetFillups(defaultVehicle.VehicleId);

            Assert.NotNull(fillups);
            Assert.Equal(0, fillups.Count());
        }

        [Fact]
        public void WhenGettingAllFillups_ThenReturnsAllFillups()
        {
            var repository = new FillupRepository();

            var fillupEntry1 = new FillupEntry
                                   {
                                       Date = DateTime.Now,
                                       Odometer = 3000,
                                       PricePerUnit = 3.24d,
                                       TotalUnits = 13.01d,
                                   };
            repository.Create(defaultTestUser.UserId, defaultVehicle.VehicleId, fillupEntry1);

            var fillupEntry2 = new FillupEntry
                                   {
                                       Date = DateTime.Now,
                                       Odometer = 3001,
                                       PricePerUnit = 3.24d,
                                       TotalUnits = 13.01d,
                                   };
            repository.Create(defaultTestUser.UserId, defaultVehicle.VehicleId, fillupEntry2);


            var fillups = repository.GetFillups(defaultVehicle.VehicleId);

            Assert.NotNull(fillups);
            Assert.Equal(2, fillups.Count());
        }

        [Fact]
        public void WhenAddingFillupEntryWithAllData_ThenPersistsToTheDatabase()
        {
            var repository = new FillupRepository();

            var fillupEntry = new FillupEntry
                                  {
                                      Date = DateTime.Now,
                                      Odometer = 3000,
                                      PricePerUnit = 3.24d,
                                      TotalUnits = 13.01d,
                                      Remarks = "Remarkable",
                                      TransactionFee = 1.25d,
                                      Vendor = "Ideal Vendor"
                                  };

            repository.Create(defaultTestUser.UserId, defaultVehicle.VehicleId, fillupEntry);

            // Verification
            var repositoryForVerification = new FillupRepository();
            var enteredFillup = repositoryForVerification.GetFillups(defaultVehicle.VehicleId).First();
            Assert.NotNull(enteredFillup);
            Assert.Equal(fillupEntry.Date.ToShortDateString(), enteredFillup.Date.ToShortDateString());
            // We only care that the dates map.
            AssertExtensions.PropertiesAreEqual(fillupEntry, enteredFillup, "Odometer", "PricePerUnit", "TotalUnits",
                                                "Remarks", "TransactionFee", "Vendor");
        }
    }
}