using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A managed depth / stencil buffer.
    /// </summary>
    public class ManagedDepthStencilBuffer : Disposable
    {
        /// <summary>
        /// Create a ManagedDepthStencilBuffer.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="surfaceToScreenRatio">The surface to screen ratio.</param>
        /// <param name="format">The format of the stencil buffer.</param>
        /// <param name="msType">The multi-sampling type of the texture produced by the render target.</param>
        /// <param name="msQuality">The multi-sampling quality of the texture produced by the render target.</param>
        public ManagedDepthStencilBuffer(XiGame game, float surfaceToScreenRatio,
            DepthFormat format, MultiSampleType msType, int msQuality)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            this.format = format;
            this.msType = msType;
            this.msQuality = msQuality;
            game.GraphicsDevice.DeviceReset += device_DeviceReset;
            SurfaceToScreenRatio = surfaceToScreenRatio;
        }
        
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
                if (_surfaceToScreenRatio == value && depthStencilBuffer != null) return; // OPTIMIZATION
                _surfaceToScreenRatio = value;
                RecreateDepthStencilBuffer();
            }
        }

        /// <summary>
        /// Activate the depth stencil buffer.
        /// </summary>
        public void Activate()
        {
            game.GraphicsDevice.DepthStencilBuffer = depthStencilBuffer;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                game.GraphicsDevice.DeviceReset -= device_DeviceReset;
                depthStencilBuffer.Dispose();
            }
            base.Dispose(disposing);
        }

        private void device_DeviceReset(object sender, EventArgs e)
        {
            XiHelper.ArgumentNullCheck(sender, e);
            RecreateDepthStencilBuffer();
        }

        private void RecreateDepthStencilBuffer()
        {
            if (depthStencilBuffer != null) depthStencilBuffer.Dispose();
            resolution = CalculateResolution(game.GraphicsDevice, SurfaceToScreenRatio);
            depthStencilBuffer = new DepthStencilBuffer(game.GraphicsDevice, resolution.X, resolution.Y, format, msType, msQuality);
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
        private readonly DepthFormat format;
        private readonly int msQuality;
        private DepthStencilBuffer depthStencilBuffer;
        private Point resolution;
        private float _surfaceToScreenRatio;
    }
}
