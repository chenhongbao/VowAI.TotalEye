using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.ClientShared
{
    public static class UpdateConfigurationExtensions
    {
        public static UpdateConfiguration Deserialize(this UpdateConfiguration instance, string text)
        {
            string[] lines = text.Split('\n');

            if (lines.Length < 2)
            {
                throw new ArgumentException("Invalid string representation.");
            }

            instance.Version = int.Parse(lines[0].Trim());
            instance.VersionUrl = lines[1].Trim();

            return instance;
        }

        public static string GetLocalPath(this UpdateConfiguration instance)
        {
            return Path.Combine(LocalComputer.GetApplicationDirectory<UpdateConfiguration>().FullName, $"{typeof(UpdateConfiguration).Name}.txt");
        }

        public static string GetDownloadDirectory(this UpdateConfiguration instance)
        {
            return LocalComputer.GetApplicationDirectory<UpdateConfiguration>().FullName;
        }
    }
}
