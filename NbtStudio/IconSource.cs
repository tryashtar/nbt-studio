using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public abstract class IconSource
    {
        public abstract ImageIcon Region { get; }
        public abstract ImageIcon Chunk { get; }
        public abstract ImageIcon File { get; }
        public abstract ImageIcon Folder { get; }
    }

    public struct ImageIcon
    {
        public readonly Image Image;
        public readonly Icon Icon;
        public ImageIcon(Image image, Icon icon)
        {
            Image = image;
            Icon = icon;
        }
    }

    // classic icons by Yusuke Kamiyamane
    public class ClassicIconSource : IconSource
    {
        public static ClassicIconSource Instance = new ClassicIconSource();
        private ClassicIconSource() { }
        public override ImageIcon Region => new ImageIcon(Properties.Resources.classic_region_image, Properties.Resources.classic_region_icon);
    }

    // new icons by AmberW
    public class NewIconSource : IconSource
    {
        public static NewIconSource Instance = new NewIconSource();
        private NewIconSource() { }
    }
}
