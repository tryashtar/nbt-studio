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

        private readonly string[] ClickedFiles;

        public MainForm(string[] args)
        {
            ClickedFiles = args;
            if (Properties.Settings.Default.RecentFiles is null)
                Properties.Settings.Default.RecentFiles = new StringCollection();
            if (Properties.Settings.Default.CustomIconSets is null)
                Properties.Settings.Default.CustomIconSets = new StringCollection();

            // stuff from the designer
            InitializeComponent();

            ViewModel = new NbtTreeModel();
            NbtTree.Font = new Font(NbtTree.Font.FontFamily, Properties.Settings.Default.TreeZoom);

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

        private void Discard(IEnumerable<Node> nodes)
        {
            var unsaved = nodes.Filter(x => x.Get<ISaveable>()).Where(x => x.HasUnsavedChanges);
            if (!unsaved.Any() || MessageBox.Show($"You currently have unsaved changes.\n\nAre you sure you would like to discard the changes to these files?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                ViewModel.RemoveMany(nodes);
        }

        private void RefreshItems(IEnumerable<IRefreshable> items)
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
                    string message = $"{StringUtils.Pluralize(errors.Count, "file")} failed to refresh:\n\n";
                    message += String.Join("\n", errors.Select(x => x.item).Where(x => x is not null).Select(x => Path.GetFileName(x.Path)));
                    var window = new ExceptionWindow("Refresh error", message, error);
                    window.ShowDialog(this);
                }
            }
        }

        private void Save()
        {
            foreach (var file in ViewModel.OpenedFiles)
            {
                if (!Save(file))
                    break;
            }
        }

        private void SaveAs()
        {
            foreach (var file in ViewModel.OpenedFiles)
            {
                if (!SaveAs(file))
                    break;
            }
        }

        private bool Save(ISaveable file)
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

        private void OpenInExplorer(IHavePath file)
        {
            var info = new ProcessStartInfo { FileName = "explorer", Arguments = $"/select, \"{file.Path}\"" };
            Process.Start(info);
        }

        private void Sort()
        {
            var obj = NbtTree.SelectedModelNode;
            if (obj is null || !obj.CanSort) return;
            UndoHistory.StartBatchOperation();
            obj.Sort();
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Sort {0}", obj), true);
        }

        private void RefreshAll()
        {
            RefreshItems(ViewModel.OpenedFiles);
        }

        private void Undo()
        {
            UndoHistory.Undo();
        }

        private void Redo()
        {
            UndoHistory.Redo();
        }

        private void ClearUndoHistory()
        {
            UndoHistory.Clear();
        }

        private void CopyLike(Func<Node, bool> check, Func<Node, DataObject> perform)
        {
            var objs = NbtTree.SelectedModelNodes.Where(check).ToList();
            if (objs.Any())
            {
                var data = objs.Select(perform).Aggregate((x, y) => Utils.Merge(x, y));
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
            var parent = NbtTree.SelectedModelNode;
            if (parent is null) return;
            Paste(parent);
        }

        private void Paste(Node node)
        {
            if (!node.CanPaste)
                return;
            IEnumerable<Node> results = Enumerable.Empty<Node>();
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

        private void Rename()
        {
            var items = NbtTree.SelectedModelNodes;
            if (ListUtils.ExactlyOne(items))
                Rename(items.Single());
            else
                BulkRename(items.Filter(x => x.GetNbtTag()));
        }

        private void Edit()
        {
            var items = NbtTree.SelectedModelNodes;
            if (ListUtils.ExactlyOne(items))
                Edit(items.Single());
            else
                BulkEdit(items.Filter(x => x.GetNbtTag()));
        }

        private void BulkRename(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            var changed = BulkEditWindow.BulkRename(IconSource, tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk rename {0}", changed), false);
        }

        private void BulkEdit(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            var changed = BulkEditWindow.BulkEdit(IconSource, tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk edit {0}", changed), false);
        }

        private void EditLike(Node node, Predicate<Node> check, Action<NbtTag> when_tag)
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

        private void Rename(Node node)
        {
            EditLike(node, x => x.CanRename, RenameTag);
        }

        private void Edit(Node node)
        {
            EditLike(node, x => x.CanEdit, EditTag);
        }

        private void RenameFile(IHavePath item)
        {
            if (item.Path is not null)
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
            var tag = NbtTree.SelectedModelNode?.GetNbtTag();
            if (tag is null) return;
            UndoHistory.StartBatchOperation();
            EditSnbtWindow.ModifyTag(IconSource, tag, EditPurpose.EditValue);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Edit {0} as SNBT", tag), false);
        }

        private void Delete()
        {
            var selected_nodes = NbtTree.SelectedModelNodes;
            var nexts = selected_nodes.Select(x => x.NextNode).Where(x => x is not null).ToList();
            var prevs = selected_nodes.Select(x => x.PreviousNode).Where(x => x is not null).ToList();
            var parents = selected_nodes.Select(x => x.Parent).Where(x => x is not null).ToList();

            var selected_objects = NbtTree.SelectedModelNodes.ToList();
            Delete(selected_objects);

            // Index == -1 checks whether this node has been removed from the tree
            if (selected_nodes.All(x => x.Index == -1))
            {
                var select_next = nexts.FirstOrDefault(x => x.Index != -1) ?? prevs.FirstOrDefault(x => x.Index != -1) ?? parents.FirstOrDefault(x => x.Index != -1);
                if (select_next is not null)
                    select_next.IsSelected = true;
            }
        }

        private FindWindow FindWindow;
        private void Find()
        {
            if (FindWindow is null || FindWindow.IsDisposed)
                FindWindow = new FindWindow(IconSource, ViewModel, NbtTree);
            if (!FindWindow.Visible)
                FindWindow.Show(this);
            FindWindow.Focus();
        }

        private AboutWindow AboutWindow;
        private void About()
        {
            if (AboutWindow is null || AboutWindow.IsDisposed)
                AboutWindow = new AboutWindow(IconSource);
            if (!AboutWindow.Visible)
                AboutWindow.Show(this);
            AboutWindow.Focus();
        }

        private IconSetWindow IconSetWindow;
        private void ChangeIcons()
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

        private void AddSnbt()
        {
            var parent = NbtTree.SelectedModelNode?.GetNbtTag() as NbtContainerTag;
            if (parent is null) return;
            var tag = EditSnbtWindow.CreateTag(IconSource, parent);
            if (tag is not null)
                tag.AddTo(parent);
        }

        private void AddChunk()
        {
            var parent = NbtTree.SelectedModelNode?.Get<RegionFile>();
            if (parent is null) return;
            var chunk = EditChunkWindow.CreateChunk(IconSource, parent, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (chunk is not null)
                chunk.AddTo(parent);
        }

        private void AddTag(NbtTagType type)
        {
            var parent = NbtTree.SelectedModelNode?.GetNbtTag() as NbtContainerTag;
            if (parent is null) return;
            AddTag(parent, type);
        }

        private void AddTag(NbtContainerTag container, NbtTagType type)
        {
            NbtTag tag;
            if (NbtUtil.IsArrayType(type))
                tag = EditHexWindow.CreateTag(IconSource, type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            else
                tag = EditTagWindow.CreateTag(IconSource, type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (tag is not null)
                container.Add(tag);
        }

        private void OpenRecentFile()
        {
            UpdateRecentFiles();
            var files = Properties.Settings.Default.RecentFiles;
            if (files.Count >= 1)
                OpenFiles(Properties.Settings.Default.RecentFiles.Cast<string>().Reverse().Take(1));
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) // only run on UI thread
                return;
            var obj = NbtTree.SelectedModelNode;
            var objs = NbtTree.SelectedModelNodes;
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
            var tag = NbtTree.NodeFromClick(e);
            if (!e.Node.CanExpand && tag.CanEdit)
                Edit(tag);
        }

        private void NbtTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(NbtTree.SelectedModelNodes.ToArray(), DragDropEffects.Move);
        }

        private void NbtTree_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = Control.ModifierKeys == Keys.Shift ? DragDropEffects.Copy : DragDropEffects.Move;
            else
            {
                var tags = NbtTree.NodesFromDrag(e);
                var drop = NbtTree.DropNode;
                if (tags.Any()
                    && NbtTree.DropNode is not null
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
                var tags = NbtTree.NodesFromDrag(e);
                var drop = NbtTree.DropNode;
                if (tags.Any())
                    MoveObjects(tags, drop, NbtTree.DropPosition.Position);
            }
        }

        private bool CanMoveObjects(IEnumerable<Node> nodes, Node target, NodePosition position)
        {
            var (destination, index) = ViewModel.GetInsertionLocation(target, position);
            if (destination is null) return false;
            return destination.CanReceiveDrop(nodes);
        }

        private void MoveObjects(IEnumerable<Node> nodes, Node target, NodePosition position)
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

        private void SetAllSelected(IEnumerable<TreeNodeAdv> nodes, bool selected)
        {
            foreach (var node in nodes)
            {
                node.IsSelected = selected;
            }
        }

        private ContextMenuStrip CreateContextMenu(TreeNodeAdvMouseEventArgs e)
        {
            var menu = new ContextMenuStrip();
            var obj = NbtTree.NodeFromClick(e);
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
            collection.AddRange(sources[^1].ToArray());
        }

        private void Discard_Click(object sender, EventArgs e)
        {
            var selected_roots = NbtTree.SelectedModelNodes.Where(x => x.Parent is ModelRootNode);
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
            var selected = NbtTree.SelectedModelNodes.Filter(x => x.Get<ISaveable>());
            foreach (var item in selected)
            {
                Save(item);
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedModelNodes.Filter(x => x.Get<IExportable>());
            foreach (var item in selected)
            {
                SaveAs(item);
            }
        }

        private void OpenInExplorer_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedModelNodes.Filter(x => x.Get<IHavePath>());
            foreach (var item in selected)
            {
                OpenInExplorer(item);
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedModelNodes.Filter(x => x.Get<IRefreshable>());
            RefreshItems(selected);
        }

        private void AddTag_Click(NbtTagType type)
        {
            var selected = NbtTree.SelectedModelNodes.Filter(x => x.GetNbtTag()).OfType<NbtContainerTag>();
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
