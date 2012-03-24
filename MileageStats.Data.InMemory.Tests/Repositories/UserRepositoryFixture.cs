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
using System.Linq;
using MileageStats.Domain.Models;
using Xunit;

namespace MileageStats.Data.InMemory.Tests.Repositories
{
    public class UserRepositoryFixture
    {
        [Fact]
        public void WhenRequestingAvailableUserByAuthenticatedId_ThenReturnsUserFromRepository()
        {
            var userData = new User
                                {
                                    AuthorizationId = "TestId",
                                    DisplayName = "TestDisplayName",
                                };

            var userRepository = new UserRepository();
            userRepository.Create(userData);

            var retrievedUser = userRepository.GetByAuthenticatedId(userData.AuthorizationId);

            Assert.NotNull(retrievedUser);
        }

        [Fact]
        public void WhenAddingUser_ThenUserPersists()
        {
            var userRepository = new UserRepository();

            var newUser = new User
                              {
                                  AuthorizationId = "AnAuthorizationId",
                                  DisplayName = "TheDisplayName",
                              };

            userRepository.Create(newUser);

            Assert.NotNull(userRepository.Set
                .Where(u => u.AuthorizationId == newUser.AuthorizationId).First());
        }

        [Fact]
        public void WhenAddingUser_ThenUserReturnsPopulatedNewUser()
        {
            var userRepository = new UserRepository();

            const string authorizationId = "AnAuthorizationId";
            const string displayName = "TheDisplayName";
            var newUser = new User
                              {
                                  AuthorizationId = authorizationId,
                                  DisplayName = displayName,
                              };

            userRepository.Create(newUser);

            Assert.NotNull(newUser);
            Assert.Equal(authorizationId, newUser.AuthorizationId);
            Assert.Equal(displayName, newUser.DisplayName);
        }

        [Fact]
        public void WhenAddingUser_ThenUserReceivesNonZeroId()
        {
            var userRepository = new UserRepository();

            var newUser = new User
                              {
                                  AuthorizationId = "AnAuthorizationId",
                                  DisplayName = "TheDisplayName",
                              };

            Assert.Equal(0, newUser.UserId);
            userRepository.Create(newUser);

            Assert.True(newUser.UserId > 0, "User did not receive non-zero UserId when persisted.");
        }
    }
}