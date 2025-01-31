using System.Diagnostics;

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

        public static void FreePort(int port, int recursionCount = 15)
        {
            bool available = false;

            while (recursionCount-- > 0 && available == false)
            {
                available = IsPortAvailable(port);
            }

            if (available == false)
            {
                throw new ArgumentException($"Port {port} is in use and can't be freed.");
            }
        }

        public static bool IsPortAvailable(int port)
        {
            /* System Idle Process will keep the port for a while after the server exits. */
            using (StringReader reader = new StringReader(RunCommand($"netstat -ano | findstr \":{port}\"")))
            {
                int pid = 0;
                string? line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    List<string> words = line.Split(' ').ToList().FindAll(word => string.IsNullOrEmpty(word) == false);

                    if (words.Count > 2 && words[1].Contains($":{port}") && int.TryParse(words.Last(), out pid) && pid != Environment.ProcessId /* don't kill itself */)
                    {
                        RunCommand($"taskkill /f /pid {pid}");
                        return false;
                    }
                }

                return true;
            }
        }

        public static string RunCommand(string cmd)
        {
            string input = cmd + "&exit";
            string output = "{no-output}";

            Process cmdProcess = new Process();

            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.RedirectStandardInput = true;
            cmdProcess.StartInfo.RedirectStandardError = true;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.CreateNoWindow = true;

            cmdProcess.Start();

            cmdProcess.StandardInput.AutoFlush = true;
            cmdProcess.StandardInput.WriteLine(input);

            output = cmdProcess.StandardOutput.ReadToEnd();

            cmdProcess.WaitForExit();
            cmdProcess.Close();

            return ClearHeading(output, input);
        }

        private static string ClearHeading(string text, string heading)
        {
            int index = text.IndexOf(heading);

            if (index == -1)
            {
                return text;
            }
            else
            {
                return text.Substring(text.IndexOf('\n', index + heading.Length) + 1);
            }
        }
    }
}
