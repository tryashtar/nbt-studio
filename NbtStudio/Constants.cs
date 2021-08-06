using fNbt;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public static class Constants
    {
        public static readonly Color SelectionColor = Color.FromArgb(181, 215, 243);
    }

    internal class Unit
    {
        public static readonly Unit One = new();
        public static Func<Unit> Wrap(Action action) => () => { action(); return One; };
        private Unit() { }
    }
}
