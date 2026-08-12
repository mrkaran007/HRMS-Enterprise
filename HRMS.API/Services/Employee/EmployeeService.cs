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
        public async Task<Models.Employee> CreateEmployeeAsync(CreateEmployeeDto employee)
        {
            var isDepartmentExists = await _context.Departments.AnyAsync(d=> d.DepartmentId == employee.DepartmentId);

            if (!isDepartmentExists) {
                throw new InvalidOperationException("Department not found");
            }

            if (employee.JoiningDate.Date > DateTime)
            {
                
            }

            var newEmployee = new Models.Employee
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary,
                JoiningDate = employee.JoiningDate,
                DepartmentId = employee.DepartmentId
            };
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();
            return newEmployee;
        }
        #endregion

        #region DeleteEmployee
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            throw new NotImplementedException();
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
        public async Task<Models.Employee?> UpdateEmployeeAsync(int id, UpdateEmployeeDto employee)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
