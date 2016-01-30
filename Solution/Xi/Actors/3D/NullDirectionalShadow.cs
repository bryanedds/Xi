using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A null implementation of IDirectionalShadow.
    /// </summary>
    public class NullDirectionalShadow : Disposable, IDirectionalShadow
    {
        /// <summary>
        /// Create a NullDirectionalShadow.
        /// </summary>
        /// <param name="game">The game.</param>
        public NullDirectionalShadow(XiGame game)
        {
            camera = new OrthoCamera(game.GraphicsDevice);
        }

        /// <inheritdoc />
        public Texture2D VolatileShadowMap { get { return null; } }

        /// <inheritdoc />
        public OrthoCamera Camera { get { return camera; } }

        /// <inheritdoc />
        public void Draw(GameTime gameTime) { }

        private readonly OrthoCamera camera;
    }
}
