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
        private NbtTreeModel ViewModel;
        private readonly Dictionary<NbtTagType, ToolStripButton> CreateTagButtons;
        private readonly string[] ClickedFiles;

        private readonly DualMenuItem ActionNew = new DualMenuItem("&New", "New File", Properties.Resources.action_new_image, Keys.Control | Keys.N);
        private readonly DualMenuItem ActionOpenFile = new DualMenuItem("&Open File", "Open File", Properties.Resources.action_open_file_image, Keys.Control | Keys.O);
        private readonly DualMenuItem ActionOpenFolder = new DualMenuItem("Open &Folder", "Open Folder", Properties.Resources.action_open_folder_image, Keys.Control | Keys.Shift | Keys.O);
        private readonly DualMenuItem ActionSave = new DualMenuItem("&Save", "Save", Properties.Resources.action_save_image, Keys.Control | Keys.S);
        private readonly ToolStripMenuItem ActionSaveAs = DualMenuItem.Single("Save &As", Properties.Resources.action_save_image, Keys.Control | Keys.Shift | Keys.S);
        private readonly DualMenuItem ActionRefresh = new DualMenuItem("&Refresh", "Refresh", Properties.Resources.action_refresh_image, Keys.F5);
        private readonly ToolStripButton ActionSort = DualMenuItem.Single("Sort", Properties.Resources.action_sort_image);
        private readonly DualMenuItem ActionCut = new DualMenuItem("Cu&t", "Cut", Properties.Resources.action_cut_image, Keys.Control | Keys.X);
        private readonly DualMenuItem ActionCopy = new DualMenuItem("&Copy", "Copy", Properties.Resources.action_copy_image, Keys.Control | Keys.C);
        private readonly DualMenuItem ActionPaste = new DualMenuItem("&Paste", "Paste", Properties.Resources.action_paste_image, Keys.Control | Keys.V);
        private readonly DualMenuItem ActionRename = new DualMenuItem("&Rename", "Rename", Properties.Resources.action_rename_image, Keys.F2);
        private readonly DualMenuItem ActionEdit = new DualMenuItem("&Edit Value", "Edit", Properties.Resources.action_edit_image, Keys.Control | Keys.E);
        private readonly DualMenuItem ActionEditSnbt = new DualMenuItem("Edit as &SNBT", "Edit as SNBT", Properties.Resources.action_edit_snbt_image, Keys.Control | Keys.Shift | Keys.E);
        private readonly DualMenuItem ActionDelete = new DualMenuItem("&Delete", "Delete", Properties.Resources.action_delete_image, Keys.Delete);
        private readonly DualMenuItem ActionFind = new DualMenuItem("&Find", "Find", Properties.Resources.action_search_image, Keys.Control | Keys.F);
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
            ActionCut.Click += (s, e) => Cut();
            ActionCopy.Click += (s, e) => Copy();
            ActionPaste.Click += (s, e) => Paste();
            ActionRename.Click += (s, e) => Rename();
            ActionEdit.Click += (s, e) => Edit();
            ActionEditSnbt.Click += (s, e) => EditSnbt();
            ActionDelete.Click += (s, e) => Delete();
            ActionFind.Click += (s, e) => Find();

            ActionNew.AddTo(Tools, MenuFile);
            ActionOpenFile.AddTo(Tools, MenuFile);
            ActionOpenFolder.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            ActionSave.AddTo(Tools, MenuFile);
            MenuFile.DropDownItems.Add(ActionSaveAs);
            ActionRefresh.AddTo(Tools, MenuFile);
            Tools.Items.Add(ActionSort);
            Tools.Items.Add(new ToolStripSeparator());
            ActionCut.AddTo(Tools, MenuEdit);
            ActionCopy.AddTo(Tools, MenuEdit);
            ActionPaste.AddTo(Tools, MenuEdit);
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            Tools.Items.Add(new ToolStripSeparator());
            ActionRename.AddTo(Tools, MenuEdit);
            ActionEdit.AddTo(Tools, MenuEdit);
            ActionEditSnbt.AddTo(Tools, MenuEdit);
            ActionDelete.AddTo(Tools, MenuEdit);

            CreateTagButtons = MakeCreateTagButtons();
            foreach (var item in CreateTagButtons.Values)
            {
                Tools.Items.Add(item);
            }

            Tools.Items.Add(new ToolStripSeparator());
            ActionFind.AddTo(Tools, MenuSearch);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NbtTree_SelectionChanged(this, EventArgs.Empty);
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
            // temporary test
            var file = (NbtFile)NbtTree.SelectedNode.Tag;
            file.SaveAs(Path.Combine(Path.GetDirectoryName(file.Path), "test.nbt"), file.ExportSettings);
        }

        private void SaveAs()
        { }

        private void DoRefresh()
        { }

        private void Sort()
        {
            var tag = ViewModel?.SelectedNbt as INbtCompound;
            if (tag == null) return;
            INbt.Sort(tag, new INbt.TagTypeSorter(), true);
        }

        private void Cut()
        {
            if (ViewModel?.SelectedNbt != null)
            {
                Copy(ViewModel.SelectedNbts);
                foreach (var item in ViewModel.SelectedNbts.ToList())
                {
                    item.Remove();
                }
            }
        }

        private void Copy()
        {
            if (ViewModel?.SelectedNbt != null)
                Copy(ViewModel.SelectedNbts);
        }

        private void Paste()
        {
            var parent = ViewModel.SelectedNbt as INbtContainer;
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
        { }

        private void Delete()
        {
            foreach (var item in ViewModel.SelectedNbts.ToList())
            {
                item.Remove();
            }
        }

        private void Find()
        { }

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
            foreach (var type in INbt.NormalTagTypes())
            {
                var button = new ToolStripButton(
                    text: $"Add {INbt.TagTypeName(type)} Tag",
                    image: INbt.TagTypeImage(type),
                    onClick: (s, e) => AddTag(type));
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                buttons.Add(type, button);
            }
            return buttons;
        }

        private void OpenFolder(string path)
        {
            ViewModel = new NbtTreeModel(new[] { new NbtFolder(path, true) }, NbtTree);
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
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo) == DialogResult.Yes;
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
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var tag = ViewModel?.SelectedNbt;
            if (tag != null && INbt.IsValueType(tag.TagType))
                EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void Copy(IEnumerable<INbtTag> objects)
        {
            Clipboard.SetText(String.Join("\n", objects.Select(x => x.ToSnbt(include_name: true))));
        }

        private void Paste(INbtContainer destination)
        {
            var snbts = Clipboard.GetText().Split('\n');
            foreach (var nbt in snbts)
            {
                if (SnbtParser.TryParse(nbt, true, out NbtTag tag) || SnbtParser.TryParse(nbt, false, out tag))
                    INbt.TransformAdd(tag.Adapt(), destination);
            }
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
            var insert = INbt.GetInsertionLocation(target, position);
            if (insert.Item1 == null) return false;
            return INbt.CanAddAll(tags, insert.Item1);
        }

        private void MoveTags(IEnumerable<INbtTag> tags, INbtTag target, NodePosition position)
        {
            var insert = INbt.GetInsertionLocation(target, position);
            if (insert.Item1 == null) return;
            // reverse so that if we start with ABC, then insert C at index 0, B at index 0, A at index 0, it ends up ABC
            foreach (var tag in tags.Reverse().ToList())
            {
                INbt.TransformInsert(tag, insert.Item1, insert.Item2);
            }
        }
    }
}
