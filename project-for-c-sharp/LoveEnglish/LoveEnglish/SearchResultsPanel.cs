using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using DictModel;
using System.Data.Entity.Core.EntityClient;

using MaterialSkin;
using MaterialSkin.Controls;

namespace LoveEnglish {
    internal class SearchResultsPanel : Panel, IMessageFilter {
        private readonly MainForm _mainForm;
        private FlowLayoutPanel _resultsFlowPanel;
        private const int verticalPadding = 8;
        private const int maxVisibleResults = 8;
        private const int resultHeight = 32;

        private readonly float fontSize = 12f;

        public SearchResultsPanel(MainForm mainForm) {
            _mainForm = mainForm;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponents();
            Application.AddMessageFilter(this);
        }

        private void InitializeComponents() {
            this.Visible = false;
            this.AutoScroll = true;
            this.MaximumSize = new Size(856, 
                maxVisibleResults * (resultHeight + verticalPadding) + verticalPadding);
            this.BackColor = SystemColors.Control;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.HorizontalScroll.Visible = false;
            this.HorizontalScroll.Enabled = false;
            this.Padding = new Padding(0, verticalPadding, 0, verticalPadding);

            _resultsFlowPanel = new FlowLayoutPanel {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0, 4, 0, 4)
            };

            this.Controls.Add(_resultsFlowPanel);
        }

        public void ShowResults(IEnumerable<Word> results, Control anchorControl) {
            _resultsFlowPanel.Controls.Clear();
            Application.AddMessageFilter(this);

            for (int i = 0; i < results.Count<Word>(); i++) {
                Word word = results.ElementAt(i);

                var item = new RoundLabel {
                    Text = $"{word.WordName} - {word.Translation}",
                    Tag = word,
                    Margin = new Padding(10, 3, 10, 3),
                    Font = FontLoader.GetFont(fontSize),
                    Size = new Size(810, resultHeight),
                    BackColor = SystemColors.Control,
                    HoverBackColor = Color.Gray,
                    ForeColor = SystemColors.ControlText,
                    Cursor = Cursors.Hand,
                    NoBorder = true,
                    CornerRadius = 0
                };

                if (i == 0) {
                    item.Font = FontLoader.GetFont(fontSize, FontStyle.Bold);
                }

                SetTextWithEllipsis(item, $"{word.WordName} - {word.Translation}");

                item.Click += (s, e) => {
                    HideResults();
                    _mainForm.ShowSearchResults(word);
                    Console.WriteLine($"wordname: {word.WordName}");
                };

                _resultsFlowPanel.Controls.Add(item);
            }

            _resultsFlowPanel.PerformLayout();
            this.PerformLayout();
            UpdatePanelSize(anchorControl);
            this.BringToFront();
            this.Visible = true;
        }

        private void UpdatePanelSize(Control anchorControl) {
            if (anchorControl == null)
                return;

            int itemCount = _resultsFlowPanel.Controls.Count;
            int contentHeight = itemCount * (resultHeight + verticalPadding);
            int maxHeight = maxVisibleResults * (resultHeight + verticalPadding) + 10;

            this.Size = new Size(anchorControl.Width,
                contentHeight > maxHeight ? maxHeight : contentHeight);

            Point screenLocation = anchorControl.Parent.PointToScreen(anchorControl.Location);
            Point formLocation = this.Parent.PointToClient(screenLocation);
            this.Location = new Point(formLocation.X, formLocation.Y + anchorControl.Height + 2);
        }

        public void HideResults() {
            this.Visible = false;
            _resultsFlowPanel.Controls.Clear();
            Application.RemoveMessageFilter(this);
            this.Refresh();
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

        public bool PreFilterMessage(ref Message m) {
            const int WM_LBUTTONDOWN = 0x201;
            if (m.Msg == WM_LBUTTONDOWN && this.Visible) {
                Point screenPoint = new Point(m.LParam.ToInt32() & 0xFFFF, m.LParam.ToInt32() >> 16);

                bool isClickInside = this.ClientRectangle.Contains(screenPoint);
                if (!isClickInside) {
                    HideResults();
                }
            }
            return false;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Application.RemoveMessageFilter(this);
            }
            base.Dispose(disposing);
        }
    }
}
