using Be.Windows.Forms;
using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditHexWindow : Form
    {
        private readonly INbtTag WorkingTag;
        private readonly INbtContainer TagParent;
        private readonly bool SettingName;
        private readonly IByteTransformer Provider;

        private EditHexWindow(INbtTag tag, INbtContainer parent, bool set_name, EditPurpose purpose)
        {
            InitializeComponent();

            WorkingTag = tag;
            TagParent = parent;

            SettingName = set_name;
            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;

            Provider = ByteProviders.GetByteProvider(tag);
            HexBox.ByteProvider = Provider;
            HexBox.GroupSize = Provider.BytesPerValue;
            HexBox.GroupSeparatorVisible = Provider.BytesPerValue > 1;
            HexBox.SelectionBackColor = Color.LightBlue;
            HexBox.SelectionForeColor = HexBox.ForeColor;
            HexBox.Size = new Size(0, 0);

            string tagname;
            if (tag is INbtList list)
            {
                tagname = NbtUtil.TagTypeName(list.ListType) + " List";
                this.Icon = NbtUtil.TagTypeIcon(list.ListType);
            }
            else
            {
                tagname = NbtUtil.TagTypeName(tag.TagType);
                this.Icon = NbtUtil.TagTypeIcon(tag.TagType);
            }
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {tagname} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
            {
                this.Text = $"Edit {tagname} Tag";
                NameBox.Text = tag.Name;
            }

            if (SettingName && purpose != EditPurpose.EditValue)
            {
                NameBox.Select();
                NameBox.SelectAll();
            }
            else
                HexBox.Select();
        }

        public static NbtTag CreateTag(NbtTagType type, INbtContainer parent)
        {
            bool has_name = parent is INbtCompound;
            var tag = NbtUtil.CreateTag(type);

            var window = new EditHexWindow(tag.Adapt(), parent, has_name, EditPurpose.Create);
            return window.ShowDialog() == DialogResult.OK ? tag : null;
        }

        public static bool ModifyTag(INbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is INbtCompound;

            var window = new EditHexWindow(existing, parent, has_name, purpose);
            return window.ShowDialog() == DialogResult.OK; // window modifies the tag by itself
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
            var name = NameBox.Text.Trim();

            // check conditions first, tag must not be modified at ALL until we can be sure it's safe
            if (SettingName)
            {
                if (name == "")
                {
                    MessageBox.Show("The name cannot be empty");
                    return false;
                }
                if (TagParent is INbtCompound compound && name != WorkingTag.Name && compound.Contains(name))
                {
                    MessageBox.Show($"Duplicate name; this compound already contains a tag named \"{name}\"");
                    return false;
                }
            }
            if (SettingName)
                WorkingTag.Name = name;

            HexBox.ByteProvider.ApplyChanges();
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }

        private void UpdateCursorLabel()
        {
            long selected_byte = HexBox.SelectionStart;
            CursorLabel.Text = $"Element {selected_byte / Provider.BytesPerValue}";
        }

        private void HexBox_CurrentLineChanged(object sender, EventArgs e)
        {
            UpdateCursorLabel();
        }

        private void HexBox_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            UpdateCursorLabel();
        }

        private void HexBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                HexBox.SelectAll();
                e.Handled = true;
            }
        }
    }
}