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
        public event EventHandler Click;
        public ToolStripDropDown DropDown
        {
            get => MenuItem.DropDown;
            set => MenuItem.DropDown = value;
        }
        public ToolStripItemCollection DropDownItems => MenuItem.DropDownItems;
        public Font Font => MenuItem?.Font;
        private bool _Enabled = true;
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                _Enabled = value;
                if (MenuItem != null)
                    MenuItem.Enabled = value;
                if (Button != null)
                    Button.Enabled = value;
            }
        }
        private bool _Visible = true;
        public bool Visible
        {
            get => _Visible;
            set
            {
                _Visible = value;
                if (MenuItem != null)
                    MenuItem.Visible = value;
                if (Button != null)
                    Button.Visible = value;
            }
        }
        private Func<IconSource, ImageIcon> _ImageGetter;
        public Func<IconSource, ImageIcon> ImageGetter
        {
            get => _ImageGetter;
            set
            {
                _ImageGetter = value;
                if (_IconSource != null && _ImageGetter != null)
                {
                    var image = _ImageGetter(_IconSource).Image;
                    if (MenuItem != null)
                        MenuItem.Image = image;
                    if (Button != null)
                        Button.Image = image;
                }
            }
        }
        private IconSource _IconSource;
        public IconSource IconSource
        {
            get => _IconSource;
            set
            {
                _IconSource = value;
                if (_IconSource != null && _ImageGetter != null)
                {
                    var image = _ImageGetter(_IconSource).Image;
                    if (MenuItem != null)
                        MenuItem.Image = image;
                    if (Button != null)
                        Button.Image = image;
                }
            }
        }

        public DualMenuItem(string text, string hover, Func<IconSource, ImageIcon> image, Keys shortcut)
        {
            _ImageGetter = image;
            MenuItem = CreateMenuItem(text, shortcut);
            Button = CreateButton(hover);
            MenuItem.Click += (s, e) => Click?.Invoke(s, e);
            Button.Click += (s, e) => Click?.Invoke(s, e);
        }

        private DualMenuItem(ToolStripMenuItem menu, ToolStripButton button)
        {
            MenuItem = menu;
            Button = button;
        }

        public static DualMenuItem SingleMenuItem(string text, Func<IconSource, ImageIcon> image, Keys shortcut)
        {
            var menu = CreateMenuItem(text, shortcut);
            var item = new DualMenuItem(menu, null);
            item._ImageGetter = image;
            menu.Click += (s, e) => item.Click?.Invoke(s, e);
            return item;
        }

        public static DualMenuItem SingleButton(string hover, Func<IconSource, ImageIcon> image)
        {
            var button = CreateButton(hover);
            var item = new DualMenuItem(null, button);
            item._ImageGetter = image;
            button.Click += (s, e) => item.Click?.Invoke(s, e);
            return item;
        }

        private static ToolStripMenuItem CreateMenuItem(string text, Keys shortcut)
        {
            var item = new ToolStripMenuItem(text);
            item.ShortcutKeys = shortcut;
            return item;
        }

        private static ToolStripButton CreateButton(string hover)
        {
            var item = new ToolStripButton(hover);
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            return item;
        }

        public void AddTo(ToolStrip strip, ToolStripMenuItem menu)
        {
            menu.DropDownItems.Add(MenuItem);
            strip.Items.Add(Button);
        }

        public void AddToToolStrip(ToolStrip strip)
        {
            strip.Items.Add(Button);
        }

        public void AddToMenuItem(ToolStripMenuItem menu)
        {
            menu.DropDownItems.Add(MenuItem);
        }

        public void AddToMenuStrip(MenuStrip strip)
        {
            strip.Items.Add(MenuItem);
        }
    }

    public class DualItemCollection
    {
        private readonly List<DualMenuItem> Items;
        public DualItemCollection(params DualMenuItem[] items)
        {
            Items = items.ToList();
        }

        public void AddRange(IEnumerable<DualMenuItem> items)
        {
            Items.AddRange(items);
        }

        public void SetIconSource(IconSource source)
        {
            foreach (var item in Items)
            {
                item.IconSource = source;
            }
        }
    }
}
