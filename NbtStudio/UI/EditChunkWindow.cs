using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class EditChunkWindow : Form
    {
        private readonly IChunk WorkingChunk;
        private readonly IRegion ChunkRegion;
        private readonly ChunkCoordsEditControls Manager;

        private EditChunkWindow(IChunk chunk, IRegion region, ChunkEditPurpose purpose)
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

            XBox.Maximum = RegionFile.ChunkXDimension - 1;
            ZBox.Maximum = RegionFile.ChunkZDimension - 1;
            XBox.Value = Math.Min(Math.Max(chunk.X, XBox.Minimum), XBox.Maximum);
            ZBox.Value = Math.Min(Math.Max(chunk.Z, ZBox.Minimum), ZBox.Maximum);
            XBox.Select();
        }

        public static IChunk CreateChunk(IRegion parent, bool bypass_window = false)
        {
            var chunk = Chunk.EmptyChunk();

            if (bypass_window)
            {
                // find first available slot
                for (int x = 0; x < RegionFile.ChunkXDimension; x++)
                {
                    for (int z = 0; z < RegionFile.ChunkZDimension; z++)
                    {
                        if (parent.GetChunk(x, z) == null)
                        {
                            chunk.Move(x, z);
                            return chunk;
                        }
                    }
                }
                return null;
            }
            else
            {
                var window = new EditChunkWindow(chunk, parent, ChunkEditPurpose.Create);
                return window.ShowDialog() == DialogResult.OK ? chunk : null;
            }
        }

        public static bool MoveChunk(IChunk existing)
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