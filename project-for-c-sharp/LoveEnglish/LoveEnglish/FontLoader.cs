using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace LoveEnglish {
    public static class FontLoader {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        public static PrivateFontCollection Pfc { get; } = new PrivateFontCollection();
        private static bool _isLoaded = false;

        public static void LoadHarmonyFont() {
            if (_isLoaded)
                return;

            try {
                const string fontResourceName = "LoveEnglish.HarmonyOS_Sans_SC_Regular.ttf"; // 修正资源名

                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(fontResourceName)) {
                    if (stream == null)
                        throw new FileNotFoundException($"Font resource '{fontResourceName}' not found");

                    // 读取字体数据
                    byte[] fontData = new byte[stream.Length];
                    stream.Read(fontData, 0, (int) stream.Length);

                    // 分配非托管内存
                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                    Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                    uint dummy = 0;
                    AddFontMemResourceEx(fontPtr, (uint) fontData.Length, IntPtr.Zero, ref dummy);

                    // 添加到字体集合
                    Pfc.AddMemoryFont(fontPtr, fontData.Length);

                    Marshal.FreeCoTaskMem(fontPtr); // 释放内存
                }
                _isLoaded = true;
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"字体加载失败: {ex.Message}");
                throw; // 抛出异常让调用方知道
            }
        }

        public static FontFamily GetFontFamily() {
            if (Pfc.Families.Length == 0)
                throw new InvalidOperationException("字体尚未加载");

            return Pfc.Families[0];
        }

        public static Font GetFont(float size, FontStyle style = FontStyle.Regular) {
            if (Pfc.Families.Length == 0)
                throw new InvalidOperationException("字体尚未加载");

            return new Font(Pfc.Families[0], size, style);
        }
    }
}
