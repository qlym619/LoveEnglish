using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using DictModel;
using System.Reflection;


namespace DictData
{
    public class ArticleService : IArticleService
    {
        private readonly string connectionString;

        public ArticleService()
        {
            connectionString = "server=localhost;port=3306;user id=remote_user;password=123123;database=loveenglish;";
        }

        public void DisplayArticleProperties(Article article)
        {
            Type type = article.GetType();
            PropertyInfo[] properties = type.GetProperties();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("单词属性列表：");
            Console.WriteLine(new string('-', 50));

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(article);
                string displayValue = (value == null) ? "null" : value.ToString();
                Console.WriteLine($"{property.Name,-20}: {displayValue}");
            }
        }

        public List<Article> GetAllArticles()
        {
            List<Article> articles = new List<Article>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, Title, Content, SourceUrl, CreatedTime FROM articles";
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Article article = new Article
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Content = reader.IsDBNull(2) ? null : reader.GetString(2),
                            SourceUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CreatedTime = reader.GetDateTime(4)
                        };
                        articles.Add(article);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取所有文章时出错: {ex.Message}");
                }
            }

            return articles;
        }

        public void AddArticles(List<Article> articles)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    foreach (var article in articles)
                    {
                        string query = @"INSERT INTO articles 
                                        (Title, Content, SourceUrl, CreatedTime) 
                                        VALUES (@Title, @Content, @SourceUrl, @CreatedTime)";

                        MySqlCommand command = new MySqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@Title", article.Title);
                        command.Parameters.AddWithValue("@Content", article.Content);
                        command.Parameters.AddWithValue("@SourceUrl", article.SourceUrl);
                        command.Parameters.AddWithValue("@CreatedTime", article.CreatedTime);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"批量添加文章时出错: {ex.Message}");
                    throw;
                }
            }
        }

        public void DeleteArticle(int articleId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM articles WHERE Id = @Id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", articleId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine($"未找到ID为 {articleId} 的文章");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"删除文章时出错: {ex.Message}");
                    throw;
                }
            }
        }
    }
}





//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Data.Entity; // 使用EntityFramework 6
//using DictModel;
//using System.Reflection;
//using MySql.Data.MySqlClient;
//namespace DictData
//{
//    public class ArticleService : IArticleService
//    {
//        private readonly LoveEnglishContext _context;

//        public ArticleService()
//        {
//            // 使用Entity Framework 6的初始化方式
//            _context = new LoveEnglishContext();
//        }

//        public void DisplayArticleProperties(Article article)
//        {
//            Type type = article.GetType();
//            PropertyInfo[] properties = type.GetProperties();
//            Console.OutputEncoding = System.Text.Encoding.UTF8;

//            Console.WriteLine("单词属性列表：");
//            Console.WriteLine(new string('-', 50));

//            foreach (PropertyInfo property in properties)
//            {
//                object value = property.GetValue(article);
//                string displayValue = (value == null) ? "null" : value.ToString();
//                Console.WriteLine($"{property.Name,-20}: {displayValue}");
//            }
//        }

//        public List<Article> GetAllArticles()
//        {
//            try
//            {
//                return _context.Articles.ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"获取所有文章时出错: {ex.Message}");
//                return new List<Article>();
//            }
//        }

//        public void AddArticles(List<Article> articles)
//        {
//            try
//            {
//                _context.Articles.AddRange(articles);
//                _context.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"批量添加文章时出错: {ex.Message}");
//                throw;
//            }
//        }

//        public void DeleteArticle(int articleId)
//        {
//            try
//            {
//                var article = _context.Articles.Find(articleId);
//                if (article != null)
//                {
//                    _context.Articles.Remove(article);
//                    _context.SaveChanges();
//                }
//                else
//                {
//                    Console.WriteLine($"未找到ID为 {articleId} 的文章");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"删除文章时出错: {ex.Message}");
//                throw;
//            }
//        }
//    }

//    // 使用Entity Framework 6的DbContext
//    public class LoveEnglishContext : DbContext
//    {
//        public LoveEnglishContext()
//            : base("name=LoveEnglishConnectionString") // 从配置文件中读取连接字符串
//        {
//            // 初始化配置
//            Configuration.LazyLoadingEnabled = false;
//            Configuration.ProxyCreationEnabled = false;
//        }
//        // 硬编码连接字符串
//        private static MySqlConnection CreateConnection()
//        {
//            // 这里直接定义连接字符串
//            var connectionString = "server=localhost;port=3306;user id=remote_user;password=123123;database=loveenglish;";
//            return new MySqlConnection(connectionString);
//        }


//        public DbSet<Article> Articles { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            // 配置实体映射
//            modelBuilder.Entity<Article>()
//                .ToTable("articles")
//                .HasKey(e => e.Id);

//            modelBuilder.Entity<Article>()
//                .Property(e => e.Id)
//                .HasColumnName("Id");

//            modelBuilder.Entity<Article>()
//                .Property(e => e.Title)
//                .HasColumnName("Title");

//            modelBuilder.Entity<Article>()
//                .Property(e => e.Content)
//                .HasColumnName("Content");

//            modelBuilder.Entity<Article>()
//                .Property(e => e.SourceUrl)
//                .HasColumnName("SourceUrl");

//            modelBuilder.Entity<Article>()
//                .Property(e => e.CreatedTime)
//                .HasColumnName("CreatedTime");

//            base.OnModelCreating(modelBuilder);
//        }
//    }
//}