using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class ActionHistory : UserControl
    {
        private int Index = -1;
        private readonly Func<int, string> TextDisplay;
        public ActionHistory(IEnumerable<KeyValuePair<int, string>> history, Action<int> command, Func<int, string> display, Font font)
        {
            InitializeComponent();

            TextDisplay = display;
            this.Font = font;
            ActionList.Click += (s, e) => command(Index);
            SetItems(history.Select(x => x.Value));
            if (ActionList.Items.Count > 0)
                SetIndex(0);
        }

        private void SetItems(IEnumerable<string> text)
        {
            var graphics = ActionList.CreateGraphics();
            var text_array = text.ToArray();
            ActionList.Items.AddRange(text_array);
            int width = 0;
            int height = 0;
            foreach (var item in text_array)
            {
                var size = graphics.MeasureString(item, ActionList.Font);
                width = Math.Max(width, (int)Math.Ceiling(size.Width));
                height = Math.Max(height, (int)Math.Ceiling(size.Height));
            }
            var box_size = new Size(width + 20, height * Math.Min(10, text_array.Length));
            var full_size = new Size(box_size.Width, box_size.Height + InfoPanel.Height);
            ActionList.MinimumSize = box_size;
            ActionList.ItemHeight = Math.Max(1, height);
            this.MinimumSize = full_size;
            this.Size = full_size;
        }

        public void SetIndex(int index)
        {
            Index = index;
            ActionList.BeginUpdate();
            ActionList.ClearSelected();
            for (int i = 0; i <= Index; i++)
            {
                ActionList.SetSelected(i, true);
            }
            SelectedLabel.Text = TextDisplay(index);
            ActionList.EndUpdate();
        }

        private void ActionList_MouseMove(object sender, MouseEventArgs e)
        {
            int hovered = ActionList.IndexFromPoint(e.Location);
            int top = Math.Max(0, Math.Min(ActionList.TopIndex + e.Delta, ActionList.Items.Count - 1));
            if (hovered != -1 && hovered != Index)
                SetIndex(hovered);
            ActionList.TopIndex = top;
        }

        private void ActionList_MouseDown(object sender, MouseEventArgs e)
        {
            int clicked = ActionList.IndexFromPoint(e.Location);
            if (clicked != -1)
                ActionList.SetSelected(clicked, true);
        }

        private void ActionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // change selected color from ugly default
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, Constants.SelectionColor);

            e.DrawBackground();
            // draw text using normal ForeColor (black) instead of e.ForeColor (white)
            e.Graphics.DrawString(ActionList.Items[e.Index].ToString(), e.Font, new SolidBrush(ActionList.ForeColor), e.Bounds, StringFormat.GenericDefault);
        }
    }
}
