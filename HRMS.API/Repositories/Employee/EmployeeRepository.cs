
using HRMS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories.Employee
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HRMSDbContext _context;
        public EmployeeRepository(HRMSDbContext context)
        {
            _context = context;
        }


        #region GetByIdAsync
        public async Task<Models.Employee?> GetByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Include(emp => emp.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }
        #endregion

        #region SearchAsync
        public async Task<IReadOnlyList<Models.Employee>> SearchAsync(string? search, int? departmentId, decimal? minimumSalary, decimal? maximumSalary, string? sortBy, string? sortOrder, int pageNumber, int pageSize)
        {
            var query = _context.Employees
                .Include(emp => emp.Department)
                .AsNoTracking();

            // search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(emp => 
                emp.FirstName.ToLower().Contains(searchLower) ||
                emp.LastName.ToLower().Contains(searchLower) ||
                emp.Email.ToLower().Contains(searchLower));
            }

            // Department filter
            if (departmentId.HasValue)
            {
                query = query.Where(emp =>
                emp.DepartmentId == departmentId.Value);
            }

            // Minimum salary
            if (minimumSalary.HasValue)
            {
                query = query.Where(emp =>
                emp.Salary >=  minimumSalary.Value);
            }

            // Maximum salary
            if (maximumSalary.HasValue)
            {
                query = query.Where(emp =>
                emp.Salary <= maximumSalary.Value);
            }

            // Sorting
            switch (sortBy?.ToLower())
            {
                case "firstname":
                    query = sortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(emp => emp.FirstName)
                        : query.OrderBy(emp => emp.FirstName); 
                    break;

                case "lastname":
                    query = sortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(emp => emp.LastName)
                        : query.OrderBy(emp => emp.LastName);
                    break;

                case "salary":
                    query = sortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(emp => emp.Salary)
                        : query.OrderBy(emp => emp.Salary);
                    break;

                case "joiningdate":
                    query = sortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(emp => emp.JoiningDate)
                        : query.OrderBy(emp => emp.JoiningDate);
                    break;

                default:
                    query = sortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(emp => emp.EmployeeId)
                        : query.OrderBy(emp => emp.EmployeeId);
                    break;
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        #endregion

        #region CountAsync
        public async Task<int> CountAsync(string? search, int? departmentId, decimal? minimumSalary, decimal? maximumSalary)
        {
            var query = _context.Employees.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(emp => 
                emp.FirstName.ToLower().Contains(searchLower) ||
                emp.LastName.ToLower().Contains(searchLower) ||
                emp.Email.ToLower().Contains(searchLower));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(emp =>
                emp.DepartmentId == departmentId.Value);
            }

            if (minimumSalary.HasValue)
            {
                query = query.Where(emp =>
                emp.Salary >=  minimumSalary.Value);
            }

            if (maximumSalary.HasValue)
            {
                query = query.Where(emp =>
                emp.Salary <= maximumSalary.Value);
            }
            
            return await query.CountAsync();
        }
        #endregion

        #region AddEmployeeAsync
        public Task AddAsync(Models.Employee employee)
        {
            _context.Employees.Add(employee);

            return Task.CompletedTask;
        }
        #endregion

        #region Delete
        public void Delete(Models.Employee employee)
        {
            _context.Employees.Remove(employee);
        }
        #endregion

        #region Update
        public void Update(Models.Employee employee)
        {
            _context.Employees.Update(employee);
        }
        #endregion

        #region GetForUpdateAsync
        public async Task<Models.Employee?> GetForUpdateAsync(int employeeId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }
        #endregion

        #region ExistsAsync
        public async Task<bool> ExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeId == employeeId);
        }
        #endregion

        #region EmployeeHasDepartment
        public async Task<bool> EmployeeHasDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .AnyAsync(e => e.DepartmentId == departmentId);
        }
        #endregion
    }
}
