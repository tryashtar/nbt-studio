using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public class ExceptionWindow : InfoWindow
    {
        public readonly IFailable Error;
        public ExceptionWindow(string title, string message, IFailable failable, string after = null, InfoWindowButtons buttons = InfoWindowButtons.OK) :
            base(title: title,
                message: message + "\n\n" + failable.ToStringSimple() + (after != null ? "\n\n" + after : ""),
                extra_details: failable.ToStringDetailed(),
                buttons: buttons)
        {
            Error = failable;
            this.Icon = SystemIcons.Warning;
        }
    }
}