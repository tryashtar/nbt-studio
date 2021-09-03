using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class InfoWindow : Form
    {
        public bool Expanded { get; private set; } = false;
        public InfoWindow(string title, string message, string extra_details, InfoWindowButtons buttons = InfoWindowButtons.OK)
        {
            InitializeComponent();
            Text = title;
            MessageLabel.Text = message;
            ExtraInfoLabel.Text = extra_details;
            if (String.IsNullOrEmpty(extra_details))
                ButtonDetails.Visible = false;
            if (buttons == InfoWindowButtons.OKCancel)
            {
                ButtonCancel.Visible = true;
                this.Width += ButtonCancel.Width;
            }
        }

        private void InfoWindow_Load(object sender, EventArgs e)
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

    public enum InfoWindowButtons
    {
        OK,
        OKCancel
    }
}