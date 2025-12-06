using DictData;
using DictModel;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static MaterialSkin.Controls.MaterialButton;

namespace LoveEnglish
{
    public partial class ExercisePage : MaterialForm
    {
        // 组件声明（Designer通常自动生成的部分）
        //private IContainer components;
        private FlowLayoutPanel flowPanel;
        private MaterialButton btnSubmit;
        //private MaterialLabel lblProgress;
        //private MaterialLabel lblScore;
        private Panel statusPanel;
        private Panel questionContainer;

        private readonly IQuesService _quesService = new QuesService();
        private List<Question> _fillInQuestions;
        private readonly MaterialSkinManager materialSkinManager;

        public ExercisePage()
        {
            // 在ExercisePage构造函数中添加

            // 必须先调用InitializeComponent
            InitializeComponent();
            this.MinimumSize = new Size(800, 600); // 设置最小尺寸防止挤压
            this.Padding = new Padding(10, 0, 10, 0); // 左右对称边距
            // 初始化Material Design
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600,
                Primary.Blue800,
                Primary.Blue300,
                Accent.Orange200,
                TextShade.WHITE
            );

            LoadQuestions();
            BuildDynamicUI();
        }

        // 手动实现的InitializeComponent
        private void InitializeComponent()
        {
            // ===== 窗体基础设置 =====
            this.Text = string.Empty;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.Dock = DockStyle.Fill;
            

            // ===== 容器初始化 =====
            // 主容器（替代原来的questionContainer）
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(50, 10, 20, 10) // 对称边距
            };

            // 流程面板（题目列表）
            this.flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(0, 0, 15, 0), // 补偿滚动条宽度
                Margin = new Padding(30, 0, 0, 0) // 额外添加左侧边距
            };

            // 提交按钮
            this.btnSubmit = new MaterialButton
            {
                Text = "提交答案",
                Dock = DockStyle.Bottom,
                Height = 50,
                Margin = new Padding(30, 20, 0, 0),
                Type = MaterialButtonType.Contained
            };

            // ===== 布局组合 =====
            mainContainer.Controls.Add(this.flowPanel);
            mainContainer.Controls.Add(this.btnSubmit);
            this.Controls.Add(mainContainer);

            // ===== 尺寸自适应处理 =====
            mainContainer.Resize += (s, e) => AdjustLayout();
            this.Load += (s, e) => AdjustLayout();

            
        }
        private void AdjustLayout()
        {
            // 动态计算题目卡片宽度
            int cardWidth = this.flowPanel.ClientSize.Width - 80; // 留出滚动条空间

            foreach (Control c in this.flowPanel.Controls)
            {
                if (c is MaterialCard card)
                {
                    card.Width = cardWidth;
                    card.Margin = new Padding(30, 10, 10, 10); // 左侧增加30p
                    foreach (Control inner in card.Controls)
                    {
                        if (inner is MaterialTextBox2 txt)
                        {
                            txt.Width = cardWidth - 60; // 输入框宽度同步调整
                            txt.Location = new Point(40, txt.Location.Y); // 输入框右移20px
                        }
                    }
                }
            }
        }
        private void LoadQuestions()
        {
            try
            {
                _fillInQuestions = _quesService.GetQuestionsByType("Fill-in");
                if (_fillInQuestions == null || _fillInQuestions.Count == 0)
                {
                    MessageBox.Show("没有找到填空题！");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载题目失败: {ex.Message}");
                this.Close();
            }
        }

        private void BuildDynamicUI()
        {
            // 动态生成题目内容
            foreach (var question in _fillInQuestions)
            {
                var questionPanel = new MaterialCard
                {
                    Width = 700,
                    Height = 120,
                    Margin = new Padding(30, 10, 10, 10), // 左间距增大
                    Tag = question.Id,
                    MinimumSize = new Size(550, 120)
                };

                var lblQuestion = new MaterialLabel
                {
                    Text = $"{_fillInQuestions.IndexOf(question) + 1}. {question.Content}",
                    Location = new Point(40, 40),
                    AutoSize = true,
                    Font = FontLoader.GetFont(14)
                };

                var txtAnswer = new MaterialTextBox2
                {
                    Hint = "在此输入答案",
                    Width = 400,
                    Location = new Point(20, lblQuestion.Bottom + 15),
                    Tag = question
                };

                questionPanel.Controls.Add(lblQuestion);
                questionPanel.Controls.Add(txtAnswer);
                flowPanel.Controls.Add(questionPanel);
            }

            // 绑定事件
            this.flowPanel.SizeChanged += (s, e) => AdjustLayout();
            this.btnSubmit.Click += SubmitAnswers;
            this.Load += (s, e) => UpdateProgress();
        }

        private void SubmitAnswers(object sender, EventArgs e)
        {
            int score = 0;
            foreach (MaterialCard panel in flowPanel.Controls)
            {
                var txtBox = panel.Controls[1] as MaterialTextBox2;
                var question = _fillInQuestions.Find(q => q.Id == (int)panel.Tag);

                if (string.Equals(txtBox.Text.Trim(), question.Answer, StringComparison.OrdinalIgnoreCase))
                {
                    score++;
                    txtBox.BackColor = Color.LightGreen;
                }
                else
                {
                    txtBox.BackColor = Color.LightPink;
                }
            }

            //lblScore.Text = $"得分: {score}/{_fillInQuestions.Count}";
            //lblProgress.Text = $"进度: {_fillInQuestions.Count}/{_fillInQuestions.Count}";

            MessageBox.Show($"练习完成！\n正确率: {score}/{_fillInQuestions.Count} ({score / (double)_fillInQuestions.Count:P2})");
        }

        private void UpdateProgress()
        {
            //lblProgress.Text = $"进度: {_fillInQuestions.Count}/{_fillInQuestions.Count}";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // 清理资源
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}