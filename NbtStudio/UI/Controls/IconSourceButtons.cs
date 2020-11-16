using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class IconSourceButtons : UserControl
    {
        public event EventHandler DeleteClicked;
        public event EventHandler ConfirmClicked;
        public IconSourceButtons(IconSource source)
        {
            InitializeComponent();
            ConfirmButton.Text = source.Name;
            DeleteButton.Visible = source is ZippedIconSource;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteClicked?.Invoke(this, e);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            ConfirmClicked?.Invoke(this, e);
        }
    }
}
