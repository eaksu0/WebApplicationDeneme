using System.ComponentModel.DataAnnotations;

namespace WebApplicationDeneme.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string Surname { get; set; } = null!;

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [StringLength(100, ErrorMessage = "Email en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 en fazla 100 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
        public string? PhoneNumber { get; set; }

        [StringLength(250, ErrorMessage = "Adres en fazla 250 karakter olabilir.")]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public int? RoleId { get; set; }

        public Role? Role { get; set; }
    }
}
