using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A surface of a BasicModel.
    /// </summary>
    public class BasicModelSurface : BaseModelSurface
    {
        /// <summary>
        /// Create a BasicModelSurface.
        /// </summary>
        public BasicModelSurface(XiGame game, BasicModel actor, int meshIndex, int partIndex)
            : base(game, actor, string.Empty, meshIndex, partIndex) { }

        /// <summary>
        /// The actor.
        /// </summary>
        public new BasicModel Actor { get { return XiHelper.Cast<BasicModel>(base.Actor); } }

        /// <inheritdoc />
        protected override Model Model { get { return Actor.Model; } }

        /// <inheritdoc />
        protected override void GetBoneAbsoluteWorld(out Matrix worldTransform)
        {
            Actor.GetBoneAbsoluteWorld(Mesh.ParentBone.Index, out worldTransform);
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            PopulateEffect(camera, drawMode);
            // OPTIMIZATION: cache these properties
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

        private void PopulateEffect(Camera camera, string drawMode)
        {
            BasicEffect basicEffect = XiHelper.Cast<BasicEffect>(Effect);
            basicEffect.TrySetCurrentTechnique("BasicEffect");
            PopulateEffectTransform(camera, basicEffect);
            PopulateEffectFogging(basicEffect);
            PopulateEffectLighting(basicEffect);
        }

        private void PopulateEffectTransform(Camera camera, BasicEffect basicEffect)
        {
            Matrix worldTransform;
            Actor.GetBoneAbsoluteWorld(Mesh.ParentBone.Index, out worldTransform);
            basicEffect.PopulateTransform(camera, ref worldTransform);
        }

        private void PopulateEffectFogging(BasicEffect basicEffect)
        {
            basicEffect.PopulateFogging(Game.Scene.Fog);
        }

        private void PopulateEffectLighting(BasicEffect basicEffect)
        {
            Scene scene = Game.Scene;
            basicEffect.PopulateLighting(
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
    }
}
