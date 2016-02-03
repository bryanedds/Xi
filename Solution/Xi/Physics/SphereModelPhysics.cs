using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Implement sphere physics for a simple model.
    /// </summary>
    public class SphereModelPhysics : Disposable, IModelPhysics
    {
        /// <summary>
        /// Create sphere model physics.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="model">The visual model.</param>
        /// <param name="position">The position of the sphere entity.</param>
        /// <param name="mass">The physics mass.</param>
        public SphereModelPhysics(XiGame game, Model model, Vector3 position, float mass)
        {
            this.game = game;
            Vector3[] vertices;
            int[] indices;
            model.GetVerticesAndIndices(out vertices, out indices);
            BoundingSphere boundingSphere = BoundingSphere.CreateFromPoints(vertices);
            body = new Sphere(position, boundingSphere.Radius, mass);
            game.SceneSpace.Add(body);
        }

        /// <summary>
        /// The sphere entity.
        /// </summary>
        public Entity Entity { get { return body; } }

        private readonly XiGame game;
        private Entity body;
    }
}
