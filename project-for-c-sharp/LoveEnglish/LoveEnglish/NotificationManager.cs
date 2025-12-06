using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoveEnglish {
    public enum NotificationType { Success, Info, Warning, Error }

    public class NotificationManager {
        private readonly Control _parent;
        private readonly Queue<NotificationPopup> _queue = new Queue<NotificationPopup>();
        private NotificationPopup _currentPopup;

        public NotificationManager(Control parent) {
            _parent = parent;
        }

        public void Enqueue(string message, NotificationType type) {
            var popup = new NotificationPopup(message, type) {
                Location = new Point(
                    _parent.ClientSize.Width,
                    GetNextYPosition()
                ),
                Size = new Size(200, 50)
            };

            _queue.Enqueue(popup);
            Console.WriteLine($"弹窗初始位置: X={popup.Location.X}, Y={popup.Location.Y}");
            ShowNext();
        }

        private void ShowNext() {
            if (_currentPopup != null || _queue.Count == 0)
                return;

            _currentPopup = _queue.Dequeue();
            _currentPopup.Disposed += (s, e) => {
                _currentPopup = null;
                ShowNext();
            };
            _currentPopup.ShowNotification(_parent);
        }

        private int GetNextYPosition() {
            int baseY = 640;
            if (_currentPopup != null)
                return _currentPopup.Location.Y - _currentPopup.Height + 80;
            return baseY;
        }
    }
}
