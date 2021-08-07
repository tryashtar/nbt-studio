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
        private UnsavedWarningHandler ConfirmIfUnsaved(string message)
        {
            return () =>
            {
                if (!App.Tree.HasAnyUnsavedChanges)
                    return true;
                return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            };
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

        private ErrorHandler ShowError(string title, string message)
        {
            return x =>
            {
                var window = new ExceptionWindow(title, message, x);
                window.ShowDialog(this);
            };
        }

        private PathsErrorHandler ShowPathsError(string title)
        {
            return x =>
            {
                string message = $"{StringUtils.Pluralize(x.FailedPaths.Count(), "file")} failed to load:\n\n";
                message += String.Join(Environment.NewLine, x.FailedPaths.Select(x => Path.GetFileName(x)));

                var window = new ExceptionWindow(title, message, x.Failable);
                window.ShowDialog(this);
            };
        }

        private PathsGetter BrowseFiles(string title, string filter)
        {
            return () =>
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
                    {
                        var attempts = new LoadFileAttempts<IHavePath>();
                        attempts.AddMany(dialog.FileNames, NbtFolder.OpenFile);
                    }
                }
                return null;
            };
        }

        private PathsGetter BrowseFolder(string title)
        {
            return () =>
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
                    {
                        var attempts = new LoadFileAttempts<IHavePath>();
                        attempts.AddAttempt(dialog.FileName, x => new NbtFolder(x, true));
                    }
                }
                return null;
            };
        }

        public IEnumerable<IHavePath> New()
        {
            return new OpenFileAction(new NbtFile())
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?")
            }.Open();
        }

        public IEnumerable<IHavePath> NewRegion()
        {
            return new OpenFileAction(RegionFile.Empty())
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?")
            }.Open();
        }

        public IEnumerable<IHavePath> OpenFile()
        {
            return new OpenPathsAction()
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Open a new file anyway?"),
                PathsGetter = BrowseFiles("Select NBT files", NbtUtil.OpenFilter()),
                ErrorHandler = ShowPathsError("Load failure")
            }.Open();
        }

        public IEnumerable<IHavePath> OpenFolder()
        {
            return new OpenPathsAction()
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Open a new folder anyway?"),
                PathsGetter = BrowseFolder("Select a folder that contains NBT files"),
                ErrorHandler = ShowPathsError("Load failure")
            }.Open();
        }

        public IEnumerable<IHavePath> NewPaste()
        {
            if (Clipboard.ContainsFileDropList())
            {
                return new OpenPathsAction()
                {
                    TreeGetter = () => App.Tree,
                    UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?"),
                    PathsGetter = FilesFromClipboard,
                    ErrorHandler = ShowPathsError("Load failure")
                }.Open();
            }
            else if (Clipboard.ContainsText())
            {
                return new OpenFileAction()
                {
                    TreeGetter = () => App.Tree,
                    UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?"),
                    FilesGetter = SnbtFromClipboard,
                    ErrorHandler = ShowError("Clipboard error", "Failed to parse SNBT from clipboard.")
                }.Open();
            }
            else
                return null;
        }

        public IEnumerable<IHavePath> ImportNew()
        {
            return new OpenFileAction(new NbtFile())
            {
                TreeGetter = () => App.Tree
            }.Import();
        }

        public IEnumerable<IHavePath> ImportNewRegion()
        {
            return new OpenFileAction(RegionFile.Empty())
            {
                TreeGetter = () => App.Tree
            }.Import();
        }

        public IEnumerable<IHavePath> ImportFile()
        {
            return new OpenPathsAction()
            {
                TreeGetter = () => App.Tree,
                PathsGetter = BrowseFiles("Select NBT files", NbtUtil.OpenFilter()),
                ErrorHandler = ShowPathsError("Load failure")
            }.Import();
        }

        public IEnumerable<IHavePath> ImportFolder()
        {
            return new OpenPathsAction()
            {
                TreeGetter = () => App.Tree,
                PathsGetter = BrowseFolder("Select a folder that contains NBT files"),
                ErrorHandler = ShowPathsError("Load failure")
            }.Import();
        }

        public LoadFileAttempts<IHavePath> FilesFromClipboard()
        {
            var paths = Clipboard.GetFileDropList().Cast<string>().Select(Path.GetFullPath).Distinct();
            var attempts = new LoadFileAttempts<IHavePath>();
            attempts.AddMany(paths, NbtFolder.OpenFileOrFolder);
            return attempts;
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
