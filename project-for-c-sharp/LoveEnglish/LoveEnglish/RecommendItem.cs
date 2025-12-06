using DictModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum RecommendType {
    SignIn,
    Progress,
    Vocabulary,
    Streak,
    Challenge
}

namespace LoveEnglish {
    internal class RecommendItem {
        public int Order { get; set; }      // 显示顺序
        public RecommendType Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Progress { get; set; }   // 0-100的进度值
        public DateTime? DueDate { get; set; } // 截止日期（用于挑战类）
        public List<Word> Words { get; set; }
        public List<Control> WordControls { get; set; } // 生词按钮列表
        public string ActionText { get; set; }  // 操作按钮文字
        public Color AccentColor { get; set; }  // 强调色
    }
}
