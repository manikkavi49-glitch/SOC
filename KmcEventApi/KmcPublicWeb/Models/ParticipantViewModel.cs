namespace KmcPublicWeb.Models
{
    public class ParticipantViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}