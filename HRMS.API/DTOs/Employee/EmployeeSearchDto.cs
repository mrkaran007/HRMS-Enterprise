namespace HRMS.API.DTOs.Employee
{
    public class EmployeeSearchDto
    {
        public string? Search { get; set; }
        public int? DepartmentId { get; set; }
        public decimal? MinimumSalary { get; set; }
        public decimal? MaximumSalary { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
