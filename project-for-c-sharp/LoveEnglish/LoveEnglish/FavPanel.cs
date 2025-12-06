using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DictData;
using DictModel;
using MaterialSkin.Controls;
using static MaterialSkin.MaterialSkinManager;

namespace LoveEnglish {
    internal class FrontFavPanel : RoundPanel {
        private readonly MainForm _mainForm;
        private RoundLabel lblMain, lblCount;

        public int wordCount; // 收藏夹单词数量
        private readonly float fontType1 = 20f;
        private readonly float fontType2 = 12f;

        public FrontFavPanel(MainForm mainForm) {
            _mainForm = mainForm;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponents();
        }

        private void InitializeComponents() {
            this.AutoScroll = false;
            this.Location = new Point(96, 16);
            this.Size = new Size(832, 48);
            this.BackColor = SystemColors.Control;
            this.NoBorder = true;

            lblMain = new RoundLabel {
                Text = "我的收藏夹",
                Font = FontLoader.GetFont(fontType1),
                ForeColor = Color.Black,
                BackColor = SystemColors.Control,
                HoverBackColor = SystemColors.Control,
                AutoSize = false,
                Size = new Size(200, 32),
                Location = new Point(8, 8),
                TextAlign = ContentAlignment.BottomLeft,
                NoBorder = true
            };

            lblCount = new RoundLabel {
                Text = $"共 {wordCount} 词",
                Font = FontLoader.GetFont(fontType2),
                ForeColor = Color.Gray,
                BackColor = SystemColors.Control,
                HoverBackColor = SystemColors.Control,
                AutoSize = false,
                Size = new Size(100, 32),
                Location = new Point(716, 4),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                TextAlign = ContentAlignment.BottomRight,
                NoBorder = true
            };

            this.Controls.Add(lblMain);
            this.Controls.Add(lblCount);
        }

        public void UpdateLabel() {
            SuspendLayout();
            lblMain.Font = FontLoader.GetFont(fontType1);
            lblCount.Font = FontLoader.GetFont(fontType2);
            lblCount.Text = $"共 {wordCount} 个单词";
            Invalidate();
            ResumeLayout(true);
        }
    }

    internal class FavPanel : RoundPanel {
        private readonly MainForm _mainForm;
        private FlowLayoutPanel _resultsFlowPanel;
        private const int verticalPadding = 8;
        private readonly WordService wordFetcher = new WordService();
        private FrontFavPanel _frontFavPanel;
        // its slow but i cannot improve it
        private List<int> favIdList;
        private List<Word> favList=new List<Word>();

        private readonly float fontType1 = 20f;
        private readonly float fontType2 = 12f;

        public bool isShowTrans = true, isShowModify = false;

        public FavPanel(MainForm mainForm) {
            _mainForm = mainForm;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponents();
        }

        public void SetFrontFavPanel(FrontFavPanel frontFavPanel) {
            _frontFavPanel = frontFavPanel;
        }

        public void SetFavButton(FavButton favButton) {
            favButton.OnTransVisibilityChanged += (showTrans) => {
                isShowTrans = showTrans;
                RefreshFavoritesDisplay();
            };
        }

        private void InitializeComponents() {
            this.AutoScroll = true;
            this.Location = new Point(96, 80);
            this.Size = new Size(832, 532);
            this.BackColor = SystemColors.Control;
            this.HoverBackColor = SystemColors.Control;
            this.HorizontalScroll.Visible = false;
            this.HorizontalScroll.Enabled = false;
            this.Padding = new Padding(0, verticalPadding, 0, verticalPadding);
            this.CornerRadius = 5;
            this.NoBorder = true;

            _resultsFlowPanel = new FlowLayoutPanel {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0, 4, 0, 4),
                Padding = new Padding(16, 4, 16, 4)
            };

            this.Controls.Add(_resultsFlowPanel);
            LoadUserFavorites();
        }

