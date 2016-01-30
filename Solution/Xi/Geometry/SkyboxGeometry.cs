using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Helper class for to creating geometry for sky boxes.
    /// </summary>
    public static class SkyboxGeometry
    {
        /// <summary>
        /// Create the geometry for a quad.
        /// </summary>
        public static Geometry Create<V>(GraphicsDevice device) where V : IVertices
        {
            XiHelper.ArgumentNullCheck(device);

            // create the verts
            IVertices verts = VerticesHelper.CreateVertices<V>(36);

            /*
             * These are the cube points we're constructing -
             * 
             *  E-----G
             *  |\    |\
             *  | C-----A
             *  F-|---H |
             *   \|    \|
             *    D-----B
             */

            // populate cube points
            Vector3 p = new Vector3(0.5f);
            Vector3 a = new Vector3(+p.X, +p.Y, +p.Z);
            Vector3 b = new Vector3(+p.X, -p.Y, +p.Z);
            Vector3 c = new Vector3(-p.X, +p.Y, +p.Z);
            Vector3 d = new Vector3(-p.X, -p.Y, +p.Z);
            Vector3 e = new Vector3(-p.X, +p.Y, -p.Z);
            Vector3 f = new Vector3(-p.X, -p.Y, -p.Z);
            Vector3 g = new Vector3(+p.X, +p.Y, -p.Z);
            Vector3 h = new Vector3(+p.X, -p.Y, -p.Z);

            // populate positions - triangles are generated in clock-wise winding order starting at
            // A and moving left around the cube. Then the top starting with G in clock-wise
            // winding order, then finally the bottom starting with B in clock-wise winding order.
            verts.SetPosition(0, a); verts.SetPosition(1, b); verts.SetPosition(2, c);
            verts.SetPosition(3, b); verts.SetPosition(4, d); verts.SetPosition(5, c);
            verts.SetPosition(6, c); verts.SetPosition(7, d); verts.SetPosition(8, e);
            verts.SetPosition(9, d); verts.SetPosition(10, f); verts.SetPosition(11, e);
            verts.SetPosition(12, e); verts.SetPosition(13, f); verts.SetPosition(14, g);
            verts.SetPosition(15, f); verts.SetPosition(16, h); verts.SetPosition(17, g);
            verts.SetPosition(18, g); verts.SetPosition(19, h); verts.SetPosition(20, a);
            verts.SetPosition(21, h); verts.SetPosition(22, b); verts.SetPosition(23, a);
            verts.SetPosition(24, g); verts.SetPosition(25, a); verts.SetPosition(26, e);
            verts.SetPosition(27, a); verts.SetPosition(28, c); verts.SetPosition(29, e);
            verts.SetPosition(30, b); verts.SetPosition(31, h); verts.SetPosition(32, d);
            verts.SetPosition(33, h); verts.SetPosition(34, f); verts.SetPosition(35, d);

            /*
             * These are the texture coordinates we're using -
             * 
             * (0,0)--(1,0)
             *  |        |
             *  |        |
             *  |        |
             * (0,1)--(1,1)
             */

            // populate texture coordinates
            for (int i = 0; i < 36; i += 6)
            {
                verts.SetTexCoord(i + 0, new Vector2(1, 0));
                verts.SetTexCoord(i + 1, new Vector2(1, 1));
                verts.SetTexCoord(i + 2, new Vector2(0, 0));
                verts.SetTexCoord(i + 3, new Vector2(1, 1));
                verts.SetTexCoord(i + 4, new Vector2(0, 1));
                verts.SetTexCoord(i + 5, new Vector2(0, 0));
            }

            // populate normals
            for (int i = 0; i < 36; i += 3)
            {
                Vector3 v1 = verts.GetPosition(i + 1) - verts.GetPosition(i);
                Vector3 v2 = verts.GetPosition(i + 2) - verts.GetPosition(i);
                Vector3 normal = Vector3.Cross(v1, v2);
                for (int j = 0; j < 3; ++j) verts.SetNormal(i + j, normal);
            }

            return new Geometry(device, PrimitiveType.TriangleList, verts);
        }
    }
}
