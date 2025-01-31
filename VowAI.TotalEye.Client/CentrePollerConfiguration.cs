namespace VowAI.TotalEye.Client
{
    public class CentrePollerConfiguration : ICentrePollerConfiguration
    {
        public string LoginUrl { get; set; }
        public string AskUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Pin { get; set; }
    }
}
