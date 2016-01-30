using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Helper methods for XNA's Plane struct.
    /// </summary>
    public static class PlaneHelper
    {
        /// <summary>
        /// Create a plane at the specified position and normal.
        /// </summary>
        public static Plane CreateFromPositionAndNormal(Vector3 position, Vector3 normal)
        {
            float d;
            Vector3.Dot(ref position, ref normal, out d);
            return new Plane(normal, -d);
        }

        /// <summary>
        /// Calculate a position on the plane.
        /// </summary>
        public static Vector3 GetPosition(this Plane plane)
        {
            return plane.Normal * -plane.D;
        }

        /// <summary>
        /// Calculate the projection of a vector onto the plane.
        /// </summary>
        public static Vector3 Project(this Plane plane, Vector3 a)
        {
            // Formula: A || B = B x (A x B / |B|) / |B|
            Vector3 b = plane.GetPosition();
            float c = b.Length();
            return Vector3.Cross(b, (Vector3.Cross(a, b) / c) / c);
        }
    }
}
