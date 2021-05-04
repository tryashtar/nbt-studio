using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class ExceptionWindow : Form
    {
        public readonly Failable Error;
        public bool Expanded { get; private set; } = false;
        public ExceptionWindow(string title, string message, Failable failable)
        {
            InitializeComponent();
            Error = failable;
            MessageLabel.Text = message + "\n\n" + failable.ToStringSimple();
            ExtraInfoLabel.Text = failable.ToStringDetailed();
            Text = title;
            this.Icon = SystemIcons.Warning;
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

        private void ExceptionWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonDetails_Click(object sender, EventArgs e)
        {
            SetExpanded(!Expanded);
        }

        public void SetExpanded(bool expanded)
        {
            Expanded = expanded;
            if (expanded)
            {
                ExtraInfoPanel.Height = 300;
                this.Width += 100;
                ExtraInfoPanel.Visible = true;
                ButtonDetails.Text = "Less Details";
            }
            else
            {
                ExtraInfoPanel.Visible = false;
                ExtraInfoPanel.Height = 300;
                this.Height -= ExtraInfoPanel.Height;
                this.Width -= 100;
                ButtonDetails.Text = "More Details";
            }
        }

        private void ButtonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MessageLabel.Text + "\n\n" + ExtraInfoLabel.Text);
        }
    }
}