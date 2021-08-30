using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public static class SearchNodeOperations
    {
        public static Node SearchFrom(IList<Node> nodes, Node start, Predicate<Node> predicate, SearchDirection direction, bool wrap, IProgress<TreeSearchReport> progress, CancellationToken token)
        {
            var very_first = direction == SearchDirection.Forward ? nodes[0] : FinalNode(nodes[^1]);
            Func<Node, Node> next_function = direction == SearchDirection.Forward ? x => NextNode(nodes, x) : x => PreviousNode(nodes, x);
            start = start is null ? very_first : next_function(start);
            return SearchFromNext(nodes, start, predicate, next_function, new TreeSearchReport(), wrap ? very_first : null, progress, token);
        }

        private static int TotalNodes(IList<Node> nodes)
        {
            return nodes.Sum(x => x.DescendantsCount);
        }

        public static IEnumerable<Node> SearchAll(IList<Node> nodes, Predicate<Node> predicate, IProgress<TreeSearchReport> progress, CancellationToken token)
        {
            var report = new TreeSearchReport();
            report.TotalNodes = TotalNodes(nodes);
            var node = nodes[0];
            while (node is not null)
            {
                if (node is not null && predicate(node))
                    yield return node;
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = TotalNodes(nodes);
                    progress.Report(report);
                    token.ThrowIfCancellationRequested();
                }
                node = NextNode(nodes, node);
            }
        }

        private static Node SearchFromNext(IList<Node> nodes, Node node, Predicate<Node> predicate, Func<Node, Node> next, TreeSearchReport report, Node wrap_start, IProgress<TreeSearchReport> progress, CancellationToken token)
        {
            var start = node;
            report.TotalNodes = TotalNodes(nodes);
            while (node is not null && !predicate(node))
            {
                node = next(node);
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = TotalNodes(nodes);
                    progress.Report(report);
                    token.ThrowIfCancellationRequested();
                }
            }
            if (node is not null)
                return node;
            if (wrap_start is null)
                return null;

            // search again from new starting point, until reaching original starting point
            node = SearchFromNext(nodes, wrap_start, x => x == start || predicate(x), next, report, null, progress, token);
            return node == start ? null : node;
        }

        public static Node NextNode(IList<Node> roots, Node node)
        {
            var children = node.Children;
            if (children.Count > 0)
                return children[0];
            Node next = null;
            while (next is null && node is not null)
            {
                if (node.Parent is null)
                {
                    int index = roots.IndexOf(node);
                    if (index == -1 || index == roots.Count - 1)
                        return null;
                    return roots[index + 1];
                }
                next = Sibling(node, 1);
                if (next is null)
                    node = node.Parent;
            }
            return next;
        }

        public static Node PreviousNode(IList<Node> roots, Node node)
        {
            var prev = Sibling(node, -1);
            if (prev is null)
            {
                if (node.Parent != null)
                    return node.Parent;
                int index = roots.IndexOf(node);
                if (index == -1 || index == 0)
                    return null;
                return FinalNode(roots[index - 1]);
            }
            while (prev is not null)
            {
                var children = prev.Children;
                if (children.Count == 0)
                    return prev;
                prev = children[^1];
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
                current = current.Children[^1];
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
