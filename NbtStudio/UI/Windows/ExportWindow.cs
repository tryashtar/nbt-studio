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
        public ExportWindow(IconSource source, ExportSettings template, string destination_path)
        {
            InitializeComponent();
            this.Icon = source.GetImage(IconType.Save).Icon;
            CompressionBox.Items.Add(new CompressionDisplay("Uncompressed", NbtCompression.None));
            CompressionBox.Items.Add(new CompressionDisplay("G-Zip", NbtCompression.GZip));
            CompressionBox.Items.Add(new CompressionDisplay("ZLib", NbtCompression.ZLib));
            CompressionBox.SelectedIndex = 0;
            if (template is not null)
            {
                RadioSnbt.Checked = template.Snbt;
                RadioNbt.Checked = !template.Snbt;
                CompressionBox.SelectedItem = CompressionBox.Items.Cast<CompressionDisplay>().FirstOrDefault(x => x.Compression == template.Compression);
                CheckMinify.Checked = template.Minified;
                CheckJson.Checked = template.Json;
                CheckLittleEndian.Checked = !template.BigEndian;
                CheckBedrockHeader.Checked = template.BedrockHeader;
            }
            else
            {
                string extension = Path.GetExtension(destination_path);
                bool? binary = NbtUtil.BinaryExtension(extension);
                RadioSnbt.Checked = binary == false;
                RadioNbt.Checked = binary == true;
                CheckLittleEndian.Checked = extension == ".mcstructure";
                CheckJson.Checked = extension == ".json";
            }
            SetEnables();
            Tooltips.SetToolTip(CheckLittleEndian, "Required for all Bedrock Edition files");
            Tooltips.SetToolTip(CheckBedrockHeader, "Required for Bedrock Edition level.dat files");
            Tooltips.SetToolTip(CheckJson, "Quotes all keys, removes type suffixes and list indicators");
        }

        public ExportSettings GetSettings()
        {
            if (RadioSnbt.Checked)
                return ExportSettings.AsSnbt(CheckMinify.Checked, CheckJson.Checked);
            else
                return ExportSettings.AsNbt(((CompressionDisplay)CompressionBox.SelectedItem).Compression, !CheckLittleEndian.Checked, CheckBedrockHeader.Checked);
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
            CheckBedrockHeader.Enabled = RadioNbt.Checked;
            CheckMinify.Enabled = RadioSnbt.Checked;
            CheckJson.Enabled = RadioSnbt.Checked;
            ButtonOk.Enabled = RadioNbt.Checked || RadioSnbt.Checked;
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