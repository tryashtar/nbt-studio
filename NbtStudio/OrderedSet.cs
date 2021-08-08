using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    [Serializable]
    public class OrderedSet<T>
    {
        private int _MaxSize = 100;
        public int MaxSize
        {
            get => _MaxSize;
            set { _MaxSize = value; Trim(); }
        }
        // items stored at the front are older
        private readonly List<T> Items = new();
        public OrderedSet() { }

        public int Count => Items.Count;

        private void Trim()
        {
            if (Items.Count > MaxSize)
                Items.RemoveRange(0, Items.Count - MaxSize);
        }

        public T GetFirst()
        {
            return Items[^1];
        }

        public void Add(T item)
        {
            Items.Remove(item);
            Items.Add(item);
            Trim();
        }

        public void AddMany(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Items.Remove(item);
                Items.Add(item);
            }
            Trim();
        }
    }
}
