namespace VowAI.TotalEye.Client
{
    public interface ICentrePollerConfiguration
    {
        public string LoginUrl { get; }
        public string AskUrl { get; }
        public string UserName { get; }
        public string Password { get; }
        public string Pin { get; }
    }
}