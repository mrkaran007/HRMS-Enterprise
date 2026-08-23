namespace HRMS.API.DTOs.Employee
{
    public class EmployeeTransferHistoryResponseDto
    {
        public int EmployeeTransferHistoryId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int FromDepartmentId { get; set; }
        public string FromDepartmentName { get; set; } = string.Empty;
        public int ToDepartmentId { get; set; }
        public string ToDepartmentName { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
    }
}
