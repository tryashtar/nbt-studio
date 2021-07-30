using fNbt;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class MainForm
    {
        public ActionContext FormContext(string unsaved_message = null)
        {
            var context = new ActionContext
            {
                SelectedNodes = NbtTree.SelectedModelNodes,
                DeleteFileWarning = ShowDeleteWarning,
                TreeSetter = x => ViewModel = x,
                TreeGetter = () => ViewModel
            };
            if (unsaved_message != null)
                context.UnsavedWarningCheck = () => ConfirmIfUnsaved(unsaved_message);
            return context;
        }

        private bool ConfirmIfUnsaved(string message)
        {
            if (!ViewModel.HasAnyUnsavedChanges)
                return true;
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private bool ShowDeleteWarning(IEnumerable<IHavePath> files)
        {
            DialogResult result;
            if (ListUtils.ExactlyOne(files))
            {
                var file = files.Single();
                if (file.Path is null)
                    result = MessageBox.Show(
                        $"Are you sure you want to remove this file?",
                        $"Really delete this unsaved file?",
                        MessageBoxButtons.YesNo);
                else
                    result = MessageBox.Show(
                        $"Are you sure you want to delete this file?\n\n" +
                        $"It will be sent to the recycle bin. This cannot be undone.",
                        $"Really delete {Path.GetFileName(file.Path)}?",
                        MessageBoxButtons.YesNo);
            }
            else
            {
                var unsaved = files.Where(x => x.Path is null);
                var saved = files.Where(x => x.Path is not null);
                if (!saved.Any())
                    result = MessageBox.Show(
                        $"Are you sure you want to remove {StringUtils.Pluralize(files.Count(), "unsaved file")}?",
                        $"Really delete these files?",
                        MessageBoxButtons.YesNo);
                else
                    result = MessageBox.Show(
                        $"Are you sure you want to delete {StringUtils.Pluralize(files.Count(), "file")}?\n\n" +
                        $"{StringUtils.Pluralize(saved.Count(), "file")} will be send to the recycle bin. This cannot be undone.",
                        $"Really delete these files?",
                        MessageBoxButtons.YesNo);
            }
            return result == DialogResult.Yes;
        }

        private void ShowError(IFailable failure, string title, string message)
        {
            var window = new ExceptionWindow(title, message, failure);
            window.ShowDialog(this);
        }

        private IEnumerable<IFailable<IFile>> BrowseFiles(string title, string filter)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = title,
                RestoreDirectory = true,
                Multiselect = true,
                Filter = filter
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    return dialog.FileNames.Select(x => NbtFolder.OpenFile(x));
            }
            return null;
        }

        private NbtFolder BrowseFolder(string title)
        {
            using (var dialog = new CommonOpenFileDialog
            {
                Title = title,
                RestoreDirectory = true,
                Multiselect = false,
                IsFolderPicker = true
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    return new NbtFolder(dialog.FileName, true);
            }
            return null;
        }

        public IEnumerable<IHavePath> New()
        {
            var context = FormContext(unsaved_message: "Create a new file anyway?");
            context.FilesGetter = ActionContext.SingleFile(new NbtFile());
            return Actions.OpenFiles(context);
        }

        public IEnumerable<IHavePath> NewRegion()
        {
            var context = FormContext(unsaved_message: "Create a new file anyway?");
            context.FilesGetter = ActionContext.SingleFile(RegionFile.Empty());
            return Actions.OpenFiles(context);
        }

        public IEnumerable<IHavePath> OpenFile()
        {
            var context = FormContext(unsaved_message: "Open a new file anyway?");
            context.FilesGetter = () => BrowseFiles("Select NBT files", NbtUtil.OpenFilter());
            context.FileErrorHandler = x => ShowError(x, "Load failure", message);
            return Actions.OpenFiles(context);
        }

        public IEnumerable<IHavePath> OpenFolder()
        {
            var context = FormContext(unsaved_message: "Open a new folder anyway?");
            context.FilesGetter = ActionContext.SingleFile(BrowseFolder("Select a folder that contains NBT files"));
            return Actions.OpenFiles(context);
        }

        public IEnumerable<IFile> NewPaste()
        {
            var context = FormContext(unsaved_message: "Open a new file anyway?");
            if (Clipboard.ContainsFileDropList())
            {
                context.FilesGetter = FilesFromClipboard;
                string message = $"{StringUtils.Pluralize(bad.Count(), "file")} failed to load:\n\n";
                message += String.Join(Environment.NewLine, bad.Select(x => Path.GetFileName(x.path)));
                context.ErrorHandler = x => ShowError(x, "Load failure", message);
            }
            else if (Clipboard.ContainsText())
            {
                context.FilesGetter = SnbtFromClipboard;
                context.ErrorHandler = x => ShowError(x, "Clipboard error", "Failed to parse SNBT from clipboard.");
            }
            return Actions.OpenFiles(context);
        }

        public IEnumerable<IHavePath> ImportNew()
        {
            var context = FormContext();
            context.FilesGetter = ActionContext.SingleFile(new NbtFile());
            return Actions.ImportFiles(context);
        }

        public IEnumerable<IHavePath> ImportNewRegion()
        {
            var context = FormContext();
            context.FilesGetter = ActionContext.SingleFile(RegionFile.Empty());
            return Actions.ImportFiles(context);
        }

        public IEnumerable<IHavePath> ImportFile()
        {
            var context = FormContext();
            context.FilesGetter = () => BrowseFiles("Select NBT files", NbtUtil.OpenFilter());
            context.FileErrorHandler = x => ShowError(x, "Load failure", message);
            return Actions.ImportFiles(context);
        }

        public IEnumerable<IHavePath> ImportFolder()
        {
            var context = FormContext();
            context.FilesGetter = ActionContext.SingleFile(BrowseFolder("Select a folder that contains NBT files"));
            return Actions.ImportFiles(context);
        }

        public IEnumerable<IFailable<IHavePath>> FilesFromClipboard()
        {
            var paths = Clipboard.GetFileDropList().Cast<string>();
            return paths.Distinct().Select(x => NbtFolder.OpenFileOrFolder(Path.GetFullPath(x)));
        }

        public IFailable<NbtTag> SnbtFromClipboard()
        {
            var text = Clipboard.GetText();
            var attempt1 = SnbtParser.TryParse(text, named: false);
            if (!attempt1.Failed)
                return attempt1;
            else
            {
                var attempt2 = SnbtParser.TryParse(text, named: true);
                if (!attempt2.Failed)
                    return attempt2;
                else
                    return FailableFactory.Aggregate(attempt1, attempt2);
            }
        }
    }
}
