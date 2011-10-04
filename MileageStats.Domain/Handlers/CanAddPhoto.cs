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
using System.Drawing;
using System.IO;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Handlers
{
    public class CanAddPhoto
    {
        private const int MaxPhotoSizeBytes = 1048576; //1024*1024 = 1 MB

        public virtual IEnumerable<ValidationResult> Execute(Stream stream, int contentLength, string contentType)
        {
            if (stream == null)
            {
                yield return new ValidationResult(Resources.InvalidVehiclePhoto);
            }
            else if (contentLength > MaxPhotoSizeBytes)
            {
                // check ContentLength first, even though it could be deliberately set incorrectly.
                yield return new ValidationResult(Resources.VehiclePhotoTooLarge);
            }
            else if (stream.Length > MaxPhotoSizeBytes)
            {
                // recheck the actual buffer since I cannot trust ContentLength
                yield return new ValidationResult(Resources.VehiclePhotoTooLarge);
            } else
            {
                bool isValidFormat;
                try
                {
                    // load the stream as an Image to verify it is valid.
                    Image.FromStream(stream);
                    isValidFormat = true;
                }
                catch
                {
                    isValidFormat = false;
                }

                if (!isValidFormat) yield return new ValidationResult(Resources.InvalidVehiclePhoto);
            }
        }
    }
}