namespace KmcEventApi.Models
{
    public class Organizer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = ""; // ආරක්ෂාව සඳහා පසුව මෙය Encrypt කරමු
    }
}