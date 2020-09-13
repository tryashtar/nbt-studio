using BrightIdeasSoftware;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NbtExplorer2.SNBT;
using System.IO;

namespace NbtExplorer2.UI
{
    public class NbtTreeView : TreeListView
    {
        public NbtTreeView()
        {
            // linking data to visibility
            CanExpandGetter = NbtCanExpand;
            ChildrenGetter = NbtGetChildren;

            // boilerplate to display icons next to tags
            LargeImageList = new ImageList() { ImageSize = new Size(16, 16) };
            SmallImageList = new ImageList() { ImageSize = new Size(16, 16) };
            PopulateImageList(LargeImageList);
            PopulateImageList(SmallImageList);

            this.HeaderStyle = ColumnHeaderStyle.None;
            AllColumns.Add(new OLVColumn()
            {
                // linking data to visibility
                AspectGetter = NbtGetText,
                ImageGetter = NbtGetImage,
                FillsFreeSpace = true
            });
            RebuildColumns();
        }

        // double-click to expand/collapse
        // (automatically does nothing for tags that can't expand/collapse)
        protected override bool ProcessLButtonDoubleClick(OlvListViewHitTestInfo hti)
        {
            ToggleExpansion(hti.RowObject);
            return base.ProcessLButtonDoubleClick(hti);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // space to toggle collapsed/expanded
            if (keyData == Keys.Space)
            {
                ToggleExpansion(SelectedObject);
                return true;
            }
            // control-space to expand all
            if (keyData == (Keys.Space | Keys.Control))
            {
                if (IsExpanded(SelectedObject))
                    Collapse(SelectedObject);
                else
                    ExpandAll(SelectedObject);
                return true;
            }
            // control-up to select parent
            if (keyData == (Keys.Up | Keys.Control))
            {
                SelectObject(GetParent(SelectedObject));
                FocusedItem = SelectedItem; // fix navigation being weird
                return true;
            }
            // control-down to select lowest sibling
            if (keyData == (Keys.Down | Keys.Control))
            {
                var parent = GetParent(SelectedObject);
                if (parent == null)
                    return true;
                var siblings = GetChildren(parent).Cast<object>();
                var last = siblings.Last();
                if (SelectedObject == last)
                    return ProcessCmdKey(ref msg, Keys.Down); // treat as normal down (does not work)
                else
                {
                    SelectObject(last);
                    FocusedItem = SelectedItem; // fix navigation being weird
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // collapse children when collapsing
        protected override void OnCollapsed(TreeBranchCollapsedEventArgs e)
        {
            CollapseAll(e.Model);
            base.OnCollapsed(e);
        }

        private void ExpandAll(object model)
        {
            // reverse order optimization
            foreach (var child in GetChildren(model))
            {
                ExpandAll(child);
            }
            if (!IsExpanded(model))
                Expand(model);
        }

        private void CollapseAll(object model)
        {
            if (IsExpanded(model))
                Collapse(model);
            foreach (var child in GetChildren(model))
            {
                CollapseAll(child);
            }
        }

        // ObjectListView images work by returning strings that are cached in the ImageList
        // set them up to contain the tag type icons
        private void PopulateImageList(ImageList list)
        {
            list.Images.Add("file", Properties.Resources.file_image);
            foreach (NbtTagType type in Enum.GetValues(typeof(NbtTagType)))
            {
                var key = type.ToString();
                var value = Util.TagTypeImage(type);
                if (value != null)
                    list.Images.Add(key, value);
            }
        }

        private object NbtGetImage(object obj)
        {
            if (obj is NbtTag tag)
                return tag.TagType.ToString();
            if (obj is NbtFile)
                return "file";
            return null;
        }

        private bool NbtCanExpand(object obj)
        {
            if (obj is NbtFile file)
                return file.RootTag.Count > 0;
            if (obj is NbtCompound compound)
                return compound.Count > 0;
            if (obj is NbtList list)
                return list.Count > 0;
            return false;
        }

        private IEnumerable NbtGetChildren(object obj)
        {
            if (obj is NbtFile file)
                return file.RootTag.Tags;
            if (obj is NbtCompound compound)
                return compound.Tags;
            if (obj is NbtList list)
                return list;
            return null;
        }

        private string NbtGetText(object obj)
        {
            string name = null;
            string value = null;
            if (obj is NbtFile file)
            {
                name = Path.GetFileName(file.FileName);
                value = Util.PreviewNbtValue(file);
            }
            else if (obj is NbtTag tag)
            {
                name = tag.Name;
                value = Util.PreviewNbtValue(tag);
            }
            if (name == null) // possible if the tag is in a list
                return value;
            return $"{name}: {value}";
        }
    }
}
