using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace LoveEnglish {
    public class WaterfallLayout : LayoutEngine {
        public int Columns { get; set; } = 2;
        public int Spacing { get; set; } = 10;

        public override bool Layout(object container, LayoutEventArgs e) {
            var parent = (Panel) container;
            int colWidth = (parent.ClientSize.Width - (Columns - 1) * Spacing) / Columns;
            var columns = new int[Columns];

            foreach (RoundPanel card in parent.Controls.OfType<RoundPanel>().OrderBy(c => c.Top)) {
                int minCol = Array.IndexOf(columns, columns.Min());

                card.Width = colWidth;
                card.Location = new Point(
                    minCol * (colWidth + Spacing) + Spacing,
                    columns[minCol]
                );

                columns[minCol] += card.Height + Spacing;
            }

            parent.AutoScrollMinSize = new Size(0, columns.Max());
            return false;
        }
    }

    public static class ControlExtensions {
        public static void SetDoubleBuffered(this Control control) {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        public static void SuspendLayout(this Control control) {
            typeof(Control).InvokeMember("SuspendLayout",
                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, null);
        }

        public static void ResumeLayout(this Control control, bool performLayout = true) {
            typeof(Control).InvokeMember("ResumeLayout",
                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { performLayout });
        }
    }
}
