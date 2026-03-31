using Lab_trvd.Interfaces.Services;
using LabsTRVD.Data;
using LabsTRVD.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace LabsTRVD.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetDashboardAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users
                .Where(u => u.LastLoginAt.HasValue &&
                       u.LastLoginAt >= DateTime.Now.AddDays(-30))
                .CountAsync();
            var blockedUsers = await _context.Users.Where(u => u.IsBlocked).CountAsync();
            var totalExpenses = await _context.Expenses.CountAsync();
            var totalIncomes = await _context.Incomes.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalBudgets = await _context.Budgets.CountAsync();

            var totalExpenseAmount = (double)await _context.Expenses.SumAsync(e => e.Amount);
            var totalIncomeAmount = (double)await _context.Incomes.SumAsync(i => i.Amount);

            return new
            {
                totalUsers,
                activeUsers,
                blockedUsers,
                transactionStats = new
                {
                    totalExpenses,
                    totalIncomes,
                    totalTransactions = totalExpenses + totalIncomes
                },
                amountStats = new
                {
                    totalExpenseAmount,
                    totalIncomeAmount,
                    balance = totalIncomeAmount - totalExpenseAmount
                },
                categories = totalCategories,
                budgets = totalBudgets,
                timestamp = DateTime.Now
            };
        }

        public async Task<object> GetMonthlySummaryAsync()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var newUsers = await _context.Users
                .Where(u => u.CreatedAt >= thirtyDaysAgo)
                .CountAsync();

            var newExpenses = (double)await _context.Expenses
                .Where(e => e.Date >= thirtyDaysAgo)
                .SumAsync(e => e.Amount);

            var newIncomes = (double)await _context.Incomes
                .Where(i => i.Date >= thirtyDaysAgo)
                .SumAsync(i => i.Amount);

            return new
            {
                period = "Last 30 days",
                newUsers,
                newExpenses,
                newIncomes,
                netFlow = newIncomes - newExpenses
            };
        }

        public async Task<IEnumerable<object>> GetTopExpenseCategoriesAsync(int limit = 10)
        {
            var raw = await _context.Expenses
                .Include(e => e.Category)
                .ToListAsync();

            var result = raw
                .GroupBy(e => new { e.CategoryId, Name = e.Category?.Name ?? "Без категорії" })
                .Select(g => new
                {
                    categoryId = g.Key.CategoryId,
                    category = g.Key.Name,
                    totalAmount = g.Sum(e => (double)e.Amount),
                    count = g.Count(),
                    averageAmount = g.Average(e => (double)e.Amount)
                })
                .OrderByDescending(x => x.totalAmount)
                .Take(limit)
                .ToList<object>();

            return result;
        }

        public async Task<IEnumerable<object>> GetUserStatisticsAsync()
        {
            var users = await _context.Users
                .Include(u => u.Expenses)
                .Include(u => u.Incomes)
                .ToListAsync();

            return users.Select(u => new
            {
                u.UserId,
                u.Email,
                u.Role,
                expenseCount = u.Expenses.Count,
                incomeCount = u.Incomes.Count,
                totalExpenses = (double)u.Expenses.Sum(e => e.Amount),
                totalIncomes = (double)u.Incomes.Sum(i => i.Amount),
                createdAt = u.CreatedAt,
                lastLoginAt = u.LastLoginAt
            }).ToList<object>();
        }
    }
}