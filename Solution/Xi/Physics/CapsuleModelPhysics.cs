using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Implement capsul physics for a simple model.
    /// </summary>
    public class CapsuleModelPhysics : Disposable, IModelPhysics
    {
        /// <summary>
        /// Create capsule model physics.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="model">The visual model.</param>
        /// <param name="position">The position of the sphere entity.</param>
        /// <param name="mass">The physics mass.</param>
        public CapsuleModelPhysics(XiGame game, Model model, Vector3 position, float mass)
        {
            this.game = game;
            Vector3[] vertices;
            int[] indices;
            model.GetVerticesAndIndices(out vertices, out indices);
            BoundingSphere boundingSphere = BoundingSphere.CreateFromPoints(vertices);
            body = new Capsule(position, boundingSphere.Radius, boundingSphere.Radius * 0.5f, mass);
            game.SceneSpace.Add(body);
        }

        /// <summary>
        /// The sphere entity.
        /// </summary>
        public Entity Entity { get { return body; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing) game.SceneSpace.Remove(body);
            base.Dispose(disposing);
        }

        private readonly XiGame game;
        private Entity body;
    }
}
