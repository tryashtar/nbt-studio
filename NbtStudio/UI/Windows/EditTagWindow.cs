using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class EditTagWindow : Form
    {
        private readonly NbtTag WorkingTag;
        private readonly NbtContainerTag TagParent;
        private readonly bool SettingName;
        private readonly bool SettingValue;

        private EditTagWindow(NbtTag tag, NbtContainerTag parent, bool set_name, bool set_value, EditPurpose purpose)
        {
            InitializeComponent();

            WorkingTag = tag;
            TagParent = parent;
            NameBox.SetTags(tag, parent);
            ValueBox.SetTags(tag, parent);

            SettingName = set_name;
            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;

            SettingValue = set_value;
            ValueLabel.Visible = SettingValue;
            ValueBox.Visible = SettingValue;

            if (tag.TagType == NbtTagType.String)
            {
                ValueBox.Multiline = true;
                ValueBox.AcceptsReturn = true;
                ValueBox.Width *= 2;
                ValueBox.Height *= 6;
            }
            this.Icon = NbtUtil.TagTypeIcon(tag.TagType);
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {NbtUtil.TagTypeName(tag.TagType)} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
                this.Text = $"Edit {NbtUtil.TagTypeName(tag.TagType)} Tag";

            if (SettingName && (!SettingValue || purpose != EditPurpose.EditValue))
            {
                NameBox.Select();
                NameBox.SelectAll();
            }
            else if (SettingValue)
            {
                ValueBox.Select();
                ValueBox.SelectAll();
            }
        }

        public static NbtTag CreateTag(NbtTagType type, NbtContainerTag parent, bool bypass_window = false)
        {
            bool has_name = parent is NbtCompound;
            bool has_value = NbtUtil.IsValueType(type);

            var tag = NbtUtil.CreateTag(type);

            if (bypass_window)
            {
                tag.Name = NbtUtil.GetAutomaticName(tag, parent);
                return tag;
            }
            else if (has_name || has_value)
            {
                var window = new EditTagWindow(tag, parent, has_name, has_value, EditPurpose.Create);
                return window.ShowDialog() == DialogResult.OK ? tag : null;
            }
            else
                return tag; // no customization required, example: adding a compound to a list
        }

        public static bool ModifyTag(NbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is NbtCompound;
            bool has_value = NbtUtil.IsValueType(existing.TagType);

            if (has_name || has_value)
            {
                var window = new EditTagWindow(existing, parent, has_name, has_value, purpose);
                return window.ShowDialog() == DialogResult.OK; // window modifies the tag by itself
            }
            return false;
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
            // check conditions first, tag must not be modified at ALL until we can be sure it's safe
            if (SettingName && !NameBox.CheckName())
                return false;
            object value = null;
            if (SettingValue && !ValueBox.CheckValue(out value))
                return false;

            if (SettingName)
                NameBox.ApplyName();
            if (SettingValue)
                ValueBox.ApplyValue(value);
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }
    }

    public enum EditPurpose
    {
        Create,
        Rename,
        EditValue
    }
}