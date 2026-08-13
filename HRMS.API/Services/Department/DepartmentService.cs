using HRMS.API.Data;
using HRMS.API.DTOs.Department;
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
                throw new InvalidOperationException("Department already exists");
            }

            var newDepartment = new Models.Department
            {
                DepartmentName = departmentDto.DepartmentName
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

            _context.Departments.Remove(existingDepartment);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetAllDepartmentsAsync
        public async Task<List<Models.Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }
        #endregion

        #region GetDepartmentByIdAsync
        public async Task<Models.Department?> GetDepartmentByIdAsync(int departmentId)
        {
            return await _context.Departments.FindAsync(departmentId);
        }
        #endregion

        #region UpdateDepartmentAsync
        public async Task<Models.Department?> UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto departmentDto)
        {
            var existingDepartment = await _context.Departments.FindAsync(departmentId);
            if (existingDepartment == null)
            {
                return null;
            }

            // Update properties
            existingDepartment.DepartmentName = departmentDto.DepartmentName;
            
            await _context.SaveChangesAsync();

            return existingDepartment;
        }
        #endregion

    }
}
