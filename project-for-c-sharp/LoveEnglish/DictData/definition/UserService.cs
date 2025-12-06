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

namespace DictData
{
    public class UserService : IUserService
    {
        private readonly string connectionString;

        public UserService()
        {
            connectionString = "server=localhost;port=3306;user id=root;password=123123;database=loveenglish;";
        }

        // 获取用户数据
        public User GetUser(int userId)
        {
            User user = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Id, CheckInDays, LastLoginTime FROM users WHERE Id = @UserId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(0),
                        CheckInDays = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                        LastLoginTime = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2)
                    };
                }
                reader.Close();
            }
            return user;
        }

        
        public bool UpdateCheckinDays(int userId, int checkInDays)
        {
            int rowsAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE users SET CheckInDays = @CheckInDays WHERE Id = @UserId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@CheckInDays", checkInDays);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }

      
        public bool UpdateLastLoginTime(int userId)
        {
            int rowsAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE users SET LastLoginTime = @LastLoginTime WHERE Id = @UserId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@LastLoginTime", DateTime.Now);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }

       
        public int GetCheckinDays(int userId)
        {
            int checkInDays = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT CheckInDays FROM users WHERE Id = @UserId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    checkInDays = Convert.ToInt32(result);
                }
            }
            return checkInDays;
        }

    }
}
