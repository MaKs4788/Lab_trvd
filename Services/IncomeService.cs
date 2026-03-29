using AutoMapper;
using LabsTRVD.DTOs.ServicesDTOs;
using LabsTRVD.Entities;
using LabsTRVD.Interfaces.Services;

namespace LabsTRVD.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IRepository<Income> _incomeRepository;
        private readonly IMapper _mapper;

        public IncomeService(IRepository<Income> incomeRepository, IMapper mapper)
        {
            _incomeRepository = incomeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IncomeDtoResponse>> GetUserIncomesAsync(Guid currentUserId)
        {
            var incomes = await _incomeRepository.FindAsync(i => i.UserId == currentUserId);
            return _mapper.Map<IEnumerable<IncomeDtoResponse>>(incomes);
        }

        public async Task<IEnumerable<IncomeDtoResponse>> GetByPeriodAsync(Guid currentUserId, DateTime from, DateTime to)
        {
            var incomes = await _incomeRepository.FindAsync(i =>
                i.UserId == currentUserId &&
                i.Date >= from &&
                i.Date <= to);
            return _mapper.Map<IEnumerable<IncomeDtoResponse>>(incomes);
        }

        public async Task<IncomeDtoResponse?> GetByIdAsync(int id, Guid currentUserId)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income == null || income.UserId != currentUserId)
                return null;
            return _mapper.Map<IncomeDtoResponse>(income);
        }

        public async Task<IncomeDtoResponse> AddIncomeAsync(IncomeDto incomeDto, Guid currentUserId)
        {
            if (incomeDto.Amount <= 0)
                throw new Exception("Сума доходу повинна бути більше 0");

            var income = _mapper.Map<Income>(incomeDto);
            income.UserId = currentUserId;   // перезаписуємо
            if (income.Date == default)
                income.Date = DateTime.Now;
            if (income.CategoryId == 0)
                income.CategoryId = null;

            await _incomeRepository.AddAsync(income);
            return _mapper.Map<IncomeDtoResponse>(income);
        }

        public async Task<IncomeDtoResponse> UpdateIncomeAsync(int id, IncomeDto incomeDto, Guid currentUserId)
        {
            if (incomeDto.Amount <= 0)
                throw new Exception("Сума доходу повинна бути більше 0");

            var existing = await _incomeRepository.GetByIdAsync(id);
            if (existing == null || existing.UserId != currentUserId)
                throw new Exception("Дохід не знайдено або доступ заборонено");

            existing.Amount = incomeDto.Amount;
            existing.Date = incomeDto.Date;
            existing.Description = incomeDto.Description;
            existing.CategoryId = incomeDto.CategoryId == 0 ? null : incomeDto.CategoryId;

            await _incomeRepository.UpdateAsync(existing);
            return _mapper.Map<IncomeDtoResponse>(existing);
        }

        public async Task DeleteIncomeAsync(int id, Guid currentUserId)
        {
            var income = await _incomeRepository.GetByIdAsync(id);
            if (income == null || income.UserId != currentUserId)
                throw new Exception("Дохід не знайдено або доступ заборонено");

            await _incomeRepository.DeleteAsync(income);
        }

        public async Task<decimal> GetTotalForPeriodAsync(Guid currentUserId, DateTime from, DateTime to)
        {
            var incomes = await _incomeRepository.FindAsync(i =>
                i.UserId == currentUserId &&
                i.Date >= from &&
                i.Date <= to);
            return incomes.Sum(i => i.Amount);
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
            var incomes = await _incomeRepository.FindAsync(i => i.UserId == currentUserId);
            return incomes.Sum(i => i.Amount);
        }
    }
}