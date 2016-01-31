using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xi;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace XiEditor
{
    public abstract class ActorContext<A> : ControllerContext where A : Actor
    {
        public ActorContext(XiGame game, EditorController controller)
            : base(game, controller)
        {
            dragger = CreateDragger();
        }

        protected IEnumerable<A> ViewSelectableActors
        {
            get { return PersistentActors.Where(x => x.ViewSelectable); }
        }

        protected IEnumerable<A> PersistentActors
        {
            get { return ActorGroup.PersistentSimulatableChildren.OfType<A>(); }
        }

        protected IEnumerable<A> SelectedActors { get { return ActorGroup.Selection.OfType<A>(); } }

        protected abstract ActorDragger<A> CreateDragger();

        protected abstract A GetPickedActor(Vector2 mousePosition);

        protected abstract void InitializeCreatedActor(A actor, Vector2 canvasPosition);

        protected abstract void InitializePastedActor(A actor, Vector2 canvasPosition);

        protected sealed override bool CreateHook(string typeName)
        {
            return CreateHook(typeName, Controller.CanvasViewport.GetCenter());
        }

        protected sealed override bool CreateHook(string typeName, Vector2 canvasPosition)
        {
            try
            {
                A actor;
                if (!TryCreateActor(typeName, out actor) && actor == null) return false;
                if (actor == null) return true;
                actor.SelectedExclusively = true;
                InitializeCreatedActor(actor, canvasPosition);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected sealed override void AdvanceHook(GameTime gameTime)
        {
            base.AdvanceHook(gameTime);
            dragger.Update(gameTime);
        }

        protected sealed override void ChangeTypeHook()
        {
            if (SelectedActors.Empty()) return;
            string typeName = PromptTypeName();
            if (typeName.Length == 0) return;
            XmlDocument document = new XmlDocument();
            XmlNode rootNode = document.CreateElement("Root");
            document.AppendChild(rootNode);
            A oldActor = SelectedActors.First();
            oldActor.WriteProperties(document, rootNode, false);
            ActorGroup.RemoveActor(oldActor);
            A newActor = ActorGroup.CreateActor<A>(typeName);
            newActor.ReadProperties(rootNode);
            newActor.Selected = true;
        }

        protected sealed override void DeleteHook()
        {
            A actor = SelectedActors.FirstOrDefault();
            if (actor != null) ActorGroup.RemoveActor(actor);
        }

        protected sealed override XmlDocument CopyHook()
        {
            if (SelectedActors.Empty()) return null;
            XmlDocument document = new XmlDocument();
            XmlNode rootNode = document.CreateElement("Root");
            document.AppendChild(rootNode);
            XmlNode instanceNode = document.CreateElement("Serializable");
            rootNode.AppendChild(instanceNode);
            SelectedActors.First().Write(document, instanceNode);
            return document;
        }

        protected sealed override void PasteHook(XmlDocument document)
        {
            Paste(document, Controller.CanvasViewport.GetCenter());
        }

        protected sealed override void PasteHook(XmlDocument document, Vector2 canvasPosition)
        {
            XmlNode instanceNode = document.SelectSingleNode("Root").SelectSingleNode("Serializable");
            A actor = ActorGroup.CreateActorFromDocument<A>(instanceNode);
            InitializePastedActor(actor, canvasPosition);
            actor.SelectedExclusively = true;
        }

        protected sealed override void EndDraggingHook()
        {
            dragger.EndDragging();
        }

        protected sealed override IEnumerable<Simulatable> GetItemsHook()
        {
            return ActorGroup.SimulatableChildren.Where(x => x is A);
        }

        protected sealed override void SelectSameTypeHook()
        {
            base.SelectSameTypeHook();
            if (SelectedActors.Empty()) return;
            A child = SelectedActors.First();
            Type childType = child.GetType();
            ActorGroup.SelectChildren(x => x.GetType() == childType);
        }

        protected sealed override bool OnMouseButtonDown(MouseButtons button, Vector2 mousePosition)
        {
            bool eventHandled = GetPickedActor(mousePosition) != null;
            if (IgnoreMouseButtonChange(button)) return eventHandled;
            switch (button)
            {
                case MouseButtons.Left: HandleLeftMouseButtonDown(mousePosition); break;
                case MouseButtons.Right: HandleRightMouseButtonDown(mousePosition); break;
            }
            return eventHandled;
        }

        protected sealed override bool OnMouseButtonUp(MouseButtons button, Vector2 mousePosition)
        {
            if (IgnoreMouseButtonChange(button)) return false;
            dragger.HandleButtonUp(mousePosition);
            return true;
        }

        protected sealed override bool OnMouseButtonDrag(MouseButtons button, Vector2 mousePosition)
        {
            if (IgnoreMouseButtonDrag(button)) return false;
            dragger.HandleButtonDrag(mousePosition);
            return dragger.IsDragging;
        }

        private IEnumerable<Type> ActorTypes
        {
            get
            {
                return Game.GetTypes().Where(x =>
                    x.IsSubclassOf(typeof(A)) &&
                    !x.IsGenericType &&
                    !x.IsAbstract);
            }
        }

        private bool TryCreateActor(string typeName, out A actor)
        {
            if (typeName != "From File...")
            {
                actor = ActorGroup.CreateActor<A>(typeName);
                return true;
            }
            OpenFileDialog opener = new OpenFileDialog { RestoreDirectory = true };
            if (opener.ShowDialog() == DialogResult.OK)
            {
                actor = ActorGroup.CreateActorFromFile<A>(opener.FileName);
                return true;
            }
            actor = null;
            return true;
        }

        private string PromptTypeName()
        {
            using (SelectTypeForm selectTypeForm = new SelectTypeForm(ActorTypes.ToArray()))
            {
                selectTypeForm.ShowDialog();
                return selectTypeForm.SelectedTypeName;
            }
        }

        private void HandleLeftMouseButtonDown(Vector2 mousePosition)
        {
            HandleLeftSelection(mousePosition);
            dragger.HandleButtonDown(mousePosition);
        }

        private void HandleLeftSelection(Vector2 mousePosition)
        {
            KeyboardState keyboardState = Game.KeyboardState;
            if (keyboardState.GetControlState()) AddSelection(mousePosition);
            else if (keyboardState.IsKeyDown(XnaKeys.Escape)) RemoveSelection(mousePosition);
            else LeftSetSelection(mousePosition);
        }

        private void LeftSetSelection(Vector2 mousePosition)
        {
            A pickedActor = GetPickedActor(mousePosition);
            if (pickedActor != null) pickedActor.SelectedExclusively = true;
            else ActorGroup.ClearSelection();
        }

        private void HandleRightMouseButtonDown(Vector2 mousePosition)
        {
            HandleRightSelection(mousePosition);
        }

        private void HandleRightSelection(Vector2 mousePosition)
        {
            if (SelectedActors.Count() <= 1) RightSetSelection(mousePosition);
        }

        private void RightSetSelection(Vector2 mousePosition)
        {
            A pickedActor = GetPickedActor(mousePosition);
            if (pickedActor != null) pickedActor.Selected = true;
        }

        private void AddSelection(Vector2 mousePosition)
        {
            A pickedActor = GetPickedActor(mousePosition);
            if (pickedActor != null) pickedActor.Selected = true;
        }

        private void RemoveSelection(Vector2 mousePosition)
        {
            A pickedActor = GetPickedActor(mousePosition);
            if (pickedActor != null) pickedActor.Selected = false;
        }

        private static bool IgnoreMouseButtonChange(MouseButtons button)
        {
            return button != MouseButtons.Left && button != MouseButtons.Right;
        }

        private static bool IgnoreMouseButtonDrag(MouseButtons button)
        {
            return button != MouseButtons.Left;
        }

        private readonly ActorDragger<A> dragger;
    }
}
