using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public static class SearchNodeOperations
    {
        public static Node SearchFrom(NbtTreeModel model, Node start, Predicate<Node> predicate, SearchDirection direction, IProgress<TreeSearchReport> progress, CancellationToken token, bool wrap)
        {
            if (direction == SearchDirection.Forward)
            {
                var first = model.Root.Children.First();
                if (start is null)
                    start = first;
                else
                    start = NextNode(start);
                return SearchFromNext(model, start, predicate, NextNode, progress, token, new TreeSearchReport(), wrap ? first : null);
            }
            else
            {
                var last = FinalNode(model.Root);
                if (start is null)
                    start = last;
                else
                    start = PreviousNode(start);
                return SearchFromNext(model, start, predicate, PreviousNode, progress, token, new TreeSearchReport(), wrap ? last : null);
            }
        }

        public static IEnumerable<Node> SearchAll(NbtTreeModel model, Predicate<Node> predicate, IProgress<TreeSearchReport> progress, CancellationToken token)
        {
            var report = new TreeSearchReport();
            report.TotalNodes = model.Root.DescendantsCount;
            var node = model.Root.Children[0];
            while (node is not null)
            {
                node = NextNode(node);
                if (node is not null && predicate(node))
                    yield return node;
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = model.Root.DescendantsCount;
                    progress.Report(report);
                    token.ThrowIfCancellationRequested();
                }
            }
        }

        private static Node SearchFromNext(NbtTreeModel model, Node node, Predicate<Node> predicate, Func<Node, Node> next, IProgress<TreeSearchReport> progress, CancellationToken token, TreeSearchReport report, Node wrap_start)
        {
            var start = node;
            report.TotalNodes = model.Root.DescendantsCount;
            while (node is not null && !predicate(node))
            {
                node = next(node);
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = model.Root.DescendantsCount;
                    progress.Report(report);
                    token.ThrowIfCancellationRequested();
                }
            }
            if (node is not null && node != model.Root)
                return node;
            if (wrap_start is null)
                return null;

            // search again from new starting point, until reaching original starting point
            node = SearchFromNext(model, wrap_start, x => x == start || predicate(x), next, progress, token, report, null);
            if (node == start)
                return null;
            else
                return node;
        }

        public static Node NextNode(Node node)
        {
            var children = node.Children;
            if (children.Count > 0)
                return children.First();
            Node next = null;
            while (next is null && node is not null)
            {
                if (node.Parent is null)
                    return null;
                next = Sibling(node, 1);
                if (next is null)
                    node = node.Parent;
            }
            return next;
        }

        public static Node PreviousNode(Node node)
        {
            var prev = Sibling(node, -1);
            if (prev is null)
                return node.Parent;
            while (prev is not null)
            {
                var children = prev.Children;
                if (children.Count == 0)
                    return prev;
                prev = children[children.Count - 1];
            }
            return null;
        }

        private static Node Sibling(Node node, int add)
        {
            if (node.Parent is null)
                return null;
            var children = node.Parent.Children;
            int i;
            for (i = 0; i < children.Count; i++)
            {
                if (children[i] == node)
                    break;
            }
            i += add;
            if (i < 0 || i >= children.Count)
                return null;
            return children[i];
        }

        public static Node FinalNode(Node root)
        {
            var current = root;
            while (true)
            {
                var children = current.Children;
                if (children.Count == 0)
                    return current;
                current = current.Children.Last();
            }
        }
    }

    public enum SearchDirection
    {
        Forward,
        Backward
    }

    public class TreeSearchReport
    {
        public int NodesSearched;
        public int TotalNodes;
        public decimal Percentage => (decimal)NodesSearched / TotalNodes;
    }
}
