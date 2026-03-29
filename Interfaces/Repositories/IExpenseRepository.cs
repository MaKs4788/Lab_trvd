using LabsTRVD.Entities;

namespace LabsTRVD.Interfaces.Repositories
{
    public interface IExpenseRepository: IRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetByUserAsync(Guid userId);
    }
}
