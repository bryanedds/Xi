using System.ComponentModel;
using System.Drawing.Design;

namespace Xi
{
    /// <summary>
    /// A skybox that only works in perspective view.
    /// TODO: make this work in orthographic view.
    /// </summary>
    public class Skybox : SingleSurfaceActor<SkyboxSurface>
    {
        /// <summary>
        /// Create a Skybox.
        /// </summary>
        /// <param name="game">The game.</param>
        public Skybox(XiGame game) : base(game, false)
        {
            surface = new SkyboxSurface(game, this);
        }

        /// <summary>
        /// The name of the diffuse map file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMapFileName
        {
            get { return surface.DiffuseMapFileName; }
            set { surface.DiffuseMapFileName = value; }
        }

        /// <inheritdoc />
        protected override SkyboxSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) surface.Dispose();
            base.Destroy(destroying);
        }

        private readonly SkyboxSurface surface;
    }
}
