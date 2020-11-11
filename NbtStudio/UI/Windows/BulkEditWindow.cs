using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class BulkEditWindow : Form
    {
        private readonly BulkEditPurpose Purpose;
        private readonly List<INbtTag> WorkingTags;

        private BulkEditWindow(List<INbtTag> tags, BulkEditPurpose purpose)
        {
            InitializeComponent();
            SetColumnSizes();

            WorkingTags = tags;
            Purpose = purpose;
            ActionList.Items.AddRange(tags.Select(x => CreateListItem(x, TagPreview(x))).ToArray());

            if (purpose == BulkEditPurpose.Rename)
            {
                this.Text = $"Rename {Util.Pluralize(tags.Count, "tag")}";
                this.Icon = Properties.Resources.action_rename_icon;
                CurrentColumn.Text = "Current Name";
                NewColumn.Text = "New Name";
            }
            else
            {
                this.Text = $"Edit {Util.Pluralize(tags.Count, "tag")}";
                this.Icon = Properties.Resources.action_edit_icon;
                CurrentColumn.Text = "Current Value";
                NewColumn.Text = "New Value";
            }
        }

        private ListViewItem CreateListItem(INbtTag tag, string str)
        {
            return new ListViewItem(new[] { str, "" }) { Tag = tag };
        }

        private string TagPreview(INbtTag tag)
        {
            if (Purpose == BulkEditPurpose.Rename)
                return tag.Name;
            else
                return NbtUtil.PreviewNbtValue(tag);
        }

        public static void BulkRename(IEnumerable<INbtTag> tags)
        {
            var list = tags.Where(x => x.Parent is INbtCompound).ToList();
            if (list.Any())
            {
                var window = new BulkEditWindow(list, BulkEditPurpose.Rename);
                window.ShowDialog();
            }
        }

        public static void BulkEdit(IEnumerable<INbtTag> tags)
        {
            var list = tags.Where(x => NbtUtil.IsValueType(x.TagType)).ToList();
            if (list.Any())
            {
                var window = new BulkEditWindow(list, BulkEditPurpose.EditValue);
                window.ShowDialog();
            }
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
            if (!FindBox.CheckRegex(out _)) return false;
            var transformer = GetTransformer();
            foreach (ListViewItem item in ActionList.Items)
            {
                string current = item.SubItems[0].Text;
                string transformed = transformer(current);
                if (current == transformed || transformed == "")
                    continue;
                var tag = (INbtTag)item.Tag;
                if (IsValidFor(tag, transformed, out var result))
                {
                    if (Purpose == BulkEditPurpose.Rename)
                        tag.Name = transformed;
                    else
                        NbtUtil.SetValue(tag, result);
                }
            }
            return true;
        }

        private void SetColumnSizes()
        {
            CurrentColumn.Width = ActionList.Width / 2;
            NewColumn.Width = ActionList.Width / 2;
        }

        private Func<string, string> GetTransformer()
        {
            string find_text = FindBox.Text;
            string replace_text = ReplaceBox.Text;
            if (find_text == "" && replace_text == "")
                return x => x;
            if (RegexCheck.Checked)
            {
                FindBox.CheckRegexQuiet(out var find);
                if (find == null)
                    return x => x;
                if (find_text == "")
                    find = new Regex(".*");
                return x => find.Replace(x, m => m.Value == "" ? "" : m.Result(replace_text));
            }
            else
            {
                if (find_text == "")
                    return x => replace_text;
                return x => x.FastReplace(find_text, replace_text, StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool IsValidFor(INbtTag tag, string value, out object result)
        {
            result = null;
            if (Purpose == BulkEditPurpose.Rename)
            {
                var existing = ((INbtCompound)tag.Parent)[value];
                return existing == null || existing == tag;
            }
            else
            {
                try { result = NbtUtil.ParseValue(value, tag.TagType); }
                catch { return false; }
                return true;
            }
        }

        private void UpdatePreview()
        {
            var transformer = GetTransformer();
            foreach (ListViewItem item in ActionList.Items)
            {
                string current = item.SubItems[0].Text;
                string transformed = transformer(current);
                if (current == transformed || transformed == "")
                {
                    item.SubItems[1].Text = "";
                    item.BackColor = default;
                }
                else
                {
                    item.SubItems[1].Text = transformed;
                    var tag = (INbtTag)item.Tag;
                    item.BackColor = IsValidFor(tag, transformed, out _) ? default : Color.Pink;
                }
            }
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }

        private void ActionList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                e.Item.Selected = false;
        }

        private void ActionList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = ActionList.Width / 2;
            e.Cancel = true;
        }

        private void RegexCheck_CheckedChanged(object sender, EventArgs e)
        {
            FindBox.RegexMode = RegexCheck.Checked;
            UpdatePreview();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                RegexCheck.Checked = !RegexCheck.Checked;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BulkEditWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
            RegexCheck.Checked = Properties.Settings.Default.FindRegex;
            FindBox.Text = Properties.Settings.Default.FindText;
            ReplaceBox.Text = Properties.Settings.Default.ReplaceText;
        }

        private void BulkEditWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.FindRegex = RegexCheck.Checked;
            Properties.Settings.Default.FindText = FindBox.Text;
            Properties.Settings.Default.ReplaceText = ReplaceBox.Text;

        }

        private void FindBox_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void ReplaceBox_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }

    public enum BulkEditPurpose
    {
        Rename,
        EditValue
    }
}