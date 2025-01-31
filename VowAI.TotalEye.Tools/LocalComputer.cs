namespace VowAI.TotalEye.Tools
{
    public class LocalComputer
    {
        public static DirectoryInfo GetApplicationDirectory<T>()
        {
            string subDirectory = (typeof(T).Namespace ?? "").Replace(".", "/");
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), subDirectory);

            if (Directory.Exists(path) == false)
            {
                return Directory.CreateDirectory(path);
            }
            else
            {
                return new DirectoryInfo(path);
            }
        }
    }
}
