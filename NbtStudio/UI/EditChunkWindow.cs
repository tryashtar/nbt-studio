using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class EditChunkWindow : Form
    {
        private readonly Chunk WorkingChunk;
        private readonly RegionFile ChunkRegion;
        private readonly ChunkCoordsEditControls Manager;

        private EditChunkWindow(Chunk chunk, RegionFile region, ChunkEditPurpose purpose)
        {
            InitializeComponent();

            WorkingChunk = chunk;
            ChunkRegion = region;
            Manager = new ChunkCoordsEditControls(chunk, region, XBox, ZBox);

            this.Icon = Properties.Resources.chunk_icon;
            if (purpose == ChunkEditPurpose.Create)
                this.Text = $"Create Chunk";
            else if (purpose == ChunkEditPurpose.Move)
                this.Text = $"Move Chunk";

            XBox.Select();
        }

        public static Chunk CreateChunk(RegionFile parent, bool bypass_window = false, NbtCompound data = null)
        {
            var chunk = Chunk.EmptyChunk(data);

            if (bypass_window)
            {
                // find first available slot
                var available = NbtUtil.GetAvailableCoords(parent).FirstOrDefault();
                if (available == null)
                    return null;
                chunk.Move(available.Item1, available.Item2);
                return chunk;
            }
            else
            {
                var window = new EditChunkWindow(chunk, parent, ChunkEditPurpose.Create);
                return window.ShowDialog() == DialogResult.OK ? chunk : null;
            }
        }

        public static bool MoveChunk(Chunk existing)
        {
            var region = existing.Region;
            var window = new EditChunkWindow(existing, region, ChunkEditPurpose.Move);
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