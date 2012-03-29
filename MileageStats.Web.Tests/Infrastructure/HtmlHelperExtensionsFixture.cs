using System.Web.Mvc;
using System.Web.Mvc.Html;
using Moq;
using Xunit;

namespace MileageStats.Web.Tests.Infrastructure
{
    public class HtmlHelperExtensionsFixture
    {
        [Fact]
        public void WhenEnablingUnobtrusiveValidation_ThenSetsFormContext()
        {
            var viewContext = new Mock<ViewContext>();
            viewContext
                .SetupSet(x => x.FormContext = It.IsAny<FormContext>())
                .Verifiable();

            var viewDataContainer = new Mock<IViewDataContainer>();
            viewDataContainer.SetupGet(x => x.ViewData).Returns(new ViewDataDictionary());

            var helper = new HtmlHelper<object>(viewContext.Object, viewDataContainer.Object);

            helper.EnableUnobtrusiveValidation();
        }
    }
}