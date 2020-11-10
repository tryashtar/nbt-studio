using Aga.Controls.Tree;
using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        private IEnumerable<TreeNodeAdv> DoSearch(SearchDirection direction, IProgress<TreeSearchReport> progress)
        {
            if (!ValidateRegex()) return null;
            var backup = direction == SearchDirection.Forward ? SearchingView.Root : SearchingView.FinalNode;
            var start = SearchingView.SelectedNode ?? LastFound ?? backup;
            var predicate = GetPredicate();
            var find = SearchingView.SearchFrom(start, predicate, direction, progress, true);
            if (find == SearchingView.Root)
                find = null;
            if (find == null)
                return null;
            else
            {
                LastFound = find;
                return new[] { find };
            }
        }

        private IEnumerable<TreeNodeAdv> DoSearchAll(IProgress<TreeSearchReport> progress)
        {
            if (!ValidateRegex()) return null;
            var predicate = GetPredicate();
            return SearchingView.SearchAll(predicate, progress).ToList();
        }

        private Task<IEnumerable<TreeNodeAdv>> ActiveSearch;
        private void StartActiveSearch(Func<IProgress<TreeSearchReport>, IEnumerable<TreeNodeAdv>> function)
        {
            if (ActiveSearch != null && !ActiveSearch.IsCompleted)
                return;
            var progress = new Progress<TreeSearchReport>();
            progress.ProgressChanged += Progress_ProgressChanged;
            ActiveSearch = new Task<IEnumerable<TreeNodeAdv>>(() => function(progress));
            ActiveSearch.Start();
            ProgressBar.Visible = true;
            ProgressBar.Value = 0;
            ActiveSearch.ContinueWith(x =>
            {
                ProgressBar.Visible = false;
                if (x.Result != null && x.Result.Any())
                {
                    SearchingView.ClearSelection();
                    LastFound = x.Result.Last();
                    foreach (var item in x.Result)
                    {
                        SearchingView.EnsureVisible(item);
                        item.IsSelected = true;
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Progress_ProgressChanged(object sender, TreeSearchReport e)
        {
            int value = (int)(e.Percentage * 100);
            ProgressBar.Value = Math.Max(1, Math.Min(100, value));
            ProgressBar.Value--; // fix animation
        }

        public void Search(SearchDirection direction)
        {
            StartActiveSearch(x => DoSearch(direction, x));
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
            NameBox.Text = Properties.Settings.Default.FindName;
            ValueBox.Text = Properties.Settings.Default.FindValue;
        }

        private void FindWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.FindRegex = RegexCheck.Checked;
            Properties.Settings.Default.FindName = NameBox.Text;
            Properties.Settings.Default.FindValue = ValueBox.Text;
        }

        private void RegexCheck_CheckedChanged(object sender, EventArgs e)
        {
            NameBox.RegexMode = RegexCheck.Checked;
            ValueBox.RegexMode = RegexCheck.Checked;
        }
    }
}