using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xi;

namespace XiEditor
{
    public class Actor3DDragger : ActorDragger<Actor3D>
    {
        public Actor3DDragger(XiGame game, EditorController controller) : base(game, controller) { }

        protected enum DragMode
        {
            Position = 0,
            Orientation
        }

        protected override void PrepareDragData(Vector2 dragPosition)
        {
            PrepareDragPosition();
            PrepareDragOrientation();
            PrepareDragOffset(dragPosition);
            PrepareDragMode();
        }

        protected override void DragSelection(Vector2 dragPosition)
        {
            switch (dragMode)
            {
                case DragMode.Position: DragTranslate(GetDragTranslation(dragPosition)); break;
                case DragMode.Orientation: DragOrientDegrees(GetDragOrientationDegrees(dragPosition)); break;
            }
        }

        private void PrepareDragPosition()
        {
            dragSelectedPosition = DraggedActor.Position;
        }

        private void PrepareDragOrientation()
        {
            dragSelectedOrientation = DraggedActor.OrientationEularDegrees;
        }

        private void PrepareDragOffset(Vector2 dragPosition)
        {
            Viewport viewport = Game.GraphicsDevice.Viewport;
            Segment dragPositionWorld = viewport.ToSegment(Game.Camera, dragPosition);
            Vector3 dragPositionObjectPlane = WorldToObjectPlane(dragSelectedPosition, dragPositionWorld.Start);
            dragOffset = dragPositionObjectPlane - DraggedActor.Position;
        }

        private void PrepareDragMode()
        {
            switch (Modifier)
            {
                case KeyboardModifier.Shift: dragMode = DragMode.Orientation; break;
                case KeyboardModifier.None: dragMode = DragMode.Position; break;
            }
        }

        private Vector3 GetDragVector(Vector2 dragPosition)
        {
            Vector3 dragVectorView = GetDragVectorView(dragPosition);
            switch (Controller.AxisConstraint)
            {
                case AxisConstraint.X: return XiMathHelper.ComponentVector(dragVectorView, Vector3.Right);
                case AxisConstraint.Y: return XiMathHelper.ComponentVector(dragVectorView, Vector3.Up);
                case AxisConstraint.Z: return XiMathHelper.ComponentVector(dragVectorView, Vector3.Backward);
                case AxisConstraint.XY: return XiMathHelper.ComponentVector(dragVectorView, Vector3.Right) + XiMathHelper.ComponentVector(dragVectorView, Vector3.Up);
                case AxisConstraint.YZ: return XiMathHelper.ComponentVector(dragVectorView, Vector3.Up) + XiMathHelper.ComponentVector(dragVectorView, Vector3.Backward);
                case AxisConstraint.ZX: return XiMathHelper.ComponentVector(dragVectorView, Vector3.Backward) + XiMathHelper.ComponentVector(dragVectorView, Vector3.Right);
                default: return dragVectorView;
            }
        }

        private Vector3 GetDragVectorView(Vector2 dragPosition)
        {
            Viewport viewport = Game.GraphicsDevice.Viewport;
            Segment dragPositionWorld = viewport.ToSegment(Game.Camera, dragPosition);
            Vector3 dragPositionObjectPlane = WorldToObjectPlane(dragSelectedPosition, dragPositionWorld.Start);
            return dragPositionObjectPlane - dragOffset - dragSelectedPosition;
        }

        private Vector3 WorldToObjectPlane(Vector3 objectPos, Vector3 nearPlanePos)
        {
            Matrix viewProjection;
            Game.Camera.GetViewProjection(out viewProjection);
            Vector4 objPos4 = new Vector4(objectPos, 1.0f);
            Vector4 nearPos4 = new Vector4(nearPlanePos, 1.0f);
            Vector4 objPosCS = Vector4.Transform(objPos4, viewProjection);
            Vector4 nearPosCS = Vector4.Transform(nearPos4, viewProjection);
            objPosCS /= objPosCS.W;
            nearPosCS /= nearPosCS.W;
            objPosCS.X = nearPosCS.X;
            objPosCS.Y = nearPosCS.Y;
            Vector4 newPosWS = Vector4.Transform(objPosCS, Matrix.Invert(viewProjection));
            return new Vector3(newPosWS.X, newPosWS.Y, newPosWS.Z) / newPosWS.W;
        }

        private Vector3 GetDragTranslation(Vector2 dragPosition)
        {
            Vector3 dragVector = GetDragVector(dragPosition);
            Vector3 newPosition = dragSelectedPosition + dragVector;
            Vector3 newSnappedPosition = newPosition.GetSnap(Controller.TranslationSnap);
            return newSnappedPosition - DraggedActor.Position;
        }

        private Vector3 GetDragOrientationDegrees(Vector2 dragPosition)
        {
            Vector3 dragVector = GetDragVector(dragPosition);
            Vector3 newOrientation = dragSelectedOrientation + dragVector * orientationCoefficient;
            Vector3 newSnappedOrientation = newOrientation.GetSnap(Controller.AngleSnap);
            return newSnappedOrientation - DraggedActor.OrientationEularDegrees;
        }

        private void DragTranslate(Vector3 translation)
        {
            DraggedActor.Position += translation;
        }

        private void DragOrientDegrees(Vector3 orientationDegrees)
        {
            DraggedActor.OrientationEularDegrees += orientationDegrees;
        }
        
        private const float orientationCoefficient = 10;
        private Vector3 dragOffset;
        private Vector3 dragSelectedPosition;
        private Vector3 dragSelectedOrientation;
        private DragMode dragMode;
    }
}
