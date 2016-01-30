using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Xi;
using SysRectangle = System.Drawing.Rectangle;

namespace XiEditor
{
    public partial class EditorForm : Form
    {
        public EditorForm(XiGame game)
        {
            InitializeComponent();
            XiHelper.ArgumentNullCheck(game);
            this.game = game;
            wrapper = new EditorFormWrapper(game, this);
            wrapper.ConfigureSettings();
            buttonLimitFPS.Checked = game.IsFixedTimeStep;
            buttonPhysics.Checked = game.PhysicsEnabled;
        }

        public EditorFormWrapper EditorFormWrapper { get { return wrapper; } }
        public SysRectangle CanvasTransform { get { return wrapper.CanvasTransform; } }
        public IntPtr CanvasHandle { get { return wrapper.CanvasHandle; } }
        public ToolStripComboBox ComboBoxActorType { get { return comboBoxActorType; } }
        public ToolStripComboBox ComboBoxFacetType { get { return comboBoxFacetType; } }
        public OpenFileDialog OpenFileDialog { get { return openFileDialog; } }
        public SaveFileDialog SaveFileDialog { get { return saveFileDialog; } }
        public PropertyGrid PropertyGrid { get { return propertyGrid; } }
        public RichTextBox RichTextBoxConsole { get { return richTextBoxConsole; } }
        public TreeView TreeView { get { return treeView; } }
        public ToolStripTextBox TextBoxTranslationSnap { get { return textBoxTranslationSnap; } }
        public ToolStripTextBox TextBoxAngleSnap { get { return textBoxAngleSnap; } }
        public ToolStripTextBox TextBoxCreationDepth { get { return textBoxCreationDepth; } }
        public ToolStripTextBox TextBoxCameraPosition { get { return textBoxCameraPosition; } }
        public ToolStripButton ButtonX { get { return buttonX; } }
        public ToolStripButton ButtonY { get { return buttonY; } }
        public ToolStripButton ButtonZ { get { return buttonZ; } }
        public ToolStripButton ButtonV { get { return buttonV; } }
        public ToolStripButton ButtonXY { get { return buttonXY; } }
        public ToolStripButton ButtonYZ { get { return buttonYZ; } }
        public ToolStripButton ButtonZX { get { return buttonZX; } }
        public ToolStripButton ButtonPlay { get { return buttonPlay; } }
        public ToolStripButton ButtonNew { get { return buttonNew; } }
        public ToolStripButton ButtonOpen { get { return buttonOpen; } }
        public ToolStripButton ButtonSave { get { return buttonSave; } }
        public ToolStripButton ButtonSetCameraPosition { get { return buttonSetCameraPosition; } }
        public ToolStripMenuItem MenuItemNew { get { return menuItemNew; } }
        public ToolStripMenuItem MenuItemOpen { get { return menuItemOpen; } }
        public ToolStripMenuItem MenuItemSave { get { return menuItemSave; } }
        public ToolStripMenuItem MenuItemSaveAs { get { return menuItemSaveAs; } }
        public ToolStripMenuItem MenuItemExit { get { return menuItemExit; } }
        public EditorCanvas Canvas { get { return canvas; } }

        public void Advance(GameTime gameTime) { wrapper.Advance(gameTime); }
        public void Visualize(GameTime gameTime) { wrapper.Visualize(gameTime); }

        protected override void OnClosed(EventArgs e)
        {
            wrapper.Dispose();
            wrapper = null;
            base.OnClosed(e);
        }
        
