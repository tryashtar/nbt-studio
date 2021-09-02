using fNbt;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class FileNameTextBox : ConvenienceTextBox
    {
        private IHavePath Item;
        public FileNameTextBox()
        {
            this.TextChanged += FileNameTextBox_TextChanged;
        }

        private void FileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetColor(CheckNameInternal());
        }

        private void SetColor(NameCheckResult result)
        {
            switch (result)
            {
                case NameCheckResult.InvalidMissingName:
                case NameCheckResult.InvalidWhitespace:
                case NameCheckResult.InvalidCharacters:
                case NameCheckResult.InvalidAlreadyTaken:
                    SetBackColor(ConvenienceTextBox.ErrorColor);
                    break;
                case NameCheckResult.Valid:
                    RestoreBackColor();
                    break;
            }
        }

        private void ShowTooltip(NameCheckResult result)
        {
            if (result == NameCheckResult.InvalidMissingName || result == NameCheckResult.InvalidWhitespace)
                ShowTooltip("Missing Name", "You must specify a name for this file", TimeSpan.FromSeconds(2));
            else if (result == NameCheckResult.InvalidCharacters)
                ShowTooltip("Illegal Characters", "The name specified contains disallowed characters", TimeSpan.FromSeconds(2));
            else if (result == NameCheckResult.InvalidAlreadyTaken)
                ShowTooltip("File Already Exists", "A file with this name already exists", TimeSpan.FromSeconds(2));
        }

        public void SetItem(IHavePath item)
        {
            Item = item;
            this.Text = Path.GetFileName(item.Path);
        }

        public string GetName()
        {
            return this.Text.Trim();
        }

        private NameCheckResult CheckNameInternal()
        {
            var name = GetName();
            if (name == "")
                return NameCheckResult.InvalidMissingName;
            if (String.IsNullOrWhiteSpace(name))
                return NameCheckResult.InvalidWhitespace;
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return NameCheckResult.InvalidCharacters;
            string destination = GetDestination(name);
            if (name != Path.GetFileName(Item.Path) && (File.Exists(destination) || Directory.Exists(destination)))
                return NameCheckResult.InvalidAlreadyTaken;
            return NameCheckResult.Valid;
        }

        public bool CheckName()
        {
            var result = CheckNameInternal();
            bool valid = result == NameCheckResult.Valid;
            SetColor(result);
            if (!valid)
            {
                ShowTooltip(result);
                this.Select();
            }
            return valid;
        }

        public void PerformRename()
        {
            var name = GetName();
            if (name == Path.GetFileName(Item.Path))
                return;
            //Item.Move(GetDestination(name));
        }

        private string GetDestination(string name)
        {
            return Path.Combine(Path.GetDirectoryName(Item.Path), name);
        }

        public enum NameCheckResult
        {
            Valid,
            InvalidMissingName,
            InvalidWhitespace,
            InvalidCharacters,
            InvalidAlreadyTaken
        }
    }
}
