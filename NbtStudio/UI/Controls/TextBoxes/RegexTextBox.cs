using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class RegexTextBox : ConvenienceTextBox
    {
        // cache results of regex parsing, so calling IsMatch repeatedly doesn't pointlessly re-parse
        private Regex LastRegex = null;
        private bool _RegexMode = false;
        public bool RegexMode
        {
            get => _RegexMode;
            set
            {
                _RegexMode = value;
                if (value)
                    SetColor(CheckRegexInternal(out _));
                else
                {
                    RestoreBackColor();
                    HideTooltip();
                }
            }
        }
        public RegexTextBox()
        {
            this.TextChanged += TagNameTextBox_TextChanged;
        }

        private void TagNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RegexMode)
                SetColor(CheckRegexInternal(out LastRegex));
        }

        private void SetColor(Exception exception)
        {
            if (exception is null)
                RestoreBackColor();
            else
                SetBackColor(Color.FromArgb(255, 230, 230));
        }

        private void ShowTooltip(Exception exception)
        {
            if (exception is not null)
            {
                string message = exception.Message;
                string redundant = $"\"{this.Text}\" - ";
                int index = message.IndexOf(redundant);
                if (index != -1)
                    message = message[(index + redundant.Length)..];
                ShowTooltip("Regex Parsing Error", message, TimeSpan.FromSeconds(3));
            }
        }

        public static bool IsMatch(string input, string search)
        {
            if (search == "")
                return true;
            if (input is null)
                return false;
            return input.IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static bool IsMatchRegex(string input, Regex search)
        {
            if (search is null)
                return false;
            if (input is null)
                return false;
            return search.IsMatch(input);
        }

        public bool IsMatch(string input)
        {
            if (RegexMode)
            {
                if (LastRegex is null)
                    CheckRegexInternal(out LastRegex);
                return IsMatchRegex(input, LastRegex);
            }
            else
                return IsMatch(input, this.Text);
        }

        public Regex ReparseRegex()
        {
#if DEBUG
            Console.WriteLine($"Parsing new regex: \"{this.Text}\"");
#endif
            return new Regex(this.Text, RegexOptions.IgnoreCase);
        }

        private Exception CheckRegexInternal(out Regex regex)
        {
            regex = null;
            try
            { regex = ReparseRegex(); }
            catch (Exception ex) { return ex; }
            return null;
        }

        public bool CheckRegexQuiet(out Regex regex)
        {
            if (!RegexMode)
            {
                regex = null;
                return true;
            }
            var error = CheckRegexInternal(out regex);
            SetColor(error);
            return error is null;
        }

        public bool CheckRegex(out Regex regex)
        {
            if (!RegexMode)
            {
                regex = null;
                return true;
            }
            var error = CheckRegexInternal(out regex);
            bool valid = error is null;
            SetColor(error);
            if (!valid)
            {
                ShowTooltip(error);
                this.Select();
            }
            return valid;
        }
    }
}
