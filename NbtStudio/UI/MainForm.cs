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
            if (!Clipboard.ContainsText())
                return;
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
            ViewModel = new NbtTreeModel(file, NbtTree);
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
            if (ViewModel == null) return;
            foreach (var file in ViewModel.OpenedFiles)
            {
                Save(file.GetSaveable());
            }
        }

        private void SaveAs()
        {
            if (ViewModel == null) return;
            foreach (var file in ViewModel.OpenedFiles)
            {
                SaveAs(file.GetSaveable());
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
                Filter = NbtUtil.SaveFilter()
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
                        var export = new ExportWindow(nbtfile.ExportSettings);
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
            var obj = ViewModel?.SelectedObject;
            if (obj == null || !obj.CanSort) return;
            ViewModel.StartBatchOperation();
            obj.Sort();
            ViewModel.FinishBatchOperation($"Sort {obj.Description}", true);
        }

        private void Undo()
        {
            ViewModel.Undo();
        }

        private void Redo()
        {
            ViewModel.Redo();
        }

        private void Cut()
        {
            Copy();
            Delete();
        }

        private void Copy()
        {
            if (ViewModel == null) return;
            var objs = ViewModel.SelectedObjects.Where(x => x.CanCopy);
            if (objs.Any())
            {
                string text = String.Join("\n", objs.Select(x => x.Copy()));
                Clipboard.SetText(text);
            }
        }

        private void Paste()
        {
            var parent = ViewModel?.SelectedObject;
            if (parent == null) return;
            Paste(parent);
        }

        private void Paste(INode node)
        {
            if (!node.CanPaste || !Clipboard.ContainsText())
                return;
            ViewModel.StartBatchOperation();
            var results = node.Paste(Clipboard.GetText());
            ViewModel.FinishBatchOperation($"Paste {NodeExtractions.Description(results)} into {node.Description}", true);
        }

        private void Rename()
        {
            var obj = ViewModel?.SelectedObject;
            if (obj == null || !obj.CanRename) return;
            var tag = obj.GetNbtTag();
            var chunk = obj.GetChunk();
            if (tag != null)
                Rename(tag);
            else if (chunk != null)
                EditChunk(chunk);
        }

        private void Edit()
        {
            var obj = ViewModel?.SelectedObject;
            if (obj == null) return;
            Edit(obj);
        }

        private void Edit(INode node)
        {
            // batch operation to combine the rename and value change into one undo
            ViewModel.StartBatchOperation();
            if (!node.CanEdit) return;
            var tag = node.GetNbtTag();
            var chunk = node.GetChunk();
            if (tag != null)
                EditTag(tag);
            else if (chunk != null)
                EditChunk(chunk);
            ViewModel.FinishBatchOperation($"Edit {node.Description}", false);
        }

        private void EditTag(INbtTag tag)
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

        private void Rename(INbtTag tag)
        {
            // likewise
            ViewModel.StartBatchOperation();
            EditTagWindow.ModifyTag(tag, EditPurpose.Rename);
            ViewModel.FinishBatchOperation($"Rename {tag.TagDescription()}", false);
        }

        private void EditSnbt()
        {
            var tag = ViewModel?.SelectedObject?.GetNbtTag();
            if (tag == null) return;
            ViewModel.StartBatchOperation();
            EditSnbtWindow.ModifyTag(tag, EditPurpose.EditValue);
            ViewModel.FinishBatchOperation($"Edit {tag.TagDescription()} as SNBT", false);
        }

        private void Delete()
        {
            if (ViewModel == null) return;
            var selected_nodes = NbtTree.SelectedNodes;
            var nexts = selected_nodes.Select(x => x.NextNode).Where(x => x != null).ToList();
            var prevs = selected_nodes.Select(x => x.PreviousNode).Where(x => x != null).ToList();
            var parents = selected_nodes.Select(x => x.Parent).Where(x => x != null).ToList();

            var selected_objects = ViewModel.SelectedObjects.ToList();
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
            var files = nodes.Select(x => x.GetSaveable()).Where(x => x != null);
            if (files.Any())
            {
                DialogResult result;
                if (Util.ExactlyOne(files))
                {
                    var file = files.Single();
                    if (file.Path == null)
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
                    var unsaved = files.Where(x => x.Path == null);
                    var saved = files.Where(x => x.Path != null);
                    if (!saved.Any())
                        result = MessageBox.Show(
                            $"Are you sure you want to remove {Util.Pluralize(files.Count(), "file")}?",
                            $"Really delete these unsaved files?",
                            MessageBoxButtons.YesNo);
                    else
                        result = MessageBox.Show(
                            $"Are you sure you want to delete {Util.Pluralize(files.Count(), "file")}?\n\n" +
                            $"{Util.Pluralize(saved.Count(), "file")} will be send to the recycle bin. This cannot be undone.",
                            $"Really delete these files?",
                            MessageBoxButtons.YesNo);
                }
                if (result != DialogResult.Yes)
                    return;
            }
            ViewModel.StartBatchOperation();
            foreach (var node in nodes)
            {
                node.Delete();
            }
            ViewModel.FinishBatchOperation($"Delete {NodeExtractions.Description(nodes)}", false);
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
            var parent = ViewModel?.SelectedObject?.GetNbtTag() as INbtContainer;
            if (parent == null) return;
            var tag = EditSnbtWindow.CreateTag(parent);
            if (tag != null)
                tag.AddTo(parent);
        }

        private void AddChunk()
        {
            var parent = ViewModel?.SelectedObject?.GetRegionFile();
            if (parent == null) return;
            var chunk = EditChunkWindow.CreateChunk(parent, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (chunk != null)
                chunk.AddTo(parent);
        }

        private void AddTag(NbtTagType type)
        {
            var parent = ViewModel?.SelectedObject?.GetNbtTag() as INbtContainer;
            if (parent == null) return;
            AddTag(parent, type);
        }

        private void AddTag(INbtContainer container, NbtTagType type)
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
            ViewModel = new NbtTreeModel(new NbtFolder(path, true), NbtTree);
        }

        private void OpenFiles(IEnumerable<string> paths, bool skip_confirm = false)
        {
            if (!skip_confirm && !ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            var files = paths.Distinct().Select(x => NbtFolder.OpenFileOrFolder(Path.GetFullPath(x))).ToList();
            var bad = files.Where(x => x == null);
            var good = files.Where(x => x != null);
            if (bad.Any())
                MessageBox.Show($"{Util.Pluralize(bad.Count(), "file")} failed to load.", "Load Failure");
            if (good.Any())
            {
                Properties.Settings.Default.RecentFiles.AddRange(good.Select(x => x.Path).ToArray());
                ViewModel = new NbtTreeModel(good, NbtTree);
            }
        }

        private bool ConfirmIfUnsaved(string message)
        {
            if (ViewModel == null || !ViewModel.HasAnyUnsavedChanges)
                return true;
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            var obj = ViewModel?.SelectedObject;
            var nbt = obj.GetNbtTag();
            var container = nbt as INbtContainer;
            var region = obj.GetRegionFile();
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = container != null && container.CanAdd(item.Key);
                item.Value.Visible = region == null;
            }
            ActionSort.Enabled = obj != null && obj.CanSort;
            ActionCut.Enabled = obj != null && obj.CanCopy && obj.CanDelete;
            ActionCopy.Enabled = obj != null && obj.CanCopy;
            ActionPaste.Enabled = obj != null && obj.CanPaste; // don't check for Clipboard.ContainsText() because listening for clipboard events (to re-enable) is ugly
            ActionDelete.Enabled = obj != null && obj.CanDelete;
            ActionRename.Enabled = obj != null && (obj.CanRename || obj.CanEdit);
            ActionEdit.Enabled = obj != null && (obj.CanEdit || obj.CanRename);
            ActionEditSnbt.Enabled = nbt != null;
            ActionAddSnbt.Enabled = container != null;

            ActionAddSnbt.Visible = region == null;
            ActionAddChunk.Visible = region != null;
        }

        private void ViewModel_Changed(object sender, EventArgs e)
        {
            ActionSave.Enabled = ViewModel?.HasAnyUnsavedChanges ?? false;
            ActionSaveAs.Enabled = ViewModel != null;
            bool multiple_files = ViewModel != null && ViewModel.OpenedFiles.Skip(1).Any();
            var save_image = multiple_files ? Properties.Resources.action_save_all_image : Properties.Resources.action_save_image;
            ActionSave.Image = save_image;
            ActionSaveAs.Image = save_image;
            ActionUndo.Enabled = ViewModel?.CanUndo ?? false;
            ActionRedo.Enabled = ViewModel?.CanRedo ?? false;
            NbtTree_SelectionChanged(sender, e);
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var tag = ViewModel?.ObjectFromClick(e);
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
                var tags = ViewModel.ObjectsFromDrag(e);
                var drop = ViewModel.DropObject;
                if (tags.Any()
                    && ViewModel.DropObject != null
                    && CanMoveObjects(tags, drop, ViewModel.DropPosition))
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
                var tags = ViewModel.ObjectsFromDrag(e);
                var drop = ViewModel.DropObject;
                if (tags.Any())
                    MoveObjects(tags, drop, ViewModel.DropPosition);
            }
        }

        private bool CanMoveObjects(IEnumerable<INode> nodes, INode target, NodePosition position)
        {
            //var insert = NbtUtil.GetInsertionLocation(target, position);
            //if (insert.Item1 == null) return false;
            //return NbtUtil.CanAddAll(nodes, insert.Item1);
            return false;
        }

        private void MoveObjects(IEnumerable<INode> nodes, INode target, NodePosition position)
        {
            //var insert = NbtUtil.GetInsertionLocation(target, position);
            //if (insert.Item1 == null) return;
            //ViewModel.StartBatchOperation();
            //NbtUtil.TransformInsert(tags, insert.Item1, insert.Item2);
            //ViewModel.FinishBatchOperation($"Move {NbtUtil.TagDescription(tags)} into {insert.Item1.TagDescription()} at position {insert.Item2}", true);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            if (!ConfirmIfUnsaved("Exit anyway?"))
                e.Cancel = true;
#endif
        }

        private void NbtTree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var obj = ViewModel.ObjectFromClick(e);
                var menu = new ContextMenuStrip();
                if (e.Node.CanExpand)
                {
                    if (e.Node.IsExpanded)
                        menu.Items.Add("&Collapse", null, (s, ea) => e.Node.Collapse());
                    else
                        menu.Items.Add("&Expand All", null, (s, ea) => e.Node.ExpandAll());
                }
                var saveable = obj.GetSaveable();
                if (saveable != null)
                {
                    if (menu.Items.Count > 0)
                        menu.Items.Add(new ToolStripSeparator());
                    menu.Items.Add("&Save File", Properties.Resources.action_save_image, (s, ea) => Save(saveable));
                    menu.Items.Add("Save File &As", Properties.Resources.action_save_image, (s, ea) => SaveAs(saveable));
                    if (saveable.Path != null)
                        menu.Items.Add("&Open in Explorer", Properties.Resources.action_open_file_image, (s, ea) => OpenInExplorer(saveable));
                }
                var container = obj.GetNbtTag() as INbtContainer;
                if (container != null)
                {
                    if (menu.Items.Count > 0)
                        menu.Items.Add(new ToolStripSeparator());
                    var addable = NbtUtil.NormalTagTypes().Where(x => container.CanAdd(x));
                    bool single = Util.ExactlyOne(addable);
                    Func<NbtTagType, string> display = single ? (Func<NbtTagType, string>)(x => $"Add {NbtUtil.TagTypeName(x)} Tag") : (x => $"{NbtUtil.TagTypeName(x)} Tag");
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
            Properties.Settings.Default.Save();
        }

        private void MenuEdit_DropDownOpening(object sender, EventArgs e)
        {
            DropDownUndoHistory.Enabled = false;
            DropDownRedoHistory.Enabled = false;
            if (ViewModel == null) return;

            var undo_history = ViewModel.GetUndoHistory();
            var redo_history = ViewModel.GetRedoHistory();

            var undo_dropdown = new ToolStripDropDown();
            DropDownUndoHistory.DropDown = undo_dropdown;
            var undo_actions = new ActionHistory(undo_history,
                x => { ViewModel.Undo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Undo {Util.Pluralize(x + 1, "action")}",
                DropDownUndoHistory.Font);
            undo_dropdown.Items.Add(new ToolStripControlHost(undo_actions));

            var redo_dropdown = new ToolStripDropDown();
            DropDownRedoHistory.DropDown = redo_dropdown;
            var redo_actions = new ActionHistory(redo_history,
                x => { ViewModel.Redo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Redo {Util.Pluralize(x + 1, "action")}",
                DropDownRedoHistory.Font);
            redo_dropdown.Items.Add(new ToolStripControlHost(redo_actions));

            DropDownUndoHistory.Enabled = undo_history.Any();
            DropDownRedoHistory.Enabled = redo_history.Any();
        }

        private void MenuFile_DropDownOpening(object sender, EventArgs e)
        {
            ActionNewClipboard.Enabled = Clipboard.ContainsText();

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
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
