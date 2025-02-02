namespace VowAI.TotalEye.Client
{
    public interface IServerPollerConfiguration
    {
        public string LoginUrl { get; }
        public string AskUrl { get; }
        public int UserId { get; }
        public string Password { get; }
        public string Pin { get; }
    }
}