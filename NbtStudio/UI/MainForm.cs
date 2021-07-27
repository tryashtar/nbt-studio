using fNbt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Specialized;
using System.Diagnostics;
using TryashtarUtils.Utility;
using TryashtarUtils.Nbt;
using TryashtarUtils.Forms;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace NbtStudio.UI
{
    public partial class MainForm : Form
    {
        private NbtTreeModel _ViewModel;
        private NbtTreeModel ViewModel
        {
            get => _ViewModel;
            set
            {
                if (_ViewModel is not null)
                {
                    _ViewModel.Changed -= ViewModel_Changed;
                }

                _ViewModel = value;
                NbtTree.Model = _ViewModel;

                _ViewModel.Changed += ViewModel_Changed;

                ViewModel_Changed(this, EventArgs.Empty);
            }
        }

        private UndoHistory UndoHistory => ViewModel.UndoHistory;
        private IconSource IconSource;
        private readonly ScriptEngine PythonEngine;


        private readonly Dictionary<NbtTagType, DualMenuItem> CreateTagButtons;
        private readonly string[] ClickedFiles;

        private readonly DualItemCollection ItemCollection;
        private readonly DualMenuItem ActionNew = new DualMenuItem("&New", "New File", IconType.NewFile, Keys.Control | Keys.N);
        private readonly DualMenuItem ActionNewClipboard = DualMenuItem.SingleMenuItem("New from &Clipboard", IconType.Paste, Keys.Control | Keys.Alt | Keys.V);
        private readonly DualMenuItem ActionNewRegion = DualMenuItem.SingleMenuItem("New &Region File", IconType.Region, Keys.Control | Keys.Alt | Keys.R);
        private readonly DualMenuItem ActionOpenFile = new DualMenuItem("&Open File", "Open File", IconType.OpenFile, Keys.Control | Keys.O);
        private readonly DualMenuItem ActionOpenFolder = new DualMenuItem("Open &Folder", "Open Folder", IconType.OpenFolder, Keys.Control | Keys.Shift | Keys.O);
        private readonly DualMenuItem ActionSave = new DualMenuItem("&Save", "Save", IconType.Save, Keys.Control | Keys.S);
        private readonly DualMenuItem ActionSaveAs = DualMenuItem.SingleMenuItem("Save &As", IconType.Save, Keys.Control | Keys.Shift | Keys.S);
        private readonly DualMenuItem DropDownRecent = DualMenuItem.SingleMenuItem("&Recent...", null, Keys.None);
        private readonly DualMenuItem DropDownImport = DualMenuItem.SingleMenuItem("&Import...", null, Keys.None);
        private readonly DualMenuItem ActionImportFile = DualMenuItem.SingleMenuItem("&File", IconType.OpenFile, Keys.Control | Keys.I);
        private readonly DualMenuItem ActionImportFolder = DualMenuItem.SingleMenuItem("F&older", IconType.OpenFolder, Keys.Control | Keys.Shift | Keys.I);
        private readonly DualMenuItem ActionImportNew = DualMenuItem.SingleMenuItem("&New File", IconType.NewFile, Keys.Control | Keys.Alt | Keys.N);
        private readonly DualMenuItem ActionImportNewRegion = DualMenuItem.SingleMenuItem("New &Region File", IconType.Region, Keys.None);
        private readonly DualMenuItem ActionImportClipboard = DualMenuItem.SingleMenuItem("From &Clipboard", IconType.Paste, Keys.Control | Keys.Alt | Keys.I);
        private readonly DualMenuItem ActionSort = DualMenuItem.SingleButton("Sort", IconType.Sort);
        private readonly DualMenuItem ActionRefresh = DualMenuItem.SingleButton("Refresh", IconType.Refresh);
        private readonly DualMenuItem ActionUndo = DualMenuItem.SingleMenuItem("&Undo", IconType.Undo, Keys.Control | Keys.Z);
        private readonly DualMenuItem ActionRedo = DualMenuItem.SingleMenuItem("&Redo", IconType.Redo, Keys.Control | Keys.Shift | Keys.Z);
        private readonly DualMenuItem ActionCut = new DualMenuItem("Cu&t", "Cut", IconType.Cut, Keys.Control | Keys.X);
        private readonly DualMenuItem ActionCopy = new DualMenuItem("&Copy", "Copy", IconType.Copy, Keys.Control | Keys.C);
        private readonly DualMenuItem ActionPaste = new DualMenuItem("&Paste", "Paste", IconType.Paste, Keys.Control | Keys.V);
        private readonly DualMenuItem ActionRename = new DualMenuItem("Re&name", "Rename", IconType.Rename, Keys.F2);
        private readonly DualMenuItem ActionEdit = new DualMenuItem("&Edit Value", "Edit", IconType.Edit, Keys.Control | Keys.E);
        private readonly DualMenuItem ActionEditSnbt = new DualMenuItem("Edit as &SNBT", "Edit as SNBT", IconType.EditSnbt, Keys.Control | Keys.Shift | Keys.E);
        private readonly DualMenuItem ActionDelete = new DualMenuItem("&Delete", "Delete", IconType.Delete, Keys.Delete);
        private readonly DualMenuItem DropDownUndoHistory = DualMenuItem.SingleMenuItem("Undo History...", IconType.Undo, Keys.None);
        private readonly DualMenuItem DropDownRedoHistory = DualMenuItem.SingleMenuItem("Redo History...", IconType.Redo, Keys.None);
        private readonly DualMenuItem ActionClearUndoHistory = DualMenuItem.SingleMenuItem("Clear Undo History", null, Keys.None);
        private readonly DualMenuItem ActionFind = new DualMenuItem("&Find", "Find", IconType.Search, Keys.Control | Keys.F);
        private readonly DualMenuItem ActionAbout = DualMenuItem.SingleMenuItem("&About", IconType.NbtStudio, Keys.Shift | Keys.F1);
        private readonly DualMenuItem ActionChangeIcons = DualMenuItem.SingleMenuItem("&Change Icons", IconType.Refresh, Keys.Control | Keys.I);
        private readonly DualMenuItem ActionAddSnbt = DualMenuItem.SingleButton("Add as SNBT", IconType.AddSnbt);
        private readonly DualMenuItem ActionAddChunk = DualMenuItem.SingleButton("Add Chunk", IconType.Chunk);
        private readonly DualMenuItem ActionUpdate = DualMenuItem.SingleMenuItem("&Update", null, Keys.None);
        private readonly DualMenuItem ActionCheckUpdates = DualMenuItem.SingleMenuItem("Check for &Updates", null, Keys.Control | Keys.U);
        private readonly DualMenuItem ActionImportScript = DualMenuItem.SingleMenuItem("&Import New Script", IconType.OpenFile, Keys.None);
        public MainForm(string[] args)
        {
            ClickedFiles = args;
            if (Properties.Settings.Default.RecentFiles is null)
                Properties.Settings.Default.RecentFiles = new StringCollection();
            if (Properties.Settings.Default.CustomIconSets is null)
                Properties.Settings.Default.CustomIconSets = new StringCollection();
            if (Properties.Settings.Default.CustomScripts is null)
                Properties.Settings.Default.CustomScripts = new StringCollection();

            // stuff from the designer
            InitializeComponent();

            // stuff excluded from the designer for cleaner/less duplicated code
            ActionNew.Click += (s, e) => New();
            ActionNewClipboard.Click += (s, e) => NewPaste();
            ActionNewRegion.Click += (s, e) => NewRegion();
            ActionOpenFile.Click += (s, e) => OpenFile();
            ActionOpenFolder.Click += (s, e) => OpenFolder();
            ActionImportFile.Click += (s, e) => ImportFile();
            ActionImportFolder.Click += (s, e) => ImportFolder();
            ActionImportNew.Click += (s, e) => ImportNew();
            ActionImportNewRegion.Click += (s, e) => ImportNewRegion();
            ActionImportClipboard.Click += (s, e) => ImportClipboard();
            ActionSave.Click += (s, e) => Save();
            ActionSaveAs.Click += (s, e) => SaveAs();
            ActionSort.Click += (s, e) => Sort();
            ActionRefresh.Click += (s, e) => RefreshAll();
            ActionUndo.Click += (s, e) => Undo();
            ActionRedo.Click += (s, e) => Redo();
            ActionClearUndoHistory.Click += (s, e) => ClearUndoHistory();
            ActionCut.Click += (s, e) => Cut();
            ActionCopy.Click += (s, e) => Copy();
            ActionPaste.Click += (s, e) => Paste();
            ActionRename.Click += (s, e) => Rename();
            ActionEdit.Click += (s, e) => Edit();
            ActionEditSnbt.Click += (s, e) => EditSnbt();
            ActionDelete.Click += (s, e) => Delete();
            ActionFind.Click += (s, e) => Find();
            ActionAbout.Click += (s, e) => About();
            ActionChangeIcons.Click += (s, e) => ChangeIcons();
            ActionAddSnbt.Click += (s, e) => AddSnbt();
            ActionAddChunk.Click += (s, e) => AddChunk();
            ActionUpdate.Click += (s, e) => ShowUpdate();
            ActionCheckUpdates.Click += (s, e) => CheckForUpdates();
            ActionImportScript.Click += (s, e) => ImportScript();

            ActionNew.AddTo(Tools, MenuFile);
            ActionNewRegion.AddToMenuItem(MenuFile);
            ActionNewClipboard.AddToMenuItem(MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            ActionOpenFile.AddTo(Tools, MenuFile);
            ActionOpenFolder.AddTo(Tools, MenuFile);
            DropDownImport.AddToMenuItem(MenuFile);
            ActionImportFile.AddToDual(DropDownImport);
            ActionImportFolder.AddToDual(DropDownImport);
            ActionImportNew.AddToDual(DropDownImport);
            ActionImportNewRegion.AddToDual(DropDownImport);
            ActionImportClipboard.AddToDual(DropDownImport);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            ActionSave.AddTo(Tools, MenuFile);
            ActionSaveAs.AddToMenuItem(MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            DropDownRecent.AddToMenuItem(MenuFile);
            ActionRefresh.AddToToolStrip(Tools);
            Tools.Items.Add(new ToolStripSeparator());
            ActionUndo.AddToMenuItem(MenuEdit);
            ActionRedo.AddToMenuItem(MenuEdit);
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            ActionCut.AddTo(Tools, MenuEdit);
            ActionCopy.AddTo(Tools, MenuEdit);
            ActionPaste.AddTo(Tools, MenuEdit);
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            Tools.Items.Add(new ToolStripSeparator());
            ActionRename.AddTo(Tools, MenuEdit);
            ActionEdit.AddTo(Tools, MenuEdit);
            ActionEditSnbt.AddTo(Tools, MenuEdit);
            ActionDelete.AddTo(Tools, MenuEdit);
            ActionSort.AddToToolStrip(Tools);
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            DropDownUndoHistory.AddToMenuItem(MenuEdit);
            DropDownRedoHistory.AddToMenuItem(MenuEdit);
            ActionClearUndoHistory.AddToMenuItem(MenuEdit);
            Tools.Items.Add(new ToolStripSeparator());
            ActionAddChunk.AddToToolStrip(Tools);
            ActionAbout.AddToMenuItem(MenuHelp);
            ActionChangeIcons.AddToMenuItem(MenuHelp);
            ActionUpdate.Visible = false;
            ActionUpdate.AddToMenuStrip(MenuStrip);
            MenuHelp.DropDownItems.Add(new ToolStripSeparator());
            ActionCheckUpdates.AddToMenuItem(MenuHelp);
            ActionImportScript.AddToMenuItem(MenuAutomate);

            CreateTagButtons = MakeCreateTagButtons();
            foreach (var item in CreateTagButtons.Values)
            {
                item.AddToToolStrip(Tools);
            }
            ActionAddSnbt.AddToToolStrip(Tools);

            Tools.Items.Add(new ToolStripSeparator());
            ActionFind.AddTo(Tools, MenuSearch);

            ViewModel = new NbtTreeModel();
            NbtTree.Font = new Font(NbtTree.Font.FontFamily, Properties.Settings.Default.TreeZoom);

            ItemCollection = new DualItemCollection(
                ActionNew, ActionNewClipboard, ActionNewRegion,
                ActionOpenFile, ActionOpenFolder, DropDownImport,
                ActionImportFile, ActionImportFolder, ActionImportClipboard,
                ActionImportNew, ActionImportNewRegion, ActionSave,
                ActionSaveAs, DropDownRecent, ActionSort, ActionRefresh,
                ActionUndo, ActionRedo, ActionCut,
                ActionCopy, ActionPaste, ActionRename,
                ActionEdit, ActionEditSnbt, ActionDelete,
                DropDownUndoHistory, DropDownRedoHistory, ActionClearUndoHistory,
                ActionFind, ActionAbout, ActionAddSnbt, ActionAddChunk,
                ActionChangeIcons, ActionUpdate, ActionCheckUpdates,
                ActionImportScript
            );
            ItemCollection.AddRange(CreateTagButtons.Values);

            foreach (var item in Properties.Settings.Default.CustomIconSets.Cast<string>().ToList())
            {
                var attempt = IconSetWindow.TryImportSource(item);
                if (attempt.Failed)
                    IconSetWindow.ShowImportFailed(item, attempt, this);
            }
            SetIconSource(IconSourceRegistry.FromID(Properties.Settings.Default.IconSet));

            UpdateChecker = new Task<AvailableUpdate>(() => Updater.CheckForUpdates());
            UpdateChecker.Start();
            UpdateChecker.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.RanToCompletion && x.Result is not null)
                {
                    ReadyUpdate = x.Result;
                    ActionUpdate.Visible = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            NbtTree.NodeAdded += NbtTree_NodeAdded;

            PythonEngine = Python.CreateEngine();
            ImportScripts(Properties.Settings.Default.CustomScripts.Cast<string>());
        }

        private void NbtTree_NodeAdded(object sender, TreeNodeAdv e)
        {
            if (NbtTree.INodeFromNode(e) is FolderNode folder)
                folder.Folder.FilesFailed += Folder_FilesFailed;
        }

        private Task<AvailableUpdate> UpdateChecker;
        private AvailableUpdate ReadyUpdate;
        private void CheckForUpdates()
        {
            if (UpdateChecker is not null && !UpdateChecker.IsCompleted)
                return;
            UpdateChecker = new Task<AvailableUpdate>(() => Updater.CheckForUpdates());
            UpdateChecker.Start();
            UpdateChecker.ContinueWith(x =>
            {
                if (x.Status == TaskStatus.Faulted)
                {
                    var window = new ExceptionWindow("Update check failed",
                        "Failed to check for updates.",
                        FailableFactory.Failure(x.Exception, "Check for updates"),
                        "Would you like to go to the update page?\n" +
                        "https://github.com/tryashtar/nbt-studio/releases",
                        ExceptionWindowButtons.OKCancel
                    );
                    window.ShowDialog(this);
                    if (window.DialogResult == DialogResult.OK)
                        IOUtils.OpenUrlInBrowser("https://github.com/tryashtar/nbt-studio/releases");
                }
                else if (x.Status == TaskStatus.RanToCompletion)
                {
                    if (x.Result is null)
                    {
                        if (MessageBox.Show("You already seem to have the latest update.\n" +
                            "Would you like to go to the update page?\n\n" +
                            "https://github.com/tryashtar/nbt-studio/releases",
                            "No update found", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            IOUtils.OpenUrlInBrowser("https://github.com/tryashtar/nbt-studio/releases");
                    }
                    else
                    {
                        ReadyUpdate = x.Result;
                        ActionUpdate.Visible = true;
                        ShowUpdate();
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SetIconSource(IconSource source)
        {
            IconSource = source;
            ItemCollection.SetIconSource(source);
            NbtTree.SetIconSource(source);
            NbtTree.Refresh();
            this.Icon = source.GetImage(IconType.NbtStudio).Icon;
            Properties.Settings.Default.IconSet = IconSourceRegistry.GetID(source);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NbtTree_SelectionChanged(this, EventArgs.Empty);
            ViewModel_Changed(this, EventArgs.Empty);
            if (ClickedFiles is not null && ClickedFiles.Any())
                OpenFiles(ClickedFiles);
        }

        public void New()
        {
            if (!ConfirmIfUnsaved("Create a new file anyway?"))
                return;
            OpenFile(new NbtFile(), skip_confirm: true);
        }

        public void ImportNew()
        {
            ViewModel.Import(new NbtFile());
        }

        public void NewRegion()
        {
            if (!ConfirmIfUnsaved("Create a new file anyway?"))
                return;
            OpenFile(RegionFile.EmptyRegion(), skip_confirm: true);
        }

        public void ImportNewRegion()
        {
            ViewModel.Import(RegionFile.EmptyRegion());
        }

        private void PasteLike(Action<IEnumerable<string>> when_paths, Action<NbtFile> when_file)
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                when_paths(files.Cast<string>());
            }
            else if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                var attempt1 = SnbtParser.TryParse(text, named: false);
                if (!attempt1.Failed)
                    PasteTagLike(attempt1.Result, when_file);
                else
                {
                    var attempt2 = SnbtParser.TryParse(text, named: true);
                    if (!attempt2.Failed)
                        PasteTagLike(attempt2.Result, when_file);
                    else
                    {
                        var error = FailableFactory.Aggregate(attempt1, attempt2);
                        var window = new ExceptionWindow("Clipboard error", "Failed to parse SNBT from clipboard.", error);
                        window.ShowDialog(this);
                    }
                }
            }
        }

        private void PasteTagLike(NbtTag tag, Action<NbtFile> when_file)
        {
            if (tag is NbtCompound compound)
                when_file(new NbtFile(compound));
            else
            {
                var root = new NbtCompound();
                tag.Name = NbtUtil.GetAutomaticName(tag, root);
                root.Add(tag);
                when_file(new NbtFile(root));
            }
        }

        public void NewPaste()
        {
            PasteLike(x => OpenFiles(x), x => OpenFile(x));
        }

        public void ImportClipboard()
        {
            PasteLike(x => ImportFiles(x), x => ImportFile(x));
        }

        private void BrowseFileLike(Action<string[]> then)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select NBT files",
                RestoreDirectory = true,
                Multiselect = true,
                Filter = NbtUtil.OpenFilter()
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    then(dialog.FileNames);
            }
        }

        private void BrowseFolderLike(Action<string> then)
        {
            using (var dialog = new CommonOpenFileDialog
            {
                Title = "Select a folder that contains NBT files",
                RestoreDirectory = true,
                Multiselect = false,
                IsFolderPicker = true
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    then(dialog.FileName);
            }
        }

        public void OpenFile()
        {
            if (!ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            BrowseFileLike(x => OpenFiles(x, skip_confirm: true));
        }

        public void OpenFile(ISaveable file, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            ViewModel = new NbtTreeModel(file);
        }

        public void ImportFile()
        {
            BrowseFileLike(x => ImportFiles(x));
        }

        public void ImportFile(ISaveable file)
        {
            ViewModel.Import(file);
        }

        public void OpenFolder()
        {
            if (!ConfirmIfUnsaved("Open a new folder anyway?"))
                return;
            BrowseFolderLike(x => OpenFolder(x, skip_confirm: true));
        }

        public void ImportFolder()
        {
            BrowseFolderLike(x => ImportFolder(x));
        }

        public void Discard(IEnumerable<INode> nodes)
        {
            var unsaved = nodes.Filter(x => x.Get<ISaveable>()).Where(x => x.HasUnsavedChanges);
            if (!unsaved.Any() || MessageBox.Show($"You currently have unsaved changes.\n\nAre you sure you would like to discard the changes to these files?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                ViewModel.RemoveMany(nodes);
        }

        public void RefreshItems(IEnumerable<IRefreshable> items)
        {
            items = items.Where(x => x.CanRefresh);
            var unsaved = items.OfType<ISaveable>().Where(x => x.HasUnsavedChanges);
            if (!unsaved.Any() || MessageBox.Show($"You currently have unsaved changes.\n\nAre you sure you would like to discard the changes to these files?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                UndoHistory.StartBatchOperation();
                var errors = new List<(IHavePath item, Exception exception)>();
                foreach (var item in items)
                {
                    try
                    {
                        item.Refresh();
                    }
                    catch (Exception ex)
                    {
                        errors.Add((item as IHavePath, ex));
                    }
                }
                UndoHistory.FinishBatchOperation(new DescriptionHolder("Refresh {0}", items.ToArray()), true);
                if (errors.Any())
                {
                    var error = FailableFactory.AggregateFailure(errors.Select(x => x.exception).ToArray());
                    string message = $"{StringUtils.Pluralize(errors.Count(), "file")} failed to refresh:\n\n";
                    message += String.Join("\n", errors.Select(x => x.item).Where(x => x is not null).Select(x => Path.GetFileName(x.Path)));
                    var window = new ExceptionWindow("Refresh error", message, error);
                    window.ShowDialog(this);
                }
            }
        }

        private void ImportScript()
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select Python script",
                RestoreDirectory = true,
                Multiselect = true,
                Filter = "Python files|*.py|All files|*"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    ImportScripts(dialog.FileNames);
            }
        }

        private void ImportScripts(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (!Properties.Settings.Default.CustomScripts.Contains(file))
                    Properties.Settings.Default.CustomScripts.Add(file);
                var action = DualMenuItem.SingleMenuItem(Path.GetFileNameWithoutExtension(file), IconType.NbtStudio, Keys.None);
                ItemCollection.Add(action);
                action.AddToMenuItem(MenuAutomate);
                action.Click += (s, e) => RunScript(file);
            }
        }

        private void RunScript(string path)
        {
            var io = PythonEngine.Runtime.IO;
            var script = PythonEngine.CreateScriptSourceFromFile(path);
            var errors = new MemoryStream();
            var results = new MemoryStream();
            io.SetErrorOutput(errors, Encoding.Default);
            io.SetOutput(results, Encoding.Default);
            var scope = PythonEngine.CreateScope();
            scope.SetVariable("tree", ViewModel);
            scope.SetVariable("form", this);
            script.Execute(scope);
#if DEBUG
            Debug.WriteLine($"Just ran script {path}");
            var reader = new StreamReader(results);
            Debug.WriteLine($"Results: {reader.ReadToEnd()}");
            reader = new StreamReader(errors);
            Debug.WriteLine($"Errors: {reader.ReadToEnd()}");
#endif
        }

        public void Save()
        {
            foreach (var file in ViewModel.OpenedFiles)
            {
                if (!Save(file))
                    break;
            }
        }

        public void SaveAs()
        {
            foreach (var file in ViewModel.OpenedFiles)
            {
                if (!SaveAs(file))
                    break;
            }
        }

        public bool Save(ISaveable file)
        {
            if (file.CanSave)
            {
                file.Save();
                NbtTree.Refresh();
                return true;
            }
            else if (file is IExportable exp)
                return SaveAs(exp);
            return false;
        }

        public bool SaveAs(IExportable file)
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
                    var export = new ExportWindow(IconSource, nbtfile.ExportSettings, dialog.FileName);
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

        public void OpenInExplorer(IHavePath file)
        {
            var info = new ProcessStartInfo { FileName = "explorer", Arguments = $"/select, \"{file.Path}\"" };
            Process.Start(info);
        }

        public void Sort()
        {
            var obj = NbtTree.SelectedINode;
            if (obj is null || !obj.CanSort) return;
            UndoHistory.StartBatchOperation();
            obj.Sort();
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Sort {0}", obj), true);
        }

        public void RefreshAll()
        {
            RefreshItems(ViewModel.OpenedFiles);
        }

        public void Undo()
        {
            UndoHistory.Undo();
        }

        public void Redo()
        {
            UndoHistory.Redo();
        }

        public void ClearUndoHistory()
        {
            UndoHistory.Clear();
        }

        private void CopyLike(Func<INode, bool> check, Func<INode, DataObject> perform)
        {
            var objs = NbtTree.SelectedINodes.Where(check).ToList();
            if (objs.Any())
            {
                var data = objs.Select(perform).Aggregate((x, y) => Utils.Merge(x, y));
                Clipboard.SetDataObject(data);
            }
        }

        public void Cut()
        {
            CopyLike(x => x.CanCut, x => x.Cut());
        }

        public void Copy()
        {
            CopyLike(x => x.CanCopy, x => x.Copy());
        }

        public void Paste()
        {
            var parent = NbtTree.SelectedINode;
            if (parent is null) return;
            Paste(parent);
        }

        public void Paste(INode node)
        {
            if (!node.CanPaste)
                return;
            IEnumerable<INode> results = Enumerable.Empty<INode>();
            UndoHistory.StartBatchOperation();
            try
            { results = node.Paste(Clipboard.GetDataObject()); }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException))
                {
                    var error = FailableFactory.Failure(ex, "Pasting");
                    var window = new ExceptionWindow("Error while pasting", "An error occurred while pasting:", error);
                    window.ShowDialog(this);
                }
            }
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Paste {0} into {1}", results, node), true);
        }

        public void Rename()
        {
            var items = NbtTree.SelectedINodes;
            if (ListUtils.ExactlyOne(items))
                Rename(items.Single());
            else
                BulkRename(items.Filter(x => x.GetNbtTag()));
        }

        public void Edit()
        {
            var items = NbtTree.SelectedINodes;
            if (ListUtils.ExactlyOne(items))
                Edit(items.Single());
            else
                BulkEdit(items.Filter(x => x.GetNbtTag()));
        }

        public void BulkRename(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            var changed = BulkEditWindow.BulkRename(IconSource, tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk rename {0}", changed), false);
        }

        public void BulkEdit(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            var changed = BulkEditWindow.BulkEdit(IconSource, tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk edit {0}", changed), false);
        }

        public void EditLike(INode node, Predicate<INode> check, Action<NbtTag> when_tag)
        {
            if (!check(node)) return;
            var chunk = node.Get<Chunk>();
            var path = node.Get<IHavePath>();
            var tag = node.GetNbtTag();
            // batch operation to combine the rename and value change into one undo
            UndoHistory.StartBatchOperation();
            if (path is not null)
                RenameFile(path);
            if (chunk is not null)
                EditChunk(chunk);
            else if (tag is not null)
                when_tag(tag);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Edit {0}", node), false);
        }

        public void Rename(INode node)
        {
            EditLike(node, x => x.CanRename, RenameTag);
        }

        public void Edit(INode node)
        {
            EditLike(node, x => x.CanEdit, EditTag);
        }

        public void RenameFile(IHavePath item)
        {
            if (item.Path is not null)
                RenameFileWindow.RenameFile(IconSource, item);
        }

        public void EditTag(NbtTag tag)
        {
            if (ByteProviders.HasProvider(tag))
                EditHexWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
            else
                EditTagWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
        }

        public void EditChunk(Chunk chunk)
        {
            EditChunkWindow.MoveChunk(IconSource, chunk);
        }

        public void RenameTag(NbtTag tag)
        {
            // likewise
            UndoHistory.StartBatchOperation();
            EditTagWindow.ModifyTag(IconSource, tag, EditPurpose.Rename);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Rename {0}", tag), false);
        }

        public void EditSnbt()
        {
            var tag = NbtTree.SelectedINode?.GetNbtTag();
            if (tag is null) return;
            UndoHistory.StartBatchOperation();
            EditSnbtWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Edit {0} as SNBT", tag), false);
        }

        public void Delete()
        {
            var selected_nodes = NbtTree.SelectedNodes;
            var nexts = selected_nodes.Select(x => x.NextNode).Where(x => x is not null).ToList();
            var prevs = selected_nodes.Select(x => x.PreviousNode).Where(x => x is not null).ToList();
            var parents = selected_nodes.Select(x => x.Parent).Where(x => x is not null).ToList();

            var selected_objects = NbtTree.SelectedINodes.ToList();
            Delete(selected_objects);

            // Index == -1 checks whether this node has been removed from the tree
            if (selected_nodes.All(x => x.Index == -1))
            {
                var select_next = nexts.FirstOrDefault(x => x.Index != -1) ?? prevs.FirstOrDefault(x => x.Index != -1) ?? parents.FirstOrDefault(x => x.Index != -1);
                if (select_next is not null)
                    select_next.IsSelected = true;
            }
        }

        public void Delete(IEnumerable<INode> nodes)
        {
            nodes = nodes.Where(x => x.CanDelete);
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

        private FindWindow FindWindow;
        public void Find()
        {
            if (FindWindow is null || FindWindow.IsDisposed)
                FindWindow = new FindWindow(IconSource, ViewModel, NbtTree);
            if (!FindWindow.Visible)
                FindWindow.Show(this);
            FindWindow.Focus();
        }

        private AboutWindow AboutWindow;
        public void About()
        {
            if (AboutWindow is null || AboutWindow.IsDisposed)
                AboutWindow = new AboutWindow(IconSource);
            if (!AboutWindow.Visible)
                AboutWindow.Show(this);
            AboutWindow.Focus();
        }

        private IconSetWindow IconSetWindow;
        public void ChangeIcons()
        {
            if (IconSetWindow is null || IconSetWindow.IsDisposed)
            {
                IconSetWindow = new IconSetWindow(IconSource);
                IconSetWindow.FormClosed += IconSetWindow_FormClosed;
            }
            if (!IconSetWindow.Visible)
                IconSetWindow.Show(this);
            IconSetWindow.Focus();
        }

        private UpdateWindow UpdateWindow;
        private void ShowUpdate()
        {
            if (ReadyUpdate is null)
                return;
            if (UpdateWindow is null || UpdateWindow.IsDisposed)
                UpdateWindow = new UpdateWindow(IconSource, ReadyUpdate);
            if (!UpdateWindow.Visible)
                UpdateWindow.Show(this);
            UpdateWindow.Focus();
        }

        private void IconSetWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IconSetWindow.SelectedSource is not null)
                SetIconSource(IconSetWindow.SelectedSource);
        }

        public void AddSnbt()
        {
            var parent = NbtTree.SelectedINode?.GetNbtTag() as NbtContainerTag;
            if (parent is null) return;
            var tag = EditSnbtWindow.CreateTag(IconSource, parent);
            if (tag is not null)
                tag.AddTo(parent);
        }

        public void AddChunk()
        {
            var parent = NbtTree.SelectedINode?.Get<RegionFile>();
            if (parent is null) return;
            var chunk = EditChunkWindow.CreateChunk(IconSource, parent, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (chunk is not null)
                chunk.AddTo(parent);
        }

        public void AddTag(NbtTagType type)
        {
            var parent = NbtTree.SelectedINode?.GetNbtTag() as NbtContainerTag;
            if (parent is null) return;
            AddTag(parent, type);
        }

        public void AddTag(NbtContainerTag container, NbtTagType type)
        {
            NbtTag tag;
            if (NbtUtil.IsArrayType(type))
                tag = EditHexWindow.CreateTag(IconSource, type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            else
                tag = EditTagWindow.CreateTag(IconSource, type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (tag is not null)
                container.Add(tag);
        }

        private Dictionary<NbtTagType, DualMenuItem> MakeCreateTagButtons()
        {
            var buttons = new Dictionary<NbtTagType, DualMenuItem>();
            foreach (var type in NbtUtil.NormalTagTypes())
            {
                var button = DualMenuItem.SingleButton(
                    hover: $"Add {NbtUtil.TagTypeName(type)} Tag",
                    icon: NbtUtil.TagIconType(type));
                button.Click += (s, e) => AddTag(type);
                buttons.Add(type, button);
            }
            return buttons;
        }

        private void OpenPathsLike(IEnumerable<string> paths, Action<IEnumerable<IHavePath>> then)
        {
            var files = paths.Distinct().Select(path => (path, item: NbtFolder.OpenFileOrFolder(Path.GetFullPath(path)))).ToList();
            var bad = files.Where(x => x.item.Failed);
            var good = files.Where(x => !x.item.Failed);
            if (bad.Any())
            {
                string message = $"{StringUtils.Pluralize(bad.Count(), "file")} failed to load:\n\n";
                message += String.Join("\n", bad.Select(x => Path.GetFileName(x.path)));
                var fail = FailableFactory.Aggregate(bad.Select(x => x.item).ToArray());
                var window = new ExceptionWindow("Load failure", message, fail);
                window.ShowDialog(this);
            }
            if (good.Any())
            {
                Properties.Settings.Default.RecentFiles.AddRange(good.Select(x => x.path).ToArray());
                var results = good.Select(x => x.item.Result);
                then(results);
            }
        }

        private void Folder_FilesFailed(object sender, IEnumerable<(string path, IFailable<IFile> file)> bad)
        {
            string message = $"{StringUtils.Pluralize(bad.Count(), "file")} failed to load:\n\n";
            message += String.Join("\n", bad.Select(x => Path.GetFileName(x.path)));
            var fail = FailableFactory.Aggregate(bad.Select(x => x.file).ToArray());
            var window = new ExceptionWindow("Load failure", message, fail);
            window.ShowDialog(this);
        }

        public void OpenFolder(string path, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new folder anyway?"))
                return;
            OpenPathsLike(new[] { path }, x => ViewModel = new NbtTreeModel(x));
        }

        public void ImportFolder(string path)
        {
            OpenPathsLike(new[] { path }, x => ViewModel.ImportMany(x));
        }

        public void OpenFiles(IEnumerable<string> paths, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            OpenPathsLike(paths, x => ViewModel = new NbtTreeModel(x));
        }

        public void ImportFiles(IEnumerable<string> paths)
        {
            OpenPathsLike(paths, x => ViewModel.ImportMany(x));
        }

        public void OpenRecentFile()
        {
            UpdateRecentFiles();
            var files = Properties.Settings.Default.RecentFiles;
            if (files.Count >= 1)
                OpenFiles(Properties.Settings.Default.RecentFiles.Cast<string>().Reverse().Take(1));
        }

        private bool ConfirmIfUnsaved(string message)
        {
            if (!ViewModel.HasAnyUnsavedChanges)
                return true;
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) // only run on UI thread
                return;
            var obj = NbtTree.SelectedINode;
            var objs = NbtTree.SelectedINodes;
            var nbt = obj.GetNbtTag();
            var container = nbt as NbtContainerTag;
            var region = obj.Get<RegionFile>();
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = container is not null && container.CanAdd(item.Key);
                item.Value.Visible = region is null;
            }
            ActionSort.Enabled = obj is not null && obj.CanSort;
            ActionCut.Enabled = obj is not null && objs.Any(x => x.CanCut);
            ActionCopy.Enabled = obj is not null && objs.Any(x => x.CanCopy);
            ActionPaste.Enabled = obj is not null && obj.CanPaste; // don't check for Clipboard.ContainsText() because listening for clipboard events (to re-enable) is ugly
            ActionDelete.Enabled = obj is not null && objs.Any(x => x.CanDelete);
            ActionRename.Enabled = obj is not null && (objs.Any(x => x.CanRename) || objs.Any(x => x.CanEdit));
            ActionEdit.Enabled = obj is not null && (objs.Any(x => x.CanRename) || objs.Any(x => x.CanEdit));
            ActionEditSnbt.Enabled = nbt is not null;
            ActionAddSnbt.Enabled = container is not null;

            ActionAddSnbt.Visible = region is null;
            ActionAddChunk.Visible = region is not null;
        }

        private void ViewModel_Changed(object sender, EventArgs e)
        {
            if (InvokeRequired) // only run on UI thread
                return;
            ActionSave.Enabled = ViewModel.HasAnyUnsavedChanges;
            ActionSaveAs.Enabled = ViewModel.OpenedFiles.Any();
            ActionRefresh.Enabled = ViewModel.OpenedFiles.Any();
            bool multiple_files = ViewModel.OpenedFiles.Skip(1).Any();
            var save_image = multiple_files ? IconType.SaveAll : IconType.Save;
            ActionSave.IconType = save_image;
            ActionSaveAs.IconType = save_image;
            ActionUndo.Enabled = UndoHistory.CanUndo;
            ActionRedo.Enabled = UndoHistory.CanRedo;
            NbtTree_SelectionChanged(sender, e);
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var tag = NbtTree.INodeFromClick(e);
            if (!e.Node.CanExpand && tag.CanEdit)
                Edit(tag);
        }

        private void NbtTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(NbtTree.SelectedNodes.ToArray(), DragDropEffects.Move);
        }

        private void NbtTree_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = Control.ModifierKeys == Keys.Shift ? DragDropEffects.Copy : DragDropEffects.Move;
            else
            {
                var tags = NbtTree.INodesFromDrag(e);
                var drop = NbtTree.DropINode;
                if (tags.Any()
                    && NbtTree.DropINode is not null
                    && CanMoveObjects(tags, drop, NbtTree.DropPosition.Position))
                    e.Effect = e.AllowedEffect;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        private void NbtTree_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (e.Effect == DragDropEffects.Move)
                    OpenFiles(files);
                else if (e.Effect == DragDropEffects.Copy)
                    ImportFiles(files);
            }
            else
            {
                var tags = NbtTree.INodesFromDrag(e);
                var drop = NbtTree.DropINode;
                if (tags.Any())
                    MoveObjects(tags, drop, NbtTree.DropPosition.Position);
            }
        }

        private bool CanMoveObjects(IEnumerable<INode> nodes, INode target, NodePosition position)
        {
            var (destination, index) = ViewModel.GetInsertionLocation(target, position);
            if (destination is null) return false;
            return destination.CanReceiveDrop(nodes);
        }

        private void MoveObjects(IEnumerable<INode> nodes, INode target, NodePosition position)
        {
            var (destination, index) = ViewModel.GetInsertionLocation(target, position);
            if (destination is null) return;
            UndoHistory.StartBatchOperation();
            destination.ReceiveDrop(nodes, index);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Move {0} into {1} at position {2}", nodes, destination, index), true);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            if (!ConfirmIfUnsaved("Exit anyway?"))
                e.Cancel = true;
#endif
        }

        public void SetAllSelected(IEnumerable<TreeNodeAdv> nodes, bool selected)
        {
            foreach (var node in nodes)
            {
                node.IsSelected = selected;
            }
        }

        private ContextMenuStrip CreateContextMenu(TreeNodeAdvMouseEventArgs e)
        {
            var menu = new ContextMenuStrip();
            var obj = NbtTree.INodeFromClick(e);
            var root_items = new List<ToolStripItem>();
            var node_items = new List<ToolStripItem>();
            var file_items = new List<ToolStripItem>();
            var nbt_items = new List<ToolStripItem>();
            if (obj.Parent is ModelRootNode)
                root_items.Add(new ToolStripMenuItem("&Discard", IconSource.GetImage(IconType.Delete).Image, Discard_Click));
            if (e.Node.CanExpand)
            {
                if (e.Node.IsExpanded)
                    node_items.Add(new ToolStripMenuItem("&Collapse", null, Collapse_Click));
                else
                    node_items.Add(new ToolStripMenuItem("&Expand All", null, ExpandAll_Click));
                var children = NbtTree.AllChildren(e.Node);
                if (children.All(x => x.IsSelected))
                    node_items.Add(new ToolStripMenuItem("Dese&lect all Children", null, DeselectChildren_Click));
                else
                    node_items.Add(new ToolStripMenuItem("Se&lect all Children", null, SelectChildren_Click));
            }
            var saveable = obj.Get<ISaveable>();
            if (saveable is not null && saveable.CanSave)
                file_items.Add(new ToolStripMenuItem("&Save File", IconSource.GetImage(IconType.Save).Image, Save_Click));
            if (obj.Get<IExportable>() is not null)
                file_items.Add(new ToolStripMenuItem("Save File &As", IconSource.GetImage(IconType.Save).Image, SaveAs_Click));
            var refresh = obj.Get<IRefreshable>();
            if (refresh is not null && refresh.CanRefresh)
                file_items.Add(new ToolStripMenuItem("&Refresh", IconSource.GetImage(IconType.Refresh).Image, Refresh_Click));
            var path = obj.Get<IHavePath>();
            if (path is not null && path.Path is not null)
                file_items.Add(new ToolStripMenuItem("&Open in Explorer", IconSource.GetImage(IconType.OpenFile).Image, OpenInExplorer_Click));
            var container = obj.GetNbtTag() as NbtContainerTag;
            if (container is not null)
            {
                var addable = NbtUtil.NormalTagTypes().Where(x => container.CanAdd(x));
                bool single = ListUtils.ExactlyOne(addable);
                var display = single ? (Func<NbtTagType, string>)(x => $"Add {NbtUtil.TagTypeName(x)} Tag") : (x => $"{NbtUtil.TagTypeName(x)} Tag");
                var items = addable.Select(x => new ToolStripMenuItem(display(x), NbtUtil.TagTypeImage(IconSource, x).Image, (s, ea) => AddTag_Click(x))).ToArray();
                if (single)
                    nbt_items.AddRange(items);
                else
                {
                    var add = new ToolStripMenuItem("Add...");
                    add.DropDownItems.AddRange(items);
                    nbt_items.Add(add);
                }
            }
            AddMenuSections(menu.Items, root_items, node_items, file_items, nbt_items);
            return menu;
        }

        private void AddMenuSections(ToolStripItemCollection collection, params IEnumerable<ToolStripItem>[] sources)
        {
            for (int i = 0; i < sources.Length - 1; i++)
            {
                collection.AddRange(sources[i].ToArray());
                if (sources[i].Any())
                    collection.Add(new ToolStripSeparator());
            }
            collection.AddRange(sources[sources.Length - 1].ToArray());
        }

        private void Discard_Click(object sender, EventArgs e)
        {
            var selected_roots = NbtTree.SelectedINodes.Where(x => x.Parent is ModelRootNode);
            Discard(selected_roots);
        }

        private void Collapse_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes;
            foreach (var node in selected)
            {
                node.CollapseAll();
            }
        }

        private void ExpandAll_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes;
            foreach (var node in selected)
            {
                node.ExpandAll();
            }
        }

        private void SelectChildren_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes.ToList();
            foreach (var node in selected)
            {
                SetAllSelected(NbtTree.AllChildren(node), true);
            }
        }

        private void DeselectChildren_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes.ToList();
            foreach (var node in selected)
            {
                SetAllSelected(NbtTree.AllChildren(node), false);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedINodes.Filter(x => x.Get<ISaveable>());
            foreach (var item in selected)
            {
                Save(item);
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedINodes.Filter(x => x.Get<IExportable>());
            foreach (var item in selected)
            {
                SaveAs(item);
            }
        }

        private void OpenInExplorer_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedINodes.Filter(x => x.Get<IHavePath>());
            foreach (var item in selected)
            {
                OpenInExplorer(item);
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedINodes.Filter(x => x.Get<IRefreshable>());
            RefreshItems(selected);
        }

        private void AddTag_Click(NbtTagType type)
        {
            var selected = NbtTree.SelectedINodes.Filter(x => x.GetNbtTag()).OfType<NbtContainerTag>();
            foreach (var item in selected)
            {
                AddTag(item, type);
            }
        }

        private void NbtTree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = CreateContextMenu(e);
                menu.Show(NbtTree, e.Location);
                e.Handled = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.TreeZoom = (int)NbtTree.Font.Size;
            var icon_sets = Properties.Settings.Default.CustomIconSets.Cast<string>().Distinct().ToArray();
            Properties.Settings.Default.CustomIconSets.Clear();
            Properties.Settings.Default.CustomIconSets.AddRange(icon_sets);
            Properties.Settings.Default.Save();
        }

        private void MenuEdit_DropDownOpening(object sender, EventArgs e)
        {
            var undo_history = UndoHistory.GetUndoHistory();
            var redo_history = UndoHistory.GetRedoHistory();

            var undo_dropdown = new ToolStripDropDown();
            DropDownUndoHistory.DropDown = undo_dropdown;
            var undo_actions = new ActionHistory(undo_history,
                x => { UndoHistory.Undo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Undo {StringUtils.Pluralize(x + 1, "action")}",
                DropDownUndoHistory.Font);
            undo_dropdown.Items.Add(new ToolStripControlHost(undo_actions));

            var redo_dropdown = new ToolStripDropDown();
            DropDownRedoHistory.DropDown = redo_dropdown;
            var redo_actions = new ActionHistory(redo_history,
                x => { UndoHistory.Redo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Redo {StringUtils.Pluralize(x + 1, "action")}",
                DropDownRedoHistory.Font);
            redo_dropdown.Items.Add(new ToolStripControlHost(redo_actions));

            DropDownUndoHistory.Enabled = undo_history.Any();
            DropDownRedoHistory.Enabled = redo_history.Any();
            ActionClearUndoHistory.Enabled = undo_history.Any() || redo_history.Any();
        }

        private void UpdateRecentFiles()
        {
            // remove duplicates of recent files and limit to 20 most recent
            var distinct = Properties.Settings.Default.RecentFiles.Cast<string>().Reverse().Distinct();
            var recents = distinct.Take(20).ToList();

            DropDownRecent.Enabled = recents.Count > 0;
            DropDownRecent.DropDownItems.Clear();
            var items = new List<ToolStripMenuItem>();
            foreach (string path in recents.ToList())
            {
                var item = RecentEntry(path);
                if (item is null)
                    recents.Remove(path);
                else
                    items.Add(item);
            }
            DropDownRecent.DropDownItems.AddRange(items.ToArray());

            Properties.Settings.Default.RecentFiles.Clear();
            Properties.Settings.Default.RecentFiles.AddRange(recents.AsEnumerable().Reverse().ToArray());
        }

        private void MenuFile_DropDownOpening(object sender, EventArgs e)
        {
            ActionNewClipboard.Enabled = Clipboard.ContainsFileDropList() || Clipboard.ContainsText();
            UpdateRecentFiles();
        }

        private ToolStripMenuItem RecentEntry(string path)
        {
            bool directory = Directory.Exists(path);
            Image image;
            EventHandler click;
            if (directory)
            {
                image = IconSource.GetImage(IconType.Folder).Image;
                click = (s, e) => OpenFolder(path);
            }
            else
            {
                if (!File.Exists(path))
                    return null;
                image = IconSource.GetImage(IconType.File).Image;
                click = (s, e) => OpenFiles(new[] { path });
            }
            return new ToolStripMenuItem(path, image, click);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                Edit();
                return true;
            }
            if (keyData == (Keys.Control | Keys.Shift | Keys.T))
            {
                OpenRecentFile();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
