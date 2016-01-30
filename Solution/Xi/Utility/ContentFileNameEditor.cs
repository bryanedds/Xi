using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Xi
{
    /// <summary>
    /// Edits content file names.
    /// </summary>
    public class ContentFileNameEditor : FileNameEditor
    {
        /// <inheritdoc />
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string fileName = XiHelper.Cast<string>(base.EditValue(context, provider, null));
            string rootPath = Path.GetFullPath(Constants.ContentPath);
            int rootPathLength = rootPath.Length + 1;
            if (fileName == null || rootPathLength >= fileName.Length) return XiHelper.Cast<string>(value);
            string fileNameRelative = fileName.Substring(rootPathLength);
            string fileNameRelativeNoExt = fileNameRelative.Substring(0, fileNameRelative.Length - ".xnb".Length);
            return fileNameRelativeNoExt.Replace('\\', '/'); // use forward slash for consistent path naming
        }

        /// <inheritdoc />
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory = Constants.ContentPath;
            openFileDialog.DefaultExt = "xnb";
            openFileDialog.Filter = "XNA Binary documents|*.xnb";
        }
    }
}
