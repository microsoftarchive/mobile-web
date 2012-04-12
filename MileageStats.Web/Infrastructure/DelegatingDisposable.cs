using System;

namespace MileageStats.Web.Infrastructure
{

    // A helper class that allows you to specify an
    // action to be invoked when the instance is disposed.
    // This is meant to be used in HTML helpers in 
    // combination with the `using` keyword.
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