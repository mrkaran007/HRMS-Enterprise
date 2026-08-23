using HRMS.API.Data;
using HRMS.API.DTOs.Common;
using HRMS.API.DTOs.Employee;
using HRMS.API.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
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
            return await GetEmployeeByIdAsync(newEmployee.EmployeeId);
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

        #region GetEmployees
        public async Task<PagedResultDto<EmployeeResponseDto>> GetEmployeesAsync(EmployeeSearchDto searchDto)
        {
            // Pagination Validation
            if (searchDto.PageSize < 1 || searchDto.PageSize > 100)
            {
                throw new BadRequestException("Page size must be between 1 and 100.");
            }
            if (searchDto.PageNumber < 1)
            {
                throw new BadRequestException("Page number must be greater than 0.");
            }

            // Salary Validation
            if ((searchDto.MinimumSalary.HasValue && searchDto.MinimumSalary.Value < 0) || (searchDto.MaximumSalary.HasValue && searchDto.MaximumSalary.Value < 0))
            {
                throw new BadRequestException("Salary values cannot be negative.");
            }
            
            if (searchDto.MaximumSalary.HasValue && searchDto.MinimumSalary.HasValue 
                && searchDto.MaximumSalary.Value < searchDto.MinimumSalary.Value)
            {
                throw new BadRequestException("Maximum salary must be greater than or equal to minimum salary.");
            }

            // This will improve performance for read-only queries
            // It also prevents the context from tracking the entities, which can save memory and improve performance for read-only queries.
            var query = _context.Employees.AsNoTracking();    // This return as IQueryable<Employee> which allows for further filtering and sorting before executing the query.

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var searchLower = searchDto.Search.ToLower();
                query = query.Where(e => 
                e.FirstName.ToLower().Contains(searchLower)
                || e.LastName.ToLower().Contains(searchLower)
                || e.Email.ToLower().Contains(searchLower)
                );
            }

            // Filter by DepartmentId if provided
            if (searchDto.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == searchDto.DepartmentId.Value);
            }

            // Filter by Salary range if provided
            if (searchDto.MinimumSalary.HasValue)
            {
                query = query.Where(e => e.Salary >= searchDto.MinimumSalary.Value);
            }

            if(searchDto.MaximumSalary.HasValue)
            {
                query = query.Where(e => e.Salary <= searchDto.MaximumSalary.Value);
            }

            // Count total filtered records before pagination
            var totalRecords = await query.CountAsync();

            // Sorting
            switch(searchDto.SortBy?.ToLower())
            {
                case "firstname":
                    query = searchDto.SortOrder?.ToLower() == "desc" ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName);
                    break;
                case "lastname":
                    query = searchDto.SortOrder?.ToLower() == "desc" ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName);
                    break;
                case "salary":
                    query = searchDto.SortOrder?.ToLower() == "desc" ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary);
                    break;
                case "joiningdate":
                    query = searchDto.SortOrder?.ToLower() == "desc" ? query.OrderByDescending(e => e.JoiningDate) : query.OrderBy(e => e.JoiningDate);
                    break;
                default:
                    // Default sorting by EmployeeId
                    query = searchDto.SortOrder?.ToLower() == "desc" ? query.OrderByDescending(e => e.EmployeeId) : query.OrderBy(e => e.EmployeeId);
                    break;
            }

            // Projection + Pagination
            var employees = await query
                .Select(e => new EmployeeResponseDto
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                Salary = e.Salary,
                JoiningDate = e.JoiningDate,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department!.DepartmentName
            }).Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / searchDto.PageSize);

            return new PagedResultDto<EmployeeResponseDto>
            {
                Items = employees,
                TotalPages = totalPages,
                TotalRecords = totalRecords,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize
            };
        }
        #endregion

        #region GetEmployeeById
        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(emp => emp.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }
            return MapToEmployeeResponseDto(employee);
        }
        #endregion

        #region UpdateEmployee
        public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employeeDto)
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
            return await GetEmployeeByIdAsync(employeeId);
        }
        #endregion

        #region TransferEmployee
        public async Task<EmployeeTransferHistoryResponseDto> TransferEmployeeAsync(int employeeId, TransferEmployeeRequestDto transferDto)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync( e => e.EmployeeId == employeeId);
            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            var targetDepartment = await _context.Departments
                .FirstOrDefaultAsync( d => d.DepartmentId == transferDto.TargetDepartmentId);
            if(targetDepartment == null)
            {
                throw new NotFoundException("Target department not found");
            }

            if (employee.DepartmentId == transferDto.TargetDepartmentId)
            {
                throw new BadRequestException("Employee is already assigned to this department.");
            }

            var fromDepartmentId = employee.DepartmentId;
            var fromDepartmentName = employee.Department!.DepartmentName;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var transferHistory = new Models.EmployeeTransferHistory
                {
                    EmployeeId = employeeId,
                    FromDepartmentId = fromDepartmentId,
                    ToDepartmentId = transferDto.TargetDepartmentId,
                    TransferDate = DateTime.UtcNow
                };

                _context.EmployeeTransferHistories.Add(transferHistory);

                employee.DepartmentId = transferDto.TargetDepartmentId;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new EmployeeTransferHistoryResponseDto
                {
                    EmployeeTransferHistoryId = transferHistory.EmployeeTransferHistoryId,
                    EmployeeId = employeeId,
                    EmployeeName = employee.FirstName + " " + employee.LastName,
                    FromDepartmentId = fromDepartmentId,
                    FromDepartmentName = fromDepartmentName,
                    ToDepartmentId = targetDepartment.DepartmentId,
                    ToDepartmentName = targetDepartment.DepartmentName,
                    TransferDate = transferHistory.TransferDate,
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
        #endregion

        #region GetEmployeeTransferHistory
        public async Task<List<EmployeeTransferHistoryResponseDto>> GetEmployeeTransferHistoryAsync(int employeeId)
        {
            var employeeExists = await _context.Employees.AnyAsync(e=> e.EmployeeId == employeeId);
            if (!employeeExists)
            {
                throw new NotFoundException("Employee not found");
            }

            var employeeTransferHistories = await _context.EmployeeTransferHistories
                .AsNoTracking()
                .Where(th => th.EmployeeId == employeeId)
                .OrderByDescending(th => th.TransferDate)
                .Select(th => new EmployeeTransferHistoryResponseDto
                {
                    EmployeeTransferHistoryId = th.EmployeeTransferHistoryId,
                    EmployeeId = th.EmployeeId,
                    EmployeeName = th.Employee.FirstName + " " + th.Employee.LastName,
                    FromDepartmentId = th.FromDepartmentId,
                    FromDepartmentName = th.FromDepartment.DepartmentName,
                    ToDepartmentId = th.ToDepartmentId,
                    ToDepartmentName= th.ToDepartment.DepartmentName,
                    TransferDate = th.TransferDate
                })
                .ToListAsync();
            return employeeTransferHistories;
        }
        #endregion

        #region MapToEmployeeResponseDto
        private static EmployeeResponseDto MapToEmployeeResponseDto(Models.Employee employee)
        {
            return new EmployeeResponseDto
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FirstName + " " + employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary,
                JoiningDate = employee.JoiningDate,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName ?? "N/A" // Handle null Department case
            };
        }
        #endregion


    }
}
