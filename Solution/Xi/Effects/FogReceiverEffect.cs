using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Represents a shader effect that draws fogged geometry.
    /// </summary>
    public class FogReceiverEffect : BaseEffect
    {
        /// <summary>
        /// Initializes a new instance of FogReceiverEffect by cloning an existing effect.
        /// </summary>
        /// <param name="device">The graphics device that will create the effect.</param>
        /// <param name="cloneSource">The effect to clone.</param>
        public FogReceiverEffect(GraphicsDevice device, Effect cloneSource)
            : base(device, cloneSource)
        {
            fogColorParam = Parameters["xFogColor"];
            fogStartParam = Parameters["xFogStart"];
            fogEndParam = Parameters["xFogEnd"];
            fogEnabledParam = Parameters["xFogEnabled"];
        }

        /// <summary>
        /// The color of the fog.
        /// </summary>
        public Color FogColor
        {
            get { return _fogColor; }
            set
            {
                if (_fogColor == value) return; // OPTIMIZATION
                _fogColor = value;
                fogColorParam.TrySetValue(value.ToVector3());
            }
        }

        /// <summary>
        /// The beginning of the fog.
        /// </summary>
        public float FogStart
        {
            get { return _fogStart; }
            set
            {
                if (_fogStart == value) return; // OPTIMIZATION
                _fogStart = value;
                fogStartParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// The end of the fog.
        /// </summary>
        public float FogEnd
        {
            get { return _fogEnd; }
            set
            {
                if (_fogEnd == value) return; // OPTIMIZATION
                _fogEnd = value;
                fogEndParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// Is fogging enabled?
        /// </summary>
        public bool FogEnabled
        {
            get { return _fogEnabled; }
            set
            {
                if (_fogEnabled == value) return; // OPTIMIZATION
                _fogEnabled = value;
                fogEnabledParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// Populate the fogging effect parameters.
        /// </summary>
        public void PopulateFogging(Fog fog)
        {
            this.FogEnabled = fog.Enabled;
            if (!fog.Enabled) return; // OPTIMIZATION
            this.FogStart = fog.Start;
            this.FogEnd = fog.End;
            this.FogColor = fog.Color;
        }
        
        private readonly EffectParameter fogColorParam;
        private readonly EffectParameter fogStartParam;
        private readonly EffectParameter fogEndParam;
        private readonly EffectParameter fogEnabledParam;
        private Color _fogColor;
        private float _fogStart;
        private float _fogEnd;
        private bool _fogEnabled;
    }
}
