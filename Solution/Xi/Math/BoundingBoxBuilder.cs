using Microsoft.Xna.Framework;

namespace Xi
{
    /// <summary>
    /// Builds a hierarchically-transformed bounding box.
    /// </summary>
    public class BoundingBoxBuilder
    {
        public BoundingBoxBuilder(BoundingBox source)
        {
            this.source = source;
            RefreshBoundingBoxWorld();
        }

        public BoundingBox BoundingBoxSource { get { return source; } }

        public BoundingBox BoundingBoxWorld { get { return world; } }

        public Matrix WorldTransform
        {
            get { return worldTransform; }
            set
            {
                worldTransform = value;
                RefreshBoundingBoxWorld();
            }
        }

        private void RefreshBoundingBoxWorld()
        {
            BoundingBox refPassableSource = source;
            BoundingBoxHelper.Transform(ref refPassableSource, ref worldTransform, out world);
        }

        private readonly BoundingBox source;
        private BoundingBox world;
        private Matrix worldTransform = Matrix.Identity;
    }
}
