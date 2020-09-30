using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2.UI
{
    public class NbtIcon : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            NbtText.DrawSelection(node, context);
            var image = GetIcon(node);
            if (image != null)
            {
                float ratio = Math.Min((float)context.Bounds.Width / (float)image.Width, (float)context.Bounds.Height / (float)image.Height);
                var rectangle = new Rectangle();
                rectangle.Width = (int)(image.Width * ratio);
                rectangle.Height = (int)(image.Height * ratio);
                rectangle.X = context.Bounds.X + (context.Bounds.Width - rectangle.Width) / 2;
                rectangle.Y = context.Bounds.Y + (context.Bounds.Height - rectangle.Height) / 2;
                context.Graphics.DrawImage(image, rectangle);
            }
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            return image == null ? Size.Empty : image.Size;
        }

        private Image GetIcon(TreeNodeAdv node)
        {
            return GetImage(node.Tag);
        }

        private static Image GetImage(object obj)
        {
            if (obj is NbtFile)
                return Properties.Resources.file_image;
            if (obj is NbtFolder)
                return Properties.Resources.folder_image;
            if (obj is RegionFile)
                return Properties.Resources.region_image;
            if (obj is Chunk)
                return Properties.Resources.chunk_image;
            if (obj is NbtTag tag)
                return NbtUtil.TagTypeImage(tag.TagType);
            return null;
        }
    }

    public class NbtText : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var halves = GetText(node);
            if (halves != null)
            {
                var size = MeasureSize(node, context);
                PointF point = new PointF(context.Bounds.X, context.Bounds.Y + (context.Bounds.Height - size.Height) / 2);
                DrawSelection(node, context);
                var boldfont = new Font(context.Font, FontStyle.Bold);
                if (halves.Item1 != null)
                {
                    context.Graphics.DrawString(halves.Item1, boldfont, new SolidBrush(Parent.ForeColor), point);
                    point.X += context.Graphics.MeasureString(halves.Item1, boldfont).Width;
                }
                context.Graphics.DrawString(halves.Item2, context.Font, new SolidBrush(Parent.ForeColor), point);
            }
        }

        public static void DrawSelection(TreeNodeAdv node, DrawContext context)
        {
            // selected nodes are not "active" while dragging
            // hovered nodes are "active" while dragging
            if (context.DrawSelection == DrawSelectionMode.Active || (node.IsSelected && !node.Tree.Focused))
                context.Graphics.FillRectangle(Brushes.LightBlue, context.Bounds);
            else if (node.IsSelected)
                context.Graphics.FillRectangle(Brushes.LightYellow, context.Bounds);
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var halves = GetText(node);
            if (halves == null)
                return Size.Empty;
            var boldfont = new Font(context.Font, FontStyle.Bold);
            SizeF s1 = halves.Item1 == null ? SizeF.Empty : context.Graphics.MeasureString(halves.Item1, boldfont);
            SizeF s2 = context.Graphics.MeasureString(halves.Item2, context.Font);
            return new Size((int)Math.Round(s1.Width + s2.Width), (int)Math.Ceiling(Math.Max(s1.Height, s2.Height)));
        }

        private Tuple<string, string> GetText(TreeNodeAdv node)
        {
            var text = PreviewNameAndValue(node);
            return Tuple.Create(Flatten(text.Item1), Flatten(text.Item2));
        }

        private Tuple<string, string> PreviewNameAndValue(TreeNodeAdv node)
        {
            string name = PreviewName(node.Tag, this.Parent);
            string value = PreviewValue(node.Tag);
            if (name == null)
                return Tuple.Create((string)null, value);
            return Tuple.Create(name + ": ", value);
        }

        public static string PreviewName(TreeNodeAdv node) => PreviewName(node.Tag, null);
        public static string PreviewValue(TreeNodeAdv node) => PreviewValue(node.Tag);

        private static string PreviewName(object obj, TreeViewAdv view)
        {
            string prefix = "";
            if (obj is ISaveable saveable && view != null && view.Model is NbtTreeModel nbtmodel && nbtmodel.HasUnsavedChanges(saveable))
                prefix = "* ";
            string result = null;
            if (obj is NbtFile file)
                result = Path.GetFileName(file.Path);
            else if (obj is NbtFolder folder)
                result = Path.GetFileName(folder.Path);
            else if (obj is RegionFile region)
                result = Path.GetFileName(region.Path);
            else if (obj is Chunk chunk)
                result = $"Chunk [{chunk.X}, {chunk.Z}]";
            else if (obj is NbtTag tag)
                result = tag.Name;
            if (result == null)
                return null;
            return prefix + result;
        }

        private static string PreviewValue(object obj)
        {
            if (obj is NbtFile file)
                return NbtUtil.PreviewNbtValue(file.RootTag.Adapt());
            if (obj is NbtFolder folder)
            {
                if (folder.HasScanned)
                {
                    if (folder.Subfolders.Any())
                        return $"[{Util.Pluralize(folder.Subfolders.Count, "folder")}, {Util.Pluralize(folder.Files.Count, "file")}]";
                    else
                        return $"[{Util.Pluralize(folder.Files.Count, "file")}]";
                }
                else
                    return "(open to load)";
            }
            if (obj is RegionFile region)
                return $"[{Util.Pluralize(region.ChunkCount, "chunk")}]";
            if (obj is Chunk chunk)
            {
                if (chunk.IsLoaded)
                    return NbtUtil.PreviewNbtValue(chunk.Data.Adapt());
                else
                    return "(open to load)";
            }
            if (obj is NbtTag tag)
                return NbtUtil.PreviewNbtValue(tag.Adapt());
            return null;
        }

        private static string Flatten(string text)
        {
            if (text == null) return null;
            return text.Replace("\n", "⏎").Replace("\r", "⏎");
        }
    }
}
