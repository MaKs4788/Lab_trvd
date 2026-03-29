using LabsTRVD.DTOs.ServicesDTOs;

namespace LabsTRVD.Interfaces.Services
{
    public interface IExpenseService
    {
        // Отримати всі витрати поточного користувача
        Task<IEnumerable<ExpenseDtoResponse>> GetUserExpensesAsync(Guid currentUserId);

        // Отримати витрати за період для поточного користувача
        Task<IEnumerable<ExpenseDtoResponse>> GetByPeriodAsync(Guid currentUserId, DateTime from, DateTime to);

        // Отримати конкретну витрату за ID, тільки якщо вона належить поточному користувачу
        Task<ExpenseDtoResponse?> GetByIdAsync(int id, Guid currentUserId);

        // Додати нову витрату (UserId береться з currentUserId, а не з DTO)
        Task<ExpenseDtoResponse> AddExpenseAsync(ExpenseDto expenseDto, Guid currentUserId);

        // Оновити витрату, тільки якщо вона належить поточному користувачу
        Task<ExpenseDtoResponse> UpdateExpenseAsync(int id, ExpenseDto expenseDto, Guid currentUserId);

        // Видалити витрату, тільки якщо вона належить поточному користувачу
        Task DeleteExpenseAsync(int id, Guid currentUserId);

        // Сума за період для поточного користувача
        Task<decimal> GetTotalForPeriodAsync(Guid currentUserId, DateTime from, DateTime to);

        // Сума за поточний місяць
        Task<decimal> GetTotalCurrentMonthAsync(Guid currentUserId);

        // Загальна сума всіх витрат поточного користувача
        Task<decimal> GetTotalAsync(Guid currentUserId);
    }
}