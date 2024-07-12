using NbtStudio.UI;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NbtStudio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

    }
}
