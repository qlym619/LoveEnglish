using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoveEnglish {
    public static class ColorExtensions {
        public static string ToHex(this Color color) {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color ColorMerge(this Color color1, Color color2, double percent) {
            return Color.FromArgb(
                (int) (color1.A * (1 - percent) + color2.A * percent),
                (int) (color1.R * (1 - percent) + color2.R * percent),
                (int) (color1.G * (1 - percent) + color2.G * percent),
                (int) (color1.B * (1 - percent) + color2.B * percent)
            );
        }
    }
}
