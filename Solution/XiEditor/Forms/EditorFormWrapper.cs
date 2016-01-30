using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Xi;
using SysPoint = System.Drawing.Point;
using SysRectangle = System.Drawing.Rectangle;

namespace XiEditor
{
    public class EditorFormWrapper : Disposable
    {
        public EditorFormWrapper(XiGame game, EditorForm form)
        {
            XiHelper.ArgumentNullCheck(game, form);
            this.game = game;
            this.form = form;
            controller = new EditorController(game);
            treeViewWrapper = new EditorTreeViewWrapper(game, controller, form.TreeView);
            SetUpGameEvents();
            SetUpCanvasEvents();
            SetUpSimulatableTypes<Actor>(game, form.ComboBoxActorType);
            SetUpSimulatableTypes<Facet>(game, form.ComboBoxFacetType);
        }

        public EditorController EditorController { get { return controller; } }

        public SysRectangle CanvasTransform
        {
            get { return new SysRectangle(form.Canvas.PointToScreen(new SysPoint()), form.Canvas.Size); }
        }

        public IntPtr CanvasHandle
        {
            get { return form.Canvas.IsHandleCreated ? form.Canvas.Handle : IntPtr.Zero; }
        }

        public void ActionFocused()
        {
            controller.Focused = true;
        }

        public void ActionDefocused()
        {
            controller.Focused = false;
        }

        public void ActionExit()
        {
            form.Close();
        }

        public void ActionOpen()
        {
            if (game.Playing) return;
            form.TreeView.BeginUpdate();
            if (ActionPromptSave() && form.OpenFileDialog.ShowDialog() == DialogResult.OK) Open();
            form.TreeView.EndUpdate();
        }

        public bool ActionSave()
        {
            if (game.Playing) return false;
            if (form.SaveFileDialog.FileName.Length == 0) form.SaveFileDialog.ShowDialog();
            if (form.SaveFileDialog.FileName.Length > 0) return Save();
            return false;
        }

        public bool ActionSaveAs()
        {
            if (game.Playing) return false;
            switch (form.SaveFileDialog.ShowDialog())
            {
                case DialogResult.OK: return Save();
                default: return false;
            }
        }

        public void ActionNew()
        {
            if (game.Playing) return;
            if (!ActionPromptSave()) return;
            game.Editing = true;
            controller.New();
            form.OpenFileDialog.FileName = string.Empty;
            form.SaveFileDialog.FileName = string.Empty;
            form.RichTextBoxConsole.WriteLine("New... Success.");
        }

        public void ActionImportProperties()
        {
            controller.ImportProperties();
        }

        public void ActionExportSelection()
        {
            controller.ExportSelection();
        }

        public void ActionExportProperties()
        {
            controller.ExportProperties();
        }

        public void ActionSetCameraPosition()
        {
            try
            {
                Vector3Converter converter = new Vector3Converter();
                string cameraPositionString = form.TextBoxCameraPosition.Text;
                controller.CameraPosition = (Vector3)converter.ConvertFrom(cameraPositionString);
                form.TextBoxCameraPosition.Text = XiHelper.Cast<string>(converter.ConvertTo(controller.CameraPosition, typeof(string)));
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, form.Text);
            }
        }

        public void ActionLook(Direction3D lookDirection)
        {
            controller.Look(lookDirection);
        }

        public void ActionX(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.X;
        }

        public void ActionY(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.Y;
        }

        public void ActionZ(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.Z;
        }

        public void ActionV(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.V;
        }

        public void ActionXY(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.XY;
        }

        public void ActionYZ(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.YZ;
        }

        public void ActionZX(object sender)
        {
            UpdateToolStripButtonAxes(sender);
            controller.AxisConstraint = AxisConstraint.ZX;
        }

        public void ActionTextBoxPositionTextChanged()
        {
            float snap;
            if (float.TryParse(form.TextBoxTranslationSnap.Text, out snap))
                controller.TranslationSnap = snap;
        }

