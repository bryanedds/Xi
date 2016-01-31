using System.Collections.Generic;
using System.ComponentModel;

namespace Xi
{
    /// <summary>
    /// An actor with a single 3D surface.
    /// </summary>
    /// <typeparam name="T">The type of surface.</typeparam>
    public abstract class SingleSurfaceActor<T> : Actor3D
        where T : Surface
    {
        /// <summary>
        /// Create an SingleSurfaceActor3D.
        /// </summary>
        /// <param name="game">The game.</param>
        public SingleSurfaceActor(XiGame game) : base(game) { }

        /// <summary>
        /// The surface.
        /// </summary>
        [Browsable(false)]
        public T Surface { get { return SurfaceHook; } }

        protected abstract T SurfaceHook { get; }

        /// <inheritdoc />
        protected sealed override List<Surface> CollectSurfacesHook(List<Surface> surfaces)
        {
            surfaces.Add(Surface);
            return surfaces;
        }
    }
}
