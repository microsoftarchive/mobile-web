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
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Properties;
using MileageStats.Domain.Models;

namespace MileageStats.Domain.Handlers
{
    public class CreateUser
    {
        private readonly IUserRepository _userRepository;

        public CreateUser(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public virtual User Execute(string authorizationId)
        {
            if (string.IsNullOrEmpty(authorizationId))
                throw new ArgumentNullException(authorizationId);

            var user = new User 
            { 
                AuthorizationId = authorizationId,
            };

            this._userRepository.Create(user);

            return new User
                {
                    UserId = user.UserId,
                    Country = user.Country,
                    HasRegistered = user.HasRegistered,
                    AuthorizationId = user.AuthorizationId,
                    DisplayName = user.DisplayName
                };
        }
    }
}
