using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xi;

namespace XiEditor
{
    public partial class SelectTypeForm : Form
    {
        public SelectTypeForm(Type[] itemTypes)
        {
            XiHelper.ArgumentNullCheck(itemTypes);
            this.itemTypes = itemTypes;
            InitializeComponent();
            PopulateItemTypes();
        }

        /// <summary>
        /// The name of the selected type.
        /// Empty string if no type chosen.
        /// </summary>
        public string SelectedTypeName { get { return selectedTypeName; } }

        private void buttonOK_Click(object sender, EventArgs e) { ActionOK(); }
        
        private void buttonCancel_Click(object sender, EventArgs e) { ActionCancel(); }

        private void ActionOK()
        {
            selectedTypeName = listBoxType.Text;
            Close();
        }

        private void ActionCancel()
        {
            selectedTypeName = string.Empty;
            Close();
        }

        private void PopulateItemTypes()
        {
            listBoxType.Items.Clear();
            foreach (Type type in itemTypes) listBoxType.Items.Add(type.FullName);
            if (itemTypes.Length != 0) listBoxType.SelectedIndex = 0;
        }

        private readonly Type[] itemTypes;
        private string selectedTypeName = string.Empty;
    }
}
