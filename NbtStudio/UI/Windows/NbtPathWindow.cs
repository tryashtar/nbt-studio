using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class NbtPathWindow : Form
    {
        private readonly NbtTreeView View;
        public NbtPathWindow(IconSource source, NbtTreeView view)
        {
            InitializeComponent();

            View = view;
            this.Icon = source.GetImage(IconType.Search).Icon;
            PathBox.SelectAll();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NbtPathWindow_Load(object sender, EventArgs e)
        {
            CenterToParent();
            PathBox.Text = Properties.Settings.Default.NbtPath;
        }

        private void NbtPathWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.NbtPath = PathBox.Text;
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            TraversePath();
        }

        private void TraversePath()
        {
            if (!PathBox.CheckPath(out var path))
                return;
            FoundResultsLabel.Visible = true;
            var node_path = new ModelNodePath(path);
            var selected_nodes = View.SelectedModelNodes.ToArray();
            if (!selected_nodes.Any())
            {
                FoundResultsLabel.Text = "Select a tag to begin";
                return;
            }
            var selected = selected_nodes.SelectMany(node_path.Traverse).ToList();
            if (selected.Count == 0)
            {
                FoundResultsLabel.Text = "No tags match that path";
                return;
            }
            View.ClearSelection();
            for (int i = 0; i < selected.Count; i++)
            {
                var node = View.FindNode(selected[i].Path, true);
                if (node is not null)
                {
                    FindWindow.FastEnsureVisible(node);
                    node.IsSelected = true;
                    if (i == 0)
                        View.ScrollTo(node);
                }
            }
            FoundResultsLabel.Text = $"Found {StringUtils.Pluralize(selected.Count(), "matching tag")}";
        }

        private class ModelNodePath
        {
            private readonly ModelNodePathNode[] Nodes;
            public ModelNodePath(NbtPath path)
            {
                Nodes = path.Nodes.Select(x => new ModelNodePathNode(x)).ToArray();
            }

            public IEnumerable<Node> Traverse(Node start)
            {
                IEnumerable<Node> list = new List<Node> { start };
                foreach (var node in Nodes)
                {
                    list = list.SelectMany(node.Get);
                }
                return list;
            }
        }

        private class ModelNodePathNode
        {
            private readonly NbtPathNode BaseNode;
            public ModelNodePathNode(NbtPathNode node)
            {
                BaseNode = node;
            }

            public IEnumerable<Node> Get(Node start)
            {
                var tag = start.GetNbtTag();
                if (tag is null)
                    return Array.Empty<Node>();
                var results = BaseNode.Get(tag);
                var children = start.Children.Where(x => RepresentsResults(x, results));
                return children;
            }

            private bool RepresentsResults(Node node, IEnumerable<NbtTag> results)
            {
                var tag = node.GetNbtTag();
                if (tag is null)
                    return false;
                return results.Contains(tag);
            }
        }
    }
}