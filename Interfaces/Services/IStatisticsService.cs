namespace Lab_trvd.Interfaces.Services
{
    public interface IStatisticsService
    {
        Task<object> GetDashboardAsync();
        Task<object> GetMonthlySummaryAsync();
        Task<IEnumerable<object>> GetTopExpenseCategoriesAsync(int limit = 10);
        Task<IEnumerable<object>> GetUserStatisticsAsync();
    }
}
