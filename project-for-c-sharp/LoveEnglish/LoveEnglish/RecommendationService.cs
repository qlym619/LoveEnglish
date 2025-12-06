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

namespace LoveEnglish
{
    internal class RecommendationService
    {

        public enum Recom { CET4, CET6, TOEFL, IELTS, GRE }

        private readonly Random _random = new Random();
        WordService _wordservice;
        UserService _userService;
        public int NowCheckInDays;
        private List<Word> recommendedWords;
        public Recom randomRecom;
        
        public event Action<Word> OnWordDetailRequested;

        public RecommendationService()
        {


            _userService = new UserService();
            _wordservice = new WordService();
           
            NowCheckInDays = _userService.GetCheckinDays(1);
        }

        public List<RecommendItem> GetRecommendations(int page)
        {
            return new List<RecommendItem> {
                NewSignInItem(),
                NewProgressItem(),
                NewVocabularyItem(),
                NewStreakItem()
            };
        }

        private RecommendItem NewSignInItem() => new RecommendItem
        {
            Type = RecommendType.SignIn,
            Title = "每日签到",
            Content = $"连续签到 {NowCheckInDays} 天",
            ActionText = "立即签到",
            AccentColor = Color.FromArgb(76, 175, 80)
        };

        private RecommendItem NewProgressItem() => new RecommendItem
        {
            Type = RecommendType.Progress,
            Title = "学习进度",
            Progress = 20,
            AccentColor = Color.FromArgb(33, 150, 243)
        };

        private RecommendItem NewVocabularyItem()
        {



            // 获取当前日期作为随机种子
            var today = DateTime.Today;
            int seed = today.Year * 10000 + today.Month * 100 + today.Day; // 组合成YYYYMMDD格式的数字

            // 创建基于日期的随机数生成器
            var dailyRandom = new Random(seed);
            randomRecom = (Recom)dailyRandom.Next(0, Enum.GetValues(typeof(Recom)).Length);
            recommendedWords = _wordservice.GetWordsByDictTag(randomRecom.ToString());
            // 生成单词列表
            var wordButtons = recommendedWords
                .OrderBy(w => dailyRandom.Next())
                .Take(Math.Min(10, recommendedWords.Count))
                .Select(word => CreateMaterialWordButton(word))
                .ToList();

            return new RecommendItem
            {
                Type = RecommendType.Vocabulary,
                Title = $"{randomRecom} 词汇推荐",
                WordControls = wordButtons,
                AccentColor = GetAccentColorByRecom(randomRecom)
            };
        }

       
        private Control CreateMaterialWordButton(Word word)
        {
            var btn = new MaterialButton
            {
                Text = word.WordName.ToLower(),
                Tag = word,
                Size = new Size(120, 40),
                Margin = new Padding(8, 4, 8, 4),
                Type = MaterialButton.MaterialButtonType.Outlined,
                UseAccentColor = true,
                Depth = 0,
                MouseState = MaterialSkin.MouseState.HOVER
                
            };

            btn.Click += (sender, e) => ShowWordDetail(word);
            btn.MouseEnter += (sender, e) => ((MaterialButton)sender).Depth = 1;
            btn.MouseLeave += (sender, e) => ((MaterialButton)sender).Depth = 0;

            return btn;
        }

        private void ShowWordDetail(Word word)
        {

            Console.WriteLine($"单词为{word.WordName},翻译为{word.Translation}");


            //跳转单词详细界面
            OnWordDetailRequested?.Invoke(word);

        }

        private Color GetAccentColorByRecom(Recom recom)
        {
            switch (recom)
            {
                case Recom.CET4: return Color.FromArgb(33, 150, 243);
                case Recom.CET6: return Color.FromArgb(0, 150, 136);
                case Recom.TOEFL: return Color.FromArgb(156, 39, 176);
                case Recom.IELTS: return Color.FromArgb(255, 152, 0);
                case Recom.GRE: return Color.FromArgb(244, 67, 54);
                default: return Color.FromArgb(156, 39, 176);
            }
        }

