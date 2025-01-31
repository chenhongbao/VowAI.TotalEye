namespace VowAI.TotalEye.ClientShared
{
    public interface IUpdateConfiguration
    {
        public int Version { get; }
        public string FileUrl { get; }
        public string VersionUrl { get; }
    }
}