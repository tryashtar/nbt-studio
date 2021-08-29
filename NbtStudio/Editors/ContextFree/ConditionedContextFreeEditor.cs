using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtStudio
{
    public class ConditionedContextFreeEditor : ContextFreeEditor
    {
        public readonly ContextFreeEditor BaseEditor;
        public readonly Func<bool> Condition;

        public ConditionedContextFreeEditor(ContextFreeEditor base_editor, Func<bool> condition)
        {
            BaseEditor = base_editor;
            Condition = condition;
        }

        public override bool CanEdit()
        {
            return Condition() && BaseEditor.CanEdit();
        }

        public override ICommand Edit()
        {
            return BaseEditor.Edit();
        }
    }
}
