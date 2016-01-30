using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A surface that receives shadows.
    /// </summary>
    public class ShadowReceiverSurface : LightReceiverSurface
    {
        /// <summary>
        /// Create a ShadowReceiverSurface.
        /// </summary>
        public ShadowReceiverSurface(XiGame game, NormalMappedModel actor, string effectFileName, int meshIndex, int partIndex)
            : base(game, actor, effectFileName, meshIndex, partIndex) { }

        /// <inheritdoc />
        protected override void PopulateEffectHook(GameTime gameTime, Camera camera, string drawMode)
        {
            base.PopulateEffectHook(gameTime, camera, drawMode);
            ShadowReceiverEffect srEffect = XiHelper.Cast<ShadowReceiverEffect>(Effect);
            Matrix worldTransform;
            Actor.GetBoneAbsoluteWorld(Mesh.ParentBone.Index, out worldTransform);
            srEffect.PopulateShadowing(this, ref worldTransform, Game.Scene.CachedDirectionalLights);
        }

        /// <inheritdoc />
        protected override Effect CreateEffectHook(string effectFileName)
        {
            Effect effectFromDisk = Game.Content.Load<Effect>(effectFileName);
            return new ShadowReceiverEffect(Game.GraphicsDevice, effectFromDisk);
        }
    }
}
