using System.Collections.Generic;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Draws physics boxes.
    /// </summary>
    public class PhysicsBoxDrawer
    {
        /// <summary>
        /// Create a PhysicsBoxDrawer.
        /// </summary>
        /// <param name="game">The game.</param>
        public PhysicsBoxDrawer(XiGame game)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            linesEffect = new BasicEffect(game.GraphicsDevice, null);
            linesEffect.LightingEnabled = false;
            linesEffect.VertexColorEnabled = true;
            linesVertexDeclaration = new VertexDeclaration(game.GraphicsDevice, VertexPositionColor.VertexElements);
        }

        /// <summary>
        /// Are the physics boxes visible?
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        /// <summary>
        /// Draw the physics boxes.
        /// </summary>
        /// <param name="view">The camera view.</param>
        /// <param name="projection">The camera projection.</param>
        public void Draw(Matrix view, Matrix projection)
        {
            if (!CanDraw) return;
            PopulateLinesEffect(view, projection);
            PopulateGraphicsDeviceVertexDeclaration();
            PopulateLines();
            DrawLines();
            ClearLines();
        }

        private bool CanDraw { get { return game.SceneSpace.Entities.Count > 0; } }

        private int LineCount { get { return game.SceneSpace.Entities.Count * 12; } }

        private void PopulateLinesEffect(Matrix view, Matrix projection)
        {
            linesEffect.World = Matrix.Identity;
            linesEffect.View = view;
            linesEffect.Projection = projection;
        }

        private void PopulateGraphicsDeviceVertexDeclaration()
        {
            game.GraphicsDevice.VertexDeclaration = linesVertexDeclaration;
        }

        private void PopulateLines()
        {
            foreach (Entity entity in game.SceneSpace.Entities)
            {
                Color color = GetLineColor(entity);
                Vector3[] boundingBoxCorners = entity.BoundingBox.GetCorners();
                for (int i = 0; i < boundingBoxIndices.Length; ++i)
                    boundingBoxLines.Add(new VertexPositionColor(boundingBoxCorners[boundingBoxIndices[i]], color));
            }
        }

        private void DrawLines()
        {
            linesEffect.Begin();
            foreach (EffectPass pass in linesEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, boundingBoxLines.ToArray(), 0, LineCount);
                pass.End();
            }
            linesEffect.End();
        }

        private void ClearLines()
        {
            boundingBoxLines.Clear();
        }

        private static Color GetLineColor(Entity entity)
        {
            Actor actor = XiHelper.Cast<Actor>(entity.Tag);
            if (actor == null) return Color.Magenta;
            if (actor.Selected) return Color.Red;
            if (!actor.ViewSelectable) return Color.LightBlue;
            return Color.Green;
        }

        private static readonly int[] boundingBoxIndices = { 0, 1, 0, 3,
                                                             0, 4, 1, 2,
                                                             1, 5, 2, 3,
                                                             2, 6, 3, 7,
                                                             4, 5, 4, 7,
                                                             5, 6, 6, 7 };
        private readonly List<VertexPositionColor> boundingBoxLines = new List<VertexPositionColor>();
        private readonly XiGame game;
        private readonly BasicEffect linesEffect;
        private readonly VertexDeclaration linesVertexDeclaration;
        private bool visible = true;
    }
}
