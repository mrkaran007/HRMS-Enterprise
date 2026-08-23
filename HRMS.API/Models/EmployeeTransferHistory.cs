namespace HRMS.API.Models
{
    public class EmployeeTransferHistory
    {
        public int EmployeeTransferHistoryId { get; set; }
        public int EmployeeId { get; set; }
        public int FromDepartmentId { get; set; }
        public int ToDepartmentId { get; set; }
        public DateTime TransferDate { get; set; }
        public Employee Employee { get; set; } = null!;
        public Department FromDepartment { get; set; } = null!;
        public Department ToDepartment { get; set;} = null!;
    }
}
