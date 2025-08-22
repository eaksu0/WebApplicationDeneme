using System.ComponentModel.DataAnnotations;

namespace WebApplicationDeneme.Models
{
    public class TeamMember
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur."), StringLength(50)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Soyad zorunludur."), StringLength(50)]
        public string Surname { get; set; } = "";

        [Required(ErrorMessage = "Ünvan zorunludur."), StringLength(80)]
        public string Title { get; set; } = "";

        [StringLength(300)]
        public string? PhotoPath { get; set; }   // /uploads/team/xxx.jpg

        [Range(0, 999, ErrorMessage = "Sıra 0–999 arası olmalı.")]
        public int DisplayOrder { get; set; } = 0;
    }
}
