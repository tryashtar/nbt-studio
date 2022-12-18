﻿using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class FindWindow : Form
    {
        private INode LastFound;
        private NbtTreeModel SearchingModel;
        private NbtTreeView SearchingView;

        public FindWindow(IconSource source, NbtTreeModel model, NbtTreeView view)
        {
            InitializeComponent();

            SearchingModel = model;
            SearchingView = view;
            this.Icon = source.GetImage(IconType.Search).Icon;
            UpdateButtons();
        }

        private static bool Matches(INode node, string name_search, string value_search)
        {
            string name = NbtText.PreviewName(node);
            string value = NbtText.PreviewValue(node);
            return RegexTextBox.IsMatch(name, name_search) && RegexTextBox.IsMatch(value, value_search);
        }

        private static bool MatchesRegex(INode node, Regex name_search, Regex value_search)
        {
            string name = NbtText.PreviewName(node);
            string value = NbtText.PreviewValue(node);
            return RegexTextBox.IsMatchRegex(name, name_search) && RegexTextBox.IsMatchRegex(value, value_search);
        }

        private Predicate<INode> GetPredicate()
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

        private INode DoSearch(SearchDirection direction, IProgress<TreeSearchReport> progress)
        {
            if (!ValidateRegex()) return null;
            var start = (SearchingView.SelectedNode?.Tag as INode) ?? LastFound;
            var predicate = GetPredicate();
            var find = SearchNodeOperations.SearchFrom(SearchingModel, start, predicate, direction, progress, CancelSource.Token, true);
            if (find is null)
                return null;
            else
            {
                LastFound = find;
                return find;
            }
        }

        private List<INode> DoSearchAll(IProgress<TreeSearchReport> progress)
        {
            if (!ValidateRegex()) return null;
            var predicate = GetPredicate();
            var results = SearchNodeOperations.SearchAll(SearchingModel, predicate, progress, CancelSource.Token).ToList();
            return results;
        }

        private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        private Task<IEnumerable<INode>> ActiveSearch;
        private void StartActiveSearch(Func<IProgress<TreeSearchReport>, List<INode>> function)
        {
            if (ActiveSearch is not null && !ActiveSearch.IsCompleted)
                return;
            var progress = new Progress<TreeSearchReport>();
            progress.ProgressChanged += Progress_ProgressChanged;
            ActiveSearch = new Task<IEnumerable<INode>>(() => function(progress));
            ActiveSearch.Start();
            ProgressBar.Visible = true;
            FoundResultsLabel.Visible = false;
            ProgressBar.Value = 0;
            ActiveSearch.ContinueWith(x =>
            {
                ProgressBar.Visible = false;
                if (x.Result is null || !x.Result.Any())
                {
                    FoundResultsLabel.Text = "No results found";
                    FoundResultsLabel.Visible = true;
                }
                else
                {
                    if (x.Result.CountGreaterThan(1))
                    {
                        FoundResultsLabel.Text = $"Found {x.Result.Count()} matching results";
                        FoundResultsLabel.Visible = true;
                    }
                    SearchingView.ClearSelection();
                    LastFound = x.Result.Last();
                    foreach (var item in x.Result)
                    {
                        var node = SearchingView.FindNode(item.Path, true);
                        if (node is not null)
                        {
                            if (x.Result.Count() < 4000)
                            {
                                FastEnsureVisible(node);
                            }
                            node.IsSelected = true;
                        }
                    }
                    var scroll = SearchingView.FindNode(LastFound.Path, true);
                    if (scroll is not null)
                        SearchingView.ScrollTo(scroll);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Progress_ProgressChanged(object sender, TreeSearchReport e)
        {
            int value = (int)(e.Percentage * 100);
            ProgressBar.Value = Math.Max(1, Math.Min(100, value));
            ProgressBar.Value--; // fix animation
        }

        private void FastEnsureVisible(TreeNodeAdv node)
        {
            while (node is not null)
            {
                node = node.Parent;
                node?.Expand();
            }
        }

        public void Search(SearchDirection direction)
        {
            List<INode> ItemOrNull(INode item)
            {
                if (item is null)
                    return null;
                return new List<INode> { item };
            }
            StartActiveSearch(x => ItemOrNull(DoSearch(direction, x)));
        }

        public void SearchAll()
        {
            StartActiveSearch(DoSearchAll);
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
            if (keyData == (Keys.Control | Keys.Enter) && ButtonFindAll.Enabled)
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
            NameBox.Text = Properties.Settings.Default.FindName;
            ValueBox.Text = Properties.Settings.Default.FindValue;
        }

        private void FindWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            CancelSource.Cancel();
            Properties.Settings.Default.FindRegex = RegexCheck.Checked;
            Properties.Settings.Default.FindName = NameBox.Text;
            Properties.Settings.Default.FindValue = ValueBox.Text;
        }

        private void RegexCheck_CheckedChanged(object sender, EventArgs e)
        {
            NameBox.RegexMode = RegexCheck.Checked;
            ValueBox.RegexMode = RegexCheck.Checked;
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void ValueBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            ButtonFindAll.Enabled = NameBox.Text != "" || ValueBox.Text != "";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }
            LastFound = null;
            SearchingModel = null;
            SearchingView = null;
            CancelSource.Cancel();
            ActiveSearch = null;
            base.Dispose(disposing);
        }
    }
}