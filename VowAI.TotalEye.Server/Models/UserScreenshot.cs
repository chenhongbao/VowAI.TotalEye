namespace VowAI.TotalEye.Server.Models
{
    public class UserScreenshot
    {
        public int ScreenshotId { get; set; }
        public User? User { get; set; }
        public ImageItem? Image { get; set; }
    }
}