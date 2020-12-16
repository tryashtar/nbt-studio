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
        public ExceptionWindow(string title, string message, Exception exception)
        {
            InitializeComponent();
            Exception = exception;
            MessageLabel.Text = message + "\n\n" + Util.ExceptionMessage(exception);
            Text = title;
            this.Icon = SystemIcons.Warning;
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
            MessageLabel.Text += "\n\n" + Exception.ToString();
            ButtonDetails.Enabled = false;
        }
    }
}