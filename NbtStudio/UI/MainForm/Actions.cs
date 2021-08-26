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
    public delegate T Getter<T>();
    public class MainFormActions
    {
        private readonly Getter<Studio> GetApp;
        private readonly Getter<MainForm> GetForm;
        private readonly Getter<NbtTreeView> GetView;
        private readonly Getter<IconSource> GetIcons;
        private Studio App => GetApp();
        private MainForm Form => GetForm();
        private NbtTreeView View => GetView();
        private IconSource Icons => GetIcons();

        public MainFormActions(Getter<Studio> app, Getter<MainForm> form, Getter<NbtTreeView> view, Getter<IconSource> icons)
        {
            GetApp = app;
            GetForm = form;
            GetView = view;
            GetIcons = icons;
        }

        private UnsavedWarningHandler ConfirmIfUnsaved(string message)
        {
            return () =>
            {
                if (!App.Tree.HasUnsavedChanges)
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
                window.ShowDialog(Form);
            };
        }

        private PathsErrorHandler ShowPathsError(string title)
        {
            return x =>
            {
                string message = $"{StringUtils.Pluralize(x.FailedPaths.Count(), "file")} failed to load:\n\n";
                message += String.Join(Environment.NewLine, x.FailedPaths.Select(x => Path.GetFileName(x)));

                var window = new ExceptionWindow(title, message, x.Failable);
                window.ShowDialog(Form);
            };
        }

        private PathsGetter DefaultBrowseFiles() => BrowseFiles("Select NBT files", NbtUtil.OpenFilter());
        private PathsGetter BrowseFiles(string title, string filter)
        {
            return () =>
            {
                using var dialog = new OpenFileDialog
                {
                    Title = title,
                    RestoreDirectory = true,
                    Multiselect = true,
                    Filter = filter
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var attempts = new LoadFileAttempts<IHavePath>();
                    attempts.AddMany(dialog.FileNames, NbtFolder.OpenFile);
                    return attempts;
                }
                else
                    return null;
            };
        }
        private PathsGetter DefaultBrowseFolder() => BrowseFolder("Select a folder that contains NBT files");
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

        private TagCreator AddTagWindow(NbtTagType type)
        {
            Func<NbtContainerTag, NbtTag> simple_method;
            if (NbtUtil.IsArrayType(type))
                simple_method = destination => EditHexWindow.CreateTag(GetIcons(), type, destination);
            else
                simple_method = destination => EditTagWindow.CreateTag(GetIcons(), type, destination);
            return destination => new Failable<NbtTag>(() => simple_method(destination), "Creating tag");
        }

        public NbtTag AddTag(NbtTagType type)
        {
            return new AddTagAction()
            {
                TagSource = AddTagWindow(type),
                ErrorHandler = AbstractAction.Throw(),
                SelectedNodes = GetView().SelectedModelNodes
            }.AddTag();
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
            return new OpenFileAction(new RegionFile())
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?")
            }.Open();
        }

        public IEnumerable<IHavePath> OpenFile()
        {
            return OpenOrImport(true, DefaultBrowseFiles());
        }

        public PathsGetter FromPaths(string[] paths)
        {
            return () =>
            {
                var attempts = new LoadFileAttempts<IHavePath>();
                attempts.AddMany(paths, NbtFolder.OpenFile);
                return attempts;
            };
        }

        public IEnumerable<IHavePath> OpenFiles(params string[] paths)
        {
            return OpenOrImport(true, FromPaths(paths));
        }

        public IEnumerable<IHavePath> ImportFiles(params string[] paths)
        {
            return OpenOrImport(false, FromPaths(paths));
        }

        public IEnumerable<IHavePath> OpenFolder()
        {
            return OpenOrImport(true, DefaultBrowseFolder(), "Open a new folder anyway?");
        }

        public IEnumerable<IHavePath> NewPaste()
        {
            if (Clipboard.ContainsFileDropList())
            {
                return OpenOrImport(true, FilesFromClipboard, "Create a new file anyway?");
            }
            else if (Clipboard.ContainsText())
            {
                return new OpenFileAction()
                {
                    TreeGetter = () => App.Tree,
                    UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?"),
                    FilesGetter = ParseSnbtFromClipboard,
                    ErrorHandler = ShowError("Clipboard error", "Failed to parse SNBT from clipboard.")
                }.Open();
            }
            else
                return null;
        }

        public IEnumerable<IHavePath> OpenOrImport(bool open, PathsGetter getter, string confirm_message = "Open a new file anyway?")
        {
            var action = new OpenPathsAction()
            {
                TreeGetter = () => App.Tree,
                ErrorHandler = ShowPathsError("Load failure")
            };
            action.PathsGetter = getter;
            if (open)
                action.UnsavedWarningCheck = ConfirmIfUnsaved(confirm_message);
            return action.Open();
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
            return new OpenFileAction(new RegionFile())
            {
                TreeGetter = () => App.Tree
            }.Import();
        }

        public IEnumerable<IHavePath> ImportFile()
        {
            return OpenOrImport(false, BrowseFiles("Select NBT files", NbtUtil.OpenFilter()));
        }

        public IEnumerable<IHavePath> ImportFolder()
        {
            return OpenOrImport(false, BrowseFolder("Select a folder that contains NBT files"));
        }

        public LoadFileAttempts<IHavePath> FilesFromClipboard()
        {
            var paths = Clipboard.GetFileDropList().Cast<string>().Select(Path.GetFullPath).Distinct();
            var attempts = new LoadFileAttempts<IHavePath>();
            attempts.AddMany(paths, NbtFolder.OpenFileOrFolder);
            return attempts;
        }

        public IFailable<NbtTag> ParseSnbt(string snbt)
        {
            var attempt1 = SnbtParser.TryParse(snbt, named: false);
            if (!attempt1.Failed)
                return attempt1;
            else
            {
                var attempt2 = SnbtParser.TryParse(snbt, named: true);
                if (!attempt2.Failed)
                    return attempt2;
                else
                    return FailableFactory.Aggregate(attempt1, attempt2);
            }
        }

        public IEnumerable<IFailable<IFile>> ParseSnbtFromClipboard()
        {
            var text = Clipboard.GetText();
            var tag = ParseSnbt(text);
            yield return tag.Then(x => (IFile)new NbtFile(x));
        }

        public void Edit()
        {
            Edit(View.SelectedModelNodes.ToArray());
        }

        public void Edit(params Node[] nodes)
        {
            EditOrRename(true, nodes);
        }

        public void Rename()
        {
            Rename(View.SelectedModelNodes.ToArray());
        }

        public void Rename(params Node[] nodes)
        {
            EditOrRename(false, nodes);
        }

        public void EditOrRename(bool edit, params Node[] nodes)
        {
            var purpose = edit ? EditPurpose.EditValue : EditPurpose.Rename;
            Action<IEnumerable<NbtTag>> bulk_editor = edit ? x => BulkEditWindow.BulkEdit(Icons, x) : x => BulkEditWindow.BulkRename(Icons, x);
            var action = new EditorAction()
            {
                Nodes = nodes
            };
            // hex editor
            action.AddEditor(new AdHocSingleEditor<NbtTagNode>(
                x => ByteProviders.HasProvider(x.Tag),
                x => EditHexWindow.ModifyTag(Icons, x.Tag, purpose)
            ));
            // tag editor
            action.AddEditor(new AdHocSingleEditor<NbtTagNode>(
                x => true,
                x => EditTagWindow.ModifyTag(Icons, x.Tag, purpose)
            ));
            // bulk tag editor
            action.AddEditor(new AdHocMultipleEditor<NbtTagNode>(
               x => true,
               x => bulk_editor(x.Select(x => x.Tag))
            ));
            // chunk editor
            action.AddEditor(new AdHocSingleEditor<ChunkNode>(
                x => true,
                x => EditChunkWindow.MoveChunk(Icons, x.Chunk)
            ));
            // TO DO: file renamer
            action.Edit();
        }
    }
}
