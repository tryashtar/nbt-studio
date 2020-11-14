using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class RenameFileWindow : Form
    {
        private readonly IHavePath OriginalItem;

        private RenameFileWindow(IconSource source, IHavePath file)
        {
            InitializeComponent();

            OriginalItem = file;
            this.Icon = source.Rename.Icon;
            NameBox.SetItem(file);
            NameBox.SelectAll();
        }

        public static bool RenameFile(IconSource source, IHavePath file)
        {
            var window = new RenameFileWindow(source, file);
            return window.ShowDialog() == DialogResult.OK;
        }

        private void Confirm()
        {
            if (TryModify())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool TryModify()
        {
            if (!NameBox.CheckName())
                return false;
            NameBox.PerformRename();
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }
    }
}