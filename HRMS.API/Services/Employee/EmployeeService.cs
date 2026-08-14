using HRMS.API.Data;
using HRMS.API.DTOs.Employee;
using HRMS.API.Exceptions;
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
                throw new NotFoundException("Department not found");
            }

            if (employeeDto.JoiningDate.Date > DateTime.Today)
            {
                throw new BadRequestException("Joining date cannot be in the future");
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
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }
            return employee;
        }
        #endregion

        #region UpdateEmployee
        public async Task<Models.Employee?> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employees.FindAsync(employeeId);
            if (existingEmployee == null) {
                throw new NotFoundException("Employee not found");
            }
            if (employeeDto.JoiningDate.Date > DateTime.Today)
            {
                throw new BadRequestException("Joining date cannot be in the future");
            }
            var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentId == employeeDto.DepartmentId);
            if (!departmentExists)
            {
                throw new NotFoundException("Department not found");
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

        #region SearchEmployeesByName
        public async Task<List<Models.Employee>> SearchEmployeesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BadRequestException("Name cannot be empty.");
            }

            var splittedName = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var employees = await _context.Employees
                .Where(e => splittedName.Any(n => e.FirstName.Contains(n)) || splittedName.Any(n => e.LastName.Contains(n)))
                .ToListAsync();
            return employees;
        }

        #endregion

        #region GetEmployeesWithMinimumSalary
        public async Task<List<Models.Employee>> GetEmployeesWithMinimumSalaryAsync(decimal minimumSalary)
        {
            if (minimumSalary < 0)
            {
                throw new BadRequestException("Minimum salary cannot be negative.");
            }

            var employees = await _context.Employees
                .Where(e => e.Salary >= minimumSalary)
                .ToListAsync();
            return employees;
        }
        #endregion

        

    }
}
