using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MileageStats.Web.ClientProfile;
using MileageStats.Web.ClientProfile.Model;
using Xunit;
using Moq;
using MileageStats.Web.Controllers;
using MileageStats.Web.Tests.Mocks;
using System.Web;
using System.Web.Mvc;

namespace MileageStats.Web.Tests.Controllers
{
    public class MobileProfileControllerFixture
    {
        private readonly Mock<IProfileManifestRepository> _profileManifestRepository;
        private readonly Mock<IProfileCookieEncoder> _profileCookieEncoder;

        public MobileProfileControllerFixture()
        {
            this._profileManifestRepository = new Mock<IProfileManifestRepository>();
            this._profileCookieEncoder = new Mock<IProfileCookieEncoder>();
        }

        [Fact]
        public void WhenRequestingMobileProfileWithNoExistingCookie_ThenJavascriptIsReturned()
        {
            _profileManifestRepository.Setup(p => p.GetProfile("generic"))
               .Returns(new ProfileManifest
               {
                   Id = "Generic-1.0",
                   Title = "Generic",
                   Features = new Feature[] { },
                   Version = "1.0"
               });

            var controller = GetTestableMobileProfileController();

            var cookies = new HttpCookieCollection();
            controller.Request.SetHttpCookies(cookies);

            var response = Mock.Get(controller.Response);

            var result = controller.ProfileScript();

            Assert.IsType<PartialViewResult>(result);
            response.VerifySet((r) => { r.ContentType = "text/javascript"; });
        }

        [Fact]
        public void WhenRequestingMobileProfile_ThenJavascriptIsReturned()
        {
            _profileManifestRepository.Setup(p => p.GetProfile("generic"))
                .Returns(new ProfileManifest
                {
                    Id = "Generic-1.0",
                    Title = "Generic",
                    Features = new Feature[] {},
                    Version = "1.0"
                });

            _profileCookieEncoder.Setup(p => p.GetDeviceCapabilities(It.IsAny<HttpCookie>()))
                .Returns(new Dictionary<string, string>{ {"id", "generic-1.0"} });
            
            var controller = GetTestableMobileProfileController();

            var response = Mock.Get(controller.Response);
            
            var cookies = new HttpCookieCollection();
            cookies.Add(new System.Web.HttpCookie("profile"));
            controller.Request.SetHttpCookies(cookies);
            
            var result = controller.ProfileScript();

            Assert.IsType<PartialViewResult>(result);
            response.VerifySet((r) => { r.ContentType = "text/javascript"; });
        }

        [Fact]
        public void WhenRequestingMobileProfileAndVersionDoesNotMatch_ThenCookieIsRemoved()
        {
            _profileManifestRepository.Setup(p => p.GetProfile("generic"))
                .Returns(new ProfileManifest
                {
                    Id = "Generic-1.0",
                    Title = "Generic",
                    Features = new Feature[] { },
                    Version = "2.0"
                });

            _profileCookieEncoder.Setup(p => p.GetDeviceCapabilities(It.IsAny<HttpCookie>()))
                .Returns(new Dictionary<string, string> { { "id", "generic-1.0" } });

            var controller = GetTestableMobileProfileController();

            var response = Mock.Get(controller.Response);

            var cookies = new HttpCookieCollection();
            cookies.Add(new System.Web.HttpCookie("profile"));
            controller.Request.SetHttpCookies(cookies);

            var result = controller.ProfileScript();

            Assert.True(cookies.Count == 0);
        }

        private MobileProfileController GetTestableMobileProfileController()
        {
            var controller = new MobileProfileController(_profileManifestRepository.Object, _profileCookieEncoder.Object);
            controller.SetFakeControllerContext();
            
            return controller;
        }
    }
}
