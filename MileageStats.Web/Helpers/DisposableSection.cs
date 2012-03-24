using System;
using System.Web.Mvc;

namespace MileageStats.Web.Helpers
{
    public class DisposableSection : IDisposable
    {
        private readonly ViewContext _viewContext;
        private readonly string _sectionName;
        private readonly bool _isRenderingMustache;

        public DisposableSection(ViewContext viewContext, string sectionName, bool isRenderingMustache)
        {
            _viewContext = viewContext;
            _sectionName = sectionName;
            this._isRenderingMustache = isRenderingMustache;
            if (string.IsNullOrEmpty(_sectionName) || !_isRenderingMustache) return;

            string beginSection = string.Format("{{{{#{0}}}}}", _sectionName);
            _viewContext.Writer.WriteLine(beginSection);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (string.IsNullOrEmpty(_sectionName) || !_isRenderingMustache) return;

                string endSection = string.Format("{{{{/{0}}}}}", _sectionName);
                _viewContext.Writer.WriteLine(endSection);
            }
        }

        
    }
}