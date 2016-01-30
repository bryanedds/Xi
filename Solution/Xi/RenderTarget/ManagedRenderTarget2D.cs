using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A managed render target with a 2D drawing surface.
    /// </summary>
    public class ManagedRenderTarget2D : Disposable
    {
        /// <summary>
        /// Create a ManagedRenderTarget2D.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="surfaceToScreenRatio">The surface to screen ratio.</param>
        /// <param name="levelCount">The number of mip-map levels of the texture produced by the render target.</param>
        /// <param name="format">The format of the render target.</param>
        /// <param name="msType">The multi-sampling type of the texture produced by the render target.</param>
        /// <param name="msQuality">The multi-sampling quality of the texture produced by the render target.</param>
        /// <param name="renderTargetIndex">The render target index.</param>
        public ManagedRenderTarget2D(
            XiGame game,
            float surfaceToScreenRatio,
            int levelCount,
            SurfaceFormat format,
            MultiSampleType msType,
            int msQuality,
            int renderTargetIndex)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            this.levelCount = levelCount;
            this.format = format;
            this.msType = msType;
            this.msQuality = msQuality;
            this.renderTargetIndex = renderTargetIndex;
            game.GraphicsDevice.DeviceReset += new EventHandler(device_DeviceReset);
            SurfaceToScreenRatio = surfaceToScreenRatio;
        }

        /// <summary>
        /// The drawing surface as a 2D texture.
        /// May be null.
        /// </summary>
        public Texture2D VolatileTexture { get { return texture; } }

        /// <summary>
        /// The resolution of the drawing surface.
        /// </summary>
        public Point Resolution { get { return resolution; } }

        /// <summary>
        /// The drawing surface size as a ratio of the screen size.
        /// </summary>
        public float SurfaceToScreenRatio
        {
            get { return _surfaceToScreenRatio; }
            set
            {
                if (_surfaceToScreenRatio == value && renderTarget != null) return; // OPTIMIZATION
                _surfaceToScreenRatio = value;
                RecreateRenderTarget();
            }
        }

        /// <summary>
        /// The render target's index on the GraphicsDevice.
        /// </summary>
        public int RenderTargetIndex { get { return renderTargetIndex; } }

        /// <inheritdoc />
        public void Activate()
        {
            if (!resolved) throw new InvalidOperationException("Cannot activate a render target that is already active.");
            game.GraphicsDevice.SetRenderTarget(renderTargetIndex, renderTarget);
            resolved = false;
        }

        /// <inheritdoc />
        public void Resolve()
        {
            if (resolved || !IsRenderTargetSameAsDeviceRenderTarget)
            {
                throw new InvalidOperationException("Cannot resolve a render target that is already resolved.");
            }
            else
            {
                game.GraphicsDevice.SetRenderTarget(0, null);
                texture = renderTarget.GetTexture();
                resolved = true;
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                game.GraphicsDevice.DeviceReset -= device_DeviceReset;
                renderTarget.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IsRenderTargetSameAsDeviceRenderTarget
        {
            get { return game.GraphicsDevice.GetRenderTarget(renderTargetIndex) == renderTarget; }
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            XiHelper.ArgumentNullCheck(sender, e);
            RecreateRenderTarget();
        }

        private void RecreateRenderTarget()
        {
            if (renderTarget != null) renderTarget.Dispose();
            resolution = CalculateResolution(game.GraphicsDevice, SurfaceToScreenRatio);
            resolved = true;
            texture = null;
            renderTarget = new RenderTarget2D(
                game.GraphicsDevice,
                resolution.X,
                resolution.Y,
                levelCount,
                format,
                msType,
                msQuality);
        }

        private static Point CalculateResolution(GraphicsDevice graphicsDevice, float surfaceToScreenRatio)
        {
            PresentationParameters presentation = graphicsDevice.PresentationParameters;
            return new Point(
                (int)(presentation.BackBufferWidth * surfaceToScreenRatio),
                (int)(presentation.BackBufferHeight * surfaceToScreenRatio));
        }

        private readonly XiGame game;
        private readonly MultiSampleType msType;
        private readonly SurfaceFormat format;
        private readonly int renderTargetIndex;
        private readonly int levelCount;
        private readonly int msQuality;
        private RenderTarget2D renderTarget;
        /// <summary>May be null.</summary>
        private Texture2D texture;
        private Point resolution;
        private bool resolved = true;
        private float _surfaceToScreenRatio;
    }
}
