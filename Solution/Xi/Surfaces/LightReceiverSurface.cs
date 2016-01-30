using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A surface that receives fog and light.
    /// </summary>
    public class LightReceiverSurface : BaseModelSurface
    {
        /// <summary>
        /// Create a LightReceiverSurface.
        /// </summary>
        public LightReceiverSurface(XiGame game, NormalMappedModel actor, string effectFileName, int meshIndex, int partIndex)
            : base(game, actor, effectFileName, meshIndex, partIndex) { }

        /// <summary>
        /// The actor.
        /// </summary>
        public new NormalMappedModel Actor { get { return XiHelper.Cast<NormalMappedModel>(base.Actor); } }
        
        /// <summary>
        /// The diffuse map.
        /// May be null.
        /// </summary>
        public Texture2D DiffuseMap { get { return _diffuseMap; } }
        
        /// <summary>
        /// The diffuse map file name.
        /// </summary>
        public string DiffuseMapFileName
        {
            get { return _diffuseMapFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_diffuseMapFileName == value) return; // OPTIMIZATION
                if (value.Length == 0) return;
                Texture2D newDiffuseMap = Game.Content.Load<Texture2D>(value);
                _diffuseMap = newDiffuseMap;
                _diffuseMapFileName = value;
            }
        }

        /// <summary>
        /// The effect that the model mesh part was loaded with.
        /// </summary>
        protected Effect OriginalEffect { get { return Part.Effect; } }

        /// <inheritdoc />
        protected override Model Model { get { return Actor.Model; } }

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
        protected override void GetBoneAbsoluteWorld(out Matrix worldTransform)
        {
            Actor.GetBoneAbsoluteWorld(Mesh.ParentBone.Index, out worldTransform);
        }

        /// <summary>
        /// Handle creating the effect with the specified file name.
        /// </summary>
        protected virtual Effect CreateEffectHook(string effectFileName)
        {
            Effect effectFromDisk = Game.Content.Load<Effect>(effectFileName);
            return new LightReceiverEffect(Game.GraphicsDevice, effectFromDisk);
        }

        /// <summary>
        /// Handle populating the effect's parameters.
        /// </summary>
        protected virtual void PopulateEffectHook(GameTime gameTime, Camera camera, string drawMode)
        {
            LightReceiverEffect lrEffect = XiHelper.Cast<LightReceiverEffect>(Effect);
            PopulateEffectTransform(camera, lrEffect);
            if (drawMode == "Normal")
            {
                PopulateEffectDiffuseMap(lrEffect);
                PopulateEffectFogging(lrEffect);
                PopulateEffectLighting(lrEffect);
            }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal" && drawMode != "DirectionalShadow") return;
            PopulateEffect(gameTime, camera, drawMode);
            GraphicsDevice device = Game.GraphicsDevice;
            ModelMesh mesh = Mesh;
            ModelMeshPart part = Part;
            Effect effect = Effect;
            BeginRenderState();
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                device.Indices = mesh.IndexBuffer;
                device.VertexDeclaration = part.VertexDeclaration;
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    part.BaseVertex,
                    0,
                    part.NumVertices,
                    part.StartIndex,
                    part.PrimitiveCount);
                pass.End();
            }
            effect.End();
            EndRenderState();
        }

        private Effect CreateEffect(string effectFileName)
        {
            return CreateEffectHook(effectFileName);
        }

        private void PopulateEffect(GameTime gameTime, Camera camera, string drawMode)
        {
            Effect.TrySetCurrentTechnique(drawMode);
            PopulateEffectHook(gameTime, camera, drawMode);
        }

        private void PopulateEffectTransform(Camera camera, LightReceiverEffect lrEffect)
        {
            Matrix worldTransform;
            Actor.GetBoneAbsoluteWorld(Mesh.ParentBone.Index, out worldTransform);
            lrEffect.PopulateTransform(camera, ref worldTransform);
        }

        private void PopulateEffectDiffuseMap(LightReceiverEffect lrEffect)
        {
            // set diffuse map to manually configured diffuse map if available
            if (DiffuseMap != null) lrEffect.DiffuseMap = DiffuseMap;
            // if no diffuse map set, set from imported diffuse map
            if (lrEffect.DiffuseMap == null) lrEffect.DiffuseMap = XiHelper.Cast<BasicEffect>(OriginalEffect).Texture;
        }

        private void PopulateEffectFogging(LightReceiverEffect lrEffect)
        {
            lrEffect.PopulateFogging(Game.Scene.Fog);
        }

        private void PopulateEffectLighting(LightReceiverEffect lrEffect)
        {
            Scene scene = Game.Scene;
            lrEffect.PopulateLighting(
                this,
                scene.CachedAmbientLights,
                scene.CachedDirectionalLights,
                scene.CachedPointLights);
        }

        private void BeginRenderState()
        {
            GraphicsDevice device = Game.GraphicsDevice;
            device.BeginFaceMode(FaceMode);
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                if (!DrawTransparentPixels)
                {
                    device.RenderState.AlphaTestEnable = true;
                    device.RenderState.AlphaFunction = CompareFunction.Greater;
                }
            }
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Game.GraphicsDevice;
            device.EndFaceMode();
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.SourceBlend = Blend.One;
                device.RenderState.DestinationBlend = Blend.Zero;
                if (!DrawTransparentPixels)
                {
                    device.RenderState.AlphaTestEnable = false;
                    device.RenderState.AlphaFunction = CompareFunction.Always;
                }
            }
        }

        /// <summary>May be null.</summary>
        private Texture2D _diffuseMap;
        private Effect _effect;
        private string _diffuseMapFileName = string.Empty;
        private string _effectFileName = string.Empty;
    }
}
