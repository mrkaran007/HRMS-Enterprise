using HRMS.API.Data;
using HRMS.API.DTOs.Employee;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HRMSDbContext _context;

        public EmployeeService(HRMSDbContext context)
        {
            _context = context;
        }

        #region CreateEmployee
        public async Task<Models.Employee> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            var departmentExists = await _context.Departments.AnyAsync(d=> d.DepartmentId == employeeDto.DepartmentId);

            if (!departmentExists) {
                throw new InvalidOperationException("Department not found");
            }

            if (employeeDto.JoiningDate.Date > DateTime.Today)
            {
                throw new InvalidOperationException("Joining date cannot be in the future");
            }

            var newEmployee = new Models.Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Phone = employeeDto.Phone,
                Salary = employeeDto.Salary,
                JoiningDate = employeeDto.JoiningDate,
                DepartmentId = employeeDto.DepartmentId
            };
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();
            return newEmployee;
        }
        #endregion

        #region DeleteEmployee
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var existingEmployee = await _context.Employees.FindAsync(employeeId);
            if (existingEmployee == null)
            {
                return false;
            }

            _context.Employees.Remove(existingEmployee);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetAllEmployees
        public async Task<List<Models.Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }
        #endregion

        #region GetEmployeeById
        public async Task<Models.Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees.FindAsync(employeeId);
        }
        #endregion

        #region UpdateEmployee
        public async Task<Models.Employee?> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employees.FindAsync(employeeId);
            if (existingEmployee == null) {
                return null;
            }
            if (employeeDto.JoiningDate.Date > DateTime.Today)
            {
                throw new InvalidOperationException("Joining date cannot be in the future");
            }
            var isDepartmentExists = await _context.Departments.AnyAsync(d => d.DepartmentId == employeeDto.DepartmentId);
            if (!isDepartmentExists)
            {
                throw new InvalidOperationException("Department not found");
            }

            // Update the properties of the existing employee with the new values
            existingEmployee.FirstName = employeeDto.FirstName;
            existingEmployee.LastName = employeeDto.LastName;
            existingEmployee.Email = employeeDto.Email;
            existingEmployee.Phone = employeeDto.Phone;
            existingEmployee.Salary = employeeDto.Salary;
            existingEmployee.JoiningDate = employeeDto.JoiningDate;
            existingEmployee.DepartmentId = employeeDto.DepartmentId;
            
            
            await _context.SaveChangesAsync();
            return existingEmployee;
        }
        #endregion

    }
}
