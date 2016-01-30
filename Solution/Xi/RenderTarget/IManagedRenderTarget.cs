using System;

namespace Xi
{
    /// <summary>
    /// Represents a managed render target.
    /// </summary>
    public interface IManagedRenderTarget : IDisposable
    {
        /// <summary>
        /// Activate the drawing surface.
        /// </summary>
        void Activate();

        /// <summary>
        /// Resolve the drawing surface.
        /// </summary>
        void Resolve();
    }
}
