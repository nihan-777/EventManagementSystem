namespace EventManagementSystem.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public int BookedTickets { get; set; }
        public int AvailableSeats => Capacity - BookedTickets;
    }
}