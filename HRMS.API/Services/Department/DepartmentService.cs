using HRMS.API.Data;
using HRMS.API.DTOs.Department;
using HRMS.API.Exceptions;
using HRMS.API.Repositories;
using HRMS.API.Repositories.Department;
using HRMS.API.Repositories.Employee;

namespace HRMS.API.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        #region CreateDepartmentAsync
        public async Task<Models.Department> CreateDepartmentAsync(CreateDepartmentDto departmentDto)
        {
            var departmentName = departmentDto.DepartmentName.Trim();
            var departmentExists = await _departmentRepository.ExistsAsync(departmentName);
            if (departmentExists)
            {
                throw new ConflictException("Department already exists");
            }

            var newDepartment = new Models.Department
            {
                DepartmentName = departmentName
            };

            _departmentRepository.Add(newDepartment);

            await _unitOfWork.SaveChangesAsync();
            return newDepartment;
        }
        #endregion

        #region DeleteDepartmentAsync
        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var existingDepartment = await _departmentRepository.GetByIdAsync(departmentId);
            if (existingDepartment == null)
            {
                return false;
            }

            var hasEmployees = await _employeeRepository.EmployeeHasDepartmentAsync(departmentId);
            if (hasEmployees)
            {
                throw new ConflictException("Department cannot be deleted because employees are assigned to it.");
            }

            _departmentRepository.Delete(existingDepartment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetAllDepartmentsAsync
        public async Task<List<Models.Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAsync();
        }
        #endregion

        #region GetDepartmentByIdAsync
        public async Task<Models.Department?> GetDepartmentByIdAsync(int departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
            {
                throw new NotFoundException("Department not found");
            }
            return department;
        }
        #endregion

        #region UpdateDepartmentAsync
        public async Task<Models.Department?> UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto departmentDto)
        {
            var departmentName = departmentDto.DepartmentName.Trim();
            var departmentExists = await _departmentRepository.ExistsAsync(departmentId, departmentName);
            if (departmentExists)
            {
                throw new ConflictException("Department already exists");
            }

            var existingDepartment = await _departmentRepository.GetByIdAsync(departmentId);
            if (existingDepartment == null)
            {
                throw new NotFoundException("Department not found");
            }

            // Update properties
            existingDepartment.DepartmentName = departmentName;
            _departmentRepository.Update(existingDepartment);
            
            await _unitOfWork.SaveChangesAsync();

            return existingDepartment;
        }
        #endregion

    }
}
