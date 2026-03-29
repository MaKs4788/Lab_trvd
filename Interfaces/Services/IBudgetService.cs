namespace LabsTRVD.Interfaces.Services
{
    public interface IBudgetService
    {
        Task SetBudgetAsync(Guid currentUserId, int month, int year, decimal limit);
        Task<decimal> GetMonthlyLimitAsync(Guid currentUserId, int month, int year);
        Task<decimal> GetUsedAmountAsync(Guid currentUserId, int month, int year);
        Task<decimal> GetRemainingBudgetAsync(Guid currentUserId, int month, int year);
        Task<double> GetUsagePercentageAsync(Guid currentUserId, int month, int year);
        Task<bool> IsBudgetExceeded(Guid currentUserId, int month, int year);
    }
}