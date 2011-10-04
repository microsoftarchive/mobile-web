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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MileageStats.Web.MobileProfiler.ClientProfile.Model;

namespace MileageStats.Web.MobileProfiler.ClientProfile
{
    public static class XmlProfileManifestParser
    {
        public static ProfileManifest Parse(XmlReader reader)
        {
            var root = XElement.Load(reader);

            var profiles = root.DescendantsAndSelf("profile")
                .Select(profile => new ProfileManifest
                {
                    Id = profile.Attribute("id").Value,
                    Title = profile.Attribute("title").Value,
                    Version = profile.Attribute("version").Value,
                    Features = profile.Descendants("feature")
                        .Select(feature => new Feature
                        {
                            Id = feature.Attribute("id").Value,
                            Name = feature.Element("name").Value,
                            Value = feature.Attribute("default").Value,
                            Property = feature.Attribute("property").Value,
                            Test = (feature.Element("test") != null) ? feature.Element("test").Value : null
                        }).ToArray()
                });

            return profiles.FirstOrDefault();
        }
    }
}
