using System;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Invokes resolution changes and observes resolution changes by machinery other than its own.
    /// </summary>
    public class ResolutionManager
    {
        /// <summary>
        /// Create a ResolutionManager.
        /// </summary>
        /// <param name="deviceManager">The graphics device manager.</param>
        public ResolutionManager(GraphicsDeviceManager deviceManager)
        {
            XiHelper.ArgumentNullCheck(deviceManager);
            this.deviceManager = deviceManager;
            resolution = new Point(deviceManager.PreferredBackBufferWidth, deviceManager.PreferredBackBufferHeight);
        }

        /// <summary>
        /// Raised when the game's resolution is changed.
        /// </summary>
        public event Action<ResolutionManager, Point> ResolutionChanged;

        /// <summary>
        /// The game's resolution.
        /// </summary>
        public Point Resolution
        {
            get { return resolution; }
            set
            {
                value.X = (int)MathHelper.Max(32, value.X);
                value.Y = (int)MathHelper.Max(32, value.Y);
                if (resolution == value) return;
                deviceManager.PreferredBackBufferWidth = value.X;
                deviceManager.PreferredBackBufferHeight = value.Y;
                deviceManager.ApplyChanges();
                Point oldResolution = resolution;
                resolution = new Point(deviceManager.PreferredBackBufferWidth, deviceManager.PreferredBackBufferHeight);
                if (oldResolution != resolution) ResolutionChanged.TryRaise(this, resolution);
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Resolution = new Point(deviceManager.PreferredBackBufferWidth, deviceManager.PreferredBackBufferHeight);
        }

        private readonly GraphicsDeviceManager deviceManager;
        private Point resolution;
    }
}
