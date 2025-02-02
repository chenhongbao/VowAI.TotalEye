namespace VowAI.TotalEye.Server.Models
{
    public class UserPolicyItem
    {
        public int ItemId { get; set; }
        public string? FilterWords { get; set; }
        public string? FilterCondition { get; set; }
        public string? Action { get; set; }
        public string? ActionDescription { get; set; }
    }
}
