namespace HRMS.API.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<EmployeeTransferHistory> TransfersFrom { get; set; } = new List<EmployeeTransferHistory>();
        public ICollection<EmployeeTransferHistory> TransfersTo { get; set; } = new List<EmployeeTransferHistory>();

    }
}
