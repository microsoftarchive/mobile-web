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
using System.Web;
using System.Web.Security;

namespace MileageStats.Web.Authentication
{
    public interface IFormsAuthentication
    {
        /// <summary>
        /// Forces signout from the authorization system.
        /// </summary>
        void Signout();

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// Decrypts a ticket from a string and returns a <see cref="FormsAuthenticationTicket"/>.
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        FormsAuthenticationTicket Decrypt(string encryptedTicket);

        FormsAuthenticationTicket GetAuthenticationTicket(HttpContextBase httpContext);
    }
}