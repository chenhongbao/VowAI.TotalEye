namespace VowAI.TotalEye.ClientShared
{
    public class UpdateConfiguration : IUpdateConfiguration
    {
        public int Version { get; set; }
        public string FileUrl { get; set; }
        public string VersionUrl { get; set; }
    }
}
