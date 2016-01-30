using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// An implementation of IManagedRenderTarget that draws to the back buffer.
    /// </summary>
    public class ManagedBackBuffer : Disposable, IManagedRenderTarget
    {
        /// <summary>
        /// Create a ManagedBackBuffer.
        /// </summary>
        public ManagedBackBuffer(XiGame game, int renderTargetIndex)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            this.renderTargetIndex = renderTargetIndex;
        }

        /// <inheritdoc />
        public Texture2D VolatileTexture { get { return null; } }

        /// <inheritdoc />
        public float SurfaceToScreenRatio
        {
            get { return 1; }
            set { }
        }

        /// <inheritdoc />
        public Point Resolution
        {
            get
            {
                PresentationParameters presentation = game.GraphicsDevice.PresentationParameters;
                return new Point(presentation.BackBufferWidth, presentation.BackBufferHeight);
            }
        }

        /// <inheritdoc />
        public int RenderTargetIndex { get { return renderTargetIndex; } }

        /// <inheritdoc />
        public void Activate()
        {
            game.GraphicsDevice.SetRenderTarget(renderTargetIndex, null);
        }

        /// <inheritdoc />
        public void Resolve() { }

        private readonly XiGame game;
        private readonly int renderTargetIndex;
    }
}
