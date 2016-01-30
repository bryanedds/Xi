using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public class CameraDragger : Dragger
    {
        public CameraDragger(XiGame game, EditorController controller)
            : base(game, dragBeginDelay)
        {
            this.controller = controller;
        }

        protected override void PrepareDragHook(Vector2 dragPosition)
        {
            dragOffset = dragPosition;
            switch (Modifier)
            {
                case KeyboardModifier.ControlShift: dragMode = DragMode.Look; break;
                case KeyboardModifier.Control: dragMode = DragMode.LookAt; break;
                case KeyboardModifier.Shift: dragMode = DragMode.UpPlane; break;
                case KeyboardModifier.None: dragMode = DragMode.ForwardPlane; break;
            }
        }

        protected override void BeginDragHook(Vector2 dragPosition) { }

        protected override void UpdateDragHook(Vector2 dragPosition)
        {
            switch (dragMode)
            {
                case DragMode.ForwardPlane: DragForwardPlane(dragPosition); break;
                case DragMode.UpPlane: DragUpPlane(dragPosition); break;
                case DragMode.LookAt: DragLookAt(dragPosition); break;
                case DragMode.Look: DragLook(dragPosition); break;
            }
        }

        protected override void EndDragHook(Vector2 dragPosition) { }

        private enum DragMode { ForwardPlane = 0, UpPlane, LookAt, Look }

        private void DragForwardPlane(Vector2 dragPosition)
        {
            Vector2 dragAmount = dragPosition - dragOffset;
            controller.CameraPosition +=
                dragAmount.X * controller.CameraLeft +
                dragAmount.Y * controller.CameraUp;
            dragOffset = dragPosition;
        }

        private void DragUpPlane(Vector2 dragPosition)
        {
            Vector2 dragAmount = dragPosition - dragOffset;
            controller.CameraPosition +=
                dragAmount.X * controller.CameraLeft +
                dragAmount.Y * controller.CameraForward;
            dragOffset = dragPosition;
        }

        private void DragLookAt(Vector2 dragPosition)
        {
            Vector2 dragAmount = dragPosition - dragOffset;
            controller.CameraAngleX += dragAmount.Y * -angleMultiplier;
            controller.CameraAngleY += dragAmount.X * -angleMultiplier;
            controller.CameraPosition +=
                dragAmount.X * controller.CameraLeft * lookAtMultiplier +
                dragAmount.Y * controller.CameraUp * lookAtMultiplier;
            dragOffset = dragPosition;
        }

        private void DragLook(Vector2 dragPosition)
        {
            Vector2 dragAmount = dragPosition - dragOffset;
            controller.CameraAngleX += dragAmount.Y * -angleMultiplier;
            controller.CameraAngleY += dragAmount.X * -angleMultiplier;
            dragOffset = dragPosition;
        }

        private const double dragBeginDelay = 0;
        private const float lookAtMultiplier = 0.5f;
        private const float angleMultiplier = 0.005f;
        private readonly EditorController controller;
        private DragMode dragMode;
        private Vector2 dragOffset;
    }
}
