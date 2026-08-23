using HRMS.API.DTOs.Common;
using HRMS.API.DTOs.Employee;

namespace HRMS.API.Services.Employee
{
    public interface IEmployeeService
    {
        
        Task<PagedResultDto<EmployeeResponseDto>> GetEmployeesAsync(EmployeeSearchDto searchDto);
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int employeeId);
        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto employee);
        Task<EmployeeResponseDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);

        Task<EmployeeTransferHistoryResponseDto> TransferEmployeeAsync(int employeeId, TransferEmployeeRequestDto transferDto);
        Task<List<EmployeeTransferHistoryResponseDto>> GetEmployeeTransferHistoryAsync(int  employeeId);

    }
}
