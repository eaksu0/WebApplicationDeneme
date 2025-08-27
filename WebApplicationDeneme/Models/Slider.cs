using System.ComponentModel.DataAnnotations;

namespace WebApplicationDeneme.Models
{
    public class Slider
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Heading { get; set; } = "";          

        [StringLength(220)]
        public string? Text { get; set; }                  
        [StringLength(40)]
        public string? LinkText { get; set; }              

        [Url, StringLength(200)]
        public string? LinkUrl { get; set; }               

        [StringLength(220)]
        public string? ImagePath { get; set; }             

        public int SortOrder { get; set; } = 0;            

        public bool IsActive { get; set; } = true;         

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
