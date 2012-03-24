using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Web.Mvc;
using Moq;
using MileageStats.Domain.Contracts;

namespace MileageStats.Web.Tests.Helpers
{
    public class ControllerExtensionsFixture
    {
        public class ControllerTest : Controller
        {
        }

        [Fact]
        public void WhenAddingValidatingResultsWithMember_ThenControllerShouldHaveModelErrors()
        {
            var controller = new ControllerTest();

            ControllerExtensions.AddModelErrors(controller, new List<ValidationResult> {
                new ValidationResult("member", "msg")
            });

            Assert.True(controller.ModelState["member"].Errors.Count == 1);
            Assert.True(controller.ModelState["member"].Errors[0].ErrorMessage == "msg");
        }

        [Fact]
        public void WhenAddingValidatingResultsWithNoMember_ThenControllerShouldHaveModelErrors()
        {
            var controller = new ControllerTest();

            ControllerExtensions.AddModelErrors(controller, new List<ValidationResult> {
                new ValidationResult("msg")
            });

            Assert.True(controller.ModelState[String.Empty].Errors.Count == 1);
            Assert.True(controller.ModelState[String.Empty].Errors[0].ErrorMessage == "msg");
        }

        [Fact]
        public void WhenAddingValidatingResultsWithNoMemberAndDefaultKey_ThenControllerShouldHaveModelErrors()
        {
            var controller = new ControllerTest();

            ControllerExtensions.AddModelErrors(controller, new List<ValidationResult> {
                new ValidationResult("msg")
            }, "test");

            Assert.True(controller.ModelState["test"].Errors.Count == 1);
            Assert.True(controller.ModelState["test"].Errors[0].ErrorMessage == "msg");
        }
    }
}
