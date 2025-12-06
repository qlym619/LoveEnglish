using System;
using System.Collections.Generic;
using DictModel;


//目前只考虑做签到功能，无登录功能


namespace DictData
{
    public interface IUserService
    {

        User GetUser(int userId);//获取用户数据
        int GetCheckinDays(int userId);// 获取打卡天数
        bool UpdateCheckinDays(int userId, int checkInDays);//更新连续打卡的天数
        bool UpdateLastLoginTime(int userId);  // 更新最后登录时间


    }
}
