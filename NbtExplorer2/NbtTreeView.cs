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

namespace NbtExplorer2
{
    public class NbtTreeView : TreeListView
    {
        public NbtTreeView()
        {
            CanExpandGetter = NbtCanExpand;
            ChildrenGetter = NbtGetChildren;

            this.HeaderStyle = ColumnHeaderStyle.None;
            LargeImageList = new ImageList() { ImageSize = new Size(16, 16) };
            SmallImageList = new ImageList() { ImageSize = new Size(16, 16) };
            PopulateImageList(LargeImageList);
            PopulateImageList(SmallImageList);
            AllColumns.Add(new OLVColumn()
            {
                AspectGetter = NbtGetText,
                ImageGetter = NbtGetImage,
                FillsFreeSpace = true
            });
            RebuildColumns();
        }

        protected override bool ProcessLButtonDoubleClick(OlvListViewHitTestInfo hti)
        {
            if (IsExpanded(hti.RowObject))
                Collapse(hti.RowObject);
            else
                Expand(hti.RowObject);
            return base.ProcessLButtonDoubleClick(hti);
        }

        private void PopulateImageList(ImageList list)
        {
            foreach (NbtTagType type in Enum.GetValues(typeof(NbtTagType)))
            {
                var key = type.ToString();
                var value = Util.TagTypeImage(type);
                if (value != null)
                    list.Images.Add(key, value);
            }
        }

        private bool NbtCanExpand(object obj)
        {
            if (obj is NbtFile)
                return true;
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
            int? count = Util.ChildrenCount(obj);
            if (count != null)
                value = $"[{ Util.Pluralize(count.Value, "entry", "entries")}]";
            if (obj is NbtTag tag)
            {
                name = tag.Name;
                if (value == null)
                    value = tag.ToSnbt(multiline: false, delimit: false);
            }
            if (name == null)
                return value;
            return $"{name}: {value}";
        }

        private object NbtGetImage(object obj)
        {
            if (obj is NbtTag tag)
                return tag.TagType.ToString();
            return null;
        }
    }
}
