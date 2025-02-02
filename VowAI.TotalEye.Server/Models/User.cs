namespace VowAI.TotalEye.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
        public string Organization { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
    }
}
