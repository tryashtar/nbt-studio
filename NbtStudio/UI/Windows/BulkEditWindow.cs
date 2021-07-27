using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class BulkEditWindow : Form
    {
        private readonly BulkEditPurpose Purpose;
        private readonly List<NbtTag> ChangedTags = new();
        private int ChangingCount = 0;
        private readonly ColumnConsistinator Consistinator;

        private BulkEditWindow(IconSource source, List<NbtTag> tags, BulkEditPurpose purpose)
        {
            InitializeComponent();

            Purpose = purpose;
            ActionList.Items.AddRange(tags.Select(x => CreateListItem(x, TagPreview(x))).ToArray());
            SetSize();
            Consistinator = new(this, ActionList);
            this.Height += 200;

            if (purpose == BulkEditPurpose.Rename)
            {
                this.Text = $"Rename {StringUtils.Pluralize(tags.Count, "tag")}";
                this.Icon = source.GetImage(IconType.Rename).Icon;
                CurrentColumn.Text = "Current Name";
                NewColumn.Text = "New Name";
            }
            else
            {
                this.Text = $"Edit {StringUtils.Pluralize(tags.Count, "tag")}";
                this.Icon = source.GetImage(IconType.Edit).Icon;
                CurrentColumn.Text = "Current Value";
                NewColumn.Text = "New Value";
            }
        }

        private ListViewItem CreateListItem(NbtTag tag, string str)
        {
            return new ListViewItem(new[] { str, "" }) { Tag = tag, Checked = true };
        }

        private string TagPreview(NbtTag tag)
        {
            if (Purpose == BulkEditPurpose.Rename)
                return tag.Name;
            else
                return NbtUtil.PreviewNbtValue(tag);
        }

        // returns changed tags
        public static IEnumerable<NbtTag> BulkRename(IconSource source, IEnumerable<NbtTag> tags)
        {
            var list = tags.Where(x => x.Parent is NbtCompound).ToList();
            if (list.Any())
            {
                var window = new BulkEditWindow(source, list, BulkEditPurpose.Rename);
                window.ShowDialog();
                return window.ChangedTags;
            }
            return Enumerable.Empty<NbtTag>();
        }

        // returns changed tags
        public static IEnumerable<NbtTag> BulkEdit(IconSource source, IEnumerable<NbtTag> tags)
        {
            var list = tags.Where(x => NbtUtil.IsValueType(x.TagType)).ToList();
            if (list.Any())
            {
                var window = new BulkEditWindow(source, list, BulkEditPurpose.EditValue);
                window.ShowDialog();
                return window.ChangedTags;
            }
            return Enumerable.Empty<NbtTag>();
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
                if (!item.Checked)
                    continue;
                string current = item.SubItems[0].Text;
                string transformed = transformer(current);
                if (current == transformed || transformed == "")
                    continue;
                var tag = (NbtTag)item.Tag;
                if (IsValidFor(tag, transformed, out var result))
                {
                    if (Purpose == BulkEditPurpose.Rename)
                        tag.Name = transformed;
                    else
                        NbtUtil.SetValue(tag, result);
                    ChangedTags.Add(tag);
                }
            }
            return true;
        }

        private static readonly int RealMinWidth = 400;
        private static readonly int RealMaxWidth = 1500;
        private void SetSize()
        {
            int width = RealMinWidth;
            var graphics = ActionList.CreateGraphics();
            foreach (ListViewItem item in ActionList.Items)
            {
                var size = graphics.MeasureString(item.Text, item.Font);
                int item_width = (int)(size.Width * 2);
                width = Math.Max(width, item_width);
                if (width > RealMaxWidth)
                {
                    width = RealMaxWidth;
                    break;
                }
            }
            this.Width = width;
        }

        private delegate string Transformer(string value);
        private Transformer GetTransformer()
        {
            string find_text = FindBox.Text;
            string replace_text = ReplaceBox.Text;
            if (find_text == "" && replace_text == "")
                return x => x;
            if (RegexCheck.Checked)
            {
                FindBox.CheckRegexQuiet(out var find);
                if (find is null)
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

        private bool IsValidFor(NbtTag tag, string value, out object result)
        {
            result = null;
            if (Purpose == BulkEditPurpose.Rename)
            {
                var existing = ((NbtCompound)tag.Parent)[value];
                return existing is null || existing == tag;
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
            ChangingCount = 0;
            foreach (ListViewItem item in ActionList.Items)
            {
                if (UpdateSinglePreview(item, transformer))
                    ChangingCount++;
            }
            UpdateChangeLabel();
        }

        private void UpdateChangeLabel()
        {
            if (Purpose == BulkEditPurpose.Rename)
                TagsChangingLabel.Text = $"Renaming {StringUtils.Pluralize(ChangingCount, "tag")}";
            else
                TagsChangingLabel.Text = $"Editing {StringUtils.Pluralize(ChangingCount, "tag")}";
            ButtonOk.Enabled = ChangingCount > 0;
        }

        // returns true if the transformation is applicable and valid
        private bool UpdateSinglePreview(ListViewItem item, Transformer transformer)
        {
            string current = item.SubItems[0].Text;
            string transformed = transformer(current);
            if (current == transformed || transformed == "" || !item.Checked)
            {
                item.SubItems[1].Text = "";
                item.BackColor = default;
                return false;
            }
            else
            {
                item.SubItems[1].Text = transformed;
                var tag = (NbtTag)item.Tag;
                bool valid = IsValidFor(tag, transformed, out _);
                item.BackColor = valid ? default : Color.Pink;
                return valid;
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

        private void ActionList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if it's currently being changed but getting unchecked, reduce count by 1
            if (e.CurrentValue == CheckState.Checked && e.NewValue == CheckState.Unchecked && UpdateSinglePreview(ActionList.Items[e.Index], GetTransformer()))
            {
                ChangingCount--;
                UpdateChangeLabel();
            }
        }

        private void ActionList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // if it just got checked and is being changed, increase count by 1
            if (e.Item.Checked && UpdateSinglePreview(e.Item, GetTransformer()))
            {
                ChangingCount++;
                UpdateChangeLabel();
            }
        }
    }

    public enum BulkEditPurpose
    {
        Rename,
        EditValue
    }
}