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
            var chunk = new Chunk(data);
            var window = new EditChunkWindow(source, chunk, ChunkEditPurpose.Create);
            return window.ShowDialog() == DialogResult.OK ? chunk : null;
        }

        public static bool MoveChunk(IconSource source, ChunkEntry existing)
        {
            var window = new EditChunkWindow(source, existing, ChunkEditPurpose.Move);
            return window.ShowDialog() == DialogResult.OK; // window moves the chunk by itself
        }

        private void Confirm()
        {
            if (TryModify())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool TryModify()
        {
            if (!Manager.CheckCoords())
                return false;
            Manager.ApplyCoords();
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