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
using System.Collections.Generic;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using MileageStats.Web.Controllers;
using Moq;
using Xunit;
using MileageStats.Domain.Models;

namespace MileageStats.Domain.Tests
{
    public class WhenGettingUser
    {
        private readonly Mock<IUserRepository> _userRepo;

        public WhenGettingUser()
        {
            _userRepo = new Mock<IUserRepository>();
        }

        [Fact]
        public void ThenUserReturned()
        {
            var user = new User { UserId = 1, DisplayName = "a friendly name" };

            _userRepo
                .Setup(ur => ur.GetByAuthenticatedId("1"))
                .Returns(new User { UserId = 1, DisplayName = "a friendly name" });

            var handler = new GetUserByClaimId(_userRepo.Object);

            var retrievedUser = handler.Execute("1");

            Assert.NotNull(retrievedUser);
            Assert.Equal(user.UserId, retrievedUser.UserId);
        }

        [Fact]
        public void ForOtherClaimIdentifier_ThenNullReturned()
        {
            var handler = new GetUserByClaimId(_userRepo.Object);

            var retrievedUser = handler.Execute("1");

            Assert.Null(retrievedUser);
        }
    }
}