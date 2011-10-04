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
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenGettingFillups
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IFillupRepository> _fillupRepositoryMock;
        private const int DefaultUserId = 99;
        private const int DefaultVehicleId = 88;
        private const int DefaultFillupId = 88;

        public WhenGettingFillups()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _fillupRepositoryMock = new Mock<IFillupRepository>();
        }

        [Fact]
        public void WhenGettingFillupsAndRepositoryThrows_ThenWrapsException()
        {
            _fillupRepositoryMock
                .Setup(f => f.GetFillups(DefaultVehicleId))
                .Throws<InvalidOperationException>();

            var handler = new GetFillupsForVehicle(_fillupRepositoryMock.Object);

            var ex = Assert.Throws<BusinessServicesException>(() => handler.Execute(DefaultVehicleId));
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void WhenGettingFillup_ThenDelegatesToFillupRepository()
        {
            _fillupRepositoryMock
                .Setup(x => x.GetFillup(DefaultFillupId))
                .Verifiable();

            var handler = new GetFillupById(_fillupRepositoryMock.Object);
           handler.Execute(DefaultFillupId);

            _fillupRepositoryMock
                .Verify(r => r.GetFillup(DefaultFillupId), Times.Once());
        }

        [Fact]
        public void WhenGettingFillupAndErrorOccurs_ThenThrows()
        {
            _fillupRepositoryMock
                .Setup(f => f.GetFillup(DefaultFillupId))
                .Throws<InvalidOperationException>();

            var handler = new GetFillupById(_fillupRepositoryMock.Object);

            var ex = Assert.Throws<BusinessServicesException>(() => handler.Execute(DefaultFillupId));
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }
    }
}