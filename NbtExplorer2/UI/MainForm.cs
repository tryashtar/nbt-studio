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

namespace NbtExplorer2.UI
{
    public partial class MainForm : Form
    {
        private NbtTreeModel ViewModel;

        private bool HasUnsavedChanges = false;
        private readonly Dictionary<NbtTagType, ToolStripButton> CreateTagButtons;

        public MainForm()
        {
            // stuff from the designer
            InitializeComponent();

            // stuff excluded from the designer for cleaner/less duplicated code
            CreateTagButtons = MakeCreateTagButtons();
            foreach (var item in CreateTagButtons.Values)
            {
                Tools.Items.Insert(Tools.Items.IndexOf(Separator4), item);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NbtTree_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void AddTag(NbtTagType type)
        {
            var tag = INbt.GetNbt(NbtTree.SelectedObject);
            if (tag == null)
                return;
            if (EditTagWindow.CreateTag(type, tag))
            {
                NbtTree.RefreshObjects(NbtTree.SelectedObjects); // don't do NbtTree.RefreshSelectedObjects(), it doesn't work properly
                NbtTree.Expand(NbtTree.SelectedObject);
                HasUnsavedChanges = true;
            }
        }

        private void ToolEdit_Click(object sender, EventArgs e)
        {
            var tag = INbt.GetNbt(NbtTree.SelectedObject);
            if (tag == null)
                return;
            if (EditTagWindow.ModifyTag(tag, EditPurpose.EditValue))
            {
                NbtTree.RefreshSelectedObjects();
                HasUnsavedChanges = true;
            }
        }

        private void ToolRename_Click(object sender, EventArgs e)
        {
            var tag = INbt.GetNbt(NbtTree.SelectedObject);
            if (tag == null)
                return;
            if (EditTagWindow.ModifyTag(tag, EditPurpose.Rename))
            {
                NbtTree.RefreshSelectedObjects();
                HasUnsavedChanges = true;
            }
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
            NbtTree.SetObjects(new[] { new NbtFile() });
            HasUnsavedChanges = false;
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
            NbtTree.SetObjects(new[] { new NbtFolder(path, true) });
            foreach (var item in NbtTree.Roots)
            {
                NbtTree.Expand(item);
            }
            HasUnsavedChanges = false;
        }

        private void OpenFiles(IEnumerable<string> paths)
        {
            //NbtTree.SetObjects(Controller.OpenFiles(paths));
            //foreach (var item in NbtTree.Roots)
            //{
            //    NbtTree.Expand(item);
            //}
            ViewModel = new NbtTreeModel(Controller.OpenFiles(paths));
            NbtTree2.Model = ViewModel;
            HasUnsavedChanges = false;
        }

        private bool ConfirmIfUnsaved(string message)
        {
            if (!HasUnsavedChanges)
                return true;
            return MessageBox.Show($"You currently have unsaved changes.\n\n{message}", "Unsaved Changes", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        private void ToolDelete_Click(object sender, EventArgs e)
        {
            ViewModel.Remove(NbtTree2.SelectedObject);
            HasUnsavedChanges = true;
        }

        private void NbtTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tag = INbt.GetNbt(NbtTree2.SelectedObject);
            foreach (var item in CreateTagButtons)
            {
                item.Value.Enabled = INbt.CanAdd(tag, item.Key);
            }
        }
    }
}
