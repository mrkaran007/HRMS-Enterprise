using HRMS.API.Data;
using HRMS.API.DTOs.Department;
using HRMS.API.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HRMSDbContext _context;

        public DepartmentService(HRMSDbContext context)
        {
            _context = context;
        }

        #region CreateDepartmentAsync
        public async Task<Models.Department> CreateDepartmentAsync(CreateDepartmentDto departmentDto)
        {
            var departmentName = departmentDto.DepartmentName.Trim();
            var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentName == departmentName);
            if (departmentExists)
            {
                throw new ConflictException("Department already exists");
            }

            var newDepartment = new Models.Department
            {
                DepartmentName = departmentName
            };
            _context.Departments.Add(newDepartment);
            await _context.SaveChangesAsync();
            return newDepartment;
        }
        #endregion

        #region DeleteDepartmentAsync
        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var existingDepartment = await _context.Departments.FindAsync(departmentId);
            if (existingDepartment == null)
            {
                return false;
            }

            var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == departmentId);
            if (hasEmployees)
            {
                throw new ConflictException("Department cannot be deleted because employees are assigned to it.");
            }

            _context.Departments.Remove(existingDepartment);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetAllDepartmentsAsync
        public async Task<List<Models.Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion

        #region GetDepartmentByIdAsync
        public async Task<Models.Department?> GetDepartmentByIdAsync(int departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);
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
            var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentName == departmentName && d.DepartmentId != departmentId);
            if (departmentExists)
            {
                throw new ConflictException("Department already exists");
            }

            var existingDepartment = await _context.Departments.FindAsync(departmentId);
            if (existingDepartment == null)
            {
                throw new NotFoundException("Department not found");
            }

            // Update properties
            existingDepartment.DepartmentName = departmentName;
            
            await _context.SaveChangesAsync();

            return existingDepartment;
        }
        #endregion

    }
}
