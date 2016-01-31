using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xi;
using SysRectangle = System.Drawing.Rectangle;

namespace XiEditor
{
    // TODO: consider adding a context-sensitive 'roll out' that exposes 3D, 2D, UI, or user-
    // defined configuration controls.
    public class Editor : XiGame
    {
        public Editor(string sourcePath, params Assembly[] assemblies) :
            base(sourcePath, assemblies) { }

        protected Form ProgramForm { get { return programForm; } }

        protected EditorForm EditorForm { get { return editorForm; } }

        protected EditorFormWrapper EditorFormWrapper { get { return editorForm.EditorFormWrapper; } }

        protected EditorController EditorController { get { return EditorFormWrapper.EditorController; } }

        protected override void Initialize()
        {
            base.Initialize();
            PhysicsEnabled = false;
            Editing = true;
            PhysicsBoxDrawerVisible = true;
            SetUpProgramForm();
            SetUpEditorForm();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            SysRectangle canvasTransform = editorForm.CanvasTransform;
            ResolutionManager.Resolution = new Point(canvasTransform.Width, canvasTransform.Height);
            programForm.Bounds = canvasTransform;
        }

        protected override void AdvanceHook(GameTime gameTime)
        {
            base.AdvanceHook(gameTime);
            editorForm.Advance(gameTime);
        }

        protected override void VisualizeHook(GameTime gameTime)
        {
            base.VisualizeHook(gameTime);
            if (editorForm.Visible) editorForm.Visualize(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Present(editorForm.CanvasHandle);
        }

        private void SetUpProgramForm()
        {
            programForm = XiHelper.Cast<Form>(Form.FromHandle(Window.Handle));
            programForm.FormBorderStyle = FormBorderStyle.None;
            programForm.Shown += (s, e) => XiHelper.Cast<Form>(s).Hide();
        }

        private void SetUpEditorForm()
        {
            editorForm = new EditorForm(this);
            editorForm.HandleDestroyed += delegate { Exit(); };
            editorForm.Show();
        }

        private Form programForm;
        private EditorForm editorForm;
    }
}
