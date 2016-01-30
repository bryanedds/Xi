using System.Windows.Forms;

namespace XiEditor
{
    public partial class EditorCanvas : UserControl
    {
        public EditorCanvas()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ShouldSinkCmdKey(keyData)) return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static bool ShouldSinkCmdKey(Keys keyData)
        {
            return
                keyData == Keys.Up ||
                keyData == Keys.Down ||
                keyData == Keys.Left ||
                keyData == Keys.Right;
        }
    }
}
