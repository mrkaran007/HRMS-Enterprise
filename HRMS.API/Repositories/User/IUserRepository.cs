namespace HRMS.API.Repositories.User
{
    public interface IUserRepository
    {
        Task<Models.User?> GetByUserNameAsync(string userName);
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> EmployeeHasUserAccountAsync(int employeeId);
        void Add(Models.User user);
    }
}
