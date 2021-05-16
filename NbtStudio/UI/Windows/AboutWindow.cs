using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class AboutWindow : Form
    {
        public AboutWindow(IconSource source)
        {
            InitializeComponent();
            this.Icon = source.GetImage(IconType.NbtStudio).Icon;
            NameLabel.Text = String.Format(NameLabel.Text, Updater.GetCurrentVersion().ToString(false));
        }

        private void AboutWindow_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NameLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IOUtils.OpenUrlInBrowser("https://github.com/tryashtar/nbt-studio");
        }

        private void NbtExplorerLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IOUtils.OpenUrlInBrowser("https://github.com/jaquadro/NBTExplorer");
        }

        private void GenericIconLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IOUtils.OpenUrlInBrowser("https://p.yusukekamiyamane.com");
        }

        private void NbtIconLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IOUtils.OpenUrlInBrowser("https://github.com/AmberWat");
        }
    }
}