using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// An ambient light in a scene.
    /// </summary>
    public class AmbientLight : Light
    {
        /// <summary>
        /// Create an AmbientLight3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        public AmbientLight(XiGame game) : base(game) { }

        /// <summary>
        /// The color of the light.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        
        private Color color = Color.Gray;
    }
}
