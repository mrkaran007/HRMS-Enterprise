
using HRMS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories.User
{

    public class UserRepository : IUserRepository
    {
        private readonly HRMSDbContext _context;
        public UserRepository(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> EmployeeHasUserAccountAsync(int employeeId)
        {
            return await _context.Users
                .AnyAsync(u => u.EmployeeId == employeeId);
        }

        public async Task<Models.User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == userName);
        }

        public void Add(Models.User user)
        {
            _context.Users.Add(user);
        }
    }
}
