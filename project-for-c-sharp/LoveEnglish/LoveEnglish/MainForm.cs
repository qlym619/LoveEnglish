using DictData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DictModel;

using MaterialSkin;
using MaterialSkin.Controls;
using MaterialSkin.Animations;
using MaterialSkin.Properties;
using System.Reflection;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing.Text;
using System.IO;
using static MaterialSkin.Controls.MaterialButton;
using System.Runtime.Remoting.Channels;

using DictData.definition;

using System.Diagnostics;
using System.Collections;


namespace LoveEnglish {
    public partial class MainForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        private int currentPage = 0;
        private bool isLoading = false;
        private List<Word> selfWordList;

        //private NotificationManager _notificationManager;


        //数据库接口
        public WordService wordFetcher = new WordService();
        private UserService userFetcher = new UserService();
        private QuesService questionFetcher = new QuesService();
        private ArticleService articleFetcher = new ArticleService();
        private DictionaryService dictionaryFetcher = new DictionaryService();

        public NotificationManager notifyManager;
        // TabPage 1
        private readonly Stack<RoundPanel> _cardPool = new Stack<RoundPanel>();
        private const int MaxPoolSize = 20;
        private RecommendationService dataService = new RecommendationService();
        private SearchResultsPanel searchResultsPanel;
        private WordDetailPanel wordDetailPanel;

        // TabPage 2

        // TabPage 3
        private FrontFavPanel frontFavPanel;
        private FavPanel favPanel;
        private FavButton favButton;

        // Instance
        private static MainForm _instance;
        public static MainForm Instance {
            get {
                if (_instance == null) {
                    _instance = new MainForm();
                }
                return _instance;
            }
        }


        /// <summary>
        /// ReadPage
        /// </summary>

        private ReadPage readPage;
        private RecitePage recitePage;


        public MainForm() {

            InitializeComponent();
            FontLoader.LoadHarmonyFont();
            var fontFamily = FontLoader.GetFontFamily();
            this.Font = new Font(fontFamily, 12);
            ApplyFontSettings(this);

            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue800,
                Primary.Blue300, Accent.Orange200,
                TextShade.WHITE
            );
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.AddFormToManage(this);

