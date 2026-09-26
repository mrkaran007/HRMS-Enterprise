namespace HRMS.API.Repositories.Employee
{
    public interface IEmployeeRepository
    {
        Task<Models.Employee?> GetByIdAsync(int employeeId);
        Task<IReadOnlyList<Models.Employee>> SearchAsync(
            string? search,
            int? departmentId,
            decimal? minimumSalary,
            decimal? maximumSalary,
            string? sortBy,
            string? sortOrder,
            int pageNumber,
            int pageSize);
        Task<int> CountAsync(
            string? search,
            int? departmentId,
            decimal? minimumSalary,
            decimal? maximumSalary);

        Task AddAsync(Models.Employee employee);
        void Delete(Models.Employee employee); 
        void Update(Models.Employee employee);
        Task<Models.Employee?> GetForUpdateAsync(int employeeId);
        Task<bool> ExistsAsync(int employeeId);
        Task<bool> EmployeeHasDepartmentAsync(int departmentId);
    }
}