        private void CreateFavoritesFlowPanel(List<Word> favoriteWords, 
            bool showTrans = true, bool showModify = false) {
            _resultsFlowPanel.SuspendLayout();
            _resultsFlowPanel.Controls.Clear();

            for (int i = 0; i < favoriteWords.Count; i++) {
                var word = favoriteWords[i];
                bool isLastItem = i == favoriteWords.Count - 1;

                var verticalPanel = new FlowLayoutPanel {
                    Size = new Size(768, 80),
                    FlowDirection = FlowDirection.TopDown,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = false,
                    Margin = new Padding(0, 4, 0, 4),
                    Padding = new Padding(0, 4, 0, 0)
                };

                var lblWord = new RoundLabel {
                    Text = word.WordName,
                    Font = FontLoader.GetFont(fontType1, FontStyle.Bold),
                    ForeColor = Color.Black,
                    BackColor = SystemColors.Control,
                    HoverBackColor = SystemColors.Control,
                    AutoSize = false,
                    Location = new Point(4, 0),
                    Size = new Size(verticalPanel.Width - 20, 40),
                    Padding = new Padding(0, 3, 0, 0),
                    TextAlign = ContentAlignment.MiddleLeft,
                    NoBorder = true,
                    ContentMargin = 0,
                    CornerRadius = 0
                };

                var lblTranslation = new RoundLabel {
                    Text = word.Translation,
                    Font = FontLoader.GetFont(fontType2),
                    ForeColor = Color.Gray,
                    BackColor = SystemColors.Control,
                    HoverBackColor = Color.Gray,
                    AutoSize = false,
                    Location = new Point(4, 0),
                    Size = new Size(verticalPanel.Width - 20, 30),
                    Padding = new Padding(0, 3, 0, 0),
                    TextAlign = ContentAlignment.MiddleLeft,
                    NoBorder = true,
                    HalfHeight = true,
                    ContentMargin = 0,
                    CornerRadius = 2
                };

                lblTranslation.Text = showTrans ? word.Translation : "——";

                // 处理文本显示
                SetTextWithEllipsis(lblWord, lblWord.Text);
                SetTextWithEllipsis(lblTranslation, lblTranslation.Text);

                verticalPanel.MouseEnter += (s, e) => lblTranslation.SetHoverStatus(true);
                lblWord.MouseEnter += (s, e) => lblTranslation.SetHoverStatus(true);
                lblTranslation.MouseEnter += (s, e) => lblTranslation.SetHoverStatus(true);

                verticalPanel.MouseLeave += (s, e) => lblTranslation.SetHoverStatus(false);
                lblWord.MouseLeave += (s, e) => lblTranslation.SetHoverStatus(false);
                lblTranslation.MouseLeave += (s, e) => lblTranslation.SetHoverStatus(false);
                

                // 添加点击事件

                lblWord.Click += (s, e) => {
                    Console.WriteLine($"wordname: {word.WordName}");
                };
                lblTranslation.Click += (s, e) => {
                    Console.WriteLine($"wordname: {word.WordName}");
                };

                // lblWord.Click += (s, e) => _mainForm.ShowWordDetail(word);
                // lblTranslation.Click += (s, e) => _mainForm.ShowWordDetail(word);

                // 需要后续实现

                verticalPanel.Controls.Add(lblWord);
                verticalPanel.Controls.Add(lblTranslation);
                // panel.Controls.Add(verticalPanel);
                _resultsFlowPanel.Controls.Add(verticalPanel);

                if (!isLastItem) {
                    var dividerPanel = new Panel {
                        Height = 2,
                        Width = verticalPanel.Width + 8,
                        Padding = new Padding(16),
                        ForeColor = Color.FromArgb(32, 32, 32, 32),
                        BackColor = Color.FromArgb(32, 32, 32, 32)
                    };
                    _resultsFlowPanel.Controls.Add(dividerPanel);
                }
            }

            _resultsFlowPanel.ResumeLayout(true);
            this.PerformLayout();
        }

        private void SetTextWithEllipsis(Control control, string text) {
            var flags = TextFormatFlags.Left
                | TextFormatFlags.EndEllipsis
                | TextFormatFlags.SingleLine
                | TextFormatFlags.NoPrefix
                | TextFormatFlags.NoClipping;

            var cutText = text
                .Replace("\r", "")
                .Replace("\n", "; ");

            if (cutText.Length > 60) {
                cutText = cutText.Substring(0, 60);
                cutText += "...";
            }
            control.Text = cutText;

            // 强制重绘控件
            using (var graphics = control.CreateGraphics()) {
                TextRenderer.DrawText(
                    graphics,
                    control.Text,
                    control.Font,
                    control.ClientRectangle,
                    control.ForeColor,
                    flags
                );
            }
        }

        public void LoadUserFavorites() {
            /* 重载用户收藏夹，需要用到数据库 
             * 要求：通过数据库获取用户收藏的单词ID，再根据ID搜索单词信息
             * 存入这个类的私有成员List<Word> favList中
             */

            favIdList = wordFetcher.GetFavoriteWordIds();
            favList = new List<Word>();

            for(int i=0;i<favIdList.Count;i++)
            {
                Console.WriteLine($"收藏id列表：{favIdList[i]   }");

            }
            foreach (int id in favIdList)
            {
                Word word =wordFetcher.GetWordById(id);
                favList.Add(word);
            }

            if (_frontFavPanel != null) {
                _frontFavPanel.wordCount = favList.Count;
                _frontFavPanel.UpdateLabel();
            }

            CreateFavoritesFlowPanel(favList, isShowTrans, isShowModify);
        }

        private void RefreshFavoritesDisplay() {
            CreateFavoritesFlowPanel(favList, isShowTrans, isShowModify);
            this.Invalidate();
        }
    }
}
