using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class BulkEditWindow : Form
    {
        private readonly List<INbtTag> WorkingTags;

        private BulkEditWindow(List<INbtTag> tags, BulkEditPurpose purpose)
        {
            InitializeComponent();
            WorkingTags = tags;
            ActionList.Items.AddRange(tags.Select(x => TagPreview(x, purpose)).ToArray());

            // FindBox.Text=Properties.Settings... (likewise in close)
            this.Icon = Properties.Resources.action_edit_icon;
            if (purpose == BulkEditPurpose.Rename)
                this.Text = $"Rename {Util.Pluralize(tags.Count, "tag")}";
            else
                this.Text = $"Edit {Util.Pluralize(tags.Count, "tag")}";
        }

        private string TagPreview(INbtTag tag, BulkEditPurpose purpose)
        {
            if (purpose == BulkEditPurpose.Rename)
                return tag.Name;
            else
                return NbtUtil.PreviewNbtValue(tag);
        }

        public static void BulkRename(IEnumerable<INbtTag> tags)
        {
            var list = tags.Where(x => x.Parent is INbtCompound).ToList();
            if (list.Any())
            {
                var window = new BulkEditWindow(list, BulkEditPurpose.Rename);
                window.ShowDialog();
            }
        }

        public static void BulkEdit(IEnumerable<INbtTag> tags)
        {
            var list = tags.Where(x => NbtUtil.IsValueType(x.TagType)).ToList();
            if (list.Any())
            {
                var window = new BulkEditWindow(list, BulkEditPurpose.EditValue);
                window.ShowDialog();
            }
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
            // check conditions first, tags must not be modified at ALL until we can be sure it's safe
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }
    }

    public enum BulkEditPurpose
    {
        Rename,
        EditValue
    }
}