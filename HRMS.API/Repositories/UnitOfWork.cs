using HRMS.API.Data;

namespace HRMS.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HRMSDbContext _context;
        public UnitOfWork(HRMSDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync()
        {
           return await _context.SaveChangesAsync();
        }
    }
}
