using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using DictModel;
using MaterialSkin;
using MaterialSkin.Controls;
using MaterialSkin.Properties;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Speech.Synthesis;
using Label = System.Windows.Forms.Label;

namespace LoveEnglish {
    internal class WordDetailPanel : Panel {
        private const int verticalMargin = 20;
        private const int lineSpacing = 4;
        private FlowLayoutPanel contentPanel;
        private readonly MaterialSkinManager skinManager;

        private readonly SpeechSynthesizer speechSynthesizer;

        private MaterialButton btnBack;
        private MaterialButton btnFavorite;
        public event Action OnBackClicked;
        public event EventHandler<FavEventArgs> OnFavoriteClicked;

        private int currentWordId;
        private bool isFavorited;

        private readonly float fontType1 = 20f;
        private readonly float fontType2 = 12f;
        private readonly float fontType3 = 10f;

        public WordDetailPanel(MaterialSkinManager skinManager) {

            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            // 可以设置语音速度、音量等
            speechSynthesizer.Rate = 0; // -10 到 10
            speechSynthesizer.Volume = 100;


            this.skinManager = skinManager;
            InitializePanel();
            InitializeContentPanel();
        }

        private void InitializePanel() {
            Location = new Point(80, 80);
            Size = new Size(864, 532);
            BackColor = SystemColors.Control;
            Visible = false;
            AutoScroll = true;
            BorderStyle = BorderStyle.None;
            Padding = new Padding(verticalMargin);
            HorizontalScroll.Enabled = false;
            HorizontalScroll.Visible = false;

            btnBack = new MaterialButton {
                Text = "返回",
                Icon = LoveEnglish.Properties.Resources.return_icon,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(80, 36),
                Location = new Point(Width - 116, 10),
                Type = MaterialButton.MaterialButtonType.Text
            };
            btnBack.Click += (s, e) => {
                HideDetails();
                OnBackClicked?.Invoke();
            };
            Controls.Add(btnBack);

            btnFavorite = new MaterialButton {
                Text = "收藏",
                Icon = LoveEnglish.Properties.Resources.favorite_icon,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(80, 36),
                Location = new Point(Width - 116, 82)
            };
            btnFavorite.Click += (s, e) => {
                isFavorited = !isFavorited;
                // 又有bug
                 if (isFavorited) {
                    MainForm.Instance.notifyManager.Enqueue(
                        "已收藏单词", NotificationType.Success);
                } else {
                    MainForm.Instance.notifyManager.Enqueue(
                        "已取消收藏单词", NotificationType.Warning);
                }
                UpdateFavoriteButtonColor();
                OnFavoriteClicked?.Invoke(this,
                    new FavEventArgs(currentWordId));
            };
            Controls.Add(btnFavorite);

            btnBack.BringToFront();
            btnFavorite.BringToFront();
        }

        private void InitializeContentPanel() {
            contentPanel = new FlowLayoutPanel {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = this.ClientSize.Width - 2 * verticalMargin - 5,
                Padding = new Padding(16, 0, 16, 16)
            };
            Controls.Add(contentPanel);
        }

        public void ShowWordDetails(Word word) {
            currentWordId = word.Id;
            isFavorited = MainForm.Instance.GetFavStatusById(word.Id);
            UpdateFavoriteButtonColor();

            contentPanel.Controls.Clear();
            CreateWordNameLabel(word);
            CreatePhoneticLabels(word);
            CreateTranslationLabel(word);
            CreateExamplesLabel(word);
            CreateExchangeLabel(word);
            Visible = true;
            BringToFront();
        }

        private void UpdateFavoriteButtonColor() {
            if (isFavorited) {
                btnFavorite.UseAccentColor = true;
            } else {
                btnFavorite.UseAccentColor = false;
            }
        }

