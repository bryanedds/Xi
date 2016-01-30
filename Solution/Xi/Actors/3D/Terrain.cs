using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// A terrain that is split into patches.
    /// </summary>
    public class Terrain : SingleSurfaceActor<TerrainSurface>
    {
        /// <summary>
        /// Create a Terrain3D.
        /// </summary>
        public Terrain(XiGame game) : base(game, false)
        {
            surface = new TerrainSurface(game, this);
        }

        /// <summary>
        /// The height map that represents the topography of the terrain.
        /// </summary>
        [Browsable(false)]
        public HeightMap HeightMap { get { return surface.HeightMap; } }

        /// <summary>
        /// The scale of each geometry quad.
        /// </summary>
        public Vector3 QuadScale
        {
            get { return surface.QuadScale; }
            set { surface.QuadScale = value; }
        }

        /// <summary>
        /// The scale of each terrain patch.
        /// </summary>
        [Browsable(false)]
        public Vector3 PatchScale { get { return surface.PatchScale; } }

        /// <summary>
        /// The scale of the grid.
        /// </summary>
        [Browsable(false)]
        public Vector3 GridScale { get { return surface.GridScale; } }

        /// <summary>
        /// The offset used to center the terrain on the [x, z] origin.
        /// </summary>
        [Browsable(false)]
        public Vector2 GridCenterOffset { get { return surface.GridCenterOffset; } }

        /// <summary>
        /// The number of times the terrain texturing repeats.
        /// </summary>
        public Vector2 TextureRepetition
        {
            get { return surface.TextureRepetition; }
            set { surface.TextureRepetition = value; }
        }

        /// <summary>
        /// The number of quads in each patch.
        /// </summary>
        [Browsable(false)]
        public Point PatchDims { get { return surface.PatchDims; } }

        /// <summary>
        /// The number of patches in the terrain.
        /// </summary>
        public Point GridDims
        {
            get { return surface.GridDims; }
            set { surface.GridDims = value; }
        }

        /// <summary>
        /// The name of the texture file that defines the topography.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string HeightMapFileName
        {
            get { return surface.HeightMapFileName; }
            set { surface.HeightMapFileName = value; }
        }

        /// <summary>
        /// The name of the 0th texture file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap0FileName
        {
            get { return surface.DiffuseMap0FileName; }
            set { surface.DiffuseMap0FileName = value; }
        }

        /// <summary>
        /// The name of the 1st texture file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap1FileName
        {
            get { return surface.DiffuseMap1FileName; }
            set { surface.DiffuseMap1FileName = value; }
        }

        /// <summary>
        /// The name of the 2nd texture file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap2FileName
        {
            get { return surface.DiffuseMap2FileName; }
            set { surface.DiffuseMap2FileName = value; }
        }

        /// <summary>
        /// The name of the 3rd texture file.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap3FileName
        {
            get { return surface.DiffuseMap3FileName; }
            set { surface.DiffuseMap3FileName = value; }
        }

        /// <summary>
        /// The amount by which the topography is smoothened.
        /// </summary>
        public float SmoothingFactor
        {
            get { return surface.SmoothingFactor; }
            set { surface.SmoothingFactor = value; }
        }

        /// <inheritdoc />
        protected override TerrainSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override void Destroy(bool destroying)
        {
            if (destroying) surface.Dispose();
            base.Destroy(destroying);
        }

        private readonly TerrainSurface surface;
    }
}
