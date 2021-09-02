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
    public class TagSnbtTextBox : ConvenienceTextBox
    {
        public NbtTagType? RequiredType;
        public bool HighlightsErrors = false; // set to true to parse and highlight as you type
        public TagSnbtTextBox()
        {
            this.TextChanged += TagSnbtTextBox_TextChanged;
        }

        private void TagSnbtTextBox_TextChanged(object sender, EventArgs e)
        {
            if (HighlightsErrors)
                SetColor(CheckTagInternal(out _));
            else
                RestoreBackColor();
        }

        private void SetColor(ISnbtCheckResult result)
        {
            if (result.IsValid)
                RestoreBackColor();
            else if (result is SnbtInvalidFormat or SnbtInvalidWrongType)
                SetBackColor(ConvenienceTextBox.ErrorColor);
        }

        private void ShowTooltip(ISnbtCheckResult result)
        {
            ShowTooltip(result.Title, result.Description, TimeSpan.FromSeconds(3));
        }

        public void SetFromTag(NbtTag tag)
        {
            this.Text = tag.ToSnbt(SnbtOptions.DefaultExpanded);
        }

        public string GetValueText()
        {
            return this.Text.Trim().Replace("\r", "");
        }

        private NbtTag ParseTag()
        {
            var text = GetValueText();
            return SnbtParser.Parse(text, named: false);
        }

        private ISnbtCheckResult CheckTagInternal(out NbtTag tag)
        {
            tag = null;
            try
            { tag = ParseTag(); }
            catch (Exception ex)
            { return new SnbtInvalidFormat(ex); }
            if (RequiredType is not null && tag.TagType != RequiredType.Value)
                return new SnbtInvalidWrongType(RequiredType.Value, tag.TagType);
            return new SnbtValid();
        }

        public bool CheckTag(out NbtTag tag)
        {
            var result = CheckTagInternal(out tag);
            bool valid = result.IsValid;
            SetColor(result);
            if (!valid)
            {
                ShowTooltip(result);
                this.Select();
            }
            return valid;
        }

        public bool TryMinify(bool minified)
        {
            CheckTag(out var tag); // continue to minify even if the required type is not met
            if (tag is not null)
            {
                var options = SnbtOptions.Default;
                if (!minified)
                    options = options.Expanded();
                this.Text = tag.ToSnbt(options);
                return true;
            }
            return false;
        }

        private interface ISnbtCheckResult
        {
            string Title { get; }
            string Description { get; }
            bool IsValid { get; }
        }

        private class SnbtInvalidFormat : ISnbtCheckResult
        {
            public string Title => "Invalid Format";
            public string Description { get; private set; }
            public bool IsValid => false;
            public SnbtInvalidFormat(Exception exception)
            {
                Description = $"Failed to parse the SNBT:\n{exception.Message}";
            }
        }

        private class SnbtInvalidWrongType : ISnbtCheckResult
        {
            private readonly NbtTagType Required;
            private readonly NbtTagType Found;
            public SnbtInvalidWrongType(NbtTagType required, NbtTagType found) { Required = required; Found = found; }
            public string Title => "Wrong Type";
            public string Description => $"The SNBT must be of type {Required}, not {Found}";
            public bool IsValid => false;
        }

        private class SnbtValid : ISnbtCheckResult
        {
            public string Title => "Valid";
            public string Description => "The SNBT parsed successfully";
            public bool IsValid => true;
        }
    }
}
