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
        private readonly INbtTag WorkingTag;
        private readonly INbtContainer TagParent;
        private readonly bool SettingName;
        private readonly bool SettingValue;

        private EditTagWindow(INbtTag tag, INbtContainer parent, bool set_name, bool set_value, EditPurpose purpose)
        {
            InitializeComponent();

            WorkingTag = tag;
            TagParent = parent;
            NameBox.SetTags(tag, parent);

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
            {
                this.Text = $"Edit {NbtUtil.TagTypeName(tag.TagType)} Tag";
                NameBox.Text = tag.Name;
                ValueBox.Text = NbtUtil.PreviewNbtValue(tag).Replace("\r", "").Replace("\n", Environment.NewLine);
            }

            if (SettingName && purpose != EditPurpose.EditValue)
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

        public static NbtTag CreateTag(NbtTagType type, INbtContainer parent, bool bypass_window = false)
        {
            bool has_name = parent is INbtCompound;
            bool has_value = NbtUtil.IsValueType(type);

            var tag = NbtUtil.CreateTag(type);

            if (bypass_window)
            {
                if (has_name)
                    tag.Name = NbtUtil.GetAutomaticName(tag.Adapt(), (INbtCompound)parent);
                return tag;
            }
            else if (has_name || has_value)
            {
                var window = new EditTagWindow(tag.Adapt(), parent, has_name, has_value, EditPurpose.Create);
                return window.ShowDialog() == DialogResult.OK ? tag : null;
            }
            else
                return tag; // no customization required, example: adding a compound to a list
        }

        public static bool ModifyTag(INbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is INbtCompound;
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
            var str_value = ValueBox.Text.Trim().Replace("\r", "");
            object parsed_value = null;

            // check conditions first, tag must not be modified at ALL until we can be sure it's safe
            if (SettingName && !NameBox.CheckName())
                return false;
            if (SettingValue && str_value != "")
            {
                try
                {
                    parsed_value = NbtUtil.ParseValue(str_value, WorkingTag.TagType);
                }
                catch (FormatException)
                {
                    MessageBox.Show($"The value is formatted incorrectly for a {NbtUtil.TagTypeName(WorkingTag.TagType).ToLower()}");
                    return false;
                }
                catch (OverflowException)
                {
                    var minmax = NbtUtil.MinMaxFor(WorkingTag.TagType);
                    MessageBox.Show($"The value for {NbtUtil.TagTypeName(WorkingTag.TagType).ToLower()}s must be between {minmax.Item1} and {minmax.Item2}");
                    return false;
                }
                catch { return false; }
            }
            if (SettingName)
                NameBox.ApplyName();
            if (SettingValue && parsed_value != null)
                NbtUtil.SetValue(WorkingTag, parsed_value);
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