        private void CreateWordNameLabel(Word word)
        {
            var nameLabel = new Label
            {
                Text = word.WordName,
                Font = FontLoader.GetFont(fontType1, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.Black,
                Height = 64,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            contentPanel.Controls.Add(nameLabel);

            var dividerPanel = new Panel
            {
                Height = 2,
                Width = 804,
                ForeColor = Color.FromArgb(32, 32, 32, 32),
                BackColor = Color.FromArgb(32, 32, 32, 32)
            };
            contentPanel.Controls.Add(dividerPanel);
        }

        //private void CreatePhoneticLabels(Word word)
        //{
        //    var parallelPanel = new FlowLayoutPanel
        //    {
        //        AutoSize = true,
        //        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        //        FlowDirection = FlowDirection.LeftToRight,
        //        WrapContents = false,
        //        Width = this.ClientSize.Width - 2 * verticalMargin - 5
        //    };

        //    var britishPanel = new RoundPanel
        //    {
        //        Height = 36,
        //        BackColor = Color.LightGray,
        //        HoverBackColor = Color.LightGray,
        //        Padding = new Padding(4, 4, 4, 4),
        //        NoBorder = true
        //    };

        //    var americanPanel = new RoundPanel
        //    {
        //        Height = 36,
        //        BackColor = Color.LightGray,
        //        HoverBackColor = Color.LightGray,
        //        Padding = new Padding(4, 4, 4, 4),
        //        NoBorder = true
        //    };


        //    var britishLabel = new Label
        //    {
        //        Text = $"英 {word.BritishPhonetic}",
        //        Font = FontLoader.GetFont(fontType2),
        //        Dock = DockStyle.Fill,
        //        ForeColor = Color.Black,
        //        BackColor = Color.Transparent,
        //        AutoSize = false,
        //        TextAlign = ContentAlignment.MiddleLeft
        //    };
        //    Graphics bg = britishLabel.CreateGraphics();
        //    SizeF bLength = bg.MeasureString(britishLabel.Text, britishLabel.Font);

        //    var americanLabel = new Label
        //    {
        //        Text = $"美 {word.AmericanPhonetic}",
        //        Font = FontLoader.GetFont(fontType2),
        //        Dock = DockStyle.Fill,
        //        ForeColor = Color.Black,
        //        BackColor = Color.Transparent,
        //        AutoSize = false,
        //        TextAlign = ContentAlignment.MiddleLeft
        //    };
        //    Graphics ag = americanLabel.CreateGraphics();
        //    SizeF aLength = ag.MeasureString(americanLabel.Text, americanLabel.Font);

        //    britishPanel.Width = (int)bLength.Width + 12;
        //    americanPanel.Width = (int)aLength.Width + 12;

        //    britishPanel.Controls.Add(britishLabel);
        //    americanPanel.Controls.Add(americanLabel);
        //    parallelPanel.Controls.Add(britishPanel);
        //    parallelPanel.Controls.Add(americanPanel);
        //    contentPanel.Controls.Add(parallelPanel);
        //    contentPanel.Controls.Add(new Panel { Height = 4 });
        //}
        private void CreatePhoneticLabels(Word word)
        {
            var parallelPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Width = this.ClientSize.Width - 2 * verticalMargin - 5
            };

            // 英式发音部分
            var britishContainer = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 0, 16, 0)
            };

            var britishPanel = new RoundPanel
            {
                Height = 36,
                BackColor = Color.LightGray,
                HoverBackColor = Color.LightGray,
                Padding = new Padding(4, 4, 4, 4),
                NoBorder = true
            };

