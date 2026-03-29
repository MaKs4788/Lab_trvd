namespace LabsTRVD.Entities
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public decimal MonthlyLimit { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}