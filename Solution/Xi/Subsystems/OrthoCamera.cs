using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A camera that uses orthographic projection.
    /// </summary>
    public class OrthoCamera : Camera
    {
        /// <summary>
        /// Create an OrthoCamera.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public OrthoCamera(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, 0, 1024) { }

        /// <summary>
        /// The width of the camera's viewing area.
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                if (_width == value) return; // OPTIMIZATION
                _width = value;
                RefreshProjection();
            }
        }

        /// <summary>
        /// The height of the camera's viewing area.
        /// </summary>
        public float Height
        {
            get { return _height; }
            set
            {
                if (_height == value) return; // OPTIMIZATION
                _height = value;
                RefreshProjection();
            }
        }

        /// <inheritdoc />
        protected override Matrix CalculateProjectionHook()
        {
            return Matrix.CreateOrthographic(Width, Height, NearPlane, FarPlane);
        }

        private float _width = 160;
        private float _height = 90;
    }
}
