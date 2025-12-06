using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static MaterialSkin.MaterialSkinManager;

namespace LoveEnglish {
    public class NotificationPopup : Control {
        private readonly Timer _animationTimer = new Timer { Interval = 16 };
        private float _animationProgress;
        private bool _isClosing;
        private bool _isMouseOver;

        public NotificationType Type { get; set; }
        public int DisplayDuration { get; set; } = 3000;
        public int SlideOffset => (int) (Width * (1 - _animationProgress));

        private readonly float fontType = 12f;

        public NotificationPopup(string message, NotificationType type) {
            Type = type;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            Visible = true;
            Size = new Size(300, 70);
            BackColor = GetTypeColor(type);


            Console.WriteLine($"BackColor: {BackColor}");
            ForeColor = Color.White;

            var label = new Label {
                Text = message,
                Font = FontLoader.GetFont(fontType),
                ForeColor = ForeColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            Console.WriteLine($"Label ForeColor: {label.ForeColor}"); 


            Controls.Add(label);

            _animationTimer.Tick += (s, e) => UpdateAnimation();
            MouseEnter += (s, e) => _isMouseOver = true;
            MouseLeave += (s, e) => _isMouseOver = false;
            Click += (s, e) => BeginClose();
        }

        private Color GetTypeColor(NotificationType type) {
            switch (type) {
                case NotificationType.Success:
                    return Color.FromArgb(255, 76, 175, 80);
                case NotificationType.Info:
                    return Color.FromArgb(255, 33, 150, 243);
                case NotificationType.Warning:
                    return Color.FromArgb(255, 255, 193, 7);
                case NotificationType.Error:
                    return Color.FromArgb(255, 244, 67, 54);
                default:
                    return Color.Gray;
            }
        }

        public void ShowNotification(Control parent) {
            parent.Controls.Add(this);
            BringToFront();

            UpdatePosition();Invalidate();
            _animationTimer.Start();
        }

        private void UpdateAnimation() {
            if (_isClosing) {
                _animationProgress -= 0.08f;
                if (_animationProgress <= 0)
                    Dispose();
            } else {
                _animationProgress = Math.Min(_animationProgress + 0.08f, 1);
                if (_animationProgress >= 1 && !_isMouseOver)
                    BeginClose(DisplayDuration);
            }

            UpdatePosition();
            Invalidate();
        }

        private void BeginClose(int delay = 0) {
            if (delay > 0) {
                var closeTimer = new Timer { Interval = delay };
                closeTimer.Tick += (s, e) => { _isClosing = true; closeTimer.Dispose(); };
                closeTimer.Start();
            } else {
                _isClosing = true;
            }
        }

        private void UpdatePosition() {
            if (Parent == null || Width == 0)
                return;

            int screenWidth = Parent.ClientSize.Width;
            float progress = EaseOutCubic(_animationProgress);
            int targetX = (int) (screenWidth - Width * progress);
            targetX = Math.Max(0, Math.Min(screenWidth, targetX));

            Location = new Point(targetX, Location.Y);
            Console.WriteLine($"动画进度: {_animationProgress}, 当前位置: X={Location.X}");
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            using (var pen = new Pen(Color.FromArgb(150, Color.Black), 1))
            using (var path = CreateRoundedPath(ClientRectangle, 10)) {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(new SolidBrush(BackColor), path);
                e.Graphics.DrawPath(pen, path);

                Console.WriteLine($"Filling with color: {BackColor}");
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius) {
            var path = new GraphicsPath();
            radius *= 2;
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static float EaseOutCubic(float t) => (float) (1 - Math.Pow(1 - t, 3));
    }
}
