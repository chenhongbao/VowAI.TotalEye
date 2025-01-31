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
    }
}
