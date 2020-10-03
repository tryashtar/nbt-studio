using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class TagNameTextBox : TextBox
    {
        private INbtTag NbtTag;
        private INbtContainer NbtParent;
        private Color TrueBackColor;
        private bool ChangingBackColor;
        public TagNameTextBox()
        {
            this.BackColorChanged += NameTextBox_BackColorChanged;
            this.TextChanged += NameTextBox_TextChanged;
        }

        private void NameTextBox_BackColorChanged(object sender, EventArgs e)
        {
            if (!ChangingBackColor)
                TrueBackColor = this.BackColor;
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetColor(CheckNameInternal());
        }

        private void SetColor(NameCheckResult result)
        {
            ChangingBackColor = true;
            switch (result)
            {
                case NameCheckResult.InvalidMissingName:
                case NameCheckResult.InvalidHasName:
                    this.BackColor = Color.FromArgb(255, 230, 230);
                    break;
                case NameCheckResult.InvalidDuplicateName:
                    this.BackColor = Color.FromArgb(255, 230, 230);
                    break;
                case NameCheckResult.Valid:
                    if (this.BackColor != TrueBackColor)
                        this.BackColor = TrueBackColor;
                    break;
                default:
                    break;
            }
            ChangingBackColor = false;
        }

        public void SetTags(INbtTag tag, INbtContainer parent)
        {
            NbtTag = tag;
            NbtParent = parent;
        }

        public string GetName()
        {
            return this.Text.Trim();
        }

        private NameCheckResult CheckNameInternal()
        {
            if (NbtTag == null || NbtParent == null)
                return NameCheckResult.Valid;
            var name = GetName();
            if (NbtParent is INbtList)
                return name == "" ? NameCheckResult.Valid : NameCheckResult.InvalidHasName;
            if (NbtParent is INbtCompound compound)
            {
                if (name == "")
                    return NameCheckResult.InvalidMissingName;
                if (name != NbtTag.Name && compound.Contains(name))
                    return NameCheckResult.InvalidDuplicateName;
            }
            return NameCheckResult.Valid;
        }

        public bool CheckName()
        {
            var result = CheckNameInternal();
            bool valid = result == NameCheckResult.Valid;
            SetColor(result);
            if (!valid)
                this.Select();
            return valid;
        }

        public void ApplyName()
        {
            var name = GetName();
            if (name == "")
                name = null;
            NbtTag.Name = name;
        }

        private enum NameCheckResult
        {
            Valid,
            InvalidMissingName,
            InvalidHasName,
            InvalidDuplicateName
        }
    }
}
