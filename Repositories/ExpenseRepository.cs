using LabsTRVD.Data;
using LabsTRVD.Entities;
using LabsTRVD.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LabsTRVD.Repositories
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetByUserAsync(Guid userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }
    }
}