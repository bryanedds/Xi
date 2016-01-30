using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Helper class for to creating geometry for grids.
    /// </summary>
    public static class GridGeometry
    {
        /// <summary>
        /// Create the geometry for a flat grid with the center at origin that is facing up.
        /// </summary>
        /// <param name="device">Where the VertexDeclaration will be created.</param>
        /// <param name="gridDims">The dimension of the grid in terms of quads.</param>
        /// <param name="quadScale">The scale of each quad.</param>
        /// <param name="textureRepetition">The amount of times the texture repeats.</param>
        public static Geometry Create<V>(
            GraphicsDevice device,
            Point gridDims,
            Vector2 quadScale,
            Vector2 textureRepetition)
            where V : IVertices
        {
            XiHelper.ArgumentNullCheck(device);

            // resolution
            Point resolution = new Point(gridDims.X + 1, gridDims.Y + 1);

            // populate vertices
            Vector3[,] vertexMap = CreateFlatVertexMap(gridDims, quadScale);
            IVertices verts = VerticesHelper.CreateVertices<V>(resolution.X * resolution.Y);
            PopulatePositionsAndTexCoords(verts, vertexMap, gridDims, quadScale, textureRepetition);
            PopulateNormals(verts, gridDims, Vector3.Up);

            // populate indices
            int[] inds = new int[gridDims.X * gridDims.Y * 6];
            PopulateIndices(inds, gridDims);

            // create the geometry
            return new Geometry(device, PrimitiveType.TriangleList, verts, inds);
        }

        private static Vector3[,] CreateFlatVertexMap(Point gridDims, Vector2 quadScale)
        {
            Point resolution = new Point(gridDims.X + 1, gridDims.Y + 1);
            Vector2 gridScale = new Vector2(gridDims.X * quadScale.X, gridDims.Y * quadScale.Y);
            Vector2 gridCenterOffset = new Vector2(gridScale.X * -0.5f, gridScale.Y * -0.5f);
            Vector3[,] vertexMap = new Vector3[resolution.X, resolution.Y];
            for (int i = 0; i < resolution.X; ++i)
                for (int j = 0; j < resolution.Y; ++j)
                    vertexMap[i, j] = new Vector3(
                        quadScale.X * i + gridCenterOffset.X,
                        0,
                        quadScale.Y * j + gridCenterOffset.Y);
            return vertexMap;
        }

        private static void PopulatePositionsAndTexCoords(
            IVertices verts,
            Vector3[,] vertexMap,
            Point gridDims,
            Vector2 quadScale,
            Vector2 textureRepetition)
        {
            Point resolution = new Point(gridDims.X + 1, gridDims.Y + 1);
            Vector2 quadTextureScale = new Vector2(textureRepetition.X / gridDims.X, textureRepetition.Y / gridDims.Y);
            for (int x = 0; x < resolution.X; ++x)
            {
                for (int y = 0; y < resolution.Y; ++y)
                {
                    int offset = x + y * resolution.X;
                    Vector2 texCoord = quadTextureScale * new Vector2(x, y);
                    verts.SetPosition(offset, vertexMap[x, y]);
                    verts.SetTexCoord(offset, texCoord);
                }
            }
        }

        private static void PopulateNormals(IVertices verts, Point gridDims, Vector3 normal)
        {
            Point resolution = new Point(gridDims.X + 1, gridDims.Y + 1);
            for (int x = 0; x < resolution.X; ++x)
                for (int y = 0; y < resolution.Y; ++y)
                    verts.SetNormal(x + y * resolution.X, normal);
        }

        private static void PopulateIndices(int[] inds, Point gridDims)
        {
            Point resolution = new Point(gridDims.X + 1, gridDims.Y + 1);
            for (int x = 0; x < gridDims.X; ++x)
            {
                for (int y = 0; y < gridDims.Y; ++y)
                {
                    int offset = (x + y * (gridDims.X)) * 6;
                    inds[offset + 3] = (x + 1) + (y + 1) * resolution.X;
                    inds[offset + 4] = x + y * resolution.X;
                    inds[offset + 5] = (x + 1) + y * resolution.X;
                    inds[offset + 0] = (x + 1) + (y + 1) * resolution.X;
                    inds[offset + 1] = x + (y + 1) * resolution.X;
                    inds[offset + 2] = x + y * resolution.X;
                }
            }
        }
    }
}
