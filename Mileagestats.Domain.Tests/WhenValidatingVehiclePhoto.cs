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
using System.Reflection;
using MileageStats.Domain.Handlers;
using Xunit;

namespace Mileagestats.Domain.Tests
{
    public class WhenValidatingVehiclePhoto
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        [Fact]
        public void ThenSetsPhotoAndReturnsEmptyValidationResults()
        {
            var stream = _assembly.GetManifestResourceStream("MileageStats.Domain.Tests.TestContent.TestVehiclePhoto.png");
            var contentLength = (int) stream.Length;
            var contentType = "/image/png";

            var handler = new CanAddPhoto();
            var result = handler.Execute(stream, contentLength, contentType);

            Assert.Empty(result);
        }

        [Fact]
        public void WithNonImageFile_ThenReturnsValidationError()
        {
            var stream = _assembly.GetManifestResourceStream("MileageStats.Domain.Tests.TestContent.NotAnImage.bin");
            var contentLength = (int) stream.Length;
            var contentType = "/image/png";

            var handler = new CanAddPhoto();
            var result = handler.Execute(stream, contentLength, contentType);

            Assert.Equal(1, result.Count());
            Assert.Contains("not an image", result.First().Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WithPhotoThatIsTooLarge_ThenReturnsValidationError()
        {
            var stream = _assembly.GetManifestResourceStream("MileageStats.Domain.Tests.TestContent.FileTooBig.jpg");
            var contentLength = (int) stream.Length;
            var contentType = "/image/png";

            var handler = new CanAddPhoto();
            var result = handler.Execute(stream, contentLength, contentType);

            Assert.Equal(1, result.Count());
            Assert.Contains("must be less than", result.First().Message, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void WhenValidateVehiclePhotoWithPhotoThatIsTooLargeAndFakeContentLength_ThenReturnsValidationError()
        {
            var stream = _assembly.GetManifestResourceStream("MileageStats.Domain.Tests.TestContent.FileTooBig.jpg");
            var contentLength = 990;
            var contentType = "/image/png";

            var handler = new CanAddPhoto();
            var result = handler.Execute(stream, contentLength, contentType);

            Assert.Equal(1, result.Count());
            Assert.Contains("must be less than", result.First().Message, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}