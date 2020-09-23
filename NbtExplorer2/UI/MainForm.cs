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

        public MainForm(string[] args)
        {
            ClickedFiles = args;

            // stuff from the designer
            InitializeComponent();

            // stuff excluded from the designer for cleaner/less duplicated code
            CreateTagButtons = MakeCreateTagButtons();
            foreach (var item in CreateTagButtons.Values)
            {
                Tools.Items.Insert(Tools.Items.IndexOf(ToolSeparator4), item);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NbtTree_SelectionChanged(this, EventArgs.Empty);
            if (ClickedFiles != null && ClickedFiles.Any())
                OpenFiles(ClickedFiles);
        }

        private void AddTag(NbtTagType type)
        {
            var parent = ViewModel.SelectedNbt as INbtContainer;
            if (parent == null) return;
            var tag = EditTagWindow.CreateTag(type, parent, bypass_window: Control.ModifierKeys == Keys.Shift);
            if (tag != null)
                parent.Add(tag);
        }

        private void ToolEdit_Click(object sender, EventArgs e)
        {
            var tag = ViewModel.SelectedNbt;
            if (tag == null) return;
            EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void ToolRename_Click(object sender, EventArgs e)
        {
            var tag = ViewModel.SelectedNbt;
            if (tag == null) return;
            EditTagWindow.ModifyTag(tag, EditPurpose.Rename);
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

        private void ToolNew_Click(object sender, EventArgs e)
        {
            if (!ConfirmIfUnsaved("Create a new file anyway?"))
                return;
            ViewModel = new NbtTreeModel(new NbtFile(), NbtTree);
        }

        private void ToolOpenFile_Click(object sender, EventArgs e)
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

        private void ToolOpenFolder_Click(object sender, EventArgs e)
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

        private void ToolDelete_Click(object sender, EventArgs e)
        {
            foreach (var item in ViewModel.SelectedNbts.ToList())
            {
                item.Remove();
            }
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            var tag = ViewModel?.SelectedNbt as INbtContainer;
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = tag == null ? false : tag.CanAdd(item.Key);
            }
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            //if (e.Node?.Tag is NbtTag tag && INbt.IsValueType(tag.TagType))
            //    EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void ToolSave_Click(object sender, EventArgs e)
        {
            // temporary test
            var file = (NbtFile)NbtTree.SelectedNode.Tag;
            file.SaveAs(Path.Combine(Path.GetDirectoryName(file.Path), "test.nbt"), file.ExportSettings);
        }

        private void ToolRefresh_Click(object sender, EventArgs e)
        {

        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {

        }

        private void ToolCut_Click(object sender, EventArgs e)
        {
            if (ViewModel.SelectedNbt != null)
            {
                Copy(ViewModel.SelectedNbts);
                foreach (var item in ViewModel.SelectedNbts.ToList())
                {
                    item.Remove();
                }
            }
        }

        private void ToolCopy_Click(object sender, EventArgs e)
        {
            if (ViewModel.SelectedNbt != null)
                Copy(ViewModel.SelectedNbts);
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

        private void ToolPaste_Click(object sender, EventArgs e)
        {
            var parent = ViewModel.SelectedNbt as INbtContainer;
            if (parent != null)
                Paste(parent);
        }

        private void ToolEditSnbt_Click(object sender, EventArgs e)
        {

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
            // reverse so that if we start with ABC, then insert C at index 0, B at index 0, A at index 0, it ends up ABC
            foreach (var tag in tags.Reverse().ToList())
            {
                INbt.TransformInsert(tag, insert.Item1, insert.Item2);
            }
        }
    }
}
