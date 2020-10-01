using fNbt;
using NbtExplorer2;
using System;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class ExportWindow : Form
    {
        private readonly byte[] Header;
        public ExportWindow(ExportSettings template)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.action_save_icon;
            if (template != null)
            {
                Header = template.Header;
                RadioSnbt.Checked = template.Snbt;
                CheckGzip.Checked = template.Compression == NbtCompression.GZip;
                CheckMinify.Checked = template.Minified;
                CheckLittleEndian.Checked = !template.BigEndian;
            }
            else
                Header = new byte[0];
            SetEnables();
        }

        public ExportSettings GetSettings()
        {
            if (RadioSnbt.Checked)
                return ExportSettings.AsSnbt(CheckMinify.Checked);
            else
                return ExportSettings.AsNbt(CheckGzip.Checked ? NbtCompression.GZip : NbtCompression.None, !CheckLittleEndian.Checked, Header);
        }

        private void Apply()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void RadioNbt_CheckedChanged(object sender, EventArgs e)
        {
            SetEnables();
        }

        private void SetEnables()
        {
            CheckGzip.Enabled = RadioNbt.Checked;
            CheckLittleEndian.Enabled = RadioNbt.Checked;
            CheckMinify.Enabled = RadioSnbt.Checked;
        }
    }
}