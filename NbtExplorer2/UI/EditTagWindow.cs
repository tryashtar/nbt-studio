using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditTagWindow : Form
    {
        private readonly INbtTag WorkingTag;
        private readonly INbtTag TagParent;
        private readonly bool SettingName;
        private readonly bool SettingValue;
        private readonly bool SettingSize;

        private EditTagWindow(INbtTag tag, INbtTag parent, bool set_name, bool set_value, bool set_size, EditPurpose purpose)
        {
            InitializeComponent();

            WorkingTag = tag;
            TagParent = parent;

            SettingName = set_name;
            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;

            SettingValue = set_value;
            ValueLabel.Visible = SettingValue;
            ValueBox.Visible = SettingValue;

            SettingSize = set_size;
            SizeLabel.Visible = SettingSize;
            SizeBox.Visible = SettingSize;

            this.Icon = NbtUtil.TagTypeIcon(tag.TagType);
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {NbtUtil.TagTypeName(tag.TagType)} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
            {
                this.Text = $"Edit {NbtUtil.TagTypeName(tag.TagType)} Tag";
                NameBox.Text = tag.Name;
                ValueBox.Text = NbtUtil.PreviewNbtValue(tag);
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

        public static NbtTag CreateTag(NbtTagType type, INbtTag parent, bool bypass_window = false)
        {
            bool has_name = parent is INbtCompound;
            bool has_value = NbtUtil.IsValueType(type);
            bool has_size = NbtUtil.IsArrayType(type);

            var tag = NbtUtil.CreateTag(type);

            if (bypass_window)
            {
                if (has_name)
                    tag.Name = NbtUtil.GetAutomaticName(tag.Adapt(), (INbtCompound)parent);
                return tag;
            }
            else if (has_name || has_value || has_size)
            {
                var window = new EditTagWindow(tag.Adapt(), parent, has_name, has_value, has_size, EditPurpose.Create);
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
                var window = new EditTagWindow(existing, parent, has_name, has_value, false, purpose);
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
            var name = NameBox.Text.Trim();
            var str_size = SizeBox.Text.Trim();
            int? int_size = null;
            var str_value = ValueBox.Text.Trim();
            object parsed_value = null;

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
            if (SettingSize && str_size != "")
            {
                try
                {
                    int_size = Util.ParseNonNegativeInt(str_size);
                }
                catch (FormatException)
                {
                    MessageBox.Show($"The size is formatted incorrectly");
                    return false;
                }
                catch (OverflowException)
                {
                    MessageBox.Show($"The value for size must be between 0 and {int.MinValue}");
                    return false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show($"The size cannot be less than zero");
                    return false;
                }
                catch { return false; }
            }
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
                WorkingTag.Name = name;
            if (SettingSize && int_size.HasValue)
                NbtUtil.SetSize(WorkingTag, int_size.Value);
            if (SettingValue && parsed_value != null)
                NbtUtil.SetValue(WorkingTag, parsed_value);
            return true;
        }

        private void OkButton_Click(object sender, EventArgs e)
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