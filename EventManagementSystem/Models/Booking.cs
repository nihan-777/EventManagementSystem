namespace EventManagementSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public int Tickets { get; set; }
        public string BookingDate { get; set; }
        public string EventTitle { get; set; }
        public string UserName { get; set; }
        public double TotalPrice { get; set; }
    }
}