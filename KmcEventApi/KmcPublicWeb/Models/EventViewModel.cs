namespace KmcPublicWeb.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Location { get; set; } = "Kandy";

        // ★ මේ පේළිය අලුතින් ඇතුළත් කරන්න
        public string CreatedBy { get; set; } = "Admin";
    }
}