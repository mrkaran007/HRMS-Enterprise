using HRMS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories.Department
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly HRMSDbContext _context;
        public DepartmentRepository(HRMSDbContext context)
        {
            _context = context;
        }

        #region GetByIdAsync
        public async Task<Models.Department?> GetByIdAsync(int departmentId)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => 
                    d.DepartmentId == departmentId);
        }
        #endregion

        #region ExistsAsync
        public async Task<bool> ExistsAsync(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId);
        }
        #endregion

        #region ExistsByNameAsync
        public async Task<bool> ExistsAsync(string departmentName)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentName == departmentName);
        }
        #endregion

        #region ExistsAsync with two parameter
        public async Task<bool> ExistsAsync(int departmentId, string departmentName)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentName == departmentName && d.DepartmentId != departmentId);
        }
        #endregion

        #region GetAllAsync
        public async Task<List<Models.Department>> GetAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion

        #region Add
        public void Add(Models.Department department)
        {
            _context.Departments.Add(department);
        }
        #endregion

        #region Delete
        public void Delete(Models.Department department)
        {
            _context.Departments.Remove(department);
        }
        #endregion

        #region Update
        public void Update(Models.Department department)
        {
            _context.Departments.Update(department);
        }
        #endregion
    }
}
