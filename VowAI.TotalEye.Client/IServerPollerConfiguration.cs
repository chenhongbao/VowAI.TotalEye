namespace VowAI.TotalEye.Client
{
    public interface IServerPollerConfiguration
    {
        public string UserLoginUrl { get; }
        public string GetInfoRequestUrl { get; }
        public int UserId { get; }
        public string Password { get; }
    }
}