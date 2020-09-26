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
        private readonly DualMenuItem ActionRefresh = new DualMenuItem("&Refresh", "Refresh", Properties.Resources.action_refresh_image, Keys.F5);
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
                Filter = "All Files|*|NBT Files|*.dat;*.nbt;*.schematic;*.mcstructure;*.snbt",
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    OpenFiles(dialog.FileNames);
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
                    OpenFolder(Path.GetDirectoryName(dialog.FileName));
            }
        }

        private void Save()
        {
            if (ViewModel == null) return;
            foreach (var file in ViewModel.OpenedFiles)
            {
                if (file.CanSave)
                    file.Save();
                else
                    SaveAs(file);
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

        private void SaveAs(INbtFile file)
        {
            using (var dialog = new SaveFileDialog
            {
                Title = file.Path == null ? "Save NBT file" : $"Save {Path.GetFileName(file.Path)} as...",
                RestoreDirectory = true,
                Filter = "All Files|*|NBT Files|*.dat;*.nbt;*.schematic;*.mcstructure;*.snbt"
            })
            {
                if (file.Path != null)
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(file.Path);
                    dialog.FileName = Path.GetFileName(file.Path);
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var export = new ExportWindow(file.ExportSettings);
                    if (export.ShowDialog() == DialogResult.OK)
                        file.SaveAs(dialog.FileName, export.GetSettings());
                }
            }
        }

        private void DoRefresh()
        { }

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
                ViewModel.StartBatchOperation();
                foreach (var item in ViewModel.SelectedNbts.ToList())
                {
                    item.Remove();
                }
                ViewModel.FinishBatchOperation();
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
            EditSnbtWindow.ModifyTag(tag);
        }

        private void Delete()
        {
            ViewModel.StartBatchOperation();
            foreach (var item in ViewModel.SelectedNbts.ToList())
            {
                item.Remove();
            }
            ViewModel.FinishBatchOperation();
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
            var tag = EditTagWindow.CreateTag(type, parent, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (tag != null)
                parent.Add(tag);
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

        private void OpenFolder(string path)
        {
            ViewModel = new NbtTreeModel(new NbtFolder(path, true), NbtTree);
        }

        private void OpenFiles(IEnumerable<string> paths)
        {
            var files = paths.Select(x => NbtFile.TryCreate(x));
            var bad = files.Where(x => x == null);
            var good = files.Where(x => x != null);
            if (bad.Any())
                MessageBox.Show($"{Util.Pluralize(bad.Count(), "file")} failed to load.", "Load failure");
            if (good.Any())
                ViewModel = new NbtTreeModel(good, NbtTree);
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
    }
}
