using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Helper class for to creating geometry for quads.
    /// </summary>
    public static class QuadGeometry
    {
        /// <summary>
        /// Create the geometry for a quad.
        /// </summary>
        public static Geometry Create<V>(GraphicsDevice device) where V : IVertices
        {
            XiHelper.ArgumentNullCheck(device);

            IVertices verts = VerticesHelper.CreateVertices<V>(4);

            Vector2 min = new Vector2(-0.5f);
            Vector2 max = new Vector2(0.5f);

            verts.SetPosition(0, new Vector3(min.X, 0, min.Y));
            verts.SetPosition(1, new Vector3(min.X, 0, max.Y));
            verts.SetPosition(2, new Vector3(max.X, 0, min.Y));
            verts.SetPosition(3, new Vector3(max.X, 0, max.Y));

            for (int i = 0; i < 4; ++i) verts.SetNormal(i, Vector3.Up);

            min = Vector2.Zero;
            max = Vector2.One;

            verts.SetTexCoord(0, new Vector2(min.X, max.Y));
            verts.SetTexCoord(1, new Vector2(min.X, min.Y));
            verts.SetTexCoord(2, new Vector2(max.X, max.Y));
            verts.SetTexCoord(3, new Vector2(max.X, min.Y));

            int[] inds = new int[6];
            inds[0] = 0; inds[1] = 2; inds[2] = 1;
            inds[3] = 1; inds[4] = 2; inds[5] = 3;

            return new Geometry(device, PrimitiveType.TriangleList, verts, inds);
        }
    }
}
