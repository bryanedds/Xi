using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// The surface of a Skybox.
    /// </summary>
    public class SkyboxSurface : Surface<Skybox>
    {
        /// <summary>
        /// Create a SkyboxSurface.
        /// TODO: consider making ambient lights affect skybox.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="actor">The parent actor.</param>
        public SkyboxSurface(XiGame game, Skybox actor)
            : base(game, actor, "Xi/3D/XiSkybox")
        {
            geometry = SkyboxGeometry.Create<VerticesPositionNormalTexture>(game.GraphicsDevice);
            DiffuseMapFileName = "Xi/3D/skyCubeMap";
            FaceMode = FaceMode.BackFaces;
            DrawStyle = DrawStyle.Prioritized;
            DrawProperties = DrawProperties.Reflecting | DrawProperties.DependantTransform;
        }

        /// <summary>
        /// The name of the diffuse map file.
        /// </summary>
        public string DiffuseMapFileName
        {
            get { return _diffuseMapFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_diffuseMapFileName == value) return; // OPTIMIZATION
                TextureCube newDiffuseMap = Game.Content.Load<TextureCube>(value);
                _diffuseMap = newDiffuseMap;
                _diffuseMapFileName = value;
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
        protected override void Dispose(bool disposing)
        {
            if (disposing) geometry.Dispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            BeginRenderState();
            BeginEffect(camera);
            DrawGeometryPasses(gameTime);
            EndEffect();
            EndRenderState();
        }

        private TextureCube DiffuseMap { get { return _diffuseMap; } }

        private void BeginRenderState()
        {
            GraphicsDevice device = Game.GraphicsDevice;
            device.RenderState.DepthBufferWriteEnable = false;
            device.BeginFaceMode(FaceMode);
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Game.GraphicsDevice;
            device.EndFaceMode();
            device.RenderState.DepthBufferWriteEnable = true;
        }

        private void BeginEffect(Camera camera)
        {
            Matrix world = Matrix.Identity;
            BaseEffect baseEffect = XiHelper.Cast<BaseEffect>(Effect);
            baseEffect.TrySetCurrentTechnique("Normal");
            baseEffect.Parameters["xSkyMap"].TrySetValue(DiffuseMap);
            baseEffect.PopulateTransform(camera, ref world);
            baseEffect.Begin();
        }

        private void EndEffect()
        {
            BaseEffect baseEffect = XiHelper.Cast<BaseEffect>(Effect);
            baseEffect.End();
        }

        private void DrawGeometryPasses(GameTime gameTime)
        {
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                DrawGeometryPass(gameTime, pass);
        }

        private void DrawGeometryPass(GameTime gameTime, EffectPass pass)
        {
            pass.Begin();
            geometry.Draw(gameTime);
            pass.End();
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Game.Content.Load<Effect>(effectFileName);
            return new BaseEffect(Game.GraphicsDevice, effectFromDisk);
        }

        private readonly Geometry geometry;
        private TextureCube _diffuseMap;
        private Effect _effect;
        private string _effectFileName;
        private string _diffuseMapFileName;
    }
}
