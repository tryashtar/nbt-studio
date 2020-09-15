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

namespace NbtExplorer2.UI
{
    public partial class MainForm : Form
    {
        private NbtTreeModel ViewModel;

        private readonly Dictionary<NbtTagType, ToolStripButton> CreateTagButtons;

        public MainForm()
        {
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
        }

        private void AddTag(NbtTagType type)
        {
            var parent = INbt.GetNbt(NbtTree.SelectedObject);
            if (parent == null)
                return;
            var tag = EditTagWindow.CreateTag(type, parent, Control.ModifierKeys == Keys.Shift);
            if (tag != null)
                ViewModel.Add(tag, NbtTree.SelectedObject); // NOT parent, because the selected object could be an NbtFile, while parent would be its compound
        }

        private void ToolEdit_Click(object sender, EventArgs e)
        {
            var tag = INbt.GetNbt(NbtTree.SelectedObject);
            if (tag == null)
                return;
            if (EditTagWindow.ModifyTag(tag, EditPurpose.EditValue))
                ViewModel.NoticeChanges(tag);
        }

        private void ToolRename_Click(object sender, EventArgs e)
        {
            var tag = INbt.GetNbt(NbtTree.SelectedObject);
            if (tag == null)
                return;
            if (EditTagWindow.ModifyTag(tag, EditPurpose.Rename))
                ViewModel.NoticeChanges(tag);
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
            ViewModel = new NbtTreeModel(new[] { new NbtFile() }, NbtTree);
        }

        private void ToolOpenFile_Click(object sender, EventArgs e)
        {
            if (!ConfirmIfUnsaved("Open a new file anyway?"))
                return;
            using (var dialog = new OpenFileDialog
            {
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
            using (var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "All Files|*|NBT Files|*.dat;*.nbt;*.schematic;*.mcstructure;*.snbt",
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    OpenFolder(Path.GetDirectoryName(dialog.FileName));
            }
        }

        private void OpenFolder(string path)
        {
            ViewModel = new NbtTreeModel(new[] { new NbtFolder(path, true) }, NbtTree);
        }

        private void OpenFiles(IEnumerable<string> paths)
        {
            ViewModel = new NbtTreeModel(Controller.OpenFiles(paths), NbtTree);
        }

        private bool ConfirmIfUnsaved(string message)
        {
            if (ViewModel == null || !ViewModel.HasUnsavedChanges)
                return true;
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        private void ToolDelete_Click(object sender, EventArgs e)
        {
            if (NbtTree.SelectedObject != null)
                ViewModel.RemoveAll(NbtTree.SelectedObjects);
        }

        private void NbtTree_SelectionChanged(object sender, EventArgs e)
        {
            var tag = INbt.GetNbt(NbtTree.SelectedObject);
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = INbt.CanAdd(tag, item.Key);
            }
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Node?.Tag is NbtTag tag && INbt.IsValueType(tag.TagType))
                EditTagWindow.ModifyTag(tag, EditPurpose.EditValue);
        }

        private void ToolSave_Click(object sender, EventArgs e)
        {

        }

        private void ToolRefresh_Click(object sender, EventArgs e)
        {

        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {

        }

        private void ToolCut_Click(object sender, EventArgs e)
        {

        }

        private void ToolCopy_Click(object sender, EventArgs e)
        {

        }

        private void ToolPaste_Click(object sender, EventArgs e)
        {

        }

        private void ToolEditSnbt_Click(object sender, EventArgs e)
        {

        }
    }
}
