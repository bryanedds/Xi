using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A 3D point light.
    /// </summary>
    public class PointLight : Light
    {
        /// <summary>
        /// Create a PointLight3D object.
        /// </summary>
        /// <param name="game">The game.</param>
        public PointLight(XiGame game) : base(game) { }

        /// <summary>
        /// The diffuse color of the light.
        /// </summary>
        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        /// <summary>
        /// The range of the light.
        /// </summary>
        public float Range
        {
            get { return range; }
            set { range = value; }
        }

        /// <summary>
        /// The falloff of the light.
        /// </summary>
        public float Falloff
        {
            get { return falloff; }
            set { falloff = value; }
        }

        private Color diffuseColor = Color.Gray;
        private Color specularColor = Color.Gray;
        private float range = 64;
        private float falloff = 64;
    }
}
