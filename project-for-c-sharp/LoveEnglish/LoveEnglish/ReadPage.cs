using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DictData;
using DictModel;
using MaterialSkin;
using MaterialSkin.Controls;
using static MaterialSkin.Controls.MaterialButton;

namespace LoveEnglish
{
    /// <summary>
    /// 阅读页面窗体，继承自MaterialForm以实现Material Design风格
    /// </summary>
    /// 
    public partial class ReadPage : MaterialForm
    {
        // === Windows API声明 ===
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 0x000B;

        // === 控件声明 === 
        private MaterialButton btnReturn, btnImportDatabase, btnNextArticle, btnPreviousArticle, btnDeleteArticle;
        private RichTextBox txShowArticle, txWordDefinition;
        private MaterialLabel lblPageNumber; // 页码标签
        private MaterialTextBox2 txtJumpTo;//跳转控件

        // === 服务依赖 ===
        private readonly IArticleService _articleService;  // 文章数据服务
        private readonly IWordService _wordService;        // 单词数据服务

        // === 状态管理 ===
        private List<Article> _articles = new List<Article>();     // 文章列表
        private int _currentArticleIndex = -1;                    // 当前文章索引
        private string _currentArticleRaw;                       // 原始文章内容（未高亮）
        private Point _scrollPosition;                            // 滚动条位置缓存
        //private string _currentTitle;                           // 存储当前标题

        // === 颜色配置 ===
        private readonly Color _baseHighlight;   // 基础高亮色（来自Material主题）
        private readonly Color _clickHighlight;  // 点击高亮色（来自Material主题）
        private readonly Color _defaultColor;    // 默认文字颜色（来自Material主题）

        // === 事件定义 ===
        public event Action ReturnToMain;  // 返回主界面事件

        /// <summary>
        /// 构造函数：初始化Material皮肤和依赖服务
        /// </summary>
        public ReadPage(IArticleService articleService, IWordService wordService)
        {
            // 初始化Material Design皮肤配置
            MaterialSkinManager.Instance.AddFormToManage(this);
            MaterialSkinManager.Instance.ColorScheme = new ColorScheme(
                Primary.Blue600,   // 主色
                Primary.Blue800,   // 暗主色
                Primary.Blue300,   // 亮主色
                Accent.Orange200,  // 强调色
                TextShade.WHITE    // 文字颜色
            );
            MaterialSkinManager.Instance.Theme = MaterialSkinManager.Themes.LIGHT;

            // 从皮肤管理器获取颜色配置
            _baseHighlight = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
            _clickHighlight = MaterialSkinManager.Instance.ColorScheme.AccentColor;
            _defaultColor = MaterialSkinManager.Instance.TextHighEmphasisColor;

            // 依赖注入
            _articleService = articleService;
            _wordService = wordService;

            InitializeComponents();  // 初始化界面组件
            LoadArticles();          // 加载文章数据


        }

