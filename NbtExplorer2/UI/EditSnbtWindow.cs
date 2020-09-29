using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditSnbtWindow : Form
    {
        private INbtTag WorkingTag;
        private readonly INbtContainer TagParent;
        private readonly bool SettingName;
        public bool CheckWhileTyping = false;

        public EditSnbtWindow(INbtTag tag, INbtContainer parent, bool set_name, EditPurpose purpose)
        {
            InitializeComponent();
            InputBox.Size = new Size(0, 0);

            WorkingTag = tag;
            TagParent = parent;
            SettingName = set_name;
            var required = RequiredType();
            if (required == null || required.Value == NbtTagType.Compound || required.Value == NbtTagType.List)
            {
                this.Width = this.Width * 5 / 2;
                this.Height = this.Height * 5 / 2;
                MinifyCheck.Visible = true;
                InputBox.Multiline = true;
            }
            else if (required.Value == NbtTagType.String || NbtUtil.IsArrayType(required.Value))
            {
                this.Width = this.Width * 2;
                this.Height = this.Height * 3 / 2;
                InputBox.Multiline = true;
                InputBox.WordWrap = true;
                InputBox.ScrollBars = ScrollBars.Vertical;
            }
            else if (required.Value == NbtTagType.Float || required.Value == NbtTagType.Double)
            {
                this.Width = this.Width * 3 / 2;
            }
            InputBox.AcceptsReturn = InputBox.Multiline;
            if (InputBox.Multiline)
                this.FormBorderStyle = FormBorderStyle.Sizable;

            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;
            if (tag != null)
            {
                NameBox.Text = tag.Name;
                InputBox.Text = tag.ToSnbt(expanded: true);
            }
            if (required == null)
            {
                this.Icon = Properties.Resources.action_add_snbt_icon;
                this.Text = "Create Tag as SNBT";
            }
            else
            {
                this.Icon = NbtUtil.TagTypeIcon(required.Value);
                this.Text = tag == null ? $"Create {NbtUtil.TagTypeName(required.Value)} Tag as SNBT" : $"Edit {NbtUtil.TagTypeName(required.Value)} Tag as SNBT";
            }
            if (SettingName && purpose != EditPurpose.EditValue)
            {
                NameBox.Select();
                NameBox.SelectAll();
            }
            else
            {
                InputBox.Select();
                InputBox.SelectAll();
            }
        }

        public static INbtTag CreateTag(INbtContainer parent)
        {
            bool has_name = parent is INbtCompound;
            var window = new EditSnbtWindow(null, parent, has_name, EditPurpose.Create);
            return window.ShowDialog() == DialogResult.OK ? window.WorkingTag : null;
        }

        public static bool ModifyTag(INbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is INbtCompound;
            var window = new EditSnbtWindow(existing, parent, has_name, purpose);
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
            return SnbtParser.Parse(InputBox.Text.Replace("\r", ""), named: false);
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                Apply();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            if (!CheckWhileTyping)
                return;
            bool valid = true;
            InputBox.BackColor = SystemColors.Window;
            try
            {
                var tag = ParseTag();
                var required = RequiredType();
                if (required != null && tag.TagType != required)
                    valid = false;
            }
            catch
            {
                valid = false;
            }
            if (!valid)
                InputBox.BackColor = Blend(Color.Red, SystemColors.Window, 0.1);
        }

        private static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }
    }
}