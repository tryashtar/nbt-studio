using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class ColumnConsistinator
    {
        public readonly Form Form;
        public readonly ListView Control;
        private readonly decimal[] ColumnRatios;
        private bool Changing = false;
        public ColumnConsistinator(Form form, ListView control)
        {
            Form = form;
            Control = control;

            Form.Resize += Form_Resize;
            Control.ColumnWidthChanged += Control_ColumnWidthChanged;
            ColumnRatios = new decimal[control.Columns.Count];
            DetermineColumnRatios();
            FixColumnRatios();
        }

        private void Control_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (Changing)
                return;
            Changing = true;
            int other_index = e.ColumnIndex == 0 ? 1 : 0;
            Control.Columns[other_index].Width = Control.Width - Control.Columns[e.ColumnIndex].Width - 5;
            DetermineColumnRatios();
            Changing = false;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            FixColumnRatios();
        }

        private void DetermineColumnRatios()
        {
            int total_width = Control.Columns.Cast<ColumnHeader>().Sum(x => x.Width);
            for (int i = 0; i < ColumnRatios.Length; i++)
            {
                ColumnRatios[i] = (decimal)Control.Columns[i].Width / total_width;
            }
        }

        private void FixColumnRatios()
        {
            if (Changing)
                return;
            Changing = true;
            for (int i = 0; i < ColumnRatios.Length; i++)
            {
                Control.Columns[i].Width = (int)(Control.Width * ColumnRatios[i]) - 2;
            }
            Changing = false;
        }
    }
}
