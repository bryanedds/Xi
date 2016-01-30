using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Helper class for to creating geometry for quads that cover the screen.
    /// </summary>
    public static class ScreenQuadGeometry
    {
        /// <summary>
        /// Create the geometry for a quad that covers the screen (homogenous clip space).
        /// </summary>
        public static Geometry Create<V>(GraphicsDevice device) where V : IVertices
        {
            XiHelper.ArgumentNullCheck(device);

            IVertices verts = VerticesHelper.CreateVertices<V>(4);

            Vector2 min = -Vector2.One;
            Vector2 max = Vector2.One;

            verts.SetPosition(0, new Vector3(max.X, min.Y, 0));
            verts.SetPosition(1, new Vector3(min.X, min.Y, 0));
            verts.SetPosition(2, new Vector3(min.X, max.Y, 0));
            verts.SetPosition(3, new Vector3(max.X, max.Y, 0));

            min = Vector2.Zero;
            max = Vector2.One;

            verts.SetTexCoord(0, new Vector2(max.X, max.Y));
            verts.SetTexCoord(1, new Vector2(min.X, max.Y));
            verts.SetTexCoord(2, new Vector2(min.X, min.Y));
            verts.SetTexCoord(3, new Vector2(max.X, min.Y));

            int[] inds = new int[6];
            inds[0] = 0; inds[1] = 1; inds[2] = 2;
            inds[3] = 2; inds[4] = 3; inds[5] = 0;

            return new Geometry(device, PrimitiveType.TriangleList, verts, inds);
        }
    }
}
