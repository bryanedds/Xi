using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents a 3D directional shadow.
    /// </summary>
	public interface IDirectionalShadow : IDisposable
    {
	    /// <summary>
	    /// The shadow map that results from drawing a scene's shadows.
        /// May be null.
	    /// </summary>
        Texture2D VolatileShadowMap { get; }

        /// <summary>
        /// The camera view from which the shadow map is drawn.
        /// </summary>
        OrthoCamera Camera { get; }

        /// <summary>
        /// Draw the shadow map for a scene.
        /// </summary>
        void Draw(GameTime gameTime);
    }
}
