using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class EditTagWindow : Form
    {
        private readonly bool SettingName;
        private readonly bool SettingValue;

        private EditTagWindow(IconSource source, NbtTag tag, NbtContainerTag parent, bool set_name, bool set_value, EditPurpose purpose)
        {
            InitializeComponent();

            NameBox.SetTags(tag, parent);
            ValueBox.SetTags(tag, parent, fill_current_value: purpose != EditPurpose.Create);

            SettingName = set_name;
            if (!SettingName)
            {
                this.MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height - MainTable.GetRowHeights()[0]);
                this.Height -= MainTable.GetRowHeights()[0];
                MainTable.RowStyles[0].Height = 0;
                NameLabel.Visible = false;
                NameBox.Visible = false;
            }

            SettingValue = set_value;
            if (!SettingValue)
            {
                this.MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height - MainTable.GetRowHeights()[1]);
                this.Height -= MainTable.GetRowHeights()[1];
                ValueLabel.Visible = false;
                ValueBox.Visible = false;
            }

            if (tag.TagType == NbtTagType.String)
            {
                ValueBox.Multiline = true;
                ValueBox.AcceptsReturn = true;
                ValueBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                this.AutoSize = false;
                this.Width = (int)(this.Width * 1.5);
                this.Height = (int)(this.Height * 1.5);
                this.FormBorderStyle = FormBorderStyle.Sizable;
                WordWrapCheck.Visible = true;
                WordWrapCheck_CheckedChanged(this, EventArgs.Empty);
            }
            else if (NbtUtil.IsNumericType(tag.TagType))
            {
                ValueBox.PlaceholderText = "0";
            }
            this.Icon = NbtUtil.TagTypeImage(source, tag.TagType).Icon;
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

        public static NbtTag CreateTag(IconSource source, NbtTagType type, NbtContainerTag parent, bool bypass_window = false)
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
                var window = new EditTagWindow(source, tag, parent, has_name, has_value, EditPurpose.Create);
                return window.ShowDialog() == DialogResult.OK ? tag : null;
            }
            else
                return tag; // no customization required, example: adding a compound to a list
        }

        public static bool ModifyTag(IconSource source, NbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is NbtCompound;
            bool has_value = NbtUtil.IsValueType(existing.TagType);

            if (has_name || has_value)
            {
                var window = new EditTagWindow(source, existing, parent, has_name, has_value, purpose);
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // normal enter is handled by the AcceptButton property being OkButton
            // except when editing a multiline string
            if (keyData == (Keys.Control | Keys.Enter) || keyData == (Keys.Shift | Keys.Enter))
            {
                Confirm();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void EditTagWindow_Load(object sender, EventArgs e)
        {
            WordWrapCheck.Checked = Properties.Settings.Default.TagWordWrap;
        }

        private void EditTagWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.TagWordWrap = WordWrapCheck.Checked;
        }

        private void WordWrapCheck_CheckedChanged(object sender, EventArgs e)
        {
            ValueBox.WordWrap = WordWrapCheck.Checked;
        }
    }

    public enum EditPurpose
    {
        Create,
        Rename,
        EditValue
    }
}