        private void menuItemExit_Click(object sender, EventArgs e) { wrapper.ActionExit(); }
        private void menuItemNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void menuItemSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void menuItemSaveAs_Click(object sender, EventArgs e) { wrapper.ActionSaveAs(); }
        private void menuItemOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void menuItemImportProperties_Click(object sender, EventArgs e) { wrapper.ActionImportProperties(); }
        private void menuItemExportSelection_Click(object sender, EventArgs e) { wrapper.ActionExportSelection(); }
        private void menuItemExportProperties_Click(object sender, EventArgs e) { wrapper.ActionExportProperties(); }
        private void menuItemUndo_Click(object sender, EventArgs e) { /*wrapper.ActionUndo();*/ }
        private void menuItemRedo_Click(object sender, EventArgs e) { /*wrapper.ActionRedo();*/ }
        private void menuItemDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void menuItemCut_Click(object sender, EventArgs e) { wrapper.ActionCut(); }
        private void menuItemCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void menuItemPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void menuItemSelectAll_Click(object sender, EventArgs e) { wrapper.ActionSelectAll(); }
        private void menuItemSelectSameType_Click(object sender, EventArgs e) { wrapper.ActionSelectSameType(); }
        private void menuItemCreateActor_Click(object sender, EventArgs e) { wrapper.ActionCreateActor(); }
        private void menuItemCreateFacet_Click(object sender, EventArgs e) { wrapper.ActionCreateFacet(); }
        private void menuItemToggleFreezing_Click(object sender, EventArgs e) { wrapper.ActionToggleFreezing(); }
        private void menuItemFreeze_Click(object sender, EventArgs e) { wrapper.ActionToggleFreezing(); }
        private void menuItemChangeType_Click(object sender, EventArgs e) { wrapper.ActionChangeType(); }
        private void menuItemCreateActorInCanvas_Click(object sender, EventArgs e) { wrapper.ActionCreateActorInCanvas(); }
        private void menuItemPasteInCanvas_Click(object sender, EventArgs e) { wrapper.ActionPasteInCanvas(); }
        private void menuItemRefreshOverlayData_Click(object sender, EventArgs e) { wrapper.ActionRefreshOverlayData(); }
        private void buttonNew_Click(object sender, EventArgs e) { wrapper.ActionNew(); }
        private void buttonOpen_Click(object sender, EventArgs e) { wrapper.ActionOpen(); }
        private void buttonSave_Click(object sender, EventArgs e) { wrapper.ActionSave(); }
        private void buttonDelete_Click(object sender, EventArgs e) { wrapper.ActionDelete(); }
        private void buttonCut_Click(object sender, EventArgs e) { wrapper.ActionCut(); }
        private void buttonCopy_Click(object sender, EventArgs e) { wrapper.ActionCopy(); }
        private void buttonPaste_Click(object sender, EventArgs e) { wrapper.ActionPaste(); }
        private void buttonCreateActor_Click(object sender, EventArgs e) { wrapper.ActionCreateActor(); }
        private void buttonCreateFacet_Click(object sender, EventArgs e) { wrapper.ActionCreateFacet(); }
        private void buttonUndo_ButtonClick(object sender, EventArgs e) { /*wrapper.ActionUndo();*/ }
        private void buttonRedo_ButtonClick(object sender, EventArgs e) { /*wrapper.ActionRedo();*/ }
        private void buttonUndo_DropDownOpening(object sender, EventArgs e) { /*wrapper.ActionPopulateUndoDropDown();*/ }
        private void buttonRedo_DropDownOpening(object sender, EventArgs e) { /*wrapper.ActionPopulateRedoDropDown();*/ }
        private void buttonUndo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { /*wrapper.ActionUndo(new Guid(e.ClickedItem.Name));*/ }
        private void buttonRedo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) { /*wrapper.ActionRedo(new Guid(e.ClickedItem.Name));*/ }
        private void buttonLimitFPS_Click(object sender, EventArgs e) { game.IsFixedTimeStep = buttonLimitFPS.Checked; }
        private void buttonPhysics_Click(object sender, EventArgs e) { game.PhysicsEnabled = buttonPhysics.Checked; }
        private void buttonPlay_Click(object sender, EventArgs e) { wrapper.ActionTogglePlay(); }
        private void buttonF_Click(object sender, EventArgs e) { wrapper.ActionLook(Direction3D.Forward); }
        private void buttonB_Click(object sender, EventArgs e) { wrapper.ActionLook(Direction3D.Backward); }
        private void buttonU_Click(object sender, EventArgs e) { wrapper.ActionLook(Direction3D.Up); }
        private void buttonD_Click(object sender, EventArgs e) { wrapper.ActionLook(Direction3D.Down); }
        private void buttonL_Click(object sender, EventArgs e) { wrapper.ActionLook(Direction3D.Left); }
        private void buttonR_Click(object sender, EventArgs e) { wrapper.ActionLook(Direction3D.Right); }
        private void buttonSetCameraPosition_Click(object sender, EventArgs e) { wrapper.ActionSetCameraPosition(); }
        private void buttonX_Click(object sender, EventArgs e) { wrapper.ActionX(sender); }
        private void buttonY_Click(object sender, EventArgs e) { wrapper.ActionY(sender); }
        private void buttonZ_Click(object sender, EventArgs e) { wrapper.ActionZ(sender); }
        private void buttonV_Click(object sender, EventArgs e) { wrapper.ActionV(sender); }
        private void buttonXY_Click(object sender, EventArgs e) { wrapper.ActionXY(sender); }
        private void buttonYZ_Click(object sender, EventArgs e) { wrapper.ActionYZ(sender); }
        private void buttonZX_Click(object sender, EventArgs e) { wrapper.ActionZX(sender); }
        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e) { e.Cancel = !wrapper.ActionPromptSave(); }
        private void canvas_Enter(object sender, EventArgs e) { wrapper.ActionFocused(); }
        private void canvas_Leave(object sender, EventArgs e) { wrapper.ActionDefocused(); }
        private void textBoxTranslationSnap_TextChanged(object sender, EventArgs e) { wrapper.ActionTextBoxPositionTextChanged(); }
        private void textBoxAngleSnap_TextChanged(object sender, EventArgs e) { wrapper.ActionTextBoxAngleSnapTextChanged(); }
        private void textBoxCreationDepth_TextChanged(object sender, EventArgs e) { wrapper.ActionTextBoxCreationDepthTextChanged(); }

        private readonly XiGame game;
        private EditorFormWrapper wrapper;
    }
}
