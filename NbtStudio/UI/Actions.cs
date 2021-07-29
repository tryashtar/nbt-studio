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
    public class Actions
    {
        // returns false if the user cancels the action
        public bool OpenFiles(ActionContext context)
        {
            if (!context.UnsavedWarningCheck())
                return false;
            context.TreeSetter(new NbtTreeModel(context.FilesGetter()));
            return true;
        }

        public void ImportFiles(ActionContext context)
        {
            context.TreeGetter().Import(context.FilesGetter());
        }

        public void AddTag(ActionContext context)
        {
            var tag = context.TagSource();
            foreach (var node in context.SelectedNbt().OfType<NbtContainerTag>())
            {
                tag.AddTo(node);
            }
        }

        public void Delete(ActionContext context)
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
