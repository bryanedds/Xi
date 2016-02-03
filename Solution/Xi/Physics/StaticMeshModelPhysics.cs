﻿using BEPUphysics;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Implement static mesh physics for a simple model.
    /// </summary>
    public class StaticMeshModelPhysics : Disposable, IModelPhysics
    {
        /// <summary>
        /// Create sphere model physics.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="model">The visual model.</param>
        /// <param name="position">The position of the static mesh entity.</param>
        /// <param name="mass">The physics mass.</param>
        public StaticMeshModelPhysics(XiGame game, Model model, Vector3 position, float mass)
        {
            this.game = game;
            StaticTriangleGroup.StaticTriangleGroupVertex[] vertices;
            int[] indices;
            model.GetVerticesAndIndices(out vertices, out indices);
            TriangleMesh triangleMesh = new TriangleMesh(vertices, indices);
            staticTriangleGroup = new StaticTriangleGroup(triangleMesh);
            game.SceneSpace.Add(staticTriangleGroup);
        }

        /// <summary>
        /// The static mesh entity.
        /// </summary>
        public Entity Entity { get { return null; } }

        private readonly XiGame game;
        private StaticTriangleGroup staticTriangleGroup;
    }
}
