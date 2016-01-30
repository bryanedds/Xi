using System.Linq;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public class Actor2DDragger : ActorDragger<Actor2D>
    {
        public Actor2DDragger(XiGame game, EditorController controller) : base(game, controller) { }

        protected enum DragMode
        {
            Translate = 0,
            Rotate
        }

        protected override void PrepareDragData(Vector2 dragPosition)
        {
            switch (Modifier)
            {
                case KeyboardModifier.Shift: BeginRotateDegrees(DragMode.Rotate, dragPosition); break;
                case KeyboardModifier.None: BeginTranslate(DragMode.Translate, dragPosition); break;
            }
        }

        protected override void DragSelection(Vector2 dragPosition)
        {
            switch (dragMode)
            {
                case DragMode.Rotate: DragRotateDegrees(dragPosition); break;
                case DragMode.Translate: DragTranslate(dragPosition); break;
            }
        }

        private void BeginTranslate(DragMode dragMode, Vector2 dragPosition)
        {
            dragOffset = dragPosition - DraggedActor.Position.GetXY();
            this.dragMode = dragMode;
        }

        private void BeginRotateDegrees(DragMode dragMode, Vector2 dragPosition)
        {
            dragOffset = new Vector2(dragPosition.X, dragPosition.Y - DraggedActor.RotationDegrees);
            this.dragMode = dragMode;
        }

        private void DragTranslate(Vector2 dragPosition)
        {
            DraggedActor.Position = new Vector3((dragPosition - dragOffset).GetSnap(Controller.TranslationSnap), DraggedActor.Position.Z);
        }

        private void DragRotateDegrees(Vector2 dragPosition)
        {
            DraggedActor.RotationDegrees = (dragPosition.Y - dragOffset.Y).GetSnap(Controller.AngleSnap);
        }

        private Vector2 dragOffset;
        private DragMode dragMode;
    }
}
