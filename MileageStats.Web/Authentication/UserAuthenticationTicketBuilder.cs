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
using System.Web.Security;
using MileageStats.Domain.Models;
using System.Web;

namespace MileageStats.Web.Authentication
{
    public class UserAuthenticationTicketBuilder
    {
        /// <summary>
        /// Creates a new <see cref="FormsAuthenticationTicket"/> from a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="issueDate"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        /// <remarks>
        /// Encodes the <see cref="UserInfo"/> into the <see cref="FormsAuthenticationTicket.UserData"/> property
        /// of the authentication ticket.  This can be recovered by using the <see cref="UserInfo.FromString"/> method.
        /// </remarks>
        public static FormsAuthenticationTicket CreateAuthenticationTicket(User user, DateTime issueDate, bool isPersistent)
        {
            UserInfo userInfo = CreateUserContextFromUser(user);

            var ticket = new FormsAuthenticationTicket(
                1,
                user.AuthorizationId,
                issueDate,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                isPersistent,
                userInfo.ToString());

            return ticket;
        }

        private static UserInfo CreateUserContextFromUser(User user)
        {
            var userContext = new UserInfo
            {
                UserId = user.UserId,
                DisplayName = user.DisplayName,
                ClaimsIdentifier = user.AuthorizationId
            };

            return userContext;
        }
    }
}