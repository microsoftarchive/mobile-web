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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class VehiclePhotoFixture
    {
        [Fact]
        public void WhenConstructed_ThenPopulated()
        {
            VehiclePhoto actual = new VehiclePhoto();

            Assert.NotNull(actual);
            Assert.Equal(0, actual.VehiclePhotoId);
            Assert.Null(actual.Image);
            Assert.Null(actual.ImageMimeType);
        }

        [Fact]
        public void WehnVehiclePhotoIdSet_ThenValueUpdated()
        {
            VehiclePhoto target = new VehiclePhoto();

            target.VehiclePhotoId = 4;

            int actual = target.VehiclePhotoId;
            Assert.Equal(4, actual);
        }

        [Fact]
        public void WhenImageSet_ThenValueUpdated()
        {
            VehiclePhoto target = new VehiclePhoto();
            byte[] expected = new byte[] {1, 2, 3};

            target.Image = expected;

            byte[] actual = target.Image;
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }

        [Fact]
        public void WhenImageSetToNull_ThenValueUpdated()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.Image = new byte[] {1, 2, 3};

            target.Image = null;

            byte[] actual = target.Image;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenImageSetToValidValue_ThenValidationPasses()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.ImageMimeType = "ImageMimeType";

            target.Image = new byte[1];

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenImageSetToNull_ThenValidationFails()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.ImageMimeType = "ImageMimeType";

            target.Image = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Image", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenImageMimeTypeSet_ThenValueUpdated()
        {
            VehiclePhoto target = new VehiclePhoto();

            target.ImageMimeType = "ImageMimeType";

            string actual = target.ImageMimeType;
            Assert.Equal("ImageMimeType", actual);
        }

        [Fact]
        public void WhenImageMimeTypeSetToNull_ThenUpdatesValue()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.ImageMimeType = "ImageMimeType";

            target.ImageMimeType = null;

            string actual = target.ImageMimeType;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenImageMimeTypeSetToValidValue_ThenValidationPasses()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.Image = new byte[1];
            target.ImageMimeType = "ImageMimeType";

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenImageMimeTypeSetToNull_ThenValidationFails()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.Image = new byte[1];

            target.ImageMimeType = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("ImageMimeType", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenImageMimeTypeSetToEmpty_ThenValidationFails()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.Image = new byte[1];
            target.ImageMimeType = string.Empty;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("ImageMimeType", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenImageMimeTypeSetTo101Characters_ThenValidationFails()
        {
            VehiclePhoto target = new VehiclePhoto();
            target.Image = new byte[1];
            target.ImageMimeType = new string('1', 101);

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("ImageMimeType", validationResults[0].MemberNames.First());
        }
    }
}