        public void ActionTextBoxAngleSnapTextChanged()
        {
            float snap;
            if (float.TryParse(form.TextBoxAngleSnap.Text, out snap))
                controller.AngleSnap = snap;
        }

        public void ActionTextBoxCreationDepthTextChanged()
        {
            float creationDepth;
            if (float.TryParse(form.TextBoxCreationDepth.Text, out creationDepth))
                controller.CreationDepth = creationDepth;
        }

        public void ActionSelectAll()
        {
            controller.SelectAll();
        }

        public void ActionSelectSameType()
        {
            controller.SelectSameType();
        }

        public void ActionCreateActor()
        {
            if (game.Playing) return;
            string actorType = form.ComboBoxActorType.Text;
            if (actorType.Length == 0) return;
            controller.CreateActor(actorType);
        }

        public void ActionCreateActorInCanvas()
        {
            if (game.Playing) return;
            string actorType = form.ComboBoxActorType.Text;
            if (actorType.Length == 0) return;
            if (CanCreateInCanvas) controller.CreateActor(actorType, rightMouseUpPosition.Value);
            else controller.CreateActor(actorType);
        }

        public void ActionCreateFacet()
        {
            if (game.Playing) return;
            string facetType = form.ComboBoxFacetType.Text;
            if (facetType.Length == 0) return;
            controller.CreateFacet(facetType);
        }

        public void ActionToggleFreezing()
        {
            foreach (Simulatable simulatable in controller.ActorGroup.SelectionDescendant)
                simulatable.Frozen = !simulatable.Frozen;
        }

        public void ActionChangeType()
        {
            controller.ChangeType();
        }

        public void ActionDelete()
        {
            controller.Delete();
        }

        public void ActionCut()
        {
            controller.Cut();
        }

        public void ActionCopy()
        {
            controller.Copy();
        }

        public void ActionPaste()
        {
            controller.Paste();
        }

        public void ActionPasteInCanvas()
        {
            if (CanCreateInCanvas) controller.Paste(rightMouseUpPosition.Value);
        }

        public bool ActionPromptSave()
        {
            if (game.Playing) return false;
            switch (MessageBox.Show("Save the current document?", form.Text, MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes: return ActionSave();
                case DialogResult.No: return true;
                default: return false;
            }
        }

        public void ActionTogglePlay()
        {
            if (form.ButtonPlay.Checked) Play();
            else Edit();
            form.ButtonPlay.Checked = game.Playing;
        }

        public void ActionRefreshOverlayData()
        {
            game.Overlayer.RefreshOverlayData();
        }

        public void Advance(GameTime gameTime)
        {
            controller.Advance(gameTime);
            controller.CanvasTransform = CanvasTransform;
        }

        public void Visualize(GameTime gameTime)
        {
            controller.Visualize(gameTime);
        }

        public void ConfigureSettings()
        {
            form.TextBoxTranslationSnap.Text = controller.TranslationSnap.ToString();
            form.TextBoxAngleSnap.Text = controller.AngleSnap.ToString();
            form.TextBoxCreationDepth.Text = controller.CreationDepth.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) controller.Dispose();
            base.Dispose(disposing);
        }

        private bool CanCreateInCanvas { get { return rightMouseUpPosition != null; } }

        private void SetUpGameEvents()
        {
            game.SimulationStructureChanged += game_SimulationStructureChanged;
            game.SimulationSelectionChanged += game_SimulationSelectionChanged;
            game.PhysicsEnabledChanged += game_PhysicsEnabledChanged;
            game.EditingChanged += game_EditModeChanged;
        }

        private void SetUpCanvasEvents()
        {
            form.Canvas.Leave += delegate { rightMouseUpPosition = null; };
            form.Canvas.MouseEnter += delegate { controller.MouseOver = true; };
            form.Canvas.MouseLeave += delegate { controller.MouseOver = false; };
            form.Canvas.MouseDown += canvas_MouseDown;
            form.Canvas.MouseUp += canvas_MouseUp;
            form.Canvas.MouseMove += canvas_MouseMove;
        }

