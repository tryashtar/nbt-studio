using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Nbt;

namespace NbtStudio.UI
{
    public class NbtPathTextBox : ConvenienceTextBox
    {
        public NbtPathTextBox()
        {
            this.TextChanged += TagNameTextBox_TextChanged;
        }

        private void TagNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetColor(CheckPathInternal(out _));
        }

        private void SetColor(Exception exception)
        {
            if (exception is null)
                RestoreBackColor();
            else
                SetBackColor(ConvenienceTextBox.ErrorColor);
        }

        private void ShowTooltip(Exception exception)
        {
            if (exception is not null)
                ShowTooltip("NBT Path Parsing Error", exception.Message, TimeSpan.FromSeconds(3));
        }

        public NbtPath GetPath()
        {
            return NbtPath.Parse(this.Text);
        }

        private Exception CheckPathInternal(out NbtPath path)
        {
            path = null;
            try
            { path = GetPath(); }
            catch (Exception ex) { return ex; }
            return null;
        }

        public bool CheckPath(out NbtPath path)
        {
            var error = CheckPathInternal(out path);
            bool valid = error is null;
            SetColor(error);
            if (!valid)
            {
                ShowTooltip(error);
                this.Select();
            }
            return valid;
        }
    }
}
