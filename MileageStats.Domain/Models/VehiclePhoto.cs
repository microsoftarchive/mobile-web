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

using System.ComponentModel.DataAnnotations;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Properties;

namespace MileageStats.Domain.Models
{
    public class VehiclePhoto : IHasIdentity
    {
        public int Id { get { return VehiclePhotoId; } set { VehiclePhotoId = value; } }

        /// <summary>
        /// Gets or sets the entity ID of vehicle photo.
        /// </summary>
        /// <value>
        /// An integer identifying the entity.
        /// </value>
        public int VehiclePhotoId { get; set; }

        /// <summary>
        /// Gets or sets the entity ID of vehicle the photo is for.
        /// </summary>
        /// <value>
        /// An integer identifying the entity.
        /// </value>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the set of bytes that is the image.
        /// </summary>
        /// <value>
        /// An array of bytes.
        /// </value>
        [Required(ErrorMessageResourceName = "VehiclePhotoImageRequired", ErrorMessageResourceType = typeof(Resources))]
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the image (e.g. image/bmp, image/gif, image/jpeg, or image/png)
        /// </summary>
        /// <value>
        /// A string.       
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "VehiclePhotoImageMimeTypeRequired", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(100)] //The longest MIME types I found was 73 characters
            public string ImageMimeType { get; set; }
    }
}