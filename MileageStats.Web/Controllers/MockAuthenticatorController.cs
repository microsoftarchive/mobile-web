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

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MileageStats.Web.Controllers
{
#if DEBUG
    /// <summary>
    /// A mock authenticator to support an offline experience for the app.
    /// </summary>
    public class MockAuthenticatorController : Controller
    {
        //
        // GET: /MockAutenticator/

        public ActionResult Index(string providerUrl, string returnUrl)
        {
            return this.View(new MockAuthenticationViewModel()
                                 {
                                     ReturnUrl = returnUrl,
                                     ProviderUrl = providerUrl,
                                     claimed_identifier = "http://oturner.myidprovider.org/"                                     
                                 });
        }
    }

    public class MockAuthenticationViewModel
    {
        public string ReturnUrl { get; set; }
        
        public string ProviderUrl { get; set; }

        [Display(Name = "Claimed Identifier")]
        public string claimed_identifier { get; set; }        
    }
#endif
}