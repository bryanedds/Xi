using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public abstract class ActorDragger<A> : Dragger where A : Actor
    {
        public ActorDragger(XiGame game, EditorController controller)
            : base(game, dragBeginDelay)
        {
            this.controller = controller;
        }

        public bool IsDragging { get { return draggedActor != null; } }

        protected EditorController Controller { get { return controller; } }

        protected A DraggedActor { get { return draggedActor; } }

        protected IEnumerable<A> SelectedActors
        {
            get { return controller.ActorGroup.Selection.OfType<A>(); }
        }

        protected A FirstSelectedActor
        {
            get { return controller.ActorGroup.Selection.OfType<A>().FirstOrDefault(); }
        }

        protected sealed override void PrepareDragHook(Vector2 dragPosition)
        {
            draggedActor = FirstSelectedActor;
            if (IsDragging) PrepareDragData(dragPosition);
        }

        protected sealed override void BeginDragHook(Vector2 dragPosition) { }

        protected sealed override void UpdateDragHook(Vector2 dragPosition)
        {
            if (IsDragging) DragSelection(dragPosition);
        }

        protected sealed override void EndDragHook(Vector2 dragPosition) { }

        protected abstract void PrepareDragData(Vector2 dragPosition);

        protected abstract void DragSelection(Vector2 dragPosition);

        private const double dragBeginDelay = 0.15;
        private readonly EditorController controller;
        /// <summary>May be null.</summary>
        private A draggedActor;
    }
}
