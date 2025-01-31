using System.Text;

namespace VowAI.TotalEye.Tools
{
    public static class Extensions
    {
        public static string? GetCommandParameter(this string[] args, string option, string? defaultValue = null)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith(option))
                {
                    return arg.Substring(option.Length);
                }
            }

            return defaultValue;
        }

        public static V WriteString<T, V>(this V content, string? file = null, Encoding? encoding = null)
        {
            string path = Path.Combine(LocalComputer.GetApplicationDirectory<T>().FullName, $"{typeof(T).Name}.log");

            using (TextWriter writer = new StreamWriter(path: path, append: true, encoding: encoding ?? Encoding.UTF8))
            {
                writer.WriteLine(DateTime.Now);
                writer.WriteLine(content);
            }

            return content;
        }

        public static string WriteString<T>(this string content, string? file = null, Encoding? encoding = null)
        {
            return content.WriteString<T, string>(file, encoding);
        }

        public static Exception WriteString<T>(this Exception exception, string? file = null, Encoding? encoding = null)
        {
            return exception.WriteString<T, Exception>(file, encoding);
        }
    }
}
