using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Xna.Framework;
using Xi;

namespace XiEditor
{
    public class ControllerContext : Disposable
    {
        public ControllerContext(XiGame game, EditorController controller)
        {
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            this.controller = controller;
        }

        public void Advance(GameTime gameTime)
        {
            AdvanceHook(gameTime);
        }

        public void Visualize(GameTime gameTime)
        {
            VisualizeHook(gameTime);
        }
        
        public bool Create(string typeName)
        {
            return CreateHook(typeName);
        }
        
        public bool Create(string typeName, Vector2 canvasPosition)
        {
            return CreateHook(typeName, canvasPosition);
        }

        public void ChangeType()
        {
            ChangeTypeHook();
        }

        public void Delete()
        {
            DeleteHook();
        }

        /// <summary>
        /// May return null.
        /// </summary>
        public XmlDocument Copy()
        {
            return CopyHook();
        }

        public void Paste(XmlDocument document)
        {
            XiHelper.ArgumentNullCheck(document);
            PasteHook(document);
        }

        public void Paste(XmlDocument document, Vector2 canvasPosition)
        {
            XiHelper.ArgumentNullCheck(document);
            PasteHook(document, canvasPosition);
        }

        public void EndDragging()
        {
            EndDraggingHook();
        }

        public void SelectSameType()
        {
            SelectSameTypeHook();
        }

        public Simulatable GetItem(int hashCode)
        {
            return GetItems().Where(x => x.GetHashCode() == hashCode).FirstOrDefault();
        }

        public IEnumerable<Simulatable> GetItems()
        {
            return GetItemsHook();
        }

        public bool Contains(Simulatable item)
        {
            return GetItems().Contains(item);
        }

        public bool HandleMouseButtonDown(MouseButtons button, Vector2 mousePosition)
        {
            return OnMouseButtonDown(button, mousePosition);
        }

        public bool HandleMouseButtonUp(MouseButtons button, Vector2 mousePosition)
        {
            return OnMouseButtonUp(button, mousePosition);
        }

        public bool HandleMouseButtonDrag(MouseButtons button, Vector2 mousePosition)
        {
            return OnMouseButtonDrag(button, mousePosition);
        }

        protected XiGame Game { get { return game; } }

        protected EditorController Controller { get { return controller; } }

        protected ActorGroup ActorGroup { get { return controller.ActorGroup; } }

        protected bool Current { get { return Controller.IsCurrentContext(this); } }

        protected virtual void AdvanceHook(GameTime gameTime) { }

        protected virtual void VisualizeHook(GameTime gameTime) { }

        protected virtual bool CreateHook(string typeName) { return false; }

        protected virtual bool CreateHook(string typeName, Vector2 canvasPosition) { return false; }

        protected virtual void ChangeTypeHook() { }

        protected virtual void DeleteHook() { }

        protected virtual XmlDocument CopyHook() { return null; }

        protected virtual void PasteHook(XmlDocument document) { }

        protected virtual void EndDraggingHook() { }

        protected virtual void PasteHook(XmlDocument document, Vector2 canvasPosition) { }

        protected virtual void SelectSameTypeHook() { }

        protected virtual IEnumerable<Simulatable> GetItemsHook() { yield break; }

        protected virtual bool OnMouseButtonDown(MouseButtons button, Vector2 mousePosition) { return false; }

        protected virtual bool OnMouseButtonUp(MouseButtons button, Vector2 mousePosition) { return false; }

        protected virtual bool OnMouseButtonDrag(MouseButtons button, Vector2 mousePosition) { return false; }

        private readonly XiGame game;
        private readonly EditorController controller;
    }
}
