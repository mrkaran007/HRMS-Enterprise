using HRMS.API.Data;
using HRMS.API.DTOs.Employee;
using HRMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories.TransferHistory
{
    public class TransferHistoryRepository : ITransferHistoryRepository
    {
        private readonly HRMSDbContext _context;
        public TransferHistoryRepository(HRMSDbContext context)
        {
            _context = context;
        }

        #region AddAsync
        public Task AddAsync(EmployeeTransferHistory transferHistory)
        {
            _context.EmployeeTransferHistories.Add(transferHistory);

            return Task.CompletedTask;
        }
        #endregion

        #region GetByEmployeeIdAsync
        public async Task<List<EmployeeTransferHistoryResponseDto>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeTransferHistories
                .AsNoTracking()
                .Where(th => th.EmployeeId  == employeeId)
                .OrderByDescending(th => th.TransferDate)
                .Select(th => new EmployeeTransferHistoryResponseDto
                {
                    EmployeeTransferHistoryId = th.EmployeeTransferHistoryId,
                    EmployeeId = th.EmployeeId,
                    EmployeeName = th.Employee.FirstName + " " + th.Employee.LastName,
                    FromDepartmentId = th.FromDepartmentId,
                    FromDepartmentName = th.FromDepartment.DepartmentName,
                    ToDepartmentId = th.ToDepartmentId,
                    ToDepartmentName = th.ToDepartment.DepartmentName,
                    TransferDate = th.TransferDate
                }).ToListAsync();
        }
        #endregion
    }
}
