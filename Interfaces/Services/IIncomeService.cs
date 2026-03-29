using LabsTRVD.DTOs.ServicesDTOs;

namespace LabsTRVD.Interfaces.Services
{
    public interface IIncomeService
    {
        Task<IEnumerable<IncomeDtoResponse>> GetUserIncomesAsync(Guid currentUserId);
        Task<IEnumerable<IncomeDtoResponse>> GetByPeriodAsync(Guid currentUserId, DateTime from, DateTime to);
        Task<IncomeDtoResponse?> GetByIdAsync(int id, Guid currentUserId);
        Task<IncomeDtoResponse> AddIncomeAsync(IncomeDto incomeDto, Guid currentUserId);
        Task<IncomeDtoResponse> UpdateIncomeAsync(int id, IncomeDto incomeDto, Guid currentUserId);
        Task DeleteIncomeAsync(int id, Guid currentUserId);
        Task<decimal> GetTotalForPeriodAsync(Guid currentUserId, DateTime from, DateTime to);
        Task<decimal> GetTotalCurrentMonthAsync(Guid currentUserId);
        Task<decimal> GetTotalAsync(Guid currentUserId);
    }
}