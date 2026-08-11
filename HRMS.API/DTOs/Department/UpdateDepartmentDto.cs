using System.ComponentModel.DataAnnotations;

namespace HRMS.API.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string DepartmentName { get; set; } = string.Empty;
    }
}
