using System.Text.Json;
using System.Text;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.ClientShared
{
    public static class Extensions
    {
        public static T Load<T>(this T configuation, Action<T, T> found, Action<T> missed, string? fullname = null)
        {
            if (fullname == null)
            {
                fullname = Path.Combine(LocalComputer.GetApplicationDirectory<T>().FullName, typeof(T).Name + ".txt");
            }

            if (File.Exists(fullname))
            {
                T? loaded = JsonSerializer.Deserialize<T>(File.ReadAllText(fullname));

                if (loaded != null)
                {
                    found(configuation, loaded);
                }
                else
                {
                    missed(configuation);
                }

                return configuation;
            }
            else
            {
                missed(configuation);
                File.AppendAllText(fullname, JsonSerializer.Serialize(configuation), Encoding.UTF8);

                return configuation;
            }
        }
    }
}
