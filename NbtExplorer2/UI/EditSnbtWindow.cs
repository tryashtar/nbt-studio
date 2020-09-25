using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditSnbtWindow : Form
    {
        public EditSnbtWindow(string text, NbtTagType? required = null)
        {
            InitializeComponent();

            RequiredType = required;
            InputBox.Text = text;
            this.Icon = required == null ? Properties.Resources.action_edit_snbt_icon : NbtUtil.TagTypeIcon(required.Value);
        }

        private readonly NbtTagType? RequiredType;
        public NbtTag NbtValue { get; private set; }

        private void Apply()
        {
            if (ValidateSnbtInput())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ValidateSnbtInput()
        {
            try
            {
                NbtValue = SnbtParser.Parse(InputBox.Text, named: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The SNBT is not valid:\n{ex.Message}");
                return false;
            }
            if (RequiredType != null && NbtValue.TagType != RequiredType)
            {
                MessageBox.Show($"The SNBT must be of type {RequiredType}, not {NbtValue.TagType}");
                return false;
            }
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void MinifyCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                NbtValue = SnbtParser.Parse(InputBox.Text, named: false);
                InputBox.Text = NbtValue.Adapt().ToSnbt(!MinifyCheck.Checked);
            }
            catch
            {
                // change it back
                MinifyCheck.CheckedChanged -= MinifyCheck_CheckedChanged;
                MinifyCheck.Checked ^= true;
                MinifyCheck.CheckedChanged += MinifyCheck_CheckedChanged;
            }
        }
    }
}