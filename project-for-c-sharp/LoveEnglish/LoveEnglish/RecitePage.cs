using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DictData;
using DictData.definition;
using DictModel;
using MaterialSkin;
using MaterialSkin.Controls;
using static MaterialSkin.Controls.MaterialButton;
using System.Speech.Synthesis;

namespace LoveEnglish
{
    public partial class RecitePage : MaterialForm
    {
        private readonly IWordService _wordService;
        private UserService userFetcher = new UserService();
        private QuesService questionFetcher = new QuesService();
        public ArticleService articleFetcher = new ArticleService();
        private DictionaryService dictionaryFetcher = new DictionaryService();

        private List<Word> _wordList;
        private int _currentIndex = 0;
        private int _errorCount = 0;
        private ReciteMode _currentMode = ReciteMode.TranslationToWord;
        private bool _isAnswerShown = false;

        // 添加HideCaret API
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        // UI 控件
        private MaterialCard cardMain;
        private MaterialLabel lblQuestion;
        private MaterialLabel lblWordInfo;
        private MaterialTextBox txtAnswer;
        private MaterialButton btnSubmit;
        private MaterialButton btnShowAnswer;
        private MaterialButton btnNext;
        private MaterialLabel lblFeedback;
        private MaterialLabel lblProgress;
        private MaterialLabel lblErrorCount;
        private MaterialComboBox cmbMode;
        private MaterialComboBox cmbDictTag;
        private MaterialButton btnStart;
        private MaterialButton btnFavorite;
        //private MaterialButton btnResetDict;
        //
        private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();

        // 背诵模式枚举
        private enum ReciteMode
        {
            TranslationToWord, // 中译英
            WordToTranslation, // 英译中
            Listening // 听写模式
        }

        public RecitePage(IWordService wordService)
        {
            _wordService = wordService;
            InitializeComponent();
            InitializeUI();

            // 初始化语音合成器
            _synthesizer.SetOutputToDefaultAudioDevice();
            _synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
        }

