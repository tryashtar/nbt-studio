using Aga.Controls.Tree;
using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NbtStudio.UI
{
    public partial class FindWindow : Form
    {
        private TreeNodeAdv LastFound;
        private readonly NbtTreeView SearchingView;
        public string SearchName => NameBox.Text;
        public string SearchValue => ValueBox.Text;

        public FindWindow(NbtTreeView view)
        {
            InitializeComponent();

            SearchingView = view;
            this.Icon = Properties.Resources.action_search_icon;
        }

        private static bool Matches(TreeNodeAdv adv, string name_search, string value_search)
        {
            string name = NbtText.PreviewName(adv);
            string value = NbtText.PreviewValue(adv);
            return RegexTextBox.IsMatch(name, name_search) && RegexTextBox.IsMatch(value, value_search);
        }

        private static bool MatchesRegex(TreeNodeAdv adv, Regex name_search, Regex value_search)
        {
            string name = NbtText.PreviewName(adv);
            string value = NbtText.PreviewValue(adv);
            return RegexTextBox.IsMatchRegex(name, name_search) && RegexTextBox.IsMatchRegex(value, value_search);
        }

        private Predicate<TreeNodeAdv> GetPredicate()
        {
            if (RegexCheck.Checked)
            {
                NameBox.CheckRegex(out var name_search);
                ValueBox.CheckRegex(out var value_search);
                return x => MatchesRegex(x, name_search, value_search);
            }
            else
            {
                string name_search = NameBox.Text;
                string value_search = ValueBox.Text;
                return x => Matches(x, name_search, value_search);
            }
        }

        public void Search(SearchDirection direction)
        {
            if (!ValidateRegex()) return;
            var backup = direction == SearchDirection.Forward ? SearchingView.Root : SearchingView.FinalNode;
            var start = SearchingView.SelectedNode ?? LastFound ?? backup;
            var predicate = GetPredicate();
            var find = SearchingView.SearchFrom(start, predicate, direction);
            if (find == SearchingView.Root)
                find = null;
            if (find == null)
            {
                // wrap around and look again, but only up until the original starting point
                if (backup != start && backup != SearchingView.Root && predicate(backup))
                    find = backup;
                else
                    find = SearchingView.SearchFrom(backup, x => x != start && predicate(x), direction);
            }
            if (find != null)
            {
                LastFound = find;
                SearchingView.SelectedNode = find;
                SearchingView.EnsureVisible(find);
            }
        }

        public void SearchAll()
        {
            if (!ValidateRegex()) return;
            var predicate = GetPredicate();
            var results = SearchingView.SearchAll(predicate);
            if (results.Any())
            {
                SearchingView.ClearSelection();
                LastFound = results.Last();
                foreach (var item in results)
                {
                    SearchingView.EnsureVisible(item);
                    item.IsSelected = true;
                }
            }
        }

        private bool ValidateRegex()
        {
            return NameBox.CheckRegex(out _) && ValueBox.CheckRegex(out _);
        }

        private void ButtonFindNext_Click(object sender, EventArgs e)
        {
            Search(SearchDirection.Forward);
        }

        private void ButtonFindPrev_Click(object sender, EventArgs e)
        {
            Search(SearchDirection.Backward);
        }

        private void ButtonFindAll_Click(object sender, EventArgs e)
        {
            SearchAll();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // handle shift-enter to search backwards
            // AcceptButton takes care of the forwards case
            if (keyData == (Keys.Shift | Keys.Enter))
            {
                Search(SearchDirection.Backward);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Enter))
            {
                SearchAll();
                return true;
            }
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

        private void FindWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
            RegexCheck.Checked = Properties.Settings.Default.FindRegex;
        }

        private void FindWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.FindRegex = RegexCheck.Checked;
        }

        private void RegexCheck_CheckedChanged(object sender, EventArgs e)
        {
            NameBox.RegexMode = RegexCheck.Checked;
            ValueBox.RegexMode = RegexCheck.Checked;
        }
    }
}