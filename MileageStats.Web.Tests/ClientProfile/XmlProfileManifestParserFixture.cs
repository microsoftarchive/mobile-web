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

using System.Linq;
using MileageStats.Web.ClientProfile;
using MileageStats.Web.ClientProfile.Model;
using Xunit;
using System.Xml;
using System.IO;

namespace MileageStats.Web.Tests.ClientProfile
{
    public class XmlProfileManifestParserFixture
    {
        [Fact]
        public void WhenParsingValidXmlDocument_ThenProfileInstanceIsReturned()
        {
            ProfileManifest model = null;

            using (var sr = new StreamReader("ClientProfile\\Generic.xml"))
            using (var reader = XmlReader.Create(sr))
            {
                model = XmlProfileManifestParser.Parse(reader);
            }

            Assert.NotNull(model);
            Assert.Equal("Generic", model.Title);
            Assert.Equal("generic", model.Id);
            Assert.Equal("1.1", model.Version);
            Assert.True(model.Features.Any(f => f.Name == "JSON"));

        }
    }
}
