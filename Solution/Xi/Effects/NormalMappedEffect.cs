using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents a shader effect that draws normal mapped geometry.
    /// </summary>
    public class NormalMappedEffect : ShadowReceiverEffect
    {
        /// <summary>
        /// Initializes a new instance of NormalMappedEffect by cloning an existing effect.
        /// </summary>
        /// <param name="device">The graphics device that will create the effect.</param>
        /// <param name="cloneSource">The effect to clone.</param>
        public NormalMappedEffect(GraphicsDevice device, Effect cloneSource)
            : base(device, cloneSource)
        {
            normalMapParam = Parameters["xNormalMap"];
            normalMapEnabledParam = Parameters["xNormalMapEnabled"];
        }
        
        /// <summary>
        /// The normal map texture.
        /// </summary>
        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                if (_normalMap == value) return; // OPTIMIZATION
                _normalMap = value;
                normalMapParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// Is normal mapping enabled?
        /// </summary>
        public bool NormalMapEnabled
        {
            get { return _normalMapEnabled; }
            set
            {
                if (_normalMapEnabled == value) return; // OPTIMIZATION
                _normalMapEnabled = value;
                normalMapEnabledParam.TrySetValue(value);
            }
        }
        
        private readonly EffectParameter normalMapParam;
        private readonly EffectParameter normalMapEnabledParam;
        private Texture2D _normalMap;
        private bool _normalMapEnabled;
    }
}
