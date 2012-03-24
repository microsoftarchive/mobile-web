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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MileageStats.Web.Models;
using Xunit;

namespace MileageStats.Web.Tests
{
    public class MileageStatsIdentityFixture
    {
        [Fact]
        public void WhenSerialized_ThenCanBeDeSerialized()
        {
            var formatter = new BinaryFormatter();
            var identity = new MileageStatsIdentity("Name", "DisplayName", 1);
            MileageStatsIdentity recoveredIdentity = null;

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, identity);
                stream.Seek(0, SeekOrigin.Begin);
                recoveredIdentity = (MileageStatsIdentity) formatter.Deserialize(stream);
            }

            Assert.NotNull(recoveredIdentity);
            Assert.Equal(identity.Name, recoveredIdentity.Name);
            Assert.Equal(identity.DisplayName, recoveredIdentity.DisplayName);
            Assert.Equal(identity.UserId, recoveredIdentity.UserId);
        }
    }
}