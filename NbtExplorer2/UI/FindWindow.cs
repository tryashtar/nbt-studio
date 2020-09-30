using Aga.Controls.Tree;
using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class FindWindow : Form
    {
        private TreeNodeAdv LastFound;
        private readonly NbtTreeView SearchingView;

        public enum SearchDirection
        {
            Forward,
            Backward
        }

        public FindWindow(NbtTreeView view)
        {
            InitializeComponent();

            SearchingView = view;
            this.Icon = Properties.Resources.action_search_icon;
        }

        private bool Matches(TreeNodeAdv adv)
        {
            var texts = NbtText.GetText(adv);
            var combined = texts.Item1 + texts.Item2;
            return combined.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public void Search(SearchDirection direction)
        {
            var backup = direction == SearchDirection.Forward ? SearchingView.Root : SearchingView.FinalNode;
            var start = SearchingView.SelectedNode ?? LastFound ?? backup;
            var find = SearchingView.SearchFrom(start, Matches, direction == SearchDirection.Forward);
            if (find == SearchingView.Root)
                find = null;
            if (find == null)
            {
                // wrap around and look again, but only up until the original starting point
                if (backup != start && backup != SearchingView.Root && Matches(backup))
                    find = backup;
                else
                    find = SearchingView.SearchFrom(backup, x => x != start && Matches(x), true);
            }
            if (find != null)
            {
                LastFound = find;
                SearchingView.SelectedNode = find;
                SearchingView.EnsureVisible(find);
            }
        }

        private void ButtonFindNext_Click(object sender, EventArgs e)
        {
            Search(SearchDirection.Forward);
        }

        private void ButtonFindPrev_Click(object sender, EventArgs e)
        {
            Search(SearchDirection.Backward);
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
            if (keyData==Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FindWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }
    }
}