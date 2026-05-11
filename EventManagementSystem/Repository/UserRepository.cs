using Microsoft.Data.Sqlite;
using EventManagementSystem.Database;
using EventManagementSystem.Models;
using System.Collections.Generic;

namespace EventManagementSystem.Repository
{
    public class UserRepository
    {
        public bool Register(string name, string email, string password)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string sql = "INSERT INTO Users (Name,Email,Password,Role) VALUES (@n,@e,@p,'user')";
                    using (var cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SqliteException) { return false; }
        }

        public User Login(string email, string password)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT * FROM Users WHERE Email=@e AND Password=@p LIMIT 1";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return new User
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Role = reader.GetString(reader.GetOrdinal("Role"))
                        };
                    }
                }
            }
        }

        public List<User> GetAllUsers()
        {
            var list = new List<User>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = "SELECT UserID, Name, Email, Role FROM Users";
                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(new User
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Role = reader.GetString(reader.GetOrdinal("Role"))
                        });
            }
            return list;
        }
    }
}