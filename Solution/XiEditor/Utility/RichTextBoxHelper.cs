using System.Windows.Forms;
using Xi;

namespace XiEditor
{
    public static class RichTextBoxHelper
    {
        public static void WriteLine(this RichTextBox richTextBox, string line)
        {
            XiHelper.ArgumentNullCheck(richTextBox);
            string[] lines = new string[richTextBox.Lines.Length + 1];
            richTextBox.Lines.CopyTo(lines, 0);
            lines[lines.Length - 1] = line;
            richTextBox.Lines = lines;
            richTextBox.ScrollToEnd();
        }

        public static void ScrollToEnd(this RichTextBox richTextBox)
        {
            XiHelper.ArgumentNullCheck(richTextBox);
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.ScrollToCaret();
        }
    }
}
