using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using EventManagementSystem.Database;
using EventManagementSystem.Models;

namespace EventManagementSystem.Repository
{
    public class EventRepository
    {
        public List<Event> GetAllEvents()
        {
            var list = new List<Event>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT e.*, COALESCE(SUM(b.Tickets),0) AS Booked
                               FROM Events e LEFT JOIN Bookings b ON e.EventID=b.EventID
                               GROUP BY e.EventID";
                using (var cmd = new SqliteCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new Event
                        {
                            EventID = r.GetInt32(r.GetOrdinal("EventID")),
                            Title = r.GetString(r.GetOrdinal("Title")),
                            Date = r.GetString(r.GetOrdinal("Date")),
                            Location = r.GetString(r.GetOrdinal("Location")),
                            Capacity = r.GetInt32(r.GetOrdinal("Capacity")),
                            Price = r.GetDouble(r.GetOrdinal("Price")),
                            BookedTickets = r.GetInt32(r.GetOrdinal("Booked"))
                        });
            }
            return list;
        }

        public List<Event> SearchEvents(string keyword)
        {
            var list = new List<Event>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT e.*, COALESCE(SUM(b.Tickets),0) AS Booked
                               FROM Events e LEFT JOIN Bookings b ON e.EventID=b.EventID
                               WHERE e.Title LIKE @kw OR e.Location LIKE @kw
                               GROUP BY e.EventID";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(new Event
                            {
                                EventID = r.GetInt32(r.GetOrdinal("EventID")),
                                Title = r.GetString(r.GetOrdinal("Title")),
                                Date = r.GetString(r.GetOrdinal("Date")),
                                Location = r.GetString(r.GetOrdinal("Location")),
                                Capacity = r.GetInt32(r.GetOrdinal("Capacity")),
                                Price = r.GetDouble(r.GetOrdinal("Price")),
                                BookedTickets = r.GetInt32(r.GetOrdinal("Booked"))
                            });
                }
            }
            return list;
        }

        public void AddEvent(Event ev)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqliteCommand(
                "INSERT INTO Events (Title,Date,Location,Capacity,Price) VALUES (@t,@d,@l,@c,@p)", conn))
            {
                cmd.Parameters.AddWithValue("@t", ev.Title);
                cmd.Parameters.AddWithValue("@d", ev.Date);
                cmd.Parameters.AddWithValue("@l", ev.Location);
                cmd.Parameters.AddWithValue("@c", ev.Capacity);
                cmd.Parameters.AddWithValue("@p", ev.Price);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateEvent(Event ev)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqliteCommand(
                "UPDATE Events SET Title=@t,Date=@d,Location=@l,Capacity=@c,Price=@p WHERE EventID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@t", ev.Title);
                cmd.Parameters.AddWithValue("@d", ev.Date);
                cmd.Parameters.AddWithValue("@l", ev.Location);
                cmd.Parameters.AddWithValue("@c", ev.Capacity);
                cmd.Parameters.AddWithValue("@p", ev.Price);
                cmd.Parameters.AddWithValue("@id", ev.EventID);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteEvent(int eventId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                using (var cmd = new SqliteCommand("DELETE FROM Bookings WHERE EventID=@id", conn))
                { cmd.Parameters.AddWithValue("@id", eventId); cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand("DELETE FROM Events WHERE EventID=@id", conn))
                { cmd.Parameters.AddWithValue("@id", eventId); cmd.ExecuteNonQuery(); }
            }
        }
    }
}