using System;
using System.Reflection;

namespace XiEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Assembly editorAssembly = Assembly.GetExecutingAssembly();
            using (Editor editor = new Editor("../../../../Xi", editorAssembly)) editor.Run();
        }
    }
}
