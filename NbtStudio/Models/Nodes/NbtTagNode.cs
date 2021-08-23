using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Nbt;

namespace NbtStudio
{
    public class NbtTagNode : Node<NbtTag, NbtTag>
    {
        public NbtTag Tag => WrappedObject;

        public NbtTagNode(Node parent, NbtTag wrapped) : base(parent, wrapped)
        {
            if (wrapped is NbtContainerTag container)
                container.ChildrenChanged += tag => MarkDirty();
        }

        protected override IEnumerable<NbtTag> GetTypedChildren()
        {
            if (WrappedObject is NbtContainerTag container)
                return container.Tags;
            return null;
        }

        protected override Node MakeTypedChild(NbtTag obj)
        {
            return new NbtTagNode(this, obj);
        }

        public override NbtTag GetNbtTag() => WrappedObject;

        public override IconType GetIcon() => NbtUtil.TagIconType(WrappedObject.TagType);
        public override string PreviewName() => Snbt.GetName(WrappedObject, SnbtOptions.Preview);
        public override string PreviewValue() => NbtUtil.PreviewNbtValue(WrappedObject);
        public override string GetTooltip()
        {
            if (WrappedObject is NbtString str)
            {
                if (str.Value.Contains("\n"))
                    return str.Value;
                if (str.Value.Length > 100)
                    return WrapTooltip(str.Value, 100);
            }
            else if (WrappedObject is NbtByteArray ba)
                return WrapTooltip(String.Join(", ", ba.Value), 100);
            else if (WrappedObject is NbtIntArray ia)
                return WrapTooltip(String.Join(", ", ia.Value), 100);
            else if (WrappedObject is NbtLongArray la)
                return WrapTooltip(String.Join(", ", la.Value), 100);
            return null;
        }

        private static string WrapTooltip(string text, int max_width)
        {
            for (int i = max_width; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]))
                {
                    text = text.Substring(0, i) + Environment.NewLine + text[(i + 1)..];
                    i += max_width;
                }
            }
            return text;
        }
    }
}
