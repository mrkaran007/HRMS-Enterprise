using HRMS.API.DTOs.Employee;
using HRMS.API.Models;

namespace HRMS.API.Services.Employee
{
    public interface IEmployeeService
    {
        Task<List<Models.Employee>> GetAllEmployeesAsync();
        Task<Models.Employee?> GetEmployeeByIdAsync(int employeeId);
        Task<Models.Employee> CreateEmployeeAsync(CreateEmployeeDto employee);
        Task<Models.Employee?> UpdateEmployeeAsync(int id, UpdateEmployeeDto employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);
    }
}
