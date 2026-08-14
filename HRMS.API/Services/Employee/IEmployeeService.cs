using HRMS.API.DTOs.Employee;

namespace HRMS.API.Services.Employee
{
    public interface IEmployeeService
    {
        Task<List<Models.Employee>> GetAllEmployeesAsync();
        Task<Models.Employee?> GetEmployeeByIdAsync(int employeeId);
        Task<Models.Employee> CreateEmployeeAsync(CreateEmployeeDto employee);
        Task<Models.Employee?> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employee);
        Task<bool> DeleteEmployeeAsync(int employeeId);

        Task<List<Models.Employee>> SearchEmployeesByNameAsync(string name);
        Task<List<Models.Employee>> GetEmployeesWithMinimumSalaryAsync(decimal minimumSalary);
    }
}
