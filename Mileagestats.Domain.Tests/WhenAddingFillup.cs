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

namespace MileageStats.Domain.Tests
{
    public class WhenAddingFillup
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IFillupRepository> _fillupRepositoryMock;
        private readonly Vehicle _vehicle;

        private const int DefaultUserId = 99;
        private const int DefaultVehicleId = 88;

        public WhenAddingFillup()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _fillupRepositoryMock = new Mock<IFillupRepository>();
            _vehicle = new Vehicle { VehicleId = DefaultVehicleId, Name = "vehicle" };
        }

        [Fact]
        public void WhenAddingFillup_ThenDelegatesToFillupRepository()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(_vehicle);

            var fillupForm = new FillupEntryFormModel
                                 {
                                    PricePerUnit = 0,
                                    TotalUnits = 0,
                                    TransactionFee = 0
                                 };

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, fillupForm);

            _fillupRepositoryMock
                .Verify(r => r.Create(DefaultUserId, DefaultVehicleId, It.IsAny<FillupEntry>()),
                Times.Once());
        }

        [Fact]
        public void WhenAddingFillup_ThenCalculatesDistance()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(_vehicle);

            var fillups = new[]
                {
                    new FillupEntry{FillupEntryId = 1, Date = new DateTime(2011, 3, 23), Odometer = 500},
                    new FillupEntry{FillupEntryId = 2, Date = new DateTime(2011, 3, 24), Odometer = 750}
                };

            var fillupForm = new FillupEntryFormModel
                                 {
                                     FillupEntryId = 3,
                                     Date = new DateTime(2011, 3, 26),
                                     Odometer = 1000,
                                     PricePerUnit = 0,
                                     TotalUnits = 0,
                                     TransactionFee = 0
                                 };

            _fillupRepositoryMock
                .Setup(x => x.GetFillups(DefaultVehicleId))
                .Returns(fillups);

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, fillupForm);

            Assert.Equal(250, fillupForm.Distance);
        }

        [Fact]
        public void WhenAddingFillupOnSameDate_ThenCalculatesDistance()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(_vehicle);

            var fillups = new[]
                              {
                                  new FillupEntry{FillupEntryId = 1, Date = new DateTime(2011, 3, 25), Odometer = 500},
                                  new FillupEntry{FillupEntryId = 2, Date = new DateTime(2011, 3, 25), Odometer = 750}
                              };

            var fillupForm = new FillupEntryFormModel
            {
                FillupEntryId = 3,
                Date = new DateTime(2011, 3, 25),
                Odometer = 1000,
                PricePerUnit = 0,
                TotalUnits = 0,
                TransactionFee = 0
            };

            _fillupRepositoryMock
                 .Setup(x => x.GetFillups(DefaultVehicleId))
                 .Returns(fillups);

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, fillupForm);

            Assert.Equal(250, fillupForm.Distance);
        }

        [Fact]
        public void WhenAddingFirstFillup_ThenDoesNotCalculatesDistance()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(_vehicle);

            var fillups = new FillupEntry[] {};

            var fillupForm = new FillupEntryFormModel
                                 {
                                     FillupEntryId = 3, 
                                     Date = new DateTime(2011, 3, 25),
                                     Odometer = 1000,
                                     PricePerUnit = 0,
                                     TotalUnits = 0,
                                     TransactionFee = 0
                                 };

            _fillupRepositoryMock
                 .Setup(x => x.GetFillups(DefaultVehicleId))
                 .Returns(fillups);

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, fillupForm);

            Assert.Null(fillupForm.Distance);
        }

        [Fact]
        public void WhenAddingFillup_UsesLocationIfVendorNull()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(_vehicle);

            var fillupForm = new FillupEntryFormModel
                                 {
                                     Vendor = null,
                                     Location = "testlocation",
                                     PricePerUnit = 0,
                                     TotalUnits = 0,
                                     TransactionFee = 0
                                 };

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);
            handler.Execute(DefaultUserId, DefaultVehicleId, fillupForm);

            _fillupRepositoryMock
                .Verify(r => r.Create(DefaultUserId, DefaultVehicleId, It.Is<FillupEntry>(f=>f.Vendor=="testlocation")));
        }

        [Fact]
        public void WhenAddingFillupAndVehicleRepositoryThrows_ThenWrapsException()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Throws<InvalidOperationException>();

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);

            var ex = Assert
                .Throws<UnauthorizedException>(() => handler.Execute(DefaultUserId, DefaultVehicleId, new FillupEntryFormModel()));
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void WhenAddingFillupAndFillupRepositoryThrows_ThenWrapsException()
        {
            _vehicleRepo
                .Setup(r => r.GetVehicle(DefaultUserId, DefaultVehicleId))
                .Returns(_vehicle);

            _fillupRepositoryMock
                .Setup(f => f.Create(DefaultUserId,DefaultVehicleId, It.IsAny<FillupEntry>()))
                .Throws<InvalidOperationException>();

            var handler = new AddFillupToVehicle(_vehicleRepo.Object, _fillupRepositoryMock.Object);
            var ex = Assert
                .Throws<UnauthorizedException>(() => handler.Execute(DefaultUserId, DefaultVehicleId, new FillupEntryFormModel()));
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }
    }
}