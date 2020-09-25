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

            this.Icon = INbt.TagTypeIcon(tag.TagType);
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {INbt.TagTypeName(tag.TagType)} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
            {
                this.Text = $"Edit {INbt.TagTypeName(tag.TagType)} Tag";
                NameBox.Text = tag.Name;
                ValueBox.Text = INbt.PreviewNbtValue(tag);
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

        public static string GetAutomaticName(INbtTag tag, INbtCompound parent)
        {
            if (tag.Name != null && !parent.Contains(tag.Name))
                return tag.Name;
            string basename = tag.Name ?? INbt.TagTypeName(tag.TagType).ToLower().Replace(' ', '_');
            for (int i = 1; i < 999999; i++)
            {
                string name = basename + i.ToString();
                if (!parent.Contains(name))
                    return name;
            }
            throw new InvalidOperationException("This compound really contains 999999 similarly named tags?!");
        }

        public static NbtTag CreateTag(NbtTagType type, INbtTag parent, bool bypass_window = false)
        {
            bool has_name = parent is INbtCompound;
            bool has_value = INbt.IsValueType(type);
            bool has_size = INbt.IsArrayType(type);

            var tag = INbt.CreateTag(type);

            if (bypass_window)
            {
                if (has_name)
                    tag.Name = GetAutomaticName(tag.Adapt(), (INbtCompound)parent);
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
            bool has_value = INbt.IsValueType(existing.TagType);

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
                    parsed_value = INbt.ParseValue(str_value, WorkingTag.TagType);
                }
                catch (FormatException)
                {
                    MessageBox.Show($"The value is formatted incorrectly for a {INbt.TagTypeName(WorkingTag.TagType).ToLower()}");
                    return false;
                }
                catch (OverflowException)
                {
                    var minmax = INbt.MinMaxFor(WorkingTag.TagType);
                    MessageBox.Show($"The value for {INbt.TagTypeName(WorkingTag.TagType).ToLower()}s must be between {minmax.Item1} and {minmax.Item2}");
                    return false;
                }
                catch { return false; }
            }
            if (SettingName)
                WorkingTag.Name = name;
            if (SettingSize && int_size.HasValue)
                INbt.SetSize(WorkingTag, int_size.Value);
            if (SettingValue && parsed_value != null)
                INbt.SetValue(WorkingTag, parsed_value);
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