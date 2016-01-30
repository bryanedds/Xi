using System.Linq;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public class ActorUIDragger : ActorDragger<ActorUI>
    {
        public ActorUIDragger(XiGame game, EditorController controller) : base(game, controller) { }

        protected enum DragMode
        {
            Translate = 0
        }

        protected override void PrepareDragData(Vector2 dragPosition)
        {
            if (Modifier == KeyboardModifier.None)
                BeginTranslate(DragMode.Translate, dragPosition);
        }

        protected override void DragSelection(Vector2 dragPosition)
        {
            if (dragMode == DragMode.Translate)
                DragTranslate(dragPosition);
        }

        private void BeginTranslate(DragMode dragMode, Vector2 dragPosition)
        {
            dragOffset = dragPosition - DraggedActor.Position.GetXY();
            this.dragMode = dragMode;
        }

        private void DragTranslate(Vector2 dragPosition)
        {
            DraggedActor.Position = new Vector3((dragPosition - dragOffset).GetSnap(Controller.TranslationSnap), DraggedActor.Position.Z);
        }

        private Vector2 dragOffset;
        private DragMode dragMode;
    }
}
