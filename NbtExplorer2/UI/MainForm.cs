using fNbt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class MainForm : Form
    {
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
            NbtTag tag = null;
            if (NbtTree.SelectedObject is NbtTag selected)
                tag = selected;
            else if (NbtTree.SelectedObject is NbtFile file)
                tag = file.RootTag;
            if (tag != null)
            {
                if (EditTagWindow.CreateTag(type, tag))
                {
                    NbtTree.RefreshObjects(NbtTree.SelectedObjects); // don't do NbtTree.RefreshSelectedObjects(), it doesn't work properly
                    NbtTree.Expand(NbtTree.SelectedObject);
                    HasUnsavedChanges = true;
                }
            }
        }

        private void ToolEdit_Click(object sender, EventArgs e)
        {
            var tag = Controller.GetTag(NbtTree.SelectedObject);
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
            var tag = Controller.GetTag(NbtTree.SelectedObject);
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
            foreach (var type in Util.NormalTagTypes())
            {
                var button = new ToolStripButton(
                    text: $"Add {Util.TagTypeName(type)} Tag",
                    image: Util.TagTypeImage(type),
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

        private void OpenFiles(IEnumerable<string> paths)
        {
            NbtTree.SetObjects(Controller.OpenFiles(paths));
            foreach (var item in NbtTree.Roots)
            {
                NbtTree.Expand(item);
            }
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
            Controller.DeleteNbt(NbtTree.SelectedObjects);
            NbtTree.RemoveObjects(NbtTree.SelectedObjects);
            HasUnsavedChanges = true;
        }

        private void NbtTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in CreateTagButtons)
            {
                if (NbtTree.SelectedObject is NbtFile)
                    item.Value.Enabled = true;
                else if (NbtTree.SelectedObject is NbtTag tag)
                    item.Value.Enabled = Util.CanAdd(tag, item.Key);
                else
                    item.Value.Enabled = false;
            }
        }
    }
}
