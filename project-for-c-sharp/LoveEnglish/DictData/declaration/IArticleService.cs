using DictModel;
using System.Collections.Generic;

namespace DictData
{
    public interface IArticleService
    {
        List<Article> GetAllArticles();// 获取所有文章
        void AddArticles(List<Article> articles);// 批量添加文章
        void DeleteArticle(int articleId);// 根据ID删除文章
    }
}