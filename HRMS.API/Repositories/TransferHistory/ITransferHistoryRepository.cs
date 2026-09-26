using HRMS.API.DTOs.Employee;
using HRMS.API.Models;

namespace HRMS.API.Repositories.TransferHistory
{
    public interface ITransferHistoryRepository
    {
        Task AddAsync(EmployeeTransferHistory transferHistory);
        Task<List<EmployeeTransferHistoryResponseDto>> GetByEmployeeIdAsync(int employeeId);
    }
}
