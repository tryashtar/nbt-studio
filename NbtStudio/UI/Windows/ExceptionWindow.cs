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
        public readonly IFailable Error;
        public bool Expanded { get; private set; } = false;
        public ExceptionWindow(string title, string message, IFailable failable, string after = null, ExceptionWindowButtons buttons = ExceptionWindowButtons.OK)
        {
            InitializeComponent();
            Error = failable;
            MessageLabel.Text = message + "\n\n" + failable.ToStringSimple() + (after != null ? "\n\n" + after : "");
            ExtraInfoLabel.Text = failable.ToStringDetailed();
            Text = title;
            this.Icon = SystemIcons.Warning;
            if (buttons == ExceptionWindowButtons.OKCancel)
            {
                ButtonCancel.Visible = true;
                this.Width += ButtonCancel.Width;
            }
        }

        private void ExceptionWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
                this.Width += 130 + ButtonCopy.Width;
                ExtraInfoPanel.Visible = true;
                ButtonDetails.Text = "Less Details";
                ButtonCopy.Visible = true;
            }
            else
            {
                ExtraInfoPanel.Visible = false;
                ExtraInfoPanel.Height = 300;
                this.Height -= ExtraInfoPanel.Height;
                this.Width -= 130 + ButtonCopy.Width;
                ButtonDetails.Text = "More Details";
                ButtonCopy.Visible = false;
            }
        }

        private void ButtonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MessageLabel.Text + "\n\n" + ExtraInfoLabel.Text);
        }
    }

    public enum ExceptionWindowButtons
    {
        OK,
        OKCancel
    }
}