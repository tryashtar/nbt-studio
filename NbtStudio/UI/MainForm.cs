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
using NbtStudio.SNBT;
using System.Collections.Specialized;
using System.Diagnostics;

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
                if (_ViewModel != null)
                    _ViewModel.Changed -= ViewModel_Changed;
                _ViewModel = value;
                NbtTree.Model = _ViewModel;
                _ViewModel.Changed += ViewModel_Changed;
                ViewModel_Changed(this, EventArgs.Empty);
            }
        }
        private UndoHistory UndoHistory => ViewModel.UndoHistory;
        private IconSource IconSource;

        private readonly Dictionary<NbtTagType, DualMenuItem> CreateTagButtons;
        private readonly string[] ClickedFiles;

        private readonly DualItemCollection ItemCollection;
        private readonly DualMenuItem ActionNew = new DualMenuItem("&New", "New File", x => x.NewFile, Keys.Control | Keys.N);
        private readonly DualMenuItem ActionNewClipboard = DualMenuItem.SingleMenuItem("New from &Clipboard", x => x.Paste, Keys.Control | Keys.Alt | Keys.V);
        private readonly DualMenuItem ActionNewRegion = DualMenuItem.SingleMenuItem("New &Region File", x => x.Region, Keys.Control | Keys.Alt | Keys.R);
        private readonly DualMenuItem ActionOpenFile = new DualMenuItem("&Open File", "Open File", x => x.OpenFile, Keys.Control | Keys.O);
        private readonly DualMenuItem ActionOpenFolder = new DualMenuItem("Open &Folder", "Open Folder", x => x.OpenFolder, Keys.Control | Keys.Shift | Keys.O);
        private readonly DualMenuItem ActionSave = new DualMenuItem("&Save", "Save", x => x.Save, Keys.Control | Keys.S);
        private readonly DualMenuItem ActionSaveAs = DualMenuItem.SingleMenuItem("Save &As", x => x.Save, Keys.Control | Keys.Shift | Keys.S);
        private readonly DualMenuItem DropDownRecent = DualMenuItem.SingleMenuItem("&Recent...", null, Keys.None);
        private readonly DualMenuItem ActionSort = DualMenuItem.SingleButton("Sort", x => x.Sort);
        private readonly DualMenuItem ActionUndo = DualMenuItem.SingleMenuItem("&Undo", x => x.Undo, Keys.Control | Keys.Z);
        private readonly DualMenuItem ActionRedo = DualMenuItem.SingleMenuItem("&Redo", x => x.Redo, Keys.Control | Keys.Shift | Keys.Z);
        private readonly DualMenuItem ActionCut = new DualMenuItem("Cu&t", "Cut", x => x.Cut, Keys.Control | Keys.X);
        private readonly DualMenuItem ActionCopy = new DualMenuItem("&Copy", "Copy", x => x.Copy, Keys.Control | Keys.C);
        private readonly DualMenuItem ActionPaste = new DualMenuItem("&Paste", "Paste", x => x.Paste, Keys.Control | Keys.V);
        private readonly DualMenuItem ActionRename = new DualMenuItem("Re&name", "Rename", x => x.Rename, Keys.F2);
        private readonly DualMenuItem ActionEdit = new DualMenuItem("&Edit Value", "Edit", x => x.Edit, Keys.Control | Keys.E);
        private readonly DualMenuItem ActionEditSnbt = new DualMenuItem("Edit as &SNBT", "Edit as SNBT", x => x.EditSnbt, Keys.Control | Keys.Shift | Keys.E);
        private readonly DualMenuItem ActionDelete = new DualMenuItem("&Delete", "Delete", x => x.Delete, Keys.Delete);
        private readonly DualMenuItem DropDownUndoHistory = DualMenuItem.SingleMenuItem("Undo History...", x => x.Undo, Keys.None);
        private readonly DualMenuItem DropDownRedoHistory = DualMenuItem.SingleMenuItem("Redo History...", x => x.Redo, Keys.None);
        private readonly DualMenuItem ActionFind = new DualMenuItem("&Find", "Find", x => x.Search, Keys.Control | Keys.F);
        private readonly DualMenuItem ActionAbout = DualMenuItem.SingleMenuItem("&About", x => x.NbtStudio, Keys.Shift | Keys.F1);
        private readonly DualMenuItem ActionChangeIcons = DualMenuItem.SingleMenuItem("&Change Icons", x => x.Refresh, Keys.Control | Keys.I);
        private readonly DualMenuItem ActionAddSnbt = DualMenuItem.SingleButton("Add as SNBT", x => x.AddSnbt);
        private readonly DualMenuItem ActionAddChunk = DualMenuItem.SingleButton("Add Chunk", x => x.Chunk);
        public MainForm(string[] args)
        {
            ClickedFiles = args;
            if (Properties.Settings.Default.RecentFiles == null)
                Properties.Settings.Default.RecentFiles = new StringCollection();

            // stuff from the designer
            InitializeComponent();

            // stuff excluded from the designer for cleaner/less duplicated code
            ActionNew.Click += (s, e) => New();
            ActionNewClipboard.Click += (s, e) => NewPaste();
            ActionNewRegion.Click += (s, e) => NewRegion();
            ActionOpenFile.Click += (s, e) => OpenFile();
            ActionOpenFolder.Click += (s, e) => OpenFolder();
            ActionSave.Click += (s, e) => Save();
            ActionSaveAs.Click += (s, e) => SaveAs();
            ActionSort.Click += (s, e) => Sort();
            ActionUndo.Click += (s, e) => Undo();
            ActionRedo.Click += (s, e) => Redo();
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

            ActionNew.AddTo(Tools, MenuFile);
            ActionOpenFile.AddTo(Tools, MenuFile);
            ActionOpenFolder.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            ActionSave.AddTo(Tools, MenuFile);
            ActionSaveAs.AddTo(MenuFile);
            ActionNewRegion.AddTo(MenuFile);
            ActionNewClipboard.AddTo(MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            DropDownRecent.AddTo(MenuFile);
            ActionSort.AddTo(Tools);
            Tools.Items.Add(new ToolStripSeparator());
            ActionUndo.AddTo(MenuEdit);
            ActionRedo.AddTo(MenuEdit);
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
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            DropDownUndoHistory.AddTo(MenuEdit);
            DropDownRedoHistory.AddTo(MenuEdit);
            Tools.Items.Add(new ToolStripSeparator());
            ActionAddChunk.AddTo(Tools);
            ActionAbout.AddTo(MenuHelp);
            ActionChangeIcons.AddTo(MenuHelp);

            CreateTagButtons = MakeCreateTagButtons();
            foreach (var item in CreateTagButtons.Values)
            {
                item.AddTo(Tools);
            }
            ActionAddSnbt.AddTo(Tools);

            Tools.Items.Add(new ToolStripSeparator());
            ActionFind.AddTo(Tools, MenuSearch);

            ViewModel = new NbtTreeModel();
            NbtTree.Font = new Font(NbtTree.Font.FontFamily, Properties.Settings.Default.TreeZoom);

            ItemCollection = new DualItemCollection(
                ActionNew, ActionNewClipboard, ActionNewRegion,
                ActionOpenFile, ActionOpenFolder, ActionSave,
                ActionSaveAs, DropDownRecent, ActionSort,
                ActionUndo, ActionRedo, ActionCut,
                ActionCopy, ActionPaste, ActionRename,
                ActionEdit, ActionEditSnbt, ActionDelete,
                DropDownUndoHistory, DropDownRedoHistory, ActionFind,
                ActionAbout, ActionAddSnbt, ActionAddChunk,
                ActionChangeIcons
            );
            ItemCollection.AddRange(CreateTagButtons.Values);

            SetIconSource(IconSourceRegistry.FromID(Properties.Settings.Default.IconSet));
        }

        private void SetIconSource(IconSource source)
        {
            IconSource = source;
            ItemCollection.SetIconSource(source);
            NbtTree.SetIconSource(source);
            NbtTree.Refresh();
            this.Icon = source.NbtStudio.Icon;
            Properties.Settings.Default.IconSet = IconSourceRegistry.GetID(source);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NbtTree_SelectionChanged(this, EventArgs.Empty);
            ViewModel_Changed(this, EventArgs.Empty);
            if (ClickedFiles != null && ClickedFiles.Any())
                OpenFiles(ClickedFiles);
        }

        private void New()
        {
            if (!ConfirmIfUnsaved("Create a new file anyway?"))
                return;
            OpenFile(new NbtFile(), skip_confirm: true);
        }

        private void NewRegion()
        {
            if (!ConfirmIfUnsaved("Create a new file anyway?"))
                return;
            OpenFile(RegionFile.EmptyRegion(), skip_confirm: true);
        }

        private void NewPaste()
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                OpenFiles(files.Cast<string>());
            }
            else if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                if (SnbtParser.TryParse(text, named: false, out var tag) || SnbtParser.TryParse(text, named: true, out tag))
                {
                    if (tag is NbtCompound compound)
                        OpenFile(new NbtFile(compound));
                    else
                    {
                        var root = new NbtCompound();
                        tag.Name = NbtUtil.GetAutomaticName(tag, root);
                        root.Add(tag);
                        OpenFile(new NbtFile(root));
                    }
                }
                else
                    MessageBox.Show("Failed to parse SNBT from clipboard.", "Clipboard Error");
            }
        }

        private void OpenFile()
        {
            if (!ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            using (var dialog = new OpenFileDialog
            {
                Title = "Select NBT files",
                RestoreDirectory = true,
                Multiselect = true,
                Filter = NbtUtil.OpenFilter()
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    OpenFiles(dialog.FileNames, skip_confirm: true);
            }
        }

        private void OpenFile(ISaveable file, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            ViewModel = new NbtTreeModel(file);
        }

        private void OpenFolder()
        {
            if (!ConfirmIfUnsaved("Open a new folder anyway?"))
                return;
            using (var dialog = new CommonOpenFileDialog
            {
                Title = "Select a folder that contains NBT files",
                RestoreDirectory = true,
                Multiselect = false,
                IsFolderPicker = true
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    OpenFolder(dialog.FileName, skip_confirm: true);
            }
        }

        private void Save()
        {
            foreach (var file in ViewModel.OpenedFiles)
            {
                Save(file);
            }
        }

        private void SaveAs()
        {
            foreach (var file in ViewModel.OpenedFiles)
            {
                SaveAs(file);
            }
        }

        private void Save(ISaveable file)
        {
            if (file.CanSave)
            {
                file.Save();
                NbtTree.Refresh();
            }
            else
                SaveAs(file);
        }

        private void SaveAs(ISaveable file)
        {
            using (var dialog = new SaveFileDialog
            {
                Title = file.Path == null ? "Save NBT file" : $"Save {Path.GetFileName(file.Path)} as...",
                RestoreDirectory = true,
                FileName = file.Path,
                Filter = NbtUtil.SaveFilter(Path.GetExtension(file.Path))
            })
            {
                if (file.Path != null)
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(file.Path);
                    dialog.FileName = Path.GetFileName(file.Path);
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
                        }
                    }
                    else
                    {
                        file.SaveAs(dialog.FileName);
                        Properties.Settings.Default.RecentFiles.Add(dialog.FileName);
                    }
                }
            }
        }

        private void OpenInExplorer(IHavePath file)
        {
            var info = new ProcessStartInfo { FileName = "explorer", Arguments = $"/select, \"{file.Path}\"" };
            Process.Start(info);
        }

        private void Sort()
        {
            var obj = NbtTree.SelectedINode;
            if (obj == null || !obj.CanSort) return;
            UndoHistory.StartBatchOperation();
            obj.Sort();
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Sort {0}", obj), true);
        }

        private void Undo()
        {
            UndoHistory.Undo();
        }

        private void Redo()
        {
            UndoHistory.Redo();
        }

        private void CopyLike(Func<INode, bool> check, Func<INode, DataObject> perform)
        {
            var objs = NbtTree.SelectedINodes.Where(check).ToList();
            if (objs.Any())
            {
                var data = objs.Select(perform).Aggregate((x, y) => Util.Merge(x, y));
                Clipboard.SetDataObject(data);
            }
        }

        private void Cut()
        {
            CopyLike(x => x.CanCut, x => x.Cut());
        }

        private void Copy()
        {
            CopyLike(x => x.CanCopy, x => x.Copy());
        }

        private void Paste()
        {
            var parent = NbtTree.SelectedINode;
            if (parent == null) return;
            Paste(parent);
        }

        private void Paste(INode node)
        {
            if (!node.CanPaste)
                return;
            IEnumerable<INode> results = Enumerable.Empty<INode>();
            UndoHistory.StartBatchOperation();
            try
            { results = node.Paste(Clipboard.GetDataObject()); }
            catch (Exception ex)
            { ShowException("Error while pasting", ex); }
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Paste {0} into {1}", results, node), true);
        }

        private void Rename()
        {
            var items = NbtTree.SelectedINodes;
            if (Util.ExactlyOne(items))
                Rename(items.Single());
            else
                BulkRename(items.Filter(x => x.GetNbtTag()));
        }

        private void Edit()
        {
            var items = NbtTree.SelectedINodes;
            if (Util.ExactlyOne(items))
                Edit(items.Single());
            else
                BulkEdit(items.Filter(x => x.GetNbtTag()));
        }

        private void BulkRename(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            BulkEditWindow.BulkRename(IconSource, tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk rename {0}", tags), false);
        }

        private void BulkEdit(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            BulkEditWindow.BulkEdit(IconSource, tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk edit {0}", tags), false);
        }

        private void EditLike(INode node, Predicate<INode> check, Action<NbtTag> when_tag)
        {
            if (!check(node)) return;
            var chunk = node.GetChunk();
            var path = node.GetHasPath();
            var tag = node.GetNbtTag();
            // batch operation to combine the rename and value change into one undo
            UndoHistory.StartBatchOperation();
            if (path != null)
                RenameFile(path);
            if (chunk != null)
                EditChunk(chunk);
            else if (tag != null)
                when_tag(tag);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Edit {0}", node), false);
        }

        private void Rename(INode node)
        {
            EditLike(node, x => x.CanRename, RenameTag);
        }

        private void Edit(INode node)
        {
            EditLike(node, x => x.CanEdit, EditTag);
        }

        private void RenameFile(IHavePath item)
        {
            if (item.Path != null)
                RenameFileWindow.RenameFile(IconSource, item);
        }

        private void EditTag(NbtTag tag)
        {
            if (ByteProviders.HasProvider(tag))
                EditHexWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
            else
                EditTagWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
        }

        private void EditChunk(Chunk chunk)
        {
            EditChunkWindow.MoveChunk(IconSource, chunk);
        }

        private void RenameTag(NbtTag tag)
        {
            // likewise
            UndoHistory.StartBatchOperation();
            EditTagWindow.ModifyTag(IconSource, tag, EditPurpose.Rename);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Rename {0}", tag), false);
        }

        private void EditSnbt()
        {
            var tag = NbtTree.SelectedINode?.GetNbtTag();
            if (tag == null) return;
            UndoHistory.StartBatchOperation();
            EditSnbtWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Edit {0} as SNBT", tag), false);
        }

        private void Delete()
        {
            var selected_nodes = NbtTree.SelectedNodes;
            var nexts = selected_nodes.Select(x => x.NextNode).Where(x => x != null).ToList();
            var prevs = selected_nodes.Select(x => x.PreviousNode).Where(x => x != null).ToList();
            var parents = selected_nodes.Select(x => x.Parent).Where(x => x != null).ToList();

            var selected_objects = NbtTree.SelectedINodes.ToList();
            Delete(selected_objects);

            // Index == -1 checks whether this node has been removed from the tree
            if (selected_nodes.All(x => x.Index == -1))
            {
                var select_next = nexts.FirstOrDefault(x => x.Index != -1) ?? prevs.FirstOrDefault(x => x.Index != -1) ?? parents.FirstOrDefault(x => x.Index != -1);
                if (select_next != null)
                    select_next.IsSelected = true;
            }
        }

        private void Delete(IEnumerable<INode> nodes)
        {
            nodes = nodes.Where(x => x.CanDelete);
            var file_nodes = nodes.Where(x => x.GetHasPath() != null);
            var files = nodes.Filter(x => x.GetHasPath());
            if (files.Any())
            {
                DialogResult result;
                if (Util.ExactlyOne(files))
                {
                    var file = files.Single();
                    if (file.Path == null)
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
                    var unsaved = files.Where(x => x.Path == null);
                    var saved = files.Where(x => x.Path != null);
                    if (!saved.Any())
                        result = MessageBox.Show(
                            $"Are you sure you want to remove {ExtractNodeOperations.Description(file_nodes)}?",
                            $"Really delete these items?",
                            MessageBoxButtons.YesNo);
                    else
                        result = MessageBox.Show(
                            $"Are you sure you want to delete {ExtractNodeOperations.Description(file_nodes)}?\n\n" +
                            $"{Util.Pluralize(saved.Count(), "item")} will be send to the recycle bin. This cannot be undone.",
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
            if (errors.Any())
                ShowException("Error while deleting", new AggregateException(errors));
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Delete {0}", nodes), false);
        }

        private FindWindow FindWindow;
        private void Find()
        {
            if (FindWindow == null || FindWindow.IsDisposed)
                FindWindow = new FindWindow(IconSource, NbtTree);
            if (!FindWindow.Visible)
                FindWindow.Show(this);
            FindWindow.Focus();
        }

        private AboutWindow AboutWindow;
        private void About()
        {
            if (AboutWindow == null || AboutWindow.IsDisposed)
                AboutWindow = new AboutWindow();
            if (!AboutWindow.Visible)
                AboutWindow.Show(this);
            AboutWindow.Focus();
        }

        private void ChangeIcons()
        {
            Properties.Settings.Default.IconSet++;
            if (Properties.Settings.Default.IconSet > IconSourceRegistry.MaxID)
                Properties.Settings.Default.IconSet = 0;
            SetIconSource(IconSourceRegistry.FromID(Properties.Settings.Default.IconSet));
        }

        private void AddSnbt()
        {
            var parent = NbtTree.SelectedINode?.GetNbtTag() as NbtContainerTag;
            if (parent == null) return;
            var tag = EditSnbtWindow.CreateTag(IconSource, parent);
            if (tag != null)
                tag.AddTo(parent);
        }

        private void AddChunk()
        {
            var parent = NbtTree.SelectedINode?.GetRegionFile();
            if (parent == null) return;
            var chunk = EditChunkWindow.CreateChunk(IconSource, parent, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (chunk != null)
                chunk.AddTo(parent);
        }

        private void AddTag(NbtTagType type)
        {
            var parent = NbtTree.SelectedINode?.GetNbtTag() as NbtContainerTag;
            if (parent == null) return;
            AddTag(parent, type);
        }

        private void AddTag(NbtContainerTag container, NbtTagType type)
        {
            NbtTag tag;
            if (NbtUtil.IsArrayType(type))
                tag = EditHexWindow.CreateTag(IconSource, type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            else
                tag = EditTagWindow.CreateTag(IconSource, type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (tag != null)
                container.Add(tag);
        }

        private Dictionary<NbtTagType, DualMenuItem> MakeCreateTagButtons()
        {
            var buttons = new Dictionary<NbtTagType, DualMenuItem>();
            foreach (var type in NbtUtil.NormalTagTypes())
            {
                var button = DualMenuItem.SingleButton(
                    hover: $"Add {NbtUtil.TagTypeName(type)} Tag",
                    image: x => NbtUtil.TagTypeImage(x, type));
                button.Click += (s, e) => AddTag(type);
                buttons.Add(type, button);
            }
            return buttons;
        }

        private void OpenFolder(string path, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new folder anyway?"))
                return;
            Properties.Settings.Default.RecentFiles.Add(path);
            ViewModel = new NbtTreeModel(new NbtFolder(path, true));
        }

        private void OpenFiles(IEnumerable<string> paths, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            var files = paths.Distinct().Select(path => (path, item: NbtFolder.OpenFileOrFolder(Path.GetFullPath(path)))).ToList();
            var bad = files.Where(x => x.item == null);
            var good = files.Where(x => x.item != null);
            if (bad.Any())
            {
                string message = $"{Util.Pluralize(bad.Count(), "file")} failed to load:\n\n";
                message += String.Join("\n", bad.Select(x => Path.GetFileName(x.path)));
                MessageBox.Show(message, "Load Failure");
            }
            if (good.Any())
            {
                Properties.Settings.Default.RecentFiles.AddRange(good.Select(x => x.path).ToArray());
                ViewModel = new NbtTreeModel(good.Select(x => x.item));
            }
        }

        private void OpenRecentFile()
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

        private void ShowException(string caption, Exception exception)
        {
            if (!(exception is OperationCanceledException))
                MessageBox.Show(Util.ExceptionMessage(exception), caption);
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) // only run on UI thread
                return;
            var obj = NbtTree.SelectedINode;
            var objs = NbtTree.SelectedINodes;
            var nbt = obj.GetNbtTag();
            var container = nbt as NbtContainerTag;
            var region = obj.GetRegionFile();
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = container != null && container.CanAdd(item.Key);
                item.Value.Visible = region == null;
            }
            ActionSort.Enabled = obj != null && obj.CanSort;
            ActionCut.Enabled = obj != null && objs.Any(x => x.CanCut);
            ActionCopy.Enabled = obj != null && objs.Any(x => x.CanCopy);
            ActionPaste.Enabled = obj != null && obj.CanPaste; // don't check for Clipboard.ContainsText() because listening for clipboard events (to re-enable) is ugly
            ActionDelete.Enabled = obj != null && objs.Any(x => x.CanDelete);
            ActionRename.Enabled = obj != null && (objs.Any(x => x.CanRename) || objs.Any(x => x.CanEdit));
            ActionEdit.Enabled = obj != null && (objs.Any(x => x.CanRename) || objs.Any(x => x.CanEdit));
            ActionEditSnbt.Enabled = nbt != null;
            ActionAddSnbt.Enabled = container != null;

            ActionAddSnbt.Visible = region == null;
            ActionAddChunk.Visible = region != null;
        }

        private void ViewModel_Changed(object sender, EventArgs e)
        {
            if (InvokeRequired) // only run on UI thread
                return;
            ActionSave.Enabled = ViewModel.HasAnyUnsavedChanges;
            ActionSaveAs.Enabled = ViewModel.OpenedFiles.Any();
            bool multiple_files = ViewModel.OpenedFiles.Skip(1).Any();
            var save_image = multiple_files ? (Func<IconSource, ImageIcon>)(x => x.Save) : (Func<IconSource, ImageIcon>)(x => x.SaveAll);
            ActionSave.ImageGetter = save_image;
            ActionSaveAs.ImageGetter = save_image;
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
                e.Effect = DragDropEffects.Copy;
            else
            {
                var tags = NbtTree.INodesFromDrag(e);
                var drop = NbtTree.DropINode;
                if (tags.Any()
                    && NbtTree.DropINode != null
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
                OpenFiles(files);
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
            if (destination == null) return false;
            return destination.CanReceiveDrop(nodes);
        }

        private void MoveObjects(IEnumerable<INode> nodes, INode target, NodePosition position)
        {
            var (destination, index) = ViewModel.GetInsertionLocation(target, position);
            if (destination == null) return;
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

        private void SetAllSelected(IEnumerable<TreeNodeAdv> nodes, bool selected)
        {
            foreach (var node in nodes)
            {
                node.IsSelected = selected;
            }
        }

        private void NbtTree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip();
                if (e.Node.CanExpand)
                {
                    if (e.Node.IsExpanded)
                        menu.Items.Add("&Collapse", null, (s, ea) => e.Node.Collapse());
                    else
                        menu.Items.Add("&Expand All", null, (s, ea) => e.Node.ExpandAll());
                    var children = NbtTree.AllChildren(e.Node);
                    if (children.All(x => x.IsSelected))
                        menu.Items.Add("R&emove Children from Selection", null, (s, ea) => SetAllSelected(children, false));
                    else
                        menu.Items.Add("A&dd Children to Selection", null, (s, ea) => SetAllSelected(children, true));
                }
                var obj = NbtTree.INodeFromClick(e);
                var saveable = obj.GetSaveable();
                if (saveable != null)
                {
                    if (menu.Items.Count > 0)
                        menu.Items.Add(new ToolStripSeparator());
                    menu.Items.Add("&Save File", IconSource.Save.Image, (s, ea) => Save(saveable));
                    menu.Items.Add("Save File &As", IconSource.Save.Image, (s, ea) => SaveAs(saveable));
                }
                var path = obj.GetHasPath();
                if (path?.Path != null)
                    menu.Items.Add("&Open in Explorer", IconSource.OpenFile.Image, (s, ea) => OpenInExplorer(path));
                var folder = obj.GetNbtFolder();
                if (folder != null)
                    menu.Items.Add("&Refresh", IconSource.Refresh.Image, (s, ea) => folder.Scan());
                var container = obj.GetNbtTag() as NbtContainerTag;
                if (container != null)
                {
                    if (menu.Items.Count > 0)
                        menu.Items.Add(new ToolStripSeparator());
                    var addable = NbtUtil.NormalTagTypes().Where(x => container.CanAdd(x));
                    bool single = Util.ExactlyOne(addable);
                    var display = single ? (Func<NbtTagType, string>)(x => $"Add {NbtUtil.TagTypeName(x)} Tag") : (x => $"{NbtUtil.TagTypeName(x)} Tag");
                    var items = addable.Select(x => new ToolStripMenuItem(display(x), NbtUtil.TagTypeImage(IconSource, x).Image, (s, ea) => AddTag(container, x))).ToArray();
                    if (single)
                        menu.Items.AddRange(items);
                    else
                    {
                        var add = new ToolStripMenuItem("Add...");
                        add.DropDownItems.AddRange(items);
                        menu.Items.Add(add);
                    }
                }
                menu.Show(NbtTree.PointToScreen(e.Location));
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.TreeZoom = (int)NbtTree.Font.Size;
            Properties.Settings.Default.Save();
        }

        private void MenuEdit_DropDownOpening(object sender, EventArgs e)
        {
            DropDownUndoHistory.Enabled = false;
            DropDownRedoHistory.Enabled = false;

            var undo_history = UndoHistory.GetUndoHistory();
            var redo_history = UndoHistory.GetRedoHistory();

            var undo_dropdown = new ToolStripDropDown();
            DropDownUndoHistory.DropDown = undo_dropdown;
            var undo_actions = new ActionHistory(undo_history,
                x => { UndoHistory.Undo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Undo {Util.Pluralize(x + 1, "action")}",
                DropDownUndoHistory.Font);
            undo_dropdown.Items.Add(new ToolStripControlHost(undo_actions));

            var redo_dropdown = new ToolStripDropDown();
            DropDownRedoHistory.DropDown = redo_dropdown;
            var redo_actions = new ActionHistory(redo_history,
                x => { UndoHistory.Redo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Redo {Util.Pluralize(x + 1, "action")}",
                DropDownRedoHistory.Font);
            redo_dropdown.Items.Add(new ToolStripControlHost(redo_actions));

            DropDownUndoHistory.Enabled = undo_history.Any();
            DropDownRedoHistory.Enabled = redo_history.Any();
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
                if (item == null)
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
                image = IconSource.Folder.Image;
                click = (s, e) => OpenFolder(path);
            }
            else
            {
                if (!File.Exists(path))
                    return null;
                image = IconSource.File.Image;
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
