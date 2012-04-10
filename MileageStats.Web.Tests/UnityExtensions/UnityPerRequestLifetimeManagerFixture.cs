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
using MileageStats.Web.UnityExtensions;
using Moq;
using Xunit;

namespace MileageStats.Web.Tests.UnityExtensions
{
    public class UnityPerRequestLifetimeManagerFixture
    {
        [Fact]
        public void WhenSetValueCalled_ThenStoredInHttpItemsContex()
        {
            var newValue = new object();
            var storeMock = new Mock<IPerRequestStore>();
            storeMock.Setup(s => s.SetValue(It.IsAny<object>(), newValue)).Verifiable();

            var lifetimeManager = new UnityPerRequestLifetimeManager(storeMock.Object);

            lifetimeManager.SetValue(newValue);

            storeMock.Verify();
        }

        [Fact]
        public void WhenGetValueCalled_ThenRetrievedFromHttpItemsContext()
        {
            var newValue = new object();
            var storeMock = new Mock<IPerRequestStore>();
            storeMock.Setup(s => s.GetValue(It.IsAny<object>())).Returns(newValue).Verifiable();

            var lifetimeManager = new UnityPerRequestLifetimeManager(storeMock.Object);

            lifetimeManager.SetValue(newValue);
            var returnedValue = lifetimeManager.GetValue();

            storeMock.Verify();
        }

        [Fact]
        public void WhenValueRemoved_ThenRemovedFromContext()
        {
            var newValue = new object();
            var storeMock = new Mock<IPerRequestStore>();
            storeMock.Setup(s => s.GetValue(It.IsAny<object>())).Returns(newValue);
            storeMock.Setup(s => s.RemoveValue(It.IsAny<object>())).Verifiable();

            var lifetimeManager = new UnityPerRequestLifetimeManager(storeMock.Object);

            lifetimeManager.SetValue(newValue);
            lifetimeManager.RemoveValue();

            storeMock.Verify();
        }

        [Fact]
        public void WhenRemovingDisposableValues_ThenDisposeInvoked()
        {
            var valueMock = new Mock<IDisposable>();
            valueMock.Setup(x => x.Dispose()).Verifiable();

            var storeMock = new Mock<IPerRequestStore>();
            storeMock.Setup(s => s.GetValue(It.IsAny<object>())).Returns(valueMock.Object);
            storeMock.Setup(s => s.RemoveValue(It.IsAny<object>())).Verifiable();

            var lifetimeManager = new UnityPerRequestLifetimeManager(storeMock.Object);

            lifetimeManager.SetValue(valueMock.Object);
            lifetimeManager.RemoveValue();

            valueMock.Verify();
        }

        [Fact]
        public void WhenDisposingLifetimeManager_DisposesValue()
        {
            var valueMock = new Mock<IDisposable>();
            valueMock.Setup(x => x.Dispose()).Verifiable();

            var storeMock = new Mock<IPerRequestStore>();
            storeMock.Setup(s => s.GetValue(It.IsAny<object>())).Returns(valueMock.Object);
            storeMock.Setup(s => s.RemoveValue(It.IsAny<object>())).Verifiable();

            var lifetimeManager = new UnityPerRequestLifetimeManager(storeMock.Object);

            lifetimeManager.SetValue(valueMock.Object);

            ((IDisposable) lifetimeManager).Dispose();

            valueMock.Verify();
        }

        [Fact]
        public void WhenRequestEnds_ThenRemovedFromContext()
        {
            var newValue = new object();
            var storeMock = new Mock<IPerRequestStore>();
            storeMock.Setup(s => s.GetValue(It.IsAny<object>())).Returns(newValue);
            storeMock.Setup(s => s.RemoveValue(It.IsAny<Object>())).Verifiable();

            var lifetimeManager = new UnityPerRequestLifetimeManager(storeMock.Object);

            lifetimeManager.SetValue(newValue);

            storeMock.Raise(a => a.EndRequest += null, EventArgs.Empty);

            storeMock.Verify();
        }
    }
}