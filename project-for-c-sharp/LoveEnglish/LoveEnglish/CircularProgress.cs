using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoveEnglish {
    public class CircularProgress : Control {
        private Timer animationTimer;
        private float angle = 0;

        public CircularProgress() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(40, 40);

            animationTimer = new Timer { Interval = 50 };
            animationTimer.Tick += (s, e) => {
                angle = (angle + 10) % 360;
                Invalidate();
            };
            animationTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e) {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 绘制背景圆
            using (var pen = new Pen(Color.White, 3)) {
                g.DrawEllipse(pen, 3, 3, Width - 6, Height - 6);
            }

            // 绘制动态弧线
            using (var pen = new Pen(Color.Black, 3)) {
                g.DrawArc(pen, 3, 3, Width - 6, Height - 6, angle, 90);
            }
        }

        protected override void Dispose(bool disposing) {
            animationTimer?.Stop();
            base.Dispose(disposing);
        }
    }
}
