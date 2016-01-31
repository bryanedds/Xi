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
            SetUpTerrain();
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
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (surface.QuadScale == value) return; // OPTIMIZATION
                surface.QuadScale = value;
                ResetTerrain();
            }
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
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (surface.GridDims == value) return; // OPTIMIZATION
                surface.GridDims = value;
                ResetTerrain();
            }
        }

        /// <summary>
        /// The name of the texture file that defines the topography.
        /// </summary>
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string HeightMapFileName
        {
            get { return surface.HeightMapFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (surface.HeightMapFileName == value) return; // OPTIMIZATION
                surface.HeightMapFileName = value;
                ResetTerrain();
            }
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
            if (destroying) TearDownTerrain();
            base.Destroy(destroying);
        }

        private void ResetTerrain()
        {
            TearDownTerrain();
            SetUpTerrain();
        }

        private void ResetSurface()
        {
            TearDownSurface();
            SetUpSurface();
        }

        private void ResetPhysics()
        {
            TearDownPhysics();
            SetUpPhysics();
        }

        private void SetUpTerrain()
        {
            ResetSurface();
            ResetPhysics();
        }

        private void SetUpSurface()
        {
            surface = new TerrainSurface(Game, this);
        }

        private void SetUpPhysics()
        {
            physics = new TerrainPhysics(Game, this);
            Entity = physics.Entity;
            Entity.IsAlwaysActive = Game.Editing;
        }

        private void TearDownTerrain()
        {
            TearDownSurface();
            TearDownPhysics();
        }

        private void TearDownSurface()
        {
            if (surface != null) surface.Dispose();
            surface = null;
        }

        private void TearDownPhysics()
        {
            if (physics != null) physics.Dispose();
            physics = null;
        }

        private TerrainSurface surface;
        private TerrainPhysics physics; 
    }
}
