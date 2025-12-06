using System;
using System.Collections.Generic;
using DictModel;

namespace DictData
{
    public interface IWordService
    {
        void DisplayWordProperties(Word word);//控制台显示所有信息
        List<Word> GetAllWords();//获得所有单词
        Word GetWordByName(string wordName);//根据单词名获得单词信息
        List<Word> GetFavoriteWords();//获得收藏的单词列表
        int GetWordIdByName(string wordName);//根据单词名称获得单词
        Word GetWordById(int id);//根据单词的id获得单词信息
        List<Word> GetWordsByDictTag(string keyword);//根据字典标签的内容获得单词列表，比如属于根据needTag"CET4"，可以获得所有带有四级标签的单词
        List<int> GetFavoriteWordIds();//获得所有收藏单词的id，返回一个列表
        List<int> GetNewWordIds();//获得所有新单词的id，返回一个列表
        List<Word> GetWordsByFuzzyName(string keyword);//模糊查找单词名称，返回列表
        List<Word> GetWordsByFuzzyTranslation(string keyword); //根据翻译模糊查找单词,返回单词列表
        List<string> GetAllWordNames();//获得所有单词名称，返回string列表
        List<int> GetWordIdsByErrorCount(int errorCount);//根据错误次数查找单词，返回id列表
        List<int> GetWordIdsByNextReviewDate(System.DateTime nextReviewDate);//根据下次复习时间查找单词，返回单词id列表
        List<int> GetWordIdsByTranslation(string translation);//根据翻译查找单词，返回单词id
        List<int> GetWordIdsByErrorCountGreaterThan(int minErrorCount);//获取错误次数大于min的id列表



        //以上为查找函数
        //-------------------------------------------------------------------------------------------------------------
        //一下为修改函数




        List<Word> ParseCsvFile(string csvFilePath);//解析csv文件，辅助用户导入单词
        List<Word> ParseExcelFile(string excelFilePath);//解析excel文件，辅助用户导入单词，有bug
        bool UpdateIsFavorite(string wordName, bool isFavorite); //根据单词名修改 IsFavorite
        bool UpdataFavById(int id, bool isFavorite);//根据id修改isfavorite
        bool UpdateIsNewWord(string wordName, bool isNewWord);  //根据单词名修改 IsNewWord
        bool UpdateErrorCount(string wordName, int errorCount); //根据单词名修改 ErrorCount 
        bool IncrementErrorCount(string wordName);// 每调用一次，根据单词名将 ErrorCount 属性加 1
        void AddWords(List<Word> wordList, string newDict);//通过单词队列在数据库中添加单词，根据WordName判断是否重复,返回添加成功的个数
        void AddDictTagById(int wordId, string newTag);//通过id添加newTag在DICTtag中

    }
}