namespace VowAI.TotalEye.Server.Models
{
    public class InfoSession
    {
        public int SessionId { get; set; }
        public string Token { get;set; }
        public UserPolicyBinding Policy { get; set; }

    }
}
