namespace KmcEventApi.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; // Event eke nama
        public string Description { get; set; } = string.Empty; // Wisthara
        public DateTime EventDate { get; set; } // Dinaya saha welawa
        public string Type { get; set; } = string.Empty; // Udaharana: Music, Workshop, Sports [cite: 27]
        public string Location { get; set; } = "Kandy"; // KMC nisa location eka Kandy [cite: 20]
        public string CreatedBy { get; set; } = string.Empty; // Organizer ID (Creator pamanak update kirima thahawuru karanna) [cite: 25]
        public string OrganizerEmail { get; set; } = ""; // Organizer ව හඳුනාගන්න
        public string Visibility { get; set; } = "Public"; // Default value is Public
    }
}