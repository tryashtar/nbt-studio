using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class TagNameTextBox : ConvenienceTextBox
    {
        private NbtTag NbtTag;
        private NbtContainerTag NbtParent;
        public TagNameTextBox()
        {
            this.TextChanged += TagNameTextBox_TextChanged;
        }

        private void TagNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetColor(CheckNameInternal(out _));
        }

        private void SetColor(NameCheckResult result)
        {
            switch (result)
            {
                case NameCheckResult.InvalidMissingName:
                case NameCheckResult.InvalidHasName:
                case NameCheckResult.InvalidDuplicateName:
                    SetBackColor(ConvenienceTextBox.ErrorColor);
                    break;
                case NameCheckResult.Valid:
                    RestoreBackColor();
                    break;
            }
        }

        private void ShowTooltip(NameCheckResult result)
        {
            if (result == NameCheckResult.InvalidMissingName)
                ShowTooltip("Missing Name", "You must specify a name for this tag", TimeSpan.FromSeconds(2));
            else if (result == NameCheckResult.InvalidHasName)
                ShowTooltip("Name Disallowed", "This tag must not have a name", TimeSpan.FromSeconds(2));
            else if (result == NameCheckResult.InvalidDuplicateName)
                ShowTooltip("Duplicate Name", "The compound already contains a tag with this name", TimeSpan.FromSeconds(2));
        }

        public void SetTags(NbtTag tag, NbtContainerTag parent)
        {
            NbtTag = tag;
            NbtParent = parent;
            this.Text = tag?.Name;
        }

        public string GetName()
        {
            return this.Text.Trim();
        }

        private NameCheckResult CheckNameInternal(out string name)
        {
            name = GetName();
            if (NbtParent is null)
                return NameCheckResult.Valid;
            if (NbtParent is NbtList)
                return name == "" ? NameCheckResult.Valid : NameCheckResult.InvalidHasName;
            if (NbtParent is NbtCompound compound)
            {
                if (name == "")
                    return NameCheckResult.InvalidMissingName;
                if (compound.Contains(name) && !(NbtTag is not null && name == NbtTag.Name))
                    return NameCheckResult.InvalidDuplicateName;
            }
            return NameCheckResult.Valid;
        }

        public bool CheckName() => CheckName(out _);
        public bool CheckName(out string name)
        {
            var result = CheckNameInternal(out name);
            bool valid = result == NameCheckResult.Valid;
            SetColor(result);
            if (!valid)
            {
                ShowTooltip(result);
                this.Select();
            }
            return valid;
        }

        public ICommand ApplyName()
        {
            var name = GetName();
            if (name == "")
                name = null;
            if (NbtTag.Name != name)
                return new RenameCommand(NbtTag, name);
            return null;
        }

        public enum NameCheckResult
        {
            Valid,
            InvalidMissingName,
            InvalidHasName,
            InvalidDuplicateName
        }
    }
}
