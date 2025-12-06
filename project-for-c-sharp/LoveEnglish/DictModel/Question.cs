using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictModel
{
    // 练习题类
    public class Question
    {
        public int Id { get; set; }

        public int WordId { get; set; }
        public Word Word { get; set; }
        public string Type { get; set; } // 题型，如选择题、拼写匹配
        public string Content { get; set; }

        public string Answer { get; set; }//答案


    }
}
