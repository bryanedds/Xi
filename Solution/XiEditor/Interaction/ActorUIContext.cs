using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public class ActorUIContext : ActorContext<ActorUI>
    {
        public ActorUIContext(XiGame game, EditorController controller) : base(game, controller) { }

        protected sealed override ActorDragger<ActorUI> CreateDragger()
        {
            return new ActorUIDragger(Game, Controller);
        }

        protected sealed override ActorUI GetPickedActor(Vector2 mousePosition)
        {
            KeyValuePair<float?, ActorUI> nearest = new KeyValuePair<float?, ActorUI>();
            foreach (ActorUI actor in ViewSelectableActors)
                if ((nearest.Key == null || actor.Position.Z < nearest.Key) &&
                    actor.Bounds.Contains((int)(mousePosition.X), (int)(mousePosition.Y)))
                    nearest = new KeyValuePair<float?, ActorUI>(actor.Position.Z, actor);
            return nearest.Value;
        }

        protected sealed override void InitializeCreatedActor(ActorUI actor, Vector2 canvasPosition)
        {
            actor.PositionXY = canvasPosition.GetSnap(Controller.TranslationSnap);
        }

        protected sealed override void InitializePastedActor(ActorUI actor, Vector2 canvasPosition)
        {
            actor.PositionXY = canvasPosition;
        }
    }
}
