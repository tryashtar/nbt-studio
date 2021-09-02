using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class ConvenienceManager
    {
        public readonly Control Control;
        private Color TrueBackColor;
        private bool ChangingBackColor;
        private ToolTip Tooltip;
        public ConvenienceManager(Control control)
        {
            Control = control;
            Control.BackColorChanged += Control_BackColorChanged;
            Control.LostFocus += Control_LostFocus;
            Control.TextChanged += Control_TextChanged;
        }

        public void SetBackColor(Color color)
        {
            ChangingBackColor = true;
            Control.BackColor = color;
            ChangingBackColor = false;
        }

        public void HideTooltip()
        {
            if (Tooltip is not null)
                Tooltip.Hide(Control);
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
            Tooltip.Show(String.Empty, Control, 0); // a bug apparently
            Tooltip.ToolTipTitle = title;
            Tooltip.Show(text, Control, (int)duration.TotalMilliseconds);
        }

        public void RestoreBackColor() => SetBackColor(TrueBackColor);

        private void Control_BackColorChanged(object sender, EventArgs e)
        {
            if (!ChangingBackColor)
                TrueBackColor = Control.BackColor;
        }

        private void Control_TextChanged(object sender, EventArgs e)
        {
            HideTooltip();
        }

        private void Control_LostFocus(object sender, EventArgs e)
        {
            HideTooltip();
        }
    }

    public class ConvenienceTextBox : TextBox
    {
        private readonly ConvenienceManager Convenience;
        public static readonly Color ErrorColor = Color.FromArgb(255, 230, 230);
        public ConvenienceTextBox()
        {
            Convenience = new ConvenienceManager(this);
        }

        public void SetBackColor(Color color) => Convenience.SetBackColor(color);
        public void HideTooltip() => Convenience.HideTooltip();
        public void ShowTooltip(string title, string text, TimeSpan duration) => Convenience.ShowTooltip(title, text, duration);
        public void RestoreBackColor() => Convenience.RestoreBackColor();
    }

    public class ConvenienceNumericUpDown : NumericUpDown
    {
        private readonly ConvenienceManager Convenience;
        public ConvenienceNumericUpDown()
        {
            Convenience = new ConvenienceManager(this);
        }

        public void SetBackColor(Color color) => Convenience.SetBackColor(color);
        public void HideTooltip() => Convenience.HideTooltip();
        public void ShowTooltip(string title, string text, TimeSpan duration) => Convenience.ShowTooltip(title, text, duration);
        public void RestoreBackColor() => Convenience.RestoreBackColor();
    }
}
