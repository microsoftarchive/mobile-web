using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using MileageStats.Domain.Models;

namespace MileageStats.Web
{
    public static class MustacheModel<T>
    {
        private class mush: DynamicObject
        {
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                return base.TryConvert(binder, out result);
            }
        }

        public static T Model()
        {
            dynamic obj = new mush();

            return obj;
        }
    }

    public class UserMustache :User
    {
        public new string DisplayName
        {
            get { return "{{DisplayName}}"; }
        }
    }
}