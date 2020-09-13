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

namespace NbtExplorer2
{
    public partial class MainForm : Form
    {
        private bool HasUnsavedChanges = false;

        public MainForm()
        {
            // stuff from the designer
            InitializeComponent();

            // stuff excluded from the designer for cleaner/less duplicated code
            foreach (var item in CreateTagButtons())
            {
                Tools.Items.Insert(Tools.Items.IndexOf(Separator4), item);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void AddTag(NbtTagType type)
        {

        }

        private IEnumerable<ToolStripButton> CreateTagButtons()
        {
            foreach (var type in Util.NormalTagTypes())
            {
                var button = new ToolStripButton(
                    text: $"Add {Util.TagTypeName(type)} Tag",
                    image: Util.TagTypeImage(type),
                    onClick: (s, e) => AddTag(type));
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                yield return button;
            }
        }

        private void ToolNew_Click(object sender, EventArgs e)
        {

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
            NbtTree.RefreshSelectedObjects();
        }
    }
}
