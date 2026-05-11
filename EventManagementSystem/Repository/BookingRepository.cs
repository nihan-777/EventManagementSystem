using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using EventManagementSystem.Database;
using EventManagementSystem.Models;

namespace EventManagementSystem.Repository
{
    public class BookingRepository
    {
        public int CreateBooking(int userId, int eventId, int tickets, double pricePerTicket)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = @"INSERT INTO Bookings (UserID,EventID,Tickets,BookingDate)
                               VALUES (@u,@e,@t,@d);
                               SELECT last_insert_rowid();";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@e", eventId);
                    cmd.Parameters.AddWithValue("@t", tickets);
                    cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        public void CancelBooking(int bookingId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqliteCommand("DELETE FROM Bookings WHERE BookingID=@id", conn))
            {
                cmd.Parameters.AddWithValue("@id", bookingId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Booking> GetBookingsByUser(int userId)
        {
            var list = new List<Booking>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT b.*,e.Title AS EventTitle,e.Price
                               FROM Bookings b JOIN Events e ON b.EventID=e.EventID
                               WHERE b.UserID=@u";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            int t = r.GetInt32(r.GetOrdinal("Tickets"));
                            double p = r.GetDouble(r.GetOrdinal("Price"));
                            list.Add(new Booking
                            {
                                BookingID = r.GetInt32(r.GetOrdinal("BookingID")),
                                UserID = r.GetInt32(r.GetOrdinal("UserID")),
                                EventID = r.GetInt32(r.GetOrdinal("EventID")),
                                Tickets = t,
                                BookingDate = r.GetString(r.GetOrdinal("BookingDate")),
                                EventTitle = r.GetString(r.GetOrdinal("EventTitle")),
                                TotalPrice = t * p
                            });
                        }
                }
            }
            return list;
        }

        public List<Booking> GetAllBookings()
        {
            var list = new List<Booking>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                string sql = @"SELECT b.*,e.Title AS EventTitle,e.Price,u.Name AS UserName
                               FROM Bookings b JOIN Events e ON b.EventID=e.EventID
                               JOIN Users u ON b.UserID=u.UserID";
                using (var cmd = new SqliteCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        int t = r.GetInt32(r.GetOrdinal("Tickets"));
                        double p = r.GetDouble(r.GetOrdinal("Price"));
                        list.Add(new Booking
                        {
                            BookingID = r.GetInt32(r.GetOrdinal("BookingID")),
                            UserID = r.GetInt32(r.GetOrdinal("UserID")),
                            EventID = r.GetInt32(r.GetOrdinal("EventID")),
                            Tickets = t,
                            BookingDate = r.GetString(r.GetOrdinal("BookingDate")),
                            EventTitle = r.GetString(r.GetOrdinal("EventTitle")),
                            UserName = r.GetString(r.GetOrdinal("UserName")),
                            TotalPrice = t * p
                        });
                    }
            }
            return list;
        }
    }
}