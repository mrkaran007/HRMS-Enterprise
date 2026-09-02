namespace HRMS.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? EmployeeId { get; set; }    // Nullable b/c Admin is user but it is not an Employee
        public Employee? Employee { get; set; } // Navigation property
    }
}
