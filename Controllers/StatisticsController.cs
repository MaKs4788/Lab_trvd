using LabsTRVD.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabsTRVD.Controllers
{
    [ApiController]
    [Route("api/admin/statistics")]
    [Authorize(Roles = "Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Отримати дашборд адміністратора</summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetDashboard()
        {
            try
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

                return Ok(new
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
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>Отримати статистику за останні 30 днів</summary>
        [HttpGet("monthly-summary")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMonthlySummary()
        {
            try
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

                return Ok(new
                {
                    period = "Last 30 days",
                    newUsers,
                    newExpenses,
                    newIncomes,
                    netFlow = newIncomes - newExpenses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>Отримати топ категорій за витратами</summary>
        [HttpGet("top-expense-categories")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTopExpenseCategories([FromQuery] int limit = 10)
        {
            try
            {
                var expenses = await _context.Expenses
                    .Include(e => e.Category)
                    .ToListAsync();

                var topCategories = expenses
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
                    .ToList();

                return Ok(topCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>Отримати статистику користувачів</summary>
        [HttpGet("user-statistics")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetUserStatistics()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Expenses)
                    .Include(u => u.Incomes)
                    .ToListAsync();

                var stats = users.Select(u => new
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
                }).ToList();

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>Отримати експорт звіту (JSON)</summary>
        [HttpGet("export-report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ExportReport()
        {
            try
            {
                var expenses = await _context.Expenses
                    .Include(e => e.Category)
                    .ToListAsync();

                var topCategories = expenses
                    .GroupBy(e => e.Category?.Name ?? "Без категорії")
                    .Select(g => new
                    {
                        category = g.Key,
                        total = (double)g.Sum(e => e.Amount),
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.total)
                    .ToList();

                var report = new
                {
                    generatedAt = DateTime.Now,
                    summary = await GetDashboardData(),
                    users = await _context.Users
                        .Select(u => new { u.UserId, u.Email, u.Role, u.CreatedAt })
                        .ToListAsync(),
                    topCategories
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private async Task<object> GetDashboardData()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalExpenses = await _context.Expenses.CountAsync();
            var totalIncomes = await _context.Incomes.CountAsync();

            return new
            {
                totalUsers,
                totalTransactions = totalExpenses + totalIncomes,
                totalExpenseAmount = (double)await _context.Expenses.SumAsync(e => e.Amount),
                totalIncomeAmount = (double)await _context.Incomes.SumAsync(i => i.Amount)
            };
        }
    }
}