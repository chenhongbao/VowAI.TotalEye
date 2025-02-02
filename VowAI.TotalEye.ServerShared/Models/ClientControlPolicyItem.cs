namespace VowAI.TotalEye.ServerShared.Models
{
    public class ClientControlPolicyItem
    {
        public int ItemId { get; set; }
        public string FilterWords { get; set; } = "";
        public string FilterCondition { get; set; } = "";
        public string Action { get; set; } = "";
        public string ActionDescription { get; set; } = "";
    }
}
