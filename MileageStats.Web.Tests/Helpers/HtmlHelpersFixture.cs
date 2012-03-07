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
using System.Globalization;
using System.Linq;
using Xunit;
using MileageStats.Web.Helpers;

namespace MileageStats.Web.Tests.Helpers
{
    public class HtmlHelpersFixture
    {
        [Fact]
        public void WhenSelectListForYearConstructed_ThenListHas3Values()
        {
            var list = SelectListFor.Year(i => i == 0);

            Assert.Equal(3, list.Count());
        }

        [Fact]
        public void WhenSelectListForYearConstructed_ThenStarts3YearsPrevious()
        {
            var current = DateTime.Now.Year;
            var list = SelectListFor.Year(i => i == 0);

            Assert.Equal( (current-2).ToString(CultureInfo.InvariantCulture), list.First().Value);
        }

        [Fact]
        public void WhenSelectListForYearConstructed_ThenMarksSelectedValue()
        {
            var current = DateTime.Now.Year;
            var list = SelectListFor.Year(i => i == current);

            Assert.True(list.First(x => x.Value == current.ToString(CultureInfo.InvariantCulture)).Selected);
        }

        [Fact]
        public void WhenSelectListForYearConstructed_ThenDoesNotMarksUnselectedValue()
        {
            var current = DateTime.Now.Year;
            var list = SelectListFor.Year(i => i == current);

            var hasSelected =list
                .Where(x => x.Value != current.ToString(CultureInfo.InvariantCulture))
                .Any(x => x.Selected);

            Assert.False(hasSelected);
        }

        [Fact]
        public void WhenSelectListForMonthConstructed_ThenHas12Values()
        {
            var list = SelectListFor.Month(i => i == 0);
            Assert.Equal(12, list.Count());
        }

        [Fact]
        public void WhenSelectListForDatesConstructed_ThenHas31Values()
        {
            var list = SelectListFor.Date(i => i == 0);
            Assert.Equal(31, list.Count());
        }
    }
}
