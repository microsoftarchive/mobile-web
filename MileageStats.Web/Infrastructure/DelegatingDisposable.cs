using System;

namespace MileageStats.Web.Infrastructure
{

    // a helper class that allows you to specify an
    // action to be invoked when the instance is disposed
    // this is meant to be used in html helpers in 
    // combination with the `using` keyword
    public sealed class DelegatingDisposable : IDisposable
    {
        private readonly Action _whenDisposed;

        public DelegatingDisposable(Action whenDisposed)
        {
            _whenDisposed = whenDisposed;
        }

        public void Dispose()
        {
            _whenDisposed();
        }
    }
}