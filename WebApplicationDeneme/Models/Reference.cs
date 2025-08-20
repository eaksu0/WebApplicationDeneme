namespace WebApplicationDeneme.Models
{
    public class Reference
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;   // Referans adı
        public string? ImagePath { get; set; }             // /uploads/references/xxx.jpg
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