            LoadReadPage();
            LoadRecitePage();
            LoadPersonalPage();
            LoadExercisePage();
            InitializeUI();
        
        }

        private void InitializeUI() {
            mainContainer.BackColor = SystemColors.Control;
            tabPage1.BackColor = SystemColors.Control;
            tabPage2.BackColor = SystemColors.Control;
            tabPage3.BackColor = SystemColors.Control;
            tabPage4.BackColor = SystemColors.Control;
            tabPage5.BackColor = SystemColors.Control;

            this.Size = new Size(960, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            notifyManager = new NotificationManager(this);

            // TabPage 1

            mainContainer.Layout += (s, e) => new WaterfallLayout().Layout(mainContainer, e);
            mainContainer.Scroll += MainContainer_Scroll;

            LoadCards(dataService.GetRecommendations(0));

            searchResultsPanel = new SearchResultsPanel(this);
            tabPage1.Controls.Add(searchResultsPanel);

            wordDetailPanel = new WordDetailPanel(materialSkinManager);
            tabPage1.Controls.Add(wordDetailPanel);

            wordDetailPanel.OnBackClicked += HandleBack;
            wordDetailPanel.OnFavoriteClicked += HandleFavorite;

            searchBox.BringToFront();
            searchResultsPanel.BringToFront();

            dataService.OnWordDetailRequested += (word) =>
            {
                // 调用显示单词详情的方法
                ShowSearchResults(word);
            };


            // TabPage 3

            frontFavPanel = new FrontFavPanel(this);
            tabPage3.Controls.Add(frontFavPanel);

            favPanel = new FavPanel(this);
            favPanel.SetFrontFavPanel(frontFavPanel);
            tabPage3.Controls.Add(favPanel);

            favButton = new FavButton(this);
            favPanel.SetFavButton(favButton);

            //TabPage 4
            //LoadDictionariesToComboBox();



        }

        public void LoadReadPage()
        {
            //加载readpage到tabpage2 
            ReadPage readPage = new ReadPage(articleFetcher, wordFetcher);
            readPage.TopLevel = false; // 设置为非顶级窗体
            readPage.AutoScroll = true; // 启用自动滚动
            this.tabPage2.Controls.Add(readPage); // 将 ReadPage 添加到 tabPage2
            readPage.Dock = DockStyle.Fill;
            readPage.Show();
        }

        public void LoadRecitePage()
        {
            if (recitePage != null && !recitePage.IsDisposed)
            {
                recitePage.Dispose();
                tabPage6.Controls.Clear();
            }
            //TabPage 6
            recitePage = new RecitePage(wordFetcher);
           
            recitePage.TopLevel = false;
            recitePage.AutoScroll = true;
            this.tabPage6.Controls.Add(recitePage);
            recitePage.Dock = DockStyle.Fill;
            recitePage.Show();


        }

        public void LoadPersonalPage()
        {
            
            PersonForm personalPage = new PersonForm();

            
            personalPage.TopLevel = false;      
            personalPage.AutoScroll = true;

            personalPage.VocabularyImported += OnVocabularyImported;
            this.tabPage4.Controls.Clear();     
            this.tabPage4.Controls.Add(personalPage);
            personalPage.Dock = DockStyle.Fill;

           
            personalPage.Show();
        }

        private void OnVocabularyImported()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    LoadRecitePage();
                   // ShowNotification();
                }));
            }
            else
            {
                LoadRecitePage();
                //ShowNotification();
            }
        }
        public void AddToTabPage3(Control control) {
            tabPage3.Controls.Add(control);
        }

        //private void LoadDictionariesToComboBox()
        //{
        //    List<Dictionary> dictionaries = dictionaryFetcher.GetAllDictionaries();
        //    foreach (var dictionary in dictionaries)
        //    {
        //        selfCombox.Items.Add(dictionary.DictName);
        //    }
        //}

        public void LoadExercisePage()
        {
            tabPage5.Padding = new Padding(-30, 0, 0, 0); // 向左补偿30px

            ExercisePage exercisePage = new ExercisePage();
            exercisePage.TopLevel = false;
            exercisePage.Dock = DockStyle.Fill;

            // 强制左边界对齐
            exercisePage.Left = -30; // 向左偏移30px

            tabPage5.Controls.Clear();
            tabPage5.Controls.Add(exercisePage);
            exercisePage.Show();
        }

        private async void LoadMoreCards() {
            if (isLoading)
                return;
            isLoading = true;

            var loadingCard = CreateLoadingCard();
            mainContainer.Controls.Add(loadingCard);

            await Task.Delay(800); // 模拟网络请求
            LoadCards(dataService.GetRecommendations(++currentPage));

            mainContainer.Controls.Remove(loadingCard);
            isLoading = false;
        }

        private MaterialCard CreateLoadingCard() {
            return new MaterialCard {
                Height = 80,
                Tag = int.MaxValue,
                Controls = { new CircularProgress {
                    Location = new Point((mainContainer.Width - 40) / 2, 20)
                }}
            };
        }

        private void LoadCards(List<RecommendItem> items) {
            mainContainer.SuspendLayout();
            try {
                items.ForEach(item => {
                    var card = CreateCard(item);
                    mainContainer.Controls.Add(card);
                });
            } finally {
                currentPage = 1;
                mainContainer.ResumeLayout(true);
            }
        }

        private RoundPanel CreateCard(RecommendItem item) {
            int baseHeight = 200; // 基础高度
            switch (item.Type) {
                case RecommendType.SignIn:
                    baseHeight = 100;
                    break;
                case RecommendType.Vocabulary:
                    baseHeight = 350; // 词汇推荐卡片更高
                    break;
                case RecommendType.Progress:
                    baseHeight = 150; // 进度条卡片稍矮
                    break;
                    
            }

            RoundPanel card = new RoundPanel {
                MinimumSize = new Size(300, baseHeight),
                MaximumSize = new Size(400, baseHeight + 50),
                Margin = new Padding(10),
                Tag = item
            };

            card.Controls.Clear();
            dataService.BuildHeader(card, item);
            dataService.BuildContent(card, item);
            dataService.BuildActionButton(card, item);

            return card;
        }

        private void MainContainer_Scroll(object sender, ScrollEventArgs e) {
            var panel = (Panel) sender;
            if (panel.VerticalScroll.Value + panel.Height > panel.VerticalScroll.Maximum - 100) {
                Task.Run(() => {
                    // this.Invoke(new Action(LoadMoreCards));
                });
            }
        }

        // ====================================

        private void ApplyFontSettings(Control parentControl) {
            foreach (Control ctrl in parentControl.Controls) {
                // 跳过MaterialTextBox2及其子类
                if (ctrl is MaterialSkin.Controls.MaterialTextBox2) {
                    continue;
                }

                float fontSize = 12f;
                try {
                    ctrl.Font = FontLoader.GetFont(fontSize, ctrl.Font.Style);
                } catch {
                    ctrl.Font = new Font(SystemFonts.DefaultFont.FontFamily, fontSize);
                }

                // 递归处理子控件
                if (ctrl.HasChildren) {
                    ApplyFontSettings(ctrl);
                }
            }
        }


     

       

    
       
      

        private void openFileDialog_FileOk(object sender, CancelEventArgs e) {

        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                string keyword = searchBox.Text.Trim();

                // MessageBox.Show("keyword = " + keyword);
                if (!string.IsNullOrEmpty(keyword))
                {
                    List<Word> results = wordFetcher.GetWordsByFuzzyName(keyword);

                    // 优先排序：完全匹配 keyword 的单词放首位
                    results = results
                        .OrderByDescending(word => word.WordName.Equals(keyword, StringComparison.OrdinalIgnoreCase))
                        .ThenBy(word => word.WordName)  // 其他按原顺序或自定义规则排序
                        .ToList();

                    searchResultsPanel.ShowResults(results, searchBox);
                }
            }
        }

        private void searchBox_Leave(object sender, EventArgs e) {
            if (searchResultsPanel.Visible) {
                searchResultsPanel.HideResults();
            }
        }

        public void ShowSearchResults(Word word) {
            mainContainer.SuspendLayout();
            try {
                DisposeAllRoundPanels(mainContainer);
                currentPage = 0;
                mainContainer.Hide();
                // LogControlTree(mainContainer);
                wordDetailPanel.ShowWordDetails(word);
            } finally {
                mainContainer.ResumeLayout(true);
                mainContainer.Refresh();
            }
        }

        private void DisposeAllRoundPanels(Control parent) {
            foreach (Control control in parent.Controls.OfType<Control>().ToList()) {
                if (control is RoundPanel roundPanel) {
                    parent.Controls.Remove(roundPanel);
                    roundPanel.Dispose();

                    // Debug.WriteLine($"已销毁: {roundPanel.Name ?? "未命名RoundPanel"}");
                } else {
                    DisposeAllRoundPanels(control);
                }
            }
        }

        private void HandleBack() {
            /* 在单词详细界面中点击退出 */
            mainContainer.SuspendLayout();
            try {
                mainContainer.Show();
                mainContainer.Invalidate();
                if (currentPage == 0) {
                    LoadCards(dataService.GetRecommendations(0));
                }
            } finally {
                mainContainer.ResumeLayout(true);
                mainContainer.Refresh();
                // LogControlTree(mainContainer);
            }
        }

      


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //ReadPage

        public bool GetFavStatusById(int id) {
            Word thisWord = wordFetcher.GetWordById(id);
            return thisWord.IsFavorite;
        }

        private void HandleFavorite(Object sender, FavEventArgs f) {
            /* 在单词详细界面中点击收藏本单词 */
            // update 20250508 进行了一个取消收藏的功能
            int favId = f.WordId;

            bool modify = true;
            if (GetFavStatusById(favId)) {
                modify = false;

            }
            wordFetcher.UpdataFavById(favId, modify);


            Console.WriteLine($"该单词的 id：{favId}，更新后为 {modify}");


          // _notificationManager.Enqueue("操作成功", NotificationType.Success);

        }

        private void materialTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (this.materialTabControl.SelectedIndex != 0) {
                // 如果当前选中的不是第一个 TabPage，则隐藏搜索结果面板
                searchResultsPanel.HideResults();
                wordDetailPanel.HideDetails();
                HandleBack();
            }

            if (this.materialTabControl.SelectedIndex == 2) // 索引 2 对应 tabPage3
            {
                if (favPanel != null && !favPanel.IsDisposed) {
                    // Console.WriteLine("重载tabpage");
                    favPanel.LoadUserFavorites();
                }
            }
            

           
        }
        private void selfCombox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ReturnToMainForm()
        {
            // 返回
            if (readPage != null && !readPage.IsDisposed)
            {
                // 更新主窗体位置
                this.Location = readPage.Location;
                readPage.Hide();
            }

            this.Show();
        }

        // 主窗体关闭时清理资源
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (readPage != null && !readPage.IsDisposed)
            {
                readPage.Close();
            }
            while (_cardPool.Count > 0) {
                _cardPool.Pop().Dispose();
            }
        }

        

        //private void importBtn_Click(object sender, EventArgs e)
        //{
        //    // 设置文件过滤器，只显示CSV文件
        //    openFileDialog.Filter = "CSV文件 (*.csv)|*.csv";

        //    if (openFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        // 双重验证：既通过过滤器限制，又检查扩展名
        //        string fileExtension = Path.GetExtension(openFileDialog.FileName).ToLower();

        //        if (fileExtension == ".csv")
        //        {
        //            try
        //            {
        //                ImportCustomVocabulary(openFileDialog.FileName);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"导入词库时发生错误: {ex.Message}", "错误",
        //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("请选择.csv格式的文件", "文件格式错误",
        //                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //}





        // For Debug
        private void LogControlTree(Control parent, int indent = 0) {
            Console.WriteLine(new string(' ', indent * 2) + parent.GetType().Name);
            foreach (Control child in parent.Controls) {
                LogControlTree(child, indent + 1);
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                cp.ExStyle |= 0x00080000; // WS_CLIPCHILDREN
                return cp;
            }
        }
    }
}