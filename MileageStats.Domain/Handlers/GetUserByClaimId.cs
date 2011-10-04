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
using MileageStats.Domain.Models;

namespace MileageStats.Domain.Handlers
{
    public class GetUserByClaimId
    {
        private readonly IUserRepository _userRepository;

        public GetUserByClaimId(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public virtual User Execute(string claimedIdentifier)
        {
            var user = _userRepository.GetByAuthenticatedId(claimedIdentifier);
            
            if (user == null)
                return null;

            return new User
            {
                AuthorizationId = user.AuthorizationId,
                Country = user.Country,
                DisplayName = user.DisplayName,
                HasRegistered = user.HasRegistered,
                UserId = user.UserId
            };
        }

    }
}
