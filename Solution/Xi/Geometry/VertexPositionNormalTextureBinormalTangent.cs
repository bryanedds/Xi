using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Implements the data structure for a vertex with a position, a normal, texture coordinates,
    /// a binormal, and a tangent.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexPositionNormalTextureBinormalTangent
    {
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
            new VertexElement(0, 24, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, 32, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0),
            new VertexElement(0, 44, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Binormal, 0)
        };
        public const int SizeInBytes = 56;

        [FieldOffset(0)]
        public Vector3 Position;
        [FieldOffset(12)]
        public Vector3 Normal;
        [FieldOffset(24)]
        public Vector2 TextureCoordinate;
        [FieldOffset(32)]
        public Vector3 Binormal;
        [FieldOffset(44)]
        public Vector3 Tangent;
    }
}
