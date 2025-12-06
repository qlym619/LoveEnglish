using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictModel
{
    // 词库类
    public class Dictionary
    {
        public int Id { get; set; }
        public string DictName { get; set; } // 词库名称
        public int WordCount { get; set; }
        //public List<Word> Words { get; set; }
    }
}
