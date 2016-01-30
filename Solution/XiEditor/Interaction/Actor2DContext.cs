using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public class Actor2DContext : ActorContext<Actor2D>
    {
        public Actor2DContext(XiGame game, EditorController controller) : base(game, controller) { }

        protected sealed override ActorDragger<Actor2D> CreateDragger()
        {
            return new Actor2DDragger(Game, Controller);
        }

        protected sealed override Actor2D GetPickedActor(Vector2 mousePosition)
        {
            Vector2 mousePosition2D = ToPosition2D(mousePosition);
            KeyValuePair<float?, Actor2D> nearest = new KeyValuePair<float?, Actor2D>();
            foreach (Actor2D actor in ViewSelectableActors)
                if ((nearest.Key == null || actor.Position.Z < nearest.Key) && actor.IsAt(mousePosition2D))
                    nearest = new KeyValuePair<float?, Actor2D>(actor.Position.Z, actor);
            return nearest.Value;
        }

        protected sealed override void InitializeCreatedActor(Actor2D actor, Vector2 canvasPosition)
        {
            actor.PositionXY = GetCreationPosition(canvasPosition);
        }

        protected sealed override void InitializePastedActor(Actor2D actor, Vector2 canvasPosition)
        {
            actor.PositionXY = ToPosition2D(canvasPosition);
        }

        private Vector2 GetCreationPosition(Vector2 canvasPosition)
        {
            return
                ToPosition2D(canvasPosition).
                GetSnap(Controller.TranslationSnap);
        }

        private Vector2 ToPosition2D(Vector2 mousePosition)
        {
            Vector2 cameraPositionXY = Game.Camera.Position.GetXY();
            return mousePosition - new Vector2(-cameraPositionXY.X, cameraPositionXY.Y); // BUG: algorithm duplicated elsewhere
        }
    }
}
