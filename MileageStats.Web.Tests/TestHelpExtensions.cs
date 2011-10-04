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
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using Moq;

namespace MileageStats.Web.Tests
{
    public static class TestHelpExtensions
    {
        public static T Extract<T>(this ActionResult result)
        {
            var partialViewResult = result as PartialViewResult; 
            if (partialViewResult != null)
            {
                return (T)partialViewResult.Model;
            }

            var contentTypeAware = result as ContentTypeAwareResult;
            if (contentTypeAware != null)
            {
                return (T)contentTypeAware.Model;
            }

            var viewResult = (ViewResult)result;
            return (T)viewResult.Model;
        }

        public static Mock<T> MockHandlerFor<T>(this Mock<IServiceLocator> serviceLocator, Func<Mock<T>> create, Action<Mock<T>> setup = null) where T : class
        {
            var mock = create();

            if (setup != null) setup(mock);

            serviceLocator
                .Setup(s => s.GetInstance<T>())
                .Returns(mock.Object);

            return mock;

        }

        public static void StandardSetup(this Mock<GetVehicleListForUser> mock, int userId, int vehicleId, int selectedVehicledId = 0)
        {
            mock.Setup(h => h
                .Execute(userId))
                .Returns(vehicleId.StandardVehicleList(selectedVehicledId));
        }

        public static IEnumerable<VehicleModel> StandardVehicleList(this int startingVehicleId, int selectedVehicledId = 0)
        {
            return new[]
                        {
                            new VehicleModel(new Vehicle{VehicleId = startingVehicleId}, new VehicleStatisticsModel()),  
                            new VehicleModel(new Vehicle{VehicleId = startingVehicleId + 1}, new VehicleStatisticsModel()),  
                            new VehicleModel(new Vehicle{VehicleId = startingVehicleId + 2}, new VehicleStatisticsModel()),  
                        };
        }

        public static Mock<T> Mock<T>() where T : class
        {
            var typeToMock = typeof(T);
            var mockType = typeof(Mock<>).MakeGenericType(typeToMock);

            var ctrs = from ctr in mockType.GetConstructors()
                       let parms = ctr.GetParameters()
                       where parms.Count() == 1 && parms.First().ParameterType == typeof(Object[])
                       select ctr;

            // how many parameters in the constructor?
            var c = typeToMock.GetConstructors().First().GetParameters().Count();
            var args = new object[c];

            var mock = ctrs.First().Invoke(new[] { args });
            return (Mock<T>)mock;
        }
    }
}
