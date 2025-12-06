using System;
using System.IO;
using System.Windows.Forms;
using DictData;
using DictData.definition;
using MaterialSkin.Controls;
namespace LoveEnglish
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.materialTabControl = new MaterialSkin.Controls.MaterialTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.searchBox = new MaterialSkin.Controls.MaterialTextBox2();
            this.mainContainer = new System.Windows.Forms.Panel();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.materialTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // materialTabControl
            // 
            this.materialTabControl.Controls.Add(this.tabPage1);
            this.materialTabControl.Controls.Add(this.tabPage6);
            this.materialTabControl.Controls.Add(this.tabPage3);
            this.materialTabControl.Controls.Add(this.tabPage5);
            this.materialTabControl.Controls.Add(this.tabPage2);
            this.materialTabControl.Controls.Add(this.tabPage4);
            this.materialTabControl.Depth = 0;
            this.materialTabControl.ImageList = this.imageList;
            this.materialTabControl.Location = new System.Drawing.Point(-4, 67);
            this.materialTabControl.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabControl.Multiline = true;
            this.materialTabControl.Name = "materialTabControl";
            this.materialTabControl.SelectedIndex = 0;
            this.materialTabControl.Size = new System.Drawing.Size(965, 647);
            this.materialTabControl.TabIndex = 15;
            this.materialTabControl.SelectedIndexChanged += new System.EventHandler(this.materialTabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.searchBox);
            this.tabPage1.Controls.Add(this.mainContainer);
            this.tabPage1.ImageKey = "home.png";
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(957, 620);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "首页";
            // 
            // searchBox
            // 
            this.searchBox.AnimateReadOnly = false;
            this.searchBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.searchBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.searchBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.searchBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.searchBox.Depth = 0;
            this.searchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.searchBox.HideSelection = true;
            this.searchBox.Hint = "Search";
            this.searchBox.LeadingIcon = ((System.Drawing.Image)(resources.GetObject("searchBox.LeadingIcon")));
            this.searchBox.Location = new System.Drawing.Point(80, 16);
            this.searchBox.MaxLength = 32767;
            this.searchBox.MouseState = MaterialSkin.MouseState.OUT;
            this.searchBox.Name = "searchBox";
            this.searchBox.PasswordChar = '\0';
            this.searchBox.PrefixSuffixText = null;
            this.searchBox.ReadOnly = false;
            this.searchBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.searchBox.SelectedText = "";
            this.searchBox.SelectionLength = 0;
            this.searchBox.SelectionStart = 0;
            this.searchBox.ShortcutsEnabled = true;
            this.searchBox.Size = new System.Drawing.Size(864, 48);
            this.searchBox.TabIndex = 0;
            this.searchBox.TabStop = false;
            this.searchBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.searchBox.TrailingIcon = null;
            this.searchBox.UseSystemPasswordChar = false;
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyDown);
            this.searchBox.Leave += new System.EventHandler(this.searchBox_Leave);
            // 
            // mainContainer
            // 
            this.mainContainer.Location = new System.Drawing.Point(80, 80);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Size = new System.Drawing.Size(864, 532);
            this.mainContainer.TabIndex = 1;
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.Color.White;
            this.tabPage6.ImageKey = "word_icon.png";
            this.tabPage6.Location = new System.Drawing.Point(4, 23);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(957, 620);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "背诵";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.White;
            this.tabPage3.ImageKey = "favorite.png";
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(957, 620);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "收藏";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.White;
            this.tabPage5.ImageKey = "ques_icon.png";
            this.tabPage5.Location = new System.Drawing.Point(4, 23);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(957, 620);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "练习";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.ImageKey = "book.png";
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(957, 620);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "阅读";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.White;
            this.tabPage4.ImageKey = "profile.png";
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(957, 620);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "我的";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "home.png");
            this.imageList.Images.SetKeyName(1, "book.png");
            this.imageList.Images.SetKeyName(2, "favorite.png");
            this.imageList.Images.SetKeyName(3, "profile.png");
            this.imageList.Images.SetKeyName(4, "return.png");
            this.imageList.Images.SetKeyName(5, "ques_icon.png");
            this.imageList.Images.SetKeyName(6, "word_icon.png");
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(960, 720);
            this.Controls.Add(this.materialTabControl);
            this.DrawerBackgroundWithAccent = true;
            this.DrawerShowIconsWhenHidden = true;
            this.DrawerTabControl = this.materialTabControl;
            this.DrawerUseColors = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Sizable = false;
            this.Text = "LoveEnglish 词汇学习应用";
            this.materialTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        //private void dictionaryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    SwitchDictionary(dictionaryComboBox.SelectedItem.ToString());
        //}



        private void classifyButton_Click(object sender, EventArgs e)
        {
        }

        private void generatePlanButton_Click(object sender, EventArgs e)
        {
        }

        private void adjustFrequencyButton_Click(object sender, EventArgs e)
        {
        }

        //private void generateExercisesButton_Click(object sender, EventArgs e)
        //{
        //    GenerateExercises(questionSourceComboBox.SelectedItem.ToString(), questionTypeComboBox.SelectedItem.ToString());
        //}

        private void recommendButton_Click(object sender, EventArgs e)
        {
        }

        private void adjustStrategyButton_Click(object sender, EventArgs e)
        {
        }
        private MaterialSkin.Controls.MaterialTabControl materialTabControl;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private ImageList imageList;
        private MaterialSkin.Controls.MaterialTextBox2 searchBox;
        private Panel mainContainer;
        private TabPage tabPage5;
        private TabPage tabPage6;
    }
}