using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DictData;
using DictData.definition;
using DictModel;
using MaterialSkin;
using MaterialSkin.Controls;
using static MaterialSkin.Controls.MaterialButton;

namespace LoveEnglish
{
    public partial class PersonForm : MaterialForm
    {
        private readonly WordService _wordService;
        private DictionaryService dictionaryFetcher = new DictionaryService();

        private MaterialCard cardDict;
        private MaterialLabel lblDict;
        private MaterialListView lvDict;
        private MaterialButton btnAddDict;

        private MaterialCard cardFavorite;
        private MaterialLabel lblFavorite;
        private MaterialLabel lblFavoriteCount;
        private MaterialLabel lblEncourage;
        private MaterialListView lvFavoriteWords;

        // 新增：错题模块控件
        private MaterialCard cardMistakes;
        private MaterialLabel lblMistakes;
        private MaterialLabel lblMistakesCount;
        private MaterialListView lvMistakes;

        // 新增：刷新按钮
        private MaterialButton btnRefresh;
        public event Action VocabularyImported;

        private RecitePage _recitePage;
        public PersonForm()
        {
            _wordService = new WordService();
            InitializeComponent();
            InitializeUI();
            LoadDictTags();
            UpdateFavoriteCount();
            UpdateMistakesCount();
        }

