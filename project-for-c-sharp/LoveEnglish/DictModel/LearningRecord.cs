using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictModel
{
    // 学习记录类
    public class LearningRecord
    {
        public Word Word { get; set; }
        public DateTime LearnDate { get; set; }
        public bool IsCorrect { get; set; } // 本次学习是否正确

    }
}
