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

namespace DictData.definition
{
    public class DictionaryService
    {
        private readonly string connectionString;

        public DictionaryService()
        {

            connectionString = "server=localhost;port=3306;user id=root;password=123123;database=loveenglish;";
        }

        public List<Dictionary> GetAllDictionaries()
        {
            List<Dictionary> dictionaries = new List<Dictionary>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, DictName,WordCount FROM Dictionaries ";
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Dictionary dictionary = new Dictionary
                        {
                            Id = reader.GetInt32(0),
                            DictName = reader.GetString(1),
                            WordCount=reader.GetInt32(2)
                        };
                        dictionaries.Add(dictionary);
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"查询词库信息时出错: {ex.Message}");
                }
            }
            return dictionaries;
        }
        public bool AddDictionary(Dictionary dictionary)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // 根据截图，表名应为dictionaries，列名为DictName和WordCount
                string query = "INSERT INTO dictionaries (DictName, WordCount) VALUES (@DictName, @WordCount)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@DictName", dictionary.DictName);
                command.Parameters.AddWithValue("@WordCount", dictionary.WordCount); // 处理可能的null值

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"添加词库时出错: {ex.Message}");
                    return false;
                }
            }
        }
        public bool IncrementWordCount(string dictName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "UPDATE dictionaries SET WordCount = WordCount + 1 WHERE DictName = @DictName";
                        MySqlCommand command = new MySqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@DictName", dictName);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"更新词库单词数量时出错: {ex.Message}");
                        return false;
                    }
                }
            }
        }



    }
}
