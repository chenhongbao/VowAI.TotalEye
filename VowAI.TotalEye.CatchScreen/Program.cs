using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using VowAI.TotalEye.Tools;


namespace VowAI.TotalEye.CatchScreen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Bitmap bmp = CaptureScreen())
            {
                (string path, ImageFormat format) = GetDestination(args);

                bmp.Save(path, format);

                Console.WriteLine(new FileInfo(path).FullName);
                Console.Out.Flush();
            }
        }

        private static (string, ImageFormat) GetDestination(string[] args)
        {
            string? candidatePath = args.GetCommandParameter("/Destination:");
            string? candidateDirectory = Path.GetDirectoryName(candidatePath);
            string path;
            ImageFormat format;

            if (candidatePath == null)
            {
                path = Path.Combine(LocalComputer.GetApplicationDirectory<Program>().FullName, $"{Guid.NewGuid()}.bmp");
                format = ImageFormat.Bmp;
            }
            else
            {
                /* Candidate directory is root directory or a normal directory. */

                if (string.IsNullOrEmpty(candidateDirectory) == false && Directory.Exists(candidateDirectory) == false)
                {
                    _ = Directory.CreateDirectory(candidateDirectory);
                }

                path = candidatePath;
                format = GetNamedFormat(Path.GetExtension(candidatePath));
            }

            return (path, format);
        }

        private static ImageFormat GetNamedFormat(string? extension)
        {
            if (extension == null)
            {
                return ImageFormat.Bmp;
            }
            else
            {
                return extension.ToLower() switch
                {
                    ".gif" => ImageFormat.Gif,
                    ".jpg" or  "jpeg" => ImageFormat.Jpeg,
                    ".bmp" => ImageFormat.Bmp,
                    ".png" => ImageFormat.Png,
                    _ => throw new ArgumentException($"非法图像格式：{extension}。"),
                };
            }
        }

        private static Bitmap CaptureScreen()
        {
            Rectangle rectangle;

            if (Screen.PrimaryScreen != null)
            {
                rectangle = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            }
            else
            {
                /* failover */
                rectangle = new Rectangle(0, 0, 1024, 768);
            }

            Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);
            Graphics gp = Graphics.FromImage(bmp);

            gp.CopyFromScreen(0, 0, 0, 0, rectangle.Size);

            return bmp;
        }
    }
}
