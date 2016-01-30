using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// A surface that is normal mapped.
    /// </summary>
    public class NormalMappedSurface : ShadowReceiverSurface
    {
        /// <summary>
        /// Create a NormalMappedSurface.
        /// </summary>
        public NormalMappedSurface(XiGame game, NormalMappedModel actor, string effectFileName, int meshIndex, int partIndex)
            : base(game, actor, effectFileName, meshIndex, partIndex)
        {
            NormalMapFileName = "Xi/3D/waves";
        }

        /// <summary>
        /// The normal map.
        /// </summary>
        public Texture2D NormalMap { get { return _normalMap; } }

        /// <summary>
        /// The normal map file name.
        /// </summary>
        public string NormalMapFileName
        {
            get { return _normalMapFileName; }
            set
            {
                XiHelper.ArgumentNullCheck(value);
                if (_normalMapFileName == value) return; // OPTIMIZATION
                Texture2D newNormalMap = Game.Content.Load<Texture2D>(value);
                _normalMap = newNormalMap;
                _normalMapFileName = value;
            }
        }
        
        /// <summary>
        /// Is normal mapping enabled?
        /// </summary>
        public bool NormalMapEnabled
        {
            get { return normalMapEnabled; }
            set { normalMapEnabled = value; }
        }

        /// <inheritdoc />
        protected override void PopulateEffectHook(GameTime gameTime, Camera camera, string drawMode)
        {
            base.PopulateEffectHook(gameTime, camera, drawMode);
            NormalMappedEffect nmEffect = XiHelper.Cast<NormalMappedEffect>(Effect);
            nmEffect.NormalMap = NormalMap;
            nmEffect.NormalMapEnabled = normalMapEnabled;
        }

        /// <inheritdoc />
        protected override Effect CreateEffectHook(string effectFileName)
        {
            Effect effectFromDisk = Game.Content.Load<Effect>(effectFileName);
            return new NormalMappedEffect(Game.GraphicsDevice, effectFromDisk);
        }
        
        private bool normalMapEnabled = true;
        private Texture2D _normalMap;
        private string _normalMapFileName = string.Empty;
    }
}
