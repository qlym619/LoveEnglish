using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DictModel
{
    public class Word
    {
        public int Id { get; set; }
        public string WordName { get; set; } // 单词名称
        public string BritishPhonetic { get; set; } // 英式音标
        public string AmericanPhonetic { get; set; } // 美式音标
        public string Translation { get; set; } // 单词释义（中文）                                               
        public string Exchange { get; set; } // 时态复数等变换
        public string Examples { get; set; } // 例句
        public string ExampleTrans { get; set; }//例句翻译
        public string ExampleSource { get; set; }//例句来源
        public bool IsNewWord { get; set; } // 是否为生词
        public DateTime? NextReviewDate { get; set; } // 下次复习日期
        public bool IsFavorite { get; set; } // 是否收藏
        public int ErrorCount { get; set; } // 错误次数
        public string DictTag { get; set; }//属于的词库
        
        

        public Word()
        {
            Id = -1;
            WordName = BritishPhonetic = AmericanPhonetic = Translation = Examples = Exchange =ExampleTrans=ExampleSource= "";
            IsNewWord = true;
            NextReviewDate = DateTime.Today.AddDays(1); // 默认将复习时间设置为当前时间 + 1天
            IsFavorite = false;
            ErrorCount = 0;
            DictTag = "";
            //PartOfSpeech = "";
        }
    }
}