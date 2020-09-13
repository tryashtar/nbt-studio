using fNbt;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class CreateTagWindow : Form
    {
        private readonly NbtTag ParentTag;
        private readonly NbtTagType Type;
        private readonly bool IsValueType;
        private readonly bool IsSizeType;
        private readonly bool HasName;

        public NbtTag CreatedTag { get; private set; }

        public CreateTagWindow(NbtTagType type, NbtTag parent)
        {
            InitializeComponent();

            Type = type;
            ParentTag = parent;
            HasName = parent is NbtCompound;

            NameLabel.Visible = HasName;
            NameBox.Visible = HasName;

            IsValueType = Util.IsValueType(type);
            ValueLabel.Visible = IsValueType;
            ValueBox.Visible = IsValueType;

            IsSizeType = Util.IsSizeType(type);
            SizeLabel.Visible = IsSizeType;
            SizeBox.Visible = IsSizeType;

            this.Icon = Util.TagTypeIcon(type);
            this.Text = $"Create {Util.TagTypeName(type)} Tag";
        }

        private void Confirm()
        {
            if (TryCreate(out var tag))
            {
                DialogResult = DialogResult.OK;
                CreatedTag = tag;
                if (ParentTag is NbtCompound compound)
                    compound.Add(tag);
                else if (ParentTag is NbtList list)
                    list.Add(tag);
                Close();
            }
        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    if (keyData==Keys.Escape)
        //    {
        //        Close();
        //        return true;
        //    }
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}

        private bool TryCreate(out NbtTag tag)
        {
            tag = null;
            try
            {
                var name = HasName ? NameBox.Text.Trim() : null;
                var value = IsSizeType ? SizeBox.Text.Trim() : ValueBox.Text.Trim();
                if (name == "")
                {
                    MessageBox.Show("The name cannot be empty");
                    return false;
                }
                if (ParentTag is NbtCompound compound && compound.Contains(name))
                {
                    MessageBox.Show($"Duplicate name; this compound already contains a tag named \"{name}\"");
                    return false;
                }
                if (value == "")
                    tag = Util.CreateTag(Type, name);
                else
                    tag = Util.CreateTag(Type, name, value);
                return true;
            }
            catch (FormatException)
            {
                if (IsValueType)
                    MessageBox.Show($"The value is formatted incorrectly for a {Util.TagTypeName(Type).ToLower()}");
                else if (IsSizeType)
                    MessageBox.Show($"The size is formatted incorrectly");
            }
            catch (OverflowException)
            {
                if (IsValueType)
                {
                    var minmax = Util.MinMaxFor(Type);
                    MessageBox.Show($"The value for {Util.TagTypeName(Type).ToLower()}s must be between {minmax.Item1} and {minmax.Item2}");
                }
                else if (IsSizeType)
                    MessageBox.Show($"The value for size must be between {int.MaxValue} and {int.MinValue}");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show($"The size cannot be less than zero");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Confirm();
        }

        private void CreateTagWindow_Load(object sender, EventArgs e)
        {
            if (!HasName && !IsValueType && !IsSizeType)
            {
                this.Visible = false;
                Confirm();
            }
        }
    }
}