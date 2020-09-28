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
using NbtExplorer2.SNBT;
using System.Collections.Specialized;
using System.Diagnostics;

namespace NbtExplorer2.UI
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
        private readonly DualMenuItem ActionOpenFile = new DualMenuItem("&Open File", "Open File", Properties.Resources.action_open_file_image, Keys.Control | Keys.O);
        private readonly DualMenuItem ActionOpenFolder = new DualMenuItem("Open &Folder", "Open Folder", Properties.Resources.action_open_folder_image, Keys.Control | Keys.Shift | Keys.O);
        private readonly DualMenuItem ActionSave = new DualMenuItem("&Save", "Save", Properties.Resources.action_save_image, Keys.Control | Keys.S);
        private readonly ToolStripMenuItem ActionSaveAs = DualMenuItem.Single("Save &As", Properties.Resources.action_save_image, Keys.Control | Keys.Shift | Keys.S);
        private readonly DualMenuItem ActionRefresh = new DualMenuItem("Re&fresh", "Refresh", Properties.Resources.action_refresh_image, Keys.F5);
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
        private readonly DualMenuItem ActionFind = new DualMenuItem("&Find", "Find", Properties.Resources.action_search_image, Keys.Control | Keys.F);
        private readonly ToolStripButton ActionAddSnbt = DualMenuItem.Single("Add as SNBT", Properties.Resources.action_add_snbt_image);
        public MainForm(string[] args)
        {
            ClickedFiles = args;
            if (Properties.Settings.Default.RecentFiles == null)
                Properties.Settings.Default.RecentFiles = new StringCollection();

            // stuff from the designer
            InitializeComponent();

            // stuff excluded from the designer for cleaner/less duplicated code
            ActionNew.Click += (s, e) => New();
            ActionOpenFile.Click += (s, e) => OpenFile();
            ActionOpenFolder.Click += (s, e) => OpenFolder();
            ActionSave.Click += (s, e) => Save();
            ActionSaveAs.Click += (s, e) => SaveAs();
            ActionRefresh.Click += (s, e) => DoRefresh();
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
            ActionAddSnbt.Click += (s, e) => AddSnbt();

            ActionNew.AddTo(Tools, MenuFile);
            ActionOpenFile.AddTo(Tools, MenuFile);
            ActionOpenFolder.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            ActionSave.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(ActionSaveAs);
            ActionRefresh.AddTo(Tools, MenuFile);
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
            Tools.Items.Add(new ToolStripSeparator());

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
            ViewModel = new NbtTreeModel(new NbtFile(), NbtTree);
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
                Filter = "All Files|*|NBT Files|*.dat;*.nbt;*.mca;*.mcr;*.schematic;*.mcstructure;*.snbt",
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    OpenFiles(dialog.FileNames, skip_confirm: true);
            }
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
                Save(file);
            }
        }

        private void SaveAs()
        {
            if (ViewModel == null) return;
            foreach (var file in ViewModel.OpenedFiles)
            {
                SaveAs(file);
            }
        }

        private void Save(ISaveable file)
        {
            if (file.CanSave)
                file.Save();
            else
                SaveAs(file);
        }

        private void SaveAs(ISaveable file)
        {
            using (var dialog = new SaveFileDialog
            {
                Title = file.Path == null ? "Save NBT file" : $"Save {Path.GetFileName(file.Path)} as...",
                RestoreDirectory = true,
                Filter = "All Files|*|NBT Files|*.dat;*.nbt;*.mca;*.mcr;*.schematic;*.mcstructure;*.snbt"
            })
            {
                if (file.Path != null)
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(file.Path);
                    dialog.FileName = Path.GetFileName(file.Path);
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (file is INbtFile nbtfile)
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

        private void DoRefresh()
        { }

        private void DoRefresh(ISaveable file)
        { }

        private void OpenInExplorer(ISaveable file)
        {
            var info = new ProcessStartInfo { FileName = "explorer", Arguments = $"/select, \"{file.Path}\"" };
            Process.Start(info);
        }

        private void Sort()
        {
            var tag = ViewModel?.SelectedNbt as INbtCompound;
            if (tag == null) return;
            ViewModel.StartBatchOperation();
            NbtUtil.Sort(tag, new NbtUtil.TagTypeSorter(), true);
            ViewModel.FinishBatchOperation();
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
            if (ViewModel?.SelectedNbt != null)
            {
                Copy(ViewModel.SelectedNbts);
                Delete();
            }
        }

        private void Copy()
        {
            if (ViewModel?.SelectedNbt != null)
                Copy(ViewModel.SelectedNbts);
        }

        private void Paste()
        {
            var parent = ViewModel?.SelectedNbt as INbtContainer;
            if (parent != null)
                Paste(parent);
        }

        private void Rename()
        {
            var tag = ViewModel?.SelectedNbt;
            if (tag == null) return;
            EditTagWindow.ModifyTag(tag, EditPurpose.Rename);
        }

        private void Edit()
        {
            var tag = ViewModel?.SelectedNbt;
            if (tag == null) return;
            EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void EditSnbt()
        {
            var tag = ViewModel?.SelectedNbt;
            if (tag == null) return;
            ViewModel.StartBatchOperation();
            EditSnbtWindow.ModifyTag(tag, EditPurpose.EditValue);
            ViewModel.FinishBatchOperation();
        }

        private void Delete()
        {
            var selected = NbtTree.SelectedNodes;
            var nexts = selected.Select(x => x.NextNode).Where(x => x != null).ToList();
            var prevs = selected.Select(x => x.PreviousNode).Where(x => x != null).ToList();
            var parents = selected.Select(x => x.Parent).Where(x => x != null).ToList();
            ViewModel.StartBatchOperation();
            foreach (var item in ViewModel.SelectedNbts.ToList())
            {
                item.Remove();
            }
            ViewModel.FinishBatchOperation();
            // Index == -1 checks whether this node has been removed from the tree
            if (selected.All(x => x.Index == -1))
            {
                var select_next = nexts.FirstOrDefault(x => x.Index != -1) ?? prevs.FirstOrDefault(x => x.Index != -1) ?? parents.FirstOrDefault(x => x.Index != -1);
                if (select_next != null)
                    select_next.IsSelected = true;
            }
        }

        private void Find()
        { }

        private void AddSnbt()
        {
            var parent = ViewModel?.SelectedNbt as INbtContainer;
            if (parent == null) return;
            var tag = EditSnbtWindow.CreateTag(parent);
            if (tag != null)
                tag.AddTo(parent);
        }

        private void AddTag(NbtTagType type)
        {
            var parent = ViewModel?.SelectedNbt as INbtContainer;
            if (parent == null) return;
            AddTag(parent, type);
        }

        private void AddTag(INbtContainer container, NbtTagType type)
        {
            var tag = EditTagWindow.CreateTag(type, container, bypass_window: Control.ModifierKeys == Keys.Shift);
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
            var files = paths.Distinct().ToDictionary(x => Path.GetFullPath(x), x => NbtFolder.OpenFileOrFolder(x)).ToList();
            var bad = files.Where(x => x.Value == null);
            var good = files.Where(x => x.Value != null);
            if (bad.Any())
                MessageBox.Show($"{Util.Pluralize(bad.Count(), "file")} failed to load.", "Load failure");
            if (good.Any())
            {
                Properties.Settings.Default.RecentFiles.AddRange(good.Select(x => x.Key).ToArray());
                ViewModel = new NbtTreeModel(good.Select(x => x.Value), NbtTree);
            }
        }

        private bool ConfirmIfUnsaved(string message)
        {
            if (ViewModel == null || !ViewModel.HasUnsavedChanges)
                return true;
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            var tag = ViewModel?.SelectedNbt;
            var container = tag as INbtContainer;
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = container != null && container.CanAdd(item.Key);
            }
            ActionSort.Enabled = tag is INbtCompound;
            ActionCut.Enabled = tag != null;
            ActionCopy.Enabled = tag != null;
            ActionPaste.Enabled = container != null;
            ActionDelete.Enabled = tag != null;
            ActionRename.Enabled = tag != null;
            ActionEdit.Enabled = tag != null;
            ActionEditSnbt.Enabled = tag != null;
            ActionAddSnbt.Enabled = container != null;
        }

        private void ViewModel_Changed(object sender, EventArgs e)
        {
            ActionSave.Enabled = ViewModel?.HasUnsavedChanges ?? false;
            ActionSaveAs.Enabled = ViewModel != null;
            var save_image = ViewModel != null && ViewModel.OpenedFiles.Skip(1).Any() ? Properties.Resources.action_save_all_image : Properties.Resources.action_save_image;
            ActionSave.Image = save_image;
            ActionSaveAs.Image = save_image;
            ActionRefresh.Enabled = ViewModel != null;
            ActionUndo.Enabled = ViewModel?.CanUndo ?? false;
            ActionRedo.Enabled = ViewModel?.CanRedo ?? false;
            NbtTree_SelectionChanged(sender, e);
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var tag = ViewModel?.SelectedNbt;
            if (tag != null && NbtUtil.IsValueType(tag.TagType))
                EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void Copy(IEnumerable<INbtTag> objects)
        {
            Clipboard.SetText(String.Join("\n", objects.Select(x => x.ToSnbt(include_name: true))));
        }

        private void Paste(INbtContainer destination)
        {
            var snbts = Clipboard.GetText().Split('\n');
            ViewModel.StartBatchOperation();
            foreach (var nbt in snbts)
            {
                if (SnbtParser.TryParse(nbt, true, out NbtTag tag) || SnbtParser.TryParse(nbt, false, out tag))
                    NbtUtil.TransformAdd(tag.Adapt(), destination);
            }
            ViewModel.FinishBatchOperation();
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
                var tags = ViewModel.NbtsFromDrag(e);
                if (tags.Any()
                    && ViewModel.DropTag != null
                    && CanMoveTags(tags, ViewModel.DropTag, ViewModel.DropPosition))
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
                if (!ConfirmIfUnsaved("Open a new file anyway?"))
                    return;
                OpenFiles(files);
            }
            else
            {
                var tags = ViewModel.NbtsFromDrag(e);
                if (tags.Any())
                    MoveTags(tags, ViewModel.DropTag, ViewModel.DropPosition);
            }
        }

        private bool CanMoveTags(IEnumerable<INbtTag> tags, INbtTag target, NodePosition position)
        {
            var insert = NbtUtil.GetInsertionLocation(target, position);
            if (insert.Item1 == null) return false;
            return NbtUtil.CanAddAll(tags, insert.Item1);
        }

        private void MoveTags(IEnumerable<INbtTag> tags, INbtTag target, NodePosition position)
        {
            var insert = NbtUtil.GetInsertionLocation(target, position);
            if (insert.Item1 == null) return;
            ViewModel.StartBatchOperation();
            // reverse so that if we start with ABC, then insert C at index 0, B at index 0, A at index 0, it ends up ABC
            foreach (var tag in tags.Reverse().ToList())
            {
                NbtUtil.TransformInsert(tag, insert.Item1, insert.Item2);
            }
            ViewModel.FinishBatchOperation();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmIfUnsaved("Exit anyway?"))
                e.Cancel = true;
        }

        private void NbtTree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var file = ViewModel.FileFromClick(e);
                var nbt = ViewModel.NbtFromClick(e);
                var menu = new ContextMenuStrip();
                if (file != null)
                {
                    menu.Items.Add("&Save File", Properties.Resources.action_save_image, (s, ea) => Save(file));
                    menu.Items.Add("Save File &As", Properties.Resources.action_save_image, (s, ea) => SaveAs(file));
                    if (file.Path != null)
                        menu.Items.Add("&Open in Explorer", Properties.Resources.action_open_file_image, (s, ea) => OpenInExplorer(file));
                    menu.Items.Add("&Refresh", Properties.Resources.action_refresh_image, (s, ea) => DoRefresh(file));
                }
                if (nbt is INbtContainer container)
                {
                    if (menu.Items.Count > 0)
                        menu.Items.Add(new ToolStripSeparator());
                    var addable = NbtUtil.NormalTagTypes().Where(x => container.CanAdd(x));
                    bool single = !addable.Skip(1).Any();
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

        private void MenuFile_DropDownOpening(object sender, EventArgs e)
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
    }
}