        private void InitializeUI()
        {
            MaterialSkinManager.Instance.AddFormToManage(this);

            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.Dock = DockStyle.Fill;


            int moduleWidth = 200;
            Console.WriteLine($"{moduleWidth}");

            // 词典库模块
            cardDict = new MaterialCard()
            {
                Size = new Size(moduleWidth, 300),
                Location = new Point(30, 30),
                BackColor = MaterialSkinManager.Instance.BackgroundColor
            };

            // 词典库标题
            lblDict = new MaterialLabel()
            {
                Text = "词典库",
                Font = new Font(FontLoader.GetFontFamily(), 20, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(moduleWidth - 100, 40),
                Location = new Point(50, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 设置为主题强调色
            lblDict.ForeColor = MaterialSkinManager.Instance.ColorScheme.AccentColor;

            // 添加阴影效果
            var shadowDict = new MaterialLabel()
            {
                Text = "词典库",
                Font = new Font(FontLoader.GetFontFamily(), 20, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(moduleWidth - 100, 40),
                Location = new Point(52, 27),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(50, 0, 0, 0)
            };
            cardDict.Controls.Add(shadowDict);
            cardDict.Controls.Add(lblDict);

            lvDict = new MaterialListView()
            {
                Size = new Size(moduleWidth - 100, 250),
                Location = new Point(50, 150),
                FullRowSelect = true,
                HideSelection = false
            };


            // 设置词典库列表视图的列
            lvDict.Columns.Add("词典名称", 150);
            lvDict.Columns.Add("单词数量", 100);
            lvDict.View = View.Details;



            btnAddDict = new MaterialButton()
            {
                Text = "添加词库",
                Size = new Size(120, 40),
                Location = new Point((moduleWidth) / 2 + 15, 400),
                Type = MaterialButtonType.Contained,
                UseAccentColor = true
            };
            btnAddDict.Click += BtnAddDict_Click;

            cardDict.Controls.AddRange(new Control[] { lblDict, lvDict, btnAddDict });

            // 收藏单词模块
            cardFavorite = new MaterialCard()
            {
                Size = new Size(moduleWidth, 300),
                Location = new Point(moduleWidth + 50, 30),
                BackColor = MaterialSkinManager.Instance.BackgroundColor
            };

            // 收藏单词标题
            lblFavorite = new MaterialLabel()
            {
                Text = "收藏单词",
                Font = new Font(FontLoader.GetFontFamily(), 20, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(moduleWidth - 100, 40),
                Location = new Point(50, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblFavorite.ForeColor = MaterialSkinManager.Instance.ColorScheme.AccentColor;

            var shadowFavorite = new MaterialLabel()
            {
                Text = "收藏单词",
                Font = new Font(FontLoader.GetFontFamily(), 20, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(moduleWidth - 100, 40),
                Location = new Point(52, 27),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(50, 0, 0, 0)
            };
            cardFavorite.Controls.Add(shadowFavorite);
            cardFavorite.Controls.Add(lblFavorite);

            lblFavoriteCount = new MaterialLabel()
            {
                Font = new Font(FontLoader.GetFontFamily(), 20),
                AutoSize = true,
                Location = new Point(50, 90)
            };

            lblEncourage = new MaterialLabel()
            {
                Font = new Font(FontLoader.GetFontFamily(), 20),
                Text = "继续加油，你可以的！",
                AutoSize = true,
                Location = new Point(50, 115)
            };

            // 添加收藏单词列表视图
            lvFavoriteWords = new MaterialListView()
            {
                Size = new Size(moduleWidth - 100, 300),
                Location = new Point(50, 150),
                FullRowSelect = true,
                HideSelection = false
            };

            // 设置收藏单词列表视图的列
            lvFavoriteWords.Columns.Add("单词", 100);
            lvFavoriteWords.Columns.Add("释义", 150);
            lvFavoriteWords.View = View.Details;

            cardFavorite.Controls.AddRange(new Control[] { lblFavorite, lblFavoriteCount, lblEncourage, lvFavoriteWords });

            // 新增：错题模块
            cardMistakes = new MaterialCard()
            {
                Size = new Size(moduleWidth, 300),
                Location = new Point(moduleWidth * 2 + 70, 30),
                BackColor = MaterialSkinManager.Instance.BackgroundColor
            };

            // 高频错题标题
            lblMistakes = new MaterialLabel()
            {
                Text = "高频错题",
                Font = new Font(FontLoader.GetFontFamily(), 20, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(moduleWidth - 100, 40),
                Location = new Point(50, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            // 使用强调色的深色变体
            Color darkAccent = ControlPaint.Dark(MaterialSkinManager.Instance.ColorScheme.AccentColor);
            lblMistakes.ForeColor = darkAccent;

            var shadowMistakes = new MaterialLabel()
            {
                Text = "高频错题",
                Font = new Font(FontLoader.GetFontFamily(), 20, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(moduleWidth - 100, 40),
                Location = new Point(52, 27),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(50, 0, 0, 0)
            };
            cardMistakes.Controls.Add(shadowMistakes);
            cardMistakes.Controls.Add(lblMistakes);

            lblMistakesCount = new MaterialLabel()
            {
                Font = new Font(FontLoader.GetFontFamily(), 20),
                AutoSize = true,
                Location = new Point(50, 90)
            };

            lvMistakes = new MaterialListView()
            {
                Size = new Size(moduleWidth - 100, 200),
                Location = new Point(50, 150),
                FullRowSelect = true,
                HideSelection = false
            };

            // 设置错题列表视图的列
            lvMistakes.Columns.Add("单词", 80);
            lvMistakes.Columns.Add("释义", 120);
            lvMistakes.Columns.Add("次数", 60);
            lvMistakes.View = View.Details;

            cardMistakes.Controls.AddRange(new Control[] { lblMistakes, lblMistakesCount, lvMistakes });

            // 分割线
            MaterialDivider divider1 = new MaterialDivider()
            {
                Location = new Point(moduleWidth + 30, 30),
                Size = new Size(10, 300)
            };

            MaterialDivider divider2 = new MaterialDivider()
            {
                Location = new Point(moduleWidth * 2 + 50, 30),
                Size = new Size(10, 300)
            };

            // 新增：刷新按钮
            btnRefresh = new MaterialButton()
            {
                Text = "刷新",
                Size = new Size(80, 40),
                Location = new Point(moduleWidth - 100, 100),
                Type = MaterialButtonType.Contained,
                UseAccentColor = true
            };
            btnRefresh.Click += BtnRefresh_Click;

            this.Controls.AddRange(new Control[] { cardDict, cardFavorite, cardMistakes, divider1, divider2, btnRefresh });
        }

        private void LoadDictTags()
        {
            lvDict.Items.Clear();

            var dictionaries = dictionaryFetcher.GetAllDictionaries();
            foreach (var dict in dictionaries)
            {
                ListViewItem item = new ListViewItem(dict.DictName);
                item.SubItems.Add(dict.WordCount.ToString());
                lvDict.Items.Add(item);
            }
        }

        private void UpdateFavoriteCount()
        {
            int favoriteCount = _wordService.GetFavoriteWordCount();
            lblFavoriteCount.Text = $"收藏单词个数: {favoriteCount}";

            // 加载收藏的单词
            LoadFavoriteWords();
        }

        private void LoadFavoriteWords()
        {
            lvFavoriteWords.Items.Clear();

            var favoriteWords = _wordService.GetFavoriteWords();
            foreach (var word in favoriteWords)
            {
                ListViewItem item = new ListViewItem(word.WordName);
                item.SubItems.Add(word.Translation);
                lvFavoriteWords.Items.Add(item);
            }
        }

        // 新增：更新错题统计
        private void UpdateMistakesCount()
        {
            
            List<int> mistakeWordIds = _wordService.GetWordIdsByErrorCountGreaterThan(3);
            lblMistakesCount.Text = $"高频错题个数: {mistakeWordIds.Count}";

            
            LoadMistakeWords(mistakeWordIds);
        }

        // 新增：加载错题单词
        private void LoadMistakeWords(List<int> wordIds)
        {
            lvMistakes.Items.Clear();

            foreach (int wordId in wordIds)
            {
                Word word = _wordService.GetWordById(wordId);
                if (word != null)
                {
                    ListViewItem item = new ListViewItem(word.WordName);
                    item.SubItems.Add(word.Translation);
                    item.SubItems.Add(word.ErrorCount.ToString());
                    lvMistakes.Items.Add(item);
                }
            }
        }

        private void BtnAddDict_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV文件 (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileExtension = Path.GetExtension(openFileDialog.FileName).ToLower();
                if (fileExtension == ".csv")
                {
                    try
                    {
                       
                        string dictTag = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        ImportCustomVocabulary(openFileDialog.FileName, dictTag);
                        LoadDictTags();
                        UpdateFavoriteCount();
                        UpdateMistakesCount(); 

                        VocabularyImported?.Invoke();
                        //RefreshRecitePage();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"导入词库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("请选择.csv格式的文件", "文件格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void ImportCustomVocabulary(string filePath, string dictTag)
        {
            
            if (!File.Exists(filePath))
            {
                MessageBox.Show("指定的文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            string fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension != ".csv")
            {
                MessageBox.Show("请选择.csv格式的文件", "文件格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
               
                List<Word> wordList = _wordService.ParseCsvFile(filePath);

                if (wordList == null || wordList.Count == 0)
                {
                    MessageBox.Show("CSV文件中没有有效数据", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                _wordService.AddWords(wordList, dictTag);

               
                MessageBox.Show($"成功导入{wordList.Count}个单词", "导入完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Dictionary dictionary = new Dictionary();
                dictionary.DictName = dictTag;
                dictionary.WordCount = wordList.Count;
                dictionaryFetcher.AddDictionary(dictionary);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入词库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDictTags();
            UpdateFavoriteCount();
            UpdateMistakesCount();
        }
    }
}