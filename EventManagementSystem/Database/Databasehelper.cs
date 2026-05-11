using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace EventManagementSystem.Database
{
    public class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "EventManagement.db");

        public static string ConnectionString => $"Data Source={DbPath}";

        public static void InitializeDatabase()
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();

                ExecuteNonQuery(conn, @"CREATE TABLE IF NOT EXISTS Users (
                    UserID   INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name     TEXT NOT NULL,
                    Email    TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role     TEXT NOT NULL DEFAULT 'user');");

                ExecuteNonQuery(conn, @"CREATE TABLE IF NOT EXISTS Events (
                    EventID  INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title    TEXT NOT NULL,
                    Date     TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    Capacity INTEGER NOT NULL,
                    Price    REAL NOT NULL DEFAULT 0);");

                ExecuteNonQuery(conn, @"CREATE TABLE IF NOT EXISTS Bookings (
                    BookingID   INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID      INTEGER NOT NULL,
                    EventID     INTEGER NOT NULL,
                    Tickets     INTEGER NOT NULL,
                    BookingDate TEXT NOT NULL,
                    FOREIGN KEY (UserID)  REFERENCES Users(UserID),
                    FOREIGN KEY (EventID) REFERENCES Events(EventID));");

                ExecuteNonQuery(conn,
                    "INSERT OR IGNORE INTO Users (Name,Email,Password,Role) VALUES ('Administrator','admin@cms.com','admin123','admin');");

                ExecuteNonQuery(conn, @"INSERT OR IGNORE INTO Events (EventID,Title,Date,Location,Capacity,Price) VALUES
                    (1,'Tech Conference 2025','2025-08-15','Dubai World Trade Centre',200,150.00),
                    (2,'Music Festival Night','2025-09-01','Zabeel Park, Dubai',500,75.00),
                    (3,'Business Networking Summit','2025-07-20','JW Marriott, Dubai',100,200.00);");
            }
        }

        private static void ExecuteNonQuery(SqliteConnection conn, string sql)
        {
            using (var cmd = new SqliteCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        public static SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}