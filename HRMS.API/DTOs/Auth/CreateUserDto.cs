using System.ComponentModel.DataAnnotations;

namespace HRMS.API.DTOs.Auth
{
    public class CreateUserDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]  
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
    }
}
