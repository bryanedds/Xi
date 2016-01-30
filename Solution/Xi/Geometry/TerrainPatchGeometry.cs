using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Helper class for to creating geometry for terrain patches.
    /// </summary>
    public static class TerrainPatchGeometry
    {
        /// <summary>
        /// Create the geometry for a terrain patch.
        /// </summary>
        /// <param name="device">Where the VertexDeclaration will be created.</param>
        /// <param name="heightMap">The height map that defines the topography of the patch.</param>
        /// <param name="portion">The portion of the height map to use to define the topography of the patch.</param>
        /// <param name="quadScale">The scale of each patch quad.</param>
        /// <param name="quadTextureScale">How far the texture stretches over each quad.</param>
        public static Geometry Create<V>(
            GraphicsDevice device,
            HeightMap heightMap,
            Rectangle portion,
            Vector3 quadScale,
            Vector2 quadTextureScale)
            where V : IVertices
        {
            XiHelper.ArgumentNullCheck(device, heightMap);

            // calculate some intermediate values for this poorly-factored method
            Vector3[,] vertexMap = HeightMapToVertexMap(heightMap, portion);
            float rx = quadScale.X / quadScale.Y;
            float rz = quadScale.Z / quadScale.Y;

            // create vertices
            IVertices verts = VerticesHelper.CreateVertices<V>(portion.Width * portion.Height);

            // populate vertex positions and texture coordinates
            for (int x = 0; x < portion.Width; ++x)
            {
                for (int y = 0; y < portion.Height; ++y)
                {
                    int offset = x + y * portion.Width;
                    Vector2 texCoord = quadTextureScale * new Vector2(portion.Left + x, portion.Top + y);
                    verts.SetPosition(offset, vertexMap[x, y]);
                    verts.SetTexCoord(offset, texCoord);
                }
            }

            // populate inner vertex normals
            for (int x = portion.Left; x < portion.Right; ++x)
            {
                for (int y = portion.Top; y < portion.Bottom; ++y)
                {
                    // disregard height points on the x edges
                    if (x != 0 && x != heightMap.GetLength(0) - 1)
                    {
                        // disregard height points on the y edges
                        if (y != 0 && y != heightMap.GetLength(1) - 1)
                        {
                            int vertexX = x - portion.Left;
                            int vertexY = y - portion.Top;                            
                            Vector3 origin = new Vector3(0, heightMap[x, y], 0);
                            Vector3 v1 = origin - new Vector3(+rx, heightMap[x+1, y+1], +rz);
                            Vector3 v2 = origin - new Vector3(-rx, heightMap[x-1, y+1], +rz);
                            Vector3 v3 = origin - new Vector3(-rx, heightMap[x-1, y-1], -rz);
                            Vector3 v4 = origin - new Vector3(+rx, heightMap[x+1, y-1], -rz);
                            Vector3 n1 = Vector3.Cross(v4, v3);
                            Vector3 n2 = Vector3.Cross(v2, v1);
                            verts.SetNormal(vertexX + vertexY * portion.Width, Vector3.Normalize(n1 + n2));
                        }
                    }
                }
            }

            // create indices
            int[] inds = new int[(portion.Width - 1) * (portion.Height - 1) * 6];

            // populate indices
            for (int x = 0; x < portion.Width - 1; ++x)
            {
                for (int y = 0; y < portion.Height - 1; ++y)
                {
                    int offset = (x + y * (portion.Width - 1)) * 6;
                    inds[offset + 3] = (x + 1) + (y + 1) * portion.Width;
                    inds[offset + 4] = x + y * portion.Width;
                    inds[offset + 5] = (x + 1) + y * portion.Width;
                    inds[offset + 0] = (x + 1) + (y + 1) * portion.Width;
                    inds[offset + 1] = x + (y + 1) * portion.Width;
                    inds[offset + 2] = x + y * portion.Width;
                }
            }

            return new Geometry(device, PrimitiveType.TriangleList, verts, inds);
        }

        /// <summary>
        /// Smooth out a height map using averaging.
        /// </summary>
        public static HeightMap SmoothenHeightMap(HeightMap heightMap, float smoothingFactor)
        {
            XiHelper.ArgumentNullCheck(heightMap);
            smoothingFactor = MathHelper.Clamp(smoothingFactor, 0, 1);
            if (smoothingFactor == 0) return heightMap;

            Point coordinate = new Point(heightMap.GetLength(0), heightMap.GetLength(1));
            float[,] newPoints = new float[coordinate.X, coordinate.Y];
            for (int x = 1; x < coordinate.X - 1; ++x)
            {
                for (int y = 1; y < coordinate.Y - 1; ++y)
                {
                    float total =
                        heightMap[x - 1, y - 1] +
                        heightMap[x, y - 1] +
                        heightMap[x + 1, y - 1] +
                        heightMap[x - 1, y] +
                        heightMap[x, y] +
                        heightMap[x, y + 1] +
                        heightMap[x - 1, y + 1] +
                        heightMap[x, y + 1] +
                        heightMap[x + 1, y + 1];

                    float average = total / 9.0f;
                    average *= smoothingFactor;

                    float current = heightMap[x, y] * (1 - smoothingFactor);
                    newPoints[x, y] = average + current;
                }
            }

            return new HeightMap(newPoints, heightMap.QuadScale, heightMap.Offset);
        }

        /// <summary>
        /// Generate a HeightMap from the red color component in the passed texture.
        /// </summary>
        public static HeightMap TextureToHeightMap(
            Texture2D heightMapTexture,
            Vector3 quadScale,
            Vector2 gridCenterOffset)
        {
            XiHelper.ArgumentNullCheck(heightMapTexture);

            // initialize witdh and height vars
            int width = heightMapTexture.Width;
            int height = heightMapTexture.Height;

            // initialize the height map 2D array
            float[,] heightMapPoints = new float[width, height];

            // initialize the Color array
            Color[] heightMapColors = new Color[width * height];

            // populate the Color array
            heightMapTexture.GetData(heightMapColors);

            // populate the HeightMap object with red component of the Color array
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    heightMapPoints[x, y] = heightMapColors[x + y * width].R / 255.0f;

            return new HeightMap(heightMapPoints, quadScale, gridCenterOffset);
        }

        /// <summary>
        /// Generate a VertexMap from a HeightMap.
        /// </summary>
        public static Vector3[,] HeightMapToVertexMap(HeightMap map, Rectangle portion)
        {
            XiHelper.ArgumentNullCheck(map);
            int width = portion.Width;
            int height = portion.Height;
            int wOffset = portion.Left;
            int hOffset = portion.Top;
            Vector3[,] vertexMap = new Vector3[width, height];
            for (int i = portion.Left; i < portion.Right; ++i)
                for (int j = portion.Top; j < portion.Bottom; ++j)
                    vertexMap[i - wOffset, j - hOffset] = HeightMapToVertex(map, new Point(i, j));
            return vertexMap;
        }

        /// <summary>
        /// Pull a single vertex from a HeightMap.
        /// </summary>
        public static Vector3 HeightMapToVertex(HeightMap map, Point index)
        {
            XiHelper.ArgumentNullCheck(map);
            return new Vector3(
                index.X * map.QuadScale.X + map.Offset.X,
                map[index.X, index.Y] * map.QuadScale.Y,
                index.Y * map.QuadScale.Z + map.Offset.Y);
        }
    }
}
