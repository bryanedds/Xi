using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xi;

namespace XiEditor
{
    public static class TreeViewHelper
    {
        public static Point GetScrollPosition(this TreeView treeView)
        {
            XiHelper.ArgumentNullCheck(treeView);
            return new Point(GetScrollPos(treeView.Handle, SB_HORZ), GetScrollPos(treeView.Handle, SB_VERT));
        }
        
        public static void SetScrollPosition(this TreeView treeView, Point scrollPosition)
        {
            XiHelper.ArgumentNullCheck(treeView);
            SetScrollPos((IntPtr)treeView.Handle, SB_HORZ, scrollPosition.X, true);
            SetScrollPos((IntPtr)treeView.Handle, SB_VERT, scrollPosition.Y, true);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
    }
}
