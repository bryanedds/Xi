using BEPUphysics;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Implement physics for a terrain.
    /// </summary>
    public class TerrainPhysics : Disposable
    {
        /// <summary>
        /// Create terrain physics.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="terrain">The visual terrain.</param>
        public TerrainPhysics(XiGame game, Terrain terrain)
        {
            this.game = game;
            
            HeightMap heightMap = terrain.HeightMap;
            int width = heightMap.GetLength(0);
            int depth = heightMap.GetLength(1);
            Vector3 quadScale = heightMap.QuadScale;
            float[,] points = new float[width, depth];
            for (int x = 0; x < width; ++x)
                for (int z = 0; z < depth; ++z)
                    points[x, z] = heightMap.Points[x, z] * quadScale.Y;

            body = new BEPUphysics.Terrain(new Vector3(heightMap.Offset.X, 0.0f, heightMap.Offset.Y));
            body.SetData(points, QuadFormats.UpperLeftLowerRight, quadScale.X, quadScale.Z);
            body.AllowedPenetration = 1.0f;
            game.SceneSpace.Add(body);
        }

        /// <summary>
        /// The friction.
        /// </summary>
        public float Friction
        {
            get { return body.Friction; }
            set { body.Friction = value; }
        }

        /// <summary>
        /// The bounciness.
        /// </summary>
        public float Bounciness
        {
            get { return body.Bounciness; }
            set { body.Bounciness = value; }
        }

        /// <summary>
        /// The collision rules.
        /// </summary>
        public EntityCollisionRules CollisionRules
        {
            get { return body.CollisionRules; }
            set { body.CollisionRules = value; }
        }

        /// <summary>
        /// The static mesh entity.
        /// </summary>
        public Entity Entity { get { return null; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing) game.SceneSpace.Remove(body);
            base.Dispose(disposing);
        }

        private readonly XiGame game;
        private readonly BEPUphysics.Terrain body;
    }
}
