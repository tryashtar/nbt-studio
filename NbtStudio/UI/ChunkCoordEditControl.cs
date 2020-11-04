using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class ChunkCoordsEditControls
    {
        public readonly Chunk Chunk;
        public readonly RegionFile Region;
        public readonly ChunkCoordEditControl XBox;
        public readonly ChunkCoordEditControl ZBox;
        public ChunkCoordsEditControls(Chunk chunk, RegionFile region, ChunkCoordEditControl xbox, ChunkCoordEditControl zbox)
        {
            Chunk = chunk;
            Region = region;
            XBox = xbox;
            ZBox = zbox;
            XBox.Maximum = RegionFile.ChunkXDimension - 1;
            ZBox.Maximum = RegionFile.ChunkZDimension - 1;
            XBox.Value = Math.Min(Math.Max(chunk.X, XBox.Minimum), XBox.Maximum);
            ZBox.Value = Math.Min(Math.Max(chunk.Z, ZBox.Minimum), ZBox.Maximum);
            if (CheckCoordsInternal() != CoordCheckResult.Valid)
            {
                var auto = NbtUtil.GetAvailableCoords(region);
                if (auto.Any())
                {
                    var (x, y) = auto.First();
                    XBox.Value = x;
                    ZBox.Value = y;
                }
            }
            XBox.TextChanged += Box_TextChanged;
            ZBox.TextChanged += Box_TextChanged;
        }

        private CoordCheckResult CheckCoordsInternal()
        {
            int xval = XBox.GetCoord();
            int zval = ZBox.GetCoord();
            if (xval == Chunk.X && zval == Chunk.Z)
                return CoordCheckResult.Valid;
            bool x_out = xval < 0 || xval >= RegionFile.ChunkXDimension;
            bool z_out = zval < 0 || zval >= RegionFile.ChunkZDimension;
            if (x_out && z_out)
                return CoordCheckResult.InvalidBothOutOfBounds;
            else if (x_out)
                return CoordCheckResult.InvalidXOutOfBounds;
            else if (z_out)
                return CoordCheckResult.InvalidZOutOfBounds;

            if (Region == null)
                return CoordCheckResult.Valid;

            if (Region.GetChunk(xval, zval) != null)
                return CoordCheckResult.InvalidAlreadyTaken;

            return CoordCheckResult.Valid;
        }

        public bool CheckCoords()
        {
            var result = CheckCoordsInternal();
            bool valid = result == CoordCheckResult.Valid;
            SetColor(result);
            if (!valid)
            {
                ShowTooltip(result);
                if (result == CoordCheckResult.InvalidXOutOfBounds)
                    XBox.Select();
                else if (result == CoordCheckResult.InvalidZOutOfBounds)
                    ZBox.Select();
            }
            return valid;
        }

        public void ApplyCoords()
        {
            int xval = XBox.GetCoord();
            int zval = ZBox.GetCoord();
            Chunk.Move(xval, zval);
        }

        private void SetColor(CoordCheckResult result)
        {
            var bad_color = Color.FromArgb(255, 230, 230);
            switch (result)
            {
                case CoordCheckResult.InvalidXOutOfBounds:
                    XBox.SetBackColor(bad_color);
                    ZBox.RestoreBackColor();
                    break;
                case CoordCheckResult.InvalidZOutOfBounds:
                    ZBox.SetBackColor(bad_color);
                    XBox.RestoreBackColor();
                    break;
                case CoordCheckResult.InvalidAlreadyTaken:
                case CoordCheckResult.InvalidBothOutOfBounds:
                    XBox.SetBackColor(bad_color);
                    ZBox.SetBackColor(bad_color);
                    break;
                case CoordCheckResult.Valid:
                    XBox.RestoreBackColor();
                    ZBox.RestoreBackColor();
                    break;
            }
        }

        private void ShowTooltip(CoordCheckResult result)
        {
            var active = ZBox.Focused ? ZBox : XBox;
            if (result == CoordCheckResult.InvalidAlreadyTaken)
                active.ShowTooltip("Chunk Already Present", "There is already a chunk in the region at these coordinates", TimeSpan.FromSeconds(2));
            else if (result == CoordCheckResult.InvalidXOutOfBounds || result == CoordCheckResult.InvalidZOutOfBounds || result == CoordCheckResult.InvalidBothOutOfBounds)
                active.ShowTooltip("Out of Bounds", $"Chunk coordinates must be between 0 and {RegionFile.ChunkXDimension - 1}", TimeSpan.FromSeconds(2));
        }

        private void Box_TextChanged(object sender, EventArgs e)
        {
            SetColor(CheckCoordsInternal());
        }

        public enum CoordCheckResult
        {
            Valid,
            InvalidAlreadyTaken,
            InvalidXOutOfBounds,
            InvalidZOutOfBounds,
            InvalidBothOutOfBounds
        }
    }

    public class ChunkCoordEditControl : ConvenienceNumericUpDown
    {
        public int GetCoord()
        {
            return (int)this.Value;
        }
    }
}
