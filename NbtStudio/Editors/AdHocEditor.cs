using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtStudio
{
    public delegate T ExtractDelegate<T>(Node node);
    public delegate ICommand EditDelegate<T>(IEnumerable<T> items);
    public delegate ICommand EditSingleDelegate<T>(T item);
    public delegate bool CanEditSingleDelegate<T>(T item);
    public class AdHocEditor<T> : TypedEditor<T>
    {
        public readonly ExtractDelegate<T> ExtractFunction;
        public readonly EditDelegate<T> EditFunction;
        public readonly CanEditSingleDelegate<T> CanEditFunction;

        public AdHocEditor(ExtractDelegate<T> extract, CanEditSingleDelegate<T> can, EditDelegate<T> edit)
        {
            ExtractFunction = extract;
            CanEditFunction = can;
            EditFunction = edit;
        }

        protected override T Extract(Node node)
        {
            return ExtractFunction(node);
        }

        protected override ICommand Edit(IEnumerable<T> items)
        {
            return EditFunction(items);
        }

        protected override bool CanEdit(T item)
        {
            return CanEditFunction(item);
        }
    }

    public class AdHocSingleEditor<T> : SingleEditor<T>
    {
        public readonly ExtractDelegate<T> ExtractFunction;
        public readonly EditSingleDelegate<T> EditFunction;
        public readonly CanEditSingleDelegate<T> CanEditFunction;

        public AdHocSingleEditor(ExtractDelegate<T> extract, CanEditSingleDelegate<T> can, EditSingleDelegate<T> edit)
        {
            ExtractFunction = extract;
            CanEditFunction = can;
            EditFunction = edit;
        }

        protected override T Extract(Node node)
        {
            return ExtractFunction(node);
        }

        protected override ICommand Edit(T item)
        {
            return EditFunction(item);
        }

        protected override bool CanEdit(T item)
        {
            return CanEditFunction(item);
        }
    }
}