        /// <summary>
        /// 初始化界面组件（Material Design风格）
        /// </summary>
        private void InitializeComponents()
        {
            this.SuspendLayout();

            // === 窗体基础设置 ===
            this.Text = ""; // 清空标题
            this.FormBorderStyle = FormBorderStyle.None; // 去除窗体边框
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false; // 去除控制框（关闭、最小化、最大化按钮）
            this.Dock = DockStyle.Fill;// 让窗体大小自适应父容器

            // === 初始化Material风格按钮 ===
            btnPreviousArticle = NewMaterialButton("上一篇", 80, 700);
            btnNextArticle = NewMaterialButton("下一篇", 240, 700);
            btnImportDatabase = NewMaterialButton("导入数据", 380, 700);
            btnDeleteArticle = NewMaterialButton("删除文章", 540, 700);

            // 文章显示区域
            txShowArticle = new RichTextBox
            {
                Size = new Size(600, 500),
                Location = new Point(80, 100),
                BackColor = MaterialSkinManager.Instance.BackgroundColor,  // 使用主题背景色
                ForeColor = _defaultColor,                               // 使用主题文字颜色
                ReadOnly = true,                                         // 只读模式
                ScrollBars = RichTextBoxScrollBars.ForcedVertical,        // 强制显示垂直滚动条
                BorderStyle = BorderStyle.None,                           // 无边框样式
                Font = new Font("Microsoft Sans Serif", 10) // 统一设置字体
            };
            txShowArticle.MouseClick += OnArticleClicked;

            // 单词释义区域
            txWordDefinition = new RichTextBox
            {
                Size = new Size(300, 500),
                Location = new Point(680, 100),
                BackColor = MaterialSkinManager.Instance.BackgroundColor,
                ForeColor = _defaultColor,
                ReadOnly = true,
                Text = "选中单词显示释义...",
                BorderStyle = BorderStyle.None
            };

            // 页码标签
            lblPageNumber = new MaterialLabel
            {
                Location = new Point(140, 600),
                AutoSize = true,
                ForeColor = MaterialSkinManager.Instance.ColorScheme.AccentColor,
                Text = "当前文章：0 / 总文章数：0"
            };

           
           txtJumpTo = new MaterialTextBox2
           {
               Hint = "输入文章ID或标题",
               Size = new Size(200, 40),
               Location = new Point(200, 640),
               MaxLength = 100
           };
            var btnJump = NewMaterialButton("跳转", 400, 640);
           btnJump.Click += (s, e) => JumpToArticle(txtJumpTo.Text.Trim());

            this.Controls.AddRange(new Control[] {
                btnPreviousArticle, btnNextArticle, btnImportDatabase,
                btnDeleteArticle, txShowArticle, txWordDefinition,
                lblPageNumber, txtJumpTo, btnJump
            });

            // 事件绑定
            btnNextArticle.Click += (s, e) => NavigateArticle(1);
            btnPreviousArticle.Click += (s, e) => NavigateArticle(-1);
            btnImportDatabase.Click += ImportArticles;
            btnDeleteArticle.Click += DeleteCurrentArticle;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// 创建标准化的Material风格按钮
        /// </summary>
        /// <param name="text">按钮文字</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>配置好的MaterialButton</returns>
        private MaterialButton NewMaterialButton(string text, int x, int y)
        {
            return new MaterialButton
            {
                Text = text,
                Size = new Size(120, 40),
                Location = new Point(x, y),
                Type = MaterialButtonType.Contained,
                UseAccentColor = true,
                HighEmphasis = true,
                Density = MaterialButtonDensity.Default,
                Margin = new Padding(0, 0, 8, 8)
            };
        }

        #region 核心功能
        /// <summary>
        /// 加载所有文章并显示第一篇
        /// </summary>
        private void LoadArticles()
        {
            _articles = _articleService.GetAllArticles();
            _currentArticleIndex = _articles.Count > 0 ? 0 : -1;
            lblPageNumber.Text = _articles.Count > 0
                ? $"当前文章：1 / 总文章数：{_articles.Count}"
                : "数据库中没有文章";
            DisplayCurrentArticle();
        }

        /// <summary>
        /// 显示当前文章内容（带高亮渲染）
        /// </summary>
        //private void DisplayCurrentArticle()
        //{
        //    if (_currentArticleIndex < 0 || _currentArticleIndex >= _articles.Count)
        //    {
        //        txShowArticle.Text = "没有可用的文章";
        //        lblPageNumber.Text = "当前无文章";
        //        return;
        //    }

        //    var article = _articles[_currentArticleIndex];
        //    _currentArticleRaw = $"Title: {article.Title}\n\n正文：{article.Content}";
        //    //_currentTitle = article.Title;

        //    // 重置格式
        //    txShowArticle.Text = _currentArticleRaw;
        //    UpdatePageNumberDisplay(article);
        //    RenderArticleWithHighlight();
        //}

        private void DisplayCurrentArticle()
        {
            if (_currentArticleIndex < 0 || _currentArticleIndex >= _articles.Count)
            {
                txShowArticle.Text = "没有可用的文章";
                lblPageNumber.Text = "当前无文章";
                return;
            }

            var article = _articles[_currentArticleIndex];

            // 处理换行符（兼容多种情况）
            string formattedContent = article.Content
                .Replace("\\n", "\n")  // 处理转义的\n
                .Replace("\n", Environment.NewLine); // 转换为系统换行符

            _currentArticleRaw = $"Title: {article.Title}{Environment.NewLine}{Environment.NewLine}{formattedContent}";

            // 确保RichTextBox设置正确
            txShowArticle.Multiline = true;
            txShowArticle.WordWrap = true;
            txShowArticle.ScrollBars = RichTextBoxScrollBars.Vertical; // 添加滚动条

            // 设置文本
            txShowArticle.Text = _currentArticleRaw;

            UpdatePageNumberDisplay(article);
            RenderArticleWithHighlight();
        }
        // 辅助方法：更新页码显示
        private void UpdatePageNumberDisplay(Article article)
        {
            lblPageNumber.Text = _articles.Count > 0
                ? $"当前文章：{_currentArticleIndex + 1}（{article.Title}） / 总文章数：{_articles.Count}"
                : "数据库中没有文章";
        }


        /// <summary>
        /// 带高亮的文章渲染方法（解决高亮残留问题的关键）
        /// </summary>
        private void RenderArticleWithHighlight()
        {
            SendMessage(txShowArticle.Handle, WM_SETREDRAW, false, 0);
            try
            {
                _scrollPosition = txShowArticle.GetScrollPosition();
                txShowArticle.Text = _currentArticleRaw;

                // 应用基础高亮
                var vocabulary = _wordService.GetAllWordNames();
                int bodyStartPos = _currentArticleRaw.IndexOf("正文：") + 3;
                foreach (var word in ExtractCleanWords(_currentArticleRaw.Substring(bodyStartPos)))
                {
                    if (vocabulary.Contains(word, StringComparer.OrdinalIgnoreCase))
                    {
                        ApplyColorHighlight(word, _baseHighlight, bodyStartPos);
                    }
                }

                txShowArticle.SetScrollPosition(_scrollPosition);
            }
            finally
            {
                SendMessage(txShowArticle.Handle, WM_SETREDRAW, true, 0);
                txShowArticle.Invalidate();
            }
        }


        #endregion

        #region 高亮管理
        /// <summary>
        /// 从文本中提取干净单词（去除标点符号）
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <returns>干净的单词列表</returns>
        private IEnumerable<string> ExtractCleanWords(string text)
        {
            return Regex.Matches(text, @"\b[\w'-]+\b")
                       .Cast<Match>()
                       .Select(m => m.Value)
                       .Distinct(StringComparer.OrdinalIgnoreCase);
        }


        /// <summary>
        /// 应用高亮的核心方法
        /// </summary>
        /// <param name="word">目标单词</param>
        /// <param name="color">高亮颜色</param>
        private void ApplyColorHighlight(string word, Color color, int startOffset = 0)
        {
            int pos = startOffset;
            while (pos < txShowArticle.TextLength)
            {
                int found = txShowArticle.Find(word, pos, RichTextBoxFinds.WholeWord);
                if (found == -1) break;

                txShowArticle.SelectionStart = found;
                txShowArticle.SelectionLength = word.Length;
                txShowArticle.SelectionColor = color;
                pos = found + word.Length;
            }
            txShowArticle.SelectionStart = txShowArticle.TextLength;
        }
        #endregion

        #region 跳转功能
        /// <summary>
        /// 
        /// </summary>
        private void JumpToArticle(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("请输入文章ID或标题", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Article targetArticle = null;

            // 尝试按ID查找
            if (int.TryParse(input, out int id))
            {
                targetArticle = _articles[int.Parse(input)-1];
            }

            // 如果未找到ID，按标题搜索
            if (targetArticle == null)
            {
                var matchedArticles = _articles
                    .Where(a => a.Title.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (matchedArticles.Count > 1)
                {
                    // 多个匹配时弹出选择对话框
                    using (var selectionForm = new ReadPageSelectForm(matchedArticles))
                    {
                        if (selectionForm.ShowDialog() == DialogResult.OK)
                        {
                            targetArticle = selectionForm.SelectedArticle;
                        }
                    }
                }
                else if (matchedArticles.Count == 1)
                {
                    targetArticle = matchedArticles[0];
                }
            }

            // 处理查找结果
            if (targetArticle != null)
            {
                _currentArticleIndex = _articles.IndexOf(targetArticle);
                DisplayCurrentArticle();
            }
            else
            {
                MessageBox.Show($"未找到匹配文章：{input}", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 文章点击事件处理（点击单词高亮并显示释义）
        /// </summary>
        private void OnArticleClicked(object sender, MouseEventArgs e)
        {
            int clickPos = txShowArticle.GetCharIndexFromPosition(e.Location);
            string clickedWord = GetWordAtPosition(clickPos);

            if (string.IsNullOrEmpty(clickedWord)) return;

            SendMessage(txShowArticle.Handle, WM_SETREDRAW, false, 0);
            try
            {
                RenderArticleWithHighlight();
                ApplyColorHighlight(clickedWord, _clickHighlight);
                ShowWordDefinition(clickedWord);
                txShowArticle.SetScrollPosition(_scrollPosition);
                txShowArticle.SelectionStart = txShowArticle.TextLength; // 移动光标到末尾
                txShowArticle.SelectionLength = 0;
            }
            finally
            {
                SendMessage(txShowArticle.Handle, WM_SETREDRAW, true, 0);
                txShowArticle.Invalidate();
            }
        }

        /// <summary>
        /// 获取指定位置的完整单词
        /// </summary>
        /// <param name="position">字符位置</param>
        /// <returns>完整单词或null</returns>
        private string GetWordAtPosition(int position)
        {
            if (position < 0 || position >= txShowArticle.TextLength)
                return null;

            // 确定单词边界
            int start = position, end = position;
            while (start > 0 && IsWordChar(txShowArticle.Text[start - 1])) start--;
            while (end < txShowArticle.TextLength - 1 && IsWordChar(txShowArticle.Text[end + 1])) end++;

            return txShowArticle.Text.Substring(start, end - start + 1);
        }

        /// <summary>
        /// 判断字符是否属于单词字符（允许字母、数字、连字符和缩写符）
        /// </summary>
        private bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '\'' || c == '-';

        /// <summary>
        /// 显示单词详细信息
        /// </summary>
        /// <param name="word">目标单词</param>
        private void ShowWordDefinition(string word)
        {
            var wordData = _wordService.GetWordByName(word);
            if (wordData == null)
            {
                txWordDefinition.Text = $"未找到 {word} 的释义";
                return;
            }

            // 构建格式化显示内容
            var sb = new StringBuilder();
            sb.AppendLine($"【{wordData.WordName}】");
           
            if (!string.IsNullOrEmpty(wordData.BritishPhonetic))
                sb.AppendLine($"英音: /{wordData.BritishPhonetic}/");
            if (!string.IsNullOrEmpty(wordData.AmericanPhonetic))
                sb.AppendLine($"美音: /{wordData.AmericanPhonetic}/");
            sb.AppendLine($"释义: {wordData.Translation}");
            if (!string.IsNullOrEmpty(wordData.Examples))
                sb.AppendLine($"例句: {wordData.Examples}");

            txWordDefinition.Text = sb.ToString();
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 文章导航（上一篇/下一篇）
        /// </summary>
        /// <param name="direction">导航方向（1：下一篇，-1：上一篇）</param>
        private void NavigateArticle(int direction)
        {
            if (_articles.Count == 0) return;

            // 计算新索引（循环切换）
            int newIndex = (_currentArticleIndex + direction + _articles.Count) % _articles.Count;

            // 更新索引并刷新显示
            _currentArticleIndex = newIndex;
            DisplayCurrentArticle();

            // 新增：动态更新按钮状态
            btnPreviousArticle.Enabled = _articles.Count > 1;
            btnNextArticle.Enabled = _articles.Count > 1;
        }

        /// <summary>
        /// 导入文章文件
        /// </summary>
        private void ImportArticles(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Filter = "文本文件|*.txt|所有文件|*.*",
                Title = "选择文章文件"
            })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    // 创建新文章对象
                    var newArticle = new Article
                    {
                        Title = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName),
                        Content = System.IO.File.ReadAllText(dialog.FileName),
                        SourceUrl = dialog.FileName,
                        CreatedTime = DateTime.Now
                    };

                    _articleService.AddArticles(new List<Article> { newArticle });
                    LoadArticles();  // 重新加载文章列表
                    MessageBox.Show("文章导入成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导入失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 删除当前文章
        /// </summary>
        private void DeleteCurrentArticle(object sender, EventArgs e)
        {
            if (_currentArticleIndex < 0) return;

            var confirm = MessageBox.Show("确定删除当前文章？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    _articleService.DeleteArticle(_articles[_currentArticleIndex].Id);
                    LoadArticles();  // 重新加载文章列表
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除失败: {ex.Message}");
                }
            }
        }
        #endregion

        // === 双缓冲设置（减少界面闪烁）===
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // 启用WS_EX_COMPOSITED样式
                return cp;
            }
        }
    }

    /// <summary>
    /// RichTextBox滚动条控制扩展方法
    /// </summary>
    public static class RichTextBoxExtensions
    {
        // Win32 API声明
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        private const int SB_HORZ = 0x0;  // 水平滚动条
        private const int SB_VERT = 0x1;  // 垂直滚动条

        /// <summary>
        /// 获取当前滚动位置
        /// </summary>
        public static Point GetScrollPosition(this RichTextBox rtb)
        {
            return new Point(
                GetScrollPos(rtb.Handle, SB_HORZ),
                GetScrollPos(rtb.Handle, SB_VERT)
            );
        }

        /// <summary>
        /// 设置滚动位置
        /// </summary>
        public static void SetScrollPosition(this RichTextBox rtb, Point position)
        {
            SetScrollPos(rtb.Handle, SB_HORZ, position.X, true);
            SetScrollPos(rtb.Handle, SB_VERT, position.Y, true);
            rtb.Refresh();  // 强制刷新确保位置生效
        }
    }
}