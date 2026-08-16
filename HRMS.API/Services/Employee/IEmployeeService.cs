using HRMS.API.DTOs.Common;
using HRMS.API.DTOs.Employee;

namespace HRMS.API.Services.Employee
{
    public interface IEmployeeService
    {
        
        Task<PagedResultDto<EmployeeResponseDto>> GetEmployeesAsync(EmployeeSearchDto searchDto);
        Task<Models.Employee?> GetEmployeeByIdAsync(int employeeId);
        Task<Models.Employee> CreateEmployeeAsync(CreateEmployeeDto employee);
        Task<Models.Employee?> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);

    }
}