            var britishLabel = new Label
            {
                Text = $"英 {word.BritishPhonetic}",
                Font = FontLoader.GetFont(fontType2),
                Dock = DockStyle.Fill,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Graphics bg = britishLabel.CreateGraphics();
            SizeF bLength = bg.MeasureString(britishLabel.Text, britishLabel.Font);
            britishPanel.Width = (int)bLength.Width + 12;
            britishPanel.Controls.Add(britishLabel);

           
            // 英式发音按钮 - 修正后的版本
            var btnBritishSpeak = new MaterialButton
            {
               
                Size = new Size(36, 36),
                Type = MaterialButton.MaterialButtonType.Text,
                Margin = new Padding(4, 0, 0, 0)
            };
            btnBritishSpeak.Text = "▶";

            btnBritishSpeak.Click += (s, e) => SpeakWord(word.WordName, VoiceGender.Female, VoiceAge.Adult, 2);

            britishContainer.Controls.Add(britishPanel);
            britishContainer.Controls.Add(btnBritishSpeak);

            // 美式发音部分
            var americanContainer = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            var americanPanel = new RoundPanel
            {
                Height = 36,
                BackColor = Color.LightGray,
                HoverBackColor = Color.LightGray,
                Padding = new Padding(4, 4, 4, 4),
                NoBorder = true
            };
            string americanPhonetic = string.IsNullOrEmpty(word.AmericanPhonetic)
            ? word.BritishPhonetic
              : word.AmericanPhonetic;
            var americanLabel = new Label
            {
                Text = $"美 {americanPhonetic}",
                Font = FontLoader.GetFont(fontType2),
                Dock = DockStyle.Fill,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnBritishSpeak.UseAccentColor = true;
            
            Graphics ag = americanLabel.CreateGraphics();
            SizeF aLength = ag.MeasureString(americanLabel.Text, americanLabel.Font);
            americanPanel.Width = (int)aLength.Width + 12;
            americanPanel.Controls.Add(americanLabel);
            
            // 美式发音按钮
            var btnAmericanSpeak = new MaterialButton
            {
                
                Size = new Size(36, 36),
                Type = MaterialButton.MaterialButtonType.Text,
                Margin = new Padding(4, 0, 0, 0)
            };
            btnAmericanSpeak.Text = "▶";
            btnAmericanSpeak.Click += (s, e) => SpeakWord(word.WordName, VoiceGender.Female, VoiceAge.Adult, 1);

            americanContainer.Controls.Add(americanPanel);
            americanContainer.Controls.Add(btnAmericanSpeak);
            btnAmericanSpeak.UseAccentColor = true;
            // 添加到主面板
            parallelPanel.Controls.Add(britishContainer);
            parallelPanel.Controls.Add(americanContainer);
            contentPanel.Controls.Add(parallelPanel);
            contentPanel.Controls.Add(new Panel { Height = 4 });
        }
        private void SpeakWord(string word, VoiceGender gender, VoiceAge age, int culture)
        {
            try
            {
                Console.WriteLine("发音ing");
                
                speechSynthesizer.SpeakAsyncCancelAll();

                
                var voices = speechSynthesizer.GetInstalledVoices()
                    .Where(v => v.Enabled &&
                           (culture == 1 ? v.VoiceInfo.Culture.Name.StartsWith("en-US") :
                            v.VoiceInfo.Culture.Name.StartsWith("en-GB")))
                    .ToList();

                if (voices.Count > 0)
                {
                    speechSynthesizer.SelectVoice(voices[0].VoiceInfo.Name);
                }

                // 异步发音
                speechSynthesizer.SpeakAsync(word);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"发音错误: {ex.Message}");
            }
        }

