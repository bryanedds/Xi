using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// When and how fast certain input is repeated when an input control is held.
    /// </summary>
    public class RepeatRate
    {
        /// <summary>
        /// The delay before the first repetition.
        /// </summary>
        public float FirstDelay
        {
            get { return firstDelay; }
            set { firstDelay = MathHelper.Clamp(value, 0, float.MaxValue); }
        }

        /// <summary>
        /// The delay between each repetition.
        /// </summary>
        public float RepeatDelay
        {
            get { return repeatDelay; }
            set { repeatDelay = MathHelper.Clamp(value, 0, float.MaxValue); }
        }

        private float firstDelay = 0.5f;
        private float repeatDelay = 0.1f;
    }
}
