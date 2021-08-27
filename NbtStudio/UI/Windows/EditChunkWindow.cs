using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class EditChunkWindow : Form
    {
        private readonly ChunkEntry WorkingChunk;
        private readonly ChunkCoordsEditControls Manager;
        public ICommand CommandResult;

        private EditChunkWindow(IconSource source, ChunkEntry chunk, ChunkEditPurpose purpose)
        {
            InitializeComponent();

            WorkingChunk = chunk;
            Manager = new ChunkCoordsEditControls(chunk, XBox, ZBox);

            this.Icon = source.GetImage(IconType.Chunk).Icon;
            if (purpose == ChunkEditPurpose.Create)
                this.Text = $"Create Chunk";
            else if (purpose == ChunkEditPurpose.Move)
                this.Text = $"Move Chunk";

            XBox.Select();
        }

        public static Chunk CreateChunk(IconSource source, RegionFile parent, NbtCompound data = null)
        {
            return null;
            //var chunk = new Chunk(data);
            //var window = new EditChunkWindow(source, chunk, ChunkEditPurpose.Create);
            //return window.ShowDialog() == DialogResult.OK ? chunk : null;
        }

        public static ICommand MoveChunk(IconSource source, ChunkEntry existing)
        {
            var window = new EditChunkWindow(source, existing, ChunkEditPurpose.Move);
            if (window.ShowDialog() == DialogResult.OK)
                return window.CommandResult;
            return null;
        }

        private void Confirm()
        {
            if (TryModify(out ICommand result))
            {
                DialogResult = DialogResult.OK;
                CommandResult = result;
                Close();
            }
        }

        private bool TryModify(out ICommand command)
        {
            command = null;
            if (!Manager.CheckCoords())
                return false;
            command = Manager.ApplyCoords();
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Confirm();
        }
    }

    public enum ChunkEditPurpose
    {
        Create,
        Move
    }
}