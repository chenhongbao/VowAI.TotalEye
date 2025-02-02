namespace VowAI.TotalEye.Server.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? No { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Pin { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? Role { get; set; }
        public string? Group { get; set; }
        public string? Organization { get; set; }
        public DateTime RegisterTime { get; set; }
        public ImageItem? Image { get; set; }
        public ICollection<UserInfoRequest>? Requests { get; set; }
        public ICollection<UserInfoSession>? Sessions { get; set; }
        public ICollection<ControlPolicy>? Policies { get; set; }
    }
}
