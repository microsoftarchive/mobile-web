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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Handlers;
using Moq;
using Xunit;
using MileageStats.Domain.Models;

namespace MileageStats.Domain.Tests
{
    public class WhenUpdatingUser
    {
        private readonly Mock<IUserRepository> _userRepo;

        public WhenUpdatingUser()
        {
            _userRepo = new Mock<IUserRepository>();
        }

        [Fact]
        public void InvokesUserRepository()
        {
            var user = new User
            {
                AuthorizationId = "id",
                Country = "country",
                DisplayName = "displayName",
                HasRegistered = true,
                UserId = 1
            };

            var handler = new UpdateUser(_userRepo.Object);
            handler.Execute(user);

            _userRepo
                .Verify(r => r.Update(It.Is<User>(u => 
                    u.AuthorizationId == user.AuthorizationId &&
                    u.Country == user.Country &&
                    u.DisplayName == user.DisplayName &&
                    u.HasRegistered == user.HasRegistered &&
                    u.UserId == user.UserId)), Times.Once());
        }
    }
}