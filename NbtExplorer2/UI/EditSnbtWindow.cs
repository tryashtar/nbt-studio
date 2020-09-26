using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditSnbtWindow : Form
    {
        private INbtTag WorkingTag;
        private readonly INbtContainer TagParent;
        private readonly bool SettingName;

        public EditSnbtWindow(INbtTag tag, INbtContainer parent, bool set_name)
        {
            InitializeComponent();

            WorkingTag = tag;
            TagParent = parent;
            SettingName = set_name;

            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;
            if (tag == null)
            {
                this.Icon = Properties.Resources.action_edit_snbt_icon;
                this.Text = $"Create Tag as SNBT";
            }
            else
            {
                this.Icon = NbtUtil.TagTypeIcon(tag.TagType);
                this.Text = $"Edit {NbtUtil.TagTypeName(tag.TagType)} Tag as SNBT";
                NameBox.Text = tag.Name;
                InputBox.Text = tag.ToSnbt(expanded: true);
            }
        }

        public static INbtTag CreateTag(INbtContainer parent)
        {
            bool has_name = parent is INbtCompound;
            var window = new EditSnbtWindow(null, parent, has_name);
            return window.ShowDialog() == DialogResult.OK ? window.WorkingTag : null;
        }

        public static bool ModifyTag(INbtTag existing)
        {
            var parent = existing.Parent;
            bool has_name = parent is INbtCompound;
            var window = new EditSnbtWindow(existing, parent, has_name);
            return window.ShowDialog() == DialogResult.OK; // window modifies the tag by itself
        }

        private void Apply()
        {
            if (TryModify())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private NbtTag ParseTag()
        {
            return SnbtParser.Parse(InputBox.Text, named: false);
        }

        private NbtTagType? RequiredType()
        {
            if (WorkingTag == null)
            {
                if (TagParent is INbtList list)
                {
                    if (list.Count == 0)
                        return null;
                    return list.ListType;
                }
                return null;
            }
            return WorkingTag.TagType;
        }

        private bool TryModify()
        {
            var name = NameBox.Text.Trim();
            if (SettingName)
            {
                if (name == "")
                {
                    MessageBox.Show("The name cannot be empty");
                    return false;
                }
                if (TagParent is INbtCompound compound && (WorkingTag == null || name != WorkingTag.Name) && compound.Contains(name))
                {
                    MessageBox.Show($"Duplicate name; this compound already contains a tag named \"{name}\"");
                    return false;
                }
            }
            NbtTag tag;
            try
            {
                tag = ParseTag();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The SNBT is not valid:\n{ex.Message}");
                return false;
            }
            var required_type = RequiredType();
            if (required_type != null && required_type.Value != tag.TagType)
            {
                MessageBox.Show($"The SNBT must be of type {required_type}, not {tag.TagType}");
                return false;
            }
            if (WorkingTag == null)
                WorkingTag = tag.Adapt();
            else
                NbtUtil.SetValue(WorkingTag, NbtUtil.GetValue(tag.Adapt()));
            if (SettingName)
                WorkingTag.Name = name;
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void MinifyCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var tag = ParseTag();
                InputBox.Text = tag.Adapt().ToSnbt(!MinifyCheck.Checked);
            }
            catch
            {
                // change it back
                MinifyCheck.CheckedChanged -= MinifyCheck_CheckedChanged;
                MinifyCheck.Checked ^= true;
                MinifyCheck.CheckedChanged += MinifyCheck_CheckedChanged;
            }
        }
    }
}