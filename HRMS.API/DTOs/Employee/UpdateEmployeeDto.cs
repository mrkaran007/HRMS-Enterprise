using System.ComponentModel.DataAnnotations;

namespace HRMS.API.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Range(0, 10000000)]
        public decimal Salary { get; set; }

        [Required]
        public DateTime JoiningDate { get; set; }

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }
}
