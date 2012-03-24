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

namespace MileageStats.Web.UnityExtensions
{
    /// <summary>
    /// The UnityHttpContextPerRequestLifetimeManager exists solely to make it easier
    /// to configure the per-request lifetime manager in a configuration file.
    /// </summary>
    /// <remarks>
    /// An alternative approach would be to use a type converter to convert the 
    /// configuration string and new up a <see cref="UnityPerRequestLifetimeManager"/>
    /// from this type converter.
    /// </remarks>
    public class UnityHttpContextPerRequestLifetimeManager : UnityPerRequestLifetimeManager
    {
        public UnityHttpContextPerRequestLifetimeManager() : base(new HttpContextPerRequestStore())
        {
        }
    }
}