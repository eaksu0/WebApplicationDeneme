using System.ComponentModel.DataAnnotations;

namespace WebApplicationDeneme.Models
{
    public class Work
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "The Description field is required"), StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public WorkStatus Status { get; set; } = WorkStatus.Yapilacak;

        [Required]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Orta;
    }

    public enum WorkStatus
    {
        Yapilacak,
        Yapiliyor,
        Yapildi
    }

    public enum PriorityLevel
    {
        Dusuk,
        Orta,
        Yuksek
    }
}
