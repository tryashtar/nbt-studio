using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtStudio
{
    public abstract class ContextFreeEditor
    {
        public abstract bool CanEdit();
        public abstract ICommand Edit();
    }
}
