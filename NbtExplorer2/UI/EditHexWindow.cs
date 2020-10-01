using Be.Windows.Forms;
using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditHexWindow : Form
    {
        private readonly INbtTag WorkingTag;
        private readonly INbtContainer TagParent;
        private readonly bool SettingName;

        private EditHexWindow(INbtTag tag, INbtContainer parent, bool set_name, EditPurpose purpose)
        {
            InitializeComponent();

            WorkingTag = tag;
            TagParent = parent;

            SettingName = set_name;
            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;

            var provider = ByteProviders.GetByteProvider(tag);
            HexBox.ByteProvider = provider;
            HexBox.UseFixedBytesPerLine = true;
            HexBox.BytesPerLine = provider.BytesPerValue;

            this.Icon = NbtUtil.TagTypeIcon(tag.TagType);
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {NbtUtil.TagTypeName(tag.TagType)} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
            {
                this.Text = $"Edit {NbtUtil.TagTypeName(tag.TagType)} Tag";
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
    }
}