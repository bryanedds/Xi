using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// The surface of a terrain in 3D.
    /// TODO: consider making terrain not cast shadows (for efficiency).
    /// </summary>
    public class TerrainSurface : Surface<Terrain>
    {
        /// <summary>
        /// Create a TerrainSurface3D.
        /// </summary>
        public TerrainSurface(XiGame game, Terrain actor)
            : base(game, actor, "Xi/3D/XiTerrain")
        {
            DiffuseMap0FileName = "Xi/3D/Sand";
            DiffuseMap1FileName = "Xi/3D/Grass";
            DiffuseMap2FileName = "Xi/3D/Rock";
            DiffuseMap3FileName = "Xi/3D/Snow";
            // OPTIMIZATION: circumvent properties, having HeightMapFileName property mutation call
            // RecreateInnards to refresh properties calculated downstream
            _quadScale = new Vector3(8, 64, 8);
            _gridDims = new Point(8, 8);
            _smoothingFactor = 1;
            _textureRepetition = new Vector2(64, 64);
            HeightMapFileName = "Xi/3D/heightMap";
        }

        /// <summary>
        /// The height map that represents the topography of the terrain.
        /// </summary>
        public HeightMap HeightMap { get { return _heightMap; } }

        /// <summary>
        /// The scale of each geometry quad.
        /// </summary>
        public Vector3 QuadScale
        {
            get { return _quadScale; }
            set
            {
                value.X = MathHelper.Clamp(value.X, 0.001f, float.MaxValue); // VALIDATION
                value.Y = MathHelper.Clamp(value.Y, 0.001f, float.MaxValue); // VALIDATION
                value.Z = MathHelper.Clamp(value.Z, 0.001f, float.MaxValue); // VALIDATION
                if (_quadScale == value) return; // OPTIMIZATION
                _quadScale = value;
                ResetPatches();
            }
        }

        /// <summary>
        /// The scale of each terrain patch.
        /// </summary>
        public Vector3 PatchScale
        {
            get { return new Vector3(PatchDims.X, 1, PatchDims.Y) * QuadScale; }
        }

        /// <summary>
        /// The scale of the grid.
        /// </summary>
        public Vector3 GridScale
        {
            get { return new Vector3(GridDims.X, 1, GridDims.Y) * PatchScale; }
        }

        /// <summary>
        /// The offset used to center the terrain on the [x, z] origin.
        /// </summary>
        public Vector2 GridCenterOffset
        {
            get { return new Vector2(GridScale.X * -0.5f, GridScale.Z * -0.5f);; }
        }

        /// <summary>
        /// The number of times the terrain texturing repeats.
        /// </summary>
        public Vector2 TextureRepetition
        {
            get { return _textureRepetition; }
            set
            {
                value.X = MathHelper.Clamp(value.X, 0.001f, float.MaxValue); // VALIDATION
                value.Y = MathHelper.Clamp(value.Y, 0.001f, float.MaxValue); // VALIDATION
                if (_textureRepetition == value) return; // OPTIMIZATION
                _textureRepetition = value;
                ResetPatches();
            }
        }

        /// <summary>
        /// The number of quads in each patch.
        /// </summary>
        public Point PatchDims
        {
            get { return new Point((HeightMapTexture.Width - 1) / GridDims.X, (HeightMapTexture.Height - 1) / GridDims.Y); }
        }

        /// <summary>
        /// The number of patches in the terrain.
        /// </summary>
        public Point GridDims
        {
            get { return _gridDims; }
            set
            {
                value.X = (int)MathHelper.Clamp(value.X, 1, 128); // VALIDATION
                value.Y = (int)MathHelper.Clamp(value.Y, 1, 128); // VALIDATION
                if (_gridDims == value) return; // OPTIMIZATION
                _gridDims = value;
                ResetPatches();
            }
        }

        /// <summary>
        /// The name of the texture file that defines the topography.
        /// </summary>
        public string HeightMapFileName
        {
            get { return _heightMapFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_heightMapFileName == value) return; // OPTIMIZATION
                Texture2D newHeightMapTexture = Game.Content.Load<Texture2D>(value);
                _heightMapTexture = newHeightMapTexture;
                _heightMapFileName = value;
                ResetPatches();
            }
        }

        /// <summary>
        /// The name of the 0th diffuse map file.
        /// </summary>
        public string DiffuseMap0FileName
        {
            get { return _diffuseMap0FileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_diffuseMap0FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap0 = Game.Content.Load<Texture2D>(value);
                _diffuseMap0 = newDiffuseMap0;
                _diffuseMap0FileName = value;
            }
        }

        /// <summary>
        /// The name of the 1st diffuse map file.
        /// </summary>
        public string DiffuseMap1FileName
        {
            get { return _diffuseMap1FileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_diffuseMap1FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap1 = Game.Content.Load<Texture2D>(value);
                _diffuseMap1 = newDiffuseMap1;
                _diffuseMap1FileName = value;
            }
        }

        /// <summary>
        /// The name of the 2nd texture file.
        /// </summary>
        public string DiffuseMap2FileName
        {
            get { return _diffuseMap2FileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_diffuseMap2FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap2 = Game.Content.Load<Texture2D>(value);
                _diffuseMap2 = newDiffuseMap2;
                _diffuseMap2FileName = value;
            }
        }

        /// <summary>
        /// The name of the 3rd texture file.
        /// </summary>
        public string DiffuseMap3FileName
        {
            get { return _diffuseMap3FileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_diffuseMap3FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap3 = Game.Content.Load<Texture2D>(value);
                _diffuseMap3 = newDiffuseMap3;
                _diffuseMap3FileName = value;
            }
        }

        /// <summary>
        /// The amount by which the topography is smoothened.
        /// </summary>
        public float SmoothingFactor
        {
            get { return _smoothingFactor; }
            set
            {
                value = MathHelper.Clamp(value, 0, 1); // VALIDATION
                if (_smoothingFactor == value) return; // OPTIMIZATION
                _smoothingFactor = value;
                ResetPatches();
            }
        }

        /// <inheritdoc />
        protected override BoundingBox BoundingBoxHook { get { return BoundingBoxHelper.CreateAllEncompassing(); } }

        /// <inheritdoc />
        protected override bool BoundlessHook { get { return true; } }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return _effect; } }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return _effectFileName; }
            set
            {
                if (_effectFileName == value) return; // OPTIMIZATION
                Effect newEffect = CreateEffect(value);
                _effect = newEffect;
                _effectFileName = value;
            }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal" && drawMode != "DirectionalShadow") return;
            // OPTIMIZATION: cache these properties
            GraphicsDevice device = Game.GraphicsDevice;
            Scene scene = Game.Scene;
            ShadowReceiverEffect srEffect = XiHelper.Cast<ShadowReceiverEffect>(Effect);
            srEffect.TrySetCurrentTechnique(drawMode);
            PopulateEffect(camera, drawMode, srEffect);
            BoundingBox[,] patchBoundingBoxes = PatchBoundingBoxes;
            Geometry[,] patches = Patches;
            BeginRenderState(device);
            srEffect.Begin();
            foreach (EffectPass pass in srEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                for (int j = 0; j < patches.GetLength(0); ++j)
                {
                    for (int k = 0; k < patches.GetLength(1); ++k)
                    {
                        BoundingBox patchBoundingBox = patchBoundingBoxes[j, k];
                        Geometry patch = patches[j, k];
                        if (camera.Contains(patchBoundingBox) != ContainmentType.Disjoint)
                        {
                            PopulateEffectPerPatch(camera, drawMode, srEffect);
                            patch.Draw(gameTime);
                        }
                    }
                }
                pass.End();
            }
            srEffect.End();
            EndRenderState(device);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) TearDownPatches();
            base.Dispose(disposing);
        }

        private BoundingBox[,] PatchBoundingBoxes { get { return _patchBoundingBoxes; } }

        private Geometry[,] Patches { get { return _patches; } }

        private Texture2D HeightMapTexture { get { return _heightMapTexture; } }

        private Texture2D DiffuseMap0 { get { return _diffuseMap0; } }

        private Texture2D DiffuseMap1 { get { return _diffuseMap1; } }

        private Texture2D DiffuseMap2 { get { return _diffuseMap2; } }

        private Texture2D DiffuseMap3 { get { return _diffuseMap3; } }

        private Vector2 QuadTextureScale
        {
            get
            {
                Vector2 gridDims = new Vector2(GridDims.X, GridDims.Y);
                Vector2 patchDims = new Vector2(PatchDims.X, PatchDims.Y);
                return TextureRepetition / gridDims / patchDims;
            }
        }

        private string HeightMapTooSmallMessage
        {
            get
            {
                return
                    "The height map requires a resolution of at least " +
                    (GridDims.X + 1) + "x" + (GridDims.Y + 1) + ".";
            }
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Game.Content.Load<Effect>(effectFileName);
            return new ShadowReceiverEffect(Game.GraphicsDevice, effectFromDisk);
        }

        private HeightMap CreateHeightMap()
        {
            HeightMap rawHeightMap = TerrainPatchGeometry.TextureToHeightMap(HeightMapTexture, QuadScale, GridCenterOffset);
            if (IsHeightMapTooSmall(rawHeightMap)) throw new FormatException(HeightMapTooSmallMessage);
            return TerrainPatchGeometry.SmoothenHeightMap(rawHeightMap, SmoothingFactor);
        }

        private void ResetPatches()
        {
            _heightMap = CreateHeightMap();
            TearDownPatches();
            SetUpPatches();
        }

        private void SetUpPatches()
        {
            _patches = new Geometry[GridDims.X, GridDims.Y];
            _patchBoundingBoxes = new BoundingBox[GridDims.X, GridDims.Y];
            for (int i = 0; i < GridDims.X; ++i)
            {
                for (int j = 0; j < GridDims.Y; ++j)
                {
                    SetUpPatch(i, j);
                    SetUpPatchBoundingBox(i, j);
                }
            }
        }

        private void SetUpPatch(int i, int j)
        {
            Rectangle terrainPortion = new Rectangle(i * PatchDims.X, j * PatchDims.Y, PatchDims.X + 1, PatchDims.Y + 1);
            Geometry patch = TerrainPatchGeometry.Create<VerticesPositionNormalTexture>(
                Game.GraphicsDevice, HeightMap, terrainPortion, QuadScale, QuadTextureScale);
            Patches[i, j] = patch;
        }

        private void SetUpPatchBoundingBox(int i, int j)
        {
            Vector3 patchMin = CalculatePatchPosition(i, j);
            Vector3 patchMax = patchMin + PatchScale;
            BoundingBox patchBoundingBox = new BoundingBox(patchMin, patchMax);
            PatchBoundingBoxes[i, j] = patchBoundingBox;
        }

        private void TearDownPatches()
        {
            for (int i = 0; i < Patches.GetLength(0); ++i)
                for (int j = 0; j < Patches.GetLength(1); ++j)
                    TearDownPatch(i, j);
            _patches = new Geometry[0, 0];
            _patchBoundingBoxes = new BoundingBox[0, 0];
        }

        private void TearDownPatch(int i, int j)
        {
            Geometry patch = Patches[i, j];
            if (patch == null) return;
            patch.Dispose();
            Patches[i, j] = null;
        }

        private void BeginRenderState(GraphicsDevice device)
        {
            device.BeginFaceMode(FaceMode);
        }

        private void EndRenderState(GraphicsDevice device)
        {
            device.EndFaceMode();
        }

        private void PopulateEffect(Camera camera, string drawMode, ShadowReceiverEffect effect)
        {
            Scene scene = Game.Scene;
            effect.PopulateTransformWorld(camera);
            if (drawMode == "Normal")
            {
                effect.PopulateFogging(scene.Fog);
                effect.PopulateLighting(this, scene.CachedAmbientLights, scene.CachedDirectionalLights, scene.CachedPointLights);
                effect.Parameters["xHeightMax"].TrySetValue(QuadScale.Y);
                effect.Parameters["xDiffuseMap0"].TrySetValue(DiffuseMap0);
                effect.Parameters["xDiffuseMap1"].TrySetValue(DiffuseMap1);
                effect.Parameters["xDiffuseMap2"].TrySetValue(DiffuseMap2);
                effect.Parameters["xDiffuseMap3"].TrySetValue(DiffuseMap3);
            }
        }

        private void PopulateEffectPerPatch(Camera camera, string drawMode, ShadowReceiverEffect effect)
        {
            Matrix world = Matrix.Identity;
            effect.PopulateTransformLocal(camera, ref world);
            effect.PopulateShadowing(this, ref world, Game.Scene.CachedDirectionalLights);
            effect.CommitChanges();
        }

        private bool IsHeightMapTooSmall(HeightMap heightMap)
        {
            return
                heightMap.GetLength(0) < GridDims.X + 1 ||
                heightMap.GetLength(1) < GridDims.Y + 1;
        }

        private Vector3 CalculatePatchPosition(int i, int j)
        {
            return new Vector3(
                PatchDims.X * QuadScale.X * i + GridCenterOffset.X,
                0,
                PatchDims.Y * QuadScale.Z * j + GridCenterOffset.Y);
        }
        
        private BoundingBox[,] _patchBoundingBoxes = new BoundingBox[0, 0];
        private Geometry[,] _patches = new Geometry[0, 0];
        private HeightMap _heightMap;
        private Texture2D _heightMapTexture;
        private Texture2D _diffuseMap0;
        private Texture2D _diffuseMap1;
        private Texture2D _diffuseMap2;
        private Texture2D _diffuseMap3;
        private Vector3 _quadScale;
        private Vector2 _textureRepetition;
        private Effect _effect;
        private Point _gridDims;
        private string _effectFileName;
        private string _heightMapFileName;
        private string _diffuseMap0FileName;
        private string _diffuseMap1FileName;
        private string _diffuseMap2FileName;
        private string _diffuseMap3FileName;
        private float _smoothingFactor;
    }
}
