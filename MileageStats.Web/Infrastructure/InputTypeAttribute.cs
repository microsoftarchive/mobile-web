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
using System.Web;

namespace MileageStats.Web.Infrastructure
{
    /// <summary>
    /// An attribute for adding Html5 input type semmantics to an existing model
    /// property. 
    /// </summary>
    public class InputTypeAttribute : Attribute
    {
        public InputTypeAttribute(string type, string length)
        {
            this.Type = type;
            this.Length = length;
        }

        public InputTypeAttribute(string type, string length, string step, string placeHolder)
        {
            this.Type = type;
            this.Step = step;
            this.PlaceHolder = placeHolder;
            this.Length = length;
        }

        public string Type { get; set; }
        public string Step { get; set; }
        public string PlaceHolder { get; set; }
        public string Length { get; set; }
    }
}