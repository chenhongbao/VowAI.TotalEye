namespace VowAI.TotalEye.Server.Models
{
    public class ControlPolicy
    {
        public int PolicyId { get; set; }
        public int Description { get; set; }
        public string? Tag { get; set; }
        public string? FilterWords { get; set; }
        public string? FilterCondition { get; set; }
        public string? Action { get; set; }
        public string? ActionDescription { get; set; }
    }
}
