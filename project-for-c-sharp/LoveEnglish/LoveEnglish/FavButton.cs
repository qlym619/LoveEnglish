using DictData;
using DictModel;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoveEnglish {
    internal class FavButton {
        private readonly MainForm _mainForm;
        public MaterialButton btnTrans;
        private const int verticalPadding = 8;

        private readonly float fontType1 = 20f;
        private readonly float fontType2 = 12f;

        public event Action<bool> OnTransVisibilityChanged;
        private bool _isShowTrans = true;
        public bool IsShowTrans {
            get => _isShowTrans;
            private set {
                if (_isShowTrans != value) {
                    _isShowTrans = value;
                    OnTransVisibilityChanged?.Invoke(_isShowTrans);
                }
            }
        }

        public FavButton(MainForm mainForm) {
            _mainForm = mainForm;
            btnTrans = new MaterialButton {
                Text = "隐藏释义",
                Icon = LoveEnglish.Properties.Resources.eyeopen_icon,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(80, 36),
                Location = new Point(820, 56)
            };

            btnTrans.Click += (s, e) => {
                IsShowTrans = !IsShowTrans;
                btnTrans.Text = IsShowTrans ? "隐藏释义" : "显示释义";
                UpdateTransButtonColor();
            };

            mainForm.AddToTabPage3(btnTrans);
            btnTrans.BringToFront();
        }

        private void UpdateTransButtonColor() {
            if (!IsShowTrans) {
                btnTrans.Icon = LoveEnglish.Properties.Resources.eyeclosed_icon;
                btnTrans.UseAccentColor = true;
            } else {
                btnTrans.Icon = LoveEnglish.Properties.Resources.eyeopen_icon;
                btnTrans.UseAccentColor = false;
            }
        }
    }
}
