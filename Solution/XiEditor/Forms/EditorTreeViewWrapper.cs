using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Xi;

namespace XiEditor
{
    // TODO: add an intelligent filtering mechanism to the tree view control
    public class EditorTreeViewWrapper
    {
        public EditorTreeViewWrapper(XiGame game, EditorController controller, TreeView treeView)
        {
            XiHelper.ArgumentNullCheck(game, controller, treeView);
            this.controller = controller;
            this.treeView = treeView;
            game.SimulationStructureChanged += game_SimulationStructureChanged;
            game.SimulationSelectionChanged += game_SimulationSelectionChanged;
            treeView.MouseDown += treeView_MouseDown;
            treeView.AfterSelect += treeView_AfterSelect;
        }

        protected EditorController Controller { get { return controller; } }

        private bool ContainsFocus { get { return treeView.ContainsFocus; } }

        private void game_SimulationStructureChanged()
        {
            RefreshTreeView();
            if (!ContainsFocus) EnsureSelectedItemVisible();
        }

        private void game_SimulationSelectionChanged()
        {
            if (handlingSelectionChange) return;
            handlingSelectionChange = true;
            treeView.SelectedNode = GetTreeNode(controller.ActorGroup.SelectionBottom.FirstOrDefault());
            handlingSelectionChange = false;
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            Trace.Assert(handlingSelectionChange == false, "Flag handlingSelectionEvent should be false on MouseDown.");
            handlingSelectionChange = true;
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) SelectNode(treeView.GetNodeAt(e.X, e.Y));
            handlingSelectionChange = false;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (handlingSelectionChange) return;
            handlingSelectionChange = true;
            SelectNode(treeView.SelectedNode);
            handlingSelectionChange = false;
        }

        private void RefreshTreeView()
        {
            treeView.BeginUpdate();
            ClearTreeNodes();
            BuildTreeNodes();
            treeView.EndUpdate();
        }

        private void ClearTreeNodes()
        {
            treeView.Nodes.Clear();
        }

        private void BuildTreeNodes()
        {
            foreach (Simulatable item in controller.ActorGroup.PersistentSimulatableChildren)
                BuildTreeNode(item, null);
        }

        private void BuildTreeNode(Simulatable item, TreeNode parentNode)
        {
            TreeNode node = new TreeNode(item.NameOrDefault);
            node.Name = item.GetHashCode().ToString();
            if (parentNode == null) treeView.Nodes.Add(node);
            else parentNode.Nodes.Add(node);
            foreach (Simulatable child in item.PersistentSimulatableChildren) BuildTreeNode(child, node);
        }

        private void EnsureSelectedItemVisible()
        {
            Simulatable selectedItem = controller.ActorGroup.SelectionBottom.FirstOrDefault();
            TreeNode node = GetTreeNode(selectedItem);
            if (node != null) node.EnsureVisible();
        }

        private TreeNode GetTreeNode(Simulatable item)
        {
            if (item == null) return null;
            TreeNode[] nodes = treeView.Nodes.Find(item.GetHashCode().ToString(), true);
            return nodes.FirstOrDefault();
        }

        private void SelectNode(TreeNode node)
        {
            treeView.SelectedNode = node;
            Simulatable item = LookUpSelectedItem();
            if (item == null) controller.ActorGroup.ClearSelection();
            else item.SelectedExclusively = true;
        }

        private Simulatable LookUpSelectedItem()
        {
            int? selectedHashCode = LookUpSelectedHashCode();
            return selectedHashCode.HasValue ? controller.GetItem(selectedHashCode.Value) : null;
        }

        private int? LookUpSelectedHashCode()
        {
            TreeNode selectedNode = treeView.SelectedNode;
            return selectedNode != null ? (int?)int.Parse(selectedNode.Name) : null;
        }

        private readonly EditorController controller;
        private readonly TreeView treeView;
        private bool handlingSelectionChange;
    }
}
