using System;
using System.Diagnostics;

namespace Xi
{
    /// <summary>
    /// Implements the standard .NET disposal pattern. Has a warning mechanism for objects that
    /// were not manually disposed in debug mode.
    /// </summary>
    public class Disposable : IDisposable
    {
        /// <summary>
        /// Handle forgotten disposal.
        /// </summary>
        ~Disposable()
        {
            TraceFailToDisposeManually();
            ForwardDispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ForwardDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing) { }

#if DEBUG
        private void ForwardDispose(bool dispose)
        {
            Trace.Assert(!disposed, "Object disposed redundantly.");
            Dispose(dispose);
            disposed = true;
        }
#else
        private void ForwardDispose(bool dispose)
        {
            Dispose(dispose);
        }
#endif

        private static void TraceFailToDisposeManually()
        {
            Trace.Fail(
                "Finalizer called.",
                "It appears that you forgot to call Dispose() on this object.");
        }

#if DEBUG
        private bool disposed;
#endif
    }
}
