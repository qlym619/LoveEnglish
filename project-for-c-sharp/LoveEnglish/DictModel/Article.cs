using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Article
{
    public int Id { get; set; } // 索引
    public string Title { get; set; } // 文章标题
    public string Content { get; set; } // 文章内容
    public string SourceUrl { get; set; } // 原始文章的来源链接，用于引用或跳转到原始内容
    public DateTime CreatedTime { get; set; } // 文章创建时间
}
