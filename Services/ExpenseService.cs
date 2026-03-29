using AutoMapper;
using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Entities;
using LabsTRVD.Interfaces.Repositories;
using LabsTRVD.Interfaces.Services;

namespace LabsTRVD.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExpenseDtoResponse>> GetUserExpensesAsync(Guid currentUserId)
        {
            var expenses = await _expenseRepository.GetByUserAsync(currentUserId);
            return _mapper.Map<IEnumerable<ExpenseDtoResponse>>(expenses);
        }

        public async Task<IEnumerable<ExpenseDtoResponse>> GetByPeriodAsync(Guid currentUserId, DateTime from, DateTime to)
        {
            var expenses = await _expenseRepository.FindAsync(e =>
                e.UserId == currentUserId &&
                e.Date >= from &&
                e.Date <= to);
            return _mapper.Map<IEnumerable<ExpenseDtoResponse>>(expenses);
        }

        public async Task<ExpenseDtoResponse?> GetByIdAsync(int id, Guid currentUserId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null || expense.UserId != currentUserId)
                return null; // або викинути NotFound
            return _mapper.Map<ExpenseDtoResponse>(expense);
        }

        public async Task<ExpenseDtoResponse> AddExpenseAsync(ExpenseDto expenseDto, Guid currentUserId)
        {
            if (expenseDto.Amount <= 0)
                throw new Exception("Сума повинна бути більше 0");

            // Ігноруємо можливий UserId з DTO – використовуємо currentUserId
            var expense = _mapper.Map<Expense>(expenseDto);
            expense.UserId = currentUserId;
            if (expense.Date == default)
                expense.Date = DateTime.Now;
            if (expense.CategoryId == 0)
                expense.CategoryId = null;

            await _expenseRepository.AddAsync(expense);
            return _mapper.Map<ExpenseDtoResponse>(expense);
        }

        public async Task<ExpenseDtoResponse> UpdateExpenseAsync(int id, ExpenseDto expenseDto, Guid currentUserId)
        {
            if (expenseDto.Amount <= 0)
                throw new Exception("Сума повинна бути більше 0");

            var existing = await _expenseRepository.GetByIdAsync(id);
            if (existing == null || existing.UserId != currentUserId)
                throw new Exception("Витрата не знайдена або доступ заборонено");

            existing.Amount = expenseDto.Amount;
            existing.Date = expenseDto.Date;
            existing.Description = expenseDto.Description;
            existing.CategoryId = expenseDto.CategoryId == 0 ? null : expenseDto.CategoryId;

            await _expenseRepository.UpdateAsync(existing);
            return _mapper.Map<ExpenseDtoResponse>(existing);
        }

        public async Task DeleteExpenseAsync(int id, Guid currentUserId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null || expense.UserId != currentUserId)
                throw new Exception("Витрата не знайдена або доступ заборонено");

            await _expenseRepository.DeleteAsync(expense);
        }

        public async Task<decimal> GetTotalForPeriodAsync(Guid currentUserId, DateTime from, DateTime to)
        {
            var expenses = await _expenseRepository.FindAsync(e =>
                e.UserId == currentUserId &&
                e.Date >= from &&
                e.Date <= to);
            return expenses.Sum(e => e.Amount);
        }

        public async Task<decimal> GetTotalCurrentMonthAsync(Guid currentUserId)
        {
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1);
            var to = from.AddMonths(1).AddTicks(-1);

            return await GetTotalForPeriodAsync(currentUserId, from, to);
        }

        public async Task<decimal> GetTotalAsync(Guid currentUserId)
        {
            var expenses = await _expenseRepository.GetByUserAsync(currentUserId);
            return expenses.Sum(e => e.Amount);
        }
    }
}