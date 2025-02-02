namespace VowAI.TotalEye.Server.Models
{
    public class SessionScreenshot
    {
        public int ScreenshotId { get; set; }
        public ImageItem? Image { get; set; }
        public UserInfoSession? Session { get; set; }
    }
}