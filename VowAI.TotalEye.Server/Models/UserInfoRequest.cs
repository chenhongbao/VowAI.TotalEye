using System.ComponentModel.DataAnnotations;

namespace VowAI.TotalEye.Server.Models
{
    public class UserInfoRequest
    {
        public int RequestId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Token { get; set; }
        public string? ReplyUrl { get; set; }
        public string? Status { get; set; }
        public User? User { get; set; }
    }
}