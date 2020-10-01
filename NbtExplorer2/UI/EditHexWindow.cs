using Be.Windows.Forms;
using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class EditHexWindow : Form
    {
        private readonly INbtTag WorkingTag;
        private readonly INbtContainer TagParent;
        private readonly bool SettingName;
        private readonly IByteTransformer Provider;

        private EditHexWindow(INbtTag tag, INbtContainer parent, bool set_name, EditPurpose purpose)
        {
            InitializeComponent();
            TabView.Size = new Size(0, 0);

            WorkingTag = tag;
            TagParent = parent;

            SettingName = set_name;
            NameLabel.Visible = SettingName;
            NameBox.Visible = SettingName;

            Provider = ByteProviders.GetByteProvider(tag);
            HexBox.ByteProvider = Provider;
            HexBox.GroupSize = Provider.BytesPerValue;
            HexBox.GroupSeparatorVisible = Provider.BytesPerValue > 1;
            HexBox.SelectionBackColor = Color.LightBlue;
            HexBox.SelectionForeColor = HexBox.ForeColor;

            string tagname;
            if (tag is INbtList list)
            {
                tagname = NbtUtil.TagTypeName(list.ListType) + " List";
                this.Icon = NbtUtil.TagTypeIcon(list.ListType);
            }
            else
            {
                tagname = NbtUtil.TagTypeName(tag.TagType);
                this.Icon = NbtUtil.TagTypeIcon(tag.TagType);
            }
            if (purpose == EditPurpose.Create)
                this.Text = $"Create {tagname} Tag";
            else if (purpose == EditPurpose.EditValue || purpose == EditPurpose.Rename)
            {
                this.Text = $"Edit {tagname} Tag";
                NameBox.Text = tag.Name;
            }

            if (SettingName && purpose != EditPurpose.EditValue)
            {
                NameBox.Select();
                NameBox.SelectAll();
            }
            else
                HexBox.Select();
        }

        public static NbtTag CreateTag(NbtTagType type, INbtContainer parent, bool bypass_window = false)
        {
            bool has_name = parent is INbtCompound;
            var tag = NbtUtil.CreateTag(type);

            if (bypass_window)
            {
                if (has_name)
                    tag.Name = NbtUtil.GetAutomaticName(tag.Adapt(), (INbtCompound)parent);
                return tag;
            }
            var window = new EditHexWindow(tag.Adapt(), parent, has_name, EditPurpose.Create);
            return window.ShowDialog() == DialogResult.OK ? tag : null;
        }

        public static bool ModifyTag(INbtTag existing, EditPurpose purpose)
        {
            if (purpose == EditPurpose.Create)
                throw new ArgumentException("Use CreateTag to create tags");
            var parent = existing.Parent;
            bool has_name = parent is INbtCompound;

            var window = new EditHexWindow(existing, parent, has_name, purpose);
            return window.ShowDialog() == DialogResult.OK; // window modifies the tag by itself
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
            var name = NameBox.Text.Trim();

            // check conditions first, tag must not be modified at ALL until we can be sure it's safe
            if (SettingName)
            {
                if (name == "")
                {
                    MessageBox.Show("The name cannot be empty");
                    return false;
                }
                if (TagParent is INbtCompound compound && name != WorkingTag.Name && compound.Contains(name))
                {
                    MessageBox.Show($"Duplicate name; this compound already contains a tag named \"{name}\"");
                    return false;
                }
            }
            if (SettingName)
                WorkingTag.Name = name;

            HexBox.ByteProvider.ApplyChanges();
            return true;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (TabView.SelectedTab == TextPage)
            {
                Provider.SetBytes(ConvertFromText(TextBox.Text, Provider.BytesPerValue));
            }
            Confirm();
        }

        private void UpdateCursorLabel()
        {
            long selected_byte = HexBox.SelectionStart;
            CursorLabel.Text = $"Element {selected_byte / Provider.BytesPerValue}";
        }

        private void HexBox_CurrentLineChanged(object sender, EventArgs e)
        {
            UpdateCursorLabel();
        }

        private void HexBox_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            UpdateCursorLabel();
        }

        private void HexBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                HexBox.SelectAll();
                e.Handled = true;
            }
        }

        private void TabView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabView.SelectedTab == HexPage)
            {
                Provider.SetBytes(ConvertFromText(TextBox.Text, Provider.BytesPerValue));
            }
            else if (TabView.SelectedTab == TextPage)
            {
                TextBox.Text = ConvertToText(Provider);
            }
        }

        private string ConvertToText(IByteTransformer provider)
        {
            var bytes = provider.CurrentBytes.ToArray();
            int size = provider.BytesPerValue;
            if (size == sizeof(byte))
                return String.Join(" ", bytes.Select(x => (sbyte)x));
            if (size == sizeof(short))
                return String.Join(" ", Util.ToShortArray(bytes));
            if (size == sizeof(int))
                return String.Join(" ", Util.ToIntArray(bytes));
            if (size == sizeof(long))
                return String.Join(" ", Util.ToLongArray(bytes));
            throw new ArgumentException($"Can't convert bytes to a numeric type with size {size}");
        }

        private byte[] ConvertFromText(string text, int size)
        {
            string[] vals = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries); // whitespace as delimiter
            if (size == sizeof(byte))
                return vals.Select(ParseByte).Select(x => (byte)x).ToArray();
            if (size == sizeof(short))
                return Util.ToByteArray(vals.Select(ParseShort).ToArray());
            if (size == sizeof(int))
                return Util.ToByteArray(vals.Select(ParseInt).ToArray());
            if (size == sizeof(long))
                return Util.ToByteArray(vals.Select(ParseLong).ToArray());
            throw new ArgumentException($"Can't convert bytes to a numeric type with size {size}");
        }

        private sbyte ParseByte(string text)
        {
            if (sbyte.TryParse(text, out sbyte val))
                return val;
            return 0;
        }

        private short ParseShort(string text)
        {
            if (short.TryParse(text, out short val))
                return val;
            return 0;
        }

        private int ParseInt(string text)
        {
            if (int.TryParse(text, out int val))
                return val;
            return 0;
        }

        private long ParseLong(string text)
        {
            if (long.TryParse(text, out long val))
                return val;
            return 0;
        }

        private void EditHexWindow_Load(object sender, EventArgs e)
        {
            TabView_SelectedIndexChanged(sender, e);
        }
    }
}