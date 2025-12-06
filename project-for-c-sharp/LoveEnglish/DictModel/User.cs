using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictModel
{
     public class User
    {
        public int Id { set; get; }//用户id
        //public string UserName { set; get; }//用户名
       // public string Account { set; get; }//登录用账户
        //public string Password { set; get; }//登录用密码
        //public List<int> UserNewWords { set; get; }//存放该用户新单词id的列表
        //public List<int> UserFavoriteWords { set; get; }//存放该用户收藏单词id的列表
        public int CheckInDays { set; get; } //连续打卡日期
        //public Dictionary<int, DateTime> WordReviewSchedule { get; set; } //单词复习计划（单词ID-下次复习时间）
        //public Dictionary<int, int> WordErrorCounts { get; set; } //单词错误次数统计（单词ID-错误次数）
        public DateTime? LastLoginTime { get; set; } // 最后登录时间
        //public int DailyLearningGoal { get; set; } // 每日学习目标（单词数）
    }
}
