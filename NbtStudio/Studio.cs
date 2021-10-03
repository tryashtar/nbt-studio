using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NbtStudio.UI;

namespace NbtStudio
{
    public class Studio
    {
        public MainForm Form { get; private set; }
        public readonly NbtTreeModel Tree = new();

        private readonly string[] CommandLineArguments;

        public Studio(string[] args)
        {
            CommandLineArguments = args;
            if (Properties.Settings.Default.RecentFiles is null)
                Properties.Settings.Default.RecentFiles = new();
            Properties.Settings.Default.RecentFiles.MaxSize = 20;
            if (Properties.Settings.Default.CustomIconSets is null)
                Properties.Settings.Default.CustomIconSets = new();
        }

        public void LaunchForm()
        {
            Form = new MainForm(this);
            Form.Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (CommandLineArguments.Length > 0)
                Form.RunEditor(Form.Editors.OpenFiles(CommandLineArguments));
        }
    }
}
