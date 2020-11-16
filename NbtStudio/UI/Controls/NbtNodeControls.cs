using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio.UI
{
    public class NbtIcon : NodeControl
    {
        public IconSource IconSource;
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
                if (context.Bounds.Width < image.Width || context.Bounds.Height < image.Height)
                    context.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                else
                    context.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                context.Graphics.DrawImage(image, rectangle);
            }
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            int height = node.Tree.RowHeight - 4;
            return image == null ? Size.Empty : new Size((int)(((float)height / image.Height) * image.Width), height);
        }

        private Image GetIcon(TreeNodeAdv node)
        {
            return GetImage(node.Tag);
        }

        private Image GetImage(object obj)
        {
            if (IconSource == null)
                return null;
            if (obj is NbtFileNode)
                return IconSource.GetImage(IconType.File).Image;
            if (obj is FolderNode)
                return IconSource.GetImage(IconType.Folder).Image;
            if (obj is RegionFileNode)
                return IconSource.GetImage(IconType.Region).Image;
            if (obj is ChunkNode)
                return IconSource.GetImage(IconType.Chunk).Image;
            if (obj is NbtTagNode tag)
                return NbtUtil.TagTypeImage(IconSource, tag.Tag.TagType).Image;
            return null;
        }
    }

    public class NbtText : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var (name, value) = GetText(node);
            var size = MeasureSizeF(node, context);
            PointF point = new PointF(context.Bounds.X, context.Bounds.Y + (context.Bounds.Height - size.Height) / 2);
            DrawSelection(node, context);
            var boldfont = new Font(context.Font, FontStyle.Bold);
            if (name != null)
            {
                context.Graphics.DrawString(name, boldfont, new SolidBrush(Parent.ForeColor), point);
                point.X += context.Graphics.MeasureString(name, boldfont).Width;
            }
            context.Graphics.DrawString(value, context.Font, new SolidBrush(Parent.ForeColor), point);
        }

        public static void DrawSelection(TreeNodeAdv node, DrawContext context)
        {
            // selected nodes are not "active" while dragging
            // hovered nodes are "active" while dragging
            if (context.DrawSelection == DrawSelectionMode.Active || (node.IsSelected && !node.Tree.Focused))
                context.Graphics.FillRectangle(new SolidBrush(Util.SelectionColor), context.Bounds);
            else if (node.IsSelected)
                context.Graphics.FillRectangle(Brushes.LightYellow, context.Bounds);
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var size = MeasureSizeF(node, context);
            return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
        }

        private SizeF MeasureSizeF(TreeNodeAdv node, DrawContext context)
        {
            var (name, value) = GetText(node);
            var boldfont = new Font(context.Font, FontStyle.Bold);
            SizeF s1 = name == null ? SizeF.Empty : context.Graphics.MeasureString(name, boldfont);
            SizeF s2 = context.Graphics.MeasureString(value, context.Font);
            return new SizeF(s1.Width + s2.Width, Math.Max(s1.Height, s2.Height));
        }

        private (string name, string value) GetText(TreeNodeAdv node)
        {
            var (name, value) = PreviewNameAndValue(node);
            return (Flatten(name), Flatten(value));
        }

        private (string name, string value) PreviewNameAndValue(TreeNodeAdv node)
        {
            string prefix = null;
            string name = PreviewName(node);
            string value = PreviewValue(node);
            if (node.Tag is INode inode)
            {
                var saveable = inode.GetSaveable();
                var chunk = inode.GetChunk();
                if ((saveable != null && saveable.HasUnsavedChanges) || (chunk != null && chunk.HasUnsavedChanges))
                    prefix = "* ";
            }
            if (name == null)
                return (prefix, value);
            return (prefix + name + ": ", value);
        }

        public static string PreviewName(TreeNodeAdv node) => PreviewName(node.Tag as INode);
        public static string PreviewValue(TreeNodeAdv node) => PreviewValue(node.Tag as INode);

        private static string PreviewName(INode node)
        {
            if (node is NbtFileNode file)
                return Path.GetFileName(file.File.Path);
            if (node is FolderNode folder)
                return Path.GetFileName(folder.Folder.Path);
            if (node is RegionFileNode region)
                return Path.GetFileName(region.Region.Path);
            if (node is ChunkNode chunk)
                return $"Chunk [{chunk.Chunk.X}, {chunk.Chunk.Z}]";
            if (node is NbtTagNode tag)
                return tag.Tag.Name;
            return null;
        }

        private static string PreviewValue(INode node)
        {
            if (node is NbtFileNode file)
                return NbtUtil.PreviewNbtValue(file.File.RootTag);
            if (node is FolderNode folder_node)
            {
                var folder = folder_node.Folder;
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
            if (node is RegionFileNode region)
                return $"[{Util.Pluralize(region.Region.ChunkCount, "chunk")}]";
            if (node is ChunkNode chunk_node)
            {
                var chunk = chunk_node.Chunk;
                if (chunk.IsLoaded)
                    return NbtUtil.PreviewNbtValue(chunk.Data);
                else if (chunk.IsExternal)
                    return "(saved externally)";
                else
                    return "(open to load)";
            }
            if (node is NbtTagNode tag)
                return NbtUtil.PreviewNbtValue(tag.Tag);
            return null;
        }

        private static string Flatten(string text)
        {
            if (text == null) return null;
            return text.Replace("\n", "⏎").Replace("\r", "⏎");
        }
    }
}
