using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// An entity with no particular form or motion.
    /// </summary>
    public class AmorphousEntity : Entity
    {
        /// <inheritdoc />
        public override void GetExtremePoint(ref Vector3 d, ref Vector3 positionToUse, ref Quaternion orientationToUse, float margin, out Vector3 extremePoint)
        {
            extremePoint = Vector3.Zero;
        }

        /// <inheritdoc />
        public override void GetExtremePoints(ref Vector3 d, out Vector3 min, out Vector3 max, float margin)
        {
            min = max = Vector3.Zero;
        }

        /// <inheritdoc />
        public override void GetExtremePoints(ref Vector3 d, ref Vector3 positionToUse, ref Quaternion orientationToUse, out Vector3 min, out Vector3 max, float margin)
        {
            min = max = Vector3.Zero;
        }

        /// <inheritdoc />
        protected override void Initialize(bool physicallySimulated) { }
    }
}
