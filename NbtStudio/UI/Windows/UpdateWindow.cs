using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class UpdateWindow : Form
    {
        private readonly AvailableUpdate AvailableUpdate;

        public UpdateWindow(IconSource source, AvailableUpdate update)
        {
            InitializeComponent();

            AvailableUpdate = update;
            this.Icon = source.GetImage(IconType.NbtStudio).Icon;
            CurrentVersionValue.Text = Updater.GetCurrentVersion().ToString(false);
            AvailableVersionValue.Text = update.Version.ToString(false);
            ChangelogBox.Text = update.Changelog;
            ButtonOk.Select();
        }

        private void Confirm()
        {
            if (TryUpdate())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool TryUpdate()
        {
            try
            {
                var path = Application.ExecutablePath;
                AvailableUpdate.Update();
                Process.Start(path);
                Application.Exit();
                return true;
            }
            catch (Exception ex)
            {
                var window = new ExceptionWindow("Update error", "Failed to update the application.", FailableFactory.Failure(ex, "Updating"));
                window.ShowDialog(this);
                return false;
            }
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
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
    }
}