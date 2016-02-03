using System;
using BEPUphysics.Entities;

namespace Xi
{
    /// <summary>
    /// Represent the physics for a simple model.
    /// </summary>
    public interface IModelPhysics : IDisposable
    {
        /// <summary>
        /// The physics entity.
        /// May be null.
        /// </summary>
        Entity Entity { get; }
    }
}