        private void CreateTranslationLabel(Word word) {
            if (string.IsNullOrEmpty(word.Translation))
                return;

            var verticalPanel = new FlowLayoutPanel {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = this.ClientSize.Width - 2 * verticalMargin - 5
            };
            var translations = word.Translation.Split(new[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var trans in translations) {
                var parts = SplitTranslation(trans);

                var horizontalPanel = new FlowLayoutPanel {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 4),
                    WrapContents = false
                };

                if (!string.IsNullOrEmpty(parts.Item1)) {
                    var posLabel = new Label {
                        Text = parts.Item1,
                        ForeColor = Color.Gray,
                        Font = FontLoader.GetFont(fontType2, FontStyle.Italic),
                        Margin = new Padding(0, 0, 8, 0),
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    horizontalPanel.Controls.Add(posLabel);
                }

                var contentLabel = new Label {
                    Text = parts.Item2,
                    ForeColor = Color.Black,
                    Font = FontLoader.GetFont(fontType2),
                    AutoSize = true,
                    MaximumSize = new Size(verticalPanel.Width - 100, 0)
                };

                horizontalPanel.Controls.Add(contentLabel);
                verticalPanel.Controls.Add(horizontalPanel);
            }

            contentPanel.Controls.Add(verticalPanel);
        }

        private void CreateExchangeLabel(Word word) {
            if (string.IsNullOrEmpty(word.Exchange)) {
                return;
            }

            var verticalPanel = new FlowLayoutPanel {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = this.ClientSize.Width - 2 * verticalMargin - 5
            };

            var exchanges = word.Exchange.Split(new[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var exc in exchanges) {
                var parts = SplitExchange(exc);

                var horizontalPanel = new FlowLayoutPanel {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 4),
                    WrapContents = false
                };

                if (!string.IsNullOrEmpty(parts.Item1)) {
                    var posLabel = new Label {
                        Text = parts.Item1,
                        ForeColor = Color.Gray,
                        Font = FontLoader.GetFont(fontType2),
                        Margin = new Padding(0, 0, 8, 0),
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    horizontalPanel.Controls.Add(posLabel);
                }

                var contentLabel = new Label {
                    Text = parts.Item2,
                    ForeColor = Color.Black,
                    Font = FontLoader.GetFont(fontType2),
                    AutoSize = true,
                    MaximumSize = new Size(verticalPanel.Width - 100, 0)
                };

                horizontalPanel.Controls.Add(contentLabel);
                verticalPanel.Controls.Add(horizontalPanel);
            }

            contentPanel.Controls.Add(verticalPanel);
        }

        private void CreateExamplesLabel(Word word) {
            if (string.IsNullOrEmpty(word.Examples) ||
                string.IsNullOrEmpty(word.ExampleTrans) ||
                string.IsNullOrEmpty(word.ExampleSource))
                return;

            var examplePanel = new RoundPanel {
                Height = 36,
                Width = 71,
                BackColor = Color.LightGray,
                HoverBackColor = Color.LightGray,
                Padding = new Padding(4, 4, 4, 4),
                NoBorder = true,
                HalfHeight = true
            };

            var exampleLabel = new Label {
                Text = "○ 例句",
                Font = FontLoader.GetFont(fontType2),
                Dock = DockStyle.Fill,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            examplePanel.Controls.Add(exampleLabel);
            contentPanel.Controls.Add(examplePanel);

            var examples = word.Examples.Split('\n');
            var exampleTrans = word.ExampleTrans.Split('\n');
            var exampleSource = word.ExampleSource.Split('\n');

            for (int i = 0; i < examples.Length; i++) {
                if (string.IsNullOrWhiteSpace(examples[i]))
                    continue;

                AddLabel($"{examples[i].Trim()}",
                        fontType2,
                        Color.Black, lineSpacing / 2);

                if (i < exampleTrans.Length && !string.IsNullOrEmpty(exampleTrans[i])) {
                    AddLabel($"{exampleTrans[i].Trim()}",
                            fontType2,
                            Color.Black, lineSpacing / 2);
                }

                if (i < exampleSource.Length && !string.IsNullOrEmpty(exampleSource[i])) {
                    AddLabel($"{exampleSource[i].Trim()}",
                            fontType3,
                            Color.Gray, lineSpacing / 2);
                }

                contentPanel.Controls.Add(new Panel { Height = 8 });
            }
        }

        private void AddLabel(string text, float fontType,
                            Color color, int marginBottom) {
            var label = new Label {
                Text = text,
                ForeColor = color,
                Font = FontLoader.GetFont(fontType),
                Margin = new Padding(0, 0, 0, marginBottom),
                AutoSize = true,
                MaximumSize = new Size(832, 0),
                Padding = new Padding(4, 0, 0 ,0)
            };

            contentPanel.Controls.Add(label);
        }

        public void HideDetails() {
            Visible = false;
            contentPanel.Controls.Clear();
        }

        private Tuple<string, string> SplitTranslation(string translation) {
            // 匹配词性 x.
            var regex = new Regex(@"^([a-zA-Z]+\.?)\s+");
            var match = regex.Match(translation);

            if (match.Success) {
                var pos = match.Groups[1].Value.Trim();
                var content = translation.Substring(match.Length).Trim();

                // 过滤掉类似"1."的数字序号
                if (Regex.IsMatch(pos, @"^\d+\.$")) {
                    return Tuple.Create("", translation.Trim());
                }
                
                return Tuple.Create(pos, content);
            }

            // 无匹配时返回完整内容
            return Tuple.Create("", translation.Trim());
        }

        private Tuple<string, string> SplitExchange(string exchange) {
            // 正则匹配冒号分隔的格式（允许前后空格）
            var regex = new Regex(@"^\s*([^:]+?)\s*:\s*(.+?)\s*$");
            var match = regex.Match(exchange);

            if (match.Success) {
                return Tuple.Create(
                    match.Groups[1].Value.Trim(),   // 词性部分（如“名词复数”）
                    match.Groups[2].Value.Trim()    // 内容部分（如“apples”）
                );
            }

            // 无冒号时返回完整字符串作为内容部分
            return Tuple.Create("", exchange.Trim());
        }
    }
}
