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

        private bool Matches(TreeNodeAdv adv)
        {
            string name = NbtText.PreviewName(adv);
            string value = NbtText.PreviewValue(adv);
            return NameBox.IsMatch(name) && ValueBox.IsMatch(value);
        }

        public void Search(SearchDirection direction)
        {
            if (!ValidateRegex()) return;
            var backup = direction == SearchDirection.Forward ? SearchingView.Root : SearchingView.FinalNode;
            var start = SearchingView.SelectedNode ?? LastFound ?? backup;
            var find = SearchingView.SearchFrom(start, Matches, direction);
            if (find == SearchingView.Root)
                find = null;
            if (find == null)
            {
                // wrap around and look again, but only up until the original starting point
                if (backup != start && backup != SearchingView.Root && Matches(backup))
                    find = backup;
                else
                    find = SearchingView.SearchFrom(backup, x => x != start && Matches(x), direction);
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
            var results = SearchingView.SearchAll(Matches);
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