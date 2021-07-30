using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public static class Actions
    {
        private static IEnumerable<IHavePath> OpenOrImport(ActionContext context, bool open)
        {
            if (!context.UnsavedWarningCheck())
                return null;
            IHavePath[] loadable;
            if (context.AdvancedFilesGetter != null)
            {
                var files = context.AdvancedFilesGetter();
                // can be null when the user cancels a file browse, for example
                if (files == null)
                    return null;
                var bad = files.Where(x => x.file.Failed).ToArray();
                loadable = files.Where(x => !x.file.Failed).Select(x => x.file.Result).ToArray();
                if (bad.Any())
                    context.AdvancedFileErrorHandler(bad);
            }
            else
            {
                var files = context.FilesGetter();
                // can be null when the user cancels a file browse, for example
                if (files == null)
                    return null;
                var bad = files.Where(x => x.Failed).ToArray();
                loadable = files.Where(x => !x.Failed).Select(x => x.Result).ToArray();
                if (bad.Any())
                    context.FileErrorHandler(FailableFactory.Aggregate(bad));
            }
            if (loadable.Any())
            {
                if (open)
                    context.TreeSetter(new NbtTreeModel(loadable));
                else
                    context.TreeGetter().Import(loadable);
                return loadable;
            }
            return null;
        }

        public static IEnumerable<IHavePath> OpenFiles(ActionContext context)
        {
            return OpenOrImport(context, true);
        }

        public static IEnumerable<IHavePath> ImportFiles(ActionContext context)
        {
            return OpenOrImport(context, false);
        }

        public static NbtTag AddTag(ActionContext context)
        {
            var tag = context.TagSource();
            if (tag.Failed)
            {
                context.TagErrorHandler(tag);
                return null;
            }
            bool has_original = true;
            foreach (var node in context.SelectedNbt().OfType<NbtContainerTag>())
            {
                var adding = has_original ? tag.Result : (NbtTag)tag.Result.Clone();
                adding.AddTo(node);
                has_original = false;
            }
            return tag.Result;
        }

        public static void Delete(ActionContext context)
        {
            var nodes = context.SelectedNodes.Where(x => x.CanDelete);
            var file_nodes = nodes.Where(x => x.Get<IHavePath>() is not null);
            var files = nodes.Filter(x => x.Get<IHavePath>());
            if (files.Any())
            {
                DialogResult result;
                if (ListUtils.ExactlyOne(files))
                {
                    var file = files.Single();
                    if (file.Path is null)
                        result = MessageBox.Show(
                            $"Are you sure you want to remove this item?",
                            $"Really delete this unsaved file?",
                            MessageBoxButtons.YesNo);
                    else
                        result = MessageBox.Show(
                            $"Are you sure you want to delete this item?\n\n" +
                            $"It will be sent to the recycle bin. This cannot be undone.",
                            $"Really delete {file_nodes.Single().Description}?",
                            MessageBoxButtons.YesNo);
                }
                else
                {
                    var unsaved = files.Where(x => x.Path is null);
                    var saved = files.Where(x => x.Path is not null);
                    if (!saved.Any())
                        result = MessageBox.Show(
                            $"Are you sure you want to remove {ExtractNodeOperations.Description(file_nodes)}?",
                            $"Really delete these items?",
                            MessageBoxButtons.YesNo);
                    else
                        result = MessageBox.Show(
                            $"Are you sure you want to delete {ExtractNodeOperations.Description(file_nodes)}?\n\n" +
                            $"{StringUtils.Pluralize(saved.Count(), "item")} will be send to the recycle bin. This cannot be undone.",
                            $"Really delete these items?",
                            MessageBoxButtons.YesNo);
                }
                if (result != DialogResult.Yes)
                    return;
            }
            UndoHistory.StartBatchOperation();
            var errors = new List<Exception>();
            foreach (var node in nodes)
            {
                try
                { node.Delete(); }
                catch (Exception ex)
                { errors.Add(ex); }
            }
            var relevant = errors.Where(x => !(x is OperationCanceledException)).ToArray();
            if (relevant.Any())
            {
                var error = FailableFactory.AggregateFailure(relevant);
                var window = new ExceptionWindow("Error while deleting", "An error occurred while deleting:", error);
                window.ShowDialog(this);
            }
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Delete {0}", nodes), false);
        }
    }
}