        private RecommendItem NewStreakItem()
        {
            return new RecommendItem
            {
                Type = RecommendType.Streak,
                Title = "连续打卡",
                Content = $"已连续学习 {NowCheckInDays} 天",
                Progress = 100,
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        public void BuildHeader(RoundPanel card, RecommendItem item)
        {
            var header = new RoundPanel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = item.AccentColor,
                NoBorder = true,
                Padding = Padding.Empty
            };

            var lblTitle = new Label
            {
                Text = item.Title,
                ForeColor = Color.Black,
                Font = FontLoader.GetFont(14f),
                Location = new Point(16, 8)
            };

            header.Controls.Add(lblTitle);
            card.Controls.Add(header);
        }

        public void BuildContent(RoundPanel card, RecommendItem item)
        {
            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 52, 16, 16)
            };

            switch (item.Type)
            {
                case RecommendType.SignIn:
                    content.Controls.Add(new MaterialLabel
                    {
                        Text = item.Content,
                        AutoSize = true,
                        Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
                    });
                    break;

                case RecommendType.Progress:
                    var progress = new MaterialProgressBar
                    {
                        Value = item.Progress,
                        Dock = DockStyle.Top,
                        Height = 10
                    };
                    content.Controls.Add(progress);
                    content.Controls.Add(new MaterialLabel
                    {
                        Text = $"已完成 {item.Progress}%",
                        AutoSize = true,
                        Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel),
                        Dock = DockStyle.Top,
                        Padding = new Padding(0, 0, 0, 8)
                    });
                    break;

                case RecommendType.Vocabulary:
                    var flowPanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoScroll = true,
                        Padding = new Padding(4),
                        Margin = new Padding(0)
                    };

                    foreach (Control control in item.WordControls)
                    {
                        flowPanel.Controls.Add(control);
                    }

                    content.Controls.Add(flowPanel);
                    break;

                case RecommendType.Streak:
                    content.Controls.Add(new MaterialLabel
                    {
                        Text = item.Content,
                        Dock = DockStyle.Fill,
                        Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
                    });
                    break;
            }

            card.Controls.Add(content);
        }

        public void BuildActionButton(RoundPanel card, RecommendItem item)
        {
            if (string.IsNullOrEmpty(item.ActionText))
                return;

            var btn = new MaterialButton
            {
                Text = item.ActionText,
                Dock = DockStyle.Bottom,
                Type = MaterialButton.MaterialButtonType.Contained,
                Size = new Size(0, 40),
                //Elevation = 2
            };

            if (item.Type == RecommendType.SignIn)
            {
                btn.Click += (sender, e) => {
                    int userId = 1;
                    User user = _userService.GetUser(1);
                    DateTime now = DateTime.Now;

                    if (user.LastLoginTime.HasValue && user.LastLoginTime.Value.Date == now.Date)
                    {
                        MessageBox.Show("今日已签到！", "签到提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        int newCheckInDays;
                        if (user.LastLoginTime.HasValue && user.LastLoginTime.Value.Date.AddDays(1) == now.Date)
                        {
                            newCheckInDays = user.CheckInDays + 1;
                        }
                        else
                        {
                            newCheckInDays = 1;
                        }

                        _userService.UpdateCheckinDays(userId, newCheckInDays);
                        _userService.UpdateLastLoginTime(userId);
                        NowCheckInDays = newCheckInDays;

                        MessageBox.Show($"连续签到 {newCheckInDays} 天，请再接再厉！", "签到成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 更新签到卡片内容
                        var contentPanel = card.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Fill);
                        var label = contentPanel?.Controls.OfType<MaterialLabel>().FirstOrDefault();
                        if (label != null)
                        {
                            label.Text = $"连续签到 {newCheckInDays} 天";
                        }
                        // 找到并更新Streak卡片
                        var parentControl = card.Parent;
                        if (parentControl != null)
                        {
                            foreach (Control control in parentControl.Controls)
                            {
                                if (control is RoundPanel streakCard && streakCard.Tag is RecommendItem streakItem && streakItem.Type == RecommendType.Streak)
                                {
                                    // 更新Streak卡片的内容
                                    streakItem.Content = $"已连续学习 {newCheckInDays} 天";

                                    // 清除原有内容并重建
                                    streakCard.Controls.Clear();
                                    BuildHeader(streakCard, streakItem);
                                    BuildContent(streakCard, streakItem);
                                    BuildActionButton(streakCard, streakItem);
                                }
                            }
                        }
                    }
                };
            }

           

            card.Controls.Add(btn);
        }
    }
}