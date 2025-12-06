using DictModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LoveEnglish
{
    public partial class ReadPageSelectForm : Form
    {
        public Article SelectedArticle { get; private set; }

        public ReadPageSelectForm(List<Article> articles)
        {
            InitializeComponents(articles);
        }

        private void InitializeComponents(List<Article> articles)
        {
            this.Text = "选择文章";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                DisplayMember = "Title",
                DataSource = articles
            };

            var btnOK = new Button { Text = "确定", DialogResult = DialogResult.OK };
            btnOK.Click += (s, e) =>
            {
                SelectedArticle = listBox.SelectedItem as Article;
                this.Close();
            };

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            panel.Controls.Add(btnOK);

            this.Controls.Add(listBox);
            this.Controls.Add(panel);
        }
    }
}