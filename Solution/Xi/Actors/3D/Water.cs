using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A body of water on the x,z plane.
    /// </summary>
    public class Water : SingleSurfaceActor<WaterSurface>
    {
        /// <summary>
        /// Create a Water3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        public Water(XiGame game) : base(game)
        {
            surface = new WaterSurface(game, this);
            Position = new Vector3(0, 10, 0); // nice offset from bottom of terrain
        }

        /// <summary>
        /// The scale of the water on the x,z plane.
        /// </summary>
        public Vector2 Scale
        {
            get { return surface.Scale; }
            set { surface.Scale = value; }
        }

        /// <summary>
        /// The velocity of the 0th wave normal map.
        /// </summary>
        public Vector2 WaveMap0Velocity
        {
            get { return surface.WaveMap0Velocity; }
            set { surface.WaveMap0Velocity = value; }
        }

        /// <summary>
        /// The velocity of the 1st wave normal map.
        /// </summary>
        public Vector2 WaveMap1Velocity
        {
            get { return surface.WaveMap1Velocity; }
            set { surface.WaveMap1Velocity = value; }
        }

        /// <summary>
        /// The multiplier of the water's color.
        /// </summary>
        public Color ColorMultiplier
        {
            get { return surface.ColorMultiplier; }
            set { surface.ColorMultiplier = value; }
        }

        /// <summary>
        /// The color of the water.
        /// </summary>
        public Color ColorAdditive
        {
            get { return surface.ColorAdditive; }
            set { surface.ColorAdditive = value; }
        }

        /// <summary>
        /// The name of the 0th wave normal map.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string WaveMap0FileName
        {
            get { return surface.WaveMap0FileName; }
            set { surface.WaveMap0FileName = value; }
        }

        /// <summary>
        /// The name of the 1st wave normal map.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string WaveMap1FileName
        {
            get { return surface.WaveMap1FileName; }
            set { surface.WaveMap1FileName = value; }
        }

        /// <summary>
        /// The length of the waves.
        /// </summary>
        public float WaveLength
        {
            get { return surface.WaveLength; }
            set { surface.WaveLength = value; }
        }

        /// <summary>
        /// The height of the waves.
        /// </summary>
        public float WaveHeight
        {
            get { return surface.WaveHeight; }
            set { surface.WaveHeight = value; }
        }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) surface.Dispose();
            base.Destroy(destroying);
        }

        /// <inheritdoc />
        protected override WaterSurface SurfaceHook { get { return surface; } }
        
        private readonly WaterSurface surface;
    }

    /// <summary>
    /// Helper methods for water.
    /// </summary>
    public static class WaterHelper
    {
        /// <summary>
        /// Create a clip plane for drawing the water reflection from the reflection camera's view.
        /// </summary>
        public static Plane CreateWaterReflectionPlane(Camera reflectionCamera, float waterHeight)
        {
            XiHelper.ArgumentNullCheck(reflectionCamera);
            Vector4 planeCoefficients = new Vector4(Vector3.Up, -waterHeight);
            Matrix inverseCamera = Matrix.Transpose(Matrix.Invert(reflectionCamera.ViewProjection));
            Vector4 transformedPlaneCoefficients = Vector4.Transform(planeCoefficients, inverseCamera);
            return new Plane(transformedPlaneCoefficients);
        }
    }
}
