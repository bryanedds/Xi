using System;
using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Represents a three-dimensional triangle.
    /// </summary>
    public struct Triangle : IEquatable<Triangle>
    {
        /// <summary>
        /// Create a Triangle.
        /// </summary>
        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// The normal of the triangle.
        /// </summary>
        public Vector3 Normal
        {
            get { return Vector3.Normalize(Vector3.Cross(B - A, C - B)); }
        }

        /// <inheritdoc />
        public bool Equals(Triangle other)
        {
            return
                A == other.A &&
                B == other.B &&
                C == other.C;
        }

        /// <summary>
        /// The first point on (and the origin of) the triangle.
        /// </summary>
        public Vector3 A;
        /// <summary>
        /// The second point on the triangle.
        /// </summary>
        public Vector3 B;
        /// <summary>
        /// The third point on the triangle.
        /// </summary>
        public Vector3 C;
    }
}