        private void InitializeUI()
        {
            // 设置Material Design皮肤
            MaterialSkinManager.Instance.AddFormToManage(this);

            // 窗体设置
            this.Text = ""; // 清空标题
            this.FormBorderStyle = FormBorderStyle.None; // 去除窗体边框
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false; // 去除控制框（关闭、最小化、最大化按钮）
            this.Dock = DockStyle.Fill;// 让窗体大小自适应父容器

            // 主卡片容器
            cardMain = new MaterialCard()
            {
                Size = new Size(800, 600),
                Location = new Point(30, 0),
                BackColor = MaterialSkinManager.Instance.BackgroundColor
            };

            // 背诵模式选择
            cmbMode = new MaterialComboBox()
            {
                Hint = "选择背诵模式",
                Size = new Size(200, 40),
                Location = new Point(140, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMode.Items.AddRange(new object[] { "中译英", "英译中", "听写模式" });
            cmbMode.SelectedIndex = 0;

            // 词库选择
            cmbDictTag = new MaterialComboBox()
            {
                Hint = "选择词库",
                Size = new Size(200, 40),
                Location = new Point(390, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadDictTags();

            // 开始按钮
            btnStart = new MaterialButton()
            {
                Text = "开始背诵",
                Size = new Size(120, 40),
                Location = new Point(640, 30),
                Type = MaterialButtonType.Contained,
                UseAccentColor = true
            };
            btnStart.Click += BtnStart_Click;
           
            //btnResetDict.Click += BtnResetDict_Click;
            // 问题标签
            lblQuestion = new MaterialLabel()
            {
                Font = new Font(FontLoader.GetFontFamily(), 24),
                AutoSize = false,
                Size = new Size(700, 150),
                Location = new Point(120, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Default
            };

            // 单词信息标签(音标等)
            lblWordInfo = new MaterialLabel()
            {
                Font = new Font(FontLoader.GetFontFamily(), 14),
                AutoSize = false,
                Size = new Size(700, 50),
                Location = new Point(120, 260),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // 答案输入框
            txtAnswer = new MaterialTextBox()
            {
                Hint = "请输入答案",
                Size = new Size(420, 40),
                Location = new Point(250, 330),
                MaxLength = 100,
                Visible = false
            };
            // 添加光标控制相关事件
            txtAnswer.GotFocus += TxtAnswer_GotFocus;
            txtAnswer.KeyDown += TxtAnswer_KeyDown;
            txtAnswer.MouseDown += TxtAnswer_MouseDown;
            txtAnswer.TextChanged += TxtAnswer_TextChanged;
            txtAnswer.KeyDown += TxtAnswer_KeyDown;

            // 提交按钮
            btnSubmit = new MaterialButton()
            {
                Text = "提交",
                Size = new Size(120, 40),
                Location = new Point(440, 390),
                Type = MaterialButtonType.Outlined,
                UseAccentColor = true,
                Visible = false
            };
            btnSubmit.Click += BtnSubmit_Click;

            // 显示答案按钮
            btnShowAnswer = new MaterialButton()
            {
                Text = "显示答案",
                Size = new Size(120, 40),
                Location = new Point(250, 390),
                Type = MaterialButtonType.Outlined,
                Visible = false
            };
            btnShowAnswer.Click += BtnShowAnswer_Click;

            // 下一题按钮
            btnNext = new MaterialButton()
            {
                Text = "下一题",
                Size = new Size(120, 40),
                Location = new Point(610, 390),
                Type = MaterialButtonType.Outlined,
                Visible = false
            };
            btnNext.Click += BtnNext_Click;

            // 反馈标签
            lblFeedback = new MaterialLabel()
            {
                Font = new Font(FontLoader.GetFontFamily(), 14),
                AutoSize = false,
                Size = new Size(700, 50),
                Location = new Point(110, 450),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // 进度标签
            lblProgress = new MaterialLabel()
            {
                Location = new Point(50, 520),
                AutoSize = true,
                Visible = false
            };

            // 错误计数标签
            lblErrorCount = new MaterialLabel()
            {
                Location = new Point(780, 520),
                AutoSize = true,
                Visible = false
            };

            // 收藏按钮
            btnFavorite = new MaterialButton()
            {
                Text = "收藏",
                Size = new Size(120, 40),
                Location = new Point(440, 500),
                Type = MaterialButtonType.Outlined,
                UseAccentColor = true,
                Visible = false
            };
            btnFavorite.Click += BtnFavorite_Click;

            // 添加控件到卡片
            cardMain.Controls.AddRange(new Control[] {
                cmbMode, cmbDictTag, btnStart,
                lblQuestion, lblWordInfo, txtAnswer,
                btnSubmit, btnShowAnswer, btnNext,
                lblFeedback, lblProgress, lblErrorCount,
                btnFavorite
            });

            // 添加卡片到窗体
            this.Controls.Add(cardMain);
        }

        private void LoadDictTags()
        {
            cmbDictTag.Items.Clear();
            cmbDictTag.Items.Add("全部词库");

            // 获取所有词库标签
            var words = _wordService.GetAllWords();
            var tags = dictionaryFetcher.GetAllDictionaries();



            foreach (var tag in tags)
            {
                cmbDictTag.Items.Add(tag.DictName);
            }

            cmbDictTag.SelectedIndex = 0;
        }
          private void BtnStart_Click(object sender, EventArgs e)
        {

            btnStart.Visible = false;
            
            // 设置背诵模式
            _currentMode = (ReciteMode)cmbMode.SelectedIndex;

            // 获取选择的词库
            string selectedTag = cmbDictTag.SelectedIndex == 0 ? null : cmbDictTag.SelectedItem.ToString();

            // 加载单词
            _wordList = selectedTag == null ?
                _wordService.GetAllWords() :
                _wordService.GetWordsByDictTag(selectedTag);

            if (_wordList.Count == 0)
            {
                MessageBox.Show("选择的词库中没有单词！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 打乱单词顺序
            ShuffleWords();

            _currentIndex = 0;
            _errorCount = 0;

            // 显示背诵界面
            cmbMode.Enabled = false;
            cmbDictTag.Enabled = false;
            btnStart.Enabled = false;

            lblQuestion.Visible = true;
            lblWordInfo.Visible = true;
            txtAnswer.Visible = true;
            btnSubmit.Visible = true;
            btnShowAnswer.Visible = true;
            btnNext.Visible = true;
            lblFeedback.Visible = true;
            lblProgress.Visible = true;
            lblErrorCount.Visible = true;
            btnFavorite.Visible = true;

            ShowCurrentWord();

        }

        private void ShuffleWords()
        {
            Random rng = new Random();
            int n = _wordList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = _wordList[k];
                _wordList[k] = _wordList[n];
                _wordList[n] = value;
            }
        }

        private void ShowCurrentWord()
        {
            if (_currentIndex < 0 || _currentIndex >= _wordList.Count)
                return;

            var currentWord = _wordList[_currentIndex];
            _isAnswerShown = false;

            // 根据模式显示问题
            switch (_currentMode)
            {
                case ReciteMode.TranslationToWord:
                    lblQuestion.Text = currentWord.Translation;
                    txtAnswer.Hint = "请输入英文单词";
                    break;
                case ReciteMode.WordToTranslation:
                    lblQuestion.Text = currentWord.WordName;
                    txtAnswer.Hint = "请输入中文释义";
                    break;
                case ReciteMode.Listening:
                    lblQuestion.Text = "🔊 点击喇叭听取发音";
                    lblQuestion.Cursor = Cursors.Hand;
                    lblQuestion.Click += PlayPronunciation;
                    txtAnswer.Hint = "请输入您听到的单词";
                    break;
            }

            txtAnswer.Text = "";
            // 重置光标状态
            txtAnswer.SelectionStart = 0;
            HideCaret(txtAnswer.Handle);

            // 显示音标信息
            lblWordInfo.Text = $"英式音标：/{currentWord.BritishPhonetic ?? ""}/     美式音标：/{currentWord.AmericanPhonetic ?? ""}/";

            txtAnswer.Text = "";
            lblFeedback.Text = "";
            lblProgress.Text = $"{_currentIndex + 1}/{_wordList.Count}";
            lblErrorCount.Text = $"错误次数: {_errorCount}";

            // 更新收藏按钮状态
            UpdateFavoriteButton(currentWord.IsFavorite);

            txtAnswer.Focus();
        }

        private void UpdateFavoriteButton(bool isFavorite)
        {
            btnFavorite.Text = isFavorite ?
            "已收藏" :
            "收藏";
        }

        private void BtnFavorite_Click(object sender, EventArgs e)
        {
            if (_currentIndex >= _wordList.Count)
                return;

            var currentWord = _wordList[_currentIndex];
            bool newFavoriteState = !currentWord.IsFavorite;

            // 更新数据库
            _wordService.UpdataFavById(currentWord.Id, newFavoriteState);

            // 更新本地数据
            currentWord.IsFavorite = newFavoriteState;

            // 更新按钮状态
            UpdateFavoriteButton(newFavoriteState);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            CheckAnswer();
        }

        private void TxtAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            // 禁用方向键和Home/End键
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Home:
                case Keys.End:
                    e.SuppressKeyPress = true;
                    break;
            }
            //Enter提交
            if (e.KeyCode == Keys.Enter)
            {
                CheckAnswer();
                e.SuppressKeyPress = true;
            }
        }

        private void TxtAnswer_GotFocus(object sender, EventArgs e)
        {
            // 隐藏光标
            HideCaret(txtAnswer.Handle);
        }

        private void TxtAnswer_MouseDown(object sender, MouseEventArgs e)
        {
            // 禁止鼠标改变光标位置
            txtAnswer.SelectionStart = txtAnswer.Text.Length;
            HideCaret(txtAnswer.Handle);
        }

        private void TxtAnswer_TextChanged(object sender, EventArgs e)
        {
            // 强制光标始终保持在文本末尾
            txtAnswer.SelectionStart = txtAnswer.Text.Length;
            HideCaret(txtAnswer.Handle);
        }

        private void PlayPronunciation(object sender, EventArgs e)
        {
            if (_currentMode == ReciteMode.Listening && _currentIndex < _wordList.Count)
            {
                var currentWord = _wordList[_currentIndex];
                _synthesizer.SpeakAsync(currentWord.WordName);
            }
        }

        private void CheckAnswer()
        {
            if (_currentIndex >= _wordList.Count || _isAnswerShown)
                return;

            var currentWord = _wordList[_currentIndex];
            string userAnswer = txtAnswer.Text.Trim();
            bool isCorrect = false;

            // 根据模式检查答案
            switch (_currentMode)
            {
                case ReciteMode.TranslationToWord:
                    isCorrect = string.Equals(userAnswer, currentWord.WordName, StringComparison.OrdinalIgnoreCase);
                    break;
                case ReciteMode.WordToTranslation:
                    isCorrect = !string.IsNullOrEmpty(userAnswer) &&
                                currentWord.Translation.Contains(userAnswer);
                    break;
                case ReciteMode.Listening:
                    isCorrect = string.Equals(userAnswer, currentWord.WordName, StringComparison.OrdinalIgnoreCase);
                    break;
            }

            if (isCorrect)
            {
                // 回答正确
                lblFeedback.Text = "✓ 正确！";
                lblFeedback.ForeColor = Color.Green;

                // 如果单词是生词，标记为非生词
                if (currentWord.IsNewWord)
                {
                    _wordService.UpdateIsNewWord(currentWord.WordName, false);
                    currentWord.IsNewWord = false;
                }

                // 延迟1秒后进入下一题
                Timer timer = new Timer();
                timer.Interval = 1000;
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    MoveToNextWord();
                };
                timer.Start();
            }
            else
            {
                // 回答错误
                lblFeedback.Text = $"✗ 错误！正确答案: {(_currentMode == ReciteMode.TranslationToWord ? currentWord.WordName : currentWord.Translation)}";
                lblFeedback.ForeColor = Color.Red;

                // 增加错误计数
                _errorCount++;
                lblErrorCount.Text = $"错误: {_errorCount}";

                // 更新数据库中的错误计数
                _wordService.IncrementErrorCount(currentWord.WordName);

                // 标记为已显示答案
                _isAnswerShown = true;

                // 重新输入
                txtAnswer.Focus();
                txtAnswer.SelectAll();
            }
        }

        private void BtnShowAnswer_Click(object sender, EventArgs e)
        {
            if (_currentIndex >= _wordList.Count || _isAnswerShown)
                return;

            var currentWord = _wordList[_currentIndex];

            lblFeedback.Text = $"正确答案: {(_currentMode == ReciteMode.TranslationToWord ? currentWord.WordName : currentWord.Translation)}";
            lblFeedback.ForeColor = Color.Blue;

            // 标记为已显示答案
            _isAnswerShown = true;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            MoveToNextWord();
        }

        private void MoveToNextWord()
        {
            _currentIndex++;

            if (_currentIndex < _wordList.Count)
            {
                ShowCurrentWord();
            }
            else
            {
                // 完成所有单词
                lblQuestion.Text = "恭喜！你已经完成了所有单词！";
                lblWordInfo.Visible = false;
                txtAnswer.Visible = false;
                btnSubmit.Visible = false;
                btnShowAnswer.Visible = false;
                btnNext.Visible = false;
                lblFeedback.Visible = false;
                btnFavorite.Visible = false;

                // 允许重新选择
                cmbMode.Enabled = true;
                cmbDictTag.Enabled = true;
                btnStart.Enabled = true;
                btnStart.Text = "重新开始";
                //btnResetDict.Visible = true;
            }
        }

        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 移除点击事件防止重复绑定
            lblQuestion.Click -= PlayPronunciation;
            lblQuestion.Cursor = Cursors.Default;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _synthesizer.Dispose();
            base.OnFormClosing(e);
        }



      
    }
}