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
using Xunit;
using Moq;
using MileageStats.Domain.Contracts;
using MileageStats.Web.Controllers;
using System.Web.Mvc;

namespace MileageStats.Web.Tests.Controllers
{
    public class GeoLocationControllerFixture
    {
        private readonly Mock<IMapService> _mapService;

        public GeoLocationControllerFixture()
        {
            this._mapService = new Mock<IMapService>();
        }

        [Fact]
        public void WhenFillupStationLocationsRequested_ThenMapServiceIsInvoked()
        {
            this._mapService.Setup(m => m.SearchKeywordLocation("Gas Stations", 1000, 2000))
                .Returns(new string[] { "addr1", "addr2" });
            
            var controller = new GeoLocationController(this._mapService.Object);
            var result = controller.GetFillupStations(1000, 2000);

            Assert.IsType<JsonResult>(result);
            Assert.True(((IEnumerable<string>)((JsonResult)result).Data).Count() == 2);
        }
    }
}
