using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LoveEnglish {
    internal class RoundPanel : Panel {
        public Color BorderColor { get; set; } = Color.Gray;
        public Color HoverBackColor { get; set; } = Color.LightGray;
        public int BorderWidth { get; set; } = 1;
        public int CornerRadius { get; set; } = 5;
        public int ContentMargin { get; set; } = 1;
        public bool NoBorder { get; set; } = false;
        public bool HalfHeight { get; set; } = false;

        private GraphicsPath _cachedPath;
        private Rectangle _lastBounds;

        private const int _animInterval = 16; // 60 FPS
        private float _hoverProgress = 0f;
        private bool _isHovering = false;
        private readonly Timer _animTimer = new Timer();

        public RoundPanel() {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint, true);
            Padding = new Padding(8, 8, 8, 8);
            BackColor = Color.White;

            _animTimer.Interval = _animInterval;
            _animTimer.Tick += (s, e) => UpdateAnimations(s, e);
            _animTimer.Start();

            this.MouseEnter += (s, e) => {
                _isHovering = true;
                Invalidate();
            };
            this.MouseLeave += (s, e) => {
                _isHovering = false;
                Invalidate();
            };
        }

        private void UpdateAnimations(object sender, EventArgs e) {
            bool needsRedraw = false;

            float hoverTarget = _isHovering ? 1f : 0f;
            if (UpdateProgress(ref _hoverProgress, hoverTarget, 0.15f))
                needsRedraw = true;

            if (needsRedraw)
                Invalidate();
        }

        private bool UpdateProgress(ref float current, float target, float speed) {
            if (Math.Abs(current - target) < 0.01f)
                return false;

            current += (target > current) ? speed : -speed;
            current = Math.Max(0, Math.Min(target, current));
            return true;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (Width == 0 || Height == 0)
                return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var drawBounds = new Rectangle(
                ContentMargin,
                ContentMargin,
                Width - ContentMargin * 2,
                Height - ContentMargin * 2
            );

            if (HalfHeight) {
                drawBounds = new Rectangle(
                    ContentMargin,
                    Height / 2 + ContentMargin,
                    Width - ContentMargin * 2,
                    Height / 2 - ContentMargin * 2
                );
            }

            if (_cachedPath == null || _lastBounds != drawBounds) {
                _cachedPath?.Dispose();
                _cachedPath = CreateRoundedPath(drawBounds, CornerRadius);
                _lastBounds = drawBounds;
            }

            using (var backBrush = new SolidBrush(BackColor)) {
                e.Graphics.FillPath(backBrush, _cachedPath);
            }

            if (_hoverProgress > 0) {
                using (var backBrush = new SolidBrush(Color.FromArgb(
                    (int) (20 * _hoverProgress), HoverBackColor))) {
                    e.Graphics.FillPath(backBrush, _cachedPath);
                }
            }

            using (var clipBrush = new SolidBrush(Parent?.BackColor ?? SystemColors.Control))
            using (var clipRegion = new Region(ClientRectangle)) {
                clipRegion.Exclude(_cachedPath);
                e.Graphics.FillRegion(clipBrush, clipRegion);
            }

            if (!NoBorder) {
                using (var borderPen = new Pen(BorderColor, BorderWidth)) {
                    borderPen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(borderPen, _cachedPath);
                }
            }
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius) {
            GraphicsPath path = new GraphicsPath();
            radius *= 2;

            if (radius > 0) {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);                     // 左上角
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);        // 右上角
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);  // 右下角
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);        // 左下角
                path.CloseFigure();
            } else {
                path.AddRectangle(rect);
            }
            return path;
        }

        public void SetHoverStatus(bool isHovering) {
            _isHovering = isHovering;
            Invalidate();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _cachedPath?.Dispose();
                _animTimer?.Stop();
                _animTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
