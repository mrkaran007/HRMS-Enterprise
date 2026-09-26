using HRMS.API.Data;
using HRMS.API.DTOs.Common;
using HRMS.API.DTOs.Employee;
using HRMS.API.Exceptions;
using HRMS.API.Repositories;
using HRMS.API.Repositories.Department;
using HRMS.API.Repositories.Employee;
using HRMS.API.Repositories.TransferHistory;

namespace HRMS.API.Services.Employee
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HRMSDbContext _context;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITransferHistoryRepository _transferHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeService(HRMSDbContext context,
            IEmployeeRepository employeeRepository,
            ITransferHistoryRepository transferHistoryRepository,
            IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
            _transferHistoryRepository = transferHistoryRepository;
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
        }

        #region CreateEmployee
        public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            var departmentExists = await _departmentRepository.ExistsAsync(employeeDto.DepartmentId);

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

            await _employeeRepository.AddAsync(newEmployee);
            await _unitOfWork.SaveChangesAsync();
            return await GetEmployeeByIdAsync(newEmployee.EmployeeId);
        }
        #endregion

        #region DeleteEmployee
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return false;
            }

            _employeeRepository.Delete(employee);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetEmployees
        public async Task<PagedResultDto<EmployeeResponseDto>> GetEmployeesAsync(EmployeeSearchDto searchDto)
        {
            // Business/API validation

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


            // Count total filtered records before pagination
            // Get total matching records
            var totalRecords = await _employeeRepository.CountAsync(
                searchDto.Search,
                searchDto.DepartmentId,
                searchDto.MinimumSalary,
                searchDto.MaximumSalary);



            // Projection + Pagination
            // Get requested page
            var employees = await _employeeRepository.SearchAsync(
                searchDto.Search,
                searchDto.DepartmentId,
                searchDto.MinimumSalary,
                searchDto.MaximumSalary,
                searchDto.SortBy,
                searchDto.SortOrder,
                searchDto.PageNumber,
                searchDto.PageSize);

            // Convert Entity -> Response DTO
            var employeeDtos = employees
                .Select(MapToEmployeeResponseDto)
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalRecords / searchDto.PageSize);

            return new PagedResultDto<EmployeeResponseDto>
            {
                Items = employeeDtos,
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
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
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
            var employee = await _employeeRepository.GetForUpdateAsync(employeeId);

            if (employee == null) {
                throw new NotFoundException("Employee not found");
            }

            if (employeeDto.JoiningDate.Date > DateTime.Today)
            {
                throw new BadRequestException("Joining date cannot be in the future");
            }

            var departmentExists = await _departmentRepository.ExistsAsync(employeeDto.DepartmentId);

            if (!departmentExists)
            {
                throw new NotFoundException("Department not found");
            }

            // Update the properties of the existing employee with the new values
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.Phone = employeeDto.Phone;
            employee.Salary = employeeDto.Salary;
            employee.JoiningDate = employeeDto.JoiningDate;
            employee.DepartmentId = employeeDto.DepartmentId;
            
            await _unitOfWork.SaveChangesAsync();

            return await GetEmployeeByIdAsync(employeeId);
        }
        #endregion

        #region TransferEmployee
        public async Task<EmployeeTransferHistoryResponseDto> TransferEmployeeAsync(int employeeId, TransferEmployeeRequestDto transferDto)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            var targetDepartment = await _departmentRepository.GetByIdAsync(transferDto.TargetDepartmentId);
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

                await _transferHistoryRepository.AddAsync(transferHistory);

                employee.DepartmentId = transferDto.TargetDepartmentId;

                _employeeRepository.Update(employee);

                await _unitOfWork.SaveChangesAsync();

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
            var employeeExists = await _employeeRepository.ExistsAsync(employeeId);
            if (!employeeExists)
            {
                throw new NotFoundException("Employee not found");
            }
            return await _transferHistoryRepository.GetByEmployeeIdAsync(employeeId);
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
