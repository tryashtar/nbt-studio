using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class ConvenienceTextBox : TextBox
    {
        private Color TrueBackColor;
        private bool ChangingBackColor;
        private ToolTip Tooltip;
        public ConvenienceTextBox()
        { }

        public void SetBackColor(Color color)
        {
            ChangingBackColor = true;
            this.BackColor = color;
            ChangingBackColor = false;
        }

        public void HideTooltip()
        {
            if (Tooltip != null)
                Tooltip.Hide(this);
        }

        public void ShowTooltip(string title, string text, TimeSpan duration)
        {
            HideTooltip();
            Tooltip = new ToolTip()
            {
                IsBalloon = false,
                UseFading = true,
                UseAnimation = true
            };
            Tooltip.Show(String.Empty, this, 0); // a bug apparently
            Tooltip.ToolTipTitle = title;
            Tooltip.Show(text, this, (int)duration.TotalMilliseconds);
        }

        public void RestoreBackColor() => SetBackColor(TrueBackColor);

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (!ChangingBackColor)
                TrueBackColor = this.BackColor;
            base.OnBackColorChanged(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            HideTooltip();
            base.OnLostFocus(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            HideTooltip();
            base.OnTextChanged(e);
        }
    }
}
