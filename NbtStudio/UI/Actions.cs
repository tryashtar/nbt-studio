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
        public bool New(ActionContext context)
        {
            if (!ConfirmIfUnsaved(context, "Create a new file anyway?"))
                return false;
            OpenFile(new NbtFile(), skip_confirm: true);
            return true;
        }

        private bool ConfirmIfUnsaved(ActionContext context, string message)
        {
            if (!ViewModel.HasAnyUnsavedChanges || context.BypassSaveWarning)
                return true;
            bool answer = MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            if (answer)
                context.BypassSaveWarning = true;
            return answer;
        }

        public bool NewRegion(ActionContext context)
        {
            if (!ConfirmIfUnsaved(context, "Create a new file anyway?"))
                return false;
            OpenFile(RegionFile.Empty(), skip_confirm: true);
            return true;
        }

        public void ImportNew(ActionContext context)
        {
            ViewModel.Import(new NbtFile());
        }

        public void ImportNewRegion(ActionContext context)
        {
            ViewModel.Import(RegionFile.Empty());
        }

        public void AddTag(ActionContext context)
        {
            foreach (var node in context.SelectedNbt())
            {
                context.AddingTag.AddTo(node);
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