        private static void SetUpSimulatableTypes<T>(XiGame game, ToolStripComboBox editableTypeList) where T : class
        {
            Type baseType = typeof(T);
            IEnumerable<Type> types = game.GetTypes().Where(x => x.IsSubclassOf(baseType) && !x.IsGenericType && !x.IsAbstract);
            foreach (Type type in types) editableTypeList.Items.Add(type.FullName);
            editableTypeList.Items.Add("From File...");
            if (editableTypeList.Items.Count != 0) editableTypeList.SelectedIndex = 0;
        }

        private void Open()
        {
            try
            {
                game.Editing = true;
                controller.Open(form.OpenFileDialog.FileName);
                form.SaveFileDialog.FileName = form.OpenFileDialog.FileName;
                form.RichTextBoxConsole.WriteLine("Open... Success.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, form.Text, MessageBoxButtons.OK);
            }
        }

        private bool Save()
        {
            try
            {
                controller.Save(form.SaveFileDialog.FileName);
                form.OpenFileDialog.FileName = form.SaveFileDialog.FileName;
                form.RichTextBoxConsole.WriteLine("Save... Success.");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, form.Text, MessageBoxButtons.OK);
                return false;
            }
        }

        private bool Play()
        {
            Trace.Assert(!game.Playing, "Attempted to redundantly enable playing.");
            if (!ActionPromptSave()) return false;
            game.Playing = true;
            return true;
        }

        private void Edit()
        {
            Trace.Assert(!game.Editing, "Attempted to redundantly enable editing.");
            if (form.OpenFileDialog.FileName.Length != 0) Open();
            game.Editing = true;
        }

        private void UpdateToolStripButtonAxes(object sender)
        {
            form.ButtonX.Checked = sender == form.ButtonX;
            form.ButtonY.Checked = sender == form.ButtonY;
            form.ButtonZ.Checked = sender == form.ButtonZ;
            form.ButtonV.Checked = sender == form.ButtonV;
            form.ButtonXY.Checked = sender == form.ButtonXY;
            form.ButtonYZ.Checked = sender == form.ButtonYZ;
            form.ButtonZX.Checked = sender == form.ButtonZX;
        }

        private void game_PhysicsEnabledChanged()
        {
            // needed since transformation properties become hidden
            form.PropertyGrid.Refresh();
        }

        private void game_EditModeChanged()
        {
            bool editing = game.Editing;
            form.RichTextBoxConsole.WriteLine(editing ? "Now editing." : "Now playing.");
            form.ButtonPlay.Checked = !editing;
            // TODO: consider using reflection and attributes to set these
            form.ButtonNew.Enabled =
                form.ButtonOpen.Enabled =
                form.ButtonSave.Enabled =
                form.MenuItemNew.Enabled =
                form.MenuItemOpen.Enabled =
                form.MenuItemSave.Enabled =
                form.MenuItemSaveAs.Enabled =
                form.MenuItemExit.Enabled =
                form.ControlBox =
                editing;
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            controller.HandleMouseButtonDown(e.Button, new Vector2(e.Location.X, e.Location.Y));
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) rightMouseUpPosition = new Vector2(e.Location.X, e.Location.Y);
            controller.HandleMouseButtonUp(e.Button, new Vector2(e.Location.X, e.Location.Y));
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            controller.HandleMouseButtonDrag(e.Button, new Vector2(e.Location.X, e.Location.Y));
        }

        private void game_SimulationStructureChanged()
        {
            if (!form.PropertyGrid.ContainsFocus) form.PropertyGrid.Refresh();
        }

        private void game_SimulationSelectionChanged()
        {
            IEnumerable<Simulatable> selection = controller.ActorGroup.SelectionBottom;
            if (selection.Empty()) form.PropertyGrid.SelectedObject = null;
            else if (selection.HasOne()) form.PropertyGrid.SelectedObject = selection.First();
            else form.PropertyGrid.SelectedObjects = selection.ToArray();
        }

        private readonly XiGame game;
        private readonly EditorForm form;
        private readonly EditorController controller;
        private readonly EditorTreeViewWrapper treeViewWrapper;
        private Vector2? rightMouseUpPosition;
    }
}
