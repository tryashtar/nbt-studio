using Aga.Controls.Tree;
using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class FindWindow : Form
    {
        private readonly NbtTreeView SearchingView;
        private IEnumerable<TreeNodeAdv> SearchResults;

        public FindWindow(NbtTreeView view)
        {
            InitializeComponent();

            SearchingView = view;
            SearchResults = SearchingView.DepthFirstSearch();
        }

        private bool Matches(TreeNodeAdv adv)
        {
            var texts = NbtText.GetText(adv);
            var combined = texts.Item1 + texts.Item2;
            return combined.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) != -1;
        }

        private void ButtonFindNext_Click(object sender, EventArgs e)
        {
            if (SearchingView.SelectedNode == null)
                SearchingView.SelectedNode = SearchingView.Root;
            bool found_current = false;
            foreach (var item in SearchResults)
            {
                if (found_current && Matches(item))
                {
                    SearchingView.SelectedNode = item;
                    break;
                }
                if (item == SearchingView.SelectedNode)
                    found_current = true;
            }
        }

        private void ButtonFindPrev_Click(object sender, EventArgs e)
        {

        }
    }
}