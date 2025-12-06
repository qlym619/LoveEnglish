using System;
using System.Collections.Generic;
using DictModel;


namespace DictData
{
    public interface IQuesService
    {
        List<Question> GetQuestionsByWordId(int wordId);//根据单词的id获得所有与其相关的问题
        void DisplayQuestionProperties(Question question);//控制台展示question所有属性
        List<Question> GetQuestionsByType(string questionType);//根据问题类型获得所有该类型的问题
        Question GetQuestionById(int questionId);//根据问题id获取


    }
}
