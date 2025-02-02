namespace VowAI.TotalEye.Server.Models
{
    public class ImageItem
    {
        public int ImageId { get; set; }
        public string? FileName { get; set; }
        public byte[]? Data { get; set; }
    }
}
