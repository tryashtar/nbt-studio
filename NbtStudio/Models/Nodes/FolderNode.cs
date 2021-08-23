using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class FolderNode : Node<NbtFolder, IHavePath>
    {
        public FolderNode(NbtFolder wrapped) : base(wrapped) { }

        protected override IEnumerable<IHavePath> GetTypedChildren()
        {
           return WrappedObject.Subfolders.Concat<IHavePath>(WrappedObject.Files);
        }

        protected override Node MakeTypedChild(IHavePath obj)
        {
            if (obj is NbtFolder folder)
                return new FolderNode(folder);
            if (obj is NbtFile file)
                return new NbtFileNode(file);
            throw new ArgumentException();
        }

        public override IconType GetIcon() => IconType.Folder;
        public override string PreviewName() => System.IO.Path.GetFileName(WrappedObject.Path);
        public override string PreviewValue()
        {
            if (WrappedObject.HasScanned)
            {
                if (WrappedObject.Subfolders.Any())
                    return $"[{StringUtils.Pluralize(WrappedObject.Subfolders.Count, "folder")}, {StringUtils.Pluralize(WrappedObject.Files.Count, "file")}]";
                else
                    return $"[{StringUtils.Pluralize(WrappedObject.Files.Count, "file")}]";
            }
            else
                return "(open to load)";
        }
    }
}
