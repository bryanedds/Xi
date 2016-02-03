using System;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Implement box physics for a simple model.
    /// </summary>
    public class BoxModelPhysics : Disposable, IModelPhysics
    {
        /// <summary>
        /// Create box model physics.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="model">The visual model.</param>
        /// <param name="position">The position of the box entity.</param>
        /// <param name="mass">The physics mass.</param>
        public BoxModelPhysics(XiGame game, Model model, Vector3 position, float mass)
        {
            this.game = game;
            Vector3[] vertices;
            int[] indices;
            model.GetVerticesAndIndices(out vertices, out indices);
            BoundingBox boundingBox = BoundingBox.CreateFromPoints(vertices);
            body = new BEPUphysics.Entities.Box(
                position,
                Math.Max(Math.Abs(boundingBox.Max.X), Math.Abs(boundingBox.Min.X)) * 2,
                Math.Max(Math.Abs(boundingBox.Max.Y), Math.Abs(boundingBox.Min.Y)) * 2,
                Math.Max(Math.Abs(boundingBox.Max.Z), Math.Abs(boundingBox.Min.Z)) * 2,
                mass);
            game.SceneSpace.Add(body);
            /*
            this alternative method does not work and I don't know why -
            Box box = new Box(
                (boundingBox.Max + boundingBox.Min) * 0.5f,
                boundingBox.Max.X - boundingBox.Min.X,
                boundingBox.Max.Y - boundingBox.Min.Y,
                boundingBox.Max.Z - boundingBox.Min.Z,
                Mass);
            body = new CompoundBody();
            body.addBody(box);
            game.SceneSpace.Add(body);
            */
        }

        /// <summary>
        /// The box entity.
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
