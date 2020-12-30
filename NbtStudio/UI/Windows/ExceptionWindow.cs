using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class ExceptionWindow : Form
    {
        public readonly Exception Exception;
        public bool Expanded { get; private set; } = false;
        public ExceptionWindow(string title, string message, Exception exception)
        {
            InitializeComponent();
            Exception = exception;
            MessageLabel.Text = message + "\n\n" + Util.ExceptionMessage(exception);
            ExtraInfoLabel.Text = Exception.ToString();
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
            ExtraInfoPanel.Visible = expanded;
            if (expanded)
            {
                this.Height += ExtraInfoPanel.Height;
                ButtonDetails.Text = "Less Details";
            }
            else
            {
                this.Height -= ExtraInfoPanel.Height;
                ButtonDetails.Text = "More Details";
            }
        }

        private void ButtonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MessageLabel.Text + "\n\n" + ExtraInfoLabel.Text);
        }
    }
}