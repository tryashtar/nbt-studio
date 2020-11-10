using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.app_icon_16;
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
            Process.Start("https://github.com/tryashtar/nbt-studio");
        }

        private void NbtExplorerLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/jaquadro/NBTExplorer");
        }

        private void GenericIconLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://p.yusukekamiyamane.com");
        }

        private void NbtIconLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/AmberWat");
        }
    }
}