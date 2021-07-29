using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class Lens<T> : IDisposable
    {
        public T Item
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return InternalItem;
            }
        }
        private readonly T InternalItem;
        private readonly Action Callback;
        private bool Disposed = false;
        public Lens(T item, Action callback)
        {
#if DEBUG
            Debug.WriteLine($"Lens created: {item}");
#endif
            InternalItem = item;
            Callback = callback;
        }

        public void Dispose()
        {
#if DEBUG
            Debug.WriteLine($"Lens disposed: {InternalItem}");
#endif
            if (!Disposed)
            {
                Disposed = true;
                Callback();
            }
        }
    }
}