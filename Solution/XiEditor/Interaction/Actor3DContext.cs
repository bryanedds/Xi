using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xi;

namespace XiEditor
{
    public class Actor3DContext : ActorContext<Actor3D>
    {
        public Actor3DContext(XiGame game, EditorController controller) : base(game, controller) { }

        protected sealed override ActorDragger<Actor3D> CreateDragger()
        {
            return new Actor3DDragger(Game, Controller);
        }

        protected sealed override Actor3D GetPickedActor(Vector2 mousePosition)
        {
            Segment mousePositionWorld = Controller.CanvasViewport.ToSegment(Game.Camera, mousePosition);
            KeyValuePair<Actor3D, float> nearest = new KeyValuePair<Actor3D, float>(null, float.MaxValue);
            foreach (Actor3D actor in ViewSelectableActors) Approach(ref mousePositionWorld, ref nearest, actor);
            return nearest.Key;
        }

        protected sealed override void InitializeCreatedActor(Actor3D actor, Vector2 canvasPosition)
        {
            if (!actor.Amorphous) actor.Position = GetCreationPosition(canvasPosition);
        }

        protected sealed override void InitializePastedActor(Actor3D actor, Vector2 canvasPosition)
        {
            if (!actor.Amorphous) actor.Position = ToPosition3D(canvasPosition);
        }

        private Vector3 GetCreationPosition(Vector2 canvasPosition)
        {
            return
                ToPosition3D(canvasPosition).
                GetSnap(Controller.TranslationSnap);
        }

        private void Approach(ref Segment mousePositionWorld, ref KeyValuePair<Actor3D, float> nearest, Actor3D current)
        {
            float distance;
            BoundingBox boundingBox = current.BoundingBox;
            if (XiMathHelper.Intersection(ref boundingBox, ref mousePositionWorld, out distance) &&
                distance < nearest.Value)
                nearest = new KeyValuePair<Actor3D, float>(current, distance);
        }

        private Vector3 ToPosition3D(Vector2 canvasPosition)
        {
            Camera camera = Game.Camera;
            Viewport viewport = Game.GraphicsDevice.Viewport;
            Vector3 cameraForward = Controller.CameraForward;
            Vector2 mousePositionViewport = canvasPosition;
            Segment mousePositionWorld = viewport.ToSegment(camera, mousePositionViewport);
            Ray creationRay = new Ray(mousePositionWorld.End, mousePositionWorld.Direction);
            Vector3 creationPlanePosition = camera.Position + cameraForward * Controller.CreationDepth;
            Plane creationPlane = XiMathHelper.CreatePlaneFromPositionAndNormal(creationPlanePosition, cameraForward);
            Vector3 rayPlaneIntersection;
            XiMathHelper.Intersection(ref creationRay, ref creationPlane, out rayPlaneIntersection);
            return rayPlaneIntersection.GetSnap(Controller.TranslationSnap);
        }
    }
}
