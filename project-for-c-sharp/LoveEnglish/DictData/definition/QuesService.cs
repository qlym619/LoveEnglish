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
    /// <summary>
    /// 实现 IQuesDatabaseManager 接口，用于操作问题数据库。
    /// </summary>
    public class QuesService : IQuesService
    {

        private readonly string connectionString;
        public QuesService()
        {
            connectionString = "server=localhost;port=3306;user id=root;password=123123;database=loveenglish;";
        }
        


        public List<Question> GetQuestionsByWordId(int wordId)
        {
            List<Question> questionList = new List<Question>();
            WordService Wfetcher = new WordService();
            Word word = Wfetcher.GetWordById(wordId);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, Content, WordId, Type, Answer FROM questions WHERE WordId = @WordId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@WordId", wordId);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Question question = new Question
                    {
                        Id = reader.GetInt32(0),
                        Word = word,
                        Content = reader.IsDBNull(1) ? null : reader.GetString(1),
                        WordId = reader.GetInt32(2),
                        Type = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Answer = reader.IsDBNull(4) ? null : reader.GetString(4)
                    };
                    questionList.Add(question);
                }
                reader.Close();
            }
            return questionList;
        }
        public void DisplayQuestionProperties(Question question)
        {
            Type type = question.GetType();
            PropertyInfo[] properties = type.GetProperties();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("问题属性列表：");
            Console.WriteLine(new string('-', 50));

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(question);
                string displayValue = (value == null) ? "null" : value.ToString();
                Console.WriteLine($"{property.Name,-20}: {displayValue}");
            }
        }
        public List<Question> GetQuestionsByType(string questionType)
        {
            List<Question> questionList = new List<Question>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, Content, WordId, Type, Answer FROM questions WHERE Type = @Type";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Type", questionType);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Question question = new Question
                        {
                            Id = reader.GetInt32(0),
                            Content = reader.IsDBNull(1) ? null : reader.GetString(1),
                            WordId = reader.GetInt32(2),
                            Type = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Answer = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                        questionList.Add(question);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"根据问题类型查询问题记录时出错: {ex.Message}");
                }
            }
            return questionList;
        }
        public Question GetQuestionById(int questionId)
        {
            Question question = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, Content, WordId, Type, Answer FROM questions WHERE Id = @Id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", questionId);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        question = new Question
                        {
                            Id = reader.GetInt32(0),
                            Content = reader.IsDBNull(1) ? null : reader.GetString(1),
                            WordId = reader.GetInt32(2),
                            Type = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Answer = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"根据问题 ID 查询问题记录时出错: {ex.Message}");
                }
            }
            return question;
        }





    }
}
