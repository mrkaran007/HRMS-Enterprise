namespace HRMS.API.Repositories.Department
{
    public interface IDepartmentRepository
    {
        Task<Models.Department?> GetByIdAsync(int departmentId);
        Task<bool> ExistsAsync(int departmentId);
        Task<bool> ExistsAsync(string departmentName);
        Task<bool> ExistsAsync(int departmentId, string departmentName);
        Task<List<Models.Department>> GetAsync();
        void Add(Models.Department department);
        void Delete(Models.Department department);
        void Update(Models.Department department);

    }
}
