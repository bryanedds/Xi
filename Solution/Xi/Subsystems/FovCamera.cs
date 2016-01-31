using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A camera that uses field-of-view projection.
    /// </summary>
    public class FovCamera : Camera
    {
        /// <summary>
        /// Create a FovCamera.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        public FovCamera(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, 1, 2048) { }

        /// <summary>
        /// The camera's field of view.
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                if (_fieldOfView == value) return; // OPTIMIZATION
                _fieldOfView = value;
                RefreshProjection();
            }
        }

        /// <inheritdoc />
        protected override Matrix CalculateProjectionHook()
        {
            float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            return Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlane, FarPlane);
        }

        private float _fieldOfView = MathHelper.PiOver4;
    }
}
