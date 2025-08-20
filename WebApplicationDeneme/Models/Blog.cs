using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationDeneme.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;  // Blog başlığı

        [Required]
        public string ShortText { get; set; } = string.Empty; // Kısa yazı (özet)

        [Required]
        public string LongText { get; set; } = string.Empty;  // Uzun yazı

        public string? ImagePath { get; set; }   // Resim yolu (wwwroot/uploads/blogs/)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
