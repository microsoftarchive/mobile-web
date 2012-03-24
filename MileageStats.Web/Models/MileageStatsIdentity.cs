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
using System.Security.Principal;
using System.Web.Security;
using MileageStats.Domain.Models;

namespace MileageStats.Web.Models
{
    // Note: On a development server (Cassini) you may get a serializationexception
    // with custom identities.  
    //
    // See http://connect.microsoft.com/VisualStudio/feedback/details/274696/using-custom-identities-in-asp-net-fails-when-using-the-asp-net-developement-server
    // for more information.
    [Serializable]
    public class MileageStatsIdentity : IIdentity
    {
        public MileageStatsIdentity(string name, string displayName, int userId)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.UserId = userId;
        }

        public MileageStatsIdentity(string name, UserInfo userInfo)
            : this(name, userInfo.DisplayName, userInfo.UserId)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");
            this.UserId = userInfo.UserId;
        }

        public MileageStatsIdentity(FormsAuthenticationTicket ticket)
            : this(ticket.Name, UserInfo.FromString(ticket.UserData))
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
        }

        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "MileageStats"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string DisplayName { get; private set; }

        public int UserId { get; private set; }
    }
}