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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moq;
using Xunit;
using MileageStats.Web.Helpers;

namespace MileageStats.Web.Tests.Helpers
{
    public class SampleModel
    {
        public string SomeValue { get; set; }
    }

    public class MustacheHelpersFixture
    {
        [Fact]
        public void ShouldRenderAMustacheTemplateWhenNoModelIsPresent()
        {
            //trigger the mustache rendering by setting the model to null
            var helper = MockHelperWithModel(null);

            var selectList = new SelectList(new List<string>());
            var actual = helper.DropDownListFor(model => model.SomeValue, selectList);
           
            const string property = "SomeValue";
            const string option = "<option value=\"{{value}}\" {{#selected}}selected{{/selected}}>{{text}}</option>";
            const string optionSection = "{{#" + property + "}}" + option + "{{/"+ property + "}}";
            var select = string.Format("<select id=\"{0}\" name=\"{0}\">{1}{2}</select>", property, optionSection, Environment.NewLine);
            var expected = MvcHtmlString.Create(select);

            Assert.Equal(expected.ToHtmlString(), actual.ToHtmlString());
        }

        [Fact]
        public void ShouldRenderAMustacheTemplateWithHtmlAttributes()
        {
            //trigger the mustache rendering by setting the model to null
            var helper = MockHelperWithModel(null);

            var selectList = new SelectList(new List<string>());
            var actual = helper.DropDownListFor(model => model.SomeValue, selectList, "ignored", new { attr ="test"} );

            const string property = "SomeValue";
            const string option = "<option value=\"{{value}}\" {{#selected}}selected{{/selected}}>{{text}}</option>";
            const string optionSection = "{{#" + property + "}}" + option + "{{/" + property + "}}";
            var select = string.Format("<select attr=\"test\" id=\"{0}\" name=\"{0}\">{1}{2}</select>", property, optionSection, Environment.NewLine);
            var expected = MvcHtmlString.Create(select);

            Assert.Equal(expected.ToHtmlString(), actual.ToHtmlString());
        }

        [Fact]
        public void ShouldRenderIgnoreTheOptionLabelWhenRenderingForMustache()
        {
            //trigger the mustache rendering by setting the model to null
            var helper = MockHelperWithModel(null);

            var selectList = new SelectList(new List<string>());
            var actual = helper.DropDownListFor(model => model.SomeValue, selectList, "an option label");

            const string property = "SomeValue";
            const string option = "<option value=\"{{value}}\" {{#selected}}selected{{/selected}}>{{text}}</option>";
            const string optionSection = "{{#" + property + "}}" + option + "{{/" + property + "}}";
            var select = string.Format("<select id=\"{0}\" name=\"{0}\">{1}{2}</select>", property, optionSection, Environment.NewLine);
            var expected = MvcHtmlString.Create(select);

            Assert.Equal(expected.ToHtmlString(), actual.ToHtmlString());
        }

        [Fact]
        public void ShouldRenderWithStandardHelperWhenModelIsPresent()
        {
            var helper = MockHelperWithModel(new SampleModel());

            var selectList = new SelectList(new List<string>{ "apple", "orange" });
            var actual = helper.DropDownListFor(model => model.SomeValue, selectList);

            const string property = "SomeValue";
            const string option1 = "<option>apple</option>";
            const string option2 = "<option>orange</option>";
            var select = string.Format("<select id=\"{0}\" name=\"{0}\">{1}{3}{2}{3}</select>", property, option1, option2, Environment.NewLine);
            var expected = MvcHtmlString.Create(select);

            Assert.Equal(expected.ToHtmlString(), actual.ToHtmlString());
        }

        private MustacheHelper<SampleModel> MockHelperWithModel(SampleModel model)
        {
            var viewData = new ViewDataDictionary();
            if (model != null) viewData.Model = model;
            var viewDataContainer = MockViewDataContainer(viewData);
            var viewContext = MockViewContext(viewData);
            var internalHelper = new HtmlHelper<SampleModel>(viewContext.Object, viewDataContainer.Object);
            return new MustacheHelper<SampleModel>(internalHelper);
        }

        private static Mock<ViewContext> MockViewContext(ViewDataDictionary viewData)
        {
            var mock = new Mock<ViewContext>();
            mock.SetupGet(x => x.ViewData).Returns(viewData);
            return mock;
        }

        private static Mock<IViewDataContainer> MockViewDataContainer(ViewDataDictionary viewData)
        {
            var mock = new Mock<IViewDataContainer>();
            mock.SetupGet(x => x.ViewData).Returns(viewData);
            return mock;
        }

    }
}
