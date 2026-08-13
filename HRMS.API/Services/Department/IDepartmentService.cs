using HRMS.API.DTOs.Department;

namespace HRMS.API.Services.Department
{
    public interface IDepartmentService
    {
        Task<List<Models.Department>> GetAllDepartmentsAsync();
        Task<Models.Department?> GetDepartmentByIdAsync(int departmentId);
        Task<Models.Department> CreateDepartmentAsync(CreateDepartmentDto departmentDto);
        Task<Models.Department?> UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto departmentDto);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}
