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
                _ViewModel.Changed += ViewModel_Changed;
                ViewModel_Changed(this, EventArgs.Empty);
            }
        }
        private UndoHistory UndoHistory => ViewModel.UndoHistory;

        private readonly Dictionary<NbtTagType, ToolStripButton> CreateTagButtons;
        private readonly string[] ClickedFiles;

        private readonly DualMenuItem ActionNew = new DualMenuItem("&New", "New File", Properties.Resources.action_new_image, Keys.Control | Keys.N);
        private readonly ToolStripMenuItem ActionNewClipboard = DualMenuItem.Single("New from &Clipboard", Properties.Resources.action_paste_image, Keys.Control | Keys.Alt | Keys.V);
        private readonly ToolStripMenuItem ActionNewRegion = DualMenuItem.Single("New &Region File", Properties.Resources.region_image, Keys.Control | Keys.Alt | Keys.R);
        private readonly DualMenuItem ActionOpenFile = new DualMenuItem("&Open File", "Open File", Properties.Resources.action_open_file_image, Keys.Control | Keys.O);
        private readonly DualMenuItem ActionOpenFolder = new DualMenuItem("Open &Folder", "Open Folder", Properties.Resources.action_open_folder_image, Keys.Control | Keys.Shift | Keys.O);
        private readonly DualMenuItem ActionSave = new DualMenuItem("&Save", "Save", Properties.Resources.action_save_image, Keys.Control | Keys.S);
        private readonly ToolStripMenuItem ActionSaveAs = DualMenuItem.Single("Save &As", Properties.Resources.action_save_image, Keys.Control | Keys.Shift | Keys.S);
        private readonly ToolStripMenuItem DropDownRecent = DualMenuItem.Single("&Recent...", null, Keys.None);
        private readonly ToolStripButton ActionSort = DualMenuItem.Single("Sort", Properties.Resources.action_sort_image);
        private readonly ToolStripMenuItem ActionUndo = DualMenuItem.Single("&Undo", Properties.Resources.action_undo_image, Keys.Control | Keys.Z);
        private readonly ToolStripMenuItem ActionRedo = DualMenuItem.Single("&Redo", Properties.Resources.action_redo_image, Keys.Control | Keys.Shift | Keys.Z);
        private readonly DualMenuItem ActionCut = new DualMenuItem("Cu&t", "Cut", Properties.Resources.action_cut_image, Keys.Control | Keys.X);
        private readonly DualMenuItem ActionCopy = new DualMenuItem("&Copy", "Copy", Properties.Resources.action_copy_image, Keys.Control | Keys.C);
        private readonly DualMenuItem ActionPaste = new DualMenuItem("&Paste", "Paste", Properties.Resources.action_paste_image, Keys.Control | Keys.V);
        private readonly DualMenuItem ActionRename = new DualMenuItem("Re&name", "Rename", Properties.Resources.action_rename_image, Keys.F2);
        private readonly DualMenuItem ActionEdit = new DualMenuItem("&Edit Value", "Edit", Properties.Resources.action_edit_image, Keys.Control | Keys.E);
        private readonly DualMenuItem ActionEditSnbt = new DualMenuItem("Edit as &SNBT", "Edit as SNBT", Properties.Resources.action_edit_snbt_image, Keys.Control | Keys.Shift | Keys.E);
        private readonly DualMenuItem ActionDelete = new DualMenuItem("&Delete", "Delete", Properties.Resources.action_delete_image, Keys.Delete);
        private readonly ToolStripMenuItem DropDownUndoHistory = DualMenuItem.Single("Undo History...", Properties.Resources.action_undo_image, Keys.None);
        private readonly ToolStripMenuItem DropDownRedoHistory = DualMenuItem.Single("Redo History...", Properties.Resources.action_redo_image, Keys.None);
        private readonly DualMenuItem ActionFind = new DualMenuItem("&Find", "Find", Properties.Resources.action_search_image, Keys.Control | Keys.F);
        private readonly ToolStripMenuItem ActionAbout = DualMenuItem.Single("&About", Properties.Resources.app_image_16, Keys.Shift | Keys.F1);
        private readonly ToolStripButton ActionAddSnbt = DualMenuItem.Single("Add as SNBT", Properties.Resources.action_add_snbt_image);
        private readonly ToolStripButton ActionAddChunk = DualMenuItem.Single("Add Chunk", Properties.Resources.chunk_image);
        public MainForm(string[] args)
        {
            ClickedFiles = args;
            if (Properties.Settings.Default.RecentFiles == null)
                Properties.Settings.Default.RecentFiles = new StringCollection();

            // stuff from the designer
            InitializeComponent();
            this.Icon = Properties.Resources.app_icon_256;

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
            ActionAddSnbt.Click += (s, e) => AddSnbt();
            ActionAddChunk.Click += (s, e) => AddChunk();

            ActionNew.AddTo(Tools, MenuFile);
            ActionOpenFile.AddTo(Tools, MenuFile);
            ActionOpenFolder.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            ActionSave.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(ActionSaveAs);
            MenuFile.DropDownItems.Add(ActionNewRegion);
            MenuFile.DropDownItems.Add(ActionNewClipboard);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            MenuFile.DropDownItems.Add(DropDownRecent);
            Tools.Items.Add(ActionSort);
            Tools.Items.Add(new ToolStripSeparator());
            MenuEdit.DropDownItems.Add(ActionUndo);
            MenuEdit.DropDownItems.Add(ActionRedo);
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
            MenuEdit.DropDownItems.Add(DropDownUndoHistory);
            MenuEdit.DropDownItems.Add(DropDownRedoHistory);
            Tools.Items.Add(new ToolStripSeparator());
            Tools.Items.Add(ActionAddChunk);
            MenuHelp.DropDownItems.Add(ActionAbout);

            CreateTagButtons = MakeCreateTagButtons();
            foreach (var item in CreateTagButtons.Values)
            {
                Tools.Items.Add(item);
            }
            Tools.Items.Add(ActionAddSnbt);

            Tools.Items.Add(new ToolStripSeparator());
            ActionFind.AddTo(Tools, MenuSearch);

            ViewModel = new NbtTreeModel();
            NbtTree.Font = new Font(NbtTree.Font.FontFamily, Properties.Settings.Default.TreeZoom);
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
                        var export = new ExportWindow(nbtfile.ExportSettings, dialog.FileName);
                        if (export.ShowDialog() == DialogResult.OK)
                            nbtfile.SaveAs(dialog.FileName, export.GetSettings());
                    }
                    else
                        file.SaveAs(dialog.FileName);
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
            BulkEditWindow.BulkRename(tags);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Bulk rename {0}", tags), false);
        }

        private void BulkEdit(IEnumerable<NbtTag> tags)
        {
            UndoHistory.StartBatchOperation();
            BulkEditWindow.BulkEdit(tags);
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
                RenameFileWindow.RenameFile(item);
        }

        private void EditTag(NbtTag tag)
        {
            if (ByteProviders.HasProvider(tag))
                EditHexWindow.ModifyTag(tag, EditPurpose.EditValue);
            else
                EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void EditChunk(Chunk chunk)
        {
            EditChunkWindow.MoveChunk(chunk);
        }

        private void RenameTag(NbtTag tag)
        {
            // likewise
            UndoHistory.StartBatchOperation();
            EditTagWindow.ModifyTag(tag, EditPurpose.Rename);
            UndoHistory.FinishBatchOperation(new DescriptionHolder("Rename {0}", tag), false);
        }

        private void EditSnbt()
        {
            var tag = NbtTree.SelectedINode?.GetNbtTag();
            if (tag == null) return;
            UndoHistory.StartBatchOperation();
            EditSnbtWindow.ModifyTag(tag, EditPurpose.EditValue);
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
                FindWindow = new FindWindow(NbtTree);
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

        private void AddSnbt()
        {
            var parent = NbtTree.SelectedINode?.GetNbtTag() as NbtContainerTag;
            if (parent == null) return;
            var tag = EditSnbtWindow.CreateTag(parent);
            if (tag != null)
                tag.AddTo(parent);
        }

        private void AddChunk()
        {
            var parent = NbtTree.SelectedINode?.GetRegionFile();
            if (parent == null) return;
            var chunk = EditChunkWindow.CreateChunk(parent, bypass_window: Control.ModifierKeys == Keys.Shift);
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
                tag = EditHexWindow.CreateTag(type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            else
                tag = EditTagWindow.CreateTag(type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (tag != null)
                container.Add(tag);
        }

        private Dictionary<NbtTagType, ToolStripButton> MakeCreateTagButtons()
        {
            var buttons = new Dictionary<NbtTagType, ToolStripButton>();
            foreach (var type in NbtUtil.NormalTagTypes())
            {
                var button = new ToolStripButton(
                    text: $"Add {NbtUtil.TagTypeName(type)} Tag",
                    image: NbtUtil.TagTypeImage(type),
                    onClick: (s, e) => AddTag(type));
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
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
            var save_image = multiple_files ? Properties.Resources.action_save_all_image : Properties.Resources.action_save_image;
            ActionSave.Image = save_image;
            ActionSaveAs.Image = save_image;
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
                    menu.Items.Add("&Save File", Properties.Resources.action_save_image, (s, ea) => Save(saveable));
                    menu.Items.Add("Save File &As", Properties.Resources.action_save_image, (s, ea) => SaveAs(saveable));
                }
                var path = obj.GetHasPath();
                if (path?.Path != null)
                    menu.Items.Add("&Open in Explorer", Properties.Resources.action_open_file_image, (s, ea) => OpenInExplorer(path));
                var folder = obj.GetNbtFolder();
                if (folder != null)
                    menu.Items.Add("&Refresh", Properties.Resources.action_refresh_image, (s, ea) => folder.Scan());
                var container = obj.GetNbtTag() as NbtContainerTag;
                if (container != null)
                {
                    if (menu.Items.Count > 0)
                        menu.Items.Add(new ToolStripSeparator());
                    var addable = NbtUtil.NormalTagTypes().Where(x => container.CanAdd(x));
                    bool single = Util.ExactlyOne(addable);
                    var display = single ? (Func<NbtTagType, string>)(x => $"Add {NbtUtil.TagTypeName(x)} Tag") : (x => $"{NbtUtil.TagTypeName(x)} Tag");
                    var items = addable.Select(x => new ToolStripMenuItem(display(x), NbtUtil.TagTypeImage(x), (s, ea) => AddTag(container, x))).ToArray();
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
                image = Properties.Resources.folder_image;
                click = (s, e) => OpenFolder(path);
            }
            else
            {
                if (!File.Exists(path))
                    return null;
                image = Properties.Resources.file_image;
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
