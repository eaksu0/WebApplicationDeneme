namespace WebApplicationDeneme.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;    
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? UserName { get; set; } 
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? Address { get; set; } 
        public bool IsActive { get; set; } = true;

        public int? RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
