using fNbt;
using NbtStudio;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class ExportWindow : Form
    {
        private bool BedrockHeader;
        public ExportWindow(ExportSettings template, string destination_path)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.action_save_icon;
            CompressionBox.Items.Add(new CompressionDisplay("Uncompressed", NbtCompression.None));
            CompressionBox.Items.Add(new CompressionDisplay("G-Zip", NbtCompression.GZip));
            CompressionBox.Items.Add(new CompressionDisplay("ZLib", NbtCompression.ZLib));
            CompressionBox.SelectedIndex = 0;
            if (template != null)
            {
                BedrockHeader = template.BedrockHeader;
                RadioSnbt.Checked = template.Snbt;
                CompressionBox.SelectedItem = CompressionBox.Items.Cast<CompressionDisplay>().FirstOrDefault(x => x.Compression == template.Compression);
                CheckMinify.Checked = template.Minified;
                CheckLittleEndian.Checked = !template.BigEndian;
            }
            else
            {
                RadioSnbt.Checked = Path.GetExtension(destination_path) == ".snbt";
                CheckLittleEndian.Checked = Path.GetExtension(destination_path) == ".mcstructure";
            }
            SetEnables();
        }

        public ExportSettings GetSettings()
        {
            if (RadioSnbt.Checked)
                return ExportSettings.AsSnbt(CheckMinify.Checked);
            else
                return ExportSettings.AsNbt(((CompressionDisplay)CompressionBox.SelectedItem).Compression, !CheckLittleEndian.Checked, BedrockHeader);
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
            CompressionBox.Enabled = RadioNbt.Checked;
            CheckLittleEndian.Enabled = RadioNbt.Checked;
            CheckMinify.Enabled = RadioSnbt.Checked;
        }

        private class CompressionDisplay
        {
            public readonly string Name;
            public readonly NbtCompression Compression;
            public CompressionDisplay(string name, NbtCompression compression)
            {
                Name = name;
                Compression = compression;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}