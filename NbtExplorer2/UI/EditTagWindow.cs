using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditTagWindow : Form
    {
        private readonly EditPurpose Purpose;
        private readonly NbtTag WorkingTag;
        private readonly NbtTag TagParent;
        private readonly bool SettingName;
        private readonly bool SettingValue;
        private readonly bool SettingSize;

        private EditTagWindow(NbtTag tag, NbtTag parent, bool set_name, bool set_value, bool set_size, EditPurpose purpose)
        {
            InitializeComponent();

            Purpose = purpose;
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

            this.Icon = Util.TagTypeIcon(tag.TagType);
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {Util.TagTypeName(tag.TagType)} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
            {
                this.Text = $"Edit {Util.TagTypeName(tag.TagType)} Tag";
                NameBox.Text = tag.Name;
                ValueBox.Text = tag.ToSnbt(false, false);
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

        public static bool CreateTag(NbtTagType type, NbtTag parent)
        {
            bool has_name = parent is NbtCompound;
            bool has_value = Util.IsValueType(type);
            bool has_size = Util.IsSizeType(type);

            var tag = Util.CreateTag(type);

            if (has_name || has_value || has_size)
            {
                var window = new EditTagWindow(tag, parent, has_name, has_value, has_size, EditPurpose.Create);
                return window.ShowDialog() == DialogResult.OK; // window adds the tag by itself
            }
            else
            {
                // no customization required, just add it directly
                // example: adding a compound to a list
                Util.Add(parent, tag);
                return true;
            }
        }

        public static bool ModifyTag(NbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is NbtCompound;
            bool has_value = Util.IsValueType(existing.TagType);

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
                if (Purpose == EditPurpose.Create)
                    Util.Add(TagParent, WorkingTag);
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
                if (TagParent is NbtCompound compound && name != WorkingTag.Name && compound.Contains(name))
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
                    parsed_value = Util.ParseValue(str_value, WorkingTag.TagType);
                }
                catch (FormatException)
                {
                    MessageBox.Show($"The value is formatted incorrectly for a {Util.TagTypeName(WorkingTag.TagType).ToLower()}");
                    return false;
                }
                catch (OverflowException)
                {
                    var minmax = Util.MinMaxFor(WorkingTag.TagType);
                    MessageBox.Show($"The value for {Util.TagTypeName(WorkingTag.TagType).ToLower()}s must be between {minmax.Item1} and {minmax.Item2}");
                    return false;
                }
                catch { return false; }
            }
            if (SettingName)
                WorkingTag.Name = name;
            if (SettingSize && int_size.HasValue)
                Util.SetSize(WorkingTag, int_size.Value);
            if (SettingValue && parsed_value != null)
                Util.SetValue(WorkingTag, parsed_value);
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