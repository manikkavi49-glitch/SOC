namespace KmcEventApi.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public int EventId { get; set; } // Register wena event eke ID eka
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
