using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Represents a height map centered on the x,z plane.
    /// </summary>
    public class HeightMap
    {
        /// <summary>
        /// Create a HeightMap.
        /// </summary>
        public HeightMap(float[,] points, Vector3 quadScale, Vector2 offset)
        {
            XiHelper.ArgumentNullCheck(points);
            this.points = XiHelper.Cast<float[,]>(points.Clone());
            this.quadScale = quadScale;
            this.offset = offset;
        }

        /// <summary>
        /// The height points that define the topography.
        /// </summary>
        public float this[int i, int j] { get { return points[i, j]; } }

        /// <summary>
        /// The amount of space between each height point.
        /// </summary>
        public Vector3 QuadScale { get { return quadScale; } }

        /// <summary>
        /// The x,z offset.
        /// </summary>
        public Vector2 Offset { get { return offset; } }

        /// <summary>
        /// The length of the indexer.
        /// </summary>
        public int GetLength(int i) { return points.GetLength(i); }

        /// <summary>
        /// Grab a height from the height map.
        /// </summary>
        /// <param name="position">The x,z position to grab from.</param>
        public float GrabHeight(Vector2 position)
        {
            Triangle triangle;
            GrabTriangle(position, out triangle);
            Ray ray = new Ray(new Vector3(position.X, 0, position.Y), Vector3.Up);
            Vector3 intersection;
            XiMathHelper.Intersection(ref ray, ref triangle, out intersection);
            return intersection.Y;
        }

        /// <summary>Grab a triangle from the height map.</summary>
        /// <param name="position">The x,z position to grab from.</param>
        /// <param name="triangle">
        /// The resulting triangle. If out of range, the resulting triangle will be flat.
        /// </param>
        public void GrabTriangle(Vector2 position, out Triangle triangle)
        {
            Vector2 positionLocal = position - offset;
            Point index = new Point(
                (int)(positionLocal.X / quadScale.X),
                (int)(positionLocal.Y / quadScale.Z));
            if (IsIndexOutOfRange(index, points)) CreateFlatTriangle(out triangle);
            else GrabTriangle(positionLocal, index, out triangle);
        }

        private void GrabTriangle(Vector2 positionLocal, Point index, out Triangle triangle)
        {
            Vector2 positionCellLocal = new Vector2(
                positionLocal.X - index.X * quadScale.X,
                positionLocal.Y - index.Y * quadScale.Z);
            Vector2 positionUnitLocal = new Vector2(
                positionCellLocal.X / quadScale.X,
                positionCellLocal.Y / quadScale.Z);
            if (InUpperRightTriangle(positionUnitLocal)) GrabUpperRightTriangle(index, out triangle);
            else GrabLowerLeftTriangle(index, out triangle);
        }

        private void GrabLowerLeftTriangle(Point index, out Triangle triangle)
        {
            float topLeft = points[index.X, index.Y];
            float bottomLeft = points[index.X, index.Y + 1];
            float bottomRight = points[index.X + 1, index.Y + 1];
            triangle.A = new Vector3(
                index.X * quadScale.X + offset.X,
                topLeft * quadScale.Y,
                index.Y * quadScale.Z + offset.Y);
            triangle.B = new Vector3(
                (index.X + 1) * quadScale.X + offset.X,
                bottomRight * quadScale.Y,
                (index.Y + 1) * quadScale.Z + offset.Y);
            triangle.C = new Vector3(
                index.X * quadScale.X + offset.X,
                bottomLeft * quadScale.Y,
                (index.Y + 1) * quadScale.Z + offset.Y);
        }

        private void GrabUpperRightTriangle(Point index, out Triangle triangle)
        {
            float topLeft = points[index.X, index.Y];
            float topRight = points[index.X + 1, index.Y];
            float bottomRight = points[index.X + 1, index.Y + 1];
            triangle.A = new Vector3(
                index.X * quadScale.X + offset.X,
                topLeft * quadScale.Y,
                index.Y * quadScale.Z + offset.Y);
            triangle.B = new Vector3(
                (index.X + 1) * quadScale.X + offset.X,
                topRight * quadScale.Y,
                index.Y * quadScale.Z + offset.Y);
            triangle.C = new Vector3(
                (index.X + 1) * quadScale.X + offset.X,
                bottomRight * quadScale.Y,
                (index.Y + 1) * quadScale.Z + offset.Y);
        }

        private static bool InUpperRightTriangle(Vector2 positionUnitLocal)
        {
            return positionUnitLocal.X > positionUnitLocal.Y;
        }

        private static bool IsIndexOutOfRange(Point index, float[,] points)
        {
            return
                index.X >= points.GetLength(0) - 1 ||
                index.Y >= points.GetLength(1) - 1 ||
                index.X < 0 ||
                index.Y < 0;
        }

        private static void CreateFlatTriangle(out Triangle triangle)
        {
            triangle.A = new Vector3(0, 0, 0);
            triangle.B = new Vector3(0, 0, 1);
            triangle.C = new Vector3(1, 0, 1);
        }

        private readonly float[,] points;
        private readonly Vector3 quadScale;
        private readonly Vector2 offset;
    }
}
