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
    public class MainFormEditors
    {
        private readonly Getter<Studio> GetApp;
        private readonly Getter<MainForm> GetForm;
        private readonly Getter<NbtTreeView> GetView;
        private readonly Getter<IconSource> GetIcons;
        private Studio App => GetApp();
        private MainForm Form => GetForm();
        private NbtTreeView View => GetView();
        private IconSource Icons => GetIcons();

        public MainFormEditors(Getter<Studio> app, Getter<MainForm> form, Getter<NbtTreeView> view, Getter<IconSource> icons)
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
                        MessageBoxButtons.YesNo
                        );
                else
                    result = MessageBox.Show(
                        $"Are you sure you want to delete this file?\n\n" +
                        $"It will be sent to the recycle bin. This cannot be undone.",
                        $"Really delete {Path.GetFileName(file.Path)}?",
                        MessageBoxButtons.YesNo
                        );
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
            return destination =>
            {
                Func<NbtContainerTag, NbtTag> simple_method;
                if (Control.ModifierKeys == Keys.Shift)
                {
                    simple_method = destination =>
                    {
                        var tag = NbtUtil.CreateTag(type);
                        tag.Name = NbtUtil.GetAutomaticName(tag, destination);
                        return tag;
                    };
                }
                else
                {
                    if (NbtUtil.IsArrayType(type))
                        simple_method = destination => EditHexWindow.CreateTag(GetIcons(), type, destination);
                    else
                        simple_method = destination => EditTagWindow.CreateTag(GetIcons(), type, destination);
                }
                return new Failable<NbtTag>(() => simple_method(destination), "Creating tag");
            };
        }

        public void Undo()
        {
            App.UndoHistory.Undo();
        }

        public void Redo()
        {
            App.UndoHistory.Redo();
        }

        public Editor AddTag(NbtTagType type)
        {
            return new AddTagEditor(type)
            {
                TagSource = AddTagWindow(type),
                ErrorHandler = ShowError("Add tag error", "Failed to add tag.")
            };
        }

        public ContextFreeEditor New()
        {
            return new OpenFileEditor(() => new NbtFile())
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?"),
                Mode = OpenMode.Open
            };
        }

        public ContextFreeEditor NewRegion()
        {
            return new OpenFileEditor(() => new RegionFile())
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?"),
                Mode = OpenMode.Open
            };
        }

        public ContextFreeEditor OpenFile()
        {
            return OpenOrImport(OpenMode.Open, DefaultBrowseFiles());
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

        public ContextFreeEditor OpenFiles(params string[] paths)
        {
            return OpenOrImport(OpenMode.Open, FromPaths(paths));
        }

        public ContextFreeEditor ImportFiles(params string[] paths)
        {
            return OpenOrImport(OpenMode.Import, FromPaths(paths));
        }

        public ContextFreeEditor OpenFolder()
        {
            return OpenOrImport(OpenMode.Open, DefaultBrowseFolder(), "Open a new folder anyway?");
        }

        public ContextFreeEditor OpenOrImportPaste(OpenMode mode)
        {
            var editor = new MultiContextFreeEditor();
            editor.Add(new ConditionedContextFreeEditor(
                base_editor: OpenOrImport(mode, FilesFromClipboard, "Create a new file anyway?"),
                condition: Clipboard.ContainsFileDropList
            ));
            var parse_editor = new OpenFileEditor()
            {
                TreeGetter = () => App.Tree,
                UnsavedWarningCheck = ConfirmIfUnsaved("Create a new file anyway?"),
                FilesGetter = ParseSnbtFromClipboard,
                ErrorHandler = ShowError("Clipboard error", "Failed to parse SNBT from clipboard."),
                Mode = mode
            };
            editor.Add(new ConditionedContextFreeEditor(
                base_editor: parse_editor,
                condition: Clipboard.ContainsText
            ));
            return editor;
        }

        public ContextFreeEditor NewPaste()
        {
            return OpenOrImportPaste(OpenMode.Open);
        }

        public ContextFreeEditor ImportPaste()
        {
            return OpenOrImportPaste(OpenMode.Import);
        }

        public ContextFreeEditor OpenOrImport(OpenMode mode, PathsGetter getter, string confirm_message = "Open a new file anyway?")
        {
            var editor = new OpenPathsEditor()
            {
                TreeGetter = () => App.Tree,
                ErrorHandler = ShowPathsError("Load failure"),
                Mode = mode
            };
            editor.PathsGetter = getter;
            if (mode == OpenMode.Open)
                editor.UnsavedWarningCheck = ConfirmIfUnsaved(confirm_message);
            return editor;
        }

        public ContextFreeEditor ImportNew()
        {
            return new OpenFileEditor(() => new NbtFile())
            {
                TreeGetter = () => App.Tree,
                Mode = OpenMode.Import
            };
        }

        public ContextFreeEditor ImportNewRegion()
        {
            return new OpenFileEditor(() => new RegionFile())
            {
                TreeGetter = () => App.Tree,
                Mode = OpenMode.Import
            };
        }

        public ContextFreeEditor ImportFile()
        {
            return OpenOrImport(OpenMode.Import, BrowseFiles("Select NBT files", NbtUtil.OpenFilter()));
        }

        public ContextFreeEditor ImportFolder()
        {
            return OpenOrImport(OpenMode.Import, BrowseFolder("Select a folder that contains NBT files"));
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

        public Editor Edit()
        {
            return EditOrRename(true);
        }

        public Editor Rename()
        {
            return EditOrRename(false);
        }

        public Editor EditOrRename(bool edit)
        {
            var purpose = edit ? EditPurpose.EditValue : EditPurpose.Rename;
            Func<IEnumerable<NbtTag>, ICommand> bulk_editor = edit ? x => BulkEditWindow.BulkEdit(Icons, x) : x => BulkEditWindow.BulkRename(Icons, x);
            var editor = new MultiEditor();
            // hex editor
            editor.Add(new AdHocSingleEditor<NbtTag>(
                x => x.GetNbtTag(),
                x => ByteProviders.HasProvider(x),
                x => EditHexWindow.ModifyTag(Icons, x, purpose)
            ));
            // tag editor
            editor.Add(new AdHocSingleEditor<NbtTag>(
                x => x.GetNbtTag(),
                x => true,
                x => EditTagWindow.ModifyTag(Icons, x, purpose)
            ));
            // bulk tag editor
            editor.Add(new AdHocEditor<NbtTag>(
               x => x.GetNbtTag(),
               x => true,
               x => bulk_editor(x)
            ));
            // chunk editor
            editor.Add(new AdHocSingleEditor<ChunkEntry>(
                x => (x as ChunkNode)?.Chunk,
                x => true,
                x => EditChunkWindow.MoveChunk(Icons, x)
            ));
            // TO DO: file renamer
            return editor;
        }

        public Editor Save()
        {
            return new AdHocEditor<IFile>(
                x => x.GetFile(),
                x => x.Any(),
                x =>
                {
                    foreach (var file in x)
                    {
                        if (file.CanSave)
                            file.Save();
                        else if (!SaveAs(file))
                            break;
                    }
                    return null;
                }
            );
        }

        public Editor SaveAs()
        {
            return new AdHocEditor<IFile>(
                x => x.GetFile(),
                x => x.Any(),
                x =>
                {
                    foreach (var file in x)
                    {
                        if (!SaveAs(file))
                            break;
                    }
                    return null;
                }
            );
        }

        private bool SaveAs(IExportable file)
        {
            string path = null;
            if (file is IHavePath has_path)
                path = has_path.Path;
            using var dialog = new SaveFileDialog
            {
                Title = path is null ? "Save NBT file" : $"Save {Path.GetFileName(path)} as...",
                RestoreDirectory = true,
                FileName = path,
                Filter = NbtUtil.SaveFilter(path, NbtUtil.GetFileType(file))
            };
            if (path is not null)
            {
                dialog.InitialDirectory = Path.GetDirectoryName(path);
                dialog.FileName = Path.GetFileName(path);
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (file is NbtFile nbtfile)
                {
                    var export = new ExportWindow(GetIcons(), nbtfile.ExportSettings, dialog.FileName);
                    if (export.ShowDialog() == DialogResult.OK)
                    {
                        nbtfile.SaveAs(dialog.FileName, export.GetSettings());
                        Properties.Settings.Default.RecentFiles.Add(dialog.FileName);
                        return true;
                    }
                }
                else
                {
                    file.SaveAs(dialog.FileName);
                    Properties.Settings.Default.RecentFiles.Add(dialog.FileName);
                    return true;
                }
            }
            return false;
        }
    }
}
