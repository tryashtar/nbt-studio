using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NbtStudio.UI;

namespace NbtStudio
{
    public static class DebugLog
    {
        private static readonly List<string> Lines = new();
        public static void WriteLine(string text)
        {
            Lines.Add(text);
            if (Lines.Count > 1000)
                Lines.RemoveRange(0, 100);
        }
        public static ReadOnlyCollection<string> Get()
        {
            return Lines.AsReadOnly();
        }
    }
}
