using System;
using System.Collections.Generic;
using DictModel;
using System.IO;
using System.Data;
using System.Configuration;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.Text;
using System.Runtime.Remoting.Messaging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Linq;
using DictData.definition;

namespace DictData
{
    /// <summary>
    /// 实现 IWordDatabaseManager 接口，用于操作单词数据库。
    /// </summary>
    public class WordService : IWordService
    {
        private readonly string connectionString;
        private DictionaryService dictionaryService = new DictionaryService();
        public WordService()
        {

            connectionString = "server=localhost;port=3306;user id=root;password=123123;database=loveenglish;";

        }
        public void DisplayWordProperties(Word word)
        {
            Type type = word.GetType();
            PropertyInfo[] properties = type.GetProperties();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("单词属性列表：");
            Console.WriteLine(new string('-', 50));

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(word);
                string displayValue = (value == null) ? "null" : value.ToString();
                Console.WriteLine($"{property.Name,-20}: {displayValue}");
            }
        }
        public List<Word> GetAllWords()
        {
            List<Word> words = new List<Word>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, Translation, Exchange, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource FROM words";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Word word = new Word
                    {
                        Id = reader.GetInt32(0),
                        WordName = reader.GetString(1),
                        BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                        AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsNewWord = reader.GetBoolean(7),
                        NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                        IsFavorite = reader.GetBoolean(9),
                        ErrorCount = reader.GetInt32(10),
                        DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                        ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                        ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                    };
                    words.Add(word);
                }
                reader.Close();
            }
            return words;
        }


        public Word GetWordByName(string wordName)
        {
            Word word = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, Translation, Exchange, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource FROM words WHERE WordName = @WordName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@WordName", wordName);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    word = new Word
                    {
                        Id = reader.GetInt32(0),
                        WordName = reader.GetString(1),
                        BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                        AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsNewWord = reader.GetBoolean(7),
                        NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                        IsFavorite = reader.GetBoolean(9),
                        ErrorCount = reader.GetInt32(10),
                        DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                        ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                        ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                    };
                }
                reader.Close();
            }
            return word;
        }

        // 获得收藏的单词列表
        public List<Word> GetFavoriteWords()
        {
            List<Word> words = new List<Word>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, Translation, Exchange, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource FROM words WHERE IsFavorite = true";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Word word = new Word
                    {
                        Id = reader.GetInt32(0),
                        WordName = reader.GetString(1),
                        BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                        AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsNewWord = reader.GetBoolean(7),
                        NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                        IsFavorite = reader.GetBoolean(9),
                        ErrorCount = reader.GetInt32(10),
                        DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                        ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                        ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                    };
                    words.Add(word);
                }
                reader.Close();
            }
            return words;
        }

        public int GetWordIdByName(string wordName)
        {
            int wordId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE WordName = @WordName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@WordName", wordName);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    wordId = Convert.ToInt32(result);
                }
            }
            return wordId;
        }

        public Word GetWordById(int id)
        {
            Word word = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, Translation, Exchange, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource FROM words WHERE Id = @Id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    word = new Word
                    {
                        Id = reader.GetInt32(0),
                        WordName = reader.GetString(1),
                        BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                        AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsNewWord = reader.GetBoolean(7),
                        NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                        IsFavorite = reader.GetBoolean(9),
                        ErrorCount = reader.GetInt32(10),
                        DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                        ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                        ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                    };
                }
                reader.Close();
            }
            return word;
        }

        public List<Word> GetWordsByDictTag(string keyword)
        {
            List<Word> words = new List<Word>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // 使用 LIKE 进行包含匹配
                string query = @"SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, 
                        Translation, Exchange, Examples, IsNewWord, NextReviewDate, 
                        IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource 
                        FROM words 
                        WHERE DictTag LIKE @Keyword
                        ";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");  // 前后都加 % 表示包含匹配

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Word word = new Word
                            {
                                Id = reader.GetInt32(0),
                                WordName = reader.GetString(1),
                                BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                                AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                                IsNewWord = reader.GetBoolean(7),
                                NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                                IsFavorite = reader.GetBoolean(9),
                                ErrorCount = reader.GetInt32(10),
                                DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                                ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                                ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                            };
                            words.Add(word);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"模糊查询DictTag时出错: {ex.Message}");
                    // 可以根据需要记录日志或抛出异常
                }
            }
            return words;
        }

        public bool UpdateIsFavorite(string wordName, bool isFavorite)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE words SET IsFavorite = @IsFavorite WHERE WordName = @WordName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsFavorite", isFavorite);
                command.Parameters.AddWithValue("@WordName", wordName);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"更新 IsFavorite 时出错: {ex.Message}");
                    return false;
                }
            }
        }

        public bool UpdataFavById(int Id,bool isFavorite)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE words SET IsFavorite = @IsFavorite WHERE Id = @Id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsFavorite", isFavorite);
                command.Parameters.AddWithValue("@Id",Id);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"更新 IsFavorite 时出错: {ex.Message}");
                    return false;
                }
            }
        }

        public bool UpdateIsNewWord(string wordName, bool isNewWord)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE words SET IsNewWord = @IsNewWord WHERE WordName = @WordName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsNewWord", isNewWord);
                command.Parameters.AddWithValue("@WordName", wordName);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"更新 IsNewWord 时出错: {ex.Message}");
                    return false;
                }
            }
        }

        public bool UpdateErrorCount(string wordName, int errorCount)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE words SET ErrorCount = @ErrorCount WHERE WordName = @WordName";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ErrorCount", errorCount);
                command.Parameters.AddWithValue("@WordName", wordName);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"更新 ErrorCount 时出错: {ex.Message}");
                    return false;
                }
            }
        }

        //public bool IncrementErrorCount(string wordName)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        string query = "UPDATE words SET ErrorCount = ErrorCount + 1 WHERE WordName = @WordName";
        //        MySqlCommand command = new MySqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@WordName", wordName);
        //        try
        //        {
        //            connection.Open();
        //            int rowsAffected = command.ExecuteNonQuery();
        //            return rowsAffected > 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"更新 ErrorCount 时出错: {ex.Message}");
        //            return false;
        //        }
        //    }
        //}
        public bool IncrementErrorCount(string wordName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. First get current error count
                        string getCountQuery = "SELECT ErrorCount FROM words WHERE WordName = @WordName";
                        MySqlCommand getCountCommand = new MySqlCommand(getCountQuery, connection, transaction);
                        getCountCommand.Parameters.AddWithValue("@WordName", wordName);

                        object countResult = getCountCommand.ExecuteScalar();
                        int currentCount = countResult != null ? Convert.ToInt32(countResult) : 0;

                        // 2. Increment the error count
                        string incrementQuery = "UPDATE words SET ErrorCount = ErrorCount + 1 WHERE WordName = @WordName";
                        MySqlCommand incrementCommand = new MySqlCommand(incrementQuery, connection, transaction);
                        incrementCommand.Parameters.AddWithValue("@WordName", wordName);
                        int rowsAffected = incrementCommand.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // 3. If new count > 3, add "错题集" tag
                        if (currentCount + 1 > 3)
                        {
                            // Get current DictTag
                            string getTagQuery = "SELECT DictTag FROM words WHERE WordName = @WordName";
                            MySqlCommand getTagCommand = new MySqlCommand(getTagQuery, connection, transaction);
                            getTagCommand.Parameters.AddWithValue("@WordName", wordName);

                            object tagResult = getTagCommand.ExecuteScalar();
                            string currentTags = tagResult?.ToString() ?? "";

                            // Check if tag already exists
                            if (!currentTags.Contains("错题集"))
                            {
                                string updatedTags = string.IsNullOrEmpty(currentTags)
                                    ? "错题集"
                                    : $"{currentTags},错题集";

                                // Update DictTag
                                string updateTagQuery = "UPDATE words SET DictTag = @DictTag WHERE WordName = @WordName";
                                MySqlCommand updateTagCommand = new MySqlCommand(updateTagQuery, connection, transaction);
                                updateTagCommand.Parameters.AddWithValue("@WordName", wordName);
                                updateTagCommand.Parameters.AddWithValue("@DictTag", updatedTags);
                                updateTagCommand.ExecuteNonQuery();
                                dictionaryService.IncrementWordCount("错题集");
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"更新 ErrorCount 时出错: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public List<int> GetFavoriteWordIds()
        {
            List<int> ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE IsFavorite = true";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    ids.Add(id);
                }
                reader.Close();
            }
            return ids;
        }

        public List<int> GetNewWordIds()
        {
            List<int> ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE IsNewWord = true";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    ids.Add(id);
                }
                reader.Close();
            }
            return ids;
        }

        public List<Word> GetWordsByFuzzyName(string keyword)
        {
            List<Word> words = new List<Word>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // 使用 LIKE 匹配开头
                string query = @"SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, 
                        Translation, Exchange, Examples, IsNewWord, NextReviewDate, 
                        IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource 
                        FROM words 
                        WHERE WordName LIKE @Keyword
                        LIMIT 30";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Keyword", $"{keyword}%");  

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Word word = new Word
                        {
                            Id = reader.GetInt32(0),
                            WordName = reader.GetString(1),
                            BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                            AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                            IsNewWord = reader.GetBoolean(7),
                            NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                            IsFavorite = reader.GetBoolean(9),
                            ErrorCount = reader.GetInt32(10),
                            DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                            ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                            ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                        };
                        words.Add(word);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"模糊查询单词时出错: {ex.Message}");
                }
            }
            return words;
        }

        public List<string> GetAllWordNames()
        {
            List<string> wordNames = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT WordName FROM words";
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string wordName = reader.GetString(0);
                        wordNames.Add(wordName);
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    // 处理数据库操作异常
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
            return wordNames;
        }

        public List<int> GetWordIdsByErrorCount(int errorCount)
        {
            List<int> ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE ErrorCount = @ErrorCount";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ErrorCount", errorCount);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        ids.Add(id);
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    // 处理数据库操作异常
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
            return ids;
        }

        public List<int> GetWordIdsByErrorCountGreaterThan(int minErrorCount)
        {
            List<int> ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE ErrorCount > @MinErrorCount";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@MinErrorCount", minErrorCount);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        ids.Add(id);
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    // 处理数据库操作异常
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
            return ids;
        }
        public List<int> GetWordIdsByNextReviewDate(System.DateTime nextReviewDate)
        {
            List<int> ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE NextReviewDate = @NextReviewDate";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@NextReviewDate", nextReviewDate);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        ids.Add(id);
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    // 处理数据库操作异常
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
            return ids;
        }

        public List<int> GetWordIdsByTranslation(string translation)
        {
            List<int> ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id FROM words WHERE Translation = @Translation";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Translation", translation);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        ids.Add(id);
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    // 处理数据库操作异常
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
            return ids;
        }

        public List<Word> GetWordsByFuzzyTranslation(string keyword)
        {
            List<Word> words = new List<Word>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, WordName, BritishPhonetic, AmericanPhonetic, Translation, Exchange, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource " +
                               "FROM words WHERE Translation LIKE @Keyword";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Word word = new Word
                        {
                            Id = reader.GetInt32(0),
                            WordName = reader.GetString(1),
                            BritishPhonetic = reader.IsDBNull(2) ? null : reader.GetString(2),
                            AmericanPhonetic = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Translation = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Exchange = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Examples = reader.IsDBNull(6) ? null : reader.GetString(6),
                            IsNewWord = reader.GetBoolean(7),
                            NextReviewDate = reader.IsDBNull(8) ? null : (DateTime?)reader.GetDateTime(8),
                            IsFavorite = reader.GetBoolean(9),
                            ErrorCount = reader.GetInt32(10),
                            DictTag = reader.IsDBNull(11) ? null : reader.GetString(11),
                            ExampleTrans = reader.IsDBNull(12) ? null : reader.GetString(12),
                            ExampleSource = reader.IsDBNull(13) ? null : reader.GetString(13)
                        };
                        words.Add(word);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"根据翻译模糊查询单词时出错: {ex.Message}");
                }
            }
            return words;
        }

        public List<Word> ParseCsvFile(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = context =>
                {
                    Console.WriteLine($"坏数据在行 {context.RawRecord}");
                },
                Mode = CsvMode.RFC4180,
                Escape = '"',
                Quote = '"'
            };

            try
            {
                using (var reader = new StreamReader(csvFilePath, Encoding.UTF8))
                using (var csv = new CsvReader(reader, config))
                {
                    // 注册映射必须在读取操作前完成
                    csv.Context.RegisterClassMap<WordMap>();
                    return csv.GetRecords<Word>().ToList();
                }
            }
            catch (CsvHelperException ex)
            {
                Console.WriteLine($"CSV解析错误: {ex.Message}");
                return new List<Word>();
            }
            catch (Exception ex)  // 添加通用异常处理
            {
                Console.WriteLine($"文件处理错误: {ex.Message}");
                return new List<Word>();
            }
        }

        // 自定义映射类
        public sealed class WordMap : ClassMap<Word>
        {
            public WordMap()
            {
                Map(m => m.WordName).Name("WordName");
                Map(m => m.BritishPhonetic).Name("BritishPhonetic");
                Map(m => m.AmericanPhonetic).Name("AmericanPhonetic");
                Map(m => m.Translation).Name("Translation");
                Map(m => m.Exchange).Name("Exchange");
                Map(m => m.Examples).Name("Examples");
                Map(m => m.IsNewWord).Name("IsNewWord")
                   .TypeConverterOption.BooleanValues(true, false, "1", "0");
                Map(m => m.NextReviewDate).Name("NextReviewDate")
                   .TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss");
                Map(m => m.IsFavorite).Name("IsFavorite")
                   .TypeConverterOption.BooleanValues(true, false);
                Map(m => m.ErrorCount).Name("ErrorCount");
                Map(m => m.DictTag).Name("DictTag");
                Map(m => m.ExampleTrans).Name("ExampleTrans");
                Map(m => m.ExampleSource).Name("ExampleSource");
            }
        }

        // 解析 Excel 文件
        public List<Word> ParseExcelFile(string excelFilePath)
        {
            var existingWordNames = GetAllWordNames();
            var wordList = new List<Word>();
            FileInfo file = new FileInfo(excelFilePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                for (int row = 2; row <= rowCount; row++)
                {
                    var addWord = new Word();
                    for (int col = 1; col <= colCount; col++)
                    {
                        string header = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                        string value = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(value))
                        {
                            // 数据格式错误，跳过该行
                            continue;
                        }
                        switch (header)
                        {
                            case "WordName":
                                addWord.WordName = value;
                                if (existingWordNames.Contains(value))
                                {
                                    continue;
                                }
                                break;
                            case "BritishPhonetic":
                                addWord.BritishPhonetic = value;
                                break;
                            case "AmericanPhonetic":
                                addWord.AmericanPhonetic = value;
                                break;
                            case "Translation":
                                addWord.Translation = value;
                                break;
                            case "Exchange":
                                addWord.Exchange = value;
                                break;
                            case "Examples":
                                addWord.Examples = value;
                                break;
                            case "IsNewWord":
                                bool resultNew;
                                if (bool.TryParse(value, out resultNew))
                                {
                                    addWord.IsNewWord = resultNew;
                                }
                                else
                                {
                                    addWord.IsNewWord = true;
                                }
                                break;
                            case "NextReviewDate":
                                try
                                {
                                    if (value != null)
                                        addWord.NextReviewDate = DateTime.Parse(value);

                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine($"导入单词{addWord.WordName}时时间出现问题: {ex.Message}");
                                }
                                break;
                            case "IsFavorite":
                                bool resultFavo;
                                if (bool.TryParse(value, out resultFavo))
                                {
                                    addWord.IsFavorite = resultFavo;
                                }
                                else
                                {
                                    addWord.IsFavorite = false;
                                }
                                break;

                            case "ErrorCount":
                                try
                                {
                                    addWord.ErrorCount = int.Parse(value);
                                }
                                catch (FormatException)
                                {
                                    addWord.ErrorCount = 0;
                                }
                                break;
                            case "DictTag":
                                addWord.DictTag = value;
                                break;
                            case "ExampleTrans":
                                addWord.ExampleTrans = value;
                                break;
                            case "ExampleSource":
                                addWord.ExampleSource = value;
                                break;
                        }
                    }
                    wordList.Add(addWord);
                }
            }
            return wordList;
        }

        //public int AddWords(List<Word> wordList)
        //{
        //    int addCount = 0;
        //    // 获取数据库中所有的单词名
        //    List<string> existingWordNames = GetAllWordNames();
        //    HashSet<string> existingWordNameSet = new HashSet<string>(existingWordNames);

        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        foreach (var word in wordList)
        //        {
        //            if (!existingWordNameSet.Contains(word.WordName))
        //            {
        //                addCount++;
        //                string insertQuery = "INSERT INTO Words (WordName, BritishPhonetic, AmericanPhonetic, Exchange, Translation, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource) " +
        //                                     "VALUES (@WordName, @BritishPhonetic, @AmericanPhonetic, @Exchange, @Translation, @Examples, @IsNewWord, @NextReviewDate, @IsFavorite, @ErrorCount, @DictTag, @ExampleTrans, @ExampleSource)";
        //                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
        //                {
        //                    insertCommand.Parameters.AddWithValue("@WordName", word.WordName);
        //                    insertCommand.Parameters.AddWithValue("@BritishPhonetic", word.BritishPhonetic);
        //                    insertCommand.Parameters.AddWithValue("@AmericanPhonetic", word.AmericanPhonetic);
        //                    insertCommand.Parameters.AddWithValue("@Exchange", word.Exchange);
        //                    insertCommand.Parameters.AddWithValue("@Translation", word.Translation);
        //                    insertCommand.Parameters.AddWithValue("@Examples", word.Examples);
        //                    insertCommand.Parameters.AddWithValue("@IsNewWord", word.IsNewWord ? 1 : 0);
        //                    if (word.NextReviewDate.HasValue)
        //                    {
        //                        insertCommand.Parameters.AddWithValue("@NextReviewDate", word.NextReviewDate.Value);
        //                    }
        //                    else
        //                    {
        //                        insertCommand.Parameters.AddWithValue("@NextReviewDate", DBNull.Value);
        //                    }
        //                    insertCommand.Parameters.AddWithValue("@IsFavorite", word.IsFavorite ? 1 : 0);
        //                    insertCommand.Parameters.AddWithValue("@ErrorCount", word.ErrorCount);
        //                    insertCommand.Parameters.AddWithValue("@DictTag", word.DictTag);
        //                    insertCommand.Parameters.AddWithValue("@ExampleTrans", word.ExampleTrans);
        //                    insertCommand.Parameters.AddWithValue("@ExampleSource", word.ExampleSource);
        //                    insertCommand.ExecuteNonQuery();
        //                    // 将新插入的单词名添加到集合中
        //                    existingWordNameSet.Add(word.WordName);
        //                }
        //            }
        //        }
        //    }
        //    return addCount;


        //}




        public void AddWords(List<Word> wordList, string newDict)
        {
            
            var existingWords = GetExistingWordsMetadata(); // 返回Dictionary<WordName, DidTag>

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var word in wordList)
                        {
                            if (!existingWords.ContainsKey(word.WordName))
                            {
                                // 2. 插入新单词（使用传入的newDict作为初始DidTag）
                                string insertQuery = "INSERT INTO Words (WordName, BritishPhonetic, AmericanPhonetic, Exchange, Translation, Examples, IsNewWord, NextReviewDate, IsFavorite, ErrorCount, DictTag, ExampleTrans, ExampleSource) " +
                                             "VALUES (@WordName, @BritishPhonetic, @AmericanPhonetic, @Exchange, @Translation, @Examples, @IsNewWord, @NextReviewDate, @IsFavorite, @ErrorCount, @DictTag, @ExampleTrans, @ExampleSource)";
                                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@WordName", word.WordName);
                                    insertCommand.Parameters.AddWithValue("@BritishPhonetic", word.BritishPhonetic);
                                    insertCommand.Parameters.AddWithValue("@AmericanPhonetic", word.AmericanPhonetic);
                                    insertCommand.Parameters.AddWithValue("@Exchange", word.Exchange);
                                    insertCommand.Parameters.AddWithValue("@Translation", word.Translation);
                                    insertCommand.Parameters.AddWithValue("@Examples", word.Examples);
                                    insertCommand.Parameters.AddWithValue("@IsNewWord", word.IsNewWord ? 1 : 0);
                                    if (word.NextReviewDate.HasValue)
                                    {
                                        insertCommand.Parameters.AddWithValue("@NextReviewDate", word.NextReviewDate.Value);
                                    }
                                    else
                                    {
                                        insertCommand.Parameters.AddWithValue("@NextReviewDate", DBNull.Value);
                                    }
                                    insertCommand.Parameters.AddWithValue("@IsFavorite", word.IsFavorite ? 1 : 0);
                                    insertCommand.Parameters.AddWithValue("@ErrorCount", word.ErrorCount);
                                    insertCommand.Parameters.AddWithValue("@DictTag", word.DictTag);
                                    insertCommand.Parameters.AddWithValue("@ExampleTrans", word.ExampleTrans);
                                    insertCommand.Parameters.AddWithValue("@ExampleSource", word.ExampleSource);
                                    insertCommand.ExecuteNonQuery();

                                }
                            }
                            else
                            {
                                // 3. 更新现有单词的DidTag（追加模式）
                                UpdateDictTag(connection, transaction, word.WordName,
                                            existingWords[word.WordName], newDict);
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AddDictTagById(int wordId, string newTag)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Get the current DictTag for the word
                        string selectQuery = "SELECT DictTag FROM Words WHERE Id = @Id";
                        string currentDictTag = null;

                        using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection, transaction))
                        {
                            selectCommand.Parameters.AddWithValue("@Id", wordId);
                            var result = selectCommand.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                currentDictTag = result.ToString();
                            }
                        }

                        // 2. Update the DictTag (append mode)
                        string updatedDictTag;
                        if (string.IsNullOrEmpty(currentDictTag))
                        {
                            updatedDictTag = newTag;
                        }
                        else
                        {
                            updatedDictTag = $"{currentDictTag},{newTag}";
                        }

                        // 3. Update the word record
                        string updateQuery = "UPDATE Words SET DictTag = @DictTag WHERE Id = @Id";
                        using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@Id", wordId);
                            updateCommand.Parameters.AddWithValue("@DictTag", updatedDictTag);
                            updateCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 辅助方法：获取所有单词名和DidTag（优化版）
        private Dictionary<string, string> GetExistingWordsMetadata()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // 忽略大小写
            using (var connection = new MySqlConnection(connectionString))
            {
                const string query = "SELECT WordName, DictTag FROM words"; // 注意字段名是DidTag
                using (var cmd = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result[reader.GetString(0)] = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                        }
                    }
                }
            }
            return result;
        }

        // 更新现有单词的DidTag（处理标签拼接逻辑）
        private void UpdateDictTag(MySqlConnection connection, MySqlTransaction transaction,
                         string wordName, string currentDictTag, string newDict)
        {
            // 标签处理逻辑（去重+逗号分隔）
            var tags = new HashSet<string>(
                currentDictTag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            );
            tags.Add(newDict.Trim()); // 自动去重并去除空格
            string updatedTag = string.Join(" ,", tags);

            const string query = "UPDATE words SET DictTag = @UpdatedTag WHERE WordName = @WordName";
            using (var cmd = new MySqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@UpdatedTag", updatedTag);
                cmd.Parameters.AddWithValue("@WordName", wordName);
                cmd.ExecuteNonQuery();
            }
        }

        public int GetFavoriteWordCount()
        {
            int favoriteCount = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM words WHERE IsFavorite = true";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                favoriteCount = Convert.ToInt32(command.ExecuteScalar());
            }
            return favoriteCount;
        }






    }



}