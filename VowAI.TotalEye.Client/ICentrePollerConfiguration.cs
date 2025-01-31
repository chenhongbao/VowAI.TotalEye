namespace VowAI.TotalEye.Client
{
    public interface ICentrePollerConfiguration
    {
        public string Url { get; }
        public string UserName { get; }
        public string Password { get; }
        public string Pin { get; }
    }
}