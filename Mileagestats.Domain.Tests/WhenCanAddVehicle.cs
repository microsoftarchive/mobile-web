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
using System.Collections.Generic;
using System.Linq;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;
using Moq;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class WhenCanAddVehicle
    {
        private readonly Mock<IVehicleRepository> _vehicleRepo;
        private readonly Mock<IVehicleManufacturerRepository> _manufacturerRepo;

        private const int UserId = 99;
        private const int DefaultVehicleId = 1;

        public WhenCanAddVehicle()
        {
            _vehicleRepo = new Mock<IVehicleRepository>();
            _manufacturerRepo = new Mock<IVehicleManufacturerRepository>();
        }

        [Fact]
        public void ThenNoValidationErrorsAreReturned()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle"
                                  };

            var vehicles = new List<Vehicle> { new Vehicle() };
            _vehicleRepo
                .Setup(vr => vr.GetVehicles(UserId))
                .Returns(vehicles);

            var subhandler = MockCanValidateVehicleYearMakeAndModel(vehicleForm);

            var handler = new CanAddVehicle(_vehicleRepo.Object, subhandler);

            var actual = handler.Execute(UserId, vehicleForm);

            Assert.Empty(actual);

        }

        [Fact]
        public void WithTooManyVehicles_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle"
                                  };

            var vehicles = Enumerable
                .Range(0, CanAddVehicle.MaxNumberOfVehiclesPerUser)
                .Select(i => new Vehicle());

            _vehicleRepo
                .Setup(vr => vr.GetVehicles(UserId))
                .Returns(vehicles);

            var subhandler = MockCanValidateVehicleYearMakeAndModel(vehicleForm);

            var handler = new CanAddVehicle(_vehicleRepo.Object, subhandler);
            var actual = handler.Execute(UserId, vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("maximum number", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }
        
        [Fact]
        public void WithMissingYearForMake_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle",
                                      MakeName = "Test"
                                  };

            _manufacturerRepo
                .Setup(x => x.IsValidYear(It.IsAny<int>()))
                .Returns(true);

            _manufacturerRepo
                .Setup(x => x.IsValidMake(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            _manufacturerRepo
                .Setup(x => x.IsValidModel(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var handler = new CanValidateVehicleYearMakeAndModel(_manufacturerRepo.Object);
            var actual = handler.Execute(vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("missing", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("year", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("make", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WithMissingYearForModel_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle",
                                      ModelName = "Test"
                                  };

            _manufacturerRepo
                .Setup(x => x.IsValidYear(It.IsAny<int>()))
                .Returns(true);

            _manufacturerRepo
                .Setup(x => x.IsValidMake(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            _manufacturerRepo
                .Setup(x => x.IsValidModel(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var handler = new CanValidateVehicleYearMakeAndModel(_manufacturerRepo.Object);
            var actual = handler.Execute(vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("missing", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("year", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("model", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WithMissingMakeForModel_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle",
                                      Year = 1975,
                                      ModelName = "Test"
                                  };

            _manufacturerRepo
                .Setup(x => x.IsValidYear(It.IsAny<int>()))
                .Returns(true);

            _manufacturerRepo
                .Setup(x => x.IsValidMake(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            _manufacturerRepo
                .Setup(x => x.IsValidModel(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var handler = new CanValidateVehicleYearMakeAndModel(_manufacturerRepo.Object);
            var actual = handler.Execute(vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("missing", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("make", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("model", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WithInvalidYear_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle",
                                      Year = 2100,
                                  };

            SetupManufacturerRepo(isYearValid: false);

            var handler = new CanValidateVehicleYearMakeAndModel(_manufacturerRepo.Object);
            var actual = handler.Execute(vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("not valid", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("year", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WithInvalidMake_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle",
                                      Year = 1,
                                      MakeName = "Test",
                                      ModelName = "Test"
                                  };

            SetupManufacturerRepo(isMakeValid: false);

            var handler = new CanValidateVehicleYearMakeAndModel(_manufacturerRepo.Object);
            var actual = handler.Execute(vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("not valid", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("make", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WithInvalidModel_ThenReturnsValidationResult()
        {
            var vehicleForm = new VehicleFormModel
                                  {
                                      Name = "vehicle",
                                      Year = 1,
                                      MakeName = "Test",
                                      ModelName = "Test"
                                  };

            SetupManufacturerRepo(isModelValid: false);

            var handler = new CanValidateVehicleYearMakeAndModel(_manufacturerRepo.Object);
            var actual = handler.Execute(vehicleForm).ToList();

            Assert.Equal(1, actual.Count);
            Assert.Contains("not valid", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains("model", actual[0].Message, StringComparison.CurrentCultureIgnoreCase);
        }

        private static CanValidateVehicleYearMakeAndModel MockCanValidateVehicleYearMakeAndModel(VehicleFormModel vehicleForm)
        {
            var subhandler = new Mock<CanValidateVehicleYearMakeAndModel>(null);

            subhandler
                .Setup(h => h.Execute(vehicleForm))
                .Returns(Enumerable.Empty<ValidationResult>());

            return subhandler.Object;
        }

        void SetupManufacturerRepo(bool isYearValid = true, bool isMakeValid = true, bool isModelValid = true)
        {
            _manufacturerRepo
                .Setup(x => x.IsValidYear(It.IsAny<int>()))
                .Returns(isYearValid);

            _manufacturerRepo
                .Setup(x => x.IsValidMake(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(isMakeValid);

            _manufacturerRepo
                .Setup(x => x.IsValidModel(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(isModelValid);
        }
    }
}