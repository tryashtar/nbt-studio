using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class DualMenuItem
    {
        private readonly ToolStripMenuItem MenuItem;
        private readonly ToolStripButton Button;
        public EventHandler Click;
        private bool _Enabled = true;
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                _Enabled = value;
                MenuItem.Enabled = value;
                Button.Enabled = value;
            }
        }
        private Image _Image;
        public Image Image
        {
            get => _Image;
            set
            {
                _Image = value;
                MenuItem.Image = value;
                Button.Image = value;
            }
        }

        public DualMenuItem(string text, string hover, Image image, Keys shortcut)
        {
            _Image = image;
            MenuItem = Single(text, image, shortcut);
            Button = Single(hover, image);
            MenuItem.Click += (s, e) => Click?.Invoke(s, e);
            Button.Click += (s, e) => Click?.Invoke(s, e);
        }

        public static ToolStripMenuItem Single(string text, Image image, Keys shortcut)
        {
            var item = new ToolStripMenuItem(text, image);
            item.ShortcutKeys = shortcut;
            return item;
        }

        public static ToolStripButton Single(string hover, Image image)
        {
            var item = new ToolStripButton(hover, image);
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            return item;
        }

        public void AddTo(ToolStrip strip, ToolStripMenuItem menu)
        {
            menu.DropDownItems.Add(MenuItem);
            strip.Items.Add(Button);
        }
    }
}
